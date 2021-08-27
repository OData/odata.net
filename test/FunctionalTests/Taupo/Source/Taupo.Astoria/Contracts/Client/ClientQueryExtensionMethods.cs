//---------------------------------------------------------------------
// <copyright file="ClientQueryExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Extension methods that extend contracts in the LinqToAstoria namespace
    /// </summary>
    public static class ClientQueryExtensionMethods
    {
        /////// <summary>
        /////// Execute and Compare results async
        /////// </summary>
        /////// <typeparam name="TResult">Item Type of the result collection</typeparam>
        /////// <param name="linqToAstoriaResultComparer">Result Comparer</param>
        /////// <param name="continuation">The async continuation</param>
        /////// <param name="expression">Query to execute</param>
        /////// <param name="expectedValue">Expected value</param>
        /////// <param name="dataContext">Data Context to execute expression from</param>
        /////// <param name="clientExpectedError">Expected Client Error</param>
        ////public static void ExecuteAndCompare<TResult>(this IClientQueryResultComparer linqToAstoriaResultComparer, IAsyncContinuation continuation, IQueryable<TResult> expression, QueryValue expectedValue, DataServiceContext dataContext, ExpectedClientErrorBaseline clientExpectedError)
        ////{
        ////    linqToAstoriaResultComparer.ExecuteAndCompare<TResult>(continuation, true, expression, expectedValue, dataContext, clientExpectedError);
        ////}

        /// <summary>
        /// Execute and Compare results async via the uri pattern
        /// </summary>
        /// <typeparam name="TResult">Item Type of the result collection</typeparam>
        /// <param name="linqToAstoriaResultComparer">Result Comparer</param>
        /// <param name="continuation">The async continuation</param>
        /// <param name="query">Query to execute</param>
        /// <param name="uriString">Uri to execute</param>
        /// <param name="expectedValue">Expected value</param>
        /// <param name="dataContext">Data Context to execute expression from</param>
        /// <param name="clientExpectedError">Expected Client Error</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "3#", Justification = "Need string as a parameter.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "uriString is a common naming convention.")]
        public static void ExecuteUriAndCompare<TResult>(this IClientQueryResultComparer linqToAstoriaResultComparer, IAsyncContinuation continuation, IQueryable<TResult> query, string uriString, QueryValue expectedValue, DataServiceContext dataContext, ExpectedClientErrorBaseline clientExpectedError)
        {
            linqToAstoriaResultComparer.ExecuteUriAndCompare<TResult>(continuation, true, query, uriString, expectedValue, dataContext, clientExpectedError);
        }

        /// <summary>
        /// Execute and Compare results sync
        /// </summary>
        /// <typeparam name="TResult">Item Type of the result collection</typeparam>
        /// <param name="linqToAstoriaResultComparer">Result Comparer</param>
        /// <param name="query">Query to execute</param>
        /// <param name="expectedValue">Expected value</param>
        /// <param name="dataContext">Data Context to execute expression from</param>
        /// <param name="clientExpectedError">Expected Client Error</param>
        public static void ExecuteAndCompareSync<TResult>(this IClientQueryResultComparer linqToAstoriaResultComparer, IQueryable<TResult> query, QueryValue expectedValue, DataServiceContext dataContext, ExpectedClientErrorBaseline clientExpectedError)
        {
            SyncHelpers.ExecuteActionAndWait(c => linqToAstoriaResultComparer.ExecuteAndCompare<TResult>(c, false, query, expectedValue, dataContext, clientExpectedError));
        }

        /// <summary>
        /// Execute and Compare results async via the uri pattern
        /// </summary>
        /// <typeparam name="TResult">Item Type of the result collection</typeparam>
        /// <param name="linqToAstoriaResultComparer">Result Comparer</param>
        /// <param name="query">Query to execute</param>
        /// <param name="uriString">Uri to execute</param>
        /// <param name="expectedValue">Expected value</param>
        /// <param name="dataContext">Data Context to execute expression from</param>
        /// <param name="clientExpectedError">Expected Client Error</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#", Justification = "Need string as a parameter.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "uriString is a common naming convention.")]
        public static void ExecuteUriAndCompareSync<TResult>(this IClientQueryResultComparer linqToAstoriaResultComparer, IQueryable<TResult> query, string uriString, QueryValue expectedValue, DataServiceContext dataContext, ExpectedClientErrorBaseline clientExpectedError)
        {
            SyncHelpers.ExecuteActionAndWait(c => linqToAstoriaResultComparer.ExecuteUriAndCompare<TResult>(c, false, query, uriString, expectedValue, dataContext, clientExpectedError));
        }
    }
}
