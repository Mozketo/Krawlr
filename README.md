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

    -u, --url URL to crawl.
    -e, --exclusions Path to a file with list of routes/keywords in URL to bypass.
    -i, --inclusions Path to a file with a hard list of routes to hit (will follow in order). Use with --follow=false
    -f, --follow-links After loading a page should links on the page be followed?
    -s, --scripts As a page DOM is ready look for PageAction-*.js Selenium scripts to execute.
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
