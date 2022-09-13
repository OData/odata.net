//---------------------------------------------------------------------
// <copyright file="ODataWriterServiceUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.TestServices
{
    using Microsoft.OData.Service;
    using Microsoft.Test.OData.Framework.Server;
    using Microsoft.Test.OData.Services.ODataWriterService;

    /// <summary>
    /// Static helper methods for interacting with ODataWriter test services.
    /// </summary>
    public static class ODataWriterServiceUtil
    {
        /// <summary>
        /// Generates a service descriptor for an ODataWriterService based service.
        /// </summary>
        /// <typeparam name="TODataWriter">The ODataWriter type for the service.</typeparam>
        /// <returns>A ServiceDescriptor for the specified service.</returns>
        public static ServiceDescriptor CreateODataWriterServiceDescriptor<TODataWriter>() where TODataWriter : DataServiceODataWriter
        {
            return new ServiceDescriptor
                   {
                       ServiceType = typeof(ODataWriterServiceBase<TODataWriter>),
                       CreateServiceUri = () => TestServiceUtil.GenerateServiceUri("ODataWriter_" + typeof(TODataWriter).Name),
                       CreateDataServiceContext = (uri) => new AstoriaDefaultServiceReference.DefaultContainer(uri),
                   };
        }
    }
}
