//---------------------------------------------------------------------
// <copyright file="InMemoryContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.OData.Service;
using System.Data.Common;
using System.Web;

namespace System.Data.Test.Astoria.InMemoryLinq
{
    using Microsoft.OData.Client;

    public abstract class InMemoryContext
    {
        internal static InMemoryEntitySetDictionary _entitySetDictionary;

        public InMemoryContext()
        {
            PendingChanges = new List<KeyValuePair<object, EntityState>>();
        }

        public InMemoryEntitySetDictionary EntitySetDictionary
        {
            get
            {
                if (_entitySetDictionary == null)
                {
                    _entitySetDictionary = InitializeEntitySetDictionary();
                }
                return _entitySetDictionary;
            }
        }

        public InMemoryEntitySetDictionary InitializeEntitySetDictionary()
        {
            //Create List of t's with the correct types
            InMemoryEntitySetDictionary inMemoryDictionary = new InMemoryEntitySetDictionary();
            foreach (PropertyInfo info in this.GetType().GetProperties())
            {
                if (typeof(IQueryable).IsAssignableFrom(info.PropertyType))
                {
                    Type t = info.PropertyType.GetGenericArguments()[0];
                    IList list = (IList)InMemoryEntitySetDictionary.CreateGeneric(typeof(List<>), t, null);
                    inMemoryDictionary.Add(t, list);
                }
            }
            return inMemoryDictionary;
        }

        internal List<KeyValuePair<object, EntityState>> PendingChanges
        {
            get;
            private set;
        }

        public void RestoreData()
        {
            foreach (var list in EntitySetDictionary.Values)
                list.Clear();

            AddData(this as IUpdatable);
        }
        #region IUpdatable implementation

        #region Resource token management

        internal class ResourceToken
        {
            public ResourceToken(object resource)
            {
                Resource = resource;
            }

            public object Resource
            {
                get;
                private set;
            }
        }

        private object ResourceToToken(object resource)
        {
            return new ResourceToken(resource);
        }

        private object TokenToResource(object resource)
        {
            if (resource == null)
                throw new ArgumentNullException("resource");
            
            ResourceToken token = resource as ResourceToken;
            
            if (token == null)
                throw new ArgumentException("Resource token was of unexpected type '" + resource.GetType() + "'");

            return token.Resource;
        }

        private object ResolveIfToken(object resource)
        {
            ResourceToken token = resource as ResourceToken;
            if(token == null)
                return resource;
            return token.Resource;
        }
        #endregion

        public virtual object CreateResource(string containerName, string fullTypeName)
        {
            Assembly a = this.GetType().Assembly;
            Type typeToCreate = a.GetType(fullTypeName, true);

            if (typeToCreate == null)
                throw new DataServiceException((int)Net.HttpStatusCode.InternalServerError, "Unable to create type:" + fullTypeName + " using Assembly:" + a.FullName + " using GetTypes");
            if (typeToCreate.IsAbstract)
                throw new DataServiceException((int)Net.HttpStatusCode.BadRequest, "Cannot create abstract type: " + fullTypeName);

            object objectToBeAdded = Activator.CreateInstance(typeToCreate);

            if (objectToBeAdded != null)
            {
                if (containerName != null)
                {
                    this.PendingChanges.Add(new KeyValuePair<object, EntityState>(objectToBeAdded, EntityState.Added));
                }
            }
            else
            {
                throw new Exception(String.Format("Invalid container name : '{0}' or invalid type name: '{1}'", containerName, fullTypeName));
            }

            return ResourceToToken(objectToBeAdded);
        } // CreateResource

