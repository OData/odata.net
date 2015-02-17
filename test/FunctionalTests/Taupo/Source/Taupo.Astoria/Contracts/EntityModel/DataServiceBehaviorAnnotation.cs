//---------------------------------------------------------------------
// <copyright file="DataServiceBehaviorAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    
    /// <summary>
    /// Annotation that is used to set DataServiceBehaviorAnnotation
    /// </summary>
    public class DataServiceBehaviorAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceBehaviorAnnotation"/> class.
        /// </summary>
        public DataServiceBehaviorAnnotation()
        {
            // false by default so that the reference to Microsoft.Spatial is only added when necessary
            this.AcceptSpatialLiteralsInQuery = false;
        }

        /// <summary>
        /// Gets or sets an AcceptCountRequests
        /// </summary>
        public bool? AcceptCountRequests { get;  set; }

        /// <summary>
        /// Gets or sets an AcceptProjectionRequests
        /// </summary>
        public bool? AcceptProjectionRequests { get;  set; }

        /// <summary>
        /// Gets or sets an InvokeInterceptorsOnLinkDelete
        /// </summary>
        public bool? InvokeInterceptorsOnLinkDelete { get;  set; }

        /// <summary>
        /// Gets or sets the MaxProtocolVersion
        /// </summary>
        public DataServiceProtocolVersion MaxProtocolVersion { get;  set; }

        /// <summary>
        /// Gets or sets an IncludeAssociationLinksInResponse
        /// </summary>
        public bool? IncludeAssociationLinksInResponse { get; set; }

        /// <summary>
        /// Gets or sets an AcceptAnyAllRequests
        /// </summary>
        public bool? AcceptAnyAllRequests { get; set; }

        /// <summary>
        /// Gets or sets whether to accept spatial literals in query
        /// </summary>
        public bool? AcceptSpatialLiteralsInQuery { get; set; }
    }
}