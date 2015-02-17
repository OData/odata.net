//---------------------------------------------------------------------
// <copyright file="EntityContainerDataPopulationDriver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;
    using Microsoft.Test.Taupo.EntityModel.Data;
    using Microsoft.Test.Taupo.EntityModel.Internal;

    /// <summary>
    /// Data population driver for an entity container.
    /// </summary>
    [ImplementationName(typeof(IEntityContainerDataPopulationDriver), "Default")]
    public class EntityContainerDataPopulationDriver : IEntityContainerDataPopulationDriver
    {
        private EntityContainer entityContainer;
        private EntitySetAndTypeSelector entitySetAndTypeSelector;
        private List<RelationshipSelector> relationshipSelectors;
        private List<KeyValuePair<EntitySet, EntityType>> entitiesToCreateInNextBatch;
        private int minimumNumberOfEntitiesPerEntitySet;
        private EntityContainerData seedData;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityContainerDataPopulationDriver"/> class.
        /// </summary>
        public EntityContainerDataPopulationDriver()
        {
            this.minimumNumberOfEntitiesPerEntitySet = 10;
            this.ThresholdForNumberOfEntities = 100;
            this.ReferentialConstraintsResolver = new ReferentialConstraintsResolver();
        }
        
        /// <summary>
        /// Gets or sets the entity container.
        /// </summary>
        /// <value>The entity container.</value>
        public EntityContainer EntityContainer
        {
            get
            {
                return this.entityContainer;
            }

            set
            {
                if (this.entityContainer != value)
                {
                    this.ClearSelectors();
                    this.entityContainer = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the random number generator.
        /// </summary>
        /// <value>The random number generator.</value>
        [InjectDependency]
        public IRandomNumberGenerator Random { get; set; }

        /// <summary>
        /// Gets or sets the structural generators.
        /// </summary>
        /// <value>The structural generators.</value>
        [InjectDependency]
        public IEntityModelConceptualDataServices StructuralDataServices { get; set; }

        /// <summary>
        /// Gets or sets the referential constraints resolver.
        /// </summary>
        /// <value>The referential constraints resolver.</value>
        [InjectDependency]
        public IReferentialConstraintsResolver ReferentialConstraintsResolver { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of entities per entity set.
        /// </summary>
        /// <value>The minimum number of entities per entity set.</value>
        [InjectTestParameter("MinimumNumberOfEntitiesPerEntitySet", DefaultValueDescription = "10", HelpText = "Minimum number of entities per entity set. Used when populating data.")]
        public int MinimumNumberOfEntitiesPerEntitySet 
        {
            get 
            { 
                return this.minimumNumberOfEntitiesPerEntitySet;
            }

            set
            {
                this.ValidateCount(value, "Value");
                this.minimumNumberOfEntitiesPerEntitySet = value;

                if (this.entitySetAndTypeSelector != null)
                {
                    this.entitySetAndTypeSelector.SetMinimumNumberOfEntities(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the threshold for number of entities populated by <see cref="TryPopulateNextData"/>.
        /// </summary>
        /// <value>The threshold for number of entities populated by <see cref="TryPopulateNextData"/>.</value>
        /// <remarks>Set the threshold to -1 if all data need to be populated in one call to <see cref="TryPopulateNextData"/>.</remarks>
        [InjectTestParameter("ThresholdForNumberOfPopulatedEntities", DefaultValueDescription = "100", HelpText = "Threshold for number of entities populated by EntityContainerDataPopulationDriver.TryPopulateNextData.")]
        public int ThresholdForNumberOfEntities { get; set; }

        /// <summary>
        /// Adds seed data to the <see cref="EntityContainerData"/> created by this instance by
        /// calling <see cref="TryPopulateNextData"/>.
        /// </summary>
        /// <param name="entitySet">The <see cref="EntitySet"/> to which the new seed instance belongs.</param>
        /// <param name="entityType">The <see cref="EntityType"/> of the new seed instance.</param>
        /// <param name="entityData">A collection of <see cref="NamedValue"/>s that describe the structural data of the instance.</param>
        /// <returns>The <see cref="EntityDataKey"/> that describes the seed instance.</returns>
        public EntityDataKey Seed(EntitySet entitySet, EntityType entityType, IEnumerable<NamedValue> entityData)
        {
            this.CreateEntitySetAndTypeSelectorIfNull();
            this.CreateRelationshipSelectorsIfNull();

            if (this.seedData == null)
            {
                this.seedData = new EntityContainerData(this.entityContainer);
            }

            var seedRow = this.PopulateEntitySetRow(this.seedData, entitySet, entityType, entityData);
            this.ConsiderCandidateForRelationships(seedRow);

            return seedRow.Key;
        }

        /// <summary>
        /// Sets the minimum number of entities to be created for the specified entity set.
        /// </summary>
        /// <param name="entitySetName">The entity set.</param>
        /// <param name="count">The minimum number of entities to create.</param>
        public void SetMinimumNumberOfEntities(string entitySetName, int count)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(entitySetName, "entitySetName");

            this.ValidateCount(count, "Count");

            this.CreateEntitySetAndTypeSelectorIfNull();

            this.entitySetAndTypeSelector.SetMinimumNumberOfEntities(entitySetName, count);
        }

        /// <summary>
        /// Sets the minimum number of entities to be created for the specified entity set and type.
        /// </summary>
        /// <param name="entitySetName">The entity set name.</param>
        /// <param name="entityTypeName">The entity type name.</param>
        /// <param name="count">The minimum number of entities to create.</param>
        public void SetMinimumNumberOfEntities(string entitySetName, string entityTypeName, int count)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(entitySetName, "entitySetName");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(entityTypeName, "entityTypeName");

            this.ValidateCount(count, "Count");

            this.CreateEntitySetAndTypeSelectorIfNull();

            this.entitySetAndTypeSelector.SetMinimumNumberOfEntities(entitySetName, entityTypeName, count);
        }

        /// <summary>
        /// Sets the minimum number of entities to be created for the specified entity set and type.
        /// </summary>
        /// <param name="entitySet">The entity set.</param>
        /// <param name="entityType">The entity type.</param>
        /// <param name="count">The minimum number of entities to create.</param>
        public void SetMinimumNumberOfEntities(EntitySet entitySet, EntityType entityType, int count)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");

            this.SetMinimumNumberOfEntities(entitySet.Name, entityType.Name,  count);
        }

        /// <summary>
        /// Sets the capacity range selector for the specified association set and end.
        /// </summary>
        /// <param name="associationSetName">The association set name.</param>
        /// <param name="roleName">The role name.</param>
        /// <param name="selector">The capacity range selector.</param>
        public void SetCapacityRangeSelector(string associationSetName, string roleName, Func<CapacityRange> selector)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(associationSetName, "associationSetName");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(roleName, "roleName");
            ExceptionUtilities.CheckArgumentNotNull(selector, "selector");

            this.CreateRelationshipSelectorsIfNull();

            bool found = false;
            foreach (RelationshipSelector rs in this.relationshipSelectors)
            {
                if (rs.SetCapacitySelectorIfApplicable(associationSetName, roleName, selector))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                throw new TaupoArgumentException(
                    string.Format(
                    CultureInfo.InvariantCulture,
                    "Cannot set the selector. Association set '{0}' is not found in the entity container '{1}' or role '{2}' is invalid or there are overlapping foreign keys which are skipped during data population.",
                    associationSetName,
                    this.EntityContainer.Name,
                    roleName));
            }
        }

        /// <summary>
        /// Sets the capacity range selector for the specified association set and end.
        /// </summary>
        /// <param name="associationSet">The association set.</param>
        /// <param name="associationSetEnd">The association set end.</param>
        /// <param name="selector">The capacity range selector.</param>
        public void SetCapacityRangeSelector(AssociationSet associationSet, AssociationSetEnd associationSetEnd, Func<CapacityRange> selector)
        {
            ExceptionUtilities.CheckArgumentNotNull(associationSet, "associationSet");
            ExceptionUtilities.CheckArgumentNotNull(associationSetEnd, "associationSetEnd");

            this.SetCapacityRangeSelector(associationSet.Name, associationSetEnd.AssociationEnd.RoleName, selector);
        }

        /// <summary>
        /// Tries to get next data to upload.
        /// </summary>
        /// <param name="data">The data to upload.</param>
        /// <returns>True if there is a data for upload, false otherwise.</returns>
        public virtual bool TryPopulateNextData(out EntityContainerData data)
        {
            ExceptionUtilities.CheckObjectNotNull(this.StructuralDataServices, "StructuralGenerators cannot be null.");

            this.CreateEntitySetAndTypeSelectorIfNull();
            this.CreateRelationshipSelectorsIfNull();

            if (this.seedData != null)
            {
                data = this.seedData;
                this.seedData = null;
            }
            else
            {
                data = new EntityContainerData(this.EntityContainer);
            }

            int createdEntitiesCount = 0, createdAssociationsCount = 0;
            this.CreateRelationships(data, ref createdEntitiesCount, ref createdAssociationsCount);

            EntitySet entitySet;
            EntityType entityType;
            while (this.GetNextEntitySetAndTypeToCreate(out entitySet, out entityType))
            {
                EntitySetDataRow row = this.PopulateNewEntitySetRow(data, entitySet, entityType);
                createdEntitiesCount++;
                this.ConsiderCandidateForRelationships(row);

                this.CreateRelationships(data, ref createdEntitiesCount, ref createdAssociationsCount);

                if (createdEntitiesCount >= this.ThresholdForNumberOfEntities && this.ThresholdForNumberOfEntities != -1)
                {
                    break;
                }
            }

            if (this.ReferentialConstraintsResolver != null)
            {
                this.ReferentialConstraintsResolver.ResolveReferentialConstraints(data);
            }

            return createdEntitiesCount > 0 || createdAssociationsCount > 0;
        }

        /// <summary>
        /// Resets the current count of entities for each entity set and entity type to 0.
        /// </summary>
        public void Reset()
        {
            if (this.entitySetAndTypeSelector != null)
            {
                this.entitySetAndTypeSelector.ResetCounters();
            }

            if (this.relationshipSelectors != null)
            {
                foreach (RelationshipSelector selector in this.relationshipSelectors)
                {
                    selector.Reset();
                }
            }

            this.entitiesToCreateInNextBatch.Clear();
        }

        private void CreateRelationships(
            EntityContainerData data,
            ref int createdEntitiesCount,
            ref int createdAssociationsCount)
        {
            List<RelationshipDescription> relationshipsToCreate;
            List<KeyValuePair<EntitySet, EntityType>> entitiesToCreateInCurrentBatch;
            int loopCount = 0;
            while (this.GetRelationshipsAndEntitiesToCreate(out relationshipsToCreate, out entitiesToCreateInCurrentBatch))
            {
                loopCount++;

                // there are times when relationships are unable to be created for particular entity type graphs
                // in these situations this check guards against running infinitely long, adding 20
                // ensures that all models get at least 100 loops to generate relationship data, otherwise this 
                // guard might be too low
                if (loopCount > ((this.relationshipSelectors.Count * 4) + 20))
                {
                    throw new TaupoInvalidOperationException("Specified relationship requirements cannot be met. Make sure capacity selectors don't contradict each other.");
                }

                this.PopulateAssociationSetRows(data, relationshipsToCreate);
                createdAssociationsCount += relationshipsToCreate.Count;

                foreach (var setTypePair in entitiesToCreateInCurrentBatch)
                {
                    EntitySetDataRow r = this.PopulateNewEntitySetRow(data, setTypePair.Key, setTypePair.Value);
                    createdEntitiesCount++;
                    this.ConsiderCandidateForRelationships(r);
                }
            }
        }

        private bool GetNextEntitySetAndTypeToCreate(out EntitySet entitySet, out EntityType entityType)
        {
            if (this.entitiesToCreateInNextBatch.Count > 0)
            {
                var entitySetAndType = this.Random.ChooseFrom(this.entitiesToCreateInNextBatch);
                entitySet = entitySetAndType.Key;
                entityType = entitySetAndType.Value;

                return true;
            }
            else
            {
                return this.entitySetAndTypeSelector.TryGetNextEntitySetAndTypeToCreate(out entitySet, out entityType);
            }
        }

        private bool GetRelationshipsAndEntitiesToCreate(
            out List<RelationshipDescription> relationshipsToCreate, 
            out List<KeyValuePair<EntitySet, EntityType>> entitiesToCreateInCurrentBatch)
        {
            relationshipsToCreate = new List<RelationshipDescription>();
            entitiesToCreateInCurrentBatch = new List<KeyValuePair<EntitySet, EntityType>>();
            this.entitiesToCreateInNextBatch.Clear();

            foreach (RelationshipSelector rs in this.relationshipSelectors)
            {
                List<RelationshipDescription> relationships;
                List<AssociationSetEnd> associationSetEndsToCreateEntitiesFor;
                rs.GetRelationshipsToCreate(out relationships, out associationSetEndsToCreateEntitiesFor);

                if (relationships != null)
                {
                    relationshipsToCreate.AddRange(relationships);
                }

                if (associationSetEndsToCreateEntitiesFor != null)
                {
                    foreach (AssociationSetEnd ase in associationSetEndsToCreateEntitiesFor)
                    {
                        EntityType entityType = this.entitySetAndTypeSelector.GetNextEntityTypeForAssociationSetEnd(ase);
                        List<KeyValuePair<EntitySet, EntityType>> entitiesToCreate;
                        if (ase.AssociationEnd.Multiplicity == EndMultiplicity.One)
                        {
                            entitiesToCreate = entitiesToCreateInCurrentBatch;
                        }
                        else
                        {
                            entitiesToCreate = this.entitiesToCreateInNextBatch;
                        }

                        if (!entitiesToCreate.Any(st => st.Key == ase.EntitySet && st.Value == entityType))
                        {
                            entitiesToCreate.Add(new KeyValuePair<EntitySet, EntityType>(ase.EntitySet, entityType));
                        }
                    }
                }
            }

            return relationshipsToCreate.Count > 0 || entitiesToCreateInCurrentBatch.Count > 0;
        }

        private EntitySetDataRow PopulateNewEntitySetRow(EntityContainerData data, EntitySet entitySet, EntityType entityType)
        {
            var entityData = this.StructuralDataServices.GetStructuralGenerator(entityType.FullName, this.EntityContainer.Name + "." + entitySet.Name).GenerateData();

            return this.PopulateEntitySetRow(data, entitySet, entityType, entityData);
        }

        private EntitySetDataRow PopulateEntitySetRow(EntityContainerData data, EntitySet entitySet, EntityType entityType, IEnumerable<NamedValue> entityData)
        {
            EntitySetDataRow row = data[entitySet].AddNewRowOfType(entityType);

            foreach (NamedValue namedValue in entityData)
            {
                row.SetValue(namedValue.Name, namedValue.Value);
            }

            this.entitySetAndTypeSelector.IncrementCount(entitySet, entityType);

            return row;
        }

        private void ConsiderCandidateForRelationships(EntitySetDataRow row)
        {
            foreach (RelationshipSelector rs in this.relationshipSelectors)
            {
                rs.ConsiderCandidate(row.Parent.EntitySet, row.EntityType, row.Key);
            }
        }

        private void PopulateAssociationSetRows(EntityContainerData data, IEnumerable<RelationshipDescription> relationshipsToCreate)
        {
            foreach (RelationshipDescription rd in relationshipsToCreate)
            {
                AssociationSetDataRow row = data[rd.AssociationSet].AddNewRow();
                row.SetRoleKey(rd.ToRoleName, rd.To);
                row.SetRoleKey(rd.FromRoleName, rd.From);
            }
        }

        private void CreateRelationshipSelectorsIfNull()
        {
            if (this.relationshipSelectors == null)
            {
                this.ValidateInputsForCreatingSelectors();
                this.CreateRelationshipSelectors();
            }
        }

        private void CreateEntitySetAndTypeSelectorIfNull()
        {
            if (this.entitySetAndTypeSelector == null)
            {
                this.ValidateInputsForCreatingSelectors();
                this.entitySetAndTypeSelector = new EntitySetAndTypeSelector(this.EntityContainer, this.Random, this.MinimumNumberOfEntitiesPerEntitySet);
                this.entitiesToCreateInNextBatch = new List<KeyValuePair<EntitySet, EntityType>>();
            }
        }

        private void ClearSelectors()
        {
            this.entitySetAndTypeSelector = null;
            this.relationshipSelectors = null;
        }

        private void CreateRelationshipSelectors()
        {
            this.relationshipSelectors = new List<RelationshipSelector>();

            List<AssociationSet> visitedAssociationSets = new List<AssociationSet>();

            foreach (AssociationSet associationSet in this.EntityContainer.AssociationSets.Where(s => !visitedAssociationSets.Contains(s)))
            {
                var identifyingEnds = this.GetAllDependentEndsFromIdentifyingGroup(associationSet);

                if (identifyingEnds.Count > 0)
                {
                    RelationshipSelector selector = new RelationshipSelector(this.Random);

                    RelationshipGroupEnd dependentGroupEnd = new RelationshipGroupEnd(identifyingEnds);
                    selector.Add(dependentGroupEnd, false);

                    foreach (KeyValuePair<AssociationSet, AssociationSetEnd> associationSetAndEndPair in identifyingEnds)
                    {
                        AssociationSetEnd principalSetEnd = associationSetAndEndPair.Key.GetOtherEnd(associationSetAndEndPair.Value);
                        RelationshipGroupEnd principalGroupEnd = new RelationshipGroupEnd(associationSetAndEndPair.Key, principalSetEnd);
                        
                        selector.Add(principalGroupEnd, true);

                        visitedAssociationSets.Add(associationSetAndEndPair.Key);
                    }

                    this.relationshipSelectors.Add(selector);
                }
                else if (!this.HasOverlappingForeignKeys(associationSet))
                {
                    RelationshipSelector selector = new RelationshipSelector(this.Random);
                    
                    foreach (AssociationSetEnd end in associationSet.Ends)
                    {
                        RelationshipGroupEnd groupEnd = new RelationshipGroupEnd(associationSet, end);
                        selector.Add(groupEnd, end.AssociationEnd.Multiplicity == EndMultiplicity.Many && associationSet.GetOtherEnd(end).AssociationEnd.Multiplicity == EndMultiplicity.Many);
                    }
                   
                    this.relationshipSelectors.Add(selector);
                }

                visitedAssociationSets.Add(associationSet);
            }
        }

        private List<KeyValuePair<AssociationSet, AssociationSetEnd>> GetAllDependentEndsFromIdentifyingGroup(AssociationSet associationSet)
        {
            List<KeyValuePair<AssociationSet, AssociationSetEnd>> result = new List<KeyValuePair<AssociationSet, AssociationSetEnd>>();

            if (associationSet.AssociationType.ReferentialConstraint == null ||
                !associationSet.AssociationType.ReferentialConstraint.DependentProperties.Any(p => p.IsPrimaryKey))
            {
                // not part of identifying relationship group.
                return result;
            }

            AssociationEnd dependentEnd = associationSet.AssociationType.ReferentialConstraint.DependentAssociationEnd;
            EntitySet dependentEntitySet = associationSet.Ends.Where(e => e.AssociationEnd == dependentEnd).Single().EntitySet;
            
            List<MemberProperty> primaryKeysWhichAreForeignKeys = new List<MemberProperty>();

            foreach (AssociationSet assocSet in this.EntityContainer.AssociationSets.Where(s => s.AssociationType.ReferentialConstraint != null))
            {
                ReferentialConstraint rc = assocSet.AssociationType.ReferentialConstraint;
                AssociationSetEnd dependentSetEnd = assocSet.Ends.Where(e => e.AssociationEnd == rc.DependentAssociationEnd).Single();
                if (dependentEnd.EntityType.IsKindOf(rc.DependentAssociationEnd.EntityType) && dependentSetEnd.EntitySet == dependentEntitySet)
                {
                    var dependentPrimaryKeys = rc.DependentProperties.Where(p => p.IsPrimaryKey).ToList();

                    if (dependentPrimaryKeys.Count > 0)
                    {
                        primaryKeysWhichAreForeignKeys.AddRange(dependentPrimaryKeys);
                        result.Add(new KeyValuePair<AssociationSet, AssociationSetEnd>(assocSet, dependentSetEnd));
                    }
                }
            }

            int primaryKeysCount = dependentEnd.EntityType.AllKeyProperties.Count();

            if (primaryKeysWhichAreForeignKeys.Count != primaryKeysCount)
            {
                // Not identifying or overlapping.
                result.Clear();
            }

            return result;
        }

        private bool HasOverlappingForeignKeys(AssociationSet associationSet)
        {
            if (associationSet.AssociationType.ReferentialConstraint == null)
            {
                return false;
            }

            foreach (MemberProperty dependentProperty in associationSet.AssociationType.ReferentialConstraint.DependentProperties)
            {
                if (this.entityContainer.Model.Associations.Any(a => a.ReferentialConstraint != null && 
                        a != associationSet.AssociationType && 
                        a.ReferentialConstraint.DependentProperties.Contains(dependentProperty)))
                {
                    return true;
                }
            }

            return false;
        }

        private void ValidateInputsForCreatingSelectors()
        {
            ExceptionUtilities.CheckObjectNotNull(this.EntityContainer, "Entity container cannot be null.");
            ExceptionUtilities.CheckObjectNotNull(this.EntityContainer.Model, "Model schema cannot be null.");
            ExceptionUtilities.CheckObjectNotNull(this.Random, "Random cannot be null.");
        }

        private void ValidateCount(int count, string argumentName)
        {
            if (count < 0)
            {
                throw new TaupoArgumentException(argumentName + " cannot be negative.");
            }
        }
     }
}
