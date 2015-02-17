//---------------------------------------------------------------------
// <copyright file="DataServiceProviderMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Diagnostics.CodeAnalysis;
    #endregion

    /// <summary>Use this class to perform late-bound operations on data service resource sets.</summary>    
    public static class DataServiceProviderMethods
    {
        #region Internal MethodInfos

        /// <summary>MethodInfo for object DataServiceProviderMethods.GetValue(this object value, string propertyName).</summary>
        internal static readonly MethodInfo GetValueMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
            "GetValue",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { typeof(object), typeof(ResourceProperty) },
            null);

        /// <summary>MethodInfo for IEnumerable&lt;T&gt; DataServiceProviderMethods.GetSequenceValue(this object value, string propertyName).</summary>
        internal static readonly MethodInfo GetSequenceValueMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
            "GetSequenceValue",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { typeof(object), typeof(ResourceProperty) },
            null);

        /// <summary>MethodInfo for Convert.</summary>
        internal static readonly MethodInfo ConvertMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
            "Convert", 
            BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for TypeIs.</summary>
        internal static readonly MethodInfo TypeIsMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
            "TypeIs", 
            BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for OfType with IQueryable parameter.</summary>
        internal static readonly MethodInfo OfTypeIQueryableMethodInfo;

        /// <summary>MethodInfo for OfType with IEnumerable parameter.</summary>
        internal static readonly MethodInfo OfTypeIEnumerableMethodInfo;

        /// <summary>MethodInfo for TypeIs.</summary>
        internal static readonly MethodInfo TypeAsMethodInfo = typeof(DataServiceProviderMethods).GetMethod(
            "TypeAs",
            BindingFlags.Static | BindingFlags.Public);

        #endregion

        /// <summary>
        /// Static constructor for data service provider methods.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Avoid multiple reflection calls")]
        static DataServiceProviderMethods()
        {
            MethodInfo[] methods = (MethodInfo[])typeof(DataServiceProviderMethods).GetMember(
                "OfType",
                MemberTypes.Method,
                BindingFlags.Public | BindingFlags.Static);

            Debug.Assert(methods.Length == 2, "There must be exactly 2 ofType methods declared");
            ParameterInfo[] parameters = methods[0].GetParameters();
            Debug.Assert(parameters.Length == 2, "There must be exactly 2 parameters in both ofType methods");
            Debug.Assert(parameters[0].ParameterType.IsGenericType, "The first parameter to OfType method must be generic");

            if (parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>))
            {
                OfTypeIQueryableMethodInfo = methods[0];
                OfTypeIEnumerableMethodInfo = methods[1];
            }
            else
            {
                OfTypeIEnumerableMethodInfo = methods[0];
                OfTypeIQueryableMethodInfo = methods[1];
            }
        }

        #region GetValue, GetSequenceValue, OfType

        /// <summary>Gets a named value from the specified object.</summary>
        /// <returns>An object that is the requested value.</returns>
        /// <param name="value">Object that contains the value.</param>
        /// <param name="property"><see cref="T:Microsoft.OData.Service.Providers.ResourceProperty" /> that is the property the value of which must be returned.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object GetValue(object value, ResourceProperty property)
        {
            throw new NotImplementedException();
        }

        /// <summary>Gets a named value from the specified object as a sequence.</summary>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> instance that contains the requested value as a sequence.</returns>
        /// <param name="value">Object that contains the value.</param>
        /// <param name="property">
        ///   <see cref="T:Microsoft.OData.Service.Providers.ResourceProperty" /> that is the property the value of which must be returned.</param>
        /// <typeparam name="T">Type of the resulting sequence.</typeparam>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "need T for proper binding to collections")]
        public static IEnumerable<T> GetSequenceValue<T>(object value, ResourceProperty property)
        {
            throw new NotImplementedException();
        }

        /// <summary>Filters the supplied query based on the specified <see cref="T:Microsoft.OData.Service.Providers.ResourceType" />.</summary>
        /// <returns>Returns an <see cref="T:System.Linq.IQueryable`1" /> instance filtered by the supplied <paramref name="resourceType" />.</returns>
        /// <param name="query">The <see cref="T:System.Linq.IQueryable`1" /> instance to be filtered.</param>
        /// <param name="resourceType">
        ///   <see cref="T:Microsoft.OData.Service.Providers.ResourceType" /> used to filter the query.</param>
        /// <typeparam name="TSource">Type of the <see cref="T:System.Linq.IQueryable`1" /> instance supplied as the <paramref name="query" /> parameter.</typeparam>
        /// <typeparam name="TResult">Type representing the resource type supplied as the <paramref name="resourceType" /> parameter.</typeparam>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "need TSource and TResult for proper binding type binding")]
        public static IQueryable<TResult> OfType<TSource, TResult>(IQueryable<TSource> query, ResourceType resourceType)
        {
            throw new NotImplementedException();
        }

        /// <summary>Filters the supplied query based on the specified <see cref="T:Microsoft.OData.Service.Providers.ResourceType" />.</summary>
        /// <returns>Returns an <see cref="T:System.Linq.IQueryable`1" /> instance filtered by the supplied <paramref name="resourceType" />.</returns>
        /// <param name="query">The <see cref="T:System.Linq.IQueryable`1" /> instance to be filtered.</param>
        /// <param name="resourceType">
        ///   <see cref="T:Microsoft.OData.Service.Providers.ResourceType" /> used to filter the query.</param>
        /// <typeparam name="TSource">Type of the <see cref="T:System.Linq.IQueryable`1" /> instance supplied as the <paramref name="query" /> parameter.</typeparam>
        /// <typeparam name="TResult">Type representing the resource type supplied as the <paramref name="resourceType" /> parameter.</typeparam>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "need TSource and TResult for proper binding type binding")]
        public static IEnumerable<TResult> OfType<TSource, TResult>(IEnumerable<TSource> query, ResourceType resourceType)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Type Conversions

        /// <summary>Casts a value to a specified type.</summary>
        /// <returns>The <paramref name="value" /> cast to the requested <paramref name="type" />.</returns>
        /// <param name="value">The value to cast to the requested type.</param>
        /// <param name="type">Resource type for which to check.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Convert(object value, ResourceType type)
        {
            throw new NotImplementedException();
        }

        /// <summary>Determines if the value is of a specified type.</summary>
        /// <returns>True if the value is of the specified type; otherwise false.</returns>
        /// <param name="value">The value to check.</param>
        /// <param name="type"><see cref="T:Microsoft.OData.Service.Providers.ResourceType" /> to compare with.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static bool TypeIs(object value, ResourceType type)
        {
            throw new NotImplementedException();
        }

        /// <summary>Checks whether the given type is assignable from the resource type of a supplied object instance.</summary>
        /// <returns>Returns a null value when the <paramref name="value" /> is not of the specified <paramref name="type" />; otherwise returns the supplied <paramref name="value" />.</returns>
        /// <param name="value">The object instance to check.</param>
        /// <param name="type">The <see cref="T:Microsoft.OData.Service.Providers.ResourceType" /> against which to check for assignability.</param>
        /// <typeparam name="T">Type of the <paramref name="value" />.</typeparam>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static T TypeAs<T>(object value, ResourceType type)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Type Comparers

        /// <summary>Returns comparison information for string parameters in an operation expression.</summary>
        /// <returns>Value Condition -1 <paramref name="left" /> is less than <paramref name="right" />. 0 x equals y. 1 <paramref name="left" /> is greater than <paramref name="right" />.</returns>
        /// <param name="left">The first parameter value.</param>
        /// <param name="right">The second parameter value.</param>
        /// <remarks>
        /// Do not change the name of this function because LINQ to SQL is sensitive about the
        /// method name, so is EF probably.
        /// </remarks>
        public static int Compare(String left, String right)
        {
            return Comparer<string>.Default.Compare(left, right);
        }

        /// <summary>Returns comparison information for Boolean parameters in an operation expression.</summary>
        /// <returns>Value Condition -1 <paramref name="left" /> is less than <paramref name="right" />. 0 x equals y. 1 <paramref name="left" /> is greater than <paramref name="right" />.</returns>
        /// <param name="left">The first parameter value.</param>
        /// <param name="right">The second parameter value.</param>
        /// <remarks>
        /// Do not change the name of this function because LINQ to SQL is sensitive about the
        /// method name, so is EF probably.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "Need implementation")]
        public static int Compare(bool left, bool right)
        {
            return Comparer<bool>.Default.Compare(left, right);
        }

        /// <summary>Returns comparison information for nullable Boolean parameters in an operation expression.</summary>
        /// <returns>Value Condition -1 <paramref name="left" /> is less than <paramref name="right" />. 0 x equals y. 1 <paramref name="left" /> is greater than <paramref name="right" />.</returns>
        /// <param name="left">The first parameter value.</param>
        /// <param name="right">The second parameter value.</param>
        /// <remarks>
        /// Do not change the name of this function because LINQ to SQL is sensitive about the
        /// method name, so is EF probably.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "Need implementation")]
        public static int Compare(bool? left, bool? right)
        {
            return Comparer<bool?>.Default.Compare(left, right);
        }

        /// <summary>Returns comparison information for GUID parameters in an operation expression.</summary>
        /// <returns>Value Condition -1 <paramref name="left" /> is less than <paramref name="right" />. 0 x equals y. 1 <paramref name="left" /> is greater than <paramref name="right" />.</returns>
        /// <param name="left">The first parameter value.</param>
        /// <param name="right">The second parameter value.</param>
        /// <remarks>
        /// Do not change the name of this function because LINQ to SQL is sensitive about the
        /// method name, so is EF probably.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "Need implementation")]
        public static int Compare(Guid left, Guid right)
        {
            return Comparer<Guid>.Default.Compare(left, right);
        }

        /// <summary>Returns comparison information for nullable GUID parameters in an operation expression.</summary>
        /// <returns>Value Condition -1 <paramref name="left" /> is less than <paramref name="right" />. 0 x equals y. 1 <paramref name="left" /> is greater than <paramref name="right" />.</returns>
        /// <param name="left">The first parameter value.</param>
        /// <param name="right">The second parameter value.</param>
        /// <remarks>
        /// Do not change the name of this function because LINQ to SQL is sensitive about the
        /// method name, so is EF probably.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "Need implementation")]
        public static int Compare(Guid? left, Guid? right)
        {
            return Comparer<Guid?>.Default.Compare(left, right);
        }

        /// <summary>Compares two byte arrays for equality.</summary>
        /// <returns>Returns a <see cref="T:System.Boolean" /> that is true when the arrays are equal; otherwise false.</returns>
        /// <param name="left">First byte array.</param>
        /// <param name="right">Second byte array.</param>
        public static bool AreByteArraysEqual(byte[] left, byte[] right)
        {
            if (left == right)
            {
                return true;
            }

            if (left == null || right == null)
            {
                return false;
            }

            if (left.Length != right.Length)
            {
                return false;
            }

            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] != right[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Compares two byte arrays for equality.</summary>
        /// <returns>Returns a <see cref="T:System.Boolean" /> that is true when the arrays are not equal; otherwise false.</returns>
        /// <param name="left">First byte array.</param>
        /// <param name="right">Second byte array.</param>
        public static bool AreByteArraysNotEqual(byte[] left, byte[] right)
        {
            return !AreByteArraysEqual(left, right);
        }
        #endregion
    }
}
