//---------------------------------------------------------------------
// <copyright file="ConstantInterceptorAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;

    /// <summary>
    /// Decorates an EntitySet with an annotation that indicates the ServiceBuilding to add an Interceptor to a set to allow all results
    /// </summary>
    public class ConstantInterceptorAnnotation : DataServiceMemberGeneratorAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the ConstantInterceptorAnnotation class.
        /// </summary>
        public ConstantInterceptorAnnotation()
        {
            this.FilterConstant = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the FilterConstant is true or false on how to filter the results
        /// </summary>
        public bool FilterConstant { get; set; }
    }
}