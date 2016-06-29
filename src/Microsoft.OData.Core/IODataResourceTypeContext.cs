//---------------------------------------------------------------------
// <copyright file="IODataResourceTypeContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Interface used for substitutability, to answer basic questions regarding the type of the resource or resource set.
    /// Metadata may come from a user-provided model or from the SetSerializationInfo() method on a resource set or resource. The latter is considered the "no-model" case since only strings
    /// are provided, and there is no interconnectedness.  The goal of this interface is to provide a way to query the metadata information available on a resource or resource set without
    /// needing to know where the metadata originated from.
    /// </summary>
    internal interface IODataResourceTypeContext
    {
        /// <summary>
        /// The navigation source name of the resource set or resource.
        /// </summary>
        string NavigationSourceName { get; }

        /// <summary>
        /// The entity type name of the navigation source of the resource set or resource.
        /// </summary>
        string NavigationSourceEntityTypeName { get; }

        /// <summary>
        /// The full type name of the navigation source of the resource set or resource.
        /// </summary>
        string NavigationSourceFullTypeName { get; }

        /// <summary>
        /// The kind of the navigation source of the resource set or resource.
        /// </summary>
        EdmNavigationSourceKind NavigationSourceKind { get; }

        /// <summary>
        /// The expected entity type name of the resource.
        /// For example, in the request URI 'http://example.com/Service.svc/People/Namespace.VIP_Person', the expected entity type is Namespace.VIP_Person.
        /// (The entity set element type name in this example may be Person, and the actual entity type of a particular entity might be a type more derived than VIP_Person)
        /// </summary>
        string ExpectedResourceTypeName { get; }

        /// <summary>
        /// true if the resource is an MLE, false otherwise.
        /// </summary>
        bool IsMediaLinkEntry { get; }

        /// <summary>
        /// The flag we use to identify if the current resource is from a navigation property with collection type or not.
        /// </summary>
        bool IsFromCollection { get; }
    }
}