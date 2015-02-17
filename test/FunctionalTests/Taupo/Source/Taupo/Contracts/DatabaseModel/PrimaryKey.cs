//---------------------------------------------------------------------
// <copyright file="PrimaryKey.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    /// <summary>
    /// Primary key constraint on a table.
    /// </summary>
    public class PrimaryKey : ColumnsConstraintBase
    {
        /// <summary>
        /// Initializes a new instance of the PrimaryKey class 
        /// </summary>
        public PrimaryKey() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PrimaryKey class with given name.
        /// </summary>
        /// <param name="name">Constraint name</param>
        public PrimaryKey(string name) : this(null, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PrimaryKey class with given schema and name.
        /// </summary>
        /// <param name="schema">Constraint schema</param>
        /// <param name="name">Constraint name</param>
        public PrimaryKey(string schema, string name) : this(null, schema, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PrimaryKey class with given catalog, schema and name.
        /// </summary>
        /// <param name="catalog">Constraint catalog</param>
        /// <param name="schema">Constraint schema</param>
        /// <param name="name">Constraint name</param>
        public PrimaryKey(string catalog, string schema, string name)
            : base(catalog, schema, name)
        {
        }

        /// <summary>
        /// Creates <see cref="ColumnReference"/> for each passed column name and and adds it to this constraint.
        /// </summary>
        /// <param name="columnNames">Names of columns that participate in the primary key.</param>
        /// <returns>This object (useful for chaining multiple calls).</returns>
        public PrimaryKey WithKeyColumns(params string[] columnNames)
        {
            return this.WithColumns<PrimaryKey>(columnNames);
        }

        /// <summary>
        /// Adds given columns to this constraint.
        /// </summary>
        /// <param name="columns">Columns that participate in the primary key.</param>
        /// <returns>This object (useful for chaining multiple calls).</returns>
        public PrimaryKey WithKeyColumns(params Column[] columns)
        {
            return this.WithColumns<PrimaryKey>(columns);
        }
    }
}
