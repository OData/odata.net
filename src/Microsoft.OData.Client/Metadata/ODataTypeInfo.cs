//---------------------------------------------------------------------
// <copyright file="ODataTypeInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Metadata
{

    #region Namespaces.

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Client = Microsoft.OData.Client;

    #endregion Namespaces.

    /// <summary>
    /// Detailed information of a Type.
    /// </summary>
    internal class ODataTypeInfo
    {
        private bool? _hasProperties;
        private PropertyInfo[] _keyProperties;
        private Type type;
        private Dictionary<string, PropertyInfo> _propertyInfoDict;
        private IEnumerable<PropertyInfo> _properties;
        private string _serverDefinedTypeName;
        private string _serverDefinedTypeFullName;
        OriginalNameAttribute originalNameAttribute;


        /// <summary>
        /// Concurrent Dictionary cache to save ClientDefinedName for the type and serverDefinedName
        /// </summary>
        private ConcurrentDictionary<string, string> ServerSideNameDict { get; set; }


        /// <summary>
        /// Creates and instance of <see cref="ODataTypeInfo"/>
        /// </summary>
        public ODataTypeInfo(Type type)
        {
            this.type = type;
            ServerSideNameDict = new ConcurrentDictionary<string, string>();
        }

        /// <summary>
        /// Property Info array for the type
        /// </summary>
        public PropertyInfo[] KeyProperties
        {
            get
            {
                if (_keyProperties == null)
                {
                    _keyProperties = GetKeyProperties();
                }

                return _keyProperties;
            }
        }

        /// <summary>
        /// See if the type has properties
        /// </summary>
        public bool HasProperties
        {
            get
            {
                if (!_hasProperties.HasValue)
                {
                    _properties = GetAllProperties();
                }

                return _hasProperties.Value;
            }
        }

        /// <summary>
        /// Sertver defined type name
        /// </summary>
        public string ServerDefinedTypeName
        {
            get
            {
                if (_serverDefinedTypeName == null)
                {
                    if (originalNameAttribute == null)
                    {
                        originalNameAttribute = (OriginalNameAttribute)type.GetCustomAttributes(typeof(OriginalNameAttribute), false).SingleOrDefault();
                    }

                    if (originalNameAttribute != null)
                    {
                        _serverDefinedTypeName = originalNameAttribute.OriginalName;
                    }
                    else
                    {
                        _serverDefinedTypeName = type.Name;
                    }
                }

                return _serverDefinedTypeName;
            }
        }

        /// <summary>
        /// Sertver defined type full name
        /// </summary>
        public string ServerDefinedTypeFullName
        {
            get
            {
                if (_serverDefinedTypeFullName == null)
                {
                    if (originalNameAttribute == null)
                    {
                        originalNameAttribute = (OriginalNameAttribute)type.GetCustomAttributes(typeof(OriginalNameAttribute), false).SingleOrDefault();
                    }

                    if (originalNameAttribute != null)
                    {
                        _serverDefinedTypeFullName = type.Namespace + "." + originalNameAttribute.OriginalName;
                    }
                    else
                    {
                        _serverDefinedTypeFullName = type.FullName;
                    }

                }

                return _serverDefinedTypeFullName;
            }
        }

        public IEnumerable<PropertyInfo> Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = GetAllProperties();
                }

                return _properties;
            }
        }

        /// <summary>
        /// Get ClientField name for a serverside name
        /// </summary>
        /// <param name="serverSideName">server side name from the list of serverdefined name</param>
        /// <returns>Client field name for the serverside name</returns>
        public string GetClientFieldName(string serverSideName)
        {
            string memberInfoName;

            if (!ServerSideNameDict.TryGetValue(serverSideName, out memberInfoName))
            {
                FieldInfo memberInfo = type.GetField(serverSideName) ?? type.GetFields().ToList().Where(m =>
                {
                    OriginalNameAttribute originalNameAttribute = (OriginalNameAttribute)m.GetCustomAttributes(typeof(OriginalNameAttribute), false).SingleOrDefault();
                    return originalNameAttribute != null && originalNameAttribute.OriginalName == serverSideName;
                }).SingleOrDefault();

                if (memberInfo == null)
                {
                    throw Client.Error.InvalidOperation(Client.Strings.ClientType_MissingProperty(type.ToString(), serverSideName));
                }

                memberInfoName = memberInfo.Name;
                ServerSideNameDict[serverSideName] = memberInfoName;
            }

            return memberInfoName;
        }

        /// <summary>
        /// Gets the clr name according to server defined name in the specified type.
        /// </summary>        
        /// <param name="serverDefinedName">Name from server.</param>
        /// <param name="undeclaredPropertyBehavior">Flag to support untyped properties.</param>
        /// <returns>Client PropertyInfo, or null if the method is not found or throws exception if undeclaredPropertyBehavior is ThrowException.</returns>
        public PropertyInfo GetClientPropertyInfo(string serverDefinedName, UndeclaredPropertyBehavior undeclaredPropertyBehavior)
        {
            PropertyInfo clientPropertyInfo = null;

            if (_propertyInfoDict == null)
            {
                _properties = GetAllProperties();
            }

            if ((!_propertyInfoDict.TryGetValue(serverDefinedName, out clientPropertyInfo)) && undeclaredPropertyBehavior == UndeclaredPropertyBehavior.ThrowException)
            {
                throw Client.Error.InvalidOperation(Client.Strings.ClientType_MissingProperty(type.ToString(), serverDefinedName));
            }

            return clientPropertyInfo;
        }

        private IEnumerable<PropertyInfo> GetAllProperties()
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();
            _hasProperties = false;
            Dictionary<string, PropertyInfo> propertyInfoDict = new Dictionary<string, PropertyInfo>();

            foreach (PropertyInfo propertyInfo in type.GetPublicProperties(true /*instanceOnly*/, false))
            {
                //// examples where class<PropertyType>

                //// the normal examples
                //// PropertyType Property { get; set }
                //// Nullable<PropertyType> Property { get; set; }

                //// if 'Property: struct' then we would be unable set the property during construction (and have them stick)
                //// but when its a class, we can navigate if non-null and set the nested properties
                //// PropertyType Property { get; } where PropertyType: class

                //// we do support adding elements to collections
                //// ICollection<PropertyType> { get; /*ignored set;*/ }

                //// indexed properties are not supporter because
                //// we don't have anything to use as the index
                //// PropertyType Property[object x] { /*ignored get;*/ /*ignored set;*/ }

                //// also ignored
                //// if PropertyType.IsPointer (like byte*)
                //// if PropertyType.IsArray except for byte[] and char[]
                //// if PropertyType == IntPtr or UIntPtr

                //// Properties overriding abstract or virtual properties on a base type
                //// are also ignored (because they are part of the base type declaration
                //// and not of the derived type).

                OriginalNameAttribute originalNameAttribute = (OriginalNameAttribute)propertyInfo.GetCustomAttributes(typeof(OriginalNameAttribute), false).SingleOrDefault();
                string serverDefinedName = originalNameAttribute != null ? originalNameAttribute.OriginalName : propertyInfo.Name;

                propertyInfoDict[serverDefinedName] = propertyInfo;

                Type propertyType = propertyInfo.PropertyType; // class / interface / value
                propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

                if (propertyType.IsPointer ||
                    (propertyType.IsArray && (typeof(byte[]) != propertyType) && typeof(char[]) != propertyType) ||
                    (typeof(IntPtr) == propertyType) ||
                    (typeof(UIntPtr) == propertyType))
                {
                    continue;
                }

                Debug.Assert(!propertyType.ContainsGenericParameters(), "remove when test case is found that encounters this");

                if (propertyInfo.CanRead &&
                    (!propertyType.IsValueType() || propertyInfo.CanWrite) &&
                    !propertyType.ContainsGenericParameters() &&
                    propertyInfo.GetIndexParameters().Length == 0)
                {
                    properties.Add(propertyInfo);
                    _hasProperties = true;
                }
            }

            _propertyInfoDict = propertyInfoDict;

            return properties;
        }

        private PropertyInfo[] GetKeyProperties()
        {
            if (CommonUtil.IsUnsupportedType(type))
            {
                throw new InvalidOperationException(Client.Strings.ClientType_UnsupportedType(type));
            }

            string typeName = type.ToString();
            IEnumerable<object> customAttributes = type.GetCustomAttributes(true);
            bool isEntity = customAttributes.OfType<EntityTypeAttribute>().Any();
            KeyAttribute dataServiceKeyAttribute = customAttributes.OfType<KeyAttribute>().FirstOrDefault();

            List<KeyValuePair<PropertyInfo, int>> keyWithOrders = new List<KeyValuePair<PropertyInfo, int>>();

            KeyKind currentKeyKind = KeyKind.NotKey;
            KeyKind newKeyKind = KeyKind.NotKey;
            foreach (PropertyInfo propertyInfo in Properties)
            {
                if ((newKeyKind = IsKeyProperty(propertyInfo, dataServiceKeyAttribute, out int order)) != KeyKind.NotKey)
                {
                    if (newKeyKind > currentKeyKind)
                    {
                        keyWithOrders.Clear();
                        currentKeyKind = newKeyKind;
                        InserKeyBasedOnOrder(keyWithOrders, propertyInfo, order);
                    }
                    else if (newKeyKind == currentKeyKind)
                    {
                        InserKeyBasedOnOrder(keyWithOrders, propertyInfo, order);
                    }
                }
            }

            List<PropertyInfo> keyProperties = new List<PropertyInfo>();
            foreach (var item in keyWithOrders)
            {
                keyProperties.Add(item.Key);
            }

            Type keyPropertyDeclaringType = null;
            foreach (PropertyInfo key in keyProperties)
            {
                if (keyPropertyDeclaringType == null)
                {
                    keyPropertyDeclaringType = key.DeclaringType;
                }
                else if (keyPropertyDeclaringType != key.DeclaringType)
                {
                    throw Client.Error.InvalidOperation(Client.Strings.ClientType_KeysOnDifferentDeclaredType(typeName));
                }

                // Check if the key property's type is a known primitive, an enum, or a nullable generic.
                // If it doesn't meet any of these conditions, throw an InvalidOperationException.
                if (!PrimitiveType.IsKnownType(key.PropertyType) && !key.PropertyType.IsEnum() && !(key.PropertyType.IsGenericType() && key.PropertyType.GetGenericTypeDefinition() == typeof(System.Nullable<>) && key.PropertyType.GetGenericArguments().First().IsEnum()))
                {
                    throw Client.Error.InvalidOperation(Client.Strings.ClientType_KeysMustBeSimpleTypes(key.Name, typeName, key.PropertyType.FullName));
                }
            }

            if (dataServiceKeyAttribute != null)
            {
                if (newKeyKind == KeyKind.AttributedKey && keyProperties.Count != dataServiceKeyAttribute?.KeyNames.Count)
                {
                    var m = (from string a in dataServiceKeyAttribute.KeyNames
                             where (from b in Properties
                                    where b.Name == a
                                    select b).FirstOrDefault() == null
                             select a).First<string>();
                    throw Client.Error.InvalidOperation(Client.Strings.ClientType_MissingProperty(typeName, m));
                }
            }

            return keyProperties.Count > 0 ? keyProperties.ToArray() : (isEntity ? ClientTypeUtil.EmptyPropertyInfoArray : null);
        }

        private static void InserKeyBasedOnOrder(List<KeyValuePair<PropertyInfo, int>> keys, PropertyInfo keyPi, int order)
        {
            var newKeyWithOrder = new KeyValuePair<PropertyInfo, int>(keyPi, order);
            if (order < 0)
            {
                // order < 0 means there's no order value setting, append this key at the end of list.
                keys.Add(newKeyWithOrder);
            }
            else
            {
                int index = 0;
                foreach (var key in keys)
                {
                    if (key.Value < 0 || key.Value > order)
                    {
                        // Insert the new key before the first negative order or first order bigger than new order.
                        // If the new order value is same as one item in the list , move to next.
                        break;
                    }

                    ++index;
                }

                keys.Insert(index, newKeyWithOrder);
            }
        }

        /// <summary>
        /// Returns the KeyKind if <paramref name="propertyInfo"/> is declared as a key in <paramref name="dataServiceKeyAttribute"/> or it follows the key naming convention.
        /// </summary>
        /// <param name="propertyInfo">Property in question.</param>
        /// <param name="dataServiceKeyAttribute">DataServiceKeyAttribute instance.</param>
        /// <returns>Returns the KeyKind if <paramref name="propertyInfo"/> is declared as a key in <paramref name="dataServiceKeyAttribute"/> or it follows the key naming convention.</returns>
        private static KeyKind IsKeyProperty(PropertyInfo propertyInfo, KeyAttribute dataServiceKeyAttribute, out int order)
        {
            Debug.Assert(propertyInfo != null, "propertyInfo != null");
            order = -1;

            //If the property's declaring type is anonymous, it is not a key.
            if (propertyInfo.IsAnonymousProperty())
            {
                return KeyKind.NotKey;
            }

            string propertyName = ClientTypeUtil.GetServerDefinedName(propertyInfo);

            KeyKind keyKind = KeyKind.NotKey;
            if (dataServiceKeyAttribute != null && dataServiceKeyAttribute.KeyNames.Contains(propertyName))
            {
                order = dataServiceKeyAttribute.KeyNames.IndexOf(propertyName);
                keyKind = KeyKind.AttributedKey;
            }
            else if (IsDataAnnotationsKeyProperty(propertyInfo, out KeyKind newKind, out int newOrder))
            {
                order = newOrder;
                keyKind = newKind;
            }
            else if (propertyName.Equals(propertyInfo.DeclaringType.Name + "Id", StringComparison.OrdinalIgnoreCase) || propertyName.Equals("Id", StringComparison.OrdinalIgnoreCase))
            {
                string declaringTypeName = propertyInfo.DeclaringType.Name;
                if ((propertyName.Length == (declaringTypeName.Length + 2)) && propertyName.StartsWith(declaringTypeName, StringComparison.OrdinalIgnoreCase))
                {
                    // matched "DeclaringType.Name+ID" pattern
                    keyKind = KeyKind.TypeNameId;
                }
                else if (propertyName.Length == 2)
                {
                    // matched "ID" pattern
                    keyKind = KeyKind.Id;
                }
            }

            return keyKind;
        }

        private static bool IsDataAnnotationsKeyProperty(PropertyInfo propertyInfo, out KeyKind kind, out int order)
        {
            order = -1;
            kind = KeyKind.NotKey;
            var attributes = propertyInfo.GetCustomAttributes();
            if (!attributes.Any(a => a is System.ComponentModel.DataAnnotations.KeyAttribute))
            {
                return false;
            }

            var columnAttribute = attributes.OfType<System.ComponentModel.DataAnnotations.Schema.ColumnAttribute>().FirstOrDefault();
            if (columnAttribute != null)
            {
                order = columnAttribute.Order;
            }

            kind = KeyKind.AttributedKey;

            return true;
        }
    }
}
