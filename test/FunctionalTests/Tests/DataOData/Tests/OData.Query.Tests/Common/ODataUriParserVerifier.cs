//---------------------------------------------------------------------
// <copyright file="ODataUriParserVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.Common
{
    using System;
    using Microsoft.OData.Edm;
    using Microsoft.OData.UriParser;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Astoria.Client;

    [ImplementationName(typeof(IQueryVerifier), "UriParser")]
    internal class ODataUriParserVerifier : IQueryVerifier
    {
        /// <summary>
        /// Converter from an entity model to schema to an EDM model.
        /// </summary>
        [InjectDependency]
        public ODataQueryEntityModelSchemaToEdmModelConverter EntityModelSchemaToEdmModelConverter { get; set; }

        /// <summary>
        /// Gets or sets the provider factory for creating an IDSQP from an EntityModelSchema
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDataServiceProviderFactory DataServiceProviderFactory { get; set; }

        /// <summary>
        /// Gets or sets the uri query visitor.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryToUriStringConverter UriQueryVisitor { get; set; }

        /// <summary>
        /// Gets or sets the model generator.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ODataTestWorkspace Workspace { get; set; }

        /// <summary>
        /// Gets or sets the test case's query evaluator.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryExpressionEvaluator Evaluator { get; set; }

        /// <summary>
        /// Gets and sets the comparer.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ClientQueryResultComparer Comparer { get; set; }

        /// <summary>
        /// Gets or sets the logger used to print diagnostic messages.
        /// </summary>
        /// <value>The logger.</value>
        [InjectDependency]
        public Logger Logger { get; set; }


        /// <summary>
        /// Runs the given query against the OdataLib's uri translator and compares the results against an inmemorycontext
        /// </summary>
        /// <param name="expression">The query expression to run against the Uri parser</param>
        public void Verify(QueryExpression expression)
        {

            // 1. test : take the expression and construct the OData URI
            // /Customers?$top=1
            string queryFragment = null;
            queryFragment = this.UriQueryVisitor.ComputeUri(expression);

            Uri serviceBaseUri = new Uri("http://localhost:9000/");

            // http://localhost:9000/Customers?$top=1
            Uri queryUri = new Uri(serviceBaseUri, queryFragment);

            this.Logger.WriteLine(LogLevel.Info, "Running Uri :{0}", queryUri.OriginalString);

            EntityModelSchema schema = Workspace.ConceptualModel;

            // Generate an EDM model based on the EntityModelSchema to use in the UriParser
            IEdmModel model = this.DataServiceProviderFactory.CreateMetadataProvider(schema);

            // Generate an IDSQP based on the EntityModelSchema to use in the Expression Translator
            var queryResolver = this.DataServiceProviderFactory.CreateQueryProvider(schema);

            // 2. product : take the URI , run it through the parser and get the Linq expression
            ODataUriParser parser = new ODataUriParser(model, serviceBaseUri, queryUri);
            var query = parser.ParseUri();
            var result = query.Path;

            // Get the expected Results
            QueryValue expectedResults = null;
            expectedResults = this.Evaluator.Evaluate(expression);
        }
    }
}
