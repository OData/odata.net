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
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Library.Expressions;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlOperation
    /// </summary>
    internal abstract class CsdlSemanticsOperation : CsdlSemanticsElement, IEdmOperation
    {
        private readonly CsdlOperation operation;
        private readonly Cache<CsdlSemanticsOperation, IEdmPathExpression> entitySetPathCache = new Cache<CsdlSemanticsOperation, IEdmPathExpression>();
        private static readonly Func<CsdlSemanticsOperation, IEdmPathExpression> ComputeEntitySetPathFunc = (me) => me.ComputeEntitySetPath();

        private readonly Cache<CsdlSemanticsOperation, IEdmTypeReference> returnTypeCache = new Cache<CsdlSemanticsOperation, IEdmTypeReference>();
        private static readonly Func<CsdlSemanticsOperation, IEdmTypeReference> ComputeReturnTypeFunc = (me) => me.ComputeReturnType();

        private readonly Cache<CsdlSemanticsOperation, IEnumerable<IEdmOperationParameter>> parametersCache = new Cache<CsdlSemanticsOperation, IEnumerable<IEdmOperationParameter>>();
        private static readonly Func<CsdlSemanticsOperation, IEnumerable<IEdmOperationParameter>> ComputeParametersFunc = (me) => me.ComputeParameters();

        public CsdlSemanticsOperation(CsdlSemanticsSchema context, CsdlOperation operation)
            : base(operation)
        {
            this.Context = context;
            this.operation = operation;
        }

        public abstract EdmSchemaElementKind SchemaElementKind { get; }

        public override CsdlSemanticsModel Model
        {
            get { return this.Context.Model; }
        }

        public string Name
        {
            get { return this.operation.Name; }
        }

        public override CsdlElement Element
        {
            get { return this.operation; }
        }

        public string Namespace
        {
            get { return this.Context.Namespace; }
        }

        public bool IsBound
        {
            get { return this.operation.IsBound; }
        }

        public IEdmPathExpression EntitySetPath
        {
            get
            {
                return this.entitySetPathCache.GetValue(this, ComputeEntitySetPathFunc, null);
            }
        }

        public IEdmTypeReference ReturnType
        {
            get { return this.returnTypeCache.GetValue(this, ComputeReturnTypeFunc, null); }
        }

        public IEnumerable<IEdmOperationParameter> Parameters
        {
            get { return this.parametersCache.GetValue(this, ComputeParametersFunc, null); }
        }

        public CsdlSemanticsSchema Context { get; private set; }

        public IEdmOperationParameter FindParameter(string name)
        {
            return this.Parameters.SingleOrDefault(p => p.Name == name);
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.Context);
        }

        private IEdmPathExpression ComputeEntitySetPath()
        {
            if (this.operation.EntitySetPath != null)
            {
                return new OperationPathExpression(this.operation.EntitySetPath) { Location = this.Location };
            }

            return null;
        }

        private IEdmTypeReference ComputeReturnType()
        {
            return CsdlSemanticsModel.WrapTypeReference(this.Context, this.operation.ReturnType);
        }

        private IEnumerable<IEdmOperationParameter> ComputeParameters()
        {
            List<IEdmOperationParameter> parameters = new List<IEdmOperationParameter>();

            foreach (var parameter in this.operation.Parameters)
            {
                parameters.Add(new CsdlSemanticsOperationParameter(this, parameter));
            }

            return parameters;
        }

        private sealed class OperationPathExpression : EdmPathExpression, IEdmLocatable
        {
            internal OperationPathExpression(string path) : base(path)
            {
            }

            public EdmLocation Location { get; set; }
        }
    }
}
