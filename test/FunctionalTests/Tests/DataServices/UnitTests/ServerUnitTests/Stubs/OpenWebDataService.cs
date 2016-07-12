//---------------------------------------------------------------------
// <copyright file="OpenWebDataService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.OData.Client;
    using Microsoft.OData;
    using Microsoft.OData.Service;
    using test = System.Data.Test.Astoria;
    using BindingFlags = System.Reflection.BindingFlags;

    #endregion Namespaces

    public class OpenWebDataServiceSettings<T>
    {
        public static void InitializeService(DataServiceConfiguration configuration)
        {
            if (OpenWebDataServiceHelper.EntitySetAccessRule.Value != null)
            {
                foreach (var rule in OpenWebDataServiceHelper.EntitySetAccessRule.Value)
                {
                    configuration.SetEntitySetAccessRule(rule.Key, rule.Value);
                }
            }
            else
            {
                configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
            }

            if (OpenWebDataServiceHelper.ServiceOperationAccessRule.Value != null)
            {
                foreach (var rule in OpenWebDataServiceHelper.ServiceOperationAccessRule.Value)
                {
                    configuration.SetServiceOperationAccessRule(rule.Key, rule.Value);
                }
            }
            else
            {
                configuration.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            }

            if (OpenWebDataServiceHelper.ServiceActionAccessRule.Value != null)
            {
                foreach (var rule in OpenWebDataServiceHelper.ServiceActionAccessRule.Value)
                {
                    configuration.SetServiceActionAccessRule(rule.Key, rule.Value);
                }
            }
            else
            {
                configuration.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);
            }

            if (OpenWebDataServiceHelper.EnableAccess.Value != null)
            {
                foreach (string type in OpenWebDataServiceHelper.EnableAccess.Value)
                {
                    ((DataServiceConfiguration)configuration).EnableTypeAccess(type);
                }
            }

            configuration.DisableValidationOnMetadataWrite = OpenWebDataServiceHelper.DisableValidationOnMetadataWrite;
            configuration.UseVerboseErrors = OpenWebDataServiceHelper.ForceVerboseErrors;
            configuration.EnableTypeConversion = OpenWebDataServiceHelper.EnableTypeConversion;

            if (OpenWebDataServiceHelper.MaxBatchCount.Value.HasValue)
            {
                configuration.MaxBatchCount = OpenWebDataServiceHelper.MaxBatchCount.Value.Value;
            }
            if (OpenWebDataServiceHelper.MaxChangeSetCount.Value.HasValue)
            {
                configuration.MaxChangesetCount = OpenWebDataServiceHelper.MaxChangeSetCount.Value.Value;
            }
            if (OpenWebDataServiceHelper.MaxObjectCountOnInsert.Value.HasValue)
            {
                configuration.MaxObjectCountOnInsert = OpenWebDataServiceHelper.MaxObjectCountOnInsert.Value.Value;
            }
            if (OpenWebDataServiceHelper.MaxResultsPerCollection.Value.HasValue)
            {
                configuration.MaxResultsPerCollection = OpenWebDataServiceHelper.MaxResultsPerCollection.Value.Value;
            }
            if (OpenWebDataServiceHelper.MaxExpandDepth.Value.HasValue)
            {
                configuration.MaxExpandDepth = OpenWebDataServiceHelper.MaxExpandDepth.Value.Value;
            }

            if (OpenWebDataServiceHelper.PageSizeCustomizer.Value != null)
            {
                OpenWebDataServiceHelper.PageSizeCustomizer.Value(configuration, typeof(T));
            }

            configuration.DataServiceBehavior.MaxProtocolVersion = OpenWebDataServiceHelper.MaxProtocolVersion;
            configuration.DataServiceBehavior.IncludeAssociationLinksInResponse = OpenWebDataServiceHelper.IncludeRelationshipLinksInResponse;
            configuration.DataServiceBehavior.AcceptCountRequests = OpenWebDataServiceHelper.AcceptCountRequests;
            configuration.DataServiceBehavior.AcceptAnyAllRequests = OpenWebDataServiceHelper.AcceptAnyAllRequests;
            configuration.DataServiceBehavior.AcceptProjectionRequests = OpenWebDataServiceHelper.AcceptProjectionRequests;
            configuration.DataServiceBehavior.InvokeInterceptorsOnLinkDelete = OpenWebDataServiceHelper.InvokeInterceptorsOnLinkDelete;
            configuration.DataServiceBehavior.UseMetadataKeyOrderForBuiltInProviders = OpenWebDataServiceHelper.UseMetadataKeyOrderForBuiltInProviders;
            configuration.DataServiceBehavior.AcceptSpatialLiteralsInQuery = OpenWebDataServiceHelper.AcceptSpatialLiteralsInQuery;
            configuration.DataServiceBehavior.AcceptReplaceFunctionInQuery = OpenWebDataServiceHelper.AcceptReplaceFunctionInQuery;

            configuration.DataServiceBehavior.UrlKeyDelimiter = OpenWebDataServiceHelper.GenerateKeyAsSegment
                ? DataServiceUrlKeyDelimiter.Slash
                : DataServiceUrlKeyDelimiter.Parentheses;

            //configuration.UseV4ExpandSyntax = false;

            // Invoke InitializeService on the context type
            MethodInfo methodInfo = typeof(T).GetMethod("InitializeService", BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            if (methodInfo != null && methodInfo.ReturnType == typeof(void))
            {
                methodInfo.Invoke(null, new object[] { configuration });
            }
        }
    }

    /// <summary>Use this class to open access to a web data service.</summary>
    public class OpenWebDataService<T> : DataService<T>, IServiceProvider
    {
        public static void InitializeService(DataServiceConfiguration configuration)
        {
            OpenWebDataServiceSettings<T>.InitializeService(configuration);
        }

        protected override void OnStartProcessingRequest(ProcessRequestArgs args)
        {
            if (OpenWebDataServiceHelper.ProcessRequestDelegate.Value != null)
            {
                OpenWebDataServiceHelper.ProcessRequestDelegate.Value(args);
            }
        }

        public OpenWebDataService()
        {
            this.ProcessingPipeline.ProcessingRequest += OpenWebDataServiceHelper.ProcessingRequest;
            this.ProcessingPipeline.ProcessingChangeset += OpenWebDataServiceHelper.ProcessingChangeset;
            this.ProcessingPipeline.ProcessedChangeset += OpenWebDataServiceHelper.ProcessedChangeset;
            this.ProcessingPipeline.ProcessedRequest += OpenWebDataServiceHelper.ProcessedRequest;

            if (OpenWebDataServiceHelper.ServiceConstructorCallback.Value != null)
            {
                OpenWebDataServiceHelper.ServiceConstructorCallback.Value(this);
            }

            if (OpenWebDataServiceHelper.CreateODataWriterDelegate != null)
            {
                this.ODataWriterFactory = OpenWebDataServiceHelper.CreateODataWriterDelegate;
            }
        }

        protected override T CreateDataSource()
        {
            if (OpenWebDataServiceHelper.CreateDataSourceCallBack.Value != null)
            {
                return (T)OpenWebDataServiceHelper.CreateDataSourceCallBack.Value();
            }

            return base.CreateDataSource();
        }

        #region IServiceProvider Members

        object IServiceProvider.GetService(Type serviceType)
        {
            return OpenWebDataServiceHelper.GetService<T>(serviceType);
        }

        #endregion
    }

    public static class OpenWebDataServiceHelper
    {
        /// <summary>Whether verbose errors should be turned on by default.</summary>
        public static bool ForceVerboseErrors { get; set; }

        private static test.Restorable<int?> maxBatchCount = new test.Restorable<int?>();
        public static test.Restorable<int?> MaxBatchCount { get { return maxBatchCount; } }
        private static test.Restorable<int?> maxChangeSetCount = new test.Restorable<int?>();
        public static test.Restorable<int?> MaxChangeSetCount { get { return maxChangeSetCount; } }
        private static test.Restorable<int?> maxObjectCountOnInsert = new test.Restorable<int?>();
        public static test.Restorable<int?> MaxObjectCountOnInsert { get { return maxObjectCountOnInsert; } }
        private static test.Restorable<int?> maxResultsPerCollection = new test.Restorable<int?>();
        public static test.Restorable<int?> MaxResultsPerCollection { get { return maxResultsPerCollection; } }
        private static test.Restorable<int?> maxExpandDepth = new test.Restorable<int?>();
        public static test.Restorable<int?> MaxExpandDepth { get { return maxExpandDepth; } }

        private static test.Restorable<Action<DataServiceConfiguration, Type>> pageSizeCustomer = new test.Restorable<Action<DataServiceConfiguration, Type>>();
        public static test.Restorable<Action<DataServiceConfiguration, Type>> PageSizeCustomizer { get { return pageSizeCustomer; } }

        private static test.Restorable<bool> includeRelationshipLinksInResponse = new test.Restorable<bool>();
        public static test.Restorable<bool> IncludeRelationshipLinksInResponse { get { return includeRelationshipLinksInResponse; } }

        private static test.Restorable<bool> enableFriendlyFeeds = new test.Restorable<bool>();
        public static test.Restorable<bool> EnableFriendlyFeeds { get { return enableFriendlyFeeds; } }

        private static test.Restorable<bool> enableBlobServer = new test.Restorable<bool>();
        public static test.Restorable<bool> EnableBlobServer { get { return enableBlobServer; } }

        private static test.Restorable<Dictionary<string, EntitySetRights>> entitySetAccessRule = new test.Restorable<Dictionary<string, EntitySetRights>>();
        public static test.Restorable<Dictionary<string, EntitySetRights>> EntitySetAccessRule { get { return entitySetAccessRule; } }
        private static test.Restorable<Dictionary<string, ServiceOperationRights>> serviceOperationAccessRule = new test.Restorable<Dictionary<string, ServiceOperationRights>>();
        public static test.Restorable<Dictionary<string, ServiceOperationRights>> ServiceOperationAccessRule { get { return serviceOperationAccessRule; } }
        private static test.Restorable<Dictionary<string, ServiceActionRights>> serviceActionAccessRule = new test.Restorable<Dictionary<string, ServiceActionRights>>();
        public static test.Restorable<Dictionary<string, ServiceActionRights>> ServiceActionAccessRule { get { return serviceActionAccessRule; } }
        private static test.Restorable<List<string>> enableAccess = new test.Restorable<List<string>>();
        public static test.Restorable<List<string>> EnableAccess { get { return enableAccess; } }
        private static test.Restorable<bool> disableValidationOnMetadataWrite = new test.Restorable<bool>(false);
        public static test.Restorable<bool> DisableValidationOnMetadataWrite { get { return disableValidationOnMetadataWrite; } }
        private static test.Restorable<bool> enableTypeConversion = new test.Restorable<bool>(true);
        public static test.Restorable<bool> EnableTypeConversion { get { return enableTypeConversion; } }
        private static test.Restorable<bool> invokeInterceptorsOnLinkDelete = new test.Restorable<bool>(true);
        public static test.Restorable<bool> InvokeInterceptorsOnLinkDelete { get { return invokeInterceptorsOnLinkDelete; } }
        private static test.Restorable<bool> acceptCountRequests = new test.Restorable<bool>(true);
        public static test.Restorable<bool> AcceptCountRequests { get { return acceptCountRequests; } }
        private static test.Restorable<bool> acceptAnyAllRequests = new test.Restorable<bool>(true);
        public static test.Restorable<bool> AcceptAnyAllRequests { get { return acceptAnyAllRequests; } }
        private static test.Restorable<bool> acceptProjectionRequests = new test.Restorable<bool>(true);
        public static test.Restorable<bool> AcceptProjectionRequests { get { return acceptProjectionRequests; } }
        private static test.Restorable<bool> useMetadataKeyOrderForBuiltInProviders = new test.Restorable<bool>(false);
        public static test.Restorable<bool> UseMetadataKeyOrderForBuiltInProviders { get { return useMetadataKeyOrderForBuiltInProviders; } }
        private static test.Restorable<bool> acceptSpatialLiteralsInQuery = new test.Restorable<bool>(false);
        public static test.Restorable<bool> AcceptSpatialLiteralsInQuery { get { return acceptSpatialLiteralsInQuery; } }
        private static test.Restorable<bool> acceptReplaceFunctionInQuery = new test.Restorable<bool>();
        public static test.Restorable<bool> AcceptReplaceFunctionInQuery { get { return acceptReplaceFunctionInQuery; } }
        private static test.Restorable<bool> generateKeyAsSegment = new test.Restorable<bool>();
        public static test.Restorable<bool> GenerateKeyAsSegment { get { return generateKeyAsSegment; } }

        private static test.Restorable<ODataProtocolVersion> maxProtocolVersion = new test.Restorable<ODataProtocolVersion>(ODataProtocolVersion.V4);
        public static test.Restorable<ODataProtocolVersion> MaxProtocolVersion { get { return maxProtocolVersion; } }

        private static test.Restorable<ProcessRequestCallBack> processRequestDelegate = new test.Restorable<ProcessRequestCallBack>();
        public static test.Restorable<ProcessRequestCallBack> ProcessRequestDelegate { get { return processRequestDelegate; } }

        // Query pipeline events
        private static test.Restorable<EventHandler<DataServiceProcessingPipelineEventArgs>> processingRequest = new test.Restorable<EventHandler<DataServiceProcessingPipelineEventArgs>>();
        public static test.Restorable<EventHandler<DataServiceProcessingPipelineEventArgs>> ProcessingRequest { get { return processingRequest; } }
        private static test.Restorable<EventHandler<DataServiceProcessingPipelineEventArgs>> processedRequest = new test.Restorable<EventHandler<DataServiceProcessingPipelineEventArgs>>();
        public static test.Restorable<EventHandler<DataServiceProcessingPipelineEventArgs>> ProcessedRequest { get { return processedRequest; } }
        private static test.Restorable<EventHandler<EventArgs>> processingChangeset = new test.Restorable<EventHandler<EventArgs>>();
        public static test.Restorable<EventHandler<EventArgs>> ProcessingChangeset { get { return processingChangeset; } }
        private static test.Restorable<EventHandler<EventArgs>> processedChangeset = new test.Restorable<EventHandler<EventArgs>>();
        public static test.Restorable<EventHandler<EventArgs>> ProcessedChangeset { get { return processedChangeset; } }

        /// <summary>Set to override the GetService behavior. If this returns string "null" a true null is returned.
        /// If this returns a normal null the default behavior will be used.</summary>
        private static test.Restorable<Func<Type, object>> getServiceCustomizer = new test.Restorable<Func<Type, object>>();
        public static test.Restorable<Func<Type, object>> GetServiceCustomizer { get { return getServiceCustomizer; } }

        /// <summary>Action called when the service object is constructed. The parameter is the DataService of T instance being constructed.</summary>
        private static test.Restorable<Action<object>> serviceConstructorCallback = new test.Restorable<Action<object>>();
        public static test.Restorable<Action<object>> ServiceConstructorCallback { get { return serviceConstructorCallback; } }

        /// <summary>CreateDataSource callback</summary>
        private static test.Restorable<Func<object>> createDataSourceCallback = new test.Restorable<Func<object>>();
        public static test.Restorable<Func<object>> CreateDataSourceCallBack { get { return createDataSourceCallback; } }

        /// <summary>CreateDataSource callback</summary>
        private static test.Restorable<Func<ODataWriter, DataServiceODataWriter>> createODataWriterDelegate = new test.Restorable<Func<ODataWriter, DataServiceODataWriter>>();
        public static test.Restorable<Func<ODataWriter, DataServiceODataWriter>> CreateODataWriterDelegate { get { return createODataWriterDelegate; } }

        public static object GetService<T>(Type serviceType)
        {
            if (GetServiceCustomizer.Value != null)
            {
                object result = GetServiceCustomizer.Value(serviceType);
                if (result != null)
                {
                    if ((result is string) && ((string)result == "null"))
                    {
                        return null;
                    }
                    else
                    {
                        return result;
                    }
                }
            }

            // Need to return provider instance for custom providers, to ensure
            // that they are treated as custom providers.
            if (typeof(T).GetInterface(typeof(IServiceProvider).Name) != null)
            {
                object contextInstance;
                if (typeof(T) == typeof(CustomRowBasedContext))
                {
                    contextInstance = AstoriaUnitTests.Stubs.CustomRowBasedContext.GetInstance();
                }
                else if (typeof(T) == typeof(CustomRowBasedOpenTypesContext))
                {
                    contextInstance = CustomRowBasedOpenTypesContext.GetInstance();
                }
                else
                {
                    contextInstance = typeof(T).GetConstructor(Type.EmptyTypes).Invoke(null);
                }

                return ((IServiceProvider)contextInstance).GetService(serviceType);
            }

            return null;
        }
    }

    public class OpenWebDataServiceDefinition : TestServiceDefinition
    {
        public class OpenWebDataServiceBehavior
        {
            public bool? InvokeInterceptorsOnLinkDelete { get; set; }
            public bool? AcceptCountRequests { get; set; }
            public bool? AcceptProjectionRequests { get; set; }
            public ODataProtocolVersion? MaxProtocolVersion { get; set; }
            public bool? IncludeRelationshipLinksInResponse { get; set; }
            public bool? UseMetadataKeyOrderForBuiltInProviders { get; set; }
            public bool? AcceptSpatialLiteralsInQuery { get; set; }
            public bool? SupportActionsAndFunctions { get; set; }
        }

        public class OpenWebProcessingPipeline
        {
            public EventHandler<DataServiceProcessingPipelineEventArgs> ProcessingRequest { get; set; }
            public EventHandler<DataServiceProcessingPipelineEventArgs> ProcessedRequest { get; set; }
            public EventHandler<EventArgs> ProcessingChangeset { get; set; }
            public EventHandler<EventArgs> ProcessedChangeset { get; set; }
        }

        public List<string> EnableAccess { get; set; }
        public bool? EnableTypeConversion { get; set; }
        public bool ForceVerboseErrors { get; set; }
        public bool DisableValidationOnMetadataWrite { get; set; }
        public Action<DataServiceConfiguration, Type> PageSizeCustomizer { get; set; }
        public int? MaxResultsPerCollection { get; set; }
        public OpenWebDataServiceBehavior DataServiceBehavior { get; set; }
        public OpenWebProcessingPipeline ProcessingPipeline { get; set; }
        public Dictionary<string, EntitySetRights> EntitySetAccessRule { get; set; }
        public Dictionary<string, ServiceOperationRights> ServiceOperationAccessRule { get; set; }
        public Dictionary<string, ServiceActionRights> ServiceActionAccessRule { get; set; }
        public bool EnableFriendlyFeeds { get; set; }

        /// <summary>Action called when the service object is constructed. The parameter is the DataService of T instance being constructed.</summary>
        public Action<object> ServiceConstructionCallback { get; set; }

        public OpenWebDataServiceDefinition()
        {
            this.DataServiceBehavior = new OpenWebDataServiceBehavior();
            this.ProcessingPipeline = new OpenWebProcessingPipeline();
        }

        /// <summary>Called to initialize the service on a given request.</summary>
        /// <param name="request">The request which was not yet used for the service to initialize on.</param>
        protected override void InitializeService(TestWebRequest request)
        {
            // For now we use the settings class to maintain backward compatibility.
            // Once all the tests are migrated to the new service definition behavior, we could get rid of the settings class
            //   and use the service definition Current directly.

            base.InitializeService(request);

            request.RegisterForDispose(test.TestUtil.MetadataCacheCleaner());
            ApplySetting(request, OpenWebDataServiceHelper.EnableAccess, this.EnableAccess);
            ApplySetting(request, OpenWebDataServiceHelper.DisableValidationOnMetadataWrite, this.DisableValidationOnMetadataWrite);
            if (this.EnableTypeConversion.HasValue) ApplySetting(request, OpenWebDataServiceHelper.EnableTypeConversion, this.EnableTypeConversion.Value);
            if (this.DataServiceBehavior.InvokeInterceptorsOnLinkDelete.HasValue) ApplySetting(request, OpenWebDataServiceHelper.InvokeInterceptorsOnLinkDelete, this.DataServiceBehavior.InvokeInterceptorsOnLinkDelete.Value);
            if (this.DataServiceBehavior.AcceptCountRequests.HasValue) ApplySetting(request, OpenWebDataServiceHelper.AcceptCountRequests, this.DataServiceBehavior.AcceptCountRequests.Value);
            if (this.DataServiceBehavior.AcceptProjectionRequests.HasValue) ApplySetting(request, OpenWebDataServiceHelper.AcceptProjectionRequests, this.DataServiceBehavior.AcceptProjectionRequests.Value);
            if (this.DataServiceBehavior.MaxProtocolVersion.HasValue) ApplySetting(request, OpenWebDataServiceHelper.MaxProtocolVersion, this.DataServiceBehavior.MaxProtocolVersion.Value);
            if (this.DataServiceBehavior.IncludeRelationshipLinksInResponse.HasValue) ApplySetting(request, OpenWebDataServiceHelper.IncludeRelationshipLinksInResponse, this.DataServiceBehavior.IncludeRelationshipLinksInResponse.Value);
            if (this.DataServiceBehavior.UseMetadataKeyOrderForBuiltInProviders.HasValue) ApplySetting(request, OpenWebDataServiceHelper.UseMetadataKeyOrderForBuiltInProviders, this.DataServiceBehavior.UseMetadataKeyOrderForBuiltInProviders.Value);
            if (this.DataServiceBehavior.AcceptSpatialLiteralsInQuery.HasValue) ApplySetting(request, OpenWebDataServiceHelper.AcceptSpatialLiteralsInQuery, this.DataServiceBehavior.AcceptSpatialLiteralsInQuery.Value);
            if (this.MaxResultsPerCollection.HasValue) ApplySetting(request, OpenWebDataServiceHelper.MaxResultsPerCollection, this.MaxResultsPerCollection);
            ApplySetting(request, OpenWebDataServiceHelper.ProcessingRequest, this.ProcessingPipeline.ProcessingRequest);
            ApplySetting(request, OpenWebDataServiceHelper.ProcessedRequest, this.ProcessingPipeline.ProcessedRequest);
            ApplySetting(request, OpenWebDataServiceHelper.ProcessingChangeset, this.ProcessingPipeline.ProcessingChangeset);
            ApplySetting(request, OpenWebDataServiceHelper.ProcessedChangeset, this.ProcessingPipeline.ProcessedChangeset);
            request.RegisterForDispose(System.Data.Test.Astoria.TestUtil.RestoreStaticValueOnDispose(typeof(OpenWebDataServiceHelper), "ForceVerboseErrors"));
            OpenWebDataServiceHelper.ForceVerboseErrors = this.ForceVerboseErrors;
            ApplySetting(request, OpenWebDataServiceHelper.ServiceConstructorCallback, this.ServiceConstructionCallback);
            ApplySetting(request, OpenWebDataServiceHelper.PageSizeCustomizer, this.PageSizeCustomizer);
            ApplySetting(request, OpenWebDataServiceHelper.EntitySetAccessRule, this.EntitySetAccessRule);
            ApplySetting(request, OpenWebDataServiceHelper.ServiceOperationAccessRule, this.ServiceOperationAccessRule);
            ApplySetting(request, OpenWebDataServiceHelper.EnableFriendlyFeeds, this.EnableFriendlyFeeds);
        }

        internal static void ApplySetting<T>(TestWebRequest request, test.Restorable<T> restorable, T value)
        {
            request.RegisterForDispose(restorable.Restore());
            restorable.Value = value;
        }
    }

    public delegate void ProcessRequestCallBack(ProcessRequestArgs args);
}
