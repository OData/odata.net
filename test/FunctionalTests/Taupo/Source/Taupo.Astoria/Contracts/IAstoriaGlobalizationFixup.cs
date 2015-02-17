//---------------------------------------------------------------------
// <copyright file="IAstoriaGlobalizationFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Applies globalization transformation to the model.
    /// </summary>
    [ImplementationSelector("AstoriaGlobalizationFixup", HelpText = "Globalization fixup to be applied to all Astoria models")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Had to create separate empty interface")]
    public interface IAstoriaGlobalizationFixup : IGlobalizationFixup
    {
    }
}
