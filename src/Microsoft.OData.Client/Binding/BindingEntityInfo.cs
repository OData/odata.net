//---------------------------------------------------------------------
// <copyright file="BindingEntityInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Microsoft.OData.Client.Metadata;
    #endregion

    /// <summary>Type of property stored in BindingPropertyInfo.</summary>
    internal enum BindingPropertyKind
    {
        /// <summary>Property type is a complex type.</summary>
        Complex,

        /// <summary>Property type is an entity type with keys.</summary>
        Entity,

        /// <summary>Property is a DataServiceCollection.</summary>
        DataServiceCollection,

        /// <summary>Property is a collection of primitives or complex types.</summary>
        PrimitiveOrComplexCollection
    }

    /// <summary>Cache of information about entity types and their observable properties</summary>
    internal class BindingEntityInfo
    {
        /// <summary>Object reference used as a 'False' flag.</summary>
        private static readonly object FalseObject = new object();

        /// <summary>Object reference used as a 'True' flag.</summary>
        private static readonly object TrueObject = new object();

        /// <summary>Lock on metadata caches.</summary>
        private static readonly ReaderWriterLockSlim metadataCacheLock = new ReaderWriterLockSlim();

        /// <summary>Types which are known not to be entity types.</summary>
        private static readonly HashSet<Type> knownNonEntityTypes = new HashSet<Type>(EqualityComparer<Type>.Default);

        /// <summary>Types which are known to be (or not) collection types.</summary>
        private static readonly Dictionary<Type, object> knownObservableCollectionTypes = new Dictionary<Type, object>(EqualityComparer<Type>.Default);

        /// <summary>Mapping between types and their corresponding entity information</summary>
        private static readonly Dictionary<Type, BindingEntityInfoPerType> bindingEntityInfos = new Dictionary<Type, BindingEntityInfoPerType>(EqualityComparer<Type>.Default);

        /// <summary>Obtain binding info corresponding to a given type</summary>
        /// <param name="entityType">Type for which to obtain information</param>
        /// <param name="model">the client model.</param>
        /// <returns>Info about the <paramref name="entityType"/></returns>
        internal static IList<BindingPropertyInfo> GetObservableProperties(Type entityType, ClientEdmModel model)
        {
            return GetBindingEntityInfoFor(entityType, model).ObservableProperties;
        }

        /// <summary>Gets the ClientType corresponding to the given type</summary>
        /// <param name="entityType">Input type</param>
        /// <param name="model">The client model.</param>
        /// <returns>Corresponding ClientType</returns>
        internal static ClientTypeAnnotation GetClientType(Type entityType, ClientEdmModel model)
        {
            return GetBindingEntityInfoFor(entityType, model).ClientType;
        }

        /// <summary>
        /// Get the entity set name for the target entity object.
        /// </summary>
        /// <param name="target">An entity object.</param>
        /// <param name="targetEntitySet">The 'currently known' entity set name for the target object.</param>
        /// <param name="model">The client model.</param>
        /// <returns>The entity set name for the target object.</returns>
        /// <remarks>
        /// Allow user code to provide the entity set name. If user code does not provide the entity set name, then
        /// this method will get the entity set name from the value of the EntitySetAttribute.
        /// The 'currently known' entity set name for top level collections can be provided through OEC constructor
        /// </remarks>
        internal static string GetEntitySet(
            object target,
            string targetEntitySet,
            ClientEdmModel model)
        {
            Debug.Assert(target != null, "Argument 'target' cannot be null.");
            Debug.Assert(BindingEntityInfo.IsEntityType(target.GetType(), model), "Argument 'target' must be an entity type.");

            // Here's the rules in order of priority for resolving entity set name
            // 1. EntitySet name passed in the constructor or extension methods of DataServiceCollection
            // 2. EntitySet name specified in the EntitySet attribute by the code gen. {Remember this attribute is
            //    not generated in case of MEST)
            if (!String.IsNullOrEmpty(targetEntitySet))
            {
                return targetEntitySet;
            }
            else
            {
                // If there is not a 'currently known' entity set name to validate against, then there must be
                // EntitySet attribute on the entity type
                return BindingEntityInfo.GetEntitySetAttribute(target.GetType(), model);
            }
        }

        /// <summary>
        /// Determine if the specified type is an DataServiceCollection.
        /// </summary>
        /// <remarks>
        /// If there a generic class in the inheritance hierarchy of the type, that has a single
        /// entity type parameter T, and is assignable to DataServiceCollection(Of T), then
        /// the type is an DataServiceCollection.
        /// </remarks>
        /// <param name="collectionType">An object type specifier.</param>
        /// <param name="model">The client model.</param>
        /// <returns>true if the type is an DataServiceCollection; otherwise false.</returns>
        internal static bool IsDataServiceCollection(Type collectionType, ClientEdmModel model)
        {
            Debug.Assert(collectionType != null, "Argument 'collectionType' cannot be null.");

            metadataCacheLock.EnterReadLock();
            try
            {
                object resultAsObject;
                if (knownObservableCollectionTypes.TryGetValue(collectionType, out resultAsObject))
                {
                    return resultAsObject == TrueObject;
                }
            }
            finally
            {
                metadataCacheLock.ExitReadLock();
            }

            Type type = collectionType;
            bool result = false;

            while (type != null)
            {
                if (type.IsGenericType())
                {
                    // Is there a generic class in the inheritance hierarchy, that has a single
                    // entity type parameter T, and is assignable to DataServiceCollection<T>
                    Type[] parms = type.GetGenericArguments();

                    if (parms != null && parms.Length == 1 && ClientTypeUtil.TypeOrElementTypeIsEntity(parms[0]))
                    {
                        // if ObservableCollection is not available dataServiceCollection will be null
                        Type dataServiceCollection = WebUtil.GetDataServiceCollectionOfT(parms);
                        if (dataServiceCollection != null && dataServiceCollection.IsAssignableFrom(type))
                        {
                            result = true;
                            break;
                        }
                    }
                }

                type = type.GetBaseType();
            }

            metadataCacheLock.EnterWriteLock();
            try
            {
                if (!knownObservableCollectionTypes.ContainsKey(collectionType))
                {
                    knownObservableCollectionTypes[collectionType] = result ? TrueObject : FalseObject;
                }
            }
            finally
            {
                metadataCacheLock.ExitWriteLock();
            }

            return result;
        }

        /// <summary>
        /// Determine if the specified type is an entity type.
        /// </summary>
        /// <param name="type">An object type specifier.</param>
        /// <param name="model">The client model.</param>
        /// <returns>true if the type is an entity type; otherwise false.</returns>
        internal static bool IsEntityType(Type type, ClientEdmModel model)
        {
            Debug.Assert(type != null, "Argument 'type' cannot be null.");

            metadataCacheLock.EnterReadLock();
            try
            {
                if (knownNonEntityTypes.Contains(type))
                {
                    return false;
                }
            }
            finally
            {
                metadataCacheLock.ExitReadLock();
            }

            bool isEntityType;
            try
            {
                if (BindingEntityInfo.IsDataServiceCollection(type, model))
                {
                    return false;
                }

                isEntityType = ClientTypeUtil.TypeOrElementTypeIsEntity(type);
            }
            catch (InvalidOperationException)
            {
                isEntityType = false;
            }

            if (!isEntityType)
            {
                metadataCacheLock.EnterWriteLock();
                try
                {
                    if (!knownNonEntityTypes.Contains(type))
                    {
                        knownNonEntityTypes.Add(type);
                    }
                }
                finally
                {
                    metadataCacheLock.ExitWriteLock();
                }
            }

            return isEntityType;
        }

        /// <summary>
        /// Tries to get the value of a property and corresponding BindingPropertyInfo or ClientPropertyAnnotation if the property exists
        /// </summary>
        /// <param name="source">Source object whose property needs to be read</param>
        /// <param name="sourceProperty">Name of the source object property</param>
        /// <param name="model">The client model.</param>
        /// <param name="bindingPropertyInfo">BindingPropertyInfo corresponding to <paramref name="sourceProperty"/></param>
        /// <param name="clientProperty">Instance of ClientProperty corresponding to <paramref name="sourceProperty"/></param>
        /// <param name="propertyValue">Value of the property</param>
        /// <returns>true if the property exists and the value was read; otherwise false.</returns>
        internal static bool TryGetPropertyValue(object source, string sourceProperty, ClientEdmModel model, out BindingPropertyInfo bindingPropertyInfo, out ClientPropertyAnnotation clientProperty, out object propertyValue)
        {
            Type sourceType = source.GetType();

            bindingPropertyInfo = BindingEntityInfo.GetObservableProperties(sourceType, model)
                                                   .SingleOrDefault(x => x.PropertyInfo.PropertyName == sourceProperty);

            bool propertyFound = bindingPropertyInfo != null;

            // bindingPropertyInfo is null for primitive properties.
            if (!propertyFound)
            {
                clientProperty = BindingEntityInfo.GetClientType(sourceType, model)
                    .GetProperty(sourceProperty, UndeclaredPropertyBehavior.Support);

                propertyFound = clientProperty != null;
                if (!propertyFound)
                {
                    propertyValue = null;
                }
                else
                {
                    propertyValue = clientProperty.GetValue(source);
                }
            }
            else
            {
                clientProperty = null;
                propertyValue = bindingPropertyInfo.PropertyInfo.GetValue(source);
            }

            return propertyFound;
        }

        /// <summary>Obtain binding info corresponding to a given type</summary>
        /// <param name="entityType">Type for which to obtain information</param>
        /// <param name="model">The client model.</param>
        /// <returns>Info about the <paramref name="entityType"/></returns>
        private static BindingEntityInfoPerType GetBindingEntityInfoFor(Type entityType, ClientEdmModel model)
        {
            BindingEntityInfoPerType bindingEntityInfo;

            metadataCacheLock.EnterReadLock();
            try
            {
                if (bindingEntityInfos.TryGetValue(entityType, out bindingEntityInfo))
                {
                    return bindingEntityInfo;
                }
            }
            finally
            {
                metadataCacheLock.ExitReadLock();
            }

            bindingEntityInfo = new BindingEntityInfoPerType();

            // Try to get the entity set name from the EntitySetAttribute attributes. In order to make the
            // inheritance work, we need to look at the attributes declared in the base types also.
            // EntitySetAttribute does not allow multiples, so there can be at most 1 instance on the type.
            EntitySetAttribute entitySetAttribute = (EntitySetAttribute)entityType.GetCustomAttributes(typeof(EntitySetAttribute), true).SingleOrDefault();

            // There must be exactly one (unambiguous) EntitySetAttribute attribute.
            bindingEntityInfo.EntitySet = entitySetAttribute != null ? entitySetAttribute.EntitySet : null;
            bindingEntityInfo.ClientType = model.GetClientTypeAnnotation(model.GetOrCreateEdmType(entityType));

            foreach (ClientPropertyAnnotation p in bindingEntityInfo.ClientType.Properties())
            {
                BindingPropertyInfo bpi = null;
                Type propertyType = p.PropertyType;

                if (p.IsStreamLinkProperty)
                {
                    // DataServiceStreamLink is not mutable externally
                    // It implements INPC to notify controls about our updates
                    // We should ignore its events since we are the only one updating it.
                    continue;
                }
                else if (p.IsPrimitiveOrEnumOrComplexCollection)
                {
                    Debug.Assert(!BindingEntityInfo.IsDataServiceCollection(propertyType, model), "DataServiceCollection cannot be the type that backs collections of primitives or complex types.");
                    bpi = new BindingPropertyInfo { PropertyKind = BindingPropertyKind.PrimitiveOrComplexCollection };
                }
                else if (p.IsEntityCollection)
                {
                    if (BindingEntityInfo.IsDataServiceCollection(propertyType, model))
                    {
                        bpi = new BindingPropertyInfo { PropertyKind = BindingPropertyKind.DataServiceCollection };
                    }
                }
                else if (BindingEntityInfo.IsEntityType(propertyType, model))
                {
                    bpi = new BindingPropertyInfo { PropertyKind = BindingPropertyKind.Entity };
                }
                else if (BindingEntityInfo.CanBeComplexType(propertyType))
                {
                    // Add complex types and nothing else.
                    Debug.Assert(!p.IsKnownType, "Known types do not implement INotifyPropertyChanged.");
                    bpi = new BindingPropertyInfo { PropertyKind = BindingPropertyKind.Complex };
                }

                if (bpi != null)
                {
                    bpi.PropertyInfo = p;

                    // For entity types, all of the above types of properties are interesting.
                    // For complex types, only observe collections and complex type properties.
                    if (bindingEntityInfo.ClientType.IsEntityType ||
                        bpi.PropertyKind == BindingPropertyKind.Complex ||
                        bpi.PropertyKind == BindingPropertyKind.PrimitiveOrComplexCollection)
                    {
                        bindingEntityInfo.ObservableProperties.Add(bpi);
                    }
                }
            }

            metadataCacheLock.EnterWriteLock();
            try
            {
                if (!bindingEntityInfos.ContainsKey(entityType))
                {
                    bindingEntityInfos[entityType] = bindingEntityInfo;
                }
            }
            finally
            {
                metadataCacheLock.ExitWriteLock();
            }

            return bindingEntityInfo;
        }

        /// <summary>Checks whether a given type can be a complex type i.e. implements INotifyPropertyChanged.</summary>
        /// <param name="type">Input type.</param>
        /// <returns>true if the type is complex type, false otherwise.</returns>
        private static bool CanBeComplexType(Type type)
        {
            Debug.Assert(type != null, "type != null");
            return typeof(INotifyPropertyChanged).IsAssignableFrom(type);
        }

        /// <summary>Gets entity set corresponding to a given type</summary>
        /// <param name="entityType">Input type</param>
        /// <param name="model">The client model.</param>
        /// <returns>Entity set name for the type</returns>
        private static string GetEntitySetAttribute(Type entityType, ClientEdmModel model)
        {
            return GetBindingEntityInfoFor(entityType, model).EntitySet;
        }

        /// <summary>Information about a property interesting for binding</summary>
        internal class BindingPropertyInfo
        {
            /// <summary>Property information</summary>
            public ClientPropertyAnnotation PropertyInfo
            {
                get;
                set;
            }

            /// <summary>Kind of the property i.e. complex, entity or collection.</summary>
            public BindingPropertyKind PropertyKind
            {
                get;
                set;
            }
        }

        /// <summary>Holder of information about entity properties for a type</summary>
        private sealed class BindingEntityInfoPerType
        {
            /// <summary>Collection of properties interesting to the observer</summary>
            private List<BindingPropertyInfo> observableProperties;

            /// <summary>Constructor</summary>
            public BindingEntityInfoPerType()
            {
                this.observableProperties = new List<BindingPropertyInfo>();
            }

            /// <summary>Entity set of the entity</summary>
            public String EntitySet
            {
                get;
                set;
            }

            /// <summary>Corresponding ClientTyp</summary>
            public ClientTypeAnnotation ClientType
            {
                get;
                set;
            }

            /// <summary>Collection of properties interesting to the observer</summary>
            public List<BindingPropertyInfo> ObservableProperties
            {
                get
                {
                    return this.observableProperties;
                }
            }
        }
    }
}
