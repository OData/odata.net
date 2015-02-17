//---------------------------------------------------------------------
// <copyright file="OpenTypeQueryProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests
{
#region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
#endregion

    public enum PagingStrategy
    {
        None,
        FixedPageSize
    }

    public class OpenTypeQueryProvider : IQueryProvider
    {
        private IQueryProvider innerProvider;
        private IDataServiceQueryProvider dsp;
        private string typeNamePropertyName;

        public static PagingStrategy PagingStrategy;
        public static int PageSize;

        public OpenTypeQueryProvider(IQueryProvider provider, IDataServiceQueryProvider dsp, string typeNamePropertyName)
        {
            this.innerProvider = provider;
            this.dsp = dsp;
            this.typeNamePropertyName = typeNamePropertyName;
        }

        public IDataServiceQueryProvider DataServiceProvider
        {
            get
            {
                if (this.dsp == null)
                {
                    throw new InvalidOperationException("IDataServiceProvider not available.");
                }

                return this.dsp;
            }
            set
            {
                this.dsp = value;
            }
        }
        
        internal object[] ContinuationToken
        {
            get;
            set;
        }
        
        internal ResourceType ResourceType
        {
            get;
            set;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new OpenTypeQueryable<TElement>(this.innerProvider.CreateQuery<TElement>(expression), this);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            IQueryable innerQueryable = this.innerProvider.CreateQuery(expression);

            return (IQueryable)Activator.CreateInstance(
                typeof(OpenTypeQueryable<>).MakeGenericType(innerQueryable.ElementType),
                innerQueryable,
                this);
        }
        
        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)this.ExecuteInternal(expression);
        }

        public object Execute(Expression expression)
        {
            return this.ExecuteInternal(expression);
        }

        internal object ExecuteInternal(Expression expression)
        {
            // Remove OpenType and DataServiceProvider methods from expression
            Expression newExpression = OpenTypeToClrConverter.ToClrExpression(expression, this.DataServiceProvider, this.typeNamePropertyName);
            LambdaExpression l = Expression.Lambda(newExpression, null);
            return l.Compile().DynamicInvoke(null);
        }

        // Remembers the continuation token for the query.
        internal void ApplyContinuationToken(IQueryable query, ResourceType resourceType, object[] continuationToken)
        {
            this.ContinuationToken = continuationToken;
            this.ResourceType = resourceType;
            
            typeof(OpenTypeQueryProvider)
                .GetMethod("ApplyContinuationToQuery", BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(query.ElementType).Invoke(this, new [] {query });
        }
        
        private void ApplyContinuationToQuery<T>(OpenTypeQueryable<T> query)
        {
            query.Initialize(this);
        }
    }

    public class OpenTypeQueryable<T> : IOrderedQueryable<T>
    {
        private readonly IQueryable<T> queryable;
        private readonly OpenTypeQueryProvider provider;

        public OpenTypeQueryable(IQueryable<T> queryable, OpenTypeQueryProvider provider)
        {
            this.queryable = queryable;
            this.provider = provider;
            Initialize(this.provider);
        }

        public Type ElementType
        {
            get { return this.queryable.ElementType; }
        }

        public Expression Expression
        {
            get { return this.queryable.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return this.provider; }
        }

        internal void Initialize(OpenTypeQueryProvider provider)
        {
            this.ContinuationToken = provider.ContinuationToken;
            this.ResourceType = provider.ResourceType;
        }

        private object[] ContinuationToken
        {
            get;
            set;
        }

        private ResourceType ResourceType
        {
            get;
            set;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            Expression pagedExpression = this.ApplyPagingStrategy(this.queryable.Expression);
            return new OpenTypeEnumerator<T>(((IEnumerable)this.provider.ExecuteInternal(pagedExpression)).GetEnumerator());
        }

        public IEnumerator<T> GetEnumerator()
        {
            Expression pagedExpression = this.ApplyPagingStrategy(this.queryable.Expression);
            return new OpenTypeEnumerator<T>(((IEnumerable<T>)this.provider.ExecuteInternal(pagedExpression)).GetEnumerator());
        }
        
        private Expression ApplyPagingStrategy(Expression input)
        {
            if (OpenTypeQueryProvider.PagingStrategy == PagingStrategy.FixedPageSize)
            {
                return this.ApplyTop(
                        this.ApplyOrdering(
                            this.ApplyFilter(input)));
            }
            else
            {
                return input;
            }
        }

        private Expression ApplyFilter(Expression input)
        {
            if (this.ContinuationToken != null)
            {
                PropertyInfo pi = this.ResourceType.InstanceType.GetProperty(this.ResourceType.KeyProperties[0].Name);

                ParameterExpression p = Expression.Parameter(this.ResourceType.InstanceType, "p");
                LambdaExpression predicate = Expression.Lambda(
                                                Expression.GreaterThan(
                                                    Expression.MakeMemberAccess(p, pi),
                                                    Expression.Constant(this.ContinuationToken[0], this.ContinuationToken[0].GetType())),
                                                p);
                return Expression.Call(
                            typeof(Queryable),
                            "Where",
                            new Type[] { this.ResourceType.InstanceType },
                            input,
                            Expression.Quote(predicate));
            }
            else
            {
                return input;
            }
        }        
        
        private Expression ApplyOrdering(Expression input)
        {
            PropertyInfo pi = this.ResourceType.InstanceType.GetProperty(this.ResourceType.KeyProperties[0].Name);
            ParameterExpression p = Expression.Parameter(this.ResourceType.InstanceType, "p");
            LambdaExpression selector = Expression.Lambda(
                                            Expression.MakeMemberAccess(p, pi), 
                                            p);
            return Expression.Call(
                        typeof(Queryable),
                        "OrderBy",
                        new Type[] { this.ResourceType.InstanceType, selector.Body.Type },
                        input,
                        Expression.Quote(selector));
        }

        private Expression ApplyTop(Expression input)
        {
            Type elementType = TypeUtils.GetElementType(input.Type);
            return Expression.Call(
                        typeof(Queryable),
                        "Take",
                        new Type[] { elementType },
                        input, 
                        Expression.Constant(OpenTypeQueryProvider.PageSize));
        }
    }

    internal static class TypeUtils
    {
        /// <summary>
        /// Gets the elementtype for a sequence
        /// </summary>
        /// <param name="seqType">The sequence type</param>
        /// <returns>The element type</returns>
        internal static Type GetElementType(Type seqType)
        {
            Type ienum = FindIEnumerable(seqType);
            if (ienum == null)
            {
                return seqType;
            }

            return ienum.GetGenericArguments()[0];
        }

        /// <summary>
        /// Finds type that implements IEnumerable so can get elemtent type
        /// </summary>
        /// <param name="seqType">The Type to check</param>
        /// <returns>returns the type which implements IEnumerable</returns>
        private static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
            {
                return null;
            }

            if (seqType.IsArray)
            {
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            }

            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }

            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces != null && ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null)
                    {
                        return ienum;
                    }
                }
            }

            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }

            return null;
        }
    }

    // Enumerator that remembers the last instance that was returned.
    public abstract class BaseEnumerator
    {
        public abstract object LastObject
        {
            get;
            set;
        }        
    }

    public class OpenTypeEnumerator<T> : BaseEnumerator, IEnumerator<T>, IDisposable
    {
        private IEnumerator innerEnumerator;

        public OpenTypeEnumerator(IEnumerator<T> innerEnumerator)
        {
            this.innerEnumerator = innerEnumerator;
        }

        public OpenTypeEnumerator(IEnumerator innerEnumerator)
        {
            this.innerEnumerator = innerEnumerator;
        }

        #region IEnumerator Members

        object IEnumerator.Current
        {
            get
            {
                object result = this.innerEnumerator.Current;
                this.LastObject = result;
                return result;
            }
        }

        public bool MoveNext()
        {
            return this.innerEnumerator.MoveNext();
        }

        public void Reset()
        {
            this.innerEnumerator.Reset();
        }

        #endregion

        #region IEnumerator<T> Members

        public T Current
        {
            get
            {
                T result = (T)this.innerEnumerator.Current;
                this.LastObject = result;
                return result;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            IDisposable d = this.innerEnumerator as IDisposable;
            if (d != null)
            {
                d.Dispose();
            }
        }

        #endregion

        public override object LastObject
        {
            get;
            set;
        }
    }    
}
