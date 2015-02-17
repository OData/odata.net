//---------------------------------------------------------------------
// <copyright file="DataServiceEntitySetAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// This annotation helps the client code-gen in generating the [EntitySet] attribute required for types with DataServiceEntity attribute
    /// </summary>
    public class DataServiceEntitySetAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the DataServiceEntitySetAnnotation class
        /// </summary>
        public DataServiceEntitySetAnnotation()
            : this("EntitySet")
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataServiceEntitySetAnnotation class
        /// </summary>
        /// <param name="entitySetName">The name of the EntitySet</param>
        public DataServiceEntitySetAnnotation(string entitySetName)
            : base()
        {
            this.EntitySetName = entitySetName;
        }

        /// <summary>
        /// Gets or sets the name of the EntitySet
        /// </summary>
        public string EntitySetName { get; set; }
    }
}
