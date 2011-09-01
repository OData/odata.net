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

namespace Microsoft.Data.Edm.Annotations
{
    /// <summary>
    /// Defines EDM annotation kinds.
    /// </summary>
    public enum EdmAnnotationKind
    {
        /// <summary>
        /// Represents an annotation with unknown or error kind.
        /// </summary>
        None,
        /// <summary>
        /// Represents an annotation implementing <see cref="IEdmImmediateValueAnnotation"/>.
        /// </summary>
        ImmediateValue,
        /// <summary>
        /// Represents an annotation implementing <see cref="IEdmTypeAnnotation"/>.
        /// </summary>
        TermType,
        /// <summary>
        /// Represents an annotation implementing <see cref="IEdmValueAnnotation"/>.
        /// </summary>
        TermValue
    }

    /// <summary>
    /// Represents an EDM annotation.
    /// </summary>
    public interface IEdmAnnotation
    {
        /// <summary>
        /// Gets the kind of an annotation.
        /// </summary>
        EdmAnnotationKind Kind { get; }

        /// <summary>
        /// Gets the term bound by the annotation.
        /// </summary>
        IEdmTerm Term { get; }
    }
}
