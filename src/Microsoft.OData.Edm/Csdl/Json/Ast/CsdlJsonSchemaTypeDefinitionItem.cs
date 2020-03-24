//---------------------------------------------------------------------
// <copyright file="CsdlJsonSchemaTypeDefinitionItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Json.Ast
{
    internal class CsdlJsonSchemaTypeDefinitionItem : CsdlJsonSchemaTypeItem
    {
        public string UnderlyingTypeName { get; set; }

        public override SchemaMemberKind Kind => SchemaMemberKind.TypeDefinition;

        public int? MaxLength { get; set; }

        public int? Precision { get; set; }

        public int? Scale { get; set; }

        public int? Srid { get; set; }

        public bool? Unicode { get; set; }
    }
}
