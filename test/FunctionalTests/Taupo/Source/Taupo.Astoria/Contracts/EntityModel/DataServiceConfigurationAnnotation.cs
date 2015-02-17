//---------------------------------------------------------------------
// <copyright file="DataServiceConfigurationAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    
    /// <summary>
    /// Annotation for configuring the DataService
    /// </summary>
    public class DataServiceConfigurationAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Gets or sets EnableTypeConversion
        /// </summary>
        public bool? EnableTypeConversion { get; set; }

        /// <summary>
        /// Gets or sets MaxBatchCount
        /// </summary>
        public int? MaxBatchCount { get; set; }

        /// <summary>
        /// Gets or sets the MaxChangesetCount
        /// </summary>
        public int? MaxChangesetCount { get; set; }

        /// <summary>
        /// Gets or sets MaxExpandCount
        /// </summary>
        public int? MaxExpandCount { get; set; }

        /// <summary>
        /// Gets or sets the MaxExpandDepth
        /// </summary>
        public int? MaxExpandDepth { get; set; }

        /// <summary>
        /// Gets or sets MaxObjectCountOnInsert
        /// </summary>
        public int? MaxObjectCountOnInsert { get; set; }

        /// <summary>
        /// Gets or sets MaxResultsPerCollection
        /// </summary>
        public int? MaxResultsPerCollection { get; set; }

        /// <summary>
        /// Gets or sets UseVerboseErrors
        /// </summary>
        public bool? UseVerboseErrors { get; set; }
    }
}