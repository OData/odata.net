//---------------------------------------------------------------------
// <copyright file="EntityModelSchemaDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;

    /// <summary>
    /// Reads an entity model schema and then converts OData-specific metadata annotations into their test annotation representation.
    /// </summary>
    public class EntityModelSchemaDeserializer
    {
        [InjectDependency(IsRequired = true)]
        public ICsdlParser BaseEntityModelSchemaDeserializer { get; set; }

        [InjectDependency(IsRequired = true)]
        public IAttributeToPropertyMappingAnnotationConverter ODataAnnotationConverter { get; set; }

        /// <summary>
        /// Parses the <paramref name="csdlContent"/> into an <see cref="EntityModelSchema"/> instance.
        /// </summary>
        /// <param name="csdlContent">The Xml representation of the CSDL to parse.</param>
        /// <returns>The <see cref="EntityModelSchema"/> corresponding to the <paramref name="csdlContent"/>.</returns>
        public EntityModelSchema Deserialize(XElement csdlContent)
        {
            // TODO: Can we inherit from the default implemenation for ICsdlParser instead of doing this in a separate step?
            EntityModelSchema schema = this.BaseEntityModelSchemaDeserializer.Parse(csdlContent);

            // TODO: the IAttributeToFeedMappingAnnotationConverter should really be about all the OData specific annotations (today only feed mapping is really interesting though)
            this.ODataAnnotationConverter.ConvertToTestAnnotations(schema);

            return schema;
        }
    }
}
