//---------------------------------------------------------------------
// <copyright file="IDataServiceProviderBehavior.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    using System;

    /// <summary>
    /// The kind of query processing behavior from the provider.
    /// </summary>
    public enum ProviderQueryBehaviorKind
    {
        /// <summary>
        /// Treat the provider query processing behavior similar to the reflection based provider.
        /// </summary>
        ReflectionProviderQueryBehavior,

        /// <summary>
        /// Treat the provider query processing behavior similar to the entity framework based provider.
        /// </summary>
        EntityFrameworkProviderQueryBehavior,

        /// <summary>
        /// Treat the provider query processing behavior as a custom provider.
        /// </summary>
        CustomProviderQueryBehavior
    }

    /// <summary>
    /// Used by the service writer to define the behavior of the providers.
    /// </summary>
    public interface IDataServiceProviderBehavior
    {
        /// <summary>
        /// Instance of provider behavior that defines the assumptions service should make
        /// about the provider.
        /// </summary>
        ProviderBehavior ProviderBehavior
        {
            get;
        }
    }

    /// <summary>
    /// Provider behavior encapsulates the runtime behavior of the provider. The service
    /// will check various properties the <see cref="ProviderBehavior"/> exposed by the <see cref="IDataServiceProviderBehavior"/> 
    /// to process the request.
    /// </summary>
    public class ProviderBehavior
    {
        /// <summary>
        /// Constructs a new instance of <see cref="ProviderBehavior"/>.
        /// </summary>
        /// <param name="queryBehaviorKind">Kind of query processing behavior for the provider.</param>
        public ProviderBehavior(ProviderQueryBehaviorKind queryBehaviorKind)
        {
            if (queryBehaviorKind != ProviderQueryBehaviorKind.CustomProviderQueryBehavior &&
                queryBehaviorKind != ProviderQueryBehaviorKind.ReflectionProviderQueryBehavior &&
                queryBehaviorKind != ProviderQueryBehaviorKind.EntityFrameworkProviderQueryBehavior)
            {
                throw new ArgumentOutOfRangeException("queryBehaviorKind");
            }

            this.ProviderQueryBehavior = queryBehaviorKind;
        }

        /// <summary>
        /// The kind of behavior service should assume from the provider.
        /// </summary>
        public ProviderQueryBehaviorKind ProviderQueryBehavior 
        { 
            get; 
            private set; 
        }
    }
}
