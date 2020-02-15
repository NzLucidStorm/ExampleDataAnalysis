// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ExampleDataAnalysis.SqlServer.Models;
using Microsoft.EntityFrameworkCore;

namespace ExampleDataAnalysis.SqlServer.Context
{
    public class ApplicationDbContext : DbContext
    {
        private readonly string connectionString;

        public DbSet<Observation> Observations { get; set; }

        public ApplicationDbContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
