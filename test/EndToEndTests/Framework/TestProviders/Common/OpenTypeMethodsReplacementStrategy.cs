//---------------------------------------------------------------------
// <copyright file="OpenTypeMethodsReplacementStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Service.Providers;
#if TESTPROVIDERS
    using Microsoft.Spatial;
#else
    using System.Spatial;
#endif
    using Microsoft.Test.OData.Framework.TestProviders.Contracts;

    /// <summary>
    /// Method replacement strategy for the late-bound methods used with open types
    /// </summary>
    public abstract class OpenTypeMethodsReplacementStrategy : StaticMethodReplacementStrategyBase, IComparer<object>
    {
        /// <summary>
        /// Initializes a new instance of the OpenTypeMethodsReplacementStrategy class
        /// </summary>
        /// <param name="queryProvider">The current query provider</param>
        protected OpenTypeMethodsReplacementStrategy(IDataServiceQueryProvider queryProvider)
        {
            this.QueryProvider = queryProvider;
        }

        /// <summary>
        /// Gets the open-type-methods static class's type
        /// </summary>
        protected internal override Type MethodDeclarationType
        {
            get { return typeof(OpenTypeMethods); }
        }

        /// <summary>
        /// Gets the current query provider
        /// </summary>
        protected IDataServiceQueryProvider QueryProvider { get; private set; }

        /// <summary>
        /// Tries to get a replacement for the method. Extends the replace-able methods to include untyped comparison
        /// </summary>
        /// <param name="toReplace">The method to replace</param>
        /// <param name="parameters">The parameters to the method</param>
        /// <param name="replaced">The replaced expression</param>
        /// <returns>True if a replacement was made, false otherwise</returns>
        public override bool TryGetReplacement(MethodInfo toReplace, IEnumerable<Expression> parameters, out Expression replaced)
        {
            ExceptionUtilities.CheckArgumentNotNull(toReplace, "toReplace");
            ExceptionUtilities.CheckArgumentNotNull(parameters, "parameters");

            if (base.TryGetReplacement(toReplace, parameters, out replaced))
            {
                return true;
            }
            else if (IsUntypedOrdering(toReplace))
            {
                // TODO: handle lazy boolean evaluation
                replaced = Expression.Call(typeof(Queryable), toReplace.Name, toReplace.GetGenericArguments(), parameters.Concat(new[] { Expression.Constant(this) }).ToArray());
                return true;
            }

            replaced = null;
            return false;
        }

        /// <summary>
        /// Compares the two objects
        /// </summary>
        /// <param name="x">The first object</param>
        /// <param name="y">The second object</param>
        /// <returns>The result of comparing the objects</returns>
        public virtual int Compare(object x, object y)
        {
            if (x == y)
            {
                return 0;
            }

            // null is 'less than' a non-null value
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else if (y == null)
            {
                return 1;
            }

            var firstArray = x as Array;
            var secondArray = y as Array;
            if (firstArray != null || secondArray != null)
            {
                return this.CompareArrays(firstArray, secondArray);
            }

            if (x.GetType() == y.GetType())
            {
                var comparable = x as IComparable;
                if (comparable != null)
                {
                    return comparable.CompareTo(y);
                }
            }

            // just compare them as doubles rather than trying to emulate real numeric type promotion
            var doubleX = System.Convert.ToDouble(x, CultureInfo.InvariantCulture);
            var doubleY = System.Convert.ToDouble(y, CultureInfo.InvariantCulture);
            return doubleX.CompareTo(doubleY);
        }

        /// <summary>
        /// Gets the value of an open property
        /// </summary>
        /// <param name="value">The instance to get the property value from</param>
        /// <param name="propertyName">The open property name</param>
        /// <returns>The open property's value</returns>
        public virtual object GetValue(object value, string propertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(value, "value");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");

            // for sub-values of a complex type inside an open property, GetValue will still be used
            // so we should check before calling GetOpenPropertyValue (which will throw on non-open types)
            ResourceType type = this.GetResourceType(value);
            ExceptionUtilities.CheckObjectNotNull(type, "GetValue called with object of unknown type. Type was '{0}'", value.GetType());
            if (type.IsOpenType)
            {
                return this.QueryProvider.GetOpenPropertyValue(value, propertyName);
            }

            ExceptionUtilities.Assert(type.ResourceTypeKind == ResourceTypeKind.ComplexType, "OpenTypeMethods.GetValue used on non-open, non-complex type '{0}'", type.FullName);
            ResourceProperty resourceProperty = type.GetAllPropertiesLazily().SingleOrDefault(p => p.Name == propertyName);
            ExceptionUtilities.CheckObjectNotNull(resourceProperty, "OpenTypeMethods.GetValue used on non-open type '{0}' with non-existent property '{1}'", type.FullName, propertyName);

            return this.QueryProvider.GetPropertyValue(value, resourceProperty);
        }

        /// <summary>
        /// Adds the two objects
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>The result of adding the objects</returns>
        public abstract object Add(object left, object right);

        /// <summary>
        /// Ands the two objects
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>The result of and-ing the objects</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "AndAlso", Justification = "Name should match product's method name")]
        public abstract object AndAlso(object left, object right);

        /// <summary>
        /// Gets the ceiling of the object
        /// </summary>
        /// <param name="value">The value to get the ceiling of</param>
        /// <returns>The ceiling of the object</returns>
        public abstract object Ceiling(object value);

        /// <summary>
        /// Concats the two objects
        /// </summary>
        /// <param name="first">The first object</param>
        /// <param name="second">The second object</param>
        /// <returns>The result of concatenating the objects</returns>
        public abstract object Concat(object first, object second);

        /// <summary>
        /// Gets the day of the object
        /// </summary>
        /// <param name="dateTimeOffset">The value to get the day of</param>
        /// <returns>The day of the object</returns>
        public abstract object Day(object dateTimeOffset);

        /// <summary>
        /// Divides the two objects
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>The result of dividing the objects</returns>
        public abstract object Divide(object left, object right);

        /// <summary>
        /// Returns whether the target object ends with the substring
        /// </summary>
        /// <param name="targetString">The target object</param>
        /// <param name="substring">The substring to check for</param>
        /// <returns>Whether it ends with the substring</returns>
        public abstract object EndsWith(object targetString, object substring);

        /// <summary>
        /// Returns whether the two objects are equal
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>Whether the two object are equal</returns>
        public abstract object Equal(object left, object right);

        /// <summary>
        /// Gets the floor of the object
        /// </summary>
        /// <param name="value">The value to get the floor of</param>
        /// <returns>The floor of the object</returns>
        public abstract object Floor(object value);

        /// <summary>
        /// Returns whether the left object is greater than the right object
        /// </summary>
        /// <param name="left">The left object</param>
        /// <param name="right">The right object</param>
        /// <returns>whether the left object is greater than the right object</returns>
        public abstract object GreaterThan(object left, object right);

        /// <summary>
        /// Returns whether the left object is greater than or equal to the right object
        /// </summary>
        /// <param name="left">The left object</param>
        /// <param name="right">The right object</param>
        /// <returns>whether the left object is greater than or equal to the right object</returns>
        public abstract object GreaterThanOrEqual(object left, object right);

        /// <summary>
        /// Gets the hour of the object
        /// </summary>
        /// <param name="dateTimeOffset">The value to get the hour of</param>
        /// <returns>The hour of the object</returns>
        public abstract object Hour(object dateTimeOffset);

        /// <summary>
        /// Returns the index where the substring first appears in the target
        /// </summary>
        /// <param name="targetString">The target object</param>
        /// <param name="substring">The substring to get the index of</param>
        /// <returns>the index where the substring first appears in the target</returns>
        public abstract object IndexOf(object targetString, object substring);

        /// <summary>
        /// Gets the length of the object
        /// </summary>
        /// <param name="value">The value to get the length of</param>
        /// <returns>The length of the object</returns>
        public abstract object Length(object value);

        /// <summary>
        /// Returns whether the left object is less than the right object
        /// </summary>
        /// <param name="left">The left object</param>
        /// <param name="right">The right object</param>
        /// <returns>whether the left object is less than the right object</returns>
        public abstract object LessThan(object left, object right);

        /// <summary>
        /// Returns whether the left object is less than or equal to the right object
        /// </summary>
        /// <param name="left">The left object</param>
        /// <param name="right">The right object</param>
        /// <returns>whether the left object is less than or equal to the right object</returns>
        public abstract object LessThanOrEqual(object left, object right);

        /// <summary>
        /// Gets the minute of the object
        /// </summary>
        /// <param name="dateTimeOffset">The value to get the minute of</param>
        /// <returns>The minute of the object</returns>
        public abstract object Minute(object dateTimeOffset);

        /// <summary>
        /// Modulos the two objects
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>The result of mod-ing the objects</returns>
        public abstract object Modulo(object left, object right);

        /// <summary>
        /// Gets the month of the object
        /// </summary>
        /// <param name="dateTimeOffset">The value to get the month of</param>
        /// <returns>The month of the object</returns>
        public abstract object Month(object dateTimeOffset);

        /// <summary>
        /// Multiplies the two objects
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>The result of multiplying the objects</returns>
        public abstract object Multiply(object left, object right);

        /// <summary>
        /// Gets the negation of the object
        /// </summary>
        /// <param name="value">The value to negate</param>
        /// <returns>The negated object</returns>
        public abstract object Negate(object value);

        /// <summary>
        /// Gets the boolean inverse of the object
        /// </summary>
        /// <param name="value">The value to get the boolean inverse of</param>
        /// <returns>The boolean inverse of the object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Not", Justification = "Name should match product's method name")]
        public abstract object Not(object value);

        /// <summary>
        /// Returns whether the two objects are not equal
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>Whether the two object are not equal</returns>
        public abstract object NotEqual(object left, object right);

        /// <summary>
        /// Elses the two objects
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>The result of else-ing the objects</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "OrElse", Justification = "Name should match product's method name")]
        public abstract object OrElse(object left, object right);

        /// <summary>
        /// Replaces instances of the given substring with the new string in the target
        /// </summary>
        /// <param name="targetString">The target object</param>
        /// <param name="substring">The substring to replace</param>
        /// <param name="newString">The object to replace it with</param>
        /// <returns>The object with replacements</returns>
        public abstract object Replace(object targetString, object substring, object newString);

        /// <summary>
        /// Gets the rounded version of the object
        /// </summary>
        /// <param name="value">The value to round</param>
        /// <returns>The rounded object</returns>
        public abstract object Round(object value);

        /// <summary>
        /// Gets the second of the object
        /// </summary>
        /// <param name="dateTimeOffset">The value to get the second of</param>
        /// <returns>The second of the object</returns>
        public abstract object Second(object dateTimeOffset);

        /// <summary>
        /// Returns whether the target object starts with the substring
        /// </summary>
        /// <param name="targetString">The target object</param>
        /// <param name="substring">The substring to check for</param>
        /// <returns>Whether it starts with the substring</returns>
        public abstract object StartsWith(object targetString, object substring);

        /// <summary>
        /// Gets the substring of the given object
        /// </summary>
        /// <param name="targetString">The object to get the substring of</param>
        /// <param name="startIndex">The start index</param>
        /// <returns>The substring of the object</returns>
        public abstract object Substring(object targetString, object startIndex);

        /// <summary>
        /// Gets the substring of the given object
        /// </summary>
        /// <param name="targetString">The object to get the substring of</param>
        /// <param name="startIndex">The start index</param>
        /// <param name="length">the length of the substring</param>
        /// <returns>The substring of the object</returns>
        public abstract object Substring(object targetString, object startIndex, object length);

        /// <summary>
        /// Returns whether the target object contains the substring
        /// </summary>
        /// <param name="targetString">The target object</param>
        /// <param name="substring">The substring to check for</param>
        /// <returns>Whether it contains with the substring</returns>
        public abstract object Contains(object targetString, object substring);

        /// <summary>
        /// Subtracts the two objects
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>The result of subtracting the objects</returns>
        public abstract object Subtract(object left, object right);

        /// <summary>
        /// Gets the lowercase version of the given object
        /// </summary>
        /// <param name="targetString">The object to lowercase</param>
        /// <returns>The lowercase version of the object</returns>
        public abstract object ToLower(object targetString);

        /// <summary>
        /// Gets the uppercase version of the given object
        /// </summary>
        /// <param name="targetString">The object to uppercase</param>
        /// <returns>The uppercase version of the object</returns>
        public abstract object ToUpper(object targetString);

        /// <summary>
        /// Trims the given object
        /// </summary>
        /// <param name="targetString">The object to trim</param>
        /// <returns>The trimmed object</returns>
        public abstract object Trim(object targetString);

        /// <summary>
        /// Gets the year of the object
        /// </summary>
        /// <param name="dateTimeOffset">The value to get the year of</param>
        /// <returns>The year of the object</returns>
        public abstract object Year(object dateTimeOffset);

        /// <summary>
        /// Converts the given object to the given type
        /// </summary>
        /// <param name="value">The object to convert</param>
        /// <param name="type">The type to convert to</param>
        /// <returns>The converted object</returns>
        public abstract object Convert(object value, ResourceType type);

        /// <summary>
        /// Returns whether the object is of the given type
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <param name="type">The type to check for</param>
        /// <returns>Whether the object is of the given type</returns>
        public abstract object TypeIs(object value, ResourceType type);

        /// <summary>
        /// Returns the distance between the operands
        /// </summary>
        /// <param name="operand1">The first operand</param>
        /// <param name="operand2">The second operand</param>
        /// <returns>The distance</returns>
        public virtual object Distance(object operand1, object operand2)
        {
            double? distance;
            if (this.TryInvokeDistance<Geography>(operand1, operand2, GeographyOperationsExtensions.Distance, out distance)
                || this.TryInvokeDistance<Geometry>(operand1, operand2, GeometryOperationsExtensions.Distance, out distance))
            {
                return distance;
            }
            
            return this.HandleNonSpatialDistance(operand1, operand2);
        }

        /// <summary>
        /// Returns whether the method needs replacing. Always true for open-type methods
        /// </summary>
        /// <param name="method">The method to consider replacing</param>
        /// <returns>Always true for this class</returns>
        protected internal override bool ShouldReplaceWithInstanceMethod(MethodInfo method)
        {
            return true;
        }

        /// <summary>
        /// Tries to invoke the distance callback if the arguments are actually spatial values
        /// </summary>
        /// <typeparam name="TSpatial">The type of the spatial.</typeparam>
        /// <param name="operand1">The first operand.</param>
        /// <param name="operand2">The second operand.</param>
        /// <param name="callback">The callback to compute distance if the arguments are spatial.</param>
        /// <param name="distance">The distance computed.</param>
        /// <returns>Whether or not the arguments were spatial.</returns>
        protected internal bool TryInvokeDistance<TSpatial>(object operand1, object operand2, Func<TSpatial, TSpatial, double?> callback, out double? distance) where TSpatial : class, ISpatial
        {
            var spatial1 = operand1 as TSpatial;
            var spatial2 = operand2 as TSpatial;

            if ((spatial1 == null && operand1 != null) || (spatial2 == null && operand2 != null))
            {
                distance = null;
                return false;
            }

            distance = callback(spatial1, spatial2);
            return true;
        }

        /// <summary>
        /// Helper method for converting an object to a generic type
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="thing">The object to convert</param>
        /// <returns>The converted object</returns>
        protected static T Convert<T>(object thing)
        {
            return (T)System.Convert.ChangeType(thing, typeof(T), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Tries to call an instance method.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="returnValue">The return value.</param>
        /// <returns>The result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Late bound method, can't tell what the type will be")]
        protected static bool TryCallInstanceMethod(object instance, string methodName, object[] arguments, out object returnValue)
        {
            returnValue = null;

            if (instance == null)
            {
                return true;
            }

            var type = instance.GetType();
            var method = type.GetMethod(methodName);
            if (method == null)
            {
                return false;
            }

            returnValue = method.Invoke(instance, arguments);
            return true;
        }

        /// <summary>
        /// Tries to get a property value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <returns>The result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Late bound method, can't tell what the type will be")]
        protected static bool TryGetPropertyValue(object instance, string propertyName, out object propertyValue)
        {
            propertyValue = null;

            if (instance == null)
            {
                return true;
            }

            var type = instance.GetType();
            var property = type.GetProperty(propertyName);
            if (property == null && type == typeof(TimeSpan))
            {
                // TimeSpan property names are slightly different for some reason
                property = type.GetProperty(propertyName + "s");
            }

            if (property == null)
            {
                return false;
            }

            propertyValue = property.GetValue(instance, null);
            return true;
        }

        /// <summary>
        /// Handles distance for non-spatial types or throws.
        /// </summary>
        /// <param name="operand1">The first operand.</param>
        /// <param name="operand2">The second operand.</param>
        /// <returns>The distance between the non-spatial values</returns>
        protected abstract object HandleNonSpatialDistance(object operand1, object operand2);

        /// <summary>
        /// Gets the resource type of the given instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>The type of the instance</returns>
        protected ResourceType GetResourceType(object instance)
        {
            ResourceType type = null;
            ProviderImplementationSettings.Override(
                s => s.EnforceMetadataCaching = false,
                () => type = this.QueryProvider.GetResourceType(instance));
            return type;
        }

        /// <summary>
        /// Attempts to get a metadata provider and retrieved the types that derive from the given type
        /// </summary>
        /// <param name="type">The type to get derived types for.</param>
        /// <returns>The derived types</returns>
        protected IEnumerable<ResourceType> GetDerivedTypes(ResourceType type)
        {
            var metadataProvider = this.QueryProvider as IDataServiceMetadataProvider;
            if (metadataProvider == null || !metadataProvider.HasDerivedTypes(type))
            {
                return Enumerable.Empty<ResourceType>();
            }

            IEnumerable<ResourceType> types = null;
            ProviderImplementationSettings.Override(
                s => s.EnforceMetadataCaching = false,
                () => types = metadataProvider.GetDerivedTypes(type));
            return types;
        }

        private static bool IsUntypedOrdering(MethodInfo method)
        {
            ExceptionUtilities.CheckArgumentNotNull(method, "method");
            return (method.Name.StartsWith("OrderBy", StringComparison.Ordinal) || method.Name.StartsWith("ThenBy", StringComparison.Ordinal)) && method.GetGenericArguments()[1] == typeof(object);
        }

        private int CompareArrays(Array firstArray, Array secondArray)
        {
            ExceptionUtilities.Assert(firstArray != null || secondArray != null, "Should only be called if at least one value is an array");
            if (firstArray == null)
            {
                return -1;
            }
            else if (secondArray == null)
            {
                return 1;
            }
            else
            {
                if (firstArray.Length < secondArray.Length)
                {
                    return -1;
                }
                else if (secondArray.Length < firstArray.Length)
                {
                    return 1;
                }
                else
                {
                    for (int i = 0; i < firstArray.Length; i++)
                    {
                        var result = this.Compare(firstArray.GetValue(i), secondArray.GetValue(i));
                        if (result != 0)
                        {
                            return result;
                        }
                    }

                    return 0;
                }
            }
        }
    }
}