//---------------------------------------------------------------------
// <copyright file="CsdlJsonSchemaTermItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Json.Ast
{
    internal class CsdlJsonSchemaTermItem : CsdlJsonSchemaItem
    {
        public string QualifiedTypeName { get; set; }

        public bool IsCollection { get; set; }

        public string DefaultValue { get; set; }

        public string AppliesTo { get; set; }

        public bool Nulable { get; set; }

        public int? MaxLength { get; set; }

        public int? Precision { get; set; }

        public int? Scale { get; set; }

        public int? Srid { get; set; }

        // 4.01 and greater payloads
        public bool? Unicode { get; set; }


        public override SchemaMemberKind Kind => SchemaMemberKind.OutOfLineAnnotations;
    }
}
