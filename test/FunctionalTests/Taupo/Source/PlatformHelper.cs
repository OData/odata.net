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
#if WIN8
    using System.Threading;
#endif
#if TAUPO_CORE || TAUPO_ASTORIA || WIN8
    using System.Resources;
    using System.Xml;
#endif

#if WIN8

    /// <summary>
    /// Replacement for Comparison&lt;T&gt;.
    /// </summary>
    /// <typeparam name="T">Type of items being compared.</typeparam>
    /// <param name="x">The first item to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>Less than 0 if x is less than y, 0 if x and y are equal, and greater than 0 if x is greater than y.</returns>
    internal delegate int Comparison<T>(T x, T y);

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
#if WIN8
        /// <summary>
        /// Replacement for Uri.UriSchemeHttp, which does not exist on Win8.
        /// </summary>
        internal static readonly string UriSchemeHttp = "http";

        /// <summary>
        /// Replacement for Uri.UriSchemeHttps, which does not exist on Win8.
        /// </summary>
        internal static readonly string UriSchemeHttps = "https";
#else
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
#endif

        /// <summary>
        /// Replacement for Type.Assembly.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static Assembly GetAssembly(this Type type)
        {
#if WIN8
            return type.GetTypeInfo().Assembly;
#else
            return type.Assembly;
#endif
        }

        /// <summary>
        /// Replacement for Type.IsValueType.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsValueType(this Type type)
        {
#if WIN8
            return type.GetTypeInfo().IsValueType;
#else
            return type.IsValueType;
#endif
        }

        /// <summary>
        /// Replacement for Type.IsGenericParameter.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsGenericParameter(this Type type)
        {
            // WIN8 TODO: Add this back to Type, then this helper method should no longer be required.
#if WIN8
            return type.GetTypeInfo().IsGenericParameter;
#else
            return type.IsGenericParameter;
#endif
        }

        /// <summary>
        /// Replacement for Type.IsAbstract.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsAbstract(this Type type)
        {
#if WIN8
            return type.GetTypeInfo().IsAbstract;
#else
            return type.IsAbstract;
#endif
        }

        /// <summary>
        /// Replacement for Type.IsGenericType.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsGenericType(this Type type)
        {
#if WIN8
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }

        /// <summary>
        /// Replacement for Type.IsGenericTypeDefinition.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsGenericTypeDefinition(this Type type)
        {
#if WIN8
            return type.GetTypeInfo().IsGenericTypeDefinition;
#else
            return type.IsGenericTypeDefinition;
#endif
        }

        /// <summary>
        /// Replacement for Type.IsVisible.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsVisible(this Type type)
        {
#if WIN8
            return type.GetTypeInfo().IsVisible;
#else
            return type.IsVisible;
#endif
        }

        /// <summary>
        /// Replacement for Type.IsInterface.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsInterface(this Type type)
        {
#if WIN8
            return type.GetTypeInfo().IsInterface;
#else
            return type.IsInterface;
#endif
        }

        /// <summary>
        /// Replacement for Type.IsClass.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsClass(this Type type)
        {
#if WIN8
            return type.GetTypeInfo().IsClass;
#else
            return type.IsClass;
#endif
        }

        /// <summary>
        /// Replacement for Type.IsEnum.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsEnum(this Type type)
        {
#if WIN8
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }

        /// <summary>
        /// Replacement for Type.IsPrimitive.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsPrimitive(this Type type)
        {
#if WIN8
            return type.GetTypeInfo().IsPrimitive;
#else
            return type.IsPrimitive;
#endif
        }

        /// <summary>
        /// Replacement for Type.IsPublic.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsPublic(this Type type)
        {
#if WIN8
            return type.GetTypeInfo().IsPublic;
#else
            return type.IsPublic;
#endif
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
#if WIN8
            return type.GetTypeInfo().IsDefined(attributeType);
#else
            return Attribute.IsDefined(type, attributeType);
#endif
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
#if WIN8
            return type.GetTypeInfo().IsDefined(attributeType, inherit);
#else
            return Attribute.IsDefined(type, attributeType, inherit);
#endif
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
#if WIN8
            return propertyInfo.GetCustomAttribute(attributeType, inherit) != null;
#else
            return propertyInfo.IsDefined(attributeType, inherit);
#endif
        }

        /// <summary>
        /// Replacement for Type.BaseType.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static Type GetBaseType(this Type type)
        {
#if WIN8
            return type.GetTypeInfo().BaseType;
#else
            return type.BaseType;
#endif
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
#if WIN8
            return new ReadOnlyCollection<T>(array);
#else
            return Array.AsReadOnly(array);
#endif
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
            // WIN8: On Win8, XmlConvert overloads that take an XmlDateTimeSerializationMode have been removed. To get conversions as close as possible to other platforms, we
            // need to use DateTimeOffset for conversions instead, then convert to and from DateTime as necessary.
#if WIN8
            // DateTimeOffset always applies an offset (using the local one if one is not present in the input), but the old XmlConvert methods
            // would produce a DateTime value with Kind=DateTimeKind.Unspecified and no offset applied if none was specified in the input string.
            // Before we convert to DateTimeOffset, we need to determine what kind of input we have so we can still produce the same kind of DateTime
            // instances that we would have gotten on other platforms with XmlConvert.ToDateTime.
            // 
            // The XML DateTime pattern is described here: http://www.w3.org/TR/xmlschema-2/#dateTime
            // For example:
            //      No timezone specified: "2012-12-21T15:01:23.1234567"
            //      UTC timezone: "2012-12-21T15:01:23.1234567Z"
            //      Timezone offset from UTC: "2012-12-21T15:01:23.1234567-08:00" or "2012-12-21T15:01:23.1234567+08:00"
            // If timezone is specified, the indicator will always be at the same place from the end of the string as in the examples above, so we can look there for the Z or +/-.
            DateTimeKind inputKind;
            if (text[text.Length - 1] == 'Z')
            {
                inputKind = DateTimeKind.Utc;
            }
            else if (text[text.Length - 6] == '-' || text[text.Length - 6] == '+')
            {
                inputKind = DateTimeKind.Local;
            }
            else
            {
                // To prevent ToDateTimeOffset from applying the local offset in this case, we will append the Z to indicate UTC time
                inputKind = DateTimeKind.Unspecified;
                text = text + "Z";
            }
            
            var dateTimeOffset = XmlConvert.ToDateTimeOffset(text);
            switch (inputKind)
            {
                case DateTimeKind.Local:
                    return dateTimeOffset.LocalDateTime;
                case DateTimeKind.Utc:
                    return dateTimeOffset.UtcDateTime;
                default:
                    Debug.Assert(inputKind == DateTimeKind.Unspecified, "All dates must be Utc, Local, or Unspecified.");
                    return dateTimeOffset.DateTime;
            }
#else
            return XmlConvert.ToDateTime(text, XmlDateTimeSerializationMode.RoundtripKind);
#endif
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
            // WIN8: On Win8, XmlConvert overloads that take an XmlDateTimeSerializationMode have been removed. To get conversions as close as possible to other platforms, we
            // need to use DateTimeOffset for conversions instead, then convert to and from DateTime as necessary.
#if WIN8
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                // If we just cast to DateTimeOffset here, the local offset will be applied, which can alter the meaning of the value.
                // Instead we need to create a new DateTimeOffset with the timezone explicitly set to UTC, which will prevent
                // any offset from being used. The resulting string does have the Z on it in that case, but we want to leave the timezone
                // unspecified here, so we will just remove that.
                DateTimeOffset dateTimeOffset = new DateTimeOffset(dateTime, TimeSpan.Zero);
                string outputWithZ = XmlConvert.ToString(dateTimeOffset);
                Debug.Assert(outputWithZ[outputWithZ.Length - 1] == 'Z', "Expected DateTimeOffset to be a UTC value.");
                return outputWithZ.TrimEnd('Z');
            }
            else
            {
                // For Utc and Local kinds, ToString produces the same string that the old XmlConvert methods would produce.
                return XmlConvert.ToString((DateTimeOffset)dateTime);
            }
