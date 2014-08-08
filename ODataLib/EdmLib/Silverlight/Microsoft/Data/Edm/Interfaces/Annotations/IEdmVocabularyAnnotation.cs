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
    /// Represents an EDM vocabulary annotation.
    /// </summary>
    public interface IEdmVocabularyAnnotation : IEdmElement
    {
        /// <summary>
        /// Gets the qualifier used to discriminate between multiple bindings of the same property or type.
        /// </summary>
        string Qualifier { get; }

        /// <summary>
        /// Gets the term bound by the annotation.
        /// </summary>
        IEdmTerm Term { get; }

        /// <summary>
        /// Gets the element the annotation applies to.
        /// </summary>
        IEdmVocabularyAnnotatable Target { get; }
    }
}