        public virtual object ResetResource(object resource)
        {
            // clear out the old token, in case the system is stashing it somewhere
            //
            resource = TokenToResource(resource);

            Type type = resource.GetType();
            object dummyResource = Activator.CreateInstance(type);

            List<string> keyPropertyNames = GetKeyPropertyNames(type).ToList();
            foreach (PropertyInfo info in type.GetProperties())
            {
                // is it a key property?
                //
                if (keyPropertyNames.Contains(info.Name))
                    continue;

                // is it a collection property, but not an array?
                //
                if (typeof(ICollection).IsAssignableFrom(info.PropertyType) && !info.PropertyType.IsArray)
                    continue;

                // is it a reference property?
                //
                bool isReference = false;
                Type possibleEntityType = info.PropertyType;
                while (possibleEntityType != null)
                {
                    if (this.EntitySetDictionary.ContainsKey(possibleEntityType))
                    {
                        isReference = true;
                        break;
                    }
                    possibleEntityType = possibleEntityType.BaseType;
                }
                if (isReference)
                    continue;

                // not a nav prop or key prop, so reset it
                //
                info.SetValue(resource, info.GetValue(dummyResource, null), null);
            }

            // this gives a different token than what was passed in, but resolves to the same instance
            //
            return ResourceToToken(resource);
        } // ResetResource

        public virtual void DeleteResource(object targetResource)
        {
            targetResource = TokenToResource(targetResource);
            this.PendingChanges.Add(new KeyValuePair<object, EntityState>(targetResource, EntityState.Deleted));
        } // DeleteResource

        public virtual object GetResource(IQueryable query, string fullTypeName)
        {
            object resource = null;
            List<object> resources = new List<object>();
            foreach (object r in query)
                resources.Add(r);

            switch (resources.Count)
            {
                case 1:
                    resource = resources[0];
                    break;

                case 0:
                    throw new DataServiceException(404, (String.Format("The query '{0}' does not refer to any existing resource", query.ToString())));

                default:
                    string[] keys = resources.Select(o => KeyToString(GetKeyValues(o))).ToArray();
                    throw new DataServiceException(404, (String.Format("The query '{0}' refers to more than one resource. Found keys: {1}. Found references", query.ToString(), string.Join(", ", keys))));
            }

            if (resource != null)
            {
                if (fullTypeName != null && resource.GetType().FullName != fullTypeName)
                {
                    throw new System.ArgumentException(String.Format("Invalid uri specified. ExpectedType: '{0}', ActualType: '{1}'", fullTypeName, resource.GetType().FullName));
                }
                return ResourceToToken(resource);
            }

            return null;
        } // GetResource

        public virtual object ResolveResource(object resource)
        {
            return TokenToResource(resource);
        } // ResolveResource

        public virtual object GetValue(object targetResource, string propertyName)
        {
            targetResource = TokenToResource(targetResource);

            object propertyValue;
            Type type = targetResource.GetType();

            PropertyInfo property = type.GetProperty(propertyName);
            if (property != null)
            {
                propertyValue = property.GetValue(targetResource, null);

                // any complex types will be defined in this class
                if (property.PropertyType.Namespace == this.GetType().Namespace)
                    propertyValue = ResourceToToken(propertyValue);
            }
            else
            {
                if (!TryGetOpenTypeValue(targetResource, propertyName, out propertyValue))
                {
                    throw new DataServiceException(500,
                        String.Format("Could not find declared or dynamic property '{0}' on entity of type '{1}'", propertyName, type.Name));
                }
            }

            return propertyValue;
        } // GetValue

        public virtual void SetValue(object targetResource, string propertyName, object propertyValue)
        {
            targetResource = TokenToResource(targetResource);
            propertyValue = ResolveIfToken(propertyValue);

            Type type = targetResource.GetType();

            PropertyInfo property = type.GetProperty(propertyName);
            if (property != null)
            {
                // NOTE: it is possible that this property cannot have NULL assigned to it, but oh well
                if (propertyValue != null && !property.PropertyType.IsAssignableFrom(propertyValue.GetType()))
                {
                    throw new DataServiceException(400,
                        String.Format("Cannot assign a value of type '{0}' to property '{1}'", propertyValue.GetType().Name, propertyName));
                }
                property.SetValue(targetResource, propertyValue, null);
            }
            else
            {
                if (!TrySetOpenTypeValue(targetResource, propertyName, propertyValue))
                {
                    throw new DataServiceException(500,
                        String.Format("Could not find declared or dynamic property '{0}' on entity of type '{1}'", propertyName, type.Name));
                }
            }
        } // SetValue

        public virtual void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            targetResource = TokenToResource(targetResource);
            resourceToBeAdded = TokenToResource(resourceToBeAdded);

