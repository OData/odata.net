//---------------------------------------------------------------------
// <copyright file="IPayloadGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;

    /// <summary>
    /// PayloadGenerator contract. Implementations should generate payloads representing different sizes and shapes. 
    /// </summary>
    [ImplementationSelector("IPayloadGenerator", DefaultImplementation = "Default", HelpText = "Return OData Payloads to enumerate over")]
    public interface IPayloadGenerator
    {
        /// <summary>
        /// A simple Enumerator method for traversing over Atom payloads 
        /// </summary>
        /// <returns>An IEnumerable of payloads</returns>
        IEnumerable<EntityInstance> GenerateAtomPayloads();

        /// <summary>
        /// A simple Enumerator method for traversing over Json payloads 
        /// </summary>
        /// <returns>An IEnumerable of payloads</returns>
        IEnumerable<EntityInstance> GenerateJsonPayloads();

        /// <summary>
        /// Generates a set of interesting payloads for a given payload (i.e., puts the payload in all
        /// the interesting places in valid OData payloads where it can appear).
        /// </summary>
        /// <param name="payload">The payload to generate reader input payloads for.</param>
        /// <returns>A set of payloads that use the <paramref name="payload"/> in interesting places.</returns>
        IEnumerable<T> GeneratePayloads<T>(T payload) where T: PayloadTestDescriptor;
    }
}
