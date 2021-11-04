//---------------------------------------------------------------------
// <copyright file="ODataJsonLightDeserializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.OData.Json;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Edm;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataJsonLightDeserializerTests
    {
        private IEdmModel edmModel;
        private ODataMessageReaderSettings messageReaderSettings;

        public ODataJsonLightDeserializerTests()
        {
            this.edmModel = new EdmModel();
            this.messageReaderSettings = new ODataMessageReaderSettings();
        }

        [Fact]
        public void ParsingNormalPropertyShouldReturnItsName()
        {
            this.RunPropertyParsingTest("{\"property\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingNormalString_MaxMinChar()
        {
            // test reading Char.Max \uffff.
            // TODO: json reader don't support reading Char.Min \0 yet.
            this.RunPropertyParsingTest("{\"property\":\"N\\uffff\"}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, "N\uffff");
                    Assert.Equal(2, (jsonReader.Value as string).Length);
                });
        }

        [Fact]
        public void ParsingCustomPropertyAnnotationShouldBeIgnored()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", "custom.type") + "\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.EndObject, null);
                    Assert.Empty(duplicateChecker.GetODataPropertyAnnotations("property"));
                });
        }

        [Fact]
        public void ParsingODataPropertyAnnotationWithNothingAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.EndObject, null);
                    var propertyAnnotation = duplicateChecker.GetODataPropertyAnnotations("property");
                    Assert.Equal(new Dictionary<string, object> { { ODataAnnotationNames.ODataType, "typename" } }, propertyAnnotation);
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingODataPropertyAnnotationWithItsProperty()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename\",\"property\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                    var propertyAnnotation = duplicateChecker.GetODataPropertyAnnotations("property");
                    Assert.Equal(new Dictionary<string, object> { { ODataAnnotationNames.ODataType, "typename" } }, propertyAnnotation);
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingODataPropertyAnnotationWithDifferentPropertyAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename\",\"otherproperty\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.Property, "otherproperty");
                    var propertyAnnotation = duplicateChecker.GetODataPropertyAnnotations("property");
                    Assert.Equal(new Dictionary<string, object> { { ODataAnnotationNames.ODataType, "typename" } }, propertyAnnotation);
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingODataPropertyAnnotationWithInsanceAnnotationAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename\",\"" + JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType + "\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.Property, JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType);
                    var propertyAnnotation = duplicateChecker.GetODataPropertyAnnotations("property");
                    Assert.Equal(new Dictionary<string, object> { { ODataAnnotationNames.ODataType, "typename" } }, propertyAnnotation);
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingODataPropertyAnnotationWithDifferentPropertyAnnotationAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename\",\"" + JsonLightUtils.GetPropertyAnnotationName("otherproperty", ODataAnnotationNames.ODataType) + "\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.Property, JsonLightUtils.GetPropertyAnnotationName("otherproperty", ODataAnnotationNames.ODataType));
                    var propertyAnnotation = duplicateChecker.GetODataPropertyAnnotations("property");
                    Assert.Equal(new Dictionary<string, object> { { ODataAnnotationNames.ODataType, "typename" } }, propertyAnnotation);
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingCustomPropertyAnnotationWithItsPropertyAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", "foo.bar") + "\":\"foo\",\"property\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                    Assert.Empty(duplicateChecker.GetODataPropertyAnnotations("property"));
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingCustomPropertyAnnotationWithDifferentPropertyAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", "foo.bar") + "\":\"foo\",\"otherproperty\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.Property, "otherproperty");
                    Assert.Empty(duplicateChecker.GetODataPropertyAnnotations("property"));
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingCustomPropertyAnnotationWithInsanceAnnotationAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", "foo.bar") + "\":\"foo\",\"" + JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType + "\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.Property, JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType);
                    Assert.Empty(duplicateChecker.GetODataPropertyAnnotations("property"));
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingCustomPropertyAnnotationWithDifferentPropertyAnnotationAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", "foo.bar") + "\":\"foo\",\"" + JsonLightUtils.GetPropertyAnnotationName("otherproperty", ODataAnnotationNames.ODataType) + "\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.Property, JsonLightUtils.GetPropertyAnnotationName("otherproperty", ODataAnnotationNames.ODataType));
                    Assert.Empty(duplicateChecker.GetODataPropertyAnnotations("property"));
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingODataPropertyAnnotationWithMetadataReferencePropertyAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename\",\"#namespace.name\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.Property, "#namespace.name");
                    var propertyAnnotation = duplicateChecker.GetODataPropertyAnnotations("property");
                    Assert.Equal(new Dictionary<string, object> { { ODataAnnotationNames.ODataType, "typename" } }, propertyAnnotation);
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingCustomPropertyAnnotationWithMetadataReferencePropertyAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", "custom.annotation") + "\":\"foo\",\"#namespace.name\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.Property, "#namespace.name");
                    Assert.Empty(duplicateChecker.GetODataPropertyAnnotations("property"));
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingDuplicateODataPropertyAnnotationShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename\",\"" + JsonLightUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename2\"}", ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject, null,
                null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.DuplicateAnnotationForPropertyNotAllowed(ODataAnnotationNames.ODataType, "property"));
        }

        [Fact]
        public void ParsingDuplicateCustomPropertyAnnotationShouldNotFail()
        {
            Action action = () => this.RunPropertyParsingTest(
                "{\"" + JsonLightUtils.GetPropertyAnnotationName("property", "custom.type") + "\":\"typename\",\"" + JsonLightUtils.GetPropertyAnnotationName("property", "custom.type") + "\":\"typename2\"}",
                ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property", null, this.ReadODataTypePropertyAnnotation);
            action.DoesNotThrow();
        }

        #region Instance Annotation tests

        [Fact]
        public void ParsingODataInstanceAnnotationShouldReturnItsName()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType + "\":42}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, ODataAnnotationNames.ODataType,
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingCustomInstanceAnnotationShouldReturnItsName()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("custom.*");
            this.RunPropertyParsingTest("{\"@custom.type\":42}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingODataInstanceAnnotationWithDifferentPropertyAfterItShouldReturnItsName()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType + "\":42,\"foo\":42}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, ODataAnnotationNames.ODataType,
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingCustomInstanceAnnotationWithDifferentPropertyAfterItShouldReturnItsName()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("custom.type");
            this.RunPropertyParsingTest("{\"@custom.type\":42,\"foo\":42}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingODataInstanceAnnotationWithDifferentPropertyAnnotationAfterItShouldReturnItsName()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType + "\":42,\"foo@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, ODataAnnotationNames.ODataType,
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingCustomInstanceAnnotationWithDifferentPropertyAnnotationAfterItShouldReturnItsName()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            this.RunPropertyParsingTest("{\"@custom.type\":42,\"foo@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingCustomInstanceAnnotationBeforePropertyAnnotationWithoutOverridingDefaultAnnotationFilterShouldSkipCustomInstanceAnnotation()
        {
            this.RunPropertyParsingTest("{\"@custom.type\":42,\"foo@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "foo",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.EndObject, null);
                });
        }

        [Fact]
        public void ParsingODataInstanceAnnotationWithDifferentInstanceAnnotationAfterItShouldReturnItsName()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType + "\":42,\"@custom.instance\":42}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, ODataAnnotationNames.ODataType,
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingCustomInstanceAnnotationWithDifferentInstanceAnnotationAfterItShouldReturnItsName()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            this.RunPropertyParsingTest("{\"@custom.type\":42,\"@odata.unknown\":42}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingCustomInstanceAnnotationWithUnknownODataAnnotationAfterItShouldSkipBothAnotationsForExcludeFilter()
        {
            this.RunPropertyParsingTest("{\"@custom.type\":42,\"@odata.unknown\":42}", ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject, null,
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.EndObject, null);
                });
        }

        [Fact]
        public void ParsingODataInstanceAnnotationWithDifferentAnnotationTargetingAnnotationAfterItShouldReturnItsName()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType + "\":42,\"custom.instance@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, ODataAnnotationNames.ODataType,
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingCustomInstanceAnnotationWithDifferentAnnotationTargetingAnnotationAfterItShouldReturnItsName()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            this.RunPropertyParsingTest("{\"@custom.type\":42,\"odata.unknown@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingODataTypeWithoutTheTargetingInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.deltaLink@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "odata.deltaLink", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "odata.deltaLink"));
        }

        [Fact]
        public void ParsingODataTypeWithoutTheTargetingCustomInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "custom.instance", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "custom.instance"));
        }

        [Fact]
        public void ParsingODataTypeTargetingODataInstanceAnnotationAfterItShouldReturnItsName()
        {
            this.RunPropertyParsingTest("{\"odata.deltaLink@odata.type\":\"#typename\", \"@odata.deltaLink\":42}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, "odata.deltaLink",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                    var propertyAnnotation = duplicateChecker.GetODataPropertyAnnotations("odata.deltaLink");
                    Assert.Equal(new Dictionary<string, object> { { ODataAnnotationNames.ODataType, "#typename" } }, propertyAnnotation);
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingODataTypeTargetingCustomInstanceAnnotationAfterItShouldReturnItsName()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("custom.*");
            this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\",\"@custom.instance\":42}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.instance",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                    var propertyAnnotation = duplicateChecker.GetODataPropertyAnnotations("custom.instance");
                    Assert.Equal(new Dictionary<string, object> { { ODataAnnotationNames.ODataType, "#typename" } }, propertyAnnotation);
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingWithoutOverridingDefaultAnnotationFilterShouldSkipOverAllCustomAnnotations()
        {
            this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\",\"@custom.instance\":42}", ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject, null,
                (jsonReader, duplicateChecker) => jsonReader.ShouldBeOn(JsonNodeType.EndObject, null),
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingODataAnnotationTargetingODataInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.deltaLink@odata.unknown\":42,\"@odata.deltaLink\":42}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, "odata.deltaLink");
            action.Throws<ODataException>(ErrorStrings.ODataJsonLightDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation("odata.unknown", "odata.deltaLink", "odata.type"));
        }

        [Fact]
        public void ParsingCustomAnnotationTargetingODataInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.deltaLink@custom.annotation\":42,\"@odata.deltaLink\":42}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, "odata.deltaLink");
            action.Throws<ODataException>(ErrorStrings.ODataJsonLightDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation("custom.annotation", "odata.deltaLink", "odata.type"));
        }

        [Fact]
        public void ParsingODataAnnotationTargetingCustomInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@odata.unknown\":42,\"@custom.instance\":42}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.instance");
            action.Throws<ODataException>(ErrorStrings.ODataJsonLightDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation("odata.unknown", "custom.instance", "odata.type"));
        }

        [Fact]
        public void ParsingCustomAnnotationTargetingCustomInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@custom.annotation\":42,\"@custom.instance\":42}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.instance");
            action.Throws<ODataException>(ErrorStrings.ODataJsonLightDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation("custom.annotation", "custom.instance", "odata.type"));
        }

        [Fact]
        public void ParsingODataTypeTargetingODataInstanceAnnotationWithDifferentPropertyAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.deltaLink@odata.type\":\"#typename\",\"foo\":42}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, ODataAnnotationNames.ODataType, null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "odata.deltaLink"));
        }

        [Fact]
        public void ParsingODataTypeTargetingCustomInstanceAnnotationWithDifferentPropertyAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\",\"foo\":42}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.instance", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "custom.instance"));
        }

        [Fact]
        public void ParsingODataTypeTargetingODataInstanceAnnotationWithDifferentPropertyAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.unknown@odata.type\":\"#typename\",\"foo@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, "odata.unknown", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "odata.unknown"));
        }

        [Fact]
        public void ParsingODataTypeTargetingCustomInstanceAnnotationWithDifferentPropertyAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\",\"foo@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.instance", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "custom.instance"));
        }

        [Fact]
        public void ParsingODataTypeTargetingODataInstanceAnnotationWithDifferentInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.unknown@odata.type\":\"#typename\",\"@custom.instance\":42}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, "odata.unknown", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "odata.unknown"));
        }

        [Fact]
        public void ParsingODataTypeTargetingCustomInstanceAnnotationWithDifferentInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\",\"@odata.unknown\":42}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.instance", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "custom.instance"));
        }

        [Fact]
        public void ParsingODataTypeTargetingODataInstanceAnnotationWithDifferentAnnotationTargetingAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.unknown@odata.type\":\"#typename\",\"custom.instance@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, "odata.unknown", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "odata.unknown"));
        }

        [Fact]
        public void ParsingODataTypeTargetingCustomInstanceAnnotationWithDifferentAnnotationTargetingAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\",\"odata.unknown@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.instance", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "custom.instance"));
        }

        [Fact]
        public void ParsingODataTypeTargetingODataInstanceAnnotationWithMetadataReferencePropertyAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.unknown@odata.type\":\"#typename\",\"@namespace.name\":42}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, "odata.unknown", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "odata.unknown"));
        }

        [Fact]
        public void ParsingODataTypeTargetingCustomInstanceAnnotationWithMetadataReferencePropertyAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\",\"@namespace.name\":42}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.instance", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "custom.instance"));
        }

        [Fact]
        public void ParsingDuplicateODataInstanceAnnotationShouldFail()
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            Action action = () => this.RunPropertyParsingTest("{\"@odata.deltaLink\":\"url\",\"@odata.deltaLink\":\"url\"}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, "odata.deltaLink",
                null, this.ReadODataTypePropertyAnnotation, propertyAndAnnotationCollector);
            action();
            action.Throws<ODataException>(ErrorStrings.DuplicateAnnotationNotAllowed("odata.deltaLink"));
        }

        [Fact]
        public void ParsingDuplicateCustomInstanceAnnotationShouldNotFail1()
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            Action action = () => this.RunPropertyParsingTest("{\"@custom.type\":\"typename\",\"@custom.type\":\"typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject,
                null, null, this.ReadODataTypePropertyAnnotation, propertyAndAnnotationCollector);
            action.DoesNotThrow();
        }

        [Fact]
        public void ParsingDuplicateCustomInstanceAnnotationShouldNotFail2()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            Action action = () => this.RunPropertyParsingTest("{\"@custom.type\":\"typename\",\"@custom.type\":\"typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                null, this.ReadODataTypePropertyAnnotation, propertyAndAnnotationCollector);
            action();
            action.DoesNotThrow();
        }

        [Fact]
        public void ParsingDuplicateODataTypeAnnotationTargetingODataInstanceAnnotationShouldFail()
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            Action action = () => this.RunPropertyParsingTest("{\"odata.annotation@odata.type\":\"#typename\",\"odata.annotation@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                null, this.ReadODataTypePropertyAnnotation, propertyAndAnnotationCollector);
            action.Throws<ODataException>(ErrorStrings.DuplicateAnnotationForInstanceAnnotationNotAllowed("odata.type", "odata.annotation"));
        }

        [Fact]
        public void ParsingDuplicateODataTypeAnnotationTargetingCustomInstanceAnnotationShouldFail()
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            Action action = () => this.RunPropertyParsingTest("{\"custom.annotation@odata.type\":\"#typename\",\"custom.annotation@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                null, this.ReadODataTypePropertyAnnotation, propertyAndAnnotationCollector);
            action.Throws<ODataException>(ErrorStrings.DuplicateAnnotationForInstanceAnnotationNotAllowed("odata.type", "custom.annotation"));
        }

        #endregion Instance Annotation tests

        #region MetadataReferenceProperty tests
        [Fact]
        public void ParsingPropertyWithHashAtBeginningShouldReturnMetadataReferenceProperty()
        {
            this.RunPropertyParsingTest(
                "{\"#myproperty\":42}",
                ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty,
                "#myproperty",
                (jsonReader, duplicateChecker) => jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42));
        }

        [Fact]
        public void ParsingPropertyWithHashInMiddleShouldReturnMetadataReferenceProperty()
        {
            this.RunPropertyParsingTest(
                "{\"http://odata.org/$metadata#myaction\":42}",
                ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty,
                "http://odata.org/$metadata#myaction",
                (jsonReader, duplicateChecker) => jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42));
        }

        [Fact]
        public void ParsingPropertyWithUriEscapedHashShouldNotBeMetadataReferenceProperty()
        {
            this.RunPropertyParsingTest(
                "{\"%23action\":42}",
                ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue,
                "%23action",
                (jsonReader, duplicateChecker) => jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42));
        }

        [Fact]
        public void RelativeMetadataReferencePropertyShouldThrow()
        {
            this.VerifyInvalidMetadataReferenceProperty("Odata.annotation#");
        }

        [Fact]
        public void MetadataReferencePropertyWithHashOnlyShouldThrow()
        {
            this.VerifyInvalidMetadataReferenceProperty("#");
        }

        [Fact]
        public void MetadataReferencePropertyWhichIsInvalidUriFragmentShouldNotThrow()
        {
            this.RunPropertyParsingTest(
                "{\"#CaratIs^InvalidPerUriSpec\":42}",
                ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty,
                "#CaratIs^InvalidPerUriSpec",
                (jsonReader, duplicateChecker) => jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42));
        }

        [Fact]
        public void MetadataReferencePropertyWithTwoHashesShouldNotThrow()
        {
            this.RunPropertyParsingTest(
                "{\"#TwoHashes#Allowed\":42}",
                ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty,
                "#TwoHashes#Allowed",
                (jsonReader, duplicateChecker) => jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42));
        }

        [Fact]
        public void ParsePropertyShouldSkipUnknownODataAnnotations()
        {
            this.RunPropertyParsingTest(
                "{\"@odata.unknown\":42}",
                ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject,
                null,
                (jsonReader, duplicateChecker) => jsonReader.ShouldBeOn(JsonNodeType.EndObject, null));

            this.RunPropertyParsingTest(
                "{\"@odata.unknown1\":42,\"@odata.unknown2\":42,\"@odata.unknown3\":42}",
                ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject,
                null,
                (jsonReader, duplicateChecker) => jsonReader.ShouldBeOn(JsonNodeType.EndObject, null));
        }

        [Fact]
        public void ParsePropertyShouldTreatUnknownODataAnnotationsAsABoundary()
        {
            this.RunPropertyParsingTest(
                "{\"NavProp@odata.navigationLink\":\"http://someUrl\",\"@odata.unknown\":42,\"NavProp@odata.associationLink\":\"http://someUrl\"}",
                ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue,
                "NavProp",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.Property, "@odata.unknown");
                    var propertyAnnotation = duplicateChecker.GetODataPropertyAnnotations("NavProp").Keys;
                    Assert.Contains("odata.navigationLink", propertyAnnotation);
                    Assert.Equal(1, propertyAnnotation.Count);
                },
                (jsonReader, name) =>
                {
                    Assert.Equal("odata.navigationLink", name);
                    return jsonReader.ReadStringValue(name);
                });
        }

        [Fact]
        public void ParsePropertyShouldSkipUnknownODataPropertyAnnotations1()
        {
            this.RunPropertyParsingTest(
                "{\"property@odata.unknown\":42}",
                ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject,
                null,
                (jsonReader, duplicateChecker) => jsonReader.ShouldBeOn(JsonNodeType.EndObject, null));

            this.RunPropertyParsingTest(
                "{\"property@odata.unknown1\":42,\"property@odata.unknown2\":42,\"property@odata.unknown3\":42}",
                ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject,
                null,
                (jsonReader, duplicateChecker) => jsonReader.ShouldBeOn(JsonNodeType.EndObject, null));
        }

        [Fact]
        public void ParsePropertyShouldSkipUnknownODataPropertyAnnotations2()
        {
            this.RunPropertyParsingTest(
                "{\"NavProp@odata.navigationLink\":\"http://someUrl\",\"NavProp@odata.unknown\":42,\"NavProp@odata.associationLink\":\"http://someUrl\"}",
                ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue,
                "NavProp",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.EndObject, null);
                    var propertyAnnotation = duplicateChecker.GetODataPropertyAnnotations("NavProp").Keys;
                    Assert.Contains("odata.navigationLink", propertyAnnotation);
                    Assert.Contains("odata.associationLink", propertyAnnotation);
                    Assert.Contains("odata.unknown", propertyAnnotation);
                    Assert.Equal(3, propertyAnnotation.Count);
                },
                (jsonReader, name) => jsonReader.ReadStringValue(name));
        }

        [Fact]
        public void MetadataReferencePropertyWithInvalidAbsoluteUriShouldThrow()
        {
            this.VerifyInvalidMetadataReferenceProperty("http:://bla.com#InvalidAbsoluteUri");
        }

        [Fact]
        public void TwoMetadataReferencePropertiesShouldResultInDuplicationException()
        {
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            this.AssertDuplicateMetadataReferencePropertyFails(propertyAndAnnotationCollector);
        }

        [Fact]
        public void TwoMetadataReferencePropertiesShouldStillResultInDuplicationExceptionIfAllowingDuplicates()
        {
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(false);
            this.AssertDuplicateMetadataReferencePropertyFails(propertyAndAnnotationCollector);
        }

        private void AssertDuplicateMetadataReferencePropertyFails(PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            string payload = "{\"#action\":42, \"#action\":43}";

            using (ODataJsonLightInputContext inputContext = this.CreateJsonLightInputContext(payload))
            {
                ODataJsonLightDeserializer deserializer = new ODataJsonLightPropertyAndValueDeserializer(inputContext);
                AdvanceReaderToFirstProperty(deserializer.JsonReader);

                deserializer.ProcessProperty(
                    propertyAndAnnotationCollector,
                    (propertyName) => null,
                    (propertyParsingResult, propertyName) =>
                    {
                        if (deserializer.JsonReader.NodeType == JsonNodeType.Property)
                        {
                            // Read over property name
                            deserializer.JsonReader.Read();
                        }

                        Assert.Equal(ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty, propertyParsingResult);
                        Assert.Equal("#action", propertyName);

                        deserializer.JsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);

                        deserializer.JsonReader.Read();
                        Assert.Equal(JsonNodeType.Property, deserializer.JsonReader.NodeType);
                    });

                Action readDuplicateProperty = () => deserializer.ProcessProperty(
                    propertyAndAnnotationCollector,
                    (propertyName) => null,
                    (propertyParsingResult, propertyName) => { });

                readDuplicateProperty.Throws<ODataException>(ErrorStrings.DuplicatePropertyNamesNotAllowed("#action"));
            }
        }

        private void VerifyInvalidMetadataReferenceProperty(string propertyName)
        {
            string jsonInput = string.Format("{{\"" + JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataContext + "\":\"http://odata.org/$metadata\"," + "\"{0}\":42}}", propertyName);
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);

            using (ODataJsonLightInputContext inputContext = this.CreateJsonLightInputContext(jsonInput))
            {
                ODataJsonLightResourceDeserializer deserializer = new ODataJsonLightResourceDeserializer(inputContext);
                deserializer.ReadPayloadStart(ODataPayloadKind.Unsupported, propertyAndAnnotationCollector, false, false);

                Action readEntryContentAction = () => deserializer.ReadResourceContent(new TestJsonLightReaderEntryState());

                var exception = Assert.Throws<ODataException>(readEntryContentAction);
                Assert.Equal(ErrorStrings.ValidationUtils_InvalidMetadataReferenceProperty(propertyName), exception.Message);
            }
        }

        private static void AdvanceReaderToFirstProperty(BufferingJsonReader bufferingJsonReader)
        {
            // Read start and then over the object start.
            bufferingJsonReader.Read();
            bufferingJsonReader.Read();
            Assert.Equal(JsonNodeType.Property, bufferingJsonReader.NodeType);
        }
        #endregion MetadataReferenceProperty tests

        [Fact]
        public void ParsingTypeDefinitionValueOfIncompatibleTypeShouldFail()
        {
            var model = new EdmModel();
            var uint32 = model.GetUInt32("NS", true);
            ODataJsonLightPropertyAndValueDeserializer deserializer = new ODataJsonLightPropertyAndValueDeserializer(this.CreateJsonLightInputContext("\"123\"", model));
            deserializer.JsonReader.Read();
            Action action = () => deserializer.ReadNonEntityValue(
                /*payloadTypeName*/ null,
                uint32,
                /*propertyAndAnnotationCollector*/ null,
                /*collectionValidator*/ null,
                /*validateNullValue*/ true,
                /*isTopLevelPropertyValue*/ true,
                /*insideResourceValue*/ false,
                /*propertyName*/ null);
            action.Throws<ODataException>(ErrorStrings.ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter("Edm.Int64"));
        }

        [Fact]
        public void ParsingTypeDefinitionOverflowValueShouldFail()
        {
            var model = new EdmModel();
            var uint16 = model.GetUInt16("NS", true);
            ODataJsonLightPropertyAndValueDeserializer deserializer = new ODataJsonLightPropertyAndValueDeserializer(this.CreateJsonLightInputContext("123456789", model));
            deserializer.JsonReader.Read();
            Action action = () => deserializer.ReadNonEntityValue(
                /*payloadTypeName*/ null,
                uint16,
                /*propertyAndAnnotationCollector*/ null,
                /*collectionValidator*/ null,
                /*validateNullValue*/ true,
                /*isTopLevelPropertyValue*/ true,
                /*insideResourceValue*/ false,
                /*propertyName*/ null);
            action.Throws<ODataException>("Value '123456789' was either too large or too small for a 'NS.UInt16'.");
        }

        #region Top level property

        [Fact]
        public void TopLevelPropertyShouldReadProperty()
        {
            var model = this.CreateEdmModelWithEntity();
            var primitiveTypeRef = ((IEdmEntityType)model.SchemaElements.First()).Properties().First().Type;
            this.messageReaderSettings = new ODataMessageReaderSettings();
            ODataJsonLightPropertyAndValueDeserializer deserializer = new ODataJsonLightPropertyAndValueDeserializer(this.CreateJsonLightInputContext("{\"@odata.context\":\"http://odata.org/test/$metadata#Customers(1)/Name\",\"value\":\"Joe\"}", model));
            ODataProperty property = deserializer.ReadTopLevelProperty(primitiveTypeRef);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue("Joe"), property.ODataValue);
        }

        [Fact]
        public void TopLevelPropertyShouldReadNullProperty()
        {
            var model = this.CreateEdmModelWithEntity();
            var primitiveTypeRef = ((IEdmEntityType)model.SchemaElements.First()).Properties().First().Type;
            this.messageReaderSettings = new ODataMessageReaderSettings();
            ODataJsonLightPropertyAndValueDeserializer deserializer = new ODataJsonLightPropertyAndValueDeserializer(this.CreateJsonLightInputContext("{\"@odata.context\":\"http://odata.org/test/$metadata#Customers(1)/Name\",\"value\":null}", model));
            ODataProperty property = deserializer.ReadTopLevelProperty(primitiveTypeRef);
            TestUtils.AssertODataValueAreEqual(new ODataNullValue(), property.ODataValue);
        }

        [Fact]
        public void TopLevelPropertyShouldRead6xNullProperty()
        {
            var model = this.CreateEdmModelWithEntity();
            var primitiveTypeRef = ((IEdmEntityType)model.SchemaElements.First()).Properties().First().Type;
            this.messageReaderSettings = new ODataMessageReaderSettings();
            ODataJsonLightPropertyAndValueDeserializer deserializer = new ODataJsonLightPropertyAndValueDeserializer(this.CreateJsonLightInputContext("{\"@odata.context\":\"http://odata.org/test/$metadata#Customers(1)/Name\",\"@odata.null\":true}", model));
            ODataProperty property = deserializer.ReadTopLevelProperty(primitiveTypeRef);
            TestUtils.AssertODataValueAreEqual(new ODataNullValue(), property.ODataValue);
        }

        [Fact]
        public void ReadPayloadStartAsyncTestForDetla()
        {
            var model = this.CreateEdmModelWithEntity();
            var primitiveTypeRef = ((IEdmEntityType)model.SchemaElements.First()).Properties().First().Type;
            this.messageReaderSettings = new ODataMessageReaderSettings();
            ODataJsonLightPropertyAndValueDeserializer deserializer = new ODataJsonLightPropertyAndValueDeserializer(this.CreateJsonLightInputContext("{\"@odata.context\":\"http://odata.org/test/$metadata#Customers/$delta\",\"value\":\"Joe\"}", model));
            deserializer.ReadPayloadStartAsync(ODataPayloadKind.Delta, null, false, true);
            Assert.Equal(ODataDeltaKind.ResourceSet, deserializer.ContextUriParseResult.DeltaKind);
        }

        public static IEnumerable<object[]> PrimitiveData => new List<object[]>
        {
            new object[] { 42,                     "\"@odata.type\":\"#Int32\",\"value\":42" },
            new object[] { new Date(2018, 11, 28), "\"@odata.type\":\"#Date\",\"value\":\"2018-11-28\"" },
            new object[] { 8.9,                    "\"@odata.type\":\"#Double\",\"value\":8.9" },
            new object[] { true,                   "\"@odata.type\":\"#Boolean\",\"value\":true" }
        };

        [Theory]
        [MemberData(nameof(PrimitiveData))]
        public void TopLevelPropertyShouldReadEdmPrimitiveTypeProperty(object value, string valueString)
        {
            string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#Customers(1)/PrimitiveProperty\"," + valueString + "}";
            var model = this.CreateEdmModelWithEntity();
            EdmEntityType entityType = model.SchemaElements.First() as EdmEntityType;
            var edmProperty = entityType.AddStructuralProperty("PrimitiveProperty", EdmPrimitiveTypeKind.PrimitiveType);
            var primitiveTypeRef = edmProperty.Type;

            this.messageReaderSettings = new ODataMessageReaderSettings();
            ODataJsonLightPropertyAndValueDeserializer deserializer = new ODataJsonLightPropertyAndValueDeserializer(this.CreateJsonLightInputContext(payload, model));
            ODataProperty property = deserializer.ReadTopLevelProperty(primitiveTypeRef);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(value), property.ODataValue);
        }

        #endregion

        #region Top level property instance annotation

        [Fact]
        public void ParsingInstanceAnnotationInTopLevelPropertyShouldThrowOnReservedODataAnnotationNamesNotApplicableToProperties()
        {
            var model = this.CreateEdmModelWithEntity();
            var primitiveTypeRef = ((IEdmEntityType)model.SchemaElements.First()).Properties().First().Type;
            this.messageReaderSettings = new ODataMessageReaderSettings { ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };
            ODataJsonLightPropertyAndValueDeserializer deserializer = new ODataJsonLightPropertyAndValueDeserializer(this.CreateJsonLightInputContext("{\"@odata.context\":\"http://odata.org/test/$metadata#Customers(1)/Name\",\"@odata.count\":123,\"value\":\"Joe\"}", model));
            Action action = () => deserializer.ReadTopLevelProperty(primitiveTypeRef);
            action.Throws<ODataException>(ErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.count"));
        }

        [Fact]
        public void ParsingInstanceAnnotationInTopLevelPropertyShouldReadProperty()
        {
            var model = this.CreateEdmModelWithEntity();
            var primitiveTypeRef = ((IEdmEntityType)model.SchemaElements.First()).Properties().First().Type;
            this.messageReaderSettings = new ODataMessageReaderSettings { ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };
            ODataJsonLightPropertyAndValueDeserializer deserializer = new ODataJsonLightPropertyAndValueDeserializer(this.CreateJsonLightInputContext("{\"@odata.context\":\"http://odata.org/test/$metadata#Customers(1)/Name\",\"@Is.ReadOnly\":true,\"value\":\"Joe\"}", model));
            ODataProperty property = deserializer.ReadTopLevelProperty(primitiveTypeRef);
            Assert.Single(property.InstanceAnnotations);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(true), property.InstanceAnnotations.Single(ia => ia.Name == "Is.ReadOnly").Value);
        }

        [Fact]
        public void ParsingInstanceAnnotationsInTopLevelPropertyShouldReadProperty()
        {
            var model = this.CreateEdmModelWithEntity();
            var primitiveTypeRef = ((IEdmEntityType)model.SchemaElements.First()).Properties().First().Type;
            this.messageReaderSettings = new ODataMessageReaderSettings { ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };
            ODataJsonLightPropertyAndValueDeserializer deserializer = new ODataJsonLightPropertyAndValueDeserializer(this.CreateJsonLightInputContext("{\"@odata.context\":\"http://odata.org/test/$metadata#Customers(1)/Name\",\"@Annotation.1\":true,\"@Annotation.2\":123,\"@Annotation.3\":\"annotation\",\"value\":\"Joe\"}", model));
            ODataProperty property = deserializer.ReadTopLevelProperty(primitiveTypeRef);
            Assert.Equal(3, property.InstanceAnnotations.Count);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(true), property.InstanceAnnotations.Single(ia => ia.Name == "Annotation.1").Value);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(123), property.InstanceAnnotations.Single(ia => ia.Name == "Annotation.2").Value);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue("annotation"), property.InstanceAnnotations.Single(ia => ia.Name == "Annotation.3").Value);
        }

        [Fact]
        public void ParsingInstanceAnnotationsInTopLevelPropertyShouldSkipBaseOnSettings()
        {
            var model = this.CreateEdmModelWithEntity();
            var primitiveTypeRef = ((IEdmEntityType)model.SchemaElements.First()).Properties().First().Type;
            ODataJsonLightPropertyAndValueDeserializer deserializer = new ODataJsonLightPropertyAndValueDeserializer(this.CreateJsonLightInputContext("{\"@odata.context\":\"http://odata.org/test/$metadata#Customers(1)/Name\",\"@Annotation.1\":true,\"@Annotation.2\":123,\"@Annotation.3\":\"annotation\",\"value\":\"Joe\"}", model));
            ODataProperty property = deserializer.ReadTopLevelProperty(primitiveTypeRef);
            Assert.Empty(property.InstanceAnnotations);
        }

        #endregion

        #region Complex properties instance annotation

        [Fact]
        public void ParsingInstanceAnnotationInComplexPropertyShouldThrowOnReservedODataAnnotationNamesNotApplicableToProperties()
        {
            var model = new EdmModel();
            var complexType = new EdmComplexType("TestNamespace", "Address");
            complexType.AddStructuralProperty("CountryRegion", EdmPrimitiveTypeKind.String);
            model.AddElement(complexType);
            var complexTypeRef = new EdmComplexTypeReference(complexType, false);
            this.messageReaderSettings = new ODataMessageReaderSettings { ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };
            var odataReader = this.CreateJsonLightInputContext("{\"CountryRegion@odata.count\":123,\"CountryRegion\":\"China\"}", model, false)
                .CreateResourceReader(null, complexType);
            Action action = () =>
            {
                while (odataReader.Read())
                {
                    if (odataReader.State == ODataReaderState.ResourceEnd)
                    {
                        var resource = odataReader.Item as ODataResource;
                        Assert.Equal(0, resource.Properties.First().InstanceAnnotations.Count);
                    }
                }
            };
        }

        [Fact]
        public void ParsingInstanceAnnotationInComplexPropertyShouldReadComplexValue()
        {
            var model = new EdmModel();
            var complexType = new EdmComplexType("TestNamespace", "Address");
            complexType.AddStructuralProperty("CountryRegion", EdmPrimitiveTypeKind.String);
            model.AddElement(complexType);
            var complexTypeRef = new EdmComplexTypeReference(complexType, false);
            this.messageReaderSettings = new ODataMessageReaderSettings { ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };
            var odataReader = this.CreateJsonLightInputContext("{\"CountryRegion@Is.ReadOnly\":true,\"CountryRegion\":\"China\"}", model, false)
                .CreateResourceReader(null, complexType);
            while(odataReader.Read())
            {
                if (odataReader.State == ODataReaderState.ResourceEnd)
                {
                    var complex = odataReader.Item as ODataResource;
                    Assert.Single(complex.Properties.First().InstanceAnnotations);
                    TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(true), complex.Properties.First().InstanceAnnotations.Single(ia => ia.Name == "Is.ReadOnly").Value);
                }
            }
        }

        [Fact]
        public void ParsingInstanceAnnotationsInComplexPropertyShouldReadComplexValue()
        {
            var model = new EdmModel();
            var complexType = new EdmComplexType("TestNamespace", "Address");
            complexType.AddStructuralProperty("CountryRegion", EdmPrimitiveTypeKind.String);
            model.AddElement(complexType);
            var complexTypeRef = new EdmComplexTypeReference(complexType, false);
            this.messageReaderSettings = new ODataMessageReaderSettings { ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };
            var odataReader = this.CreateJsonLightInputContext("{\"CountryRegion@Annotation.1\":true,\"CountryRegion@Annotation.2\":123,\"CountryRegion@Annotation.3\":\"annotation\",\"CountryRegion\":\"China\"}", model, false)
                .CreateResourceReader(null, complexType);
            while (odataReader.Read())
            {
                if (odataReader.State == ODataReaderState.ResourceEnd)
                {
                    var complex = odataReader.Item as ODataResource;
                    Assert.Equal(3, complex.Properties.First().InstanceAnnotations.Count);
                    TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(true), complex.Properties.First().InstanceAnnotations.Single(ia => ia.Name == "Annotation.1").Value);
                    TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(123), complex.Properties.First().InstanceAnnotations.Single(ia => ia.Name == "Annotation.2").Value);
                    TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue("annotation"), complex.Properties.First().InstanceAnnotations.Single(ia => ia.Name == "Annotation.3").Value);
                }
            }
        }

        [Fact]
        public void ParsingInstanceAnnotationsInComplexPropertyShouldSkipBaseOnSettings()
        {
            var model = new EdmModel();
            var complexType = new EdmComplexType("TestNamespace", "Address");
            complexType.AddStructuralProperty("CountryRegion", EdmPrimitiveTypeKind.String);
            model.AddElement(complexType);
            var complexTypeRef = new EdmComplexTypeReference(complexType, false);
            var odataReader = this.CreateJsonLightInputContext("{\"CountryRegion@Is.ReadOnly\":true,\"CountryRegion\":\"China\"}", model, false)
                .CreateResourceReader(null, complexType);

            while (odataReader.Read())
            {
                if (odataReader.State == ODataReaderState.ResourceEnd)
                {
                    var complex = odataReader.Item as ODataResource;
                    Assert.Empty(complex.Properties.First().InstanceAnnotations);
                }
            }

        }

        #endregion

        #region Complex value instance annotation

        [Fact]
        public void ParsingInstanceAnnotationInComplexValueShouldThrowOnReservedODataAnnotationNamesNotApplicableToComplexValues()
        {
            var model = new EdmModel();
            var complexType = new EdmComplexType("TestNamespace", "Address");
            model.AddElement(complexType);
            var complexTypeRef = new EdmComplexTypeReference(complexType, false);
            this.messageReaderSettings = new ODataMessageReaderSettings { ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };
            var odataReader = this.CreateJsonLightInputContext("{\"@odata.count\":\"123\"}", model, false)
                .CreateResourceReader(null, complexType);
            Action action = () =>
            {
                while (odataReader.Read()) ;
            };

            action.Throws<ODataException>(ErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.count"));
        }

        [Fact]
        public void ParsingInstanceAnnotationInComplexValueShouldReadComplexValue()
        {
            var model = new EdmModel();
            var complexType = new EdmComplexType("TestNamespace", "Address");
            model.AddElement(complexType);
            var complexTypeRef = new EdmComplexTypeReference(complexType, false);
            this.messageReaderSettings = new ODataMessageReaderSettings { ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };

            var odataReader = this.CreateJsonLightInputContext("{\"@Is.ReadOnly\":true}", model, false)
                .CreateResourceReader(null, complexType);
            while (odataReader.Read())
            {
                if (odataReader.State == ODataReaderState.ResourceEnd)
                {
                    var complex = odataReader.Item as ODataResource;
                    Assert.Single(complex.InstanceAnnotations);
                }
            }
        }

        [Fact]
        public void ParsingInstanceAnnotationsInComplexValueShouldReadComplexValue()
        {
            var model = new EdmModel();
            var complexType = new EdmComplexType("TestNamespace", "Address");
            model.AddElement(complexType);
            var complexTypeRef = new EdmComplexTypeReference(complexType, false);
            this.messageReaderSettings = new ODataMessageReaderSettings { ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };
            var odataReader = this.CreateJsonLightInputContext("{\"@Annotation.1\":true,\"@Annotation.2\":123,\"Annotation.3\":\"annotation\"}", model, false)
                .CreateResourceReader(null, complexType);
            while (odataReader.Read())
            {
                if (odataReader.State == ODataReaderState.ResourceEnd)
                {
                    var complex = odataReader.Item as ODataResource;
                    Assert.Equal(3, complex.InstanceAnnotations.Count);
                    TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(true), complex.InstanceAnnotations.Single(ia => ia.Name == "Annotation.1").Value);
                    TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(123), complex.InstanceAnnotations.Single(ia => ia.Name == "Annotation.2").Value);
                    TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue("annotation"), complex.InstanceAnnotations.Single(ia => ia.Name == "Annotation.3").Value);
                }
            }
        }

        [Fact]
        public void ParsingInstanceAnnotationsInDisorderInComplexValueShouldPass()
        {
            var model = new EdmModel();
            var complexType = new EdmComplexType("TestNamespace", "Address");
            complexType.AddStructuralProperty("CountryRegion", EdmPrimitiveTypeKind.String);
            model.AddElement(complexType);
            var complexTypeRef = new EdmComplexTypeReference(complexType, false);
            this.messageReaderSettings = new ODataMessageReaderSettings { ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };
            var odataReader = this.CreateJsonLightInputContext("{\"@Annotation.1\":true,\"@Annotation.2\":123,\"CountryRegion\":\"China\",\"@Annotation.3\":\"annotation\"}", model, false)
                            .CreateResourceReader(null, complexType);
            Action action = () =>
            {
                while (odataReader.Read()) ;
            };
            action.DoesNotThrow();
        }

        [Fact]
        public void ParsingInstanceAnnotationsInComplexValueShouldSkipBaseOnSettings()
        {
            var model = new EdmModel();
            var complexType = new EdmComplexType("TestNamespace", "Address");
            model.AddElement(complexType);
            var complexTypeRef = new EdmComplexTypeReference(complexType, false);
            this.messageReaderSettings = new ODataMessageReaderSettings();
            var odataReader = this.CreateJsonLightInputContext("{\"@Annotation.1\":true,\"@Annotation.2\":123,\"Annotation.3\":\"annotation\"}", model, false)
                .CreateResourceReader(null, complexType);
            while (odataReader.Read())
            {
                if (odataReader.State == ODataReaderState.ResourceEnd)
                {
                    var complex = odataReader.Item as ODataResource;
                    Assert.Empty(complex.InstanceAnnotations);
                }
            }
        }

        #endregion

        [Fact]
        public void ParsingExpectedComplexPropertyActualNotShouldThrow()
        {
            var model = new EdmModel();
            var complexType = new EdmComplexType("TestNamespace", "Address");
            model.AddElement(complexType);
            var complexTypeRef = new EdmComplexTypeReference(complexType, false);
            var odataReader = this.CreateJsonLightInputContext("\"CountryRegion\":\"China\"", model, false)
                .CreateResourceReader(null, complexType);
            Action action = () => 
            {
                while (odataReader.Read())
                {
                    if (odataReader.State == ODataReaderState.ResourceEnd)
                    {
                        var complex = odataReader.Item as ODataResource;
                        Assert.Empty(complex.InstanceAnnotations);
                    }
                }
            };

            action.Throws<ODataException>(ErrorStrings.JsonReaderExtensions_UnexpectedNodeDetected("StartObject", "PrimitiveValue"));
        }

        private ODataJsonLightInputContext CreateJsonLightInputContext(string payload)
        {
            return this.CreateJsonLightInputContext(payload, this.edmModel);
        }

        private ODataJsonLightInputContext CreateJsonLightInputContext(string payload, IEdmModel model, bool isResponse = true)
        {
            var messageInfo = new ODataMessageInfo
            {
                IsResponse = isResponse,
                MediaType = JsonLightUtils.JsonLightStreamingMediaType,
                IsAsync = false,
                Model = model,
            };

            return new ODataJsonLightInputContext(
                new StringReader(payload), messageInfo, this.messageReaderSettings);
        }

        private object ReadODataTypePropertyAnnotation(IJsonReader jsonReader, string name)
        {
            Assert.Equal(ODataAnnotationNames.ODataType, name);
            return jsonReader.ReadStringValue();
        }

        private void RunPropertyParsingTest(
            string jsonInput,
            ODataJsonLightDeserializer.PropertyParsingResult expectedPropertyParsingResult,
            string expectedName,
            Action<IJsonReader, PropertyAndAnnotationCollector> additionalVerification = null,
            Func<IJsonReader, string, object> readPropertyAnnotationValue = null,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = null)
        {
            if (propertyAndAnnotationCollector == null)
            {
                propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            }

            if (readPropertyAnnotationValue == null)
            {
                readPropertyAnnotationValue = (jsonReader, annotationName) => jsonReader.ReadPrimitiveValue();
            }

            using (ODataJsonLightInputContext inputContext = this.CreateJsonLightInputContext(jsonInput))
            {
                ODataJsonLightDeserializer deserializer = new ODataJsonLightPropertyAndValueDeserializer(inputContext);
                AdvanceReaderToFirstProperty(deserializer.JsonReader);

                deserializer.ProcessProperty(
                    propertyAndAnnotationCollector,
                    (propertyName) => readPropertyAnnotationValue(deserializer.JsonReader, propertyName),
                    (propertyParsingResult, propertyName) =>
                    {
                        if (propertyParsingResult != ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue && deserializer.JsonReader.NodeType == JsonNodeType.Property)
                        {
                            // Read over property name
                            deserializer.JsonReader.Read();
                        }

                        Assert.Equal(expectedPropertyParsingResult, propertyParsingResult);
                        Assert.Equal(expectedName, propertyName);
                        if (additionalVerification != null)
                        {
                            additionalVerification(deserializer.JsonReader, propertyAndAnnotationCollector);
                        }
                    });
            }
        }

        private EdmModel CreateEdmModelWithEntity()
        {
            var model = new EdmModel();
            var entityType = new EdmEntityType("TestNamespace", "Customer");
            entityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            EdmStructuralProperty id = new EdmStructuralProperty(entityType, "FloatId", EdmCoreModel.Instance.GetSingle(false));
            entityType.AddKeys(id);
            entityType.AddProperty(id);
            model.AddElement(entityType);
            var defaultContainer = new EdmEntityContainer("TestNamespace", "DefaultContainer_sub");
            var entitySet = new EdmEntitySet(defaultContainer, "Customers", entityType);
            defaultContainer.AddEntitySet(entitySet.Name, entityType);
            model.AddElement(defaultContainer);
            return model;
        }
    }
}
