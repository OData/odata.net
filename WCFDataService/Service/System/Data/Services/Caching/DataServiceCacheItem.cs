//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Common;
    using System.Data.Services.Providers;
    using System.Diagnostics;

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
