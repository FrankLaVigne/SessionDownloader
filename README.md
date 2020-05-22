# SessionDownloader
Starting in 2019, Build used a differnt format/web site for storing slides and video presentations. This made the venerable version (https://github.com/FrankLaVigne/SessionDownloaderLegacy) written for PDC 09 and subsequent events outdated. 

## Update: May 2020
I've updated the repo for Build 2020.

## No Frills
This code is no frills and, since it does what I need, I'm probably not going to modify it anytime soon. If you're impatient, feel free to modify, and submit a PR.

## History
It began humbly as a WPF app to capture all the sessions from PDC 2009. 

2009 was a simpler time, mobile broadband was expensive and not widely available and I wanted a way to watch PDC videos at home on my TV (sad, right?) or on the go on my laptop. The answer was to grab all the content using the RSS/ATOM feed Channel9 made available.

In 2015, I updated it to download the sessions from Build 2015. In 2016, I added parameters to make it grab all items on Channel9 with an Event RSS feed. In 2018, I encountered some issues: first with the RSS feed then with some other oddities stemming from the previous approach, which had more or less in place since 2009. 

Starting in 2019, Build used a different system.

## Future
I'd like to add parameters, IoC, and some other goodies to make this a tad more robust.

Ok, a lot more robust.
