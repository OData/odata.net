//---------------------------------------------------------------------
// <copyright file="ODataQueryTestCase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ServiceReferences;
    using Microsoft.Test.Taupo.Astoria.Contracts.WebServices.DataOracleService.DotNet;
    using Microsoft.Test.Taupo.Astoria.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.EntityModel;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Base class for all astoria tests.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The testcase base class has to be coupled with many classes")]
    public abstract class ODataQueryTestCase : QueryTestCase
    {
        /// <summary>
        /// Gets or sets the query resolver to resolve untyped expressions to runtime-type bound expression
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IAstoriaQueryVerifierCapabilityInspector QueryVerifierCapabilityInspector { get; set; }

        /// <summary>
        /// Gets or sets the query resolver to resolve untyped expressions to runtime-type bound expression
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ILinqToAstoriaQueryResolver QueryResolver { get; set; }

        /// <summary>
        /// Gets or sets the services to use for data generation
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IEntityModelConceptualDataServices StructuralDataServices { get; set; }

        /// <summary>
        /// Gets or sets the component to convert QueryExpressions to Uris
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryToUriStringConverter QueryExpressionToUriConverter { get; set; }

        /// <summary>
        /// Gets the collection of tested primitive types
        /// </summary>
        public Collection<Type> PrimitiveTypesTested { get; private set; }

        /// <summary>
        /// Gets a queryExpression that represents a Query to an existing entity
        /// </summary>
        /// <param name="rootQuery"> The root query expression of the set</param>
        /// <param name="existingEntity">The structural value representing an existing entity</param>
        /// <returns>A QueryExpression representing a query to an existing entity</returns>
        /// <remarks>
        /// 1. We don't use the .Key syntax because : 
        ///     Key forces the Query option to take on the Key FOrmat
        ///     /Customers('ALFKI')
        ///     We change the query to be  Customers?$filter=Id eq 'ALFKI' and true so that we don't use the key syntax as it doesn't work with Live servers.
        /// 2. We append the 'and true' part because : 
        ///     If you use the client library and completely cover the key, the client's linq translator will use the key syntax.
        ///     To avoid this, we completely cover the key and also add the 'true" so that the client doesn't produce a key expression string.
        /// </remarks>
        protected internal static QueryExpression GetExistingEntityQuery(QueryExpression rootQuery, QueryStructuralValue existingEntity)
        {
            // We cannot create new LinqParameterExpressions as we track the parameters by name, we need to reuse the parameter from the root query
            Func<LinqParameterExpression, QueryExpression> generateFilterExpression = (entityParameter) =>
            {
                QueryExpression keyFilterExpression = GetExistingEntityKeyComparisonExpression(entityParameter, existingEntity);
                return keyFilterExpression;
            };

            var query = rootQuery.Where(eParam => generateFilterExpression(eParam).And(CommonQueryBuilder.Constant(true, existingEntity.Type.EvaluationStrategy.BooleanType)));
            return query;
        }

        /// <summary>
        /// Resets the collection of tested primitive types
        /// </summary>
        protected internal void ResetPrimitiveTypesTested()
        {
            this.PrimitiveTypesTested = new Collection<Type>();
        }

        /// <summary>
        /// Generates new property values for the given type using this test's structural data services
        /// </summary>
        /// <param name="set">The type's entity set</param>
        /// <param name="type">The type to generate properties for</param>
        /// <returns>A set of new property values for the given type</returns>
        protected internal IEnumerable<NamedValue> GeneratePropertyValues(EntitySet set, EntityType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(set, "set");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            EntityContainer container = set.Container;
            ExceptionUtilities.CheckObjectNotNull(container, "Entity set did not have its entity container set");

            var generator = this.StructuralDataServices.GetStructuralGenerator(type.FullName, container.Name + '.' + set.Name);
            return generator.GenerateData();
        }

        /// <summary>
        /// Generates a set of values for an entity type's key properties. Will throw if any key property is a dependent/foreign-key. For composite keys, only one of the values is guaranteed to be unique.
        /// </summary>
        /// <param name="type">The entity type to generate a key for</param>
        /// <returns>The key property values in metadata order</returns>
        protected internal IEnumerable<NamedValue> GeneratePropertyValuesWithUniqueKey(QueryEntityType type)
        {
            return this.GeneratePropertyValuesWithUniqueKey(type.EntitySet, type.EntityType);
        }

        /// <summary>
        /// Gets the existing values of the given primitive properties using the in-memory query evaluator
        /// </summary>
        /// <param name="set">The entity set</param>
        /// <param name="propertyNames">The properties to get values for</param>
        /// <returns>The values for the given properties as returned by the query evaluator</returns>
        protected internal Dictionary<string, List<object>> GetExistingPrimitivePropertyValues(EntitySet set, IEnumerable<string> propertyNames)
        {
            // build a projection query to find the values for the key properties
            var query = this.BuildRootQueryForSet(set).Select(o => LinqBuilder.New(propertyNames, propertyNames.Select(p => o.Property(p))));

            // evaluate the projection
            var collection = this.Evaluator.Evaluate(query) as QueryCollectionValue;
            ExceptionUtilities.CheckObjectNotNull(collection, "Query did not return a collection: {0}", query);
            ExceptionUtilities.Assert(collection.EvaluationError == null, "Query evaluation error: " + collection.EvaluationError);

            var structuralValues = collection.Elements.Cast<QueryStructuralValue>();
            var existingValues = propertyNames
                .ToDictionary(p => p, p => structuralValues.Select(v => v.GetScalarValue(p).Value).ToList());

            return existingValues;
        }

        /// <summary>
        /// Generates a set of values for an entity type's key properties. Will throw if any key property is a dependent/foreign-key. For composite keys, only one of the values is guaranteed to be unique.
        /// </summary>
        /// <param name="set">The entity set the type belongs to</param>
        /// <param name="type">The entity type to generate a key for</param>
        /// <returns>The key property values in metadata order</returns>
        protected internal IEnumerable<NamedValue> GeneratePropertyValuesWithUniqueKey(EntitySet set, EntityType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(set, "set");
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            AssertPrimaryKeyHasNoReferentialConstraints(type);

            // get the key property names
            var keyPropertyNames = type.AllKeyProperties.Select(p => p.Name).ToList();

            // get the existing values for the key properties
            var existingValues = this.GetExistingPrimitivePropertyValues(set, keyPropertyNames);

            // generate an initial set of values
            var generatedValues = this.GeneratePropertyValues(set, type);

            // allocate storage for the key values we find. Note that we will generate them in random order, then reorder them later.
            // Doing it this way so that the 1st property is not the one that must be unique for composite keys
            var uniqueValues = new Dictionary<string, object>();

            // loop through the key property names in random order
            // if a unique value is found, then there is no need to check for the rest
            bool keyIsUnique = false;
            while (keyPropertyNames.Any())
            {
                // pick a random property name
                var propertyName = this.Random.ChooseFrom(keyPropertyNames);
                keyPropertyNames.Remove(propertyName);

                // only retry one more time than there are values, since by then we really should have found something
                var tries = existingValues[propertyName].Count + 1;

                // regenerate property values until a unique value is found for this property
                var value = generatedValues.Single(v => v.Name == propertyName).Value;
                while (!keyIsUnique && tries-- > 0)
                {
                    keyIsUnique = !existingValues[propertyName].Any(v => ValueComparer.Instance.Equals(v, value));
                    if (!keyIsUnique)
                    {
                        generatedValues = this.GeneratePropertyValues(set, type);
                        value = generatedValues.Single(v => v.Name == propertyName).Value;
                    }
                }

                // even if we did not find a unique value for this property, we should populate it, because another property
                // may result in the key being unique later on
                uniqueValues[propertyName] = value;
            }

            // if we could not generate a unique key, throw an exception
            ExceptionUtilities.Assert(keyIsUnique, "Could not generate a unique key for type '{0}'", type.FullName);

            if (!type.Annotations.OfType<HasStreamAnnotation>().Any())
            {
                // add in the non-key values
                foreach (var value in generatedValues)
                {
                    if (!uniqueValues.ContainsKey(value.Name))
                    {
                        uniqueValues[value.Name] = value.Value;
                    }
                }
            }

            // return them in metadata order
            return type.AllProperties.SelectMany(p => uniqueValues.Where(pair => pair.Key.Split('.').First().Equals(p.Name)).Select(pair => new NamedValue(pair.Key, pair.Value)));
        }

        /// <summary>
        /// Generates new property values for the given type using this test's structural data services
        /// </summary>
        /// <param name="type">The type to generate properties for</param>
        /// <returns>A set of new property values for the given type</returns>
        protected internal IEnumerable<NamedValue> GeneratePropertyValues(QueryEntityType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            return this.GeneratePropertyValues(type.EntitySet, type.EntityType);
        }

        /// <summary>
        /// Generates new property values for the given type using this test's structural data services
        /// </summary>
        /// <param name="type">The type to generate properties for</param>
        /// <returns>A set of new property values for the given type</returns>
        protected internal IEnumerable<NamedValue> GeneratePropertyValues(ComplexType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            var generator = this.StructuralDataServices.GetStructuralGenerator(type.FullName);
            return generator.GenerateData();
        }

        /// <summary>
        /// Gets a queryExpression that represents a Query to an existing entity
        /// </summary>
        /// <param name="entityParameter">The LinqParameterExpression which represents the root parameter</param>
        /// <param name="existingEntity">The structural value representing an existing entity</param>
        /// <returns>A QueryExpression representing a query to an existing entity</returns>
        protected static QueryExpression GetExistingEntityKeyComparisonExpression(QueryExpression entityParameter, QueryStructuralValue existingEntity)
        {
            QueryExpression keyFilterExpression = null;
            QueryEntityType entityType = existingEntity.Type as QueryEntityType;
            foreach (var keyProperty in entityType.EntityType.AllKeyProperties)
            {
                var queryProperty = entityType.Properties.SingleOrDefault(p => p.Name == keyProperty.Name);
                ExceptionUtilities.CheckObjectNotNull(queryProperty, "Could not find property with name '{0}' on type '{1}'", keyProperty.Name, entityType);

                var value = existingEntity.GetScalarValue(keyProperty.Name);
                QueryExpression keyComparision = entityParameter.Property(keyProperty.Name).EqualTo(CommonQueryBuilder.Constant(value));

                if (keyFilterExpression == null)
                {
                    keyFilterExpression = keyComparision;
                }
                else
                {
                    keyFilterExpression = CommonQueryBuilder.And(keyFilterExpression, keyComparision);
                }
            }

            return keyFilterExpression;
        }

        /// <summary>
        /// Get properties in the structural type that support a specific Binary Operation.
        /// </summary>
        /// <param name="type">Structural Type.</param>
        /// <param name="op">An enum that represents a Query Binary Operation.</param>
        /// <returns>A collection of properties.</returns>
        protected IEnumerable<QueryProperty<QueryScalarType>> GetPropertiesWithBinaryOpSupport(QueryStructuralType type, QueryBinaryOperation op)
        {
            List<QueryProperty<QueryScalarType>> queryProperties = type.Properties.Primitive().Where(m => m.PropertyType.Supports(op)).ToList();

            DataServiceExecuteVerifier verifier = this.Verifier as DataServiceExecuteVerifier;
            if (verifier != null && verifier.IsUri == false)
            {
                if (op == QueryBinaryOperation.GreaterThan || op == QueryBinaryOperation.GreaterThanOrEqualTo || op == QueryBinaryOperation.LessThan || op == QueryBinaryOperation.LessThanOrEqualTo)
                {
                    // Exclude string, DateTime and boolean properties as these don't support greater than or less than operators in the Client Linq Code
                    queryProperties = queryProperties.Where(qp => (qp.PropertyType as IQueryClrType).ClrType != typeof(string)).ToList();
                    queryProperties = queryProperties.Where(qp => (qp.PropertyType as IQueryClrType).ClrType != typeof(bool)).ToList();
                    queryProperties = queryProperties.Where(qp => (qp.PropertyType as IQueryClrType).ClrType != typeof(DateTime)).ToList();
                    queryProperties = queryProperties.Where(qp => (qp.PropertyType as IQueryClrType).ClrType != typeof(Guid)).ToList();
                }
            }

            return queryProperties;
        }

        private static void AssertPrimaryKeyHasNoReferentialConstraints(EntityType type)
        {
            // dependent properties are not supported
            var keyMembersWithReferentialContraints = type.PrimaryKeyPropertiesWithReferentialConstraints();
            if (keyMembersWithReferentialContraints.Any())
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Cannot generate key value for property '{0}' on type '{1}' because it is a foreign key",
                    string.Join(",", keyMembersWithReferentialContraints.Select(p => p.Name).ToArray()),
                    type.FullName);
                throw new TaupoInvalidOperationException(message);
            }
        }
    }
}
