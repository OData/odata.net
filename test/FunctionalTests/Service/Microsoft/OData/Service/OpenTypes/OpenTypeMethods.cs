//---------------------------------------------------------------------
// <copyright file="OpenTypeMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Service.Parsing;
    #endregion

    /// <summary>Use this class to perform late-bound operations on open properties.</summary>    
    public static class OpenTypeMethods
    {
        #region Reflection OpenType MethodInfos

        /// <summary>MethodInfo for Add.</summary>
        internal static readonly MethodInfo AddMethodInfo = typeof(OpenTypeMethods).GetMethod("Add", BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for AndAlso.</summary>
        internal static readonly MethodInfo AndAlsoMethodInfo = typeof(OpenTypeMethods).GetMethod("AndAlso", BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for Convert.</summary>
        internal static readonly MethodInfo ConvertMethodInfo = typeof(OpenTypeMethods).GetMethod("Convert", BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for Divide.</summary>
        internal static readonly MethodInfo DivideMethodInfo = typeof(OpenTypeMethods).GetMethod("Divide", BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for Equal.</summary>
        internal static readonly MethodInfo EqualMethodInfo = typeof(OpenTypeMethods).GetMethod("Equal", BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for GreaterThan.</summary>
        internal static readonly MethodInfo GreaterThanMethodInfo = typeof(OpenTypeMethods).GetMethod("GreaterThan", BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for GreaterThanOrEqual.</summary>
        internal static readonly MethodInfo GreaterThanOrEqualMethodInfo = typeof(OpenTypeMethods).GetMethod("GreaterThanOrEqual", BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for LessThan.</summary>
        internal static readonly MethodInfo LessThanMethodInfo = typeof(OpenTypeMethods).GetMethod("LessThan", BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for LessThanOrEqual.</summary>
        internal static readonly MethodInfo LessThanOrEqualMethodInfo = typeof(OpenTypeMethods).GetMethod("LessThanOrEqual", BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for Modulo.</summary>
        internal static readonly MethodInfo ModuloMethodInfo = typeof(OpenTypeMethods).GetMethod("Modulo", BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for Multiply.</summary>
        internal static readonly MethodInfo MultiplyMethodInfo = typeof(OpenTypeMethods).GetMethod("Multiply", BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for Negate.</summary>
        internal static readonly MethodInfo NegateMethodInfo = typeof(OpenTypeMethods).GetMethod("Negate", BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for Not.</summary>
        internal static readonly MethodInfo NotMethodInfo = typeof(OpenTypeMethods).GetMethod("Not", BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for NotEqual.</summary>
        internal static readonly MethodInfo NotEqualMethodInfo = typeof(OpenTypeMethods).GetMethod("NotEqual", BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for OrElse.</summary>
        internal static readonly MethodInfo OrElseMethodInfo = typeof(OpenTypeMethods).GetMethod("OrElse", BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for Subtract.</summary>
        internal static readonly MethodInfo SubtractMethodInfo = typeof(OpenTypeMethods).GetMethod("Subtract", BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for TypeIs.</summary>
        internal static readonly MethodInfo TypeIsMethodInfo = typeof(OpenTypeMethods).GetMethod("TypeIs", BindingFlags.Static | BindingFlags.Public);

        /// <summary>MethodInfo for object OpenTypeMethods.GetValue(this object value, string propertyName).</summary>
        internal static readonly MethodInfo GetValueOpenPropertyMethodInfo = typeof(OpenTypeMethods).GetMethod(
            "GetValue",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new Type[] { typeof(object), typeof(string) },
            null);

        /// <summary>MethodInfo for IEnumerable&lt;object&gt; OpenTypeMethods.GetCollectionValue(this object value, string propertyName).</summary>
        internal static readonly MethodInfo GetCollectionValueOpenPropertyMethodInfo = typeof(OpenTypeMethods).GetMethod(
            "GetCollectionValue",
            new Type[] { typeof(object), typeof(string) },
            true /*isPublic*/,
            true /*isStatic*/);

        #endregion Internal fields.

        #region Property Accessor

        /// <summary>Gets a value from the specified property of a specified object.</summary>
        /// <returns>The requested value; null if the value cannot be determined.</returns>
        /// <param name="value">Object from which to get the property value.</param>
        /// <param name="propertyName">Name of property from which to get the value.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object GetValue(object value, string propertyName)
        {
            throw new NotImplementedException();
        }

        /// <summary>Gets a named collection value from the specified object.</summary>
        /// <param name='value'>Object to get value from.</param>
        /// <param name='propertyName'>Name of property to get.</param>
        /// <returns>The requested collection value; null if not found.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static IEnumerable<object> GetCollectionValue(object value, string propertyName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region binary operators

        /// <summary>Adds two values.</summary>
        /// <returns>The result of the arithmetic operation.</returns>
        /// <param name="left">First value to add.</param>
        /// <param name="right">Second value to add.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Add(object left, object right)
        {
            throw new NotImplementedException();
        }

        /// <summary>Performs a logical and operation between two expressions.</summary>
        /// <returns>The result of the logical and operation.</returns>
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object AndAlso(object left, object right)
        {
            throw new NotImplementedException();
        }

        /// <summary>Divides two values.</summary>
        /// <returns>The divided value.</returns>
        /// <param name="left">The first value (dividend).</param>
        /// <param name="right">The second value (divisor).</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Divide(object left, object right)
        {
            throw new NotImplementedException();
        }

        /// <summary>Determines whether the specified objects are considered equal.</summary>
        /// <returns>true when both objects are equal; otherwise, false.</returns>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Equal(object left, object right)
        {
            throw new NotImplementedException();
        }

        /// <summary>Determines whether the value of one object is greater than another object.</summary>
        /// <returns>true if the value of the first object is greater than that of the second object; otherwise, false.</returns>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object GreaterThan(object left, object right)
        {
            throw new NotImplementedException();
        }

        /// <summary>Determines whether the value of one object is greater than or equal to another object.</summary>
        /// <returns>true when the value of the first object is greater than or equal to that of the second object; otherwise, false.</returns>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object GreaterThanOrEqual(object left, object right)
        {
            throw new NotImplementedException();
        }

        /// <summary>Determines whether the value of one object is less than another object.</summary>
        /// <returns>true if the value of the first object is less than that of the second object; otherwise, false.</returns>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object LessThan(object left, object right)
        {
            throw new NotImplementedException();
        }

        /// <summary>Determines whether the value of one object is less than or equal to another object.</summary>
        /// <returns>true if the value of the first object is less than or equal to that of the second object; otherwise, false.</returns>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object LessThanOrEqual(object left, object right)
        {
            throw new NotImplementedException();
        }

        /// <summary>Calculates the arithmetic remainder of dividing one value by a second value. </summary>
        /// <returns>The remainder value.</returns>
        /// <param name="left">The first value (dividend).</param>
        /// <param name="right">The second value (divisor).</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Modulo(object left, object right)
        {
            throw new NotImplementedException();
        }

        /// <summary>Multiplies two values.</summary>
        /// <returns>The product of the two values.</returns>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Multiply(object left, object right)
        {
            throw new NotImplementedException();
        }

        /// <summary>Performs a logical comparison of the two values to determine if they are not equal.</summary>
        /// <returns>true if both objects are not equal; otherwise, false.</returns>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object NotEqual(object left, object right)
        {
            throw new NotImplementedException();
        }

        /// <summary>Performs a logical OR operation on two values.</summary>
        /// <returns>The result of the logical OR operation.</returns>
        /// <param name="left">The first value.</param>
        /// <param name="right">The second value.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object OrElse(object left, object right)
        {
            throw new NotImplementedException();
        }

        /// <summary>Subtracts two values.</summary>
        /// <returns>The result of the arithmetic operation.</returns>
        /// <param name="left">First value in the subtraction.</param>
        /// <param name="right">Second value in the subtraction.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Subtract(object left, object right)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region unary operators

        /// <summary>Returns the result of multiplying the specified value by negative one.</summary>
        /// <returns>The product of <paramref name="value" /> multiplied by negative one.</returns>
        /// <param name="value">The value to negate.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Negate(object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>Performs a bitwise (logical) complement operation on the supplied value.</summary>
        /// <returns>A bitwise complement of the supplied value.</returns>
        /// <param name="value">Value to logically complement.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Not(object value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Type Conversions

        /// <summary>Converts a value to the specified type.</summary>
        /// <returns>The converted value.</returns>
        /// <param name="value">Value to convert.</param>
        /// <param name="type">Resource type for the conversion.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Convert(object value, ResourceType type)
        {
            throw new NotImplementedException();
        }

        /// <summary>Checks the type of a specified value.</summary>
        /// <returns>true if the value is of the specified resource type; otherwise, false.</returns>
        /// <param name="value">The value to check.</param>
        /// <param name="type">Resource type for which to check.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object TypeIs(object value, ResourceType type)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Canonical functions

        #region String functions

        /// <summary>Concatenates two string values.</summary>
        /// <returns>A new instance that is the concatenated string.</returns>
        /// <param name="first">The first string.</param>
        /// <param name="second">The second string.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Concat(object first, object second)
        {
            throw new NotImplementedException();
        }

        /// <summary>Determines whether the end of one string matches another string.</summary>
        /// <returns>true when <paramref name="targetString" /> ends with <paramref name="substring" />; otherwise, false.</returns>
        /// <param name="targetString">The string being compared.</param>
        /// <param name="substring">The string to compare to.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object EndsWith(object targetString, object substring)
        {
            throw new NotImplementedException();
        }

        /// <summary>Returns the index of a substring in the target string.</summary>
        /// <returns>The index of the location of <paramref name="substring" /> in the <paramref name="targetString" />.</returns>
        /// <param name="targetString">The target string.</param>
        /// <param name="substring">The substring to find.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object IndexOf(object targetString, object substring)
        {
            throw new NotImplementedException();
        }

        /// <summary>Gets the number of characters in the supplied string object. </summary>
        /// <returns>The length of the string value.</returns>
        /// <param name="value">The string to be checked.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Length(object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>Replaces one substring with a second substring in a target string.</summary>
        /// <returns>A new string with the substring replaced with the new substring.</returns>
        /// <param name="targetString">The string with the substring to replace.</param>
        /// <param name="substring">The substring to be replaced.</param>
        /// <param name="newString">The new substring.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Replace(object targetString, object substring, object newString)
        {
            throw new NotImplementedException();
        }

        /// <summary>Checks whether the target string starts with the substring.</summary>
        /// <returns>true if the target string starts with the given substring; otherwise, false.</returns>
        /// <param name="targetString">The string being compared.</param>
        /// <param name="substring">The substring that the <paramref name="targetString" /> might start with.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object StartsWith(object targetString, object substring)
        {
            throw new NotImplementedException();
        }

        /// <summary>Returns the substring after the specified starting index location.</summary>
        /// <returns>The substring.</returns>
        /// <param name="targetString">The string from which to return the substring.</param>
        /// <param name="startIndex">The starting index for the substring.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Substring(object targetString, object startIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>Returns the substring of a specific length after the specified starting index location.</summary>
        /// <returns>The substring.</returns>
        /// <param name="targetString">The string from which to return the substring.</param>
        /// <param name="startIndex">The starting index for the substring.</param>
        /// <param name="length">The length of the substring.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Substring(object targetString, object startIndex, object length)
        {
            throw new NotImplementedException();
        }

        /// <summary>Determines whether a substring occurs in another string.</summary>
        /// <returns>true if <paramref name="substring" /> occurs in <paramref name="targetString" />; otherwise, false.</returns>
        /// <param name="targetString">The string to search.</param>
        /// <param name="substring">The substring to locate.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Contains(object targetString, object substring)
        {
            throw new NotImplementedException();
        }

        /// <summary>Returns a copy of a string converted to lowercase.</summary>
        /// <returns>A new string value with only lowercase.</returns>
        /// <param name="targetString">The string to convert.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        [SuppressMessage("Microsoft.Globalization", "CA1308", Justification = "Need to support ToLower function")]
        public static object ToLower(object targetString)
        {
            throw new NotImplementedException();
        }

        /// <summary>Returns a copy of a string converted to uppercase.</summary>
        /// <returns>A new string value with only uppercase characters.</returns>
        /// <param name="targetString">The string to convert.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object ToUpper(object targetString)
        {
            throw new NotImplementedException();
        }

        /// <summary>Removes all leading and trailing white-space characters from a string.</summary>
        /// <returns>The trimmed string.</returns>
        /// <param name="targetString">The string to trim.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Trim(object targetString)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Datetime functions

        /// <summary>Returns the year value of the given <see cref="T:System.DateTime" /> instance.</summary>
        /// <returns>The year value of the given <see cref="T:System.DateTime" /> instance.</returns>
        /// <param name="dateTime">A <see cref="T:System.DateTime" /> object.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Year(object dateTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>Returns the month value of the given <see cref="T:System.DateTime" /> instance.</summary>
        /// <returns>The month value of the given <see cref="T:System.DateTime" /> instance.</returns>
        /// <param name="dateTime">A <see cref="T:System.DateTime" /> object.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Month(object dateTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>Returns the day value of the given <see cref="T:System.DateTime" /> instance.</summary>
        /// <returns>The day value of the given <see cref="T:System.DateTime" /> instance.</returns>
        /// <param name="dateTime">A <see cref="T:System.DateTime" /> object.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Day(object dateTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>Returns the hour value of the given <see cref="T:System.DateTime" /> instance.</summary>
        /// <returns>The hour value of the given <see cref="T:System.DateTime" /> instance.</returns>
        /// <param name="dateTime">A <see cref="T:System.DateTime" /> object.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Hour(object dateTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>Returns the minute value of the given <see cref="T:System.DateTime" /> instance.</summary>
        /// <returns>The minute value of the given <see cref="T:System.DateTime" /> instance.</returns>
        /// <param name="dateTime">A <see cref="T:System.DateTime" /> object.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Minute(object dateTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>Returns the second value of the given <see cref="T:System.DateTime" /> instance.</summary>
        /// <returns>The second value of the given <see cref="T:System.DateTime" /> instance.</returns>
        /// <param name="dateTime">A <see cref="T:System.DateTime" /> object.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Second(object dateTime)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Numeric functions

        /// <summary>Returns the ceiling of the given value.</summary>
        /// <returns>The ceiling value for the given value.</returns>
        /// <param name="value">A <see cref="T:System.Decimal" /> or <see cref="T:System.Double" /> object.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Ceiling(object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>Returns the floor of the given value.</summary>
        /// <returns>Returns the floor value for the given object.</returns>
        /// <param name="value">The <see cref="T:System.Decimal" /> or <see cref="T:System.Double" /> object to evaluate.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Floor(object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>Rounds the supplied value.</summary>
        /// <returns>The rounded value.</returns>
        /// <param name="value">A <see cref="T:System.Decimal" /> or <see cref="T:System.Double" /> to round.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Round(object value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Spatial

        /// <summary>Returns the distance between the specified objects.</summary>
        /// <returns>The distance between the specified objects.</returns>
        /// <param name="left">The first object.</param>
        /// <param name="right">The second object.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Distance(object left, object right)
        {
            throw new NotImplementedException();
        }

        /// <summary>Returns if pint and polygon will intersect.</summary>
        /// <returns>The distance between the specified objects.</returns>
        /// <param name="left">The first object, point.</param>
        /// <param name="right">The second object, polygon.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Parameters will be used in the actual impl")]
        public static object Intersects(object left, object right)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region Factory methods for expression tree nodes.

        /// <summary>Creates an expression that adds two values with no overflow checking.</summary>
        /// <param name='left'>Left value.</param><param name='right'>Right value.</param>
        /// <returns>The added value.</returns>
        internal static Expression AddExpression(Expression left, Expression right)
        {
            return Expression.Add(
                    ExpressionAsObject(left),
                    ExpressionAsObject(right),
                    AddMethodInfo);
        }

        /// <summary>Creates a call expression that represents a conditional AND operation that evaluates the second operand only if it has to.</summary>
        /// <param name='left'>Left value.</param><param name='right'>Right value.</param>
        /// <returns>The conditional expression; null if the expressions aren't of the right type.</returns>
        internal static Expression AndAlsoExpression(Expression left, Expression right)
        {
            return Expression.Call(
                    AndAlsoMethodInfo,
                    ExpressionAsObject(left),
                    ExpressionAsObject(right));
        }

        /// <summary>Creates an expression that divides two values.</summary>
        /// <param name='left'>Left value.</param><param name='right'>Right value.</param>
        /// <returns>The divided value.</returns>
        internal static Expression DivideExpression(Expression left, Expression right)
        {
            return Expression.Divide(
                    ExpressionAsObject(left),
                    ExpressionAsObject(right),
                    DivideMethodInfo);
        }

        /// <summary>Creates an expression that checks whether two values are equal.</summary>
        /// <param name='left'>Left value.</param><param name='right'>Right value.</param>
        /// <returns>true if left equals right; false otherwise.</returns>
        internal static Expression EqualExpression(Expression left, Expression right)
        {
            return Expression.Equal(
                    ExpressionAsObject(left),
                    ExpressionAsObject(right),
                    false,
                    EqualMethodInfo);
        }

        /// <summary>Creates an expression that checks whether the left value is greater than the right value.</summary>
        /// <param name='left'>Left value.</param><param name='right'>Right value.</param>
        /// <returns>true if left is greater than right; false otherwise.</returns>
        internal static Expression GreaterThanExpression(Expression left, Expression right)
        {
            return Expression.GreaterThan(
                    ExpressionAsObject(left),
                    ExpressionAsObject(right),
                    false,
                    GreaterThanMethodInfo);
        }

        /// <summary>Creates an expression that checks whether the left value is greater than or equal to the right value.</summary>
        /// <param name='left'>Left value.</param><param name='right'>Right value.</param>
        /// <returns>true if left is greater than or equal to right; false otherwise.</returns>
        internal static Expression GreaterThanOrEqualExpression(Expression left, Expression right)
        {
            return Expression.GreaterThanOrEqual(
                    ExpressionAsObject(left),
                    ExpressionAsObject(right),
                    false,
                    GreaterThanOrEqualMethodInfo);
        }

        /// <summary>Creates an expression that checks whether the left value is less than the right value.</summary>
        /// <param name='left'>Left value.</param><param name='right'>Right value.</param>
        /// <returns>true if left is less than right; false otherwise.</returns>
        internal static Expression LessThanExpression(Expression left, Expression right)
        {
            return Expression.LessThan(
                    ExpressionAsObject(left),
                    ExpressionAsObject(right),
                    false,
                    LessThanMethodInfo);
        }

        /// <summary>Creates an expression that checks whether the left value is less than or equal to the right value.</summary>
        /// <param name='left'>Left value.</param><param name='right'>Right value.</param>
        /// <returns>true if left is less than or equal to right; false otherwise.</returns>
        internal static Expression LessThanOrEqualExpression(Expression left, Expression right)
        {
            return Expression.LessThanOrEqual(
                    ExpressionAsObject(left),
                    ExpressionAsObject(right),
                    false,
                    LessThanOrEqualMethodInfo);
        }

        /// <summary>Creates an expression that calculates the remainder of dividing the left value by the right value.</summary>
        /// <param name='left'>Left value.</param><param name='right'>Right value.</param>
        /// <returns>The remainder value.</returns>
        internal static Expression ModuloExpression(Expression left, Expression right)
        {
            return Expression.Modulo(
                    ExpressionAsObject(left),
                    ExpressionAsObject(right),
                    ModuloMethodInfo);
        }

        /// <summary>Creates an expression that multiplies two values with no overflow checking.</summary>
        /// <param name='left'>Left value.</param><param name='right'>Right value.</param>
        /// <returns>The multiplication value.</returns>
        internal static Expression MultiplyExpression(Expression left, Expression right)
        {
            return Expression.Multiply(
                    ExpressionAsObject(left),
                    ExpressionAsObject(right),
                    MultiplyMethodInfo);
        }

        /// <summary>Creates a call expression that represents a conditional OR operation that evaluates the second operand only if it has to.</summary>
        /// <param name='left'>Left value.</param><param name='right'>Right value.</param>
        /// <returns>The conditional expression; null if the expressions aren't of the right type.</returns>
        internal static Expression OrElseExpression(Expression left, Expression right)
        {
            return Expression.Call(
                    OrElseMethodInfo,
                    ExpressionAsObject(left),
                    ExpressionAsObject(right));
        }

        /// <summary>Creates an expression that checks whether two values are not equal.</summary>
        /// <param name='left'>Left value.</param><param name='right'>Right value.</param>
        /// <returns>true if left is does not equal right; false otherwise.</returns>
        internal static Expression NotEqualExpression(Expression left, Expression right)
        {
            return Expression.NotEqual(
                    ExpressionAsObject(left),
                    ExpressionAsObject(right),
                    false,
                    NotEqualMethodInfo);
        }

        /// <summary>Creates an expression that subtracts the right value from the left value.</summary>
        /// <param name='left'>Left value.</param><param name='right'>Right value.</param>
        /// <returns>The subtraction value.</returns>
        internal static Expression SubtractExpression(Expression left, Expression right)
        {
            return Expression.Subtract(
                    ExpressionAsObject(left),
                    ExpressionAsObject(right),
                    SubtractMethodInfo);
        }

        /// <summary>Creates an expression that negates (arithmetically) the specified value.</summary>
        /// <param name='expression'>Value expression.</param>
        /// <returns>The negated value.</returns>
        internal static Expression NegateExpression(Expression expression)
        {
            return Expression.Negate(
                    ExpressionAsObject(expression),
                    NegateMethodInfo);
        }

        /// <summary>Creates an expression that negates (logically) the specified value.</summary>
        /// <param name='expression'>Value expression.</param>
        /// <returns>The negated value.</returns>
        internal static Expression NotExpression(Expression expression)
        {
            return Expression.Not(
                    ExpressionAsObject(expression),
                    NotMethodInfo);
        }

        #endregion Factory methods for expression tree nodes.

        #region Helper methods

        /// <summary>
        /// Checks whether the specified <paramref name="expression"/> is part of an open property expression.
        /// </summary>
        /// <param name="expression">Non-null <see cref="Expression"/> to check.</param>
        /// <returns>true if <paramref name="expression"/> is based on an open property; false otherwise.</returns>
        internal static bool IsOpenPropertyExpression(Expression expression)
        {
            Debug.Assert(expression != null, "expression != null");
            return expression != ExpressionUtils.NullLiteral && expression.Type == typeof(object) && IsOpenExpression(expression);
        }

        /// <summary>Checks if the given input expression refers to open types.</summary>
        /// <param name="input">Input expression.</param>
        /// <returns>true if the input is an open expression, false otherwise.</returns>
        internal static bool IsOpenExpression(Expression input)
        {
            Debug.Assert(input != null, "input != null");
            input = StripObjectConvert(input);

            switch (input.NodeType)
            {
                case ExpressionType.Call:
                    return ((MethodCallExpression)input).Method.DeclaringType == typeof(OpenTypeMethods);

                case ExpressionType.Negate:
                case ExpressionType.Not:
                    {
                        UnaryExpression unaryExpression = (UnaryExpression)input;
                        return unaryExpression.Method != null && unaryExpression.Method.DeclaringType == typeof(OpenTypeMethods);
                    }

                case ExpressionType.Add:
                case ExpressionType.AndAlso:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.NotEqual:
                case ExpressionType.OrElse:
                case ExpressionType.Subtract:
                    BinaryExpression binaryExpression = ((BinaryExpression)input);
                    return binaryExpression.Method != null && binaryExpression.Method.DeclaringType == typeof(OpenTypeMethods);

                case ExpressionType.Conditional:
                    // This handles that null propagation scenario.
                    return IsOpenExpression(((ConditionalExpression)input).IfFalse);

                case ExpressionType.Convert:
                case ExpressionType.Constant:
                case ExpressionType.MemberAccess:
                case ExpressionType.TypeIs:
                    return false;

                default:
                    Debug.Assert(false, "Unexpected expression type found.");
                    break;
            }

            return false;
        }

        /// <summary>Strips all Expression.Convert(object) calls from the input expression.</summary>
        /// <param name="input">Input expression.</param>
        /// <returns>First non-Convert expression inside Converts that converts to non-object type.</returns>
        private static Expression StripObjectConvert(Expression input)
        {
            Debug.Assert(input != null, "input != null");
            while (input.NodeType == ExpressionType.Convert && input.Type == typeof(object))
            {
                UnaryExpression inner = (UnaryExpression)input;
                input = inner.Operand;
            }

            return input;
        }

        /// <summary>
        /// Returns the specified <paramref name="expression"/> with a 
        /// type assignable to System.Object.
        /// </summary>
        /// <param name="expression">Expression to convert.</param>
        /// <returns>
        /// The specified <paramref name="expression"/> with a type assignable 
        /// to System.Object.
        /// </returns>
        private static Expression ExpressionAsObject(Expression expression)
        {
            Debug.Assert(expression != null, "expression != null");
            return expression.Type.IsValueType ? Expression.Convert(expression, typeof(object)) : expression;
        }

        #endregion
    }
}
