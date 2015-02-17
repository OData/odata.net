//---------------------------------------------------------------------
// <copyright file="IDatabaseFunctionBodyResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    /// <summary>
    /// Resolves database function's body: if body is not specified generates valid functions's body and adds annotations if needed
    /// so that function can be successfuly created in a database.
    /// </summary>
    public interface IDatabaseFunctionBodyResolver
    {
        /// <summary>
        /// Resolves database function's body: if body is not specified generates valid body and adds annotations if needed.
        /// </summary>
        /// <param name="databaseFunction">The store function to resolve body for.</param>
        void ResolveFunctionBody(DatabaseFunction databaseFunction);

        /// <summary>
        /// Resolves stored procedure function's body: if body is not specified generates valid body and adds annotations if needed.
        /// </summary>
        /// <param name="storedProcedure">The store function to resolve body for.</param>
        void ResolveStoredProcedureBody(StoredProcedure storedProcedure);
    }
}
