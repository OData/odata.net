//---------------------------------------------------------------------
// <copyright file="CustomObjectContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Data.Test.Astoria.CustomData.Metadata;
using System.Reflection;

namespace System.Data.Test.Astoria.CustomData.Runtime
{
    // --- CODE SNIPPET START ---

    //
    // CustomObjectContext class
    //
    #region CustomObjectContext class

    public class CustomObjectContext : IDisposable
    {
        #region Private fields

        private CustomMetadataWorkspace _metadata = null;

        private CustomEntitySetContainer _entitySetContainer = null;

        private CustomObjectStateManager _stateManager = null;

        private CustomObjectRelationshipHelper _relationshipHelper = null;

        private EventHandler _onSavingChangesEvent;

        #endregion

        #region Constructors

        public CustomObjectContext(
                        CustomEntitySetContainer entitySetContainer
                    )
        {
            CustomUtils.CheckArgumentNotNull(entitySetContainer, "entitySetContainer");
            
            _entitySetContainer = entitySetContainer;
            _metadata = _entitySetContainer.MetadataWorkspace;

            _stateManager = new CustomObjectStateManager(this);
            _relationshipHelper = new CustomObjectRelationshipHelper(this, _stateManager);

            // NOTE in current implementation we have no anything like "query pipeline" and all queries
            // to the entity sets will be executed directly against underlying "pseudo-store"
            // collections "as is", so in order to track state of the _existing_ data 
            // objects and support correct semantic of the "get changes", "save changes" and
            // "clear changes" operations, we require the objects that could be queried to be
            // "pre-attached" to the context. Changes made using the context including creating
            // new data objects will be automatically monitored by object state manager.
            //
            // NOTE currently we do not support cascading of changes for one-to-one relationships
            // (only any-to-many collections are supported) so keep this limitation in mind while
            // using the context.
            _entitySetContainer.PreAtachExistingDataObjects(this);
        }

        #endregion

        #region Public properties

        public CustomMetadataWorkspace MetadataWorkspace
        {
            get { return _metadata; }
        }

        public event EventHandler SavingChanges
        {
            add { _onSavingChangesEvent += value; }
            remove { _onSavingChangesEvent -= value; }
        }

        #endregion

        #region Internal properties

        internal CustomObjectRelationshipHelper RelationshipHelper
        {
            get { return _relationshipHelper; }
        }

        internal CustomEntitySetContainer EntitySetContainer
        {
            get { return _entitySetContainer; }
        }

        #endregion

        #region Public methods

        public IQueryable<EntityObjectType> CreateQueryOfT<EntityObjectType>(string entitySetName)
        {
            // NOTE in current implementation we assume that existing data objects were "pre-attached" to the context
            // during its initialization.
            return _entitySetContainer.CreateQueryOfT<EntityObjectType>(entitySetName);
        }

        public IQueryable CreateQuery(string entitySetName)
        {
            return _entitySetContainer.CreateQuery(entitySetName);
        }

        public bool TryGetObjectByKey(CustomEntityKey entityKey, out object entityObject)
        {
            CustomUtils.CheckArgumentNotNull(entityKey, "entityKey");

            CustomObjectStateEntry stateEntry;
            if (!_stateManager.TryGetObjectStateEntryByKey(entityKey, out stateEntry))
            {
                entityObject = null;
                return false;
            }

            entityObject = stateEntry.EntityObject;
            return true;
        }

        public bool TryGetObjectStateEntry(object entityObject, out CustomObjectStateEntry stateEntry)
        {
            CustomUtils.CheckArgumentNotNull(entityObject, "entityObject");
            return _stateManager.TryGetObjectStateEntry(entityObject, out stateEntry);
        }

        public object AddObject(string entitySetName, object entityObject)
        {
            CustomEntityType entityType;
            CustomEntitySetType entitySetType;
            ValidateTypeConsistencyForAddOperation(
                                    entitySetName, entityObject,
                                    out entitySetType, out entityType
                                );

            if (_stateManager.IsAlreadyDeletedFromStore(entityObject))
            {
                throw new InvalidOperationException(String.Format(
                                    "The entity object '{0}' being added was already deleted from the store.",
                                    entityObject
                                ));
            }

            var stateEntry = _stateManager.FindObjectStateEntryOrAttach(entityObject, entityType, entitySetType);
            _stateManager.TransitStateWithCascading(stateEntry, CustomStoreOperationKind.Add);

            return entityObject;
        }

        public void DeleteObject(object entityObject)
        {
            CustomUtils.CheckArgumentNotNull(entityObject, "entityObject");

            if (_stateManager.IsAlreadyDeletedFromStore(entityObject))
            {
                return;
            }

            var stateEntry = _stateManager.GetObjectStateEntry(entityObject);

            CustomEntityType entityType = _metadata.GetEntityType(entityObject.GetType());
            Debug.Assert(
                        _metadata.GetEntitySet(stateEntry.EntityKey.EntitySetName)
                            .BaseElementType.IsAssignableFrom(entityType)
                    );

            _stateManager.TransitStateWithCascading(stateEntry, CustomStoreOperationKind.Delete);
        }

        public int SaveAllChanges()
        {
            if (null != _onSavingChangesEvent)
            {
                _onSavingChangesEvent(this, new EventArgs());
            }

            int numOfSavedEntities = _stateManager.SaveAllChanges(_entitySetContainer);
            if (0 < numOfSavedEntities)
            {
                _entitySetContainer.Flush();
            }
            return numOfSavedEntities;
        }

        public int ClearAllChanges()
        {
            return _stateManager.ClearAllChanges();
        }

        public void ResetState()
        {
            _stateManager = new CustomObjectStateManager(this);
            _relationshipHelper = new CustomObjectRelationshipHelper(this, _stateManager);
        }

        #endregion

        #region State reporting

        public IEnumerable<CustomObjectStateEntry> GetPendingChanges()
        {
            return _stateManager.GetStateEntiries(
                                CustomObjectStates.PendingInsertion |
                                CustomObjectStates.PendingDeletion |
                                CustomObjectStates.Changed
                            );
        }

        public IEnumerable<CustomObjectStateEntry> GetStateEntires()
        {
            return GetStateEntires(CustomObjectStates.All);
        }

        public IEnumerable<CustomObjectStateEntry> GetStateEntires(CustomObjectStates statesToInclude)
        {
            return _stateManager.GetStateEntiries(statesToInclude);
        }

        #endregion

        #region Private methods

