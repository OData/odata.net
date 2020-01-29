//---------------------------------------------------------------------
// <copyright file="CsdlEntityReferenceType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL entity reference type.
    /// </summary>
    internal class CsdlEntityReferenceType : CsdlElement, ICsdlTypeExpression
    {
        private readonly CsdlTypeReference entityType;

        public CsdlEntityReferenceType(CsdlTypeReference entityType, CsdlLocation location)
            : base(location)
        {
            this.entityType = entityType;
        }

        public CsdlTypeReference EntityType
        {
            get { return this.entityType; }
        }
    }
}
