//---------------------------------------------------------------------
// <copyright file="IDatabaseSchemaSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Contracts.DatabaseModel;

    /// <summary>
    /// Serializes an in memory DatabaseSchema object graph
    /// </summary>
    public interface IDatabaseSchemaSerializer
    {
        /// <summary>
        /// Serializes an in memory <see cref="DatabaseModel.DatabaseSchema"/> object graph to a TextWriter
        /// </summary>
        /// <param name="schema">The schema to serialize</param>
        void Serialize(DatabaseSchema schema);
    }
}
