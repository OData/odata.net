//---------------------------------------------------------------------
// <copyright file="MethodCodeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Annotation that stores the generated code for a Function
    /// </summary>
    public class MethodCodeAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Gets or sets the code
        /// </summary>
        public string Code { get; set; }
    }
}
