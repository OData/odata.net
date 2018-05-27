//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsOperationImport.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal abstract class CsdlSemanticsOperationImport : CsdlSemanticsElement, IEdmOperationImport
    {
        private readonly CsdlOperationImport operationImport;
        private readonly CsdlSemanticsEntityContainer container;

        private readonly Cache<CsdlSemanticsOperationImport, IEdmExpression> entitySetCache = new Cache<CsdlSemanticsOperationImport, IEdmExpression>();
        private static readonly Func<CsdlSemanticsOperationImport, IEdmExpression> ComputeEntitySetFunc = (me) => me.ComputeEntitySet();

        protected CsdlSemanticsOperationImport(CsdlSemanticsEntityContainer container, CsdlOperationImport operationImport, IEdmOperation operation)
            : base(operationImport)
        {
            this.container = container;
            this.operationImport = operationImport;
            this.Operation = operation;
        }

        public IEdmOperation Operation { get; private set; }

        public override CsdlSemanticsModel Model
        {
            get { return this.container.Context.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.operationImport; }
        }

        public string Name
        {
            get { return this.operationImport.Name; }
        }

        public IEdmEntityContainer Container
        {
            get { return this.container; }
        }

        public IEdmExpression EntitySet
        {
            get
            {
                return this.entitySetCache.GetValue(this, ComputeEntitySetFunc, null);
            }
        }

        public abstract EdmContainerElementKind ContainerElementKind { get; }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.container.Context);
        }

        private IEdmExpression ComputeEntitySet()
        {
            if (this.operationImport.EntitySet != null)
            {
                return new OperationImportPathExpression(this.operationImport.EntitySet) { Location = this.Location };
            }

            return null;
        }

        private sealed class OperationImportPathExpression : EdmPathExpression, IEdmLocatable
        {
            internal OperationImportPathExpression(string path)
                : base(path)
            {
            }

            public EdmLocation Location
            {
                get;
                set;
            }
        }
    }
}
