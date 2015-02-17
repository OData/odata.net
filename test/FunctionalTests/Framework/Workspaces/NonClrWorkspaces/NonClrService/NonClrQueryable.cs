//---------------------------------------------------------------------
// <copyright file="NonClrQueryable.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Data.Test.Astoria.LateBound;
using Microsoft.OData.Service.Providers;
using System.Reflection;

namespace System.Data.Test.Astoria.NonClr
{
    public class CreateQueryInvokedEventArgs : EventArgs
    {
        public CreateQueryInvokedEventArgs(Expression expression)
        {
            this.Expression = expression;
        }

        public readonly Expression Expression;
    }

    public class NonClrQueryProvider : IQueryProvider
    {
        public IQueryProvider realProvider
        {
            get;
            private set;
        }

        private IDataServiceQueryProvider provider;
        public IDataServiceQueryProvider Provider
        {
            get
            {
                if (this.provider == null)
                {
                    throw new InvalidOperationException("IDataServiceQueryProvider not available.");
                }

                return this.provider;
            }
            set
            {
                this.provider = value;
            }
        }

        public NonClrQueryProvider(IQueryProvider provider, IDataServiceQueryProvider dsp)
        {
            this.realProvider = provider;
            this.provider = dsp;
        }

        public System.Linq.IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
        {
            return new NonClrQueryable<TElement>(this.realProvider.CreateQuery<TElement>(expression), this);
        }

        public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
        {
            IQueryable realQueryable = this.realProvider.CreateQuery(expression);
            Type queryableType = typeof(NonClrQueryable<>).MakeGenericType(realQueryable.ElementType);
            return (IQueryable)Activator.CreateInstance(queryableType, realQueryable, this);
        }

        public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
        {
            System.Linq.Expressions.Expression newExpression = LateBoundToClrConverter.ToClrExpression(expression, this.Provider);
            return this.realProvider.Execute<TResult>(newExpression);
        }

        public object Execute(System.Linq.Expressions.Expression expression)
        {
            System.Linq.Expressions.Expression newExpression = LateBoundToClrConverter.ToClrExpression(expression, this.Provider);
            return this.realProvider.Execute(newExpression);
        }
    }

    public class NonClrQueryable : IOrderedQueryable
    {
        private readonly IQueryable queryable;
        protected readonly NonClrQueryProvider provider;

        public NonClrQueryable(IQueryable queryable, NonClrQueryProvider provider)
        {
            this.queryable = queryable;
            this.provider = provider;
        }

        public Type ElementType
        {
            get { return this.queryable.ElementType; }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { return this.queryable.Expression; }
        }

        public System.Linq.IQueryProvider Provider
        {
            get { return this.provider; }
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            CallOrder.APICallLog.Current.Add("NonClrQueryable.GetEnumerator");

            try
            {
                // Remove LateBound methods from expression
                Expression newExpression = LateBoundToClrConverter.ToClrExpression(this.queryable.Expression, this.provider.Provider);
                return new Providers.EnumeratorWrapper(this.provider.realProvider.CreateQuery(newExpression).GetEnumerator());
            }
            finally
            {
                CallOrder.APICallLog.Current.Pop();
            }
        }
    }

    public class NonClrQueryable<TElement> : NonClrQueryable, IOrderedQueryable<TElement>
    {
        private IQueryable<TElement> queryable;

        public NonClrQueryable(IQueryable<TElement> queryable, NonClrQueryProvider provider)
            : base(queryable, provider)
        {
            this.queryable = queryable;
        }

        public new System.Collections.Generic.IEnumerator<TElement> GetEnumerator()
        {
            CallOrder.APICallLog.Current.Add("NonClrQueryableOfT.GetEnumerator");

            try
            {
                // Remove LateBound methods from expression
                Expression newExpression = LateBoundToClrConverter.ToClrExpression(this.queryable.Expression, this.provider.Provider);
                return this.provider.realProvider.CreateQuery<TElement>(newExpression).GetEnumerator();
            }
            finally
            {
                CallOrder.APICallLog.Current.Pop();
            }
        }
    }
}
