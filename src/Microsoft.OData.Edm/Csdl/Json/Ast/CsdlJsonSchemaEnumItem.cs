//---------------------------------------------------------------------
// <copyright file="CsdlJsonSchemaEnumItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Json.Ast
{
    internal class CsdlJsonSchemaEnumItem : CsdlJsonSchemaStructuredItem
    {
        public string UnderlyingTypeName { get; set; }

        public override SchemaMemberKind Kind => SchemaMemberKind.Enum;

        public bool IsFlags { get; set; }
    }
}
