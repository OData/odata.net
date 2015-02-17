//---------------------------------------------------------------------
// <copyright file="IQueryClrType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;

    /// <summary>
    /// Query type with CLR type information.
    /// </summary>
    public interface IQueryClrType
    {
        /// <summary>
        /// Gets the CLR type information.
        /// </summary>
        Type ClrType { get; }
    }
}
