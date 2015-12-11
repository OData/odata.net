//---------------------------------------------------------------------
// <copyright file="ServiceDescriptors.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.TestServices
{
    using Microsoft.Test.OData.Framework.Server;
#if !SILVERLIGHT && !PORTABLELIB
    using Microsoft.Test.OData.Services.ActionOverloadingService;
    using Microsoft.Test.OData.Services.KeyAsSegmentService;
    using Microsoft.Test.OData.Services.ODataWCFService;
    using Microsoft.Test.OData.Services.ODataWriterService;
    using Microsoft.Test.OData.Services.PublicProvider;
    using Microsoft.Test.OData.Services.UrlModifyingService;
    using Microsoft.Test.OData.Services.ODataOperationService;
#endif

    /// <summary>
    /// Service Descriptors for OData Test Services
    /// </summary>
    public static class ServiceDescriptors
    {
        private static readonly ServiceDescriptor astoriaDefaultServiceDescriptor =
            new ServiceDescriptor
            {
#if !SILVERLIGHT && !PORTABLELIB
                ServiceType = typeof(AstoriaDefaultService.Service),
#endif
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("AstoriaDefault"),
                CreateDataServiceContext = (uri) => new AstoriaDefaultServiceReference.DefaultContainer(uri),
            };

        private static readonly ServiceDescriptor astoriaDefaultModifiedClientTypesServiceDescriptor =
            new ServiceDescriptor
            {
#if !SILVERLIGHT && !PORTABLELIB
                ServiceType = typeof(AstoriaDefaultService.Service),
#endif
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("AstoriaDefault"),
                CreateDataServiceContext = (uri) => new AstoriaDefaultServiceReferenceModifiedClientTypes.DefaultContainer(uri),
            };

        private static readonly ServiceDescriptor astoriaDefaultWithAccessRestrictionsServiceDescriptor =
            new ServiceDescriptor
            {
#if !SILVERLIGHT && !PORTABLELIB
                ServiceType = typeof(AstoriaDefaultService.ServiceWithAccessRestrictions),
#endif
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("AstoriaDefaultWithAccessRestrictions"),
                CreateDataServiceContext = (uri) => new AstoriaDefaultWithAccessRestrictionsServiceReference.DefaultContainer(uri),
            };

        private static readonly ServiceDescriptor keyAsSegmentServiceDescriptor =
            new ServiceDescriptor
            {
#if !SILVERLIGHT && !PORTABLELIB
                ServiceType = typeof(KeyAsSegmentService),
#endif
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("KeyAsSegment"),
                CreateDataServiceContext = (uri) => new KeyAsSegmentServiceReference.DefaultContainer(uri),
            };

        private static readonly ServiceDescriptor primitiveKeysServiceDescriptor =
            new ServiceDescriptor
            {
#if !SILVERLIGHT && !PORTABLELIB
                ServiceType = typeof(PrimitiveKeysService.ReflectionService),
#endif
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("PrimitiveKeys"),
                CreateDataServiceContext = (uri) => new PrimitiveKeysServiceReference.TestContext(uri),
            };

        private static readonly ServiceDescriptor odataWriterDefaultServiceDescriptor =
            new ServiceDescriptor
            {
#if !SILVERLIGHT && !PORTABLELIB
                ServiceType = typeof(ODataWriterDefaultService),
#endif
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("ODataWriterDefault"),
                CreateDataServiceContext = (uri) => new ODataWriterDefaultServiceReference.DefaultContainer(uri),
            };

        private static readonly ServiceDescriptor urlModifyingServiceDescriptor =
            new ServiceDescriptor
            {
#if !SILVERLIGHT && !PORTABLELIB
                ServiceType = typeof(UrlModifyingService),
#endif
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("UrlModifying"),
                CreateDataServiceContext = (uri) => new AstoriaDefaultServiceReference.DefaultContainer(uri),
            };

        private static readonly ServiceDescriptor openTypesServiceDescriptor =
            new ServiceDescriptor
            {
#if !SILVERLIGHT && !PORTABLELIB
                ServiceType = typeof(OpenTypesService.OpenTypeService),
#endif
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("OpenTypes"),
                CreateDataServiceContext = (uri) => new OpenTypesServiceReference.DefaultContainer(uri),
            };

        private static readonly ServiceDescriptor actionOverloadingServiceDescriptor =
            new ServiceDescriptor
            {
#if !SILVERLIGHT && !PORTABLELIB
                ServiceType = typeof(ActionOverloadingService),
#endif
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("ActionOverloading"),
                CreateDataServiceContext = (uri) => new ActionOverloadingServiceReference.DefaultContainer(uri),
            };

        private static readonly ServiceDescriptor publicProviderHybridServiceDescriptor =
            new ServiceDescriptor
            {
#if !SILVERLIGHT && !PORTABLELIB
                ServiceType = typeof(HybridService),
#endif
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("Hybrid"),
                CreateDataServiceContext = uri => new PublicProviderHybridServiceReference.HybridService.AstoriaDefaultServiceDBEntities(uri),
            };

        private static readonly ServiceDescriptor publicProviderReflectionServiceDescriptor =
            new ServiceDescriptor
            {
#if !SILVERLIGHT && !PORTABLELIB
                ServiceType = typeof(ReflectionService),
#endif
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("Reflection"),
                CreateDataServiceContext = uri => new PublicProviderReflectionServiceReference.DefaultContainer(uri),
            };

        private static readonly ServiceDescriptor publicProviderEFServiceDescriptor =
            new ServiceDescriptor
            {
#if !SILVERLIGHT && !PORTABLELIB
                ServiceType = typeof(EFService),
#endif
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("EF"),
                CreateDataServiceContext = uri => new PublicProviderEFServiceReference.Microsoft.Test.OData.Services.Astoria.AstoriaDefaultServiceDBEntities(uri)
            };

        private static readonly ServiceDescriptor oDataWCFServiceDescriptor =
            new ServiceDescriptor
            {
#if !SILVERLIGHT && !PORTABLELIB
                ServiceType = typeof(DefaultWCFService),
#endif
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("ODL"),
                CreateDataServiceContext = uri => new ODataWCFServiceReference.InMemoryEntities(uri),
            };

        private static readonly ServiceDescriptor oDataWCFServicePlusDescriptor =
            new ServiceDescriptor
            {
#if !SILVERLIGHT && !PORTABLELIB
                ServiceType = typeof(DefaultWCFService),
#endif
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("ODL"),
                CreateDataServiceContext = uri => new ODataWCFServiceReferencePlus.InMemoryEntitiesPlus(uri),
            };

        private static readonly ServiceDescriptor tripPinServiceDescriptor =
            new ServiceDescriptor
            {
#if !SILVERLIGHT && !PORTABLELIB
                ServiceType = typeof(TripPinService),
#endif
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("TripPin"),
                CreateDataServiceContext = uri => new ODataWCFServiceReference.InMemoryEntities(uri) //TODO-jiajyu: update generated code
            };

        private static readonly ServiceDescriptor operationServiceDescriptor =
           new ServiceDescriptor
           {
#if !SILVERLIGHT && !PORTABLELIB
               ServiceType = typeof(OperationService),
#endif
               CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("OperationService"),
               CreateDataServiceContext = uri => new  ODataWCFServiceReference.InMemoryEntities(uri) //TODO-jiajyu: update generated code
           };

        private static readonly ServiceDescriptor modelRefServiceDescriptor =
           new ServiceDescriptor
           {
#if !SILVERLIGHT && !PORTABLELIB
               ServiceType = typeof(ModelRefService),
#endif
               CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("ModelRef"),
               CreateDataServiceContext = uri => new ODataWCFServiceReference.InMemoryEntities(uri) //TODO-lianw: update generated code
           };

        private static readonly ServiceDescriptor typeDefinitionServiceDescriptor =
           new ServiceDescriptor
           {
#if !SILVERLIGHT && !PORTABLELIB
               ServiceType = typeof(TypeDefinitionService),
#endif
               CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("TypeDefinition"),
               CreateDataServiceContext = uri => new ODataWCFServiceReference.InMemoryEntities(uri) //TODO-tiano: update generated code
           };

        private static readonly ServiceDescriptor pluggableFormatServiceDescriptor =
            new ServiceDescriptor
            {
#if !SILVERLIGHT && !PORTABLELIB
                ServiceType = typeof(PluggableFormat.PluggableFormatService),
#endif
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("PluggableFormat"),
                CreateDataServiceContext = (uri) => new PluggableFormatServiceReference.PluggableFormatService(uri)
            };

        private static readonly ServiceDescriptor payloadValueConverterServiceDescriptor =
            new ServiceDescriptor
            {
#if !SILVERLIGHT && !PORTABLELIB
                ServiceType = typeof(PluggableFormat.PayloadValueConverterService),
#endif
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("PayloadValueConverter"),
                // Share PluggableFormatService's client code.
                CreateDataServiceContext = (uri) => new PluggableFormatServiceReference.PluggableFormatService(uri)
            };

        private static readonly ServiceDescriptor odataSimplifiedServiceDescriptor =
            new ServiceDescriptor
            {
        #if !SILVERLIGHT && !PORTABLELIB
                ServiceType = typeof(ODataSimplifiedService),
        #endif
                CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("ODataSimplifiedService"),
                CreateDataServiceContext = uri => new ODataSimplifiedServiceReference.ODataSimplifiedService(uri)
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
