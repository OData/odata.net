//---------------------------------------------------------------------
// <copyright file="ClientTypeUtil.cs" company="Microsoft">
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
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Edm;
    using c = Microsoft.OData.Client;

    #endregion Namespaces.


    /// <summary>
    /// Enumeration for the kind of key
    /// </summary>
    internal enum KeyKind
    {
        /// <summary>If this is not a key </summary>
        NotKey = 0,

        /// <summary> If the key property name was equal to ID </summary>
        Id = 1,

        /// <summary> If the key property name was equal to TypeName+ID </summary>
        TypeNameId = 2,

        /// <summary> if the key property was attributed </summary>
        AttributedKey = 3,
    }

    /// <summary>
    /// Utility methods for client types.
    /// </summary>
    internal static class ClientTypeUtil
    {
        /// <summary>A static empty PropertyInfo array.</summary>
        internal static readonly PropertyInfo[] EmptyPropertyInfoArray = new PropertyInfo[0];

        internal static ConcurrentDictionary<Type, ODataTypeInfo> ODataTypeInfoCache { get; set; } = new ConcurrentDictionary<Type, ODataTypeInfo>();

        /// <summary>
        /// Sets the single instance of <see cref="ClientTypeAnnotation"/> on the given instance of <paramref name="edmType"/>.
        /// </summary>
        /// <param name="model">The model the <paramref name="edmType"/> belongs to.</param>
        /// <param name="edmType">IEdmType instance to set the annotation on.</param>
        /// <param name="annotation">The annotation to set</param>
        internal static void SetClientTypeAnnotation(this IEdmModel model, IEdmType edmType, ClientTypeAnnotation annotation)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(edmType != null, "edmType != null");
            Debug.Assert(annotation != null, "annotation != null");
            model.SetAnnotationValue<ClientTypeAnnotation>(edmType, annotation);
        }

        /// <summary>
        /// Gets the ClientTypeAnnotation for the given type.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="type">Type for which the annotation needs to be returned.</param>
        /// <returns>An instance of ClientTypeAnnotation containing metadata about the given type.</returns>
        internal static ClientTypeAnnotation GetClientTypeAnnotation(this ClientEdmModel model, Type type)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(type != null, "type != null");

            IEdmType edmType = model.GetOrCreateEdmType(type);
            return model.GetClientTypeAnnotation(edmType);
        }

        /// <summary>
        /// Gets the single instance of <see cref="ClientTypeAnnotation"/> from the given instance of <paramref name="edmType"/>.
        /// </summary>
        /// <param name="model">The model the <paramref name="edmType"/> belongs to.</param>
        /// <param name="edmType">IEdmType instance to get the annotation.</param>
        /// <returns>Returns the single instance of <see cref="ClientTypeAnnotation"/> from the given instance of <paramref name="edmType"/>.</returns>
        internal static ClientTypeAnnotation GetClientTypeAnnotation(this IEdmModel model, IEdmType edmType)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(edmType != null, "edmType != null");

            return model.GetAnnotationValue<ClientTypeAnnotation>(edmType);
        }

        /// <summary>
        /// Sets the given instance of <paramref name="annotation"/> to the given instance of <paramref name="edmProperty"/>.
        /// </summary>
        /// <param name="edmProperty">IEdmProperty instance to set the annotation.</param>
        /// <param name="annotation">Annotation instance to set.</param>
        internal static void SetClientPropertyAnnotation(this IEdmProperty edmProperty, ClientPropertyAnnotation annotation)
        {
            Debug.Assert(edmProperty != null, "edmProperty != null");
            Debug.Assert(annotation != null, "annotation != null");
            annotation.Model.SetAnnotationValue<ClientPropertyAnnotation>(edmProperty, annotation);
        }

        /// <summary>
        /// Gets the single instance of ClientPropertyAnnotation from the given instance of <paramref name="edmProperty"/>.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="edmProperty">IEdmProperty instance to get the annotation.</param>
        /// <returns>Returns the single instance of ClientPropertyAnnotation from the given instance of <paramref name="edmProperty"/>.</returns>
        internal static ClientPropertyAnnotation GetClientPropertyAnnotation(this IEdmModel model, IEdmProperty edmProperty)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(edmProperty != null, "edmProperty != null");

            return model.GetAnnotationValue<ClientPropertyAnnotation>(edmProperty);
        }

        /// <summary>
        /// Gets the instance of ClientTypeAnnotation from the given instance of <paramref name="edmProperty"/>.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="edmProperty">IEdmProperty instance to get the annotation.</param>
        /// <returns>Returns the instance of ClientTypeAnnotation from the given instance of <paramref name="edmProperty"/>.</returns>
        internal static ClientTypeAnnotation GetClientTypeAnnotation(this IEdmModel model, IEdmProperty edmProperty)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(edmProperty != null, "edmProperty != null");

            IEdmType edmType = edmProperty.Type.Definition;
            Debug.Assert(edmType != null, "edmType != null");

            return model.GetAnnotationValue<ClientTypeAnnotation>(edmType);
        }

        /// <summary>
        /// Returns the corresponding edm type reference for the given edm type.
        /// </summary>
        /// <param name="edmType">EdmType instance.</param>
        /// <param name="isNullable">A boolean value indicating whether the clr type of this edm type is nullable</param>
        /// <returns>Returns the corresponding edm type reference for the given edm type.</returns>
        internal static IEdmTypeReference ToEdmTypeReference(this IEdmType edmType, bool isNullable)
        {
            return EdmLibraryExtensions.ToTypeReference(edmType, isNullable);
        }

        /// <summary>
        /// Returns the full name for the given edm type
        /// </summary>
        /// <param name="edmType">EdmType instance.</param>
        /// <returns>the full name of the edmType.</returns>
        internal static string FullName(this IEdmType edmType)
        {
            IEdmSchemaElement schemaElement = edmType as IEdmSchemaElement;
            if (schemaElement != null)
            {
                return schemaElement.FullName();
            }

            return null;
        }

        /// <summary>
        /// Returns MethodInfo instance for a generic type retrieved by using <paramref name="methodName"/> and gets
        /// element type for the provided <paramref name="genericTypeDefinition"/>.
        /// </summary>
        /// <param name="propertyType">starting type</param>
        /// <param name="genericTypeDefinition">the generic type definition to find</param>
        /// <param name="methodName">the method to search for</param>
        /// <param name="type">the element type for <paramref name="genericTypeDefinition" /> if found</param>
        /// <returns>element types</returns>
        internal static MethodInfo GetMethodForGenericType(Type propertyType, Type genericTypeDefinition, string methodName, out Type type)
        {
            Debug.Assert(propertyType != null, "null propertyType");
            Debug.Assert(genericTypeDefinition != null, "null genericTypeDefinition");
            Debug.Assert(genericTypeDefinition.IsGenericTypeDefinition(), "!IsGenericTypeDefinition");

            type = null;

            Type implementationType = ClientTypeUtil.GetImplementationType(propertyType, genericTypeDefinition);
            if (implementationType != null)
            {
                Type[] genericArguments = implementationType.GetGenericArguments();
                MethodInfo methodInfo = implementationType.GetMethod(methodName);
                Debug.Assert(methodInfo != null, "should have found the method");

#if DEBUG
                Debug.Assert(genericArguments != null, "null genericArguments");
                ParameterInfo[] parameters = methodInfo.GetParameters();
                if (parameters.Length > 0)
                {
                    // following assert was disabled for Contains which returns bool
                    //// Debug.Assert(typeof(void) == methodInfo.ReturnParameter.ParameterType, "method doesn't return void");

                    Debug.Assert(genericArguments.Length == parameters.Length, "genericArguments don't match parameters");
                    for (int i = 0; i < genericArguments.Length; ++i)
                    {
                        Debug.Assert(genericArguments[i] == parameters[i].ParameterType, "parameter doesn't match generic argument");
                    }
                }
#endif
                type = genericArguments[genericArguments.Length - 1];
                return methodInfo;
            }

            return null;
        }

        /// <summary>Gets a delegate that can be invoked to add an item to a collection of the specified type.</summary>
        /// <param name='listType'>Type of list to use.</param>
        /// <returns>The delegate to invoke.</returns>
        internal static Action<object, object> GetAddToCollectionDelegate(Type listType)
        {
            Debug.Assert(listType != null, "listType != null");

            Type listElementType;
            MethodInfo addMethod = ClientTypeUtil.GetAddToCollectionMethod(listType, out listElementType);
            ParameterExpression list = Expression.Parameter(typeof(object), "list");
            ParameterExpression item = Expression.Parameter(typeof(object), "element");
            Expression body = Expression.Call(Expression.Convert(list, listType), addMethod, Expression.Convert(item, listElementType));
            LambdaExpression lambda = Expression.Lambda(body, list, item);

            return (Action<object, object>)lambda.Compile();
        }

        /// <summary>
        /// Gets the Add method to add items to a collection of the specified type.
        /// </summary>
        /// <param name="collectionType">Type for the collection.</param>
        /// <param name="type">The element type in the collection if found; null otherwise.</param>
        /// <returns>The method to invoke to add to a collection of the specified type.</returns>
        internal static MethodInfo GetAddToCollectionMethod(Type collectionType, out Type type)
        {
            return ClientTypeUtil.GetMethodForGenericType(collectionType, typeof(ICollection<>), "Add", out type);
        }

        /// <summary>
        /// get concrete type that implements the genericTypeDefinition
        /// </summary>
        /// <param name="type">starting type</param>
        /// <param name="genericTypeDefinition">the generic type definition to find</param>
        /// <returns>concrete type that implements the generic type</returns>
        internal static Type GetImplementationType(Type type, Type genericTypeDefinition)
        {
            if (IsConstructedGeneric(type, genericTypeDefinition))
            {   // propertyType is genericTypeDefinition (e.g. ICollection<T>)
                return type;
            }
            else
            {
                Type implementationType = null;
                foreach (Type interfaceType in type.GetInterfaces())
                {
                    if (IsConstructedGeneric(interfaceType, genericTypeDefinition))
                    {
                        if (implementationType == null)
                        {   // found implementation of genericTypeDefinition (e.g. ICollection<T>)
                            implementationType = interfaceType;
                        }
                        else
                        {   // Multiple implementations (e.g. ICollection<int> and ICollection<int?>)
                            throw c.Error.NotSupported(c.Strings.ClientType_MultipleImplementationNotSupported);
                        }
                    }
                }

                return implementationType;
            }
        }

        /// <summary>
        /// Is the type an Entity Type?
        /// </summary>
        /// <param name="t">Type to examine</param>
        /// <param name="model">The client model.</param>
        /// <returns>bool indicating whether or not entity type</returns>
        internal static bool TypeIsEntity(Type t, ClientEdmModel model)
        {
            return model.GetOrCreateEdmType(t).TypeKind == EdmTypeKind.Entity;
        }

        /// <summary>
        /// Is the type a Complex Type?
        /// </summary>
        /// <param name="t">Type to examine</param>
        /// <param name="model">The client model.</param>
        /// <returns>bool indicating whether or not complex type</returns>
        internal static bool TypeIsComplex(Type t, ClientEdmModel model)
        {
            return model.GetOrCreateEdmType(t).TypeKind == EdmTypeKind.Complex;
        }

        /// <summary>
        /// Is the type an structured type?
        /// </summary>
        /// <param name="t">Type to examine</param>
        /// <param name="model">The client model.</param>
        /// <returns>bool indicating whether or not structured type</returns>
        internal static bool TypeIsStructured(Type t, ClientEdmModel model)
        {
            var typeKind = model.GetOrCreateEdmType(t).TypeKind;
            return typeKind == EdmTypeKind.Entity || typeKind == EdmTypeKind.Complex;
        }

        /// <summary>
        /// Is the type or element type (in the case of nullableOfT or IEnumOfT) a Entity Type?
        /// </summary>
        /// <param name="type">Type to examine</param>
        /// <returns>bool indicating whether or not entity type</returns>
        internal static bool TypeOrElementTypeIsEntity(Type type)
        {
            type = TypeSystem.GetElementType(type);
            type = Nullable.GetUnderlyingType(type) ?? type;
            return !PrimitiveType.IsKnownType(type) && ClientTypeUtil.GetKeyPropertiesOnType(type) != null;
        }

        /// <summary>
        /// Is the type or element type (in the case of nullableOfT or IEnumOfT) a structured type?
        /// </summary>
        /// <param name="type">Type to examine</param>
        /// <returns>bool indicating whether or not structured type</returns>
        internal static bool TypeOrElementTypeIsStructured(Type type)
        {
            type = TypeSystem.GetElementType(type);
            type = Nullable.GetUnderlyingType(type) ?? type;
            return !PrimitiveType.IsKnownType(type) && !type.IsEnum();
        }

        /// <summary>Checks whether the specified type is a DataServiceCollection type (or inherits from one).</summary>
        /// <param name='type'>Type to check.</param>
        /// <returns>true if the type inherits from DataServiceCollection; false otherwise.</returns>
        internal static bool IsDataServiceCollection(Type type)
        {
            while (type != null)
            {
                if (c.PlatformHelper.IsGenericType(type) && WebUtil.IsDataServiceCollectionType(type.GetGenericTypeDefinition()))
                {
                    return true;
                }

                type = c.PlatformHelper.GetBaseType(type);
            }

            return false;
        }

        /// <summary>Whether a variable of <paramref name="type"/> can be assigned null.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>true if a variable of type <paramref name="type"/> can be assigned null; false otherwise.</returns>
        internal static bool CanAssignNull(Type type)
        {
            Debug.Assert(type != null, "type != null");
            return !type.IsValueType() || (type.IsGenericType() && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        /// <summary>Returns the list of properties defined on <paramref name="type"/>.</summary>
        /// <param name="type">Type instance in question.</param>
        /// <param name="declaredOnly">True to to get the properties declared on <paramref name="type"/>; false to get all properties defined on <paramref name="type"/>.</param>
        /// <returns>Returns the list of properties defined on <paramref name="type"/>.</returns>
        internal static IEnumerable<PropertyInfo> GetPropertiesOnType(Type type, bool declaredOnly)
        {
            if (!PrimitiveType.IsKnownType(type))
            {
                ODataTypeInfo typeInfo = GetODataTypeInfo(type);
                foreach (PropertyInfo propertyInfo in typeInfo.Properties)
                {
                    if (declaredOnly && IsOverride(type, propertyInfo))
                    {
                        continue;
                    }

                    yield return propertyInfo;
                }
            }
        }

        /// <summary>
        /// Returns the list of key properties defined on <paramref name="type"/>; null if <paramref name="type"/> is complex.
        /// </summary>
        /// <param name="type">Type in question.</param>
        /// <returns>Returns the list of key properties defined on <paramref name="type"/>; null if <paramref name="type"/> is complex.</returns>
        internal static PropertyInfo[] GetKeyPropertiesOnType(Type type)
        {
            bool hasProperties;
            return GetKeyPropertiesOnType(type, out hasProperties);
        }

        /// <summary>
        /// Returns the list of key properties defined on <paramref name="type"/>; null if <paramref name="type"/> is complex.
        /// </summary>
        /// <param name="type">Type in question.</param>
        /// <param name="hasProperties">true if <paramref name="type"/> has any (declared or inherited) properties; otherwise false.</param>
        /// <returns>Returns the list of key properties defined on <paramref name="type"/>; null if <paramref name="type"/> is complex.</returns>
        internal static PropertyInfo[] GetKeyPropertiesOnType(Type type, out bool hasProperties)
        {
            ODataTypeInfo typeInfo = GetODataTypeInfo(type);

            hasProperties = typeInfo.HasProperties;
            return typeInfo.KeyProperties;
        }

        /// <summary>Gets the type of the specified <paramref name="member"/>.</summary>
        /// <param name="member">Member to get type of (typically PropertyInfo or FieldInfo).</param>
        /// <returns>The type of property or field type.</returns>
        internal static Type GetMemberType(MemberInfo member)
        {
            Debug.Assert(member != null, "member != null");

            PropertyInfo propertyInfo = member as PropertyInfo;
            if (propertyInfo != null)
            {
                return propertyInfo.PropertyType;
            }

            FieldInfo fieldInfo = member as FieldInfo;
            Debug.Assert(fieldInfo != null, "fieldInfo != null -- otherwise Expression.Member factory should have thrown an argument exception");
            return fieldInfo.FieldType;
        }

        /// <summary>Gets the server defined name in <see cref="OriginalNameAttribute"/> of the specified <paramref name="propertyInfo"/>.</summary>
        /// <param name="propertyInfo">Member to get server defined name of.</param>
        /// <returns>Server defined name.</returns>
        internal static string GetServerDefinedName(PropertyInfo propertyInfo)
        {
            OriginalNameAttribute originalNameAttribute = (OriginalNameAttribute)propertyInfo.GetCustomAttributes(typeof(OriginalNameAttribute), false).SingleOrDefault();
            if (originalNameAttribute != null)
            {
                return originalNameAttribute.OriginalName;
            }

            return propertyInfo.Name;
        }

        /// <summary>Gets the server defined name in <see cref="OriginalNameAttribute"/> of the specified <paramref name="memberInfo"/>.</summary>
        /// <param name="memberInfo">Member to get server defined name of.</param>
        /// <returns>The server defined name.</returns>
        internal static string GetServerDefinedName(MemberInfo memberInfo)
        {
            OriginalNameAttribute originalNameAttribute = (OriginalNameAttribute)memberInfo.GetCustomAttributes(typeof(OriginalNameAttribute), false).SingleOrDefault();
            if (originalNameAttribute != null)
            {
                return originalNameAttribute.OriginalName;
            }

            return memberInfo.Name;
        }

        /// <summary>Gets the server defined type name in <see cref="OriginalNameAttribute"/> of the specified <paramref name="type"/>.</summary>
        /// <param name="type">Member to get server defined type name of.</param>
        /// <returns>The server defined type name.</returns>
        internal static string GetServerDefinedTypeName(Type type)
        {
            ODataTypeInfo typeInfo = GetODataTypeInfo(type);           

            return typeInfo.ServerDefinedTypeName;
        }

        /// <summary>Gets the full server defined type name in <see cref="OriginalNameAttribute"/> of the specified <paramref name="type"/>.</summary>
        /// <param name="type">Member to get server defined name of.</param>
        /// <returns>The server defined type full name.</returns>
        internal static string GetServerDefinedTypeFullName(Type type)
        {
            ODataTypeInfo typeInfo = GetODataTypeInfo(type);

            return typeInfo.ServerDefinedTypeFullName;
        }

        /// <summary>
        /// Gets the clr name according to server defined name in the specified <paramref name="t"/>.
        /// </summary>
        /// <param name="t">Member to get clr name for.</param>
        /// <param name="serverDefinedName">name from server.</param>
        /// <returns>Client clr name.</returns>
        internal static string GetClientFieldName(Type t, string serverDefinedName)
        {
            ODataTypeInfo typeInfo = GetODataTypeInfo(t);
                        
            List<string> serverDefinedNames = serverDefinedName.Split(',').Select(name => name.Trim()).ToList();
            List<string> clientMemberNames = new List<string>();
            foreach (var serverSideName in serverDefinedNames)
            {
                string memberInfoName = typeInfo.GetClientFieldName(serverSideName);

                clientMemberNames.Add(memberInfoName);
            }

            return string.Join(",", clientMemberNames);
        }

        /// <summary>
        /// Gets the clr name according to server defined name in the specified <paramref name="t"/>.
        /// </summary>
        /// <param name="t">The type used to get the client PropertyInfo.</param>
        /// <param name="serverDefinedName">Name from server.</param>
        /// <param name="undeclaredPropertyBehavior">Flag to support untyped properties.</param>
        /// <returns>Client PropertyInfo, or null if the method is not found or throws exception if undeclaredPropertyBehavior is ThrowException.</returns>
        internal static PropertyInfo GetClientPropertyInfo(Type t, string serverDefinedName, UndeclaredPropertyBehavior undeclaredPropertyBehavior)
        {
            ODataTypeInfo typeInfo = GetODataTypeInfo(t);
            return typeInfo.GetClientPropertyInfo(serverDefinedName, undeclaredPropertyBehavior);
        }

        /// <summary>
        /// Gets the clr name according to server defined name in the specified <paramref name="t"/>.
        /// </summary>
        /// <param name="t">The type used to get the client property name.</param>
        /// <param name="serverDefinedName">Name from server.</param>
        /// <param name="undeclaredPropertyBehavior">Flag to support untyped properties.</param>
        /// <returns>Client property name, or null if the property is not found.</returns>
        internal static string GetClientPropertyName(Type t, string serverDefinedName, UndeclaredPropertyBehavior undeclaredPropertyBehavior)
        {
            PropertyInfo propertyInfo = GetClientPropertyInfo(t, serverDefinedName, undeclaredPropertyBehavior);
            return propertyInfo == null ? serverDefinedName : propertyInfo.Name;
        }

        /// <summary>
        /// Get a client method with the specified server side name.
        /// </summary>
        /// <param name="t">Client type used to search method.</param>
        /// <param name="serverDefinedName">Method name on server side.</param>
        /// <param name="parameters">An array of System.Type objects of the parameters for the method to get</param>
        /// <returns>Client MethodInfo, or null if the method is not found.</returns>
        internal static MethodInfo GetClientMethod(Type t, string serverDefinedName, Type[] parameters)
        {
            MethodInfo methodInfo = t.GetMethod(serverDefinedName, parameters);
            if (methodInfo == null)
            {
                var clientNamedMethodInfo = t.GetMethods().Where(
                    m =>
                    {
                        OriginalNameAttribute originalNameAttribute = (OriginalNameAttribute)m.GetCustomAttributes(typeof(OriginalNameAttribute), false).SingleOrDefault();
                        return originalNameAttribute != null && originalNameAttribute.OriginalName == serverDefinedName;
                    }).FirstOrDefault();

                if (clientNamedMethodInfo != null)
                {
                    methodInfo = t.GetMethod(clientNamedMethodInfo.Name, parameters);
                }
            }

            return methodInfo;
        }

        /// <summary>
        /// Get the enum string split by "," with their server side names
        /// </summary>
        /// <param name="enumString">enum string with names in client side</param>
        /// <param name="enumType">the source enum type</param>
        /// <returns>The enum string split by "," with their server side names</returns>
        internal static string GetEnumValuesString(string enumString, Type enumType)
        {
            string[] enums = enumString.Split(',').Select(v => v.Trim()).ToArray();
            List<string> memberValues = new List<string>();
            foreach (var enumValue in enums)
            {
                MemberInfo member = enumType.GetField(enumValue);
                if (member == null)
                {
                    throw new NotSupportedException(Strings.Serializer_InvalidEnumMemberValue(enumType.Name, enumValue));
                }

                memberValues.Add(ClientTypeUtil.GetServerDefinedName(member));
            }

            return string.Join(",", memberValues);
        }

        /// <summary>
        /// Returns true if the <paramref name="instance"/> is an instance of an open type in the <paramref name="model"/>.
        /// </summary>
        /// <param name="instance">The instance to examine</param>
        /// <param name="model">The client model.</param>
        /// <returns>Returns true if the <paramref name="instance"/> is an instance of an open type in the <paramref name="model"/></returns>
        internal static bool IsInstanceOfOpenType(object instance, ClientEdmModel model)
        {
            Debug.Assert(instance != null, "instance !=null");
            Debug.Assert(model != null, "model !=null");

            Type clientType = instance.GetType();
            ClientTypeAnnotation clientTypeAnnotation = model.GetClientTypeAnnotation(clientType);

            Debug.Assert(clientTypeAnnotation != null, "clientTypeAnnotation != null");

            return clientTypeAnnotation.EdmType.IsOpen();
        }

        /// <summary>
        /// Returns true if the <paramref name="instance"/> contains a non-null dictionary property of string and object
        /// The dictionary should also not be decorated with IgnoreClientPropertyAttribute
        /// </summary>
        /// <param name="instance">Object with expected container property</param>
        /// <param name="containerProperty">Reference to the container property</param>
        /// <returns>true if expected container property is found</returns>
        internal static bool TryGetContainerProperty(object instance, out IDictionary<string, object> containerProperty)
        {
            Debug.Assert(instance != null, "instance != null");

            containerProperty = default(IDictionary<string, object>);

            PropertyInfo propertyInfo = instance.GetType().GetPublicProperties(true /* instanceOnly */).Where(p =>
                                p.GetCustomAttributes(typeof(ContainerPropertyAttribute), true).Any() &&
                                typeof(IDictionary<string, object>).IsAssignableFrom(p.PropertyType)).FirstOrDefault();

            if (propertyInfo == null)
            {
                return false;
            }

            containerProperty = (IDictionary<string, object>)propertyInfo.GetValue(instance);

            // Is property initialized?
            if (containerProperty == null)
            {
                Type propertyType = propertyInfo.PropertyType;
                
                // Handle Dictionary<,> , SortedDictionary<,> , ConcurrentDictionary<,> , etc - must also have parameterless constructor
                if (!propertyType.IsInterface() && !propertyType.IsAbstract() && propertyType.GetInstanceConstructor(true, new Type[0]) != null)
                {
                    containerProperty = (IDictionary<string, object>)Util.ActivatorCreateInstance(propertyType);
                }
                else if (propertyType.Equals(typeof(IDictionary<string, object>)))
                {
                    // Default to Dictionary<,> for IDictionary<,> property
                    Type dictionaryType = typeof(Dictionary<,>).MakeGenericType(new Type[] { typeof(string), typeof(object) });
                    containerProperty = (IDictionary<string, object>)Util.ActivatorCreateInstance(dictionaryType);
                }
                else
                { 
                    // Not easy to figure out the implementing type
                    return false;
                }

                propertyInfo.SetValue(instance, containerProperty);

                return true;
            }

            return true;
        }

        private static ODataTypeInfo GetODataTypeInfo(Type type)
        {
            return ODataTypeInfoCache.GetOrAdd(type, (key) => new ODataTypeInfo(type));
        }

        /// <summary>
        /// Checks whether the specified <paramref name="type"/> is a
        /// closed constructed type of the generic type.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <param name="genericTypeDefinition">Generic type for checking.</param>
        /// <returns>true if <paramref name="type"/> is a constructed type of <paramref name="genericTypeDefinition"/>.</returns>
        /// <remarks>The check is an immediate check; no inheritance rules are applied.</remarks>
        private static bool IsConstructedGeneric(Type type, Type genericTypeDefinition)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(genericTypeDefinition != null, "genericTypeDefinition != null");

            return type.IsGenericType() && (type.GetGenericTypeDefinition() == genericTypeDefinition) && !type.ContainsGenericParameters();
        }

        /// <summary>
        /// Determines whether the <paramref name="propertyInfo"/> declared on <paramref name="type"/>
        /// overrides a (virtual/abstract) property of a base type.
        /// </summary>
        /// <param name="type">The declaring type of the property.</param>
        /// <param name="propertyInfo">The property to check.</param>
        /// <returns>true if <paramref name="propertyInfo"/> overrides a property on a base types; otherwise false.</returns>
        private static bool IsOverride(Type type, PropertyInfo propertyInfo)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(propertyInfo != null, "propertyInfo != null");

            // We only check the getter method; if a property does not have a getter method we don't consider it
            MethodInfo getMethod = propertyInfo.GetGetMethod();
            if (getMethod != null && getMethod.GetBaseDefinition().DeclaringType != type)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to resolve a type with specified name from the loaded assemblies.
        /// </summary>
        /// <param name="typeName">Name of the type to resolve.</param>
        /// <param name="fullNamespace">Namespace of the type.</param>
        /// <param name="languageDependentNamespace">Namespace that the resolved type is expected to be.
        /// Usually same as <paramref name="fullNamespace"/> but can be different
        /// where namespace for client types does not match namespace in service types.</param>
        /// <param name="matchedType">The resolved type.</param>
        /// <returns>true if type was successfully resolved; otherwise false.</returns>
        internal static bool TryResolveType(string typeName, string fullNamespace, string languageDependentNamespace, out Type matchedType)
        {
            Debug.Assert(typeName != null, "typeName != null");

            matchedType = null;
            int namespaceLength = fullNamespace?.Length ?? 0;
            string serverDefinedName = typeName.Substring(namespaceLength + 1);

            // Searching only loaded assemblies, not referenced assemblies
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                matchedType = assembly.GetType(string.Concat(languageDependentNamespace, typeName.Substring(namespaceLength)), false);
                if (matchedType != null)
                {
                    return true;
                }

                IEnumerable<Type> types = null;

                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException)
                {
                    // Ignore
                }

                if (types != null)
                {
                    foreach (Type type in types)
                    {
                        OriginalNameAttribute originalNameAttribute = (OriginalNameAttribute)type.GetCustomAttributes(typeof(OriginalNameAttribute), true).SingleOrDefault();
                        if (string.Equals(originalNameAttribute?.OriginalName, serverDefinedName, StringComparison.Ordinal)
                            && type.Namespace.Equals(languageDependentNamespace, StringComparison.Ordinal))
                        {
                            matchedType = type;
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
