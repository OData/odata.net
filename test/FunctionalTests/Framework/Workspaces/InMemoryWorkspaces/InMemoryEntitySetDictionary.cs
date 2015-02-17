//---------------------------------------------------------------------
// <copyright file="InMemoryEntitySetDictionary.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace System.Data.Test.Astoria.InMemoryLinq
{
    public class InMemoryEntitySetDictionary : Dictionary<Type, IList>, ICloneable
    {

        public InMemoryEntitySetDictionary()
        {
        }

        public IQueryable<T> GetEntitySet<T>() where T : class
        {
            if (!this.ContainsKey(typeof(T)))
                throw new KeyNotFoundException("Unable to find entityset of type:" + typeof(T).Name);
            return ((List<T>)this[typeof(T)]).AsQueryable();
        }
        public IList GetEntitySet(Type type)
        {
            Type baseType = FindBaseTypeKey(type);
            if (baseType == null)
                throw new NullReferenceException("Unable to find a set for the type:" + type.Name);
            if (!this.ContainsKey(baseType))
            {
                throw new KeyNotFoundException("Unable to find entityset of type:" + baseType.Name);
            }
            return this[baseType];
        }
        private Type FindBaseTypeKey(Type t)
        {
            foreach (Type type in this.Keys)
            {
                if (t.IsSubclassOf(type))
                    return type;
                else if (t.Equals(type))
                    return type;
            }
            return null;
        }
        private Type[] GetTypes()
        {
            return this.Keys.ToArray();
        }
        #region ICloneable Members

        public object Clone()
        {
            InMemoryEntitySetDictionary inMemoryEntitySetDictionaryClone = new InMemoryEntitySetDictionary();
            foreach (Type t in this.Keys)
            {
                IList newList = this.CloneList(this[t], t, this.GetTypes());
                inMemoryEntitySetDictionaryClone.Add(t, newList);
            }
            this.HookupRelationshipsInClone(inMemoryEntitySetDictionaryClone, this.GetTypes());
            return inMemoryEntitySetDictionaryClone;
        }
        private IList CloneList(IList list, Type t, Type[] entityTypesToSkipOnClone)
        {
            IList clonedList = (IList)CreateGeneric(typeof(List<>), t, new object[] { });
            foreach (object o in list)
            {
                object clonedEntityObject = CloneEntity(o, entityTypesToSkipOnClone);
                clonedList.Add(clonedEntityObject);
            }
            return clonedList;
        }
        public static object CreateGeneric(Type generic, Type innerType, params object[] args)
        {
            System.Type specificType = generic.MakeGenericType(new System.Type[] { innerType });
            return Activator.CreateInstance(specificType, args);
        }

        private object CloneEntity(object o, Type[] entityTypesToSkipOnClone)
        {
            object clonedObject = Activator.CreateInstance(o.GetType());
            SetPropertiesOnClone(clonedObject, o, entityTypesToSkipOnClone);
            return clonedObject;
        }

        private void SetPropertiesOnClone(object clonedObject, object originalObject, Type[] typesToSkip)
        {
            foreach (PropertyInfo pi in originalObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanWrite == true))
            {
                //Skip list properties
                if (pi.PropertyType.Name.Contains("List"))
                    continue;
                if (typesToSkip.Contains(pi.PropertyType))
                    continue;
                object propertyValue = pi.GetValue(originalObject, null);

                if (typeof(IEnumerable<KeyValuePair<string, object>>).IsAssignableFrom(pi.PropertyType))
                {
                    IEnumerable<KeyValuePair<string, object>> list = propertyValue as IEnumerable<KeyValuePair<string, object>>;
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();

                    foreach (var pair in list)
                    {
                        object value = pair.Value;
                        if (!IsPrimitiveType(value.GetType()))
                            value = CloneObject(value, typesToSkip);
                        dictionary[pair.Key] = value;
                    }

                    if (!typeof(IDictionary).IsAssignableFrom(pi.PropertyType))
                        propertyValue = dictionary.AsEnumerable();
                    else
                        propertyValue = dictionary;
                }
                else if (!IsPrimitiveType(propertyValue.GetType()))
                {
                    //Clone any classes so different reference
                    propertyValue = CloneObject(propertyValue, typesToSkip);
                }

                pi.SetValue(clonedObject, propertyValue, null);
            }
        }
        /// <summary>Checks whether the specified type is a known primitive type.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>true if the specified type is known to be a primitive type; false otherwise.</returns>
        private static bool IsPrimitiveType(Type type)
        {

            //removing the Internal method invocation to fix Partial Trust issues
            return PrimitiveTypesArray.Contains(type);

            // This is really a one-to-one mapping with the internal method.
            //Assembly assembly = typeof(Microsoft.OData.Service.IDataServiceHost).Assembly;
            //Type providerType = assembly.GetType("Microsoft.OData.Service.WebUtil", true);
            //MethodInfo method = providerType.GetMethod("IsPrimitiveType", BindingFlags.NonPublic | BindingFlags.Static);
            //return (bool)method.Invoke(null, new object[] { type });
        }
        private static Type[] PrimitiveTypesArray
        {
            get
            {
                if (primitiveTypes == null)
                {
                    primitiveTypes = new Type[]{
                                   typeof(string),
                typeof(Boolean),
                typeof(Boolean?),
                typeof(Byte),
                typeof(Byte?),
                typeof(DateTime),
                typeof(DateTime?),
                typeof(Decimal),
                typeof(Decimal?),
                typeof(Double),
                typeof(Double?),
                typeof(Guid),
                typeof(Guid?),
                typeof(Int16),
                typeof(Int16?),
                typeof(Int32),
                typeof(Int32?),
                typeof(Int64),
                typeof(Int64?),
                typeof(SByte),
                typeof(SByte?),
                typeof(Single),
                typeof(Single?),
                typeof(byte[]) 
                                    };
                }
                return primitiveTypes;
            }
        }

        private static Type[] primitiveTypes;

        private object CloneObject(object o, Type[] typesToSkip)
        {
            if (!o.GetType().GetConstructors().Any(constr => constr.GetParameters().Length == 0))
            {
                throw (new Exception("Failed to filter out :" + o.GetType().FullName));
            }
            object clonedObject = Activator.CreateInstance(o.GetType());
            SetPropertiesOnClone(clonedObject, o, typesToSkip);
            return clonedObject;
        }
        private void HookupRelationshipsInClone(InMemoryEntitySetDictionary dictionary, Type[] entityTypesToHookup)
        {
            foreach (Type t in dictionary.Keys)
            {
                IList clonedList = dictionary[t];
                IList originalList = this[t];
                for (int i = 0; i < clonedList.Count; i++)
                {
                    FixupEntityObject(originalList[i], clonedList[i], dictionary, entityTypesToHookup);
                }
            }
        }
        private void FixupEntityObject(object original, object cloned, InMemoryEntitySetDictionary dictionary, Type[] entityTypesToHookup)
        {
            //Reference properties
            foreach (PropertyInfo pi in original.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => entityTypesToHookup.Contains(p.PropertyType)))
            {
                IList list = dictionary.GetEntitySet(pi.PropertyType);
                object originalLinkedObject = pi.GetValue(original, null);
                object clonedLinkedObject = null;
                if (originalLinkedObject != null)
                {
                    var key = InMemoryContext.GetKeyValues(originalLinkedObject);

                    List<string> failedKeys = new List<string>();
                    foreach (object o in list)
                    {
                        var otherKey = InMemoryContext.GetKeyValues(o);

                        if (InMemoryContext.EqualPropertyValues(key, otherKey))
                            clonedLinkedObject = o;
                        else
                            failedKeys.Add(InMemoryContext.KeyToString(otherKey));
                    }

                    if (clonedLinkedObject == null)
                    {
                        throw new ApplicationException(String.Format(
                            "Unable to find linked object with key '{0}' for reference property '{1}'. Other keys were: {2}",
                            InMemoryContext.KeyToString(key), pi.Name, string.Join(", ", failedKeys.ToArray())));
                    }

                    pi.SetValue(cloned, clonedLinkedObject, null);
                }
            }
            //Fix up for lists
            foreach (PropertyInfo pi in original.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.PropertyType.Name.Contains("List")))
            {
                Type t = pi.PropertyType.GetGenericArguments()[0];
                IList list = dictionary.GetEntitySet(t);
                IList originalListLinkedObject = (IList)pi.GetValue(original, null);
                IList clonedListLinkedObject = (IList)CreateGeneric(typeof(List<>), t, null);
                foreach (object originalItemInList in originalListLinkedObject)
                {
                    object clonedLinkedObject = null;
                    foreach (object o in list)
                    {
                        if (InMemoryContext.EqualKeys(o, originalItemInList))
                            clonedLinkedObject = o;
                    }
                    if (clonedLinkedObject == null)
                        throw new ApplicationException("Unable to find linked object for collection property '" + pi.Name + "'");
                    clonedListLinkedObject.Add(clonedLinkedObject);
                }
                pi.SetValue(cloned, clonedListLinkedObject, null);
            }
        }


        #endregion
    }
}
