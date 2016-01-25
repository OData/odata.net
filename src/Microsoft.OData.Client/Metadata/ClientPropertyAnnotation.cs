//---------------------------------------------------------------------
// <copyright file="ClientPropertyAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Metadata
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Edm;
    using c = Microsoft.OData.Client;

    #endregion Namespaces.

    /// <summary>
    /// Represents an EDM property for client types and caches methods for the propertyInfo.
    /// </summary>
    [DebuggerDisplay("{PropertyName}")]
    internal sealed class ClientPropertyAnnotation
    {
        /// <summary>Back reference to the EdmProperty this annotation is part of.</summary>
        internal readonly IEdmProperty EdmProperty;

        /// <summary>property name for debugging</summary>
        internal readonly string PropertyName;

        /// <summary>PropertyInfo in CLR type.</summary>
        internal readonly PropertyInfo PropertyInfo;

        /// <summary>Exact property type; possibly nullable but not necessarily so.</summary>
        internal readonly Type NullablePropertyType;

        /// <summary>type of the property</summary>
        internal readonly Type PropertyType;

        /// <summary>what is the dictionary value type</summary>
        internal readonly Type DictionaryValueType;

        /// <summary>what type was this property declared on?</summary>
        internal readonly Type DeclaringClrType;

        /// <summary>
        /// Is this a known primitive/reference type or an entity/complex/collection type?
        /// </summary>
        internal readonly bool IsKnownType;

        /// <summary>property getter</summary>
        private readonly Func<Object, Object> propertyGetter;

        /// <summary>property setter</summary>
        private readonly Action<Object, Object> propertySetter;

        /// <summary>"set_Item" method supporting IDictionary properties</summary>
        private readonly Action<Object, String, Object> dictionarySetter;

        /// <summary>"Add" method supporting ICollection&lt;&gt; properties</summary>
        private readonly Action<Object, Object> collectionAdd;

        /// <summary>"Remove" method supporting ICollection&lt;&gt; properties</summary>
        private readonly Func<Object, Object, Boolean> collectionRemove;

        /// <summary>"Contains" method supporting ICollection&lt;&gt; properties</summary>
        private readonly Func<Object, Object, Boolean> collectionContains;

        /// <summary>"Clear" method supporting ICollection&lt;&gt; properties</summary>
        private readonly Action<Object> collectionClear;

        /// <summary>ICollection&lt;&gt; generic type</summary>
        private readonly Type collectionGenericType;

        /// <summary>cached value for IsPrimitiveOrEnumOrComplexCollection property</summary>
        private bool? isPrimitiveOrEnumOrComplexCollection;

        /// <summary>cached value of IsGeographyOrGeometry property</summary>
        private bool? isSpatialType;

        /// <summary>The other property in this type that holds the MIME type for this one</summary>
        private ClientPropertyAnnotation mimeTypeProperty;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="edmProperty">Back reference to the EdmProperty this annotation is part of.</param>
        /// <param name="propertyInfo">propertyInfo instance.</param>
        /// <param name="model">The client model.</param>
        internal ClientPropertyAnnotation(IEdmProperty edmProperty, PropertyInfo propertyInfo, ClientEdmModel model)
        {
            Debug.Assert(edmProperty != null, "edmProperty != null");
            Debug.Assert(propertyInfo != null, "null propertyInfo");

            // Property should always have DeclaringType
            Debug.Assert(propertyInfo.DeclaringType != null, "Property without a declaring type");

            this.EdmProperty = edmProperty;
            this.PropertyName = ClientTypeUtil.GetServerDefinedName(propertyInfo);
            this.PropertyInfo = propertyInfo;
            this.NullablePropertyType = propertyInfo.PropertyType;
            this.PropertyType = Nullable.GetUnderlyingType(this.NullablePropertyType) ?? this.NullablePropertyType;
            this.DeclaringClrType = propertyInfo.DeclaringType;

            MethodInfo propertyGetMethod = propertyInfo.GetGetMethod();

            // Add the parameter to make set method is returned even it is not public. Portable lib does not support this.
#if PORTABLELIB
            MethodInfo propertySetMethod = propertyInfo.GetSetMethod();
#else
            MethodInfo propertySetMethod = propertyInfo.GetSetMethod(true);
#endif

            ParameterExpression instance = Expression.Parameter(typeof(Object), "instance");
            ParameterExpression value = Expression.Parameter(typeof(Object), "value");

            // instance => (Object)(((T)instance).get_PropertyName());  <-- we need to box the value back to object to return
            this.propertyGetter = propertyGetMethod == null ? null : (Func<object, object>)Expression.Lambda(
                Expression.Convert(
                    Expression.Call(
                        Expression.Convert(instance, this.DeclaringClrType),
                        propertyGetMethod),
                    typeof(Object)),
                instance).Compile();

            // (instance, value) => ((T)instance).set_PropertyName((T1)value);
            this.propertySetter = propertySetMethod == null ? null : (Action<object, object>)Expression.Lambda(
                Expression.Call(
                    Expression.Convert(instance, this.DeclaringClrType),
                    propertySetMethod,
                    Expression.Convert(value, this.NullablePropertyType)),
                instance,
                value).Compile();

            this.Model = model;

            this.IsKnownType = PrimitiveType.IsKnownType(this.PropertyType);

            // non primitive types: dictionary/collections
            if (!this.IsKnownType)
            {
                var setMethodInfo = ClientTypeUtil.GetMethodForGenericType(this.PropertyType, typeof(IDictionary<,>), "set_Item", out this.DictionaryValueType);

                if (setMethodInfo != null)
                {
                    ParameterExpression propertyNameParam = Expression.Parameter(typeof(String), "propertyName");

                    // (instance, propertyName, value) => ((IDictionary<string, DictionaryValueType>)instance)[propertyName] = (DictionaryValueType)value;
                    this.dictionarySetter = (Action<Object, String, Object>)Expression.Lambda(
                        Expression.Call(
                            Expression.Convert(instance, typeof(IDictionary<,>).MakeGenericType(typeof(String), this.DictionaryValueType)),
                            setMethodInfo,
                            propertyNameParam,
                            Expression.Convert(value, this.DictionaryValueType)),
                        instance,
                        propertyNameParam,
                        value).Compile();
                }
                else
                {
                    var containsMethod = ClientTypeUtil.GetMethodForGenericType(this.PropertyType, typeof(ICollection<>), "Contains", out this.collectionGenericType);
                    var addMethod = ClientTypeUtil.GetAddToCollectionMethod(this.PropertyType, out this.collectionGenericType);
                    var removeMethod = ClientTypeUtil.GetMethodForGenericType(this.PropertyType, typeof(ICollection<>), "Remove", out this.collectionGenericType);
                    var clearMethod = ClientTypeUtil.GetMethodForGenericType(this.PropertyType, typeof(ICollection<>), "Clear", out this.collectionGenericType);

                    // (instance, value) => ((PropertyType)instance).Contains((CollectionType)value);  returns boolean
                    this.collectionContains = containsMethod == null ? null : (Func<Object, Object, Boolean>)Expression.Lambda(
                        Expression.Call(
                            Expression.Convert(instance, this.PropertyType),
                            containsMethod,
                            Expression.Convert(value, this.collectionGenericType)),
                        instance,
                        value).Compile();

                    // (instance, value) => ((PropertyType)instance).Add((CollectionType)value);
                    this.collectionAdd = addMethod == null ? null : (Action<Object, Object>)Expression.Lambda(
                        Expression.Call(
                            Expression.Convert(instance, this.PropertyType),
                            addMethod,
                            Expression.Convert(value, this.collectionGenericType)),
                        instance,
                        value).Compile();

                    // (instance, value) => ((PropertyType)instance).Remove((CollectionType)value);  returns boolean
                    this.collectionRemove = removeMethod == null ? null : (Func<Object, Object, Boolean>)Expression.Lambda(
                        Expression.Call(
                            Expression.Convert(instance, this.PropertyType),
                            removeMethod,
                            Expression.Convert(value, this.collectionGenericType)),
                        instance,
                        value).Compile();

                    // (instance) => ((PropertyType)instance).Clear();
                    this.collectionClear = clearMethod == null ? null : (Action<Object>)Expression.Lambda(
                        Expression.Call(
                             Expression.Convert(instance, this.PropertyType),
                             clearMethod),
                        instance).Compile();
                }
            }

            Debug.Assert(this.collectionGenericType == null || this.DictionaryValueType == null, "collectionGenericType and DictionaryItemType mutually exclusive. (They both can be null though).");
        }

        /// <summary>
        /// Gets the client model.
        /// </summary>
        internal ClientEdmModel Model { get; private set; }

        /// <summary>The other property in this type that holds the MIME type for this one</summary>
        internal ClientPropertyAnnotation MimeTypeProperty
        {
            get { return this.mimeTypeProperty; }
            set { this.mimeTypeProperty = value; }
        }

        /// <summary>what is the nested collection element</summary>
        internal Type EntityCollectionItemType
        {
            get { return this.IsEntityCollection ? this.collectionGenericType : null; }
        }

        /// <summary>Is this property a collection of entities?</summary>
        internal bool IsEntityCollection
        {
            get { return this.collectionGenericType != null && !this.IsPrimitiveOrEnumOrComplexCollection; }
        }

        /// <summary>Type of items in the primitive or complex collection.</summary>
        internal Type PrimitiveOrComplexCollectionItemType
        {
            get
            {
                if (this.IsPrimitiveOrEnumOrComplexCollection)
                {
                    return this.collectionGenericType;
                }

                return null;
            }
        }

        /// <summary>Returns if this property is enum type.</summary>
        internal bool IsEnumType
        {
            get { return this.PropertyType.IsEnum(); }
        }

        /// <summary>Is this property a collection of primitive or enum or complex types?</summary>
        internal bool IsPrimitiveOrEnumOrComplexCollection
        {
            get
            {
                if (!this.isPrimitiveOrEnumOrComplexCollection.HasValue)
                {
                    if (this.collectionGenericType == null)
                    {
                        this.isPrimitiveOrEnumOrComplexCollection = false;
                    }
                    else
                    {
                        bool collection = this.EdmProperty.PropertyKind == EdmPropertyKind.Structural && this.EdmProperty.Type.TypeKind() == EdmTypeKind.Collection;

                        this.isPrimitiveOrEnumOrComplexCollection = collection;
                    }
                }

                return this.isPrimitiveOrEnumOrComplexCollection.Value;
            }
        }

        /// <summary>Returns true if the type of property is a Geography or Geometry type, otherwise returns false.</summary>
        internal bool IsSpatialType
        {
            get
            {
                if (!this.isSpatialType.HasValue)
                {
                    if (typeof(Microsoft.Spatial.ISpatial).IsAssignableFrom(this.PropertyType))
                    {
                        this.isSpatialType = true;
                    }
                    else
                    {
                        this.isSpatialType = false;
                    }
                }

                return this.isSpatialType.Value;
            }
        }

        /// <summary>Is this property a dictionary?</summary>
        internal bool IsDictionary
        {
            get { return this.DictionaryValueType != null; }
        }

        /// <summary>Returns true if this property is a stream link property, otherwise false.</summary>
        internal bool IsStreamLinkProperty
        {
            get { return this.PropertyType == typeof(DataServiceStreamLink); }
        }

        /// <summary>
        /// get property value from an object
        /// </summary>
        /// <param name="instance">object to get the property value from</param>
        /// <returns>property value</returns>
        internal object GetValue(object instance)
        {
            Debug.Assert(null != instance, "null instance");
            Debug.Assert(null != this.propertyGetter, "null propertyGetter");
            return this.propertyGetter.Invoke(instance);
        }

        /// <summary>
        /// remove a item from the collection instance
        /// </summary>
        /// <param name="instance">collection</param>
        /// <param name="value">item to remove</param>
        internal void RemoveValue(object instance, object value)
        {
            Debug.Assert(null != instance, "null instance");
            Debug.Assert(null != this.collectionRemove, "missing removeMethod");

            Debug.Assert(this.PropertyType.IsAssignableFrom(instance.GetType()), "unexpected collection instance");
            Debug.Assert((null == value) || this.EntityCollectionItemType.IsAssignableFrom(value.GetType()) || this.PrimitiveOrComplexCollectionItemType.IsAssignableFrom(value.GetType()), "unexpected collection value to remove");
            this.collectionRemove.Invoke(instance, value);
        }

        /// <summary>
        /// set property value on an object
        /// </summary>
        /// <param name="instance">object to set the property value on</param>
        /// <param name="value">property value</param>
        /// <param name="propertyName">used for open type</param>
        /// <param name="allowAdd">allow add to a collection if available, else allow setting collection property</param>
        internal void SetValue(object instance, object value, string propertyName, bool allowAdd)
        {
            Debug.Assert(null != instance, "null instance");
            if (null != this.dictionarySetter)
            {
                {
                    Debug.Assert(this.PropertyType.IsAssignableFrom(instance.GetType()), "unexpected dictionary instance");
                    Debug.Assert((null == value) || this.DictionaryValueType.IsAssignableFrom(value.GetType()), "unexpected dictionary value to set");

                    // ((IDictionary<string, DictionaryValueType>)instance)[propertyName] = (DictionaryValueType)value;
                    this.dictionarySetter.Invoke(instance, propertyName, value);
                }
            }
            else if (allowAdd && (null != this.collectionAdd))
            {
                Debug.Assert(this.PropertyType.IsAssignableFrom(instance.GetType()), "unexpected collection instance");
                Debug.Assert(
                    (null == value) ||
                    (this.EntityCollectionItemType != null && this.EntityCollectionItemType.IsAssignableFrom(value.GetType())) ||
                    (this.PrimitiveOrComplexCollectionItemType != null && this.PrimitiveOrComplexCollectionItemType.IsAssignableFrom(value.GetType())),
                    "unexpected collection value to add");

                if (!this.collectionContains.Invoke(instance, value))
                {
                    this.AddValueToBackingICollectionInstance(instance, value);
                }
            }
            else if (null != this.propertySetter)
            {
                // ((ElementType)instance).PropertyName = (PropertyType)value;
                this.propertySetter.Invoke(instance, value);
            }
            else
            {
                throw c.Error.InvalidOperation(c.Strings.ClientType_MissingProperty(value.GetType().ToString(), propertyName));
            }
        }

        /// <summary>
        /// Clears <paramref name="collectionInstance"/>.
        /// </summary>
        /// <param name="collectionInstance">ICollection instance that needs to be cleared.</param>
        internal void ClearBackingICollectionInstance(object collectionInstance)
        {
            Debug.Assert(this.IsEntityCollection || this.IsPrimitiveOrEnumOrComplexCollection, "this.IsEntityCollection or this.IsPrimitiveOrComplexCollection has to be true otherwise it is not a collection");
            Debug.Assert(this.collectionClear != null, "For collections the clearMethod must not be null");

            this.collectionClear.Invoke(collectionInstance);
        }

        /// <summary>
        /// Adds value to a collection.
        /// </summary>
        /// <param name="collectionInstance">ICollection Instance to add <paramref name="value"/> to.</param>
        /// <param name="value">Value to be added to <paramref name="collectionInstance" />.</param>
        internal void AddValueToBackingICollectionInstance(object collectionInstance, object value)
        {
            Debug.Assert(this.IsEntityCollection || this.IsPrimitiveOrEnumOrComplexCollection, "this.IsEntityCollection or this.IsPrimitiveOrComplexCollection has to be true otherwise it is not a collection");
            Debug.Assert(this.collectionAdd != null, "For collections the addMethod must not be null");

            this.collectionAdd.Invoke(collectionInstance, value);
        }
    }
}
