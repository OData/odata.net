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
    /// Represents an EDM anonymous function expression.
    /// </summary>
    public interface IEdmAnonymousFunctionExpression : IEdmExpression
    {
        /// <summary>
        /// Gets the parameters of the function.
        /// </summary>
        IEnumerable<IEdmFunctionParameter> Parameters { get; }

        /// <summary>
        /// Gets the function body.
        /// </summary>
        IEdmExpression Body { get; }
    }
}
