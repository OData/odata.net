//---------------------------------------------------------------------
// <copyright file="EntityModelSchemaSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;

    /// <summary>
    /// Converts OData-specific metadata annotations on an entity model schema into their serializable representation 
    /// and then writes the entity model schema.
    /// </summary>
    public class EntityModelSchemaSerializer
    {
        [InjectDependency(IsRequired = true)]
        public ICsdlContentGenerator BaseEntityModelSchemaSerializer { get; set; }

        [InjectDependency(IsRequired = true)]
        public ITestToProductAnnotationConverter ODataAnnotationConverter { get; set; }

        /// <summary>
        /// Serializes the <paramref name="schema"/> in the specified <paramref name="csdlVersion"/> and returns it.
        /// </summary>
        /// <param name="csdlVersion">The CSDL version to use for serializing the entity model schema.</param>
        /// <param name="schema">The schema to serialize.</param>
        /// <returns>The serialized <paramref name="schema"/>.</returns>
        public IEnumerable<FileContents<XElement>> Serialize(EdmVersion csdlVersion, EntityModelSchema schema)
        {
            ExceptionUtilities.Assert(schema != null, "schema != null");

            // TODO: Can we inherit from the default implemenation of the CSDL content generator instead of doing this in a separate step?
            schema = this.ODataAnnotationConverter.ConvertToProductAnnotations(schema);
            return this.BaseEntityModelSchemaSerializer.Generate(csdlVersion, schema);
        }
    }
}
