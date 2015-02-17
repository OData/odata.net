//---------------------------------------------------------------------
// <copyright file="StoreFunctionBodyAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Annotation for additional metadata for function in storage model
    /// </summary>
    public class StoreFunctionBodyAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the StoreFunctionBodyAnnotation class.
        /// </summary>
        /// <param name="body">The body of the function (T-SQL)</param>
        public StoreFunctionBodyAnnotation(string body)
        {
            this.Body = body;
        }

        /// <summary>
        /// Gets or sets the Body
        /// </summary>
        public string Body { get; set; }
    }
}
