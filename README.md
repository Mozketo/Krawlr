# Krawlr
C# &amp; Selenium web page crawler. It's easy to use and designed to be run from the commandline.

## Usage

After cloning the source and building let's leverage the command-line. 

    // The simplest way to start the crawl
    $ Krawlr.exe --url http://global.clsnightly.test.janison.com
    
    // While crawling it's possible to ignore some routes by supplying a simple list of keywords to ignore
    $ Krawlr.exe --url http://global.clsnightly.test.janison.com --exclude ExcludeUrls.txt
    
    // Krawlr can follow a preset list of routes to follow
    $ Krawlr.exe --url http://global.clsnightly.test.janison.com --include Routes.txt --follow-links no
    
    // Need to login to a page? It's possible to manipulate the DOM with Page Actions (more below)
    $ Krawlr.exe --url http://global.clsnightly.test.janison.com --scripts Path\to\scripts
    
    // Use ChromeDriver?
    $ Krawlr.exe --url http://global.clsnightly.test.janison.com --webdriver=Chrome
    
    // Experimental - disconnected client/server
    // RabbitMQ server needs to be installed and running (Krawlr will not autostart RabbitMQ server).
    $ Krawlr.exe --url http://global.clsnightly.test.janison.com --server
    $ Krawlr.exe --url http://global.clsnightly.test.janison.com --client

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

By enabling the `--output` commandline argument a crawl history will be written out with the following headers:

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
/logout
` This is a comment (backtick)
delete
```
