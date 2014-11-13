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

namespace System.Data.Services.Providers
{
    using System;
    
    /// <summary>
    /// Enumeration for the kinds of property a resource can have.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1714", Justification = "Avoid unnecesarily changing existing code")]
    [Flags]
    public enum ResourcePropertyKind
    {
        /// <summary>A primitive type property.</summary>
        Primitive = 0x1,

        /// <summary>A property that is part of the key.</summary>
        Key = 0x2,

        /// <summary>A complex (compound) property.</summary>
        ComplexType = 0x4,

        /// <summary>A reference to another resource.</summary>
        ResourceReference = 0x8,

        /// <summary>A reference to a resource set.</summary>
        ResourceSetReference = 0x10,

        /// <summary>Whether this property is a etag property.</summary>
        ETag = 0x20,

        /// <summary>A collection of primitive or complex types.</summary>
        Collection = 0x40,

        /// <summary>A Named Resource Stream</summary>
        Stream = 0x80
    }
}
