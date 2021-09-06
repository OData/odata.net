//---------------------------------------------------------------------
// <copyright file="ClientQueryResultComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts;
    using HttpHeaders = Contracts.Http.HttpHeaders;

    /// <summary>
    /// Compares results of LINQ to Astoria expression evaluation to a baseline also compares exceptions thrown during query evaluation (if any).
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Need to refactor client verification related classes in the future.")]
    [ImplementationName(typeof(IClientQueryResultComparer), "Default")]
    public class ClientQueryResultComparer : ClientResultComparerBase, IClientQueryResultComparer
    {
        /// <summary>
        /// stores the baseline query value if using payload-driven verification.
        /// </summary>
        private QueryValue baselineQueryValue;

        /// <summary>
        /// stores all the executing expressions. A query expression is removed from the queue if baseline value is produced.
        /// </summary>
        private Queue<QueryExpression> expressionQueue;

        private DataServiceContext currentContext;
        private bool clientExceptionVerified = false;
        private bool isAsynchronous;

        /// <summary>
        /// Initializes a new instance of the ClientQueryResultComparer class.
        /// </summary>
        public ClientQueryResultComparer()
            : base()
        {
            this.expressionQueue = new Queue<QueryExpression>();
            this.SkipStreamDescriptorValuesVerification = false;
        }

        /// <summary>
        /// Gets or sets the http tracker.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDataServiceContextHttpTracker HttpTracker { get; set; }

        /// <summary>
        /// Gets or sets the base line query value builder.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IClientResponseQueryValueBuilder ResponseQueryValueBuilder { get; set; }

        /// <summary>
        /// Gets or sets the xml to payload element converter
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IXmlToPayloadElementConverter XmlToPayloadElementConverter { get; set; }

        /// <summary>
        /// Gets or sets the Client exception message Verifier
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IClientExpectedErrorComparer ClientExpectedExceptionComparer { get; set; }

        /// <summary>
        /// Gets or sets the data provider settings.
        /// </summary>
        /// <value>The data provider settings.</value>
        [InjectDependency(IsRequired = true)]
        public DataProviderSettings DataProviderSettings { get; set; }

        /// <summary>
        /// Gets or sets the data context format applier
        /// </summary>
        /// <value>The datacontext format applier.</value>
        [InjectDependency(IsRequired = true)]
        public IClientDataContextFormatApplier ClientDataContextFormatApplier { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tests should generate named streams 
        /// </summary>
        [InjectTestParameter("SkipNamedStreamsDataPopulation", DefaultValueDescription = "False", HelpText = "Whether the tests should generate named streams")]
        public bool SkipGenerateNamedStream { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tests should generate named streams 
        /// </summary>
        [InjectTestParameter("SkipStreamDescriptorValuesVerification", DefaultValueDescription = "False", HelpText = "Whether the tests should validate the links returned for stream data")]
        public bool SkipStreamDescriptorValuesVerification { get; set; }

        /// <summary>
        /// Gets or sets the code generator.
        /// </summary>
        /// <value>The code generator.</value>
        [InjectDependency(IsRequired = true)]
        public IClientCodeLayerGenerator CodeGenerator { get; set; }

        /// <summary>
        /// Gets or sets the assertion handler to use.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public AssertionHandler Assert { get; set; }

        /// <summary>
        /// Enqueues the executing query to be used by result comparer later.
        /// </summary>
        /// <param name="expression">The query expression</param>
        public void EnqueueNextQuery(QueryExpression expression)
        {
            this.expressionQueue.Enqueue(expression);
        }

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "3#", Justification = "Naming is consistent with original patterns")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "Naming is consistent with original patterns")]
        public void ExecuteUriAndCompare(IAsyncContinuation continuation, bool isAsync, string uriString, HttpVerb verb, OperationParameter[] inputParameters, DataServiceContext dataContext, ExpectedClientErrorBaseline clientExpectedError)
        {
            this.isAsynchronous = isAsync;
            this.ExecuteQueryAction(
                continuation,
                null,
                dataContext,
                clientExpectedError,
                delegate(List<object> entityPayloads, IAsyncContinuation continuation2)
                {
                    AsyncHelpers.CatchErrors(
                        continuation2,
                        () =>
                        {
                            this.Logger.WriteLine(LogLevel.Verbose, "Executing Invoke Action, async:{0}, Uri:{1}:", isAsync, dataContext.BaseUri + "/" + uriString);
                            Uri builtUri = new Uri(dataContext.BaseUri + "/" + uriString);
                            EventHandler<SendingRequest2EventArgs> sendingRequest = delegate(object sender, SendingRequest2EventArgs args)
                            {
                                HttpRequestMessage request = ((HttpClientRequestMessage)args.RequestMessage).HttpRequestMessage;
                                this.VerifyActionExecuteHeaders(verb, request.Headers, inputParameters);
                                this.VerifyCommonExecuteHeaders(request.Headers);
                            };

                            dataContext.SendingRequest2 += sendingRequest;
                            dataContext.ExecuteUri(
                                continuation2,
                                isAsync,
                                builtUri,
                                verb,
                                inputParameters,
                                delegate
                                {
                                    dataContext.SendingRequest2 -= sendingRequest;
                                    continuation2.Continue();
                                });
                        });
                });
        }

        /// <summary>
        /// Executes an action
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="continuation">The async continuation.</param>
        /// <param name="isAsync">Determines whether to execute this using the Async pattern or not</param>
        /// <param name="uriString">The action uri expression</param>
        /// <param name="verb">Method to invoke execute on</param>
        /// <param name="inputParameters">Input parameters for Execute Uri</param>
        /// <param name="singleResult">returns Single result or not</param>
        /// <param name="dataContext">data context executed on</param>
        /// <param name="clientExpectedError">expected client error</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "3#", Justification = "Naming is consistent with original patterns")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "Naming is consistent with original patterns")]
        public void ExecuteUriAndCompare<TResult>(IAsyncContinuation continuation, bool isAsync, string uriString, HttpVerb verb, OperationParameter[] inputParameters, bool singleResult, DataServiceContext dataContext, ExpectedClientErrorBaseline clientExpectedError)
        {
            this.isAsynchronous = isAsync;
            this.ExecuteQueryAction(
                continuation,
                null,
                dataContext,
                clientExpectedError,
                delegate(List<object> entityPayloads, IAsyncContinuation continuation2)
                {
                    AsyncHelpers.CatchErrors(
                        continuation2,
                        () =>
                        {
                            this.Logger.WriteLine(LogLevel.Verbose, "Executing Invoke Action, async:{0}, Uri:{1}:", isAsync, dataContext.BaseUri + "/" + uriString);
                            Uri builtUri = new Uri(dataContext.BaseUri + "/" + uriString);
                            EventHandler<SendingRequest2EventArgs> sendingRequest = delegate(object sender, SendingRequest2EventArgs args)
                            {
                                HttpRequestMessage request = ((HttpClientRequestMessage)args.RequestMessage).HttpRequestMessage;
                                this.VerifyActionExecuteHeaders(verb, request.Headers, inputParameters);
                                this.VerifyCommonExecuteHeaders(request.Headers);
                            };

                            dataContext.SendingRequest2 += sendingRequest;
                            dataContext.ExecuteUri<TResult>(
                                continuation2,
                                isAsync,
                                builtUri,
                                verb,
                                inputParameters,
                                singleResult,
                                delegate(QueryOperationResponse<TResult> results)
                                {
                                    var savedRes = results.ToList();
                                    if (singleResult)
                                    {
                                        // With singleResult=true, we expected 0/1 result item. Compare() will calculate and update the expected QueryValue to be non-NullValue.
                                        this.Compare(this.expressionQueue.Peek().ExpressionType.NullValue, () => savedRes.SingleOrDefault(), dataContext);
                                    }
                                    else
                                    {
                                        // With singleResult=false, we expected multiple result items. Compare() will calculate and update the expected QueryValue to be non-NullValue.
                                        this.Compare(this.expressionQueue.Peek().ExpressionType.NullValue, () => savedRes, dataContext);
                                    }

                                    dataContext.SendingRequest2 -= sendingRequest;

                                    continuation2.Continue();
                                });
                        });
                });
        }

        /// <summary>
        /// Executes the given expression asynchronously and compares the result to a given expected value.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="continuation">The continuation.</param>
        /// <param name="isAsync">Determines if we execute async or not</param>
        /// <param name="query">Query to execute</param>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="dataContext">The data context.</param>
        /// <param name="clientExpectedError">Client Expected Error Information</param>
        public void ExecuteAndCompare<TResult>(IAsyncContinuation continuation, bool isAsync, IQueryable<TResult> query, QueryValue expectedValue, DataServiceContext dataContext, ExpectedClientErrorBaseline clientExpectedError)
        {
            this.isAsynchronous = isAsync;
            this.ExecuteQueryAction(
                continuation,
                expectedValue,
                dataContext,
                clientExpectedError,
                delegate(List<object> entityPayloads, IAsyncContinuation continuation2)
                {
                    AsyncHelpers.CatchErrors(
                        continuation2,
                        () =>
                        {
                            var dataServiceQuery = (DataServiceQuery<TResult>)query;
                            this.Logger.WriteLine(LogLevel.Verbose, "Executing query async:{0}, Expression:{1}:", isAsync, dataServiceQuery.Expression);

                            dataServiceQuery.Execute(
                                continuation2,
                                isAsync,
                                delegate(IEnumerable<TResult> results)
                                {
                                    this.Compare(expectedValue, () => results, dataContext);

                                    if (entityPayloads.Count > 0)
                                    {
                                        this.VerifyDescriptorAfterExecute(continuation2, dataContext, entityPayloads, expectedValue);
                                    }
                                    else
                                    {
                                        continuation2.Continue();
                                    }
                                });
                        });
                });
        }

        /// <summary>
        /// Executes the given URI query asynchronously and compares the result to a given expected value.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="continuation">The continuation.</param>
        /// <param name="isAsync">Determines if we execute async or not</param>
        /// <param name="query">The query expression.</param>
        /// <param name="uriString">The uri string.</param>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="dataContext">The data context.</param>
        /// <param name="clientExpectedError">Client Expected Error Information</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "3#", Justification = "Naming is consistent with original patterns")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "Naming is consistent with original patterns")]
        public void ExecuteUriAndCompare<TResult>(IAsyncContinuation continuation, bool isAsync, IQueryable<TResult> query, string uriString, QueryValue expectedValue, DataServiceContext dataContext, ExpectedClientErrorBaseline clientExpectedError)
        {
            this.isAsynchronous = isAsync;
            this.ExecuteQueryAction(
                continuation,
                expectedValue,
                dataContext,
                clientExpectedError,
                delegate(List<object> entityPayloads, IAsyncContinuation continuation2)
                {
                    AsyncHelpers.CatchErrors(
                        continuation2,
                        () =>
                        {
                            this.Logger.WriteLine(LogLevel.Verbose, "Executing query, async:{0}, Uri:{1}:", isAsync, dataContext.BaseUri + "/" + uriString);

                            Uri builtUri = new Uri(dataContext.BaseUri + "/" + uriString);

                            EventHandler<SendingRequest2EventArgs> sendingRequest = delegate(object sender, SendingRequest2EventArgs args)
                            {
                                HttpRequestMessage request = ((HttpClientRequestMessage)args.RequestMessage).HttpRequestMessage;
                                this.VerifyCommonExecuteHeaders(request.Headers);
                            };

                            dataContext.SendingRequest2 += sendingRequest;

                            dataContext.Execute(
                                continuation2,
                                isAsync,
                                builtUri,
                                delegate(IEnumerable<TResult> results)
                                {
                                    this.Compare(expectedValue, () => results, dataContext);

                                    if (entityPayloads.Count > 0)
                                    {
                                        this.VerifyDescriptorAfterExecute(continuation2, dataContext, entityPayloads, expectedValue);
                                    }
                                    else
                                    {

                                        dataContext.SendingRequest2 -= sendingRequest;

                                        continuation2.Continue();
                                    }
                                });
                        });
                });
        }


        internal void VerifyActionExecuteHeaders(HttpVerb verb, HttpRequestHeaders headers, OperationParameter[] inputParameters)
        {
            if (verb == HttpVerb.Post && inputParameters.OfType<BodyOperationParameter>().Any())
            {
                this.Assert.IsTrue(headers.GetValues(HttpHeaders.ContentType).Contains(MimeTypes.ApplicationJsonLight), string.Format("Unexpected Content-Type header value:{0}", headers.GetValues(HttpHeaders.ContentType)));
            }
            else
            {
                this.Assert.IsNull(headers.GetValues(HttpHeaders.ContentType), "Unexpected Content-Type header.");
            }

            this.Assert.IsNull(headers.GetValues(HttpHeaders.ContentLength), "Unexpected Content-Length header.");
            this.Assert.AreEqual("3.0;NetFx", headers.GetValues(HttpHeaders.MaxDataServiceVersion), "Unexpected MaxDataServiceVersion header value.");
            this.Assert.AreEqual(MimeTypes.ApplicationAtomXml + "," + MimeTypes.ApplicationXml, headers.GetValues(HttpHeaders.Accept), "Unexpected Accept header value.");
        }

        internal void VerifyCommonExecuteHeaders(HttpRequestHeaders headers)
        {
            this.Assert.IsNull(headers.GetValues(HttpHeaders.IfMatch), "Unexpected IfMatch.");
            this.Assert.IsNull(headers.GetValues(HttpHeaders.IfNoneMatch), "Unexpected IfNoneMatch.");
            this.Assert.IsNull(headers.GetValues(HttpHeaders.ETag), "Unexpected ETag.");
        }

        /// <summary>
        /// Compares object with the expected value.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="path">The path to the compared object (for debugging purposes).</param>
        /// <param name="shouldThrow">Should exception be thrown if error is encountered.</param>
        /// <returns>Result of the comparison, Success, Failure or Skipped.</returns>
        protected override ComparisonResult Compare(QueryValue expected, object actual, string path, bool shouldThrow)
        {
            if (this.DataProviderSettings.UsePayloadDrivenVerification && path == "result")
            {

                this.HttpTracker.TryCompleteCurrentRequest(this.currentContext);

                expected = this.baselineQueryValue;
            }

            return base.Compare(expected, actual, path, shouldThrow);
        }

        /// <summary>
        /// Compares structural object with the expected value.
        /// </summary>
        /// <param name="expected">Expected value.</param>
        /// <param name="actual">Actual value.</param>
        /// <param name="path">The path to the compared object (for debugging purposes).</param>
        /// <param name="shouldThrow">Should exception be thrown if error is encountered.</param>
        /// <returns>
        /// Result of the comparison, Success, Failure or Skipped.
        /// </returns>
        protected override ComparisonResult CompareStructural(QueryStructuralValue expected, object actual, string path, bool shouldThrow)
        {
            var hasDefaultStreamProperty = expected.Type.Properties.Any(p => p.Name == AstoriaQueryStreamType.DefaultStreamPropertyName);
            if (hasDefaultStreamProperty)
            {
                MaskedQueryStructuralValue newExpected = MaskedQueryStructuralValue.Create(expected);

                // need to remove the default stream property from the list of MemberNames for the expected value since the actual won't have that
                newExpected.HideMember(AstoriaQueryStreamType.DefaultStreamPropertyName);

                return base.CompareStructural(newExpected, actual, path, shouldThrow);
            }
            else
            {
                return base.CompareStructural(expected, actual, path, shouldThrow);
            }
        }

        /// <summary>
        /// Compares properties of a structural object with expected value. Overridden here to handle named streams.
        /// </summary>
        /// <param name="structuralValue">The structural object containing the property to compare</param>
        /// <param name="expectedName">The name of the property to compare</param>
        /// <param name="expectedPropertyType">The expected type of the property</param>
        /// <param name="actualValue">The actual value of the property</param>
        /// <param name="path">The path to the compared object (for debugging purposes)</param>
        /// <param name="shouldThrow">Should exception be thrown if error is encountered</param>
        /// <returns>The comparison result</returns>
        protected override ComparisonResult CompareProperty(QueryStructuralValue structuralValue, string expectedName, QueryType expectedPropertyType, object actualValue, string path, bool shouldThrow)
        {
            if (this.CodeGenerator.GetType() == typeof(RemoteClientCodeLayerGenerator))
            {
                if (structuralValue.GetValue(expectedName).IsNull && expectedPropertyType is QueryComplexType)
                {
                    return ComparisonResult.Success;
                }
            }

            if (expectedPropertyType is AstoriaQueryStreamType)
            {
                DataServiceStreamLink actualStreamLink = (DataServiceStreamLink)actualValue;
                AstoriaQueryStreamValue expectedStreamValue = (AstoriaQueryStreamValue)structuralValue.GetStreamValue(expectedName);

                if (actualStreamLink == null)
                {
                    if (!expectedStreamValue.IsNull)
                    {
                        this.ThrowOrLogError(shouldThrow, "Expected DataServiceStreamLink property to be null. Actual: {0}", actualStreamLink);
                        return ComparisonResult.Failure;
                    }
                    else
                    {
                        return ComparisonResult.Success;
                    }
                }

                try
                {
                    this.VerifyStreamLink(expectedStreamValue, actualStreamLink);
                    return ComparisonResult.Success;
                }
                catch (TestFailedException e)
                {
                    this.ThrowOrLogError(shouldThrow, e.ToString());
                    return ComparisonResult.Failure;
                }
            }
            else
            {
                return base.CompareProperty(structuralValue, expectedName, expectedPropertyType, actualValue, path, shouldThrow);
            }
        }

        /// <summary>
        /// Compares Collections, optimizes entity collection comparisons for better error messages or defers to the base
        /// </summary>
        /// <param name="expected">Expected Value</param>
        /// <param name="actualElements">Actual Elements to compare against</param>
        /// <param name="path">Path of the elements</param>
        /// <param name="shouldThrow">Should throw on error or not</param>
        /// <param name="comparisonSkippedForAnyElement">Skip for particular comparisons</param>
        /// <returns>A Comparison result</returns>
        protected override ComparisonResult CompareCollections(QueryCollectionValue expected, IEnumerable<object> actualElements, string path, bool shouldThrow, out bool comparisonSkippedForAnyElement)
        {
            comparisonSkippedForAnyElement = false;
            QueryEntityType queryEntityType = expected.Type.ElementType as QueryEntityType;

            if (queryEntityType != null)
            {
                List<QueryProperty> keyProperties = queryEntityType.Properties.Where(p => p.IsPrimaryKey).ToList();
                ExceptionUtilities.Assert(queryEntityType.Properties.Where(p => p.IsPrimaryKey).Count() > 0, "QueryEntityType '{0}' must have a Primary key defined in order to compare using primary keys", queryEntityType.EntityType.FullName);

                List<object> unprocessableElements = actualElements.Where(e => !queryEntityType.ClrType.IsAssignableFrom(e.GetType())).ToList();
                if (unprocessableElements.Count > 0)
                {
                    string errorDetails = string.Join(", ", unprocessableElements.Select(o => string.Format(CultureInfo.InvariantCulture, "Expected object '{0}' to be assignable to '{1}'", o.GetType().FullName, queryEntityType.ClrType.FullName)).ToArray());
                    this.ThrowOrLogError(shouldThrow, "There are '{0}' elements that are unprocessable, Details '{1}'", unprocessableElements.Count, errorDetails);
                    return ComparisonResult.Failure;
                }

                List<object> unprocessedElements = actualElements.Where(e => queryEntityType.ClrType.IsAssignableFrom(e.GetType())).ToList();
                foreach (QueryStructuralValue queryStructuralValue in expected.Elements)
                {
                    var queryEntityValue = queryStructuralValue as QueryEntityValue;
                    ExceptionUtilities.CheckObjectNotNull(queryEntityValue, "Expected Collection element to be a 'QueryEntityValue', got a '{0}' instead", queryStructuralValue);
                    ExceptionUtilities.CheckObjectNotNull(queryEntityValue.IsNull, "Expected Collection of QueryEntityValues to not contain a QueryEntityValue that is null '{0}'", queryEntityValue.ToString());

                    // Note: We could fix this limitation by simply using the normal unsorted code path instead in these cases but no point implementing this unless its needed.
                    ExceptionUtilities.CheckObjectNotNull(keyProperties.Select(kp => queryEntityValue.GetScalarValue(kp.Name).IsNull).Any(), "Error: QueryEntityValue must have key values defined");

                    QueryKeyStructuralValue key = queryEntityValue.Key();
                    object entityInstance = unprocessedElements.Where(upe => queryEntityType.GetEntityInstanceKey(upe).Equals(key)).SingleOrDefault();
                    if (entityInstance == null)
                    {
                        this.ThrowOrLogError(shouldThrow, "Cannot find actual EntityInstance that is assignable to type '{0}' with key '{1}' in expected collection '{2}'", queryEntityType.EntityType.FullName, key.GetDebugKeyString(), expected);
                        return ComparisonResult.Failure;
                    }
                    else
                    {
                        unprocessedElements.Remove(entityInstance);
                        string childPath = string.Format(CultureInfo.InvariantCulture, "{0}.CollectionItem(ElementType='{1}',Key='{2}')", path, queryEntityType.EntityType.FullName, key.GetDebugKeyString());
                        ComparisonResult innerResult = this.Compare(queryEntityValue, entityInstance, childPath, false);

                        if (innerResult == ComparisonResult.Failure)
                        {
                            this.DisplayLog();
                            this.ThrowOrLogError(shouldThrow, "Comparision of EntityInstance in path '{0}' failed, see log for details", childPath, key.GetDebugKeyString(), expected);
                            return ComparisonResult.Failure;
                        }
                    }
                }

                if (unprocessedElements.Count > 0)
                {
                    string errorDetails = string.Join(", ", unprocessedElements.Select(o => string.Format(CultureInfo.InvariantCulture, "Actual EntityInstance of type '{0}' with key '{1}' is not contained in expected collection '{2}'", o.GetType().FullName, queryEntityType.GetEntityInstanceKey(o).GetDebugKeyString(), expected)).ToArray());
                    this.ThrowOrLogError(shouldThrow, "There are '{0}' elements that are unprocessable, Details '{1}'", unprocessedElements.Count, errorDetails);
                    return ComparisonResult.Failure;
                }

                return ComparisonResult.Success;
            }
            else
            {
                return base.CompareCollections(expected, actualElements, path, shouldThrow, out comparisonSkippedForAnyElement);
            }
        }

        /// <summary>
        /// Verifies the descriptor after execute.
        /// </summary>
        /// <param name="continuation">The continuation.</param>
        /// <param name="dataContext">The data context.</param>
        /// <param name="entityPayloads">The list of entities</param>
        /// <param name="expectedValue">The expected value.</param>
        protected void VerifyDescriptorAfterExecute(IAsyncContinuation continuation, DataServiceContext dataContext, IEnumerable<object> entityPayloads, QueryValue expectedValue)
        {
            // If no streams were generated as part of the test initialization then this block of verification is needed
            if (this.SkipGenerateNamedStream)
            {
                continuation.Continue();
            }
            else
            {
                object[] entities = entityPayloads.ToArray();
                if (this.DataProviderSettings.UsePayloadDrivenVerification)
                {
                    expectedValue = this.baselineQueryValue;
                }

                var expectedEntities = expectedValue as QueryCollectionValue;
                QueryStructuralValue element;

                if (expectedEntities == null)
                {
                    continuation.Continue();
                }
                else
                {
                    AsyncHelpers.AsyncForEach(
                             expectedEntities.Elements.ToList(),
                             continuation,
                             (qv, entityContinuation) =>
                             {
                                 // NOTE: The results could be a list of AnonTypes at which point these wouldn't be have descriptors so 
                                 // no need to verify
                                 element = qv as QueryStructuralValue;

                                 var queryEntityType = element.Type as QueryEntityType;
                                 if (queryEntityType == null)
                                 {
                                     entityContinuation.Continue();
                                 }
                                 else
                                 {
                                     var queryEntityValue = element as QueryEntityValue;
                                     QueryKeyStructuralValue key = queryEntityValue.Key();

                                     // This handles the expand scenario (Orders(1)?$expand=Customer) where the entity in the list doesn't have a corresponding QueryStructuralValue
                                     object entity = entities.Where(upe => queryEntityType.ClrType.IsAssignableFrom(upe.GetType()) && queryEntityType.GetEntityInstanceKey(upe).Equals(key)).FirstOrDefault();

                                     if (entity == null)
                                     {
                                         entityContinuation.Continue();
                                     }
                                     else
                                     {
                                         EntityDescriptor ed = dataContext.GetEntityDescriptor(entity);
                                         var streamProperties = queryEntityType.Properties.Where(p => p.IsStream()).ToList(); // intentionally include the default stream
                                         int expectedStreamDescriptorCount = streamProperties.Count(p => p.Name != AstoriaQueryStreamType.DefaultStreamPropertyName);
                                         this.Assert.AreEqual(expectedStreamDescriptorCount, ed.StreamDescriptors.Count, "Entity descriptor had unexpected number of stream descriptors");

                                         AsyncHelpers.AsyncForEach(
                                             streamProperties,
                                             entityContinuation,
                                             (streamProperty, streamDescriptorContinuation) =>
                                             {
                                                 if (streamProperty.Name == AstoriaQueryStreamType.DefaultStreamPropertyName)
                                                 {
                                                     this.VerifyDefaultStream(dataContext, element, ed, streamDescriptorContinuation);
                                                 }
                                                 else
                                                 {
                                                     this.VerifyNamedStreams(dataContext, element, streamProperty, ed, streamDescriptorContinuation);
                                                 }
                                             });
                                     }
                                 }
                             });
                }
            }
        }

        /// <summary>
        /// Verifies the streams data.
        /// </summary>
        /// <param name="expectedStreamValue">The expected stream value.</param>
        /// <param name="response">The stream response.</param>
        /// <returns>The result of stream verification</returns>
        protected bool VerifyStreams(AstoriaQueryStreamValue expectedStreamValue, DataServiceStreamResponse response)
        {
            var expectedBytes = new byte[0];
            if (!expectedStreamValue.IsNull)
            {
                expectedBytes = expectedStreamValue.Value;
            }

            var expectedStream = new MemoryStream(expectedBytes);

            try
            {
                ExceptionUtilities.Assert(response.Stream.CanRead, "Cannot read from the stream");

                return StreamHelpers.CompareStream(response.Stream, expectedStream);
            }
            finally
            {
                response.Stream.Dispose();
                expectedStream.Dispose();
            }
        }

        /// <summary>
        /// Handles response payload received from server.
        /// </summary>
        /// <param name="context">The data service context being tracked.</param>
        /// <param name="request">The http request data sent to the server.</param>
        /// <param name="response">The http response data received from the server.</param>
        protected void BuildBaselineValueFromResponse(DataServiceContext context, HttpRequestData request, HttpResponseData response)
        {
            // If queryType is not void, expect NullValue of queryType for 204 response verification.
            QueryType queryType = this.expressionQueue.Peek().ExpressionType;
            if (!(queryType is QueryVoidType))
            {
                this.baselineQueryValue = queryType.NullValue;
            }

            if (response.Body != null)
            {
                var query = this.expressionQueue.Peek();
                this.baselineQueryValue = this.ResponseQueryValueBuilder.BuildQueryValue(query, response);
            }
        }

        private static Uri GetExpectedReadStreamUri(AstoriaQueryStreamValue expectedStreamValue)
        {
            var expectedReadStreamUri = expectedStreamValue.SelfLink;
            if (expectedReadStreamUri == null)
            {
                expectedReadStreamUri = expectedStreamValue.EditLink;
            }

            return expectedReadStreamUri;
        }

        private static void UpdateStreamValueFromHeaders(AstoriaQueryStreamValue expectedStreamValue, DataServiceStreamResponse response)
        {
            expectedStreamValue.ContentType = response.Headers[HttpHeaders.ContentType];

            string etag;
            response.Headers.TryGetValue(HttpHeaders.ETag, out etag);
            expectedStreamValue.ETag = etag;
        }

        /// <summary>
        /// Wraps query Execution action sync or async and verifies exceptions
        /// </summary>
        /// <param name="continuation">Continuation token to register success or failure to</param>
        /// <param name="expectedValue">Expected Value of the Query</param>
        /// <param name="dataContext">Content the Query is executed on</param>
        /// <param name="clientExpectedError">Expected Error the query is executed on</param>
        /// <param name="executeAction">Action to execute specified query</param>
        private void ExecuteQueryAction(
            IAsyncContinuation continuation,
            QueryValue expectedValue,
            DataServiceContext dataContext,
            ExpectedClientErrorBaseline clientExpectedError,
            Action<List<object>, IAsyncContinuation> executeAction)
        {
            ExceptionUtilities.CheckAllRequiredDependencies(this);
            this.clientExceptionVerified = false;

            continuation = continuation.OnFail(() => this.expressionQueue.Clear());
            continuation = continuation.OnContinue(() => this.expressionQueue.TryDequeue());

            AsyncHelpers.HandleException<Exception>(
                continuation,
                exceptionHandlerContinuation =>
                {
                    List<object> entityPayloads = new List<object>();

                    if (this.DataProviderSettings.UsePayloadDrivenVerification && clientExpectedError == null)
                    {

                        this.HttpTracker.RegisterHandler(dataContext, this.BuildBaselineValueFromResponse);
                        exceptionHandlerContinuation = AsyncHelpers.OnContinueOrFail(
                            exceptionHandlerContinuation,
                            () => this.HttpTracker.UnregisterHandler(dataContext, this.BuildBaselineValueFromResponse, !this.clientExceptionVerified));

                        if (dataContext != this.currentContext)
                        {
                            this.currentContext = dataContext;
                        }
                    }

                    exceptionHandlerContinuation = exceptionHandlerContinuation.OnContinue(
                    () =>
                    {
                        if (clientExpectedError != null && !this.clientExceptionVerified)
                        {
                            continuation.Fail(new TaupoInvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Expected a client exception with resource id '{0}' and of exception type '{1}'.", clientExpectedError.ExpectedExceptionMessage.ResourceIdentifier, clientExpectedError.ExpectedExceptionType.FullName)));
                        }
                    });

                    executeAction(entityPayloads, exceptionHandlerContinuation);
                },
                e =>
                {
                    if (this.DataProviderSettings.UsePayloadDrivenVerification && this.baselineQueryValue != null && this.baselineQueryValue.EvaluationError != null)
                    {
                        ExceptionUtilities.Assert(e.GetType() == typeof(DataServiceQueryException), "Expected DataServiceQueryException, received " + e.GetType().ToString());
                        this.Logger.WriteLine(LogLevel.Verbose, this.baselineQueryValue.EvaluationError.ToString());
                    }
                    else
                    {
                        this.CompareClientOrEvaluationException(expectedValue, clientExpectedError, e);
                    }

                    continuation.Continue();
                });
        }


        /// <summary>
        /// Verifies the named streams.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="queryEntityValue">The query entity value.</param>
        /// <param name="streamProperty">The stream property.</param>
        /// <param name="ed">The entity descriptor</param>
        /// <param name="continuation">The stream descriptor continuation.</param>
        private void VerifyNamedStreams(DataServiceContext dataContext, QueryStructuralValue queryEntityValue, QueryProperty streamProperty, EntityDescriptor ed, IAsyncContinuation continuation)
        {
            var expectedStreamValue = queryEntityValue.GetStreamValue(streamProperty.Name);
            var streamDescriptor = ed.StreamDescriptors.SingleOrDefault(s => s.StreamLink.Name == streamProperty.Name);
            this.Assert.IsNotNull(streamDescriptor, "Entity missing stream descriptor for stream '{0}'", streamProperty.Name);
            this.VerifyStreamLink(expectedStreamValue, streamDescriptor.StreamLink);

            var expectedReadStreamUri = GetExpectedReadStreamUri(expectedStreamValue);

            Uri readStreamUri = dataContext.GetReadStreamUri(ed.Entity, streamProperty.Name);
            this.Assert.AreEqual(expectedReadStreamUri, readStreamUri, "Read stream uri did not match for stream '{0}'", streamProperty.Name);

            dataContext.GetReadStream(
                continuation,
                this.isAsynchronous,
                ed.Entity,
                streamProperty.Name,
                new DataServiceRequestArgs() { },
                    response =>
                    {
                        UpdateStreamValueFromHeaders(expectedStreamValue, response);
                        this.VerifyStreamLink(expectedStreamValue, streamDescriptor.StreamLink);
                        // skip this verification when using payload driven verification since we don't have the expected content for streams
                        if (!this.DataProviderSettings.UsePayloadDrivenVerification)
                        {
                            this.Assert.IsTrue(this.VerifyStreams(expectedStreamValue, response), "Failed to compare value of stream '{0}'", streamProperty.Name);
                        }

                        continuation.Continue();
                    });
        }

        /// <summary>
        /// Verifies the default stream.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="queryEntityValue">The query entity value.</param>
        /// <param name="ed">The entity descriptor.</param>
        /// <param name="continuation">The stream descriptor continuation.</param>
        private void VerifyDefaultStream(DataServiceContext dataContext, QueryStructuralValue queryEntityValue, EntityDescriptor ed, IAsyncContinuation continuation)
        {
            var expectedStreamValue = queryEntityValue.GetDefaultStreamValue();
            this.VerifyStreamDescriptorValues(expectedStreamValue, null, null, ed.StreamETag, ed.EditStreamUri, ed.ReadStreamUri);

            var expectedReadStreamUri = GetExpectedReadStreamUri(expectedStreamValue);

            Uri readStreamUri = dataContext.GetReadStreamUri(ed.Entity);
            this.Assert.AreEqual(expectedReadStreamUri, readStreamUri, "Read stream uri did not match for default stream");

            dataContext.GetReadStream(
                continuation,
                this.isAsynchronous,
                ed.Entity,
                null,
                new DataServiceRequestArgs() { },
                    response =>
                    {
                        UpdateStreamValueFromHeaders(expectedStreamValue, response);

                        this.VerifyStreamDescriptorValues(expectedStreamValue, null, null, ed.StreamETag, ed.EditStreamUri, ed.ReadStreamUri);

                        // skip this verification when using payload driven verification since we don't have the expected content for streams
                        if (!this.DataProviderSettings.UsePayloadDrivenVerification)
                        {
                            this.Assert.IsTrue(this.VerifyStreams(expectedStreamValue, response), "Failed to compare the default stream");
                        }

                        continuation.Continue();
                    });
        }

        private void VerifyStreamLink(AstoriaQueryStreamValue expected, DataServiceStreamLink actual)
        {
            this.VerifyStreamDescriptorValues(expected, actual.Name, actual.ContentType, actual.ETag, actual.EditLink, actual.SelfLink);
        }

        private void VerifyStreamDescriptorValues(AstoriaQueryStreamValue expected, string name, string contentType, string etag, Uri editLink, Uri selfLink)
        {
            if (!this.SkipStreamDescriptorValuesVerification)
            {
                string message;
                if (name == null)
                {
                    message = "default stream";
                }
                else
                {
                    message = "named stream " + name;
                }

                if (name != null)
                {
                    this.Assert.AreEqual(expected.ContentType, contentType, "Content type did not match for {0}", message);
                }

                this.Assert.AreEqual(expected.GetExpectedETag(), etag, "ETag did not match for {0}", message);
                this.Assert.AreEqual(expected.EditLink, editLink, "Edit link did not match for {0}", message);
                this.Assert.AreEqual(expected.SelfLink, selfLink, "Self link did not match for {0}", message);
            }
        }

        private void CompareClientOrEvaluationException(QueryValue expectedValue, ExpectedClientErrorBaseline clientExpectedException, Exception ex)
        {
            this.clientExceptionVerified = true;
            if (clientExpectedException != null)
            {
                this.ClientExpectedExceptionComparer.Compare(clientExpectedException, ex);
            }
            else if (ex != null)
            {
                QueryError queryError = null;
                if (expectedValue != null)
                {
                    queryError = expectedValue.EvaluationError;
                }

                this.CompareException(queryError, ex);
            }
        }
    }
}
