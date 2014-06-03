//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