#else
            return XmlConvert.ToString(dateTime, XmlDateTimeSerializationMode.RoundtripKind);
#endif
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
            // WIN8 TODO: Add Type.GetType(string, bool) back to Type so this method should no longer be required.
#if WIN8
            Type type = Type.GetType(typeName);
            if (type == null)
            {
                // devnote (sparra): Not throwing a custom error message here because this method is expected to be removed. See comments above.
                throw new TypeLoadException();
            }

            return type;
#else
            return Type.GetType(typeName, true);
#endif
        }

        /// <summary>
        /// Replacement for usage of MemberInfo.MemberType property.
        /// </summary>
        /// <param name="member">MemberInfo on which to access this method.</param>
        /// <returns>True if the specified member is a property, otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsProperty(MemberInfo member)
        {
            // WIN8: MemberInfo.MemberType property has been removed from Win8, along with the MemberTypes enum.
#if WIN8
            return member is PropertyInfo;
#else
            return member.MemberType == MemberTypes.Property;
#endif
        }

        /// <summary>
        /// Replacement for usage of MemberInfo.MemberType property.
        /// </summary>
        /// <param name="member">MemberInfo on which to access this method.</param>
        /// <returns>True if the specified member is a method, otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool IsMethod(MemberInfo member)
        {
            // WIN8: MemberInfo.MemberType property has been removed from Win8, along with the MemberTypes enum.
#if WIN8
            return member is MethodInfo;
#else
            return member.MemberType == MemberTypes.Method;
#endif
        }

        /// <summary>
        /// Gets instance properties for the specified type.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>Enumerable of instance properties for the type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static IEnumerable<PropertyInfo> GetAllInstanceProperties(this Type type)
        {
            // WIN8: The BindingFlags enum and all related reflection method overloads have been removed from Win8. Instead of trying to provide
            // a general purpose flags enum and methods that can take any combination of the flags, we provide more restrictive methods that
            // still allow for the same functionality as needed by the calling code.
#if WIN8
            throw new NotImplementedException();
#else
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
#endif
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
            // WIN8: The BindingFlags enum and all related reflection method overloads have been removed from Win8. Instead of trying to provide
            // a general purpose flags enum and methods that can take any combination of the flags, we provide more restrictive methods that
            // still allow for the same functionality as needed by the calling code.
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
            // WIN8: The BindingFlags enum and all related reflection method overloads have been removed from Win8. Instead of trying to provide
            // a general purpose flags enum and methods that can take any combination of the flags, we provide more restrictive methods that
            // still allow for the same functionality as needed by the calling code.
#if WIN8
            // TypeInfo.DeclaredProperties and Type.GetRuntimeProperties return both public and private properties, so need to filter out only public ones.
            IEnumerable<PropertyInfo> properties = declaredOnly ? type.GetTypeInfo().DeclaredProperties : type.GetRuntimeProperties();
            return properties.Where(p => IsPublic(p) && (!instanceOnly || IsInstance(p)));
#else
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
#endif
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
            // WIN8: The BindingFlags enum and all related reflection method overloads have been removed from Win8. Instead of trying to provide
            // a general purpose flags enum and methods that can take any combination of the flags, we provide more restrictive methods that
            // still allow for the same functionality as needed by the calling code.
#if WIN8
            return type.GetTypeInfo().DeclaredConstructors.Where(c => !c.IsStatic && isPublic == c.IsPublic);
#else
            BindingFlags bindingFlags = BindingFlags.Instance;
            bindingFlags |= isPublic ? BindingFlags.Public : BindingFlags.NonPublic;
            return type.GetConstructors(bindingFlags);
#endif
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
            // WIN8: The BindingFlags enum and all related reflection method overloads have been removed from Win8. Instead of trying to provide
            // a general purpose flags enum and methods that can take any combination of the flags, we provide more restrictive methods that
            // still allow for the same functionality as needed by the calling code.
#if WIN8
            return GetInstanceConstructors(type, isPublic).SingleOrDefault(c => CheckTypeArgs(c, argTypes));
#else
            BindingFlags bindingFlags = BindingFlags.Instance;
            bindingFlags |= isPublic ? BindingFlags.Public : BindingFlags.NonPublic;
            return type.GetConstructor(bindingFlags, null, argTypes, null);
#endif
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
            // WIN8: The BindingFlags enum and all related reflection method overloads have been removed from Win8. Instead of trying to provide
            // a general purpose flags enum and methods that can take any combination of the flags, we provide more restrictive methods that
            // still allow for the same functionality as needed by the calling code.
#if WIN8
            return type.GetRuntimeMethods().Where(m => m.Name == name && (isPublic == null || m.IsPublic == isPublic) && (isStatic == null || m.IsStatic == isStatic)).SingleOrDefault();
#else
            return type.GetMethod(name, GetBindingFlags(isPublic, isStatic));
#endif
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
            // WIN8: The BindingFlags enum and all related reflection method overloads have been removed from Win8. Instead of trying to provide
            // a general purpose flags enum and methods that can take any combination of the flags, we provide more restrictive methods that
            // still allow for the same functionality as needed by the calling code.
#if WIN8
            MethodInfo methodInfo = type.GetRuntimeMethod(name, types);
            if (isPublic == methodInfo.IsPublic && isStatic == methodInfo.IsStatic)
            {
                return methodInfo;
            }

            return null;
#else
            return type.GetMethod(name, GetBindingFlags(isPublic, isStatic), null, types, null);
#endif
        }

        /// <summary>
        /// Replacement for Delegate.Method
        /// </summary>
        /// <param name="thisDelegate">The delegate on which to call this helper method</param>
        /// <returns>The MethodInfo for the static method represented by the current delegate</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static MethodInfo Method(this Delegate thisDelegate)
        {
#if WIN8
            return thisDelegate.GetType().GetMethod("GetMethodImpl", false, false);
#else
            return thisDelegate.Method;
#endif
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
            // WIN8: The BindingFlags enum and all related reflection method overloads have been removed from Win8. Instead of trying to provide
            // a general purpose flags enum and methods that can take any combination of the flags, we provide more restrictive methods that
            // still allow for the same functionality as needed by the calling code.
#if WIN8
            return type.GetRuntimeFields().Where(f => f.Name == name && (isPublic == null || f.IsPublic == isPublic) && (isStatic == null || f.IsStatic == isStatic)).SingleOrDefault();
#else
            return type.GetField(name, GetBindingFlags(isPublic, isStatic));
#endif
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
            // WIN8: The BindingFlags enum and all related reflection method overloads have been removed from Win8. Instead of trying to provide
            // a general purpose flags enum and methods that can take any combination of the flags, we provide more restrictive methods that
            // still allow for the same functionality as needed by the calling code.
#if WIN8
            return type.GetRuntimeProperties().Where(p => p.Name == name && (isPublic == null || p.GetMethod.IsPublic == isPublic) && (isStatic == null || p.GetMethod.IsStatic == isStatic)).SingleOrDefault();
#else
            return type.GetProperty(name, GetBindingFlags(isPublic, isStatic));
#endif
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
#if WIN8
            var matchingProperties = new List<PropertyInfo>();
            var propertyCandidates = type.GetRuntimeProperties().Where(p => p.Name == name && (isPublic == null || p.GetMethod.IsPublic == isPublic) && (isStatic == null || p.GetMethod.IsStatic == isStatic));
            foreach(var propertyCandidate in propertyCandidates)
            {
                if (propertyCandidate.PropertyType != returnType)
                {
                    continue;
                }

                var candidateParameters = propertyCandidate.GetIndexParameters().Select(p => p.ParameterType).ToArray();
                if (parameterTypes == null && candidateParameters.Length != 0)
                {
                    continue;
                }

                if (candidateParameters.Length != parameterTypes.Length)
                {
                    continue;
                }

                bool isMatch = true;
                for (int i = 0; i < candidateParameters.Length; i++)
			    {
                    if (candidateParameters[i] != parameterTypes[i])
                    {
                        isMatch = false;
                        break;
                    }
			    }

                if (isMatch)
                {
                    matchingProperties.Add(propertyCandidate);
                }
            }

            return matchingProperties.SingleOrDefault();
#else
            return type.GetProperty(name, GetBindingFlags(isPublic, isStatic), null, returnType, parameterTypes, null);
#endif
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
            // WIN8: The BindingFlags enum and all related reflection method overloads have been removed from Win8. Instead of trying to provide
            // a general purpose flags enum and methods that can take any combination of the flags, we provide more restrictive methods that
            // still allow for the same functionality as needed by the calling code.
#if WIN8
            return type.GetRuntimeMethods().Where(m => (isPublic == null || m.IsPublic == isPublic) &&  (isStatic == null || m.IsStatic == isStatic));
#else
            return type.GetMethods(GetBindingFlags(isPublic, isStatic));
#endif
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
            // WIN8: The BindingFlags enum and all related reflection method overloads have been removed from Win8. Instead of trying to provide
            // a general purpose flags enum and methods that can take any combination of the flags, we provide more restrictive methods that
            // still allow for the same functionality as needed by the calling code.
#if WIN8
            return type.GetRuntimeFields().Where(f => (isPublic == null || f.IsPublic == isPublic) && (isStatic == null || f.IsStatic == isStatic));
#else
            return type.GetFields(GetBindingFlags(isPublic, isStatic));
#endif
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
            // WIN8: The BindingFlags enum and all related reflection method overloads have been removed from Win8. Instead of trying to provide
            // a general purpose flags enum and methods that can take any combination of the flags, we provide more restrictive methods that
            // still allow for the same functionality as needed by the calling code.
#if WIN8
            return type.GetRuntimeProperties().Where(p => (isPublic == null || p.GetMethod.IsPublic == isPublic) && (isStatic == null || p.GetMethod.IsStatic == isStatic));
#else
            return type.GetProperties(GetBindingFlags(isPublic, isStatic));
#endif
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
#if WIN8
            return type.GetTypeInfo().DeclaredNestedTypes.Where(t => isPublic == null || t.IsPublic == isPublic).Select(t => t.AsType());
#else
            // WIN8: The BindingFlags enum and all related reflection method overloads have been removed from Win8. Instead of trying to provide
            // a general purpose flags enum and methods that can take any combination of the flags, we provide more restrictive methods that
            // still allow for the same functionality as needed by the calling code.
            return type.GetNestedTypes(GetBindingFlags(isPublic, false));
#endif
        }

        /// <summary>
        /// Replacement for Type.ContainsGenericParameters.
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static bool ContainsGenericParameters(this Type type)
        {
#if WIN8
            return type.IsGenericType() && !type.GetGenericArguments().Any(ga => ga.IsGenericParameter);
#else
            return type.ContainsGenericParameters;
#endif
        }

        /// <summary>
        /// Gets UnicodeCategory enum for a given character.
        /// </summary>
        /// <param name="character">Character to get UnicodeCategory for..</param>
        /// <returns>FieldInfos for the fields with the specified characteristics.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is shared among multiple assemblies and this method should be available as a helper in case it is needed in new code.")]
        internal static UnicodeCategory GetUnicodeCategory(this char character)
        {
#if WIN8
            return CharUnicodeInfo.GetUnicodeCategory(character);
#else
            return char.GetUnicodeCategory(character);
#endif
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
#if WIN8
            return PlatformHelper.GetCustomAttributes(element, inherit).OfType<TAttribute>().SingleOrDefault();
#else
            return (TAttribute)Attribute.GetCustomAttribute(element, typeof(TAttribute), inherit);
#endif
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
#if WIN8
            return (TAttribute)element.GetCustomAttribute(typeof(TAttribute), inherit);
#else
            return (TAttribute)Attribute.GetCustomAttribute(element, typeof(TAttribute), inherit);
#endif
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
#if WIN8
            return new DateTime(year, month, day);
#else
            return new DateTime(year, month, day, CultureInfo.InvariantCulture.Calendar);
#endif
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
#if WIN8
            return (TResult)Convert.ChangeType(argument, typeof(TResult), CultureInfo.InvariantCulture);
#else
            return (TResult)((IConvertible)argument).ToType(typeof(TResult), CultureInfo.InvariantCulture);
#endif
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
#if WIN8
            return methodInfo.CreateDelegate(delegateType, target);
#else
            return Delegate.CreateDelegate(delegateType, target, methodInfo);
#endif
        }

