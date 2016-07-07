//---------------------------------------------------------------------
// <copyright file="RangeVariable.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// A RangeVariable, which represents an iterator variable either over a collection, either of entities or not.
    /// Exists outside of the main SemanticAST, but hooked in via a RangeVariableReferenceNode (either Non-Entity or Entity).
    /// </summary>
    public abstract class RangeVariable
    {
        /// <summary>
        /// Gets the name of the associated rangeVariable.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the type of entity referenced by this rangeVariable
        /// </summary>
        public abstract IEdmTypeReference TypeReference { get; }

        /// <summary>
        /// Gets the kind of this rangeVariable.
        /// </summary>
        public abstract int Kind { get; }
    }
}