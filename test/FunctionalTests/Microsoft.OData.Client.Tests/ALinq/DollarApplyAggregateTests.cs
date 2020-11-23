//---------------------------------------------------------------------
// <copyright file="DollarApplyAggregateTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace Microsoft.OData.Client.Tests.ALinq
{
    public class DollarApplyAggregateTests : DollarApplyTestsBase
    {
        public DollarApplyAggregateTests() : base()
        {
        }

        // Generates aggregation test data for 40 scenarios involving Average, Sum, Mix, Max
        public static IEnumerable<object[]> AggregationTestData()
        {
            var testData = new List<object[]>();

            foreach (var aggregationMethodName in new[] { "Sum", "Average", "Min", "Max" })
            {
                foreach (var type in new[] { "Int", "Double", "Decimal", "Long", "Single" })
                {
                    var propertyName = $"{type}Prop";  // e.g. IntProp
                    var nullablePropertyName = $"Nullable{propertyName}"; // e.g. NullableIntProp

                    testData.Add(new object[] { propertyName, aggregationMethodName });
                    testData.Add(new object[] { nullablePropertyName, aggregationMethodName });
                    // e.g.
                    // { "Average", "IntProp" }
                    // { "Average", "NullableIntProp" }
                }
            }

            foreach (var item in testData)
                yield return item;
        }

        [Theory]
        [MemberData(nameof(AggregationTestData))]
        public void Aggregation_ExpressionTranslatedToExpectedUri(string propertyName, string aggregationMethodName)
        {
            // Arrange
            var queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);
            var propertyInfo = queryable.ElementType.GetProperty(propertyName);

            // Get the aggregation method
            var aggregationMethod = GetAggregationMethod(aggregationMethodName, propertyInfo.PropertyType);

            // Build method call expression dynamically, e.g. Queryable.Average(d1 => d1.Prop)
            var methodCallExpr = BuildMethodCallExpression(queryable, aggregationMethod, propertyInfo);

            // Act
            // Call factory method for creating DataServiceOrderedQuery based on expression
            var query = new DataServiceQueryProvider(dsContext).CreateQuery(methodCallExpr);

            // Assert
            // E.g. http://tempuri.org/Sales?apply=aggregate(Prop with average as AverageProp)
            var expectedAggregateUri = $"{serviceUri}/{numbersEntitySetName}?$apply=" + string.Format(
                aggregateTransformationTemplate, string.Format(
                    aggregateExpressionTemplate,
                    propertyName,
                    aggregationMethodName.ToLower(),
                    $"{aggregationMethodName}{propertyName}"));

            Assert.Equal(expectedAggregateUri, query.ToString());
        }

        [Theory]
        [MemberData(nameof(AggregationTestData))]
        public void Aggregation_ReturnsExpectedResult(string propertyName, string aggregationMethodName)
        {
            // Arrange
            var queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);
            var propertyInfo = queryable.ElementType.GetProperty(propertyName);

            // Get the aggregation method
            var aggregationMethod = GetAggregationMethod(aggregationMethodName, propertyInfo.PropertyType);
            var returnType = GetMethodReturnType(aggregationMethod, propertyInfo.PropertyType);

            // Build expression dynamically, e.g. [Queryable].Average(d1 => d1.Prop)
            var methodCallExpr = BuildMethodCallExpression(queryable, aggregationMethod, propertyInfo);

            // Mock aggregated response
            var aggregationAlias = $"{aggregationMethodName}{propertyName}"; // E.g. AverageProp
            var randomAggregateResult = GenerateRandomAggregateValue(aggregationMethodName, returnType);

            InterceptRequestAndMockResponse(aggregationAlias, randomAggregateResult);

            var queryProvider = new DataServiceQueryProvider(dsContext);
            // Get Execute method defined in DataServiceQueryProvider class
            var executeMethod = GetExecuteMethod();

            // Act
            var result = executeMethod.MakeGenericMethod(returnType).Invoke(queryProvider, new object[] { methodCallExpr });

            // Assert
            Assert.Equal(result, randomAggregateResult);
        }

        [Theory]
        [InlineData("Average")]
        [InlineData("Sum")]
        [InlineData("Min")]
        [InlineData("Max")]
        public void Aggregation_TargetingNavProperty_ExpressionTranslatedToExpectedUri(string aggregationMethodName)
        {
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);
            var navPropertyName = "Product";
            var propertyName = "TaxRate";

            // Build selector expression, e.g. d1 => d1.NavProp.Prop
            var parameterExpr = Expression.Parameter(queryable.ElementType, "d1");
            var selectorExpr = Expression.Lambda(
                Expression.MakeMemberAccess(
                    Expression.MakeMemberAccess(parameterExpr, typeof(Sale).GetProperty(navPropertyName)),
                    typeof(Product).GetProperty(propertyName)),
                parameterExpr);

            var propertyType = ((MemberExpression)selectorExpr.Body).Type;
            // Get the aggregation method
            var aggregationMethod = GetAggregationMethod(aggregationMethodName, propertyType);

            // Build expression dynamically, e.g. [Queryable].Average(d1 => d1.NavProp.Prop)
            var methodCallExpr = BuildMethodCallExpression(queryable, aggregationMethod, selectorExpr);

            // Act
            // Call factory method for creating DataServiceOrderedQuery based on expression
            var query = new DataServiceQueryProvider(dsContext).CreateQuery(methodCallExpr);

            // Assert
            // E.g. http://tempuri.org/Sales?apply=aggregate(NavProp/Prop with average as AverageNavProp_Prop)
            var expectedAggregateUri = $"{serviceUri}/{salesEntitySetName}?$apply=" + string.Format(
                aggregateTransformationTemplate, string.Format(
                    aggregateExpressionTemplate,
                    $"{navPropertyName}/{propertyName}",
                    aggregationMethodName.ToLower(),
                    $"{aggregationMethodName}{navPropertyName}_{propertyName}"));

            Assert.Equal(expectedAggregateUri, query.ToString());
        }

        [Theory]
        [InlineData("Average")]
        [InlineData("Sum")]
        [InlineData("Min")]
        [InlineData("Max")]
        public void Aggregation_TargetingNavProperty_ReturnsExpectedResult(string aggregationMethodName)
        {
            // Arrange
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);
            var navPropertyName = "Product";
            var propertyName = "TaxRate";

            // Build selector expression, e.g. d1 => d1.NavProp.Prop
            var parameterExpr = Expression.Parameter(queryable.ElementType, "d1");
            var selectorExpr = Expression.Lambda(
                Expression.MakeMemberAccess(
                    Expression.MakeMemberAccess(parameterExpr, typeof(Sale).GetProperty(navPropertyName)),
                    typeof(Product).GetProperty(propertyName)),
                parameterExpr);

            var propertyType = ((MemberExpression)selectorExpr.Body).Type;
            // Get the aggregation method
            var aggregationMethod = GetAggregationMethod(aggregationMethodName, propertyType);
            var returnType = GetMethodReturnType(aggregationMethod, propertyType);

            // Build expression dynamically, e.g. [Queryable].Average(d1 => d1.NavProp.Prop)
            var methodCallExpr = BuildMethodCallExpression(queryable, aggregationMethod, selectorExpr);

            // Mock aggregated response
            var aggregationAlias = $"{aggregationMethodName}{navPropertyName}_{propertyName}"; // E.g. AverageNavProp_Prop
            var randomAggregateResult = GenerateRandomAggregateValue(aggregationMethodName, returnType);

            InterceptRequestAndMockResponse(aggregationAlias, randomAggregateResult);

            var queryProvider = new DataServiceQueryProvider(dsContext);
            // Get Execute method defined in DataServiceQueryProvider class
            var executeMethod = GetExecuteMethod();

            // Act
            var result = executeMethod.MakeGenericMethod(returnType).Invoke(queryProvider, new object[] { methodCallExpr });

            // Assert
            Assert.Equal(result, randomAggregateResult);
        }

        [Fact]
        public void CountDistinct_ExpressionTranslatedToExpectedUri()
        {
            // Arrange
            var queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var countDistinctMethod = GetCountDistinctMethod();

            var propertyInfo = queryable.ElementType.GetProperty("RowParity");
            var parameterExpr = Expression.Parameter(queryable.ElementType, "d1");
            var selectorExpr = Expression.Lambda(Expression.MakeMemberAccess(parameterExpr, propertyInfo), parameterExpr);

            var methodCallExpr = Expression.Call(
                null,
                countDistinctMethod.MakeGenericMethod(new Type[] { queryable.ElementType, propertyInfo.PropertyType }),
                new[] { queryable.Expression, Expression.Quote(selectorExpr) });

            // Act
            // Call factory method for creating DataServiceOrderedQuery based on expression
            var query = new DataServiceQueryProvider(dsContext).CreateQuery(methodCallExpr);

            // Assert
            var expectedAggregateUri = $"{serviceUri}/{numbersEntitySetName}?$apply=aggregate(RowParity with countdistinct as CountDistinctRowParity)";
            Assert.Equal(expectedAggregateUri, query.ToString());
        }

        [Fact]
        public void CountDistinct_ReturnsExpectedResult()
        {
            // Arrange
            var queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            MockCountDistinct();

            // Act
            int countDistinct = queryable.CountDistinct(d1 => d1.RowParity);

            // Assert
            Assert.Equal(3, countDistinct);
        }

        [Fact]
        public void CountDistinct_TargetingNavProperty()
        {
            // Arrange
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            MockCountDistinct_TargetingNavProperty();

            // Act
            int countDistinct = queryable.CountDistinct(d1 => d1.Customer.Country);

            // Assert
            Assert.Equal(2, countDistinct);
        }

        [Theory]
        [InlineData("Average")]
        [InlineData("Sum")]
        [InlineData("Min")]
        [InlineData("Max")]
        public void Aggregation_NotSupportedException_ThrownForNonAggregatableProperty(string aggregationMethodName)
        {
            // Arrange
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            // Build selector expression, e.g. d1 => d1.Prop.Length
            var parameterExpr = Expression.Parameter(queryable.ElementType, "d1");
            var selectorExpr = Expression.Lambda(
                Expression.MakeMemberAccess(
                    Expression.MakeMemberAccess(parameterExpr, typeof(Sale).GetProperty("ProductId")),
                    typeof(string).GetProperty("Length")),
                parameterExpr);

            var propertyType = ((MemberExpression)selectorExpr.Body).Type;
            // Get the aggregation method
            var aggregationMethod = GetAggregationMethod(aggregationMethodName, propertyType);
            var returnType = GetMethodReturnType(aggregationMethod, propertyType);

            // Build method call expression dynamically, e.g. Queryable.Average(d1 => d1.Prop.Length)
            var methodCallExpr = BuildMethodCallExpression(queryable, aggregationMethod, selectorExpr);

            var queryProvider = new DataServiceQueryProvider(dsContext);
            // Get Execute method defined in DataServiceQueryProvider class
            var executeMethod = GetExecuteMethod();

            // Act & Assert
            var ex = Assert.Throws<TargetInvocationException>(() =>
                executeMethod.MakeGenericMethod(returnType).Invoke(queryProvider, new object[] { methodCallExpr }));
            Assert.True(ex.InnerException is NotSupportedException);
        }

        [Theory]
        [InlineData("Average")]
        [InlineData("Sum")]
        [InlineData("Min")]
        [InlineData("Max")]
        public void Aggregation_NotSupportedException_ThrownForMemberAccessOnCollectionProperty(string aggregationMethodName)
        {
            // Arrange
            var queryable = this.dsContext.CreateQuery<Product>(productsEntitySetName);

            // Build selector expression, e.g. d1 => d1.CollectionProp.Member
            var parameterExpr = Expression.Parameter(queryable.ElementType, "d1");
            var selectorExpr = Expression.Lambda(
                Expression.MakeMemberAccess(
                    Expression.MakeMemberAccess(parameterExpr, typeof(Product).GetProperty("Sales")),
                    typeof(Collection<Sale>).GetProperty("Count")),
                parameterExpr);

            var propertyType = ((MemberExpression)selectorExpr.Body).Type;
            // Get the aggregation method
            var aggregationMethod = GetAggregationMethod(aggregationMethodName, propertyType);
            var returnType = GetMethodReturnType(aggregationMethod, propertyType);

            // Build method call expression dynamically, e.g. Queryable.Average(d1 => d1.CollectionProp.Member)
            var methodCallExpr = BuildMethodCallExpression(queryable, aggregationMethod, selectorExpr);

            var queryProvider = new DataServiceQueryProvider(dsContext);
            // Get Execute method defined in DataServiceQueryProvider class
            var executeMethod = GetExecuteMethod();

            // Act & Assert
            var ex = Assert.Throws<TargetInvocationException>(() =>
                executeMethod.MakeGenericMethod(returnType).Invoke(queryProvider, new object[] { methodCallExpr }));
            Assert.True(ex.InnerException is NotSupportedException);
        }

        [Fact]
        public void CountDistinct_NotSupportedException_ThrownForNonKnownPrimitiveProperty()
        {
            // Arrange
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => queryable.CountDistinct(d => d.Product));
        }

        [Fact]
        public void CountDistinct_NotSupportedException_ThrownForCollectionProperty()
        {
            // Arrange
            var queryable = this.dsContext.CreateQuery<Product>(productsEntitySetName);

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => queryable.CountDistinct(d => d.Sales));
        }

        [Theory]
        [InlineData("Average")]
        [InlineData("Sum")]
        [InlineData("Min")]
        [InlineData("Max")]
        public void Aggregation_OnFilteredInputSet(string aggregationMethodName)
        {
            // Arrange
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            PropertyInfo propertyInfo = queryable.ElementType.GetProperty("Amount");
            var parameter1Expr = Expression.Parameter(queryable.ElementType, "d1");
            // d1.Amount
            var memberExpr = Expression.MakeMemberAccess(parameter1Expr, propertyInfo);
            // d1.Amount > 1
            var greaterThanExpr = Expression.GreaterThan(memberExpr, Expression.Constant((decimal)1));

            // Get Where method
            var whereMethod = GetWhereMethod();
            // .Where(d1 => d1.Amount > 1)
            var whereExpr = Expression.Call(
                null,
                whereMethod.MakeGenericMethod(new Type[] { queryable.ElementType }),
                new[] {
                    queryable.Expression,
                    Expression.Lambda<Func<Sale, bool>>(greaterThanExpr, parameter1Expr)
                });

            var parameter2Expr = Expression.Parameter(queryable.ElementType, "d2");
            // d2 => d2.Amount
            var selectorExpr = Expression.Lambda(
                Expression.MakeMemberAccess(parameter2Expr, propertyInfo),
                parameter2Expr);

            var propertyType = ((MemberExpression)selectorExpr.Body).Type;
            // Get aggregation method
            var aggregationMethod = GetAggregationMethod(aggregationMethodName, propertyType);

            List<Type> genericArguments = new List<Type>();
            genericArguments.Add(queryable.ElementType);
            if (aggregationMethod.GetGenericArguments().Length > 1)
            {
                genericArguments.Add(propertyType);
            }

            // E.g .Where(d1 => d1.Amount > 1).Average(d2 => d2.Amount)
            var aggregationMethodExpr = Expression.Call(
                null,
                aggregationMethod.MakeGenericMethod(genericArguments.ToArray()),
                new Expression[] { whereExpr, Expression.Quote(selectorExpr) });

            // Act
            // Call factory method for creating DataServiceOrderedQuery based on expression
            var query = new DataServiceQueryProvider(dsContext).CreateQuery(aggregationMethodExpr);

            // Assert
            var expectedAggregateUri = $"{serviceUri}/{salesEntitySetName}?$apply=filter(Amount gt 1)" +
                $"/aggregate(Amount with {aggregationMethodName.ToLower()} as {aggregationMethodName}Amount)";
            Assert.Equal(expectedAggregateUri, query.ToString());
        }

        [Fact]
        public void Aggregation_PrecededByOrderBy_Throws_NotSupportedException()
        {
            // Arrange
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => queryable.OrderBy(d => d.Id).Average(d => d.Amount));
        }

        [Fact]
        public void Aggregation_PrecededBySkip_Throws_NotSupportedException()
        {
            // Arrange
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => queryable.Skip(1).Sum(d => d.Amount));
        }

        [Fact]
        public void Aggregation_PrecededByTake_Throws_NotSupportedException()
        {
            // Arrange
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => queryable.Take(1).Min(d => d.Amount));
        }

        #region Mock Aggregation Responses

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

        #endregion Mock Aggregation Responses
    }
}
