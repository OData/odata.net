//---------------------------------------------------------------------
// <copyright file="CustomObjectContextWithPaging.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.OData.Service.Providers;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;
using Microsoft.OData.Service;
using AstoriaUnitTests.Stubs;
using ocs = AstoriaUnitTests.ObjectContextStubs;
using Microsoft.OData.Service.Internal;

namespace AstoriaUnitTests.Tests
{
    public class CustomObjectContextWithPaging : CustomRowBasedContext, IServiceProvider
    {
        public new IQueryable<RowEntityTypeWithIDAsKey> Customers
        {
            get
            {
                IDataServiceMetadataProvider provider = base.GetService(typeof(IDataServiceMetadataProvider)) as IDataServiceMetadataProvider;
                ResourceType resourceType = provider.Types.First(t => t.Name == "Customer");
                IQueryable source = base.Customers;
                return new AstoriaUnitTests.Tests.PagedQueryProvider(
                            source.Provider,
                            resourceType,
                            null,
                            new AstoriaUnitTests.Tests.CountManager())
                            .CreateQuery(source.Expression) as IQueryable<RowEntityTypeWithIDAsKey>;
            }
        }

        protected static new string GetTypeName(object instance, out bool collection)
        {
            return CustomRowBasedContext.GetTypeName(instance, out collection);
        }

        #region IServiceProvider Members

        object IServiceProvider.GetService(Type serviceType)
        {
            if (serviceType == typeof(IDataServicePagingProvider))
            {
                return new PagingProvider();
            }

            return base.GetService(serviceType);
        }

        #endregion
    }

    public class CustomReflectionContextWithPaging : CustomDataContext, IServiceProvider
    {
        public new IQueryable<Customer> Customers
        {
            get
            {
                IQueryable source = base.Customers;
                return new AstoriaUnitTests.Tests.PagedQueryProvider(
                    source.Provider,
                    null,
                    null,
                    new AstoriaUnitTests.Tests.CountManager())
                    .CreateQuery(source.Expression) as IQueryable<Customer>;
            }
        }

        #region IServiceProvider Members

        object IServiceProvider.GetService(Type serviceType)
        {
            if (serviceType == typeof(IDataServicePagingProvider))
            {
                return new PagingProvider();
            }

            return null;
        }

        #endregion
    }

    internal class PagingProvider : IDataServicePagingProvider
    {

        private IDataServiceQueryProvider resourceQueryProvider;

        public PagingProvider()
            : this(null) { }

        public PagingProvider(IDataServiceQueryProvider resourceQueryProvider)
        {
            this.resourceQueryProvider = resourceQueryProvider;
        }

        #region IDataServicePagingProvider Members

        public object[] GetContinuationToken(IEnumerator enumerator)
        {
            CountingEnumerator countingEnumerator = enumerator as CountingEnumerator;
            if (countingEnumerator != null)
            {
                return countingEnumerator.GetSkipToken(this.resourceQueryProvider);
            }

            return null;
        }

        public void SetContinuationToken(IQueryable query, ResourceType resourceType, object[] continuationToken)
        {
            PagedQueryProvider pqp = query.Provider as PagedQueryProvider;
            pqp.ApplyContinuationToken(query, resourceType, continuationToken);
        }

        #endregion
    }


    #region Enumerators

    public class CountManager
    {
        public static int MaxCount
        {
            get;
            set;
        }
        
        private int currentCount;
        
        public bool Increment()
        {
            if (this.currentCount == MaxCount)
                return false;

            this.currentCount++;
            
            return true;
        }
    }

    internal class CountingEnumerator : IEnumerable, IEnumerator, IDisposable
    {
        private IEnumerable innerEnumerable;
        private IEnumerator innerEnumerator;
        private object lastObject;
        private ResourceType resourceType;
        private CountManager cm;

        public CountingEnumerator(IEnumerable innerEnumerable, ResourceType resourceType, CountManager cm)
        {
            this.innerEnumerable = innerEnumerable;
            this.resourceType = resourceType;
            this.cm = cm;
        }

