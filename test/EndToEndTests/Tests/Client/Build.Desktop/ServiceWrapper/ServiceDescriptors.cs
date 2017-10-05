//---------------------------------------------------------------------
// <copyright file="ServiceDescriptors.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.TestServices
{
    /// <summary>
    /// Service Descriptors for OData Test Services. This class acts as an interface to interact with the test service.
    /// </summary>
    public static class ServiceDescriptors
    {
        public static OData.Framework.Server.ServiceDescriptor AstoriaDefaultService { get; }
        public static OData.Framework.Server.ServiceDescriptor PluggableFormatServiceDescriptor { get; }
        public static OData.Framework.Server.ServiceDescriptor TypeDefinitionServiceDescriptor { get; }
        public static OData.Framework.Server.ServiceDescriptor ModelRefServiceDescriptor { get; }
        public static OData.Framework.Server.ServiceDescriptor OperationServiceDescriptor { get; }
        public static OData.Framework.Server.ServiceDescriptor TripPinServiceDescriptor { get; }
        public static OData.Framework.Server.ServiceDescriptor ODataWCFServicePlusDescriptor { get; }
        public static OData.Framework.Server.ServiceDescriptor ODataWCFServiceDescriptor { get; }
        public static OData.Framework.Server.ServiceDescriptor AstoriaDefaultWithAccessRestrictions { get; }
        public static OData.Framework.Server.ServiceDescriptor PayloadValueConverterServiceDescriptor { get; }
        public static OData.Framework.Server.ServiceDescriptor PublicProviderEFService { get; }
        public static OData.Framework.Server.ServiceDescriptor PublicProviderHybridService { get; }
        public static OData.Framework.Server.ServiceDescriptor ActionOverloadingService { get; }
        public static OData.Framework.Server.ServiceDescriptor OpenTypesService { get; }
        public static OData.Framework.Server.ServiceDescriptor UrlModifyingService { get; }
        public static OData.Framework.Server.ServiceDescriptor ODataWriterService { get; }
        public static OData.Framework.Server.ServiceDescriptor PrimitiveKeysService { get; }
        public static OData.Framework.Server.ServiceDescriptor KeyAsSegmentService { get; }
        public static OData.Framework.Server.ServiceDescriptor AstoriaDefaultServiceModifiedClientTypes { get; }
        public static OData.Framework.Server.ServiceDescriptor PublicProviderReflectionService { get; }
        public static OData.Framework.Server.ServiceDescriptor ODataSimplifiedServiceDescriptor { get; }
    }
}