        private void ValidateTypeConsistencyForAddOperation(
                                    string entitySetName,
                                    object entityObject,
                                    out CustomEntitySetType entitySetType,
                                    out CustomEntityType entityType
                            )
        {
            CustomUtils.CheckArgumentNotNull(entitySetName, "entitySetName");
            CustomUtils.CheckArgumentNotNull(entityObject, "entityObject");

            entityType = _metadata.GetEntityType(entityObject.GetType());
            entitySetType = _metadata.GetEntitySet(entitySetName);

            if (!entitySetType.BaseElementType.IsAssignableFrom(entityType))
            {
                throw new InvalidOperationException(String.Format(
                                    "Entity object '{0}' cannot be stored in entity set '{1}' because " +
                                    "the base element type of the entity set '{2}' is not assignable from " +
                                    "the entity type '{3}'.",
                                    entityObject, entitySetName, entitySetType.BaseElementType, entityType
                                ));
            }
        }

        #endregion

        #region Protected methods

        protected bool IsAttached(object entityObject)
        {
            return _stateManager.IsAttached(entityObject);
        }

        #endregion

        #region Internal methods

        internal int AcceptAllChanges()
        {
            return _stateManager.SaveAllChanges(_entitySetContainer, true /* onlyAcceptChanges */);
        }

        #endregion

        #region Exception throwing helpers

        private void ThrowAnotherObjectIsAlreadyAttached(object anotherEntityObject, CustomEntityKey entityKey)
        {
            throw new InvalidOperationException(String.Format(
                                "Another entity object '{0}' with the same key '{1}' " +
                                "is already attached to the context.",
                                anotherEntityObject, entityKey
                            ));
        }

        private void ThrowConcurrencyExceptionWhenStoreObjectIsAlreadyDeleted(
                                object entityObject,
                                CustomEntityKey entityKey,
                                CustomStoreObjectNotFoundException innerException
                        )
        {
            throw new CustomOptimisticConcurrencyException(
                                String.Format(
                                    "Object '{0}' with the key '{1}' has been already deleted in the store.",
                                    entityObject, entityKey
                                ),
                                innerException
                            );
        }

        #endregion

        #region IDisposable implementation

        public virtual void Dispose()
        {
            // do not clear changes even for verification puposes while object context shares
            // same memory for data objects with data source, because we are not sure when this
            // method could be called and data objects could be already loaded into another context
            // ClearAllChanges();

            _entitySetContainer = null;
            _stateManager = null;
            _relationshipHelper = null;
        }

        #endregion
    }

    #endregion

    //
    // CustomObjectStateManager class
    //
    #region CustomObjectStateManager class

    internal class CustomObjectStateManager
    {
        #region Private fields

        private CustomObjectContext _objectContext = null;

        private IDictionary<object, CustomEntityKey> _entityKeys
                                                = new Dictionary<object, CustomEntityKey>();

        private IDictionary<CustomEntityKey, CustomObjectStateEntry> _stateEntries
                                                        = new Dictionary<CustomEntityKey, CustomObjectStateEntry>();

        private HashSet<CustomObjectStateEntry> _objectsToInsert = new HashSet<CustomObjectStateEntry>();
        private HashSet<CustomObjectStateEntry> _objectsToDeleteOrDetach = new HashSet<CustomObjectStateEntry>();

        private IDictionary<object, CustomObjectStateEntry> _deletedObjects = new Dictionary<object, CustomObjectStateEntry>();

        #endregion

        #region Constructor

        internal CustomObjectStateManager(CustomObjectContext objectContext)
        {
            _objectContext = objectContext;
        }

        #endregion        

        #region Internal state transition and cascading

        internal void TransitStateWithCascading(
                                CustomObjectStateEntry stateEntry,
                                CustomStoreOperationKind operationKind
                            )
        {
            if (TransitStateIfNeeded(stateEntry, operationKind))
            {
                _objectContext.RelationshipHelper.CascadeStoreOperation(
                                                        stateEntry.EntityObject,
                                                        stateEntry.EntityKey.EntityType,
                                                        operationKind
                                                    );
            }
        }

        private bool TransitStateIfNeeded(
                                CustomObjectStateEntry stateEntry,
                                CustomStoreOperationKind operationKind
                            )
        {
            bool cascadingNeeded = false;

            switch (operationKind)
            {
                case CustomStoreOperationKind.Add:
                    if (IsQueuedForDeletionOrDetachment(stateEntry))
                    {
                        bool dequeued = _objectsToDeleteOrDetach.Remove(stateEntry);
                        Debug.Assert(dequeued);

                        cascadingNeeded = true;
                    }

                    if (!stateEntry.IsPersisted)
                    {
                        if (!IsQueuedForInsertion(stateEntry))
                        {
                            bool enqueued = _objectsToInsert.Add(stateEntry);
                            Debug.Assert(enqueued);

                            cascadingNeeded = true;
                        }
                    }
                    else
                    {
                        // nothing should happen if the object is already persisted and
                        // was not queued for deletion
                        // ...
                    }
                    
                    break;
                case CustomStoreOperationKind.Delete:
                    if (IsAlreadyDeletedFromStore(stateEntry))
                    {
                        // no operation for the "zombie" state entry when the object
                        // was already deleted from the store using this context
                        // ...
                    }
                    else
                    {
                        if (IsQueuedForInsertion(stateEntry))
                        {
                            bool dequeued = _objectsToInsert.Remove(stateEntry);
                            Debug.Assert(dequeued);

                            cascadingNeeded = true;
                        }

                        if (!IsQueuedForDeletionOrDetachment(stateEntry))
                        {
                            // object will be detached if it is in transient state or deleted if
                            // it was in persisted state
                            bool enqueued = _objectsToDeleteOrDetach.Add(stateEntry);
                            Debug.Assert(enqueued);

                            cascadingNeeded = true;
                        }
                    }
                    break;
                default:
                    Debug.Fail("Unexpected store operation kind", operationKind.ToString());
                    break;
            }

            return cascadingNeeded;
        }

        internal bool IsAlreadyDeletedFromStore(object entityObject)
        {
            return _deletedObjects.ContainsKey(entityObject);
        }

        internal bool IsAlreadyDeletedFromStore(CustomObjectStateEntry stateEntry)
        {
            if (_deletedObjects.ContainsKey(stateEntry.EntityObject))
            {
                Debug.Assert(!_objectsToDeleteOrDetach.Contains(stateEntry));
                Debug.Assert(!_objectsToInsert.Contains(stateEntry));
                Debug.Assert(stateEntry.IsPersisted);
                return true;
            }
            return false;
        }

