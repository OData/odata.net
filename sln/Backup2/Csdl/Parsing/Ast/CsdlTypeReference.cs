//---------------------------------------------------------------------
// <copyright file="CsdlTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Base type for the two kinds of type reference: <see cref="CsdlNamedTypeReference"/> and <see cref="CsdlExpressionTypeReference"/>.
    /// </summary>
    internal abstract class CsdlTypeReference : CsdlElement
    {
        private readonly bool isNullable;

        protected CsdlTypeReference(bool isNullable, CsdlLocation location)
            : base(location)
        {
            this.isNullable = isNullable;
        }

        public bool IsNullable
        {
            get { return this.isNullable; }
        }
    }
}
