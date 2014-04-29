//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

#if ASTORIA_CLIENT
namespace System.Data.Services.Client
#else
#if SPATIAL
namespace System.Spatial
#else
#if ODATALIB || ODATALIB_QUERY
namespace Microsoft.Data.OData
#else
namespace Microsoft.Data.Edm
#endif
#endif
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Xml;


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


        #region Helper methods for properties

        /// <summary>
        /// Replacement for Type.Assembly.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static Assembly GetAssembly(this Type type)
        {
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
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
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
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
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
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
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
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
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
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
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
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
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
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
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
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
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
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
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
            return type.IsEnum;
        }

        /// <summary>
        /// Replacement for Type.BaseType.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static Type GetBaseType(this Type type)
        {
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
            return type.BaseType;
        }

        /// <summary>
        /// Replacement for Type.ContainsGenericParameters.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool ContainsGenericParameters(this Type type)
        {
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
            return type.ContainsGenericParameters;
        }

        #endregion

        #region Helper methods for static methods

        /// <summary>
        /// Replacement for Array.AsReadOnly(T[]).
        /// </summary>
        /// <typeparam name="T">Type of items in the array.</typeparam>
        /// <param name="array">Array to use to create the ReadOnlyCollection.</param>
        /// <returns>ReadOnlyCollection containing the specified array items.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static ReadOnlyCollection<T> AsReadOnly<T>(this T[] array)
        {
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
            return Array.AsReadOnly(array);
        }

        /// <summary>
        /// Converts a string to a DateTime.
        /// </summary>
        /// <param name="text">String to be converted.</param>
        /// <returns>See documentation for method being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static DateTime ConvertStringToDateTime(string text)
        {
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
            return XmlConvert.ToDateTime(text, XmlDateTimeSerializationMode.RoundtripKind);
        }

        /// <summary>
        /// Converts a DateTime to a string.
        /// </summary>
        /// <param name="dateTime">DateTime to be converted.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static string ConvertDateTimeToString(DateTime dateTime)
        {
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
            return XmlConvert.ToString(dateTime, XmlDateTimeSerializationMode.RoundtripKind);
        }

        /// <summary>
        /// Gets the specified type.
        /// </summary>
        /// <param name="typeName">Name of the type to get.</param>
        /// <exception cref="TypeLoadException">Throws if the type could not be found.</exception>
        /// <returns>Type instance that represents the specified type name.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static Type GetTypeOrThrow(string typeName)
        {
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
            return Type.GetType(typeName, true);
        }

        /// <summary>
        /// Gets the TypeCode for the specified type.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>TypeCode representing the specified type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static TypeCode GetTypeCode(Type type)
        {
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
            return Type.GetTypeCode(type);
        }

#endregion

        #region Methods to replace other changed functionality where the replacement doesn't map exactly to an existing method on other platforms

        /// <summary>
        /// Replacement for usage of MemberInfo.MemberType property.
        /// </summary>
        /// <param name="member">MemberInfo on which to access this method.</param>
        /// <returns>True if the specified member is a property, otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsProperty(MemberInfo member)
        {
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
            return member.MemberType == MemberTypes.Property;
        }

        /// <summary>
        /// Replacement for usage of Type.IsPrimitive property.
        /// </summary>
        /// <param name="type">Type on which to access this method.</param>
        /// <returns>True if the specified type is primitive, otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsPrimitive(this Type type)
        {
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
            return type.IsPrimitive;
        }

        /// <summary>
        /// Replacement for usage of Type.IsSealed property.
        /// </summary>
        /// <param name="type">Type on which to access this method.</param>
        /// <returns>True if the specified type is sealed, otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsSealed(this Type type)
        {
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
            return type.IsSealed;
        }

        /// <summary>
        /// Replacement for usage of MemberInfo.MemberType property.
        /// </summary>
        /// <param name="member">MemberInfo on which to access this method.</param>
        /// <returns>True if the specified member is a method, otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsMethod(MemberInfo member)
        {
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
            return member.MemberType == MemberTypes.Method;
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
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
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
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
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
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
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
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
            BindingFlags bindingFlags = BindingFlags.Instance;
            bindingFlags |= isPublic ? BindingFlags.Public : BindingFlags.NonPublic;
            return type.GetConstructor(bindingFlags, null, argTypes, null);
        }

        /// <summary>
        /// Gets a method on the specified type.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="name">Name of the method on the type.</param>
        /// <param name="isPublic">True if method should search only public methods, false if it should search only non-public methods.</param>
        /// <param name="isStatic">True if method should search only static methods, false if it should search only instance methods.</param>
        /// <returns>MethodInfo for the method with the specified characteristics if found, otherwise null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static MethodInfo GetMethod(this Type type, string name, bool isPublic, bool isStatic)
        {
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
            BindingFlags bindingFlags = BindingFlags.Default;
            bindingFlags |= isPublic ? BindingFlags.Public : BindingFlags.NonPublic;
            bindingFlags |= isStatic ? BindingFlags.Static : BindingFlags.Instance;
            return type.GetMethod(name, bindingFlags);
        }

        /// <summary>
        /// Gets a method on the specified type.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="name">Name of the method on the type.</param>
        /// <param name="types">Argument types for the method.</param>
        /// <param name="isPublic">True if method should search only public methods, false if it should search only non-public methods.</param>
        /// <param name="isStatic">True if method should search only static methods, false if it should search only instance methods.</param>
        /// <returns>MethodInfo for the method with the specified characteristics if found, otherwise null.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static MethodInfo GetMethod(this Type type, string name, Type[] types, bool isPublic, bool isStatic)
        {
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
            BindingFlags bindingFlags = BindingFlags.Default;
            bindingFlags |= isPublic ? BindingFlags.Public : BindingFlags.NonPublic;
            bindingFlags |= isStatic ? BindingFlags.Static : BindingFlags.Instance;
            return type.GetMethod(name, bindingFlags, null, types, null);
        }

        /// <summary>
        /// Gets all public static methods for a type.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>Enumerable of all public static methods for the specified type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static IEnumerable<MethodInfo> GetPublicStaticMethods(this Type type)
        {
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
            return type.GetMethods(BindingFlags.Static | BindingFlags.Public);
        }

        /// <summary>
        /// Replacement for Type.GetNestedTypes(BindingFlags.NonPublic)
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>All types nested in the current type</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static IEnumerable<Type> GetNonPublicNestedTypes(this Type type)
        {
#if ODATALIB
            DebugUtils.CheckNoExternalCallers();
#endif
            return type.GetNestedTypes(BindingFlags.NonPublic);
        }
        #endregion

    }
}
