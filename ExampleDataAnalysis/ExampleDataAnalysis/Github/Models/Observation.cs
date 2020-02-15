// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace ExampleDataAnalysis.Github.Models
{
    public class Observation
    {
        public string Province { get; set; }

        public string Country { get; set; }

        public double Lat { get; set; }

        public double Lon { get; set; }

        public DateTime Timestamp { get; set; }

        public int Confirmed { get; set; }

        public int Deaths { get; set; }

        public int Recovered { get; set; }

    }
}
