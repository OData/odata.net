//---------------------------------------------------------------------
// <copyright file="ODataJsonLightEntryAndFeedSerializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;
using ErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.JsonLight
{
    public class ODataJsonLightEntryAndFeedSerializerTests
    {
        private Uri serviceDocumentUri = new Uri("http://odata.org/test/");

        [Fact]
        public void SerializedNavigationPropertyShouldIncludeAssociationLinkUrl()
        {
            var jsonResult = this.SerializeJsonFragment(serializer =>
                serializer.WriteNavigationLinkMetadata(
                    new ODataNavigationLink
                    {
                        Name = "NavigationProperty",
                        AssociationLinkUrl = new Uri("http://example.com/association"),
                        Url = new Uri("http://example.com/navigation")
                    },
                    new DuplicatePropertyNamesChecker(false, true)));

            jsonResult.Should().Contain("NavigationProperty@odata.associationLink\":\"http://example.com/association");
        }

        [Fact]
        public void SerializedNavigationPropertyShouldIncludeNavigationLinkUrl()
        {
            var jsonResult = this.SerializeJsonFragment(serializer =>
                serializer.WriteNavigationLinkMetadata(
                    new ODataNavigationLink
                    {
                        Name = "NavigationProperty",
                        AssociationLinkUrl = new Uri("http://example.com/association"),
                        Url = new Uri("http://example.com/navigation")
                    },
                    new DuplicatePropertyNamesChecker(false, true)));

            jsonResult.Should().Contain("NavigationProperty@odata.navigationLink\":\"http://example.com/navigation");
        }

        [Fact]
        public void WriteOperationsOnRequestsShouldThrow()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var context = this.CreateJsonLightOutputContext(stream, writingResponse: false);
                var serializer = new ODataJsonLightEntryAndFeedSerializer(context);
                Action test = () => serializer.WriteOperations(new ODataOperation[] { new ODataAction { Metadata = new Uri("#foo", UriKind.Relative) } }, /*isAction*/true);
                test.ShouldThrow<ODataException>().WithMessage(ErrorStrings.WriterValidationUtils_OperationInRequest("#foo"));
            }
        }

        [Fact]
        public void ShouldThrowWhenWritingRelativeActionsTargetWhenMetadataDocumentUriIsNotSet()
        {
            IEnumerable<ODataOperation> operations = new ODataOperation[] { new ODataAction { Metadata = new Uri("#foo", UriKind.Relative), Target = new Uri("foo", UriKind.Relative) } };
            Action test = () => this.WriteOperationsAndValidatePayload(operations, null, true, false /*setMetadataDocumentUri*/);
            test.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataOutputContext_MetadataDocumentUriMissing);
        }

        [Fact]
        public void ShouldWriteRelativeActionTargetOnWireMetadataDocumentUriIsSetAndMetadataAnnotationIsWritten()
        {
            var operations = new ODataOperation[] { new ODataAction { Metadata = new Uri("#foo", UriKind.Relative), Target = new Uri("foo", UriKind.Relative) } };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata\",\"#foo\":{\"target\":\"foo\"}}";
            this.WriteOperationsAndValidatePayload(operations, expectedPayload, /*isAction*/ true, true /*setMetadataDocumentUri*/, true /*writeMetadataAnnotation*/);
        }

        [Fact]
        public void ShouldWriteAbsoluteActionTargetOnWireWhenMetadataDocumentUriIsSetButMetadataAnnotationIsNotWritten()
        {
            var operations = new ODataOperation[] { new ODataAction { Metadata = new Uri("#foo", UriKind.Relative), Target = new Uri("foo", UriKind.Relative) } };
            const string expectedPayload = "{\"#foo\":{\"target\":\"http://odata.org/test/foo\"}}";
            this.WriteOperationsAndValidatePayload(operations, expectedPayload, /*isAction*/ true, true /*setMetadataDocumentUri*/, false /*writeMetadataAnnotation*/);
        }

        [Fact]
        public void ShouldWriteOneActionWithNoTitleNoTarget()
        {
            var operations = new ODataOperation[] { new ODataAction { Metadata = new Uri("#foo", UriKind.Relative) } };
            const string expectedPayload = "{\"#foo\":{}}";
            this.WriteOperationsAndValidatePayload(operations, expectedPayload);
        }

        [Fact]
        public void ShouldWriteOneActionWithOneTarget()
        {
            var operations = new ODataOperation[] { new ODataAction { Metadata = new Uri("#foo", UriKind.Relative), Target = new Uri("http://www.example.com/foo") } };
            const string expectedPayload = "{\"#foo\":{\"target\":\"http://www.example.com/foo\"}}";
            this.WriteOperationsAndValidatePayload(operations, expectedPayload);
        }

        [Fact]
        public void ShouldWriteOneActionWithOneTitle()
        {
            var operations = new ODataOperation[] { new ODataAction { Metadata = new Uri("#foo", UriKind.Relative), Title = "foo" } };
            const string expectedPayload = "{\"#foo\":{\"title\":\"foo\"}}";
            this.WriteOperationsAndValidatePayload(operations, expectedPayload);
        }

        [Fact]
        public void ShouldWriteOneActionWithTitleAndTarget()
        {
            var operations = new ODataOperation[] { new ODataAction { Metadata = new Uri("#foo", UriKind.Relative), Title = "foo", Target = new Uri("http://www.example.com/foo") } };
            const string expectedPayload = "{\"#foo\":{\"title\":\"foo\",\"target\":\"http://www.example.com/foo\"}}";
            this.WriteOperationsAndValidatePayload(operations, expectedPayload);
        }

        [Fact]
        public void ShouldThrowWhenWritingTwoActionsWithSameMetadataAndOneNullTarget()
        {
            var operations = new ODataOperation[] { new ODataAction { Metadata = new Uri("#foo", UriKind.Relative) }, new ODataAction { Metadata = new Uri("#foo", UriKind.Relative), Target = new Uri("http://www.example.com/foo") } };
            Action test = () => this.WriteOperationsAndValidatePayload(operations, null);
            test.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightEntryAndFeedSerializer_ActionsAndFunctionsGroupMustSpecifyTarget("#foo"));
        }

        [Fact]
        public void ShouldThrowWhenWritingTwoActionsWithSameMetadataAndTwoNullTargets()
        {
            var operations = new ODataOperation[] { new ODataAction { Metadata = new Uri("#foo", UriKind.Relative) }, new ODataAction { Metadata = new Uri("#foo", UriKind.Relative) } };
            Action test = () => this.WriteOperationsAndValidatePayload(operations, null);
            test.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightEntryAndFeedSerializer_ActionsAndFunctionsGroupMustSpecifyTarget("#foo"));
        }

        [Fact]
        public void ShouldThrowWhenWritingTwoActionsWithSameMetadataAndSameTargets()
        {
            var operations = new ODataOperation[] { new ODataAction { Metadata = new Uri("#foo", UriKind.Relative), Target = new Uri("http://www.example.com/foo") }, new ODataAction { Metadata = new Uri("#foo", UriKind.Relative), Target = new Uri("http://www.example.com/foo") } };
            Action test = () => this.WriteOperationsAndValidatePayload(operations, null);
            test.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightEntryAndFeedSerializer_ActionsAndFunctionsGroupMustNotHaveDuplicateTarget("#foo", "http://www.example.com/foo"));
        }

        [Fact]
        public void ShouldWriteOneActionWithTwoBindingTargets()
        {
            var operations = new ODataOperation[] { new ODataAction { Metadata = new Uri("#foo", UriKind.Relative), Target = new Uri("http://www.example.com/foo") }, new ODataAction { Metadata = new Uri("#foo", UriKind.Relative), Target = new Uri("http://www.example.com/baz/foo") } };
            const string expectedPayload = "{\"#foo\":[{\"target\":\"http://www.example.com/foo\"},{\"target\":\"http://www.example.com/baz/foo\"}]}";
            this.WriteOperationsAndValidatePayload(operations, expectedPayload);
        }

        [Fact]
        public void ShouldWriteTwoActionsWithOneTargetEach()
        {
            var operations = new ODataOperation[] { new ODataAction { Metadata = new Uri("#foo", UriKind.Relative), Title = "foo", Target = new Uri("http://www.example.com/foo") }, new ODataAction { Metadata = new Uri("#baz", UriKind.Relative), Target = new Uri("http://www.example.com/baz") } };
            const string expectedPayload = "{\"#foo\":{\"title\":\"foo\",\"target\":\"http://www.example.com/foo\"},\"#baz\":{\"target\":\"http://www.example.com/baz\"}}";
            this.WriteOperationsAndValidatePayload(operations, expectedPayload);
        }

        [Fact]
        public void ShouldWriteTwoActionsWithTwoBindingTargetsEach()
        {
            var operations = new ODataOperation[] { new ODataAction { Metadata = new Uri("#foo", UriKind.Relative), Title = "foo", Target = new Uri("http://www.example.com/1/foo") }, new ODataAction { Metadata = new Uri("#foo", UriKind.Relative), Target = new Uri("http://www.example.com/2/foo") }, new ODataAction { Metadata = new Uri("#baz", UriKind.Relative), Title = "baz", Target = new Uri("http://www.example.com/1/baz") }, new ODataAction { Metadata = new Uri("#baz", UriKind.Relative), Target = new Uri("http://www.example.com/2/baz") } };
            const string expectedPayload = "{\"#foo\":[{\"title\":\"foo\",\"target\":\"http://www.example.com/1/foo\"},{\"target\":\"http://www.example.com/2/foo\"}],\"#baz\":[{\"title\":\"baz\",\"target\":\"http://www.example.com/1/baz\"},{\"target\":\"http://www.example.com/2/baz\"}]}";
            this.WriteOperationsAndValidatePayload(operations, expectedPayload);
        }

        private void WriteOperationsAndValidatePayload(IEnumerable<ODataOperation> operations, string expectedPayload, bool isAction = true, bool setMetadataDocumentUri = true, bool writeMetadataAnnotation = false)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var context = this.CreateJsonLightOutputContext(stream, /*writingResponse*/true, setMetadataDocumentUri);
                var serializer = new ODataJsonLightEntryAndFeedSerializer(context);
                serializer.JsonWriter.StartObjectScope();
                if (writeMetadataAnnotation)
                {
                    serializer.WriteContextUriProperty(ODataPayloadKind.ServiceDocument);
                }

                serializer.WriteOperations(operations, isAction);
                serializer.JsonWriter.EndObjectScope();
                context.Flush();
                stream.Position = 0;
                string actualPayload = (new StreamReader(stream)).ReadToEnd();
                Assert.Equal(expectedPayload, actualPayload);
            }

        }

        private ODataJsonLightOutputContext CreateJsonLightOutputContext(MemoryStream stream, bool writingResponse = true, bool setMetadataDocumentUri = true)
        {
            IEdmModel model = new EdmModel();

            ODataMessageWriterSettings settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            if (setMetadataDocumentUri)
            {
                settings.SetServiceDocumentUri(this.serviceDocumentUri);
            }

            return new ODataJsonLightOutputContext(
                ODataFormat.Json,
                new NonDisposingStream(stream),
                new ODataMediaType("application", "json"),
                Encoding.UTF8,
                settings,
                writingResponse,
                /*synchronous*/ true,
                TestUtils.WrapReferencedModelsToMainModel(model),
                /*urlResolver*/ null);
        }

        private string SerializeJsonFragment(Action<ODataJsonLightEntryAndFeedSerializer> writeWithSerializer)
        {
            string result;
            using (MemoryStream stream = new MemoryStream())
            {
                var context = this.CreateJsonLightOutputContext(stream);
                var serializer = new ODataJsonLightEntryAndFeedSerializer(context);
                serializer.JsonWriter.StartObjectScope();
                writeWithSerializer(serializer);
                serializer.JsonWriter.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                result = new StreamReader(stream).ReadToEnd();
            }

            return result;
        }
    }
}
