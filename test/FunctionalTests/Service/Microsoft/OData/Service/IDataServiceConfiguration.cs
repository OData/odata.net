//---------------------------------------------------------------------
// <copyright file="IDataServiceConfiguration.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
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
        /// <param name="rights">The rights allowed according to <see cref="T:Microsoft.OData.Service.ServiceOperationRights" /> enumeration. </param>
        void SetServiceOperationAccessRule(string name, ServiceOperationRights rights);

        /// <summary>Registers a resource type for use by the data service.</summary>
        /// <param name="type">The resource type to register.</param>
        void RegisterKnownType(Type type);

        #endregion Methods
    }
}
