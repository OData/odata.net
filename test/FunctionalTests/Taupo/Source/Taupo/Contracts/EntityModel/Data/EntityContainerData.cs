//---------------------------------------------------------------------
// <copyright file="EntityContainerData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Contains data for an EntityContainer.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class EntityContainerData : IEnumerable
    {
        private IDictionary<EntitySet, EntitySetData> entityDataSets;
        private IDictionary<AssociationSet, AssociationSetData> associationDataSets;
        private PropertyPathBuilder propertyPathResolver;
        private IDictionary<StructuralType, IList<string>> nonKeyPropertyPathsPerStructuralType;
        private IDictionary<EntityType, IList<string>> keyPropertyNamesPerEntityType;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityContainerData"/> class.
        /// </summary>
        /// <param name="entityContainer">The entity container.</param>
        /// <exception cref="TaupoArgumentException">
        /// When entity container does not belong to a model
        /// </exception>        
        public EntityContainerData(EntityContainer entityContainer)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityContainer, "entityContainer");
            
            if (entityContainer.Model == null)
            {
                throw new TaupoArgumentException("Specified entity container does not belong to a model schema.");
            }

            this.Initialize(entityContainer);
        }

        /// <summary>
        /// Gets the entity container.
        /// </summary>
        /// <value>The entity container.</value>
        public EntityContainer EntityContainer { get; private set; }

        /// <summary>
        /// Gets the <see cref="Microsoft.Test.Taupo.Contracts.EntityModel.Data.EntitySetData"/> for the specified entity set.
        /// </summary>
        /// <param name="entitySet">EntitySet to get the data for.</param>
        /// <value>Instance of <see cref="EntitySetData"/>.</value>
        /// <remarks>The entity set must be part of the EntityContainer.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers", Justification = "Convenience accessor.")]
        public EntitySetData this[EntitySet entitySet]
        {
            get
            {
                ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");

                EntitySetData esd;
                if (!this.entityDataSets.TryGetValue(entitySet, out esd))
                {
                    this.ValidateEntitySet(entitySet);

                    esd = new EntitySetData(this, entitySet);
                    this.entityDataSets.Add(entitySet, esd);
                }

                return esd;
            }
        }

        /// <summary>
        /// Gets the <see cref="Microsoft.Test.Taupo.Contracts.EntityModel.Data.AssociationSetData"/> with the specified association set.
        /// </summary>
        /// <param name="associationSet">association set to get data for.</param>
        /// <value>Instance of <see cref="AssociationSetData"/>.</value>
        /// <remarks>The AssociationSet must be part of the EntityContainer.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers", Justification = "Convenience accessor.")]
        public AssociationSetData this[AssociationSet associationSet]
        {
            get
            {
                ExceptionUtilities.CheckArgumentNotNull(associationSet, "associationSet");

                AssociationSetData asd;
                if (!this.associationDataSets.TryGetValue(associationSet, out asd))
                {
                    this.ValidateAssociationSet(associationSet);

                    asd = new AssociationSetData(this, associationSet);
                    this.associationDataSets.Add(associationSet, asd);
                }

                return asd;
            }
        }

        /// <summary>
        /// Gets the entity set data.
        /// </summary>
        /// <param name="entitySetName">Name of the entity set.</param>
        /// <returns>Entity set data</returns>
        public EntitySetData GetEntitySetData(string entitySetName)
        {
            EntitySet entitySet = this.GetEntitySet(entitySetName);
            return this[entitySet];
        }

        /// <summary>
        /// Gets the association set data.
        /// </summary>
        /// <param name="associationSetName">Name of the association set.</param>
        /// <returns>Association set data</returns>
        public AssociationSetData GetAssociationSetData(string associationSetName)
        {
            AssociationSet associationSet = this.GetAssociationSet(associationSetName);
            return this[associationSet];
        }

        /// <summary>
        /// Adds data to the entity set.
        /// </summary>
        /// <param name="entitySet">The entity set.</param>
        /// <param name="data">The initial data.</param>
        /// <remarks>
        /// Data in <paramref name="data"/> is typically an array of anonymous objects.
        /// This allows readable, in-line definition of data.
        /// </remarks>
        /// <example>
        /// data.Add(entitySet,
        ///    new { ID = 1, Name = "Food Product", CategoryID = -1 },
        ///    new { ID = 2, Name = "Beverages", CategoryID = -2 },
        ///    new { ID = 3, Name = "Meat", CategoryID = -3 });
        /// </example>
        /// <exception cref="TaupoInvalidOperationException">When entitySet contains multple entity types.</exception>
        public void Add(EntitySet entitySet, params object[] data)
        {
            this[entitySet].ImportFrom(data);
        }

        /// <summary>
        /// Adds data to the entity set for the specified entity type.
        /// </summary>
        /// <param name="entitySetName">The name of the entity set.</param>
        /// <param name="entityTypeName">The name of the entity type.</param>
        /// <param name="data">The initial data.</param>
        /// <remarks>
        /// Data in <paramref name="data"/> is typically an array of anonymous objects.
        /// This allows readable, in-line definition of data.
        /// </remarks>
        /// <example>
        /// data.Add("Products", "DiscontinuedProduct",
        ///    new { ID = 1, Name = "Food Product", CategoryID = -1 },
        ///    new { ID = 2, Name = "Beverages", CategoryID = -2 },
        ///    new { ID = 3, Name = "Meat", CategoryID = -3 });
        /// </example>
        public void Add(string entitySetName, string entityTypeName, params object[] data)
        {
            EntitySet entitySet = this.GetEntitySet(entitySetName);
            EntityType entityType = this.GetEntityType(entityTypeName);

            this.Add(entitySet, entityType, data);
        }

        /// <summary>
        /// Adds data to the entity set for the specified entity type.
        /// </summary>
        /// <param name="entitySet">The entity set.</param>
        /// <param name="entityType">The entity type.</param>
        /// <param name="data">The initial data.</param>
        /// <remarks>
        /// Data in <paramref name="data"/> is typically an array of anonymous objects.
        /// This allows readable, in-line definition of data.
        /// </remarks>
        /// <example>
        /// data.Add(entitySet, entityType,
        ///    new { ID = 1, Name = "Food Product", CategoryID = -1 },
        ///    new { ID = 2, Name = "Beverages", CategoryID = -2 },
        ///    new { ID = 3, Name = "Meat", CategoryID = -3 });
        /// </example>
        public void Add(EntitySet entitySet, EntityType entityType, params object[] data)
        {
            this[entitySet].ImportFrom(entityType, data);
        }

        /// <summary>
        /// Adds data to the association or entity set.
        /// </summary>
        /// <param name="setName">The name of association or entity set.</param>
        /// <param name="data">The initial data.</param>
        /// <remarks>
        /// Data in <paramref name="data"/> is typically an array of anonymous objects.
        /// This allows readable, in-line definition of data.
        /// </remarks>
        /// <example>
        /// data.Add("Product_Category",
        ///    new { Product = 1, Category = new {Id1 = 1; Id2 = 1 } },
        ///    new { Product = 2, Category = new {Id1 = 1; Id2 = 2 } })
        /// </example>
        /// <exception cref="TaupoArgumentException">
        /// When EntitySet or AssociationSet with the sepcified name does not exist in the EntityContainer.
        /// </exception>
        public void Add(string setName, params object[] data)
        {
            AssociationSet associationSet = this.FindAssociationSet(setName);
            EntitySet entitySet = this.FindEntitySet(setName);

            if (associationSet == null && entitySet == null)
            {
                throw new TaupoArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "Cannot find entity set or association set '{0}' in the entity container '{1}'.", setName, this.EntityContainer.Name));
            }

            if (associationSet != null && entitySet != null)
            {
                throw new TaupoArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "Found both entity set and association set with name '{0}' in the entity container '{1}'.", setName, this.EntityContainer.Name));
            }

            if (entitySet != null)
            {
                this.Add(entitySet, data);
            }
            else
            {
                this.Add(associationSet, data);
            }
        }

        /// <summary>
        /// Adds data to the association set.
        /// </summary>
        /// <param name="associationSet">The association set.</param>
        /// <param name="data">The initial data.</param>
        /// <remarks>
        /// Data in <paramref name="data"/> is typically an array of anonymous objects.
        /// This allows readable, in-line definition of data.
        /// </remarks>
        /// <example>
        /// data.Add(associationSet,
        ///    new { Product = 1, Category = new {Id1 = 1; Id2 = 1 } },
        ///    new { Product = 2, Category = new {Id1 = 1; Id2 = 2 } })
        /// </example>
        public void Add(AssociationSet associationSet, params object[] data)
        {
            this[associationSet].ImportFrom(data);
        }

        /// <summary>
        /// Copies entity container data.
        /// </summary>
        /// <param name="data">The entity container data to copy data to.</param>
        public void CopyTo(EntityContainerData data)
        {
            foreach (EntitySet entitySet in this.EntityContainer.EntitySets)
            {
                foreach (EntitySetDataRow sourceRow in this[entitySet].Rows)
                {
                    EntitySetDataRow destinationRow = data[entitySet].AddNewRowOfType(sourceRow.EntityType);
                    foreach (string path in sourceRow.PropertyPaths)
                    {
                        destinationRow[path] = sourceRow[path];
                    }
                }
            }

            foreach (AssociationSet associationSet in this.EntityContainer.AssociationSets)
            {
                foreach (AssociationSetDataRow sourceRow in this[associationSet].Rows)
                {
                    AssociationSetDataRow destinationRow = data[associationSet].AddNewRow();
                    foreach (string roleName in sourceRow.RoleNames)
                    {
                        destinationRow[roleName] = sourceRow[roleName];
                    }
                }
            }
        }

        /// <summary>
        /// Generates a dump of the contents of the <see cref="EntityContainer"/> (suitable for tracing).
        /// </summary>
        /// <returns>Contents of the entity container rendered as a string.</returns>
        public string ToTraceString()
        {
            var sb = new StringBuilder();

            foreach (var kvp in this.entityDataSets.Where(c => c.Value.Rows.Count > 0).OrderBy(c => c.Key.Name))
            {
                sb.Append(kvp.Value.ToTraceString());
            }

            foreach (var kvp in this.associationDataSets.Where(c => c.Value.Rows.Count > 0).OrderBy(c => c.Key.Name))
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

        internal void ValidateEntityType(EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");

            if (!this.EntityContainer.Model.EntityTypes.Contains(entityType))
            {
                throw new TaupoArgumentException("Entity type '" + entityType.FullName + "' not found in the model schema.");
            }
        }

        internal EntityType GetEntityType(string entityTypeName)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(entityTypeName, "entityTypeName");

            // Search by FullName first.
            EntityType result = this.EntityContainer.Model.EntityTypes.Where(t => t.FullName == entityTypeName).SingleOrDefault();
            if (result == null)
            {
                // Search by name only - if there is only one EntiyType with this name  - return it.
                var matches = this.EntityContainer.Model.EntityTypes.Where(t => t.Name == entityTypeName).ToList();
                if (matches.Count > 1)
                {
                    throw new TaupoArgumentException(
                        string.Format(CultureInfo.InvariantCulture, "Found more than one entity type with name '{0}'. Try to provide full name.", entityTypeName));
                }
                else if (matches.Count == 1)
                {
                    result = matches[0];
                }
            }

            if (result == null)
            {
                throw new TaupoArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "Entity type '{0}' is not found in the model schema.", entityTypeName));
            }

            return result;
        }

        internal IList<string> GetKeyPropertyNames(EntityType entityType)
        {
            return this.keyPropertyNamesPerEntityType[entityType];
        }

        private void ValidateEntitySet(EntitySet entitySet)
        {
            if (!this.EntityContainer.EntitySets.Contains(entitySet))
            {
                throw new TaupoArgumentException("Entity set '" + entitySet.Name + "' not found in the entity container '" + this.EntityContainer.Name + "'.");
            }
        }

        private void ValidateAssociationSet(AssociationSet associationSet)
        {
            if (!this.EntityContainer.AssociationSets.Contains(associationSet))
            {
                throw new TaupoArgumentException("Association set '" + associationSet.Name + "' not found in the entity container '" + this.EntityContainer.Name + "'.");
            }
        }

        private EntitySet GetEntitySet(string name)
        {
            EntitySet entitySet = this.FindEntitySet(name);
            if (entitySet == null)
            {
                throw new TaupoArgumentException("Entity set '" + name + "' not found in the entity container '" + this.EntityContainer.Name + "'.");
            }

            return entitySet;
        }

        private EntitySet FindEntitySet(string name)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(name, "name");

            EntitySet entitySet = this.EntityContainer.EntitySets.SingleOrDefault(s => s.Name == name || this.EntityContainer.Name + "." + s.Name == name);

            return entitySet;
        }

        private AssociationSet GetAssociationSet(string name)
        {
            AssociationSet associationSet = this.FindAssociationSet(name);
            if (associationSet == null)
            {
                throw new TaupoArgumentException("Association set '" + name + "' not found in the entity container '" + this.EntityContainer.Name + "'.");
            }

            return associationSet;
        }

        private AssociationSet FindAssociationSet(string name)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(name, "name");

            AssociationSet associationSet = this.EntityContainer.AssociationSets.SingleOrDefault(s => s.Name == name || name == this.EntityContainer.Name + "." + s.Name);

            return associationSet;
        }

        private void Initialize(EntityContainer entityContainer)
        {
            this.EntityContainer = entityContainer;

            this.entityDataSets = new Dictionary<EntitySet, EntitySetData>();
            this.associationDataSets = new Dictionary<AssociationSet, AssociationSetData>();
            this.propertyPathResolver = new PropertyPathBuilder(this);

            this.InitializePropertyPaths();
        }

        private void InitializePropertyPaths()
        {
            this.nonKeyPropertyPathsPerStructuralType = new Dictionary<StructuralType, IList<string>>();

            var nonAbstractEntityTypes = this.EntityContainer.Model.EntityTypes.Where(t => !t.IsAbstract);

            this.BuildPropertyPaths<ComplexType>(this.EntityContainer.Model.ComplexTypes, (ct) => ct.Properties);
            this.BuildPropertyPaths<EntityType>(nonAbstractEntityTypes, (et) => et.AllProperties.Where(p => !p.IsPrimaryKey));
           
            this.keyPropertyNamesPerEntityType = new Dictionary<EntityType, IList<string>>();

            foreach (EntityType et in nonAbstractEntityTypes)
            {
                this.keyPropertyNamesPerEntityType[et] = et.AllKeyProperties.Select(p => p.Name).ToList().AsReadOnly();
            }
        }

        private void BuildPropertyPaths<TType>(IEnumerable<TType> structuralTypes, Func<TType, IEnumerable<MemberProperty>> propertiesSelector)
            where TType : NamedStructuralType
        {
            foreach (TType st in structuralTypes)
            {
                this.nonKeyPropertyPathsPerStructuralType[st] = this.propertyPathResolver.BuildPropertyPaths(propertiesSelector(st), st.FullName);
            }
        }

        /// <summary>
        /// Builder for properties paths.
        /// </summary>
        private class PropertyPathBuilder : IDataTypeVisitor<IList<string>>
        {
            private static readonly IList<string> emptyList = new List<string>();
            private readonly EntityContainerData entityContainerData;

            /// <summary>
            /// Initializes a new instance of the <see cref="PropertyPathBuilder"/> class.
            /// </summary>
            /// <param name="entityContainerData">The entity container data.</param>
            public PropertyPathBuilder(EntityContainerData entityContainerData)
            {
                this.entityContainerData = entityContainerData;
            }

            /// <summary>
            /// Builds the property paths.
            /// </summary>
            /// <param name="properties">The properties.</param>
            /// <param name="parentTypeFullName">Full name of the parent type.</param>
            /// <returns>Property paths.</returns>
            public IList<string> BuildPropertyPaths(IEnumerable<MemberProperty> properties, string parentTypeFullName)
            {
                var list = new List<string>();

                foreach (MemberProperty property in properties)
                {
                    string path = property.Name;
                    ExceptionUtilities.CheckObjectNotNull(property.PropertyType, "Property '{0}' on type '{1}' cannot have null type.", property.Name, parentTypeFullName);

                    IList<string> subPaths = property.PropertyType.Accept(this);
                    if (subPaths.Count == 0)
                    {
                        list.Add(path);
                    }
                    else
                    {
                        list.AddRange(subPaths.Select(p => path + "." + p));
                    }
                }

                return list.AsReadOnly();
            }

            /// <summary>
            /// Visits the specified collection data type.
            /// </summary>
            /// <param name="dataType">Collection data type.</param>
            /// <returns>Implementation-specific value.</returns>
            public IList<string> Visit(CollectionDataType dataType)
            {
                return emptyList;
            }

            /// <summary>
            /// Visits the specified complex type.
            /// </summary>
            /// <param name="dataType">Entity data type.</param>
            /// <returns>Implementation-specific value.</returns>
            public IList<string> Visit(ComplexDataType dataType)
            {
                ComplexType complexType = dataType.Definition;
                IList<string> paths;

                if (!this.entityContainerData.nonKeyPropertyPathsPerStructuralType.TryGetValue(complexType, out paths))
                {
                    paths = this.BuildPropertyPaths(complexType.Properties, complexType.FullName);

                    this.entityContainerData.nonKeyPropertyPathsPerStructuralType[complexType] = paths;
                }

                return paths;
            }

            /// <summary>
            /// Visits the specified entity data type.
            /// </summary>
            /// <param name="dataType">Data type.</param>
            /// <returns>Implementation-specific value.</returns>
            public IList<string> Visit(EntityDataType dataType)
            {
                throw new TaupoNotSupportedException("Properties with entity data type are not supported.");
            }

            /// <summary>
            /// Visits the specified primitive type.
            /// </summary>
            /// <param name="dataType">Data type.</param>
            /// <returns>Implementation-specific value.</returns>
            public IList<string> Visit(PrimitiveDataType dataType)
            {
                return emptyList;
            }

            /// <summary>
            /// Visits the specified reference data type.
            /// </summary>
            /// <param name="dataType">Data type.</param>
            /// <returns>Implementation-specific value.</returns>
            public IList<string> Visit(ReferenceDataType dataType)
            {
                throw new TaupoNotSupportedException("Properties with reference data type are not supported.");
            }
        }
    }
}
