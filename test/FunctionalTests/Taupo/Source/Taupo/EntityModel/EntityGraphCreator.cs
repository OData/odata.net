//---------------------------------------------------------------------
// <copyright file="EntityGraphCreator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;
    using Microsoft.Test.Taupo.Utilities;

    /// <summary>
    /// The default implementation of the <see cref="IEntityGraphCreator"/> contract.
    /// </summary>
    public class EntityGraphCreator : IEntityGraphCreator
    {
        private EntityContainer entityContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityGraphCreator"/> class.
        /// </summary>
        /// <param name="entityContainer">The <see cref="EntityContainer"/> from the conceptual model
        /// upon which to base metadata for graph creation.</param>
        public EntityGraphCreator(EntityContainer entityContainer)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityContainer, "entityContainer");

            this.entityContainer = entityContainer;

            this.CreateEntityContainerData = CreateDefaultEntityContainerData;
        }

        /// <summary>
        /// Gets or sets the object services utilities used to create objects.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IEntityModelObjectServices ObjectServices { get; set; }

        /// <summary>
        /// Gets or sets the conceptual data services used to populate data.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IEntityModelConceptualDataServices ConceptualDataServices { get; set; }

        /// <summary>
        /// Gets or sets the random number generator.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IRandomNumberGenerator Random { get; set; }

        internal Func<EntityContainerDataPopulationDriver, EntityContainerData> CreateEntityContainerData { get; set; }

        /// <summary>
        /// Creates a set of objects based on the specified <paramref name="root"/> entity instance
        /// and only its required relationships.
        /// </summary>
        /// <param name="entitySetName">The entity set name for the root entity.</param>
        /// <param name="root">The root entity around which the graph is based.</param>
        /// <param name="entityCreated">A callback function invoked every time a new entity instance is created and its properties are initialized.</param>
        /// <param name="connectEntities">A callback used to connect two objects together. Examples of actions include setting navigation properties,
        /// synchronizing FKs to PK values, and/or using the IRelatedEnd or SetLink APIs. The first two parameters are the objects that need to
        /// be connected, and the third is the <see cref="RelationshipSide"/> describing the side of the relationship which the first object participates
        /// in.</param>
        /// <returns>A list of <see cref="IEntitySetData"/> representing all objects in the graph used to satisfy the required relationships.
        /// The first object in the list is always the <paramref name="root"/>.</returns>
        public IList<IEntitySetData> CreateGraphWithRequiredRelationships(
            string entitySetName,
            object root,
            Action<IEntitySetData> entityCreated,
            ConnectEntitiesCallback connectEntities)
        {
            ExceptionUtilities.CheckArgumentNotNull(root, "root");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(entitySetName, "entitySetName");

            ExceptionUtilities.CheckObjectNotNull(
                this.ObjectServices,
                "An {0} instance is required to create graphs. Populate the ObjectServices property.",
                typeof(IEntityModelObjectServices).Name,
                ExpressionUtilities.NameOf(() => this.ObjectServices));

            var dataPopulationDriver = this.CreateDataPopulationDriver();

            var rootEntitySet = this.entityContainer.EntitySets.SingleOrDefault(es => es.ContainerQualifiedName == entitySetName);
            var rootEntityType = this.entityContainer.Model.EntityTypes.SingleOrDefault(et => et.Name == root.GetType().Name);

            ExceptionUtilities.CheckObjectNotNull(rootEntitySet, "Could not locate entity set '{0}' in the model.", entitySetName);
            ExceptionUtilities.CheckObjectNotNull(rootEntityType, "Could not locate entity type '{0}' in the model.", root.GetType().FullName);

            var rootKey = dataPopulationDriver.Seed(rootEntitySet, rootEntityType, this.ObjectServices.GetPropertiesValues(root, rootEntityType));

            var data = this.CreateEntityContainerData(dataPopulationDriver);
            var graph = new List<IEntitySetData>();
            var rootData = this.CreateEntitySetObjectData(root, entitySetName);
            graph.Add(rootData);

            if (data != null)
            {
                var processedEntities = new Dictionary<EntityKey, IEntitySetData>();
                processedEntities.Add(new EntityKey(entitySetName, rootKey), rootData);
                this.CreateGraphCore(
                    graph,
                    rootData,
                    rootEntitySet,
                    rootEntityType,
                    rootKey,
                    data,
                    processedEntities,
                    new HashSet<AssociationInstance>(),
                    entityCreated,
                    connectEntities);
            }

            return graph;
        }

        private static EntityContainerData CreateDefaultEntityContainerData(EntityContainerDataPopulationDriver dataPopulationDriver)
        {
            EntityContainerData data;
            if (dataPopulationDriver.TryPopulateNextData(out data))
            {
                return data;
            }

            return null;
        }

        private static void InvokeConnectEntitiesCallback(
            IEntitySetData source,
            IEntitySetData target,
            RelationshipType relationship,
            RelationshipSide sourceSide,
            HashSet<AssociationInstance> processedAssociations,
            ConnectEntitiesCallback connectEntities)
        {
            if (connectEntities != null && processedAssociations.Add(new AssociationInstance(source, target, relationship.AssociationType.Name, sourceSide.FromRoleName)))
            {
                connectEntities(source, target, relationship.AssociationType.Name, sourceSide.FromRoleName);
            }
        }

        /// <summary>
        /// Examines the required relationships from <paramref name="source"/> and then populates them in <paramref name="graph"/>.
        /// </summary>
        /// <param name="graph">The <see cref="List{IEntitySetData}"/> to which to add the new instance.</param>
        /// <param name="source">The entity from which to find required relationships and populate them.</param>
        /// <param name="sourceEntitySet">The <see cref="EntitySet"/> in which <paramref name="source"/> resides.</param>
        /// <param name="sourceEntityType">The <see cref="EntityType"/> of which the <paramref name="source"/> is an instance.</param>
        /// <param name="sourceKey">The <see cref="EntityDataKey"/> of the <paramref name="source"/> instance.</param>
        /// <param name="data"><see cref="EntityContainerData"/> that contains the structural data from which to create objects.</param>
        /// <param name="processedEntities">The entity instances which have been translated from structural data into objects.</param>
        /// <param name="processedAssociations">The association instances which have been translated from structural data into calls to <paramref name="connectEntities"/>.</param>
        /// <param name="entityCreated">A callback function invoked every time a new entity instance is created and its properties are initialized.</param>
        /// <param name="connectEntities">A callback used to connect two objects together. Examples of actions include setting navigation properties,
        /// synchronizing FKs to PK values, and/or using the IRelatedEnd or SetLink APIs. The first two parameters are the objects that need to
        /// be connected, and the third is the <see cref="RelationshipSide"/> describing the side of the relationship which the first object participates
        /// in.</param>
        private void CreateGraphCore(
            List<IEntitySetData> graph,
            IEntitySetData source,
            EntitySet sourceEntitySet,
            EntityType sourceEntityType,
            EntityDataKey sourceKey,
            EntityContainerData data,
            Dictionary<EntityKey, IEntitySetData> processedEntities,
            HashSet<AssociationInstance> processedAssociations,
            Action<IEntitySetData> entityCreated,
            ConnectEntitiesCallback connectEntities)
        {
            var requiredRelationships =
                from r in sourceEntitySet.Container.RelationshipTypes()
                let side = r.Sides.FirstOrDefault(e =>
                    sourceEntityType.IsKindOf(e.FromEntityType)
                    && sourceEntitySet == e.FromEntitySet
                    && e.ToMultiplicity == EndMultiplicity.One)
                where side != null
                select new
                {
                    Relationship = r,
                    SourceSide = side
                };

            foreach (var r in requiredRelationships)
            {
                var relationship = r.Relationship;
                var sourceSide = r.SourceSide;
                var associationRow = data.GetAssociationSetData(relationship.AssociationSet.Name).Rows
                    .Single(row => row.GetRoleKey(sourceSide.FromRoleName).Equals(sourceKey));
                var targetKey = associationRow.GetRoleKey(sourceSide.ToRoleName);
                var targetEntitySet = sourceSide.ToEntitySet;
                var targetEntityKey = new EntityKey(targetEntitySet.ContainerQualifiedName, targetKey);

                IEntitySetData targetEntity;
                if (!processedEntities.TryGetValue(targetEntityKey, out targetEntity))
                {
                    var targetRow = data.GetEntitySetData(targetEntitySet.Name).Rows.Single(row => row.Key.Equals(targetKey));
                    targetEntity = this.CreateObjectFromRow(targetRow);

                    if (entityCreated != null)
                    {
                        entityCreated(targetEntity);
                    }

                    graph.Add(targetEntity);
                    processedEntities.Add(targetEntityKey, targetEntity);
                    InvokeConnectEntitiesCallback(source, targetEntity, relationship, sourceSide, processedAssociations, connectEntities);

                    this.CreateGraphCore(
                        graph,
                        targetEntity,
                        targetEntitySet,
                        targetRow.EntityType,
                        targetKey,
                        data,
                        processedEntities,
                        processedAssociations,
                        entityCreated,
                        connectEntities);
                }
                else
                {
                    InvokeConnectEntitiesCallback(source, targetEntity, relationship, sourceSide, processedAssociations, connectEntities);
                }
            }
        }

        private IEntitySetData CreateObjectFromRow(EntitySetDataRow targetRow)
        {
            var targetEntityType = targetRow.EntityType;

            var targetObject = this.ObjectServices.GetObjectAdapter(targetEntityType.FullName).CreateData(
                targetRow.PropertyPaths.Select(p => new NamedValue(p, targetRow.GetValue(p))).Where(v => v.Value != UninitializedData.Value));

            return this.CreateEntitySetObjectData(targetObject, targetRow.Parent.EntitySet.ContainerQualifiedName);
        }

        private IEntitySetData CreateEntitySetObjectData(object entity, string entitysetName)
        {
            return new EntitySetObjectData(entity, entitysetName);
        }

        private EntityContainerDataPopulationDriver CreateDataPopulationDriver()
        {
            var dataPopulationDriver = new EntityContainerDataPopulationDriver();
            dataPopulationDriver.EntityContainer = this.entityContainer;
            dataPopulationDriver.ThresholdForNumberOfEntities = -1;
            dataPopulationDriver.StructuralDataServices = this.ConceptualDataServices;
            dataPopulationDriver.Random = this.Random;
            dataPopulationDriver.MinimumNumberOfEntitiesPerEntitySet = 0;
            return dataPopulationDriver;
        }

        /// <summary>
        /// Represents an instance of an association, used to track which associations have been populated
        /// by the graph creator.
        /// </summary>
        private sealed class AssociationInstance : IEquatable<AssociationInstance>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AssociationInstance"/> class.
            /// </summary>
            /// <param name="source">The source entity in the association.</param>
            /// <param name="target">The target entity in the association.</param>
            /// <param name="associationName">The name of the association which relates <paramref name="source"/> and <paramref name="target"/>.</param>
            /// <param name="sourceRoleName">The role name that the <paramref name="source"/> object plays in this association.</param>
            public AssociationInstance(IEntitySetData source, IEntitySetData target, string associationName, string sourceRoleName)
            {
                this.Source = source;
                this.Target = target;
                this.AssociationName = associationName;
                this.SourceRoleName = sourceRoleName;
            }

            /// <summary>
            /// Gets the name of the association which relates <see cref="Source"/> and <see cref="Target"/>.
            /// </summary>
            public string AssociationName { get; private set; }

            /// <summary>
            /// Gets the source entity in the association.
            /// </summary>
            public IEntitySetData Source { get; private set; }

            /// <summary>
            /// Gets the role name that the <see cref="Source"/> plays in the association.
            /// </summary>
            public string SourceRoleName { get; private set; }

            /// <summary>
            /// Gets the target entity in the association.
            /// </summary>
            public IEntitySetData Target { get; private set; }

            /// <summary>
            /// Determines whether this <see cref="AssociationInstance"/> is equal to another <see cref="AssociationInstance"/>.
            /// </summary>
            /// <param name="other">The other <see cref="AssociationInstance"/>.</param>
            /// <returns>True if this <see cref="AssociationInstance"/> is equal to the <paramref name="other"/>, false otherwise.</returns>
            public bool Equals(AssociationInstance other)
            {
                if (other == null)
                {
                    return false;
                }

                if (other.AssociationName != this.AssociationName)
                {
                    return false;
                }

                if (other.Source == this.Source)
                {
                    return other.Target == this.Target && this.SourceRoleName == other.SourceRoleName;
                }
                else if (other.Source == this.Target)
                {
                    return other.Target == this.Source && this.SourceRoleName != other.SourceRoleName;
                }

                return false;
            }

            /// <summary>
            /// Determines whether this <see cref="AssociationInstance"/> is equal to the specified object.
            /// </summary>
            /// <param name="obj">The other object to compare with this <see cref="AssociationInstance"/>.</param>
            /// <returns>True if this <see cref="AssociationInstance"/> is equal to the <paramref name="obj"/>, false otherwise.</returns>
            public override bool Equals(object obj)
            {
                return this.Equals(obj as AssociationInstance);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                var result = 17;
                result = (37 * result) + this.AssociationName.GetHashCode();

                // Objects with the same IEntitySetData instances, regardless of their
                // position (First vs Second), are considered equivalent.
                result = (37 * result) + (this.Source.GetHashCode() + this.Target.GetHashCode());

                return result;
            }
        }

        /// <summary>
        /// Encapsulates a key for an entity type as well as the entity set to which it belongs.
        /// </summary>
        private sealed class EntityKey : IEquatable<EntityKey>
        {
            private string entitySetName;
            private EntityDataKey entityDataKey;

            /// <summary>
            /// Initializes a new instance of the <see cref="EntityKey"/> class.
            /// </summary>
            /// <param name="entitySetName">The name of the entity set.</param>
            /// <param name="entityDataKey">The data key which contains key names and values.</param>
            public EntityKey(string entitySetName, EntityDataKey entityDataKey)
            {
                this.entitySetName = entitySetName;
                this.entityDataKey = entityDataKey;
            }

            /// <summary>
            /// Determines whether this <see cref="EntityKey"/> is equal to another <see cref="EntityKey"/>.
            /// </summary>
            /// <param name="other">The other <see cref="EntityKey"/>.</param>
            /// <returns>True if this <see cref="EntityKey"/> is equal to the <paramref name="other"/>, false otherwise.</returns>
            public bool Equals(EntityKey other)
            {
                if (other == null)
                {
                    return false;
                }

                if (this.entitySetName == other.entitySetName)
                {
                    return this.entityDataKey.Equals(other.entityDataKey);
                }

                return false;
            }

            /// <summary>
            /// Determines whether this <see cref="EntityKey"/> is equal to the specified object.
            /// </summary>
            /// <param name="obj">The other object to compare with this <see cref="EntityKey"/>.</param>
            /// <returns>True if this <see cref="EntityKey"/> is equal to the <paramref name="obj"/>, false otherwise.</returns>
            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                var result = 17;
                result = (37 * result) + this.entitySetName.GetHashCode();
                result = (37 * result) + this.entityDataKey.GetHashCode();

                return result;
            }
        }
    }
}