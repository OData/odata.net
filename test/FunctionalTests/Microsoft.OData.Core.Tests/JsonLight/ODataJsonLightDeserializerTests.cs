﻿//---------------------------------------------------------------------
// <copyright file="ODataJsonLightDeserializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using FluentAssertions;
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
                    jsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, 42);
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
                    jsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, "N\uffff");
                    (jsonReader.Value as string).Length.Should().Be(2);
                });
        }

        [Fact]
        public void ParsingCustomPropertyAnnotationShouldBeIgnored()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", "custom.type") + "\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.EndObject, null);
                    duplicateChecker.GetODataPropertyAnnotations("property").Should().BeEmpty();
                });
        }

        [Fact]
        public void ParsingODataPropertyAnnotationWithNothingAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.EndObject, null);
                    duplicateChecker.GetODataPropertyAnnotations("property").Should().Equal(new Dictionary<string, object> { { ODataAnnotationNames.ODataType, "typename" } });
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingODataPropertyAnnotationWithItsProperty()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename\",\"property\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, 42);
                    duplicateChecker.GetODataPropertyAnnotations("property").Should().Equal(new Dictionary<string, object> { { ODataAnnotationNames.ODataType, "typename" } });
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingODataPropertyAnnotationWithDifferentPropertyAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename\",\"otherproperty\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.Property, "otherproperty");
                    duplicateChecker.GetODataPropertyAnnotations("property").Should().Equal(new Dictionary<string, object> { { ODataAnnotationNames.ODataType, "typename" } });
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingODataPropertyAnnotationWithInsanceAnnotationAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename\",\"" + JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType + "\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.Property, JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType);
                    duplicateChecker.GetODataPropertyAnnotations("property").Should().Equal(new Dictionary<string, object> { { ODataAnnotationNames.ODataType, "typename" } });
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingODataPropertyAnnotationWithDifferentPropertyAnnotationAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename\",\"" + JsonLightUtils.GetPropertyAnnotationName("otherproperty", ODataAnnotationNames.ODataType) + "\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.Property, JsonLightUtils.GetPropertyAnnotationName("otherproperty", ODataAnnotationNames.ODataType));
                    duplicateChecker.GetODataPropertyAnnotations("property").Should().Equal(new Dictionary<string, object> { { ODataAnnotationNames.ODataType, "typename" } });
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingCustomPropertyAnnotationWithItsPropertyAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", "foo.bar") + "\":\"foo\",\"property\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, 42);
                    duplicateChecker.GetODataPropertyAnnotations("property").Should().BeEmpty();
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingCustomPropertyAnnotationWithDifferentPropertyAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", "foo.bar") + "\":\"foo\",\"otherproperty\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.Property, "otherproperty");
                    duplicateChecker.GetODataPropertyAnnotations("property").Should().BeEmpty();
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingCustomPropertyAnnotationWithInsanceAnnotationAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", "foo.bar") + "\":\"foo\",\"" + JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType + "\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.Property, JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType);
                    duplicateChecker.GetODataPropertyAnnotations("property").Should().BeEmpty();
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingCustomPropertyAnnotationWithDifferentPropertyAnnotationAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", "foo.bar") + "\":\"foo\",\"" + JsonLightUtils.GetPropertyAnnotationName("otherproperty", ODataAnnotationNames.ODataType) + "\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.Property, JsonLightUtils.GetPropertyAnnotationName("otherproperty", ODataAnnotationNames.ODataType));
                    duplicateChecker.GetODataPropertyAnnotations("property").Should().BeEmpty();
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingODataPropertyAnnotationWithMetadataReferencePropertyAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename\",\"#namespace.name\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.Property, "#namespace.name");
                    duplicateChecker.GetODataPropertyAnnotations("property").Should().Equal(new Dictionary<string, object> { { ODataAnnotationNames.ODataType, "typename" } });
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingCustomPropertyAnnotationWithMetadataReferencePropertyAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", "custom.annotation") + "\":\"foo\",\"#namespace.name\":42}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.Property, "#namespace.name");
                    duplicateChecker.GetODataPropertyAnnotations("property").Should().BeEmpty();
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingDuplicateODataPropertyAnnotationShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"" + JsonLightUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename\",\"" + JsonLightUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename2\"}", ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject, null,
                null, this.ReadODataTypePropertyAnnotation);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.DuplicateAnnotationForPropertyNotAllowed(ODataAnnotationNames.ODataType, "property"));
        }

        [Fact]
        public void ParsingDuplicateCustomPropertyAnnotationShouldNotFail()
        {
            Action action = () => this.RunPropertyParsingTest(
                "{\"" + JsonLightUtils.GetPropertyAnnotationName("property", "custom.type") + "\":\"typename\",\"" + JsonLightUtils.GetPropertyAnnotationName("property", "custom.type") + "\":\"typename2\"}",
                ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "property", null, this.ReadODataTypePropertyAnnotation);
            action.ShouldNotThrow();
        }

        #region Instance Annotation tests

        [Fact]
        public void ParsingODataInstanceAnnotationShouldReturnItsName()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType + "\":42}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, ODataAnnotationNames.ODataType,
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingCustomInstanceAnnotationShouldReturnItsName()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("custom.*");
            this.RunPropertyParsingTest("{\"@custom.type\":42}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingODataInstanceAnnotationWithDifferentPropertyAfterItShouldReturnItsName()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType + "\":42,\"foo\":42}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, ODataAnnotationNames.ODataType,
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingCustomInstanceAnnotationWithDifferentPropertyAfterItShouldReturnItsName()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("custom.type");
            this.RunPropertyParsingTest("{\"@custom.type\":42,\"foo\":42}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingODataInstanceAnnotationWithDifferentPropertyAnnotationAfterItShouldReturnItsName()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType + "\":42,\"foo@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, ODataAnnotationNames.ODataType,
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingCustomInstanceAnnotationWithDifferentPropertyAnnotationAfterItShouldReturnItsName()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            this.RunPropertyParsingTest("{\"@custom.type\":42,\"foo@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingCustomInstanceAnnotationBeforePropertyAnnotationWithoutOverridingDefaultAnnotationFilterShouldSkipCustomInstanceAnnotation()
        {
            this.RunPropertyParsingTest("{\"@custom.type\":42,\"foo@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "foo",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.EndObject, null);
                });
        }

        [Fact]
        public void ParsingODataInstanceAnnotationWithDifferentInstanceAnnotationAfterItShouldReturnItsName()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType + "\":42,\"@custom.instance\":42}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, ODataAnnotationNames.ODataType,
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingCustomInstanceAnnotationWithDifferentInstanceAnnotationAfterItShouldReturnItsName()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            this.RunPropertyParsingTest("{\"@custom.type\":42,\"@odata.unknown\":42}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingCustomInstanceAnnotationWithUnknownODataAnnotationAfterItShouldSkipBothAnotationsForExcludeFilter()
        {
            this.RunPropertyParsingTest("{\"@custom.type\":42,\"@odata.unknown\":42}", ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject, null,
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.EndObject, null);
                });
        }

        [Fact]
        public void ParsingODataInstanceAnnotationWithDifferentAnnotationTargetingAnnotationAfterItShouldReturnItsName()
        {
            this.RunPropertyParsingTest("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType + "\":42,\"custom.instance@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, ODataAnnotationNames.ODataType,
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingCustomInstanceAnnotationWithDifferentAnnotationTargetingAnnotationAfterItShouldReturnItsName()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            this.RunPropertyParsingTest("{\"@custom.type\":42,\"odata.unknown@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingODataTypeWithoutTheTargetingInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.deltaLink@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "odata.deltaLink", null, this.ReadODataTypePropertyAnnotation);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "odata.deltaLink"));
        }

        [Fact]
        public void ParsingODataTypeWithoutTheTargetingCustomInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithoutValue, "custom.instance", null, this.ReadODataTypePropertyAnnotation);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "custom.instance"));
        }

        [Fact]
        public void ParsingODataTypeTargetingODataInstanceAnnotationAfterItShouldReturnItsName()
        {
            this.RunPropertyParsingTest("{\"odata.deltaLink@odata.type\":\"#typename\", \"@odata.deltaLink\":42}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, "odata.deltaLink",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, 42);
                    duplicateChecker.GetODataPropertyAnnotations("odata.deltaLink").Should().Equal(new Dictionary<string, object> { { ODataAnnotationNames.ODataType, "#typename" } });
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
                    jsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, 42);
                    duplicateChecker.GetODataPropertyAnnotations("custom.instance").Should().Equal(new Dictionary<string, object> { { ODataAnnotationNames.ODataType, "#typename" } });
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingWithoutOverridingDefaultAnnotationFilterShouldSkipOverAllCustomAnnotations()
        {
            this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\",\"@custom.instance\":42}", ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject, null,
                (jsonReader, duplicateChecker) => jsonReader.Should().BeOn(JsonNodeType.EndObject, null),
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingODataAnnotationTargetingODataInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.deltaLink@odata.unknown\":42,\"@odata.deltaLink\":42}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, "odata.deltaLink");
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation("odata.unknown", "odata.deltaLink", "odata.type"));
        }

        [Fact]
        public void ParsingCustomAnnotationTargetingODataInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.deltaLink@custom.annotation\":42,\"@odata.deltaLink\":42}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, "odata.deltaLink");
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation("custom.annotation", "odata.deltaLink", "odata.type"));
        }

        [Fact]
        public void ParsingODataAnnotationTargetingCustomInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@odata.unknown\":42,\"@custom.instance\":42}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.instance");
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation("odata.unknown", "custom.instance", "odata.type"));
        }

        [Fact]
        public void ParsingCustomAnnotationTargetingCustomInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@custom.annotation\":42,\"@custom.instance\":42}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.instance");
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation("custom.annotation", "custom.instance", "odata.type"));
        }

        [Fact]
        public void ParsingODataTypeTargetingODataInstanceAnnotationWithDifferentPropertyAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.deltaLink@odata.type\":\"#typename\",\"foo\":42}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, ODataAnnotationNames.ODataType, null, this.ReadODataTypePropertyAnnotation);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "odata.deltaLink"));
        }

        [Fact]
        public void ParsingODataTypeTargetingCustomInstanceAnnotationWithDifferentPropertyAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\",\"foo\":42}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.instance", null, this.ReadODataTypePropertyAnnotation);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "custom.instance"));
        }

        [Fact]
        public void ParsingODataTypeTargetingODataInstanceAnnotationWithDifferentPropertyAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.unknown@odata.type\":\"#typename\",\"foo@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, "odata.unknown", null, this.ReadODataTypePropertyAnnotation);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "odata.unknown"));
        }

        [Fact]
        public void ParsingODataTypeTargetingCustomInstanceAnnotationWithDifferentPropertyAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\",\"foo@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.instance", null, this.ReadODataTypePropertyAnnotation);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "custom.instance"));
        }

        [Fact]
        public void ParsingODataTypeTargetingODataInstanceAnnotationWithDifferentInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.unknown@odata.type\":\"#typename\",\"@custom.instance\":42}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, "odata.unknown", null, this.ReadODataTypePropertyAnnotation);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "odata.unknown"));
        }

        [Fact]
        public void ParsingODataTypeTargetingCustomInstanceAnnotationWithDifferentInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\",\"@odata.unknown\":42}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.instance", null, this.ReadODataTypePropertyAnnotation);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "custom.instance"));
        }

        [Fact]
        public void ParsingODataTypeTargetingODataInstanceAnnotationWithDifferentAnnotationTargetingAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.unknown@odata.type\":\"#typename\",\"custom.instance@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, "odata.unknown", null, this.ReadODataTypePropertyAnnotation);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "odata.unknown"));
        }

        [Fact]
        public void ParsingODataTypeTargetingCustomInstanceAnnotationWithDifferentAnnotationTargetingAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\",\"odata.unknown@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.instance", null, this.ReadODataTypePropertyAnnotation);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "custom.instance"));
        }

        [Fact]
        public void ParsingODataTypeTargetingODataInstanceAnnotationWithMetadataReferencePropertyAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.unknown@odata.type\":\"#typename\",\"@namespace.name\":42}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, "odata.unknown", null, this.ReadODataTypePropertyAnnotation);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "odata.unknown"));
        }

        [Fact]
        public void ParsingODataTypeTargetingCustomInstanceAnnotationWithMetadataReferencePropertyAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\",\"@namespace.name\":42}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.instance", null, this.ReadODataTypePropertyAnnotation);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "custom.instance"));
        }

        [Fact]
        public void ParsingDuplicateODataInstanceAnnotationShouldFail()
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            Action action = () => this.RunPropertyParsingTest("{\"@odata.deltaLink\":\"url\",\"@odata.deltaLink\":\"url\"}", ODataJsonLightDeserializer.PropertyParsingResult.ODataInstanceAnnotation, "odata.deltaLink",
                null, this.ReadODataTypePropertyAnnotation, propertyAndAnnotationCollector);
            action();
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.DuplicateAnnotationNotAllowed("odata.deltaLink"));
        }

        [Fact]
        public void ParsingDuplicateCustomInstanceAnnotationShouldNotFail1()
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            Action action = () => this.RunPropertyParsingTest("{\"@custom.type\":\"typename\",\"@custom.type\":\"typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject,
                null, null, this.ReadODataTypePropertyAnnotation, propertyAndAnnotationCollector);
            action.ShouldNotThrow();
        }

        [Fact]
        public void ParsingDuplicateCustomInstanceAnnotationShouldNotFail2()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            Action action = () => this.RunPropertyParsingTest("{\"@custom.type\":\"typename\",\"@custom.type\":\"typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                null, this.ReadODataTypePropertyAnnotation, propertyAndAnnotationCollector);
            action();
            action.ShouldNotThrow();
        }

        [Fact]
        public void ParsingDuplicateODataTypeAnnotationTargetingODataInstanceAnnotationShouldFail()
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            Action action = () => this.RunPropertyParsingTest("{\"odata.annotation@odata.type\":\"#typename\",\"odata.annotation@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                null, this.ReadODataTypePropertyAnnotation, propertyAndAnnotationCollector);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.DuplicateAnnotationForInstanceAnnotationNotAllowed("odata.type", "odata.annotation"));
        }

        [Fact]
        public void ParsingDuplicateODataTypeAnnotationTargetingCustomInstanceAnnotationShouldFail()
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            Action action = () => this.RunPropertyParsingTest("{\"custom.annotation@odata.type\":\"#typename\",\"custom.annotation@odata.type\":\"#typename\"}", ODataJsonLightDeserializer.PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                null, this.ReadODataTypePropertyAnnotation, propertyAndAnnotationCollector);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.DuplicateAnnotationForInstanceAnnotationNotAllowed("odata.type", "custom.annotation"));
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
                (jsonReader, duplicateChecker) => jsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, 42));
        }

        [Fact]
        public void ParsingPropertyWithHashInMiddleShouldReturnMetadataReferenceProperty()
        {
            this.RunPropertyParsingTest(
                "{\"http://odata.org/$metadata#myaction\":42}",
                ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty,
                "http://odata.org/$metadata#myaction",
                (jsonReader, duplicateChecker) => jsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, 42));
        }

        [Fact]
        public void ParsingPropertyWithUriEscapedHashShouldNotBeMetadataReferenceProperty()
        {
            this.RunPropertyParsingTest(
                "{\"%23action\":42}",
                ODataJsonLightDeserializer.PropertyParsingResult.PropertyWithValue,
                "%23action",
                (jsonReader, duplicateChecker) => jsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, 42));
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
                (jsonReader, duplicateChecker) => jsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, 42));
        }

        [Fact]
        public void MetadataReferencePropertyWithTwoHashesShouldNotThrow()
        {
            this.RunPropertyParsingTest(
                "{\"#TwoHashes#Allowed\":42}",
                ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty,
                "#TwoHashes#Allowed",
                (jsonReader, duplicateChecker) => jsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, 42));
        }

        [Fact]
        public void ParsePropertyShouldSkipUnknownODataAnnotations()
        {
            this.RunPropertyParsingTest(
                "{\"@odata.unknown\":42}",
                ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject,
                null,
                (jsonReader, duplicateChecker) => jsonReader.Should().BeOn(JsonNodeType.EndObject, null));

            this.RunPropertyParsingTest(
                "{\"@odata.unknown1\":42,\"@odata.unknown2\":42,\"@odata.unknown3\":42}",
                ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject,
                null,
                (jsonReader, duplicateChecker) => jsonReader.Should().BeOn(JsonNodeType.EndObject, null));
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
                    jsonReader.Should().BeOn(JsonNodeType.Property, "@odata.unknown");
                    duplicateChecker.GetODataPropertyAnnotations("NavProp").Keys.Should().Contain("odata.navigationLink").And.HaveCount(1);
                },
                (jsonReader, name) =>
                {
                    name.Should().Be("odata.navigationLink");
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
                (jsonReader, duplicateChecker) => jsonReader.Should().BeOn(JsonNodeType.EndObject, null));

            this.RunPropertyParsingTest(
                "{\"property@odata.unknown1\":42,\"property@odata.unknown2\":42,\"property@odata.unknown3\":42}",
                ODataJsonLightDeserializer.PropertyParsingResult.EndOfObject,
                null,
                (jsonReader, duplicateChecker) => jsonReader.Should().BeOn(JsonNodeType.EndObject, null));
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
                    jsonReader.Should().BeOn(JsonNodeType.EndObject, null);
                    duplicateChecker.GetODataPropertyAnnotations("NavProp").Keys.Should().Contain("odata.navigationLink").And.Contain("odata.associationLink")
                    .And.Contain("odata.unknown").And.HaveCount(3);
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
                        propertyParsingResult.Should().Be(ODataJsonLightDeserializer.PropertyParsingResult.MetadataReferenceProperty, "parsing JSON object '{0}'", payload);
                        propertyName.Should().Be("#action", "reported name is wrong for JSON object '{0}'", payload);

                        deserializer.JsonReader.Should().BeOn(JsonNodeType.PrimitiveValue, 42);

                        deserializer.JsonReader.Read();
                        deserializer.JsonReader.NodeType.Should().Be(JsonNodeType.Property);
                    });

                Action readDuplicateProperty = () => deserializer.ProcessProperty(
                    propertyAndAnnotationCollector,
                    (propertyName) => null,
                    (propertyParsingResult, propertyName) => { });

                readDuplicateProperty.ShouldThrow<ODataException>("two metadata reference properties were encountered with the same name").WithMessage(ErrorStrings.DuplicatePropertyNamesNotAllowed("#action"));
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

                readEntryContentAction
                    .ShouldThrow<ODataException>("the property name \"{0}\" contains a hash but is not a valid URI or URI fragment", propertyName)
                    .WithMessage(ErrorStrings.ValidationUtils_InvalidMetadataReferenceProperty(propertyName));
            }
        }

        private static void AdvanceReaderToFirstProperty(BufferingJsonReader bufferingJsonReader)
        {
            // Read start and then over the object start.
            bufferingJsonReader.Read();
            bufferingJsonReader.Read();
            bufferingJsonReader.NodeType.Should().Be(JsonNodeType.Property);
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
                /*insideComplexValue*/ false,
                /*propertyName*/ null);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter("Edm.Int64"));
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
                /*insideComplexValue*/ false,
                /*propertyName*/ null);
            action.ShouldThrow<ODataException>().WithMessage("Value '123456789' was either too large or too small for a 'NS.UInt16'.");
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
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.count"));
        }

        [Fact]
        public void ParsingInstanceAnnotationInTopLevelPropertyShouldReadProperty()
        {
            var model = this.CreateEdmModelWithEntity();
            var primitiveTypeRef = ((IEdmEntityType)model.SchemaElements.First()).Properties().First().Type;
            this.messageReaderSettings = new ODataMessageReaderSettings { ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };
            ODataJsonLightPropertyAndValueDeserializer deserializer = new ODataJsonLightPropertyAndValueDeserializer(this.CreateJsonLightInputContext("{\"@odata.context\":\"http://odata.org/test/$metadata#Customers(1)/Name\",\"@Is.ReadOnly\":true,\"value\":\"Joe\"}", model));
            ODataProperty property = deserializer.ReadTopLevelProperty(primitiveTypeRef);
            property.InstanceAnnotations.Count.Should().Be(1);
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
            property.InstanceAnnotations.Count.Should().Be(3);
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
            property.InstanceAnnotations.Count.Should().Be(0);
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
                    complex.Properties.First().InstanceAnnotations.Count.Should().Be(1);
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
                    complex.Properties.First().InstanceAnnotations.Count.Should().Be(3);
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
                    complex.Properties.First().InstanceAnnotations.Count.Should().Be(0);
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

            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.count"));
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
                    complex.InstanceAnnotations.Count.Should().Be(1);
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
                    complex.InstanceAnnotations.Count.Should().Be(3);
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
            action.ShouldNotThrow();
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
                    complex.InstanceAnnotations.Count.Should().Be(0);
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
                        complex.InstanceAnnotations.Count.Should().Be(0);
                    }
                }
            };

            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.JsonReaderExtensions_UnexpectedNodeDetected("StartObject", "PrimitiveValue"));
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
            name.Should().Be(ODataAnnotationNames.ODataType, "we found a property annotation in the odata namespace with unexpected name.");
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
                        propertyParsingResult.Should().Be(expectedPropertyParsingResult, "parsing JSON object '{0}'", jsonInput);
                        propertyName.Should().Be(expectedName, "reported name is wrong for JSON object '{0}'", jsonInput);
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
