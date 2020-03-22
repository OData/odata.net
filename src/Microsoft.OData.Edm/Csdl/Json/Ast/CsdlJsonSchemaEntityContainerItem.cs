//---------------------------------------------------------------------
// <copyright file="CsdlJsonSchemaEntityContainerItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Json.Ast
{
    internal class CsdlJsonSchemaEntityContainerItem : CsdlJsonSchemaItem
    {
        public string Extends { get; set; }

        public override SchemaMemberKind Kind => SchemaMemberKind.EntityContainer;
    }
}
