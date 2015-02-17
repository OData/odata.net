//---------------------------------------------------------------------
// <copyright file="PrimitiveClrTypeFacet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Facet that indicates primitive CLR type.
    /// </summary>
    public class PrimitiveClrTypeFacet : PrimitiveDataTypeFacet<Type>
    {
        /// <summary>
        /// Initializes a new instance of the PrimitiveClrTypeFacet class.
        /// </summary>
        /// <param name="clrType">The CLR type.</param>
        public PrimitiveClrTypeFacet(Type clrType)
            : base(clrType)
        {
            ExceptionUtilities.CheckArgumentNotNull(clrType, "clrType");
        }
    }
}
