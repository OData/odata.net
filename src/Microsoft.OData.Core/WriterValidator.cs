//---------------------------------------------------------------------
// <copyright file="WriterValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;

    /// <summary>
    /// Writer validator factory.
    /// </summary>
    internal static class WriterValidator
    {
        /// <summary>
        /// Creates a writer validator.
        /// </summary>
        /// <param name="settings">An ODataMessageWriterSettings instance that contains the
        /// validation settings to use for the created writer validator.</param>
        /// <returns>The created writer validator.</returns>
        internal static IWriterValidator Create(ODataMessageWriterSettings settings)
        {
            return settings.BasicValidation ? new WriterValidatorWithBasicValidation(settings) :
                                              new WriterValidatorWithoutBasicValidation(settings);
        }
    }
}
