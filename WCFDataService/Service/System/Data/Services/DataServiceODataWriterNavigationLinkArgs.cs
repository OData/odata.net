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