        internal bool IsQueuedForInsertion(CustomObjectStateEntry stateEntry)
        {
            if (_objectsToInsert.Contains(stateEntry))
            {
                Debug.Assert(!_deletedObjects.ContainsKey(stateEntry.EntityObject));
                Debug.Assert(!_objectsToDeleteOrDetach.Contains(stateEntry));
                Debug.Assert(!stateEntry.IsPersisted);
                return true;
            }
            return false;
        }

        internal bool IsQueuedForDeletionOrDetachment(CustomObjectStateEntry stateEntry)
        {
            if (_objectsToDeleteOrDetach.Contains(stateEntry))
            {
                Debug.Assert(!_deletedObjects.ContainsKey(stateEntry.EntityObject));
                Debug.Assert(!_objectsToInsert.Contains(stateEntry));
                return true;
            }
            return false;
        }

        #endregion

        #region Public state reporting routine

        public IEnumerable<CustomObjectStateEntry> GetStateEntiries(CustomObjectStates statesToInclude)
        {
            foreach (var entityEntry in _entityKeys)
            {
                object entityObject = entityEntry.Key;
                CustomObjectStateEntry stateEntry = _stateEntries[entityEntry.Value];
                Debug.Assert(Object.ReferenceEquals(stateEntry.EntityObject, entityObject),
                                String.Format(
                                    "object referenced in state entry '{0}'/'{1}' is not the same object as " +
                                    "the one '{2}' referenced in entity key entry; if you see this assert it means that " +
                                    "object state manager is broken; the state manager should be doing " +
                                    "everything possible to guarantee uniqueness and consistency for all " +
                                    "loaded objects, however while \"mock\" object context shares same memory for data objects " +
                                    "with the \"mock\" data source, bad things could happen because the objects could " +
                                    "be changed in another context; could you please extract the steps to " +
                                    "reproduce this problem? do you work with the same data source from " +
                                    "two or more object contexts?",
                                    stateEntry.EntityObject, stateEntry.EntityKey, entityObject
                                )
                            );

                if (IsQueuedForInsertion(stateEntry))
                {
                    Debug.Assert(!stateEntry.IsPersisted);
                    stateEntry.ObjectState = CustomObjectStates.PendingInsertion;

                    if (0 != (CustomObjectStates.PendingInsertion & statesToInclude))
                    {
                        yield return stateEntry;
                    }
                }
                else if (IsQueuedForDeletionOrDetachment(stateEntry))
                {         
                    stateEntry.ObjectState = CustomObjectStates.PendingDeletion;

                    if (0 != (CustomObjectStates.PendingDeletion & statesToInclude))
                    {
                        yield return stateEntry;
                    }
                }
                else
                {
                    Debug.Assert(
                                stateEntry.IsPersisted,
                                "Transient entity object must always be queued either for " +
                                "insertion or detachment."
                            );

                    stateEntry.ObjectState = stateEntry.CheckSnapshot();

                    if (
                            (
                                0 != (CustomObjectStates.Changed & stateEntry.ObjectState & statesToInclude)
                            )
                                ||
                            (
                                0 != (CustomObjectStates.Unchanged & stateEntry.ObjectState & statesToInclude)
                            )
                        )
                    {
                        yield return stateEntry;
                    }
                }
            }
        }

        #endregion

        #region Internal save all changes routine

        internal int SaveAllChanges(CustomEntitySetContainer entitySetContainer)
        {
            return SaveAllChanges(entitySetContainer, false);
        }

        internal int SaveAllChanges(
                            CustomEntitySetContainer entitySetContainer,
                            bool onlyAcceptChanges
                        )
        {
            int numOfAffectedEntities = 0;

            // delete all queued persited entities and just detach the ones in transient state
            foreach (var entryToDelete in _objectsToDeleteOrDetach)
            {
                if (entryToDelete.IsPersisted)
                {
                    if (!onlyAcceptChanges)
                    {
                        entitySetContainer.Delete(entryToDelete.EntityKey.EntitySetName, entryToDelete.EntityObject);
                    }

                    Debug.Assert(!_deletedObjects.ContainsKey(entryToDelete.EntityObject));
                    _deletedObjects.Add(entryToDelete.EntityObject, entryToDelete);
                    entryToDelete.ObjectState = CustomObjectStates.Deleted;
                }

                DetachStateEntryInternal(entryToDelete);
            }

            numOfAffectedEntities += _objectsToDeleteOrDetach.Count;
            _objectsToDeleteOrDetach.Clear();

            // we don't need to sort insert operations with respect to dependencies between primary key
            // properties and related foreign key properties because we deffer promotion of entity keys
            // to the point when all entities are inserted and all primary keys are assigned
            //
            // however if we will implement topological sorting for insert operations we can enable
            // validation of foreign key constraints in the store            
            foreach (var entryToInsert in _objectsToInsert)
            {
                Debug.Assert(!entryToInsert.IsPersisted);
                if (!onlyAcceptChanges)
                {
                    entitySetContainer.Add(entryToInsert.EntityKey.EntitySetName, entryToInsert.EntityObject);
                }
            }
            numOfAffectedEntities += _objectsToInsert.Count;

            // promote all entity keys for all inserted entities; at this point all primary key properties
            // have an assigned store generated value and related foreign key properties in other entities
            // return correct values from referenced primary key properties
            foreach (var entryToInsert in _objectsToInsert)
            {
                PromoteStateEntryInternal(entryToInsert);
            }            

            // send update command for all persisted entities whose data was changed
            foreach (var entityEntry in _entityKeys)
            {
                CustomObjectStateEntry stateEntry = _stateEntries[entityEntry.Value];
                Debug.Assert(Object.ReferenceEquals(stateEntry.EntityObject, entityEntry.Key));

                if (stateEntry.IsPersisted && !_objectsToInsert.Contains(stateEntry))
                {
                    if (0 != (stateEntry.CheckSnapshot() & CustomObjectStates.Changed))
                    {
                        if (!onlyAcceptChanges)
                        {
                            entitySetContainer.Update(stateEntry.EntityKey.EntitySetName, stateEntry.EntityObject);
                        }

                        stateEntry.SynchronizeAfterStoreOperation();
                        numOfAffectedEntities++;
                    }
                }
            }

            _objectsToInsert.Clear();

            return numOfAffectedEntities;
        }

