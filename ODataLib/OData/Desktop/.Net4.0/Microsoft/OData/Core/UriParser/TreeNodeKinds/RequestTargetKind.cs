//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.TreeNodeKinds
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
