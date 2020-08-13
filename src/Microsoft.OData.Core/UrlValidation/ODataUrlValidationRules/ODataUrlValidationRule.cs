//---------------------------------------------------------------------
// <copyright file="ODataUrlValidationRule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//--------

using System;

namespace Microsoft.OData.UriParser.Validation
{
    /// <summary>
    /// Base class for OData URL Validation Rules
    /// </summary>
    public abstract class ODataUrlValidationRule
    {
        /// <summary>
        /// True if rule should include implicitly selected properties (i.e., in the absence of a $select)
        /// </summary>
        public bool IncludeImpliedProperties { get; set; }

        /// <summary>
        /// Name of the rule.
        /// </summary>
        public string RuleName { get; protected set; }

        /// <summary>
        /// Internal API called by the validation engine to validate the item of the specified type
        /// </summary>
        /// <param name="context">The validatation context.</param>
        /// <param name="component">The component.</param>
        internal abstract void Validate(ODataUrlValidationContext context, object component);

        /// <summary>
        /// Internal API called to get the type of Url element that the rule targets
        /// </summary>
        /// <returns>The type of the Url element that the rule targets</returns>
        internal abstract Type GetRuleType();
    }

    /// <summary>
    /// Rule for validating all elements of an ODataUrl of a particular type.
    /// </summary>
    /// <typeparam name="T">The type of OData Url element that the rule validates.</typeparam>
    public sealed class ODataUrlValidationRule<T> : ODataUrlValidationRule where T : class
    {
        /// <summary>
        /// The action that validates an instance of the specified type.
        /// </summary>
        private Action<ODataUrlValidationContext, T> validateMethod;

        /// <summary>
        /// The type of the Url element that the rule targets
        /// </summary>
        private Type ruleType = typeof(T);

        /// <summary>
        /// Constructs an instance of an ODataValdiationRule of a particular type.
        /// </summary>
        /// <param name="ruleName">The rule name.</param>
        /// <param name="validateMethod">The Action that validates an instance of the specified type.</param>
        public ODataUrlValidationRule(string ruleName, Action<ODataUrlValidationContext, T> validateMethod) :
            this (ruleName, validateMethod, /*includeImpliedProperties*/ false)
        {
        }

        /// <summary>
        /// Constructs an instance of an ODataValdiationRule of a particular type.
        /// </summary>
        /// <param name="ruleName">The rule name.</param>
        /// <param name="validateMethod">The Action that validates an instance of the specified type.</param>
        /// <param name="includeImpliedProperties">Whether the rule should include implicitly selected properties.</param>
        public ODataUrlValidationRule(string ruleName, Action<ODataUrlValidationContext, T> validateMethod, bool includeImpliedProperties)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(ruleName, "ruleName");
            this.RuleName = ruleName;
            this.IncludeImpliedProperties = includeImpliedProperties;
            this.validateMethod = validateMethod;
        }

        /// <summary>
        /// Internal API called by the validation engine to validate the item of the specified type
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="component">The component.</param>
        internal override void Validate(ODataUrlValidationContext context, object component)
        {
            validateMethod(context, component as T);
        }

        /// <summary>
        /// Internal API called to get the type of Url element that the rule targets
        /// </summary>
        /// <returns>The type of the Url element that the rule targets</returns>
        internal override Type GetRuleType()
        {
            return ruleType;
        }
    }
}