        public object[] GetSkipToken(IDataServiceQueryProvider dataServiceQueryPovider)
        {
            if (this.lastObject != null)
            {
                if (this.lastObject.GetType() == typeof(int))
                {
                    return new object[] { -1 };
                }

                object resource = this.lastObject;
                IExpandedResult expandedResult = resource as IExpandedResult;
                if (expandedResult != null)
                {
                    resource = expandedResult.ExpandedElement;
                }

                IEnumerable<string> keyPropertyNames = this.IsReflectableResourceType ?
                    new string[] { "ID" } :
                    this.resourceType.KeyProperties.Select(property => property.Name);

                ProjectedWrapper projectedWrapper = resource as ProjectedWrapper;
                if (projectedWrapper != null)
                {
                    return keyPropertyNames.Select(propertyName =>
                        projectedWrapper.GetProjectedPropertyValue(propertyName)).ToArray();
                }
                else if (dataServiceQueryPovider != null)
                {
                    return keyPropertyNames.Select(propertyName =>
                        dataServiceQueryPovider.GetPropertyValue(resource, this.resourceType.Properties.Single(p => p.Name == propertyName))).ToArray();                    
                }
                else
                {
                    return keyPropertyNames.Select(propertyName =>
                        resource.GetType().GetProperty(propertyName).GetValue(resource, null)).ToArray();
                }
            }

            return new Random().Next() % 2 == 0 ? null : new object[0];
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            this.innerEnumerator = this.innerEnumerable.GetEnumerator();
            return this;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (this.innerEnumerator is IDisposable)
            {
                (this.innerEnumerator as IDisposable).Dispose();
            }
        }

        #endregion

        #region IEnumerator Members

        object IEnumerator.Current
        {
            get
            {
                return this.lastObject;
            }
        }

        private bool IsExpandedWrapperInstance(object resource)
        {
            return resource.GetType().IsGenericType && 
                resource.GetType().GetGenericTypeDefinition() == typeof(Microsoft.OData.Service.Internal.ExpandedWrapper<,>);
        }

