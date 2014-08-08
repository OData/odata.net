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

namespace System.Data.Services
{
    using System.Diagnostics;
    using Microsoft.Data.OData;

    /// <summary>
    /// Class that keeps track of the ODataNavigationLink and other information
    /// that we need to provide to the service author when they choose to provide their own
    /// instance of ODataWriter.
    /// </summary>
    public sealed class DataServiceODataWriterNavigationLinkArgs
    {
        /// <summary>
        /// Creates a new instance of DataServiceODataWriterNavigationLinkArgs.
        /// </summary>
        /// <param name="navigationLink">Instance of ODataNavigationLink.</param>
        /// <param name="operationContext">Instance of DataServiceOperationContext.</param>
        public DataServiceODataWriterNavigationLinkArgs(
            ODataNavigationLink navigationLink,
            DataServiceOperationContext operationContext)
        {
            WebUtil.CheckArgumentNull(navigationLink, "navigationLink != null");
            Debug.Assert(operationContext != null, "navigationLink != null");
            this.NavigationLink = navigationLink;
            this.OperationContext = operationContext;
        }

        /// <summary>
        /// Gets the ODataNavigationLink instance that is going to be serialized.
        /// </summary>
        public ODataNavigationLink NavigationLink
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the instance of DataServiceOperationContext.
        /// </summary>
        public DataServiceOperationContext OperationContext
        {
            get;
            private set;
        }
    }
}
