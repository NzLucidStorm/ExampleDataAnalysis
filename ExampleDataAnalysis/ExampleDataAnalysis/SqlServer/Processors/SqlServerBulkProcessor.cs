// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ExampleDataAnalysis.SqlServer.Models;
using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ExampleDataAnalysis.SqlServer.Processors
{
    public class SqlServerBulkProcessor
    {
        private readonly string connectionString;

        public SqlServerBulkProcessor(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void Write(IEnumerable<Observation> observations)
        {
            if (observations == null)
            {
                return;
            }

            // There may be duplicates, avoid duplicates in Batch:
            var groupedObservations = observations
                .GroupBy(x => new { x.Province, x.Country, x.Timestamp })
                .Select(x => x.First());

            using (var conn = new SqlConnection(connectionString))
            {
                // Open the Connection:
                conn.Open();

                // Execute the Batch Write Command:
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Build the Stored Procedure Command:
                    cmd.CommandText = "[sample].[InsertOrUpdateObservation]";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Create the TVP:
                    SqlParameter parameter = new SqlParameter();

                    parameter.ParameterName = "@Entities";
                    parameter.SqlDbType = SqlDbType.Structured;
                    parameter.TypeName = "[sample].[ObservationType]";
                    parameter.Value = ToSqlDataRecords(groupedObservations);

                    // Add it as a Parameter:
                    cmd.Parameters.Add(parameter);

                    // And execute it:
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private IEnumerable<SqlDataRecord> ToSqlDataRecords(IEnumerable<Observation> observations)
        {
            // Construct the Data Record with the MetaData:
            SqlDataRecord sdr = new SqlDataRecord(
                new SqlMetaData("Province", SqlDbType.NVarChar, 100),
                new SqlMetaData("Country", SqlDbType.NVarChar, 100),
                new SqlMetaData("Timestamp", SqlDbType.DateTime2),
                new SqlMetaData("Confirmed", SqlDbType.Int),
                new SqlMetaData("Deaths", SqlDbType.Int),
                new SqlMetaData("Recovered", SqlDbType.Int),
                new SqlMetaData("Lat", SqlDbType.Real),
                new SqlMetaData("Lon", SqlDbType.Real)
            );

            // Now yield the Measurements in the Data Record:
            foreach (var observation in observations)
            {
                sdr.SetString(0, observation.Province);
                sdr.SetString(1, observation.Country);
                sdr.SetDateTime(2, observation.Timestamp);
                sdr.SetInt32(3, observation.Confirmed);
                sdr.SetInt32(4, observation.Deaths);
                sdr.SetInt32(5, observation.Recovered);
                sdr.SetFloat(6, (float) observation.Lat);
                sdr.SetFloat(7, (float) observation.Lon);

                yield return sdr;
            }
        }
    }
}
