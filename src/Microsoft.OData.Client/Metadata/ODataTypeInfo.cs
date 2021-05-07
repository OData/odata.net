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
    using c = Microsoft.OData.Client;

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

        /// <summary>
        /// Creates and instance of <see cref="ODataTypeInfo"/>
        /// </summary>
        public ODataTypeInfo(Type type)
        {
            this.type = type;
            ClientDefinedNameDict = new ConcurrentDictionary<string, string>();
            _propertyInfoDict = new Dictionary<string, PropertyInfo>();            
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
        public string ServerDefinedTypeName { get; set; }

        /// <summary>
        /// Sertver defined type full name
        /// </summary>
        public string ServerDefinedTypeFullName { get; set; }

        /// <summary>
        /// Concurrent Dictionary cache to save ClientDefinedName for the type and serverDefinedName
        /// </summary>
        public ConcurrentDictionary<string, string> ClientDefinedNameDict { get; set; }

        /// <summary>
        /// Dictionary cache to save Propertyinfo for the type
        /// </summary>
        public Dictionary<string, PropertyInfo> PropertyInfoDict
        {
            get
            {
                if (_propertyInfoDict == null)
                {
                    _properties = GetAllProperties();
                }

                return _propertyInfoDict;
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

        private IEnumerable<PropertyInfo> GetAllProperties()
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();
            _hasProperties = false;

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
                string serverDefinedName = originalNameAttribute != null? originalNameAttribute.OriginalName: propertyInfo.Name;

                _propertyInfoDict[serverDefinedName] = propertyInfo;

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

            return properties;
        }

        private PropertyInfo[] GetKeyProperties()
        {           
            if (CommonUtil.IsUnsupportedType(type))
            {
                throw new InvalidOperationException(c.Strings.ClientType_UnsupportedType(type));
            }

            string typeName = type.ToString();
            IEnumerable<object> customAttributes = type.GetCustomAttributes(true);
            bool isEntity = customAttributes.OfType<EntityTypeAttribute>().Any();
            KeyAttribute dataServiceKeyAttribute = customAttributes.OfType<KeyAttribute>().FirstOrDefault();
            List<PropertyInfo> keyProperties = new List<PropertyInfo>();
            PropertyInfo[] properties = Properties.ToArray();

            KeyKind currentKeyKind = KeyKind.NotKey;
            KeyKind newKeyKind = KeyKind.NotKey;
            foreach (PropertyInfo propertyInfo in properties)
            {
                if ((newKeyKind = IsKeyProperty(propertyInfo, dataServiceKeyAttribute)) != KeyKind.NotKey)
                {
                    if (newKeyKind > currentKeyKind)
                    {
                        keyProperties.Clear();
                        currentKeyKind = newKeyKind;
                        keyProperties.Add(propertyInfo);
                    }
                    else if (newKeyKind == currentKeyKind)
                    {
                        keyProperties.Add(propertyInfo);
                    }
                }
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
                    throw c.Error.InvalidOperation(c.Strings.ClientType_KeysOnDifferentDeclaredType(typeName));
                }

                if (!PrimitiveType.IsKnownType(key.PropertyType) && !(key.PropertyType.GetGenericTypeDefinition() == typeof(System.Nullable<>) && key.PropertyType.GetGenericArguments().First().IsEnum()))
                {
                    throw c.Error.InvalidOperation(c.Strings.ClientType_KeysMustBeSimpleTypes(key.Name, typeName, key.PropertyType.FullName));
                }
            }

            if (dataServiceKeyAttribute != null)
            {
                if (newKeyKind == KeyKind.AttributedKey && keyProperties.Count != dataServiceKeyAttribute?.KeyNames.Count)
                {
                    var m = (from string a in dataServiceKeyAttribute.KeyNames
                                where (from b in properties
                                    where b.Name == a
                                    select b).FirstOrDefault() == null
                                select a).First<string>();
                    throw c.Error.InvalidOperation(c.Strings.ClientType_MissingProperty(typeName, m));
                }
            }

            return keyProperties.Count > 0 ? keyProperties.ToArray() : (isEntity ? ClientTypeUtil.EmptyPropertyInfoArray : null);
        }

        /// <summary>
        /// Returns the KeyKind if <paramref name="propertyInfo"/> is declared as a key in <paramref name="dataServiceKeyAttribute"/> or it follows the key naming convention.
        /// </summary>
        /// <param name="propertyInfo">Property in question.</param>
        /// <param name="dataServiceKeyAttribute">DataServiceKeyAttribute instance.</param>
        /// <returns>Returns the KeyKind if <paramref name="propertyInfo"/> is declared as a key in <paramref name="dataServiceKeyAttribute"/> or it follows the key naming convention.</returns>
        private static KeyKind IsKeyProperty(PropertyInfo propertyInfo, KeyAttribute dataServiceKeyAttribute)
        {
            Debug.Assert(propertyInfo != null, "propertyInfo != null");

            string propertyName = ClientTypeUtil.GetServerDefinedName(propertyInfo);

            KeyKind keyKind = KeyKind.NotKey;
            if (dataServiceKeyAttribute != null && dataServiceKeyAttribute.KeyNames.Contains(propertyName))
            {
                keyKind = KeyKind.AttributedKey;
            }
#if !PORTABLELIB
            else if (propertyInfo.GetCustomAttributes().OfType<System.ComponentModel.DataAnnotations.KeyAttribute>().Any())
            {
                keyKind = KeyKind.AttributedKey;
            }
#endif
            else if (propertyName.EndsWith("ID", StringComparison.Ordinal))
            {
                string declaringTypeName = propertyInfo.DeclaringType.Name;
                if ((propertyName.Length == (declaringTypeName.Length + 2)) && propertyName.StartsWith(declaringTypeName, StringComparison.Ordinal))
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

    }
}