        internal void PromoteStateEntryInternal(CustomObjectStateEntry stateEntry)
        {
            Debug.Assert(!stateEntry.IsPersisted, "Invalid state transition - entity must be already persisted");

            try
            {
                bool removedStateEntry = _stateEntries.Remove(stateEntry.EntityKey);
                Debug.Assert(removedStateEntry);

                stateEntry.SynchronizeAfterStoreOperation();

                Debug.Assert(!stateEntry.EntityKey.IsTemporary);
                Debug.Assert(stateEntry.IsPersisted);
            }
            finally
            {
                if (_stateEntries.ContainsKey(stateEntry.EntityKey))
                {
                    throw new CustomStoreException(String.Format(
                                        "Store has returned a duplicate key '{0}' for the object '{1}' " +
                                        "of an entity type '{2}'. Another object with the same key " +
                                        "is already attached to the state manager. Verify " +
                                        "\"assign store generated values\" code for " +
                                        "primitive types used in the key for '{2}'. " +
                                        "The confilict could be also caused by entities with a " +
                                        "primary key composed of multiple foreign keys when " +
                                        "entities are referencing same primary ends in " +
                                        "many-to-one associations.",
                                        stateEntry.EntityKey, stateEntry.EntityObject,
                                        stateEntry.EntityKey.EntityType
                                    ));
                }
                _stateEntries[stateEntry.EntityKey] = stateEntry;
            }
        }

        internal void DetachStateEntryInternal(CustomObjectStateEntry stateEntry)
        {
            bool entityKeyRemoved = _entityKeys.Remove(stateEntry.EntityObject);
            Debug.Assert(entityKeyRemoved);

            bool stateEntryRemoved = _stateEntries.Remove(stateEntry.EntityKey);
            Debug.Assert(stateEntryRemoved);

            _objectContext.RelationshipHelper.DetachTrackingCollections(stateEntry.EntityObject, stateEntry.EntityKey.EntityType);
        }

        #endregion

        #region Internal clear all changes routine

        internal int ClearAllChanges()
        {
            int numOfAffectedEntities = 0;

            foreach (var entityEntry in _entityKeys)
            {
                CustomObjectStateEntry stateEntry = _stateEntries[entityEntry.Value];
                Debug.Assert(Object.ReferenceEquals(stateEntry.EntityObject, entityEntry.Key));

                if (stateEntry.IsPersisted)
                {
                    if (0 != (stateEntry.CheckSnapshot() & CustomObjectStates.Changed))
                    {
                        stateEntry.RestoreStateFromSnapshot();
                        if (!_objectsToDeleteOrDetach.Contains(stateEntry))
                        {
                            numOfAffectedEntities++;
                        }
                    }
                }
            }

            numOfAffectedEntities += _objectsToDeleteOrDetach.Count;
            _objectsToDeleteOrDetach.Clear();

            foreach (var entryToInsert in _objectsToInsert)
            {
                Debug.Assert(!entryToInsert.IsPersisted);
                DetachStateEntryInternal(entryToInsert);
                numOfAffectedEntities++;
            }

            _objectsToInsert.Clear();            

            return numOfAffectedEntities;
        }

        #endregion

        #region State entry access

        internal bool TryGetObjectStateEntryByKey(CustomEntityKey entityKey, out CustomObjectStateEntry stateEntry)
        {
            if(_stateEntries.TryGetValue(entityKey, out stateEntry))
            {
                return true;
            }
            return false;
        }

        internal bool TryGetObjectStateEntry(object entityObject, out CustomObjectStateEntry stateEntry)
        {
            Debug.Assert(null != entityObject);

            CustomEntityKey entityKey;
            if (!_entityKeys.TryGetValue(entityObject, out entityKey))
            {
                stateEntry = null;
                return false;
            }

            CustomObjectStateEntry existingStateEntry;
            if (_stateEntries.TryGetValue(entityKey, out existingStateEntry))
            {
                CustomEntityType entityType = _objectContext.MetadataWorkspace.GetEntityType(entityObject.GetType());
                Debug.Assert(entityType.ClrObjectType == entityObject.GetType());

                if (!Object.ReferenceEquals(entityObject, existingStateEntry.EntityObject))
                {
                    ThrowAnotherObjectIsAlreadyAttached(existingStateEntry.EntityObject, entityKey);
                }
            }

            stateEntry = existingStateEntry;
            return true;
        }

        internal CustomObjectStateEntry GetObjectStateEntry(object entityObject)
        {
            CustomObjectStateEntry existingStateEntry;
            if (!TryGetObjectStateEntry(entityObject, out existingStateEntry))
            {
                throw new InvalidOperationException(String.Format(
                        "The given object '{0}' is either not a valid entity object " +
                        "or is not attached to the current object context.",
                        entityObject
                    ));
            }
            return existingStateEntry;
        }

        internal CustomObjectStateEntry FindObjectStateEntryOrAttach(
                                                    object entityObject,
                                                    CustomEntityType entityType,
                                                    CustomEntitySetType entitySetType
                                                )
        {
            Debug.Assert(null != entityObject);
            Debug.Assert(null != entitySetType.Name);
            Debug.Assert(entitySetType.BaseElementType.IsAssignableFrom(entityType));

            CustomEntityKey tempEntityKey = null;
            if (!_entityKeys.TryGetValue(entityObject, out tempEntityKey))
            {
                tempEntityKey = new CustomEntityKey(entityType, entitySetType.Name);
                tempEntityKey.RefreshKeyValues(entityObject);
            }
            else
            {
                if (!tempEntityKey.EntitySetName.Equals(entitySetType.Name))
                {
                    throw new NotImplementedException(String.Format(
                                        "Unsupported operation: entity '{0}' cannot be added to another entity set " +
                                        "than the one specified when the object was added to the context first time. " +
                                        "Expected: '{1}'. Actual: '{2}'. This version does not support this operation. " +
                                        "Try to modify the model to have only one possible entity set per type.",
                                        entityObject, tempEntityKey.EntitySetName, entitySetType.Name
                                    ));
                }
            }

            CustomObjectStateEntry existingStateEntry;
            if (_stateEntries.TryGetValue(tempEntityKey, out existingStateEntry))
            {
                if (!Object.ReferenceEquals(entityObject, existingStateEntry.EntityObject))
                {
                    ThrowAnotherObjectIsAlreadyAttached(existingStateEntry.EntityObject, tempEntityKey);
                }

                Debug.Assert(_entityKeys.ContainsKey(entityObject));
                Debug.Assert(_entityKeys[entityObject].Equals(existingStateEntry.EntityKey));
                Debug.Assert(IsAttached(entityObject));
            }
            else
            {
                existingStateEntry = new CustomObjectStateEntry(tempEntityKey, entityObject, this);

                _stateEntries.Add(tempEntityKey, existingStateEntry);
                _entityKeys.Add(entityObject, tempEntityKey);

                _objectContext.RelationshipHelper.CreateOrReuseChangeTrackingCollections(entityObject, entityType);
            }

            return existingStateEntry;
        }

