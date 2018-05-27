//---------------------------------------------------------------------
// <copyright file="DSPResourceQueryProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.OData.Service.Providers;
    using BindingFlags = System.Reflection.BindingFlags;

    /// <summary>Implementation of the <see cref="IDataServiceQueryProvider"/>.</summary>
    public class DSPResourceQueryProvider : IDataServiceQueryProvider
    {
        /// <summary>The "context" which is the data source.</summary>
        private DSPContext dataSource;

        /// <summary>The metadata provider.</summary>
        private DSPMetadata metadata;

        private bool customPagingEnabled;

        /// <summary>Constructor.</summary>
        /// <param name="metadata">The metadata provider.</param>
        public DSPResourceQueryProvider(DSPMetadata metadata, bool enableCustomPaging)
        {
            this.metadata = metadata;
            this.customPagingEnabled = enableCustomPaging;
        }

        #region IDataServiceQueryProvider Members

        /// <summary>Sets or gets the data source.</summary>
        public object CurrentDataSource
        {
            get
            {
                return this.dataSource;
            }
            set
            {
                if (this.dataSource != null)
                {
                    throw new InvalidOperationException("CurrentDataSource should only be set once.");
                }

                this.dataSource = (DSPContext)value;
            }
        }

        /// <summary>Gets a value of open property for the specified resource.</summary>
        /// <param name="target">The target resource to get a value of property from.</param>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <returns>Value of the property (can be null).</returns>
        /// <remarks>The method will only be called for properties which are not declared on the resource's resource type.
        /// Properties can have null values. The type of the property is determined from the returned object, only primitive types are supported for now.</remarks>
        public object GetOpenPropertyValue(object target, string propertyName)
        {
            DSPResource entity = target as DSPResource;
            if (entity != null)
            {
                return entity.GetOpenPropertyValue(propertyName);
            }
            else
            {
                throw new NotSupportedException("Unrecognized resource type.");
            }
        }

        /// <summary>Gets a list of all open properties for the specified resource.</summary>
        /// <param name="target">The target resource to get open properties from.</param>
        /// <returns>Enumerable of pairs, where the Key is the name of the open property and the Value is the value if that property.</returns>
        /// <remarks>This method should only return open properties. Properties declared in metadata for this resource's resource type
        /// must not be returned by this method.
        /// Properties can have null values. The type of the property is determined from the returned object, only primitive types are supported for now.</remarks>
        public IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(object target)
        {
            DSPResource entity = target as DSPResource;
            if (entity != null)
            {
                return entity.GetOpenPropertyValues();
            }
            else
            {
                throw new NotSupportedException("Unrecognized resource type.");
            }
        }

        /// <summary>Gets a value of a declared property for a resource.</summary>
        /// <param name="target">The target resource to get a value of the property from.</param>
        /// <param name="resourceProperty">The name of the property to get.</param>
        /// <returns>The value of the property.</returns>
        /// <remarks>The returned value's type should match the type declared in the resource's resource type.</remarks>
        public object GetPropertyValue(object target, ResourceProperty resourceProperty)
        {
            DSPResource entity = target as DSPResource;
            if (entity != null)
            {
                return entity.GetValue(resourceProperty.Name);
            }
            else
            {
                throw new NotSupportedException("Unrecognized resource type.");
            }
        }

        /// <summary>Helper strongly typed method to create the query for a resource set root.</summary>
        /// <typeparam name="TElement">The instance type of a single resource in the set.</typeparam>
        /// <param name="resourceSet">The resource set to get the query root for.</param>
        /// <returns>Strongly typed query for the resource set root.</returns>
        private System.Linq.IQueryable GetTypedQueryRootForResourceSet<TElement>(ResourceSet resourceSet)
        {
            IQueryable source = DSPLinqQueryProvider.CreateQuery(this.dataSource.GetResourceSetEntities(resourceSet.Name).Cast<TElement>().AsQueryable());

            return WrapWithPagingProvider(source);
        }

        /// <summary>
        /// Wrap the DSPLinqQuery with a PagedQueryProvider
        /// </summary>
        /// <param name="source">The DSPLinqQuery Instance</param>
        /// <returns>A new queryable which supports paging</returns>
        private IQueryable WrapWithPagingProvider(IQueryable source)
        {
            if (customPagingEnabled)
            {
                IQueryProvider provider = new AstoriaUnitTests.Tests.PagedQueryProvider(
                    this,
                    source.Provider,
                    null,
                    null,
                    new AstoriaUnitTests.Tests.CountManager());
                return DSPLinqQueryProvider.CreateQuery(provider, source.Expression);
            }
            else
            {
                return source;
            }
        }

        /// <summary>Returns a query which can be used to retrive resource from the specified resource set.</summary>
        /// <param name="resourceSet">The resource set to get the quqeyr for.</param>
        /// <returns>An <see cref="IQueryable"/> which will be used to get resources from the specified resource set.</returns>
        /// <remarks>The data service will use the LINQ to build the actual query required for the resource set. It's up to this provider
        /// to return an <see cref="IQueryable"/> which can handle such queries. If the resource set is not recognized by the provider it should return null.</remarks>
        public System.Linq.IQueryable GetQueryRootForResourceSet(ResourceSet resourceSet)
        {
            MethodInfo mi = typeof(DSPResourceQueryProvider).GetMethod("GetTypedQueryRootForResourceSet", BindingFlags.NonPublic | BindingFlags.Instance);
            return (IQueryable)mi.MakeGenericMethod(resourceSet.ResourceType.InstanceType).Invoke(
                this,
                new object[] { resourceSet });
        }

        /// <summary>Returns a resource type for the specified resource.</summary>
        /// <param name="target">The target resource for which to determine its type.</param>
        /// <returns>The <see cref="ResourceType"/> of the specified resource.</returns>
        /// <remarks>The method should throw if the resource is not recognized. If it returns null the data service will throw instead.</remarks>
        public virtual ResourceType GetResourceType(object target)
        {
            Type targetType = target.GetType();

            if (target is DSPResource)
            {
                return ((DSPResource)target).ResourceType;
            }
            else
            {
                // This is rather slow (linear search of all resource types), to make this better have an index (dictionary) or resource types
                //   with the key being the instance type.
                return this.metadata.Types.FirstOrDefault(rt => rt.InstanceType == targetType);
            }
        }

        /// <summary>Invokes the specified service operation.</summary>
        /// <param name="serviceOperation">The service operation to invoke.</param>
        /// <param name="parameters">The parameters for the service operation. Their types will match the types of the declared parameters for the service operation.</param>
        /// <returns>The result of the service operation.
        /// If the service operation is declared to return void, this method should return null.
        /// Otherwise the method should return object of the type declared as the return type for the service operation.</returns>
        public object InvokeServiceOperation(ServiceOperation serviceOperation, object[] parameters)
        {
            Func<Object[], Object> operation;
            if( this.dataSource.ServiceOperations.TryGetValue(serviceOperation.Name, out operation))
            {
                object result = operation(parameters);
                if (result is IQueryable)
                {
                    // in order to further compose on top of this query, we need to turn this into a DSPLinqQuery
                    return WrapWithPagingProvider(DSPLinqQueryProvider.CreateQuery((IQueryable)result));
                }

                return result;
            }

            throw new InvalidOperationException("Specified service operation is not implemented by the data source");
        }

        /// <summary>Gets a value indicating whether null propagation is required in expression trees.</summary>
        /// <remarks>If this is set to true than the generated expression trees will contain conditionals to check for nulls when trying to access properties on objects.
        /// If this is set to false, no such checks will be present in the tree and the data service assumes that the query provider can handle
        /// accesses to properties on instances which are null (what happens in that case is up to the provider, but it is assumed that value of such property is also null).</remarks>
        public bool IsNullPropagationRequired
        {
            // Our provider requires null propagation because it relies on LINQ to Objects. LINQ to Objects simply compiles
            //   the expression into IL and executes it. So if there's an access to a member on instance which is null it will throw NullReferenceException
            //   So we need the null checks in the expression tree to avoid this situation.
            get { return true; }
        }

        #endregion
    }
}
