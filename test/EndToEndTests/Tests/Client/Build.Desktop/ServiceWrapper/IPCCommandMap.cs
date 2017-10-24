//---------------------------------------------------------------------
// <copyright file="IPCCommandMap.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client
{
    /// <summary>
    /// Commands used by Microsoft.Test.OData.Tests.Client to communicate with the out-proc test service.
    /// </summary>
    public static class IPCCommandMap
    {
        /// <summary>
        /// Enums representing commands to pass to the test service.
        /// </summary>
        public enum ServiceCommands
        {
            StartService,
            StopService,
            Unknown
        }

        /// <summary>
        /// Enums representing options to communicate with external process.
        /// </summary>
        public enum ServiceDescriptorType
        {
            AstoriaDefaultService,
            PluggableFormatServiceDescriptor,
            TypeDefinitionServiceDescriptor,
            ModelRefServiceDescriptor,
            OperationServiceDescriptor,
            TripPinServiceDescriptor,
            ODataWCFServicePlusDescriptor,
            ODataWCFServiceDescriptor,
            AstoriaDefaultWithAccessRestrictions,
            PayloadValueConverterServiceDescriptor,
            PublicProviderEFService,
            PublicProviderHybridService,
            ActionOverloadingService,
            OpenTypesService,
            UrlModifyingService,
            ODataWriterService,
            PrimitiveKeysService,
            KeyAsSegmentService,
            AstoriaDefaultServiceModifiedClientTypes,
            PublicProviderReflectionService,
            ODataSimplifiedServiceDescriptor,
            Unknown
        }

        /// <summary>
        /// Enum to describe the type of service being spawned.
        /// </summary>
        public enum ServiceType
        {
            Default,
            WCF,
            Unknown
        }
    }
}