            PropertyInfo pi = targetResource.GetType().GetProperty(propertyName);
            if (pi == null)
                throw new DataServiceException((int)Net.HttpStatusCode.InternalServerError, String.Format("Unable to find property:{0} on type:{1}", propertyName, targetResource.GetType().Name));

            IList list = pi.GetValue(targetResource, null) as IList;
            if (list == null)
                throw new DataServiceException((int)Net.HttpStatusCode.InternalServerError, String.Format("Unable to get an IList from Unable to find property:{0} on type:{1}", propertyName, targetResource.GetType().Name));

            List<KeyValuePair<string, object>> keyBeingAdded = GetKeyValues(resourceToBeAdded);
            foreach (object existingEntity in list)
            {
                List<KeyValuePair<string, object>> existingKey = GetKeyValues(existingEntity);
                // check if there is not another instance with the same id
                if (EqualPropertyValues(keyBeingAdded, existingKey))
                {
                    return;
                    //throw new DataServiceException(500, String.Format("Entity with the key '{0}' already present in collection '{1}' on {2} entity with key '{3}'",
                    //    KeyToString(keyBeingAdded), propertyName, targetResource.GetType().Name, KeyToString(GetKeyValues(targetResource))));
                }
            }
            list.Add(resourceToBeAdded);

            AddBackReference(targetResource, propertyName, resourceToBeAdded);
        } // AddReferenceToCollection

        public virtual void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            targetResource = TokenToResource(targetResource);
            resourceToBeRemoved = TokenToResource(resourceToBeRemoved);

            RemoveBackReference(targetResource, propertyName, resourceToBeRemoved);

            object propertyValue = targetResource.GetType().GetProperty(propertyName).GetValue(targetResource, null);

            propertyValue.GetType().GetMethod("Remove").Invoke(propertyValue, new object[] { resourceToBeRemoved });
        } // RemoveReferenceFromCollection

        public virtual void SetReference(object targetResource, string propertyName, object propertyValue)
        {
            targetResource = TokenToResource(targetResource);
            if (propertyValue != null)
                propertyValue = TokenToResource(propertyValue);

            RemoveBackReference(targetResource, propertyName, propertyValue);

            Type type = targetResource.GetType();
            PropertyInfo property = type.GetProperty(propertyName);
            if (property == null)
                throw new DataServiceException(500, "Could not find property '" + propertyName + "' on instance of type '" + type.FullName + "'");
            property.SetValue(targetResource, propertyValue, null);

            AddBackReference(targetResource, propertyName, propertyValue);
        } // SetReference

        public virtual void SaveChanges()
        {
            foreach (KeyValuePair<object, EntityState> pendingChange in this.PendingChanges)
            {
                switch (pendingChange.Value)
                {
                    case EntityState.Added:
                        AddResource(pendingChange.Key);
                        break;
                    case EntityState.Deleted:
                        // find the entity set for the object
                        IList entitySetInstance = EntitySetDictionary.GetEntitySet(pendingChange.Key.GetType());
                        DeleteEntity(entitySetInstance, pendingChange.Key, true /*throwIfNotPresent*/);
                        break;
                    default:
                        throw new Exception("Unsupported State");
                }
            }

            this.PendingChanges.Clear();
        } // SaveChanges

        public virtual void ClearChanges()
        {
            this.PendingChanges.Clear();
        } // ClearChanges
        #endregion

#if !ASTORIA_PRE_V2
        #region IDataServiceUpdateProvider implementation
        public void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
        {
            if (!checkForEquality.HasValue)
            {
                throw new DataServiceException(417 /*ExpectationFailed*/, "IDataServiceUpdateProvider: Missing ETag for update operation");
            }

            if (!checkForEquality.Value)
                throw new DataServiceException(500, "Should not be called with checkForEquality != true");

            // If-Match: *
            if (!concurrencyValues.Any())
                return;

            object resource = TokenToResource(resourceCookie);

            List<KeyValuePair<string, object>> propertyValues = GetPropertyValues(resource, concurrencyValues.Select(pair => pair.Key));
            try
            {
                EqualPropertyValues(propertyValues, concurrencyValues, true);
            }
            catch (Exception e)
            {
                throw new DataServiceException(412, null, "The etag value in the request header does not match with the current etag value of the object.", null, e);
            }
        } // SetConcurrencyValues
        #endregion
