//---------------------------------------------------------------------
// <copyright file="DollarApplyTestsBase.cs" company="Microsoft">
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
using Microsoft.OData.Edm;

namespace Microsoft.OData.Client.Tests.ALinq
{
    /// <summary>
    /// Base class for $apply tests
    /// </summary>
    public class DollarApplyTestsBase
    {
        private Random rand = new Random();

        protected readonly DataServiceContext dsContext;
        protected const string serviceUri = "http://tempuri.org";
        protected const string numbersEntitySetName = "Numbers";
        protected const string salesEntitySetName = "Sales";
        protected const string productsEntitySetName = "Products";
        protected static string aggregateExpressionTemplate = "{0} with {1} as {2}";
        protected static string aggregateTransformationTemplate = "aggregate({0})";

        public DollarApplyTestsBase()
        {
            EdmModel model = BuildEdmModel();

            dsContext = new DataServiceContext(new Uri(serviceUri));
            dsContext.Format.UseJson(model);
        }

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
            entityContainer.AddEntitySet(productsEntitySetName, productEntity);

            return model;
        }

        /// <summary>
        /// Uses reflection to find the relevant aggregation method
        /// </summary>
        protected static MethodInfo GetAggregationMethod(string aggregationMethodName, Type genericArgumentType)
        {
            return typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                            .Where(d1 => d1.Name.Equals(aggregationMethodName))
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

        /// <summary>
        /// Uses reflection to find the CountDistinct method
        /// </summary>
        protected static MethodInfo GetCountDistinctMethod()
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

        /// <summary>
        /// Uses reflection to find the Where method
        /// </summary>
        protected static MethodInfo GetWhereMethod()
        {
            return typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .Where(d1 => d1.Name.Equals("Where", StringComparison.Ordinal))
                    .Select(d2 => new { Method = d2, Parameters = d2.GetParameters() })
                    .Where(d3 => d3.Parameters.Length.Equals(2)
                        && d3.Parameters[0].ParameterType.IsGenericType
                        && d3.Parameters[0].ParameterType.GetGenericTypeDefinition().Equals(typeof(IQueryable<>))
                        && d3.Parameters[1].ParameterType.IsGenericType
                        && d3.Parameters[1].ParameterType.GetGenericTypeDefinition().Equals(typeof(Expression<>)))
                    .Select(d4 => new { d4.Method, SelectorArguments = d4.Parameters[1].ParameterType.GetGenericArguments() })
                    .Where(d5 => d5.SelectorArguments.Length.Equals(1)
                        && d5.SelectorArguments[0].IsGenericType
                        && d5.SelectorArguments[0].GetGenericTypeDefinition().Equals(typeof(Func<,>))) // Func<TSource, Boolean>
                    .Select(d6 => d6.Method).Single();
        }

        /// <summary>
        /// Builds a method call expression dynamically.
        /// </summary>
        protected static MethodCallExpression BuildMethodCallExpression<T>(DataServiceQuery<T> queryable, MethodInfo methodInfo, PropertyInfo propertyInfo)
        {
            ParameterExpression parameterExpr = Expression.Parameter(queryable.ElementType, "d1");
            LambdaExpression selectorExpr = Expression.Lambda(Expression.MakeMemberAccess(parameterExpr, propertyInfo), parameterExpr);

            return BuildMethodCallExpression(queryable, methodInfo, selectorExpr);
        }

        /// <summary>
        /// Builds a method call expression dynamically.
        /// </summary>
        protected static MethodCallExpression BuildMethodCallExpression<T>(DataServiceQuery<T> queryable, MethodInfo methodInfo, LambdaExpression selectorExpr)
        {
            Type propertyType = ((MemberExpression)selectorExpr.Body).Type;

            List<Type> genericArguments = new List<Type>();
            genericArguments.Add(queryable.ElementType);
            if (methodInfo.GetGenericArguments().Length > 1)
            {
                genericArguments.Add(propertyType);
            }

            return Expression.Call(
                null,
                methodInfo.MakeGenericMethod(genericArguments.ToArray()),
                new[] { queryable.Expression, Expression.Quote(selectorExpr) });
        }

        /// <summary>
        /// Uses reflection to find the Execute method defined in DataServiceQueryProvider class
        /// </summary>
        protected static MethodInfo GetExecuteMethod()
        {
            return typeof(DataServiceQueryProvider).GetMethods()
                .Where(d => d.Name.Equals("Execute")
                    && d.IsGenericMethodDefinition
                    && d.GetParameters().Length == 1
                    && d.GetParameters()[0].ParameterType.Equals(typeof(Expression))
                    && d.IsPublic
                    && !d.IsStatic
                ).FirstOrDefault();
        }

        protected static Type GetMethodReturnType(MethodInfo methodInfo, Type genericArgumentType)
        {
            Type returnType = genericArgumentType;
            if ((genericArgumentType.Equals(typeof(int)) || genericArgumentType.Equals(typeof(long)))
                && methodInfo.Name.Equals("Average", StringComparison.OrdinalIgnoreCase))
            {
                returnType = methodInfo.ReturnType;
            }

            return returnType;
        }

        /// <summary>
        /// Generates a random and valid aggregated value based on the aggregation method
        /// </summary>
        protected object GenerateRandomAggregateValue(string aggregationMethodName, Type returnType)
        {
            int lowerBound = 100;
            int upperBound = 1000;
            object aggregationValue;

            switch (aggregationMethodName.ToLowerInvariant())
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

        protected void InterceptRequestAndMockResponse(string aggregateAlias, object aggregateValue)
        {
            var mockResponse = string.Format(
                    "{{\"@odata.context\":\"{0}/$metadata#{1}({2})\",\"value\":[{{\"@odata.id\":null,\"{2}\":{3}}}]}}",
                    serviceUri,
                    numbersEntitySetName,
                    aggregateAlias,
                    aggregateValue);

            InterceptRequestAndMockResponse(mockResponse);
        }

        protected void InterceptRequestAndMockResponse(string mockResponse)
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
    }
}