        internal bool IsAttached(object entityObject)
        {
            CustomObjectStateEntry stateEntry;
            if (TryGetObjectStateEntry(entityObject, out stateEntry))
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Exception throwing helpers

        private void ThrowAnotherObjectIsAlreadyAttached(object anotherEntityObject, CustomEntityKey entityKey)
        {
            throw new InvalidOperationException(String.Format(
                                "Another entity object '{0}' with the same key '{1}' " +
                                "is already attached to the context.",
                                anotherEntityObject, entityKey
                            ));
        }

        #endregion
    }

    #endregion

    //
    // CustomObjectRelationshipHelper class
    //
    #region CustomObjectRelationshipHelper class

    /// <remarks>
    /// Ideally it should not be a set of helper methods to handle relationships, but
    /// a manager kind class like object state manager that will encapsulate all the
    /// logic behind processing relationships.
    /// </remarks>
    public class CustomObjectRelationshipHelper
    {
        #region Private fields

        private CustomObjectContext _objectContext;

        private CustomObjectStateManager _stateManager;

        private HashSet<CustomRelationshipEnd> _visitedRelationshipEnds = new HashSet<CustomRelationshipEnd>();

        #endregion

        #region Constructor

        internal CustomObjectRelationshipHelper(
                            CustomObjectContext objectContext,
                            CustomObjectStateManager stateManager
                        )
        {
            _objectContext = objectContext;
            _stateManager = stateManager;
        }

        #endregion

        #region Change cascading helpers

        internal void CascadeStoreOperation(
                                object entityObject,
                                CustomEntityType entityType,
                                CustomStoreOperationKind operationKind
                            )
        {
            foreach (var navigation in entityType.NavigationProperties)
            {
                object reference = navigation.GetValue(entityObject);
                if (null != reference && reference is ICustomRelationshipManager)
                {
                    ICustomRelationshipManager relationshipManager = (ICustomRelationshipManager)reference;
                    if (!relationshipManager.IsAttachedToContext(_objectContext))
                    {
                        relationshipManager.AttachToContext(_objectContext);
                    }

                    relationshipManager.CascadeStoreOperation(operationKind);
                }
            }
        }

        internal void AddToVisitedRelationshipEnds(CustomRelationshipEnd relationshipEnd)
        {
            bool added = _visitedRelationshipEnds.Add(relationshipEnd);
            Debug.Assert(added);
        }

        internal void RemoveFromVisitedRelationshipEnds(CustomRelationshipEnd relationshipEnd)
        {
            bool removed = _visitedRelationshipEnds.Remove(relationshipEnd);
            Debug.Assert(removed);
        }

        internal bool IsVisitedRelationshipEnd(CustomRelationshipEnd relationshipEnd)
        {
            return _visitedRelationshipEnds.Contains(relationshipEnd);
        }

        internal bool IsTransietObject(object entityObject)
        {
            CustomObjectStateEntry stateEntry = _stateManager.GetObjectStateEntry(entityObject);
            return !stateEntry.IsPersisted;
        }

        #endregion

        #region Tracking collections initialization and cleanup

        internal void DetachTrackingCollections(object entityObject, CustomEntityType entityType)
        {
            foreach (var navigation in entityType.NavigationProperties)
            {
                object reference = navigation.GetValue(entityObject);
                if (null != reference && reference is ICustomRelationshipManager)
                {
                    if (((ICustomRelationshipManager)reference).IsAttachedToContext(_objectContext))
                    {
                        ((ICustomRelationshipManager)reference).DeattachFromContext();
                    }
                }
            }
        }

        internal void CreateOrReuseChangeTrackingCollections(object entityObject, CustomEntityType entityType)
        {
            foreach (var navigation in entityType.NavigationProperties)
            {
                //
                // NOTE for now we track automatically only any-to-many associations
                //

                if (CustomMultiplicity.Many == navigation.To.Multiplicity)
                {
                    AttachCollection(entityObject, navigation);
                }
            }
        }

        private void AttachCollection(object entityObject, CustomNavigationPropertyType navigation)
        {
            Debug.Assert(CustomMultiplicity.Many == navigation.To.Multiplicity);
            object currentCollection = navigation.GetValue(entityObject);

            if (null != currentCollection && currentCollection is CustomObjectCollectionBase)
            {
                // Reuse existing "context-aware" object collection
                //

                CustomObjectCollectionBase currentManagedCollection = ((CustomObjectCollectionBase)currentCollection);
                Debug.Assert(currentManagedCollection is ICustomRelationshipManager);

                ICustomRelationshipManager relationshipManager = (ICustomRelationshipManager)currentManagedCollection;
                if (!relationshipManager.IsAttachedToContext(_objectContext))
                {
                    relationshipManager.AttachToContext(_objectContext);
                    relationshipManager.CascadeStoreOperation(CustomStoreOperationKind.Add);
                }                
            }
            else
            {
                // Create new "context-aware" any-to-many collection
                //

                Type thisEndObjectClrType = navigation.From.EntityType.ClrObjectType;
                Type relatedEndObjectClrType = navigation.To.EntityType.ClrObjectType;
                Type anyToManyCollectionType;

                if (CustomMultiplicity.Many == navigation.From.Multiplicity)
                {
                    anyToManyCollectionType = typeof(CustomManyToManyObjectCollection<,>);
                }
                else
                {
                    Debug.Assert(CustomMultiplicity.One == navigation.From.Multiplicity);
                    anyToManyCollectionType = typeof(CustomOneToManyObjectCollection<,>);
                }

                anyToManyCollectionType = anyToManyCollectionType.MakeGenericType(
                                                    thisEndObjectClrType,
                                                    relatedEndObjectClrType
                                                );

                CustomObjectCollectionBase newManagedCollection
                            = (CustomObjectCollectionBase)Activator.CreateInstance(
                                                                anyToManyCollectionType,
                                                                entityObject,
                                                                navigation
                                                            );
                navigation.SetValue(entityObject, newManagedCollection);
                

                Debug.Assert(newManagedCollection is ICustomRelationshipManager);
                ((ICustomRelationshipManager)newManagedCollection).AttachToContext(_objectContext);                

                // Copy entity objects from old "unmanaged" collection if exist
                //

                if (null != currentCollection)
                {
                    Debug.Assert(currentCollection is IEnumerable);
                    
                    foreach (object obj in (IEnumerable) currentCollection)
                    {
                        newManagedCollection.Add(obj);
                    }

                    newManagedCollection.ForgotChanges();
                }                
            }
        }

