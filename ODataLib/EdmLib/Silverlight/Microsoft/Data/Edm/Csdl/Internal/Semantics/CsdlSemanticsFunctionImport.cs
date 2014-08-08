//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library.Expressions;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    internal class CsdlSemanticsFunctionImport : CsdlSemanticsFunctionBase, IEdmFunctionImport
    {
        private readonly CsdlFunctionImport functionImport;
        private readonly CsdlSemanticsEntityContainer container;

        private readonly Cache<CsdlSemanticsFunctionImport, IEdmExpression> entitySetCache = new Cache<CsdlSemanticsFunctionImport, IEdmExpression>();
        private static readonly Func<CsdlSemanticsFunctionImport, IEdmExpression> ComputeEntitySetFunc = (me) => me.ComputeEntitySet();

        public CsdlSemanticsFunctionImport(CsdlSemanticsEntityContainer container, CsdlFunctionImport functionImport)
            : base(container.Context, functionImport)
        {
            this.container = container;
            this.functionImport = functionImport;
        }

        public bool IsSideEffecting
        {
            get { return this.functionImport.SideEffecting; }
        }

        public bool IsComposable
        {
            get { return this.functionImport.Composable; }
        }

        public bool IsBindable
        {
            get { return this.functionImport.Bindable; }
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

        private IEdmExpression ComputeEntitySet()
        {
            if (this.functionImport.EntitySet != null)
            {
                IEdmEntitySet entitySet = this.container.FindEntitySet(this.functionImport.EntitySet) ??
                    new UnresolvedEntitySet(this.functionImport.EntitySet, this.Container, this.Location);
                return new FunctionImportEntitySetReferenceExpression(entitySet) { Location = this.Location };
            }

            if (this.functionImport.EntitySetPath != null)
            {
                return new FunctionImportPathExpression(this.functionImport.EntitySetPath) { Location = this.Location };
            }

            return null;
        }

        private sealed class FunctionImportEntitySetReferenceExpression : EdmEntitySetReferenceExpression, IEdmLocatable
        {
            internal FunctionImportEntitySetReferenceExpression(IEdmEntitySet referencedEntitySet)
                : base(referencedEntitySet)
            {
            }

            public EdmLocation Location
            {
                get;
                set;
            }
        }

        private sealed class FunctionImportPathExpression : EdmPathExpression, IEdmLocatable
        {
            internal FunctionImportPathExpression(string path)
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
