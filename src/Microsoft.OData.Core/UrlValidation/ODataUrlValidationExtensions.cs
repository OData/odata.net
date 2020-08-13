//---------------------------------------------------------------------
// <copyright file="ODataUrlValidationExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser.Validation
{
    /// <summary>
    /// Extension methods to add validation methods to an OData Url
    /// </summary>
    public static class ODataUrlValidationExtensions
    {
        /// <summary>
        /// Validate a Uri as an OData Url for a given model using a specified set of rules.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> to validate.</param>
        /// <param name="model">The model to validate the Uri against.</param>
        /// <param name="rules">The set of rules to use in validating the OData Uri.</param>
        /// <param name="validationMessages">The collection of validation messages found during validation.</param>
        /// <returns>True if validation messages are discovered during validation, otherwise false.</returns>
        public static bool ValidateODataUrl(this Uri uri, IEdmModel model, ODataUrlValidationRuleSet rules, out IEnumerable<ODataUrlValidationMessage> validationMessages)
        {
            try
            {
                ODataUriParser parser = new ODataUriParser(model, uri);
                return parser.Validate(rules, out validationMessages);
            }
            catch (ODataException e)
            {
                validationMessages = new ODataUrlValidationMessage[]
                {
                    new ODataUrlValidationMessage(ODataUrlValidationMessageCodes.UnableToParseUri, e.Message, Severity.Error)
                };

                return false;
            }
        }
    }
}
