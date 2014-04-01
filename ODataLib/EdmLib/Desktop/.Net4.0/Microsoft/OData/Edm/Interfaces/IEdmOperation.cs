//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm
{
    using System.Collections.Generic;
    using Microsoft.OData.Edm.Expressions;

    /// <summary>
    /// Represents an EDM operation.
    /// </summary>
    public interface IEdmOperation : IEdmSchemaElement
    {
        /// <summary>
        /// Gets the return type of this function.
        /// </summary>
        IEdmTypeReference ReturnType { get; }

        /// <summary>
        /// Gets the collection of parameters for this function.
        /// </summary>
        IEnumerable<IEdmOperationParameter> Parameters { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is bound.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is bound; otherwise, <c>false</c>.
        /// </value>
        bool IsBound { get; }

        /// <summary>
        /// Gets the entity set path expression.
        /// </summary>
        /// <value>
        /// The entity set path expression.
        /// </value>
        IEdmPathExpression EntitySetPath { get; }

        /// <summary>
        /// Searches for a parameter with the given name, and returns null if no such parameter exists.
        /// </summary>
        /// <param name="name">The name of the parameter being found.</param>
        /// <returns>The requested parameter or null if no such parameter exists.</returns>
        IEdmOperationParameter FindParameter(string name);
    }
}
