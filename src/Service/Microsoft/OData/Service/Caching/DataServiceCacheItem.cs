//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Caching
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Service.Providers;

    /// <summary>Use this class to cache data service information (configuration + metadata for builtin providers).</summary>
    internal class DataServiceCacheItem
    {
        #region Private fields

        /// <summary>Service configuration information.</summary>
        private readonly DataServiceConfiguration configuration;

        /// <summary>
        /// Service configuration information which is static such as query/change interceptors.
        /// </summary>
        private readonly DataServiceStaticConfiguration staticConfiguration;

        /// <summary>
        /// Keep track of the calculated visibility of resource types.
        /// </summary>
        private readonly Dictionary<string, ResourceType> visibleTypeCache;

        /// <summary>
        /// Maps resource set names to ResourceSetWrappers.
        /// </summary>
        private readonly Dictionary<string, ResourceSetWrapper> resourceSetWrapperCache;

        /// <summary>
        /// Maps names to ResourceAssociationSets.
        /// </summary>
        private readonly Dictionary<string, ResourceAssociationSet> resourceAssociationSetCache;

        #endregion Private fields

        /// <summary>Initializes a new <see cref="ProviderMetadataCacheItem"/> instance.</summary>
        /// <param name="dataServiceConfiguration">IDataServiceConfiguration instance containing all the configuration data.</param>
        /// <param name="staticConfiguration">Static configuration information which includes query/change interceptors.</param>
        internal DataServiceCacheItem(DataServiceConfiguration dataServiceConfiguration, DataServiceStaticConfiguration staticConfiguration)
        {
            this.configuration = dataServiceConfiguration;
            this.staticConfiguration = staticConfiguration;

            this.resourceSetWrapperCache = new Dictionary<string, ResourceSetWrapper>(EqualityComparer<string>.Default);
            this.visibleTypeCache = new Dictionary<string, ResourceType>(EqualityComparer<string>.Default);
            this.resourceAssociationSetCache = new Dictionary<string, ResourceAssociationSet>(EqualityComparer<string>.Default);
        }

        #region Properties

        /// <summary>Service configuration information.</summary>
        internal DataServiceConfiguration Configuration
        {
            [DebuggerStepThrough]
            get
            {
                return this.configuration;
            }
        }

        /// <summary>
        /// Static configuration information.
        /// </summary>
        internal DataServiceStaticConfiguration StaticConfiguration
        {
            [DebuggerStepThrough]
            get
            {
                return this.staticConfiguration;
            }
        }

        /// <summary>
        /// Keep track of the calculated visibility of resource types.
        /// </summary>
        internal Dictionary<string, ResourceType> VisibleTypeCache
        {
            [DebuggerStepThrough]
            get { return this.visibleTypeCache; }
        }

        /// <summary>
        /// Maps resource set names to ResourceSetWrappers.
        /// </summary>
        internal Dictionary<string, ResourceSetWrapper> ResourceSetWrapperCache
        {
            [DebuggerStepThrough]
            get { return this.resourceSetWrapperCache; }
        }

        /// <summary>
        /// Maps names to ResourceAssociationSets.
        /// </summary>
        internal Dictionary<string, ResourceAssociationSet> ResourceAssociationSetCache
        {
            [DebuggerStepThrough]
            get { return this.resourceAssociationSetCache; }
        }

        #endregion Properties
    }
}
