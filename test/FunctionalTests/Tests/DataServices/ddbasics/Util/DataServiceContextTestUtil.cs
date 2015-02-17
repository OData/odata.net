//---------------------------------------------------------------------
// <copyright file="DataServiceContextTestUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.OData.Client;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Suites.Data.Test;

namespace AstoriaUnitTests
{
    public static class DataServiceContextTestUtil
    {
        /// <summary>new Type[] { typeof(Uri) };</summary>
        public static readonly Type[] TypesUri = new Type[] { typeof(Uri) };

        /// <summary>new Type[] { typeof(IAsyncResult) };</summary>
        private static readonly Type[] TypesIAsyncResult = new Type[] { typeof(IAsyncResult) };

        /// <summary>new Type[] { typeof(AsyncCallback), typeof(object) };</summary>
        private static readonly Type[] TypesAsyncCallbackObject = new Type[] { typeof(AsyncCallback), typeof(object) };

        /// <summary>new Type[] { typeof(Uri), typeof(AsyncCallback), typeof(object) };</summary>
        private static readonly Type[] TypesUriAsyncCallbackObject = new Type[] { typeof(Uri), typeof(AsyncCallback), typeof(object) };

        private static Uri LastUriRequest;

        public static DataServiceResponse SaveChanges(DataServiceContext context, SaveChangesOptions options, SaveChangesMode saveMode)
        {
            IList<EntityDescriptor> entites = (from e in context.Entities where e.State != EntityStates.Unchanged select e).ToArray();
            IList<LinkDescriptor> links = (from e in context.Links where e.State != EntityStates.Unchanged select e).ToArray();
            IList<StreamDescriptor> streams = context.Entities.SelectMany(e => e.StreamDescriptors).Where(s => s.State != EntityStates.Unchanged).ToArray();
            IList<EntityDescriptor> deletedEntities = (from e in entites where e.State == EntityStates.Deleted select e).ToArray();
            IList<LinkDescriptor> deletedLinks = (from e in links where e.State == EntityStates.Deleted select e).ToArray();

            DataServiceResponse response = null;

            switch (saveMode)
            {
                case SaveChangesMode.Synchronous:
                    response = context.SaveChanges(options);
                    break;

                case SaveChangesMode.AsyncWaitOnAsyncWaitHandle:
                    {
                        IAsyncResult async = context.BeginSaveChanges(options, null, null);
                        if (!async.CompletedSynchronously)
                        {
                            Assert.IsTrue(async.AsyncWaitHandle.WaitOne(new TimeSpan(0, 0, TestConstants.MaxTestTimeout), false), "BeginSaveChanges {0} timeout", options);
                        }

                        Assert.IsTrue(async.IsCompleted);
                        response = context.EndSaveChanges(async);
                        break;
                    }

                case SaveChangesMode.AsyncCallback:
                    {
                        SaveChangesCallback callback = new SaveChangesCallback();
                        IAsyncResult async = context.BeginSaveChanges(options, callback.CallbackMethod, new object[] { options, context });

                        Assert.IsTrue(callback.Finished.WaitOne(new TimeSpan(0, 0, TestConstants.MaxTestTimeout), false), "BeginSaveChanges {0} Asyncallback timeout", options);
                        Assert.IsTrue(async.IsCompleted);

                        if (null != callback.CallbackFailure)
                        {
                            Assert.IsNull(callback.CallbackResult, callback.CallbackFailure.ToString());
                            throw callback.CallbackFailure;
                        }

                        response = (DataServiceResponse)callback.CallbackResult;
                        break;
                    }

                default:
                    Assert.Fail("shouldn't be here");
                    break;
            }

            int entityIndex = 0;
            int linkIndex = 0;
            int streamIndex = 0;
            if (options == SaveChangesOptions.BatchWithSingleChangeset)
            {
                Assert.AreEqual<int>(response.BatchStatusCode, (int)HttpStatusCode.Accepted, "Expecting 202 as the status code for batch requests");
                Assert.IsTrue(response.BatchHeaders["Content-Type"].StartsWith("multipart/mixed; boundary=batchresponse_"), "expecting content type to be multipart mixed with a boundary value");
                Assert.IsTrue(response.IsBatchResponse, "Expecting response to be batch response");
            }
            else
            {
                Assert.AreEqual<int>(response.BatchStatusCode, -1, "expecting status code to be zero");
                Assert.IsTrue(response.BatchHeaders.Count == 0, "expecting no header information");
                Assert.IsFalse(response.IsBatchResponse, "expecting this to be non batch response");
            }

            foreach (ChangeOperationResponse changeset in response)
            {
                EntityStates state;
                bool wasDeletedState;

                if (changeset.Descriptor is EntityDescriptor)
                {
                    EntityDescriptor tor = (EntityDescriptor)changeset.Descriptor;
                    state = tor.State;
                    wasDeletedState = deletedEntities.Contains(tor);

                    // for MLE, more than one request can be sent for the same entity descriptor
                    if (entites.Count > entityIndex && Object.ReferenceEquals(entites[entityIndex].Entity, tor.Entity))
                    {
                        entityIndex++;
                    }
                    else
                    {
                        Assert.IsTrue(Object.ReferenceEquals(tor.Entity, entites[entityIndex - 1].Entity), "For MLE, it must match with the previous request");
                    }

                    Assert.IsNull(changeset.Error);
                }
                else if (changeset.Descriptor is LinkDescriptor)
                {
                    LinkDescriptor tor = (LinkDescriptor)changeset.Descriptor;
                    state = tor.State;
                    wasDeletedState = deletedLinks.Contains(tor);

                    Assert.AreSame(tor.Source, links[linkIndex].Source);
                    Assert.AreEqual(tor.SourceProperty, links[linkIndex].SourceProperty);
                    Assert.AreSame(tor.Target, links[linkIndex].Target);
                    Assert.IsNull(changeset.Error);
                    linkIndex++;
                }
                else
                {
                    Assert.IsTrue(changeset.Descriptor is StreamDescriptor, "Must be stream descriptor");
                    if (streams.Count > streamIndex && streams.Contains(changeset.Descriptor))
                    {
                        streamIndex++;
                    }

                    state = changeset.Descriptor.State;
                    wasDeletedState = false;
                }

                if (changeset.Error != null)
                {
                    Assert.AreNotEqual(EntityStates.Unchanged, state);
                }
                else
                {
                    if (wasDeletedState)
                    {
                        Assert.AreEqual(EntityStates.Detached, state);
                    }
                    else
                    {
                        Assert.AreEqual(EntityStates.Unchanged, state);
                    }
                }
            }

            Assert.AreEqual(entites.Count, entityIndex, "entities SaveChangesOptions.{0}", options);
            Assert.AreEqual(links.Count, linkIndex, "links SaveChangesOptions.{0}", options);
            Assert.AreEqual(streams.Count, streamIndex, "streams SaveChangesOptions.{0}", options);

            entites = context.Entities;
            links = context.Links;
            return response;
        }