#if !WIN8
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
#else
#if WIN8

        /// <summary>
        /// Replacement for List.AsReadOnly().
        /// </summary>
        /// <typeparam name="T">Type of items in the list.</typeparam>
        /// <param name="list">List to use as the source of the ReadOnlyCollection.</param>
        /// <returns>ReadOnlyCollection of the items in the specified list.</returns>
        internal static ReadOnlyCollection<T> AsReadOnly<T>(this List<T> list)
        {
            return new ReadOnlyCollection<T>(list);
        }

        /// <summary>
        /// Replacement for Type.IsAssignableFrom(Type)
        /// </summary>
        /// <param name="thisType">Type on which to call this helper method.</param>
        /// <param name="otherType">Type to test for assignability.</param>
        /// <returns>See documentation for method being accessed in the body of the method.</returns>
        internal static bool IsAssignableFrom(this Type thisType, Type otherType)
        {
            if (null == otherType)
            {
                return false;
            }
            return thisType.GetTypeInfo().IsAssignableFrom(otherType.GetTypeInfo());
        }

        /// <summary>
        /// Replacement for Type.IsSubclassOf(Type).
        /// </summary>
        /// <param name="thisType">Type on which to call this helper method.</param>
        /// <param name="otherType">Type to test if typeType is a subclass.</param>
        /// <returns>True if thisType is a subclass of otherType, otherwise false.</returns>
        /// <remarks>
        /// TODO: Add this back to TypeInfo. This method will still be needed since it works on Type, but the 
        ///       implementation should just be able to call the TypeInfo version directly instead of the full implementation here.
        /// </remarks>
        internal static bool IsSubclassOf(this Type thisType, Type otherType)
        {
            // devnotes (sparra):
            //      (1) This intentionally does not take interfaces into account, because the other platform implementations also do not.
            //          E.g, a type is never a subclass of an interface, even if the type is an interface itself.
            //      (2) On other platforms, there is an odd behavior where typeof(SomeInterface).IsSubclassOf(typeof(object)) returns true for the default
            //          implementation on Type, even though object is not a BaseType of any interface. Since we are not using IsSubclassOf in any way that
            //          would be affected by this, we do not try to duplicate that behavior with this helper method, and it will return false in that case.

            // If the types are the same one cannot be a subclass of the other.
            if (thisType == otherType)
            {
                return false;
            }

            Type type = thisType.GetTypeInfo().BaseType;
            while (type != null)
            {
                if (type == otherType)
                {
                    return true;
                }

                type = type.GetTypeInfo().BaseType;
            }

            return false;
        }

        /// <summary>
        /// Replacement for List.Sort(Comparison).
        /// </summary>
        /// <typeparam name="T">Type of the items in the list to be sorted.</typeparam>
        /// <param name="list">List of items to be sorted.</param>
        /// <param name="comparison">Delegate to use to compare items during sorting.</param>
        internal static void Sort<T>(this List<T> list, Comparison<T> comparison)
        {
            Comparer<T> comparer = new Comparer<T>(comparison);
            list.Sort(comparer);
        }

        /// <summary>
        /// Replacement for assembly.GetExportedTypes().
        /// </summary>
        /// <param name="assembly">Assembly on which to call this helper method.</param>
        /// <returns>List of exported types.</returns>
        internal static IEnumerable<Type> GetExportedTypes(this Assembly assembly)
        {
            return assembly.ExportedTypes;
        }

        /// <summary>
        /// Replacement for GetMethod(Type, string).
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="name">Method to find on the specified type.</param>
        /// <returns>MethodInfo if one was found for the specified type, otherwise false.</returns>
        internal static MethodInfo GetMethod(this Type type, string name)
        {
            return GetMethods(type).Where(m => m.Name == name).SingleOrDefault();
        }

        /// <summary>
        /// Replacement for Type.GetMethods().
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>Enumerable of all public methods for the specified type.</returns>
        internal static IEnumerable<MethodInfo> GetMethods(this Type type)
        {
            // GetRuntimeMethods() returns both public and private members, so need to filter to public only to match Type.GetMethods() behavior on other platforms.
            return type.GetRuntimeMethods().Where(m => m.IsPublic);
        }

        /// <summary>
        /// Replacement for Type.GetMethod(string, Type[]).
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="name">Name of method to find on the specified type.</param>
        /// <param name="types">Array of arguments to the method.</param>
        /// <returns>MethodInfo if one was found for the specified type, otherwise false.</returns>
        internal static MethodInfo GetMethod(this Type type, string name, Type[] types)
        {
            // GetRuntimeMethod(string, Type[]) only searched public methods, so it matches Type.GetMethod(string, Type[]) behavior on other platforms.
            return type.GetRuntimeMethod(name, types);
        }

        /// <summary>
        /// Replacement for Type.GetProperty(string, Type).
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="name">Name of public property to find on the specified type.</param>
        /// <param name="returnType">Return type for the property.</param>
        /// <returns>PropertyInfo if a property was found on the type with the specified name and return type, otherwise null.</returns>
        internal static PropertyInfo GetProperty(this Type type, string name, Type returnType)
        {
            // Type.GetRuntimeProperty returns public properties only
            PropertyInfo propertyInfo = type.GetRuntimeProperty(name);
            if (propertyInfo != null && propertyInfo.PropertyType == returnType)
            {
                return propertyInfo;
            }

            return null;
        }

        /// <summary>
        /// Replacement for Type.GetProperty(string).
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="name">Name of public property to find on the specified type.</param>
        /// <returns>PropertyInfo if a property was found on the type with the specified name and return type, otherwise null.</returns>
        internal static PropertyInfo GetProperty(this Type type, string name)
        {
            // Type.GetRuntimeProperty returns public properties only
            return type.GetRuntimeProperty(name);
        }

        /// <summary>
        /// Replacement for PropertyInfo.GetGetMethod().
        /// </summary>
        /// <param name="propertyInfo">PropertyInfo on which to call this helper method.</param>
        /// <returns>MethodInfo for the public get accessor of the specified PropertyInfo, or null if there is no get accessor or it is non-public.</returns>
        internal static MethodInfo GetGetMethod(this PropertyInfo propertyInfo)
        {
            MethodInfo getMethod = propertyInfo.GetMethod;
            if (getMethod != null && getMethod.IsPublic)
            {
                return getMethod;
            }

            return null;
        }

        /// <summary>
        /// Replacement for PropertyInfo.GetSetMethod().
        /// </summary>
        /// <param name="propertyInfo">PropertyInfo on which to call this helper method.</param>
        /// <returns>MethodInfo for the public set accessor of the specified PropertyInfo, or null if there is no set accessor or it is non-public.</returns>
        internal static MethodInfo GetSetMethod(this PropertyInfo propertyInfo)
        {
            MethodInfo setMethod = propertyInfo.SetMethod;
            if (setMethod != null && setMethod.IsPublic)
            {
                return setMethod;
            }

            return null;
        }

        /// <summary>
        /// Replacement for MethodInfo.GetBaseDefinition().
        /// </summary>
        /// <param name="methodInfo">MethodInfo on which to call this helper method.</param>
        /// <returns>See documentation for method being accessed in the body of the method.</returns>
        internal static MethodInfo GetBaseDefinition(this MethodInfo methodInfo)
        {
            return methodInfo.GetRuntimeBaseDefinition();
        }

        /// <summary>
        /// Replacement for Type.GetProperties().
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>Enumerable of all instance and static public properties on the type.</returns>
        internal static IEnumerable<PropertyInfo> GetProperties(this Type type)
        {
            return GetPublicProperties(type, false /*instanceOnly*/);
        }

        /// <summary>
        /// Replacement for Type.GetMembers().
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>Enumerable of all instance and static public properties on the type.</returns>
        internal static IEnumerable<MemberInfo> GetMembers(this Type type)
        {
            var fields = type.GetFields().Cast<MemberInfo>();
            var properties = type.GetProperties().Cast<MemberInfo>();
            var methods = type.GetMethods().Cast<MemberInfo>();
            var events = type.GetTypeInfo().DeclaredEvents.Cast<MemberInfo>();

            return fields.Concat(properties).Concat(methods).Concat(events);
        }

        /// <summary>
        /// Replacement for Type.GetCustomAttributes(Type, bool).
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="attributeType">Attribute type to find on the specified type.</param>
        /// <param name="inherit">True if the base types should be searched, false otherwise.</param>
        /// <returns>See documentation for method being accessed in the body of the method.</returns>
        internal static IEnumerable<object> GetCustomAttributes(this Type type, Type attributeType, bool inherit)
        {
            return type.GetTypeInfo().GetCustomAttributes(attributeType, inherit);
        }

        /// <summary>
        /// Replacement for Type.GetCustomAttributes(bool).
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="inherit">True if the base types should be searched, false otherwise.</param>
        /// <returns>See documentation for method being accessed in the body of the method.</returns>
        internal static IEnumerable<object> GetCustomAttributes(this Type type, bool inherit)
        {
            return type.GetTypeInfo().GetCustomAttributes(inherit);
        }

        /// <summary>
        /// Replacement for Type.GetGenericArguments().
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>Array of Type objects that represent the type arguments of a generic type or the type parameters of a generic type definition.</returns>
        internal static Type[] GetGenericArguments(this Type type)
        {
            if (type.GetTypeInfo().IsGenericTypeDefinition)
            {
                return type.GetTypeInfo().GenericTypeParameters;
            }
            else
            {
                return type.GenericTypeArguments;
            }
        }

        /// <summary>
        /// Replacement for Type.GetInterfaces().
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <returns>See documentation for property being accessed in the body of the method.</returns>
        internal static IEnumerable<Type> GetInterfaces(this Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces;
        }

        /// <summary>
        /// Replacement for Type.IsInstanceOfType(object).
        /// </summary>
        /// <param name="type">Type on which to call this helper method.</param>
        /// <param name="obj">Object to test to see if it's an instance of the specified type.</param>
        /// <returns>See documentation for method being accessed in the body of the method.</returns>
        internal static bool IsInstanceOfType(this Type type, object obj)
        {
            return type.GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo());
        }

        /// <summary>
        /// Replacement for string.Contains(char).
        /// </summary>
        /// <param name="str">String on which to call this helper method.</param>
        /// <param name="c">Character to find in the string.</param>
        /// <returns>True if the string contains the specified character, otherwise false.</returns>
        /// <remarks>
        /// String does not implement IEnumerable&lt;char&gt; on Win8.
        /// </remarks>
        internal static bool Contains(this string str, char c)
        {
            return str.IndexOf(c) != -1; 
        }

        /// <summary>
        /// Replacement for string.All(Func).
        /// </summary>
        /// <param name="str">String on which to call this helper method.</param>
        /// <param name="predicate">Predicate to call on each character of the string.</param>
        /// <returns>True if <paramref name="predicate"/> is true for all characters in the string.</returns>
        /// <remarks>
        /// String does not implement IEnumerable&lt;char&gt; on Win8.
        /// </remarks>
        internal static bool All(this string str, Func<char, bool> predicate)
        {
            return str.ToCharArray().All(predicate);
        }

        /// <summary>
        /// Replacement for string.Any(Func).
        /// </summary>
        /// <param name="str">String on which to call this helper method.</param>
        /// <param name="predicate">Predicate to call on each character of the string.</param>
        /// <returns>True if <paramref name="predicate"/> is true for any characters in the string.</returns>
        /// <remarks>
        /// String does not implement IEnumerable&lt;char&gt; on Win8.
        /// </remarks>
        internal static bool Any(this string str, Func<char, bool> predicate)
        {
            return str.ToCharArray().Any(predicate);
        }

        /// <summary>
        /// Replacement for string.Count(Func).
        /// </summary>
        /// <param name="str">String on which to call this helper method.</param>
        /// <param name="predicate">Predicate to call on each character of the string.</param>
        /// <returns>True if <paramref name="predicate"/> is true for any characters in the string.</returns>
        /// <remarks>
        /// String does not implement IEnumerable&lt;char&gt; on Win8.
        /// </remarks>
        internal static int Count(this string str, Func<char, bool> predicate)
        {
            return str.ToCharArray().Count(predicate);
        }

        /// <summary>
        /// Replacement for Stream.Close().
        /// </summary>
        /// <param name="stream">Stream on which to call this helper method.</param>
        /// <remarks>
        /// Many Close methods have been eliminated on Win8, the recommended pattern is to just use Dispose instead.
        /// </remarks>
        internal static void Close(this Stream stream)
        {
            stream.Dispose();
        }

        /// <summary>
        /// Replacement for XmlWriter.Close().
        /// </summary>
        /// <param name="writer">XmlWriter on which to call this helper method.</param>
        /// <remarks>
        /// Many Close methods have been eliminated on Win8, the recommended pattern is to just use Dispose instead.
        /// </remarks>
        internal static void Close(this XmlWriter writer)
        {
            writer.Dispose();
        }

        /// <summary>
        /// Replacement for XmlReader.Close()
        /// </summary>
        /// <param name="reader">XmlReader on which to call this helper method.</param>
        /// <remarks>
        /// Many Close methods have been eliminated on Win8, the recommended pattern is to just use Dispose instead.
        /// </remarks>
        internal static void Close(this XmlReader reader)
        {
            reader.Dispose();
        }

        /// <summary>
        /// Replacement for MemoryStream.GetBuffer().
        /// </summary>
        /// <param name="stream">Stream on which to call this helper method.</param>
        /// <returns>Byte array for contents of the stream buffer.</returns>
        internal static byte[] GetBuffer(this MemoryStream stream)
        {
            return stream.ToArray();
        }

        /// <summary>
        /// Replacement for Assembly.GetType(string, bool).
        /// </summary>
        /// <param name="assembly">Assembly on which to call this helper method.</param>
        /// <param name="typeName">Name of the type to get from the assembly.</param>
        /// <param name="throwOnError">True if an exception should be thrown if the type cannot be found, otherwise false.</param>
        /// <returns>Type instance if the type could be found in the assembly, otherwise null.</returns>
        /// <remarks>
        /// TODO: Add a new method called Assembly.GetDefinedType(string) that returns a TypeInfo and will throw like Assembly.GetType(string, true) used to.
        ///       This helper method will still be needed but should be updated to use the new implementation once it exists.
        /// </remarks>
        internal static Type GetType(this Assembly assembly, string typeName, bool throwOnError)
        {
            Type type = assembly.GetType(typeName);
            if (type == null && throwOnError)
            {
                throw new TypeLoadException();
            }

            return type;
        }

        /// <summary>
        /// Replacement for Assembly.GetTypes().
        /// </summary>
        /// <param name="assembly">Assembly on which to call this helper method.</param>
        /// <returns>Enumerable of the types in the assembly.</returns>
        internal static IEnumerable<Type> GetTypes(this Assembly assembly)
        {
            return assembly.DefinedTypes.Select(dt => dt.AsType());
        }

        /// <summary>
        /// Replacement for Guid.ToString(string format, IFormatProvider provider).
        /// </summary>
        /// <param name="argument">Guid to convert to string.</param>
        /// <param name="format">Conversion format.</param>
        /// <param name="provider">Format provider.</param>
        /// <returns>Guid converted to string.</returns>
        internal static string ToString(this Guid argument, string format, IFormatProvider provider)
        {
            return argument.ToString(format);
        }

        /// <summary>
        /// Replacement for List<T>.Foreach.
        /// </summary>
        /// <param name="list">List to iterate over.</param>
        /// <param name="action">Action to execute on every element.</param>
        internal static void ForEach<T>(this List<T> list, Action<T> action)
        {
            foreach(var item in list)
            {
                action(item);
            }
        }

        /// <summary>
        /// Checks if the specified constructor takes arguments of the specified types.
        /// </summary>
        /// <param name="constructorInfo">ConstructorInfo on which to call this helper method.</param>
        /// <param name="types">Array of type arguments to check against the constructor parameters.</param>
        /// <returns>True if the constructor takes arguments of the specified types, otherwise false.</returns>
        private static bool CheckTypeArgs(ConstructorInfo constructorInfo, Type[] types)
        {
            Debug.Assert(types != null, "Types should not be null, use a different overload of the calling method if you don't care about the parameter types.");
            
            ParameterInfo[] parameters = constructorInfo.GetParameters();
            if (parameters.Length != types.Length)
            {
                return false;
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType != types[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the specified PropertyInfo is an instance property.
        /// </summary>
        /// <param name="propertyInfo">PropertyInfo on which to call this helper method.</param>
        /// <returns>True if either the GetMethod or SetMethod for the property is an instance method.</returns>
        private static bool IsInstance(PropertyInfo propertyInfo)
        {
            return (propertyInfo.GetMethod != null && !propertyInfo.GetMethod.IsStatic) || (propertyInfo.SetMethod != null && !propertyInfo.SetMethod.IsStatic);
        }

        /// <summary>
        /// Checks if the specified PropertyInfo is a public property.
        /// </summary>
        /// <param name="propertyInfo">PropertyInfo on which to call this helper method.</param>
        /// <returns>True if either the GetMethod or SetMethod for the property is public.</returns>
        private static bool IsPublic(PropertyInfo propertyInfo)
        {
            return (propertyInfo.GetMethod != null && propertyInfo.GetMethod.IsPublic) || (propertyInfo.SetMethod != null && propertyInfo.SetMethod.IsPublic);
        }

        /// <summary>
        /// Implementation of IComparer&lt;T&gt; to be used with sorting methods.
        /// </summary>
        /// <typeparam name="T">Type of the items being compared.</typeparam>
        /// <remarks>
        /// TODO: This is needed just to support the Sort method in this class, because we have to provide an IComparer.
        ///       Provide an implementation of this in the framework so we can get rid of this and just use that one.
        /// </remarks>
        private class Comparer<T> : IComparer<T>
        {
            /// <summary>
            /// Comparison method to use when comparing two items.
            /// </summary>
            private readonly Comparison<T> comparison;

            /// <summary>
            /// Creates a new Comparer.
            /// </summary>
            /// <param name="comparison">Comparison method to use when comparing two items.</param>
            public Comparer(Comparison<T> comparison)
            {
                this.comparison = comparison;
            }

            /// <summary>
            /// Compare two items.
            /// </summary>
            /// <param name="x">First item to be compared.</param>
            /// <param name="y">Second item to be compared.</param>
            /// <returns>Less than 0 if x is less than y, 0 if x and y are equal, and greater than 0 if x is greater than y.</returns>
            public int Compare(T x, T y)
            {
                return this.comparison(x, y);
            }
        }
#endif
#endif
    }
}
