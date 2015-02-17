//---------------------------------------------------------------------
// <copyright file="TableReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    /// <summary>
    /// Reference to a named <see cref="Table"/> or <see cref="View"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public sealed class TableReference : Table
    {
        /// <summary>
        /// Initializes a new instance of the TableReference class with given name.
        /// </summary>
        /// <param name="name">Table name.</param>
        public TableReference(string name) : this(null, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TableReference class with given schema and name.
        /// </summary>
        /// <param name="schema">Schema name.</param>
        /// <param name="name">Table name.</param>
        public TableReference(string schema, string name) : this(null, schema, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TableReference class with given catalog, schema and name.
        /// </summary>
        /// <param name="catalog">Catalog name.</param>
        /// <param name="schema">Schema name.</param>
        /// <param name="name">Table name.</param>
        public TableReference(string catalog, string schema, string name) : base(catalog, schema, name)
        {
        }
    }
}
