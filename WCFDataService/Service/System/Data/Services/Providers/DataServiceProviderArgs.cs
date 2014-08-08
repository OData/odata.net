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
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    #endregion

    /// <summary>
    /// Argument class whose instance is passed for constructing a ReflectionDataServiceProvider
    /// or EntityFrameworkDataServiceProvider instance.
    /// </summary>
    public class DataServiceProviderArgs
    {
        /// <summary>
        /// Creates an instance of <see cref="DataServiceProviderArgs"/>.
        /// </summary>
        /// <param name="dataServiceInstance">Required instance of data service object.</param>
        /// <param name="dataSourceInstance">Required instance of data source object.</param>
        /// <param name="knownTypes">Optional collection of known types.</param>
        /// <param name="useMetadataKeyOrder">Whether metadata key order is to be used instead of default service defined key order.</param>
        public DataServiceProviderArgs(object dataServiceInstance, object dataSourceInstance, IEnumerable<Type> knownTypes, bool useMetadataKeyOrder)
        {
            WebUtil.CheckArgumentNull(dataServiceInstance, "dataServiceInstance");
            WebUtil.CheckArgumentNull(dataSourceInstance, "dataSourceInstance");

            this.DataServiceInstance = dataServiceInstance;
            this.DataSourceInstance = dataSourceInstance;

            if (knownTypes != null)
            {
                this.KnownTypes = new ReadOnlyCollection<Type>(new List<Type>(knownTypes));
            }
            else
            {
                this.KnownTypes = Enumerable.Empty<Type>();
            }

            this.UseMetadataKeyOrder = useMetadataKeyOrder;
            this.SkipServiceOperationMetadata = false;
        }

        /// <summary>
        /// Instance of the data service.
        /// </summary>
        public object DataServiceInstance
        {
            get;
            private set;
        }

        /// <summary>
        /// Instance of the data source.
        /// </summary>
        public object DataSourceInstance
        {
            get;
            private set;
        }

        /// <summary>
        /// Collection of known types.
        /// </summary>
        public IEnumerable<Type> KnownTypes
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether metadata defined ordering is to be used instead of service defined ordering.
        /// </summary>
        public bool UseMetadataKeyOrder
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether to load service operations on the service. Set to <c>false</c> by default.
        /// </summary>
        public bool SkipServiceOperationMetadata
        {
            get;
            set;
        }
    }
}
