//---------------------------------------------------------------------
// <copyright file="CustomWebObjectContext.cs" company="Microsoft">
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
using Microsoft.OData.Client;

namespace System.Data.Test.Astoria.CustomData.Runtime
{
    // --- CODE SNIPPET START ---

    //
    // CustomWebObjectContext class
    //
    #region CustomWebObjectContext class

    public class CustomWebObjectContext
                        : CustomObjectContext
                        , Microsoft.OData.Service.IUpdatable
    {
        #region Private constants

        private const int NoContentHttpCode = 204;

        #endregion

        #region Public constructors

        public CustomWebObjectContext(CustomEntitySetContainer entitySetContainer)
            : base(entitySetContainer)
        {
        }

        #endregion

        #region Private helpers for update operations

        private object FindResourceByQuery(IQueryable query)
        {
            object matchingResource = null;
            foreach (object resource in query)
            {
                if (null != matchingResource)
                {
                    throw new ArgumentException(String.Format(
                                    "Invalid URI specified. The query '{0}' must refer to a single resource.",
                                    query.ToString()
                                ));
                }

                matchingResource = resource;
            }
            return matchingResource;
        }

        private bool TryGetEntityMember(
                                object targetResource,
                                string memberName,
                                out CustomMemberType entityMember
                        )
        {
            entityMember = null;

            CustomEntityType entityType;
            if (!MetadataWorkspace.TryGetEntityType(targetResource.GetType(), out entityType))
            {
                return false;
            }

            entityMember = entityType.Members
                                        .Where(m => m.Name.Equals(memberName))
                                        .Select(m => m)
                                        .SingleOrDefault();

            return (null != entityMember);
        }

        private CustomMemberType GetEntityMember(object targetResource, string memberName)
        {
            CustomMemberType entityMember;
            if (!TryGetEntityMember(targetResource, memberName, out entityMember))
            {
                throw new ArgumentException(String.Format(
                                        "Invalid URI specified. The given object '{0}' is not a valid " +
                                        "entity object or does not contain member '{1}'.",
                                        targetResource.GetType().FullName,
                                        memberName
                                    ));
            }
            return entityMember;
        }

        private IEnumerable GetObjectCollection(object targetResource, string propertyName)
        {
            CustomMemberType targetMember = GetEntityMember(targetResource, propertyName);
            if (!(targetMember is CustomNavigationPropertyType && CustomMultiplicity.Many == ((CustomNavigationPropertyType)targetMember).To.Multiplicity))
            {
                throw new ArgumentException(String.Format(
                                "Invalid URI specified.  Member '{0}' is not an object collection property " +
                                "in entity type '{1}'.",
                                propertyName, targetResource.GetType().FullName
                            ));
            }

            object objCollection = targetMember.GetValue(targetResource);
            Debug.Assert(objCollection is IEnumerable);
            return (IEnumerable)objCollection;
        }

        private static bool IsComplexType(Type type)
        {
            bool isComplex = (null != TryGetAttribute<CustomComplexTypeAttribute>(type));
            if (isComplex)
            {
                Debug.Assert(null == TryGetAttribute<KeyAttribute>(type));
            }
            return isComplex;
        }

        private static PropertyInfo GetProperty(Type resourceType, string propertyName)
        {
            var property = resourceType.GetProperty(propertyName);
            if (null == property)
            {
                throw new ArgumentException(String.Format(
                                    "Invalid URI specified. Resource type '{0}' does not contain " +
                                    "property '{1}'.",
                                    resourceType.Name,
                                    propertyName
                                ));
            }
            return property;
        }

        private static AttributeType TryGetAttribute<AttributeType>(MemberInfo forMember)
        {
            return forMember.GetCustomAttributes(typeof(AttributeType), true /* inherit */)
                        .Select(a => (AttributeType)a).SingleOrDefault();
        }

        private void CheckEntityObjectAttached(object entityObject)
        {
            if (!IsAttached(entityObject))
            {
                throw new ArgumentException(String.Format(                                    
                                        "The given object '{0}' is not attached to the context " +
                                        "or is not a valid entity object.",
                                        entityObject
                                    ));
            }
        }

        /// <summary>
        /// Workaround to bypass design constraints and allow IUpdatable.SetValue() to be
        /// called on complex type properties. See comment inside SetValue() method.
        /// </summary>
        void CopyComplexTypeObject(
                            object firstComplexObject,
                            object complexObjectToCopy
                        )
        {
            Debug.Assert(
                        null != firstComplexObject && null != complexObjectToCopy,
                        "should no be possible; we always create instances of complex types; " +
                        "and properties of complex type have no setter, so they cannot be set " +
                        "to null"
                    );
            Debug.Assert(IsComplexType(firstComplexObject.GetType()));
            Debug.Assert(IsComplexType(complexObjectToCopy.GetType()));

            foreach (PropertyInfo innerProperty in firstComplexObject.GetType().GetProperties())
            {                
                object valueToSet = innerProperty.GetValue(complexObjectToCopy, null);

                if (IsComplexType(innerProperty.PropertyType))
                {
                    object complexValue = innerProperty.GetValue(firstComplexObject, null);
                    CopyComplexTypeObject(complexValue, valueToSet);
                }
                else
                {
                    if (valueToSet is ICloneable)
                    {
                        valueToSet = ((ICloneable)valueToSet).Clone();
                    }
                    innerProperty.SetValue(firstComplexObject, valueToSet, null);
                }
            }
        }

        #endregion

        #region IUpdatable members

        public object CreateResource(string containerName, string fullTypeName)
        {
            CustomUtils.CheckArgumentNotNull(fullTypeName, "fullTypeName");
            
            Type entityOrComplexType = Type.GetType(fullTypeName);
            object obj = Activator.CreateInstance(entityOrComplexType);

            if (IsComplexType(entityOrComplexType))
            {
                if (null != containerName)
                {
                    throw new ArgumentException(String.Format(
                                    "Parameter 'containerName' must be null when CreateResource is called for " +
                                    "complex type objects. Container name: '{0}'. Complex type: '{1}'.",
                                    containerName, entityOrComplexType.Name
                                ));
                }                
                return obj;
            }
            else
            {
                CustomUtils.CheckArgumentNotNull(containerName, "containerName");
                AddObject(containerName, obj);  // will throw if the object is not a valid entity
                Debug.Assert(IsAttached(obj));
            }

            return obj;
        }

        public object GetResource(IQueryable query, string fullTypeName)
        {
            CustomUtils.CheckArgumentNotNull(query, "query");

            object resource = FindResourceByQuery(query);

            if (
                    null != resource &&
                    null != fullTypeName &&
                    resource.GetType().FullName != fullTypeName)
            {
                throw new ArgumentException(String.Format(
                                    "Invalid URI specified. ExpectedType: '{0}'. ActualType: '{1}'.",
                                    fullTypeName, resource.GetType().FullName
                                ));
            }

            return resource;
        }

        public object ResetResource(object resource)
        {
            CustomUtils.CheckArgumentNotNull(resource, "resource");

            CustomEntityType entityType;
            if(!MetadataWorkspace.TryGetEntityType(resource.GetType(), out entityType))
            {
                throw new NotImplementedException(String.Format(
                                    "ResetResource() is implemented only for entity types. " +
                                    "The given object '{0}' is not a valid entity."
                                ));
            }

            foreach(var property in entityType.Propeties)
            {
                if(!property.IsReadOnly && !property.IsPrimaryKey && null == property.ForeignKeyConstraint)
                {
                    object defaultValue;
                    if(property.IsNullable)
                    {
                        defaultValue = null;
                    }
                    else
                    {
                        defaultValue = Activator.CreateInstance(property.ClrType);
                    }

                    property.SetValue(resource, defaultValue);
                }
            }

            return resource;
        }

        public void SetValue(object targetResource, string propertyName, object propertyValue)
        {
            CustomUtils.CheckArgumentNotNull(targetResource, "targetResource");
            CustomUtils.CheckArgumentNotNull(propertyName, "propertyName");

            var property = GetProperty(targetResource.GetType(), propertyName);
            if (IsComplexType(property.PropertyType))
            {
                // we treat complex types as value types, an inherit part of an entity type,
                // they cannot be null, have no separate metadata, always exist and
                // by design complex type properties have no setters
                //
                // we can relax these constraints later, but now change detection
                // logic in custom object context depends on it

                if (null == propertyValue)
                {
                    throw new ArgumentNullException(String.Format(
                                        "Value for complex type property '{0}' cannot be null.",
                                        propertyName
                                    ));
                }

                object currentComplexValue = property.GetValue(targetResource, null);
                CopyComplexTypeObject(currentComplexValue, propertyValue);
                return;
            }
          
            CustomMemberType entityMember;
            if (TryGetEntityMember(targetResource, propertyName, out entityMember))
            {
                if (entityMember is CustomNavigationPropertyType)
                {
                    throw new InvalidOperationException(String.Format(
                                        "Given property '{0}' of entity object '{1}' is a navigation " +
                                        "property of type '{2}'. SetValue() is not supposed for " +
                                        "changing navigation properties. Use SetReference() or " +
                                        "AddReferenceToCollection() methods.",
                                        propertyName, targetResource, entityMember.ClrType
                                    ));
                }

                Debug.Assert(entityMember is CustomPropertyType);

                CustomPropertyType entityProperty = (CustomPropertyType)entityMember;

                if (null != entityProperty.ForeignKeyConstraint)
                {
                    // attempt to set a foreign key property is incorrect in the world of our custom
                    // data model because there is no setter for such properties at all and the getter
                    // simply returns value of a primary key property from the primary end entity using
                    // corresponding navigation property
                    // get
                    // {
                    //    if(null == _primaryEnd)
                    //    {
                    //       return default(int);
                    //    }
                    //    return _primaryEnd.Id;
                    // }
                    //
                    // silently return from SetValue without error reporting for compatibility
                    // with existing tests
                    return;
                }                

                if (entityProperty.IsPrimaryKey)
                {
                    // all our primary keys are store generated and have internal setter, but
                    // test framework for Astoria suppose to be able to set primary keys and
                    // then use same values to query the object; we allow it here to be
                    // compatible with the existing tests; for objects with initialized primary keys
                    // the custom object context will actually queue an update operation, but not insert
                    entityProperty.SetValue(targetResource, propertyValue);
                    return;
                }
            }
            else
            {
                Debug.Assert(IsComplexType(targetResource.GetType()));
            }
           
            if (null == property.GetSetMethod() || !property.CanWrite)
            {
                throw new InvalidOperationException(String.Format(
                                    "Property '{0}' of resource type '{1}' is not writable.",
                                    propertyName,
                                    targetResource.GetType().Name
                                ));
            }
            property.SetValue(targetResource, propertyValue, null);
        }

        public object GetValue(object targetResource, string propertyName)
        {            
            CustomUtils.CheckArgumentNotNull(targetResource, "targetResource");
            CustomUtils.CheckArgumentNotNull(propertyName, "propertyName");

            var property = GetProperty(targetResource.GetType(), propertyName);
            if (null == property.GetGetMethod() || !property.CanRead)
            {
                // we don't generate properties without getter; but just in case...
                throw new InvalidOperationException(String.Format(
                                    "Property '{0}' of resource type '{1}' is not accessible.",
                                    propertyName,
                                    targetResource.GetType().Name
                                ));
            }
            return property.GetValue(targetResource, null);
        }

        public void SetReference(object targetResource, string propertyName, object propertyValue)
        {
            CustomUtils.CheckArgumentNotNull(targetResource, "targetResource");
            CustomUtils.CheckArgumentNotNull(propertyName, "propertyName");

            GetEntityMember(targetResource, propertyName).SetValue(targetResource, propertyValue);
        }

        public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            CustomUtils.CheckArgumentNotNull(targetResource, "targetResource");
            CustomUtils.CheckArgumentNotNull(propertyName, "propertyName");

            object objCollection = GetObjectCollection(targetResource, propertyName);

            if (objCollection is CustomObjectCollectionBase)
            {
                // the entity object was already added to the context
                // and the underlying CLR collection was wrapped into our internal
                // tracking collection
                ((CustomObjectCollectionBase)objCollection).Add(resourceToBeAdded);
            }
            else
            {
                MethodInfo addMethod = objCollection.GetType().GetMethod("Add");
                Debug.Assert(null != addMethod);
                addMethod.Invoke(targetResource, new object[] { resourceToBeAdded });
            }                               
        }

