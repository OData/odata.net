//---------------------------------------------------------------------
// <copyright file="CsdlOperationReturnType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL operation return type.
    /// </summary>
    internal class CsdlOperationReturnType : CsdlElement
    {
        public CsdlOperationReturnType(CsdlTypeReference returnType, CsdlLocation location)
            : base(location)
        {
            this.ReturnType = returnType;
        }

        public CsdlTypeReference ReturnType { get; }
    }
}