        #endregion
    }

    #endregion

    //
    // CustomOptimisticConcurrencyException class
    //
    #region CustomOptimisticConcurrencyException class

    public class CustomOptimisticConcurrencyException : Exception
    {
        public CustomOptimisticConcurrencyException()
        {
        }

        public CustomOptimisticConcurrencyException(string message)
                    : base(message)
        {
        }

        public CustomOptimisticConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    #endregion

    //
    // CustomCollectionChangeKind enum
    //
    #region CustomCollectionChangeKind enum

    public enum CustomCollectionChangeKind
    {
        Added,
        Deleted
    }

    #endregion

    //
    // ICustomCollectionWithChangeTracking interface
    //
    #region ICustomCollectionWithChangeTracking interface

    public interface ICustomCollectionWithChangeTracking
    {
        bool IsDirty { get; }

        IEnumerable<KeyValuePair<object, CustomCollectionChangeKind>> GetChangeHistory();

        void RevertChanges();

        void ForgotChanges();
    }

    #endregion

    //
    // ICustomRelationshipManager interface
    //
    #region ICustomRelationshipManager interface

    public interface ICustomRelationshipManager
    {
        bool IsAttachedToContext(CustomObjectContext objectContext);

        void AttachToContext(CustomObjectContext objectContext);

        void DeattachFromContext();

        void CascadeStoreOperation(CustomStoreOperationKind changeKind);
    }

    #endregion

    //
    // CustomRelationshipEnd class
    //
    #region CustomRelationshipEnd class

    internal struct CustomRelationshipEnd : IEquatable<CustomRelationshipEnd>
    {
        #region Private fields

        private object _entityObject;
        private CustomRelationshipEndType _relationshipEndType;

        #endregion

        #region Constructor

        public CustomRelationshipEnd(object entityObject, CustomRelationshipEndType relationshipEndType)
        {
            _entityObject = entityObject;
            _relationshipEndType = relationshipEndType;
        }

        #endregion

        #region IEquatable implementation

        public bool Equals(CustomRelationshipEnd otherEnd)
        {
            if (
                    Object.ReferenceEquals(this._entityObject, otherEnd._entityObject)
                    &&
                    Object.ReferenceEquals(
                            this._relationshipEndType.RelatedProperty.ClrPropertyInfo,
                            otherEnd._relationshipEndType.RelatedProperty.ClrPropertyInfo
                        )
                )
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Equals / GetHashCode

        public override bool Equals(object obj)
        {
            return obj is CustomRelationshipEnd && Equals((CustomRelationshipEnd)obj);
        }

        public override int GetHashCode()
        {
            return _entityObject.GetHashCode() ^ _relationshipEndType.RelatedProperty.ClrPropertyInfo.GetHashCode();
        }

        #endregion
    }

    #endregion

    //
    // CustomObjectStates enum
    //
    #region CustomObjectStates enum

    [Flags]
    public enum CustomObjectStates
    {
        //
        // these flags should be used only to report a user code of the object state manager
        // about pending changes; they should not be used by state manager in decision making
        // process; the state manager use its own internal predicates making decisions about
        // transitions between states
        //

        New                 = 0x00000,

        PendingInsertion    = 0x00001,
        PendingDeletion     = 0x00002,
        
        Changed             = 0x00004,
        Unchanged           = 0x00008,

        Deleted             = 0x00010,

        Persisted           = Changed | Unchanged,
        Attached            = New | Persisted | PendingInsertion | PendingDeletion,
        All                 = Attached | Deleted
    }

    #endregion

    //
    // CustomStoreOperationKind enum
    //
    #region CustomStoreOperationKind enum

    public enum CustomStoreOperationKind
    {
        Add,
        Delete
    }

    #endregion

    //
    // CustomObjectStateEntry class
    //
    #region CustomObjectStateEntry class

    public class CustomObjectStateEntry
    {
        #region Private fields

        private bool _isPersisted;
        private CustomEntityKey _entityKey;

        private object _entityObject;
        private CustomObjectStateManager _stateManager;

        private CustomObjectDataSnapshot _dataSnapshot;

        private CustomObjectStates _objectState;    // state flags are used only for reporting purposes
                                                    // and are initialized by special change
                                                    // reporting routine

        #endregion

        #region Constructors

        internal CustomObjectStateEntry(
                        CustomEntityKey entityKey,
                        object entityObject,
                        CustomObjectStateManager stateManager
                    )
        {
            Debug.Assert(null != entityKey);
            Debug.Assert(null != entityObject);
            Debug.Assert(null != stateManager);
            
            _entityKey = entityKey;
            _entityObject = entityObject;
            _stateManager = stateManager;

            _isPersisted = false;
            _dataSnapshot = null;
        }

        #endregion

        #region Properties

        public bool IsPersisted
        {
            get
            {
                Debug.Assert(!_isPersisted || !_entityKey.IsTemporary && null != _dataSnapshot);
                return _isPersisted;
            }
        }

        public CustomEntityKey EntityKey
        {
            get
            {
                Debug.Assert(!_entityKey.IsTemporary || !_isPersisted && null == _dataSnapshot);
                return _entityKey;
            }
        }

        public object EntityObject
        {
            get { return _entityObject; }
        }

        public CustomObjectStates ObjectState
        {
            set { _objectState = value; }
            get { return _objectState; }
        }

        #endregion

        #region Methods

        internal void SynchronizeAfterStoreOperation()
        {
            if (!IsPersisted)
            {
                // it was insertion of entity with no assigned primary key; verify that
                // the key was promoted
                bool keyAssigned = _entityKey.RefreshKeyValues(_entityObject);
                if (!keyAssigned)
                {
                    throw new CustomStoreException(String.Format(
                                            "Entity key for object '{0}' was not assigned after " +
                                            "the store operation.",
                                            _entityObject
                                        ));
                }
                Debug.Assert(!_entityKey.IsTemporary);
            }

            // take new snapshot of the entity state
            _isPersisted = true;
            _dataSnapshot = CustomObjectDataSnapshot.Create(_entityKey.EntityType, _entityObject);

            // clear change history in all collections; their state is synchronized now with the store
            foreach (var member in _entityKey.EntityType.Members)
            {
                object value = member.GetValue(_entityObject);
                if (null != value && value is ICustomCollectionWithChangeTracking)
                {
                    ((ICustomCollectionWithChangeTracking)value).ForgotChanges();
                }
            }
        }

