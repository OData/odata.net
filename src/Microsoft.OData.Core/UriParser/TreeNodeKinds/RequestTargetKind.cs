//---------------------------------------------------------------------
// <copyright file="RequestTargetKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
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

        /// <summary>Resource is requested - it can be a collection or a single value of entity type or complex type</summary>
        Resource,

        /// <summary>A single primitive property is requested (eg: a Picture property).</summary>
        Primitive,

        /// <summary>A single primitive value is requested (eg: the raw stream of a Picture).</summary>
        PrimitiveValue,

        /// <summary>A single enumeration property is requested (eg:the property value like &lt;d:ColorFlags ... &gt;SolidYellow&lt;/d:ColorFlags&gt;).</summary>
        Enum,

        /// <summary>A single enumeration value is requested (eg: the raw value like 'SolidYellow').</summary>
        EnumValue,

        /// <summary>System metadata.</summary>
        Metadata,

        /// <summary>A data-service-defined operation that doesn't return anything.</summary>
        VoidOperation,

        /// <summary>The request is a batch request.</summary>
        Batch,

        /// <summary>An unknown path or a dynamic property is requested.</summary>
        Dynamic,

        /// <summary>The value of an unknown path or a dynamic property value is requested.</summary>
        DynamicValue,

        /// <summary>A stream property value is requested.</summary>
        MediaResource,

        /// <summary>A single collection of primitive or enum values is requested.</summary>
        Collection,
    }
}
