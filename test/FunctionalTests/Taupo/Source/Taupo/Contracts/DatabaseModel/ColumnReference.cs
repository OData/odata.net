//---------------------------------------------------------------------
// <copyright file="ColumnReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    /// <summary>
    /// Represents a named column reference.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class ColumnReference : Column
    {
        /// <summary>
        /// Initializes a new instance of the ColumnReference class with a given name.
        /// </summary>
        /// <param name="name">Column name</param>
        public ColumnReference(string name)
        {
            Name = name;
        }
    }
}
