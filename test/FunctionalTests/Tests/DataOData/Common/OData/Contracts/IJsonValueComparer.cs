//---------------------------------------------------------------------
// <copyright file="IJsonValueComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;
    #endregion

    /// <summary>
    /// Contract for comparing two JsonValue OMs.
    /// </summary>
    [ImplementationSelector("JsonValueComparer", DefaultImplementation = "Default")]
    public interface IJsonValueComparer
    {
        /// <summary>
        /// Compares the two JsonValue OMs
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        void Compare(JsonValue expected, JsonValue actual);
    }
}