        public static IEnumerable ExecuteQuery(DataServiceContext context, DataServiceRequest query, QueryMode queryMode)
        {
            bool isQuery = (null != (query as DataServiceQuery));

            object result = null;
            switch (queryMode)
            {
                case QueryMode.GetEnumerator: // IEnumerable.GetEnumerator
                    {
                        if (isQuery)
                        {
                            result = query;
                        }
                        else
                        {
                            goto case QueryMode.ExecuteMethod;
                        }
                        break;
                    }

                case QueryMode.ExecuteMethod: // DataServiceQuery<T>.Execute
                    {
                        if (isQuery)
                        {
                            result = UnitTestCodeGen.InvokeMethod(query.GetType(), "Execute", null, null, query, null);
                        }
                        else
                        {
                            result = UnitTestCodeGen.InvokeMethod(typeof(DataServiceContext), "Execute", TypesUri, new Type[] { query.ElementType }, context, query.RequestUri);
                        }

                        break;
                    }

                case QueryMode.AsyncExecute: // DataServiceQuery<T>.BeginExecute and wait
                    {
                        if (isQuery)
                        {
                            IAsyncResult async = (IAsyncResult)UnitTestCodeGen.InvokeMethod(query.GetType(), "BeginExecute", TypesAsyncCallbackObject, null, query, new object[] { null, null });
                            if (!async.CompletedSynchronously)
                            {
                                Assert.IsTrue(async.AsyncWaitHandle.WaitOne(new TimeSpan(0, 0, TestConstants.MaxTestTimeout), false), "BeginExecute timeout");
                            }

                            result = UnitTestCodeGen.InvokeMethod(query.GetType(), "EndExecute", TypesIAsyncResult, null, query, new object[] { async });
                        }
                        else
                        {
                            IAsyncResult async = UnitTestCodeGen.InvokeMethod<DataServiceContext, IAsyncResult>("BeginExecute", TypesUriAsyncCallbackObject, new Type[] { query.ElementType }, context, query.RequestUri, null, null);
                            if (!async.CompletedSynchronously)
                            {
                                Assert.IsTrue(async.AsyncWaitHandle.WaitOne(new TimeSpan(0, 0, TestConstants.MaxTestTimeout), false), "BeginExecute timeout");
                            }

                            result = UnitTestCodeGen.InvokeMethod(typeof(DataServiceContext), "EndExecute", TypesIAsyncResult, new Type[] { query.ElementType }, context, async);
                        }

                        break;
                    }

                case QueryMode.AsyncExecuteWithCallback: // DataServiceQuery<T>.BeginExecute with callback
                    {
                        ExecuteCallback callback = new ExecuteCallback();
                        IAsyncResult async;
                        if (isQuery)
                        {
                            async = (IAsyncResult)UnitTestCodeGen.InvokeMethod(query.GetType(), "BeginExecute", TypesAsyncCallbackObject, null, query, new object[] { (AsyncCallback)callback.CallbackMethod, new object[] { query, context } });
                        }
                        else
                        {
                            async = UnitTestCodeGen.InvokeMethod<DataServiceContext, IAsyncResult>("BeginExecute", TypesUriAsyncCallbackObject, new Type[] { query.ElementType }, context, new object[] { query.RequestUri, (AsyncCallback)callback.CallbackMethod, new object[] { query, context } });
                        }

                        Assert.IsTrue(callback.Finished.WaitOne(new TimeSpan(0, 0, TestConstants.MaxTestTimeout), false), "Asyncallback timeout");
                        Assert.IsTrue(async.IsCompleted);

                        if (null != callback.CallbackFailure)
                        {
                            Assert.IsNull(callback.CallbackResult, callback.CallbackFailure.ToString());
                            throw new Exception("failure in callback", callback.CallbackFailure);
                        }

                        result = callback.CallbackResult;
                        Assert.IsNotNull(result);
                        break;
                    }

                case QueryMode.BatchExecute: // DataServiceContext.ExecuteBatch
                    {
                        LastUriRequest = query.RequestUri;
                        int countBefore = context.Entities.Count + context.Links.Count;
                        DataServiceResponse response = context.ExecuteBatch(query);
                        int countAfter = context.Entities.Count + context.Links.Count;
                        Assert.AreEqual(countBefore, countAfter, "should not materialize during ExecuteBatch");
                        result = HandleQueryResponse(response, query, context);
                    }
                    break;

                case QueryMode.BatchAsyncExecute: // DataServiceContext.BeginExecuteBatch and wait
                    {
                        int count = context.Entities.Count + context.Links.Count;
                        LastUriRequest = query.RequestUri;
                        IAsyncResult async = context.BeginExecuteBatch(null, null, query);
                        if (!async.CompletedSynchronously)
                        {
                            Assert.IsTrue(async.AsyncWaitHandle.WaitOne(new TimeSpan(0, 0, TestConstants.MaxTestTimeout), false), "BeginExecuteBatch timeout");
                        }

                        Assert.AreEqual(count, context.Entities.Count + context.Links.Count, "should not materialize until EndExecuteBatch");
                        DataServiceResponse response = context.EndExecuteBatch(async);
                        result = HandleQueryResponse(response, query, context);
                        break;
                    }

                case QueryMode.BatchAsyncExecuteWithCallback: // DataServiceContext.BeginExecuteBatch with callback
                    {
                        ExecuteBatchCallback callback = new ExecuteBatchCallback();
                        LastUriRequest = query.RequestUri;
                        IAsyncResult async = context.BeginExecuteBatch(callback.CallbackMethod, new object[] { query, context }, query);

                        Assert.IsTrue(callback.Finished.WaitOne(new TimeSpan(0, 0, TestConstants.MaxTestTimeout), false), "Asyncallback timeout {0}", LastUriRequest);
                        Assert.IsTrue(async.IsCompleted);

                        if (null != callback.CallbackFailure)
                        {
                            Assert.IsNull(callback.CallbackResult, callback.CallbackFailure.ToString());
                            throw new Exception("failure in callback", callback.CallbackFailure);
                        }

                        result = callback.CallbackResult;
                        Assert.IsNotNull(result);
                        break;
                    }

                default:
                    Assert.Fail("shouldn't be here");
                    break;
            }

            return (IEnumerable)result;
        }

