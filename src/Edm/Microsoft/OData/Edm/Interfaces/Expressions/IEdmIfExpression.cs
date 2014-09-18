//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.OData.Edm.Expressions
{
    /// <summary>
    /// Represents an EDM if expression.
    /// </summary>
    public interface IEdmIfExpression : IEdmExpression
    {
        /// <summary>
        /// Gets the test expression.
        /// </summary>
        IEdmExpression TestExpression { get; }

        /// <summary>
        /// Gets the expression to evaluate if <see cref="TestExpression"/> evaluates to true.
        /// </summary>
        IEdmExpression TrueExpression { get; }

        /// <summary>
        /// Gets the expression to evaluate if <see cref="TestExpression"/> evaluates to false.
        /// </summary>
        IEdmExpression FalseExpression { get; }
    }
}
