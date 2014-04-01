//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.OData.Edm.Internal;

namespace Microsoft.OData.Edm.Csdl.Internal.CsdlSemantics
{
    internal class CsdlSemanticsFunctionBase : CsdlSemanticsElement, IEdmFunctionBase
    {
        private readonly CsdlSemanticsSchema context;
        private readonly CsdlFunctionBase operationBase;

        private readonly Cache<CsdlSemanticsFunctionBase, IEdmTypeReference> returnTypeCache = new Cache<CsdlSemanticsFunctionBase, IEdmTypeReference>();
        private static readonly Func<CsdlSemanticsFunctionBase, IEdmTypeReference> ComputeReturnTypeFunc = (me) => me.ComputeReturnType();

        private readonly Cache<CsdlSemanticsFunctionBase, IEnumerable<IEdmOperationParameter>> parametersCache = new Cache<CsdlSemanticsFunctionBase, IEnumerable<IEdmOperationParameter>>();
        private static readonly Func<CsdlSemanticsFunctionBase, IEnumerable<IEdmOperationParameter>> ComputeParametersFunc = (me) => me.ComputeParameters();

        public CsdlSemanticsFunctionBase(CsdlSemanticsSchema context, CsdlFunctionBase operationBase)
            : base(operationBase)
        {
            this.context = context;
            this.operationBase = operationBase;
        }

        public string Name
        {
            get { return this.operationBase.Name; }
        }

        public IEdmTypeReference ReturnType
        {
            get { return this.returnTypeCache.GetValue(this, ComputeReturnTypeFunc, null); }
        }

        public IEnumerable<IEdmOperationParameter> Parameters
        {
            get { return this.parametersCache.GetValue(this, ComputeParametersFunc, null); }
        }

        public CsdlSemanticsSchema Context
        {
            get { return this.context; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.Context.Model; }
        }

        public override CsdlElement Element
        {
            get { return this.operationBase; }
        }

        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.OperationImport; }
        }

        public IEdmOperationParameter FindParameter(string name)
        {
            return this.Parameters.SingleOrDefault(p => p.Name == name);
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.Context);
        }

        private IEdmTypeReference ComputeReturnType()
        {
            return CsdlSemanticsModel.WrapTypeReference(this.Context, this.operationBase.ReturnType);
        }

        private IEnumerable<IEdmOperationParameter> ComputeParameters()
        {
            List<IEdmOperationParameter> parameters = new List<IEdmOperationParameter>();

            foreach (var parameter in this.operationBase.Parameters)
            {
                parameters.Add(new CsdlSemanticsOperationParameter(this, parameter));
            }

            return parameters;
        }
    }
}
