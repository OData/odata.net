//---------------------------------------------------------------------
// <copyright file="IPayloadTransformFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Generates instances of IPayloadTransform.
    /// </summary>
    [ImplementationSelector("IPayloadTransformFactory", DefaultImplementation = "Default", HelpText = "Generates payload transform instances.")]
    public interface IPayloadTransformFactory
    {
        /// <summary>
        /// Gets all the payload transforms of the specified type.
        /// </summary>
        /// <typeparam name="TPayload">Payload element object type.</typeparam>
        /// <returns>A composite instance of all payload transforms.</returns>
        IPayloadTransform<TPayload> GetTransform<TPayload>();

        /// <summary>
        /// Gets a scope that can be used to modify what transforms are returned by the factory in a way that does not corrupt the factory itself.
        /// </summary>
        /// <param name="empty">A value indicating whether the scope should start out empty. If false, the current set of default transformations will be used.</param>
        /// <returns>A scope which can be used to fluently add/remove transforms without modifying the factory's long-term state.</returns>
        IPayloadTransformationScope GetScope(bool empty);
    }
}
