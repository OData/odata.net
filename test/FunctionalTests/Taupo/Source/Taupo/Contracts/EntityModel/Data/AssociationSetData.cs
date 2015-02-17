//---------------------------------------------------------------------
// <copyright file="AssociationSetData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel.Data
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contains data for an AssociationSet.
    /// </summary>
    [DebuggerDisplay("AssociationSet {this.AssociationSet.Name} Rows: {this.Rows.Count}")]
    public class AssociationSetData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssociationSetData"/> class.
        /// </summary>
        /// <param name="parent">The parent entity container data.</param>
        /// <param name="associationSet">The association set.</param>
        internal AssociationSetData(EntityContainerData parent, AssociationSet associationSet)
        {
            this.Parent = parent;
            this.AssociationSet = associationSet;
            this.Rows = new List<AssociationSetDataRow>();
        }

        /// <summary>
        /// Gets the parent entity container data.
        /// </summary>
        /// <value>The parent entity container data.</value>
        public EntityContainerData Parent { get; private set; }

        /// <summary>
        /// Gets the association set.
        /// </summary>
        /// <value>The association set.</value>
        public AssociationSet AssociationSet { get; private set; }

        /// <summary>
        /// Gets the rows of the association set.
        /// </summary>
        /// <value>The collection of rows.</value>
        public IList<AssociationSetDataRow> Rows { get; private set; }

        /// <summary>
        /// Adds a new row to the association set.
        /// </summary>
        /// <returns>Instance of <see cref="AssociationSetDataRow "/> for the newly added row.</returns>
        public AssociationSetDataRow AddNewRow()
        {
            var row = new AssociationSetDataRow(this);
            this.Rows.Add(row);
            return row;
        }

        /// <summary>
        /// Imports data into the association set.
        /// </summary>
        /// <param name="data">The data to import. Each item in the data array corresponds to a row in the association set.</param>
        /// <example>
        /// data.ImportFrom(
        ///    new { Product = 1, Category = new {Id1 = 1; Id2 = 1 } },
        ///    new { Product = 2, Category = new {Id1 = 1; Id2 = 2 } })
        /// </example>
        public void ImportFrom(object[] data)
        {
            ExceptionUtilities.CheckArgumentNotNull(data, "data");

            var addedRows = new List<AssociationSetDataRow>();
            foreach (object item in data)
            {
                var row = new AssociationSetDataRow(this);
                row.ImportFrom(item);

                // Add new row only if Import succeeded.
                addedRows.Add(row);
            }

            // Add rows when all of them are successfully imported.
            foreach (AssociationSetDataRow row in addedRows)
            {
                this.Rows.Add(row);
            }
        }

        /// <summary>
        /// Gets the ordinal of association role name.
        /// </summary>
        /// <param name="roleName">Association role name.</param>
        /// <returns>Ordinal of the role name.</returns>
        internal int GetRoleNameOrdinal(string roleName)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(roleName, "roleName");

            for (int i = 0; i < this.AssociationSet.Ends.Count; i++)
            {
                if (this.AssociationSet.Ends[i].AssociationEnd.RoleName == roleName)
                {
                    return i;
                }
            }

            throw new TaupoArgumentException(
                string.Format(CultureInfo.InvariantCulture, "Association set '{0}' does not have end with the role name '{1}'.", this.AssociationSet.Name, roleName));
        }

        internal string ToTraceString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Association set '" + this.AssociationSet.Name + "': " + this.Rows.Count + " rows");

            foreach (AssociationSetDataRow row in this.Rows)
            {
                sb.AppendLine(row.ToString());
            }

            return sb.ToString();
        }
    }
}
