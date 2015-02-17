//---------------------------------------------------------------------
// <copyright file="TableData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel.Data
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// Contents of a database table.
    /// </summary>
    [DebuggerDisplay("Table '{this.TableDefinition}' Rows: {this.Rows.Count}")]
    public class TableData
    {
        internal TableData(DatabaseData parent, Table table)
        {
            this.TableDefinition = table;
            this.Parent = parent;
            this.Rows = new List<RowData>();
        }

        /// <summary>
        /// Gets the parent <see cref="DatabaseData"/>.
        /// </summary>
        /// <value>The parent.</value>
        public DatabaseData Parent { get; private set; }

        /// <summary>
        /// Gets the definition of the table.
        /// </summary>
        /// <value>The table definition.</value>
        public Table TableDefinition { get; private set; }

        /// <summary>
        /// Gets the rows of the table.
        /// </summary>
        /// <value>The collection of rows.</value>
        public IList<RowData> Rows { get; private set; }

        /// <summary>
        /// Adds a new row to the table.
        /// </summary>
        /// <returns>Instance of <see cref="RowData"/> for the newly added row.</returns>
        public RowData AddNewRow()
        {
            var row = new RowData(this);
            this.Rows.Add(row);
            return row;
        }

        /// <summary>
        /// Imports data into the table.
        /// </summary>
        /// <typeparam name="T">Type of data item.</typeparam>
        /// <param name="data">The data to import.</param>
        public void ImportFrom<T>(T[] data)
        {
            foreach (T item in data)
            {
                RowData row = this.AddNewRow();
                row.ImportFrom(item);
            }
        }

        /// <summary>
        /// Generates a dump of the contents of the table data (suitable for tracing).
        /// </summary>
        /// <returns>Contents of the table rendered as a string.</returns>
        public string ToTraceString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.TableDefinition + ": " + this.Rows.Count + " rows");

            foreach (RowData row in this.Rows)
            {
                sb.AppendLine(row.ToTraceString());
            }

            return sb.ToString();
        }
    }
}
