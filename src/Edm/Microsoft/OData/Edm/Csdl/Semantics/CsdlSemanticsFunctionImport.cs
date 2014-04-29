//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class CsdlSemanticsFunctionImport : CsdlSemanticsOperationImport, IEdmFunctionImport
    {
        private readonly CsdlFunctionImport functionImport;
        private readonly CsdlSemanticsSchema csdlSchema;

        public CsdlSemanticsFunctionImport(CsdlSemanticsEntityContainer container, CsdlFunctionImport functionImport, IEdmFunction backingfunction)
            : base(container, functionImport, backingfunction)
        {
            this.csdlSchema = container.Context;
            this.functionImport = functionImport;
        }

        public IEdmFunction Function
        {
            get { return (IEdmFunction)this.Operation; }
        }

        public bool IncludeInServiceDocument
        {
            get { return this.functionImport.IncludeInServiceDocument; }
        }

        public override EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.FunctionImport; }
        }
    }
}
