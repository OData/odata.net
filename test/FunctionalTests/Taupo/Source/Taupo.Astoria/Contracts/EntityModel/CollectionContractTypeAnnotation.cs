//---------------------------------------------------------------------
// <copyright file="CollectionContractTypeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Annotation to represent the contract type of a collection property
    /// </summary>
    public class CollectionContractTypeAnnotation : CollectionTypeAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the CollectionContractTypeAnnotation class
        /// </summary>
        public CollectionContractTypeAnnotation()
        {
            this.Usage = CodeGenerationTypeUsage.Declaration;
        }
    }
}