//---------------------------------------------------------------------
// <copyright file="ODataJsonDeserializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.Spatial;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;
using PropertyParsingResult = Microsoft.OData.Json.ODataJsonDeserializer.PropertyParsingResult;

namespace Microsoft.OData.Tests.Json
{
    public class ODataJsonDeserializerTests
    {
        private IEdmModel edmModel;
        private ODataMessageReaderSettings messageReaderSettings;

        public ODataJsonDeserializerTests()
        {
            this.edmModel = new EdmModel();
            this.messageReaderSettings = new ODataMessageReaderSettings();
        }

        [Fact]
        public void ParsingNormalPropertyShouldReturnItsName()
        {
            this.RunPropertyParsingTest("{\"property\":42}", PropertyParsingResult.PropertyWithValue, "property",
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
            this.RunPropertyParsingTest("{\"property\":\"N\\uffff\"}", PropertyParsingResult.PropertyWithValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, "N\uffff");
                    Assert.Equal(2, (jsonReader.GetValue() as string).Length);
                });
        }

        [Fact]
        public void ParsingCustomPropertyAnnotationShouldBeIgnored()
        {
            this.RunPropertyParsingTest("{\"" + JsonUtils.GetPropertyAnnotationName("property", "custom.type") + "\":42}", PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.EndObject, null);
                    Assert.Empty(duplicateChecker.GetODataPropertyAnnotations("property"));
                });
        }

        [Fact]
        public void ParsingODataPropertyAnnotationWithNothingAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename\"}", PropertyParsingResult.PropertyWithoutValue, "property",
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
            this.RunPropertyParsingTest("{\"" + JsonUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename\",\"property\":42}", PropertyParsingResult.PropertyWithValue, "property",
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
            this.RunPropertyParsingTest("{\"" + JsonUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename\",\"otherproperty\":42}", PropertyParsingResult.PropertyWithoutValue, "property",
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
            this.RunPropertyParsingTest("{\"" + JsonUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename\",\"" + ODataJsonConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType + "\":42}", PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.Property, ODataJsonConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType);
                    var propertyAnnotation = duplicateChecker.GetODataPropertyAnnotations("property");
                    Assert.Equal(new Dictionary<string, object> { { ODataAnnotationNames.ODataType, "typename" } }, propertyAnnotation);
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingODataPropertyAnnotationWithDifferentPropertyAnnotationAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename\",\"" + JsonUtils.GetPropertyAnnotationName("otherproperty", ODataAnnotationNames.ODataType) + "\":42}", PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.Property, JsonUtils.GetPropertyAnnotationName("otherproperty", ODataAnnotationNames.ODataType));
                    var propertyAnnotation = duplicateChecker.GetODataPropertyAnnotations("property");
                    Assert.Equal(new Dictionary<string, object> { { ODataAnnotationNames.ODataType, "typename" } }, propertyAnnotation);
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingCustomPropertyAnnotationWithItsPropertyAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonUtils.GetPropertyAnnotationName("property", "foo.bar") + "\":\"foo\",\"property\":42}", PropertyParsingResult.PropertyWithValue, "property",
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
            this.RunPropertyParsingTest("{\"" + JsonUtils.GetPropertyAnnotationName("property", "foo.bar") + "\":\"foo\",\"otherproperty\":42}", PropertyParsingResult.PropertyWithoutValue, "property",
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
            this.RunPropertyParsingTest("{\"" + JsonUtils.GetPropertyAnnotationName("property", "foo.bar") + "\":\"foo\",\"" + ODataJsonConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType + "\":42}", PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.Property, ODataJsonConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType);
                    Assert.Empty(duplicateChecker.GetODataPropertyAnnotations("property"));
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingCustomPropertyAnnotationWithDifferentPropertyAnnotationAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonUtils.GetPropertyAnnotationName("property", "foo.bar") + "\":\"foo\",\"" + JsonUtils.GetPropertyAnnotationName("otherproperty", ODataAnnotationNames.ODataType) + "\":42}", PropertyParsingResult.PropertyWithoutValue, "property",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.Property, JsonUtils.GetPropertyAnnotationName("otherproperty", ODataAnnotationNames.ODataType));
                    Assert.Empty(duplicateChecker.GetODataPropertyAnnotations("property"));
                },
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingODataPropertyAnnotationWithMetadataReferencePropertyAfterIt()
        {
            this.RunPropertyParsingTest("{\"" + JsonUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename\",\"#namespace.name\":42}", PropertyParsingResult.PropertyWithoutValue, "property",
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
            this.RunPropertyParsingTest("{\"" + JsonUtils.GetPropertyAnnotationName("property", "custom.annotation") + "\":\"foo\",\"#namespace.name\":42}", PropertyParsingResult.PropertyWithoutValue, "property",
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
            Action action = () => this.RunPropertyParsingTest("{\"" + JsonUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename\",\"" + JsonUtils.GetPropertyAnnotationName("property", ODataAnnotationNames.ODataType) + "\":\"typename2\"}", PropertyParsingResult.EndOfObject, null,
                null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.DuplicateAnnotationForPropertyNotAllowed(ODataAnnotationNames.ODataType, "property"));
        }

        [Fact]
        public void ParsingDuplicateCustomPropertyAnnotationShouldNotFail()
        {
            Action action = () => this.RunPropertyParsingTest(
                "{\"" + JsonUtils.GetPropertyAnnotationName("property", "custom.type") + "\":\"typename\",\"" + JsonUtils.GetPropertyAnnotationName("property", "custom.type") + "\":\"typename2\"}",
                PropertyParsingResult.PropertyWithoutValue, "property", null, this.ReadODataTypePropertyAnnotation);
            action.DoesNotThrow();
        }

        #region Instance Annotation tests

        [Fact]
        public void ParsingODataInstanceAnnotationShouldReturnItsName()
        {
            this.RunPropertyParsingTest("{\"" + ODataJsonConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType + "\":42}", PropertyParsingResult.ODataInstanceAnnotation, ODataAnnotationNames.ODataType,
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingCustomInstanceAnnotationShouldReturnItsName()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("custom.*");
            this.RunPropertyParsingTest("{\"@custom.type\":42}", PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingODataInstanceAnnotationWithDifferentPropertyAfterItShouldReturnItsName()
        {
            this.RunPropertyParsingTest("{\"" + ODataJsonConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType + "\":42,\"foo\":42}", PropertyParsingResult.ODataInstanceAnnotation, ODataAnnotationNames.ODataType,
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingCustomInstanceAnnotationWithDifferentPropertyAfterItShouldReturnItsName()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("custom.type");
            this.RunPropertyParsingTest("{\"@custom.type\":42,\"foo\":42}", PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingODataInstanceAnnotationWithDifferentPropertyAnnotationAfterItShouldReturnItsName()
        {
            this.RunPropertyParsingTest("{\"" + ODataJsonConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType + "\":42,\"foo@odata.type\":\"#typename\"}", PropertyParsingResult.ODataInstanceAnnotation, ODataAnnotationNames.ODataType,
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingCustomInstanceAnnotationWithDifferentPropertyAnnotationAfterItShouldReturnItsName()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            this.RunPropertyParsingTest("{\"@custom.type\":42,\"foo@odata.type\":\"#typename\"}", PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingCustomInstanceAnnotationBeforePropertyAnnotationWithoutOverridingDefaultAnnotationFilterShouldSkipCustomInstanceAnnotation()
        {
            this.RunPropertyParsingTest("{\"@custom.type\":42,\"foo@odata.type\":\"#typename\"}", PropertyParsingResult.PropertyWithoutValue, "foo",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.EndObject, null);
                });
        }

        [Fact]
        public void ParsingODataInstanceAnnotationWithDifferentInstanceAnnotationAfterItShouldReturnItsName()
        {
            this.RunPropertyParsingTest("{\"" + ODataJsonConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType + "\":42,\"@custom.instance\":42}", PropertyParsingResult.ODataInstanceAnnotation, ODataAnnotationNames.ODataType,
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingCustomInstanceAnnotationWithDifferentInstanceAnnotationAfterItShouldReturnItsName()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            this.RunPropertyParsingTest("{\"@custom.type\":42,\"@odata.unknown\":42}", PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingCustomInstanceAnnotationWithUnknownODataAnnotationAfterItShouldSkipBothAnotationsForExcludeFilter()
        {
            this.RunPropertyParsingTest("{\"@custom.type\":42,\"@odata.unknown\":42}", PropertyParsingResult.EndOfObject, null,
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.EndObject, null);
                });
        }

        [Fact]
        public void ParsingODataInstanceAnnotationWithDifferentAnnotationTargetingAnnotationAfterItShouldReturnItsName()
        {
            this.RunPropertyParsingTest("{\"" + ODataJsonConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataType + "\":42,\"custom.instance@odata.type\":\"#typename\"}", PropertyParsingResult.ODataInstanceAnnotation, ODataAnnotationNames.ODataType,
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingCustomInstanceAnnotationWithDifferentAnnotationTargetingAnnotationAfterItShouldReturnItsName()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            this.RunPropertyParsingTest("{\"@custom.type\":42,\"odata.unknown@odata.type\":\"#typename\"}", PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                (jsonReader, duplicateChecker) =>
                {
                    jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42);
                });
        }

        [Fact]
        public void ParsingODataTypeWithoutTheTargetingInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.deltaLink@odata.type\":\"#typename\"}", PropertyParsingResult.PropertyWithoutValue, "odata.deltaLink", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "odata.deltaLink"));
        }

        [Fact]
        public void ParsingODataTypeWithoutTheTargetingCustomInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\"}", PropertyParsingResult.PropertyWithoutValue, "custom.instance", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "custom.instance"));
        }

        [Fact]
        public void ParsingODataTypeTargetingODataInstanceAnnotationAfterItShouldReturnItsName()
        {
            this.RunPropertyParsingTest("{\"odata.deltaLink@odata.type\":\"#typename\", \"@odata.deltaLink\":42}", PropertyParsingResult.ODataInstanceAnnotation, "odata.deltaLink",
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
            this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\",\"@custom.instance\":42}", PropertyParsingResult.CustomInstanceAnnotation, "custom.instance",
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
            this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\",\"@custom.instance\":42}", PropertyParsingResult.EndOfObject, null,
                (jsonReader, duplicateChecker) => jsonReader.ShouldBeOn(JsonNodeType.EndObject, null),
                this.ReadODataTypePropertyAnnotation);
        }

        [Fact]
        public void ParsingODataAnnotationTargetingODataInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.deltaLink@odata.unknown\":42,\"@odata.deltaLink\":42}", PropertyParsingResult.ODataInstanceAnnotation, "odata.deltaLink");
            action.Throws<ODataException>(ErrorStrings.ODataJsonDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation("odata.unknown", "odata.deltaLink", "odata.type"));
        }

        [Fact]
        public void ParsingCustomAnnotationTargetingODataInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.deltaLink@custom.annotation\":42,\"@odata.deltaLink\":42}", PropertyParsingResult.ODataInstanceAnnotation, "odata.deltaLink");
            action.Throws<ODataException>(ErrorStrings.ODataJsonDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation("custom.annotation", "odata.deltaLink", "odata.type"));
        }

        [Fact]
        public void ParsingODataAnnotationTargetingCustomInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@odata.unknown\":42,\"@custom.instance\":42}", PropertyParsingResult.CustomInstanceAnnotation, "custom.instance");
            action.Throws<ODataException>(ErrorStrings.ODataJsonDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation("odata.unknown", "custom.instance", "odata.type"));
        }

        [Fact]
        public void ParsingCustomAnnotationTargetingCustomInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@custom.annotation\":42,\"@custom.instance\":42}", PropertyParsingResult.CustomInstanceAnnotation, "custom.instance");
            action.Throws<ODataException>(ErrorStrings.ODataJsonDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation("custom.annotation", "custom.instance", "odata.type"));
        }

        [Fact]
        public void ParsingODataTypeTargetingODataInstanceAnnotationWithDifferentPropertyAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.deltaLink@odata.type\":\"#typename\",\"foo\":42}", PropertyParsingResult.ODataInstanceAnnotation, ODataAnnotationNames.ODataType, null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "odata.deltaLink"));
        }

        [Fact]
        public void ParsingODataTypeTargetingCustomInstanceAnnotationWithDifferentPropertyAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\",\"foo\":42}", PropertyParsingResult.CustomInstanceAnnotation, "custom.instance", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "custom.instance"));
        }

        [Fact]
        public void ParsingODataTypeTargetingODataInstanceAnnotationWithDifferentPropertyAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.unknown@odata.type\":\"#typename\",\"foo@odata.type\":\"#typename\"}", PropertyParsingResult.ODataInstanceAnnotation, "odata.unknown", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "odata.unknown"));
        }

        [Fact]
        public void ParsingODataTypeTargetingCustomInstanceAnnotationWithDifferentPropertyAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\",\"foo@odata.type\":\"#typename\"}", PropertyParsingResult.CustomInstanceAnnotation, "custom.instance", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "custom.instance"));
        }

        [Fact]
        public void ParsingODataTypeTargetingODataInstanceAnnotationWithDifferentInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.unknown@odata.type\":\"#typename\",\"@custom.instance\":42}", PropertyParsingResult.ODataInstanceAnnotation, "odata.unknown", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "odata.unknown"));
        }

        [Fact]
        public void ParsingODataTypeTargetingCustomInstanceAnnotationWithDifferentInstanceAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\",\"@odata.unknown\":42}", PropertyParsingResult.CustomInstanceAnnotation, "custom.instance", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "custom.instance"));
        }

        [Fact]
        public void ParsingODataTypeTargetingODataInstanceAnnotationWithDifferentAnnotationTargetingAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.unknown@odata.type\":\"#typename\",\"custom.instance@odata.type\":\"#typename\"}", PropertyParsingResult.ODataInstanceAnnotation, "odata.unknown", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "odata.unknown"));
        }

        [Fact]
        public void ParsingODataTypeTargetingCustomInstanceAnnotationWithDifferentAnnotationTargetingAnnotationAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\",\"odata.unknown@odata.type\":\"#typename\"}", PropertyParsingResult.CustomInstanceAnnotation, "custom.instance", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "custom.instance"));
        }

        [Fact]
        public void ParsingODataTypeTargetingODataInstanceAnnotationWithMetadataReferencePropertyAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"odata.unknown@odata.type\":\"#typename\",\"@namespace.name\":42}", PropertyParsingResult.ODataInstanceAnnotation, "odata.unknown", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "odata.unknown"));
        }

        [Fact]
        public void ParsingODataTypeTargetingCustomInstanceAnnotationWithMetadataReferencePropertyAfterItShouldFail()
        {
            Action action = () => this.RunPropertyParsingTest("{\"custom.instance@odata.type\":\"#typename\",\"@namespace.name\":42}", PropertyParsingResult.CustomInstanceAnnotation, "custom.instance", null, this.ReadODataTypePropertyAnnotation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", "custom.instance"));
        }

        [Fact]
        public void ParsingDuplicateODataInstanceAnnotationShouldFail()
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            Action action = () => this.RunPropertyParsingTest("{\"@odata.deltaLink\":\"url\",\"@odata.deltaLink\":\"url\"}", PropertyParsingResult.ODataInstanceAnnotation, "odata.deltaLink",
                null, this.ReadODataTypePropertyAnnotation, propertyAndAnnotationCollector);
            action();
            action.Throws<ODataException>(ErrorStrings.DuplicateAnnotationNotAllowed("odata.deltaLink"));
        }

        [Fact]
        public void ParsingDuplicateCustomInstanceAnnotationShouldNotFail1()
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            Action action = () => this.RunPropertyParsingTest("{\"@custom.type\":\"typename\",\"@custom.type\":\"typename\"}", PropertyParsingResult.EndOfObject,
                null, null, this.ReadODataTypePropertyAnnotation, propertyAndAnnotationCollector);
            action.DoesNotThrow();
        }

        [Fact]
        public void ParsingDuplicateCustomInstanceAnnotationShouldNotFail2()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            Action action = () => this.RunPropertyParsingTest("{\"@custom.type\":\"typename\",\"@custom.type\":\"typename\"}", PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                null, this.ReadODataTypePropertyAnnotation, propertyAndAnnotationCollector);
            action();
            action.DoesNotThrow();
        }

        [Fact]
        public void ParsingDuplicateODataTypeAnnotationTargetingODataInstanceAnnotationShouldFail()
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            Action action = () => this.RunPropertyParsingTest("{\"odata.annotation@odata.type\":\"#typename\",\"odata.annotation@odata.type\":\"#typename\"}", PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
                null, this.ReadODataTypePropertyAnnotation, propertyAndAnnotationCollector);
            action.Throws<ODataException>(ErrorStrings.DuplicateAnnotationForInstanceAnnotationNotAllowed("odata.type", "odata.annotation"));
        }

        [Fact]
        public void ParsingDuplicateODataTypeAnnotationTargetingCustomInstanceAnnotationShouldFail()
        {
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
            Action action = () => this.RunPropertyParsingTest("{\"custom.annotation@odata.type\":\"#typename\",\"custom.annotation@odata.type\":\"#typename\"}", PropertyParsingResult.CustomInstanceAnnotation, "custom.type",
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
                PropertyParsingResult.MetadataReferenceProperty,
                "#myproperty",
                (jsonReader, duplicateChecker) => jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42));
        }

        [Fact]
        public void ParsingPropertyWithHashInMiddleShouldReturnMetadataReferenceProperty()
        {
            this.RunPropertyParsingTest(
                "{\"http://odata.org/$metadata#myaction\":42}",
                PropertyParsingResult.MetadataReferenceProperty,
                "http://odata.org/$metadata#myaction",
                (jsonReader, duplicateChecker) => jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42));
        }

        [Fact]
        public void ParsingPropertyWithUriEscapedHashShouldNotBeMetadataReferenceProperty()
        {
            this.RunPropertyParsingTest(
                "{\"%23action\":42}",
                PropertyParsingResult.PropertyWithValue,
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
                PropertyParsingResult.MetadataReferenceProperty,
                "#CaratIs^InvalidPerUriSpec",
                (jsonReader, duplicateChecker) => jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42));
        }

        [Fact]
        public void MetadataReferencePropertyWithTwoHashesShouldNotThrow()
        {
            this.RunPropertyParsingTest(
                "{\"#TwoHashes#Allowed\":42}",
                PropertyParsingResult.MetadataReferenceProperty,
                "#TwoHashes#Allowed",
                (jsonReader, duplicateChecker) => jsonReader.ShouldBeOn(JsonNodeType.PrimitiveValue, 42));
        }

        [Fact]
        public void ParsePropertyShouldSkipUnknownODataAnnotations()
        {
            this.RunPropertyParsingTest(
                "{\"@odata.unknown\":42}",
                PropertyParsingResult.EndOfObject,
                null,
                (jsonReader, duplicateChecker) => jsonReader.ShouldBeOn(JsonNodeType.EndObject, null));

            this.RunPropertyParsingTest(
                "{\"@odata.unknown1\":42,\"@odata.unknown2\":42,\"@odata.unknown3\":42}",
                PropertyParsingResult.EndOfObject,
                null,
                (jsonReader, duplicateChecker) => jsonReader.ShouldBeOn(JsonNodeType.EndObject, null));
        }

        [Fact]
        public void ParsePropertyShouldTreatUnknownODataAnnotationsAsABoundary()
        {
            this.RunPropertyParsingTest(
                "{\"NavProp@odata.navigationLink\":\"http://someUrl\",\"@odata.unknown\":42,\"NavProp@odata.associationLink\":\"http://someUrl\"}",
                PropertyParsingResult.PropertyWithoutValue,
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
                PropertyParsingResult.EndOfObject,
                null,
                (jsonReader, duplicateChecker) => jsonReader.ShouldBeOn(JsonNodeType.EndObject, null));

            this.RunPropertyParsingTest(
                "{\"property@odata.unknown1\":42,\"property@odata.unknown2\":42,\"property@odata.unknown3\":42}",
                PropertyParsingResult.EndOfObject,
                null,
                (jsonReader, duplicateChecker) => jsonReader.ShouldBeOn(JsonNodeType.EndObject, null));
        }

        [Fact]
        public void ParsePropertyShouldSkipUnknownODataPropertyAnnotations2()
        {
            this.RunPropertyParsingTest(
                "{\"NavProp@odata.navigationLink\":\"http://someUrl\",\"NavProp@odata.unknown\":42,\"NavProp@odata.associationLink\":\"http://someUrl\"}",
                PropertyParsingResult.PropertyWithoutValue,
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

            using (ODataJsonInputContext inputContext = this.CreateJsonInputContext(payload))
            {
                ODataJsonDeserializer deserializer = new ODataJsonPropertyAndValueDeserializer(inputContext);
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

                        Assert.Equal(PropertyParsingResult.MetadataReferenceProperty, propertyParsingResult);
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
            string jsonInput = string.Format("{{\"" + ODataJsonConstants.ODataPropertyAnnotationSeparatorChar + ODataAnnotationNames.ODataContext + "\":\"http://odata.org/$metadata\"," + "\"{0}\":42}}", propertyName);
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);

            using (ODataJsonInputContext inputContext = this.CreateJsonInputContext(jsonInput))
            {
                ODataJsonResourceDeserializer deserializer = new ODataJsonResourceDeserializer(inputContext);
                deserializer.ReadPayloadStart(ODataPayloadKind.Unsupported, propertyAndAnnotationCollector, false, false);

                Action readEntryContentAction = () => deserializer.ReadResourceContent(new TestJsonReaderEntryState());

                var exception = Assert.Throws<ODataException>(readEntryContentAction);
                Assert.Equal(ErrorStrings.ValidationUtils_InvalidMetadataReferenceProperty(propertyName), exception.Message);
            }
        }
        #endregion MetadataReferenceProperty tests

        [Fact]
        public void ParsingTypeDefinitionValueOfIncompatibleTypeShouldFail()
        {
            var model = new EdmModel();
            var uint32 = model.GetUInt32("NS", true);
            ODataJsonPropertyAndValueDeserializer deserializer = new ODataJsonPropertyAndValueDeserializer(this.CreateJsonInputContext("\"123\"", model));
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
            ODataJsonPropertyAndValueDeserializer deserializer = new ODataJsonPropertyAndValueDeserializer(this.CreateJsonInputContext("123456789", model));
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
            ODataJsonPropertyAndValueDeserializer deserializer = new ODataJsonPropertyAndValueDeserializer(this.CreateJsonInputContext("{\"@odata.context\":\"http://odata.org/test/$metadata#Customers(1)/Name\",\"value\":\"Joe\"}", model));
            ODataProperty property = deserializer.ReadTopLevelProperty(primitiveTypeRef);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue("Joe"), property.ODataValue);
        }

        [Fact]
        public void TopLevelPropertyShouldReadNullProperty()
        {
            var model = this.CreateEdmModelWithEntity();
            var primitiveTypeRef = ((IEdmEntityType)model.SchemaElements.First()).Properties().First().Type;
            this.messageReaderSettings = new ODataMessageReaderSettings();
            ODataJsonPropertyAndValueDeserializer deserializer = new ODataJsonPropertyAndValueDeserializer(this.CreateJsonInputContext("{\"@odata.context\":\"http://odata.org/test/$metadata#Customers(1)/Name\",\"value\":null}", model));
            ODataProperty property = deserializer.ReadTopLevelProperty(primitiveTypeRef);
            TestUtils.AssertODataValueAreEqual(new ODataNullValue(), property.ODataValue);
        }

        [Fact]
        public void TopLevelPropertyShouldRead6xNullProperty()
        {
            var model = this.CreateEdmModelWithEntity();
            var primitiveTypeRef = ((IEdmEntityType)model.SchemaElements.First()).Properties().First().Type;
            this.messageReaderSettings = new ODataMessageReaderSettings();
            ODataJsonPropertyAndValueDeserializer deserializer = new ODataJsonPropertyAndValueDeserializer(this.CreateJsonInputContext("{\"@odata.context\":\"http://odata.org/test/$metadata#Customers(1)/Name\",\"@odata.null\":true}", model));
            ODataProperty property = deserializer.ReadTopLevelProperty(primitiveTypeRef);
            TestUtils.AssertODataValueAreEqual(new ODataNullValue(), property.ODataValue);
        }

        [Fact]
        public void ReadPayloadStartAsyncTestForDetla()
        {
            var model = this.CreateEdmModelWithEntity();
            var primitiveTypeRef = ((IEdmEntityType)model.SchemaElements.First()).Properties().First().Type;
            this.messageReaderSettings = new ODataMessageReaderSettings();
            ODataJsonPropertyAndValueDeserializer deserializer = new ODataJsonPropertyAndValueDeserializer(this.CreateJsonInputContext("{\"@odata.context\":\"http://odata.org/test/$metadata#Customers/$delta\",\"value\":\"Joe\"}", model));
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
            ODataJsonPropertyAndValueDeserializer deserializer = new ODataJsonPropertyAndValueDeserializer(this.CreateJsonInputContext(payload, model));
            ODataProperty property = deserializer.ReadTopLevelProperty(primitiveTypeRef);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(value), property.ODataValue);
        }

        [Fact]
        public void TopLevelPropertyShouldReadContextUriAsRelativeUri()
        {
            var model = this.CreateEdmModelWithEntity();
            var primitiveTypeRef = ((IEdmEntityType)model.SchemaElements.First()).Properties().First().Type;
            this.messageReaderSettings = new ODataMessageReaderSettings() { BaseUri = new Uri("http://odata.org/test/") };
            ODataJsonPropertyAndValueDeserializer deserializer = new ODataJsonPropertyAndValueDeserializer(this.CreateJsonInputContext("{\"@odata.context\":\"Customers(1)/Name\",\"value\":\"Joe\"}", model));
            ODataProperty property = deserializer.ReadTopLevelProperty(primitiveTypeRef);

            Assert.NotNull(property);
            Assert.Equal("http://odata.org/test/$metadata#Customers(1)/Name", deserializer.ContextUriParseResult.ContextUri.ToString());

            deserializer = new ODataJsonPropertyAndValueDeserializer(this.CreateJsonInputContext("{\"@odata.context\":\"$metadata#Customers(1)/Name\",\"value\":\"Joe\"}", model));
            property = deserializer.ReadTopLevelProperty(primitiveTypeRef);

            Assert.NotNull(property);
            Assert.Equal("http://odata.org/test/$metadata#Customers(1)/Name", deserializer.ContextUriParseResult.ContextUri.ToString());
        }


        #endregion

        #region Top level property instance annotation

        [Fact]
        public void ParsingInstanceAnnotationInTopLevelPropertyShouldThrowOnReservedODataAnnotationNamesNotApplicableToProperties()
        {
            var model = this.CreateEdmModelWithEntity();
            var primitiveTypeRef = ((IEdmEntityType)model.SchemaElements.First()).Properties().First().Type;
            this.messageReaderSettings = new ODataMessageReaderSettings { ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };
            ODataJsonPropertyAndValueDeserializer deserializer = new ODataJsonPropertyAndValueDeserializer(this.CreateJsonInputContext("{\"@odata.context\":\"http://odata.org/test/$metadata#Customers(1)/Name\",\"@odata.count\":123,\"value\":\"Joe\"}", model));
            Action action = () => deserializer.ReadTopLevelProperty(primitiveTypeRef);
            action.Throws<ODataException>(ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.count"));
        }

        [Fact]
        public void ParsingInstanceAnnotationInTopLevelPropertyShouldReadProperty()
        {
            var model = this.CreateEdmModelWithEntity();
            var primitiveTypeRef = ((IEdmEntityType)model.SchemaElements.First()).Properties().First().Type;
            this.messageReaderSettings = new ODataMessageReaderSettings { ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };
            ODataJsonPropertyAndValueDeserializer deserializer = new ODataJsonPropertyAndValueDeserializer(this.CreateJsonInputContext("{\"@odata.context\":\"http://odata.org/test/$metadata#Customers(1)/Name\",\"@Is.ReadOnly\":true,\"value\":\"Joe\"}", model));
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
            ODataJsonPropertyAndValueDeserializer deserializer = new ODataJsonPropertyAndValueDeserializer(this.CreateJsonInputContext("{\"@odata.context\":\"http://odata.org/test/$metadata#Customers(1)/Name\",\"@Annotation.1\":true,\"@Annotation.2\":123,\"@Annotation.3\":\"annotation\",\"value\":\"Joe\"}", model));
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
            ODataJsonPropertyAndValueDeserializer deserializer = new ODataJsonPropertyAndValueDeserializer(this.CreateJsonInputContext("{\"@odata.context\":\"http://odata.org/test/$metadata#Customers(1)/Name\",\"@Annotation.1\":true,\"@Annotation.2\":123,\"@Annotation.3\":\"annotation\",\"value\":\"Joe\"}", model));
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
            var odataReader = this.CreateJsonInputContext("{\"CountryRegion@odata.count\":123,\"CountryRegion\":\"China\"}", model, false)
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
            var odataReader = this.CreateJsonInputContext("{\"CountryRegion@Is.ReadOnly\":true,\"CountryRegion\":\"China\"}", model, false)
                .CreateResourceReader(null, complexType);
            while (odataReader.Read())
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
            var odataReader = this.CreateJsonInputContext("{\"CountryRegion@Annotation.1\":true,\"CountryRegion@Annotation.2\":123,\"CountryRegion@Annotation.3\":\"annotation\",\"CountryRegion\":\"China\"}", model, false)
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
            var odataReader = this.CreateJsonInputContext("{\"CountryRegion@Is.ReadOnly\":true,\"CountryRegion\":\"China\"}", model, false)
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
            var odataReader = this.CreateJsonInputContext("{\"@odata.count\":\"123\"}", model, false)
                .CreateResourceReader(null, complexType);
            Action action = () =>
            {
                while (odataReader.Read()) ;
            };

            action.Throws<ODataException>(ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.count"));
        }

        [Fact]
        public void ParsingInstanceAnnotationInComplexValueShouldReadComplexValue()
        {
            var model = new EdmModel();
            var complexType = new EdmComplexType("TestNamespace", "Address");
            model.AddElement(complexType);
            var complexTypeRef = new EdmComplexTypeReference(complexType, false);
            this.messageReaderSettings = new ODataMessageReaderSettings { ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };

            var odataReader = this.CreateJsonInputContext("{\"@Is.ReadOnly\":true}", model, false)
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
            var odataReader = this.CreateJsonInputContext("{\"@Annotation.1\":true,\"@Annotation.2\":123,\"Annotation.3\":\"annotation\"}", model, false)
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
            var odataReader = this.CreateJsonInputContext("{\"@Annotation.1\":true,\"@Annotation.2\":123,\"CountryRegion\":\"China\",\"@Annotation.3\":\"annotation\"}", model, false)
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
            var odataReader = this.CreateJsonInputContext("{\"@Annotation.1\":true,\"@Annotation.2\":123,\"Annotation.3\":\"annotation\"}", model, false)
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
            var odataReader = this.CreateJsonInputContext("\"CountryRegion\":\"China\"", model, false)
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

        #region Async Tests

        [Fact]
        public async Task ReadAndValidateAnnotationStringValueAsync()
        {
            var payload = "{\"@odata.id\":\"Customers(1)\"}";

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                EdmCoreModel.Instance,
                async (jsonDeserializer) =>
                {
                    await AdvanceReaderToFirstPropertyAsync(jsonDeserializer.JsonReader);
                    await jsonDeserializer.JsonReader.ReadPropertyNameAsync();

                    var annotationValue = await jsonDeserializer.ReadAndValidateAnnotationStringValueAsync("@odata.id");
                    Assert.Equal("Customers(1)", annotationValue);
                });
        }

        [Fact]
        public async Task ReadAnnotationStringValueAsync()
        {
            var payload = "{\"@odata.id\":\"Customers(1)\"}";

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                EdmCoreModel.Instance,
                async (jsonDeserializer) =>
                {
                    await AdvanceReaderToFirstPropertyAsync(jsonDeserializer.JsonReader);
                    await jsonDeserializer.JsonReader.ReadPropertyNameAsync();

                    var annotationValue = await jsonDeserializer.ReadAnnotationStringValueAsync("@odata.id");
                    Assert.Equal("Customers(1)", annotationValue);
                });
        }

        [Theory]
        [InlineData("{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"}", ODataPayloadKind.Delta, "http://tempuri.org/$metadata#Customers/$entity")]
        [InlineData("{\"err\":{\"code\":\"NRE\",\"message\":\"Object reference not set to an instance of an object.\"}}", ODataPayloadKind.Error, null)]
        [InlineData("", ODataPayloadKind.Value, null)]
        public async Task ReadPayloadStartAsync(string payload, ODataPayloadKind payloadKind, string expectedContextUri)
        {
            var model = InitializeModel();

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                async (jsonDeserializer) =>
                {
                    await jsonDeserializer.ReadPayloadStartAsync(
                    payloadKind,
                    new PropertyAndAnnotationCollector(true),
                    isReadingNestedPayload: false,
                    allowEmptyPayload: true);

                    if (expectedContextUri == null)
                    {
                        Assert.Null(jsonDeserializer.ContextUriParseResult);
                    }
                    else
                    {
                        Assert.Equal(expectedContextUri, jsonDeserializer.ContextUriParseResult?.ContextUri?.AbsoluteUri);
                    }
                });
        }

        [Theory]
        [InlineData("{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"}", "http://tempuri.org/$metadata#Customers/$entity")]
        [InlineData("{\"@odata.type\":\"#Collection(NS.Customer)\"}", null)]
        [InlineData("{}", null)]
        public async Task ReadContextUriAnnotationAsync(string payload, string expectedContextUri)
        {
            var model = InitializeModel();

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                async (jsonDeserializer) =>
                {
                    await AdvanceReaderToFirstPropertyAsync(jsonDeserializer.JsonReader);
                    var contextUri = await jsonDeserializer.ReadContextUriAnnotationAsync(
                        ODataPayloadKind.Resource,
                        new PropertyAndAnnotationCollector(true),
                        failOnMissingContextUriAnnotation: false);

                    Assert.Equal(expectedContextUri, contextUri);
                });
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("{\"@odata.type\":\"#Collection(NS.Customer)\"}")]
        public async Task ReadContextUriAnnotationAsync_ThrowsExceptionForMissingContextUriAnnotation(string payload)
        {
            var model = InitializeModel();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                    payload,
                    model,
                    async (jsonDeserializer) =>
                    {
                        await AdvanceReaderToFirstPropertyAsync(jsonDeserializer.JsonReader);

                        await jsonDeserializer.ReadContextUriAnnotationAsync(
                            ODataPayloadKind.Resource,
                            new PropertyAndAnnotationCollector(true),
                            failOnMissingContextUriAnnotation: true);
                    }));

            Assert.Equal(
                ErrorStrings.ODataJsonDeserializer_ContextLinkNotFoundAsFirstProperty,
                exception.Message);
        }

        [Theory]
        [InlineData("null", null)]
        [InlineData("\"Customers?$deltatoken=13\"", "http://tempuri.org/Customers?$deltatoken=13")]
        public async Task ReadAnnotationStringValueAsUriAsync(string deltaLink, string expected)
        {
            var model = InitializeModel();

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$delta\"," +
                $"\"@odata.deltaLink\":{deltaLink}}}";

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                async (jsonDeserializer) =>
                {
                    await jsonDeserializer.ReadPayloadStartAsync(ODataPayloadKind.Delta, new PropertyAndAnnotationCollector(true), false, true);
                    Assert.Equal("@odata.deltaLink", await jsonDeserializer.JsonReader.ReadPropertyNameAsync());
                    var annotationValueAsUri = await jsonDeserializer.ReadAnnotationStringValueAsUriAsync("@odata.deltaLink");

                    var expectedUri = expected == null ? null : new Uri(expected);
                    Assert.Equal(expectedUri, annotationValueAsUri);
                    Assert.Equal(
                        "http://tempuri.org/$metadata#Customers/$delta",
                        jsonDeserializer.ContextUriParseResult.ContextUri.AbsoluteUri);
                });
        }

        [Fact]
        public async Task ReadAndValidateAnnotationStringValueAsUriAsync()
        {
            var model = InitializeModel();

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$delta\"," +
                "\"@odata.deltaLink\":\"Customers?$deltatoken=13\"}";

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                async (jsonDeserializer) =>
                {
                    await jsonDeserializer.ReadPayloadStartAsync(ODataPayloadKind.Delta, new PropertyAndAnnotationCollector(true), false, true);
                    Assert.Equal("@odata.deltaLink", await jsonDeserializer.JsonReader.ReadPropertyNameAsync());
                    var annotationValueAsUri = await jsonDeserializer.ReadAndValidateAnnotationStringValueAsUriAsync("@odata.deltaLink");

                    Assert.Equal(
                        "http://tempuri.org/$metadata#Customers/$delta",
                        jsonDeserializer.ContextUriParseResult.ContextUri.AbsoluteUri);
                    Assert.Equal(new Uri("http://tempuri.org/Customers?$deltatoken=13"), annotationValueAsUri);
                });
        }

        [Theory]
        [InlineData("13", false)]
        [InlineData("\"13\"", true)]
        public async Task ReadAndValidateAnnotationAsLongForIeee754CompatibleAsync(string data, bool isIeee754Compatible)
        {
            var payload = $"{{\"Data\":{data}}}";

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                EdmCoreModel.Instance,
                async (jsonDeserializer) =>
                {
                    await AdvanceReaderToFirstPropertyAsync(jsonDeserializer.JsonReader);
                    await jsonDeserializer.JsonReader.ReadPropertyNameAsync();

                    var value = await jsonDeserializer.ReadAndValidateAnnotationAsLongForIeee754CompatibleAsync("@odata.count");

                    Assert.Equal(13, value);
                },
                isIeee754Compatible: isIeee754Compatible);
        }

        [Fact]
        public async Task ProcessPropertyWithValueAsync()
        {
            var payload = "{\"Colors@odata.type\":\"#Collection(Edm.String)\",\"Colors\":[\"Black\",\"White\"]}";

            await SetupJsonPropertyAndValueDeserializerAndRunProcessPropertyTestAsync(
                payload,
                verificationDelegate: (jsonReader, propertyAndAnnotationCollector, propertyParsingResult, propertyName) =>
                {
                    Assert.Equal("Colors", propertyName);
                    Assert.Equal(
                        PropertyParsingResult.PropertyWithValue,
                        propertyParsingResult);
                    var odataPropertyAnnotations = propertyAndAnnotationCollector.GetODataPropertyAnnotations("Colors");
                    Assert.Contains("odata.type", odataPropertyAnnotations);
                    Assert.Equal("#Collection(Edm.String)", odataPropertyAnnotations["odata.type"]);

                    return TaskUtils.CompletedTask;
                });
        }

        [Fact]
        public async Task ProcessPropertyWithoutValueAsync()
        {
            var payload = "{\"Prop@custom.annotation\":\"jiba jaba\"}";

            await SetupJsonPropertyAndValueDeserializerAndRunProcessPropertyTestAsync(
                payload,
                verificationDelegate: (jsonReader, propertyAndAnnotationCollector, propertyParsingResult, propertyName) =>
                {
                    Assert.Equal("Prop", propertyName);
                    Assert.Equal(
                        PropertyParsingResult.PropertyWithoutValue,
                        propertyParsingResult);
                    Assert.Empty(propertyAndAnnotationCollector.GetCustomScopeAnnotation());
                    Assert.Empty(propertyAndAnnotationCollector.GetCustomPropertyAnnotations("Prop"));

                    return TaskUtils.CompletedTask;
                });
        }

        [Fact]
        public async Task ProcessPropertyAsync_CustomInstanceAnnotationIsSkippedByDefault()
        {
            var payload = "{\"@custom.annotation\":\"jiba jaba\"}";

            await SetupJsonPropertyAndValueDeserializerAndRunProcessPropertyTestAsync(
                payload,
                verificationDelegate: (jsonReader, propertyAndAnnotationCollector, propertyParsingResult, propertyName) =>
                {
                    Assert.Null(propertyName);
                    Assert.Equal(
                        PropertyParsingResult.EndOfObject,
                        propertyParsingResult);

                    return TaskUtils.CompletedTask;
                });
        }

        [Theory]
        [InlineData("custom.annotation")]
        [InlineData("custom.*")]
        [InlineData("*")]
        public async Task ReadTopLevelPropertyAsync_CustomInstanceAnnotationIsNotSkippedBasedOnSetting(string annotationFilter)
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter(annotationFilter);

            var model = InitializeModel();

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)\"," +
                "\"@odata.type\":\"#NS.Customer\",\"Name@custom.annotation\":\"jiba jaba\",\"Name\":\"Sue\"}";

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                async (jsonDeserializer) =>
                {
                    var property = await jsonDeserializer.ReadTopLevelPropertyAsync(null);

                    var resource = Assert.IsType<ODataResourceValue>(property.ODataValue);
                    var nameProperty = Assert.Single(resource.Properties);
                    Assert.Equal("Name", nameProperty.Name);
                    Assert.Equal("Sue", nameProperty.Value);
                    var instanceAnnotation = Assert.Single(nameProperty.InstanceAnnotations);
                    Assert.Equal("custom.annotation", instanceAnnotation.Name);
                    var annotationValue = Assert.IsType<ODataPrimitiveValue>(instanceAnnotation.Value);
                    Assert.Equal("jiba jaba", annotationValue.Value);
                },
                isResponse: false);
        }

        [Fact]
        public async Task ProcessPropertyAsync_UnknownODataInstanceAnnotationIsSkipped()
        {
            var payload = "{\"@odata.unknown\":\"jiba jaba\"}";

            await SetupJsonPropertyAndValueDeserializerAndRunProcessPropertyTestAsync(
                payload,
                verificationDelegate: (jsonReader, propertyAndAnnotationCollector, propertyParsingResult, propertyName) =>
                {
                    Assert.Null(propertyName);
                    Assert.Equal(
                        PropertyParsingResult.EndOfObject,
                        propertyParsingResult);

                    return TaskUtils.CompletedTask;
                });
        }

        [Fact]
        public async Task ProcessODataInstanceAnnotationPropertyAsync()
        {
            var payload = "{\"@odata.id\":\"Customers(1)\"}";

            await SetupJsonPropertyAndValueDeserializerAndRunProcessPropertyTestAsync(
                payload,
                verificationDelegate: (jsonReader, propertyAndAnnotationCollector, propertyParsingResult, propertyName) =>
                {
                    Assert.Equal("odata.id", propertyName);
                    Assert.Equal(
                        PropertyParsingResult.ODataInstanceAnnotation,
                        propertyParsingResult);

                    return TaskUtils.CompletedTask;
                });
        }

        [Fact]
        public async Task ProcessMetadataResponsePropertyAsync()
        {
            var payload = "{\"#RefProp\":true}";

            await SetupJsonPropertyAndValueDeserializerAndRunProcessPropertyTestAsync(
                payload,
                verificationDelegate: (jsonReader, propertyAndAnnotationCollector, propertyParsingResult, propertyName) =>
                {
                    Assert.Equal("#RefProp", propertyName);
                    Assert.Equal(
                        PropertyParsingResult.MetadataReferenceProperty,
                        propertyParsingResult);

                    return TaskUtils.CompletedTask;
                });
        }

        [Fact]
        public async Task ProcessPropertyAnnotationForUnrelatedPropertyAsync()
        {
            var payload = "{\"Colors@odata.type\":\"#Collection(Edm.String)\",\"LuckyNumber\":13}";

            await SetupJsonPropertyAndValueDeserializerAndRunProcessPropertyTestAsync(
                payload,
                verificationDelegate: (jsonReader, propertyAndAnnotationCollector, propertyParsingResult, propertyName) =>
                {
                    Assert.Equal("Colors", propertyName);
                    Assert.Equal(
                        PropertyParsingResult.PropertyWithoutValue,
                        propertyParsingResult);

                    return TaskUtils.CompletedTask;
                });
        }

        [Theory]
        [InlineData("13", true)]
        [InlineData("\"13\"", false)]
        public async Task ReadAndValidateAnnotationAsLongForIeee754CompatibleAsync_ThrowsExceptionForConflictBetweenInputFormatAndParameter(
            string data,
            bool isIeee754Compatible)
        {
            var payload = $"{{\"Data\":{data}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                    payload,
                    EdmCoreModel.Instance,
                    async (jsonDeserializer) =>
                    {
                        await AdvanceReaderToFirstPropertyAsync(jsonDeserializer.JsonReader);
                        await jsonDeserializer.JsonReader.ReadPropertyNameAsync();

                        await jsonDeserializer.ReadAndValidateAnnotationAsLongForIeee754CompatibleAsync("@odata.count");
                    },
                    isIeee754Compatible: isIeee754Compatible));

            Assert.Equal(
                ErrorStrings.ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter("Edm.Int64"),
                exception.Message);
        }

        [Theory]
        [InlineData("{\"odata.deltaLink@custom.annotation\":\"foobar\",\"@odata.deltaLink\":\"http://tempuri.org/deltaLink\"}", "custom.annotation", "odata.deltaLink")]
        [InlineData("{\"odata.deltaLink@odata.unknown\":\"unknown\",\"@odata.deltaLink\":\"http://tempuri.org/deltaLink\"}", "odata.unknown", "odata.deltaLink")]
        [InlineData("{\"custom.annotation@odata.unknown\":\"unknown\",\"@custom.annotation\":\"foobar\"}", "odata.unknown", "custom.annotation")]
        [InlineData("{\"custom.annotation@custom.temp\":\"temp\",\"@custom.annotation\":\"foobar\"}", "custom.temp", "custom.annotation")]
        public async Task ProcessPropertyAsync_ThrowsExceptionForNonODataTypeAnnotationTargetingAnnotation(
            string payload,
            string annotation,
            string targetedAnnotation)
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunProcessPropertyTestAsync(payload));

            Assert.Equal(
                ErrorStrings.ODataJsonDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation(
                    annotation, targetedAnnotation, "odata.type"),
                exception.Message);
        }

        [Theory]
        [InlineData("{\"odata.deltaLink@odata.type\":\"#Edm.String\",\"@odata.deltaLink\":\"http://tempuri.org/deltaLink\"}", "odata.deltaLink")]
        [InlineData("{\"custom.annotation@odata.type\":\"#Edm.String\",\"@custom.annotation\":\"foobar\"}", "custom.annotation")]
        public async Task ProcessPropertyAsync_PermitsODataTypeAnnotationTargetingAnnotation(string payload, string annotationProperty)
        {
            await SetupJsonPropertyAndValueDeserializerAndRunProcessPropertyTestAsync(
                payload,
                verificationDelegate: (jsonReader, propertyAndAnnotationCollector, propertyParsingResult, propertyName) =>
                {
                    var odataPropertyAnnotations = propertyAndAnnotationCollector.GetODataPropertyAnnotations(annotationProperty);
                    Assert.Single(odataPropertyAnnotations);
                    Assert.Contains("odata.type", odataPropertyAnnotations);
                    Assert.Equal("#Edm.String", odataPropertyAnnotations["odata.type"]);

                    return TaskUtils.CompletedTask;
                });
        }

        [Theory]
        [InlineData("{\"odata.unknown@odata.type\":\"#Edm.String\",\"custom.annotation@odata.type\":\"#Edm.String\"}", "odata.unknown")]
        [InlineData("{\"custom.annotation@odata.type\":\"#Edm.String\",\"odata.unknown@odata.type\":\"#Edm.String\"}", "custom.annotation")]
        public async Task ProcessPropertyAsync_ThrowsExceptionForAnnotationTargetingAnnotationWithoutValue(string payload, string targetAnnotation)
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunProcessPropertyTestAsync(payload));

            Assert.Equal(
                ErrorStrings.ODataJsonDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue("odata.type", targetAnnotation),
                exception.Message);
        }

        [Theory]
        [InlineData("{\"Prop@odata.type\":\"#Edm.String\",\"#namespace.name\":\"noname\"}", "PropertyWithoutValue")]
        [InlineData("{\"Prop@odata.unknown\":\"unknown\",\"#namespace.name\":\"noname\"}", "MetadataReferenceProperty")]
        [InlineData("{\"Prop@custom.annotation\":\"foobar\",\"#namespace.name\":\"noname\"}", "PropertyWithoutValue")]
        public async Task ParsePropertyAnnotationFollowedByMetadataReferencePropertyAsync(string payload, string expectedPropertyParsingResult)
        {
            await SetupJsonPropertyAndValueDeserializerAndRunProcessPropertyTestAsync(
                payload,
                verificationDelegate: (jsonReader, propertyAndAnnotationCollector, propertyParsingResult, propertyName) =>
                {
                    // A parameter of PropertyParsingResult type isn't possible due to internal access level
                    Assert.Equal(
                        (PropertyParsingResult)Enum.Parse(typeof(PropertyParsingResult), expectedPropertyParsingResult),
                        propertyParsingResult);

                    return TaskUtils.CompletedTask;
                });
        }

        [Fact]
        public async Task ProcessPropertyAsync_ThrowsExceptionForDuplicateODataAnnotation()
        {
            var payload = "{\"@odata.deltaLink\":\"http://tempuri.org/deltaLink\",\"@odata.deltaLink\":\"http://tempuri.org/deltaLink\"}";
            var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);

            Func<Task> func = () => SetupJsonPropertyAndValueDeserializerAndRunProcessPropertyTestAsync(
                payload,
                propertyAndAnnotationCollector: propertyAndAnnotationCollector);

            await func();
            var exception = await Assert.ThrowsAsync<ODataException>(func);
        }

        [Fact]
        public async Task ProcessPropertyAsync_ThrowsExceptionForDuplicateUnknownODataAnnotation()
        {
            var payload = "{\"@odata.unknown\":\"unknown\",\"@odata.unknown\":\"unknown\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunProcessPropertyTestAsync(payload));

            Assert.Equal(ErrorStrings.DuplicateAnnotationNotAllowed("odata.unknown"),
                exception.Message);
        }

        [Fact]
        public async Task ProcessPropertyAsync_PermitsDuplicateCustomAnnotation()
        {
            var payload = "{\"@custom.annotation\":\"foobar\",\"@custom.annotation\":\"foobar\"}";

            await SetupJsonPropertyAndValueDeserializerAndRunProcessPropertyTestAsync(
                payload,
                verificationDelegate: (jsonReader, propertyAndAnnotationCollector, propertyParsingResult, propertyName) =>
                {
                    Assert.Equal(PropertyParsingResult.EndOfObject, propertyParsingResult);

                    return TaskUtils.CompletedTask;
                });
        }

        [Theory]
        [InlineData("{\"odata.deltaLink@odata.type\":\"#Edm.String\",\"odata.deltaLink@odata.type\":\"#Edm.String\"}", "odata.deltaLink")]
        [InlineData("{\"custom.annotation@odata.type\":\"#Edm.String\",\"custom.annotation@odata.type\":\"#Edm.String\"}", "custom.annotation")]
        public async Task ProcessPropertyAsync_ThrowsExceptionForDuplicateODataTypeAnnotationTargetingAnnotation(string payload, string annotationProperty)
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunProcessPropertyTestAsync(payload));

            Assert.Equal(
                ErrorStrings.DuplicateAnnotationForInstanceAnnotationNotAllowed("odata.type", annotationProperty),
                exception.Message);
        }

        public static IEnumerable<object[]> GetReadTopLevelPrimitivePropertyTestData()
        {
            return new List<object[]>
            {
                new object [] { "\"@odata.type\":\"#Int32\",\"value\":13", 13, EdmPrimitiveTypeKind.Int32, false },
                new object [] { "\"@odata.type\":\"#Date\",\"value\":\"2021-07-13\"", new Date(2021, 7, 13), EdmPrimitiveTypeKind.Date, false },
                new object [] { "\"@odata.type\":\"#Double\",\"value\":3.14159265359", 3.14159265359, EdmPrimitiveTypeKind.Double, false },
                new object [] { "\"@odata.type\":\"#Boolean\",\"value\":true", true, EdmPrimitiveTypeKind.Boolean, false },
                new object [] { "\"@odata.type\":\"#String\",\"value\":\"Sue\"", "Sue", EdmPrimitiveTypeKind.String, false },
                new object [] { "\"@odata.type\":\"#Int64\",\"value\":6078747774547", 6078747774547L, EdmPrimitiveTypeKind.Int64, false },
                new object [] { "\"@odata.type\":\"#Decimal\",\"value\":7654321", 7654321M, EdmPrimitiveTypeKind.Decimal, false },
                new object [] { "\"@odata.type\":\"#Int64\",\"value\":\"6078747774547\"", 6078747774547L, EdmPrimitiveTypeKind.Int64, true },
                new object [] { "\"@odata.type\":\"#Decimal\",\"value\":\"7654321\"", 7654321M, EdmPrimitiveTypeKind.Decimal, true }
            };
        }

        [Theory]
        [MemberData(nameof(GetReadTopLevelPrimitivePropertyTestData))]
        public async Task ReadTopLevelPrimitivePropertyAsync_ReturnsExpectedResult(
            string valuePart,
            object expected,
            EdmPrimitiveTypeKind primitiveTypeKind,
            bool isIeee754Compatible)
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.AddStructuralProperty("TempProperty", primitiveTypeKind);

            var payload = $"{{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/TempProperty\",{valuePart}}}";

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                async (jsonDeserializer) =>
                {
                    var property = await jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type);

                    var primitiveValue = Assert.IsType<ODataPrimitiveValue>(property.ODataValue);
                    Assert.Equal(expected, primitiveValue.Value);
                },
                isIeee754Compatible: isIeee754Compatible);
        }

        [Fact]
        public async Task Test2Async()
        {
            this.messageReaderSettings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;

            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var addressComplexType = model.SchemaElements.Single(d => d.Name.Equals("Address")) as EdmComplexType;
            var edmProperty = customerEntityType.AddStructuralProperty(
                "TempProperty",
                new EdmComplexTypeReference(addressComplexType, true));

            var valuePart = "\"@odata.type\":\"#NS.Address\",\"Street\":\"S1\",\"City\":\"C1\",\"TempProperty\":17.30";
            var payload = $"{{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/TempProperty\",{valuePart}}}";

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                async (jsonDeserializer) =>
                {
                    var property = await jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type);
                });
        }

        public static IEnumerable<object[]> GetReadTopLevelPrimitivePropertyWithConflictingInputFormatAndParameterTestData()
        {
            return new List<object[]>
            {
                new object [] { "\"@odata.type\":\"#Int64\",\"value\":6078747774547", EdmPrimitiveTypeKind.Int64, true },
                new object [] { "\"@odata.type\":\"#Decimal\",\"value\":7654321", EdmPrimitiveTypeKind.Decimal, true },
                new object [] { "\"@odata.type\":\"#Int64\",\"value\":\"6078747774547\"", EdmPrimitiveTypeKind.Int64, false },
                new object [] { "\"@odata.type\":\"#Decimal\",\"value\":\"7654321\"", EdmPrimitiveTypeKind.Decimal, false }
            };
        }

        [Theory]
        [MemberData(nameof(GetReadTopLevelPrimitivePropertyWithConflictingInputFormatAndParameterTestData))]
        public async Task ReadTopLevelPrimitivePropertyAsync_ThrowsExceptionForConflictingInputFormatAndParameter(
            string valuePart,
            EdmPrimitiveTypeKind primitiveTypeKind,
            bool isIeee754Compatible)
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.AddStructuralProperty("TempProperty", primitiveTypeKind);

            var payload = $"{{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/TempProperty\",{valuePart}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                    payload,
                    model,
                    (jsonDeserializer) => jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type),
                    isIeee754Compatible: isIeee754Compatible));

            Assert.Equal(
                ErrorStrings.ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter(edmProperty.Type.FullName()),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelPrimitivePropertyAsync_ThrowsExceptionForValueAsJsonObject()
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.AddStructuralProperty("TempProperty", EdmPrimitiveTypeKind.Int32);

            var valuePart = "\"value\":{}";
            var payload = $"{{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/TempProperty\",{valuePart}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                    payload,
                    model,
                    (jsonDeserializer) => jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type)));

            Assert.Equal(
                    ErrorStrings.JsonReaderExtensions_UnexpectedNodeDetectedWithPropertyName("PrimitiveValue", "StartObject", "value"),
                    exception.Message);
        }

        public static IEnumerable<object[]> GetTopLevelPrimitivePropertyUnsupportedScenariosTestData()
        {
            return new List<object[]>
            {
                new object[] {
                    "\"@odata.id\":\"#1\",\"value\":13",
                    ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.id")
                },
                new object[] {
                    "\"#action\":\"Action\"",
                    ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty("#action")
                },
                new object[] {
                    "\"value@custom.annotation\":\"foobar\"",
                    ErrorStrings.ODataJsonPropertyAndValueDeserializer_TopLevelPropertyAnnotationWithoutProperty("value")
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetTopLevelPrimitivePropertyUnsupportedScenariosTestData))]
        public async Task ReadTopLevelPrimitivePropertyAsync_ThrowsExceptionForUnsupportedScenarios(string valuePart, string exceptionMessage)
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.AddStructuralProperty("TempProperty", EdmPrimitiveTypeKind.Int32);

            var payload = $"{{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/TempProperty\",{valuePart}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                    payload,
                    model,
                    (jsonDeserializer) => jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type)));

            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("*")]
        public async Task ReadTopLevelPrimitivePropertyAsync_PermitsCustomAnnotation(string annotationFilter)
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter(annotationFilter);

            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.AddStructuralProperty("TempProperty", EdmPrimitiveTypeKind.Int32);

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/TempProperty\"," +
                    "\"@custom.annotation\":\"foobar\",\"value\":13}";

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                async (jsonDeserializer) =>
                {
                    var property = await jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type);

                    var primitiveValue = Assert.IsType<ODataPrimitiveValue>(property.ODataValue);
                    Assert.Equal(13, primitiveValue.Value);
                    if (!string.IsNullOrEmpty(annotationFilter))
                    {
                        var instanceAnnotation = Assert.Single(property.InstanceAnnotations);
                        Assert.Equal("custom.annotation", instanceAnnotation.Name);
                        var annotationValue = Assert.IsType<ODataPrimitiveValue>(instanceAnnotation.Value);
                        Assert.Equal("foobar", annotationValue.Value);
                    }
                });
        }

        [Theory]
        [InlineData("")]
        [InlineData("\"@custom.annotation\":\"foobar\",\"@odata.type\":\"#Collection(Edm.String)\",")]
        public async Task ReadTopLevelPrimitiveCollectionPropertyAsync(string annotationsPart)
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.AddStructuralProperty(
                "TempProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/TempProperty\"," +
                annotationsPart +
                "\"value\":[\"foo\",\"bar\"]}";

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                async (jsonDeserializer) =>
                {
                    var property = await jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type);
                    var collectionValue = Assert.IsType<ODataCollectionValue>(property.Value);
                    Assert.Equal(2, collectionValue.Items.Count());
                    Assert.Equal("foo", collectionValue.Items.First());
                    Assert.Equal("bar", collectionValue.Items.Last());
                });
        }

        [Fact]
        public async Task ReadTopLevelTypeDefinitionPropertyAsync_ReturnsExpectedResult()
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var moneyTypeDefinition = new EdmTypeDefinition("NS", "Money", EdmPrimitiveTypeKind.Decimal);
            model.AddElement(moneyTypeDefinition);
            var edmProperty = customerEntityType.AddStructuralProperty(
                "TempProperty",
                new EdmTypeDefinitionReference(moneyTypeDefinition, false));

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/TempProperty\"," +
                "\"value\":1730}";

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                async (jsonDeserializer) =>
                {
                    var tempProperty = await jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type);

                    Assert.Equal(1730M, tempProperty.Value);
                });
        }

        [Theory]
        [InlineData("\"value\":null")]
        [InlineData("\"@odata.null\":true")]
        public async Task ReadTopLevelNullPropertyAsync_ReturnsExpectedResult(string valuePart)
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.Properties().Single(d => d.Name.Equals("Name")) as EdmProperty;

            var payload = $"{{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/Name\",{valuePart}}}";

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                async (jsonDeserializer) =>
                {
                    var property = await jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type);

                    Assert.IsType<ODataNullValue>(property.ODataValue);
                });
        }

        [Fact]
        public async Task ReadTopLevelResourcePropertyAsync_ReturnsExpectedResult()
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.Properties().First(d => d.Name.Equals("ShippingAddress"));

            var valuePart = "\"@odata.type\":\"#NS.Address\",\"Street\":\"S2\",\"City\":\"C2\"";
            var payload = $"{{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/ShippingAddress\",{valuePart}}}";

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                async (jsonDeserializer) =>
                {
                    var property = await jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type);

                    var resource = Assert.IsType<ODataResourceValue>(property.ODataValue);
                    Assert.Equal(2, resource.Properties.Count());
                    var properties = resource.Properties.ToList();
                    Assert.Equal("Street", properties[0].Name);
                    Assert.Equal("S2", properties[0].Value);
                    Assert.Equal("City", properties[1].Name);
                    Assert.Equal("C2", properties[1].Value);
                });
        }

        [Fact]
        public async Task ReadTopLevelResourcePropertyAsync_UndeclaredNumericPropertyOfUnspecifiedTypeReadAsDecimalThenConvertedToDouble()
        {
            this.messageReaderSettings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;

            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.Properties().First(d => d.Name.Equals("ShippingAddress"));

            var valuePart = "\"@odata.type\":\"#NS.Address\",\"TempProperty\":17.30";
            var payload = $"{{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/ShippingAddress\",{valuePart}}}";

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                async (jsonDeserializer) =>
                {
                    var property = await jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type);

                    var resource = Assert.IsType<ODataResourceValue>(property.ODataValue);
                    var tempProperty = Assert.Single(resource.Properties);
                    Assert.Equal("TempProperty", tempProperty.Name);
                    Assert.Equal(17.3, Assert.IsType<double>(tempProperty.Value));
                });
        }

        public static IEnumerable<object[]> GetTopLevelResourcePropertyUnsupportedScenariosTestData()
        {
            yield return new object[]
            {
                "\"#action\":\"Action\"",
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty("#action")
            };

            yield return new object[]
            {
                "\"Street@custom.annotation\":\"foobar\"",
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_ResourceValuePropertyAnnotationWithoutProperty("Street")
            };

            yield return new object[]
            {
                "\"@odata.id\":\"#1\"",
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.id")
            };

            yield return new object[]
            {
                "\"Street\":\"S1\",\"@odata.type\":\"#Edm.Int32\"",
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_ResourceTypeAnnotationNotFirst
            };
        }

        [Theory]
        [MemberData(nameof(GetTopLevelResourcePropertyUnsupportedScenariosTestData))]
        public async Task ReadTopLevelResourcePropertyAsync_ThrowsExceptionForUnsupportedScenarios(string valuePart, string exceptionMessage)
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.Properties().First(d => d.Name.Equals("ShippingAddress"));

            var payload = $"{{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/ShippingAddress\",{valuePart}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                    payload,
                    model,
                    (jsonDeserializer) => jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type)));

            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelResourcePropertyAsync_PermitsODataTypePropertyAnnotation()
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.Properties().First(d => d.Name.Equals("ShippingAddress"));

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/ShippingAddress\"," +
                "\"Street@odata.type\":\"#Edm.String\"," +
                "\"Street\":\"S1\"," +
                "\"City@odata.type\":\"#Edm.String\"," +
                "\"City\":\"C1\"}";

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                async (jsonDeserializer) =>
                {
                    var property = await jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type);

                    var resource = Assert.IsType<ODataResourceValue>(property.ODataValue);
                    Assert.Equal(2, resource.Properties.Count());
                    var streetProperty = Assert.Single(resource.Properties.Where(d => d.Name.Equals("Street")));
                    var cityProperty = Assert.Single(resource.Properties.Where(d => d.Name.Equals("City")));
                    Assert.Equal("S1", streetProperty.Value);
                    Assert.Equal("C1", cityProperty.Value);
                });
        }

        [Fact]
        public async Task ReadTopLevelResourcePropertyAsync_ThrowsExceptionForODataTypePropertyAnnotationValueIsNull()
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.Properties().First(d => d.Name.Equals("ShippingAddress"));

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/ShippingAddress\"," +
                "\"Street@odata.type\":null," +
                "\"Street\":\"S1\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                (jsonDeserializer) => jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type)));

            Assert.Equal(
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_InvalidTypeName(null),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelResourcePropertyAsync_ThrowsExceptionForUnexpectedODataPropertyAnnotation()
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.Properties().First(d => d.Name.Equals("ShippingAddress"));

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/ShippingAddress\"," +
                "\"Street@odata.etag\":\"etag\"," +
                "\"Street\":\"S1\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                (jsonDeserializer) => jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type)));

            Assert.Equal(
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.etag"),
                exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("*")]
        public async Task ReadTopLevelResourcePropertyAsync_PermitsCustomAnnotation(string annotationFilter)
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter(annotationFilter);

            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.Properties().First(d => d.Name.Equals("ShippingAddress"));

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/ShippingAddress\"," +
                "\"custom.annotation@odata.type\":\"#Edm.String\",\"custom.annotation\":\"foobar\"}";

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                async (jsonDeserializer) =>
                {
                    var property = await jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type);

                    var resource = Assert.IsType<ODataResourceValue>(property.ODataValue);
                    if (!string.IsNullOrEmpty(annotationFilter))
                    {
                        var instanceAnnotation = Assert.Single(resource.InstanceAnnotations);
                        Assert.Equal("custom.annotation", instanceAnnotation.Name);
                        var annotationValue = Assert.IsType<ODataPrimitiveValue>(instanceAnnotation.Value);
                        Assert.Equal("foobar", annotationValue.Value);
                    }
                });
        }

        [Fact]
        public async Task ReadTopLevelEnumCollectionPropertyAsync_ReturnsExpectedResult()
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var segmentEnumType = model.SchemaElements.Single(d => d.Name.Equals("Segment")) as EdmEnumType;
            var edmProperty = customerEntityType.AddStructuralProperty("TargetSegments",
                new EdmCollectionTypeReference(
                    new EdmCollectionType(
                        new EdmEnumTypeReference(
                            segmentEnumType, true))));

            var valuePart = "\"@odata.type\":\"#Collection(NS.Segment)\"," +
                "\"value\":[\"Retail\",\"Wholesale\"]";
            var payload = $"{{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(NS.Segment)\",{valuePart}}}";

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                async (jsonDeserializer) =>
                {
                    var property = await jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type);

                    var collectionValue = Assert.IsType<ODataCollectionValue>(property.ODataValue);
                    Assert.Equal(2, collectionValue.Items.Count());
                    var retailEnumValue = Assert.IsType<ODataEnumValue>(collectionValue.Items.First());
                    var wholesaleEnumValue = Assert.IsType<ODataEnumValue>(collectionValue.Items.Last());
                    Assert.Equal("Retail", retailEnumValue.Value);
                    Assert.Equal("Wholesale", wholesaleEnumValue.Value);
                });
        }

        [Fact]
        public async Task ReadTopLevelEnumCollectionPropertyAsync_ThrowsExceptionForValueAsJsonObject()
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var segmentEnumType = model.SchemaElements.Single(d => d.Name.Equals("Segment")) as EdmEnumType;
            var edmProperty = customerEntityType.AddStructuralProperty("TargetSegment", new EdmEnumTypeReference(segmentEnumType, true));

            var valuePart = "\"@odata.type\":\"#NS.Segment\",\"value\":{}";
            var payload = $"{{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/TargetSegment\",{valuePart}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                (jsonDeserializer) => jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type)));

            Assert.Equal(
                ErrorStrings.JsonReaderExtensions_UnexpectedNodeDetectedWithPropertyName("PrimitiveValue", "StartObject", "value"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelSpatialPropertyAsync_ReturnsExpectedResult()
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.AddStructuralProperty("Location",
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true));

            var valuePart = "\"@odata.type\":\"#Edm.GeographyPoint\"," +
                "\"value\":{\"type\":\"Point\",\"coordinates\":[-110.0,33.1],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}";
            var payload = $"{{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/Location\",{valuePart}}}";

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                async (jsonDeserializer) =>
                {
                    var property = await jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type);

                    var primitiveValue = Assert.IsType<ODataPrimitiveValue>(property.ODataValue);
                    var geographyPoint = Assert.IsAssignableFrom<GeographyPoint>(primitiveValue.Value);

                    Assert.Equal(4326, geographyPoint.CoordinateSystem.EpsgId);
                    Assert.Equal(-110.0d, geographyPoint.Longitude);
                    Assert.Equal(33.1d, geographyPoint.Latitude);
                });
        }

        [Theory]
        [InlineData(null)]
        [InlineData("*")]
        public async Task ReadTopLevel6xNullPropertyAsync_PermitsCustomAnnotation(string annotationFilter)
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter(annotationFilter);

            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.AddStructuralProperty("TempProperty", EdmPrimitiveTypeKind.String);

            var valuePart = "\"@odata.null\":true,\"@custom.annotation\":\"foobar\"";
            var payload = $"{{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/TempProperty\",{valuePart}}}";

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                async (jsonDeserializer) =>
                {
                    var property = await jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type);

                    Assert.IsType<ODataNullValue>(property.ODataValue);
                });
        }

        public static IEnumerable<object[]> GetTopLevel6xNullPropertyExceptionScenariosTestData()
        {
            yield return new object[]
            {
                "\"@odata.null\":true,\"value\":\"foobar\"",
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_NoPropertyAndAnnotationAllowedInNullPayload("value")
            };

            yield return new object[]
            {
                "\"@odata.null\":true,\"TempProperty@odata.type\":\"#Edm.String\"",
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedODataPropertyAnnotation("odata.type")
            };

            yield return new object[]
            {
                "\"@odata.null\":true,\"TempProperty@custom.annotation\":\"foobar\"",
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_TopLevelPropertyAnnotationWithoutProperty("TempProperty")
            };

            yield return new object[]
            {
                "\"@odata.null\":true,\"#action\":\"Action\"",
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty("#action")
            };

            yield return new object[]
            {
                "\"@odata.null\":true,\"@odata.type\":\"#Edm.String\"",
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.type")
            };
        }

        [Theory]
        [MemberData(nameof(GetTopLevel6xNullPropertyExceptionScenariosTestData))]
        public async Task ReadTopLevel6xNullPropertyAsync_ThrowsExceptionForDisallowedPropertyAndAnnotationInPayload(string valuePart, string exceptionMessage)
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.AddStructuralProperty("TempProperty", EdmPrimitiveTypeKind.String);

            var payload = $"{{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/TempProperty\",{valuePart}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                    payload,
                    model,
                    (jsonDeserializer) => jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type)));

            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Theory]
        [InlineData("false")]
        [InlineData("13")]
        public async Task ReadTopLevel6xNullPropertyAsync_ThrowsExceptionForInvalidValue(string value)
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.AddStructuralProperty("TempProperty", EdmPrimitiveTypeKind.String);

            var valuePart = $"\"@odata.null\":{value}";
            var payload = $"{{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/TempProperty\",{valuePart}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                    payload,
                    model,
                    (jsonDeserializer) => jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type)));

            Assert.Equal(
                ErrorStrings.ODataJsonReaderUtils_InvalidValueForODataNullAnnotation("odata.null", "true"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelPropertyAsync_ThrowsExceptionForConflictingODataTypeAnnotationInPayload()
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.AddStructuralProperty("TempProperty", EdmPrimitiveTypeKind.Int32);

            var valuePart = "\"@odata.type\":\"#Collection(Edm.Int32)\",\"value\":42}";
            var payload = $"{{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/TempProperty\",{valuePart}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                    payload,
                    model,
                    (jsonDeserializer) => jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type)));

            Assert.Equal(
                ErrorStrings.ValidationUtils_IncorrectTypeKind("Collection(Edm.Int32)", "Primitive", "Collection"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelPropertyAsync_ThrowsExceptionForUnexpectedODataPropertyAnnotation()
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.AddStructuralProperty("TempProperty", EdmPrimitiveTypeKind.Int32);

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/TempProperty\"," +
                "\"value@odata.type\":\"#Edm.Int32\"," +
                "\"value\":42}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                    payload,
                    model,
                    (jsonDeserializer) => jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type)));

            Assert.Equal(
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedODataPropertyAnnotation("odata.type"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelPropertyAsync_ThrowsExceptionForODataTypeAnnotationAfterValueProperty()
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.AddStructuralProperty("TempProperty", EdmPrimitiveTypeKind.Int32);
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/TempProperty\"," +
                "\"value\":42," +
                "\"@odata.type\":\"#Edm.Int32\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                    payload,
                    model,
                    (jsonDeserializer) => jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type)));

            Assert.Equal(
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_TypePropertyAfterValueProperty("odata.type", "value"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelPropertyAsync_ThrowsExceptionForInvalidTopLevelPropertyPayload()
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.AddStructuralProperty("TempProperty", EdmPrimitiveTypeKind.Int32);
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/TempProperty\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                    payload,
                    model,
                    (jsonDeserializer) => jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type)));

            Assert.Equal(
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_InvalidTopLevelPropertyPayload,
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelPropertyAsync_ThrowsExceptionForInvalidTopLevelPropertyName()
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.AddStructuralProperty("TempProperty", EdmPrimitiveTypeKind.Int32);
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/TempProperty\"," +
                "\"foobar\":42}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                    payload,
                    model,
                    (jsonDeserializer) => jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type)));

            Assert.Equal(
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_InvalidTopLevelPropertyName("foobar", "value"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelPrimitiveCollectionPropertyAsync_ThrowsExceptionForValueAsJsonObject()
        {
            var model = this.InitializeModel();
            var customerEntityType = model.SchemaElements.Single(d => d.Name.Equals("Customer")) as EdmEntityType;
            var edmProperty = customerEntityType.AddStructuralProperty(
                "TempProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/TempProperty\"," +
                "\"@odata.type\":\"#Collection(Edm.String)\",\"value\":{\"foo\":\"bar\"}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                    payload,
                    model,
                    (jsonDeserializer) => jsonDeserializer.ReadTopLevelPropertyAsync(edmProperty.Type)));

            Assert.Equal(
                ErrorStrings.JsonReaderExtensions_UnexpectedNodeDetectedWithPropertyName("StartArray", "StartObject", "value"),
                exception.Message);
        }

        [Fact]
        public async Task ReadUntypedValueAsync()
        {
            var model = this.InitializeModel();
            var payload = "\"foobar\"";

            await SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                payload,
                model,
                async (jsonDeserializer) =>
                {
                    await jsonDeserializer.JsonReader.ReadAsync();
                    var nonEntityValue = await jsonDeserializer.ReadNonEntityValueAsync(
                        payloadTypeName: "Edm.Untyped",
                        expectedValueTypeReference: EdmCoreModel.Instance.GetUntyped(),
                        propertyAndAnnotationCollector: null,
                        collectionValidator: null,
                        validateNullValue: true,
                        isTopLevelPropertyValue: false,
                        insideResourceValue: false,
                        propertyName: "UntypedProp",
                        isDynamicProperty: false);

                    var untypedValue = Assert.IsType<ODataUntypedValue>(nonEntityValue);
                    Assert.Equal("\"foobar\"", untypedValue.RawValue);
                });
        }

        [Fact]
        public async Task ReadNonEntityValueAsync_ThrowsExceptionForResourcePropertyValueNotJsonObject()
        {
            var model = this.InitializeModel();
            var addressComplexType = model.SchemaElements.Single(d => d.Name.Equals("Address")) as EdmComplexType;
            var payload = "\"S1, C1\"";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
                    payload,
                    model,
                    async (jsonDeserializer) =>
                    {
                        await jsonDeserializer.JsonReader.ReadAsync();
                        await jsonDeserializer.ReadNonEntityValueAsync(
                            payloadTypeName: "NS.Address",
                            expectedValueTypeReference: new EdmComplexTypeReference(addressComplexType, true),
                            propertyAndAnnotationCollector: null,
                            collectionValidator: null,
                            validateNullValue: false,
                            isTopLevelPropertyValue: false,
                            insideResourceValue: false,
                            propertyName: "HomeAddress",
                            isDynamicProperty: false);
                    }));

            Assert.Equal(
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_ODataResourceExpectedForProperty("HomeAddress", "PrimitiveValue", "NS.Address"),
                exception.Message);
        }

        #endregion Async Tests

        private ODataJsonInputContext CreateJsonInputContext(string payload)
        {
            return this.CreateJsonInputContext(payload, this.edmModel);
        }

        private ODataJsonInputContext CreateJsonInputContext(
            string payload,
            IEdmModel model,
            bool isResponse = true,
            bool isAsync = false,
            bool isIeee754Compatible = false)
        {
            var messageInfo = new ODataMessageInfo
            {
                IsResponse = isResponse,
                MediaType = CreateMediaType(isIeee754Compatible),
                IsAsync = isAsync,
                Model = model,
            };

            return new ODataJsonInputContext(
                new StringReader(payload), messageInfo, this.messageReaderSettings);
        }

        private ODataMediaType CreateMediaType(bool isIeee754Compatible = false)
        {
            return new ODataMediaType(
                MimeConstants.MimeApplicationType,
                MimeConstants.MimeJsonSubType,
                new[]{
                    new KeyValuePair<string, string>(
                        MimeConstants.MimeMetadataParameterName,
                        MimeConstants.MimeMetadataParameterValueMinimal),
                    new KeyValuePair<string, string>(
                        MimeConstants.MimeStreamingParameterName,
                        MimeConstants.MimeParameterValueTrue),
                    new KeyValuePair<string, string>(
                        MimeConstants.MimeIeee754CompatibleParameterName,
                        isIeee754Compatible ? MimeConstants.MimeParameterValueTrue : MimeConstants.MimeParameterValueFalse)
                });
        }

        private object ReadODataTypePropertyAnnotation(IJsonReader jsonReader, string name)
        {
            Assert.Equal(ODataAnnotationNames.ODataType, name);
            return jsonReader.ReadStringValue();
        }

        private void RunPropertyParsingTest(
            string jsonInput,
            PropertyParsingResult expectedPropertyParsingResult,
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

            using (ODataJsonInputContext inputContext = this.CreateJsonInputContext(jsonInput))
            {
                ODataJsonDeserializer deserializer = new ODataJsonPropertyAndValueDeserializer(inputContext);
                AdvanceReaderToFirstProperty(deserializer.JsonReader);

                deserializer.ProcessProperty(
                    propertyAndAnnotationCollector,
                    (propertyName) => readPropertyAnnotationValue(deserializer.JsonReader, propertyName),
                    (propertyParsingResult, propertyName) =>
                    {
                        if (propertyParsingResult != PropertyParsingResult.PropertyWithoutValue && deserializer.JsonReader.NodeType == JsonNodeType.Property)
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

        private static void AdvanceReaderToFirstProperty(BufferingJsonReader bufferingJsonReader)
        {
            // Read start and then over the object start.
            bufferingJsonReader.Read();
            bufferingJsonReader.Read();
            Assert.Equal(JsonNodeType.Property, bufferingJsonReader.NodeType);
        }

        private async Task SetupJsonPropertyAndValueDeserializerAndRunTestAsync(
            string payload,
            IEdmModel model,
            Func<ODataJsonPropertyAndValueDeserializer, Task> func,
            bool isResponse = true,
            bool isIeee754Compatible = false)
        {
            using (var jsonInputContext = CreateJsonInputContext(
                payload,
                model,
                isResponse: isResponse,
                isAsync: true,
                isIeee754Compatible: isIeee754Compatible))
            {
                var jsonDeserializer = new ODataJsonPropertyAndValueDeserializer(jsonInputContext);

                await func(jsonDeserializer);
            }
        }

        private async Task SetupJsonPropertyAndValueDeserializerAndRunProcessPropertyTestAsync(
            string payload,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = null,
            Func<IJsonReader, string, Task<object>> readPropertyAnnotationValueDelegate = null,
            Func<IJsonReader, Task> setupDelegate = null,
            Func<IJsonReader, PropertyAndAnnotationCollector, PropertyParsingResult, string, Task> verificationDelegate = null)
        {
            using (var jsonInputContext = CreateJsonInputContext(payload, EdmCoreModel.Instance, isAsync: true))
            {
                var jsonDeserializer = new ODataJsonPropertyAndValueDeserializer(jsonInputContext);

                if (readPropertyAnnotationValueDelegate == null)
                {
                    readPropertyAnnotationValueDelegate = (jsonReader, propertyName) => jsonReader.ReadPrimitiveValueAsync();
                }

                if (setupDelegate == null)
                {
                    setupDelegate = async (jsonReader) =>
                    {
                        await jsonReader.ReadAsync(); // Position the reader on the first node
                        await jsonReader.ReadAsync(); // Read StartObject
                    };
                }

                if (verificationDelegate == null)
                {
                    verificationDelegate = (arg1, arg2, arg3, arg4) => TaskUtils.CompletedTask;
                }

                if (propertyAndAnnotationCollector == null)
                {
                    propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
                }

                await setupDelegate(jsonDeserializer.JsonReader);

                await jsonDeserializer.ProcessPropertyAsync(
                    propertyAndAnnotationCollector: propertyAndAnnotationCollector,
                    readPropertyAnnotationValueDelegate: (propertyName) => readPropertyAnnotationValueDelegate(
                        jsonDeserializer.JsonReader,
                        propertyName),
                    handlePropertyDelegate: (propertyParsingResult, propertyName) => verificationDelegate(
                        jsonDeserializer.JsonReader,
                        propertyAndAnnotationCollector,
                        propertyParsingResult,
                        propertyName));
            }
        }

        private static async Task AdvanceReaderToFirstPropertyAsync(BufferingJsonReader bufferingJsonReader)
        {
            await bufferingJsonReader.ReadAsync(); // Position the reader on the first node
            await bufferingJsonReader.ReadAsync(); // Read StartObject
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

        private EdmModel InitializeModel()
        {
            var model = new EdmModel();

            var segmentEnumType = new EdmEnumType("NS", "Segment");
            segmentEnumType.AddMember("Retail", new EdmEnumMemberValue(0));
            segmentEnumType.AddMember("Wholesale", new EdmEnumMemberValue(1));
            model.AddElement(segmentEnumType);

            var addressComplexType = new EdmComplexType("NS", "Address");
            addressComplexType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            addressComplexType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            model.AddElement(addressComplexType);

            var customerEntityType = new EdmEntityType("NS", "Customer");
            customerEntityType.AddKeys(customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            customerEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            customerEntityType.AddStructuralProperty("ShippingAddress", new EdmComplexTypeReference(addressComplexType, true));
            model.AddElement(customerEntityType);

            var defaultContainer = new EdmEntityContainer("NS", "Default");
            defaultContainer.AddEntitySet("Customers", customerEntityType);
            model.AddElement(defaultContainer);

            return model;
        }
    }
}
