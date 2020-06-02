//---------------------------------------------------------------------
// <copyright file="ODataUrlValidationMessageCodes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser.Validation
{
    /// <summary>
    /// Message codes used in validating OData Urls.
    /// </summary>
    public static class ODataUrlValidationMessageCodes
    {
        /// <summary>
        /// An invalid rule was found when validating the OData Url.
        /// </summary>
        public const string InvalidRule = "invalidRule";

        /// <summary>
        /// Unable to Parse the OData Url.
        /// </summary>
        public const string UnableToParseUri = "unableToParseUrl";

        /// <summary>
        /// The OData Url referenced one or more elements marked as deprecated.
        /// </summary>
        public const string DeprecatedElement = "deprecated";

        /// <summary>
        /// The OData Url requests a top level or nested structured value that doesn't include a $select statement.
        /// </summary>
        public const string MissingSelect = "missingSelect";
    }
}
