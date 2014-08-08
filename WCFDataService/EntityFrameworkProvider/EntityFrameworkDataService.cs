//   WCF Data Services Entity Framework Provider for OData ver. 1.0.0
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
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;

    /// <summary>
    /// Represents a strongly typed service that can process data-oriented 
    /// resource requests that use EntityFramework for building the data model.
    /// </summary>
    /// <typeparam name="T">The type of the store to provide resources.</typeparam>
    public abstract class EntityFrameworkDataService<T> : DataService<T>
    {
        /// <summary>
        /// Overrides the base CreateInternalProvider implementation, only supports EF6 here.
        /// </summary>
        /// <param name="dataSourceInstance">The datasource instance for the provider.</param>
        /// <returns>
        /// The internal provider to be created. 
        /// Note that this provider also need to implement <see cref="IDataServiceMetadataProvider"/> and <see cref="IDataServiceQueryProvider"/>
        /// </returns>
        protected override IDataServiceInternalProvider CreateInternalProvider(object dataSourceInstance)
        {
            return new EntityFrameworkDataServiceProvider2(this, dataSourceInstance);
        }
    }
}
