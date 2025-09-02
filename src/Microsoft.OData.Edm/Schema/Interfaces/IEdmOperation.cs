//---------------------------------------------------------------------
// <copyright file="IEdmOperation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an EDM operation.
    /// </summary>
    public interface IEdmOperation : IEdmSchemaElement
    {
        /// <summary>
        /// Gets the return type of this operation.
        /// </summary>
        [Obsolete("Use 'GetReturn()' to get 'IEdmOperationReturn'. This will be dropped in the 9.x release when 'IEdmOperation' exposes 'IEdmOperationReturn' directly. See: https://github.com/OData/odata.net/issues/3085", false)]
        IEdmTypeReference ReturnType { get; }

        /// <summary>
        /// Gets the collection of parameters for this operation.
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