        public static T CreateEntity<T>(DataServiceContext ctx, string entitySetName, EntityStates state) where T: class, new()
        {
            DataServiceQuery<T> query = ctx.CreateQuery<T>(entitySetName).AddQueryOption("$top", 1);
            return CreateEntity<T>(ctx, entitySetName, state, query);
        }

        public static T CreateEntity<T>(DataServiceContext ctx, string entitySetName, EntityStates state, DataServiceQuery<T> query) where T : class, new()
        {
            T entity = null;

            try
            {
                switch (state)
                {
                    case EntityStates.Added:
                        entity = new T();
                        ctx.AddObject(entitySetName, entity);
                        break;

                    case EntityStates.Deleted:
                        entity = CreateEntity(ctx, entitySetName, EntityStates.Unchanged, query);
                        ctx.DeleteObject(entity);
                        break;

                    case EntityStates.Detached:
                        entity = query.Execute().Single();
                        Assert.AreEqual(MergeOption.NoTracking != ctx.MergeOption, ctx.Detach(entity));
                        break;

                    case EntityStates.Unchanged:
                        entity = query.Execute().Single();
                        if (MergeOption.NoTracking == ctx.MergeOption)
                        {
                            ctx.AttachTo(entitySetName, entity);
                        }

                        break;

                    case EntityStates.Modified:
                        entity = CreateEntity(ctx, entitySetName, EntityStates.Unchanged, query);
                        ctx.UpdateObject(entity);
                        break;

                    default:
                        Assert.Fail(String.Format("unexpected state encountered: {0}", state));
                        break;
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("{0}", ex);
            }

            return entity;
        }

        private static object HandleQueryResponse(DataServiceResponse response, DataServiceRequest query, DataServiceContext context)
        {
            if ((int)HttpStatusCode.Accepted == response.BatchStatusCode)
            {
                // Assert.IsFalse(response.HasErrors, "response.HasErrors should be false (" + DescribeErrors(response) + ")");
                QueryOperationResponse queryResponse = (QueryOperationResponse)response.First<OperationResponse>();
                if (queryResponse.Error != null)
                {
                    Assert.IsNotNull(queryResponse.Headers);
                }

                try
                {
                    Assert.AreEqual(200, queryResponse.StatusCode);
                    return queryResponse;
                }
                catch (AssertFailedException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Assert.IsTrue(queryResponse.Error != null, "expected HasErrors: {0}", e);
                    throw;
                }
            }
            else
            {
                QueryOperationResponse queryResponse = (QueryOperationResponse)response.First<OperationResponse>();
                Assert.AreEqual(200, queryResponse.StatusCode);
                Assert.IsTrue(queryResponse.Error != null);

                throw new WebException("batch query failed", WebExceptionStatus.ProtocolError);
            }
        }

        public static void CheckArgumentNull(string parameterName, Action action)
        {
            try
            {
                action();
                Assert.Fail("expected an exception, but no exception was thrown");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual(ex.ParamName, parameterName, "parameter name must match");
            }
        }

        public static void CheckArgumentEmpty(string parameterName, Action action)
        {
            try
            {
                action();
                Assert.Fail("expected an exception, but no exception was thrown");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(ex.ParamName, parameterName, "parameter name must match");
                Assert.IsTrue(ex.Message.Contains(AstoriaUnitTests.DataServicesClientResourceUtil.GetString("Util_EmptyString")), "error message should match");
            }
        }

        public static void VerifyInvalidRequest(Type exceptionType, string errorMsgId, Action action, params object[] parameters)
        {
            try
            {
                action();
                Assert.Fail("expected an exception, but no exception was thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.GetType(), exceptionType, "type of exception should be as expected");
                Assert.AreEqual(ex.Message, AstoriaUnitTests.DataServicesClientResourceUtil.GetString(errorMsgId, parameters));
            }
        }

        public static void CheckArgumentException(string errorMsgId, Action action, string paramName)
        {
            try
            {
                action();
                Assert.Fail("expected an exception, but no exception was thrown");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(ex.Message, new ArgumentException(AstoriaUnitTests.DataServicesClientResourceUtil.GetString(errorMsgId), paramName).Message);
            }
        }

        public static object GetClientModel(ODataProtocolVersion protocolVersion)
        {
            Type clientEdmModelType = typeof(DataServiceContext).Assembly.GetType("Microsoft.OData.Client.DataServiceContext+ClientEdmModelCache");
            MethodInfo clientEdmModelDotGetModel = clientEdmModelType.GetMethod("GetModel", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(ODataProtocolVersion) }, null);
            return clientEdmModelDotGetModel.Invoke(null, new object[] { protocolVersion });
        }

        public static IEdmType GetOrCreateEdmType(object clientEdmModel, Type type)
        {
            Type clientEdmModelType = typeof(DataServiceContext).Assembly.GetType("Microsoft.OData.Client.ClientEdmModel");
            MethodInfo clientEdmModelDotCreate = clientEdmModelType.GetMethod("GetOrCreateEdmType", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(Type) }, null);
            return (IEdmType)clientEdmModelDotCreate.Invoke(clientEdmModel, new object[] { type });
        }

