//---------------------------------------------------------------------
// <copyright file="IClientQueryResultComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System;
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Executes Astoria queries and compares results against a baseline.
    /// </summary>
    [ImplementationSelector("ClientQueryResultComparer", DefaultImplementation = "Default")]
    public interface IClientQueryResultComparer
    {
        /// <summary>
        /// Executes an action
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="continuation">The async continuation.</param>
        /// <param name="isAsync">Determines whether to execute this using the Async pattern or not</param>
        /// <param name="uriString">The action uri expression</param>
        /// <param name="verb">Method to invoke execute on</param>
        /// <param name="inputParameters">Input parameters for Execute Uri</param>
        /// <param name="dataContext">data context executed on</param>
        /// <param name="clientExpectedError">expected client error</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#", Justification = "Need string as a parameter.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "uriString is a common naming convention.")]
        void ExecuteUriAndCompare(IAsyncContinuation continuation, bool isAsync, string uriString, HttpVerb verb, OperationParameter[] inputParameters, DataServiceContext dataContext, ExpectedClientErrorBaseline clientExpectedError);

        /// <summary>
        /// Executes an action
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="continuation">The async continuation.</param>
        /// <param name="isAsync">Determines whether to execute this using the Async pattern or not</param>
        /// <param name="uriString">The action uri expression</param>
        /// <param name="verb">Method to invoke execute on</param>
        /// <param name="inputParameters">Input parameters for Execute Uri</param>
        /// <param name="singleResult">Determines whether its returning a multivalue or a single primitive</param>
        /// <param name="dataContext">data context executed on</param>
        /// <param name="clientExpectedError">expected client error</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#", Justification = "Need string as a parameter.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "uriString is a common naming convention.")]
        void ExecuteUriAndCompare<TResult>(IAsyncContinuation continuation, bool isAsync, string uriString, HttpVerb verb, OperationParameter[] inputParameters, bool singleResult, DataServiceContext dataContext, ExpectedClientErrorBaseline clientExpectedError);

        /// <summary>
        /// Executes the given expression asynchronously and compares the result to a given expected value.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="continuation">The async continuation.</param>
        /// <param name="isAsync">Determines whether to execute this using the Async pattern or not</param>
        /// <param name="query">The query expression.</param>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="dataContext">The data context.</param>
        /// <param name="clientExpectedError">Client Expected Error Information</param>
        void ExecuteAndCompare<TResult>(IAsyncContinuation continuation, bool isAsync, IQueryable<TResult> query, QueryValue expectedValue, DataServiceContext dataContext, ExpectedClientErrorBaseline clientExpectedError);

        /// <summary>
        /// Executes the given URI query asynchronously and compares the result to a given expected value.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="continuation">The async continuation.</param>
        /// <param name="isAsync">Determines whether to execute this using the Async pattern or not</param>
        /// <param name="query">The query.</param>
        /// <param name="uriString">The uri string.</param>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="dataContext">The data context.</param>
        /// <param name="clientExpectedError">Client Expected Error Information</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "3#", Justification = "Need string as a parameter.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "uriString is a common naming convention.")]
        void ExecuteUriAndCompare<TResult>(IAsyncContinuation continuation, bool isAsync, IQueryable<TResult> query, string uriString, QueryValue expectedValue, DataServiceContext dataContext, ExpectedClientErrorBaseline clientExpectedError);

        /// <summary>
        /// Enqueues the executing query to be used by result comparer later.
        /// </summary>
        /// <param name="expression">The query expression</param>
        void EnqueueNextQuery(QueryExpression expression);

        /// <summary>
        /// Evaluates the specified function and compares the result against provided value.
        /// If the evaluation function throws, the exception is compared against
        /// </summary>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="queryEvaluationFunc">Function which is evaluated to get the results of a query
        /// (or throw an exception which is then caught and verified by the comparer).</param>
        /// <param name="context">Context to be used when executing and comparing results of the query.</param>
        void Compare(QueryValue expectedValue, Func<object> queryEvaluationFunc, object context);
    }
}
