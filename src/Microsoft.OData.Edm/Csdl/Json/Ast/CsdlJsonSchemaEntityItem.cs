//---------------------------------------------------------------------
// <copyright file="CsdlJsonSchemaEntityItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Json.Ast
{
    internal class CsdlJsonSchemaEntityItem : CsdlJsonSchemaStructuredItem
    {
        public bool HasStream { get; set; }

        public override SchemaMemberKind Kind => SchemaMemberKind.Entity;
    }
}