        internal void RestoreStateFromSnapshot()
        {
            Debug.Assert(IsPersisted);

            _objectState = CustomObjectStates.Unchanged;
            _dataSnapshot.Restore();
        }

        internal CustomObjectStates CheckSnapshot()
        {
            Debug.Assert(IsPersisted);

            _objectState &= ~CustomObjectStates.Persisted;
            _objectState = _dataSnapshot.ReportChanges();

            return _objectState;
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("Key=");
            buf.Append(_entityKey.ToString());
            
            buf.Append(", State=");
            buf.Append(_objectState.ToString());    // correct state flags should be set by
                                                    // user-level change reporting routine

            return buf.ToString();
        }

        #endregion
    }

    #endregion

    //
    // CustomObjectDataSnapshot class
    //
    #region CustomObjectDataSnapshot class

    internal class CustomObjectDataSnapshot
    {
        #region Private fields

        private CustomEntityType _entityType;
        private object _entityObject;
        private object _snapshotObject;

        #endregion

        #region Private constructor

        private CustomObjectDataSnapshot(
                        CustomEntityType entityType,
                        object entityObject
                    )
        {
            Debug.Assert(null != entityType);
            Debug.Assert(null != entityObject);
            Debug.Assert(entityType.ClrObjectType.Equals(entityObject.GetType()));

            _entityType = entityType;
            _entityObject = entityObject;

            ConstructSnapshotInternal();
        }

        #endregion

        #region Create / Restore / Check snapshot

        public static CustomObjectDataSnapshot Create(
                                                    CustomEntityType entityType,
                                                    object entityObject
                                                )
        {
            return new CustomObjectDataSnapshot(entityType, entityObject);
        }

        public void Update()
        {
            ConstructSnapshotInternal();
        }

        public void Restore()
        {
            foreach (CustomMemberType memberType in _entityType.Members)
            {
                if (memberType.IsReadOnly)
                {
                    continue;
                }

                object currValue = memberType.GetValue(_entityObject);
                if (null != currValue && currValue is ICustomCollectionWithChangeTracking)
                {
                    ((ICustomCollectionWithChangeTracking)currValue).RevertChanges();
                }
                else
                {
                    // We guarantee that all any-to-many collections of attached objects
                    // always exist and implement its own change tracking mechanism
                    Debug.Assert(!(memberType is CustomNavigationPropertyType
                            && ((CustomNavigationPropertyType)memberType).To.Multiplicity == CustomMultiplicity.Many));

                    memberType.SetValue(_entityObject, memberType.GetValue(_snapshotObject));
                }
            }
        }

        public CustomObjectStates ReportChanges()
        {
            foreach (CustomMemberType memberType in _entityType.Members)
            {
                if (!memberType.IsReadOnly && IsDirty(memberType))
                {
                    return CustomObjectStates.Changed;
                }
            }

            return CustomObjectStates.Unchanged;
        }


        private bool IsDirty(CustomMemberType memberType)
        {
            object currValue = memberType.GetValue(_entityObject);

            if (null != currValue && currValue is ICustomCollectionWithChangeTracking)
            {
                return ((ICustomCollectionWithChangeTracking)currValue).IsDirty;
            }

            return !Object.Equals(currValue, memberType.GetValue(_snapshotObject));
        }

        private void ConstructSnapshotInternal()
        {
            _snapshotObject = Activator.CreateInstance(_entityObject.GetType());
            foreach (CustomMemberType memberType in _entityType.Members)
            {
                if (memberType.IsReadOnly)
                {
                    continue;
                }

                object currValue = memberType.GetValue(_entityObject);

                if (null != currValue && currValue is ICustomCollectionWithChangeTracking)
                {
                    // Skipping object implementing its own change tracking mechanism
                    continue;
                }

                if (
                    memberType is CustomNavigationPropertyType &&
                    CustomMultiplicity.Many == ((CustomNavigationPropertyType)memberType).To.Multiplicity)
                {
                    // This is a property of type any-to-many collection. We guarantee that
                    // all collections of the related objects will be always wrapped into
                    // our own internal collection with its own builtin change tracking mechanism
                    // when the attachment process for the entity will complete. We dont' need
                    // to include this property into snapshot.
                    continue;
                }

                if (memberType is CustomPropertyType)
                {
                    memberType.SetValue(_snapshotObject, currValue);
                }
                else if (memberType is CustomNavigationPropertyType)
                {
                    var navProperty = (CustomNavigationPropertyType)memberType;
                    Debug.Assert(CustomMultiplicity.One == navProperty.To.Multiplicity);
                    memberType.SetValue(_snapshotObject, currValue);
                 }
            }
        }

        #endregion
    }

    #endregion

    //
    // CustomEntityKey class
    //
    #region CustomEntityKey class

    public class CustomEntityKey
    {
        #region Private fields

        private CustomEntityType _entityType;

        private string _entitySetName;

        private object[] _keyValues;

        private string _temporaryId;

        int? _precomputedHashCode;

        string _objToString;

        #endregion

        #region Constructors

        public CustomEntityKey(CustomEntityType entityType, string entitySetName)
        {
            CustomUtils.CheckArgumentNotNull(entityType, "entityType");
            CustomUtils.CheckArgumentNotNull(entitySetName, "entitySetName");

            if (0 >= entityType.PrimaryKeyProperties.Count)
            {
                throw new ArgumentException(String.Format(
                            "Entity type '{0}' doesn't define primary key property.",
                            entityType.Name
                        ));
            }

            _entityType = entityType;
            _entitySetName = entitySetName;
            _temporaryId = Guid.NewGuid().ToString().Substring(0, 16);
        }

