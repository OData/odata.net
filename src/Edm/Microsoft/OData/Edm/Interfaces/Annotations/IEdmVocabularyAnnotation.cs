//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Annotations
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
