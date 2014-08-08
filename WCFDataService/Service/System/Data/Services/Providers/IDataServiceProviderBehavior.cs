//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services.Providers
{
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
