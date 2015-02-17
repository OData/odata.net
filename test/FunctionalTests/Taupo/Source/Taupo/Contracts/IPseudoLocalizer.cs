//---------------------------------------------------------------------
// <copyright file="IPseudoLocalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Translates ASCII strings to Unicode characters by replacing letters with similar looking non-ASCII characters 
    /// (typically with diacritics or other alphabets)
    /// </summary>
    [ImplementationSelector("PseudoLocalizer", DefaultImplementation = "Default", HelpText = "Pseudo-localizes strings.")]
    public interface IPseudoLocalizer
    {
        /// <summary>
        /// Performs pseudo-localization of a given text.
        /// </summary>
        /// <param name="text">Text to be pseudo-localized.</param>
        /// <param name="isPropertyName">Indicates if the item is a property name</param>
        /// <returns>Pseudo-localized text.</returns>
        string PseudoLocalize(string text, bool isPropertyName);
    }
}
