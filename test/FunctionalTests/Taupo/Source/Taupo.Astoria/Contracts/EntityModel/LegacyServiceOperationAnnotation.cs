//---------------------------------------------------------------------
// <copyright file="LegacyServiceOperationAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Decorates a Function as a service operation
    /// </summary>
    public class LegacyServiceOperationAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Gets or sets the HTTP method that the operation responds to
        /// </summary>
        public HttpVerb Method { get; set; }

        /// <summary>
        /// Gets or sets the return type qualifier
        /// </summary>
        public ServiceOperationReturnTypeQualifier ReturnTypeQualifier { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to generate a SingleResult attribute
        /// </summary>
        public bool SingleResult { get; set; }
    }
}
