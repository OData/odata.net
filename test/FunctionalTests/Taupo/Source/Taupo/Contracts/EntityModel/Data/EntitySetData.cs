//---------------------------------------------------------------------
// <copyright file="EntitySetData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel.Data
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contains data for an entity set.
    /// </summary>
    [DebuggerDisplay("EntitySet {this.EntitySet.Name} Rows: {this.Rows.Count}")]
    public class EntitySetData
    {
        private readonly EntityType singleEntityType;

        internal EntitySetData(EntityContainerData parent, EntitySet entitySet)
        {
            this.EntitySet = entitySet;
            this.Parent = parent;
            this.Rows = new List<EntitySetDataRow>();

            var possibleEntityTypes = this.GetAllNonAbstractTypesOfKind(entitySet.EntityType).ToList();
            if (possibleEntityTypes.Count == 1)
            {
                this.singleEntityType = possibleEntityTypes[0];
            }
        }

        /// <summary>
        /// Gets the EntityContainerData this entity set data belongs to.<see cref="EntityContainerData"/>.
        /// </summary>
        /// <value>The parent.</value>
        public EntityContainerData Parent { get; private set; }

        /// <summary>
        /// Gets the definition of the EntitySet.
        /// </summary>
        /// <value>The EntitySet definition.</value>
        public EntitySet EntitySet { get; private set; }

        /// <summary>
        /// Gets the rows of the EntitySet.
        /// </summary>
        /// <value>The collection of rows.</value>
        public IList<EntitySetDataRow> Rows { get; private set; }

        /// <summary>
        /// Adds a new row to the EntitySet.
        /// </summary>
        /// <returns>Instance of <see cref="EntitySetDataRow"/> for the newly added row.</returns>
        public EntitySetDataRow AddNewRow()
        {
            this.CheckSingleEntityType();

            return this.AddNewRowOfTypeInternal(this.singleEntityType);
        }

        /// <summary>
        /// Adds a new row to the EntitySet with the specified entity type.
        /// </summary>
        /// <param name="entityTypeName">Entity type name for the new row.</param>
        /// <returns>Instance of <see cref="EntitySetDataRow"/> for the newly added row.</returns>
        public EntitySetDataRow AddNewRowOfType(string entityTypeName)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(entityTypeName, "entityTypeName");
            EntityType entityType = this.Parent.GetEntityType(entityTypeName);

            return this.AddNewRowOfType(entityType);
        }

        /// <summary>
        /// Adds a new row to the EntitySet with the specified entity type.
        /// </summary>
        /// <param name="entityType">Entity type for the new row.</param>
        /// <returns>Instance of <see cref="EntitySetDataRow"/> for the newly added row.</returns>
        public EntitySetDataRow AddNewRowOfType(EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            this.CheckEntityTypeBelongsToEntitySet(entityType);

            return this.AddNewRowOfTypeInternal(entityType);
        }

        /// <summary>
        /// Imports data into the EntitySet.
        /// </summary>
        /// <param name="data">The data to import.</param>
        public void ImportFrom(object[] data)
        {
            ExceptionUtilities.CheckArgumentNotNull(data, "data");
            this.CheckSingleEntityType();

            this.InternalImportFrom(this.singleEntityType, data);
        }

        /// <summary>
        /// Imports data into the EntitySet.
        /// </summary>
        /// <param name="entityType">entity type of the data.</param>
        /// <param name="data">The data to import.</param>
        public void ImportFrom(EntityType entityType, object[] data)
        {
            ExceptionUtilities.CheckArgumentNotNull(data, "data");
            this.Parent.ValidateEntityType(entityType);
            this.CheckEntityTypeBelongsToEntitySet(entityType);

            this.InternalImportFrom(entityType, data);
        }

        /// <summary>
        /// Generates a dump of the contents of the EntitySet data (suitable for tracing).
        /// </summary>
        /// <returns>Contents of the EntitySet rendered as a string.</returns>
        public string ToTraceString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Entity Set '" + this.EntitySet.Name + "': " + this.Rows.Count + " rows");

            foreach (EntitySetDataRow row in this.Rows)
            {
                sb.AppendLine(row.ToTraceString());
            }

            return sb.ToString();
        }

        private void InternalImportFrom(EntityType entityType, object[] data)
        {
            var addedRows = new List<EntitySetDataRow>();
            foreach (object item in data)
            {
                var row = new EntitySetDataRow(this, entityType);
                row.ImportFrom(item);

                // Add new row only if Import succeeded.
                addedRows.Add(row);
            }

            // Add rows when all of them are successfully imported.
            foreach (EntitySetDataRow row in addedRows)
            {
                this.Rows.Add(row);
            }
        }

        private EntitySetDataRow AddNewRowOfTypeInternal(EntityType entityType)
        {
            var row = new EntitySetDataRow(this, entityType);
            this.Rows.Add(row);
            return row;
        }

        private void CheckEntityTypeBelongsToEntitySet(EntityType entityType)
        {
            if (entityType.IsAbstract)
            {
                throw new TaupoArgumentException(string.Format(CultureInfo.InvariantCulture, "Entity type '{0}' is abstract and cannot have data instances.", entityType.FullName));
            }

            if (!this.GetAllNonAbstractTypesOfKind(this.EntitySet.EntityType).Contains(entityType))
            {
                throw new TaupoArgumentException(string.Format(CultureInfo.InvariantCulture, "Entity type '{0}' does not belong to entity set '{1}'.", entityType.FullName, this.EntitySet.Name));
            }
        }
        
        private IEnumerable<EntityType> GetAllNonAbstractTypesOfKind(EntityType entityType)
        {
            return this.Parent.EntityContainer.Model.EntityTypes.Where(t => !t.IsAbstract && t.IsKindOf(entityType));
        }

        private void CheckSingleEntityType()
        {
            if (this.singleEntityType == null)
            {
                throw new TaupoInvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "Entity set '{0}' contains multiple entity types.", this.EntitySet.Name));
            }
        }
    }
}
