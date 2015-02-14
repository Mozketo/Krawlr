# Krawlr
C# &amp; Selenium web page crawler. It's easy to use and designed to be run from the commandline.

## Usage

After cloning the source and building let's leverage the command-line. 

    // The simplest way to start the crawl
    $ Krawlr.exe --url http://global.clsnightly.test.janison.com
    
    // While crawling it's possible to ignore some routes by supplying a simple list of keywords to ignore
    $ Krawlr.exe --url http://global.clsnightly.test.janison.com --exclusions ExcludeUrls.txt
    
    // Krawlr can follow a preset list of routes to follow
    $ Krawlr.exe --url http://global.clsnightly.test.janison.com --inclusions Routes.txt --follow-links=false
    
    // Need to login to a page? It's possible to manipulate the DOM with Page Actions (more below)
    $ Krawlr.exe --url http://global.clsnightly.test.janison.com --scripts Path\to\scripts

## Arguments

    -u, --url Required. URL to start crawling.
    -f, --follow-links If true after the page is ready it will be checked for all a href links and be added to the queue of pages to load. (Default: true)
    --max-follow-links Limit the number of pages to crawl. Default: 0 (no limit)
    -e, --exclusions Path to a file with list of routes/keywords in URL to bypass.
    -i, --inclusions Path to a file with a hard list of routes to hit (will follow in order). Use with --follow=false
    -s, --scripts After each page is loaded a script may be executed against the page to manipulate the DOM. Recommended for adding Login support to the crawl.
    -w, --webdriver Define WebDriver to use. Firefox, Chrome, Remote (Default: Chrome)
    --webdriver-proxy Using Chrome or Remote should route via Fiddler Core? (Default: true)
    --webdriver-proxy-port If WebDriver proxy is engaged define the port to use. (Default: 0 (autoselect))
    -h, --help Display commandline argument help page.
    
## Selenium scripts (aka Page Actions)

Page Actions are blocks of javascript code that will executed upon DOM ready of each Selenium Web Driver navigation. 
Either place your files along side the Krawlr.exe or you can supply a path to scripts using the `--scripts` commandline argument

Note: scripts need to be named `PageAction-*.js` to be parsed and executed.

Take a look at a *sample* or two in the source [repo](src/Krawlr.Console/PageAction-Login.js).

## Reporting

TODO

## Inclusions / Exclusions

Sample inclusion file

```
http://somesite.com
http://somesite.com/login
http://somesite.com/list/something
```

Sample exclusion file

```
/logout
delete
```
