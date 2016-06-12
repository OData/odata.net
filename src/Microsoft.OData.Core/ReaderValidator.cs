//---------------------------------------------------------------------
// <copyright file="ReaderValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;

    /// <summary>
    /// Reader validator factory.
    /// </summary>
    internal static class ReaderValidator
    {
        /// <summary>
        /// Creates a reader validator.
        /// </summary>
        /// <param name="settings">An ODataMessageWriterSettings instance that contains the
        /// validation settings to use for the created reader validator.</param>
        /// <returns>The created reader validator.</returns>
        internal static IReaderValidator Create(ODataMessageReaderSettings settings)
        {
            return ((settings.Validations & ReaderValidations.BasicValidation) != 0) ?
                new ReaderValidatorWithBasicValidation(settings) :
                new ReaderValidatorWithoutBasicValidation(settings);
        }
    }
}
