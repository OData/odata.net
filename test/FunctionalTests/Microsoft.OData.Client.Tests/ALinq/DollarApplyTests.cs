//---------------------------------------------------------------------
// <copyright file="DollarApplyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Xunit;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Client.Tests.ALinq
{
    public class DollarApplyTests
    {
        private Random rand = new Random();
        private readonly DataServiceContext dsContext;
        private const string serviceUri = "http://tempuri.org";
        private const string numbersEntitySetName = "Numbers";
        private const string salesEntitySetName = "Sales";
        private static string aggregationExprTemplate = "{0} with {1} as {2}";
        private static string aggregateTemplate = "aggregate({0})";

        public DollarApplyTests()
        {
            EdmModel model = BuildEdmModel();

            dsContext = new DataServiceContext(new Uri(serviceUri));
            dsContext.Format.UseJson(model);
        }

        public static IEnumerable<object[]> GetAggregationData()
        {
            var testData = new List<object[]>();

            foreach (var aggregationMethod in new[] { "Sum", "Average", "Min", "Max" })
            {
                foreach (var type in new[] { "Int", "Double", "Decimal", "Long", "Single" })
                {
                    var aggregationMethodToLower = aggregationMethod.ToLower(); // e.g. sum
                    var propertyName = type + "Prop";  // e.g. IntProp
                    var aggregationAlias = aggregationMethod + propertyName; // e.g. SumIntProp
                    var nullablePropertyName = "Nullable" + propertyName; // e.g. NullableIntProp
                    var nullableAggregationAlias = aggregationMethod + nullablePropertyName; // e.g. SumNullableIntProp

                    testData.Add(new object[]
                    {
                        aggregationMethod,
                        aggregationAlias,
                        propertyName,
                        string.Format(aggregateTemplate, string.Format(aggregationExprTemplate, propertyName, aggregationMethodToLower, aggregationAlias))
                    });
                    testData.Add(new object[]
                    {
                        aggregationMethod,
                        nullableAggregationAlias,
                        nullablePropertyName,
                        string.Format(aggregateTemplate, string.Format(aggregationExprTemplate, nullablePropertyName, aggregationMethodToLower, nullableAggregationAlias))
                    });
                    // e.g.
                    // { "Sum", "IntProp", "aggregate(IntProp with sum as SumIntProp)" }
                    // { "Sum", "NullableIntProp", "aggregate(NullableIntProp with sum as SumNullableIntProp)" }
                }
            }

            foreach (var item in testData)
                yield return item;
        }

        [Theory]
        [MemberData(nameof(GetAggregationData))]
        public void TranslateThenExecuteAggregateExpression(string aggregationMethod, string aggregationAlias, string propertyName, string aggregateTemplate)
        {
            string expectedAggregateUri = serviceUri + '/' + numbersEntitySetName + "?$apply=" + aggregateTemplate;
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            PropertyInfo propertyInfo = queryable.ElementType.GetProperty(propertyName);
            ParameterExpression parameterExpr = Expression.Parameter(queryable.ElementType, "d1");
            Expression selectorExpr = Expression.Lambda(Expression.MakeMemberAccess(parameterExpr, propertyInfo), parameterExpr);
            Type propertyType = propertyInfo.PropertyType;

            // Find matching aggregation method - using reflection
            MethodInfo methodInfo = GetAggregationMethod(aggregationMethod, propertyType);

            List<Type> genericArguments = new List<Type>();
            genericArguments.Add(queryable.ElementType);
            if (methodInfo.GetGenericArguments().Length > 1)
            {
                genericArguments.Add(propertyType);
            }

            MethodCallExpression methodCallExpr = Expression.Call(
                null,
                methodInfo.MakeGenericMethod(genericArguments.ToArray()),
                new[] { queryable.Expression, Expression.Quote(selectorExpr) });

            // Call factory method for creating DataServiceOrderedQuery based on Linq expression
            var query = new DataServiceQueryProvider(dsContext).CreateQuery(methodCallExpr);

            // Verify expected aggregate Uri
            Assert.Equal(expectedAggregateUri, query.ToString());

            Type returnType = propertyType;
            if ((propertyType.Equals(typeof(int)) || propertyType.Equals(typeof(long)))
                && aggregationMethod.Equals("Average", StringComparison.OrdinalIgnoreCase))
            {
                returnType = methodInfo.ReturnType;
            }

            // Execute expression and verify result
            var randomAggregateValue = GenerateRandomValue(aggregationMethod, returnType);
            InterceptRequestAndMockResponse(aggregationAlias, randomAggregateValue);

            // Use reflection to get Execute method - should make it easy to apply different return types
            MethodInfo executeMethodInfo = typeof(DataServiceQueryProvider).GetMethods()
                .Where(d => d.Name.Equals("Execute")
                    && d.IsGenericMethodDefinition
                    && d.GetParameters().Length == 1
                    && d.GetParameters()[0].ParameterType.Equals(typeof(Expression))
                    && d.IsPublic
                    && !d.IsStatic
                ).FirstOrDefault();

            var queryProvider = new DataServiceQueryProvider(dsContext);
            var result = executeMethodInfo.MakeGenericMethod(returnType).Invoke(queryProvider, new object[] { methodCallExpr });

            Assert.Equal(result, randomAggregateValue);
        }

        [Fact]
        public void GroupByWithResultSelector_Sum_ByConstant()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1, (d2, d3) => new
            {
                SumIntProp = d3.Sum(d4 => d4.IntProp),
                SumNullableIntProp = d3.Sum(d4 => d4.NullableIntProp),
                SumDoubleProp = d3.Sum(d4 => d4.DoubleProp),
                SumNullableDoubleProp = d3.Sum(d4 => d4.NullableDoubleProp),
                SumDecimalProp = d3.Sum(d4 => d4.DecimalProp),
                SumNullableDecimalProp = d3.Sum(d4 => d4.NullableDecimalProp),
                SumLongProp = d3.Sum(d4 => d4.LongProp),
                SumNullableLongProp = d3.Sum(d4 => d4.NullableLongProp),
                SumSingleProp = d3.Sum(d4 => d4.SingleProp),
                SumNullableSingleProp = d3.Sum(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=aggregate(" +
                    "IntProp with sum as SumIntProp,NullableIntProp with sum as SumNullableIntProp," +
                    "DoubleProp with sum as SumDoubleProp,NullableDoubleProp with sum as SumNullableDoubleProp," +
                    "DecimalProp with sum as SumDecimalProp,NullableDecimalProp with sum as SumNullableDecimalProp," +
                    "LongProp with sum as SumLongProp,NullableLongProp with sum as SumNullableLongProp," +
                    "SingleProp with sum as SumSingleProp,NullableSingleProp with sum as SumNullableSingleProp)", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Sum_ByConstant();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Single(aggregateResult);

            var singleResult = aggregateResult.First();

            Assert.Equal(506, singleResult.SumIntProp);
            Assert.Equal(530, singleResult.SumNullableIntProp);
            Assert.Equal(464.72, singleResult.SumDoubleProp);
            Assert.Equal(534.02, singleResult.SumNullableDoubleProp);
            Assert.Equal(559.4M, singleResult.SumDecimalProp);
            Assert.Equal(393.7M, singleResult.SumNullableDecimalProp);
            Assert.Equal(1298L, singleResult.SumLongProp);
            Assert.Equal(993L, singleResult.SumNullableLongProp);
            Assert.Equal(333.79f, singleResult.SumSingleProp);
            Assert.Equal(528.44f, singleResult.SumNullableSingleProp);
        }

        [Fact]
        public void GroupByWithProjection_Sum_ByConstant()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1).Select(d2 => new
            {
                SumIntProp = d2.Sum(d3 => d3.IntProp),
                SumNullableIntProp = d2.Sum(d3 => d3.NullableIntProp),
                SumDoubleProp = d2.Sum(d3 => d3.DoubleProp),
                SumNullableDoubleProp = d2.Sum(d3 => d3.NullableDoubleProp),
                SumDecimalProp = d2.Sum(d3 => d3.DecimalProp),
                SumNullableDecimalProp = d2.Sum(d3 => d3.NullableDecimalProp),
                SumLongProp = d2.Sum(d3 => d3.LongProp),
                SumNullableLongProp = d2.Sum(d3 => d3.NullableLongProp),
                SumSingleProp = d2.Sum(d3 => d3.SingleProp),
                SumNullableSingleProp = d2.Sum(d3 => d3.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=aggregate(" +
                    "IntProp with sum as SumIntProp,NullableIntProp with sum as SumNullableIntProp," +
                    "DoubleProp with sum as SumDoubleProp,NullableDoubleProp with sum as SumNullableDoubleProp," +
                    "DecimalProp with sum as SumDecimalProp,NullableDecimalProp with sum as SumNullableDecimalProp," +
                    "LongProp with sum as SumLongProp,NullableLongProp with sum as SumNullableLongProp," +
                    "SingleProp with sum as SumSingleProp,NullableSingleProp with sum as SumNullableSingleProp)", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Sum_ByConstant();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Single(aggregateResult);

            var singleResult = aggregateResult.First();

            Assert.Equal(506, singleResult.SumIntProp);
            Assert.Equal(530, singleResult.SumNullableIntProp);
            Assert.Equal(464.72, singleResult.SumDoubleProp);
            Assert.Equal(534.02, singleResult.SumNullableDoubleProp);
            Assert.Equal(559.4M, singleResult.SumDecimalProp);
            Assert.Equal(393.7M, singleResult.SumNullableDecimalProp);
            Assert.Equal(1298L, singleResult.SumLongProp);
            Assert.Equal(993L, singleResult.SumNullableLongProp);
            Assert.Equal(333.79f, singleResult.SumSingleProp);
            Assert.Equal(528.44f, singleResult.SumNullableSingleProp);
        }

        [Fact]
        public void GroupByWithResultSelector_Sum_BySingleProperty()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.RowParity, (d2, d3) => new
            {
                RowParity = d2,
                SumIntProp = d3.Sum(d4 => d4.IntProp),
                SumNullableIntProp = d3.Sum(d4 => d4.NullableIntProp),
                SumDoubleProp = d3.Sum(d4 => d4.DoubleProp),
                SumNullableDoubleProp = d3.Sum(d4 => d4.NullableDoubleProp),
                SumDecimalProp = d3.Sum(d4 => d4.DecimalProp),
                SumNullableDecimalProp = d3.Sum(d4 => d4.NullableDecimalProp),
                SumLongProp = d3.Sum(d4 => d4.LongProp),
                SumNullableLongProp = d3.Sum(d4 => d4.NullableLongProp),
                SumSingleProp = d3.Sum(d4 => d4.SingleProp),
                SumNullableSingleProp = d3.Sum(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=groupby((RowParity),aggregate(" +
                    "IntProp with sum as SumIntProp,NullableIntProp with sum as SumNullableIntProp," +
                    "DoubleProp with sum as SumDoubleProp,NullableDoubleProp with sum as SumNullableDoubleProp," +
                    "DecimalProp with sum as SumDecimalProp,NullableDecimalProp with sum as SumNullableDecimalProp," +
                    "LongProp with sum as SumLongProp,NullableLongProp with sum as SumNullableLongProp," +
                    "SingleProp with sum as SumSingleProp,NullableSingleProp with sum as SumNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Sum_BySingleProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Even", aggregateResult[0].RowParity);
            Assert.Equal("Odd", aggregateResult[1].RowParity);
        }

        [Fact]
        public void GroupByWithProjection_Sum_BySingleProperty()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.RowParity).Select(d2 => new
            {
                RowParity = d2.Key,
                SumIntProp = d2.Sum(d3 => d3.IntProp),
                SumNullableIntProp = d2.Sum(d3 => d3.NullableIntProp),
                SumDoubleProp = d2.Sum(d3 => d3.DoubleProp),
                SumNullableDoubleProp = d2.Sum(d3 => d3.NullableDoubleProp),
                SumDecimalProp = d2.Sum(d3 => d3.DecimalProp),
                SumNullableDecimalProp = d2.Sum(d3 => d3.NullableDecimalProp),
                SumLongProp = d2.Sum(d3 => d3.LongProp),
                SumNullableLongProp = d2.Sum(d3 => d3.NullableLongProp),
                SumSingleProp = d2.Sum(d3 => d3.SingleProp),
                SumNullableSingleProp = d2.Sum(d3 => d3.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=groupby((RowParity),aggregate(" +
                    "IntProp with sum as SumIntProp,NullableIntProp with sum as SumNullableIntProp," +
                    "DoubleProp with sum as SumDoubleProp,NullableDoubleProp with sum as SumNullableDoubleProp," +
                    "DecimalProp with sum as SumDecimalProp,NullableDecimalProp with sum as SumNullableDecimalProp," +
                    "LongProp with sum as SumLongProp,NullableLongProp with sum as SumNullableLongProp," +
                    "SingleProp with sum as SumSingleProp,NullableSingleProp with sum as SumNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Sum_BySingleProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Even", aggregateResult[0].RowParity);
            Assert.Equal("Odd", aggregateResult[1].RowParity);
        }

        [Fact]
        public void GroupByWithResultSelector_Sum_ByMultipleProperties()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.RowParity, d1.RowCategory }, (d2, d3) => new
            {
                d2.RowParity,
                d2.RowCategory,
                SumIntProp = d3.Sum(d4 => d4.IntProp),
                SumNullableIntProp = d3.Sum(d4 => d4.NullableIntProp),
                SumDoubleProp = d3.Sum(d4 => d4.DoubleProp),
                SumNullableDoubleProp = d3.Sum(d4 => d4.NullableDoubleProp),
                SumDecimalProp = d3.Sum(d4 => d4.DecimalProp),
                SumNullableDecimalProp = d3.Sum(d4 => d4.NullableDecimalProp),
                SumLongProp = d3.Sum(d4 => d4.LongProp),
                SumNullableLongProp = d3.Sum(d4 => d4.NullableLongProp),
                SumSingleProp = d3.Sum(d4 => d4.SingleProp),
                SumNullableSingleProp = d3.Sum(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=groupby((RowParity,RowCategory),aggregate(" +
                    "IntProp with sum as SumIntProp,NullableIntProp with sum as SumNullableIntProp," +
                    "DoubleProp with sum as SumDoubleProp,NullableDoubleProp with sum as SumNullableDoubleProp," +
                    "DecimalProp with sum as SumDecimalProp,NullableDecimalProp with sum as SumNullableDecimalProp," +
                    "LongProp with sum as SumLongProp,NullableLongProp with sum as SumNullableLongProp," +
                    "SingleProp with sum as SumSingleProp,NullableSingleProp with sum as SumNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Sum_ByMultipleProperties();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(3, aggregateResult.Length);
            Assert.True(aggregateResult[0].RowParity.Equals("Even") && aggregateResult[0].RowCategory.Equals("Composite"));
            Assert.True(aggregateResult[1].RowParity.Equals("Odd") && aggregateResult[1].RowCategory.Equals("None"));
            Assert.True(aggregateResult[2].RowParity.Equals("Odd") && aggregateResult[2].RowCategory.Equals("Prime"));
        }

        [Fact]
        public void GroupByWithProjection_Sum_ByMultipleProperties()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.RowParity, d1.RowCategory }).Select(d2 => new
            {
                d2.Key.RowParity,
                d2.Key.RowCategory,
                SumIntProp = d2.Sum(d3 => d3.IntProp),
                SumNullableIntProp = d2.Sum(d3 => d3.NullableIntProp),
                SumDoubleProp = d2.Sum(d3 => d3.DoubleProp),
                SumNullableDoubleProp = d2.Sum(d3 => d3.NullableDoubleProp),
                SumDecimalProp = d2.Sum(d3 => d3.DecimalProp),
                SumNullableDecimalProp = d2.Sum(d3 => d3.NullableDecimalProp),
                SumLongProp = d2.Sum(d3 => d3.LongProp),
                SumNullableLongProp = d2.Sum(d3 => d3.NullableLongProp),
                SumSingleProp = d2.Sum(d3 => d3.SingleProp),
                SumNullableSingleProp = d2.Sum(d3 => d3.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=groupby((RowParity,RowCategory),aggregate(" +
                    "IntProp with sum as SumIntProp,NullableIntProp with sum as SumNullableIntProp," +
                    "DoubleProp with sum as SumDoubleProp,NullableDoubleProp with sum as SumNullableDoubleProp," +
                    "DecimalProp with sum as SumDecimalProp,NullableDecimalProp with sum as SumNullableDecimalProp," +
                    "LongProp with sum as SumLongProp,NullableLongProp with sum as SumNullableLongProp," +
                    "SingleProp with sum as SumSingleProp,NullableSingleProp with sum as SumNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Sum_ByMultipleProperties();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(3, aggregateResult.Length);
            Assert.True(aggregateResult[0].RowParity.Equals("Even") && aggregateResult[0].RowCategory.Equals("Composite"));
            Assert.True(aggregateResult[1].RowParity.Equals("Odd") && aggregateResult[1].RowCategory.Equals("None"));
            Assert.True(aggregateResult[2].RowParity.Equals("Odd") && aggregateResult[2].RowCategory.Equals("Prime"));
        }

        [Fact]
        public void GroupByWithResultSelector_Average_ByConstant()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1, (d2, d3) => new
            {
                AverageIntProp = d3.Average(d4 => d4.IntProp),
                AverageNullableIntProp = d3.Average(d4 => d4.NullableIntProp),
                AverageDoubleProp = d3.Average(d4 => d4.DoubleProp),
                AverageNullableDoubleProp = d3.Average(d4 => d4.NullableDoubleProp),
                AverageDecimalProp = d3.Average(d4 => d4.DecimalProp),
                AverageNullableDecimalProp = d3.Average(d4 => d4.NullableDecimalProp),
                AverageLongProp = d3.Average(d4 => d4.LongProp),
                AverageNullableLongProp = d3.Average(d4 => d4.NullableLongProp),
                AverageSingleProp = d3.Average(d4 => d4.SingleProp),
                AverageNullableSingleProp = d3.Average(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=aggregate(" +
                    "IntProp with average as AverageIntProp,NullableIntProp with average as AverageNullableIntProp," +
                    "DoubleProp with average as AverageDoubleProp,NullableDoubleProp with average as AverageNullableDoubleProp," +
                    "DecimalProp with average as AverageDecimalProp,NullableDecimalProp with average as AverageNullableDecimalProp," +
                    "LongProp with average as AverageLongProp,NullableLongProp with average as AverageNullableLongProp," +
                    "SingleProp with average as AverageSingleProp,NullableSingleProp with average as AverageNullableSingleProp)", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Average_ByConstant();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Single(aggregateResult);

            var singleResult = aggregateResult.First();

            Assert.Equal(101.2, singleResult.AverageIntProp);
            Assert.Equal(132.5, singleResult.AverageNullableIntProp);
            Assert.Equal(92.944, singleResult.AverageDoubleProp);
            Assert.Equal(133.505, singleResult.AverageNullableDoubleProp);
            Assert.Equal(111.88M, singleResult.AverageDecimalProp);
            Assert.Equal(98.425M, singleResult.AverageNullableDecimalProp);
            Assert.Equal(259.6, singleResult.AverageLongProp);
            Assert.Equal(248.25, singleResult.AverageNullableLongProp);
            Assert.Equal(66.758f, singleResult.AverageSingleProp);
            Assert.Equal(132.11f, singleResult.AverageNullableSingleProp);
        }

        [Fact]
        public void GroupByWithProjection_Average_ByConstant()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1).Select(d2 => new
            {
                AverageIntProp = d2.Average(d3 => d3.IntProp),
                AverageNullableIntProp = d2.Average(d3 => d3.NullableIntProp),
                AverageDoubleProp = d2.Average(d3 => d3.DoubleProp),
                AverageNullableDoubleProp = d2.Average(d3 => d3.NullableDoubleProp),
                AverageDecimalProp = d2.Average(d3 => d3.DecimalProp),
                AverageNullableDecimalProp = d2.Average(d3 => d3.NullableDecimalProp),
                AverageLongProp = d2.Average(d3 => d3.LongProp),
                AverageNullableLongProp = d2.Average(d3 => d3.NullableLongProp),
                AverageSingleProp = d2.Average(d3 => d3.SingleProp),
                AverageNullableSingleProp = d2.Average(d3 => d3.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=aggregate(" +
                    "IntProp with average as AverageIntProp,NullableIntProp with average as AverageNullableIntProp," +
                    "DoubleProp with average as AverageDoubleProp,NullableDoubleProp with average as AverageNullableDoubleProp," +
                    "DecimalProp with average as AverageDecimalProp,NullableDecimalProp with average as AverageNullableDecimalProp," +
                    "LongProp with average as AverageLongProp,NullableLongProp with average as AverageNullableLongProp," +
                    "SingleProp with average as AverageSingleProp,NullableSingleProp with average as AverageNullableSingleProp)", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Average_ByConstant();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Single(aggregateResult);

            var singleResult = aggregateResult.First();

            Assert.Equal(101.2, singleResult.AverageIntProp);
            Assert.Equal(132.5, singleResult.AverageNullableIntProp);
            Assert.Equal(92.944, singleResult.AverageDoubleProp);
            Assert.Equal(133.505, singleResult.AverageNullableDoubleProp);
            Assert.Equal(111.88M, singleResult.AverageDecimalProp);
            Assert.Equal(98.425M, singleResult.AverageNullableDecimalProp);
            Assert.Equal(259.6, singleResult.AverageLongProp);
            Assert.Equal(248.25, singleResult.AverageNullableLongProp);
            Assert.Equal(66.758f, singleResult.AverageSingleProp);
            Assert.Equal(132.11f, singleResult.AverageNullableSingleProp);
        }

        [Fact]
        public void GroupByWithResultSelector_Average_BySingleProperty()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.RowParity, (d2, d3) => new
            {
                RowParity = d2,
                AverageIntProp = d3.Average(d4 => d4.IntProp),
                AverageNullableIntProp = d3.Average(d4 => d4.NullableIntProp),
                AverageDoubleProp = d3.Average(d4 => d4.DoubleProp),
                AverageNullableDoubleProp = d3.Average(d4 => d4.NullableDoubleProp),
                AverageDecimalProp = d3.Average(d4 => d4.DecimalProp),
                AverageNullableDecimalProp = d3.Average(d4 => d4.NullableDecimalProp),
                AverageLongProp = d3.Average(d4 => d4.LongProp),
                AverageNullableLongProp = d3.Average(d4 => d4.NullableLongProp),
                AverageSingleProp = d3.Average(d4 => d4.SingleProp),
                AverageNullableSingleProp = d3.Average(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=groupby((RowParity),aggregate(" +
                    "IntProp with average as AverageIntProp,NullableIntProp with average as AverageNullableIntProp," +
                    "DoubleProp with average as AverageDoubleProp,NullableDoubleProp with average as AverageNullableDoubleProp," +
                    "DecimalProp with average as AverageDecimalProp,NullableDecimalProp with average as AverageNullableDecimalProp," +
                    "LongProp with average as AverageLongProp,NullableLongProp with average as AverageNullableLongProp," +
                    "SingleProp with average as AverageSingleProp,NullableSingleProp with average as AverageNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Average_BySingleProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Even", aggregateResult[0].RowParity);
            Assert.Equal("Odd", aggregateResult[1].RowParity);
        }

        [Fact]
        public void GroupByWithProjection_Average_BySingleProperty()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.RowParity).Select(d2 => new
            {
                RowParity = d2.Key,
                AverageIntProp = d2.Average(d3 => d3.IntProp),
                AverageNullableIntProp = d2.Average(d3 => d3.NullableIntProp),
                AverageDoubleProp = d2.Average(d3 => d3.DoubleProp),
                AverageNullableDoubleProp = d2.Average(d3 => d3.NullableDoubleProp),
                AverageDecimalProp = d2.Average(d3 => d3.DecimalProp),
                AverageNullableDecimalProp = d2.Average(d3 => d3.NullableDecimalProp),
                AverageLongProp = d2.Average(d3 => d3.LongProp),
                AverageNullableLongProp = d2.Average(d3 => d3.NullableLongProp),
                AverageSingleProp = d2.Average(d3 => d3.SingleProp),
                AverageNullableSingleProp = d2.Average(d3 => d3.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=groupby((RowParity),aggregate(" +
                    "IntProp with average as AverageIntProp,NullableIntProp with average as AverageNullableIntProp," +
                    "DoubleProp with average as AverageDoubleProp,NullableDoubleProp with average as AverageNullableDoubleProp," +
                    "DecimalProp with average as AverageDecimalProp,NullableDecimalProp with average as AverageNullableDecimalProp," +
                    "LongProp with average as AverageLongProp,NullableLongProp with average as AverageNullableLongProp," +
                    "SingleProp with average as AverageSingleProp,NullableSingleProp with average as AverageNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Average_BySingleProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Even", aggregateResult[0].RowParity);
            Assert.Equal("Odd", aggregateResult[1].RowParity);
        }

        [Fact]
        public void GroupByWithResultSelector_Average_ByMultipleProperties()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.RowParity, d1.RowCategory }, (d2, d3) => new
            {
                d2.RowParity,
                d2.RowCategory,
                AverageIntProp = d3.Average(d4 => d4.IntProp),
                AverageNullableIntProp = d3.Average(d4 => d4.NullableIntProp),
                AverageDoubleProp = d3.Average(d4 => d4.DoubleProp),
                AverageNullableDoubleProp = d3.Average(d4 => d4.NullableDoubleProp),
                AverageDecimalProp = d3.Average(d4 => d4.DecimalProp),
                AverageNullableDecimalProp = d3.Average(d4 => d4.NullableDecimalProp),
                AverageLongProp = d3.Average(d4 => d4.LongProp),
                AverageNullableLongProp = d3.Average(d4 => d4.NullableLongProp),
                AverageSingleProp = d3.Average(d4 => d4.SingleProp),
                AverageNullableSingleProp = d3.Average(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=groupby((RowParity,RowCategory),aggregate(" +
                    "IntProp with average as AverageIntProp,NullableIntProp with average as AverageNullableIntProp," +
                    "DoubleProp with average as AverageDoubleProp,NullableDoubleProp with average as AverageNullableDoubleProp," +
                    "DecimalProp with average as AverageDecimalProp,NullableDecimalProp with average as AverageNullableDecimalProp," +
                    "LongProp with average as AverageLongProp,NullableLongProp with average as AverageNullableLongProp," +
                    "SingleProp with average as AverageSingleProp,NullableSingleProp with average as AverageNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Average_ByMultipleProperties();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(3, aggregateResult.Length);
            Assert.True(aggregateResult[0].RowParity.Equals("Even") && aggregateResult[0].RowCategory.Equals("Composite"));
            Assert.True(aggregateResult[1].RowParity.Equals("Odd") && aggregateResult[1].RowCategory.Equals("None"));
            Assert.True(aggregateResult[2].RowParity.Equals("Odd") && aggregateResult[2].RowCategory.Equals("Prime"));
        }

        [Fact]
        public void GroupByWithProjection_Average_ByMultipleProperties()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.RowParity, d1.RowCategory }).Select(d2 => new
            {
                d2.Key.RowParity,
                d2.Key.RowCategory,
                AverageIntProp = d2.Average(d3 => d3.IntProp),
                AverageNullableIntProp = d2.Average(d3 => d3.NullableIntProp),
                AverageDoubleProp = d2.Average(d3 => d3.DoubleProp),
                AverageNullableDoubleProp = d2.Average(d3 => d3.NullableDoubleProp),
                AverageDecimalProp = d2.Average(d3 => d3.DecimalProp),
                AverageNullableDecimalProp = d2.Average(d3 => d3.NullableDecimalProp),
                AverageLongProp = d2.Average(d3 => d3.LongProp),
                AverageNullableLongProp = d2.Average(d3 => d3.NullableLongProp),
                AverageSingleProp = d2.Average(d3 => d3.SingleProp),
                AverageNullableSingleProp = d2.Average(d3 => d3.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=groupby((RowParity,RowCategory),aggregate(" +
                    "IntProp with average as AverageIntProp,NullableIntProp with average as AverageNullableIntProp," +
                    "DoubleProp with average as AverageDoubleProp,NullableDoubleProp with average as AverageNullableDoubleProp," +
                    "DecimalProp with average as AverageDecimalProp,NullableDecimalProp with average as AverageNullableDecimalProp," +
                    "LongProp with average as AverageLongProp,NullableLongProp with average as AverageNullableLongProp," +
                    "SingleProp with average as AverageSingleProp,NullableSingleProp with average as AverageNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Average_ByMultipleProperties();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(3, aggregateResult.Length);
            Assert.True(aggregateResult[0].RowParity.Equals("Even") && aggregateResult[0].RowCategory.Equals("Composite"));
            Assert.True(aggregateResult[1].RowParity.Equals("Odd") && aggregateResult[1].RowCategory.Equals("None"));
            Assert.True(aggregateResult[2].RowParity.Equals("Odd") && aggregateResult[2].RowCategory.Equals("Prime"));
        }

        [Fact]
        public void GroupByWithResultSelector_Min_ByConstant()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1, (d2, d3) => new
            {
                MinIntProp = d3.Min(d4 => d4.IntProp),
                MinNullableIntProp = d3.Min(d4 => d4.NullableIntProp),
                MinDoubleProp = d3.Min(d4 => d4.DoubleProp),
                MinNullableDoubleProp = d3.Min(d4 => d4.NullableDoubleProp),
                MinDecimalProp = d3.Min(d4 => d4.DecimalProp),
                MinNullableDecimalProp = d3.Min(d4 => d4.NullableDecimalProp),
                MinLongProp = d3.Min(d4 => d4.LongProp),
                MinNullableLongProp = d3.Min(d4 => d4.NullableLongProp),
                MinSingleProp = d3.Min(d4 => d4.SingleProp),
                MinNullableSingleProp = d3.Min(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=aggregate(" +
                    "IntProp with min as MinIntProp,NullableIntProp with min as MinNullableIntProp," +
                    "DoubleProp with min as MinDoubleProp,NullableDoubleProp with min as MinNullableDoubleProp," +
                    "DecimalProp with min as MinDecimalProp,NullableDecimalProp with min as MinNullableDecimalProp," +
                    "LongProp with min as MinLongProp,NullableLongProp with min as MinNullableLongProp," +
                    "SingleProp with min as MinSingleProp,NullableSingleProp with min as MinNullableSingleProp)", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Min_ByConstant();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Single(aggregateResult);

            var singleResult = aggregateResult.First();

            Assert.Equal(63, singleResult.MinIntProp);
            Assert.Equal(34, singleResult.MinNullableIntProp);
            Assert.Equal(2.34, singleResult.MinDoubleProp);
            Assert.Equal(16.1, singleResult.MinNullableDoubleProp);
            Assert.Equal(42.70M, singleResult.MinDecimalProp);
            Assert.Equal(12.90M, singleResult.MinNullableDecimalProp);
            Assert.Equal(220L, singleResult.MinLongProp);
            Assert.Equal(201L, singleResult.MinNullableLongProp);
            Assert.Equal(1.29f, singleResult.MinSingleProp);
            Assert.Equal(81.94f, singleResult.MinNullableSingleProp);
        }

        [Fact]
        public void GroupByWithProjection_Min_ByConstant()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1).Select(d2 => new
            {
                MinIntProp = d2.Min(d3 => d3.IntProp),
                MinNullableIntProp = d2.Min(d3 => d3.NullableIntProp),
                MinDoubleProp = d2.Min(d3 => d3.DoubleProp),
                MinNullableDoubleProp = d2.Min(d3 => d3.NullableDoubleProp),
                MinDecimalProp = d2.Min(d3 => d3.DecimalProp),
                MinNullableDecimalProp = d2.Min(d3 => d3.NullableDecimalProp),
                MinLongProp = d2.Min(d3 => d3.LongProp),
                MinNullableLongProp = d2.Min(d3 => d3.NullableLongProp),
                MinSingleProp = d2.Min(d3 => d3.SingleProp),
                MinNullableSingleProp = d2.Min(d3 => d3.NullableSingleProp),
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=aggregate(" +
                "IntProp with min as MinIntProp,NullableIntProp with min as MinNullableIntProp," +
                "DoubleProp with min as MinDoubleProp,NullableDoubleProp with min as MinNullableDoubleProp," +
                "DecimalProp with min as MinDecimalProp,NullableDecimalProp with min as MinNullableDecimalProp," +
                "LongProp with min as MinLongProp,NullableLongProp with min as MinNullableLongProp," +
                "SingleProp with min as MinSingleProp,NullableSingleProp with min as MinNullableSingleProp)", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Min_ByConstant();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Single(aggregateResult);

            var singleResult = aggregateResult.First();

            Assert.Equal(63, singleResult.MinIntProp);
            Assert.Equal(34, singleResult.MinNullableIntProp);
            Assert.Equal(2.34, singleResult.MinDoubleProp);
            Assert.Equal(16.1, singleResult.MinNullableDoubleProp);
            Assert.Equal(42.70M, singleResult.MinDecimalProp);
            Assert.Equal(12.90M, singleResult.MinNullableDecimalProp);
            Assert.Equal(220L, singleResult.MinLongProp);
            Assert.Equal(201L, singleResult.MinNullableLongProp);
            Assert.Equal(1.29f, singleResult.MinSingleProp);
            Assert.Equal(81.94f, singleResult.MinNullableSingleProp);
        }

        [Fact]
        public void GroupByWithResultSelector_Min_BySingleProperty()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.RowParity, (d2, d3) => new
            {
                RowParity = d2,
                MinIntProp = d3.Min(d4 => d4.IntProp),
                MinNullableIntProp = d3.Min(d4 => d4.NullableIntProp),
                MinDoubleProp = d3.Min(d4 => d4.DoubleProp),
                MinNullableDoubleProp = d3.Min(d4 => d4.NullableDoubleProp),
                MinDecimalProp = d3.Min(d4 => d4.DecimalProp),
                MinNullableDecimalProp = d3.Min(d4 => d4.NullableDecimalProp),
                MinLongProp = d3.Min(d4 => d4.LongProp),
                MinNullableLongProp = d3.Min(d4 => d4.NullableLongProp),
                MinSingleProp = d3.Min(d4 => d4.SingleProp),
                MinNullableSingleProp = d3.Min(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=groupby((RowParity),aggregate(" +
                    "IntProp with min as MinIntProp,NullableIntProp with min as MinNullableIntProp," +
                    "DoubleProp with min as MinDoubleProp,NullableDoubleProp with min as MinNullableDoubleProp," +
                    "DecimalProp with min as MinDecimalProp,NullableDecimalProp with min as MinNullableDecimalProp," +
                    "LongProp with min as MinLongProp,NullableLongProp with min as MinNullableLongProp," +
                    "SingleProp with min as MinSingleProp,NullableSingleProp with min as MinNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Min_BySingleProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Even", aggregateResult[0].RowParity);
            Assert.Equal("Odd", aggregateResult[1].RowParity);
        }

        [Fact]
        public void GroupByWithProjection_Min_BySingleProperty()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.RowParity).Select(d2 => new
            {
                RowParity = d2.Key,
                MinIntProp = d2.Min(d3 => d3.IntProp),
                MinNullableIntProp = d2.Min(d3 => d3.NullableIntProp),
                MinDoubleProp = d2.Min(d3 => d3.DoubleProp),
                MinNullableDoubleProp = d2.Min(d3 => d3.NullableDoubleProp),
                MinDecimalProp = d2.Min(d3 => d3.DecimalProp),
                MinNullableDecimalProp = d2.Min(d3 => d3.NullableDecimalProp),
                MinLongProp = d2.Min(d3 => d3.LongProp),
                MinNullableLongProp = d2.Min(d3 => d3.NullableLongProp),
                MinSingleProp = d2.Min(d3 => d3.SingleProp),
                MinNullableSingleProp = d2.Min(d3 => d3.NullableSingleProp),
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=groupby((RowParity),aggregate(" +
                "IntProp with min as MinIntProp,NullableIntProp with min as MinNullableIntProp," +
                "DoubleProp with min as MinDoubleProp,NullableDoubleProp with min as MinNullableDoubleProp," +
                "DecimalProp with min as MinDecimalProp,NullableDecimalProp with min as MinNullableDecimalProp," +
                "LongProp with min as MinLongProp,NullableLongProp with min as MinNullableLongProp," +
                "SingleProp with min as MinSingleProp,NullableSingleProp with min as MinNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Min_BySingleProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Even", aggregateResult[0].RowParity);
            Assert.Equal("Odd", aggregateResult[1].RowParity);
        }

        [Fact]
        public void GroupByWithResultSelector_Min_ByMultiProperties()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.RowParity, d1.RowCategory }, (d2, d3) => new
            {
                d2.RowParity,
                d2.RowCategory,
                MinIntProp = d3.Min(d4 => d4.IntProp),
                MinNullableIntProp = d3.Min(d4 => d4.NullableIntProp),
                MinDoubleProp = d3.Min(d4 => d4.DoubleProp),
                MinNullableDoubleProp = d3.Min(d4 => d4.NullableDoubleProp),
                MinDecimalProp = d3.Min(d4 => d4.DecimalProp),
                MinNullableDecimalProp = d3.Min(d4 => d4.NullableDecimalProp),
                MinLongProp = d3.Min(d4 => d4.LongProp),
                MinNullableLongProp = d3.Min(d4 => d4.NullableLongProp),
                MinSingleProp = d3.Min(d4 => d4.SingleProp),
                MinNullableSingleProp = d3.Min(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=groupby((RowParity,RowCategory),aggregate(" +
                    "IntProp with min as MinIntProp,NullableIntProp with min as MinNullableIntProp," +
                    "DoubleProp with min as MinDoubleProp,NullableDoubleProp with min as MinNullableDoubleProp," +
                    "DecimalProp with min as MinDecimalProp,NullableDecimalProp with min as MinNullableDecimalProp," +
                    "LongProp with min as MinLongProp,NullableLongProp with min as MinNullableLongProp," +
                    "SingleProp with min as MinSingleProp,NullableSingleProp with min as MinNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Min_ByMultipleProperties();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(3, aggregateResult.Length);
            Assert.True(aggregateResult[0].RowParity.Equals("Even") && aggregateResult[0].RowCategory.Equals("Composite"));
            Assert.True(aggregateResult[1].RowParity.Equals("Odd") && aggregateResult[1].RowCategory.Equals("None"));
            Assert.True(aggregateResult[2].RowParity.Equals("Odd") && aggregateResult[2].RowCategory.Equals("Prime"));
        }

        [Fact]
        public void GroupByWithProjection_Min_ByMultiProperties()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.RowParity, d1.RowCategory }).Select(d2 => new
            {
                d2.Key.RowParity,
                d2.Key.RowCategory,
                MinIntProp = d2.Min(d3 => d3.IntProp),
                MinNullableIntProp = d2.Min(d3 => d3.NullableIntProp),
                MinDoubleProp = d2.Min(d3 => d3.DoubleProp),
                MinNullableDoubleProp = d2.Min(d3 => d3.NullableDoubleProp),
                MinDecimalProp = d2.Min(d3 => d3.DecimalProp),
                MinNullableDecimalProp = d2.Min(d3 => d3.NullableDecimalProp),
                MinLongProp = d2.Min(d3 => d3.LongProp),
                MinNullableLongProp = d2.Min(d3 => d3.NullableLongProp),
                MinSingleProp = d2.Min(d3 => d3.SingleProp),
                MinNullableSingleProp = d2.Min(d3 => d3.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=groupby((RowParity,RowCategory),aggregate(" +
                    "IntProp with min as MinIntProp,NullableIntProp with min as MinNullableIntProp," +
                    "DoubleProp with min as MinDoubleProp,NullableDoubleProp with min as MinNullableDoubleProp," +
                    "DecimalProp with min as MinDecimalProp,NullableDecimalProp with min as MinNullableDecimalProp," +
                    "LongProp with min as MinLongProp,NullableLongProp with min as MinNullableLongProp," +
                    "SingleProp with min as MinSingleProp,NullableSingleProp with min as MinNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Min_ByMultipleProperties();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(3, aggregateResult.Length);
            Assert.True(aggregateResult[0].RowParity.Equals("Even") && aggregateResult[0].RowCategory.Equals("Composite"));
            Assert.True(aggregateResult[1].RowParity.Equals("Odd") && aggregateResult[1].RowCategory.Equals("None"));
            Assert.True(aggregateResult[2].RowParity.Equals("Odd") && aggregateResult[2].RowCategory.Equals("Prime"));
        }

        [Fact]
        public void GroupByWithResultSelector_Max_ByConstant()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1, (d2, d3) => new
            {
                MaxIntProp = d3.Max(d4 => d4.IntProp),
                MaxNullableIntProp = d3.Max(d4 => d4.NullableIntProp),
                MaxDoubleProp = d3.Max(d4 => d4.DoubleProp),
                MaxNullableDoubleProp = d3.Max(d4 => d4.NullableDoubleProp),
                MaxDecimalProp = d3.Max(d4 => d4.DecimalProp),
                MaxNullableDecimalProp = d3.Max(d4 => d4.NullableDecimalProp),
                MaxLongProp = d3.Max(d4 => d4.LongProp),
                MaxNullableLongProp = d3.Max(d4 => d4.NullableLongProp),
                MaxSingleProp = d3.Max(d4 => d4.SingleProp),
                MaxNullableSingleProp = d3.Max(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=aggregate(" +
                "IntProp with max as MaxIntProp,NullableIntProp with max as MaxNullableIntProp," +
                "DoubleProp with max as MaxDoubleProp,NullableDoubleProp with max as MaxNullableDoubleProp," +
                "DecimalProp with max as MaxDecimalProp,NullableDecimalProp with max as MaxNullableDecimalProp," +
                "LongProp with max as MaxLongProp,NullableLongProp with max as MaxNullableLongProp," +
                "SingleProp with max as MaxSingleProp,NullableSingleProp with max as MaxNullableSingleProp)", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Max_ByConstant();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Single(aggregateResult);

            var singleResult = aggregateResult.First();

            Assert.Equal(141, singleResult.MaxIntProp);
            Assert.Equal(199, singleResult.MaxNullableIntProp);
            Assert.Equal(155.85, singleResult.MaxDoubleProp);
            Assert.Equal(178.49, singleResult.MaxNullableDoubleProp);
            Assert.Equal(173.90M, singleResult.MaxDecimalProp);
            Assert.Equal(157.30M, singleResult.MaxNullableDecimalProp);
            Assert.Equal(300L, singleResult.MaxLongProp);
            Assert.Equal(295L, singleResult.MaxNullableLongProp);
            Assert.Equal(171.22f, singleResult.MaxSingleProp);
            Assert.Equal(174.99f, singleResult.MaxNullableSingleProp);
        }

        [Fact]
        public void GroupByWithProjection_Max_ByConstant()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1).Select(d2 => new
            {
                MaxIntProp = d2.Max(d3 => d3.IntProp),
                MaxNullableIntProp = d2.Max(d3 => d3.NullableIntProp),
                MaxDoubleProp = d2.Max(d3 => d3.DoubleProp),
                MaxNullableDoubleProp = d2.Max(d3 => d3.NullableDoubleProp),
                MaxDecimalProp = d2.Max(d3 => d3.DecimalProp),
                MaxNullableDecimalProp = d2.Max(d3 => d3.NullableDecimalProp),
                MaxLongProp = d2.Max(d3 => d3.LongProp),
                MaxNullableLongProp = d2.Max(d3 => d3.NullableLongProp),
                MaxSingleProp = d2.Max(d3 => d3.SingleProp),
                MaxNullableSingleProp = d2.Max(d3 => d3.NullableSingleProp),
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=aggregate(" +
                "IntProp with max as MaxIntProp,NullableIntProp with max as MaxNullableIntProp," +
                "DoubleProp with max as MaxDoubleProp,NullableDoubleProp with max as MaxNullableDoubleProp," +
                "DecimalProp with max as MaxDecimalProp,NullableDecimalProp with max as MaxNullableDecimalProp," +
                "LongProp with max as MaxLongProp,NullableLongProp with max as MaxNullableLongProp," +
                "SingleProp with max as MaxSingleProp,NullableSingleProp with max as MaxNullableSingleProp)", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Max_ByConstant();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Single(aggregateResult);

            var singleResult = aggregateResult.First();

            Assert.Equal(141, singleResult.MaxIntProp);
            Assert.Equal(199, singleResult.MaxNullableIntProp);
            Assert.Equal(155.85, singleResult.MaxDoubleProp);
            Assert.Equal(178.49, singleResult.MaxNullableDoubleProp);
            Assert.Equal(173.90M, singleResult.MaxDecimalProp);
            Assert.Equal(157.30M, singleResult.MaxNullableDecimalProp);
            Assert.Equal(300L, singleResult.MaxLongProp);
            Assert.Equal(295L, singleResult.MaxNullableLongProp);
            Assert.Equal(171.22f, singleResult.MaxSingleProp);
            Assert.Equal(174.99f, singleResult.MaxNullableSingleProp);
        }

        [Fact]
        public void GroupByWithResultSelector_Max_BySingleProperty()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.RowParity, (d2, d3) => new
            {
                RowParity = d2,
                MaxIntProp = d3.Max(d4 => d4.IntProp),
                MaxNullableIntProp = d3.Max(d4 => d4.NullableIntProp),
                MaxDoubleProp = d3.Max(d4 => d4.DoubleProp),
                MaxNullableDoubleProp = d3.Max(d4 => d4.NullableDoubleProp),
                MaxDecimalProp = d3.Max(d4 => d4.DecimalProp),
                MaxNullableDecimalProp = d3.Max(d4 => d4.NullableDecimalProp),
                MaxLongProp = d3.Max(d4 => d4.LongProp),
                MaxNullableLongProp = d3.Max(d4 => d4.NullableLongProp),
                MaxSingleProp = d3.Max(d4 => d4.SingleProp),
                MaxNullableSingleProp = d3.Max(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=groupby((RowParity),aggregate(" +
                "IntProp with max as MaxIntProp,NullableIntProp with max as MaxNullableIntProp," +
                "DoubleProp with max as MaxDoubleProp,NullableDoubleProp with max as MaxNullableDoubleProp," +
                "DecimalProp with max as MaxDecimalProp,NullableDecimalProp with max as MaxNullableDecimalProp," +
                "LongProp with max as MaxLongProp,NullableLongProp with max as MaxNullableLongProp," +
                "SingleProp with max as MaxSingleProp,NullableSingleProp with max as MaxNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Max_BySingleProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Even", aggregateResult[0].RowParity);
            Assert.Equal("Odd", aggregateResult[1].RowParity);
        }

        [Fact]
        public void GroupByWithProjection_Max_BySingleProperty()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.RowParity).Select(d2 => new
            {
                RowParity = d2.Key,
                MaxIntProp = d2.Max(d3 => d3.IntProp),
                MaxNullableIntProp = d2.Max(d3 => d3.NullableIntProp),
                MaxDoubleProp = d2.Max(d3 => d3.DoubleProp),
                MaxNullableDoubleProp = d2.Max(d3 => d3.NullableDoubleProp),
                MaxDecimalProp = d2.Max(d3 => d3.DecimalProp),
                MaxNullableDecimalProp = d2.Max(d3 => d3.NullableDecimalProp),
                MaxLongProp = d2.Max(d3 => d3.LongProp),
                MaxNullableLongProp = d2.Max(d3 => d3.NullableLongProp),
                MaxSingleProp = d2.Max(d3 => d3.SingleProp),
                MaxNullableSingleProp = d2.Max(d3 => d3.NullableSingleProp),
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=groupby((RowParity),aggregate(" +
                "IntProp with max as MaxIntProp,NullableIntProp with max as MaxNullableIntProp," +
                "DoubleProp with max as MaxDoubleProp,NullableDoubleProp with max as MaxNullableDoubleProp," +
                "DecimalProp with max as MaxDecimalProp,NullableDecimalProp with max as MaxNullableDecimalProp," +
                "LongProp with max as MaxLongProp,NullableLongProp with max as MaxNullableLongProp," +
                "SingleProp with max as MaxSingleProp,NullableSingleProp with max as MaxNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Max_BySingleProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Even", aggregateResult[0].RowParity);
            Assert.Equal("Odd", aggregateResult[1].RowParity);
        }

        [Fact]
        public void GroupByWithResultSelector_Max_ByMultipleProperties()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.RowParity, d1.RowCategory }, (d2, d3) => new
            {
                d2.RowParity,
                d2.RowCategory,
                MaxIntProp = d3.Max(d4 => d4.IntProp),
                MaxNullableIntProp = d3.Max(d4 => d4.NullableIntProp),
                MaxDoubleProp = d3.Max(d4 => d4.DoubleProp),
                MaxNullableDoubleProp = d3.Max(d4 => d4.NullableDoubleProp),
                MaxDecimalProp = d3.Max(d4 => d4.DecimalProp),
                MaxNullableDecimalProp = d3.Max(d4 => d4.NullableDecimalProp),
                MaxLongProp = d3.Max(d4 => d4.LongProp),
                MaxNullableLongProp = d3.Max(d4 => d4.NullableLongProp),
                MaxSingleProp = d3.Max(d4 => d4.SingleProp),
                MaxNullableSingleProp = d3.Max(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=groupby((RowParity,RowCategory),aggregate(" +
                "IntProp with max as MaxIntProp,NullableIntProp with max as MaxNullableIntProp," +
                "DoubleProp with max as MaxDoubleProp,NullableDoubleProp with max as MaxNullableDoubleProp," +
                "DecimalProp with max as MaxDecimalProp,NullableDecimalProp with max as MaxNullableDecimalProp," +
                "LongProp with max as MaxLongProp,NullableLongProp with max as MaxNullableLongProp," +
                "SingleProp with max as MaxSingleProp,NullableSingleProp with max as MaxNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Max_ByMultipleProperties();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(3, aggregateResult.Length);
            Assert.True(aggregateResult[0].RowParity.Equals("Even") && aggregateResult[0].RowCategory.Equals("Composite"));
            Assert.True(aggregateResult[1].RowParity.Equals("Odd") && aggregateResult[1].RowCategory.Equals("None"));
            Assert.True(aggregateResult[2].RowParity.Equals("Odd") && aggregateResult[2].RowCategory.Equals("Prime"));
        }

        [Fact]
        public void GroupByWithProjection_Max_ByMultipleProperties()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.RowParity, d1.RowCategory }).Select(d2 => new
            {
                d2.Key.RowParity,
                d2.Key.RowCategory,
                MaxIntProp = d2.Max(d3 => d3.IntProp),
                MaxNullableIntProp = d2.Max(d3 => d3.NullableIntProp),
                MaxDoubleProp = d2.Max(d3 => d3.DoubleProp),
                MaxNullableDoubleProp = d2.Max(d3 => d3.NullableDoubleProp),
                MaxDecimalProp = d2.Max(d3 => d3.DecimalProp),
                MaxNullableDecimalProp = d2.Max(d3 => d3.NullableDecimalProp),
                MaxLongProp = d2.Max(d3 => d3.LongProp),
                MaxNullableLongProp = d2.Max(d3 => d3.NullableLongProp),
                MaxSingleProp = d2.Max(d3 => d3.SingleProp),
                MaxNullableSingleProp = d2.Max(d3 => d3.NullableSingleProp),
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=groupby((RowParity,RowCategory),aggregate(" +
                "IntProp with max as MaxIntProp,NullableIntProp with max as MaxNullableIntProp," +
                "DoubleProp with max as MaxDoubleProp,NullableDoubleProp with max as MaxNullableDoubleProp," +
                "DecimalProp with max as MaxDecimalProp,NullableDecimalProp with max as MaxNullableDecimalProp," +
                "LongProp with max as MaxLongProp,NullableLongProp with max as MaxNullableLongProp," +
                "SingleProp with max as MaxSingleProp,NullableSingleProp with max as MaxNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Max_ByMultipleProperties();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(3, aggregateResult.Length);
            Assert.True(aggregateResult[0].RowParity.Equals("Even") && aggregateResult[0].RowCategory.Equals("Composite"));
            Assert.True(aggregateResult[1].RowParity.Equals("Odd") && aggregateResult[1].RowCategory.Equals("None"));
            Assert.True(aggregateResult[2].RowParity.Equals("Odd") && aggregateResult[2].RowCategory.Equals("Prime"));
        }

        [Fact]
        public void GroupByWithResultSelector_Count_ByConstant()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1, (d2, d3) => new
            {
                Count = d3.Count(),
                CountDistinct = d3.CountDistinct(d4 => d4.RowCategory)
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=aggregate(" +
                "$count as Count,RowCategory with countdistinct as CountDistinct)", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Count_ByConstant();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Single(aggregateResult);
            Assert.Equal(5, aggregateResult[0].Count);
            Assert.Equal(3, aggregateResult[0].CountDistinct);
        }

        [Fact]
        public void GroupByWithProjection_Count_ByConstant()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1).Select(d2 => new
            {
                Count = d2.Count(),
                CountDistinct = d2.CountDistinct(d3 => d3.RowCategory)
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=aggregate(" +
                "$count as Count,RowCategory with countdistinct as CountDistinct)", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Count_ByConstant();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Single(aggregateResult);
            Assert.Equal(5, aggregateResult[0].Count);
            Assert.Equal(3, aggregateResult[0].CountDistinct);
        }

        [Fact]
        public void GroupByWithResultSelector_Count_BySingleProperty()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.RowParity, (d2, d3) => new
            {
                RowParity = d2,
                Count = d3.Count(),
                CountDistinct = d3.CountDistinct(d4 => d4.RowCategory)
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=groupby((RowParity),aggregate(" +
                "$count as Count,RowCategory with countdistinct as CountDistinct))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Count_BySingleProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Even", aggregateResult[0].RowParity);
            Assert.Equal("Odd", aggregateResult[1].RowParity);
        }

        [Fact]
        public void GroupByWithProjection_Count_BySingleProperty()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.RowParity).Select(d2 => new
            {
                RowParity = d2.Key,
                Count = d2.Count(),
                CountDistinct = d2.CountDistinct(d3 => d3.RowCategory)
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=groupby((RowParity),aggregate(" +
                "$count as Count,RowCategory with countdistinct as CountDistinct))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Count_BySingleProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Even", aggregateResult[0].RowParity);
            Assert.Equal("Odd", aggregateResult[1].RowParity);
        }

        [Fact]
        public void GroupByWithResultSelector_Count_ByMultipleProperties()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.RowParity, d1.RowCategory }, (d2, d3) => new
            {
                d2.RowParity,
                d2.RowCategory,
                Count = d3.Count(),
                CountDistinct = d3.CountDistinct(d4 => d4.RowCategory)
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=groupby((RowParity,RowCategory),aggregate(" +
                "$count as Count,RowCategory with countdistinct as CountDistinct))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Count_ByMultipleProperties();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(3, aggregateResult.Length);
            Assert.True(aggregateResult[0].RowParity.Equals("Even") && aggregateResult[0].RowCategory.Equals("Composite"));
            Assert.True(aggregateResult[1].RowParity.Equals("Odd") && aggregateResult[1].RowCategory.Equals("None"));
            Assert.True(aggregateResult[2].RowParity.Equals("Odd") && aggregateResult[2].RowCategory.Equals("Prime"));
        }

        [Fact]
        public void GroupByWithProjection_Count_ByMultipleProperties()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.RowParity, d1.RowCategory }).Select(d2 => new
            {
                d2.Key.RowParity,
                d2.Key.RowCategory,
                Count = d2.Count(),
                CountDistinct = d2.CountDistinct(d3 => d3.RowCategory)
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=groupby((RowParity,RowCategory),aggregate(" +
                "$count as Count,RowCategory with countdistinct as CountDistinct))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Count_ByMultipleProperties();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(3, aggregateResult.Length);
            Assert.True(aggregateResult[0].RowParity.Equals("Even") && aggregateResult[0].RowCategory.Equals("Composite"));
            Assert.True(aggregateResult[1].RowParity.Equals("Odd") && aggregateResult[1].RowCategory.Equals("None"));
            Assert.True(aggregateResult[2].RowParity.Equals("Odd") && aggregateResult[2].RowCategory.Equals("Prime"));
        }

        [Fact]
        public void GroupByWithResultSelector_WithMixedAggregations()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.RowParity, (d2, d3) => new
            {
                RowParity = d2,
                SumIntProp = d3.Sum(d4 => d4.IntProp),
                AverageDoubleProp = d3.Average(d4 => d4.DoubleProp),
                MinDecimalProp = d3.Min(d4 => d4.DecimalProp),
                MaxLongProp = d3.Max(d4 => d4.LongProp),
                Count = d3.Count(),
                CountDistinct = d3.CountDistinct(d4 => d4.RowCategory)
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=groupby((RowParity),aggregate(" +
                "IntProp with sum as SumIntProp,DoubleProp with average as AverageDoubleProp," +
                "DecimalProp with min as MinDecimalProp,LongProp with max as MaxLongProp," +
                "$count as Count,RowCategory with countdistinct as CountDistinct))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_WithMixedAggregations();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Even", aggregateResult[0].RowParity);
            Assert.Equal("Odd", aggregateResult[1].RowParity);
        }

        [Fact]
        public void GroupByWithProjection_WithMixedAggregations()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.RowParity).Select(d2 => new
            {
                RowParity = d2.Key,
                SumIntProp = d2.Sum(d3 => d3.IntProp),
                AverageDoubleProp = d2.Average(d3 => d3.DoubleProp),
                MinDecimalProp = d2.Min(d3 => d3.DecimalProp),
                MaxLongProp = d2.Max(d3 => d3.LongProp),
                Count = d2.Count(),
                CountDistinct = d2.CountDistinct(d3 => d3.RowCategory)
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=groupby((RowParity),aggregate(" +
                "IntProp with sum as SumIntProp,DoubleProp with average as AverageDoubleProp," +
                "DecimalProp with min as MinDecimalProp,LongProp with max as MaxLongProp," +
                "$count as Count,RowCategory with countdistinct as CountDistinct))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_WithMixedAggregations();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Even", aggregateResult[0].RowParity);
            Assert.Equal("Odd", aggregateResult[1].RowParity);
        }

        [Fact]
        public void GroupByWithResultSelector_UsingMemberInitialization()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.RowParity, (d2, d3) => new NumbersGroupedResult
            {
                RowParity = d2,
                SumIntProp = d3.Sum(d4 => d4.IntProp),
                AverageDoubleProp = d3.Average(d4 => d4.DoubleProp),
                MinDecimalProp = d3.Min(d4 => d4.DecimalProp),
                MaxLongProp = d3.Max(d4 => d4.LongProp),
                Count = d3.Count(),
                CountDistinct = d3.CountDistinct(d4 => d4.RowCategory)
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=groupby((RowParity),aggregate(" +
                "IntProp with sum as SumIntProp,DoubleProp with average as AverageDoubleProp," +
                "DecimalProp with min as MinDecimalProp,LongProp with max as MaxLongProp," +
                "$count as Count,RowCategory with countdistinct as CountDistinct))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_WithMixedAggregations();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Even", aggregateResult[0].RowParity);
            Assert.Equal("Odd", aggregateResult[1].RowParity);
        }

        [Fact]
        public void GroupByWithProjection_UsingMemberInitialization()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.RowParity).Select(d2 => new NumbersGroupedResult
            {
                RowParity = d2.Key,
                SumIntProp = d2.Sum(d3 => d3.IntProp),
                AverageDoubleProp = d2.Average(d3 => d3.DoubleProp),
                MinDecimalProp = d2.Min(d3 => d3.DecimalProp),
                MaxLongProp = d2.Max(d3 => d3.LongProp),
                Count = d2.Count(),
                CountDistinct = d2.CountDistinct(d3 => d3.RowCategory)
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=groupby((RowParity),aggregate(" +
                "IntProp with sum as SumIntProp,DoubleProp with average as AverageDoubleProp," +
                "DecimalProp with min as MinDecimalProp,LongProp with max as MaxLongProp," +
                "$count as Count,RowCategory with countdistinct as CountDistinct))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_WithMixedAggregations();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Even", aggregateResult[0].RowParity);
            Assert.Equal("Odd", aggregateResult[1].RowParity);
        }

        [Fact]
        public void CountDistinct_ExpressionTranslatedToExpectedUri()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            MethodInfo countDistinctMethod = GetCountDistinctMethod();

            PropertyInfo propertyInfo = queryable.ElementType.GetProperty("RowParity");
            ParameterExpression parameterExpr = Expression.Parameter(queryable.ElementType, "d1");
            LambdaExpression selector = Expression.Lambda(Expression.MakeMemberAccess(parameterExpr, propertyInfo), parameterExpr);

            MethodCallExpression methodCallExpr = Expression.Call(
                            null,
                            countDistinctMethod.MakeGenericMethod(new Type[] { queryable.ElementType, propertyInfo.PropertyType }),
                            new[] { queryable.Expression, Expression.Quote(selector) });

            // Call factory method for creating DataServiceOrderedQuery based on Linq expression
            var query = new DataServiceQueryProvider(dsContext).CreateQuery(methodCallExpr);

            // Verify expected aggregate Uri
            string expectedAggregateUri = serviceUri + '/' + numbersEntitySetName + "?$apply=aggregate(RowParity with countdistinct as CountDistinctRowParity)";
            Assert.Equal(expectedAggregateUri, query.ToString());
        }

        [Fact]
        public void CountDistinct_ReturnsExpectedResult()
        {
            DataServiceQuery<Number> queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            MockCountDistinct();

            int countDistinct = queryable.CountDistinct(d1 => d1.RowParity);

            Assert.Equal(3, countDistinct);
        }

        [Fact]
        public void CountDistinct_TargetingNavProperty()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            MockCountDistinct_TargetingNavProperty();

            int countDistinct = queryable.CountDistinct(d1 => d1.Customer.Country);

            Assert.Equal(2, countDistinct);
        }

        [Fact]
        public void GroupByWithResultSelector_BySingleNavProperty()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.Product.Color,
                (d1, d2) => new
                {
                    Color = d1,
                    SumAmount = d2.Sum(d3 => d3.Amount),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinAmount = d2.Min(d3 => d3.Amount),
                    MaxAmount = d2.Max(d3 => d3.Amount)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Color),aggregate(" +
                "Amount with sum as SumAmount,Amount with average as AvgAmount," +
                "Amount with min as MinAmount,Amount with max as MaxAmount))", serviceUri),
                aggregateQuery.ToString());

            MockGroupBySingleNavProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
        }

        [Fact]
        public void GroupByWithProjection_BySingleNavProperty()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.Product.Color)
                .Select(d2 => new
                {
                    Color = d2.Key,
                    SumAmount = d2.Sum(d3 => d3.Amount),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinAmount = d2.Min(d3 => d3.Amount),
                    MaxAmount = d2.Max(d3 => d3.Amount)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Color),aggregate(" +
                "Amount with sum as SumAmount,Amount with average as AvgAmount," +
                "Amount with min as MinAmount,Amount with max as MaxAmount))", serviceUri),
                aggregateQuery.ToString());

            MockGroupBySingleNavProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
        }

        [Fact]
        public void GroupByWithResultSelector_ByMultipleNavProperties()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.Product.Color, d1.Customer.Country },
                (d1, d2) => new
                {
                    d1.Color,
                    d1.Country,
                    SumAmount = d2.Sum(d3 => d3.Amount),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinAmount = d2.Min(d3 => d3.Amount),
                    MaxAmount = d2.Max(d3 => d3.Amount)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Color,Customer/Country),aggregate(" +
                "Amount with sum as SumAmount,Amount with average as AvgAmount," +
                "Amount with min as MinAmount,Amount with max as MaxAmount))", serviceUri),
                aggregateQuery.ToString());

            MockGroupByMultipleNavProperties();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
        }

        [Fact]
        public void GroupByWithProjection_ByMultipleNavProperties()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.Product.Color, d1.Customer.Country })
                .Select(d2 => new
                {
                    d2.Key.Color,
                    d2.Key.Country,
                    SumAmount = d2.Sum(d3 => d3.Amount),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinAmount = d2.Min(d3 => d3.Amount),
                    MaxAmount = d2.Max(d3 => d3.Amount)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Color,Customer/Country),aggregate(" +
                "Amount with sum as SumAmount,Amount with average as AvgAmount," +
                "Amount with min as MinAmount,Amount with max as MaxAmount))", serviceUri),
                aggregateQuery.ToString());

            MockGroupByMultipleNavProperties();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
        }

        [Fact]
        public void GroupByWithResultSelector_BySingleNavProperty_AggregationsTargetingNavProperty()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.Currency.Code,
                (d1, d2) => new
                {
                    Currency = d1,
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgTaxRate = d2.Average(d3 => d3.Product.TaxRate),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxTaxRate = d2.Max(d3 => d3.Product.TaxRate)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Currency/Code),aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Product/TaxRate with average as AvgTaxRate," +
                "Product/TaxRate with min as MinTaxRate,Product/TaxRate with max as MaxTaxRate))", serviceUri),
                aggregateQuery.ToString());

            MockGroupBySingleNavProperty_AggregationsTargetingNavProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
        }

        [Fact]
        public void GroupByWithProjection_BySingleNavProperty_AggregationsTargetingNavProperty()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.Currency.Code)
                .Select(d2 => new
                {
                    Currency = d2.Key,
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgTaxRate = d2.Average(d3 => d3.Product.TaxRate),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxTaxRate = d2.Max(d3 => d3.Product.TaxRate)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Currency/Code),aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Product/TaxRate with average as AvgTaxRate," +
                "Product/TaxRate with min as MinTaxRate,Product/TaxRate with max as MaxTaxRate))", serviceUri),
                aggregateQuery.ToString());

            MockGroupBySingleNavProperty_AggregationsTargetingNavProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
        }

        [Fact]
        public void GroupByWithResultSelector_ByMultipleNavProperties_AggregationsTargetingNavProperty()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.Product.Color, d1.Customer.Country },
                (d1, d2) => new
                {
                    d1.Color,
                    d1.Country,
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgTaxRate = d2.Average(d3 => d3.Product.TaxRate),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxTaxRate = d2.Max(d3 => d3.Product.TaxRate)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Color,Customer/Country),aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Product/TaxRate with average as AvgTaxRate," +
                "Product/TaxRate with min as MinTaxRate,Product/TaxRate with max as MaxTaxRate))", serviceUri),
                aggregateQuery.ToString());

            MockGroupByMultipleNavProperties_AggregationsTargetingNavProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
        }

        [Fact]
        public void GroupByWithProjection_ByMultipleNavProperties_AggregationsTargetingNavProperty()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.Product.Color, d1.Customer.Country })
                .Select(d2 => new
                {
                    d2.Key.Color,
                    d2.Key.Country,
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgTaxRate = d2.Average(d3 => d3.Product.TaxRate),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxTaxRate = d2.Max(d3 => d3.Product.TaxRate)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Color,Customer/Country),aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Product/TaxRate with average as AvgTaxRate," +
                "Product/TaxRate with min as MinTaxRate,Product/TaxRate with max as MaxTaxRate))", serviceUri),
                aggregateQuery.ToString());

            MockGroupByMultipleNavProperties_AggregationsTargetingNavProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
        }

        [Fact]
        public void GroupByWithResultSelector_ByConstant_AggregationsTargetingNavProperty()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1,
                (d1, d2) => new
                {
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgTaxRate = d2.Average(d3 => d3.Product.TaxRate),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxTaxRate = d2.Max(d3 => d3.Product.TaxRate)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Product/TaxRate with average as AvgTaxRate," +
                "Product/TaxRate with min as MinTaxRate,Product/TaxRate with max as MaxTaxRate)", serviceUri),
                aggregateQuery.ToString());

            MockGroupByConstant_AggregationsTargetingNavProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Single(aggregateResult);
        }

        [Fact]
        public void GroupByWithProjection_ByConstant_AggregationsTargetingNavProperty()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1)
                .Select(d2 => new
                {
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgTaxRate = d2.Average(d3 => d3.Product.TaxRate),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxTaxRate = d2.Max(d3 => d3.Product.TaxRate)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Product/TaxRate with average as AvgTaxRate," +
                "Product/TaxRate with min as MinTaxRate,Product/TaxRate with max as MaxTaxRate)", serviceUri),
                aggregateQuery.ToString());

            MockGroupByConstant_AggregationsTargetingNavProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Single(aggregateResult);
        }

        [Fact]
        public void GroupByWithResultSelector_ByConstant_MixedScenarios()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1,
                (d1, d2) => new
                {
                    GroupingConstant = d1,
                    GibberishConstant = "dfksjfl",
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxAmount = d2.Max(d3 => d3.Amount),
                    GroupCount = d2.Count(),
                    DistinctCurrency = d2.CountDistinct(d3 => d3.Currency.Code)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Amount with average as AvgAmount," +
                "Product/TaxRate with min as MinTaxRate,Amount with max as MaxAmount," +
                "$count as GroupCount,Currency/Code with countdistinct as DistinctCurrency)", serviceUri),
                aggregateQuery.ToString());

            MockGroupByConstant_MixedScenarios();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Single(aggregateResult);
        }

        [Fact]
        public void GroupByWithProjection_ByConstant_MixedScenarios()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1)
                .Select(d2 => new
                {
                    GroupingConstant = d2.Key,
                    GibberishConstant = "dfksjfl",
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxAmount = d2.Max(d3 => d3.Amount),
                    GroupCount = d2.Count(),
                    DistinctCurrency = d2.CountDistinct(d3 => d3.Currency.Code)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Amount with average as AvgAmount," +
                "Product/TaxRate with min as MinTaxRate,Amount with max as MaxAmount," +
                "$count as GroupCount,Currency/Code with countdistinct as DistinctCurrency)", serviceUri),
                aggregateQuery.ToString());

            MockGroupByConstant_MixedScenarios();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Single(aggregateResult);
        }


        [Fact]
        public void GroupByWithResultSelector_BySingleNavProperty_MixedScenarios()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.Product.Category.Id,
                (d1, d2) => new
                {
                    GibberishConstant = "dfksjfl",
                    CategoryId = d1,
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxAmount = d2.Max(d3 => d3.Amount),
                    GroupCount = d2.Count(),
                    DistinctCurrency = d2.CountDistinct(d3 => d3.Currency.Code)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Category/Id),aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Amount with average as AvgAmount," +
                "Product/TaxRate with min as MinTaxRate,Amount with max as MaxAmount," +
                "$count as GroupCount,Currency/Code with countdistinct as DistinctCurrency))", serviceUri),
                aggregateQuery.ToString());

            MockGroupBySingleNavProperty_MixedScenarios();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
        }

        [Fact]
        public void GroupByWithProjection_BySingleNavProperty_MixedScenarios()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.Product.Category.Id)
                .Select(d2 => new
                {
                    GibberishConstant = "dfksjfl",
                    CategoryId = d2.Key,
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxAmount = d2.Max(d3 => d3.Amount),
                    GroupCount = d2.Count(),
                    DistinctCurrency = d2.CountDistinct(d3 => d3.Currency.Code)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Category/Id),aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Amount with average as AvgAmount," +
                "Product/TaxRate with min as MinTaxRate,Amount with max as MaxAmount," +
                "$count as GroupCount,Currency/Code with countdistinct as DistinctCurrency))", serviceUri),
                aggregateQuery.ToString());

            MockGroupBySingleNavProperty_MixedScenarios();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
        }

        [Fact]
        public void GroupByWithResultSelector_ByMultipleNavProperties_MixedScenarios()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.Product.Category.Id, d1.Customer.Country },
                (d1, d2) => new
                {
                    GibberishConstant = "dfksjfl",
                    CategoryId = d1.Id,
                    d1.Country,
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxAmount = d2.Max(d3 => d3.Amount),
                    GroupCount = d2.Count(),
                    DistinctCurrency = d2.CountDistinct(d3 => d3.Currency.Code)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Category/Id,Customer/Country),aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Amount with average as AvgAmount," +
                "Product/TaxRate with min as MinTaxRate,Amount with max as MaxAmount," +
                "$count as GroupCount,Currency/Code with countdistinct as DistinctCurrency))", serviceUri),
                aggregateQuery.ToString());

            MockGroupByMultipleNavProperties_MixedScenarios();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
        }

        [Fact]
        public void GroupByWithProjection_ByMultipleNavProperties_MixedScenarios()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.Product.Category.Id, d1.Customer.Country })
                .Select(d2 => new
                {
                    GibberishConstant = "dfksjfl",
                    CategoryId = d2.Key.Id,
                    d2.Key.Country,
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxAmount = d2.Max(d3 => d3.Amount),
                    GroupCount = d2.Count(),
                    DistinctCurrency = d2.CountDistinct(d3 => d3.Currency.Code)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Category/Id,Customer/Country),aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Amount with average as AvgAmount," +
                "Product/TaxRate with min as MinTaxRate,Amount with max as MaxAmount," +
                "$count as GroupCount,Currency/Code with countdistinct as DistinctCurrency))", serviceUri),
                aggregateQuery.ToString());

            MockGroupByMultipleNavProperties_MixedScenarios();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
        }

        [Fact]
        public void GroupByWithResultSelector_ByConstant_UsingMemberInitialization()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1,
                (d1, d2) => new SalesGroupedResult01
                {
                    GroupingConstant = d1,
                    GibberishConstant = "dfksjfl",
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxAmount = d2.Max(d3 => d3.Amount),
                    GroupCount = d2.Count(),
                    DistinctCurrency = d2.CountDistinct(d3 => d3.Currency.Code)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Amount with average as AvgAmount," +
                "Product/TaxRate with min as MinTaxRate,Amount with max as MaxAmount," +
                "$count as GroupCount,Currency/Code with countdistinct as DistinctCurrency)", serviceUri),
                aggregateQuery.ToString());

            MockGroupByConstant_MixedScenarios();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Single(aggregateResult);
        }

        [Fact]
        public void GroupByWithProjection_ByConstant_UsingMemberInitialization()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1)
                .Select(d2 => new SalesGroupedResult01
                {
                    GroupingConstant = d2.Key,
                    GibberishConstant = "dfksjfl",
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxAmount = d2.Max(d3 => d3.Amount),
                    GroupCount = d2.Count(),
                    DistinctCurrency = d2.CountDistinct(d3 => d3.Currency.Code)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Amount with average as AvgAmount," +
                "Product/TaxRate with min as MinTaxRate,Amount with max as MaxAmount," +
                "$count as GroupCount,Currency/Code with countdistinct as DistinctCurrency)", serviceUri),
                aggregateQuery.ToString());

            MockGroupByConstant_MixedScenarios();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Single(aggregateResult);
        }


        [Fact]
        public void GroupByWithResultSelector_BySingleNavProperty_UsingMemberInitialization()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.Product.Category.Id,
                (d1, d2) => new SalesGroupedResult02
                {
                    GibberishConstant = "dfksjfl",
                    CategoryId = d1,
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxAmount = d2.Max(d3 => d3.Amount),
                    GroupCount = d2.Count(),
                    DistinctCurrency = d2.CountDistinct(d3 => d3.Currency.Code)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Category/Id),aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Amount with average as AvgAmount," +
                "Product/TaxRate with min as MinTaxRate,Amount with max as MaxAmount," +
                "$count as GroupCount,Currency/Code with countdistinct as DistinctCurrency))", serviceUri),
                aggregateQuery.ToString());

            MockGroupBySingleNavProperty_MixedScenarios();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
        }

        [Fact]
        public void GroupByWithProjection_BySingleNavProperty_UsingMemberInitialization()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.Product.Category.Id)
                .Select(d2 => new SalesGroupedResult02
                {
                    GibberishConstant = "dfksjfl",
                    CategoryId = d2.Key,
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxAmount = d2.Max(d3 => d3.Amount),
                    GroupCount = d2.Count(),
                    DistinctCurrency = d2.CountDistinct(d3 => d3.Currency.Code)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Category/Id),aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Amount with average as AvgAmount," +
                "Product/TaxRate with min as MinTaxRate,Amount with max as MaxAmount," +
                "$count as GroupCount,Currency/Code with countdistinct as DistinctCurrency))", serviceUri),
                aggregateQuery.ToString());

            MockGroupBySingleNavProperty_MixedScenarios();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
        }

        [Fact]
        public void GroupByWithResultSelector_ByMultipleNavProperties_UsingMemberInitialization()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.Product.Category.Id, d1.Customer.Country },
                (d1, d2) => new SalesGroupedResult03
                {
                    GibberishConstant = "dfksjfl",
                    CategoryId = d1.Id,
                    Country = d1.Country,
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxAmount = d2.Max(d3 => d3.Amount),
                    GroupCount = d2.Count(),
                    DistinctCurrency = d2.CountDistinct(d3 => d3.Currency.Code)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Category/Id,Customer/Country),aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Amount with average as AvgAmount," +
                "Product/TaxRate with min as MinTaxRate,Amount with max as MaxAmount," +
                "$count as GroupCount,Currency/Code with countdistinct as DistinctCurrency))", serviceUri),
                aggregateQuery.ToString());

            MockGroupByMultipleNavProperties_MixedScenarios();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
        }

        [Fact]
        public void GroupByWithProjection_ByMultipleNavProperties_UsingMemberInitialization()
        {
            DataServiceQuery<Sale> queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.Product.Category.Id, d1.Customer.Country })
                .Select(d2 => new SalesGroupedResult03
                {
                    GibberishConstant = "dfksjfl",
                    CategoryId = d2.Key.Id,
                    Country = d2.Key.Country,
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxAmount = d2.Max(d3 => d3.Amount),
                    GroupCount = d2.Count(),
                    DistinctCurrency = d2.CountDistinct(d3 => d3.Currency.Code)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Category/Id,Customer/Country),aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Amount with average as AvgAmount," +
                "Product/TaxRate with min as MinTaxRate,Amount with max as MaxAmount," +
                "$count as GroupCount,Currency/Code with countdistinct as DistinctCurrency))", serviceUri),
                aggregateQuery.ToString());

            MockGroupByMultipleNavProperties_MixedScenarios();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
        }

        #region Mock Aggregation Responses

        private void MockGroupBy_Sum_ByConstant()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "SumIntProp,SumNullableIntProp," +
                "SumDoubleProp,SumNullableDoubleProp," +
                "SumDecimalProp,SumNullableDecimalProp," +
                "SumLongProp,SumNullableLongProp," +
                "SumSingleProp,SumNullableSingleProp)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null," +
                "\"SumIntProp\":506,\"SumNullableIntProp\":530," +
                "\"SumDoubleProp\":464.72,\"SumNullableDoubleProp\":534.02," +
                "\"SumDecimalProp\":559.4,\"SumNullableDecimalProp\":393.7," +
                "\"SumLongProp\":1298,\"SumNullableLongProp\":993," +
                "\"SumSingleProp\":333.79,\"SumNullableSingleProp\":528.44}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Sum_BySingleProperty()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "RowParity," +
                "SumIntProp,SumNullableIntProp," +
                "SumDoubleProp,SumNullableDoubleProp," +
                "SumDecimalProp,SumNullableDecimalProp," +
                "SumLongProp,SumNullableLongProp," +
                "SumSingleProp,SumNullableSingleProp)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Even\"," +
                "\"SumIntProp\":132,\"SumNullableIntProp\":146," +
                "\"SumDoubleProp\":46.53,\"SumNullableDoubleProp\":343.8," +
                "\"SumDecimalProp\":342.30,\"SumNullableDecimalProp\":100.60," +
                "\"SumLongProp\":481,\"SumNullableLongProp\":544," +
                "\"SumSingleProp\":221.88,\"SumNullableSingleProp\":286.03}}," +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Odd\"," +
                "\"SumIntProp\":374,\"SumNullableIntProp\":384," +
                "\"SumDoubleProp\":418.19,\"SumNullableDoubleProp\":190.22," +
                "\"SumDecimalProp\":217.10,\"SumNullableDecimalProp\":293.10," +
                "\"SumLongProp\":817,\"SumNullableLongProp\":449," +
                "\"SumSingleProp\":111.91,\"SumNullableSingleProp\":242.41}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Sum_ByMultipleProperties()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "RowParity,RowCategory," +
                "SumIntProp,SumNullableIntProp," +
                "SumDoubleProp,SumNullableDoubleProp," +
                "SumDecimalProp,SumNullableDecimalProp," +
                "SumLongProp,SumNullableLongProp," +
                "SumSingleProp,SumNullableSingleProp)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Even\",\"RowCategory\":\"Composite\"," +
                "\"SumIntProp\":63,\"SumNullableIntProp\":146," +
                "\"SumDoubleProp\":44.19,\"SumNullableDoubleProp\":165.31," +
                "\"SumDecimalProp\":173.90,\"SumNullableDecimalProp\":null," +
                "\"SumLongProp\":259,\"SumNullableLongProp\":249," +
                "\"SumSingleProp\":171.22,\"SumNullableSingleProp\":174.99}}," +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Odd\",\"RowCategory\":\"None\"," +
                "\"SumIntProp\":109,\"SumNullableIntProp\":199," +
                "\"SumDoubleProp\":155.85,\"SumNullableDoubleProp\":null," +
                "\"SumDecimalProp\":101.60,\"SumNullableDecimalProp\":122.90," +
                "\"SumLongProp\":300,\"SumNullableLongProp\":201," +
                "\"SumSingleProp\":107.66,\"SumNullableSingleProp\":81.94}}," +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Odd\",\"RowCategory\":\"Prime\"," +
                "\"SumIntProp\":265,\"SumNullableIntProp\":185," +
                "\"SumDoubleProp\":262.34,\"SumNullableDoubleProp\":190.22," +
                "\"SumDecimalProp\":115.50,\"SumNullableDecimalProp\":170.20," +
                "\"SumLongProp\":517,\"SumNullableLongProp\":248," +
                "\"SumSingleProp\":4.25,\"SumNullableSingleProp\":160.47}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Average_ByConstant()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "AverageIntProp,AverageNullableIntProp," +
                "AverageDoubleProp,AverageNullableDoubleProp," +
                "AverageDecimalProp,AverageNullableDecimalProp," +
                "AverageLongProp,AverageNullableLongProp," +
                "AverageSingleProp,AverageNullableSingleProp)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null," +
                "\"AverageIntProp\":101.2,\"AverageNullableIntProp\":132.5," +
                "\"AverageDoubleProp\":92.944,\"AverageNullableDoubleProp\":133.505," +
                "\"AverageDecimalProp\":111.88,\"AverageNullableDecimalProp\":98.425," +
                "\"AverageLongProp\":259.6,\"AverageNullableLongProp\":248.25," +
                "\"AverageSingleProp\":66.758,\"AverageNullableSingleProp\":132.11}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Average_BySingleProperty()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "RowParity," +
                "AverageIntProp,AverageNullableIntProp," +
                "AverageDoubleProp,AverageNullableDoubleProp," +
                "AverageDecimalProp,AverageNullableDecimalProp," +
                "AverageLongProp,AverageNullableLongProp," +
                "AverageSingleProp,AverageNullableSingleProp)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Even\"," +
                "\"AverageIntProp\":66.0,\"AverageNullableIntProp\":146.0," +
                "\"AverageDoubleProp\":23.265,\"AverageNullableDoubleProp\":171.9," +
                "\"AverageDecimalProp\":171.15,\"AverageNullableDecimalProp\":100.60," +
                "\"AverageLongProp\":240.5,\"AverageNullableLongProp\":272.0," +
                "\"AverageSingleProp\":110.94,\"AverageNullableSingleProp\":143.015}}," +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Odd\"," +
                "\"AverageIntProp\":124.67,\"AverageNullableIntProp\":128.0," +
                "\"AverageDoubleProp\":139.397,\"AverageNullableDoubleProp\":95.11," +
                "\"AverageDecimalProp\":72.37,\"AverageNullableDecimalProp\":97.70," +
                "\"AverageLongProp\":272.33,\"AverageNullableLongProp\":224.5," +
                "\"AverageSingleProp\":37.30,\"AverageNullableSingleProp\":121.205}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Average_ByMultipleProperties()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "RowParity,RowCategory," +
                "AverageIntProp,AverageNullableIntProp," +
                "AverageDoubleProp,AverageNullableDoubleProp," +
                "AverageDecimalProp,AverageNullableDecimalProp," +
                "AverageLongProp,AverageNullableLongProp," +
                "AverageSingleProp,AverageNullableSingleProp)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Even\",\"RowCategory\":\"Composite\"," +
                "\"AverageNullableIntProp\":146,\"AverageIntProp\":63," +
                "\"AverageNullableDoubleProp\":165.31,\"AverageDoubleProp\":44.19," +
                "\"AverageNullableDecimalProp\":null,\"AverageDecimalProp\":173.9," +
                "\"AverageNullableLongProp\":249,\"AverageLongProp\":259," +
                "\"AverageNullableSingleProp\":174.99,\"AverageSingleProp\":171.22}}," +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Odd\",\"RowCategory\":\"None\"," +
                "\"AverageNullableIntProp\":199,\"AverageIntProp\":109," +
                "\"AverageNullableDoubleProp\":null,\"AverageDoubleProp\":155.85," +
                "\"AverageNullableDecimalProp\":122.9,\"AverageDecimalProp\":101.6," +
                "\"AverageNullableLongProp\":201,\"AverageLongProp\":300," +
                "\"AverageNullableSingleProp\":81.94,\"AverageSingleProp\":107.66}}," +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Odd\",\"RowCategory\":\"Prime\"," +
                "\"AverageNullableIntProp\":92.5,\"AverageIntProp\":132.5," +
                "\"AverageNullableDoubleProp\":95.11,\"AverageDoubleProp\":131.17," +
                "\"AverageNullableDecimalProp\":85.1,\"AverageDecimalProp\":57.75," +
                "\"AverageNullableLongProp\":248,\"AverageLongProp\":258.5," +
                "\"AverageNullableSingleProp\":160.47,\"AverageSingleProp\":2.125}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Min_ByConstant()
        {
            string mockResponse = string.Format("{{" +
                   "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                   "MinIntProp,MinNullableIntProp," +
                   "MinDoubleProp,MinNullableDoubleProp," +
                   "MinDecimalProp,MinNullableDecimalProp," +
                   "MinLongProp,MinNullableLongProp," +
                   "MinSingleProp,MinNullableSingleProp)\"," +
                   "\"value\":[" +
                   "{{\"@odata.id\":null," +
                   "\"MinIntProp\":63,\"MinNullableIntProp\":34," +
                   "\"MinDoubleProp\":2.34,\"MinNullableDoubleProp\":16.1," +
                   "\"MinDecimalProp\":42.70,\"MinNullableDecimalProp\":12.90," +
                   "\"MinLongProp\":220,\"MinNullableLongProp\":201," +
                   "\"MinSingleProp\":1.29,\"MinNullableSingleProp\":81.94}}" +
                   "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Min_BySingleProperty()
        {
            string mockResponse = string.Format("{{" +
                   "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                   "RowParity," +
                   "MinIntProp,MinNullableIntProp," +
                   "MinDoubleProp,MinNullableDoubleProp," +
                   "MinDecimalProp,MinNullableDecimalProp," +
                   "MinLongProp,MinNullableLongProp," +
                   "MinSingleProp,MinNullableSingleProp)\"," +
                   "\"value\":[" +
                   "{{\"@odata.id\":null," +
                   "\"RowParity\":\"Even\"," +
                   "\"MinIntProp\":63,\"MinNullableIntProp\":146," +
                   "\"MinDoubleProp\":2.34,\"MinNullableDoubleProp\":165.31," +
                   "\"MinDecimalProp\":168.40,\"MinNullableDecimalProp\":100.60," +
                   "\"MinLongProp\":222,\"MinNullableLongProp\":249," +
                   "\"MinSingleProp\":50.66,\"MinNullableSingleProp\":111.04}}," +
                   "{{\"@odata.id\":null," +
                   "\"RowParity\":\"Odd\"," +
                   "\"MinIntProp\":109,\"MinNullableIntProp\":34," +
                   "\"MinDoubleProp\":129.37,\"MinNullableDoubleProp\":16.1," +
                   "\"MinDecimalProp\":42.70,\"MinNullableDecimalProp\":12.90," +
                   "\"MinLongProp\":220,\"MinNullableLongProp\":201," +
                   "\"MinSingleProp\":1.29,\"MinNullableSingleProp\":81.94}}" +
                   "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Min_ByMultipleProperties()
        {
            string mockResponse = string.Format("{{" +
                   "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                   "RowParity,RowCategory" +
                   "MinIntProp,MinNullableIntProp," +
                   "MinDoubleProp,MinNullableDoubleProp," +
                   "MinDecimalProp,MinNullableDecimalProp," +
                   "MinLongProp,MinNullableLongProp," +
                   "MinSingleProp,MinNullableSingleProp)\"," +
                   "\"value\":[" +
                   "{{\"@odata.id\":null," +
                   "\"RowParity\":\"Even\",\"RowCategory\":\"Composite\"," +
                   "\"MinNullableIntProp\":146,\"MinIntProp\":63," +
                   "\"MinNullableDoubleProp\":165.31,\"MinDoubleProp\":44.19," +
                   "\"MinNullableDecimalProp\":null,\"MinDecimalProp\":173.9," +
                   "\"MinNullableLongProp\":249,\"MinLongProp\":259," +
                   "\"MinNullableSingleProp\":174.99,\"MinSingleProp\":171.22}}," +
                   "{{\"@odata.id\":null," +
                   "\"RowParity\":\"Odd\",\"RowCategory\":\"None\"," +
                   "\"MinNullableIntProp\":199,\"MinIntProp\":109," +
                   "\"MinNullableDoubleProp\":null,\"MinDoubleProp\":155.85," +
                   "\"MinNullableDecimalProp\":122.9,\"MinDecimalProp\":101.6," +
                   "\"MinNullableLongProp\":201,\"MinLongProp\":300," +
                   "\"MinNullableSingleProp\":81.94,\"MinSingleProp\":107.66}}," +
                   "{{\"@odata.id\":null," +
                   "\"RowParity\":\"Odd\",\"RowCategory\":\"Prime\"," +
                   "\"MinNullableIntProp\":34,\"MinIntProp\":124," +
                   "\"MinNullableDoubleProp\":16.1,\"MinDoubleProp\":129.37," +
                   "\"MinNullableDecimalProp\":12.9,\"MinDecimalProp\":42.7," +
                   "\"MinNullableLongProp\":248,\"MinLongProp\":220," +
                   "\"MinNullableSingleProp\":160.47,\"MinSingleProp\":1.29}}" +
                   "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Max_ByConstant()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "MaxIntProp,MaxNullableIntProp," +
                "MaxDoubleProp,MaxNullableDoubleProp," +
                "MaxDecimalProp,MaxNullableDecimalProp," +
                "MaxLongProp,MaxNullableLongProp," +
                "MaxSingleProp,MaxNullableSingleProp)\"," +
                "\"value\":[{{" +
                "\"@odata.id\":null," +
                "\"MaxIntProp\":141,\"MaxNullableIntProp\":199," +
                "\"MaxDoubleProp\":155.85,\"MaxNullableDoubleProp\":178.49," +
                "\"MaxDecimalProp\":173.90,\"MaxNullableDecimalProp\":157.30," +
                "\"MaxLongProp\":300,\"MaxNullableLongProp\":295," +
                "\"MaxSingleProp\":171.22,\"MaxNullableSingleProp\":174.99}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Max_BySingleProperty()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "RowParity," +
                "MaxIntProp,MaxNullableIntProp," +
                "MaxDoubleProp,MaxNullableDoubleProp," +
                "MaxDecimalProp,MaxNullableDecimalProp," +
                "MaxLongProp,MaxNullableLongProp," +
                "MaxSingleProp,MaxNullableSingleProp)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Even\"," +
                "\"MaxIntProp\":69,\"MaxNullableIntProp\":146," +
                "\"MaxDoubleProp\":44.19,\"MaxNullableDoubleProp\":178.49," +
                "\"MaxDecimalProp\":173.90,\"MaxNullableDecimalProp\":100.60," +
                "\"MaxLongProp\":259,\"MaxNullableLongProp\":295," +
                "\"MaxSingleProp\":171.22,\"MaxNullableSingleProp\":174.99}}," +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Odd\"," +
                "\"MaxIntProp\":141,\"MaxNullableIntProp\":199," +
                "\"MaxDoubleProp\":155.85,\"MaxNullableDoubleProp\":174.12," +
                "\"MaxDecimalProp\":101.60,\"MaxNullableDecimalProp\":157.30," +
                "\"MaxLongProp\":300,\"MaxNullableLongProp\":248," +
                "\"MaxSingleProp\":107.66,\"MaxNullableSingleProp\":160.47}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Max_ByMultipleProperties()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "RowParity,RowCategory," +
                "MaxIntProp,MaxNullableIntProp," +
                "MaxDoubleProp,MaxNullableDoubleProp," +
                "MaxDecimalProp,MaxNullableDecimalProp," +
                "MaxLongProp,MaxNullableLongProp," +
                "MaxSingleProp,MaxNullableSingleProp)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Even\",\"RowCategory\":\"Composite\"," +
                "\"MaxNullableIntProp\":146,\"MaxIntProp\":63," +
                "\"MaxNullableDoubleProp\":165.31,\"MaxDoubleProp\":44.19," +
                "\"MaxNullableDecimalProp\":null,\"MaxDecimalProp\":173.9," +
                "\"MaxNullableLongProp\":249,\"MaxLongProp\":259," +
                "\"MaxNullableSingleProp\":174.99,\"MaxSingleProp\":171.22}}," +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Odd\",\"RowCategory\":\"None\"," +
                "\"MaxNullableIntProp\":199,\"MaxIntProp\":109," +
                "\"MaxNullableDoubleProp\":null,\"MaxDoubleProp\":155.85," +
                "\"MaxNullableDecimalProp\":122.9,\"MaxDecimalProp\":101.6," +
                "\"MaxNullableLongProp\":201,\"MaxLongProp\":300," +
                "\"MaxNullableSingleProp\":81.94,\"MaxSingleProp\":107.66}}," +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Odd\",\"RowCategory\":\"Prime\"," +
                "\"MaxNullableIntProp\":151,\"MaxIntProp\":141," +
                "\"MaxNullableDoubleProp\":174.12,\"MaxDoubleProp\":132.97," +
                "\"MaxNullableDecimalProp\":157.3,\"MaxDecimalProp\":72.8," +
                "\"MaxNullableLongProp\":248,\"MaxLongProp\":297," +
                "\"MaxNullableSingleProp\":160.47,\"MaxSingleProp\":2.96}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Count_ByConstant()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(Count,CountDistinct)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null,\"CountDistinct\":3,\"Count\":5}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Count_BySingleProperty()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(RowParity,Count,CountDistinct)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null,\"RowParity\":\"Even\",\"CountDistinct\":2,\"Count\":2}}," +
                "{{\"@odata.id\":null,\"RowParity\":\"Odd\",\"CountDistinct\":2,\"Count\":3}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Count_ByMultipleProperties()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(RowParity,RowCategory,Count,CountDistinct)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null,\"RowCategory\":\"Composite\",\"RowParity\":\"Even\",\"CountDistinct\":1,\"Count\":1}}," +
                "{{\"@odata.id\":null,\"RowCategory\":\"None\",\"RowParity\":\"Odd\",\"CountDistinct\":1,\"Count\":1}}," +
                "{{\"@odata.id\":null,\"RowCategory\":\"Prime\",\"RowParity\":\"Odd\",\"CountDistinct\":1,\"Count\":2}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_WithMixedAggregations()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "RowParity,SumIntProp,AverageDoubleProp,MinDecimalProp,MaxLongProp,Count,CountDistinct)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Even\"," +
                "\"Count\":2,\"CountDistinct\":2," +
                "\"SumIntProp\":132,\"AverageDoubleProp\":23.265," +
                "\"MinDecimalProp\":168.4,\"MaxLongProp\":259}}," +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Odd\"," +
                "\"Count\":3,\"CountDistinct\":2," +
                "\"SumIntProp\":374,\"AverageDoubleProp\":139.4," +
                "\"MinDecimalProp\":42.7,\"MaxLongProp\":300}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockCountDistinct()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "CountDistinctRowParity)\"," +
                "\"value\":[{{\"@odata.id\":null,\"CountDistinctRowParity\":3}}]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockCountDistinct_TargetingNavProperty()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "CountDistinctCustomerCountry)\"," +
                "\"value\":[{{\"@odata.id\":null,\"CountDistinctCustomerCountry\":2}}]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBySingleNavProperty()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Sales" +
                    "(Product(Color),SumAmount,AvgAmount,MinAmount,MaxAmount)\"," +
                    "\"value\":[" +
                    "{{\"@odata.id\":null,\"MaxAmount\":8.00,\"MinAmount\":4.00,\"AvgAmount\":6.000000,\"SumAmount\":12.00," +
                    "\"Product\":{{\"@odata.id\":null,\"Color\":\"Brown\"}}}}," +
                    "{{\"@odata.id\":null,\"MaxAmount\":4.00,\"MinAmount\":1.00,\"AvgAmount\":2.000000,\"SumAmount\":12.00," +
                    "\"Product\":{{\"@odata.id\":null,\"Color\":\"White\"}}}}" +
                    "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupByMultipleNavProperties()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Sales" +
                    "(Product(Color),Customer(Country),SumAmount,AvgAmount,MinAmount,MaxAmount)\"," +
                    "\"value\":[" +
                    "{{\"@odata.id\":null,\"MaxAmount\":8.00,\"MinAmount\":4.00,\"AvgAmount\":6.000000,\"SumAmount\":12.00," +
                    "\"Customer\":{{\"@odata.id\":null,\"Country\":\"USA\"}}," +
                    "\"Product\":{{\"@odata.id\":null,\"Color\":\"Brown\"}}}}," +
                    "{{\"@odata.id\":null,\"MaxAmount\":2.00,\"MinAmount\":1.00,\"AvgAmount\":1.666666,\"SumAmount\":5.00," +
                    "\"Customer\":{{\"@odata.id\":null,\"Country\":\"Netherlands\"}}," +
                    "\"Product\":{{\"@odata.id\":null,\"Color\":\"White\"}}}}" +
                    "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBySingleNavProperty_AggregationsTargetingNavProperty()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Sales" +
                    "(Currency(Code),SumTaxRate,AvgTaxRate,MinTaxRate,MaxTaxRate)\"," +
                    "\"value\":[" +
                    "{{\"@odata.id\":null,\"MaxTaxRate\":0.14,\"MinTaxRate\":0.06,\"AvgTaxRate\":0.113333,\"SumTaxRate\":0.34," +
                    "\"Currency\":{{\"@odata.id\":null,\"Code\":\"EUR\"}}}}," +
                    "{{\"@odata.id\":null,\"MaxTaxRate\":0.14,\"MinTaxRate\":0.06,\"AvgTaxRate\":0.092000,\"SumTaxRate\":0.46," +
                    "\"Currency\":{{\"@odata.id\":null,\"Code\":\"USD\"}}}}" +
                    "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupByMultipleNavProperties_AggregationsTargetingNavProperty()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Sales" +
                    "(Product(Color),Customer(Country),SumTaxRate,AvgTaxRate,MinTaxRate,MaxTaxRate)\"," +
                    "\"value\":[" +
                    "{{\"@odata.id\":null,\"MaxTaxRate\":0.14,\"MinTaxRate\":0.06,\"AvgTaxRate\":0.113333,\"SumTaxRate\":0.34," +
                    "\"Customer\":{{\"@odata.id\":null,\"Country\":\"Netherlands\"}}," +
                    "\"Product\":{{\"@odata.id\":null,\"Color\":\"White\"}}}}," +
                    "{{\"@odata.id\":null,\"MaxTaxRate\":0.06,\"MinTaxRate\":0.06,\"AvgTaxRate\":0.060000,\"SumTaxRate\":0.12," +
                    "\"Customer\":{{\"@odata.id\":null,\"Country\":\"USA\"}}," +
                    "\"Product\":{{\"@odata.id\":null,\"Color\":\"Brown\"}}}}" +
                    "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupByConstant_AggregationsTargetingNavProperty()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Sales" +
                    "(SumTaxRate,AvgTaxRate,MinTaxRate,MaxTaxRate)\"," +
                    "\"value\":[" +
                    "{{\"@odata.id\":null,\"MaxTaxRate\":0.14,\"MinTaxRate\":0.06,\"AvgTaxRate\":0.100000,\"SumTaxRate\":0.80}}" +
                    "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupByConstant_MixedScenarios()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Sales" +
                    "(SumTaxRate,AvgAmount,MinTaxRate,MaxAmount,GroupCount,DistinctCurrency)\"," +
                    "\"value\":[" +
                    "{{\"@odata.id\":null,\"DistinctCurrency\":2,\"GroupCount\":8," +
                    "\"MaxAmount\":8.00,\"MinTaxRate\":0.06,\"AvgAmount\":3.000000,\"SumTaxRate\":0.80}}" +
                    "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBySingleNavProperty_MixedScenarios()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Sales" +
                    "(Product(Category(Id)),SumTaxRate,AvgAmount,MinTaxRate,MaxAmount,GroupCount,DistinctCurrency)\"," +
                    "\"value\":[" +
                    "{{\"@odata.id\":null,\"DistinctCurrency\":2,\"GroupCount\":4," +
                    "\"MaxAmount\":8.00,\"MinTaxRate\":0.06,\"AvgAmount\":4.000000,\"SumTaxRate\":0.24," +
                    "\"Product\":{{\"@odata.id\":null,\"Category\":{{\"@odata.id\":null,\"Id\":\"PG1\"}}}}}}," +
                    "{{\"@odata.id\":null,\"DistinctCurrency\":2,\"GroupCount\":4," +
                    "\"MaxAmount\":4.00,\"MinTaxRate\":0.14,\"AvgAmount\":2.000000,\"SumTaxRate\":0.56," +
                    "\"Product\":{{\"@odata.id\":null,\"Category\":{{\"@odata.id\":null,\"Id\":\"PG2\"}}}}}}" +
                    "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupByMultipleNavProperties_MixedScenarios()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Sales" +
                    "(Product(Category(Id)),Customer(Country),SumTaxRate,AvgAmount,MinTaxRate,MaxAmount,GroupCount,DistinctCurrency)\"," +
                    "\"value\":[" +
                    "{{\"@odata.id\":null,\"DistinctCurrency\":1,\"GroupCount\":1," +
                    "\"MaxAmount\":2.00,\"MinTaxRate\":0.06,\"AvgAmount\":2.000000,\"SumTaxRate\":0.06," +
                    "\"Customer\":{{\"@odata.id\":null,\"Country\":\"Netherlands\"}}," +
                    "\"Product\":{{\"@odata.id\":null,\"Category\":{{\"@odata.id\":null,\"Id\":\"PG1\"}}}}}}," +
                    "{{\"@odata.id\":null,\"DistinctCurrency\":1,\"GroupCount\":2," +
                    "\"MaxAmount\":2.00,\"MinTaxRate\":0.14,\"AvgAmount\":1.500000,\"SumTaxRate\":0.28," +
                    "\"Customer\":{{\"@odata.id\":null,\"Country\":\"Netherlands\"}}," +
                    "\"Product\":{{\"@odata.id\":null,\"Category\":{{\"@odata.id\":null,\"Id\":\"PG2\"}}}}}}" +
                    "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        #endregion Mock Aggregation Responses

        #region Helper Methods

        private static EdmModel BuildEdmModel()
        {
            var model = new EdmModel();

            var numberEntity = new EdmEntityType("NS", "Number");
            numberEntity.AddKeys(numberEntity.AddStructuralProperty("RowId", EdmCoreModel.Instance.GetInt32(false)));
            numberEntity.AddStructuralProperty("RowParity", EdmCoreModel.Instance.GetString(false));
            numberEntity.AddStructuralProperty("RowCategory", EdmCoreModel.Instance.GetString(false));
            numberEntity.AddStructuralProperty("IntProp", EdmCoreModel.Instance.GetInt32(false));
            numberEntity.AddStructuralProperty("NullableIntProp", EdmCoreModel.Instance.GetInt32(true));
            numberEntity.AddStructuralProperty("DoubleProp", EdmCoreModel.Instance.GetDouble(false));
            numberEntity.AddStructuralProperty("NullableDoubleProp", EdmCoreModel.Instance.GetDouble(true));
            numberEntity.AddStructuralProperty("DecimalProp", EdmCoreModel.Instance.GetDecimal(false));
            numberEntity.AddStructuralProperty("NullableDecimalProp", EdmCoreModel.Instance.GetDecimal(true));
            numberEntity.AddStructuralProperty("LongProp", EdmCoreModel.Instance.GetInt64(false));
            numberEntity.AddStructuralProperty("NullableLongProp", EdmCoreModel.Instance.GetInt64(true));
            numberEntity.AddStructuralProperty("SingleProp", EdmCoreModel.Instance.GetSingle(false));
            numberEntity.AddStructuralProperty("NullableSingleProp", EdmCoreModel.Instance.GetSingle(true));

            var saleEntity = new EdmEntityType("NS", "Sale");
            saleEntity.AddKeys(saleEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            saleEntity.AddStructuralProperty("CustomerId", EdmCoreModel.Instance.GetString(false));
            saleEntity.AddStructuralProperty("Date", EdmCoreModel.Instance.GetString(false));
            saleEntity.AddStructuralProperty("ProductId", EdmCoreModel.Instance.GetString(false));
            saleEntity.AddStructuralProperty("CurrencyCode", EdmCoreModel.Instance.GetString(false));
            saleEntity.AddStructuralProperty("Amount", EdmCoreModel.Instance.GetDecimal(false));

            var productEntity = new EdmEntityType("NS", "Product");
            productEntity.AddKeys(productEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            productEntity.AddStructuralProperty("CategoryId", EdmCoreModel.Instance.GetString(false));
            productEntity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            productEntity.AddStructuralProperty("Color", EdmCoreModel.Instance.GetString(false));
            productEntity.AddStructuralProperty("TaxRate", EdmCoreModel.Instance.GetDecimal(false));

            var customerEntity = new EdmEntityType("NS", "Customer");
            customerEntity.AddKeys(customerEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetString(false)));
            customerEntity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            customerEntity.AddStructuralProperty("Country", EdmCoreModel.Instance.GetString(false));

            var categoryEntity = new EdmEntityType("NS", "Category");
            categoryEntity.AddKeys(categoryEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetString(false)));
            categoryEntity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));

            var currencyEntity = new EdmEntityType("NS", "Currency");
            currencyEntity.AddKeys(currencyEntity.AddStructuralProperty("Code", EdmCoreModel.Instance.GetString(false)));
            currencyEntity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));

            // Associations
            saleEntity.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo { Name = "Customer", Target = customerEntity, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo { Name = "Sales", Target = saleEntity, TargetMultiplicity = EdmMultiplicity.Many });
            saleEntity.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo { Name = "Product", Target = productEntity, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo { Name = "Sales", Target = saleEntity, TargetMultiplicity = EdmMultiplicity.Many });
            saleEntity.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo { Name = "Currency", Target = currencyEntity, TargetMultiplicity = EdmMultiplicity.One });

            productEntity.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo { Name = "Category", Target = categoryEntity, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo { Name = "Products", Target = productEntity, TargetMultiplicity = EdmMultiplicity.Many });

            var entityContainer = new EdmEntityContainer("NS", "Container");

            model.AddElement(numberEntity);
            model.AddElement(saleEntity);
            model.AddElement(productEntity);
            model.AddElement(customerEntity);
            model.AddElement(categoryEntity);
            model.AddElement(currencyEntity);
            model.AddElement(entityContainer);

            entityContainer.AddEntitySet(numbersEntitySetName, numberEntity);
            entityContainer.AddEntitySet(salesEntitySetName, saleEntity);

            return model;
        }

        // To find matching aggregation method - using reflection
        private static MethodInfo GetAggregationMethod(string aggregationMethod, Type genericArgumentType)
        {
            return typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                            .Where(d1 => d1.Name.Equals(aggregationMethod))
                            .Select(d2 => new { Method = d2, Parameters = d2.GetParameters() })
                            .Where(d3 => d3.Parameters.Length.Equals(2)
                                && d3.Parameters[0].ParameterType.IsGenericType
                                && d3.Parameters[0].ParameterType.GetGenericTypeDefinition().Equals(typeof(IQueryable<>))
                                && d3.Parameters[1].ParameterType.IsGenericType
                                && d3.Parameters[1].ParameterType.GetGenericTypeDefinition().Equals(typeof(Expression<>)))
                            .Select(d4 => new { d4.Method, Arguments = d4.Parameters[1].ParameterType.GetGenericArguments() })
                            .Where(d5 => d5.Arguments.Length > 0
                                && d5.Arguments[0].IsGenericType
                                && d5.Arguments[0].GetGenericTypeDefinition().Equals(typeof(Func<,>)))
                            .Select(d6 => new { d6.Method, Arguments = d6.Arguments[0].GetGenericArguments() })
                            .Where(d7 => d7.Arguments.Length > 1
                                && d7.Arguments[0].IsGenericParameter
                                && new[] { "Min", "Max" }.Contains(d7.Method.Name) ? true : d7.Arguments[1].Equals(genericArgumentType))
                            .Select(d8 => d8.Method)
                            .FirstOrDefault();
        }

        private static MethodInfo GetCountDistinctMethod()
        {
            return typeof(DataServiceExtensions).GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .Where(d1 => d1.Name.Equals("CountDistinct", StringComparison.Ordinal))
                    .Select(d2 => new { Method = d2, Parameters = d2.GetParameters() })
                    .Where(d3 => d3.Parameters.Length.Equals(2)
                        && d3.Parameters[0].ParameterType.IsGenericType
                        && d3.Parameters[0].ParameterType.GetGenericTypeDefinition().Equals(typeof(IQueryable<>))
                        && d3.Parameters[1].ParameterType.IsGenericType
                        && d3.Parameters[1].ParameterType.GetGenericTypeDefinition().Equals(typeof(Expression<>)))
                    .Select(d4 => new { d4.Method, SelectorArguments = d4.Parameters[1].ParameterType.GetGenericArguments() })
                    .Where(d5 => d5.SelectorArguments.Length.Equals(1)
                        && d5.SelectorArguments[0].IsGenericType
                        && d5.SelectorArguments[0].GetGenericTypeDefinition().Equals(typeof(Func<,>)))
                    .Select(d6 => d6.Method).Single();
        }

        // To generate a relevant aggregate value to be returned in the mock response
        private object GenerateRandomValue(string aggregationMethod, Type returnType)
        {
            int lowerBound = 100;
            int upperBound = 1000;
            object aggregationValue;

            switch (aggregationMethod.ToLowerInvariant())
            {
                case "average":
                    // A decimal value should suffice as average for all types
                    aggregationValue = Math.Round((double)upperBound * rand.NextDouble(), 2);
                    break;
                case "min":
                case "max":
                case "sum":
                    if (returnType.Equals(typeof(int)) || returnType.Equals(typeof(long)))
                        aggregationValue = rand.Next(lowerBound, upperBound);
                    else
                        aggregationValue = Math.Round(upperBound * rand.NextDouble(), 2);
                    break;
                default:
                    aggregationValue = rand.Next(lowerBound, upperBound);
                    break;
            }

            // Get underlying type if type is nullable - to use in Convert.ChangeType
            Type underlyingType = Nullable.GetUnderlyingType(returnType);
            if (underlyingType == null) // Not a nullable type
            {
                underlyingType = returnType;
            }

            return Convert.ChangeType(aggregationValue, underlyingType, CultureInfo.InvariantCulture.NumberFormat);
        }

        private void InterceptRequestAndMockResponse(string aggregateAlias, object aggregateValue)
        {
            var mockResponse = string.Format(
                    "{{\"@odata.context\":\"{0}/$metadata#{1}({2})\",\"value\":[{{\"@odata.id\":null,\"{2}\":{3}}}]}}",
                    serviceUri,
                    numbersEntitySetName,
                    aggregateAlias,
                    aggregateValue);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void InterceptRequestAndMockResponse(string mockResponse)
        {
            dsContext.Configurations.RequestPipeline.OnMessageCreating = (args) =>
            {
                var contentTypeHeader = "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8";
                var odataVersionHeader = "4.0";

                return new TestHttpWebRequestMessage(args,
                    new Dictionary<string, string>
                    {
                        {"Content-Type", contentTypeHeader},
                        {"OData-Version", odataVersionHeader},
                    },
                    () => new MemoryStream(Encoding.UTF8.GetBytes(mockResponse)));
            };
        }

        #endregion

        #region Types

        public class Number
        {
            public int RowId { get; set; }
            public string RowParity { get; set; }
            public string RowCategory { get; set; }
            public int IntProp { get; set; }
            public int? NullableIntProp { get; set; }
            public double DoubleProp { get; set; }
            public double? NullableDoubleProp { get; set; }
            public decimal DecimalProp { get; set; }
            public decimal? NullableDecimalProp { get; set; }
            public long LongProp { get; set; }
            public long? NullableLongProp { get; set; }
            public float SingleProp { get; set; }
            public float? NullableSingleProp { get; set; }
        }

        public class Sale
        {
            public int Id { get; set; }
            public string CustomerId { get; set; }
            public Customer Customer { get; set; }
            public string Date { get; set; }
            public string ProductId { get; set; }
            public Product Product { get; set; }
            public string CurrencyCode { get; set; }
            public Currency Currency { get; set; }
            public decimal Amount { get; set; }
        }

        public class Product
        {
            public string Id { get; set; }
            public string CategoryId { get; set; }
            public Category Category { get; set; }
            public string Name { get; set; }
            public string Color { get; set; }
            public decimal TaxRate { get; set; }
            public Collection<Sale> Sales { get; set; }
        }

        public class Customer
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Country { get; set; }
            public Collection<Sale> Sales { get; set; }
        }

        public class Category
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public Collection<Product> Products { get; set; }
        }

        public class Currency
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        #endregion

        #region Helper Classes

        class NumbersGroupedResult
        {
            public string RowParity { get; set; }
            public int SumIntProp { get; set; }
            public double AverageDoubleProp { get; set; }
            public decimal MinDecimalProp { get; set; }
            public long MaxLongProp { get; set; }
            public int Count { get; set; }
            public int CountDistinct { get; set; }
        }

        class SalesGroupedResult
        {
            public decimal SumTaxRate { get; set; }
            public decimal AvgAmount { get; set; }
            public decimal MinTaxRate { get; set; }
            public decimal MaxAmount { get; set; }
            public int GroupCount { get; set; }
            public int DistinctCurrency { get; set; }
        }

        class SalesGroupedResult01 : SalesGroupedResult
        {
            public int GroupingConstant { get; set; }
            public string GibberishConstant { get; set; }
        }

        class SalesGroupedResult02 : SalesGroupedResult
        {
            public string GibberishConstant { get; set; }
            public string CategoryId { get; set; }
        }

        class SalesGroupedResult03 : SalesGroupedResult
        {
            public string GibberishConstant { get; set; }
            public string CategoryId { get; set; }
            public string Country { get; set; }
        }

        #endregion Helper Classes
    }
}
