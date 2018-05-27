//---------------------------------------------------------------------
// <copyright file="CsdlExpressionBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Common base type for a CSDL expression.
    /// </summary>
    internal abstract class CsdlExpressionBase : CsdlElement
    {
        public CsdlExpressionBase(CsdlLocation location)
            : base(location)
        {
        }

        public abstract EdmExpressionKind ExpressionKind { get; }
    }
}
