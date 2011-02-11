//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services.Providers
{
    #region Namespaces.
    using System;
    #endregion Namespaces.
    
    /// <summary>
    /// Enumeration for the kinds of property a resource can have.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1714", Justification = "Avoid unnecesarily changing existing code")]
    [Flags]
#if INTERNAL_DROP
    internal enum ResourcePropertyKind
#else
    public enum ResourcePropertyKind
#endif
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

        /// <summary>A multiValue of primitive or complex types.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "MultiValue is a Name")]
        MultiValue = 0x40,

        /// <summary>A Named Resource Stream</summary>
        Stream = 0x80
    }
}
