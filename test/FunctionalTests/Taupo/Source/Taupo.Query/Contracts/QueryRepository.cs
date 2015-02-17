//---------------------------------------------------------------------
// <copyright file="QueryRepository.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;

    /// <summary>
    /// Contains the information necessary to build queries for verification.
    /// </summary>
    public class QueryRepository
    {
        /// <summary>
        /// Initializes a new instance of the QueryRepository class.
        /// </summary>
        /// <param name="typeLibrary">The query type library.</param>
        /// <param name="rootQueries">The collection of QueryExpressions used to initialize the RootQueries property</param>
        /// <param name="constants">The collection of QueryConstants used to initialize the Constants property</param>
        /// <param name="scalarTypes">The collection of scalar types.</param>
        /// <param name="rootDataTypes">The root data types.</param>
        /// <param name="dataSet">The data set used to initialize the DataSet property.</param>
        public QueryRepository(QueryTypeLibrary typeLibrary, IEnumerable<QueryExpression> rootQueries, IEnumerable<QueryConstantExpression> constants, IEnumerable<QueryScalarType> scalarTypes, IDictionary<string, QueryStructuralType> rootDataTypes, IQueryDataSet dataSet)
        {
            ExceptionUtilities.CheckArgumentNotNull(typeLibrary, "typeLibrary");
            ExceptionUtilities.CheckArgumentNotNull(rootQueries, "rootQueries");
            ExceptionUtilities.CheckArgumentNotNull(constants, "constants");
            ExceptionUtilities.CheckArgumentNotNull(scalarTypes, "scalarTypes");
            ExceptionUtilities.CheckArgumentNotNull(rootDataTypes, "rootDataTypes");
            ExceptionUtilities.CheckArgumentNotNull(dataSet, "dataSet");

            this.TypeLibrary = typeLibrary;
            this.RootQueries = rootQueries.ToList().AsReadOnly();
            this.Constants = constants.ToList().AsReadOnly();
            this.ScalarTypes = scalarTypes.ToList().AsReadOnly();
            this.RootDataTypes = new Dictionary<string, QueryStructuralType>(rootDataTypes);
            this.DataSet = dataSet;
        }

        /// <summary>
        /// Gets or sets random number generator to be used by tests.
        /// </summary>
        [InjectDependency]
        public IRandomNumberGenerator Random { get; set; }

        /// <summary>
        /// Gets the repository's collection of root queries
        /// </summary>
        public ReadOnlyCollection<QueryExpression> RootQueries { get; private set; }

        /// <summary>
        /// Gets the repository's collection of constants
        /// </summary>
        public ReadOnlyCollection<QueryConstantExpression> Constants { get; private set; }

        /// <summary>
        /// Gets the repository's collection of scalar types
        /// </summary>
        public ReadOnlyCollection<QueryScalarType> ScalarTypes { get; private set; }

        /// <summary>
        /// Gets the root data types.
        /// </summary>
        public IDictionary<string, QueryStructuralType> RootDataTypes { get; private set; }

        /// <summary>
        /// Gets the repository's query data set
        /// </summary>
        public IQueryDataSet DataSet { get; private set; }

        /// <summary>
        /// Gets the repository's query type library
        /// </summary>
        public QueryTypeLibrary TypeLibrary { get; private set; }

        /// <summary>
        /// Builds a constant expression designed to be used in query filters. The constant is one that is likely to return results although
        /// results are not guaranteed.
        /// </summary>
        /// <param name="rootExpression">The <cref>QueryRootExpression</cref> that contains the property used to generate the constant.</param>
        /// <param name="propertyPaths">The paths to the property.</param>
        /// <returns>A constant expression matching the property's type</returns>
        public QueryConstantExpression GetConstantForProperty(QueryRootExpression rootExpression, string[] propertyPaths)
        {
            var dataRows = this.DataSet[rootExpression.Name].Elements.Cast<QueryStructuralValue>();

            IEnumerable<QueryScalarValue> propertyDataSet = 
                this.GetPropertyRows(
                    dataRows, 
                    propertyPaths.ToList<string>(), 
                    ((QueryCollectionType<QueryStructuralType>)rootExpression.ExpressionType).ElementType);

            return CommonQueryBuilder.Constant(this.SelectRelevantValue(propertyDataSet));
        }

        private IEnumerable<QueryScalarValue> GetPropertyRows(IEnumerable<QueryStructuralValue> dataRows, IList<string> propertyPaths, QueryStructuralType rowType)
        {
            var currentPropertyPath = propertyPaths[0];
            var nestedPropertyType = rowType.Properties.Where(p => p.Name == currentPropertyPath).Single().PropertyType;
            
            if (propertyPaths.Count == 1)
            {
                var nestedScalarType = nestedPropertyType as QueryScalarType;
                ExceptionUtilities.CheckObjectNotNull(nestedScalarType, "The leaf property of any structural type must be a scalar type");
                var propertyDataSet = dataRows.Select(qsv => qsv.GetValue(currentPropertyPath));

                if (propertyDataSet.Count() > 0)
                {
                    ExceptionUtilities.Assert(propertyDataSet.First() is QueryScalarValue, "Type mismatch between property type and data set, property {0} should be a scalar value in the dataSet", currentPropertyPath);
                    return propertyDataSet.Cast<QueryScalarValue>();
                }
                else
                {
                    var propertyDataSetList = new List<QueryScalarValue>();
                    propertyDataSetList.Add(nestedScalarType.DefaultValue as QueryScalarValue);
                    return propertyDataSetList.AsEnumerable<QueryScalarValue>();
                }
            }
            else
            {
                // if we are not at the leaf property, find it recursively.
                var nestedStructuralType = nestedPropertyType as QueryStructuralType;
                ExceptionUtilities.CheckObjectNotNull(nestedStructuralType, "A non-leaf property of a structural type must be a structural type");
                propertyPaths.RemoveAt(0);
                var nestedProperties = dataRows.Select(qsv => qsv.GetValue(currentPropertyPath)).Cast<QueryStructuralValue>();

                return this.GetPropertyRows(nestedProperties, propertyPaths, nestedStructuralType);
            }
        }

        private QueryScalarValue SelectRelevantValue(IEnumerable<QueryScalarValue> dataValues)                                                                                   
        {
            ExceptionUtilities.CheckCollectionNotEmpty(dataValues, "dataValues");

            // Don't select null constants where possible, null constants should be tested separately using the isNull and isNotNull expressions.
            if (dataValues.All(dv => dv.IsNull))
            {
                return dataValues.First();
            }
            else
            {
                var dataType = dataValues.First().Type;
                var scalarDataValues = dataValues.Where(dv => !dv.IsNull).Select(dv => dv.Value);

                // if we have more than 3 rows to choose from, then removing the largest and the smallest values from being chosen allows us to
                // guarantee that the query returns a result for equality and order comparisons.
                if (scalarDataValues.Count() >= 3 && scalarDataValues.First() is IComparable)
                {
                    var maxValue = scalarDataValues.Max();
                    var minValue = scalarDataValues.Min();

                    var valuesList = scalarDataValues.ToList();
                    valuesList.Remove(maxValue);
                    valuesList.Remove(minValue);
                    scalarDataValues = valuesList.AsEnumerable();
                }

                return dataType.CreateValue(this.Random.ChooseFrom(scalarDataValues));
            }
        }
    }
}
