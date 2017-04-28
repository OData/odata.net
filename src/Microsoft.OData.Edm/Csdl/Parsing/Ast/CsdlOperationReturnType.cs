//---------------------------------------------------------------------
// <copyright file="CsdlOperationReturnType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL function return type.
    /// </summary>
    internal class CsdlOperationReturnType : CsdlElement, IEdmVocabularyAnnotatable
    {
        private readonly CsdlTypeReference returnType;

        public CsdlOperationReturnType(CsdlTypeReference returnType, CsdlLocation location)
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
