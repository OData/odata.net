//---------------------------------------------------------------------
// <copyright file="ODataErrorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.Spatial;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataErrorTests
    {
        private ODataError odataError;

        public ODataErrorTests()
        {
            this.odataError = new ODataError();
        }

        [Fact]
        public void InstanceAnnotationsPropertyShouldNotBeNullAtCreation()
        {
            Assert.NotNull(this.odataError.InstanceAnnotations);
        }

        [Fact]
        public void InstanceAnnotationsPropertyShouldReturnAWritableCollectionAtCreation()
        {
            Assert.NotNull(this.odataError.InstanceAnnotations);
            this.odataError.InstanceAnnotations.Add(new ODataInstanceAnnotation("namespace.name", new ODataPrimitiveValue("value")));
            Assert.Single(this.odataError.InstanceAnnotations);
        }

        [Fact]
        public void SetNullValueToInstanceAnnotationsPropertyShouldThrow()
        {
            Action test = () => this.odataError.InstanceAnnotations = null;
            Assert.Throws<ArgumentNullException>("value", test);
        }

        [Fact]
        public void SetListValueToInstanceAnnotationsPropertyShouldPass()
        {
            ICollection<ODataInstanceAnnotation> initialCollection = this.odataError.InstanceAnnotations;
            ICollection<ODataInstanceAnnotation> newCollection = new List<ODataInstanceAnnotation>();
            this.odataError.InstanceAnnotations = newCollection;
            Assert.Same(this.odataError.InstanceAnnotations, newCollection);
            Assert.NotSame(this.odataError.InstanceAnnotations, initialCollection);
        }

        [Fact]
        public void GetInstanceAnnotationsForWritingShouldReturnEmptyInstanceAnnotationsFromNewODataError()
        {
            Assert.NotNull(this.odataError.InstanceAnnotations);
            Assert.Empty(this.odataError.InstanceAnnotations);
        }

        [Fact]
        public void GetInstanceAnnotationsForWritingShouldReturnInstanceAnnotationsFromODataErrorWithInstanceAnnotations()
        {
            ODataInstanceAnnotation instanceAnnotation = new ODataInstanceAnnotation("namespace.name", new ODataPrimitiveValue("value"));
            this.odataError.InstanceAnnotations.Add(instanceAnnotation);
            var annotation = Assert.Single(this.odataError.InstanceAnnotations);
            Assert.Same(annotation, instanceAnnotation);
        }

        public static IEnumerable<object[]> GetODataInnerErrorTestData()
        {
            return
            [
                [
                    new ODataInnerError(),
                    "{}"
                ],
                [
                    new ODataInnerError(new Dictionary<string, ODataValue>
                    {
                        { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM1") }
                    }),
                    "{\"message\":\"OIEM1\"}"
                ],
                [
                    new ODataInnerError(new Dictionary<string, ODataValue>
                    {
                        { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET1") }
                    }),
                    "{\"type\":\"OIET1\"}"
                ],
                [
                    new ODataInnerError(new Dictionary<string, ODataValue>
                    {
                        { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES1") }
                    }),
                    "{\"stacktrace\":\"OIES1\"}"
                ],
                [
                    new ODataInnerError(new Dictionary<string, ODataValue>
                    {
                        { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM1") },
                        { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET1") }
                    }),
                    "{\"message\":\"OIEM1\",\"type\":\"OIET1\"}"
                ],
                [
                    new ODataInnerError(new Dictionary<string, ODataValue>
                    {
                        { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM1") },
                        { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES1") }
                    }),
                    "{\"message\":\"OIEM1\",\"stacktrace\":\"OIES1\"}"
                ],
                [
                    new ODataInnerError(new Dictionary<string, ODataValue>
                    {
                        { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET1") },
                        { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES1") }
                    }),
                    "{\"type\":\"OIET1\",\"stacktrace\":\"OIES1\"}"
                ],
                [
                    new ODataInnerError(new Dictionary<string, ODataValue>
                    {
                        { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM1") },
                        { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET1") },
                        { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES1") }
                    }),
                    "{\"message\":\"OIEM1\",\"type\":\"OIET1\",\"stacktrace\":\"OIES1\"}"
                ],
                [
                    new ODataInnerError(
                        new Dictionary<string, ODataValue>
                        {
                            { "p1", new ODataPrimitiveValue(1) },
                            { "p2", new ODataPrimitiveValue("SP") }
                        }),
                    "{\"p1\":1,\"p2\":\"SP\"}"
                ],
                [
                    new ODataInnerError
                    {
                        InnerError = new ODataInnerError()
                    },
                    "{\"innererror\":{}}"
                ],
                [
                    new ODataInnerError(new Dictionary<string, ODataValue>
                    {
                        { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM1") },
                        { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET1") },
                        { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES1") }
                    })
                    {
                        InnerError = new ODataInnerError(new Dictionary<string, ODataValue>
                        {
                            { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM2") },
                            { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET2") },
                            { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES2") }
                        })
                    },
                    "{\"message\":\"OIEM1\"," +
                    "\"type\":\"OIET1\"," +
                    "\"stacktrace\":\"OIES1\"," +
                    "\"innererror\":{\"message\":\"OIEM2\",\"type\":\"OIET2\",\"stacktrace\":\"OIES2\"}}"
                ],
                [
                    new ODataInnerError(
                        new Dictionary<string, ODataValue>
                        {
                            { "p1", new ODataPrimitiveValue(1) },
                            { "p2", new ODataPrimitiveValue("SP") },
                            { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM1") },
                            { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET1") },
                            { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES1") }
                        })
                    {
                        InnerError = new ODataInnerError(new Dictionary<string, ODataValue>
                        {
                            { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM2") },
                            { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET2") },
                            { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES2") }
                        })
                    },
                    "{\"p1\":1," +
                    "\"p2\":\"SP\"," +
                    "\"message\":\"OIEM1\"," +
                    "\"type\":\"OIET1\"," +
                    "\"stacktrace\":\"OIES1\"," +
                    "\"innererror\":{\"message\":\"OIEM2\",\"type\":\"OIET2\",\"stacktrace\":\"OIES2\"}}"
                ],
                [
                    new ODataInnerError(new Dictionary<string, ODataValue>
                    {
                        { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM1") },
                        { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET1") },
                        { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES1") }
                    })
                    {
                        InnerError = new ODataInnerError(new Dictionary<string, ODataValue>
                        {
                            { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM2") },
                            { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET2") },
                            { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES2") }
                        })
                        {
                            InnerError = new ODataInnerError(new Dictionary<string, ODataValue>
                            {
                                { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM3") },
                                { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET3") },
                                { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES3") }
                            })
                        }
                    },
                    "{\"message\":\"OIEM1\"," +
                    "\"type\":\"OIET1\"," +
                    "\"stacktrace\":\"OIES1\"," +
                    "\"innererror\":{" +
                    "\"message\":\"OIEM2\"," +
                    "\"type\":\"OIET2\"," +
                    "\"stacktrace\":\"OIES2\"," +
                    "\"innererror\":{\"message\":\"OIEM3\",\"type\":\"OIET3\",\"stacktrace\":\"OIES3\"}}}"
                ]
            ];
        }

        [Theory]
        [MemberData(nameof(GetODataInnerErrorTestData))]
        public void TestODataInnerErrorToJsonString(ODataInnerError odataInnerError, string expectedJsonString)
        {
            // Arrange & Act
            var jsonString = odataInnerError.ToJsonString();

            // Assert
            Assert.Equal(expectedJsonString, jsonString);
        }

        public static IEnumerable<object[]> GetODataErrorDetailTestData()
        {
            return
            [
                [
                    new ODataErrorDetail(),
                    "{\"code\":\"\",\"message\":\"\"}"
                ],
                [
                    new ODataErrorDetail { Code = "OEDC1", Message = "OEDM1" },
                    "{\"code\":\"OEDC1\",\"message\":\"OEDM1\"}"
                ],
                [
                    new ODataErrorDetail { Code = "OEDC1", Message = "OEDM1", Target = "OEDT1" },
                    "{\"code\":\"OEDC1\",\"message\":\"OEDM1\",\"target\":\"OEDT1\"}"
                ],
                [
                    new ODataErrorDetail { Code = "OEDC1", Message = "OEDM1", Target = "" },
                    "{\"code\":\"OEDC1\",\"message\":\"OEDM1\",\"target\":\"\"}"
                ]
            ];
        }

        [Theory]
        [MemberData(nameof(GetODataErrorDetailTestData))]
        public void TestODataErrorDetailToJsonString(ODataErrorDetail odataErrorDetail, string expectedJsonString)
        {
            // Arrange & Act
            var jsonString = odataErrorDetail.ToJsonString();

            // Assert
            Assert.Equal(expectedJsonString, jsonString);
        }

        [Fact]
        public void TestODataInnerErrorToJsonStringForUnsupportedODataValues()
        {
            var jsonInputString = "{\"foo\":\"bar\"}";

            using (var jsonDocument = JsonDocument.Parse(jsonInputString))
            {
                using (var stream = new MemoryStream())
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write("1234567890");
                        writer.Flush();
                        stream.Position = 0;

                        // Arrange
                        var odataInnerError = new ODataInnerError(
                            new Dictionary<string, ODataValue>
                            {
                                { "p1", new ODataEnumValue("Black") },
                                { "p2", new ODataUntypedValue { RawValue = "\"roygbiv\"" } },
                                { "p3", new ODataBinaryStreamValue(stream, leaveOpen: false) },
                                { "p4", new ODataStreamReferenceValue() },
                                { "p5", new ODataJsonElementValue(jsonDocument.RootElement) }
                            });

                        // Act
                        var jsonString = odataInnerError.ToJsonString();

                        // Assert
                        Assert.Equal(
                            "{" +
                            "\"p1\":\"The value of type 'Microsoft.OData.ODataEnumValue' is not supported and cannot be converted to a JSON representation.\"," +
                            "\"p2\":\"The value of type 'Microsoft.OData.ODataUntypedValue' is not supported and cannot be converted to a JSON representation.\"," +
                            "\"p3\":\"The value of type 'Microsoft.OData.ODataBinaryStreamValue' is not supported and cannot be converted to a JSON representation.\"," +
                            "\"p4\":\"The value of type 'Microsoft.OData.ODataStreamReferenceValue' is not supported and cannot be converted to a JSON representation.\"," +
                            "\"p5\":\"The value of type 'Microsoft.OData.ODataJsonElementValue' is not supported and cannot be converted to a JSON representation.\"}",
                            jsonString);
                    }
                }
            }
        }

        [Fact]
        public void TestODataInnerErrorToJsonStringForUnsupportedODataPrimitiveValues()
        {
            // Arrange
            var odataInnerError = new ODataInnerError(
                new Dictionary<string, ODataValue>
                {
                    { "p1", new ODataPrimitiveValue(GeographyPoint.Create(22.2, 22.2)) },
                });

            // Act
            var jsonString = odataInnerError.ToJsonString();

            // Assert
            Assert.Equal(
                "{\"p1\":\"The value of type 'Microsoft.Spatial.GeographyPointImplementation' is not supported and cannot be converted to a JSON representation.\"}",
                jsonString);
        }

        [Fact]
        public void TestODataInnerErrorToJsonStringForUnsupportedODataValuesInODataResourceValue()
        {
            var jsonInputString = "{\"foo\":\"bar\"}";

            using (var jsonDocument = JsonDocument.Parse(jsonInputString))
            {
                using (var stream = new MemoryStream())
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write("1234567890");
                        writer.Flush();
                        stream.Position = 0;

                        // Arrange
                        var odataInnerError = new ODataInnerError(
                            new Dictionary<string, ODataValue>
                            {
                                {
                                    "p1",
                                    new ODataResourceValue
                                    {
                                        Properties = new List<ODataProperty>
                                        {
                                            new ODataProperty { Name = "prop1", Value = new ODataEnumValue("Black") },
                                            new ODataProperty { Name = "prop2", Value = new ODataUntypedValue { RawValue = "\"roygbiv\"" } },
                                            new ODataProperty { Name = "prop3", Value = new ODataBinaryStreamValue(stream, leaveOpen: false) },
                                            new ODataProperty { Name = "prop4", Value = new ODataStreamReferenceValue() },
                                            new ODataProperty { Name = "prop5", Value = new ODataJsonElementValue(jsonDocument.RootElement) },
                                        }
                                    }
                                }
                            });

                        // Act
                        var jsonString = odataInnerError.ToJsonString();

                        // Assert
                        Assert.Equal(
                            "{" +
                            "\"p1\":{" +
                            "\"prop1\":\"The value of type 'Microsoft.OData.ODataEnumValue' is not supported and cannot be converted to a JSON representation.\"," +
                            "\"prop2\":\"The value of type 'Microsoft.OData.ODataUntypedValue' is not supported and cannot be converted to a JSON representation.\"," +
                            "\"prop3\":\"The value of type 'Microsoft.OData.ODataBinaryStreamValue' is not supported and cannot be converted to a JSON representation.\"," +
                            "\"prop4\":\"The value of type 'Microsoft.OData.ODataStreamReferenceValue' is not supported and cannot be converted to a JSON representation.\"," +
                            "\"prop5\":\"The value of type 'Microsoft.OData.ODataJsonElementValue' is not supported and cannot be converted to a JSON representation.\"}}",
                            jsonString);
                    }
                }
            }
        }

        [Fact]
        public void TestODataInnerErrorToJsonStringForUnsupportedODataValuesInODataCollectionValue()
        {
            var jsonInputString = "{\"foo\":\"bar\"}";

            using (var jsonDocument = JsonDocument.Parse(jsonInputString))
            {
                using (var stream = new MemoryStream())
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write("1234567890");
                        writer.Flush();
                        stream.Position = 0;

                        // Arrange
                        var odataInnerError = new ODataInnerError(
                            new Dictionary<string, ODataValue>
                            {
                                {
                                    "p1",
                                    new ODataCollectionValue
                                    {
                                        Items = new Collection<object>
                                        {
                                            new ODataEnumValue("Black"),
                                            new ODataUntypedValue { RawValue = "\"roygbiv\"" },
                                            new ODataBinaryStreamValue(stream, leaveOpen: false),
                                            new ODataStreamReferenceValue(),
                                            new ODataJsonElementValue(jsonDocument.RootElement),
                                            130M,
                                            new object()
                                        }
                                    }
                                }
                            });

                        // Act
                        var jsonString = odataInnerError.ToJsonString();

                        // Assert
                        Assert.Equal(
                            "{" +
                            "\"p1\":[" +
                            "\"The value of type 'Microsoft.OData.ODataEnumValue' is not supported and cannot be converted to a JSON representation.\"," +
                            "\"The value of type 'Microsoft.OData.ODataUntypedValue' is not supported and cannot be converted to a JSON representation.\"," +
                            "\"The value of type 'Microsoft.OData.ODataBinaryStreamValue' is not supported and cannot be converted to a JSON representation.\"," +
                            "\"The value of type 'Microsoft.OData.ODataStreamReferenceValue' is not supported and cannot be converted to a JSON representation.\"," +
                            "\"The value of type 'Microsoft.OData.ODataJsonElementValue' is not supported and cannot be converted to a JSON representation.\"," +
                            "\"The value of type 'System.Decimal' is not supported and cannot be converted to a JSON representation.\"," +
                            "\"The value of type 'System.Object' is not supported and cannot be converted to a JSON representation.\"]}",
                            jsonString);
                    }
                }
            }
        }

        public static IEnumerable<object[]> GetODataErrorTestData()
        {
            return
            [
                [
                    new ODataError(),
                    "{\"error\":{\"code\":\"\",\"message\":\"\"}}"
                ],
                [
                    new ODataError { Code = "OEC1" },
                    "{\"error\":{\"code\":\"OEC1\",\"message\":\"\"}}"
                ],
                [
                    new ODataError { Message = "OEM1" },
                    "{\"error\":{\"code\":\"\",\"message\":\"OEM1\"}}"
                ],
                [
                    new ODataError { Code = "OEC1", Message = "OEM1" },
                    "{\"error\":{\"code\":\"OEC1\",\"message\":\"OEM1\"}}"
                ],
                [
                    new ODataError { Code = "OEC1", Message = "OEM1", Target = "OET1" },
                    "{\"error\":{\"code\":\"OEC1\",\"message\":\"OEM1\",\"target\":\"OET1\"}}"
                ],
                [
                    new ODataError { Code = "OEC1", Message = "OEM1", Target = "" },
                    "{\"error\":{\"code\":\"OEC1\",\"message\":\"OEM1\",\"target\":\"\"}}"
                ],
                [
                    new ODataError
                    {
                        Code = "OEC1",
                        Message = "OEM1",
                        Target = "OET1",
                        InnerError = new ODataInnerError()
                    },
                    "{\"error\":{" +
                    "\"code\":\"OEC1\"," +
                    "\"message\":\"OEM1\"," +
                    "\"target\":\"OET1\"," +
                    "\"innererror\":{}}}"
                ],
                [
                    new ODataError
                    {
                        Code = "OEC1",
                        Message = "OEM1",
                        Target = "OET1",
                        InnerError = new ODataInnerError(
                            new Dictionary<string, ODataValue>
                            {
                                { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM1") }
                            })
                    },
                    "{\"error\":{" +
                    "\"code\":\"OEC1\"," +
                    "\"message\":\"OEM1\"," +
                    "\"target\":\"OET1\"," +
                    "\"innererror\":{\"message\":\"OIEM1\"}}}"
                ],
                [
                    new ODataError
                    {
                        Code = "OEC1",
                        Message = "OEM1",
                        Target = "OET1",
                        InnerError = new ODataInnerError(
                            new Dictionary<string, ODataValue>
                            {
                                { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET1") }
                            })
                    },
                    "{\"error\":{" +
                    "\"code\":\"OEC1\"," +
                    "\"message\":\"OEM1\"," +
                    "\"target\":\"OET1\"," +
                    "\"innererror\":{\"type\":\"OIET1\"}}}"
                ],
                [
                    new ODataError
                    {
                        Code = "OEC1",
                        Message = "OEM1",
                        Target = "OET1",
                        InnerError = new ODataInnerError(
                            new Dictionary<string, ODataValue>
                            {
                                { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES1") }
                            })
                    },
                    "{\"error\":{" +
                    "\"code\":\"OEC1\"," +
                    "\"message\":\"OEM1\"," +
                    "\"target\":\"OET1\"," +
                    "\"innererror\":{\"stacktrace\":\"OIES1\"}}}"
                ],
                [
                    new ODataError
                    {
                        Code = "OEC1",
                        Message = "OEM1",
                        Target = "OET1",
                        InnerError = new ODataInnerError(
                            new Dictionary<string, ODataValue>
                            {
                                { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM1") },
                                { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET1") }
                            })
                    },
                    "{\"error\":{" +
                    "\"code\":\"OEC1\"," +
                    "\"message\":\"OEM1\"," +
                    "\"target\":\"OET1\"," +
                    "\"innererror\":{\"message\":\"OIEM1\",\"type\":\"OIET1\"}}}"
                ],
                [
                    new ODataError
                    {
                        Code = "OEC1",
                        Message = "OEM1",
                        Target = "OET1",
                        InnerError = new ODataInnerError(
                            new Dictionary<string, ODataValue>
                            {
                                { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM1") },
                                { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES1") }
                            })
                    },
                    "{\"error\":{" +
                    "\"code\":\"OEC1\"," +
                    "\"message\":\"OEM1\"," +
                    "\"target\":\"OET1\"," +
                    "\"innererror\":{\"message\":\"OIEM1\",\"stacktrace\":\"OIES1\"}}}"
                ],
                [
                    new ODataError
                    {
                        Code = "OEC1",
                        Message = "OEM1",
                        Target = "OET1",
                        InnerError = new ODataInnerError(
                            new Dictionary<string, ODataValue>
                            {
                                { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET1") },
                                { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES1") }
                            })
                    },
                    "{\"error\":{" +
                    "\"code\":\"OEC1\"," +
                    "\"message\":\"OEM1\"," +
                    "\"target\":\"OET1\"," +
                    "\"innererror\":{\"type\":\"OIET1\",\"stacktrace\":\"OIES1\"}}}"
                ],
                [
                    new ODataError
                    {
                        Code = "OEC1",
                        Message = "OEM1",
                        Target = "OET1",
                        InnerError = new ODataInnerError(
                            new Dictionary<string, ODataValue>
                            {
                                { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM1") },
                                { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET1") },
                                { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES1") }
                            })
                    },
                    "{\"error\":{" +
                    "\"code\":\"OEC1\"," +
                    "\"message\":\"OEM1\"," +
                    "\"target\":\"OET1\"," +
                    "\"innererror\":{\"message\":\"OIEM1\",\"type\":\"OIET1\",\"stacktrace\":\"OIES1\"}}}"
                ],
                [
                    new ODataError
                    {
                        Code = "OEC1",
                        Message = "OEM1",
                        Target = "OET1",
                        InnerError = new ODataInnerError(
                            new Dictionary<string, ODataValue>
                            {
                                { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM1") },
                                { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET1") },
                                { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES1") }
                            })
                        {
                            InnerError = new ODataInnerError(
                            new Dictionary<string, ODataValue>
                            {
                                { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM2") },
                                { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET2") },
                                { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES2") }
                            })
                        }
                    },
                    "{\"error\":{" +
                    "\"code\":\"OEC1\"," +
                    "\"message\":\"OEM1\"," +
                    "\"target\":\"OET1\"," +
                    "\"innererror\":{" +
                    "\"message\":\"OIEM1\"," +
                    "\"type\":\"OIET1\"," +
                    "\"stacktrace\":\"OIES1\"," +
                    "\"innererror\":{\"message\":\"OIEM2\",\"type\":\"OIET2\",\"stacktrace\":\"OIES2\"}}}}"
                ],
                [
                    new ODataError
                    {
                        Code = "OEC1",
                        Message = "OEM1",
                        Target = "OET1",
                        InnerError = new ODataInnerError(
                            new Dictionary<string, ODataValue>
                            {
                                { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM1") },
                                { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET1") },
                                { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES1") }
                            })
                        {
                            InnerError = new ODataInnerError(
                                new Dictionary<string, ODataValue>
                                {
                                    { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM2") },
                                    { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET2") },
                                    { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES2") }
                                })
                            {
                                InnerError = new ODataInnerError(
                                    new Dictionary<string, ODataValue>
                                    {
                                        { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM3") },
                                        { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET3") },
                                        { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES3") }

                                    })
                            }
                        }
                    },
                    "{\"error\":{" +
                    "\"code\":\"OEC1\"," +
                    "\"message\":\"OEM1\"," +
                    "\"target\":\"OET1\"," +
                    "\"innererror\":{" +
                    "\"message\":\"OIEM1\"," +
                    "\"type\":\"OIET1\"," +
                    "\"stacktrace\":\"OIES1\"," +
                    "\"innererror\":{" +
                    "\"message\":\"OIEM2\"," +
                    "\"type\":\"OIET2\"," +
                    "\"stacktrace\":\"OIES2\"," +
                    "\"innererror\":{\"message\":\"OIEM3\",\"type\":\"OIET3\",\"stacktrace\":\"OIES3\"}}}}}"
                ],
                [
                    new ODataError
                    {
                        Code = "OEC1",
                        Message = "OEM1",
                        Target = "OET1",
                        InnerError = new ODataInnerError(
                            new Dictionary<string, ODataValue>
                            {
                                { "p1", new ODataPrimitiveValue(1) },
                                { "p2", new ODataPrimitiveValue("SP") },
                                { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM1") },
                                { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET1") },
                                { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES1") }
                            })
                    },
                    "{\"error\":{" +
                    "\"code\":\"OEC1\"," +
                    "\"message\":\"OEM1\"," +
                    "\"target\":\"OET1\"," +
                    "\"innererror\":{\"p1\":1,\"p2\":\"SP\",\"message\":\"OIEM1\",\"type\":\"OIET1\",\"stacktrace\":\"OIES1\"}}}"
                ],
                [
                    new ODataError
                    {
                        Code = "OEC1",
                        Message = "OEM1",
                        Target = "OET1",
                        Details = new Collection<ODataErrorDetail>()
                    },
                    "{\"error\":{\"code\":\"OEC1\",\"message\":\"OEM1\",\"target\":\"OET1\",\"details\":[]}}"
                ],
                [
                    new ODataError
                    {
                        Code = "OEC1",
                        Message = "OEM1",
                        Target = "OET1",
                        Details = new Collection<ODataErrorDetail> { null }
                    },
                    "{\"error\":{\"code\":\"OEC1\",\"message\":\"OEM1\",\"target\":\"OET1\",\"details\":[]}}"
                ],
                [
                    new ODataError
                    {
                        Code = "OEC1",
                        Message = "OEM1",
                        Target = "OET1",
                        Details = new Collection<ODataErrorDetail>
                        {
                            new ODataErrorDetail { Code = "OEDC1", Message = "OEDM1", Target = "OEDT1" }
                        }
                    },
                    "{\"error\":{" +
                    "\"code\":\"OEC1\"," +
                    "\"message\":\"OEM1\"," +
                    "\"target\":\"OET1\"," +
                    "\"details\":[{\"code\":\"OEDC1\",\"message\":\"OEDM1\",\"target\":\"OEDT1\"}]}}"
                ],
                [
                    new ODataError
                    {
                        Code = "OEC1",
                        Message = "OEM1",
                        Target = "OET1",
                        Details = new Collection<ODataErrorDetail>
                        {
                            new ODataErrorDetail { Code = "OEDC1", Message = "OEDM1", Target = "OEDT1" },
                            new ODataErrorDetail { Code = "OEDC2", Message = "OEDM2", Target = "OEDT2" }
                        }
                    },
                    "{\"error\":{" +
                    "\"code\":\"OEC1\"," +
                    "\"message\":\"OEM1\"," +
                    "\"target\":\"OET1\"," +
                    "\"details\":[" +
                    "{\"code\":\"OEDC1\",\"message\":\"OEDM1\",\"target\":\"OEDT1\"}," +
                    "{\"code\":\"OEDC2\",\"message\":\"OEDM2\",\"target\":\"OEDT2\"}]}}"
                ],
                [
                    new ODataError
                    {
                        Code = "OEC1",
                        Message = "OEM1",
                        Target = "OET1",
                        InnerError = new ODataInnerError(
                            new Dictionary<string, ODataValue>
                            {
                                { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM1") },
                                { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET1") },
                                { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES1") }
                            }),
                        Details = new Collection<ODataErrorDetail>
                        {
                            new ODataErrorDetail { Code = "OEDC1", Message = "OEDM1", Target = "OEDT1" }
                        }
                    },
                    "{\"error\":{" +
                    "\"code\":\"OEC1\"," +
                    "\"message\":\"OEM1\"," +
                    "\"target\":\"OET1\"," +
                    "\"details\":[{\"code\":\"OEDC1\",\"message\":\"OEDM1\",\"target\":\"OEDT1\"}]," +
                    "\"innererror\":{\"message\":\"OIEM1\",\"type\":\"OIET1\",\"stacktrace\":\"OIES1\"}}}"
                ],
                [
                    new ODataError
                    {
                        Code = "OEC1",
                        Message = "OEM1",
                        Target = "OET1",
                        InnerError = new ODataInnerError(
                            new Dictionary<string, ODataValue>
                            {
                                { "p1", new ODataPrimitiveValue(1) },
                                { "p2", new ODataPrimitiveValue("SP") },
                                { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM1") },
                                { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET1") },
                                { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES1") }
                            })
                        {
                            InnerError = new ODataInnerError(
                                new Dictionary<string, ODataValue>
                                {
                                    { "p3", new ODataPrimitiveValue(13M) },
                                    { "p4", new ODataPrimitiveValue(new DateTimeOffset(2024, 5, 2, 13, 27, 30, TimeSpan.Zero)) },
                                    { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM2") },
                                    { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET2") },
                                    { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES2") }
                                })
                        },
                        Details = new Collection<ODataErrorDetail>
                        {
                            new ODataErrorDetail { Code = "OEDC1", Message = "OEDM1", Target = "OEDT1" }
                        }
                    },
                    "{\"error\":{" +
                    "\"code\":\"OEC1\"," +
                    "\"message\":\"OEM1\"," +
                    "\"target\":\"OET1\"," +
                    "\"details\":[{\"code\":\"OEDC1\",\"message\":\"OEDM1\",\"target\":\"OEDT1\"}]," +
                    "\"innererror\":{" +
                    "\"p1\":1," +
                    "\"p2\":\"SP\"," +
                    "\"message\":\"OIEM1\"," +
                    "\"type\":\"OIET1\"," +
                    "\"stacktrace\":\"OIES1\"," +
                    "\"innererror\":{" +
                    "\"p3\":13," +
                    "\"p4\":\"2024-05-02T13:27:30Z\"," +
                    "\"message\":\"OIEM2\"," +
                    "\"type\":\"OIET2\"," +
                    "\"stacktrace\":\"OIES2\"}}}}"
                ],
                [
                    new ODataError
                    {
                        Code = "OEC1",
                        Message = "OEM1",
                        Target = "OET1",
                        InnerError = new ODataInnerError(
                            new Dictionary<string, ODataValue>
                            {
                                { "p1", null },
                                { "p2", new ODataNullValue() },
                                { "p3", new ODataPrimitiveValue(true) },
                                { "p4", new ODataPrimitiveValue((byte)10) },
                                { "p5", new ODataPrimitiveValue(130M) },
                                { "p6", new ODataPrimitiveValue(3.14159265359d) },
                                { "p7", new ODataPrimitiveValue((short)7) },
                                { "p8", new ODataPrimitiveValue(13) },
                                { "p9", new ODataPrimitiveValue(299792458L) },
                                { "p10", new ODataPrimitiveValue((sbyte)-10) },
                                { "p11", new ODataPrimitiveValue(3.142f) },
                                { "p12", new ODataPrimitiveValue("foobar") },
                                { "p13", new ODataPrimitiveValue(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }) },
                                { "p14", new ODataPrimitiveValue(new DateTimeOffset(2024, 5, 2, 13, 27, 30, TimeSpan.Zero)) },
                                { "p15", new ODataPrimitiveValue(Guid.Parse("6693ceb2-5d47-45c7-b928-900ebdebe898")) },
                                { "p16", new ODataPrimitiveValue(new TimeSpan(1, 12, 0)) },
                                { "p17", new ODataPrimitiveValue(new Date(2024, 5, 2)) },
                                { "p18", new ODataPrimitiveValue(new TimeOfDay(12, 42, 30, 100)) },
                                { "p19", new ODataResourceValue
                                    {
                                        Properties = new List<ODataProperty>
                                        {
                                            new ODataProperty
                                            {
                                                Name = "DateOfBirth",
                                                Value = new ODataPrimitiveValue(new DateTimeOffset(1980, 12, 13, 11, 1, 1, TimeSpan.Zero))
                                            }
                                        }
                                    }
                                },
                                { "p20", new ODataCollectionValue
                                    {
                                        Items = new List<ODataPrimitiveValue>
                                        {
                                            new ODataPrimitiveValue(new DateTimeOffset(1990, 7, 17, 23, 59, 59, TimeSpan.Zero))
                                        }
                                    }
                                },
                                {
                                    "p21", new ODataPrimitiveValue("foo\tbar")
                                },
                                {
                                    "p22", new ODataPrimitiveValue("cA_Россия\r\n")
                                },
                                {
                                    "p23", new ODataPrimitiveValue("Быстрая коричневая лиса прыгает через ленивую собаку")
                                },
                                {
                                    "p24", new ODataPrimitiveValue("roygbiv\\\uD800\udc05 \u00e4")
                                },
                                {
                                    "p25", new ODataPrimitiveValue("ボァゼあクゾ\"")
                                },
                                {
                                    "p26", new ODataPrimitiveValue("😊😊😊")
                                },
                                { "p27", new ODataPrimitiveValue((DateTimeOffset?)new DateTimeOffset(2024, 5, 2, 13, 27, 30, TimeSpan.Zero)) },
                                { "p28", new ODataPrimitiveValue((Guid?)Guid.Parse("6693ceb2-5d47-45c7-b928-900ebdebe898")) },
                                { "p29", new ODataPrimitiveValue((TimeSpan?)new TimeSpan(1, 12, 0)) },
                                { "p30", new ODataPrimitiveValue((Date?)new Date(2024, 5, 2)) },
                                { "p31", new ODataPrimitiveValue((TimeOfDay?)new TimeOfDay(12, 42, 30, 100)) },
                                { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM1") },
                                { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET1") },
                                { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES1") }
                            })
                    },
                    "{\"error\":{" +
                    "\"code\":\"OEC1\"," +
                    "\"message\":\"OEM1\"," +
                    "\"target\":\"OET1\"," +
                    "\"innererror\":{" +
                    "\"p1\":null," +
                    "\"p2\":null," +
                    "\"p3\":true," +
                    "\"p4\":10," +
                    "\"p5\":130," +
                    "\"p6\":3.14159265359," +
                    "\"p7\":7," +
                    "\"p8\":13," +
                    "\"p9\":299792458," +
                    "\"p10\":-10," +
                    "\"p11\":3.142," +
                    "\"p12\":\"foobar\"," +
                    "\"p13\":\"AQIDBAUGBwgJAA==\"," +
                    "\"p14\":\"2024-05-02T13:27:30Z\"," +
                    "\"p15\":\"6693ceb2-5d47-45c7-b928-900ebdebe898\"," +
                    "\"p16\":\"PT1H12M\"," +
                    "\"p17\":\"2024-05-02\"," +
                    "\"p18\":\"12:42:30.1000000\"," +
                    "\"p19\":{\"DateOfBirth\":\"1980-12-13T11:01:01Z\"}," +
                    "\"p20\":[\"1990-07-17T23:59:59Z\"]," +
                    "\"p21\":\"foo\\tbar\"," +
                    "\"p22\":\"cA_\\u0420\\u043e\\u0441\\u0441\\u0438\\u044f\\r\\n\"," +
                    "\"p23\":\"\\u0411\\u044b\\u0441\\u0442\\u0440\\u0430\\u044f \\u043a\\u043e\\u0440\\u0438\\u0447\\u043d\\u0435\\u0432\\u0430\\u044f \\u043b\\u0438\\u0441\\u0430 \\u043f\\u0440\\u044b\\u0433\\u0430\\u0435\\u0442 \\u0447\\u0435\\u0440\\u0435\\u0437 \\u043b\\u0435\\u043d\\u0438\\u0432\\u0443\\u044e \\u0441\\u043e\\u0431\\u0430\\u043a\\u0443\"," +
                    "\"p24\":\"roygbiv\\\\\\ud800\\udc05 \\u00e4\"," +
                    "\"p25\":\"\\u30dc\\u30a1\\u30bc\\u3042\\u30af\\u30be\\\"\"," +
                    "\"p26\":\"\\ud83d\\ude0a\\ud83d\\ude0a\\ud83d\\ude0a\"," +
                    "\"p27\":\"2024-05-02T13:27:30Z\"," +
                    "\"p28\":\"6693ceb2-5d47-45c7-b928-900ebdebe898\"," +
                    "\"p29\":\"PT1H12M\"," +
                    "\"p30\":\"2024-05-02\"," +
                    "\"p31\":\"12:42:30.1000000\"," +
                    "\"message\":\"OIEM1\"," +
                    "\"type\":\"OIET1\"," +
                    "\"stacktrace\":\"OIES1\"}}}"
                ]
            ];
        }

        [Theory]
        [MemberData(nameof(GetODataErrorTestData))]
        public void TestODataErrorToString(ODataError odataError, string expectedJsonString)
        {
            // Arrange
            var stringWriter = new StringWriter();
            var jsonWriter = new JsonWriter(stringWriter, isIeee754Compatible: false);

            // Act
            var jsonString = odataError.ToString();
            ODataJsonWriterUtils.WriteError(
                jsonWriter,
                enumerable => { },
                odataError,
                includeDebugInformation: true,
                new ODataMessageWriterSettings { MessageQuotas = new ODataMessageQuotas { MaxNestingDepth = 3 } });

            // Assert
            Assert.Equal(expectedJsonString, stringWriter.ToString());
            Assert.Equal(expectedJsonString, jsonString);
        }

        [Theory]
        [MemberData(nameof(GetODataErrorTestData))]
        public async Task TestODataErrorToStringAsync(ODataError odataError, string expectedJsonString)
        {
            // Arrange
            var stringWriter = new StringWriter();
            var jsonWriter = new JsonWriter(stringWriter, isIeee754Compatible: false);

            // Act
            var jsonString = odataError.ToString();
            await ODataJsonWriterUtils.WriteErrorAsync(
                jsonWriter,
                enumerable => { return Task.CompletedTask; },
                odataError,
                includeDebugInformation: true,
                new ODataMessageWriterSettings { MessageQuotas = new ODataMessageQuotas { MaxNestingDepth = 3 } });

            // Assert
            Assert.Equal(expectedJsonString, stringWriter.ToString());
            Assert.Equal(expectedJsonString, jsonString);
        }
    }
}
