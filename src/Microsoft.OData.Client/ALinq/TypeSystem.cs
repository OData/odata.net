//---------------------------------------------------------------------
// <copyright file="TypeSystem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using Microsoft.OData.Edm;
    using Microsoft.OData.UriParser.Aggregation;
    using Microsoft.Spatial;

    /// <summary>Utility functions for processing Expression trees</summary>
    internal static class TypeSystem
    {
        /// <summary>Used for comparison with external assemblies for desktop like Microsoft.VisualBasic.</summary>
        private const string OfficialDesktopPublicKeyToken = "b03f5f7f11d50a3a";

        /// <summary> Method map for methods in URI query options</summary>
        private static readonly Dictionary<MethodInfo, string> expressionMethodMap;

        /// <summary> VB Method map for methods in URI query options</summary>
        private static readonly Dictionary<string, string> expressionVBMethodMap;

        /// <summary> Properties that should be represented as methods</summary>
        private static readonly Dictionary<PropertyInfo, MethodInfo> propertiesAsMethodsMap;

        /// <summary>
        /// Cache used to store element type (TElement) for key Type if key Type implements IEnumerable{TElement} or
        /// null if the key Type does not implement IEnumerable{T} e.g.:
        /// List{Entity} - Entity
        /// Entity       - null
        /// </summary>
        private static readonly Dictionary<Type, Type> ienumerableElementTypeCache = new Dictionary<Type, Type>(EqualityComparer<Type>.Default);

        /// <summary> Aggregation method map to the URI equivalent</summary>
        private static Dictionary<AggregationMethod, string> aggregationMethodMap = new Dictionary<AggregationMethod, string>(EqualityComparer<AggregationMethod>.Default);

        /// <summary> VB Assembly name</summary>
        private const string VisualBasicAssemblyName = "Microsoft.VisualBasic,";

        /// <summary> VB Assembly public key token</summary>
#pragma warning disable 436
        private const string VisualBasicAssemblyPublicKeyToken = "PublicKeyToken=" + OfficialDesktopPublicKeyToken;
#pragma warning restore 436
        /// <summary>
        /// Initializes method map
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Cleaner code")]
        static TypeSystem()
        {
#if !PORTABLELIB
            const int ExpectedCount = 43;
#endif
            // string functions
            expressionMethodMap = new Dictionary<MethodInfo, string>(EqualityComparer<MethodInfo>.Default);
            expressionMethodMap.Add(typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), @"contains");
            expressionMethodMap.Add(typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }), @"endswith");
            expressionMethodMap.Add(typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), @"startswith");
            expressionMethodMap.Add(typeof(string).GetMethod("IndexOf", new Type[] { typeof(string) }), @"indexof");
            expressionMethodMap.Add(typeof(string).GetMethod("Replace", new Type[] { typeof(string), typeof(string) }), @"replace");
            expressionMethodMap.Add(typeof(string).GetMethod("Substring", new Type[] { typeof(int) }), @"substring");
            expressionMethodMap.Add(typeof(string).GetMethod("Substring", new Type[] { typeof(int), typeof(int) }), @"substring");
            expressionMethodMap.Add(typeof(string).GetMethod("ToLower", PlatformHelper.EmptyTypes), @"tolower");
            expressionMethodMap.Add(typeof(string).GetMethod("ToUpper", PlatformHelper.EmptyTypes), @"toupper");
            expressionMethodMap.Add(typeof(string).GetMethod("Trim", PlatformHelper.EmptyTypes), @"trim");
            expressionMethodMap.Add(typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) }), @"concat");
            expressionMethodMap.Add(typeof(string).GetProperty("Length", typeof(int)).GetGetMethod(), @"length");

            // date methods
            expressionMethodMap.Add(typeof(Date).GetProperty("Day", typeof(int)).GetGetMethod(), @"day");
            expressionMethodMap.Add(typeof(Date).GetProperty("Month", typeof(int)).GetGetMethod(), @"month");
            expressionMethodMap.Add(typeof(Date).GetProperty("Year", typeof(int)).GetGetMethod(), @"year");

            // timeOfDay methods
            expressionMethodMap.Add(typeof(TimeOfDay).GetProperty("Hours", typeof(int)).GetGetMethod(), @"hour");
            expressionMethodMap.Add(typeof(TimeOfDay).GetProperty("Minutes", typeof(int)).GetGetMethod(), @"minute");
            expressionMethodMap.Add(typeof(TimeOfDay).GetProperty("Seconds", typeof(int)).GetGetMethod(), @"second");

            // datetimeoffset methods
            expressionMethodMap.Add(typeof(DateTimeOffset).GetProperty("Date", typeof(DateTime)).GetGetMethod(), @"date");
            expressionMethodMap.Add(typeof(DateTimeOffset).GetProperty("Day", typeof(int)).GetGetMethod(), @"day");
            expressionMethodMap.Add(typeof(DateTimeOffset).GetProperty("Hour", typeof(int)).GetGetMethod(), @"hour");
            expressionMethodMap.Add(typeof(DateTimeOffset).GetProperty("Month", typeof(int)).GetGetMethod(), @"month");
            expressionMethodMap.Add(typeof(DateTimeOffset).GetProperty("Minute", typeof(int)).GetGetMethod(), @"minute");
            expressionMethodMap.Add(typeof(DateTimeOffset).GetProperty("Second", typeof(int)).GetGetMethod(), @"second");
            expressionMethodMap.Add(typeof(DateTimeOffset).GetProperty("Year", typeof(int)).GetGetMethod(), @"year");

            // datetime methods
            expressionMethodMap.Add(typeof(DateTime).GetProperty("Date", typeof(DateTime)).GetGetMethod(), @"date");
            expressionMethodMap.Add(typeof(DateTime).GetProperty("Day", typeof(int)).GetGetMethod(), @"day");
            expressionMethodMap.Add(typeof(DateTime).GetProperty("Hour", typeof(int)).GetGetMethod(), @"hour");
            expressionMethodMap.Add(typeof(DateTime).GetProperty("Month", typeof(int)).GetGetMethod(), @"month");
            expressionMethodMap.Add(typeof(DateTime).GetProperty("Minute", typeof(int)).GetGetMethod(), @"minute");
            expressionMethodMap.Add(typeof(DateTime).GetProperty("Second", typeof(int)).GetGetMethod(), @"second");
            expressionMethodMap.Add(typeof(DateTime).GetProperty("Year", typeof(int)).GetGetMethod(), @"year");

            // timespan methods
            expressionMethodMap.Add(typeof(TimeSpan).GetProperty("Hours", typeof(int)).GetGetMethod(), @"hour");
            expressionMethodMap.Add(typeof(TimeSpan).GetProperty("Minutes", typeof(int)).GetGetMethod(), @"minute");
            expressionMethodMap.Add(typeof(TimeSpan).GetProperty("Seconds", typeof(int)).GetGetMethod(), @"second");

            // math methods
            expressionMethodMap.Add(typeof(Math).GetMethod("Round", new Type[] { typeof(double) }), @"round");
            expressionMethodMap.Add(typeof(Math).GetMethod("Round", new Type[] { typeof(decimal) }), @"round");
            expressionMethodMap.Add(typeof(Math).GetMethod("Floor", new Type[] { typeof(double) }), @"floor");

            MethodInfo foundMethod = null;
            if (typeof(Math).TryGetMethod("Floor", new Type[] { typeof(decimal) }, out foundMethod))
            {
                expressionMethodMap.Add(foundMethod, @"floor");
            }

            expressionMethodMap.Add(typeof(Math).GetMethod("Ceiling", new Type[] { typeof(double) }), @"ceiling");

            if (typeof(Math).TryGetMethod("Ceiling", new Type[] { typeof(decimal) }, out foundMethod))
            {
                expressionMethodMap.Add(foundMethod, @"ceiling");
            }

            // Spatial methods
            expressionMethodMap.Add(typeof(GeographyOperationsExtensions).GetMethod("Distance", new Type[] { typeof(GeographyPoint), typeof(GeographyPoint) }, true /*isPublic*/, true /*isStatic*/), @"geo.distance");
            expressionMethodMap.Add(typeof(GeometryOperationsExtensions).GetMethod("Distance", new Type[] { typeof(GeometryPoint), typeof(GeometryPoint) }, true /*isPublic*/, true /*isStatic*/), @"geo.distance");

            // Portable Lib can be 35 or 33 depending on if its running on Silverlight or not, disabling in this case
