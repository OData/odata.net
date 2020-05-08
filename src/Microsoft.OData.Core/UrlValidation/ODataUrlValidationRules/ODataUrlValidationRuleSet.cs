//---------------------------------------------------------------------
// <copyright file="ODataUrlValidationRuleSet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//--------

using System.Collections.Generic;

namespace Microsoft.OData.UriParser.Validation
{
    /// <summary>
    /// A collection of <see cref="ODataUrlValidationRule"s/>
    /// </summary>
    public class ODataUrlValidationRuleSet : List<ODataUrlValidationRule>
    {
        private static IEnumerable<ODataUrlValidationRule> allRules = new ODataUrlValidationRule[]
        {
            ODataUrlValidationRules.DeprecatedPropertyRule,
            ODataUrlValidationRules.DeprecatedTypeRule,
            ODataUrlValidationRules.DeprecatedNavigationSourceRule,
            ODataUrlValidationRules.RequireSelectRule
        };

        /// <summary>
        /// Constructs an ODataUrlValidationRuleSet with all built-in rules.
        /// </summary>
        /// <remarks>
        /// Note that Uris that validate using current rules may fail to validate in the future as additional rules are added.
        /// In order to guarantee a consistent behavior, pass an explicit set of rules to the constructor.
        /// </remarks>
        public ODataUrlValidationRuleSet() : base(allRules)
        {
        }

        /// <summary>
        /// Constructs an ODataUrlValidationRuleSet given a list of <see cref="ODataUrlValidationRule">s 
        /// </summary>
        /// <param name="rules">The rules to include in the ODataUrlValidationRuleSet.</param>
        public ODataUrlValidationRuleSet(IEnumerable<ODataUrlValidationRule> rules) : base(rules)
        {
        }
    }
}