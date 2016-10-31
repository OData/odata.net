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
    using System;

    /// <summary>
    /// Use this interface to modify the configuration of a web data service.
    /// </summary>
    public interface IDataServiceConfiguration
    {
        #region Properties

        /// <summary>Gets the maximum number of requests that can be handled in a batch.</summary>
        /// <returns>Integer value that indicates the maximum number of requests that can be handled in a batch.</returns>
        int MaxBatchCount
        {
            get;
            set;
        }

        /// <summary>Gets the maximum number of change sets that can be handled in a batch.</summary>
        /// <returns>Integer value that indicates the maximum number of change sets that can be handled in a batch.</returns>
        int MaxChangesetCount
        {
            get;
            set;
        }

        /// <summary>Gets or sets the maximum number of segments that can be expanded by the $expand query option for all requests to the data service.  </summary>
        /// <returns>The maximum number of segments to expand.</returns>
        int MaxExpandCount
        {
            get;
            set;
        }

        /// <summary>Gets or sets a maximum number of segments supported in a single $expand path for all requests to the data service.</summary>
        /// <returns>Integer representing the maximum number of supported segments in $expand path.</returns>
        int MaxExpandDepth
        {
            get;
            set;
        }

        /// <summary>Gets the maximum number of results per collection.</summary>
        /// <returns>The integer value that indicates the maximum number of results per collection.</returns>
        int MaxResultsPerCollection
        {
            get;
            set;
        }

        /// <summary>Gets or sets the maximum number of objects that can be inserted in a single request. </summary>
        /// <returns>The integer value that contains the maximum number of objects that can be inserted in a single request.</returns>
        int MaxObjectCountOnInsert
        {
            get;
            set;
        }

        /// <summary>Gets or sets whether verbose errors are used by default for all responses from the data service.  </summary>
        /// <returns>A Boolean value that indicates whether verbose errors are returned.</returns>
        /// <remarks>
        /// This property sets the default for the whole service; individual responses may behave differently
        /// depending on the value of the VerboseResponse property of the arguments to the HandleException
        /// method on the <see cref="DataService&lt;T&gt;"/> class.
        /// </remarks>
        bool UseVerboseErrors 
        { 
            get; 
            set;
        }

        /// <summary>Gets or sets whether the data model is validated before it is written as a response to a request to the metadata endpoint.</summary>
        /// <returns>true when metadata validation is disabled; otherwise false.</returns>
        bool DisableValidationOnMetadataWrite
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>Sets the access rules for the specified entity set.</summary>
        /// <param name="name">The name of the entity set for configured access.</param>
        /// <param name="rights">The rights allowed for the entity set.</param>
        void SetEntitySetAccessRule(string name, EntitySetRights rights);

        /// <summary>Sets the access rules for the specified service operation.</summary>
        /// <param name="name">The name of the service operation on which to set access rights.</param>
        /// <param name="rights">The rights allowed according to <see cref="T:System.Data.Services.ServiceOperationRights" /> enumeration. </param>
        void SetServiceOperationAccessRule(string name, ServiceOperationRights rights);

        /// <summary>Registers a resource type for use by the data service.</summary>
        /// <param name="type">The resource type to register.</param>
        void RegisterKnownType(Type type);

        #endregion Methods
    }
}
