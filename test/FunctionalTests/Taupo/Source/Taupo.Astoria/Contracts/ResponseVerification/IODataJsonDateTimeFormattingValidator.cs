//---------------------------------------------------------------------
// <copyright file="IODataJsonDateTimeFormattingValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification
{
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Component for verifying that datetime and datetimeoffset values are formatted correctly in JSON
    /// </summary>
    [ImplementationSelector("ODataJsonDateTimeFormattingValidator", DefaultImplementation = "Default")]
    public interface IODataJsonDateTimeFormattingValidator
    {
        /// <summary>
        /// Validates that any datetime or datetimeoffset values in the payload are formmatted correctly for the given version.
        /// </summary>
        /// <param name="payload">The payload.</param>
        void ValidateDateTimeFormatting(ODataPayloadElement payload);
    }
}
