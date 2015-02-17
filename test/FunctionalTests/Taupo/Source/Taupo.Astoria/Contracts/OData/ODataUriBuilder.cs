//---------------------------------------------------------------------
// <copyright file="ODataUriBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// A collection of helper methods for building OData uris
    /// </summary>
    public static class ODataUriBuilder
    {
        /// <summary>
        /// Constructs a segment for a property with the given name on the given type, or an unknown segment if one cannot be found
        /// </summary>
        /// <param name="type">The metadata for the type</param>
        /// <param name="propertyName">The property name</param>
        /// <returns>A property, navigation, named stream, or unknown segment</returns>
        public static ODataUriSegment Property(StructuralType type, string propertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(propertyName, "propertyName");

            MemberProperty property;
            var entityType = type as EntityType;
            if (entityType != null)
            {
                var navigation = entityType.AllNavigationProperties.SingleOrDefault(p => p.Name == propertyName);
                if (navigation != null)
                {
                    return new NavigationSegment(navigation);
                }

                property = entityType.AllProperties.SingleOrDefault(p => p.Name == propertyName);
            }
            else
            {
                var complexType = type as ComplexType;
                ExceptionUtilities.CheckObjectNotNull(complexType, "Structural type was neither an entity type nor a complex type");
                property = complexType.Properties.SingleOrDefault(p => p.Name == propertyName);
            }

            if (property != null)
            {
                return new PropertySegment(property);
            }

            return new UnrecognizedSegment(propertyName);
        }

        /// <summary>
        /// Constructs a named stream segment for a property with the given name on the given type
        /// </summary>
        /// <param name="type">The metadata for the type</param>
        /// <param name="propertyName">The property name</param>
        /// <returns>A named streamsegment</returns>
        public static ODataUriSegment NamedStream(StructuralType type, string propertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(propertyName, "propertyName");

            return new NamedStreamSegment(propertyName);
        }

        /// <summary>
        /// Constructs a key expression segment with the given values
        /// </summary>
        /// <param name="type">The type for the key</param>
        /// <param name="values">The values for the key</param>
        /// <returns>A key expression segment</returns>
        public static ODataUriSegment Key(EntityType type, IEnumerable<NamedValue> values)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            ExceptionUtilities.CheckCollectionNotEmpty(values, "values");

            // intentionally avoiding using a linq-to-objects query as it introduces a compiler-generated class that shows up in the dependency layering output
            List<KeyValuePair<MemberProperty, object>> pairs = new List<KeyValuePair<MemberProperty, object>>();
            Dictionary<string, MemberProperty> keyProperties = new Dictionary<string, MemberProperty>();
            foreach (var keyProperty in type.AllKeyProperties)
            {
                keyProperties[keyProperty.Name] = keyProperty;
            }

            foreach (var pair in values)
            {
                MemberProperty keyProperty;
                ExceptionUtilities.Assert(keyProperties.TryGetValue(pair.Name, out keyProperty), "Could not find key property '" + pair.Name + "'");
                pairs.Add(new KeyValuePair<MemberProperty, object>(keyProperty, pair.Value));
            }

            ExceptionUtilities.Assert(pairs.Count == values.Count(), "Number of pairs does not match input");
   
            return new KeyExpressionSegment(pairs);
        }

        /// <summary>
        /// Constructs an entity set segment
        /// </summary>
        /// <param name="set">The entity set</param>
        /// <returns>An entity set segment</returns>
        public static ODataUriSegment EntitySet(EntitySet set)
        {
            ExceptionUtilities.CheckArgumentNotNull(set, "set");
            return new EntitySetSegment(set);
        }

        /// <summary>
        /// Constructs an entity set segment
        /// </summary>
        /// <param name="set">The entity set</param>
        /// <returns>An entity set segment</returns>
        public static ODataUriSegment EntitySet(IEdmEntitySet set)
        {
            ExceptionUtilities.CheckArgumentNotNull(set, "set");
            return new EntitySetSegment(set);
        }

        /// <summary>
        /// Constructs a type filtering expression segment with the given type
        /// </summary>
        /// <param name="type">The type used to filter the set by</param>
        /// <returns>An entity type segment</returns>
        public static ODataUriSegment EntityType(EntityType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            return new EntityTypeSegment(type);
        }

        /// <summary>
        /// Constructs a segment representing the root of a service
        /// </summary>
        /// <param name="uri">The service root</param>
        /// <returns>A service root segment</returns>
        public static ODataUriSegment Root(Uri uri)
        {
            ExceptionUtilities.CheckArgumentNotNull(uri, "uri");
            return new ServiceRootSegment(uri);
        }

        /// <summary>
        /// Constructs service operation segment
        /// </summary>
        /// <param name="function">The function representing the service operation</param>
        /// <returns>A service operations segment</returns>
        public static ODataUriSegment ServiceOperation(Function function)
        {
            return ServiceOperation(function, null, false);
        }

        /// <summary>
        /// Constructs service operation segment
        /// </summary>
        /// <param name="function">The function representing the service operation</param>
        /// <param name="useParentheses">used to determine if the uri generated will have parentheses</param>
        /// <returns>A service operations segment</returns>
        public static ODataUriSegment ServiceOperation(Function function, bool useParentheses)
        {
            return ServiceOperation(function, null, useParentheses);
        }

        /// <summary>
        /// Constructs service operation segment
        /// </summary>
        /// <param name="function">The function representing the service operation</param>
        /// <param name="container">The container for the function</param>
        /// <returns>A service operations segment</returns>
        public static ODataUriSegment ServiceOperation(Function function, EntityContainer container)
        {
            return ServiceOperation(function, container, false);
        }

        /// <summary>
        /// Constructs service operation segment
        /// </summary>
        /// <param name="function">The function representing the service operation</param>
        /// <param name="container">The container for the function</param>
        /// <param name="useParentheses">used to determine if the uri generated will have parentheses</param>
        /// <returns>A service operations segment</returns>
        public static ODataUriSegment ServiceOperation(Function function, EntityContainer container, bool useParentheses)
        {
            ExceptionUtilities.CheckArgumentNotNull(function, "function");
            return new FunctionSegment(function, container, useParentheses);
        }

        /// <summary>
        /// Constructs an unrecognized/raw-string segment
        /// </summary>
        /// <param name="value">The value of the segment</param>
        /// <returns>An unrecognized segment</returns>
        public static ODataUriSegment Unrecognized(string value)
        {
            return new UnrecognizedSegment(value);
        }
    }
}
