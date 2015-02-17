//---------------------------------------------------------------------
// <copyright file="ColumnsConstraintBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Base class for columns constraint.
    /// </summary>
    public abstract class ColumnsConstraintBase : SchemaObject
    {
        /// <summary>
        /// Initializes a new instance of the ColumnsConstraintBase class with given catalog, schema and name.
        /// </summary>
        /// <param name="catalog">Constraint catalog.</param>
        /// <param name="schema">Constraint schema.</param>
        /// <param name="name">Constraint name.</param>
        protected ColumnsConstraintBase(string catalog, string schema, string name)
            : base(catalog, schema, name)
        {
            this.Columns = new List<Column>();
        }

        /// <summary>
        /// Gets columns that are part of this constraint.
        /// </summary>
        public IList<Column> Columns { get; private set; }

        /// <summary>
        /// Adds the specified column.
        /// </summary>
        /// <param name="column">The column to add.</param>
        public void Add(Column column)
        {
            this.Columns.Add(column);
        }

        /// <summary>
        /// Creates <see cref="ColumnReference"/> for each passed column name and adds them to
        /// <see cref="Columns"/>.
        /// </summary>
        /// <typeparam name="TConstraint">The type of the constraint.</typeparam>
        /// <param name="columnNames">Names of columns that participate in the constraint.</param>
        /// <returns>This object (useful for chaining multiple calls).</returns>
        protected TConstraint WithColumns<TConstraint>(params string[] columnNames) where TConstraint : ColumnsConstraintBase
        {
            ExceptionUtilities.CheckArgumentNotNull(columnNames, "columnNames");
            return this.WithColumns<TConstraint>(columnNames.Select(c => new ColumnReference(c)).ToArray());
        }

        /// <summary>
        /// Adds given columns to <see cref="Columns"/> collection.
        /// </summary>
        /// <typeparam name="TConstraint">The type of the constraint.</typeparam>
        /// <param name="columns">Columns that participate in the constraint.</param>
        /// <returns>This object (useful for chaining multiple calls).</returns>
        protected TConstraint WithColumns<TConstraint>(params Column[] columns) where TConstraint : ColumnsConstraintBase
        {
            ExceptionUtilities.CheckCollectionDoesNotContainNulls(columns, "columns");
            ExceptionUtilities.Assert(this.Columns.Count == 0, "This method can only be called on a constraint that doesn't have any columns defined.");

            foreach (Column c in columns)
            {
                this.Add(c);
            }

            return (TConstraint)this;
        }
    }
}