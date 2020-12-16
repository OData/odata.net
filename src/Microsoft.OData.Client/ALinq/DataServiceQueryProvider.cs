//---------------------------------------------------------------------
// <copyright file="DataServiceQueryProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
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
            IQueryable<TElement> query = new DataServiceQuery<TElement>.DataServiceOrderedQuery(expression, this);

            MethodCallExpression mce = expression as MethodCallExpression;
            Debug.Assert(mce != null, "mce != null");

            SequenceMethod sequenceMethod;
            if (ReflectionUtil.TryIdentifySequenceMethod(mce.Method, out sequenceMethod))
            {
                switch (sequenceMethod)
                {
                    case SequenceMethod.Single:
                        return query.AsEnumerable().Single();
                    case SequenceMethod.SingleOrDefault:
                        return query.AsEnumerable().SingleOrDefault();
                    case SequenceMethod.First:
                        return query.AsEnumerable().First();
                    case SequenceMethod.FirstOrDefault:
                        return query.AsEnumerable().FirstOrDefault();
                    case SequenceMethod.LongCount:
                    case SequenceMethod.Count:
                        return ((DataServiceQuery<TElement>)query).GetValue(this.Context, ParseQuerySetCount<TElement>);
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
            Type lastSegmentType = re.Projection == null ? re.ResourceType : re.Projection.Selector.Parameters[0].Type;
            LambdaExpression selector = re.Projection == null ? null : re.Projection.Selector;
            return new QueryComponents(uri, version, lastSegmentType, selector, normalizerRewrites);
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
            IDictionary<string, string> responseHeaders = new Dictionary<string, string>();
            responseHeaders.Add(ODataConstants.ContentTypeHeader, "application/json");
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
                            if (entry != null && entry.Properties.Any())
                            {
                                ODataProperty aggregationProperty = entry.Properties.First();
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