        private object WrapEnumerables(object resource)
        {
            if (IsExpandedWrapperInstance(resource))
            {
                IQueryable collectionPropertyQueryable = (resource.GetType().GetProperty("ProjectedProperty0").GetValue(resource, null) as IEnumerable).AsQueryable();
                Type collectionElementType = collectionPropertyQueryable.ElementType;

                object expandedElement = resource.GetType().GetProperty("ExpandedElement").GetValue(resource, null);
                string description = (string)resource.GetType().GetProperty("Description").GetValue(resource, null);

                ResourceProperty expandedProperty = this.resourceType.Properties.First(p => p.Name == description);

                ParameterExpression innerParam = Expression.Parameter(collectionElementType, "inner");
                PropertyInfo keyProperty = collectionElementType.GetProperties().First(p => p.Name.Contains("ID"));

                Expression propertyAccess = Expression.Property(
                                                innerParam,
                                                keyProperty);

                Expression innerExpression = Expression.Call(
                    typeof(Queryable),
                    "OrderBy",
                    new Type[] { collectionElementType, keyProperty.PropertyType },
                    Expression.Constant(collectionPropertyQueryable),
                    Expression.Lambda(propertyAccess, innerParam));

                collectionPropertyQueryable = collectionPropertyQueryable.Provider.CreateQuery(innerExpression);

                object value = typeof(CountingEnumerator)
                                .GetMethod("CreateCountingEnumerator", BindingFlags.Static | BindingFlags.NonPublic)
                                .MakeGenericMethod(collectionElementType)
                                .Invoke(null, new object[] { collectionPropertyQueryable, expandedProperty.ResourceType, this.cm });

                object clone = Activator.CreateInstance(resource.GetType());
                clone.GetType().GetProperty("ExpandedElement").SetValue(clone, expandedElement, null);
                clone.GetType().GetProperty("Description").SetValue(clone, description, null);
                clone.GetType().GetProperty("ProjectedProperty0").SetValue(clone, value, null);
                return clone;
            }
            else if (this.IsReflectableResourceType && typeof(Customer).IsAssignableFrom(resource.GetType()))
            {
                PropertyInfo collectionProperty = resource.GetType().GetProperties().First(p => IsCollectionProperty(p));
                
                IQueryable collectionPropertyQueryable = (collectionProperty.GetValue(resource, null) as IEnumerable).AsQueryable();
                
                Type collectionElementType = AstoriaUnitTests.TypeUtils.GetElementType(collectionProperty.PropertyType);

                ParameterExpression innerParam = Expression.Parameter(collectionElementType, "inner");
                
                PropertyInfo keyProperty = collectionElementType.GetProperties().First(p => p.Name.Contains("ID"));
                
                Expression propertyAccess = Expression.Property(
                                                innerParam,
                                                keyProperty);

                Expression innerExpression = Expression.Call(
                    typeof(Queryable),
                    "OrderBy",
                    new Type[] { collectionElementType, keyProperty.PropertyType },
                    Expression.Constant(collectionPropertyQueryable),
                    Expression.Lambda(propertyAccess, innerParam));
                
                collectionPropertyQueryable = collectionPropertyQueryable.Provider.CreateQuery(innerExpression);

                object value = typeof(CountingEnumerator)
                                .GetMethod("CreateCountingEnumerator", BindingFlags.Static | BindingFlags.NonPublic)
                                .MakeGenericMethod(collectionElementType)
                                .Invoke(null, new object[] { collectionPropertyQueryable, null, this.cm });

                object clone = GetClone(resource);
                collectionProperty.SetValue(clone, value, null);
                return clone;
            }
            else
            {
                RowEntityTypeWithIDAsKey row = resource as RowEntityTypeWithIDAsKey;
                if (row == null)
                {
                    return resource;
                }

                RowEntityTypeWithIDAsKey newRow = new RowEntityTypeWithIDAsKey(row.TypeName);
                newRow.ID = row.ID;
                foreach (KeyValuePair<string, object> kv in row.Properties)
                {
                    ResourceProperty property = this.resourceType.Properties.FirstOrDefault(p => p.Name == kv.Key);
                    if (property != null && property.Kind == ResourcePropertyKind.ResourceSetReference)
                    {
                        IEnumerable enumerable = kv.Value as IEnumerable;
                        if (enumerable == null)
                        {
                            newRow.Properties.Add(kv);
                        }
                        else
                        {
                            IQueryable queryable = enumerable.AsQueryable();
                            ParameterExpression innerParam = Expression.Parameter(property.ResourceType.InstanceType, "inner");
                            ResourceProperty keyProperty = property.ResourceType.KeyProperties[0];
                            Expression propertyAccess = keyProperty.CanReflectOnInstanceTypeProperty ?
                                (Expression)Expression.Property(
                                    innerParam,
                                    property.ResourceType.InstanceType.GetProperty(keyProperty.Name)) :
                                (Expression)Expression.Convert(Expression.Call(
                                    Expression.Property(innerParam, "Properties"),
                                    Expression.Property(innerParam, "Properties").Type.GetProperty("Item").GetGetMethod(),
                                    Expression.Constant(keyProperty.Name)), keyProperty.ResourceType.InstanceType);
                            Expression innerExpression = Expression.Call(
                                typeof(Queryable),
                                "OrderBy",
                                new Type[] { property.ResourceType.InstanceType, property.ResourceType.KeyProperties[0].ResourceType.InstanceType },
                                Expression.Constant(queryable),
                                Expression.Lambda(propertyAccess, innerParam));
                            queryable = queryable.Provider.CreateQuery(innerExpression);

                            object value = new CountingEnumerator(queryable, property.ResourceType, this.cm);
                            newRow.Properties.Add(kv.Key, value);
                        }
                    }
                    else
                    {
                        newRow.Properties[kv.Key] = kv.Value;
                    }
                }

                return newRow;
            }
        }

        public bool MoveNext()
        {
            if (this.cm.Increment())
            {
                if (!this.innerEnumerator.MoveNext())
                {
                    this.lastObject = null;
                }
                else
                {
                    this.lastObject = this.WrapEnumerables(this.innerEnumerator.Current);
                    return true;
                }
            }
            else
            {
                if (this.innerEnumerator.MoveNext())
                {
                    if (this.lastObject == null)
                    {
                        this.lastObject = -1;
                    }
                }
                else
                {
                    this.lastObject = null;
                }
            }

            return false;
        }

        public void Reset()
        {
            this.innerEnumerator.Reset();
        }

        #endregion

        private static CountingEnumerator<T> CreateCountingEnumerator<T>(IEnumerable e, ResourceType rt, CountManager cm) where T : class
        {
            return new CountingEnumerator<T>(e, rt, cm);
        }

        private static object GetClone(object resource)
        {
            object clone;
            if (resource.GetType() == typeof(Customer))
            {
                Customer original = (Customer)resource;
                clone = new Customer { ID = original.ID };
                original.Clone(clone);
            }
            else
            {
                CustomerWithBirthday original = (CustomerWithBirthday)resource;
                clone = new CustomerWithBirthday { ID = original.ID };
                original.Clone(clone);
            }

