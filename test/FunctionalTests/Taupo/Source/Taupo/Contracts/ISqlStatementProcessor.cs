//---------------------------------------------------------------------
// <copyright file="ISqlStatementProcessor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;

    /// <summary>
    /// Processes generated SQL statements.
    /// </summary>
    public interface ISqlStatementProcessor : IDisposable
    {
        /// <summary>
        /// Processes given SQL statement.
        /// </summary>
        /// <param name="statementText">Statement text</param>
        void ProcessStatement(string statementText);
    }
}
