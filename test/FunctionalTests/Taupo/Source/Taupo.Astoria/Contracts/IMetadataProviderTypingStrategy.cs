//---------------------------------------------------------------------
// <copyright file="IMetadataProviderTypingStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    
    /// <summary>
    /// Contract for configuring how an IDataServiceMetadataProvider implementation sets up it's types
    /// </summary>
    [ImplementationSelector("MetadataProviderTypingStrategy", DefaultImplementation = "Lazy")]
    public interface IMetadataProviderTypingStrategy
    {
        /// <summary>
        /// Returns an entity-model fixup that sets up the appropriate annotations for this strategy
        /// </summary>
        /// <returns>An entity model fixup</returns>
        IEntityModelFixup GetModelFixup();
    }
}