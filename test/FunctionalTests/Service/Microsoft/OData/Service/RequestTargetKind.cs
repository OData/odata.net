//---------------------------------------------------------------------
// <copyright file="RequestTargetKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CORE
namespace Microsoft.OData.UriParser
#else
namespace Microsoft.OData.Service
#endif
{
    /// <summary>
    /// Provides values to describe the kind of thing targetted by a 
    /// client request.
    /// </summary>
    internal enum RequestTargetKind
    {
        /// <summary>Nothing specific is being requested.</summary>
        Nothing,

        /// <summary>A top-level directory of service capabilities.</summary>
        ServiceDirectory,

        /// <summary>Entity Resource is requested - it can be a collection or a single value.</summary>
        Resource,

        /// <summary>A single complex value is requested (eg: an Address).</summary>
        ComplexObject,

        /// <summary>A single value is requested (eg: a Picture property).</summary>
        Primitive,

        /// <summary>A single value is requested (eg: the raw stream of a Picture).</summary>
        PrimitiveValue,

        /// <summary>System metadata.</summary>
        Metadata,

        /// <summary>A data-service-defined operation that doesn't return anything.</summary>
        VoidOperation,

        /// <summary>The request is a batch request.</summary>
        Batch,

        /// <summary>The request is a link operation - bind or unbind or simple get</summary>
        Link,

        /// <summary>An open property is requested.</summary>
        OpenProperty,

        /// <summary>An open property value is requested.</summary>
        OpenPropertyValue,

        /// <summary>A stream property value is requested.</summary>
        MediaResource,

        /// <summary>A single collection of primitive or complex values is requested.</summary>
        Collection,
    }
}
