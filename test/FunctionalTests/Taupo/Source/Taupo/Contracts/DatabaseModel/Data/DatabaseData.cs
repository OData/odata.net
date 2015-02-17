//---------------------------------------------------------------------
// <copyright file="DatabaseData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel.Data
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Data in a database.
    /// </summary>
    [DebuggerDisplay("{this.tables.Count} Tables")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class DatabaseData : IEnumerable
    {
        private IDictionary<Table, TableData> tables;

        /// <summary>
        /// Initializes a new instance of the DatabaseData class.
        /// </summary>
        /// <param name="databaseSchema">The database schema.</param>
        public DatabaseData(DatabaseSchema databaseSchema)
        {
            this.SchemaDefinition = databaseSchema;
            this.tables = new Dictionary<Table, TableData>();
        }

        /// <summary>
        /// Gets the database schema definition.
        /// </summary>
        /// <value>The schema definition.</value>
        public DatabaseSchema SchemaDefinition { get; private set; }

        /// <summary>
        /// Gets the <see cref="Microsoft.Test.Taupo.Contracts.DatabaseModel.Data.TableData"/> for the specified table.
        /// </summary>
        /// <param name="table">Table to get the data for.</param>
        /// <value>Instance of <see cref="TableData"/>.</value>
        /// <remarks>The table must be part of the schema definition.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Need exception here.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers", Justification = "Convenience accessor.")]
        public TableData this[Table table]
        {
            get
            {
                TableData td;

                if (!this.tables.TryGetValue(table, out td))
                {
                    if (!this.SchemaDefinition.Tables.Contains(table))
                    {
                        throw new TaupoArgumentException("Table '" + table + "' not found in the schema.");
                    }

                    td = new TableData(this, table);
                    this.tables.Add(table, td);
                }

                return td;
            }
        }

        /// <summary>
        /// Gets the <see cref="Microsoft.Test.Taupo.Contracts.DatabaseModel.Data.TableData"/> with the specified name.
        /// </summary>
        /// <param name="name">Name of the table.</param>
        /// <value>Instance of <see cref="TableData"/>.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Need exception here.")]
        public TableData this[string name]
        {
            get
            {
                var table = this.SchemaDefinition.Tables.SingleOrDefault(c => c.Name == name);
                if (table == null)
                {
                    throw new TaupoArgumentException("Table '" + name + "' not found in the schema.");
                }

                return this[table];
            }
        }

        /// <summary>
        /// Adds data for the specified table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="initialData">The initial data.</param>
        /// <remarks>
        /// Data in <paramref name="initialData"/> is typically an array of anonymous objects.
        /// This allows readable, in-line definition of data.
        /// </remarks>
        /// <example>
        /// data.Add("Products",
        ///    new { ID = 1, Name = "Food Product", CategoryID = -1 },
        ///    new { ID = 2, Name = "Beverages", CategoryID = -2 },
        ///    new { ID = 3, Name = "Meat", CategoryID = -3 });
        /// </example>
        public void Add(string tableName, params object[] initialData)
        {
            var table = this.SchemaDefinition.Tables.SingleOrDefault(c => c.Name == tableName);
            if (table == null)
            {
                throw new TaupoArgumentException("Table '" + tableName + "' not found in the schema.");
            }

            this.Add(table, initialData);
        }

        /// <summary>
        /// Adds data for the specified table.
        /// </summary>
        /// <typeparam name="T">Type of data to add.</typeparam>
        /// <param name="table">The table.</param>
        /// <param name="initialData">The initial data.</param>
        /// <remarks>
        /// Data in <paramref name="initialData"/> is typically an array of anonymous objects.
        /// This allows readable, in-line definition of data.
        /// </remarks>
        /// <example>
        /// data.Add("Products",
        ///    new { ID = 1, Name = "Food Product", CategoryID = -1 },
        ///    new { ID = 2, Name = "Beverages", CategoryID = -2 },
        ///    new { ID = 3, Name = "Meat", CategoryID = -3 });
        /// </example>
        public void Add<T>(Table table, params T[] initialData)
        {
            this[table].ImportFrom(initialData);
        }

        /// <summary>
        /// Generates a dump of the contents of the <see cref="DatabaseData"/> (suitable for tracing).
        /// </summary>
        /// <returns>Contents of the database rendered as a string.</returns>
        public string ToTraceString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var kvp in this.tables.OrderBy(c => c.Key.Name))
            {
                sb.Append(kvp.Value.ToTraceString());
            }

            return sb.ToString();
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
