//---------------------------------------------------------------------
// <copyright file="PlatformHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if TAUPO_CORE
namespace Microsoft.Test.Taupo
#else
#if TAUPO_TESTSHELL
namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
#else
#if TAUPO_QUERY
namespace Microsoft.Test.Taupo.Query
#else
#if TAUPO_ASTORIA
namespace Microsoft.Test.Taupo.Astoria
#else
#if TAUPO_SPATIAL
namespace Microsoft.Test.Taupo.Spatial
#else
#if CODEDOM
namespace System.CodeDom
#else
#if ASTORIATESTS_SHARED
namespace Microsoft.Test.Taupo.Astoria.Shared.Tests
#else 
#if ASTORIATESTS_CLIENT
namespace Microsoft.Test.Taupo.Astoria.Client.Tests
#else
#if METRORUNNERBASE
namespace Microsoft.Test.TaupoMetroRunnerBase
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
#if TAUPO_CORE || TAUPO_ASTORIA
    using System.Resources;
    using System.Xml;
#endif

    /// <summary>
    /// Helper methods that provide a common API surface on all platforms.
    /// </summary>
    internal static class PlatformHelper
    {
        /// <summary>
        /// Use this instead of Type.EmptyTypes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static readonly Type[] EmptyTypes = new Type[0];

#if TAUPO_ASTORIA
        /// <summary>
        /// Use this instead of Uri.UriSchemeHttp.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static readonly string UriSchemeHttp = Uri.UriSchemeHttp;

        /// <summary>
        /// Use this instead of Uri.UriSchemeHttps.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static readonly string UriSchemeHttps = Uri.UriSchemeHttps;
#endif

        /// <summary>
        /// Replacement for Type.Assembly.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static Assembly GetAssembly(this Type type)
        {
            return type.Assembly;
        }

        /// <summary>
        /// Replacement for Type.IsValueType.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsValueType(this Type type)
        {
            return type.IsValueType;
        }

        /// <summary>
        /// Replacement for Type.IsGenericParameter.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsGenericParameter(this Type type)
        {
            return type.IsGenericParameter;
        }

        /// <summary>
        /// Replacement for Type.IsAbstract.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsAbstract(this Type type)
        {
            return type.IsAbstract;
        }

        /// <summary>
        /// Replacement for Type.IsGenericType.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsGenericType(this Type type)
        {
            return type.IsGenericType;
        }

        /// <summary>
        /// Replacement for Type.IsGenericTypeDefinition.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsGenericTypeDefinition(this Type type)
        {
            return type.IsGenericTypeDefinition;
        }

        /// <summary>
        /// Replacement for Type.IsVisible.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsVisible(this Type type)
        {
            return type.IsVisible;
        }

        /// <summary>
        /// Replacement for Type.IsInterface.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsInterface(this Type type)
        {

            return type.IsInterface;
        }

        /// <summary>
        /// Replacement for Type.IsClass.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsClass(this Type type)
        {
            return type.IsClass;
        }

        /// <summary>
        /// Replacement for Type.IsEnum.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsEnum(this Type type)
        {
            return type.IsEnum;
        }

        /// <summary>
        /// Replacement for Type.IsPrimitive.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsPrimitive(this Type type)
        {
            return type.IsPrimitive;
        }

        /// <summary>
        /// Replacement for Type.IsPublic.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsPublic(this Type type)
        {
            return type.IsPublic;
        }

        /// <summary>
        /// Replacement for Type.IsDefined.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="attributeType">Attribute type to check.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsDefined(this Type type, Type attributeType)
        {
            return Attribute.IsDefined(type, attributeType);
        }

        /// <summary>
        /// Replacement for Type.IsDefined.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="attributeType">Attribute type to check.</param>
        /// <param name="inherit">Check for inherited attributes.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsDefined(this Type type, Type attributeType, bool inherit)
        {
            return Attribute.IsDefined(type, attributeType, inherit);
        }

        /// <summary>
        /// Replacement for PropertyInfo.IsDefined.
        /// </summary>
        /// <param name="propertyInfo">PropertyInfo on which to call this helper method.</param>
        /// <param name="attributeType">Attribute type to check.</param>
        /// <param name="inherit">Check for inherited attributes.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsDefined(this PropertyInfo propertyInfo, Type attributeType, bool inherit)
        {
            return propertyInfo.IsDefined(attributeType, inherit);
        }

        /// <summary>
        /// Replacement for Type.BaseType.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static Type GetBaseType(this Type type)
        {
            return type.BaseType;
        }

        /// <summary>
        /// Replacement for Array.AsReadOnly(T[]).
        /// </summary>
        /// <typeparam name="T">Type of items in the array.</typeparam>
        /// <param name="array">Array to use to create the ReadOnlyCollection.</param>
        /// <returns>ReadOnlyCollection containing the specified array items.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static ReadOnlyCollection<T> AsReadOnly<T>(this T[] array)
        {
            return Array.AsReadOnly(array);
        }

#if TAUPO_ASTORIA || TAUPO_CORE
        /// <summary>
        /// Converts a string to a DateTime.
        /// </summary>
        /// <param name="text">String to be converted.</param>
        /// <returns>See documentation for method being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Taupo.Platforms", "TP0001:PlatformApiDifferencesRule", Justification = "This is the helper method that must be used")]
        internal static DateTime ConvertStringToDateTime(string text)
        {
            return XmlConvert.ToDateTime(text, XmlDateTimeSerializationMode.RoundtripKind);
        }

        /// <summary>
        /// Converts a DateTime to a string.
        /// </summary>
        /// <param name="dateTime">DateTime to be converted.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Taupo.Platforms", "TP0001:PlatformApiDifferencesRule", Justification = "This is the helper method that must be used")]
        internal static string ConvertDateTimeToString(DateTime dateTime)
        {
            return XmlConvert.ToString(dateTime, XmlDateTimeSerializationMode.RoundtripKind);
        }
#endif

        /// <summary>
        /// Gets the specified type.
        /// </summary>
        /// <param name="typeName">Name of the type to get.</param>
        /// <exception cref="TypeLoadException">Throws if the type could not be found.</exception>
        /// <returns>Type instance that represents the specified type name.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static Type GetTypeOrThrow(string typeName)
        {
            return Type.GetType(typeName, true);
        }

        /// <summary>
        /// Replacement for usage of MemberInfo.MemberType property.
        /// </summary>
        /// <param name="member">MemberInfo on which to access this method.</param>
        /// <returns>True if the specified member is a property, otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsProperty(MemberInfo member)
        {
            return member.MemberType == MemberTypes.Property;
        }

        /// <summary>
        /// Replacement for usage of MemberInfo.MemberType property.
        /// </summary>
        /// <param name="member">MemberInfo on which to access this method.</param>
        /// <returns>True if the specified member is a method, otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsMethod(MemberInfo member)
        {
            return member.MemberType == MemberTypes.Method;
        }

        /// <summary>
        /// Gets instance properties for the specified type.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>Enumerable of instance properties for the type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static IEnumerable<PropertyInfo> GetAllInstanceProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Gets public properties for the specified type.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="instanceOnly">True if method should return only instance properties, false if it should return both instance and static properties.</param>
        /// <returns>Enumerable of public properties for the type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static IEnumerable<PropertyInfo> GetPublicProperties(this Type type, bool instanceOnly)
        {
            return GetPublicProperties(type, instanceOnly, false /*declaredOnly*/);
        }

        /// <summary>
        /// Gets public properties for the specified type.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="instanceOnly">True if method should return only instance properties, false if it should return both instance and static properties.</param>
        /// <param name="declaredOnly">True if method should return only properties that are declared on the type, false if it should return properties declared on the type as well as those inherited from any base types.</param>
        /// <returns>Enumerable of public properties for the type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static IEnumerable<PropertyInfo> GetPublicProperties(this Type type, bool instanceOnly, bool declaredOnly)
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
            if (!instanceOnly)
            {
                bindingFlags |= BindingFlags.Static;
            }

            if (declaredOnly)
            {
                bindingFlags |= BindingFlags.DeclaredOnly;
            }

            return type.GetProperties(bindingFlags);
        }

        /// <summary>
        /// Gets instance constructors for the specified type.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="isPublic">True if method should return only public constructors, false if it should return only non-public constructors.</param>
        /// <returns>Enumerable of instance constructors for the specified type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static IEnumerable<ConstructorInfo> GetInstanceConstructors(this Type type, bool isPublic)
        {
            BindingFlags bindingFlags = BindingFlags.Instance;
            bindingFlags |= isPublic ? BindingFlags.Public : BindingFlags.NonPublic;
            return type.GetConstructors(bindingFlags);
        }

        /// <summary>
        /// Gets a instance constructor for the type that takes the specified argument types.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="isPublic">True if method should search only public constructors, false if it should search only non-public constructors.</param>
        /// <param name="argTypes">Array of argument types for the constructor.</param>
        /// <returns>ConstructorInfo for the constructor with the specified characteristics if found, otherwise null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static ConstructorInfo GetInstanceConstructor(this Type type, bool isPublic, Type[] argTypes)
        {
            BindingFlags bindingFlags = BindingFlags.Instance;
            bindingFlags |= isPublic ? BindingFlags.Public : BindingFlags.NonPublic;
            return type.GetConstructor(bindingFlags, null, argTypes, null);
        }

        /// <summary>
        /// Gets a method on the specified type.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="name">Name of the method on the type.</param>
        /// <param name="isPublic">True if method should search only public methods, false if it should search only non-public methods, and null for both.</param>
        /// <param name="isStatic">True if method should search only static methods, false if it should search only instance methods, and null for both.</param>
        /// <returns>MethodInfo for the method with the specified characteristics if found, otherwise null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static MethodInfo GetMethod(this Type type, string name, bool? isPublic, bool? isStatic)
        {
            return type.GetMethod(name, GetBindingFlags(isPublic, isStatic));
        }

        /// <summary>
        /// Gets a method on the specified type.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="name">Name of the method on the type.</param>
        /// <param name="types">Argument types for the method.</param>
        /// <param name="isPublic">True if method should search only public methods, false if it should search only non-public methods, and null for both.</param>
        /// <param name="isStatic">True if method should search only static methods, false if it should search only instance methods, and null for both.</param>
        /// <returns>MethodInfo for the method with the specified characteristics if found, otherwise null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static MethodInfo GetMethod(this Type type, string name, Type[] types, bool? isPublic, bool? isStatic)
        {
            return type.GetMethod(name, GetBindingFlags(isPublic, isStatic), null, types, null);
        }

        /// <summary>
        /// Replacement for Delegate.Method
        /// </summary>
        /// <param name="thisDelegate">The delegate on which to call this helper method</param>
        /// <returns>The MethodInfo for the static method represented by the current delegate</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static MethodInfo Method(this Delegate thisDelegate)
        {
            return thisDelegate.Method;
        }

        /// <summary>
        /// Gets a field on the specified type.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="name">Name of the field on the type.</param>
        /// <param name="isPublic">True if method should search only public fields, false if it should search only non-public fields, and null for both.</param>
        /// <param name="isStatic">True if method should search only static fields, false if it should search only instance fields, and null for both.</param>
        /// <returns>FieldInfo for the field with the specified characteristics if found, otherwise null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static FieldInfo GetField(this Type type, string name, bool? isPublic, bool? isStatic)
        {
            return type.GetField(name, GetBindingFlags(isPublic, isStatic));
        }

        /// <summary>
        /// Gets a property on the specified type.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="name">Name of the property on the type.</param>
        /// <param name="isPublic">True if method should search only public properties, false if it should search only non-public properties, and null for both.</param>
        /// <param name="isStatic">True if method should search only static properties, false if it should search only instance properties, and null for both.</param>
        /// <returns>PropertyInfo for the property with the specified characteristics if found, otherwise null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static PropertyInfo GetProperty(this Type type, string name, bool? isPublic, bool? isStatic)
        {
            return type.GetProperty(name, GetBindingFlags(isPublic, isStatic));
        }

        /// <summary>
        /// Gets a property on the specified type.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="name">Name of the property on the type.</param>
        /// <param name="isPublic">True if method should search only public properties, false if it should search only non-public properties, and null for both.</param>
        /// <param name="isStatic">True if method should search only static properties, false if it should search only instance properties, and null for both.</param>
        /// <param name="returnType">Return type of the property.</param>
        /// <param name="parameterTypes">Parameter types of the property.</param>
        /// <returns>PropertyInfo for the property with the specified characteristics if found, otherwise null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static PropertyInfo GetProperty(this Type type, string name, bool? isPublic, bool? isStatic, Type returnType, Type[] parameterTypes)
        {
            return type.GetProperty(name, GetBindingFlags(isPublic, isStatic), null, returnType, parameterTypes, null);
        }

        /// <summary>
        /// Gets methods on the specified type.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="isPublic">True if method should search only public methods, false if it should search only non-public methods, and null for both.</param>
        /// <param name="isStatic">True if method should search only static methods, false if it should search only instance methods, and null for both.</param>
        /// <returns>MethodInfos for the methods with the specified characteristics.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static IEnumerable<MethodInfo> GetMethods(this Type type, bool? isPublic, bool? isStatic)
        {
            return type.GetMethods(GetBindingFlags(isPublic, isStatic));
        }

        /// <summary>
        /// Gets fields on the specified type.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>FieldInfos for the fields with default characteristics.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Taupo.Platforms", "TP0001:PlatformApiDifferencesRule", Justification = "This is the helper method that must be used")]
        internal static IEnumerable<FieldInfo> GetFields(this Type type)
        {
            return type.GetFields(null, null);
        }

        /// <summary>
        /// Gets fields on the specified type.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="isPublic">True if method should search only public fields, false if it should search only non-public fields, and null for both.</param>
        /// <param name="isStatic">True if method should search only static fields, false if it should search only instance fields, and null for both.</param>
        /// <returns>FieldInfos for the fields with the specified characteristics.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static IEnumerable<FieldInfo> GetFields(this Type type, bool? isPublic, bool? isStatic)
        {
            return type.GetFields(GetBindingFlags(isPublic, isStatic));
        }

        /// <summary>
        /// Gets properties on the specified type.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="isPublic">True if method should search only public properties, false if it should search only non-public properties, and null for both.</param>
        /// <param name="isStatic">True if method should search only static properties, false if it should search only instance properties, and null for both.</param>
        /// <returns>PropertyInfos for the properties with the specified characteristics.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static IEnumerable<PropertyInfo> GetProperties(this Type type, bool? isPublic, bool? isStatic)
        {
            return type.GetProperties(GetBindingFlags(isPublic, isStatic));
        }

        /// <summary>
        /// Gets nested types on the specified type
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="isPublic">True if method should search only public types, false if it should search only non-public types, and null for both.</param>
        /// <returns>Nested types with the specified criteria</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Taupo.Platforms", "TP0001:PlatformApiDifferencesRule", Justification = "This is the helper method that must be used")]
        internal static IEnumerable<Type> GetNestedTypes(this Type type, bool? isPublic)
        { 
            return type.GetNestedTypes(GetBindingFlags(isPublic, false));
        }

        /// <summary>
        /// Replacement for Type.ContainsGenericParameters.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool ContainsGenericParameters(this Type type)
        {
            return type.ContainsGenericParameters;
        }

        /// <summary>
        /// Gets UnicodeCategory enum for a given character.
        /// </summary>
        /// <param name="character">Character to get UnicodeCategory for..</param>
        /// <returns>FieldInfos for the fields with the specified characteristics.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static UnicodeCategory GetUnicodeCategory(this char character)
        {
            return char.GetUnicodeCategory(character);
        }

        /// <summary>
        /// Replacement for Attribute.GetCustomAttribute
        /// </summary>
        /// <param name="element">Element to look custom attribute at.</param>
        /// <typeparam name="TAttribute">Attribute type.</typeparam>
        /// <returns>Found attribute of the given type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static TAttribute GetCustomAttribute<TAttribute>(Type element) where TAttribute : Attribute
        {
            return PlatformHelper.GetCustomAttribute<TAttribute>(element, true);
        }
        
        /// <summary>
        /// Replacement for Attribute.GetCustomAttribute
        /// </summary>
        /// <param name="element">Element to look custom attribute at.</param>
        /// <param name="inherit">Whether to look for inherited attributes.</param>
        /// <typeparam name="TAttribute">Attribute type.</typeparam>
        /// <returns>Found attribute of the given type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static TAttribute GetCustomAttribute<TAttribute>(Type element, bool inherit) where TAttribute : Attribute
        {
            return (TAttribute)Attribute.GetCustomAttribute(element, typeof(TAttribute), inherit);
        }

        /// <summary>
        /// Replacement for Attribute.GetCustomAttribute
        /// </summary>
        /// <param name="element">Element to look custom attribute at.</param>
        /// <typeparam name="TAttribute">Attribute type.</typeparam>
        /// <returns>Found attribute of the given type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static TAttribute GetCustomAttribute<TAttribute>(PropertyInfo element) where TAttribute : Attribute
        {
            return PlatformHelper.GetCustomAttribute<TAttribute>(element, true);
        }

        /// <summary>
        /// Replacement for Attribute.GetCustomAttribute
        /// </summary>
        /// <param name="element">Element to look custom attribute at.</param>
        /// <param name="inherit">Whether to look for inherited attributes.</param>
        /// <typeparam name="TAttribute">Attribute type.</typeparam>
        /// <returns>Found attribute of the given type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static TAttribute GetCustomAttribute<TAttribute>(PropertyInfo element, bool inherit) where TAttribute : Attribute
        {
            return (TAttribute)Attribute.GetCustomAttribute(element, typeof(TAttribute), inherit);
        }

        /// <summary>
        /// Creates a DateTime with CultureInfo.InvariantCulture.Calendar
        /// </summary>
        /// <param name="year">Year argument.</param>
        /// <param name="month">Month argument.</param>
        /// <param name="day">Day argument.</param>
        /// <returns>Created DateTime.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static DateTime CreateDateTimeWithInvariantCultureCalendar(int year, int month, int day)
        {
            return new DateTime(year, month, day, CultureInfo.InvariantCulture.Calendar);
        }

        /// <summary>
        /// Replacement for IConvertible.ToType
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="argument">Argument to convert.</param>
        /// <returns>Argument converted to a requested type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static TResult ConvertToType<TResult>(this long argument)
        {
            return (TResult)((IConvertible)argument).ToType(typeof(TResult), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Replacement for Delegate.CreateDelegate
        /// </summary>
        /// <param name="methodInfo">MethodInfo used for the delegate.</param>
        /// <param name="delegateType">Type of the delegate.</param>
        /// <param name="target">Target object.</param>
        /// <returns>Created delegate.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static Delegate CreateDelegate(this MethodInfo methodInfo, Type delegateType, object target)
        {
            return Delegate.CreateDelegate(delegateType, target, methodInfo);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method is called by the internal helpers.")]
        private static BindingFlags GetBindingFlags(bool? isPublic, bool? isStatic)
        {
            BindingFlags bindingFlags = BindingFlags.Default;
            if (true == isPublic)
            {
                bindingFlags |= BindingFlags.Public;
            }
            else if (false == isPublic)
            {
                bindingFlags |= BindingFlags.NonPublic;
            }
            else
            {
                bindingFlags |= BindingFlags.Public | BindingFlags.NonPublic;
            }

            if (true == isStatic)
            {
                bindingFlags |= BindingFlags.Static;
            }
            else if (false == isStatic)
            {
                bindingFlags |= BindingFlags.Instance;
            }
            else
            {
                bindingFlags |= BindingFlags.Static | BindingFlags.Instance;
            }

            return bindingFlags;
        }
    }
}
