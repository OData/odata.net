//---------------------------------------------------------------------
// <copyright file="IPayloadTransform`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Generic interface for all payload transforms.
    /// </summary>
    /// <typeparam name="TPayload">The payload type.</typeparam>
    public interface IPayloadTransform<TPayload>
    {
        /// <summary>
        /// Transforms the original payload. 
        /// </summary>
        /// <param name="originalPayload">Original payload.</param>
        /// <param name="transformedPayload">Transformed payload.</param>
        /// <returns>True if the transformation is applied else returns false.</returns>
        bool TryTransform(TPayload originalPayload, out TPayload transformedPayload);
    }
}
