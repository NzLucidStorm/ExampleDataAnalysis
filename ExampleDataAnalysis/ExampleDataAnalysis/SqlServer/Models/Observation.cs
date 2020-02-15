// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExampleDataAnalysis.SqlServer.Models
{
    [Table("Observation", Schema = "sample")]
    public class Observation
    {
        [Column("ObservationID")]
        public int ObservationId { get; set; }

        [Column("Province")]
        public string Province { get; set; }

        [Column("Country")]
        public string Country { get; set; }

        [Column("Timestamp")]
        public DateTime Timestamp { get; set; }

        [Column("Confirmed")]
        public int Confirmed { get; set; }

        [Column("Deaths")]
        public int Deaths { get; set; }

        [Column("Recovered")]
        public int Recovered { get; set; }

        [Column("Lat")]
        public double Lat { get; set; }

        [Column("Lon")]
        public double Lon { get; set; }
    }
}
