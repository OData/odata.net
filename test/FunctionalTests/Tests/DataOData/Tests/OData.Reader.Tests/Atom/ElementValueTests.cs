//---------------------------------------------------------------------
// <copyright file="ElementValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of values of elements (we should use the exact same algorithm for all elements)
    /// </summary>
    [TestClass, TestCase]
    public class ElementValueTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        private sealed class ElementValueTestTarget
        {
            public Func<string, Func<string, string, string>, ODataPayloadElement> PayloadElement { get; set; }
        }

        private sealed class ElementValueTestTargetWithValue
        {
            public Func<Func<string, string, string>, ODataPayloadElement> PayloadElement { get; set; }
        }

        private sealed class ElementValueTestValue
        {
            public Func<string, string, string> XmlValueCreator { get; set; }
            public ExpectedException ExpectedException { get; set; }
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies that elements values reading works consistently everywhere.")]
        public void ElementValueTest()
        {
            // TODO: Add test cases for ATOM metadata once it's implemented
            // Note that we don't test EPM here which we could do as well, although if we test ATOM metadata, then EPM uses almost the same code path.
            var targetsForEmptyString = new ElementValueTestTarget[]
            {
                // Property value
                new ElementValueTestTarget
                {
                    PayloadElement = (value, xmlValueCreator) =>
                        PayloadBuilder.PrimitiveProperty(null, value).XmlRepresentation(xmlValueCreator("<m:value>{0}</m:value>", value))
                },
                // Error code
                new ElementValueTestTarget
                {
                    PayloadElement = (value, xmlValueCreator) =>
                        PayloadBuilder.Error().Code(value).XmlRepresentation(xmlValueCreator("<m:error><m:code>{0}</m:code></m:error>", value))
                },
                // Error message
                new ElementValueTestTarget
                {
                    PayloadElement = (value, xmlValueCreator) =>
                        PayloadBuilder.Error().Code("code").Message(value).XmlRepresentation(xmlValueCreator("<m:error><m:code>code</m:code><m:message>{0}</m:message></m:error>", value))
                },
                // Inner error message
                new ElementValueTestTarget
                {
                    PayloadElement = (value, xmlValueCreator) =>
                        PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().Message(value)).XmlRepresentation(xmlValueCreator("<m:error><m:innererror><m:message>{0}</m:message></m:innererror></m:error>", value))
                },
                // Inner error type name
                new ElementValueTestTarget
                {
                    PayloadElement = (value, xmlValueCreator) =>
                        PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().TypeName(value)).XmlRepresentation(xmlValueCreator("<m:error><m:innererror><m:type>{0}</m:type></m:innererror></m:error>", value))
                },
                // Inner error stacktrace
                new ElementValueTestTarget
                {
                    PayloadElement = (value, xmlValueCreator) =>
                        PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().StackTrace(value)).XmlRepresentation(xmlValueCreator("<m:error><m:innererror><m:stacktrace>{0}</m:stacktrace></m:innererror></m:error>", value))
                },
            };

            var targetsForNonEmptyString = new ElementValueTestTarget[]
            {
                // Entity ID
                new ElementValueTestTarget
                {
                    PayloadElement = (value, xmlValueCreator) =>
                        PayloadBuilder.Entity().Id(value).XmlRepresentation(xmlValueCreator("<entry><id>{0}</id></entry>", value))
                },
                // Feed ID
                new ElementValueTestTarget
                {
                    PayloadElement = (value, xmlValueCreator) =>
                        PayloadBuilder.EntitySet().AtomId(value).XmlRepresentation(xmlValueCreator("<feed><id>{0}</id></feed>", value))
                }
            };

            var targets =
                SetValueForTarget(
                    new string[]
                    {
                        string.Empty
                    },
                    targetsForEmptyString)
                .Concat(SetValueForTarget(
                    new string[]
                    {
                        "urn:id"
                    },
                    targetsForNonEmptyString.Concat(targetsForEmptyString)));

            var values = new ElementValueTestValue[]
            {
                // Just a simple string with no xml:space
                new ElementValueTestValue
                {
                    XmlValueCreator = (template, value) => string.Format(CultureInfo.InvariantCulture, template, value)
                },
                // Comments and PIS before and after with no xml:space
                new ElementValueTestValue
                {
                    XmlValueCreator = (template, value) => string.Format(CultureInfo.InvariantCulture, template, 
                        "<!--before-->" + value + "<!--after-->")
                },
                // Insignificant whitespace before and after
                new ElementValueTestValue
                {
                    XmlValueCreator = (template, value) => string.Format(CultureInfo.InvariantCulture, template, 
                        "   <?before?>" + value + "<!--after-->\n\t")
                },
                // Comments, PI and whitespace in the middle
                new ElementValueTestValue
                {
                    XmlValueCreator = (template, value) =>
                        {
                            if (value.Length < 2) { return string.Format(CultureInfo.InvariantCulture, template, value); }
                            int splitIndex = value.Length / 2;
                            string firstPart = value.Substring(0, splitIndex);
                            string secondPart = value.Substring(splitIndex);
                            return string.Format(CultureInfo.InvariantCulture, template,
                                firstPart + "<!--middle-->  \n\t<?middle?>" + secondPart);
                        }
                },
                // Element before causes a failure
                new ElementValueTestValue
                {
                    XmlValueCreator = (template, value) => string.Format(CultureInfo.InvariantCulture, template, 
                        "<some/>" + value),
                    ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element")
                },
                // Element after causes a failure
                new ElementValueTestValue
                {
                    XmlValueCreator = (template, value) => string.Format(CultureInfo.InvariantCulture, template, 
                        value + "<some/>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element")
                }
            };

            var testDescriptors = targets.SelectMany(target => values.Select(value =>
                {
                    return new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = target.PayloadElement(value.XmlValueCreator),
                        ExpectedException = value.ExpectedException
                    };
                }));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        private IEnumerable<ElementValueTestTargetWithValue> SetValueForTarget(IEnumerable<string> values, IEnumerable<ElementValueTestTarget> targets)
        {
            return values.SelectMany(value => targets.Select(target =>
                new ElementValueTestTargetWithValue
                {
                    PayloadElement = new Func<Func<string, string, string>, ODataPayloadElement>(xmlValueCreator =>
                        target.PayloadElement(value, xmlValueCreator))
                }));
        }
    }
}