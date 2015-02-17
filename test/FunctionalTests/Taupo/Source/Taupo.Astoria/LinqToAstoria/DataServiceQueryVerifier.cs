//---------------------------------------------------------------------
// <copyright file="DataServiceQueryVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.LinqToAstoria
{
#if WINDOWS_PHONE || WIN8
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Astoria.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    using Microsoft.Test.Taupo.Utilities;
    using CoreLinq = System.Linq.Expressions;
    using DSC = Microsoft.OData.Service.Common;

    /// <summary>
    /// QueryVerifier for Windows Phone platform which uses the product's linq translator to run queries and execute tests.
    /// </summary>
    [ImplementationName(typeof(IQueryVerifier), "ClientQuery")]
    public class DataServiceQueryVerifier : IQueryVerifier
    {
        private AstoriaWorkspace workspace;
        private List<QueryExpression> pendingQueries = new List<QueryExpression>();

        /// <summary>
        /// Initializes a new instance of the DataServiceQueryVerifier class.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        public DataServiceQueryVerifier(AstoriaWorkspace workspace)
        {
            this.workspace = workspace;
            this.Logger = Logger.Null;
            this.IsAsync = true;
        }

        /// <summary>
        /// Gets or sets the logger used to print diagnostic messages.
        /// </summary>
        /// <value>The logger.</value>
        [InjectDependency]
        public Logger Logger { get; set; }

        /// <summary>
        /// Gets or sets the code builder used to generate code.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ILinqToAstoriaQueryResolver LinqQueryResolver { get; set; }

        /// <summary>
        /// Gets or sets the code builder used to generate code.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ILinqToAstoriaQuerySpanGenerator SpanGenerator { get; set; }
        
        /// <summary>
        /// Gets or sets the query expression evaluator.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryExpressionEvaluator Evaluator { get; set; }

        /// <summary>
        /// Gets or sets the result comparer.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IClientQueryResultComparer ResultComparer { get; set; }

        /// <summary>
        /// Gets or sets the uri query visitor.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IClientQueryVersionErrorCalculator LinqToAstoriaErrorCalculator { get; set; }

        /// <summary>
        /// Gets or sets the uri query visitor.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryToUriStringConverter UriQueryVisitor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is async.
        /// </summary>
        /// <value><c>true</c> if this instance is async; otherwise, <c>false</c>.</value>
        [InjectTestParameter("Asynchronous", DefaultValueDescription = "true")]
        public bool IsAsync { get; set; }

        /// <summary>
        /// Gets or sets the DataServiceContextCreator 
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDataServiceContextCreator DataServiceContextCreator { get; set; }

        /// <summary>
        /// Gets or sets the DataServiceContextScope 
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDataServiceContextTrackingScope DataServiceContextScope { get; set; }

        /// <summary>
        /// Gets or sets logger to be used for printing diagnostics messages.
        /// </summary>
        [InjectDependency]
        public Logger Log { get; set; }

#if !WINDOWS_PHONE
        /// <summary>
        /// Gets or sets the sending request event verifier to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ISendingRequestEventVerifier SendingRequestEventVerifier { get; set; }
#endif

        /// <summary>
        /// Gets or sets a value indicating whether ExecuteURI test parameter is true.
        /// </summary>
        /// <value><c>true</c> if ExecuteURI test parameter is true; otherwise, <c>false</c>.</value>
        [InjectTestParameter("ExecuteURI", DefaultValueDescription = "false")]
        public bool IsExecuteURI { get; set; }

        /// <summary>
        /// Verify the passed query tree.
        /// </summary>
        /// <param name="expression">The query tree which will be verified</param>
        public virtual void Verify(QueryExpression expression)
        {
            ExceptionUtilities.CheckAllRequiredDependencies(this);
            ExceptionUtilities.Assert(this.IsAsync, "this verifier can only run async");

            this.ThrowSkippedExceptionIfQueryExpressionNotSupported(expression);

            var resolvedQuery = this.LinqQueryResolver.Resolve(expression);
            var dataServiceContext = this.DataServiceContextCreator.CreateContext(this.DataServiceContextScope, this.workspace.ContextType, this.workspace.ServiceUri);

            if (this.IsExecuteURI)
            {
                // TODO: update this verifier to call ExecuteUriAndCompare if ExecuteURI test parameter is set to true
            }
            else
            {
                ClientQueryGenerator queryGenerator = new ClientQueryGenerator(new CSharpGenericTypeBuilder());
                DataServiceContext clientContext = dataServiceContext.Product as DataServiceContext;
                CoreLinq.Expression queryLinqExpression = queryGenerator.Generate(resolvedQuery, clientContext);
                IQueryProvider queryProvider = clientContext.CreateQuery<int>("Set").Provider as IQueryProvider;
                DataServiceQuery dataServiceQuery = queryProvider.CreateQuery(queryLinqExpression) as DataServiceQuery;

                this.ResultComparer.EnqueueNextQuery(resolvedQuery);
                AsyncExecutionContext.EnqueueAsynchronousAction((queryContinuation) =>
                {
                    this.Log.WriteLine(LogLevel.Info, dataServiceQuery.ToString());
                    QueryValue baselineValue = this.GetBaselineValue(resolvedQuery);
                    DataServiceProtocolVersion maxProtocolVersion = this.workspace.ConceptualModel.GetMaxProtocolVersion();
#if  WINDOWS_PHONE
                DataServiceProtocolVersion maxClientProtocolVersion = DataServiceProtocolVersion.V4;
#else
                    DataServiceProtocolVersion maxClientProtocolVersion = ((DSC.DataServiceProtocolVersion)dataServiceContext.MaxProtocolVersion.Product).ToTestEnum();
#endif
                    // Calculate version errors
                    ExpectedClientErrorBaseline clientError = this.LinqToAstoriaErrorCalculator.CalculateExpectedClientVersionError(resolvedQuery, true, maxClientProtocolVersion, maxProtocolVersion);
                    if (clientError != null)
                    {
                        this.Log.WriteLine(LogLevel.Info, "Expected client exception: " + clientError.ExpectedExceptionType.ToString());
                    }

                    Type queryType = dataServiceQuery.ElementType;
                    MethodInfo genericExecuteMethod = this.ResultComparer.GetType().GetMethod("ExecuteAndCompare").MakeGenericMethod(queryType);
                    genericExecuteMethod.Invoke(this.ResultComparer, new object[] { queryContinuation, this.IsAsync, dataServiceQuery, baselineValue, clientContext, clientError });
                });
            }
        }
      
        private QueryValue GetBaselineValue(QueryExpression resolvedQuery)
        {
            QueryValue baselineValue = this.Evaluator.Evaluate(resolvedQuery);

            // get the expand segments and select segments from the query
            this.UriQueryVisitor.ComputeUri(resolvedQuery);
            string select = this.UriQueryVisitor.SelectString;
            string expand = this.UriQueryVisitor.ExpandString;

            IEnumerable<string> selectedPaths = (new string[] { }).AsEnumerable();
            IEnumerable<string> expandedPaths = (new string[] { }).AsEnumerable();
            if (select != null)
            {
                selectedPaths = select.Split(new string[] { "," }, StringSplitOptions.None).AsEnumerable();
            }

            if (expand != null)
            {
                expandedPaths = expand.Split(new string[] { "," }, StringSplitOptions.None).AsEnumerable();
            }

            baselineValue = this.SpanGenerator.GenerateSpan(baselineValue, expandedPaths, selectedPaths);

            return baselineValue;
        }

        private void ThrowSkippedExceptionIfQueryExpressionNotSupported(QueryExpression expression)
        {
            QueryCustomFunctionCallExpression customFunctionCallExpression = expression as QueryCustomFunctionCallExpression;
            if (!this.IsExecuteURI && customFunctionCallExpression != null)
            {
                throw new TestSkippedException(string.Format(CultureInfo.InvariantCulture, "Invalid Query '{0}', operation query not supported through DataServiceQuery", expression.ToString()));
            }

            // Uri will always work, Client linq has cases where it will not
            ODataUriToClientLinqReplacingVisitor visitor = new ODataUriToClientLinqReplacingVisitor();
            visitor.ReplaceExpression(expression);

            if (visitor.InvalidClientQuery == true)
            {
                throw new TestSkippedException(string.Format(CultureInfo.InvariantCulture, "Invalid Query '{0}', query not supported using Linq to Astoria client", expression.ToString()));
            }
        }

        /// <summary>
        /// Class visits QueryExpression and indicates whether this expression is valid for a client
        /// query test
        /// </summary>
        private class ODataUriToClientLinqReplacingVisitor : LinqToAstoriaExpressionReplacingVisitor
        {
            internal ODataUriToClientLinqReplacingVisitor()
            {
                this.InvalidClientQuery = false;
            }

            internal bool InvalidClientQuery { get; private set; }

            /// <summary>
            /// Overrides the Visit expression and finds and analyzes property expressions to see if the KeyExpression is before
            /// If it is then this is an invalid query for the client
            /// </summary>
            /// <param name="expression">Expression to be analyzed</param>
            /// <returns>The same expression</returns>
            public override QueryExpression Visit(QueryPropertyExpression expression)
            {
                var keyExpression = expression.Instance as LinqToAstoriaKeyExpression;
                if (keyExpression != null)
                {
                    this.InvalidClientQuery = true;
                }

                return base.Visit(expression);
            }
        }
    }
#else
    /// <summary>
    /// Stub verifier class, this class is only available on the Windows Phone platform
    /// </summary>
    public class DataServiceQueryVerifier
    {
    }
#endif
}