            return clone;
        }

        private bool IsReflectableResourceType
        {
            get
            {
                return this.resourceType == null || this.resourceType.CanReflectOnInstanceType == true;
            }
        }

        private bool IsCollectionProperty(PropertyInfo property)
        {
            return typeof(IEnumerable<Customer>).IsAssignableFrom(property.PropertyType) ||
                   typeof(IEnumerable<Order>).IsAssignableFrom(property.PropertyType);
        }
    }

    // Class to make CustomDataContext.Orders property assignment work.
    internal class CountingEnumerator<T> : CountingEnumerator, IEnumerable<T>, IEnumerator<T>, ICollection<T>
    {
        public CountingEnumerator(IEnumerable innerEnumerable, ResourceType resourceType, CountManager cm) 
            : base(innerEnumerable, resourceType, cm)
        {
        }

        #region IEnumerable<T> Members
        public IEnumerator<T> GetEnumerator() 
        {
            ((IEnumerable)this).GetEnumerator();
            return this; 
        }
        #endregion

        #region IEnumerator<T> Members
        public T Current 
        { 
            get 
            {
                return (T)((IEnumerator)this).Current;
            } 
        }
        #endregion

        #region ICollection<T> Members
        public void Add(T item) { throw new NotImplementedException(); }
        public void Clear() { throw new NotImplementedException(); }
        public bool Contains(T item) { throw new NotImplementedException(); }
        public void CopyTo(T[] array, int arrayIndex) { throw new NotImplementedException(); }
        public int Count { get { throw new NotImplementedException(); } }
        public bool IsReadOnly { get { throw new NotImplementedException(); } }
        public bool Remove(T item) { throw new NotImplementedException(); }
        #endregion
    }

    #endregion

    #region Query support

    internal class PagedQueryProvider : IQueryProvider
    {
        private readonly IDataServiceQueryProvider dataServiceQueryProvider;

        public PagedQueryProvider(IDataServiceQueryProvider dataServiceQueryProvider, IQueryProvider provider, ResourceType resourceType, object[] continuationToken, CountManager countManager)
            : this(provider, resourceType, continuationToken, countManager)
        {
            this.dataServiceQueryProvider = dataServiceQueryProvider;
        }

        public PagedQueryProvider(IQueryProvider provider, ResourceType resourceType, object[] continuationToken, CountManager countManager)
        {
            this.InnerProvider = provider;
            this.ContinuationToken = continuationToken;
            this.ResourceType = resourceType;
            this.CountManager = countManager;
        }

        public ResourceType ResourceType
        {
            get;
            set;
        }

        public object[] ContinuationToken
        {
            get;
            set;
        }

        public CountManager CountManager
        {
            get;
            set;
        }

        private IQueryProvider InnerProvider
        {
            get;
            set;
        }

        public IDataServiceQueryProvider DataServiceQueryProvider
        {
            get { return this.dataServiceQueryProvider; }
        }

        #region IQueryProvider Members

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            IQueryable<TElement> innerQueryable = this.InnerProvider.CreateQuery<TElement>(expression);
            this.InnerProvider = innerQueryable.Provider;
            return new PagingQueryable<TElement>(innerQueryable, this);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            IQueryable innerQueryable = this.InnerProvider.CreateQuery(expression);
            this.InnerProvider = innerQueryable.Provider;
            return (IQueryable)Activator.CreateInstance(
                typeof(PagingQueryable<>).MakeGenericType(innerQueryable.ElementType),
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
            return this.InnerProvider.Execute(expression);
        }

        internal void ApplyContinuationToken(IQueryable query, ResourceType resourceType, object[] continuationToken)
        {
            this.ContinuationToken = continuationToken;
            this.ResourceType = resourceType;

            typeof(PagedQueryProvider)
                .GetMethod("ApplyContinuationToQuery", BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(query.ElementType).Invoke(this, new[] { query });
        }

        private void ApplyContinuationToQuery<T>(PagingQueryable<T> query)
        {
            query.Initialize((PagedQueryProvider)query.Provider);
        }

        #endregion
    }

    internal class PagingQueryable<T> : IOrderedQueryable<T>
    {
        private readonly IQueryable<T> queryable;
        private readonly PagedQueryProvider provider;