#if !PORTABLELIB
            Debug.Assert(expressionMethodMap.Count == ExpectedCount, "expressionMethodMap.Count == ExpectedCount");
#endif
            // vb methods
            // lookup these by type name + method name
            expressionVBMethodMap = new Dictionary<string, string>(StringComparer.Ordinal);

            expressionVBMethodMap.Add("Microsoft.VisualBasic.Strings.Trim", @"trim");
            expressionVBMethodMap.Add("Microsoft.VisualBasic.Strings.Len", @"length");
            expressionVBMethodMap.Add("Microsoft.VisualBasic.Strings.Mid", @"substring");
            expressionVBMethodMap.Add("Microsoft.VisualBasic.Strings.UCase", @"toupper");
            expressionVBMethodMap.Add("Microsoft.VisualBasic.Strings.LCase", @"tolower");
            expressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Year", @"year");
            expressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Month", @"month");
            expressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Day", @"day");
            expressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Hour", @"hour");
            expressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Minute", @"minute");
            expressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Second", @"second");

            Debug.Assert(expressionVBMethodMap.Count == 11, "expressionVBMethodMap.Count == 11");

            propertiesAsMethodsMap = new Dictionary<PropertyInfo, MethodInfo>(EqualityComparer<PropertyInfo>.Default);
            propertiesAsMethodsMap.Add(
                typeof(string).GetProperty("Length", typeof(int)),
                typeof(string).GetProperty("Length", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(
                typeof(DateTimeOffset).GetProperty("Day", typeof(int)),
                typeof(DateTimeOffset).GetProperty("Day", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(
                typeof(DateTimeOffset).GetProperty("Date", typeof(DateTime)),
                typeof(DateTimeOffset).GetProperty("Date", typeof(DateTime)).GetGetMethod());
            propertiesAsMethodsMap.Add(
                typeof(DateTimeOffset).GetProperty("Hour", typeof(int)),
                typeof(DateTimeOffset).GetProperty("Hour", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(
                typeof(DateTimeOffset).GetProperty("Minute", typeof(int)),
                typeof(DateTimeOffset).GetProperty("Minute", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(
                typeof(DateTimeOffset).GetProperty("Second", typeof(int)),
                typeof(DateTimeOffset).GetProperty("Second", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(
                typeof(DateTimeOffset).GetProperty("Month", typeof(int)),
                typeof(DateTimeOffset).GetProperty("Month", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(
                typeof(DateTimeOffset).GetProperty("Year", typeof(int)),
                typeof(DateTimeOffset).GetProperty("Year", typeof(int)).GetGetMethod());

            propertiesAsMethodsMap.Add(
                typeof(DateTime).GetProperty("Day", typeof(int)),
                typeof(DateTime).GetProperty("Day", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(
                typeof(DateTime).GetProperty("Date", typeof(DateTime)),
                typeof(DateTime).GetProperty("Date", typeof(DateTime)).GetGetMethod());
            propertiesAsMethodsMap.Add(
                typeof(DateTime).GetProperty("Hour", typeof(int)),
                typeof(DateTime).GetProperty("Hour", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(
                typeof(DateTime).GetProperty("Minute", typeof(int)),
                typeof(DateTime).GetProperty("Minute", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(
                typeof(DateTime).GetProperty("Second", typeof(int)),
                typeof(DateTime).GetProperty("Second", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(
                typeof(DateTime).GetProperty("Month", typeof(int)),
                typeof(DateTime).GetProperty("Month", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(
                typeof(DateTime).GetProperty("Year", typeof(int)),
                typeof(DateTime).GetProperty("Year", typeof(int)).GetGetMethod());

            propertiesAsMethodsMap.Add(
                typeof(TimeSpan).GetProperty("Hours", typeof(int)),
                typeof(TimeSpan).GetProperty("Hours", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(
                typeof(TimeSpan).GetProperty("Minutes", typeof(int)),
                typeof(TimeSpan).GetProperty("Minutes", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(
                typeof(TimeSpan).GetProperty("Seconds", typeof(int)),
                typeof(TimeSpan).GetProperty("Seconds", typeof(int)).GetGetMethod());

            propertiesAsMethodsMap.Add(
                typeof(TimeOfDay).GetProperty("Hours", typeof(int)),
                typeof(TimeOfDay).GetProperty("Hours", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(
                typeof(TimeOfDay).GetProperty("Minutes", typeof(int)),
                typeof(TimeOfDay).GetProperty("Minutes", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(
                typeof(TimeOfDay).GetProperty("Seconds", typeof(int)),
                typeof(TimeOfDay).GetProperty("Seconds", typeof(int)).GetGetMethod());

            propertiesAsMethodsMap.Add(
                typeof(Date).GetProperty("Year", typeof(int)),
                typeof(Date).GetProperty("Year", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(
                typeof(Date).GetProperty("Month", typeof(int)),
                typeof(Date).GetProperty("Month", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(
                typeof(Date).GetProperty("Day", typeof(int)),
                typeof(Date).GetProperty("Day", typeof(int)).GetGetMethod());

            Debug.Assert(propertiesAsMethodsMap.Count == 24, "propertiesAsMethodsMap.Count == 24");

            aggregationMethodMap.Add(AggregationMethod.Sum, UriHelper.SUM);
            aggregationMethodMap.Add(AggregationMethod.Average, UriHelper.AVERAGE);
            aggregationMethodMap.Add(AggregationMethod.Min, UriHelper.MIN);
            aggregationMethodMap.Add(AggregationMethod.Max, UriHelper.MAX);
            aggregationMethodMap.Add(AggregationMethod.CountDistinct, UriHelper.COUNTDISTINCT);
            aggregationMethodMap.Add(AggregationMethod.VirtualPropertyCount, UriHelper.VIRTUALPROPERTYCOUNT);
        }

        /// <summary>
        /// Sees if method has URI equivalent
        /// </summary>
        /// <param name="mi">The method info</param>
        /// <param name="methodName">uri method name</param>
        /// <returns>true/ false</returns>
        internal static bool TryGetQueryOptionMethod(MethodInfo mi, out string methodName)
        {
            return (expressionMethodMap.TryGetValue(mi, out methodName) ||
                (IsVisualBasicAssembly(mi.DeclaringType.GetAssembly()) &&
                 expressionVBMethodMap.TryGetValue(mi.DeclaringType.FullName + "." + mi.Name, out methodName)));
        }

        /// <summary>
        /// Sees if property can be represented as method for translation to URI
        /// </summary>
        /// <param name="pi">The property info</param>
        /// <param name="mi">get method for property</param>
        /// <returns>true/ false</returns>
        internal static bool TryGetPropertyAsMethod(PropertyInfo pi, out MethodInfo mi)
        {
            return propertiesAsMethodsMap.TryGetValue(pi, out mi);
        }

        /// <summary>
        /// Gets the elementtype for a sequence
        /// </summary>
        /// <param name="seqType">The sequence type</param>
        /// <returns>The element type</returns>
        internal static Type GetElementType(Type seqType)
        {
            Type ienum = FindIEnumerable(seqType);
            if (ienum == null)
            {
                return seqType;
            }

            return ienum.GetGenericArguments()[0];
        }

        /// <summary>
        /// Determines whether a property is private
        /// </summary>
        /// <param name="pi">The PropertyInfo structure for the property</param>
        /// <returns>true/ false if property is private</returns>
        internal static bool IsPrivate(PropertyInfo pi)
        {
            MethodInfo mi = pi.GetGetMethod() ?? pi.GetSetMethod();
            if (mi != null)
            {
                return mi.IsPrivate;
            }

            return true;
        }

        /// <summary>
        /// Finds type that implements IEnumerable so can get element type
        /// </summary>
        /// <param name="seqType">The Type to check</param>
        /// <returns>returns the type which implements IEnumerable</returns>
        internal static Type FindIEnumerable(Type seqType)
        {
            // shortcircuit types that never implement IEnumerable (including value types as the only value types recognized by the client
            // are primitive types like decimal, GUID or DateTimeOffset)
            if (seqType == null || seqType == typeof(string) || seqType.IsPrimitive() || seqType.IsValueType() || Nullable.GetUnderlyingType(seqType) != null)
            {
                return null;
            }

            Type resultType;
            lock (ienumerableElementTypeCache)
            {
                if (!ienumerableElementTypeCache.TryGetValue(seqType, out resultType))
                {
                    resultType = FindIEnumerableForNonPrimitiveType(seqType);
                    ienumerableElementTypeCache.Add(seqType, resultType);
                }
            }

            return resultType;
        }

        /// <summary>
        /// See if aggregation method has URI equivalent
        /// </summary>
        /// <param name="aggregationMethod">The aggregation method.</param>
        /// <param name="uriEquivalent">The URI equivalent.</param>
        /// <returns>true if the aggregation method is mapped to its URI equivalent</returns>
        internal static bool TryGetUriEquivalent(AggregationMethod aggregationMethod, out string uriEquivalent)
        {
            return aggregationMethodMap.TryGetValue(aggregationMethod, out uriEquivalent);
        }

        /// <summary>Finds whether a non-primitive implements IEnumerable and returns element type if it does.</summary>
        /// <param name="seqType">Type to check.</param>
        /// <returns>Type of the element if the <paramref name="seqType"/> implements IEnumerable{T}. Otherwise null.</returns>
        private static Type FindIEnumerableForNonPrimitiveType(Type seqType)
        {
            Debug.Assert(seqType != null, "seqType != null");
            Debug.Assert(seqType != typeof(string) && !seqType.IsPrimitive() && !seqType.IsValueType() && Nullable.GetUnderlyingType(seqType) == null, "seqType must not be a primitive type (nullable or not)");

            if (seqType.IsArray)
            {
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            }

            if (seqType.IsGenericType())
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }

            IEnumerable<Type> ifaces = seqType.GetInterfaces();
            if (ifaces != null)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null)
                    {
                        return ienum;
                    }
                }
            }

            if (seqType.GetBaseType() != null && seqType.GetBaseType() != typeof(object))
            {
                return FindIEnumerable(seqType.GetBaseType());
            }

            return null;
        }

        /// <summary>
        /// Checks if the given assembly is the VisualBasic assembly.
        /// </summary>
        /// <param name="assembly">assembly to check.</param>
        /// <returns>true if the assembly is Microsoft.VisualBasic, otherwise returns false.root
        /// </returns>
        private static bool IsVisualBasicAssembly(Assembly assembly)
        {
            string assemblyFullName = assembly.FullName;
            if (assemblyFullName.Contains(VisualBasicAssemblyName) && assembly.FullName.Contains(VisualBasicAssemblyPublicKeyToken))
            {
                return true;
            }

            return false;
        }
    }
}
