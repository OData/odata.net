//---------------------------------------------------------------------
// <copyright file="CsdlCollectionType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL collection type.
    /// </summary>
    internal class CsdlCollectionType : CsdlElement, ICsdlTypeExpression
    {
        private readonly CsdlTypeReference elementType;

        public CsdlCollectionType(CsdlTypeReference elementType, CsdlLocation location)
            : base(location)
        {
            this.elementType = elementType;
        }

        public CsdlTypeReference ElementType
        {
            get { return this.elementType; }
        }
    }
}
