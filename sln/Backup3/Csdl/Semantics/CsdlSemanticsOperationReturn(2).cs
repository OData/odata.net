//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsOperationReturn.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlOperationReturn.
    /// </summary>
    internal class CsdlSemanticsOperationReturn : CsdlSemanticsElement, IEdmOperationReturn
    {
        private readonly CsdlSemanticsOperation declaringOperation;
        private readonly CsdlOperationReturn operationReturn;

        private readonly Cache<CsdlSemanticsOperationReturn, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsOperationReturn, IEdmTypeReference>();
        private static readonly Func<CsdlSemanticsOperationReturn, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        public CsdlSemanticsOperationReturn(CsdlSemanticsOperation declaringOperation, CsdlOperationReturn operationReturn)
            : base(operationReturn)
        {
            this.declaringOperation = declaringOperation;
            this.operationReturn = operationReturn;
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.declaringOperation.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.operationReturn; }
        }

        public IEdmTypeReference Type
        {
            get { return this.typeCache.GetValue(this, ComputeTypeFunc, null); }
        }

        public IEdmOperation DeclaringOperation
        {
            get { return this.declaringOperation; }
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.declaringOperation.Context);
        }

        private IEdmTypeReference ComputeType()
        {
            return CsdlSemanticsModel.WrapTypeReference(this.declaringOperation.Context, this.operationReturn.ReturnType);
        }
    }
}
