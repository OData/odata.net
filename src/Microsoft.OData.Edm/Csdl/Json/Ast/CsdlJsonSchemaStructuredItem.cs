//---------------------------------------------------------------------
// <copyright file="CsdlJsonSchemaStructuredItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Json.Ast
{
    internal abstract class CsdlJsonSchemaStructuredItem : CsdlJsonSchemaTypeItem
    {
        public string BaseType { get; set; }

        public bool IsAbstract { get; set; }

        public bool IsOpen { get; set; }
    }

}
