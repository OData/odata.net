//---------------------------------------------------------------------
// <copyright file="IGlobalizationFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Applies globalization transformation to the model.
    /// </summary>
    [ImplementationSelector("GlobalizationFixup", HelpText = "Globalization fixup to be applied to all models")]
    public interface IGlobalizationFixup
    {
        /// <summary>
        /// Applies globalization transformation to the specified entity model.
        /// </summary>
        /// <param name="entityModel">The entity model.</param>
        void Fixup(EntityModelSchema entityModel);
    }
}
