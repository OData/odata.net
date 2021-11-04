//---------------------------------------------------------------------
// <copyright file="DataServiceContextTrackingScope.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.Product;
    using Microsoft.Test.Taupo.Astoria.Contracts.Wrappers;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Wrappers;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Wrapper scope that tracks DataServiceContext.
    /// </summary>
    [ImplementationName(typeof(IDataServiceContextTrackingScope), "Default")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Coupling is temporarily allowed until further refactoring of current design.")]
    public sealed class DataServiceContextTrackingScope : IDataServiceContextTrackingScope
    {
        private static readonly string[] ContextMethodsThatSendRequests = new string[]
        {
            "SaveChanges",
            "LoadProperty",
            "Execute",
            "ExecuteBatch",
            "EndSaveChanges",
            "EndLoadProperty",
            "EndExecute",
            "EndExecuteBatch",
            "GetReadStream",
            "EndGetReadStream",
        };

        private static readonly string[] QueryMethodsThatSendRequests = new string[]
        {
            "Execute",
            "EndExecute",
            "GetEnumerator",
        };

        private Dictionary<MethodInfo, MethodInfo> trackerMethodMap = new Dictionary<MethodInfo, MethodInfo>();
        private Dictionary<DSClient.DataServiceContext, DataServiceContextData> contextDatas = new Dictionary<DSClient.DataServiceContext, DataServiceContextData>();
        private Dictionary<DSClient.DataServiceContext, GetReadStreamInfo> getReadStreamInfos = new Dictionary<DSClient.DataServiceContext, GetReadStreamInfo>();
        private Dictionary<DSClient.DataServiceContext, SaveChangesInfo> saveChangesInfos = new Dictionary<DSClient.DataServiceContext, SaveChangesInfo>();
        private Dictionary<IQueryProvider, DSClient.DataServiceContext> querysCreated = new Dictionary<IQueryProvider, DSClient.DataServiceContext>();

        private event Action OnCurrentMethodReturn;

        /// <summary>
        /// Gets or sets the XML converter.
        /// </summary>
        /// <value>The XML converter.</value>
        [InjectDependency]
        public IXmlToPayloadElementConverter XmlConverter { get; set; }

        /// <summary>
        /// Gets or sets the calculator to use for entity descriptor values
        /// </summary>
        [InjectDependency]
        public IEntityDescriptorValueCalculator EntityDescriptorCalculator { get; set; }

        /// <summary>
        /// Gets or sets the descriptor data change tracker to use
        /// </summary>
        [InjectDependency]
        public IEntityDescriptorDataChangeTracker DescriptorDataChangeTracker { get; set; }

        /// <summary>
        /// Gets or sets the type of format applier that is being used
        /// </summary>
        [InjectDependency]
        public IClientDataContextFormatApplier ClientDataContextFormatApplier { get; set; }

        /// <summary>
        /// Gets or sets the http tracker to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDataServiceContextHttpTracker HttpTracker { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use the product event for reading entities instead of the test hook
        /// </summary>
        [InjectTestParameter("UseProductEventsInsteadOfTestHook", DefaultValueDescription = "false")]
        public bool UseProductEventsInsteadOfTestHooks { get; set; }

        /// <summary>
        /// Gets the data service context data for the specified context.
        /// </summary>
        /// <param name="context">The context for which to get the data service context data.</param>
        /// <returns>
        /// <see cref="DataServiceContextData"/> that represents data for the context.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "The method does not apply to WrappedObject.")]
        public DataServiceContextData GetDataServiceContextData(WrappedDataServiceContext context)
        {
            DSClient.DataServiceContext unwrappedContext = (DSClient.DataServiceContext)context.Product;
            DataServiceContextData data;
            if (!this.contextDatas.TryGetValue(unwrappedContext, out data))
            {
                throw new TaupoInvalidOperationException("Given data service context is not tracked in this scope.");
            }

            return data;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.contextDatas.Clear();
            this.getReadStreamInfos.Clear();
        }

        /// <summary>
        /// Wraps the specified product. If product is a DataServiceContext then registeres it for tracking.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="product">The product.</param>
        /// <returns>The wrapper for the product instance.</returns>
        public TResult Wrap<TResult>(object product) where TResult : IWrappedObject
        {
            if (product == null)
            {
                return default(TResult);
            }

            Type[] argTypes = new Type[] { typeof(IWrapperScope), typeof(object) };
            Type resultType = typeof(TResult);

            ConstructorInfo ctor = resultType.GetInstanceConstructor(true, argTypes);

            ExceptionUtilities.CheckObjectNotNull(ctor, "Cannot find constructor: {0}(IWrapperScope, object)", typeof(TResult).Name);

            DSClient.DataServiceContext context = product as DSClient.DataServiceContext;

            if (context != null)
            {
                DataServiceProtocolVersion maxProtocolVersion = DataServiceProtocolVersion.Unspecified;
                maxProtocolVersion = context.MaxProtocolVersion.ToTestEnum();

                var contextData = DataServiceContextData.CreateDataServiceContextDataFromDataServiceContext(context, maxProtocolVersion);

                this.contextDatas.Add(context, contextData);
            }

            return (TResult)ctor.Invoke(new object[] { this, product });
        }

        /// <summary>
        /// Begins tracing of the call.
        /// </summary>
        /// <param name="methodInfo">The method info that is being invoked.</param>
        /// <param name="instance">The object instance.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns>
        /// Call handle which is used to correlate parameters with result values.
        /// </returns>
        /// <remarks>
        /// If the call handle is 0, the wrapper will not call TraceResult() or TraceException()
        /// </remarks>
        int IWrapperScope.BeginTraceCall(MethodBase methodInfo, object instance, object[] parameterValues)
        {
            DSClient.DataServiceContext context = instance as DSClient.DataServiceContext;
            if (context != null)
            {
                if (methodInfo.Name == "LoadProperty" || methodInfo.Name == "BeginLoadProperty")
                {
                }
                else if (methodInfo.Name == "BeginGetReadStream")
                {
                    GetReadStreamInfo info = new GetReadStreamInfo() { Entity = parameterValues[0] };
                    if (parameterValues.Length > 1)
                    {
                        info.StreamName = parameterValues[1] as string;
                    }

                    this.getReadStreamInfos[context] = info;
                }
                else if (methodInfo.Name == "SaveChanges" || methodInfo.Name == "BeginSaveChanges")
                {
                    ExceptionUtilities.CheckObjectNotNull(this.DescriptorDataChangeTracker, "Cannot track SaveChanges without descriptor change tracker");
                    ExceptionUtilities.Assert(this.DescriptorDataChangeTracker.ApplyUpdatesImmediately, "ApplyUpdatesImmediately unexpectedly false before SaveChanges call");
                    this.DescriptorDataChangeTracker.ApplyUpdatesImmediately = false;

                    var info = new SaveChangesInfo();
                    this.saveChangesInfos[context] = info;
                    info.Options = context.SaveChangesDefaultOptions.ToTestEnum();
                    if (methodInfo.Name == "BeginSaveChanges")
                    {
                        if (parameterValues.Length > 2)
                        {
                            info.Options = (SaveChangesOptions)parameterValues[0];
                        }
                    }
                    else if (parameterValues.Length > 0)
                    {
                        info.Options = (SaveChangesOptions)parameterValues[0];
                    }
                }
                else if (methodInfo.Name == "SetSaveStream")
                {
                    this.WrapSetSaveStreamArguments(methodInfo, parameterValues);
                }
            }

            return 1;
        }

        /// <summary>
        /// Traces the result of a call.
        /// </summary>
        /// <param name="callId">The call id (returned by BeginTraceCall).</param>
        /// <param name="methodInfo">The method info that is being invoked.</param>
        /// <param name="instance">The object instance.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <param name="returnValue">The return value.</param>
        void IWrapperScope.TraceResult(int callId, MethodBase methodInfo, object instance, object[] parameterValues, ref object returnValue)
        {
            DSClient.DataServiceContext context = instance as DSClient.DataServiceContext;
            DataServiceContextData data;

            if (context != null)
            {
                if (this.contextDatas.TryGetValue(context, out data))
                {
                    this.TrackDataServiceContext(data, context, methodInfo, returnValue, parameterValues);
                }

                if (methodInfo.Name == "LoadProperty" || methodInfo.Name == "EndLoadProperty")
                {
                }

                if (methodInfo.Name == "EndGetReadStream")
                {
                    this.getReadStreamInfos.Remove(context);
                }

                if (methodInfo.Name == "SaveChanges" || methodInfo.Name == "EndSaveChanges")
                {
                    this.saveChangesInfos.Remove(context);
                }

                if (methodInfo.Name == "CreateQuery")
                {
                    this.querysCreated.Add(((DSClient.DataServiceQuery)returnValue).Provider, context);
                }

                if (ContextMethodsThatSendRequests.Contains(methodInfo.Name))
                {
                    this.HttpTracker.TryCompleteCurrentRequest(context);
                }
            }

            var query = instance as DSClient.DataServiceQuery;
            if (query != null)
            {
                if (this.querysCreated.TryGetValue(query.Provider, out context))
                {
                    if (QueryMethodsThatSendRequests.Contains(methodInfo.Name))
                    {
                        this.HttpTracker.TryCompleteCurrentRequest(context);
                    }
                }
            }

            if (this.OnCurrentMethodReturn != null)
            {
                this.OnCurrentMethodReturn();
                this.OnCurrentMethodReturn = null;
            }
        }

        /// <summary>
        /// Traces the exception which occured durion a call.
        /// </summary>
        /// <param name="callId">The call id (returned by BeginTraceCall).</param>
        /// <param name="methodInfo">The method info that is being invoked.</param>
        /// <param name="instance">The object instance.</param>
        /// <param name="exception">The exception which occured.</param>
        void IWrapperScope.TraceException(int callId, MethodBase methodInfo, object instance, Exception exception)
        {
            DSClient.DataServiceContext context = instance as DSClient.DataServiceContext;
            if (context != null)
            {
                if (methodInfo.Name == "LoadProperty" || methodInfo.Name == "EndLoadProperty")
                {
                }
                else if (methodInfo.Name == "EndGetReadStream")
                {
                    this.getReadStreamInfos.Remove(context);
                }
                else if (methodInfo.Name.EndsWith("SaveChanges", StringComparison.Ordinal))
                {
                    ExceptionUtilities.CheckObjectNotNull(this.DescriptorDataChangeTracker, "Cannot track SaveChanges without descriptor change tracker");
                    ExceptionUtilities.Assert(!this.DescriptorDataChangeTracker.ApplyUpdatesImmediately, "ApplyUpdatesImmediately unexpectedly true after SaveChanges call");
                    this.DescriptorDataChangeTracker.ApplyPendingUpdates();
                    this.DescriptorDataChangeTracker.ApplyUpdatesImmediately = true;
                    this.saveChangesInfos.Remove(context);
                }

                this.HttpTracker.TryCompleteCurrentRequest(context);
            }
        }

        private void TrackDataServiceContext(DataServiceContextData data, DSClient.DataServiceContext context, MethodBase methodInfo, object returnValue, object[] parameterValues)
        {
            MethodInfo methodToTrack = methodInfo as MethodInfo;

            ExceptionUtilities.CheckObjectNotNull(methodToTrack, "Unhandled type for method info '{0}': {1}.", methodInfo.Name, methodInfo);

            if (methodToTrack.Name == "AttachTo")
            {
                string entitySetName = (string)parameterValues[0];
                object entity = parameterValues[1];
                string etag = parameterValues.Length == 3 ? (string)parameterValues[2] : null;

                ExceptionUtilities.CheckObjectNotNull(this.EntityDescriptorCalculator, "Entity descriptor value calculator dependency has not been set.");

                var identity = this.EntityDescriptorCalculator.CalculateEntityId(data, entitySetName, entity);
                var editLink = this.EntityDescriptorCalculator.CalculateEditLink(data, entitySetName, entity);

                data.TrackAttachTo(entitySetName, entity, identity, etag, editLink);
                return;
            }
            else if (methodToTrack.Name == "SaveChanges" || methodToTrack.Name == "EndSaveChanges")
            {
                ExceptionUtilities.CheckObjectNotNull(this.DescriptorDataChangeTracker, "Cannot track SaveChanges without descriptor change tracker");
                ExceptionUtilities.Assert(!this.DescriptorDataChangeTracker.ApplyUpdatesImmediately, "ApplyUpdatesImmediately unexpectedly true after SaveChanges call");
                this.DescriptorDataChangeTracker.ApplyPendingUpdates();
                this.DescriptorDataChangeTracker.ApplyUpdatesImmediately = true;

                SaveChangesInfo info;
                ExceptionUtilities.Assert(this.saveChangesInfos.TryGetValue(context, out info), "Could not find save changes info");
                var response = (DSClient.DataServiceResponse)returnValue;
                var operationResponses = response.ToList();

                data.TrackSaveChanges(info.Options, response, operationResponses, this.DescriptorDataChangeTracker);
                this.DescriptorDataChangeTracker.ApplyPendingUpdates();
                return;
            }
            else if (methodToTrack.Name == "Detach")
            {
                data.TrackDetach(parameterValues[0]);
            }
            else if (methodToTrack.Name == "DetachLink")
            {
                data.TrackDetachLink(parameterValues[0], (string)parameterValues[1], parameterValues[2]);
            }
            else if (methodToTrack.Name == "SetSaveStream")
            {
                this.TrackSetSaveStream(data, methodToTrack, parameterValues);
            }
            else if (methodToTrack.Name == "GetReadStream" || methodToTrack.Name == "EndGetReadStream")
            {
                object entity = null;
                string name = null;
                if (methodToTrack.Name == "GetReadStream")
                {
                    entity = parameterValues[0];

                    // if the second arg is not present or not a string, then it is an MR, in which case null is correct
                    if (parameterValues.Length > 2)
                    {
                        name = parameterValues[1] as string;
                    }
                }
                else
                {
                    GetReadStreamInfo info;
                    ExceptionUtilities.Assert(this.getReadStreamInfos.TryGetValue(context, out info), "Get read stream info missing");
                    entity = info.Entity;
                    name = info.StreamName;
                }

                var response = (DSClient.DataServiceStreamResponse)returnValue;
                data.TrackGetReadStream(entity, name, response.ContentType, response.Headers);
            }
            else if (methodToTrack.Name.StartsWith("set_", StringComparison.Ordinal))
            {
                string propertyName = methodToTrack.Name.Substring(4);
                var testProperty = typeof(DataServiceContextData).GetProperty(propertyName, true, false);
                if (testProperty != null)
                {
                    testProperty.SetValue(data, parameterValues[0], null);
                }
            }
            else
            {
                MethodInfo trackerMethod = this.GetTrackingMethod(methodToTrack);

                if (trackerMethod != null)
                {
                    object[] trakerParameters = new object[trackerMethod.GetParameters().Length];
                    trakerParameters[0] = data;
                    for (int i = 0; i < parameterValues.Length; i++)
                    {
                        trakerParameters[i + 1] = parameterValues[i];
                    }

                    trackerMethod.Invoke(null, trakerParameters);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose", Justification = "Will be disposed by the product later")]
        private void WrapSetSaveStreamArguments(MethodBase methodToTrack, object[] parameterValues)
        {
            int streamParameterIndex;
            if (methodToTrack.GetParameters()[1].Name == "name")
            {
                streamParameterIndex = 2;
            }
            else
            {
                streamParameterIndex = 1;
            }

            var stream = (Stream)parameterValues[streamParameterIndex];
            if (stream != null)
            {
                var streamProxy = new LoggingStreamProxy(stream, false);
                parameterValues[streamParameterIndex] = streamProxy;
            }
        }

        private void TrackSetSaveStream(DataServiceContextData data, MethodBase methodToTrack, object[] parameterValues)
        {
            string name;
            int streamParameterIndex;
            bool closeStream;
            IEnumerable<KeyValuePair<string, string>> headers;

            // if the second arg is not a string, then it is an MR, in which case null is correct
            if (methodToTrack.GetParameters()[1].Name == "name")
            {
                name = (string)parameterValues[1];
                streamParameterIndex = 2;
                closeStream = (bool)parameterValues[3];
                var args = parameterValues[4] as DSClient.DataServiceRequestArgs;
                if (args == null)
                {
                    headers = new HttpHeaderCollection() { ContentType = (string)parameterValues[4] };
                }
                else
                {
                    headers = args.Headers;
                }
            }
            else
            {
                name = null;
                streamParameterIndex = 1;
                closeStream = (bool)parameterValues[2];
                var args = parameterValues[3] as DSClient.DataServiceRequestArgs;
                if (args == null)
                {
                    headers = new HttpHeaderCollection() { ContentType = (string)parameterValues[3], Slug = (string)parameterValues[4] };
                }
                else
                {
                    headers = args.Headers;
                }
            }

            var streamProxy = parameterValues[streamParameterIndex] as IStreamLogger;
            ExceptionUtilities.CheckObjectNotNull(streamProxy, "Stream was not wrapped by a stream logger");
            data.TrackSetSaveStream(parameterValues[0], name, streamProxy, closeStream, headers);
        }

        private MethodInfo GetTrackingMethod(MethodInfo methodToTrack)
        {
            MethodInfo result;
            if (!this.trackerMethodMap.TryGetValue(methodToTrack, out result))
            {
                int parametersCount = methodToTrack.GetParameters().Length;
                int trackerParametersCount = 1 + (methodToTrack.ReturnType.FullName != "System.Void" ? parametersCount + 1 : parametersCount);
                result = typeof(DataServiceContextTracker).GetMethods(true, true).Where(m => m.Name == "Track" + methodToTrack.Name && m.GetParameters().Length == trackerParametersCount).SingleOrDefault();

                this.trackerMethodMap.Add(methodToTrack, result);
            }

            return result;
        }

        /// <summary>
        /// Holds information about BeginGetReadStream calls
        /// </summary>
        private class GetReadStreamInfo
        {
            /// <summary>
            /// Gets or sets the entity
            /// </summary>
            public object Entity { get; set; }

            /// <summary>
            /// Gets or sets the stream name
            /// </summary>
            public string StreamName { get; set; }
        }

        /// <summary>
        /// Holds infomation about BeginSaveChanges calls
        /// </summary>
        private class SaveChangesInfo
        {
            /// <summary>
            /// Gets or sets the save changes options
            /// </summary>
            public SaveChangesOptions Options { get; set; }
        }
    }
}