#endif // ASTORIA_PRE_V2

        #region simple IExpandProvider implementation used for call-order testing against V1
        public virtual IEnumerable ApplyExpansions(IQueryable queryable, ICollection<ExpandSegmentCollection> expandPaths)
        {
            return queryable;
        }
        #endregion

        #region reflection helpers
        public static List<KeyValuePair<string, object>> GetPropertyValues(object o, IEnumerable<string> propertyNames)
        {
            Type t = o.GetType();
            List<KeyValuePair<string, object>> values = new List<KeyValuePair<string, object>>();
            foreach (string propertyName in propertyNames)
            {
                PropertyInfo info = t.GetProperty(propertyName);
                if (info != null)
                    values.Add(new KeyValuePair<string, object>(info.Name, info.GetValue(o, null)));
            }
            return values;
        }

        internal static void SetPropertyValues(List<KeyValuePair<string, object>> values, object o)
        {
            Type t = o.GetType();
            foreach (var pair in values)
            {
                PropertyInfo info = t.GetProperty(pair.Key);
                if (info == null)
                    throw new DataServiceException(500, "Property '" + pair.Key + "' does not exist on type '" + t.Name + "'");

                info.SetValue(o, pair.Value, null);
            }
        }


        public static bool EqualPropertyValues(IEnumerable<KeyValuePair<string, object>> values1, IEnumerable<KeyValuePair<string, object>> values2)
        {
            return EqualPropertyValues(values1, values2, false);
        }

        public static bool EqualPropertyValues(IEnumerable<KeyValuePair<string, object>> values1,
            IEnumerable<KeyValuePair<string, object>> values2,
            bool throwOnFailure)
        {
            if (values1.Count() != values2.Count())
            {
                if (throwOnFailure)
                    throw new Exception("Count of values does not match. Expected " + values1.Count() + ", but observed " + values2.Count());
                return false;
            }

            foreach (var pair1 in values1)
            {
                bool found = false;
                foreach (var pair2 in values2)
                {
                    if (pair1.Key != pair2.Key)
                        continue;

                    found = true;

                    // compare values (null-safe) at this position
                    //
                    object value1 = pair1.Value;
                    object value2 = pair2.Value;

                    if (value1 == null && value2 == null)
                        break;

                    if (value1 == null && value2 != null)
                    {
                        if (throwOnFailure)
                            throw new Exception("Value for property '" + pair1.Key + "' does not match. Expected null, observed: " + value2.ToString());
                        return false;
                    }
                    if (value1 != null && value2 == null)
                    {
                        if (throwOnFailure)
                            throw new Exception("Value for property '" + pair1.Key + "' does not match. Observed null, expected: " + value1.ToString());
                        return false;
                    }

                    // see if they are arrays, and compare sub-items if so
                    //
                    Array array1 = value1 as Array;
                    Array array2 = value2 as Array;

                    if (array1 != null || array2 != null)
                    {
                        if (array2 == null)
                        {
                            if (throwOnFailure)
                                throw new Exception("Value for property '" + pair1.Key + "' does not match. Expected array, observed: " + value2.ToString());
                            return false;
                        }
                        if (array1 == null)
                        {
                            if (throwOnFailure)
                                throw new Exception("Value for property '" + pair1.Key + "' does not match. Observed array, expected: " + value1.ToString());
                            return false;
                        }

                        if (array1.Length != array2.Length)
                        {
                            if (throwOnFailure)
                                throw new Exception("Value for property '" + pair1.Key + "' does not match. Expected array of length " + array1.Length + ", observed array of length " + array2.Length);
                            return false;
                        }

                        for (int i = 0; i < array1.Length; i++)
                        {
                            // compare values (null-safe) at this position
                            //
                            value1 = array1.GetValue(i);
                            value2 = array2.GetValue(i);

                            if (value1 == null && value2 != null)
                            {
                                if (throwOnFailure)
                                    throw new Exception("Value for array property '" + pair1.Key + "' does not match at position " + i + ". Expected null, observed: " + value2.ToString());
                                return false;
                            }
                            if (value1 != null && value2 == null)
                            {
                                if (throwOnFailure)
                                    throw new Exception("Value for array property '" + pair1.Key + "' does not match at position " + i + ". Observed null, expected: " + value1.ToString());
                                return false;
                            }
                            if (!value1.Equals(value2))
                            {
                                if (throwOnFailure)
                                    throw new Exception("Value for array property '" + pair1.Key + "' does not match at position " + i + ". Expected: " + value1.ToString() + ". Observed: " + value2.ToString());
                                return false;
                            }
                        }
                        break;
                    }

                    // non null, non array
                    //
                    if (!value1.Equals(value2))
                    {
                        if (throwOnFailure)
                            throw new Exception("Value for property '" + pair1.Key + "' does not match. Expected: " + value1.ToString() + ". Observed: " + value2.ToString());
                        return false;
                    }
                }
                if (!found)
                {
                    if (throwOnFailure)
                        throw new Exception("Could not find corresponding value for property '" + pair1.Key + "'");
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region key helpers
        private static IEnumerable<string> GetKeyPropertyNames(Type t)
        {
            object[] attributes = t.GetCustomAttributes(typeof(KeyAttribute), false);
            if (attributes.Length > 0)
            {
                KeyAttribute attrib = (KeyAttribute)attributes[0];
                return attrib.KeyNames;
            }

            if (t.GetProperty("ID") != null)
                return new string[] { "ID" };

            return new string[] { };
        }

        public static List<KeyValuePair<string, object>> GetKeyValues(object o)
        {
            return GetPropertyValues(o, GetKeyPropertyNames(o.GetType()));
        }

        public static bool EqualKeys(object o, object o2)
        {
            List<KeyValuePair<string, object>> key1 = GetKeyValues(o);
            List<KeyValuePair<string, object>> key2 = GetKeyValues(o2);

            return EqualPropertyValues(key1, key2);
        }

        public static string KeyToString(List<KeyValuePair<string, object>> key)
        {
            return "(" + string.Join(",", key.Select(pair => pair.Key + " = " + pair.Value).ToArray()) + ")";
        }
        #endregion

        #region open type helpers
        private bool TryGetOpenTypeValue(object entity, string propertyName, out object value)
        {
            value = null; //catch all for when we return false

            IEnumerable<KeyValuePair<string, object>> values;
            if (!TryResolveOpenProperties(entity, out values))
                return false;

            foreach (var pair in values)
            {
                if (pair.Key == propertyName)
                {
                    value = pair.Value;
                    return true;
                }
            }
            return false;
        }

        private bool TrySetOpenTypeValue(object entity, string propertyName, object value)
        {
            IDictionary<string, object> dict;
            if (!TryResolveOpenProperties(entity, out dict))
                return false;

            dict[propertyName] = value;
            return true;
        }

        private bool TryResolveOpenProperties<T>(object entity, out T propertyBag) where T : IEnumerable<KeyValuePair<string, object>>
        {
            propertyBag = default(T); //catch all for when we return false

            // see if the attribute even exists in the current version
            Type OpenTypeAttribute = typeof(DataService<>).Assembly.GetType("Microsoft.OData.Service.OpenTypeAttribute");
            if (OpenTypeAttribute == null)
                return false;

            // figure out if this is an open type, and what the open property name is
            object[] atts = entity.GetType().GetCustomAttributes(OpenTypeAttribute, true);
            if (!atts.Any())
                return false;

            string propertyName = (string)OpenTypeAttribute.GetProperty("PropertyName").GetValue(atts[0], null);

            PropertyInfo property = entity.GetType().GetProperty(propertyName);
            if (property == null || !typeof(T).IsAssignableFrom(property.PropertyType))
            {
                throw new DataServiceException(500,
                            String.Format("Could not find open-type property '{0}' on type '{1}'", propertyName, entity.GetType().Name));
            }

            propertyBag = (T)property.GetValue(entity, null);
            return true;
        }

        #endregion

        internal void AddResource(object resource)
        {
            IList entitySetInstance = EntitySetDictionary.GetEntitySet(resource.GetType());
            foreach (object entity in entitySetInstance)
            {
                // check if there is not another instance with the same id
                if (EqualKeys(resource, entity))
                {
                    throw new DataServiceException(500, "An entity with the given key already exists");
                }
            }
            entitySetInstance.Add(resource);
        }

        private void DeleteEntity(IEnumerable collection, object entity, bool throwIfNotPresent)
        {
            object entityToBeDeleted = collection.Cast<object>().SingleOrDefault(o => EqualKeys(entity, o));

            if (entityToBeDeleted == null && throwIfNotPresent)
            {
                throw new Exception("No entity found with the given ID");
            }

            if (entityToBeDeleted != null)
            {
                // Make sure that property type implements ICollection<T> If yes, then call remove method on it to remove the
                // resource
                Type collectionType = collection.GetType();
                if (!typeof(ICollection).IsAssignableFrom(collectionType))
                    throw new DataServiceException(500, "Collection type does not implement ICollection");
                if(!collectionType.IsGenericType)
                    throw new DataServiceException(500, "Collection type is not generic");
                Type elementType = collectionType.GetGenericArguments()[0];

                Type genericType = typeof(ICollection<>).MakeGenericType(elementType);
                if (!genericType.IsAssignableFrom(collectionType))
                    throw new DataServiceException(500, "Collection is not an ICollection<T>");

                genericType.GetMethod("Remove").Invoke(collection, new object[] { entityToBeDeleted });
            }
        }

        private static string GetOtherSideNavigationProperty(object resource, string currentNavProperty)
        {
            MethodInfo mi = resource.GetType().GetMethod("GetOtherSideNavigationProperty", BindingFlags.Instance | BindingFlags.NonPublic);
            if (mi != null)
            {
                return (string)mi.Invoke(resource, new object[] { currentNavProperty });
            }
            return null;
        }

        private static void RemoveBackReference(object targetResource, string propertyName, object otherAssociatedResource)
        {
            string otherPropertyName = GetOtherSideNavigationProperty(targetResource, propertyName);
            if (otherPropertyName == null)
                return;

            if (otherAssociatedResource == null)
                return;

            PropertyInfo otherProperty = otherAssociatedResource.GetType().GetProperty(otherPropertyName);
            if (otherProperty == null)
                throw new DataServiceException(500, String.Format("Unable to set the back pointer, cannot find property:{0} on type:{1}", otherPropertyName, otherAssociatedResource.GetType().Name));

            if (typeof(IList).IsAssignableFrom(otherProperty.PropertyType))
            {
                IList otherSidePropertyValueAsList = otherProperty.GetValue(otherAssociatedResource, null) as IList;
                foreach (object child in otherSidePropertyValueAsList)
                {
                    if (EqualKeys(child, targetResource))
                    {
                        otherSidePropertyValueAsList.Remove(targetResource);
                        break;
                    }
                }
            }
            else
            {
                otherProperty.SetValue(otherAssociatedResource, null, null);
            }
        }

        private static void AddBackReference(object targetResource, string propertyName, object otherAssociatedResource)
        {
            string otherPropertyName = GetOtherSideNavigationProperty(targetResource, propertyName);
            if (otherPropertyName == null)
                return;

            if (otherAssociatedResource == null)
                return;

            PropertyInfo otherProperty = otherAssociatedResource.GetType().GetProperty(otherPropertyName);
            if (otherProperty == null)
                throw new DataServiceException(500, String.Format("Unable to set the back pointer, cannot find property:{0} on type:{1}", otherPropertyName, otherAssociatedResource.GetType().Name));

            // if its a list, add the value
            if (typeof(IList).IsAssignableFrom(otherProperty.PropertyType))
            {
                IList otherSidePropertyValueAsList = otherProperty.GetValue(otherAssociatedResource, null) as IList;

                if(!otherSidePropertyValueAsList.Cast<object>().Any(o => EqualKeys(o, targetResource)))
                    otherSidePropertyValueAsList.Add(targetResource);
            }
            // otherwise just set the value
            else
            {
                otherProperty.SetValue(otherAssociatedResource, targetResource, null);
            }
        }

        private static void AddData(IUpdatable updatable)
        {
            // Do Nothing
        } // AddData
    }
}
