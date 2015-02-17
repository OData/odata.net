//---------------------------------------------------------------------
// <copyright file="ThrowDataServiceExceptionAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Annotation that indicates the that something in the service should throw a DataServiceException with
    /// a particular status code and message
    /// </summary>
    public class ThrowDataServiceExceptionAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the ThrowDataServiceExceptionAnnotation class
        /// </summary>
        public ThrowDataServiceExceptionAnnotation()
        {
            this.ErrorStatusCode = 500;
            this.ErrorMessage = "Forced Test DataServiceException throw by DataServices";
        }

        /// <summary>
        /// Gets or sets a error status code
        /// </summary>
        public int? ErrorStatusCode { get; set; }

        /// <summary>
        /// Gets or sets a error status code
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}