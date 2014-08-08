//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace Microsoft.Data.Edm.Annotations
{
    /// <summary>
    /// Represents the combination of an EDM annotation with an immediate value and the element to which it is attached.
    /// </summary>
    public interface IEdmDirectValueAnnotationBinding
    {
        /// <summary>
        /// Gets the element to which the annotation is attached
        /// </summary>
        IEdmElement Element { get; }

        /// <summary>
        /// Gets the namespace URI of the annotation.
        /// </summary>
        string NamespaceUri { get; }

        /// <summary>
        /// Gets the local name of this annotation.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the value of this annotation.
        /// </summary>
        object Value { get; }
    }
}
