// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ExampleDataAnalysis.Elasticsearch;
using ExampleDataAnalysis.SqlServer.Context;
using ExampleDataAnalysis.SqlServer.Processors;
using Microsoft.EntityFrameworkCore;

namespace ExampleDataAnalysis
{
    class Program
    {
        private static string SqlServerConnectionString = @"Server=.\MSSQLSERVER2017;Database=SampleDatabase;Integrated Security=True";

        static void Main(string[] args)
        {
            UpdateAllDatabases();

            Console.WriteLine("Press [Enter] to exit ...");
            Console.Read();
        }

        private static void UpdateAllDatabases()
        {
            Console.WriteLine("Getting latest Data from Github ...");

            var source = GetGithubData();

            Console.WriteLine("Writing to SQL Server ...");

            // Then update the data in SQL Server:
            WriteToSqlServer(source);

            // Indexes the Data to Elasticsearch for Dashboards:
            Console.WriteLine("Do you want to write the data to Elasticsearch? (Y/N)");
            
            if(string.Equals(Console.ReadLine(), "Y", StringComparison.InvariantCultureIgnoreCase))
            {
                UpdateElasticsearchIndex();
            }
        }

        private static IEnumerable<Github.Models.Observation> GetGithubData()
        {

            return new Github.GithubObservationReader()
                .GetObservations()
                .ToList();
        }

        private static void WriteToSqlServer(IEnumerable<Github.Models.Observation> source)
        {
            // Convert the data into the SQL representation:
            var target = GetSqlServerData(source);

            // This kicks off a Stored Procedure using a MERGE to either insert or 
            // update an existing entries in a bulk fashion: 
            var processor = new SqlServerBulkProcessor(SqlServerConnectionString);

            processor.Write(target);
        }

        private static IEnumerable<SqlServer.Models.Observation> GetSqlServerData(IEnumerable<Github.Models.Observation> observations)
        {
            foreach (var observation in observations)
            {
                yield return new SqlServer.Models.Observation
                {
                    Province = observation.Province,
                    Country = observation.Country,
                    Timestamp = observation.Timestamp,
                    Confirmed = observation.Confirmed,
                    Deaths = observation.Deaths,
                    Recovered = observation.Recovered,
                    Lat = observation.Lat,
                    Lon = observation.Lon
                };
            }
        }

        private static void UpdateElasticsearchIndex()
        {
            var client = new ElasticSearchObservationClient(new Uri("http://localhost:9200"), "observations");

            using (var context = new ApplicationDbContext(SqlServerConnectionString))
            {
                var documents = context.Observations
                    // Do not track the Entities in the DbContext:
                    .AsNoTracking()
                    // Turn all Entities into the Elasticsearch Representation:
                    .Select(x => new Elasticsearch.Models.Observation
                    {
                        ObservationId = x.ObservationId,
                        Confirmed = x.Confirmed,
                        Country = x.Country,
                        Deaths = x.Deaths,
                        Timestamp = x.Timestamp,
                        Location = new Nest.GeoLocation(x.Lat, x.Lon),
                        Province = x.Province,
                        Recovered = x.Recovered
                    })
                    .AsEnumerable();

                var response = client.BulkIndex(documents);
            }
        }
    }
}
