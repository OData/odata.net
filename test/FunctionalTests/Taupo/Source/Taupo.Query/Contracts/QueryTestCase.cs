//---------------------------------------------------------------------
// <copyright file="QueryTestCase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;

    /// <summary>
    /// A test case used for constructing and verifying queries
    /// </summary>
    public abstract class QueryTestCase : TestCase 
    {
        private List<QueryBinaryOperation> arithmeticOperations = new List<QueryBinaryOperation>()
        {            
            QueryBinaryOperation.Add,
            QueryBinaryOperation.Divide,
            QueryBinaryOperation.Subtract,
            QueryBinaryOperation.Multiply,
            QueryBinaryOperation.Modulo,
        };

        /// <summary>
        /// Determines how properties of an entities are filtered based on the type of operation that is supported.
        /// </summary>
        protected enum SupportOperationsFilter
        {
            /// <summary>
            /// Filter on values which are equal comparable.
            /// </summary>
            EqualComparable,

            /// <summary>
            /// Filter on values which are order comparable.
            /// </summary>
            OrderComparable,

            /// <summary>
            /// Filter on values which support arithmetic operators.
            /// </summary>
            ArithmeticOperable,

            /// <summary>
            /// Select all properties.
            /// </summary>
            NoFilter,
        }

        /// <summary>
        /// Determines whether navigation properties should be selected when filtering the properties of an entity.
        /// </summary>
        protected enum NavigationPropertyFilter
        {
            /// <summary>
            /// Select navigation properties only.
            /// </summary>
            NavigationOnly,

            /// <summary>
            /// Select non navigation properties only.
            /// </summary>
            NonNavigationOnly,

            /// <summary>
            /// Select all properties.
            /// </summary>
            NoFilter,
        }

        /// <summary>
        /// Gets or sets the test case's query verifier.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryVerifier Verifier { get; set;  }

        /// <summary>
        /// Gets or sets the test case's query repository.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public QueryRepository Repository { get; set; }

        /// <summary>
        /// Gets or sets the test case's query evaluator.
        /// </summary>
        [InjectDependency(IsRequired = false)]
        public IQueryExpressionEvaluator Evaluator { get; set; }

        /// <summary>
        /// Constructs a root query expression for the given set
        /// </summary>
        /// <param name="entitySet">The entity set to build the root query for</param>
        /// <returns>A root query expression for the given set</returns>
        protected internal QueryExpression BuildRootQueryForSet(EntitySet entitySet)
        {
            return CommonQueryBuilder.Root(entitySet.Name, this.Repository.RootDataTypes[entitySet.Name].CreateCollectionType());
        }

        /// <summary>
        /// Determines whether a given type supports arithmetic operations
        /// </summary>
        /// <param name="queryScalarType">The type to test for arithmetic operation support</param>
        /// <returns>A boolean value indicating whether the type supports arithmetic operations</returns>
        protected bool SupportsArithmeticOperations(QueryScalarType queryScalarType)
        {
            foreach (QueryBinaryOperation op in this.arithmeticOperations)
            {
                if (!queryScalarType.Supports(op))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets all the nested properties of a structural type recursively.
        /// </summary>
        /// <param name="structuralType">The type from which to extra properties.</param>
        /// <param name="operationFilter">The filter to apply based on supported operations of each property.</param>
        /// <param name="navigationPropertyFilter">The filter to apply based on whether or not to select navigation properties.</param>
        /// <returns>A list of string arrays representing the property paths to each property (including nested properties) in the entity.</returns>
        protected IEnumerable<string[]> GetAllNestedPropertyPaths(QueryStructuralType structuralType, SupportOperationsFilter operationFilter, NavigationPropertyFilter navigationPropertyFilter)
        {
            IList<string[]> allPropertyPaths = new List<string[]>();

            foreach (var property in structuralType.Properties)
            {
                var propertyType = property.PropertyType;
                var qst = propertyType as QueryScalarType;
                var qcxt = propertyType as QueryComplexType;
                var qct = propertyType as QueryCollectionType;

                if (qst != null)
                {
                    if (operationFilter == SupportOperationsFilter.NoFilter ||
                        (operationFilter == SupportOperationsFilter.ArithmeticOperable && this.SupportsArithmeticOperations(qst)) ||
                        (operationFilter == SupportOperationsFilter.EqualComparable && qst.SupportsEqualityComparison) ||
                        (operationFilter == SupportOperationsFilter.OrderComparable && qst.SupportsOrderComparison))
                    {
                        if (navigationPropertyFilter != NavigationPropertyFilter.NavigationOnly)
                        {
                            allPropertyPaths.Add(new string[] { property.Name });
                        }
                    }
                }
                else if (qcxt != null)
                {
                    // For complex types recursively find child properties and add them to the list of property paths.
                    var nestedTypePaths = this.GetAllNestedPropertyPaths(qcxt, operationFilter, navigationPropertyFilter);

                    foreach (var nestedTypePath in nestedTypePaths)
                    {
                        var propertyPath = new string[] { property.Name };
                        propertyPath = propertyPath.Concat(nestedTypePath).ToArray();

                        if (navigationPropertyFilter != NavigationPropertyFilter.NavigationOnly)
                        {
                            allPropertyPaths.Add(propertyPath);
                        }
                    }
                }
                else if (qct != null)
                {
                    if (navigationPropertyFilter != NavigationPropertyFilter.NonNavigationOnly)
                    {
                        allPropertyPaths.Add(new string[] { property.Name });
                    }
                }
            }

            return allPropertyPaths.AsEnumerable();
        }

        /// <summary>
        /// Builds a <see cref="QueryPropertyExpression"/> using a property path.
        /// </summary>
        /// <param name="instance">The instance expression from which to create the property expression.</param>
        /// <param name="propertyPath">The property path to the property.</param>
        /// <returns>The property expression.</returns>
        protected QueryPropertyExpression BuildPropertyExpression(QueryExpression instance, string[] propertyPath)
        {
            this.Assert.IsTrue(propertyPath.Length > 0, "Cannot get property expression with an empty property path");

            QueryExpression expression = instance;

            foreach (var pathSegment in propertyPath)
            {
                expression = expression.Property(pathSegment);
            }

            return expression as QueryPropertyExpression;
        }
    }
}
