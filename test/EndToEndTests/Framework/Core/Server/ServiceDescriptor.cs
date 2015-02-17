//---------------------------------------------------------------------
// <copyright file="ServiceDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Server
{
    using System;
    using Microsoft.OData.Client;

    /// <summary>
    /// Describes a test OData service.
    /// </summary>
    public class ServiceDescriptor
    {
        /// <summary>
        /// Gets or sets the type of the service.
        /// </summary>
        public Type ServiceType { get; set; }

        /// <summary>
        /// Gets or sets the delegate for creating a DataServiceContext for the service.
        /// </summary>
        public Func<Uri, DataServiceContext> CreateDataServiceContext { get; set; }

        /// <summary>
        /// Gets or sets the delegate for creating the URI for the service.
        /// </summary>
        public Func<Uri> CreateServiceUri { get; set; }
    }
}
