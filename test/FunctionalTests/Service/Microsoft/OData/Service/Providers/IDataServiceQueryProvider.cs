//---------------------------------------------------------------------
// <copyright file="IDataServiceQueryProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides a metadata and query source abstraction for a 
    /// web data service's store.
    /// </summary>
    public interface IDataServiceQueryProvider
    {
        /// <summary>The data source object from which data is provided.</summary>
        /// <returns>The data source.</returns>
        object CurrentDataSource
        {
            get;
            set;
        }

        /// <summary>Gets a value that indicates whether null propagation is required in expression trees.</summary>
        /// <returns>A <see cref="T:System.Boolean" /> value that indicates whether null propagation is required.</returns>
        bool IsNullPropagationRequired
        {
            get;
        }

        /// <summary>Gets the <see cref="T:System.Linq.IQueryable`1" /> that represents the container. </summary>
        /// <returns>An <see cref="T:System.Linq.IQueryable`1" /> that represents the resource set, or a null value if there is no resource set for the specified <paramref name="resourceSet" />.</returns>
        /// <param name="resourceSet">The resource set.</param>
        IQueryable GetQueryRootForResourceSet(ResourceSet resourceSet);

        /// <summary>Gets the resource type for the instance that is specified by the parameter.</summary>
        /// <returns>The <see cref="T:Microsoft.OData.Service.Providers.ResourceType" /> of the supplied object. </returns>
        /// <param name="target">Instance to extract a resource type from.</param>
        ResourceType GetResourceType(object target);

        /// <summary>Gets the value of the open property.</summary>
        /// <returns>Value for the property.</returns>
        /// <param name="target">Instance of the type that declares the open property.</param>
        /// <param name="resourceProperty">Value for the open property.</param>
        object GetPropertyValue(object target, ResourceProperty resourceProperty);

        /// <summary>Gets the value of the open property.</summary>
        /// <returns>The value of the open property.</returns>
        /// <param name="target">Instance of the type that declares the open property.</param>
        /// <param name="propertyName">Name of the open property.</param>
        object GetOpenPropertyValue(object target, string propertyName);

        /// <summary>Gets the name and values of all the properties that are defined in the given instance of an open type.</summary>
        /// <returns>A collection of name and values of all the open properties.</returns>
        /// <param name="target">Instance of the type that declares the open property.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "need to return a collection of key value pair")]
        IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(object target);

        /// <summary>Invokes the given service operation and returns the results.</summary>
        /// <returns>The result of the service operation, or a null value for a service operation that returns void.</returns>
        /// <param name="serviceOperation">Service operation to invoke.</param>
        /// <param name="parameters">Values of parameters to pass to the service operation.</param>
        object InvokeServiceOperation(ServiceOperation serviceOperation, object[] parameters);
    }
}
