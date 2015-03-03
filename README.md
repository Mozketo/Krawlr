# Krawlr
C# &amp; Selenium web page crawler. It's easy to use and designed to be run from the commandline.

## Download

Soon.

## Build

After downloading the source you run `build.cmd` navigate to `\Krawlr.Console\Bin\Release` and _boom_ there's Krawlr.

## Usage

After cloning the source and building let's leverage the command-line. 

    // The simplest way to start the crawl
    $ Krawlr.exe --url http://simple-crawl.com/
    
    // While crawling it's possible to ignore some routes by supplying a simple list of keywords to ignore
    $ Krawlr.exe --url http://my-awesome-site.com/ --exclude ExcludeUrls.txt
    
    // Krawlr can follow a preset list of routes to follow
    $ Krawlr.exe --url http://my-awesome-site.com/ --include Routes.txt --follow-links no
    
    // Need to login to a page? It's possible to manipulate the DOM with Page Actions (more below)
    $ Krawlr.exe --url http://crawl.me/ --scripts Path\to\scripts
    
    // Use ChromeDriver?
    $ Krawlr.exe --url http://so-awesome.com/ --webdriver=Chrome
    
    // Persist results in Database (SQL Server) and persist some metadata against the _CrawlRun_ table
    --url http://my-awesome-site.com/ --output "Data Source=<server>;Initial Catalog=<datbase-name>;User ID=<user>;Password=<password>" --metadata "production v1.0"

    // Experimental - disconnected client/server
    // RabbitMQ server needs to be installed and running (Krawlr will not autostart RabbitMQ server).
    $ Krawlr.exe --url http://my-awesome-site.com/ --server
    $ Krawlr.exe --url http://my-awesome-site.com/ --client

## Arguments

    -u, --url=VALUE        Starting URL to begin crawling.
    -q, --quiet            Run quietly with less detailed console logging
    --no-follow-links      After loading a page don't find and follow links on the page
    --ignore-guids         When analysing URLs remove guids as this removes repeat crawling like /items/item/guid. Value is yes / no (Default: yes) 
    --max-follow-links=VALUE Limit the number of pages to crawl. Default: 0 (no limit).
    -e, --exclude=VALUE    Path to a file with list of routes/keywords in URL to bypass.
    -i, --include=VALUE    Path to a file with a hard list of routes to hit (will follow in order). Use with --no-follow-links false.
    -s, --scripts=VALUE    After each page is loaded a script may be executed against the page to manipulate the DOM. Recommended for adding Login support to the crawl.
    -o, --output=VALUE     Write crawling activity to CSV file with path or write to a SQL Server DB with a connection string.
    --metadata=VALUE       When using the DB writer the metadata will be written to the CrawlRun entity.
    -w, --webdriver=VALUE  Define WebDriver to use. Firefox, Chrome, Remote (Default: Firefox)
    --webdriver-proxy      Using Chrome or Remote should route via FiddlerCore?
    --webdriver-proxy-port If WebDriver proxy is engaged define the port to use. (Default: 0 (autoselect))
    --mode=VALUE           Disibution mode use to use: clientserver, server, client (if server & client a running RabbitMQ server is required)
    -h, -?, --help             Show this message and exit.

## Selenium scripts (aka Page Actions)

Page Actions are blocks of javascript code that will executed upon DOM ready of each Selenium Web Driver navigation. 
Either place your files along side the Krawlr.exe or you can supply a path to scripts using the `--scripts` commandline argument

Note: scripts need to be named with a javascript extension `*.js` to be parsed and executed.

Take a look at a *sample* or two in the source [repo](src/Krawlr.Console/PageAction-Login.js).

## Reporting

### Exporting to Database (SQL Server)

You can leverage the `--output` commandline argument with a database connection string (current we only support SQL Server). The crawl history will be stored in a database as opposed to a CSV (as configured below). Database persistance offers a little more information (see `--metadata` argument) and makes it easier to spot trends or changes in the data over time.

Interested in using a DB connection instead of CSV? Then you'll need to perform the following:

1. Find a SQL Server instance locally or somewhere on a server. Maybe you're lucky and you don't need to configure it yourself,
2. Run [this](scripts/Krawlr-SqlServer-CreateTablesAndKeys.sql) script to add the required tables to the database,
3. Configure your commandline arguments to use this database. See examples above in _Usage_.

As the crawl begins a record will be stored in the _CrawlRun_ table along with the base domain used, time started, and metadata passed on the commandline.

As the crawl progresses the results are stored in the _CrawlResults_ table and are linked back to the _CrawlRun_ table via the _CrawlRunId_ field. This makes it easy to find the crawl results for a particular run.

### Exporting to CSV

By enabling the `--output` commandline argument a crawl history will be written out with the following information:

* URL,
* Page response code (eg 200, 404, 500),
* If Javascript errors were present on the page,
* Time taken for the page to load and DOM ready (in milliseconds).

Krawlr will output to CSV file as this is easy to use in spreadsheet application and easy to parse if you wish to extra the data into your own persistent store.

## Inclusions / Exclusions

Sample inclusion file

```
http://somesite.com
` This is a comment (backtick)
http://somesite.com/login
http://somesite.com/list/something
```

Sample exclusion file

```
` This is a comment (backtick).
` We can filter by a route like this,
/auth/logout
` Or just exclude URLs based on a keyword.
delete
```
