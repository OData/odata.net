//---------------------------------------------------------------------
// <copyright file="ODataUrlValidationRules.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//--------

using Microsoft.OData.UriParser.Validation.Rules;

namespace Microsoft.OData.UriParser.Validation
{
    /// <summary>
    /// Built-in ODataUrlValidationRules
    /// </summary>
    public static class ODataUrlValidationRules
    {
        /// <summary>
        /// Rule to validate that no properties referenced by the Url are marked as deprecated.
        /// </summary>
        public static ODataUrlValidationRule DeprecatedPropertyRule = DeprecationRules.DeprecatedPropertyRule;

        /// <summary>
        /// Rule to validate that no types used within the URL are deprecated.
        /// </summary>
        public static ODataUrlValidationRule DeprecatedTypeRule = DeprecationRules.DeprecatedTypeRule;

        /// <summary>
        /// Rule to validate that no singletons or entity sets used within the Url are deprecated.
        /// </summary>
        public static ODataUrlValidationRule DeprecatedNavigationSourceRule = DeprecationRules.DeprecatedNavigationSourceRule;

        /// <summary>
        /// Rule to validate that structured properties within the Url include an explicit select statement.
        /// </summary>
        public static ODataUrlValidationRule RequireSelectRule = Rules.RequireSelectRules.RequireSelectRule;
    }
}