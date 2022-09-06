//---------------------------------------------------------------------
// <copyright file="ForeignKey.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Foreign key constraint.
    /// </summary>
    public class ForeignKey : SchemaObject
    {
        /// <summary>
        /// Initializes a new instance of the ForeignKey class.
        /// </summary>
        public ForeignKey() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ForeignKey class with given name.
        /// </summary>
        /// <param name="name">Constraint name</param>
        public ForeignKey(string name)
            : this(null, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ForeignKey class with given schema and name.
        /// </summary>
        /// <param name="schema">Constraint schema</param>
        /// <param name="name">Constraint name</param>
        public ForeignKey(string schema, string name)
            : this(null, schema, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ForeignKey class with given catalog, schema and name.
        /// </summary>
        /// <param name="catalog">Constraint catalog</param>
        /// <param name="schema">Constraint schema</param>
        /// <param name="name">Constraint name</param>
        public ForeignKey(string catalog, string schema, string name)
            : base(catalog, schema, name)
        {
            this.SourceColumns = new List<Column>();
            this.TargetColumns = new List<Column>();
        }

        /// <summary>
        /// Gets or sets target table.
        /// </summary>
        public Table Target { get; set; }

        /// <summary>
        /// Gets a foreign key columns from the source table.
        /// </summary>
        public IList<Column> SourceColumns { get; private set; }

        /// <summary>
        /// Gets a primary key columns from the <see cref="Target"/> table.
        /// </summary>
        public IList<Column> TargetColumns { get; private set; }

        /// <summary>
        /// Gets the delete action on the <see cref="TargetColumns"/> collection
        /// </summary>
        public DeleteAction DeleteAction { get; private set; }

        /// <summary>
        /// Adds given column to <see cref="SourceColumns"/> collection.
        /// </summary>
        /// <param name="sourceColumnName">Name of the source column.</param>
        /// <returns>This object (useful for chaining multiple calls)</returns>
        public ForeignKey WithSourceColumn(string sourceColumnName)
        {
            return this.WithSourceColumns(new[] { sourceColumnName });
        }

        /// <summary>
        /// Adds given columns to <see cref="SourceColumns"/> collection.
        /// </summary>
        /// <param name="sourceColumnNames">Names of source columns.</param>
        /// <returns>This object (useful for chaining multiple calls)</returns>
        public ForeignKey WithSourceColumns(params string[] sourceColumnNames)
        {
            return this.WithSourceColumns(sourceColumnNames.Select(c => new ColumnReference(c)).ToArray());
        }

        /// <summary>
        /// Adds given columns to <see cref="SourceColumns"/> collection.
        /// </summary>
        /// <param name="sourceColumns">Source columns.</param>
        /// <returns>This object (useful for chaining multiple calls)</returns>
        public ForeignKey WithSourceColumns(params Column[] sourceColumns)
        {
            var currentMethodName = MethodInfo.GetCurrentMethod().Name;

            if (this.SourceColumns.Count > 0)
            {
                throw new TaupoInvalidOperationException(currentMethodName + "() can only be called on Foreign Key without source columns defined.");
            }

            foreach (Column c in sourceColumns)
            {
                this.SourceColumns.Add(c);
            }

            return this;
        }

        /// <summary>
        /// Sets the delete action for this foreign key
        /// </summary>
        /// <param name="action">the delete action to set</param>
        /// <returns>the foreign key with deleteAction added</returns>
        public ForeignKey WithDeleteAction(DeleteAction action)
        {
            this.DeleteAction = action;
            return this;
        }

        /// <summary>
        /// Sets up  <see cref="Target"/> table and target columns.
        /// </summary>
        /// <param name="targetName">Target table name.</param>
        /// <param name="targetColumns">Target column names.</param>
        /// <returns>This object (useful for chaining multiple calls)</returns>
        public ForeignKey References(string targetName, params string[] targetColumns)
        {
            return this.References(new TableReference(targetName), targetColumns);
        }

        /// <summary>
        /// Sets up  <see cref="Target"/> table and target columns.
        /// </summary>
        /// <param name="targetCatalogName">Target table's catalog name.</param>
        /// <param name="targetSchemaName">Target table's schema name.</param>
        /// <param name="targetTableName">Target table name.</param>
        /// <param name="targetColumns">Target column names.</param>
        /// <returns>This object (useful for chaining multiple calls)</returns>   
        public ForeignKey References(string targetCatalogName, string targetSchemaName, string targetTableName, string[] targetColumns)
        {
            return this.References(new TableReference(targetCatalogName, targetSchemaName, targetTableName), targetColumns);
        }

        /// <summary>
        /// Sets up  <see cref="Target"/> table and target columns.
        /// </summary>
        /// <param name="target">Target table.</param>
        /// <param name="targetColumns">Target column names.</param>
        /// <returns>This object (useful for chaining multiple calls)</returns>
        public ForeignKey References(Table target, params string[] targetColumns)
        {
            return this.References(target, targetColumns.Select(c => new ColumnReference(c)).ToArray());
        }

        /// <summary>
        /// Sets up  <see cref="Target"/> table and target columns.
        /// </summary>
        /// <param name="target">Target table.</param>
        /// <param name="targetColumns">Target columns.</param>
        /// <returns>This object (useful for chaining multiple calls)</returns>
        public ForeignKey References(Table target, params Column[] targetColumns)
        {
            if (this.TargetColumns.Count > 0)
            {
                throw new TaupoInvalidOperationException("References() can only be called on a Foreign Key without target columns.");
            }

            this.Target = target;
            foreach (Column c in targetColumns)
            {
                this.TargetColumns.Add(c);
            }

            return this;
        }
    }
}