        public static IEdmType GetOrCreateEdmType(Type type, ODataProtocolVersion protocolVersion)
        {
            object clientEdmModel = GetClientModel(protocolVersion);

            Type clientEdmModelType = typeof(DataServiceContext).Assembly.GetType("Microsoft.OData.Client.ClientEdmModel");
            MethodInfo clientEdmModelDotCreate = clientEdmModelType.GetMethod("GetOrCreateEdmType", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(Type) }, null);
            return (IEdmType)clientEdmModelDotCreate.Invoke(clientEdmModel, new object[] { type });
        }

        public static object GetClientTypeAnnotation(Type type, ODataProtocolVersion protocolVersion)
        {
            object clientEdmModel = GetClientModel(protocolVersion);
            IEdmType edmTypeInstance = DataServiceContextTestUtil.GetOrCreateEdmType(clientEdmModel, type);

            Type clientTypeUtilType = typeof(DataServiceContext).Assembly.GetType("Microsoft.OData.Client.Metadata.ClientTypeUtil");
            MethodInfo getAnnotationMethod = clientTypeUtilType.GetMethod("GetClientTypeAnnotation", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(IEdmModel), typeof(IEdmType) }, null);
            return getAnnotationMethod.Invoke(null, new object[] { clientEdmModel, edmTypeInstance });
        }

        public static void ForceClientEpmPopulation(Type type, ODataProtocolVersion protocolVersion)
        {
            object annotation = GetClientTypeAnnotation(type, protocolVersion);

            try
            {
                // this will force the mappings to load
                PropertyInfo pi = annotation.GetType().GetProperty("HasEntityPropertyMappings", BindingFlags.Instance | BindingFlags.NonPublic);
                object value = pi.GetValue(annotation, null);
            }
            catch (TargetInvocationException e)
            {
                // unwrap the reflection exception
                throw e.InnerException;
            }
        }

        private class ExecuteCallback : AsyncOperationTestCallback
        {
            public override object GetResults(IAsyncResult async)
            {
                object[] state = (object[])async.AsyncState;
                DataServiceRequest query = (DataServiceRequest)state[0];
                DataServiceContext context = (DataServiceContext)state[1];

                Assert.IsTrue(async.IsCompleted);
                if (query is DataServiceQuery)
                {
                    return UnitTestCodeGen.InvokeMethod(query.GetType(), "EndExecute", TypesIAsyncResult, null, query, new object[] { async });
                }
                else
                {
                    return UnitTestCodeGen.InvokeMethod(typeof(DataServiceContext), "EndExecute", TypesIAsyncResult, new Type[] { query.ElementType }, context, new object[] { async });
                }
            }
        }

        private class ExecuteBatchCallback : AsyncOperationTestCallback
        {
            public override object GetResults(IAsyncResult async)
            {
                object[] state = (object[])async.AsyncState;
                DataServiceRequest query = (DataServiceRequest)state[0];
                DataServiceContext context = (DataServiceContext)state[1];

                DataServiceResponse response = context.EndExecuteBatch(async);
                return HandleQueryResponse(response, query, context);
            }
        }
    }

    public enum SaveChangesMode
    {
        Synchronous = 0,
        AsyncWaitOnAsyncWaitHandle = 1,
        AsyncCallback = 2,
    }

    public enum QueryMode
    {
        GetEnumerator = 0,
        ExecuteMethod,
        AsyncExecute,
        AsyncExecuteWithCallback,
        BatchExecute,
        BatchAsyncExecute,
        BatchAsyncExecuteWithCallback
    }

    public class SaveChangesCallback : AsyncOperationTestCallback
    {
        public override object GetResults(IAsyncResult async)
        {
            object[] state = (object[])async.AsyncState;
            SaveChangesOptions option = (SaveChangesOptions)state[0];
            DataServiceContext context = (DataServiceContext)state[1];

            return context.EndSaveChanges(async);
        }
    }

    public class AsyncOperation
    {
        private Exception exception;
        private ManualResetEvent finished = new ManualResetEvent(false);

        public ManualResetEvent Finished
        {
            get { return finished; }
        }

        public Exception CallbackFailure
        {
            get { return exception; }
            protected set { exception = value; }
        }
    }

    public abstract class AsyncOperationTestCallback : AsyncOperation
    {
        private object callbackResult;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public object CallbackResult
        {
            get { return Interlocked.Exchange<object>(ref callbackResult, null); }
        }

        public abstract object GetResults(IAsyncResult async);

        public void CallbackMethod(IAsyncResult async)
        {
            try
            {
                Assert.IsTrue(async.IsCompleted);
                callbackResult = GetResults(async);
            }
            catch (AssertFailedException e)
            {
                CallbackFailure = e;
                throw;
            }
            catch (Exception e)
            {
                CallbackFailure = e;
            }
            finally
            {
                Finished.Set();
            }
        }
    }
}
