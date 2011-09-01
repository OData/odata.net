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
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    internal class CsdlSemanticsFunctionBase : CsdlSemanticsElement, IEdmFunctionBase
    {
        private readonly CsdlFunctionBase functionBase;
        private readonly CsdlSemanticsSchema context;

        private readonly Cache<CsdlSemanticsFunctionBase, IEdmTypeReference> returnTypeCache = new Cache<CsdlSemanticsFunctionBase, IEdmTypeReference>();
        private readonly static Func<CsdlSemanticsFunctionBase, IEdmTypeReference> s_computeReturnType = (me) => me.ComputeReturnType();

        private readonly Cache<CsdlSemanticsFunctionBase, IEnumerable<IEdmFunctionParameter>> parametersCache = new Cache<CsdlSemanticsFunctionBase, IEnumerable<IEdmFunctionParameter>>();
        private readonly static Func<CsdlSemanticsFunctionBase, IEnumerable<IEdmFunctionParameter>> s_computeParameters = (me) => me.ComputeParameters();

        public CsdlSemanticsFunctionBase(CsdlSemanticsSchema context, CsdlFunctionBase functionBase)
        {
            this.context = context;
            this.functionBase = functionBase;
        }

        public string Name
        {
            get { return this.functionBase.Name; }
        }

        public IEdmTypeReference ReturnType
        {
            get { return this.returnTypeCache.GetValue(this, s_computeReturnType, null); }
        }

        private IEdmTypeReference ComputeReturnType()
        {
            return CsdlSemanticsModel.WrapTypeReference(this.context, this.functionBase.ReturnType);
        }

        public IEnumerable<IEdmFunctionParameter> Parameters
        {
            get { return this.parametersCache.GetValue(this, s_computeParameters, null); }
        }

        private IEnumerable<IEdmFunctionParameter> ComputeParameters()
        {
            List<IEdmFunctionParameter> parameters = new List<IEdmFunctionParameter>();

            foreach (var parameter in this.functionBase.Parameters)
            {
                parameters.Add(new CsdlSemanticsFunctionParameter(this.context, parameter));
            }

            return parameters;
        }

        public IEdmFunctionParameter FindParameter(string name)
        {
            return this.Parameters.SingleOrDefault(p => p.Name == name);
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.context.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.functionBase; }
        }

        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.FunctionImport; }
        }

        protected override IEnumerable<IEdmAnnotation> ComputeImmutableAnnotations()
        {
            return this.Model.WrapAnnotations(this, this.context);
        }
    }
}
