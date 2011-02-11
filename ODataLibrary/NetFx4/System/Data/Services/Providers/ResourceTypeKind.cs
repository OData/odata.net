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
    /// <summary>
    /// Enumeration for the kind of resource types
    /// </summary>
#if INTERNAL_DROP
    internal enum ResourceTypeKind
#else
    public enum ResourceTypeKind
#endif
    {
        /// <summary> Resource type with keys </summary>
        EntityType,

        /// <summary> Resource type without keys </summary>
        ComplexType,

        /// <summary>A resource type without keys and with no properties.</summary>
        Primitive,

        /// <summary>Resource type representing a multiValue property of primitive or complex types.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "MultiValue is a Name")]
        MultiValue,
    }
}
