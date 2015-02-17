//---------------------------------------------------------------------
// <copyright file="CollectionInstanceTypeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Annotation representing the instance type of a collection property
    /// </summary>
    public class CollectionInstanceTypeAnnotation : CollectionTypeAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the CollectionInstanceTypeAnnotation class
        /// </summary>
        public CollectionInstanceTypeAnnotation()
        {
            this.Usage = CodeGenerationTypeUsage.Instantiation;
        }
    }
}