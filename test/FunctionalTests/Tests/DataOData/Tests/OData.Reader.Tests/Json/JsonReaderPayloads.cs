//---------------------------------------------------------------------
// <copyright file="JsonReaderPayloads.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Json
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    #endregion Namespaces

    /// <summary>
    /// Helper class to generate interesting JSON reader payloads.
    /// </summary>
    public static class JsonReaderPayloads
    {
        /// <summary>
        /// Returns all descriptors where for each test descriptor from the input all payload cases returned by the func are applied.
        /// </summary>
        /// <param name="testDescriptors">The test descriptors to use.</param>
        /// <param name="payloadCasesFunc">The payload case func to use.</param>
        /// <returns>Enumeration of all test descriptors.</returns>
        public static IEnumerable<JsonReaderTestCaseDescriptor> PayloadCases(
            this IEnumerable<JsonReaderTestCaseDescriptor> testDescriptors,
            Func<JsonReaderTestCaseDescriptor, IEnumerable<JsonReaderTestCaseDescriptor>> payloadCasesFunc)
        {
            return testDescriptors.SelectMany(testDescriptor => 
                {
                    if (testDescriptor.DisablePayloadCombinations)
                    {
                        return LinqExtensions.FromSingle(testDescriptor);
                    }
                    else
                    {
                        return payloadCasesFunc(testDescriptor);
                    }
                });
        }

        /// <summary>
        /// Generates interesting payloads by replacing the whitespace marks (^) with different kinds of whitespaces.
        /// If two consecutive marks are used (^^) it means that this needs to be replaced with non-empty whitespace.
        /// </summary>
        /// <param name="testCase">The test case to process.</param>
        /// <returns>Enumeration of interesting test cases.</returns>
        public static IEnumerable<JsonReaderTestCaseDescriptor> WhitespacePaylods(JsonReaderTestCaseDescriptor testCase)
        {
            var whitespaces = new string[][]
            {
                new string [] { "" },
                new string [] { " " },
                new string [] { " ", "\t", "\n", "\r" },
                new string [] { "  \t", "\r\n", "\t\t", "     \r\n\t" }
            };

            return whitespaces.Select(ws => 
                new JsonReaderTestCaseDescriptor(testCase) 
                { 
                    JsonText = ReplaceWhitespaceMarks(testCase.JsonText, ws.EndLessLoop()) 
                });
        }

        /// <summary>
        /// Generates interesting payloads for JSON values.
        /// </summary>
        /// <param name="primitiveValuePayloadTestCase">The value test case, where the JsonText represents 
        /// a single value and ExpectedJson is a JsonValue instance.</param>
        /// <returns>Enumeration of all interesting test cases.</returns>
        public static IEnumerable<JsonReaderTestCaseDescriptor> ValuePayloads(JsonReaderTestCaseDescriptor valueTestCase)
        {
            Debug.Assert(
                valueTestCase.ExpectedJson is JsonValue || valueTestCase.ExpectedException != null,
                "This method only works on value test cases.");

            // The value itself at the root - just add whitespace marks around it.
            yield return new JsonReaderTestCaseDescriptor(valueTestCase) { JsonText = "^" + valueTestCase.JsonText + "^" };

            // Value in an array - the only element
            yield return new JsonReaderTestCaseDescriptor(valueTestCase)
            {
                JsonText = "^[^" + valueTestCase.JsonText + "^]^",
                FragmentExtractor = (json) => json.Array().Elements.First()
            };

            // Value in an array - surrounded by other elements
            yield return new JsonReaderTestCaseDescriptor(valueTestCase)
            {
                JsonText = "^[^\"before\"^,^" + valueTestCase.JsonText + "^,42^]^",
                FragmentExtractor = (json) => json.Array().Elements.Skip(1).First()
            };

            // Value in a nested array
            yield return new JsonReaderTestCaseDescriptor(valueTestCase)
            {
                JsonText = "^[^[^" + valueTestCase.JsonText + "^]^]^",
                FragmentExtractor = (json) => json.Array().Elements.First().Array().Elements.First()
            };

            // Value of a property in an object
            yield return new JsonReaderTestCaseDescriptor(valueTestCase)
            {
                JsonText = "^{^\"propertyName\"^:^" + valueTestCase.JsonText + "^}^",
                FragmentExtractor = (json) => json.Object().Properties.First().Value
            };

            // Value of a property in an nested object
            yield return new JsonReaderTestCaseDescriptor(valueTestCase)
            {
                JsonText = "^{^\"child\":^{^\"propertyName\"^:^" + valueTestCase.JsonText + "^}^}^",
                FragmentExtractor = (json) => json.Object().Properties.First().Value.Object().Properties.First().Value
            };

            // Value in an array in an object in an array
            yield return new JsonReaderTestCaseDescriptor(valueTestCase)
            {
                JsonText = "^[^42,^{^\"propertyName\"^:^[^" + valueTestCase.JsonText + "^,^{^}^]^}^,^[^]^]^",
                FragmentExtractor = (json) => json.Array().Elements.Skip(1).First().Object().Properties.First().Value.Array().Elements.First()
            };
        }

        /// <summary>
        /// Generates interesting payloads for property cases.
        /// </summary>
        /// <param name="propertyPayloadTestCase">The property test case, where the JsonText represents 
        /// a single property and ExpectedJson is a JsonProperty instance.</param>
        /// <returns>Enumeration of all interesting test cases.</returns>
        public static IEnumerable<JsonReaderTestCaseDescriptor> PropertyPayloads(JsonReaderTestCaseDescriptor propertyTestCase)
        {
            Debug.Assert(
                propertyTestCase.ExpectedJson is JsonProperty || propertyTestCase.ExpectedException != null,
                "This method only works on property test cases.");

            // The property alone in an object
            yield return new JsonReaderTestCaseDescriptor(propertyTestCase) 
            { 
                JsonText = "^{^" + propertyTestCase.JsonText + "^}^",
                FragmentExtractor = (json) => json.Object().Properties.First()
            };

            // The property surrounded by other properties
            yield return new JsonReaderTestCaseDescriptor(propertyTestCase)
            {
                JsonText = "^{^\"firstprop\"^:^1^,^" + propertyTestCase.JsonText + "^,^\"lastprop\"^:^2^}^",
                FragmentExtractor = (json) => json.Object().Properties.Skip(1).First()
            };

            // The property as first property followed by another.
            yield return new JsonReaderTestCaseDescriptor(propertyTestCase)
            {
                JsonText = "^{^" + propertyTestCase.JsonText + "^,^\"lastprop\"^:^2^}^",
                FragmentExtractor = (json) => json.Object().Properties.First()
            };

            // The property as last property preceded by another
            yield return new JsonReaderTestCaseDescriptor(propertyTestCase)
            {
                JsonText = "^{^\"firstprop\"^:^1^,^" + propertyTestCase.JsonText + "^}^",
                FragmentExtractor = (json) => json.Object().Properties.Skip(1).First()
            };
        }

        /// <summary>
        /// Replaces whitepace marks (^) with values from the enumerator.
        /// If two consecutive marks are used (^^) it means that this needs to be replaced with non-empty whitespace.
        /// </summary>
        /// <param name="original">The original string to process.</param>
        /// <param name="whitespaceLoop">Enumerator which returns whitespace to use for replacement.</param>
        /// <returns>The new string with whitespace marks replaced.</returns>
        private static string ReplaceWhitespaceMarks(string original, IEnumerator<string> whitespaceLoop)
        {
            StringBuilder sb = new StringBuilder();
            
            int start = 0;
            while (start < original.Length)
            {
                int i = original.IndexOf('^', start);
                if (i == -1)
                {
                    break;
                }

                sb.Append(original.Substring(start, i - start));
                whitespaceLoop.MoveNext();
                if ((i < original.Length - 1) && original[i + 1] == '^')
                {
                    if (whitespaceLoop.Current.Length == 0)
                    {
                        sb.Append(' ');
                    }
                    else
                    {
                        sb.Append(whitespaceLoop.Current);
                    }

                    start = i + 2;
                }
                else
                {
                    sb.Append(whitespaceLoop.Current);
                    start = i + 1;
                }
            }

            if (start < original.Length)
            {
                sb.Append(original.Substring(start));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates invalid error representations in JSON (e.g., extra properties where they are not allowed,
        /// invalid property value types, etc.) - no duplicate properties.
        /// </summary>
        /// <param name="settings">The test descriptor settings to use for the generated <see cref="PayloadReaderTestDescriptor"/>.</param>
        /// <param name="isJsonLight">true if the payloads should be in Json lite format; false if they should be in verbose Json.</param>
        /// <returns>An enumerable of <see cref="PayloadReaderTestDescriptor"/> representing the invalid error payloads.</returns>
        public static IEnumerable<PayloadReaderTestDescriptor> CreateInvalidErrorDescriptors(PayloadReaderTestDescriptor.Settings settings, bool isJsonLight)
        {
            string errorPropertyName = JsonLightConstants.ODataErrorPropertyName;

            return new PayloadReaderTestDescriptor[]
                   {
                       #region Extra properties at the top-level
                       // extra properties in top-level object
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error("foo").Message("msg1")
                               .JsonRepresentation("{ \"foo\": \"bar\" }"),
                           ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonErrorDeserializer_TopLevelErrorWithInvalidProperty", "foo"),
                       },
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error("foo").Message("msg1")
                               .JsonRepresentation("{ \"foo\": \"bar\", \"" + errorPropertyName + "\": { } }"),
                           ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonErrorDeserializer_TopLevelErrorWithInvalidProperty", "foo"),
                       },
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error("foo").Message("msg1")
                               .JsonRepresentation("{ \"" + errorPropertyName + "\": { }, \"foo\": \"bar\" }"),
                           ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonErrorDeserializer_TopLevelErrorWithInvalidProperty", "foo"),
                       },

                       // extra properties in top-level error object
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": { \"foo\": \"bar\" } }"),
                           ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightErrorDeserializer_TopLevelErrorValueWithInvalidProperty", "foo"),
                       },
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error("foo").Message("msg1")
                               .JsonRepresentation("{ \"" + errorPropertyName + "\": { \"message\": \"msg1\", \"foo\": \"bar\" } } }"),
                           ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightErrorDeserializer_TopLevelErrorValueWithInvalidProperty", "foo"),
                       },
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error("foo").Message("msg1")
                               .JsonRepresentation("{ \"" + errorPropertyName + "\": { \"foo\": \"bar\", \"message\": \"msg1\" , \"foo2\": \"bar2\" } } }"),
                           ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightErrorDeserializer_TopLevelErrorValueWithInvalidProperty", "foo"),
                       },
                       #endregion Extra properties at the top-level

                       #region Invalid property values
                       // invalid property values for top-level error property
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": null }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "PrimitiveValue"),
                       },
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": 42 }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "PrimitiveValue"),
                       },
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": [ ] }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "StartArray"),
                       },

                       // invalid property values for message property
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": { \"message\": [ ] }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartArray"),
                       },
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": { \"message\": 42 }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_CannotReadPropertyValueAsString", "42", "message"),
                       },
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": { \"message\": {} }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartObject"),
                       },

                       // invalid property values for innererror value property
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": { \"innererror\": 42 }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "PrimitiveValue"),
                       },
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": { \"innererror\": [ ] }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "StartArray"),
                       },

                       // invalid property values for message property on innererror value
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": { \"innererror\": { \"message\": { } }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartObject"),
                       },
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": { \"innererror\": { \"message\": [ ] } }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartArray"),
                       },

                       // invalid property values for stacktrace property on innererror value
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": { \"innererror\": { \"stacktrace\": { } }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartObject"),
                       },
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": { \"innererror\": { \"stacktrace\": [ ] } }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartArray"),
                       },

                       // invalid property values for type name property on innererror value
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": { \"innererror\": { \"type\": { } }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartObject"),
                       },
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": { \"innererror\": { \"type\": [ ] } }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartArray"),
                       },

                       // invalid property values for internal exception property on innererror value
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": { \"innererror\": { \"internalexception\": 42 }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "PrimitiveValue"),
                       },
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": { \"innererror\": { \"internalexception\": [ ] } }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "StartArray"),
                       },

                       // invalid property values for message property on internal exception property value
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": { \"innererror\": { \"internalexception\": { \"message\": { } } }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartObject"),
                       },
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": { \"innererror\": { \"internalexception\": { \"message\": [ ] } } }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartArray"),
                       },

                       // invalid property values for stacktrace property on internal exception property value
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": { \"innererror\": { \"internalexception\": { \"stacktrace\": { } } }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartObject"),
                       },
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": { \"innererror\": { \"internalexception\": { \"stacktrace\": [ ] } } }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartArray"),
                       },

                       // invalid property values for type name property on internal exception property value
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": { \"innererror\": { \"internalexception\": { \"type\": { } } }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartObject"),
                       },
                       new PayloadReaderTestDescriptor(settings)
                       {
                           PayloadElement = PayloadBuilder.Error().JsonRepresentation("{ \"" + errorPropertyName + "\": { \"innererror\": { \"internalexception\": { \"type\": [ ] } } }"),
                           ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartArray"),
                       },
                       #endregion Invalid property values
                   };
        }
    }
}