        public CustomEntityKey(CustomEntityType entityType, string entitySetName, KeyValuePair<string, object>[] keyValues)
                    : this(entityType, entitySetName)
        {
            CustomUtils.CheckArgumentNotNull(keyValues, "keyValues");

            if (keyValues.Length != entityType.PrimaryKeyProperties.Count)
            {
                throw new ArgumentException(String.Format(
                                    "Wrong number of values for primary key for entity type '{0}'. " +
                                    "Expected: {1}. Actual: {2}.",
                                    entityType, entityType.PrimaryKeyProperties.Count, keyValues.Length
                                ));
            }

            object[] orderedKeyValues = new object[_entityType.PrimaryKeyProperties.Count];
            int keyIdx = 0;

            foreach (var keyProperty in _entityType.PrimaryKeyProperties)
            {
                var matchingValues = keyValues.Where(p => p.Key.Equals(keyProperty.Name))
                                        .Select(p => p.Value);

                if (1 < matchingValues.Count())
                {
                    throw new ArgumentException(String.Format(
                                        "Duplicate value for key property '{0}'.",
                                        keyProperty.Name
                                ));
                }

                object keyValue = matchingValues.SingleOrDefault();
                if (null == keyValue || IsDefaultKeyValue(keyProperty, keyValue))
                {
                    throw new ArgumentException(String.Format(
                                    "Provided value for key property '{0}' cannot be null or have its default CLR value.",
                                    keyProperty.Name
                                ));
                }

                if (!keyProperty.ClrType.Equals(keyValue.GetType()))
                {
                    throw new ArgumentException(String.Format(
                                    "Type of value '{0}' provided for key property '{1}' doesn't match with " +
                                    "the expected type of the property '{2}'.",
                                    keyValue.GetType().Name, keyProperty.Name, keyProperty.ClrType.Name
                                ));
                }

                orderedKeyValues[keyIdx++] = keyValue;
            }

            AssignKeyValues(orderedKeyValues);
        }

        #endregion

        #region Properties

        public bool IsTemporary
        {
            get { return _keyValues == null; }
        }

        public CustomEntityType EntityType
        {
            get { return _entityType; }
        }

        public string EntitySetName
        {
            get { return _entitySetName; }
        }

        #endregion

        #region Key values manipulation routines

        public object[] GetKeyValues()
        {
            return (object[])_keyValues.Clone();
        }

        public bool RefreshKeyValues(object entityObject)
        {
            Debug.Assert(_entityType.ClrObjectType.Equals(entityObject.GetType()));

            object[] keyValues = null;
            if (TryGetKeyValues(_entityType, entityObject, out keyValues))
            {
                AssignKeyValues(keyValues);
                return true;
            }
            return false;
        }

        private void AssignKeyValues(object[] keyValues)
        {
            Debug.Assert(keyValues.Length == _entityType.PrimaryKeyProperties.Count);
            _keyValues = new object[keyValues.Length];

            int keyIdx = 0;
            foreach (CustomPropertyType keyProperty in _entityType.PrimaryKeyProperties)
            {
                Debug.Assert(null != keyValues[keyIdx]);
                Debug.Assert(keyValues[keyIdx].GetType().Equals(keyProperty.ClrType));
                Debug.Assert(!IsDefaultKeyValue(keyProperty, keyValues[keyIdx]));

                _keyValues[keyIdx] = keyValues[keyIdx];
                keyIdx++;
            }

            _objToString = null;
        }

        private static bool TryGetKeyValues(CustomEntityType entityType, object entityObject, out object[] keyValues)
        {
            keyValues = new object[entityType.PrimaryKeyProperties.Count];
            int keyIdx = 0;

            foreach (CustomPropertyType keyProperty in entityType.PrimaryKeyProperties)
            {
                object keyValue = keyProperty.GetValue(entityObject);
                
                if (IsDefaultKeyValue(keyProperty, keyValue))
                {
                    keyValues = null;
                    return false;
                }

                keyValues[keyIdx++] = keyValue;
            }

            return true;
        }

        internal static bool IsDefaultKeyValue(CustomPropertyType keyProperty, object keyValue)
        {
            //
            // NOTE for simplicity assume that the assigned store generated key value
            // is never equal to the default value of the corresponding CLR type
            //

            object defaultValue = null;

            if(
                    keyProperty.ClrType.IsPrimitive ||
                    keyProperty.ClrType.Equals(typeof(Decimal)) ||
                    keyProperty.ClrType.Equals(typeof(Double)) ||
                    keyProperty.ClrType.Equals(typeof(Single))
                )
            {
                defaultValue = Convert.ChangeType(0, keyProperty.ClrType);
            }
            else if (keyProperty.ClrType.IsClass || keyProperty.ClrType.IsInterface)
            {
                defaultValue = null;
            }
            else if(keyProperty.ClrType.Equals(typeof(DateTime)))
            {
                defaultValue = DateTime.MinValue;
            }
            else if (keyProperty.ClrType.Equals(typeof(Guid)))
            {
                defaultValue = Guid.Empty;
            }
            else
            {
                Debug.Fail("Unexpected key property type", keyProperty.ClrType.Name);
            }
            
            return Object.Equals(keyValue, defaultValue);
        }

        #endregion

        #region ToString / Equals / GetHashCode

        public override string ToString()
        {
            if (_objToString != null)
            {
                return _objToString;
            }

            StringBuilder buf = new StringBuilder();

            buf.Append(_entitySetName);
            buf.Append(".");
            buf.Append(_entityType.Name);
            buf.Append("[");

            if (IsTemporary)
            {
                buf.Append("Temporary, ");
                buf.Append(_temporaryId);
            }
            else
            {
                int keyIdx = 0;
                string separator = String.Empty;

                foreach (var keyProperty in _entityType.PrimaryKeyProperties)
                {
                    object keyValue = _keyValues[keyIdx];
                    Debug.Assert(null != keyValue);

                    buf.Append(separator);
                    buf.Append(keyProperty.Name);
                    buf.Append("=");
                    buf.Append(keyValue);

                    separator = ",";
                    keyIdx++;
                }
            }

            buf.Append("]");
            return (_objToString = buf.ToString());
        }

        public override bool Equals(object obj)
        {
            CustomEntityKey other = obj as CustomEntityKey;

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }
            
            if (
                    null == other ||
                    IsTemporary || other.IsTemporary ||                    
                    other._keyValues.Length != _keyValues.Length ||
                    !other._entitySetName.Equals(_entitySetName)
                )
            {
                return false;
            }       

            for (int keyIdx = 0; keyIdx < _keyValues.Length; keyIdx++)
            {
                if (!_keyValues[keyIdx].Equals(other._keyValues[keyIdx]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            if (IsTemporary)
            {
                Debug.Assert(!_precomputedHashCode.HasValue);
                return base.GetHashCode();
            }

            if (_precomputedHashCode.HasValue)
            {
                return _precomputedHashCode.Value;
            }

            int hashCode = 0;
            foreach (object keyValue in _keyValues)
            {
                Debug.Assert(null != keyValue);
                hashCode ^= keyValue.GetHashCode();
            }
            hashCode ^= _entitySetName.GetHashCode();

            return (_precomputedHashCode = hashCode).Value;
        }

        #endregion
    }

    #endregion

    // --- CODE SNIPPET END ---
}
