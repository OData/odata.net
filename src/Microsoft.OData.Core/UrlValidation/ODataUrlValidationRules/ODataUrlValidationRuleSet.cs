//---------------------------------------------------------------------
// <copyright file="ODataUrlValidationRuleSet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//--------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData.UriParser.Validation
{
    /// <summary>
    /// A collection of <see cref="ODataUrlValidationRule" />
    /// </summary>
    [SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "Set is more meaning than collection.")]
    public sealed class ODataUrlValidationRuleSet : List<ODataUrlValidationRule>
    {
        /// <summary>
        /// An ODataUrlValidationRuleSet with all built-in rules.
        /// </summary>
        /// <remarks>
        /// Note that Uris that validate using AllRules may fail to validate in the future as additional rules are added.
        /// In order to guarantee a consistent behavior, pass an explicit set of rules to the <see cref="ODataUrlValidationRuleSet"/> constructor.
        /// </remarks>
        public static ODataUrlValidationRuleSet AllRules = new ODataUrlValidationRuleSet(new ODataUrlValidationRule[]
        {
            ODataUrlValidationRules.DeprecatedPropertyRule,
            ODataUrlValidationRules.DeprecatedTypeRule,
            ODataUrlValidationRules.DeprecatedNavigationSourceRule,
            ODataUrlValidationRules.RequireSelectRule
        });

        /// <summary>
        /// Constructs an ODataUrlValidationRuleSet given a list of <see cref="ODataUrlValidationRule" />.
        /// </summary>
        /// <param name="rules">The rules to include in the ODataUrlValidationRuleSet.</param>
        public ODataUrlValidationRuleSet(IEnumerable<ODataUrlValidationRule> rules) : base(rules)
        {
        }
    }
}