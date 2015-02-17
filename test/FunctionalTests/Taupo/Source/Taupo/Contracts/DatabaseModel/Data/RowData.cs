//---------------------------------------------------------------------
// <copyright file="RowData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel.Data
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Utilities;

    /// <summary>
    /// Row of data in the table.
    /// </summary>
    [DebuggerDisplay("{ToTraceString()}")]
    public class RowData
    {
        private readonly object[] data;

        internal RowData(TableData parent)
        {
            this.Parent  = parent;
            this.data = new object[parent.TableDefinition.Columns.Count];
        }

        /// <summary>
        /// Gets the parent table for the row.
        /// </summary>
        /// <value>The parent.</value>
        public TableData Parent { get; private set; }

        /// <summary>
        /// Gets the number of columns in a row.
        /// </summary>
        public int ColumnCount
        {
            get { return this.data.Length; }
        }

        /// <summary>
        /// Gets or sets the value of the the specified column.
        /// </summary>
        /// <param name="column">Column to get or set.</param>
        /// <value>Value of the column.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers", Justification = "Convenience accessor.")]
        public object this[Column column]
        {
            get { return this.GetValue(column); }
            set { this.SetValue(column, value); }
        }

        /// <summary>
        /// Gets or sets the value of the the specified column.
        /// </summary>
        /// <param name="columnName">Name of the column to get or set.</param>
        /// <value>Value of the column.</value>
        public object this[string columnName]
        {
            get { return this.GetValue(columnName); }
            set { this.SetValue(columnName, value); }
        }

        /// <summary>
        /// Gets or sets the value of the the specified column.
        /// </summary>
        /// <param name="columnOrdinal">Ordinal of the column to get or set.</param>
        /// <value>Value of the column.</value>
        public object this[int columnOrdinal]
        {
            get { return this.GetValue(columnOrdinal); }
            set { this.SetValue(columnOrdinal, value); }
        }

        /// <summary>
        /// Gets the value of the specified column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>Column value</returns>
        public object GetValue(Column column)
        {
            return this.GetValue(this.Parent.TableDefinition.GetColumnOrdinal(column));
        }

        /// <summary>
        /// Gets the value of the specified column.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>Column value</returns>
        public object GetValue(string columnName)
        {
            return this.GetValue(this.Parent.TableDefinition.GetColumnOrdinal(columnName));
        }

        /// <summary>
        /// Gets the value of the specified column.
        /// </summary>
        /// <param name="columnOrdinal">The ordinal of the column.</param>
        /// <returns>Column value</returns>
        public object GetValue(int columnOrdinal)
        {
            return this.data[columnOrdinal];
        }

        /// <summary>
        /// Sets the value of the specified column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        public void SetValue(Column column, object value)
        {
            this.SetValue(this.Parent.TableDefinition.GetColumnOrdinal(column), value);
        }

        /// <summary>
        /// Sets the value of the specified column.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="value">The value.</param>
        public void SetValue(string columnName, object value)
        {
            this.SetValue(this.Parent.TableDefinition.GetColumnOrdinal(columnName), value);
        }

        /// <summary>
        /// Sets the value of the specified column.
        /// </summary>
        /// <param name="columnOrdinal">The column ordinal.</param>
        /// <param name="value">The value.</param>
        public void SetValue(int columnOrdinal, object value)
        {
            Column col = this.Parent.TableDefinition.Columns[columnOrdinal];
            if (col.ColumnType != null)
            {
                try
                {
                    col.ColumnType.AssertValueCompatible(value);
                }
                catch (Exception ex)
                {
                    throw new TaupoArgumentException("Error occurred when trying to set " + this.Parent.TableDefinition + "." + col.Name + " to " + ToStringConverter.ConvertObjectToString(value), ex);
                }
            }

            this.data[columnOrdinal] = value;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>New instance of <see cref="RowData"/> with the same values.</returns>
        public RowData Clone()
        {
            var result = new RowData(this.Parent);
            for (int i = 0; i < result.data.Length; ++i)
            {
                result.data[i] = this.data[i];
            }

            return result;
        }

        internal void ImportFrom(object item)
        {
            var columns = this.Parent.TableDefinition.Columns;

            for (int i = 0; i < columns.Count; ++i)
            {
                PropertyInfo propInfo = item.GetType().GetProperty(columns[i].Name);
                if (propInfo != null)
                {
                    object propValue = propInfo.GetValue(item, null);

                    this[i] = propValue;
                }
            }
        }

        internal string ToTraceString()
        {
            var sb = new StringBuilder();
            sb.Append(" {");
            string separator = " ";

            var columns = this.Parent.TableDefinition.Columns;
            for (int i = 0; i < columns.Count; ++i)
            {
                sb.Append(separator);
                sb.Append(columns[i].Name);
                sb.Append("=");
                sb.Append(ToStringConverter.ConvertObjectToString(this.GetValue(i)));
                separator = "; ";
            }

            sb.Append(" }");
            return sb.ToString();
        }
    }
}
