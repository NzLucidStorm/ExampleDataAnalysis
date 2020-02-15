// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nest;
using System;

namespace ExampleDataAnalysis.Elasticsearch.Models
{
    public class Observation
    {
        [Number(NumberType.Long)]
        public long ObservationId { get; set; }

        [Text]
        public string Province { get; set; }

        [Text]
        public string Country { get; set; }

        [Date]
        public DateTime Timestamp { get; set; }

        [Number(NumberType.Integer)]
        public int Confirmed { get; set; }

        [Number(NumberType.Integer)]
        public int Deaths { get; set; }

        [Number(NumberType.Integer)]
        public int Recovered { get; set; }

        [GeoPoint]
        public GeoLocation Location { get; set; }
    }
}
