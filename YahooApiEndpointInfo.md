## The problem
If I, as a developer, want historical information from Yahoo Finance, I have to do some light hacking to get API endpoints. Luckily for us, Chrome's dev tools are very handy. Using the network tab, I was able to search by XHR requests and poke through each one until I found an endpoint that had the information I desired. I have made this file, as to remember how to access the information I wanted at a later date.

```
https://query2.finance.yahoo.com/v8/finance/chart/AAPL?formatted=true&crumb=AuzYOzjmUdE&lang=en-US&region=US&period1=511077600&period2=1553662800&interval=1d&events=div%7Csplit&corsDomain=finance.yahoo.com
```
Above is an example `GET` endpoint searching for AAPL's historical data. If you look closely at the URL parameters, you will see a `period1` & `period2` with large numerical values. These are the timestamp ranges to extract information from. These timestamps are UNIX encoded from what I could tell, meaning they are counting the seconds from 1/1/1970 instead of the milliseconds from 1970. To easily convert between the two use one of the following two formulas

`timeInMilliseconds / 1000 = timeInSeconds`
or
`timeInSeconds * 1000 = timeInMilliseconds`

These formula's will also help address timestamp issues when accessing historical data and tying it to a date.
