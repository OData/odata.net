//---------------------------------------------------------------------
// <copyright file="CsdlOperationReturn.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL function return type.
    /// </summary>
    internal class CsdlOperationReturn : CsdlElement
    {
        private readonly CsdlTypeReference returnType;

        public CsdlOperationReturn(CsdlTypeReference returnType, CsdlLocation location)
            : base(location)
        {
            this.returnType = returnType;
        }

        public CsdlTypeReference ReturnType
        {
            get { return this.returnType; }
        }
    }
}
