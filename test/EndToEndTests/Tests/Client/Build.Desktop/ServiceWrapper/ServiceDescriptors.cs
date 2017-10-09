//---------------------------------------------------------------------
// <copyright file="ServiceDescriptors.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client
{
    using Microsoft.Test.OData.Services.TestServices;
    using IPC = IPCCommandMap.ServiceDescriptorType;

    /// <summary>
    /// Service Descriptors for OData Test Services. This class acts as an interface to interact with the test service.
    /// </summary>
    public static class ServiceDescriptors
    {
        private static readonly ServiceDescriptor astoriaDefaultServiceDescriptor =
            new ServiceDescriptor
            {
                IPCCommand = IPC.AstoriaDefaultService,
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("AstoriaDefault"),
                CreateDataServiceContext = (uri) => new Services.TestServices.AstoriaDefaultServiceReference.DefaultContainer(uri)
            };

        private static readonly ServiceDescriptor astoriaDefaultModifiedClientTypesServiceDescriptor =
            new ServiceDescriptor
            {
                IPCCommand = IPC.AstoriaDefaultServiceModifiedClientTypes,
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("AstoriaDefault"),
                CreateDataServiceContext = (uri) => new Services.TestServices.AstoriaDefaultServiceReferenceModifiedClientTypes.DefaultContainer(uri)
            };

        private static readonly ServiceDescriptor astoriaDefaultWithAccessRestrictionsServiceDescriptor =
            new ServiceDescriptor
            {
                IPCCommand = IPC.AstoriaDefaultWithAccessRestrictions,
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("AstoriaDefaultWithAccessRestrictions"),
                CreateDataServiceContext = (uri) => new Services.TestServices.AstoriaDefaultWithAccessRestrictionsServiceReference.DefaultContainer(uri)
            };

        private static readonly ServiceDescriptor keyAsSegmentServiceDescriptor =
            new ServiceDescriptor
            {
                IPCCommand = IPC.KeyAsSegmentService,
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("KeyAsSegment"),
                CreateDataServiceContext = (uri) => new Services.TestServices.KeyAsSegmentServiceReference.DefaultContainer(uri)
            };

        private static readonly ServiceDescriptor primitiveKeysServiceDescriptor =
            new ServiceDescriptor
            {
                IPCCommand = IPC.PrimitiveKeysService,
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("PrimitiveKeys"),
                CreateDataServiceContext = (uri) => new Services.TestServices.PrimitiveKeysServiceReference.TestContext(uri)
            };

        private static readonly ServiceDescriptor odataWriterDefaultServiceDescriptor =
            new ServiceDescriptor
            {
                IPCCommand = IPC.ODataWriterService,
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("ODataWriterDefault"),
                CreateDataServiceContext = (uri) => new Services.TestServices.ODataWriterDefaultServiceReference.DefaultContainer(uri)
            };

        private static readonly ServiceDescriptor urlModifyingServiceDescriptor =
            new ServiceDescriptor
            {
                IPCCommand = IPC.UrlModifyingService,
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("UrlModifying"),
                CreateDataServiceContext = (uri) => new Services.TestServices.AstoriaDefaultServiceReference.DefaultContainer(uri)
            };

        private static readonly ServiceDescriptor openTypesServiceDescriptor =
            new ServiceDescriptor
            {
                IPCCommand = IPC.OpenTypesService,
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("OpenTypes"),
                CreateDataServiceContext = (uri) => new Services.TestServices.OpenTypesServiceReference.DefaultContainer(uri)
            };

        private static readonly ServiceDescriptor actionOverloadingServiceDescriptor =
            new ServiceDescriptor
            {
                IPCCommand = IPC.ActionOverloadingService,
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("ActionOverloading"),
                CreateDataServiceContext = (uri) => new Services.TestServices.ActionOverloadingServiceReference.DefaultContainer(uri)
            };

        private static readonly ServiceDescriptor publicProviderHybridServiceDescriptor =
            new ServiceDescriptor
            {
                IPCCommand = IPC.PublicProviderHybridService,
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("Hybrid"),
                CreateDataServiceContext = uri => new Services.TestServices.PublicProviderHybridServiceReference.HybridService.AstoriaDefaultServiceDBEntities(uri)
            };

        private static readonly ServiceDescriptor publicProviderReflectionServiceDescriptor =
            new ServiceDescriptor
            {
                IPCCommand = IPC.PublicProviderReflectionService,
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("Reflection"),
                CreateDataServiceContext = uri => new Services.TestServices.PublicProviderReflectionServiceReference.DefaultContainer(uri)
            };

        private static readonly ServiceDescriptor publicProviderEFServiceDescriptor =
            new ServiceDescriptor
            {
                IPCCommand = IPC.PublicProviderEFService,
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("EF"),
                CreateDataServiceContext = uri => new Services.TestServices.PublicProviderEFServiceReference.Microsoft.Test.OData.Services.Astoria.AstoriaDefaultServiceDBEntities(uri)
            };

        private static readonly ServiceDescriptor oDataWCFServiceDescriptor =
            new ServiceDescriptor
            {
                IPCCommand = IPC.ODataWCFServiceDescriptor,
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("ODL"),
                CreateDataServiceContext = uri => new Services.TestServices.ODataWCFServiceReference.InMemoryEntities(uri)
            };

        private static readonly ServiceDescriptor oDataWCFServicePlusDescriptor =
            new ServiceDescriptor
            {
                IPCCommand = IPC.ODataWCFServicePlusDescriptor,
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("ODL"),
                CreateDataServiceContext = uri => new Services.TestServices.ODataWCFServiceReferencePlus.InMemoryEntitiesPlus(uri)
            };

        private static readonly ServiceDescriptor tripPinServiceDescriptor =
            new ServiceDescriptor
            {
                IPCCommand = IPC.TripPinServiceDescriptor,
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("TripPin"),
                CreateDataServiceContext = uri => new Services.TestServices.ODataWCFServiceReference.InMemoryEntities(uri) //TODO-jiajyu: update generated code
            };

        private static readonly ServiceDescriptor operationServiceDescriptor =
           new ServiceDescriptor
           {
               IPCCommand = IPC.OperationServiceDescriptor,
               CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("OperationService"),
               CreateDataServiceContext = uri => new Services.TestServices.ODataWCFServiceReference.InMemoryEntities(uri) //TODO-jiajyu: update generated code
           };

        private static readonly ServiceDescriptor modelRefServiceDescriptor =
           new ServiceDescriptor
           {
               IPCCommand = IPC.ModelRefServiceDescriptor,
               CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("ModelRef"),
               CreateDataServiceContext = uri => new Services.TestServices.ODataWCFServiceReference.InMemoryEntities(uri) //TODO-lianw: update generated code
           };

        private static readonly ServiceDescriptor typeDefinitionServiceDescriptor =
           new ServiceDescriptor
           {
               IPCCommand = IPC.TypeDefinitionServiceDescriptor,
               CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("TypeDefinition"),
               CreateDataServiceContext = uri => new Services.TestServices.ODataWCFServiceReference.InMemoryEntities(uri) //TODO-tiano: update generated code
           };

        private static readonly ServiceDescriptor pluggableFormatServiceDescriptor =
            new ServiceDescriptor
            {
                IPCCommand = IPC.PluggableFormatServiceDescriptor,
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("PluggableFormat"),
                CreateDataServiceContext = (uri) => new Services.TestServices.PluggableFormatServiceReference.PluggableFormatService(uri)
            };

        private static readonly ServiceDescriptor payloadValueConverterServiceDescriptor =
            new ServiceDescriptor
            {
                IPCCommand = IPC.PayloadValueConverterServiceDescriptor,
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("PayloadValueConverter"),
                // Share PluggableFormatService's client code.
                CreateDataServiceContext = (uri) => new Services.TestServices.PluggableFormatServiceReference.PluggableFormatService(uri)
            };

        private static readonly ServiceDescriptor odataSimplifiedServiceDescriptor =
            new ServiceDescriptor
            {
                IPCCommand = IPC.ODataSimplifiedServiceDescriptor,
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("ODataSimplifiedService"),
                CreateDataServiceContext = uri => new Services.TestServices.ODataSimplifiedServiceReference.ODataSimplifiedService(uri)
            };

        /// <summary>
        /// Gets the ServiceDescriptor for the AstoriaDefault test service.
        /// </summary>
        public static ServiceDescriptor AstoriaDefaultService
        {
            get { return astoriaDefaultServiceDescriptor; }
        }

        /// <summary>
        /// Gets the ServiceDescriptor for the AstoriaDefault test service with modified client types.
        /// </summary>
        public static ServiceDescriptor AstoriaDefaultServiceModifiedClientTypes
        {
            get { return astoriaDefaultModifiedClientTypesServiceDescriptor; }
        }

        /// <summary>
        /// Gets the ServiceDescriptor for the KeyAsSegment test service.
        /// </summary>
        public static ServiceDescriptor KeyAsSegmentService
        {
            get { return keyAsSegmentServiceDescriptor; }
        }

        /// <summary>
        /// Gets the ServiceDescriptor for the PrimitiveKeys test service.
        /// </summary>
        public static ServiceDescriptor PrimitiveKeysService
        {
            get { return primitiveKeysServiceDescriptor; }
        }

        /// <summary>
        /// Gets the ServiceDescriptor for the ODataWriter test service.
        /// </summary>
        public static ServiceDescriptor ODataWriterService
        {
            get { return odataWriterDefaultServiceDescriptor; }
        }

        /// <summary>
        /// Gets the ServiceDescriptor for the AstoriaDefault test service.
        /// </summary>
        public static ServiceDescriptor UrlModifyingService
        {
            get { return urlModifyingServiceDescriptor; }
        }

        /// <summary>
        /// Gets the ServiceDescriptor for the OpenTypes test service.
        /// </summary>
        public static ServiceDescriptor OpenTypesService
        {
            get { return openTypesServiceDescriptor; }
        }

        /// <summary>
        /// Gets the ServiceDescriptor for the ActionOverloading test service.
        /// </summary>
        public static ServiceDescriptor ActionOverloadingService
        {
            get { return actionOverloadingServiceDescriptor; }
        }

        /// <summary>
        /// Gets the ServiceDescriptor for the PublicProviderHybrid test service.
        /// </summary>
        public static ServiceDescriptor PublicProviderHybridService
        {
            get { return publicProviderHybridServiceDescriptor; }
        }

        /// <summary>
        /// Gets the ServiceDescriptor for the PublicProviderReflection test service.
        /// </summary>
        public static ServiceDescriptor PublicProviderReflectionService
        {
            get { return publicProviderReflectionServiceDescriptor; }
        }

        /// <summary>
        /// Gets the ServiceDescriptor for the PublicProviderEFService test service.
        /// </summary>
        public static ServiceDescriptor PublicProviderEFService
        {
            get { return publicProviderEFServiceDescriptor; }
        }

        /// <summary>
        /// Gets the ServiceDescriptor for the AstoriaDefaultWithAccessRestrictions test service.
        /// </summary>
        public static ServiceDescriptor AstoriaDefaultWithAccessRestrictions
        {
            get { return astoriaDefaultWithAccessRestrictionsServiceDescriptor; }
        }

        /// <summary>
        /// Gets the ServiceDescriptor for the ODataWCFService test service.
        /// </summary>
        public static ServiceDescriptor ODataWCFServiceDescriptor
        {
            get { return oDataWCFServiceDescriptor; }
        }

        /// <summary>
        /// Gets the ServiceDescriptor for the ODataWCFService test service.
        /// </summary>
        public static ServiceDescriptor ODataWCFServicePlusDescriptor
        {
            get { return oDataWCFServicePlusDescriptor; }
        }

        /// <summary>
        /// Gets the TripPinServiceDescriptor for the ODataWCFService test service.
        /// </summary>
        public static ServiceDescriptor TripPinServiceDescriptor
        {
            get { return tripPinServiceDescriptor; }
        }

        /// <summary>
        /// Gets the OperationServiceDescriptor for the ODataWCFService test service.
        /// </summary>
        public static ServiceDescriptor OperationServiceDescriptor
        {
            get { return operationServiceDescriptor; }
        }

        /// <summary>
        /// Gets the ModelRefServiceDescriptor for the ODataWCFService test service.
        /// </summary>
        public static ServiceDescriptor ModelRefServiceDescriptor
        {
            get { return modelRefServiceDescriptor; }
        }

        /// <summary>
        /// Gets the TypeDefinitionServiceDescriptor for the TypeDefinition test service.
        /// </summary>
        public static ServiceDescriptor TypeDefinitionServiceDescriptor
        {
            get { return typeDefinitionServiceDescriptor; }
        }

        /// <summary>
        /// Gets the PluggableFormatServiceDescriptor for the PluggableFormat test service.
        /// </summary>
        public static ServiceDescriptor PluggableFormatServiceDescriptor
        {
            get { return pluggableFormatServiceDescriptor; }
        }

        /// <summary>
        /// Gets the PayloadValueConverterServiceDescriptor for the PayloadValueConverter test service.
        /// </summary>
        public static ServiceDescriptor PayloadValueConverterServiceDescriptor
        {
            get { return payloadValueConverterServiceDescriptor; }
        }

        /// <summary>
        /// Gets the ODataSimplifiedServiceDescriptor for the ODataSimplified test service.
        /// </summary>
        public static ServiceDescriptor ODataSimplifiedServiceDescriptor
        {
            get { return odataSimplifiedServiceDescriptor; }
        }
    }
}