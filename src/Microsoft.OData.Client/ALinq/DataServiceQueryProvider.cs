//---------------------------------------------------------------------
// <copyright file="DataServiceQueryProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Xml;

    #endregion Namespaces

    /// <summary>
    /// QueryProvider implementation
    /// </summary>
    public sealed class DataServiceQueryProvider : IQueryProvider
    {
        /// <summary>DataServiceContext for query provider</summary>
        internal readonly DataServiceContext Context;

        /// <summary>Constructs a query provider based on the context passed in </summary>
        /// <param name="context">The context for the query provider</param>
        internal DataServiceQueryProvider(DataServiceContext context)
        {
            this.Context = context;
        }

        #region IQueryProvider implementation

        /// <summary>Factory method for creating DataServiceOrderedQuery based on expression </summary>
        /// <param name="expression">The expression for the new query</param>
        /// <returns>new DataServiceQuery</returns>
        public IQueryable CreateQuery(Expression expression)
        {
            Util.CheckArgumentNull(expression, "expression");
            Type et = TypeSystem.GetElementType(expression.Type);
            Type qt = typeof(DataServiceQuery<>.DataServiceOrderedQuery).MakeGenericType(et);
            object[] args = new object[] { expression, this };

            ConstructorInfo ci = qt.GetInstanceConstructor(
                false /*isPublic*/,
                new Type[] { typeof(Expression), typeof(DataServiceQueryProvider) });

            return (IQueryable)Util.ConstructorInvoke(ci, args);
        }

        /// <summary>Factory method for creating DataServiceOrderedQuery based on expression </summary>
        /// <typeparam name="TElement">generic type</typeparam>
        /// <param name="expression">The expression for the new query</param>
        /// <returns>new DataServiceQuery</returns>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            Util.CheckArgumentNull(expression, "expression");
            return new DataServiceQuery<TElement>.DataServiceOrderedQuery(expression, this);
        }

        /// <summary>Creates and executes a DataServiceQuery based on the passed in expression</summary>
        /// <param name="expression">The expression for the new query</param>
        /// <returns>the results</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining | System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
        public object Execute(Expression expression)
        {
            Util.CheckArgumentNull(expression, "expression");

            MethodInfo mi = typeof(DataServiceQueryProvider).GetMethod("ReturnSingleton", false /*isPublic*/, false /*isStatic*/);
            return mi.MakeGenericMethod(expression.Type).Invoke(this, new object[] { expression });
        }

        /// <summary>Creates and executes a DataServiceQuery based on the passed in expression</summary>
        /// <typeparam name="TResult">generic type</typeparam>
        /// <param name="expression">The expression for the new query</param>
        /// <returns>the results</returns>
        public TResult Execute<TResult>(Expression expression)
        {
            Util.CheckArgumentNull(expression, "expression");
            return ReturnSingleton<TResult>(expression);
        }

        #endregion


        /// <summary>Creates and executes a DataServiceQuery based on the passed in expression which results a single value</summary>
        /// <typeparam name="TElement">generic type</typeparam>
        /// <param name="expression">The expression for the new query</param>
        /// <returns>single valued results</returns>
        internal TElement ReturnSingleton<TElement>(Expression expression)
        {
            IQueryable<TElement> query = CreateQuery<TElement>(expression);

            MethodCallExpression mce = expression as MethodCallExpression;
            Debug.Assert(mce != null, "mce != null");

            SequenceMethod sequenceMethod;
            if (ReflectionUtil.TryIdentifySequenceMethod(mce.Method, out sequenceMethod))
            {
                switch (sequenceMethod)
                {
                    case SequenceMethod.Single:
                        return query.AsEnumerable().Single();
                    case SequenceMethod.SinglePredicate:
                        query = CreateQuery<TElement>(NestPredicateExpression(mce));
                        return query.AsEnumerable().Single();
                    case SequenceMethod.SingleOrDefault:
                        return query.AsEnumerable().SingleOrDefault();
                    case SequenceMethod.SingleOrDefaultPredicate:
                        query = CreateQuery<TElement>(NestPredicateExpression(mce));
                        return query.AsEnumerable().SingleOrDefault();
                    case SequenceMethod.First:
                        return query.AsEnumerable().First();
                    case SequenceMethod.FirstPredicate:
                        query = CreateQuery<TElement>(NestPredicateExpression(mce));
                        return query.AsEnumerable().First();
                    case SequenceMethod.FirstOrDefault:
                        return query.AsEnumerable().FirstOrDefault();
                    case SequenceMethod.FirstOrDefaultPredicate:
                        query = CreateQuery<TElement>(NestPredicateExpression(mce));
                        return query.AsEnumerable().FirstOrDefault();
                    case SequenceMethod.LongCount:
                    case SequenceMethod.Count:
                        return ((DataServiceQuery<TElement>)query).GetValue(this.Context, ParseQuerySetCount<TElement>);
                    case SequenceMethod.LongCountPredicate:
                    case SequenceMethod.CountPredicate:
                        query = CreateQuery<TElement>(NestPredicateExpression(mce));
                        return ((DataServiceQuery<TElement>)query).GetValue(this.Context, ParseQuerySetCount<TElement>);
                    case SequenceMethod.Any:
                        return GetValueForAny<TElement>(mce);
                    case SequenceMethod.AnyPredicate:
                        return GetValueForAny<TElement>(NestPredicateExpression(mce));
                    case SequenceMethod.SumIntSelector:
                    case SequenceMethod.SumDoubleSelector:
                    case SequenceMethod.SumDecimalSelector:
                    case SequenceMethod.SumLongSelector:
                    case SequenceMethod.SumSingleSelector:
                    case SequenceMethod.SumNullableIntSelector:
                    case SequenceMethod.SumNullableDoubleSelector:
                    case SequenceMethod.SumNullableDecimalSelector:
                    case SequenceMethod.SumNullableLongSelector:
                    case SequenceMethod.SumNullableSingleSelector:
                    case SequenceMethod.AverageIntSelector:
                    case SequenceMethod.AverageDoubleSelector:
                    case SequenceMethod.AverageDecimalSelector:
                    case SequenceMethod.AverageLongSelector:
                    case SequenceMethod.AverageSingleSelector:
                    case SequenceMethod.AverageNullableIntSelector:
                    case SequenceMethod.AverageNullableDoubleSelector:
                    case SequenceMethod.AverageNullableDecimalSelector:
                    case SequenceMethod.AverageNullableLongSelector:
                    case SequenceMethod.AverageNullableSingleSelector:
                    case SequenceMethod.MinSelector: // Mapped to a generic expression - Min(IQueryable`1<T0>, Expression`1<Func`2<T0, T1>>)->T1
                    case SequenceMethod.MaxSelector: // Mapped to a generic expression - Max(IQueryable`1<T0>, Expression`1<Func`2<T0, T1>>)->T1
                    case SequenceMethod.CountDistinctSelector: // Mapped to a generic expression - CountDistinct(IQueryable`1<T0>, Expression`1<Func`2<T0, T1>>)->Int32
                        return ((DataServiceQuery<TElement>)query).GetValue(this.Context, this.ParseAggregateSingletonResult<TElement>);
                    default:
                        throw Error.MethodNotSupported(mce);
                }
            }

            // Should never get here - should be caught by expression compiler.
            Debug.Assert(false, "Not supported singleton operator not caught by Resource Binder");
            throw Error.MethodNotSupported(mce);
        }

        /// <summary>Builds the Uri for the expression passed in.</summary>
        /// <param name="e">The expression to translate into a Uri</param>
        /// <returns>Query components</returns>
        internal QueryComponents Translate(Expression e)
        {
            Uri uri;
            Version version;
            bool addTrailingParens = false;
            Dictionary<Expression, Expression> normalizerRewrites = null;

            // short cut analysis if just a resource set or singleton resource.
            // note - to be backwards compatible with V1, will only append trailing () for queries
            // that include more then just a resource set.
            if (!(e is QueryableResourceExpression))
            {
                normalizerRewrites = new Dictionary<Expression, Expression>(ReferenceEqualityComparer<Expression>.Instance);
                e = Evaluator.PartialEval(e);
                e = ExpressionNormalizer.Normalize(e, normalizerRewrites);
                e = ResourceBinder.Bind(e, this.Context);
                addTrailingParens = true;
            }

            UriWriter.Translate(this.Context, addTrailingParens, e, out uri, out version);
            ResourceExpression re = e as ResourceExpression;
            ApplyQueryOptionExpression applyQueryOptionExpr = (re as QueryableResourceExpression)?.Apply;
            Type lastSegmentType;

            // The KeySelectorMap property is initialized and populated with a least one item if we're dealing with a GroupBy expression.
            // In that case, the selector in the Projection will take the following form:
            // (d2, d3) => new <>f_AnonymousType13`2(CategoryName = d2, AverageAmount = d3.Average(d4 => d4))
            // We examine the 2nd parameter to determine the type of values in the IGrouping<TKey, TElement>
            // The TElement type implements IEnumerable and the first generic argument should be our last segment type
            if (applyQueryOptionExpr?.KeySelectorMap?.Count > 0)
            {
                lastSegmentType = re.Projection.Selector.Parameters[1].Type.GetGenericArguments()[0];
            }
            else
            {
                lastSegmentType = re.Projection == null ? re.ResourceType : re.Projection.Selector.Parameters[0].Type;
            }

            LambdaExpression selector = re.Projection == null ? null : re.Projection.Selector;
            QueryComponents queryComponents = new QueryComponents(uri, version, lastSegmentType, selector, normalizerRewrites);
            queryComponents.GroupByKeySelectorMap = applyQueryOptionExpr?.KeySelectorMap;

            return queryComponents;
        }

        /// <summary>
        /// Transforms the 'any' query into a 'count' request since OData does not have a spcific query for 'any'.
        /// Then the result is casted to the corresponding return type (boolean).
        /// </summary>
        /// <typeparam name="TElement">The return type.</typeparam>
        /// <param name="mce">The original expression with predicate.</param>
        /// <returns></returns>
        private TElement GetValueForAny<TElement>(MethodCallExpression mce)
        {
            Expression arg0 = mce.Arguments[0];
            Expression countExpression = Expression.Call(
                typeof(Enumerable),
                "Count",
                new Type[] { arg0.Type.GetGenericArguments()[0] },
                arg0
            );
            var query = CreateQuery<TElement>(countExpression) as DataServiceQuery<TElement>;
            return query.GetValue(Context, ParseQuerySetCount<TElement>);
        }

        /// <summary>
        /// Transforms the expression type to one of type 'where'.
        /// Then it wraps this 'where' expression into one of the received type but without a predicate.
        /// </summary>
        /// <param name="mce">The original expression with predicate.</param>
        /// <returns>The wrapped expression.</returns>
        private static MethodCallExpression NestPredicateExpression(MethodCallExpression mce)
        {
            Type resourceType = mce.Arguments[0].Type.GetGenericArguments()[0];

            Expression where = Expression.Call(
                typeof(Queryable),
                "Where",
                new Type[] { resourceType },
                mce.Arguments[0],
                mce.Arguments[1]
            );

            return Expression.Call(
                typeof(Enumerable),
                mce.Method.Name,
                new Type[] { resourceType },
                where
            );
        }

        /// <summary>
        /// Parses the result of a query set count request.
        /// </summary>
        /// <typeparam name="TElement">The return type.</typeparam>
        /// <param name="queryResult">The query result.</param>
        /// <returns></returns>
        private static TElement ParseQuerySetCount<TElement>(QueryResult queryResult)
        {
            StreamReader reader = new StreamReader(queryResult.GetResponseStream());
            long querySetCount = -1;

            try
            {
                querySetCount = XmlConvert.ToInt64(reader.ReadToEnd());
            }
            finally
            {
                reader.Close();
            }

            return (TElement)Convert.ChangeType(querySetCount, typeof(TElement), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <summary>
        /// Parses the scalar result of an aggegrate request.
        /// </summary>
        /// <typeparam name="TElement">The return type.</typeparam>
        /// <param name="queryResult">The query result.</param>
        /// <returns></returns>
        private TElement ParseAggregateSingletonResult<TElement>(QueryResult queryResult)
        {
            IDictionary<string, string> responseHeaders = new Dictionary<string, string>
            {
                { ODataConstants.ContentTypeHeader, "application/json" }
            };
            HttpWebResponseMessage httpWebResponseMessage = new HttpWebResponseMessage(
                responseHeaders, (int)queryResult.StatusCode, queryResult.GetResponseStream);

            ODataMessageReaderSettings messageReaderSettings = new ODataMessageReaderSettings
            {
                Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType
            };

            ODataResource entry = default(ODataResource);
            using (ODataMessageReader messageReader = new ODataMessageReader(
                httpWebResponseMessage, messageReaderSettings, this.Context.Format.ServiceModel))
            {
                ODataReader reader = messageReader.CreateODataResourceSetReader();
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceEnd:
                            entry = reader.Item as ODataResource;
                            IEnumerable<ODataProperty> properties = entry.Properties?.OfType<ODataProperty>();
                            if (entry != null && properties?.Any() == true)
                            {
                                ODataProperty aggregationProperty = properties.First();
                                ODataUntypedValue untypedValue = aggregationProperty.Value as ODataUntypedValue;

                                Type underlyingType = Nullable.GetUnderlyingType(typeof(TElement));
                                if (underlyingType == null) // Not a nullable type
                                {
                                    underlyingType = typeof(TElement);
                                }

                                return (TElement)Convert.ChangeType(
                                    untypedValue.RawValue,
                                    underlyingType,
                                    System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            // Failed to retrieve the aggregate result for whatever reason
            throw new DataServiceQueryException(Strings.DataServiceRequest_FailGetValue);
        }
    }
}
