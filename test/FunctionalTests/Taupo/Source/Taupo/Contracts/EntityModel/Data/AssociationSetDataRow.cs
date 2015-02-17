//---------------------------------------------------------------------
// <copyright file="AssociationSetDataRow.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel.Data
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Row of data in the association set.
    /// </summary>
    [DebuggerDisplay("{ToTraceString()}")]
    public class AssociationSetDataRow
    {
        private readonly EntityDataKey[] data;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssociationSetDataRow"/> class.
        /// </summary>
        /// <param name="parent">The association set data this row belongs to.</param>
        internal AssociationSetDataRow(AssociationSetData parent)
        {
            this.Parent = parent;
            this.data = new EntityDataKey[parent.AssociationSet.Ends.Count];
        }

        /// <summary>
        /// Gets the association set data this row belongs to.
        /// </summary>
        /// <value>The parent association set data.</value>
        public AssociationSetData Parent { get; private set; }

        /// <summary>
        /// Gets the association role names.
        /// </summary>
        /// <value>The association role names.</value>
        public IEnumerable<string> RoleNames
        {
            get { return this.Parent.AssociationSet.Ends.Select(e => e.AssociationEnd.RoleName); }
        }

        /// <summary>
        /// Gets or sets the EntityDataKey for the specified association role.
        /// </summary>
        /// <param name="roleName">The association role name</param>
        /// <value>EntityDataKey for the association role.</value>
        public EntityDataKey this[string roleName]
        {
            get { return this.GetRoleKey(roleName); }
            set { this.SetRoleKey(roleName, value); }
        }

        /// <summary>
        /// Gets the EntityDataKey for the specified association role.
        /// </summary>
        /// <param name="roleName">The association role name.</param>
        /// <returns>EntityDataKey for the association role.</returns>
        public EntityDataKey GetRoleKey(string roleName)
        {
            int ordinal = this.GetRoleNameOrdinal(roleName);
            EntityDataKey key = this.data[ordinal];

            if (key == null)
            {
                key = new EntityDataKey(this.GetKeyNamesForRoleName(roleName));
                this.data[ordinal] = key;
            }

            return key;
        }

        /// <summary>
        /// Sets the EntityDataKey for the specified association role.
        /// </summary>
        /// <param name="roleName">The association role name.</param>
        /// <param name="key">EntityDataKey for the association role.</param>
        public void SetRoleKey(string roleName, EntityDataKey key)
        {
            ExceptionUtilities.CheckArgumentNotNull(key, "key");
            int ordinal = this.GetRoleNameOrdinal(roleName);

            List<string> expectedKeyNames = this.GetKeyNamesForRoleName(roleName).ToList();

            if (expectedKeyNames.Count != key.KeyNames.Count ||
                expectedKeyNames.Join(key.KeyNames, n1 => n1, n2 => n2, (n1, n2) => n1).Count() != expectedKeyNames.Count)
            {
                throw new TaupoArgumentException("Specified key does not match key metadata for the role '" + roleName + "'.");
            }

            this.data[ordinal] = key;
        }

        /// <summary>
        /// Imports the EntityDataKey for the specified association role name.
        /// </summary>
        /// <param name="roleName">The association role name.</param>
        /// <param name="value">The key value. Can be scalar value (for singleton key) or anonymous object (for compostie key).</param>
        public void ImportRoleKey(string roleName, object value)
        {
            this.GetRoleKey(roleName).ImportFrom(value);
        }

        /// <summary>
        /// Returns a string representation of this <see cref="AssociationSetDataRow"/>, for use in debugging and logging.
        /// </summary>
        /// <returns>String representation of this <see cref="AssociationSetDataRow"/>.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(" {");
            string separator = " ";

            foreach (string roleName in this.RoleNames)
            {
                sb.Append(separator);
                sb.Append(roleName);
                sb.Append(" = ");
                sb.Append(this.GetRoleKey(roleName).ToString());
                separator = "; ";
            }

            sb.Append(" }");
            return sb.ToString();
        }

        /// <summary>
        /// Imports association data row from an object.
        /// </summary>
        /// <param name="item">The item structurally representing association data row. Usually anonymous object.</param>
        /// <example>new { Product = 1; Category = 5 }</example>
        internal void ImportFrom(object item)
        {
            ExceptionUtilities.CheckObjectNotNull(item, "Cannot import row from null object.");

            foreach (PropertyInfo propertyInfo in item.GetType().GetProperties())
            {
                string roleName = propertyInfo.Name;
                object value = propertyInfo.GetValue(item, null);
                var key = value as EntityDataKey;
                if (key != null)
                {
                    this.SetRoleKey(roleName, key);
                }
                else
                {
                    this.ImportRoleKey(roleName, value);
                }
            }
        }

        private int GetRoleNameOrdinal(string roleName)
        {
            return this.Parent.GetRoleNameOrdinal(roleName);
        }

        private IEnumerable<string> GetKeyNamesForRoleName(string roleName)
        {
            EntityType entityType = this.Parent.AssociationSet.Ends
                    .Where(e => e.AssociationEnd.RoleName == roleName).Select(e => e.AssociationEnd.EntityType).Single();

            return entityType.AllKeyProperties.Select(p => p.Name);
        }
    }
}
