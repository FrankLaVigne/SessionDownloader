# SessionDownloader
Starting in 2019, Build used a differnt format/web site for storing slides and video presentations. This made the venerable version (https://github.com/FrankLaVigne/SessionDownloaderLegacy) written for PDC 09 and subsequent events outdated. 

## Updates

### Updates: May 2020

* Updated for Build 2020
* Parameterization added
* Caption support added
* started code and project reogranization

## No Frills
This code is no frills and, since it does what I need, I'm probably not going to modify it anytime soon. If you're impatient, feel free to modify, and submit a PR.


## Download from Ignite, Build, and More!

### Parameters
The program now supports parameters

#### File Download Path
File path to a directory to download files to

#### Feed URL
Location of JSON file for session data

#### Content Type

One letter code based on the table below.

| Code | Name     | Description           | 
|------|----------|-----------------------|
| A    | All      | Get all related files |
| V    | Video    | Get session video     |
| S    | Slides   | Get session slides    |
| C    | Captions | Get session captions  |
| N    | None     | Just show metadata    |



#### Examples

To get all the slides from Ignite 2019

`d:\I19\ https://api-myignite.techcommunity.microsoft.com/api/session/all s`

To get all the videos from Build 2020

`d:\b20\ https://api.mybuild.microsoft.com/api/session/all v`

To get all the captions from Build 2020

`d:\b20\ https://api.mybuild.microsoft.com/api/session/all c`


## History
It began humbly as a WPF app to capture all the sessions from PDC 2009. 

2009 was a simpler time, mobile broadband was expensive and not widely available and I wanted a way to watch PDC videos at home on my TV (sad, right?) or on the go on my laptop. The answer was to grab all the content using the RSS/ATOM feed Channel9 made available.

In 2015, I updated it to download the sessions from Build 2015. In 2016, I added parameters to make it grab all items on Channel9 with an Event RSS feed. In 2018, I encountered some issues: first with the RSS feed then with some other oddities stemming from the previous approach, which had more or less in place since 2009. 

Starting in 2019, Build used a different system.

## Future
I'd like to add 

* Unit tests
* more robust



### Flexible architecture
It would be nice to modularize the code so that adding a new event would be as easy as creating a JSON file
