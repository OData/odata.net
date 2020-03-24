//---------------------------------------------------------------------
// <copyright file="CsdlJsonSchema.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Json.Value;

namespace Microsoft.OData.Edm.Csdl.Json.Ast
{
    internal class CsdlJsonSchema
    {
        private IList<CsdlJsonSchemaItem> _schemaItems = new List<CsdlJsonSchemaItem>();

        public CsdlJsonSchema(string schemaNamespace)
        {
            Namespace = schemaNamespace;
        }

        public string Namespace { get; set; }

        public string Alias { get; set; }

        public IJsonValue OutOfLineAnnotations { get; set; }

        public IEnumerable<CsdlJsonSchemaItem> SchemaItems
        {
            get
            {
                return _schemaItems;
            }
        }


        public IEnumerable<CsdlJsonSchemaOperationItem> Operations
        {
            get
            {
                return _schemaItems.OfType<CsdlJsonSchemaOperationItem>();
            }
        }

        public IEnumerable<CsdlJsonSchemaEntityContainerItem> EntityContainers
        {
            get
            {
                return _schemaItems.OfType<CsdlJsonSchemaEntityContainerItem>();
            }
        }

        public IEnumerable<CsdlJsonSchemaStructuredItem> StructuredTypes
        {
            get
            {
                return _schemaItems.OfType<CsdlJsonSchemaStructuredItem>();
            }
        }

        public IEnumerable<CsdlJsonSchemaEnumItem> EnumTypes
        {
            get
            {
                return _schemaItems.OfType<CsdlJsonSchemaEnumItem>();
            }
        }

        public IEnumerable<CsdlJsonSchemaTypeDefinitionItem> TypeDefinitions
        {
            get
            {
                return _schemaItems.OfType<CsdlJsonSchemaTypeDefinitionItem>();
            }
        }

        public IEnumerable<CsdlJsonSchemaTermItem> Terms
        {
            get
            {
                return _schemaItems.OfType<CsdlJsonSchemaTermItem>();
            }
        }

        public void Add(CsdlJsonSchemaItem item)
        {
            _schemaItems.Add(item);
        }
    }
}