        public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            CustomUtils.CheckArgumentNotNull(targetResource, "targetResource");
            CustomUtils.CheckArgumentNotNull(propertyName, "propertyName");

            object objCollection = GetObjectCollection(targetResource, propertyName);
            if (objCollection is CustomObjectCollectionBase)
            {
                // the entity object was already added to the context
                // and the underlying CLR collection was wrapped into our internal
                // tracking collection
                ((CustomObjectCollectionBase)objCollection).Remove(resourceToBeRemoved);
            }
            else
            {
                MethodInfo addMethod = objCollection.GetType().GetMethod("Remove");
                Debug.Assert(null != addMethod);
                addMethod.Invoke(targetResource, new object[] { resourceToBeRemoved });
            } 
        }

        public void DeleteResource(object targetResource)
        {
            CustomUtils.CheckArgumentNotNull(targetResource, "targetResource");
            CheckEntityObjectAttached(targetResource);

            DeleteObject(targetResource);
        }

        public void SaveChanges()
        {
            SaveAllChanges();
        }

        public object ResolveResource(object targetResource)
        {
            CustomUtils.CheckArgumentNotNull(targetResource, "targetResource");
            CheckEntityObjectAttached(targetResource);

            return targetResource;
        }

        public void ClearChanges()
        {
            ClearAllChanges();
        }

        #endregion
    }

    #endregion

    // --- CODE SNIPPET END ---
}
