//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
