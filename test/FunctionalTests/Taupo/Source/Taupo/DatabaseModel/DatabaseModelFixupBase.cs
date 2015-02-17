//---------------------------------------------------------------------
// <copyright file="DatabaseModelFixupBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DatabaseModel
{
    using Microsoft.Test.Taupo.Contracts.DatabaseModel;

    /// <summary>
    /// Base class for fixup classes working on <see cref="DatabaseSchema"/>
    /// </summary>
    public abstract class DatabaseModelFixupBase
    {
        /// <summary>
        /// Performs the fixup.
        /// </summary>
        /// <param name="schema">Database schema</param>
        public abstract void Fixup(DatabaseSchema schema);
    }
}
