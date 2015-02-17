//---------------------------------------------------------------------
// <copyright file="EntitySetAndTypeSelector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Internal
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// EntitySet and EntityType selector which is used by data population driver.
    /// </summary>
    internal class EntitySetAndTypeSelector
    {
        private Dictionary<string, Dictionary<string, Counter>> counters = new Dictionary<string, Dictionary<string, Counter>>();
        private Dictionary<string, List<EntityType>> entitySetsWithInheritance = new Dictionary<string, List<EntityType>>();
        private EntityContainer entityContainer;
        private IRandomNumberGenerator random;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySetAndTypeSelector"/> class.
        /// </summary>
        /// <param name="entityContainer">The entity container.</param>
        /// <param name="random">The random number generator.</param>
        /// <param name="minimumNumberOfEntitiesPerEntitySet">The minimum number of entities per entity set.</param>
        public EntitySetAndTypeSelector(
            EntityContainer entityContainer,
            IRandomNumberGenerator random,
            int minimumNumberOfEntitiesPerEntitySet)
        {
            this.entityContainer = entityContainer;
            this.random = random;

            this.InitializeTargets(minimumNumberOfEntitiesPerEntitySet);
        }

        /// <summary>
        /// Sets the minimum number of entities to be created for each entity set. 
        /// </summary>
        /// <param name="count">The minimum number of entities to create for each entity set.</param>
        public void SetMinimumNumberOfEntities(int count)
        {
            foreach (ICollection<Counter> counterForEntitySet in this.counters.Values.Select(v => v.Values))
            {
                this.SetTargetCount(counterForEntitySet, count);
            }
        }

        /// <summary>
        /// Sets the minimum number of entities to be created for the specified entity set.
        /// </summary>
        /// <param name="entitySetName">The entity set name.</param>
        /// <param name="count">The minimum number of entities to create.</param>
        public void SetMinimumNumberOfEntities(string entitySetName, int count)
        {
            Dictionary<string, Counter> countersForEntitySet;

            if (!this.counters.TryGetValue(entitySetName, out countersForEntitySet))
            {
                throw new TaupoArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "Entity set '{0}' does not belong to the entity container '{1}' or contains only abstract types.", entitySetName, this.entityContainer.Name));
            }

            ExceptionUtilities.Assert(countersForEntitySet.Count >= 1, "Should not have cached entity set with no concrete types.");

            this.SetTargetCount(countersForEntitySet.Values, count);
        }

        /// <summary>
        /// Sets the minimum number of entities to be created for the specified entity set and type.
        /// </summary>
        /// <param name="entitySetName">The entity set.</param>
        /// <param name="entityTypeName">The entity type name.</param>
        /// <param name="count">The minimum number of entities to create.</param>
        public void SetMinimumNumberOfEntities(string entitySetName, string entityTypeName, int count)
        {
            Dictionary<string, Counter> countersPerType;

            if (!this.counters.TryGetValue(entitySetName, out countersPerType))
            {
                throw new TaupoArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "Entity set '{0}' does not belong to the entity container '{1}' or contains only abstract types.", entitySetName, this.entityContainer.Name));
            }

            Counter counter;
            if (!countersPerType.TryGetValue(entityTypeName, out counter))
            {
                throw new TaupoArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "Entity type '{0}' does not belong to the entity set '{1}' or it is abstract or contains overlapping non-nullable foreign keys.", entityTypeName, entitySetName));
            }

            counter.TargetCount = count;
        }

        /// <summary>
        /// Tries to get next entity set and type to create.
        /// </summary>
        /// <param name="entitySet">The entity set.</param>
        /// <param name="entityType">The entity type.</param>
        /// <returns>True if there is a next entity set and type needs to be created, false otherwise.</returns>
        public bool TryGetNextEntitySetAndTypeToCreate(out EntitySet entitySet, out EntityType entityType)
        {
            entitySet = null;
            entityType = null;

            int maxCountRemaining = 0;
            List<Counter> farthestFromTarget = new List<Counter>();
            foreach (string entitySetName in this.counters.Keys)
            {
                this.FindFarthestFromTarget(entitySetName, ref maxCountRemaining, farthestFromTarget, null);
            }

            Counter counter = null;
            if (farthestFromTarget.Count != 0)
            {
                counter = this.random.ChooseFrom(farthestFromTarget);
                entitySet = counter.EntitySet;
                entityType = counter.EntityType;
            }

            return counter != null;
        }

        /// <summary>
        /// Gets the next entity type for association set end.
        /// </summary>
        /// <param name="associationSetEnd">The association set end.</param>
        /// <returns>Entity type.</returns>
        public EntityType GetNextEntityTypeForAssociationSetEnd(AssociationSetEnd associationSetEnd)
        {
            EntitySet entitySet = associationSetEnd.EntitySet;
            EntityType entityType = associationSetEnd.AssociationEnd.EntityType;

            Counter counter;
            List<EntityType> possibleTypes;
            if (this.entitySetsWithInheritance.TryGetValue(entitySet.Name, out possibleTypes))
            {
                // Take into account associations between subtypes
                possibleTypes = possibleTypes.Where(t => t.IsKindOf(entityType)).ToList();

                List<Counter> farthestFromTarget = new List<Counter>();
                int maxCountRemainig = 0;
                this.FindFarthestFromTarget(entitySet.Name, ref maxCountRemainig, farthestFromTarget, possibleTypes.Select(t => t.Name));

                if (farthestFromTarget.Count > 0)
                {
                    counter = this.random.ChooseFrom(farthestFromTarget);
                    entityType = counter.EntityType;
                }
                else
                {
                    entityType = this.random.ChooseFrom(possibleTypes);
                }
            }

            return entityType;
        }

        /// <summary>
        /// Increments count of entities for the specified entity set and type.
        /// </summary>
        /// <param name="entitySet">The entity set.</param>
        /// <param name="entityType">The entity type.</param>
        public void IncrementCount(EntitySet entitySet, EntityType entityType)
        {
            this.counters[entitySet.Name][entityType.Name].Increment();
        }
        
        /// <summary>
        /// Resets the count for each EntitySet and EntityType to 0.
        /// </summary>
        public void ResetCounters()
        {
            foreach (var countersList in this.counters.Values.Select(v => v.Values))
            {
                foreach (Counter c in countersList)
                {
                    c.Reset();
                }
            }
        }

        private void InitializeTargets(int minimumNumberOfEntitiesPerEntitySet)
        {
            foreach (EntitySet entitySet in this.entityContainer.EntitySets)
            {
                List<EntityType> possibleTypes = this.entityContainer.Model.EntityTypes.Where(
                    t => t.IsKindOf(entitySet.EntityType) && !t.IsAbstract && !this.HasOverlappingNonNullableForeignKeys(entitySet, t)).ToList();

                if (possibleTypes.Count == 0)
                {
                    continue;
                }

                if (possibleTypes.Count > 1 || entitySet.EntityType.IsAbstract)
                {
                    this.entitySetsWithInheritance.Add(entitySet.Name, possibleTypes);
                }

                Dictionary<string, Counter> countersForEntitySet = new Dictionary<string, Counter>();

                foreach (EntityType entityType in possibleTypes)
                {
                    countersForEntitySet.Add(entityType.Name, new Counter(entitySet, entityType));
                }

                this.SetTargetCount(countersForEntitySet.Values, minimumNumberOfEntitiesPerEntitySet);

                this.counters.Add(entitySet.Name, countersForEntitySet);
            }
        }

        private void SetTargetCount(ICollection<Counter> countersForEntitySet, int count)
        {
            int minPerType = count / countersForEntitySet.Count();
            int minForTheFirstType = minPerType + (count % countersForEntitySet.Count);

            int currentCount = minForTheFirstType;
            foreach (var counter in countersForEntitySet)
            {
                counter.TargetCount = currentCount;
                currentCount = minPerType;
            }
        }

        private void FindFarthestFromTarget(string entitySetName, ref int maxCountRemainig, List<Counter> listToUpdate, IEnumerable<string> restrictToEntityTypeNames)
        {
            var countersForSet = this.counters[entitySetName];
            var entityTypeNames = restrictToEntityTypeNames != null ? restrictToEntityTypeNames : countersForSet.Keys;
            foreach (string entityTypeName in entityTypeNames)
            {
                var currentCounter = countersForSet[entityTypeName];
                int currentCountRemaining = currentCounter.TargetCount - currentCounter.Count;

                if (currentCountRemaining <= 0)
                {
                    continue;
                }

                if (currentCountRemaining > maxCountRemainig)
                {
                    maxCountRemainig = currentCountRemaining;
                    listToUpdate.Clear();
                    listToUpdate.Add(currentCounter);
                }
                else if (currentCountRemaining == maxCountRemainig)
                {
                    listToUpdate.Add(currentCounter);
                }
            }
        }

        private bool HasOverlappingNonNullableForeignKeys(EntitySet entitySet, EntityType entityType)
        {
            foreach (AssociationSet associationSet in this.entityContainer.AssociationSets
                .Where(s => s.AssociationType.ReferentialConstraint != null && entityType.IsKindOf(s.AssociationType.ReferentialConstraint.DependentAssociationEnd.EntityType)
                    && s.Ends.Where(e => e.AssociationEnd == s.AssociationType.ReferentialConstraint.DependentAssociationEnd).Single().EntitySet == entitySet))
            {
                foreach (MemberProperty dependentProperty in associationSet.AssociationType.ReferentialConstraint.DependentProperties.Where(p => !p.PropertyType.IsNullable))
                {
                    if (entitySet.Container.Model.Associations.Any(a => a.ReferentialConstraint != null &&
                            a != associationSet.AssociationType &&
                            a.ReferentialConstraint.DependentProperties.Contains(dependentProperty)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Counter for the number of entities of the specific entity type in the entity set.
        /// </summary>
        private class Counter
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Counter"/> class.
            /// </summary>
            /// <param name="entitySet">The entity set.</param>
            /// <param name="entityType">Type of the entity.</param>
            public Counter(EntitySet entitySet, EntityType entityType)
            {
                this.EntitySet = entitySet;
                this.EntityType = entityType;
            }

            /// <summary>
            /// Gets the entity set.
            /// </summary>
            /// <value>The entity set.</value>
            public EntitySet EntitySet { get; private set; }

            /// <summary>
            /// Gets the type of the entity.
            /// </summary>
            /// <value>The type of the entity.</value>
            public EntityType EntityType { get; private set; }

            /// <summary>
            /// Gets the current count.
            /// </summary>
            /// <value>The current count.</value>
            public int Count { get; private set; }

            /// <summary>
            /// Gets or sets the target count.
            /// </summary>
            /// <value>The target count.</value>
            public int TargetCount { get; set; }

            /// <summary>
            /// Increments the current count.
            /// </summary>
            public void Increment()
            {
                this.Count = this.Count + 1;
            }

            /// <summary>
            /// Resets the Count to 0.
            /// </summary>
            public void Reset()
            {
                this.Count = 0;
            }
        }
    }
}
