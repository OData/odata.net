//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsOperationDecimalReturnType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides the semantics operation return type as reference to an EDM Decimal type.
    /// </summary>
    internal class CsdlSemanticsOperationDecimalReturnType : CsdlSemanticsDecimalTypeReference, IEdmOperationReturnType
    {
        private readonly CsdlSemanticsOperation declaringOperation;

        public CsdlSemanticsOperationDecimalReturnType(CsdlSemanticsSchema schema, CsdlSemanticsOperation operation, CsdlDecimalTypeReference reference)
            : base(schema, reference)
        {
            declaringOperation = operation;
            Type = this;
        }

        public IEdmOperation DeclaringOperation { get { return declaringOperation; } }

        public IEdmTypeReference Type { get; }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.declaringOperation.Context);
        }
    }
}
