//   OData .NET Libraries ver. 5.6.3
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
