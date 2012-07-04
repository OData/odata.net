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

namespace Microsoft.Data.Edm.Expressions
{
    /// <summary>
    /// Represents an EDM record construction expression.
    /// </summary>
    public interface IEdmRecordExpression : IEdmExpression
    {
        /// <summary>
        /// Gets the declared type of the record, or null if there is no declared type.
        /// </summary>
        IEdmStructuredTypeReference DeclaredType { get; }

        /// <summary>
        /// Gets the constructed property values.
        /// </summary>
        IEnumerable<IEdmPropertyConstructor> Properties { get; }
    }
}
