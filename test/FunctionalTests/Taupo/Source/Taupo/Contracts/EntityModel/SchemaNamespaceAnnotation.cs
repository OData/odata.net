//---------------------------------------------------------------------
// <copyright file="SchemaNamespaceAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// SchemaNamespaceAnnotation carries any additional information that is in the schema element 
    /// when a EntityModel is parsed
    /// </summary>
    public class SchemaNamespaceAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Gets or sets the Edm Namespace version of the EntityModelSchema
        /// </summary>
        public string EdmNamespaceVersion { get; set; }

        /// <summary>
        /// Gets or sets the Schema Namespace of the EntityModelSchema
        /// </summary>
        public string SchemaNamespace { get; set; }
    }
}
