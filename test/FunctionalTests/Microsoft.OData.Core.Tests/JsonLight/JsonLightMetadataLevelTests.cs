//---------------------------------------------------------------------
// <copyright file="JsonLightMetadataLevelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.JsonLight
{
    public class JsonLightMetadataLevelTests
    {
        private static readonly Uri MetadataDocumentUri = new Uri("http://host/service.svc/$metadata", UriKind.Absolute);
        private static readonly IEdmModel Model = TestUtils.WrapReferencedModelsToMainModel(new EdmModel());
        private List<KeyValuePair<string, string>> parameterList;
        private ODataMediaType applicationJsonMediaType;

        public JsonLightMetadataLevelTests()
        {
            this.parameterList = new List<KeyValuePair<string, string>>();
            this.applicationJsonMediaType = new ODataMediaType("application", "json", this.parameterList);
        }

        [Fact]
        public void ODataMinimalMetadataShouldCreateMinimalMetadataLevel()
        {
            this.parameterList.Add(new KeyValuePair<string, string>("odata.metadata", "minimal"));
            JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, Model, /*writingResponse*/ true).Should().BeOfType<JsonMinimalMetadataLevel>();
        }

        [Fact]
        public void ODataFullMetadataShouldCreateFullMetadataLevel()
        {
            this.parameterList.Add(new KeyValuePair<string, string>("odata.metadata", "full"));
            JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, Model, /*writingResponse*/ true).Should().BeOfType<JsonFullMetadataLevel>();
        }

        [Fact]
        public void ODataNoMetadataShouldCreateNoMetadataLevel()
        {
            this.parameterList.Add(new KeyValuePair<string, string>("odata.metadata", "none"));
            JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, Model, /*writingResponse*/ true).Should().BeOfType<JsonNoMetadataLevel>();
        }

        [Fact]
        public void NoODataParameterShouldCreateMinimalMetadataLevel()
        {
            JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, Model, /*writingResponse*/ true).Should().BeOfType<JsonMinimalMetadataLevel>();
        }

        [Fact]
        public void FooFullMetadataShouldCreateMinimalMetadataLevel()
        {
            this.parameterList.Add(new KeyValuePair<string, string>("foo", "full"));
            JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, Model, /*writingResponse*/ true).Should().BeOfType<JsonMinimalMetadataLevel>();
        }

        [Fact]
        public void FooNoMetadataShouldCreateMinimalMetadataLevel()
        {
            this.parameterList.Add(new KeyValuePair<string, string>("foo", "none"));
            JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, Model, /*writingResponse*/ true).Should().BeOfType<JsonMinimalMetadataLevel>();
        }

        [Fact]
        public void FooMinimalMetadataShouldCreateMinimalMetadataLevel()
        {
            this.parameterList.Add(new KeyValuePair<string, string>("foo", "minimal"));
            JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, Model, /*writingResponse*/ true).Should().BeOfType<JsonMinimalMetadataLevel>();
        }

        [Fact]
        public void ODataFullMetadataParameterNotInFirstPostionShouldCreateFullMetadataLevel()
        {
            this.parameterList.Add(new KeyValuePair<string, string>("foo", "something"));
            this.parameterList.Add(new KeyValuePair<string, string>("odata.metadata", "full"));
            JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, Model, /*writingResponse*/ true).Should().BeOfType<JsonFullMetadataLevel>();
        }

        [Fact]
        public void MultipleODataParametersShouldCreateMetadataLevelBasedOnFirstEncountered()
        {
            // We could also throw in this case, but this should have already been validated.
            this.parameterList.Add(new KeyValuePair<string, string>("odata.metadata", "none"));
            this.parameterList.Add(new KeyValuePair<string, string>("odata.metadata", "full"));
            JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, Model, /*writingResponse*/ true).Should().BeOfType<JsonNoMetadataLevel>();
        }

        [Fact]
        public void MediaTypeWithNullParameterListShouldCreateMinimalMetadataLevel()
        {
            JsonLightMetadataLevel.Create(new ODataMediaType("application", "json"), MetadataDocumentUri, Model, /*writingResponse*/ true).Should().BeOfType<JsonMinimalMetadataLevel>();
        }
    }
}
