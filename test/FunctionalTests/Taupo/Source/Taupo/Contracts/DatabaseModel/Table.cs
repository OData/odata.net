//---------------------------------------------------------------------
// <copyright file="Table.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Database table or view.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class Table : SchemaObject, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the Table class without a name.
        /// </summary>
        public Table() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Table class with a name.
        /// </summary>
        /// <param name="name">Table name</param>
        public Table(string name) : this(null, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Table class with schema and name.
        /// </summary>
        /// <param name="schema">Table schema</param>
        /// <param name="name">Table name</param>
        public Table(string schema, string name)
            : this(null, schema, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Table class with catalog, schema and name.
        /// </summary>
        /// <param name="catalog">Table catalog</param>
        /// <param name="schema">Table schema</param>
        /// <param name="name">Table name</param>
        public Table(string catalog, string schema, string name)
            : base(catalog, schema, name)
        {
            this.TableType = new TableDataType();
            this.ForeignKeys = new List<ForeignKey>();
        }

        /// <summary>
        /// Gets table definition.
        /// </summary>
        public TableDataType TableType { get; private set; }

        /// <summary>
        /// Gets table columns.
        /// </summary>
        public IList<Column> Columns
        {
            get { return this.TableType.Columns; }
        }

        /// <summary>
        /// Gets or sets primary key constraint.
        /// </summary>
        public PrimaryKey PrimaryKey
        {
            get { return this.TableType.PrimaryKey; }
            set { this.TableType.PrimaryKey = value; }
        }

        /// <summary>
        /// Gets unique constraint.
        /// </summary>
        public IList<UniqueConstraint> UniqueConstraints
        {
            get { return this.TableType.UniqueConstraints; }
        }

        /// <summary>
        /// Gets foreign key constraints.
        /// </summary>
        public IList<ForeignKey> ForeignKeys { get; private set; }

        /// <summary>
        /// Adds new <see cref="Column"/> to the table.
        /// </summary>
        /// <param name="column">Column to add.</param>
        public void Add(Column column)
        {
            this.Columns.Add(column);
        }

        /// <summary>
        /// Adds new <see cref="PrimaryKey"/> to the <see cref="Table"/>.
        /// </summary>
        /// <param name="constraint">Constraint to add</param>
        /// <remarks>Only one <see cref="PrimaryKey"/> is allowed per table.</remarks>
        public void Add(PrimaryKey constraint)
        {
            this.TableType.Add(constraint);
        }

        /// <summary>
        /// Adds new <see cref="ForeignKey"/> to <see cref="ForeignKeys"/> collection.
        /// </summary>
        /// <param name="constraint">Constraint to add.</param>
        public void Add(ForeignKey constraint)
        {
            this.ForeignKeys.Add(constraint);
        }

        /// <summary>
        /// Adds a <see cref="UniqueConstraint"/> to the <see cref="Table"/>.
        /// </summary>
        /// <param name="constraint">Constraint to add.</param>
        public void Add(UniqueConstraint constraint)
        {
            this.TableType.Add(constraint);
        }

        /// <summary>
        /// Gets the ordinal of the specified column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>Zero-based ordinal of the specified column.</returns>
        /// <remarks>This function throws if the column was not found.</remarks>
        public int GetColumnOrdinal(Column column)
        {
            int ordinal;
            if (!this.TableType.TryGetColumnOrdinal(column, out ordinal))
            {
                throw new TaupoInvalidOperationException("Column '" + column.Name + "' not found in " + this);
            }

            return ordinal;
        }

        /// <summary>
        /// Gets the ordinal of the specified column.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>
        /// Zero-based ordinal of the specified column.
        /// </returns>
        /// <remarks>This function throws if the column was not found.</remarks>
        public int GetColumnOrdinal(string columnName)
        {
            int ordinal;
            if (!this.TableType.TryGetColumnOrdinal(columnName, out ordinal))
            {
                throw new TaupoInvalidOperationException("Column '" + columnName + "' not found in " + this);
            }

            return ordinal;
        }

        /// <summary>
        /// This method is not supported and throws <see cref="TaupoNotSupportedException"/>
        /// </summary>
        /// <returns>This method never returns a result.</returns>
        public IEnumerator GetEnumerator()
        {
            throw ExceptionUtilities.CreateIEnumerableNotImplementedException();
        }
    }
}
