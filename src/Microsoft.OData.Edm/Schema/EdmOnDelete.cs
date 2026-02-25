//---------------------------------------------------------------------
// <copyright file="EdmOnDelete.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a definition of an EDM OnDelete element.
    /// </summary>
    public sealed class EdmOnDelete : IEdmOnDelete
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmOnDelete"/> class.
        /// </summary>
        /// <param name="action"></param>
        public EdmOnDelete(EdmOnDeleteAction action)
        {
            Action = action;
        }

        /// <summary>
        /// Gets the action to execute on the deletion of this end of a bidirectional association.
        /// </summary>
        public EdmOnDeleteAction Action { get; }
    }
}
