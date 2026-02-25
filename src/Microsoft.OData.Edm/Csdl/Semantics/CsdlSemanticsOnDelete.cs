//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsOnDelete.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class CsdlSemanticsOnDelete : CsdlSemanticsElement, IEdmOnDelete
    {
        protected readonly CsdlSemanticsModel model;
        protected readonly CsdlSemanticsSchema schema;
        protected CsdlOnDelete onDelete;

        public CsdlSemanticsOnDelete(CsdlSemanticsModel model, CsdlSemanticsSchema schema, CsdlOnDelete onDelete)
            : base(onDelete)
        {
            this.model = model;
            this.schema = schema;
            this.onDelete = onDelete;
        }

        public override CsdlSemanticsModel Model => this.model;

        public override CsdlElement Element => onDelete;

        public EdmOnDeleteAction Action => onDelete.Action;

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, schema);
        }
    }
}
