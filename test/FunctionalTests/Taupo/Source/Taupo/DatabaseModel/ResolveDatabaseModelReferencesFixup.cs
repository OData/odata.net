//---------------------------------------------------------------------
// <copyright file="ResolveDatabaseModelReferencesFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DatabaseModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.DatabaseModel;

    /// <summary>
    /// Resolves references in the database model by converting <see cref="TableReference" />
    /// instances to <see cref="Table" /> and <see cref="ColumnReference"/> to <see cref="Column"/>.
    /// </summary>
    public class ResolveDatabaseModelReferencesFixup : DatabaseModelFixupBase
    {
        /// <summary>
        /// Resolves references in the database model by converting <see cref="TableReference" />
        /// instances to <see cref="Table" /> and <see cref="ColumnReference"/> to <see cref="Column"/>.
        /// </summary>
        /// <param name="schema">Database schema to fixup</param>
        public override void Fixup(DatabaseSchema schema)
        {
            foreach (Table table in schema.Tables.Concat(schema.Views.Cast<Table>()))
            {
                if (table.PrimaryKey != null)
                {
                    this.ResolveColumnsInConstraint(table, table.PrimaryKey);
                }

                foreach (var constraint in table.UniqueConstraints)
                {
                    this.ResolveColumnsInConstraint(table, constraint);
                }

                this.ResolveForeignKeyConstraints(schema, table);
            }
        }

        private void ResolveColumnsInConstraint(Table table, ColumnsConstraintBase constraint)
        {
            this.FixupColumns(table, constraint.Columns);
        }

        private void ResolveForeignKeyConstraints(DatabaseSchema schema, Table table)
        {
            foreach (ForeignKey foreignKey in table.ForeignKeys)
            {
                foreignKey.Target = this.ResolveTableReference(schema, foreignKey.Target);
                this.FixupColumns(table, foreignKey.SourceColumns);
                this.FixupColumns(foreignKey.Target, foreignKey.TargetColumns);
            }
        }

        private void FixupColumns(Table table, IList<Column> columns)
        {
            for (int i = 0; i < columns.Count; ++i)
            {
                columns[i] = this.ResolveColumnReference(table, columns[i]);
            }
        }

        private Column ResolveColumnReference(Table table, Column column)
        {
            ColumnReference columnRef = column as ColumnReference;
            if (columnRef == null)
            {
                return column;
            }

            var result = table.Columns.SingleOrDefault(c => c.Name == columnRef.Name);
            if (result == null)
            {
                throw new TaupoInvalidOperationException("The column '" + columnRef.Name + "' was not found in '" + table + "'");
            }

            return result;
        }

        private Table ResolveTableReference(DatabaseSchema schema, Table table)
        {
            TableReference tableRef = table as TableReference;
            if (tableRef == null)
            {
                return table;
            }

            var result = schema.Tables.Concat(schema.Views.Cast<Table>())
                .Where(t => t.Catalog == tableRef.Catalog && t.Schema == tableRef.Schema && t.Name == tableRef.Name)
                .SingleOrDefault();

            if (result == null)
            {
                throw new TaupoInvalidOperationException("The table or view '" + tableRef + "' was not found in the schema.");
            }

            return result;
        }
    }
}
