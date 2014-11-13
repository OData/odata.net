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
