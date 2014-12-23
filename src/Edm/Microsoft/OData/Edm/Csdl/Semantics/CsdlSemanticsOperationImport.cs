//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Library.Expressions;

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
                // if its a target path then create as such otherwise its targeting an entityset.
                if (this.operationImport.EntitySet.IndexOf("/", StringComparison.Ordinal) > -1)
                {
                    return new OperationImportPathExpression(this.operationImport.EntitySet) { Location = this.Location };
                }
                else
                {
                    IEdmEntitySet entitySet = this.container.FindEntitySetExtended(this.operationImport.EntitySet) ?? new UnresolvedEntitySet(this.operationImport.EntitySet, this.Container, this.Location);
                    return new OperationImportEntitySetReferenceExpression(entitySet) { Location = this.Location };
                }
            }

            return null;
        }

        private sealed class OperationImportEntitySetReferenceExpression : EdmEntitySetReferenceExpression, IEdmLocatable
        {
            internal OperationImportEntitySetReferenceExpression(IEdmEntitySet referencedEntitySet)
                : base(referencedEntitySet)
            {
            }

            public EdmLocation Location
            {
                get;
                set;
            }
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
