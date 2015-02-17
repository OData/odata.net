//---------------------------------------------------------------------
// <copyright file="CsdlFunctionBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a base class for CSDL functions and operation imports.
    /// </summary>
    internal abstract class CsdlFunctionBase : CsdlNamedElement
    {
        private readonly List<CsdlOperationParameter> parameters;
        private readonly CsdlTypeReference returnType;

        protected CsdlFunctionBase(string name, IEnumerable<CsdlOperationParameter> parameters, CsdlTypeReference returnType, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, documentation, location)
        {
            this.parameters = new List<CsdlOperationParameter>(parameters);
            this.returnType = returnType;
        }

        public IEnumerable<CsdlOperationParameter> Parameters
        {
            get { return this.parameters; }
        }

        public CsdlTypeReference ReturnType
        {
            get { return this.returnType; }
        }
    }
}
