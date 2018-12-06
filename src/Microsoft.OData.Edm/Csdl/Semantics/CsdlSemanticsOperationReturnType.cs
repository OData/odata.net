//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsOperationReturnType.cs" company="Microsoft">
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
    /// Provides semantics for a CsdlOperationReturnType.
    /// </summary>
    internal class CsdlSemanticsOperationReturnType : CsdlSemanticsElement, IEdmOperationReturnType
    {
        private readonly CsdlSemanticsOperation declaringOperation;
        private readonly CsdlOperationReturnType returnType;

        private readonly Cache<CsdlSemanticsOperationReturnType, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsOperationReturnType, IEdmTypeReference>();
        private static readonly Func<CsdlSemanticsOperationReturnType, IEdmTypeReference> ComputeTypeFunc = (me) => me.ComputeType();

        public CsdlSemanticsOperationReturnType(CsdlSemanticsOperation declaringOperation, CsdlOperationReturnType returnType)
            : base(returnType)
        {
            this.returnType = returnType;
            this.declaringOperation = declaringOperation;
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.declaringOperation.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.returnType; }
        }

        public IEdmTypeReference Type
        {
            get { return this.typeCache.GetValue(this, ComputeTypeFunc, null); }
        }

        public IEdmOperation DeclaringOperation
        {
            get { return this.declaringOperation; }
        }

        public bool IsNullable
        {
            get { return Type.IsNullable; }
        }

        public IEdmType Definition
        {
            get { return Type.Definition; }
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.declaringOperation.Context);
        }

        private IEdmTypeReference ComputeType()
        {
            return CsdlSemanticsModel.WrapTypeReference(this.declaringOperation.Context, this.returnType.ReturnType);
        }
    }
}
