//---------------------------------------------------------------------
// <copyright file="QueryDataSetSynchronizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.LinqToAstoria
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.WebServices.DataOracleService.DotNet;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Oracle based synchronizer for keeping a query data set in sync
    /// </summary>
    public class QueryDataSetSynchronizer : IOracleBasedDataSynchronizer
    {
        private Dictionary<SerializableEntity, QueryStructuralValue> synchronizationCache =
            new Dictionary<SerializableEntity, QueryStructuralValue>(ReferenceEqualityComparer.Create<SerializableEntity>());

        /// <summary>
        /// Gets or sets the query data set
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryDataSet QueryDataSet { get; set; }

        /// <summary>
        /// Gets or sets the model
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public EntityModelSchema Model { get; set; }

        /// <summary>
        /// Gets or sets the structural value updater
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public INamedValueToQueryValueUpdater StructuralUpdater { get; set; }

        /// <summary>
        /// Gets or sets the convention-based link generator to use
        /// </summary>
        [InjectDependency]
        public IODataConventionBasedLinkGenerator LinkGenerator { get; set; }

        /// <summary>
        /// Gets or sets the data oracle converter
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDataOracleResultConverter DataOracleConverter { get; set; }

        /// <summary>
        /// Synchronizes an entity set given the output from a data oracle
        /// </summary>
        /// <param name="entitySetName">The name of the set to synchronize</param>
        /// <param name="allEntitiesInSet">All of the entities that exist in the set</param>
        public void SynchronizeEntireEntitySet(string entitySetName, IEnumerable<SerializableEntity> allEntitiesInSet)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySetName, "entitySetName");
            ExceptionUtilities.CheckArgumentNotNull(allEntitiesInSet, "allEntitiesInSet");
            ExceptionUtilities.CheckAllRequiredDependencies(this);
            ExceptionUtilities.Assert(allEntitiesInSet.All(e => e.EntitySetName == entitySetName), "Given entities had inconsistent entity set names. Expected '{0}'", entitySetName);

            // reset the cache
            this.synchronizationCache.Clear();

            var currentSet = this.QueryDataSet[entitySetName];
            ExceptionUtilities.CheckObjectNotNull(currentSet, "Entity set with name '{0}' did not exist in query data set", entitySetName);
            var entityType = currentSet.Type.ElementType as QueryEntityType;
            ExceptionUtilities.CheckObjectNotNull(entityType, "Entity set with name '{0}' did not have a structural element type", entitySetName);

            var oldEntityMap = this.GetEntitiesMappedByKey(currentSet.Elements.Cast<QueryStructuralValue>());
            var newEntityMap = this.GetEntitiesMappedByKey(entityType, allEntitiesInSet);
            var deletedKeys = oldEntityMap.Keys.Except(newEntityMap.Keys);

            // update individual entities
            foreach (var entity in allEntitiesInSet)
            {
                // this will add new items as well as synchronizing existing ones
                this.FindAndSynchronizeEntity(entityType, entity, oldEntityMap);
            }

            // any key that is missing from the new list should be treated as a delete, and removed from the data set
            // NOTE: this must happen after the earlier update step, as some of the entities could be linked to the deleted entities
            // and not all IUpdatable implementations will do the cleanup of these dangling links, so the deleted entities will
            // be unintentionally re-added
            foreach (var deletedKey in deletedKeys)
            {
                var entity = oldEntityMap[deletedKey];
                this.RemoveEntity(entity);
            }
        }

        /// <summary>
        /// Synchronizes an entity and its related subgraph given the output from a data oracle
        /// </summary>
        /// <param name="entity">The entity returned from the oracle</param>
        public void SynchronizeEntityInstanceGraph(SerializableEntity entity)
        {
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            // reset the cache
            this.synchronizationCache.Clear();

            // this will add a item or synchronizing an existing one
            this.FindAndSynchronizeEntity(entity);
        }

        private static void InitMemberStreamTypes(QueryEntityType type, QueryStructuralValue structural)
        {
            // initialize named streams
            foreach (var namedStream in type.Properties.Streams())
            {
                AstoriaQueryStreamValue qsv = new AstoriaQueryStreamValue((AstoriaQueryStreamType)namedStream.PropertyType, (byte[])null, null, type.EvaluationStrategy);
                structural.SetStreamValue(namedStream.Name, qsv);
            }
        }

        /// <summary>
        /// Finds and synchronizes the given entity, or add it if 
        /// </summary>
        /// <param name="entity">The entity to find and synchronize</param>
        /// <returns>The synchronized entity</returns>
        private QueryStructuralValue FindAndSynchronizeEntity(SerializableEntity entity)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            ExceptionUtilities.CheckObjectNotNull(entity.EntitySetName, "Given serializable entity did not have its entity set name set");

            var currentSet = this.QueryDataSet[entity.EntitySetName];
            ExceptionUtilities.CheckObjectNotNull(currentSet, "Entity set with name '{0}' did not exist in query data set", entity.EntitySetName);
            var entityType = currentSet.Type.ElementType as QueryEntityType;
            ExceptionUtilities.CheckObjectNotNull(entityType, "Entity set with name '{0}' did not have a structural element type", entity.EntitySetName);

            var oldEntityMap = this.GetEntitiesMappedByKey(currentSet.Elements.Cast<QueryStructuralValue>());
            return this.FindAndSynchronizeEntity(entityType, entity, oldEntityMap);
        }

        /// <summary>
        /// Finds the structural value for the given serializable entity and updates it, or create it if it does not exist
        /// </summary>
        /// <param name="baseType">The base type of the entity's set</param>
        /// <param name="entity">The entity to find and synchronize</param>
        /// <param name="existingEntityGraph">The existing entities in the set, mapped by key</param>
        /// <returns>The synchronized entity</returns>
        private QueryStructuralValue FindAndSynchronizeEntity(QueryEntityType baseType, SerializableEntity entity, IDictionary<EntityDataKey, QueryStructuralValue> existingEntityGraph)
        {
            QueryStructuralValue alreadySynchronized;
            if (this.synchronizationCache.TryGetValue(entity, out alreadySynchronized))
            {
                return alreadySynchronized;
            }

            // if the type does not match, it must be a derived type
            var type = baseType;
            if (type.EntityType.FullName != entity.EntityType)
            {
                type = type.DerivedTypes.Cast<QueryEntityType>().SingleOrDefault(t => t.EntityType.FullName == entity.EntityType);
                ExceptionUtilities.CheckObjectNotNull(type, "Could not find a type named '{0}'", entity.EntityType);
            }

            // compute the key for the entity
            var key = this.GetEntityKey(type, entity);
            QueryStructuralValue structural;
            bool isNewInstance = !existingEntityGraph.TryGetValue(key, out structural);

            // if we don't find it, create it
            if (isNewInstance)
            {
                structural = type.CreateNewInstance();
                InitMemberStreamTypes(type, structural);
            }

            this.synchronizationCache[entity] = structural;
            this.SynchronizeProperties(baseType, entity, type, structural, existingEntityGraph);
            this.SynchronizeStreams(entity, structural);

            structural.MarkDynamicPropertyValues();

            // if its a new instance, add it to the top-level collection
            if (isNewInstance)
            {
                this.QueryDataSet[type.EntitySet.Name].Elements.Add(structural);
                existingEntityGraph[this.GetEntityKey(structural)] = structural;
            }

            return structural;
        }

        private void SynchronizeProperties(QueryEntityType entitySetBaseType, SerializableEntity serializedEntity, QueryEntityType entityType, QueryStructuralValue entity, IDictionary<EntityDataKey, QueryStructuralValue> existingEntityGraph)
        {
            var originalPropertyNames = entity.MemberNames.ToList();
            foreach (string streamName in entity.Type.Properties.Where(p => p.IsStream()).Select(p => p.Name))
            {
                originalPropertyNames.Remove(streamName);
            }

            var nonNavigationValues = new List<NamedValue>();

            // go throught the properties and recursively update any navigations, while saving the non-navigation properties for updating later
            foreach (var namedValue in serializedEntity.Properties)
            {
                NavigationProperty navProp = entityType.EntityType.AllNavigationProperties.Where(np => np.Name == namedValue.Name).SingleOrDefault();
                if (navProp != null)
                {
                    originalPropertyNames.Remove(namedValue.Name);
                    this.SynchronizeNavigationProperty(entitySetBaseType, entity, namedValue, existingEntityGraph);
                }
                else
                {
                    nonNavigationValues.Add(this.DataOracleConverter.Convert(namedValue));
                }
            }

            // synchronize the non-navigation values
            this.StructuralUpdater.UpdateValues(entity, nonNavigationValues);

            // Remove all structural properties that have been s
            string[] structualMemberPropertiesUpdated = this.GetTopLevelPropertyNames(nonNavigationValues);
            foreach (string propertyName in originalPropertyNames)
            {
                if (!structualMemberPropertiesUpdated.Contains(propertyName))
                {
                    entity.RemoveMember(propertyName);
                }
            }
        }

        private void SynchronizeStreams(SerializableEntity serializedEntity, QueryStructuralValue instance)
        {
            if (serializedEntity.Streams != null)
            {
                foreach (var stream in serializedEntity.Streams)
                {
                    if (stream.IsEditLinkBasedOnConvention)
                    {
                        ExceptionUtilities.CheckObjectNotNull(this.LinkGenerator, "Cannot compute convention-based edit link without injected generator");
                        stream.EditLink = this.LinkGenerator.GenerateStreamEditLink(instance, stream.Name);

                        // for the default stream, there must always be a self-link
                        if (stream.Name == null && stream.SelfLink == null)
                        {
                            stream.SelfLink = stream.EditLink;
                        }
                    }

                    instance.SetStreamValue(stream.Name, stream.ContentType, stream.ETag, stream.EditLink, stream.SelfLink, stream.Content);
                }
            }
        }
        
        private void SynchronizeNavigationProperty(QueryEntityType entitySetBaseType, QueryStructuralValue entity, SerializableNamedValue namedValue, IDictionary<EntityDataKey, QueryStructuralValue> existingEntityGraph)
        {
            var reference = namedValue.Value as SerializableEntity;
            if (reference != null)
            {
                QueryStructuralValue synchronized;
                if (reference.EntitySetName == entitySetBaseType.EntitySet.Name)
                {
                    // for self references, we don't need to rebuild the set of existing entities
                    synchronized = this.FindAndSynchronizeEntity(entitySetBaseType, reference, existingEntityGraph);
                }
                else
                {
                    synchronized = this.FindAndSynchronizeEntity(reference);
                }

                entity.SetValue(namedValue.Name, synchronized);
                return;
            }

            var collection = namedValue.Value as IEnumerable<SerializableEntity>;
            if (collection != null)
            {
                var oldCollection = entity.GetCollectionValue(namedValue.Name);

                // clear out the old collection, if it exists
                if (entity.MemberNames.Contains(namedValue.Name))
                {
                    oldCollection.Elements.Clear();
                }
                else
                {
                    // by default we set IsSorted to true, enabling ordering verification.
                    oldCollection = QueryCollectionValue.Create(oldCollection.Type.ElementType, new QueryValue[0], true);
                }

                entity.SetValue(namedValue.Name, oldCollection);

                // go through the new elements, synchronize them, and add them
                foreach (var related in collection)
                {
                    QueryStructuralValue synchronized;
                    if (related.EntitySetName == entitySetBaseType.EntitySet.Name)
                    {
                        // for self references, we don't need to rebuild the set of existing entities
                        synchronized = this.FindAndSynchronizeEntity(entitySetBaseType, related, existingEntityGraph);
                    }
                    else
                    {
                        synchronized = this.FindAndSynchronizeEntity(related);
                    }

                    oldCollection.Elements.Add(synchronized);
                }

                return;
            }

            ExceptionUtilities.Assert(namedValue.Value == null, "Value of navigation property '{0}' was not a reference, a collection of references, or null. Value was '{1}'", namedValue.Name, namedValue.Value);
            entity.SetValue(namedValue.Name, entity.Type.Properties.Single(p => p.Name == namedValue.Name).PropertyType.NullValue);
        }

        private string[] GetTopLevelPropertyNames(IEnumerable<NamedValue> namedValues)
        {
            List<string> topLevelProperties = new List<string>();
            foreach (NamedValue namedValue in namedValues)
            {
                string[] splitNames = namedValue.Name.Split('.');
                ExceptionUtilities.Assert(splitNames.Length > 0, "Error: NamedValue doesn't have first level name after spliting '{0}'", namedValue);
                if (!topLevelProperties.Contains(splitNames[0]))
                {
                    topLevelProperties.Add(splitNames[0]);
                }
            }

            return topLevelProperties.ToArray();
        }

        /// <summary>
        /// Removes all references to the given entity
        /// </summary>
        /// <param name="type">The type of the entity</param>
        /// <param name="entity">The entity to remove</param>
        private void RemoveAllReferences(QueryEntityType type, QueryStructuralValue entity)
        {
            ExceptionUtilities.Assert(type == entity.Type, "QueryStructuralValue must have the same type as what is provided");

            // get all association sets that touch the entity's set
            var associationSets = this.Model.EntityContainers
                .SelectMany(c => c.AssociationSets)
                .Where(a => a.Ends.Any(e => e.EntitySet == type.EntitySet && type.EntityType.IsKindOf(e.AssociationEnd.EntityType)))
                .ToList();

            // get all the navigation properties for each set. Note that there will only be more than one in the case of a self-reference
            var navigationPropertiesBySet = associationSets
                .SelectMany(a => a.Ends)
                .ToLookup(
                    e => e.EntitySet.Name,
                    e => e.AssociationEnd.EntityType.AllNavigationProperties.SingleOrDefault(n => n.FromAssociationEnd == e.AssociationEnd));

            // compute the key of this entity
            var key = this.GetEntityKey(entity);

            // go through each potentially-affected set and remove the entity from the navigations of each element
            foreach (var setNavigations in navigationPropertiesBySet)
            {
                var setName = setNavigations.Key;
                var setContents = this.QueryDataSet[setName].Elements.Cast<QueryStructuralValue>();

                // go through each element in the set
                foreach (var related in setContents)
                {
                    // and remove the entity from each navigation
                    foreach (var navigation in setNavigations.Where(n => n != null))
                    {
                        // We have associations between sub types in the hierarchy so some instances will not have particular associations so we should filter them out
                        var relatedQueryEntityType = related.Type as QueryEntityType;
                        if (relatedQueryEntityType.EntityType.IsKindOf(navigation.FromAssociationEnd.EntityType))
                        {
                            this.RemoveFromNavigationProperty(key, related, navigation);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes the the entity with the given key from the given the given property on the given structural value
        /// </summary>
        /// <param name="keyToRemove">The key of the entity to remove</param>
        /// <param name="related">The structural value to remove from</param>
        /// <param name="navigation">The property to remove from</param>
        private void RemoveFromNavigationProperty(EntityDataKey keyToRemove, QueryStructuralValue related, NavigationProperty navigation)
        {
            if (navigation.ToAssociationEnd.Multiplicity == EndMultiplicity.Many)
            {
                var collection = related.GetCollectionValue(navigation.Name);

                // remove the element with the given key from the collection, if it is present
                foreach (var element in collection.Elements.Cast<QueryStructuralValue>().ToList())
                {
                    if (this.GetEntityKey(element).Equals(keyToRemove))
                    {
                        collection.Elements.Remove(element);
                    }
                }
            }
            else
            {
                // if the value's key matches, set it to null
                var value = related.GetStructuralValue(navigation.Name);
                if (!value.IsNull && this.GetEntityKey(value).Equals(keyToRemove))
                {
                    related.SetValue(navigation.Name, value.Type.NullValue);
                }
            }
        }

        /// <summary>
        /// Removes the given entity from the query data set
        /// Will remove all references to the entity if the provider is relational, and will delete any related objects that are marked as cascade-delete
        /// </summary>
        /// <param name="entity">The entity to remove</param>
        private void RemoveEntity(QueryStructuralValue entity)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            QueryEntityType entityType = entity.Type as QueryEntityType;
            ExceptionUtilities.CheckObjectNotNull(entityType, "Value was not an entity");

            // remove it from the top level set
            this.QueryDataSet[entityType.EntitySet.Name].Elements.Remove(entity);

            // remove all other references to this entity
            this.RemoveAllReferences(entityType, entity);

            // if any of this entity's relationships have cascading deletes, we need to emulate them
            foreach (var navigation in entityType.EntityType.AllNavigationProperties)
            {
                if (navigation.ToAssociationEnd.DeleteBehavior == OperationAction.Cascade)
                {
                    if (navigation.ToAssociationEnd.Multiplicity == EndMultiplicity.Many)
                    {
                        var collection = entity.GetCollectionValue(navigation.Name);

                        // recurse into the elements
                        foreach (var related in collection.Elements.Cast<QueryStructuralValue>())
                        {
                            this.RemoveEntity(related);
                        }
                    }
                    else
                    {
                        // recurse into the reference
                        var reference = entity.GetStructuralValue(navigation.Name);
                        if (!reference.IsNull)
                        {
                            this.RemoveEntity(reference);
                        }
                    }
                }
            }
        }

        private IDictionary<EntityDataKey, SerializableEntity> GetEntitiesMappedByKey(QueryStructuralType type, IEnumerable<SerializableEntity> entities)
        {
            return this.GetEntitiesMappedByKey(entities, e => this.GetEntityKey(type, e));
        }

        private IDictionary<EntityDataKey, QueryStructuralValue> GetEntitiesMappedByKey(IEnumerable<QueryStructuralValue> entities)
        {
            return this.GetEntitiesMappedByKey(entities, this.GetEntityKey);
        }

        private IDictionary<EntityDataKey, TEntity> GetEntitiesMappedByKey<TEntity>(IEnumerable<TEntity> entities, Func<TEntity, EntityDataKey> getKey)
        {
            Dictionary<EntityDataKey, TEntity> dictionary = new Dictionary<EntityDataKey, TEntity>();
            foreach (var entity in entities)
            {
                var key = getKey(entity);
                ExceptionUtilities.Assert(!dictionary.ContainsKey(key), "Duplicate entity key found: {0}", key);
                dictionary.Add(key, entity); // note that we still do Add, just in case
            }

            return dictionary;
        }

        private EntityDataKey GetEntityKey(QueryStructuralType type, SerializableEntity entity)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            return EntityDataKey.CreateFromValues(entity.Properties.Where(p => type.Properties.Any(k => k.IsPrimaryKey && k.Name == p.Name)).Select(p => this.DataOracleConverter.Convert(p)));
        }

        private EntityDataKey GetEntityKey(QueryStructuralValue entity)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            return EntityDataKey.CreateFromValues(entity.Type.Properties.Where(p => p.IsPrimaryKey).Select(p => new NamedValue(p.Name, entity.GetScalarValue(p.Name).Value)));
        }
    }
}
