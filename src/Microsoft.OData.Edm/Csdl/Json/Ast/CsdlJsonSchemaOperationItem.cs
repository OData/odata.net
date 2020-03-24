//---------------------------------------------------------------------
// <copyright file="CsdlJsonSchemaOperationItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Json.Ast
{
    internal abstract class CsdlJsonSchemaOperationItem : CsdlJsonSchemaItem
    {
        public CsdlJsonSchemaReturnTypeItem ReturnType { get; set; }
    }

    internal abstract class CsdlJsonSchemaReturnTypeItem : CsdlJsonSchemaItem
    {

    }
}
