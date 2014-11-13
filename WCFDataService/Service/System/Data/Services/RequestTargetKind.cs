//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#if ODATALIB
namespace Microsoft.Data.OData.Query
#else
namespace System.Data.Services
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
