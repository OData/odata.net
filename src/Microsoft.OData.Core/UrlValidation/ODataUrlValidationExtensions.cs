//---------------------------------------------------------------------
// <copyright file="ODataUrlValidationExtension.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser.Validation.ValidationEngine;

namespace Microsoft.OData.UriParser.Validation
{
    /// <summary>
    /// Extension methods to add validation methods to an ODataUri
    /// </summary>
    public static class ODataUrlValidationExtensions
    {
        /// <summary>
        /// Validate the ODataUrl for a given model using all rules.
        /// </summary>
        /// <remarks>
        /// Note that Uris that validate using current rules may fail to validate in the future as additional rules are added.
        /// In order to guarantee a consistent behavior, pass an explicit set of rules to the Valdiate method.
        /// </remarks>
        /// <param name="odataUri">The <see cref="ODataUri"/> to validate.</param>
        /// <param name="model">The model to validate the OData Uri against.</param>
        /// <param name="errors">The collection of errors found during validation.</param>
        /// <returns>True if errors are discovered during validation, otherwise false.</returns>
        public static bool Validate(this ODataUri odataUri, IEdmModel model, out IEnumerable<ODataUrlValidationError> errors)
        {
            return odataUri.Validate(model, new ODataUrlValidationRuleSet(), out errors);
        }

        /// <summary>
        /// Validate the ODataUrl for a given model using a specified set of rules.
        /// </summary>
        /// <param name="odataUri">The <see cref="ODataUri"/> to validate.</param>
        /// <param name="model">The model to validate the OData Uri against.</param>
        /// <param name="rules">The set of rules to use in validating the OData Uri.</param>
        /// <param name="errors">The collection of errors found during validation.</param>
        /// <returns>True if errors are discovered during validation, otherwise false.</returns>
        public static bool Validate(this ODataUri odataUri, IEdmModel model, ODataUrlValidationRuleSet rules, out IEnumerable<ODataUrlValidationError> errors)
        {
            ODataUrlValidator validator = new ODataUrlValidator(model, rules);
            return validator.ValidateUrl(odataUri, out errors);
        }
    }
}
