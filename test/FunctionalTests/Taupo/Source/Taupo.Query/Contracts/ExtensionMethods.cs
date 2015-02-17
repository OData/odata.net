//---------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Utilities;

    /// <summary>
    /// Extension methods that improve the experience of writing query tests.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Is the query clr type nullable or not
        /// </summary>
        /// <param name="queryClrType">Query Clr Type to test for condition</param>
        /// <returns>true if its nullable</returns>
        public static bool IsNullable(this IQueryClrType queryClrType)
        {
            ExceptionUtilities.CheckArgumentNotNull(queryClrType, "queryClrType");
            if (queryClrType.ClrType.IsValueType() && Nullable.GetUnderlyingType(queryClrType.ClrType) == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns an enumeration of the type's ancestor types and itself
        /// </summary>
        /// <param name="type">The type to get base types of</param>
        /// <returns>An enumeration containing the base types and the given type</returns>
        public static IEnumerable<QueryStructuralType> GetBaseTypesAndSelf(this QueryStructuralType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            var list = new List<QueryStructuralType> { type };

            while (type.Parent != null)
            {
                list.Insert(0, type.Parent);
                type = type.Parent;
            }

            return list.AsEnumerable();
        }

        /// <summary>
        /// Creates a key structural value for a QueryEntityValue
        /// </summary>
        /// <param name="queryEntityType">Query Entity Type that has key metadata to extract key information</param>
        /// <param name="entityInstance">Entity Instance object value that contains the key</param>
        /// <returns>A Query Key Structural Value</returns>
        public static QueryKeyStructuralValue GetEntityInstanceKey(this QueryEntityType queryEntityType, object entityInstance)
        {
            ExceptionUtilities.CheckArgumentNotNull(queryEntityType, "queryEntityType");
            ExceptionUtilities.CheckArgumentNotNull(entityInstance, "entityInstance");
            ExceptionUtilities.Assert(queryEntityType.ClrType.IsAssignableFrom(entityInstance.GetType()), "entityInstance of type '{0}' is not assignable to queryEntityType '{1}'", entityInstance.GetType(), queryEntityType.EntityType.FullName);

            QueryEntityValue keyValueContainer = new QueryEntityValue(queryEntityType, false, null, queryEntityType.EvaluationStrategy);
            foreach (QueryProperty keyProperty in queryEntityType.Properties.Where(m => m.IsPrimaryKey))
            {
                PropertyInfo pi = entityInstance.GetType().GetProperty(keyProperty.Name);
                keyValueContainer.SetPrimitiveValue(keyProperty.Name, pi.GetValue(entityInstance, null));
            }

            return new QueryKeyStructuralValue(keyValueContainer);
        }

        /// <summary>
        /// Creates a key structural value for a QueryEntityValue
        /// </summary>
        /// <param name="queryEntityValue">Query EntityValue to create the key from</param>
        /// <returns>A Query Key Structural Value</returns>
        public static QueryKeyStructuralValue Key(this QueryEntityValue queryEntityValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(queryEntityValue, "queryEntityValue");
            ExceptionUtilities.Assert(!queryEntityValue.IsNull, "queryEntityValue cannot be null");

            return new QueryKeyStructuralValue(queryEntityValue);
        }

        /// <summary>
        /// Gets a Debug entity instance comma delimited string 
        /// </summary>
        /// <param name="queryKeyStructuralValue">QueryKey Structural Value</param>
        /// <returns>Comma Delimited list of key values</returns>
        public static string GetDebugKeyString(this QueryKeyStructuralValue queryKeyStructuralValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(queryKeyStructuralValue, "queryKeyStructuralValue");
            List<QueryProperty> keyProperties = queryKeyStructuralValue.Type.Properties.Where(p => p.IsPrimaryKey).ToList();
            return string.Join(", ", keyProperties.Select(kp => ToStringConverter.ConvertPrimitiveValueToString(queryKeyStructuralValue.GetScalarValue(kp.Name).Value)));
        }

        /// <summary>
        /// Returns expressions which type is a collection of a given QueryType.
        /// </summary>
        /// <typeparam name="TElement">Type of the element in the collection.</typeparam>
        /// <param name="expressions">List of expressions to filter.</param>
        /// <returns>Filtered list of expressions.</returns>
        public static IEnumerable<TypedQueryExpression<QueryCollectionType<TElement>>> Collections<TElement>(this IEnumerable<QueryExpression> expressions)
            where TElement : QueryType
        {
            return expressions
                .Where(c => c.ExpressionType is QueryCollectionType<TElement>)
                .Select(c => new TypedQueryExpression<QueryCollectionType<TElement>>(c));
        }

        // TODO: Rename Extension method Primitive() to Scalar()

        /// <summary>
        /// Filters the list of properties and returns only properties of primitive type.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns>Filtered list of properties.</returns>
        public static IEnumerable<QueryProperty<QueryScalarType>> Primitive(this IEnumerable<QueryProperty> properties)
        {
            return properties.Where(p => p.PropertyType is QueryScalarType).Cast<QueryProperty<QueryScalarType>>();
        }

        /// <summary>
        /// Transforms a value from one scalar type to another.
        /// </summary>
        /// <param name="valueToTransform">The value to transform.</param>
        /// <param name="targetType">The target type of the value.</param>
        /// <returns>The value transformed to the target type.</returns>
        public static QueryScalarValue MaterializeValueIfEnum(this QueryScalarValue valueToTransform, QueryScalarType targetType)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetType, "targetType");
            var clrType = ((IQueryClrType)targetType).ClrType;

            Type clrEnumType;
            object enumValue = null;

            if (TryExtractEnumType(clrType, out clrEnumType))
            {
                if (valueToTransform.Value != null)
                {
                    enumValue = Enum.ToObject(clrEnumType, valueToTransform.Value);
                }

                return targetType.CreateValue(enumValue);
            }
            else
            {
                return valueToTransform;
            }
        }

        /// <summary>
        /// Checks whether 2 QueryScalarTypes are representing the same type.
        /// </summary>
        /// <param name="type1">The first scalar type.</param>
        /// <param name="type2">The second scalar type.</param>
        /// <returns>True if the scalar types are the same, false otherwise.</returns>
        public static bool IsSameQueryScalarType(this QueryScalarType type1, QueryScalarType type2)
        {
            // All query scalar types have clr types, its sufficient to filter out constants that the same clr type.
            var clrType1 = type1 as IQueryClrType;
            var clrType2 = type2 as IQueryClrType;

            ExceptionUtilities.Assert(clrType1 != null, "Scalar Types must always have a Clr type");
            ExceptionUtilities.Assert(clrType2 != null, "Scalar Types must always have a Clr type");

            if (clrType1.ClrType == clrType2.ClrType)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Evaluates the given query, asserts that it results in a collection value, and chooses a random element from the collection. If the collection is empty, null is returned.
        /// </summary>
        /// <param name="evaluator">The query evaluator to use</param>
        /// <param name="queryExpression">The query to evaluate, must result in a collection</param>
        /// <param name="random">The random number generator to use for selecting a result from the collection</param>
        /// <returns>A random element of the evaluated collection, or null if no elements are found</returns>
        public static QueryValue ChooseRandomEvaluationResult(this IQueryExpressionEvaluator evaluator, QueryExpression queryExpression, IRandomNumberGenerator random)
        {
            QueryCollectionValue collection;
            return evaluator.ChooseRandomEvaluationResult(queryExpression, random, out collection);
        }

        /// <summary>
        /// Evaluates the given query, asserts that it results in a collection value, and chooses a random element from the collection. If the collection is empty, null is returned.
        /// </summary>
        /// <param name="evaluator">The query evaluator to use</param>
        /// <param name="queryExpression">The query to evaluate, must result in a collection</param>
        /// <param name="random">The random number generator to use for selecting a result from the collection</param>
        /// <param name="collection">The evaluation result</param>
        /// <returns>A random element of the evaluated collection, or null if no elements are found</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#",
            Justification = "Need to return both values")]
        public static QueryValue ChooseRandomEvaluationResult(this IQueryExpressionEvaluator evaluator, QueryExpression queryExpression, IRandomNumberGenerator random, out QueryCollectionValue collection)
        {
            ExceptionUtilities.CheckArgumentNotNull(evaluator, "evaluator");
            ExceptionUtilities.CheckArgumentNotNull(queryExpression, "queryExpression");
            ExceptionUtilities.CheckArgumentNotNull(random, "random");

            collection = evaluator.Evaluate(queryExpression) as QueryCollectionValue;
            ExceptionUtilities.CheckObjectNotNull(collection, "Query unexpectedly evaluated to null or was not a collection: {0}", queryExpression);
            ExceptionUtilities.Assert(collection.EvaluationError == null, "Query resulted in evaluation error: {0}", collection.EvaluationError);

            if (collection.IsNull || collection.Elements.Count == 0)
            {
                return null;
            }

            return random.ChooseFrom(collection.Elements);
        }

        /// <summary>
        /// Determines whether the property is a Navigation one or not
        /// </summary>
        /// <param name="property">Property to determine its type</param>
        /// <returns>true if its a navigation or false if its not</returns>
        public static bool IsNavigationProperty(this QueryProperty property)
        {
            var collection = property.PropertyType as QueryCollectionType;
            var entity = property.PropertyType as QueryEntityType;

            if (entity != null)
            {
                return true;
            }
            else if (collection != null)
            {
                return collection.ElementType is QueryEntityType;
            }

            return false;
        }

        /// <summary>
        /// Returns the list of Navigation Properties
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns>Filtered list of properties.</returns>
        public static IEnumerable<QueryProperty> NavigationProperties(this IEnumerable<QueryProperty> properties)
        {
            ExceptionUtilities.CheckArgumentNotNull(properties, "properties");

            return properties.Where(property => property.IsNavigationProperty());
        }

        /// <summary>
        /// Gets the related entity type for the given navigation property on the entity type
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="navigationPropertyName">The name of the navigation property</param>
        /// <returns>The entity type related to the current one via the navigation property</returns>
        public static QueryEntityType GetRelatedEntityType(this QueryEntityType entityType, string navigationPropertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            ExceptionUtilities.CheckArgumentNotNull(navigationPropertyName, "navigationPropertyName");

            var propertyType = entityType.Properties.Where(p => p.Name == navigationPropertyName).Select(p => p.PropertyType).SingleOrDefault();
            ExceptionUtilities.CheckObjectNotNull(propertyType, "Could not find type of property '{0}' on type '{1}'", navigationPropertyName, entityType.StringRepresentation);

            QueryEntityType propertyEntityType = propertyType as QueryEntityType;
            if (propertyEntityType == null)
            {
                var collectionType = propertyType as QueryCollectionType;
                ExceptionUtilities.CheckObjectNotNull(collectionType, "Property '{0}' on type '{1}' was neither an entity nor collection type. Type was '{2}'", navigationPropertyName, entityType.StringRepresentation, propertyType.StringRepresentation);
                propertyEntityType = collectionType.ElementType as QueryEntityType;
            }

            ExceptionUtilities.CheckObjectNotNull(propertyEntityType, "Property '{0}' on type '{1}' was neither an entity nor collection type. Type was '{2}'", navigationPropertyName, entityType.StringRepresentation, propertyType.StringRepresentation);
            return propertyEntityType;
        }

        /// <summary>
        /// Compares a NamedValue to a QueryStructuralValue
        /// </summary>
        /// <param name="comparer">Comparer used to see if the specified values are equal</param>
        /// <param name="existing">Existing QueryStructualValue to compare to</param>
        /// <param name="namedValue">NamedValue to compare against</param>
        /// <param name="scalarComparer">Scalar comparer to use</param>
        public static void CompareStructuralValue(this IQueryStructuralValueToNamedValueComparer comparer, QueryStructuralValue existing, NamedValue namedValue, IQueryScalarValueToClrValueComparer scalarComparer)
        {
            comparer.Compare(existing, (new NamedValue[] { namedValue }).AsEnumerable(), scalarComparer);
        }

        /// <summary>
        /// Attaches the function evaluator to the function.
        /// </summary>
        /// <param name="function">The function to add the evaluator for.</param>
        /// <param name="functionEvaluator">The function evaluator which evaluates the function based on the result type and argument values.</param>
        /// <returns>The function.</returns>
        public static Function WithEvaluator(this Function function, Func<QueryType, QueryValue[], QueryValue> functionEvaluator)
        {
            function.Annotations.Add(new FunctionEvaluatorAnnotation(functionEvaluator));

            return function;
        }

        /// <summary>
        /// Gets the type of the expression for non collections or the type of the elements of the expression for collections
        /// </summary>
        /// <param name="expression">The expression from which to get the ExpressionType or ElementType.</param>
        /// <returns>The QueryType of the expression</returns>
        public static QueryType GetExpressionTypeOrElementType(this QueryExpression expression)
        {
            ExceptionUtilities.CheckArgumentNotNull(expression, "expression");

            var expressionType = expression.ExpressionType;
            var collectionType = expressionType as QueryCollectionType;
            while (collectionType != null)
            {
                expressionType = collectionType.ElementType;
                collectionType = expressionType as QueryCollectionType;
            }

            return expressionType;
        }

        private static bool TryExtractEnumType(Type clrType, out Type clrEnumType)
        {
            if (clrType.IsGenericType() && clrType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                clrEnumType = clrType.GetGenericArguments().Single();

                if (clrEnumType.IsEnum())
                {
                    return true;
                }
            }
            else if (clrType.IsEnum())
            {
                clrEnumType = clrType;
                return true;
            }

            clrEnumType = null;
            return false;
        }
    }
}