        public PagingQueryable(IQueryable<T> queryable, PagedQueryProvider provider)
        {
            this.queryable = queryable;
            this.provider = provider;
            this.Initialize(this.provider);
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

        public IQueryable<T> InnerQueryable
        {
            get { return this.queryable; }
        }

        internal void Initialize(PagedQueryProvider provider)
        {
            this.CountManager = provider.CountManager;
            this.ContinuationToken = provider.ContinuationToken;
            this.ResourceType = provider.ResourceType;
        }

        private object[] ContinuationToken
        {
            get;
            set;
        }

        private CountManager CountManager
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
            return this.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            Expression expression = this.InnerQueryable.Expression;
            expression = this.ApplyPagingStrategy(expression);
            PagingQueryable<T> finalQuery = (PagingQueryable<T>)this.provider.CreateQuery<T>(expression);
            return ((new CountingEnumerator<T>(finalQuery.InnerQueryable as IEnumerable, this.ResourceType, this.CountManager)) as IEnumerable<T>).GetEnumerator();
        }

        private Expression ApplyPagingStrategy(Expression input)
        {
            if (this.ContinuationToken != null)
            {
                input = this.ApplyFilter(input);
            }

            return this.ApplyOrdering(input);
        }

        private Expression ApplyFilter(Expression input)
        {
            // When continuation token is -1 then we have to return everything.
            if (this.ContinuationToken.Length == 1 && this.ContinuationToken[0].GetType() == typeof(int) && ((int)this.ContinuationToken[0]) == -1)
            {
                return input;
            }
        
            Type filteredType = input.Type.GetGenericArguments()[0];

            ParameterExpression p = Expression.Parameter(filteredType, "p");

            LambdaExpression predicate = Expression.Lambda(
                Expression.GreaterThan(
                    Expression.Convert(
                        AccessProperty(p, this.ResourceType, this.ResourceType.KeyProperties[0].Name, provider.DataServiceQueryProvider), 
                        this.ContinuationToken[0].GetType()),
                    Expression.Constant(this.ContinuationToken[0], this.ContinuationToken[0].GetType())),
                p);

            return Expression.Call(
                        typeof(Queryable),
                        "Where",
                        new Type[] { filteredType },
                        input,
                        Expression.Quote(predicate));
        }

        private Expression ApplyOrdering(Expression input)
        {
            Type orderedType = input.Type.GetGenericArguments()[0];
            
            ParameterExpression p = Expression.Parameter(orderedType, "p");

            string keyPropertyName = this.ResourceType == null ?
                "ID" :
                this.ResourceType.KeyProperties[0].Name;

            LambdaExpression selector = Expression.Lambda(
                AccessProperty(p, this.ResourceType, keyPropertyName, provider.DataServiceQueryProvider),
                p);
                                                       
            return Expression.Call(
                        typeof(Queryable),
                        "OrderBy",
                        new Type[] { orderedType, selector.Body.Type },
                        input,
                        Expression.Quote(selector));
        }

        private static Expression AccessProperty(Expression source, ResourceType resourceType, string propertyName, IDataServiceQueryProvider dataServiceQueryProvider)
        {
            source = UnwrapExpandedWrapper(source);

            Type projectedWrapperType = typeof(Microsoft.OData.Service.Internal.ProjectedWrapper);
            if (projectedWrapperType.IsAssignableFrom(source.Type))
            {
                return Expression.Call(
                    source,
                    projectedWrapperType.GetMethod("GetProjectedPropertyValue"),
                    Expression.Constant(propertyName));
            }
            else if (dataServiceQueryProvider != null)
            {
                return Expression.Call(
                    Expression.Constant(dataServiceQueryProvider),
                    typeof(IDataServiceQueryProvider).GetMethod("GetPropertyValue"),
                    source,
                    Expression.Constant(resourceType.Properties.Single(p => p.Name == propertyName)));
            }
            else
            {
                return Expression.Property(Expression.Convert(source, resourceType.InstanceType), propertyName);
            }
        }

        private static Expression UnwrapExpandedWrapper(Expression expression)
        {
            Type expandedResultType = typeof(Microsoft.OData.Service.IExpandedResult);
            if (expandedResultType.IsAssignableFrom(expression.Type))
            {
                expression = Expression.Property(expression, expandedResultType.GetProperty("ExpandedElement"));
            }

            return expression;
        }
    }
    #endregion
}
