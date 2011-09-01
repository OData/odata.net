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

using System.Collections.Generic;
using Microsoft.Data.Edm.Annotations;

namespace Microsoft.Data.Edm
{
    /// <summary>
    /// Represents an EDM object that can have annotations.
    /// </summary>
    public interface IEdmAnnotatable
    {
        /// <summary>
        /// Gets annotations associated with this element.
        /// </summary>
        IEnumerable<IEdmAnnotation> Annotations { get; }

        /// <summary>
        /// Sets an annotation value in an EDM element. If the value is null, no annotation is added and an existing annotation with the same name is removed.
        /// </summary>
        /// <param name="namespaceName">Namespace that the annotation belongs to.</param>
        /// <param name="localName">Name of the annotation within the namespace.</param>
        /// <param name="value">New annotation to set</param>
        void SetAnnotation(string namespaceName, string localName, object value);

        /// <summary>
        /// Retrieves an annotation value from an EDM element. Returns null if no annotation with the given name exists.
        /// </summary>
        /// <param name="namespaceName">Namespace that the annotation belongs to.</param>
        /// <param name="localName">Local name of the annotation.</param>
        /// <returns>Returns the annotation that corresponds to the provided name. Returns null if no annotation with the given name exists. </returns>
        object GetAnnotation(string namespaceName, string localName);
    }
}
