//   Copyright 2011 Microsoft Corporation
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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    internal class CsdlSemanticsFunctionImport : CsdlSemanticsFunctionBase, IEdmFunctionImport, IEdmCheckable
    {
        private readonly CsdlFunctionImport functionImport;
        private readonly CsdlSemanticsEntityContainer container;

        private readonly Cache<CsdlSemanticsFunctionImport, IEdmEntitySet> entitySetCache = new Cache<CsdlSemanticsFunctionImport, IEdmEntitySet>();
        private readonly static Func<CsdlSemanticsFunctionImport, IEdmEntitySet> s_computeEntitySet = (me) => me.ComputeEntitySet();

        public CsdlSemanticsFunctionImport(CsdlSemanticsEntityContainer container, CsdlFunctionImport functionImport)
            :base(container.Context, functionImport)
        {
            this.container = container;
            this.functionImport = functionImport;
        }

        public bool SideEffecting
        {
            get { return this.functionImport.SideEffecting; }
        }

        public bool Composable
        {
            get { return this.functionImport.Composable; }
        }

        public bool Bindable
        {
            get { return this.functionImport.Bindable; }
        }

        public string EntitySetPath
        {
            get { return this.functionImport.EntitySetPath; }
        }

        public IEdmEntitySet EntitySet
        { 
            get 
            {
                return this.entitySetCache.GetValue(this, s_computeEntitySet, null);
            } 
        }

        private IEdmEntitySet ComputeEntitySet()
        {
            if (this.functionImport.EntitySet == null)
            {
                return null;
            }

            return this.container.FindEntitySet(this.functionImport.EntitySet) ?? new UnresolvedEntitySet(this.functionImport.EntitySet, this.functionImport.Location);
        }

        public IEnumerable<EdmError> Errors
        {
            get
            {
                if (this.EntitySet is UnresolvedEntitySet)
                {
                    return this.EntitySet.Errors();
                }

                return Enumerable.Empty<EdmError>();
            }
        }
    }
}
