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

using Microsoft.Data.Edm.Expressions;

namespace Microsoft.Data.Edm.Annotations
{
    /// <summary>
    /// Represents a property binding specified as part of an EDM Type Annotation.
    /// </summary>
    public interface IEdmPropertyValueBinding
    {
        /// <summary>
        /// Gets the property given a value by the annotation.
        /// </summary>
        IEdmProperty BoundProperty { get; }

        /// <summary>
        /// Gets the expression producing the value of the annotation.
        /// </summary>
        IEdmExpression Value { get; }
    }
} 
