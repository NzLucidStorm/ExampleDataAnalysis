// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Elasticsearch.Net;
using ExampleDataAnalysis.Elasticsearch.Models;
using Nest;
using System;
using System.Collections.Generic;

namespace ExampleDataAnalysis.Elasticsearch
{
    public class ElasticSearchObservationClient
    {
        public readonly string IndexName;

        protected readonly IElasticClient Client;

        public ElasticSearchObservationClient(IElasticClient client, string indexName)
        {
            IndexName = indexName;
            Client = client;
        }

        public ElasticSearchObservationClient(Uri uri, string indexName)
            : this(CreateClient(uri), indexName)
        {
        }

        public CreateIndexResponse CreateIndex()
        {
            var response = Client.Indices.Exists(IndexName);
            
            if (response.Exists)
            {
                return null;
            }

            return Client.Indices.Create(IndexName, index => index.Map<Observation>(ms => ms.AutoMap()));
        }

        public BulkResponse BulkIndex(IEnumerable<Observation> observations)
        {
            var request = new BulkDescriptor();

            foreach (var observation in observations)
            {
                request.Index<Observation>(op => op
                .Id(observation.ObservationId)
                .Index(IndexName)
                .Document(observation));
            }

            return Client.Bulk(request);
        }

        private static IElasticClient CreateClient(Uri uri)
        {
            var connectionPool = new SingleNodeConnectionPool(uri);
            var connectionSettings = new ConnectionSettings(connectionPool);

            return new ElasticClient(connectionSettings);
        }
    }
}
