//---------------------------------------------------------------------
// <copyright file="SearchHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    /// <summary>
    /// The class helps build $search node to .NET expression.
    /// </summary>
    public sealed class SearchHelper
    {
        // All searchable primitive .NET value types. The collection does not include primitive .NET reference types.
        private static readonly IEnumerable<Type> s_primitiveValueTypes = new Type[]
        {
            typeof(Boolean),
            typeof(Byte),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(Decimal),
            typeof(Double),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(Int16),
            typeof(Int32),
            typeof(Int64),
            typeof(SByte),
            typeof(Single),
        };
        private readonly string searchText;
        private readonly Expression instance;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="searchText">The text to search.</param>
        /// <param name="instance">The search source.</param>
        public SearchHelper(string searchText, Expression instance)
        {
            this.searchText = searchText;
            this.instance = instance;
        }

        /// <summary>
        /// Builds search expression.
        /// </summary>
        /// <returns>The return type of the expression is boolean. It also has a parameter which represents the search source.</returns>
        public Expression Build()
        {
            // the returned expression is in the format Expression<Func<T, bool>>. You could invoke it via
            //   Func<Person, bool> function = Expression.Lambda<Func<Person, bool>>(searchExpression).Compile();
            //   bool result = function(new Person());
            return Expression.Call(
                Expression.Constant(this),
                typeof(SearchHelper).GetMethod("Visit", BindingFlags.Instance | BindingFlags.NonPublic),
                Expression.Constant(this.instance.Type, typeof(Type)),
                this.instance);
        }

        /// <summary>
        /// The method dispatches the search field to different methods based on the type of the field.
        /// </summary>
        /// <param name="type">The type of the field.</param>
        /// <param name="value">The value of the search field.</param>
        /// <returns>Returns true if found, otherwise false.</returns>
        private bool Visit(Type type, object value)
        {
            if (IsValueType(type))
            {
                return OnVisitValue(type, value);
            }

            if (IsStringType(type))
            {
                return OnVisitString(type, value);
            }

            if (IsCollectionType(type))
            {
                return OnVisitCollection(type, value);
            }

            return OnVisitComplex(type, value);
        }

        /// <summary>
        /// Called when the type of the search field is a .NET value type.
        /// </summary>
        /// <param name="type">The type of the search field.</param>
        /// <param name="value">The value of the search field.</param>
        /// <returns>Returns true if found, otherwise false.</returns>
        private bool OnVisitValue(Type type, object value)
        {
            return OnVisitString(type, value);
        }

        /// <summary>
        /// Called when the type of the search field is a string.
        /// </summary>
        /// <param name="type">The type of the search field.</param>
        /// <param name="value">The value of the search field.</param>
        /// <returns>Returns true if found, otherwise false.</returns>
        private bool OnVisitString(Type type, object value)
        {
            return value != null && value.ToString().Contains(this.searchText);
        }

        /// <summary>
        /// Called when the type of the search field is a collection.
        /// </summary>
        /// <param name="type">The type of the search field.</param>
        /// <param name="value">The value of the search field.</param>
        /// <returns>Returns true if found, otherwise false.</returns>
        private bool OnVisitCollection(Type type, object value)
        {
            if (value != null)
            {
                foreach (var item in (IEnumerable)value)
                {
                    // ignores null value
                    if (item == null) continue;

                    var itemType = item.GetType();

                    if (Visit(itemType, item))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Called when the type of the search field is a complex type.
        /// </summary>
        /// <param name="type">The type of the search field.</param>
        /// <param name="value">The value of the search field.</param>
        /// <returns>Returns true if found, otherwise false.</returns>
        private bool OnVisitComplex(Type type, object value)
        {
            if (value != null)
            {
                var properties = GetSearchProperties(type);

                foreach (var property in properties)
                {
                    var val = property.GetValue(value, null);

                    // ignores null value
                    if (val == null) continue;

                    if (Visit(property.PropertyType, val))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Whether the type of the field is a .NET value type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>Returns true if yes, otherwise false.</returns>
        private static bool IsValueType(Type type)
        {
            if (s_primitiveValueTypes.Any(t => t == type) || type.IsEnum)
            {
                return true;
            }

            // check Nullable<> types, e.g. int? or Nullable<int>
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var argu = type.GetGenericArguments()[0];

                if (s_primitiveValueTypes.Any(t => t == argu) || argu.IsEnum)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Whether the type of the field is a string.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>Returns true if yes, otherwise false.</returns>
        private static bool IsStringType(Type type)
        {
            return type == typeof(string);
        }

        /// <summary>
        /// Whether the type of the field is a collection.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>Returns true if yes, otherwise false.</returns>
        private static bool IsCollectionType(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        /// <summary>
        /// Gets all fields that searchable.
        /// </summary>
        /// <param name="type">The target type that to get.</param>
        /// <returns>All searchable fields.</returns>
        private static IEnumerable<PropertyInfo> GetSearchProperties(Type type)
        {
            // the best way to filter out the searchable fields should match the following steps
            // 1. get all non navigation fields
            // 2. get the properties which has SearchFieldAttribute applied from the previous result set
            // but currently we omitted the first step, so make sure you DO NOT add SearchFieldAttribute on the navigation fields.
            return type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public)
                       .Where(property => property.IsDefined(typeof(SearchFieldAttribute), true));
        }
    }
}