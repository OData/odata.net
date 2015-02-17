//---------------------------------------------------------------------
// <copyright file="CustomObjectCollection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Runtime.Serialization;
using System.Data.Test.Astoria.CustomData.Metadata;
using System.Reflection;

namespace System.Data.Test.Astoria.CustomData.Runtime
{
    // --- CODE SNIPPET START ---

    //
    // CustomCollectionWithChangeTracking class
    //
    #region CustomCollectionWithChangeTracking class

    public abstract class CustomCollectionWithChangeTracking
                            : ICollection
                            , ICustomCollectionWithChangeTracking
    {
        #region Private fields

        private IDictionary<object, CustomCollectionChangeKind> _changeEntries = null;

        #endregion

        #region Protected fields

        protected IDictionary<object, CustomCollectionChangeKind> ChangeEntries
        {
            get { return _changeEntries; }
        }

        #endregion

        #region Public consturctor

        public CustomCollectionWithChangeTracking()
        {
            _changeEntries = new Dictionary<object, CustomCollectionChangeKind>();
        }

        #endregion

        #region ICollection memebers

        public abstract int Count
        {
            get;
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        public abstract void CopyTo(Array array, int index);

        #endregion

        #region IEnumerable members

        public virtual IEnumerator GetEnumerator()
        {
            return GetEnumeratorImpl();
        }

        protected abstract IEnumerator GetEnumeratorImpl();

        #endregion

        #region Change management
       
        public bool IsDirty
        {
            get { return _changeEntries.Count > 0; }
        }

        public void ForgotChanges()
        {
            _changeEntries.Clear();
        }

        public IEnumerable<KeyValuePair<object, CustomCollectionChangeKind>> GetChangeHistory()
        {
            return _changeEntries.AsEnumerable();
        }

        public void RevertChanges()
        {
            object[] changedElements = new object[_changeEntries.Keys.Count];
            _changeEntries.Keys.CopyTo(changedElements, 0);

            foreach (var element in changedElements)
            {
                CustomCollectionChangeKind changeKind = _changeEntries[element];

                switch (changeKind)
                {
                    case CustomCollectionChangeKind.Added:
                        Remove(element);
                        break;
                    case CustomCollectionChangeKind.Deleted:
                        Add(element);
                        break;
                    default:
                        Debug.Fail("Unexpected change kind", changeKind.ToString());
                        break;
                }
            }

            _changeEntries.Clear();
        }

        protected virtual void OnChangeCompleted(object entityObject, CustomStoreOperationKind changeKind)
        {
            if (CustomStoreOperationKind.Add == changeKind)
            {
                CustomCollectionChangeKind oldChangeKind;
                if (ChangeEntries.TryGetValue(entityObject, out oldChangeKind))
                {
                    Debug.Assert(oldChangeKind == CustomCollectionChangeKind.Deleted);
                    bool removed = ChangeEntries.Remove(entityObject);
                    Debug.Assert(removed);
                }
                else
                {
                    ChangeEntries[entityObject] = CustomCollectionChangeKind.Added;
                }
            }
            else if (CustomStoreOperationKind.Delete == changeKind)
            {
                CustomCollectionChangeKind oldChangeKind;
                if (ChangeEntries.TryGetValue(entityObject, out oldChangeKind))
                {
                    Debug.Assert(oldChangeKind == CustomCollectionChangeKind.Added);
                    bool removed = ChangeEntries.Remove(entityObject);
                    Debug.Assert(removed);
                }
                else
                {
                    ChangeEntries[entityObject] = CustomCollectionChangeKind.Deleted;
                }
            }
            else
            {
                Debug.Fail("Unexpected change kind", changeKind.ToString());
            }
        }

        #endregion 

        #region Adding / Deleting elements

        public abstract void Add(object element);

        public abstract bool Remove(object element);

        public abstract bool Contains(object element);

        #endregion 
    }

    #endregion

    //
    // CustomObjectCollectionBase class
    //
    #region CustomObjectCollectionBase class

    public abstract class CustomObjectCollectionBase
                            : CustomCollectionWithChangeTracking
                            , ICustomRelationshipManager
    {
        #region Private fields

        private WeakReference<CustomObjectContext> _objectContextWeakRef;

        private CustomNavigationPropertyType _relationshipMetadata;

        #endregion

        #region Protected members

        protected CustomNavigationPropertyType RelationshipMetadata
        {
            get { return _relationshipMetadata; }
        }

        protected WeakReference<CustomObjectContext> ObjectContextWeakRef
        {
            get { return _objectContextWeakRef; }
        }

        #endregion

        #region Internal properties

        internal abstract IEnumerable InnerCollection
        {
            get;
        }

        #endregion

        #region Constructors

        public CustomObjectCollectionBase(CustomNavigationPropertyType relationshipMetadata)
        {
            _relationshipMetadata = relationshipMetadata;
        }

        #endregion      

        #region Relationship management

        public void AttachToContext(CustomObjectContext objectContext)
        {
            if (!((ICustomRelationshipManager)this).IsAttachedToContext(objectContext))
            {
                _objectContextWeakRef = new WeakReference<CustomObjectContext>(objectContext);
            }
        }

        public void DeattachFromContext()
        {
            _objectContextWeakRef = null;
        }

        public bool IsAttachedToContext(CustomObjectContext objectContext)
        {
            return null != _objectContextWeakRef
                    && _objectContextWeakRef.IsAlive
                    && Object.ReferenceEquals(_objectContextWeakRef.Target, objectContext);
        }

        public abstract void CascadeStoreOperation(CustomStoreOperationKind changeKind);

        #endregion
    }

    #endregion

    //
    // CustomObjectCollectionOfTBase<EntityObjectType> class
    //
    #region CustomObjectCollectionOfTBase<EntityObjectType> class

    public abstract class CustomObjectCollectionOfTBase<EntityObjectType>
                        : CustomObjectCollectionBase
                        , ICollection<EntityObjectType>
    {
        #region Private fields

        private HashSet<EntityObjectType> _innerObjectSet = new HashSet<EntityObjectType>();

        private object _thisEndEntityObject;

        #endregion

        #region Constructor

        public CustomObjectCollectionOfTBase(object thisEndEntityObject, CustomNavigationPropertyType relationshipMetadata)
                : base(relationshipMetadata)
        {
            _thisEndEntityObject = thisEndEntityObject;
        }

        #endregion

        #region Internal properties

        internal override IEnumerable InnerCollection
        {
            get { return _innerObjectSet; }
        }

        #endregion

        #region Adding / Deleting entity objects

        public override void Add(object entityObject)
        {
            Add((EntityObjectType)entityObject);
        }

        public void Add(EntityObjectType entityObject)
        {
            ValidateStateTransition(entityObject, CustomStoreOperationKind.Add);

            if (!_innerObjectSet.Add(entityObject))
            {
                return;
            }

            OnChangeCompleted(entityObject, CustomStoreOperationKind.Add);
        }

        public override bool Remove(object entityObject)
        {
            return Remove((EntityObjectType)entityObject);
        }

        public bool Remove(EntityObjectType entityObject)
        {
            ValidateStateTransition(entityObject, CustomStoreOperationKind.Delete);

            bool removed = _innerObjectSet.Remove(entityObject);
            if (removed)
            {
                OnChangeCompleted(entityObject, CustomStoreOperationKind.Delete);
            }
            return removed;
        }

        #endregion

        #region Misc ICollection<T> members

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public override bool Contains(object entityObject)
        {
            return Contains((EntityObjectType) entityObject);
        }

        public virtual bool Contains(EntityObjectType entityObject)
        {
            return _innerObjectSet.Contains(entityObject);
        }

        public override int Count
        {
            get { return _innerObjectSet.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public override void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public virtual void CopyTo(EntityObjectType[] array, int arrayIndex)
        {
            _innerObjectSet.CopyTo(array, arrayIndex);
        }

        #endregion

        #region IEnumerable members

        public new IEnumerator<EntityObjectType> GetEnumerator()
        {
            return _innerObjectSet.GetEnumerator();
        }

        protected override IEnumerator GetEnumeratorImpl()
        {
            return _innerObjectSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumeratorImpl();
        }

        #endregion        

        #region Relationship management

        protected void ValidateStateTransition(
                                        EntityObjectType relatedEntityObject,
                                        CustomStoreOperationKind changeKind
                                    )
        {
            if (null != ObjectContextWeakRef)
            {
                CustomObjectContext objectContext = ObjectContextWeakRef.Target;
                if (null != objectContext && null != RelationshipMetadata)
                {
                    var thisRelationshipEnd = new CustomRelationshipEnd(_thisEndEntityObject, RelationshipMetadata.From);
                    if (objectContext.RelationshipHelper.IsVisitedRelationshipEnd(thisRelationshipEnd))
                    {
                        return;
                    }

                    try
                    {
                        objectContext.RelationshipHelper.AddToVisitedRelationshipEnds(thisRelationshipEnd);
                        ValidateStateTransitionInternal(objectContext, relatedEntityObject, changeKind);
                    }
                    finally
                    {
                        objectContext.RelationshipHelper.RemoveFromVisitedRelationshipEnds(thisRelationshipEnd);
                    }                    
                }
            }
        }

        protected override void OnChangeCompleted(
                                        object relatedEntityObject,
                                        CustomStoreOperationKind changeKind
                                    )
        {
            base.OnChangeCompleted(relatedEntityObject, changeKind);

            if (null != ObjectContextWeakRef)
            {
                Debug.Assert(relatedEntityObject is EntityObjectType);

                CustomObjectContext objectContext = ObjectContextWeakRef.Target;
                if (null != objectContext && null != RelationshipMetadata)
                {
                    var thisRelationshipEnd = new CustomRelationshipEnd(_thisEndEntityObject, RelationshipMetadata.From);
                    if (objectContext.RelationshipHelper.IsVisitedRelationshipEnd(thisRelationshipEnd))
                    {
                        return;
                    }

                    try
                    {
                        objectContext.RelationshipHelper.AddToVisitedRelationshipEnds(thisRelationshipEnd);
                        UpdateRelatedEndInternal(objectContext, (EntityObjectType)relatedEntityObject, changeKind);
                    }
                    finally
                    {
                        objectContext.RelationshipHelper.RemoveFromVisitedRelationshipEnds(thisRelationshipEnd);
                    }
                }
            }
        }

        protected virtual void ValidateStateTransitionInternal(
                                        CustomObjectContext objectContext,
                                        EntityObjectType relatedEntityObject,
                                        CustomStoreOperationKind changeKind
                                    )
        {
        }

        protected abstract void UpdateRelatedEndInternal(
                                        CustomObjectContext objectContext,
                                        EntityObjectType relatedEntityObject,
                                        CustomStoreOperationKind changeKind
                                    );

        #endregion
    }

    #endregion

    //
    // CustomOneToManyObjectCollection<ThisEndEntityObject, RelatedEndEntityObject> class
    //
    #region CustomOneToManyObjectCollection<ThisEndEntityObject, RelatedEndEntityObject> class

    public class CustomOneToManyObjectCollection<ThisEndEntityObject, RelatedEndEntityObject>
                                    : CustomObjectCollectionOfTBase<RelatedEndEntityObject>
                                    , ICustomRelationshipManager
    {
        #region Private fields

        private ThisEndEntityObject _thisEndObject;

        #endregion

        #region Constructor

        public CustomOneToManyObjectCollection(
                    ThisEndEntityObject thisEndEntityObject,
                    CustomNavigationPropertyType relationshipMetadata
                )
            : base(thisEndEntityObject, relationshipMetadata)
        {
            Debug.Assert(null != thisEndEntityObject);
            _thisEndObject = thisEndEntityObject;

            Debug.Assert(null != relationshipMetadata);
            Debug.Assert(
                    CustomMultiplicity.One == RelationshipMetadata.From.Multiplicity &&
                    CustomMultiplicity.Many == RelationshipMetadata.To.Multiplicity
                );
        }

        #endregion

        #region Relationship management

        protected override void ValidateStateTransitionInternal(
                                        CustomObjectContext objectContext,
                                        RelatedEndEntityObject relatedEntityObject,
                                        CustomStoreOperationKind changeKind
                                    )
        {
            if (Object.ReferenceEquals(_thisEndObject, relatedEntityObject))
            {
                throw new InvalidOperationException(String.Format(
                                        "Cannot add entity object '{0}' to its one one-to-many collection.",
                                        _thisEndObject
                                    ));
            }

            if (CustomStoreOperationKind.Add == changeKind)
            {
                if (null != RelationshipMetadata.To.RelatedProperty)
                {
                    var sameOneEndRoleObject = RelationshipMetadata.To.RelatedProperty.GetValue(relatedEntityObject);
                    if (
                        !Object.ReferenceEquals(sameOneEndRoleObject, null) &&
                        !Object.ReferenceEquals(sameOneEndRoleObject, _thisEndObject))
                    {
#if _DO_NOT_REMOVE_SILENTLY_SAME_ROLE_INSTANCE
                        throw new InvalidOperationException(String.Format(
                                            "Entity object '{0}' is already associated with another " +
                                            "instance '{1}' of principal end in the one-to-many relationship " +
                                            "between '{2}' and '{3}'.",
                                            relatedEntityObject, sameOneEndRoleObject,
                                            typeof(ThisEndEntityObject), typeof(RelatedEndEntityObject)
                                        ));
#else                        
                        CustomEntityType entityType = RelationshipMetadata.From.EntityType;
                        
                        // TODO: do we need this assert? saw it failing in Astoria tests; may be it is
                        // caused by multiple object contexts loaded from the same data source?
                        // in this implementation all object contexts share same objects
#if _NOT_ASTORIA_TESTS
                        Debug.Assert(entityType.ClrObjectType.IsAssignableFrom(sameOneEndRoleObject.GetType()));
#endif


                        object otherCollection = RelationshipMetadata.GetValue(sameOneEndRoleObject);
                        Debug.Assert(null != otherCollection);
                        
                        if (otherCollection is CustomObjectCollectionBase)
                        {
                            bool removed = ((CustomObjectCollectionBase)otherCollection).Remove(relatedEntityObject);
                            
                            // TODO: need to investigate; saw a failure here with Northwind model in Astoria tests
                            // same problem with many object contexts loaded from the same data source as above?
#if _NOT_ASTORIA_TESTS
                            Debug.Assert(removed);
#endif
                        }
                        else
                        {
                            MethodInfo removeMethod = otherCollection.GetType().GetMethod("Remove");
                            Debug.Assert(null != removeMethod);
                            removeMethod.Invoke(otherCollection, new object[] { relatedEntityObject });
                        }                        
#endif
                    }
                }
            }
        }

        protected override void UpdateRelatedEndInternal(
                                        CustomObjectContext objectContext,
                                        RelatedEndEntityObject relatedEntityObject,
                                        CustomStoreOperationKind changeKind
                                    )
        {
            if (CustomStoreOperationKind.Add == changeKind)
            {
                if (null != RelationshipMetadata.To.RelatedProperty)
                {
                    RelationshipMetadata.To.RelatedProperty.SetValue(relatedEntityObject, _thisEndObject);
                }

                if (null != objectContext) 
                {
                    objectContext.AddObject(RelationshipMetadata.To.EntitySetType.Name, relatedEntityObject);
                }
            }
            else
            {
                Debug.Assert(CustomStoreOperationKind.Delete == changeKind);

                if (null != RelationshipMetadata.To.RelatedProperty)
                {
                    RelationshipMetadata.To.RelatedProperty.SetValue(relatedEntityObject, null);
                }

                if (null != objectContext)
                {
                    objectContext.DeleteObject(relatedEntityObject);
                }
            }
        }

        public override void CascadeStoreOperation(CustomStoreOperationKind operationKind)
        {
            CustomObjectContext context = ObjectContextWeakRef.Target;
            Debug.Assert(null != context);

            foreach (object entityObject in this)
            {
                if (CustomStoreOperationKind.Add == operationKind)
                {
                    context.AddObject(RelationshipMetadata.To.EntitySetType.Name, entityObject);
                }
                else if (CustomStoreOperationKind.Delete == operationKind)
                {
                    context.DeleteObject(entityObject);
                }
                else
                {
                    Debug.Fail("Unexpected store operation kind", operationKind.ToString());
                }
            }
        }

        #endregion
    }

    #endregion

    //
    // CustomAnyToManyObjectCollection<ThisEndEntityObject, RelatedEndEntityObject> class
    //
    #region CustomManyToManyObjectCollection<ThisEndEntityObject, RelatedEndEntityObject> class

    public class CustomManyToManyObjectCollection<ThisEndEntityObject, RelatedEndEntityObject>
                                    : CustomObjectCollectionOfTBase<RelatedEndEntityObject>
                                    , ICustomRelationshipManager
    {
        #region Private fields

        private ThisEndEntityObject _thisEndObject;

        #endregion

        #region Constructor

        public CustomManyToManyObjectCollection(
                    ThisEndEntityObject thisEndEntityObject,
                    CustomNavigationPropertyType relationshipMetadata
                )
            : base(thisEndEntityObject, relationshipMetadata)
        {
            Debug.Assert(null != thisEndEntityObject);
            _thisEndObject = thisEndEntityObject;

            Debug.Assert(null != relationshipMetadata);
            Debug.Assert(
                    CustomMultiplicity.Many == RelationshipMetadata.From.Multiplicity &&
                    CustomMultiplicity.Many == RelationshipMetadata.To.Multiplicity
                );
        }

        #endregion

        #region Relationship management

        protected override void UpdateRelatedEndInternal(
                                            CustomObjectContext objectContext,
                                            RelatedEndEntityObject relatedEntityObject,
                                            CustomStoreOperationKind changeKind
                                        )
        {
            if (CustomStoreOperationKind.Add == changeKind)
            {
                if (null != RelationshipMetadata.To.RelatedProperty)
                {
                    object relatedManyToManyCollection = RelationshipMetadata.To.RelatedProperty.GetValue(relatedEntityObject);
                    Debug.Assert(null != relatedManyToManyCollection);

                    if (relatedManyToManyCollection is CustomObjectCollectionBase)
                    {
                        Debug.Assert(relatedManyToManyCollection is CustomObjectCollectionBase);
                        var managedManyToManyCollection = (CustomObjectCollectionBase)relatedManyToManyCollection;
                        managedManyToManyCollection.Add(_thisEndObject);
                    }
                    else
                    {
                        MethodInfo addMethod = relatedManyToManyCollection.GetType().GetMethod("Add");
                        Debug.Assert(null != addMethod);
                        addMethod.Invoke(relatedManyToManyCollection, new object[] { _thisEndObject });
                    }
                }

                if (null != objectContext)
                {
                    objectContext.AddObject(RelationshipMetadata.To.EntitySetType.Name, relatedEntityObject);
                }
            }
            else
            {
                Debug.Assert(CustomStoreOperationKind.Delete == changeKind);

                if (null != RelationshipMetadata.To.RelatedProperty)
                {
                    object relatedManyToManyCollection = RelationshipMetadata.To.RelatedProperty.GetValue(relatedEntityObject);
                    if (relatedManyToManyCollection is CustomObjectCollectionBase)
                    {
                        Debug.Assert(relatedManyToManyCollection is CustomObjectCollectionBase);
                        var managedManyToManyCollection = (CustomObjectCollectionBase)relatedManyToManyCollection;
                        managedManyToManyCollection.Remove(_thisEndObject);
                    }
                    else
                    {
                        MethodInfo removeMethod = relatedManyToManyCollection.GetType().GetMethod("Remove");
                        Debug.Assert(null != removeMethod);
                        removeMethod.Invoke(relatedManyToManyCollection, new object[] { _thisEndObject });
                    }

                    if (null != objectContext)
                    {
                        DeleteObjectIfTransietOrphan(objectContext, relatedEntityObject);
                    }
                }
            }
        }

        public override void CascadeStoreOperation(CustomStoreOperationKind changeKind)
        {
            CustomObjectContext context = ObjectContextWeakRef.Target;
            Debug.Assert(null != context);

            Queue<object> objectsToBeRemoved = null;

            foreach (object entityObject in this)
            {
                if (CustomStoreOperationKind.Add == changeKind)
                {
                    context.AddObject(RelationshipMetadata.To.EntitySetType.Name, entityObject);
                }
                else if (CustomStoreOperationKind.Delete == changeKind)
                {
                    if (null == objectsToBeRemoved)
                    {
                        objectsToBeRemoved = new Queue<object>();
                    }
                    objectsToBeRemoved.Enqueue(entityObject);
                }
            }

            if (null != objectsToBeRemoved && objectsToBeRemoved.Count > 0)
            {
                foreach (object entityObject in objectsToBeRemoved)
                {
                    Remove(entityObject);
                    DeleteObjectIfTransietOrphan(context, entityObject);
                }
            }
        }

        private void DeleteObjectIfTransietOrphan(CustomObjectContext objectContext, object entityObject)
        {
            if (objectContext.RelationshipHelper.IsTransietObject(entityObject))
            {
                // we just deleted an entity object that was not persited yet
                // if the object becomes "orphan" e.g. doesn't participate in
                // any other collection - delete it from the store

                // TODO: implement
            }
        }

        #endregion
    }

    #endregion

    //
    // WeakReference<T> class
    //
    #region WeakReference<T> class

    public class WeakReference<T>
                    : WeakReference where T : class
    {
        public WeakReference(T target)
            : base(target)
        { }

        public WeakReference(T target, bool trackResurrection)
            : base(target, trackResurrection)
        { }

        protected WeakReference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public new T Target
        {
            get
            {
                return (T)base.Target;
            }
           set
            {
                base.Target = value;
            }
        }
    }        

    #endregion

    // --- CODE SNIPPET END ---
}
