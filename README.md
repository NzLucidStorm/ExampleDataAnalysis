# ExampleDataAnalysis #

A new decease called [COVID-19] appeared in December 2019 and is headlining news all 
around the world. There is much we still have to learn about it, but fear travels fast 
these days... thanks to Twitter, Facebook and social media. 

So what can we do about it? Maybe get some official data first.

The [John Hopkins University] operates a dashboard for tracking the [COVID-19] spread and 
shares the data in a Github repository:

> This is the data repository for the 2019 Novel Coronavirus Visual Dashboard operated by 
> the Johns Hopkins University Center for Systems Science and Engineering (JHU CCSE). Also, 
> Supported by ESRI Living Atlas Team and the Johns Hopkins University Applied Physics Lab 
> (JHU APL).

The John Hopkins Github repository with the data is available at:

* [https://github.com/CSSEGISandData/COVID-19](https://github.com/CSSEGISandData/COVID-19)

I thought a first step is to show how to parse the data by the John Hopkins University and 
get it into a SQL database. From there we can process the data using tools like SQL, Microsoft 
Excel, R and so on.

The article for this repository can be found at:

* [https://bytefish.de/blog/parsing_covid_19_data/](https://bytefish.de/blog/parsing_covid_19_data/)

### Creating the SQL Server Database ###

You create the SQL Server database by running the Batch Script located at:

* [ExampleDataAnalysis/ExampleDataAnalysis/SqlServer/Sql/create_database.bat](https://github.com/bytefish/ExampleDataAnalysis/blob/master/ExampleDataAnalysis/ExampleDataAnalysis/SqlServer/Sql/create_database.bat)

## License ##

The code is released under terms of the [MIT License].

[MIT License]: https://opensource.org/licenses/MIT
[John Hopkins University]: [https://systems.jhu.edu/]