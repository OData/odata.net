//---------------------------------------------------------------------
// <copyright file="ValidationRuleSetTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Edm.Validation;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Validation
{
    /// <summary>
    /// ValidationRulesTests tests
    /// </summary>
    public class ValidationRuleSetTests
    {
        [Fact]
        public void NewValidationRulesShouldBeInTheRuleSetExceptBaselinedExceptionRules()
        {
            var validationRules = new Dictionary<object, string>();
            var items = typeof(ValidationRules).GetFields().Select(f=> new KeyValuePair<object, string>(f.GetValue(null), f.Name));
            foreach (var item in items)
            {
                validationRules.Add(item.Key, item.Value);
            }

            var unFoundValidationRules = new List<object>();

            foreach(var ruleSet in ValidationRuleSet.GetEdmModelRuleSet(new Version(4, 0)))
            {
                if (validationRules.ContainsKey(ruleSet))
                {
                    validationRules.Remove(ruleSet);
                }
                else
                {
                    unFoundValidationRules.Add(validationRules);
                }
            }

            unFoundValidationRules.Should().HaveCount(0);

            // The 4 remaining rules are deprecated:
            // ComplexTypeMustContainProperties
            // OnlyEntityTypesCanBeOpen
            // ComplexTypeInvalidPolymorphicComplexType
            // ComplexTypeInvalidAbstractComplexType
            validationRules.ToList().Should().HaveCount(4);
        }
    }
}
