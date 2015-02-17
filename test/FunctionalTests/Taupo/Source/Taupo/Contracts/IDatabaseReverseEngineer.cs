//---------------------------------------------------------------------
// <copyright file="IDatabaseReverseEngineer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Contracts.DatabaseModel;

    /// <summary>
    /// Handles reverse engineering of a database to a Taupo <see cref="DatabaseModel.DatabaseSchema"/> 
    /// </summary>
    public interface IDatabaseReverseEngineer
    {
        /// <summary>
        /// Perfoms reverse engineering of the database
        /// </summary>
        /// <returns>Taupo representation of the schema</returns>
        DatabaseSchema ReverseEngineer();
    }
}
