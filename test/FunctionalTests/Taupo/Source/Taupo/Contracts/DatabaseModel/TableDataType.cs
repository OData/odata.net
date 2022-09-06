//---------------------------------------------------------------------
// <copyright file="TableDataType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Table data type.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class TableDataType : DataType, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the TableDataType class.
        /// </summary>
        public TableDataType()
            : base(false)
        {
            this.Columns = new List<Column>();
            this.UniqueConstraints = new List<UniqueConstraint>();
        }

        /// <summary>
        /// Gets table columns.
        /// </summary>
        public IList<Column> Columns { get; private set; }

        /// <summary>
        /// Gets or sets primary key constraint.
        /// </summary>
        public PrimaryKey PrimaryKey { get; set; }

        /// <summary>
        /// Gets table's unique constraints.
        /// </summary>
        public IList<UniqueConstraint> UniqueConstraints { get; private set; }

        /// <summary>
        /// Adds new <see cref="Column"/> to the table.
        /// </summary>
        /// <param name="column">Column to add.</param>
        public void Add(Column column)
        {
            this.Columns.Add(column);
        }

        /// <summary>
        /// Adds new <see cref="PrimaryKey"/> to this table data type.
        /// </summary>
        /// <param name="constraint">Constraint to add.</param>
        /// <remarks>Only one <see cref="PrimaryKey"/> is allowed per table.</remarks>
        public void Add(PrimaryKey constraint)
        {
            if (this.PrimaryKey != null)
            {
                throw new TaupoInvalidOperationException("This table already has a primary key!");
            }

            this.PrimaryKey = constraint;
        }

        /// <summary>
        /// Adds a <see cref="UniqueConstraint"/> to this table data type.
        /// </summary>
        /// <param name="constraint">Constraint to add.</param>
        public void Add(UniqueConstraint constraint)
        {
            ExceptionUtilities.CheckArgumentNotNull(constraint, "constraint");

            this.UniqueConstraints.Add(constraint);
        }

        /// <summary>
        /// Gets the ordinal of the specified column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>Zero-based ordinal of the specified column.</returns>
        /// <remarks>This function throws if the column was not found.</remarks>
        public int GetColumnOrdinal(Column column)
        {
            return this.GetColumnOrdinal((c) => c == column, column.Name);
        }

        /// <summary>
        /// Tries to get the ordinal of the specified column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>true, if the column was found, false if not.</returns>
        public bool TryGetColumnOrdinal(Column column, out int ordinal)
        {
            return this.TryGetColumnOrdinal((c) => c == column, out ordinal);
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
            return this.GetColumnOrdinal((c) => c.Name == columnName, columnName);
        }

        /// <summary>
        /// Tries to get the ordinal of the specified column.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>true, if the column was found, false if not.</returns>
        public bool TryGetColumnOrdinal(string columnName, out int ordinal)
        {
            return this.TryGetColumnOrdinal((c) => c.Name == columnName, out ordinal);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);
        }

        /// <summary>
        /// Determines whether the specified <see cref="DataType"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="DataType"/> to compare with this instance.</param>
        /// <returns>
        /// A value of <c>true</c> if the specified <see cref="DataType"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(DataType other)
        {
            return object.ReferenceEquals(this, other);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.GetType().Name);
            sb.Append("(");
            sb.Append(string.Join(", ", this.Columns.Select(c => c.Name + (c.ColumnType == null ? string.Empty : " " + c.ColumnType.ToString())).ToArray()));
            sb.Append(")");

            return sb.ToString();
        }

        /// <summary>
        /// Accepts the specified visitor by calling its Visit method.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="visitor">The visitor.</param>
        /// <returns>Visitor-specific value.</returns>
        public override TValue Accept<TValue>(IDataTypeVisitor<TValue> visitor)
        {
            // TODO: remove this method from the base class
            throw new TaupoInvalidOperationException("This method is not supported.");
        }

        /// <summary>
        /// This method is not supported and throws <see cref="TaupoNotSupportedException"/>
        /// </summary>
        /// <returns>This method never returns a result.</returns>
        public IEnumerator GetEnumerator()
        {
            throw ExceptionUtilities.CreateIEnumerableNotImplementedException();
        }

        /// <summary>
        /// Determines whether the specified value is compatible with the type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A value of <c>true</c> if the value is compatible with the type; otherwise <c>false</c>.
        /// </returns>
        protected override bool IsValueCompatible(object value)
        {
            // TODO: remove this method from the base class
            throw new TaupoInvalidOperationException("This method is not supported.");
        }

        private bool TryGetColumnOrdinal(Func<Column, bool> comparer, out int ordinal)
        {
            var columns = this.Columns;
            ordinal = -1;

            for (int i = 0; i < columns.Count; ++i)
            {
                if (comparer(columns[i]))
                {
                    ordinal = i;
                }
            }

            return ordinal >= 0;
        }

        private int GetColumnOrdinal(Func<Column, bool> comparer, string columnName)
        {
            int ordinal;
            if (!this.TryGetColumnOrdinal(comparer, out ordinal))
            {
                throw new TaupoInvalidOperationException("Column '" + columnName + "' not found in " + this);
            }

            return ordinal;
        }
    }
}