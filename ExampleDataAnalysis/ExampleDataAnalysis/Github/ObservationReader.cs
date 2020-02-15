// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ExampleDataAnalysis.Github.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TinyCsvParser.Tokenizer.RFC4180;

namespace ExampleDataAnalysis.Github
{
    public class GithubObservationReader
    {
        private readonly HttpClient httpClient;

        public GithubObservationReader()
            : this(new HttpClient()) { }

        public GithubObservationReader(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public IEnumerable<Observation> GetObservations()
        {
            var confirmed = GetConfirmedCasesFromGithubAsync().Result
                .Split(new[] { '\n' }, StringSplitOptions.None)
                .ToList();

            var deaths = GetDeathCasesFromGithubAsync().Result
                .Split(new[] { '\n' }, StringSplitOptions.None)
                .ToList();

            var recovered = GetRecoveredCasesFromGithubAsync().Result
                .Split(new[] { '\n' }, StringSplitOptions.None)
                .ToList();

            // Make sure all data has the same header, so the Timestamps match:
            if(!new[] { deaths[0], recovered[0] }.All(x => string.Equals(x, confirmed[0], StringComparison.InvariantCulture)))
            {
                throw new Exception($"Different Headers (Confirmed = {confirmed[0]}, Deaths = {deaths[0]}, Recovered = {recovered[0]}");
            }

            // Make sure all data has the same number of rows, or we can stop here:
            if(!new[] { deaths.Count, recovered.Count}.All(x => x == confirmed.Count))
            {
                throw new Exception($"Different Number of Rows (Confirmed = {confirmed.Count}, Deaths = {deaths.Count}, Recovered = {recovered.Count}");
            }

            var tokenizer = new RFC4180Tokenizer(new Options('"', '\\', ','));

            // Get Header Row:
            var header = tokenizer.Tokenize(confirmed[0])
                .ToArray();

            // Get the TimeStamps:
            var observationDateTimes = header
                .Skip(4)
                .Select(x => DateTime.Parse(x, CultureInfo.InvariantCulture))
                .ToArray();

            // Now create a Lookup on the Raw Datas Province and Country:
            var confirmedLookup = confirmed.Skip(1)
                .Select(x => tokenizer.Tokenize(x))
                .Where(x => x.Length == header.Length)
                .ToDictionary(x => $"{x[0]},{x[1]}", x => x);

            var deathsLookup = deaths.Skip(1)
                .Select(x => tokenizer.Tokenize(x))
                .Where(x => x.Length == header.Length)
                .ToDictionary(x => $"{x[0]},{x[1]}", x => x);

            var recoveredLookup = recovered.Skip(1)
                .Select(x => tokenizer.Tokenize(x))
                .Where(x => x.Length == header.Length)
                .ToDictionary(x => $"{x[0]},{x[1]}", x => x);

            // Get all keys we want to iterate over:
            var keys = confirmedLookup.Keys
                .Concat(deathsLookup.Keys)
                .Concat(recoveredLookup.Keys)
                .Distinct()
                .ToList();

            foreach(var key in keys)
            {
                var confirmedValues = confirmedLookup[key];
                var deathValues = deathsLookup[key];
                var recoveredValues = recoveredLookup[key];

                for(int timeStepIdx = 0; timeStepIdx < observationDateTimes.Length; timeStepIdx++)
                {
                    yield return new Observation
                    {
                        Province = confirmedLookup[key][0],
                        Country = confirmedLookup[key][1],
                        Lat = double.Parse(confirmedLookup[key][2].Trim(), CultureInfo.InvariantCulture),
                        Lon = double.Parse(confirmedLookup[key][3].Trim(), CultureInfo.InvariantCulture),
                        Timestamp = observationDateTimes[timeStepIdx],
                        Confirmed = GetCountSafe(confirmedValues[timeStepIdx + 4]),
                        Deaths = GetCountSafe(deathValues[timeStepIdx + 4]),
                        Recovered = GetCountSafe(recoveredValues[timeStepIdx + 4])
                    };
                }
            }
        }
        
        private static int GetCountSafe(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }
            
            return (int) double.Parse(value, CultureInfo.InvariantCulture);
        }

        public async Task<string> GetConfirmedCasesFromGithubAsync()
        {
            return await httpClient
                .GetStringAsync("https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_19-covid-Confirmed.csv")
                .ConfigureAwait(false);
        }

        public async Task<string> GetDeathCasesFromGithubAsync()
        {
            return await httpClient
                .GetStringAsync("https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_19-covid-Deaths.csv")
                .ConfigureAwait(false);
        }

        public async Task<string> GetRecoveredCasesFromGithubAsync()
        {
            return await httpClient
                .GetStringAsync("https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_19-covid-Recovered.csv")
                .ConfigureAwait(false);
        }
    }
}
