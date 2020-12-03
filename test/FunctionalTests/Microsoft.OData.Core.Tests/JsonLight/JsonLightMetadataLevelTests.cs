//---------------------------------------------------------------------
// <copyright file="JsonLightMetadataLevelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
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
            Assert.IsType<JsonMinimalMetadataLevel>(JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, /* alwaysAddTypeAnnotationsForDerivedTypes */ false, Model, /*writingResponse*/ true));
        }

        [Fact]
        public void ODataFullMetadataShouldCreateFullMetadataLevel()
        {
            this.parameterList.Add(new KeyValuePair<string, string>("odata.metadata", "full"));
            Assert.IsType<JsonFullMetadataLevel>(JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, /* alwaysAddTypeAnnotationsForDerivedTypes */ false, Model, /*writingResponse*/ true));
        }

        [Fact]
        public void ODataNoMetadataShouldCreateNoMetadataLevel()
        {
            this.parameterList.Add(new KeyValuePair<string, string>("odata.metadata", "none"));
            Assert.IsType<JsonNoMetadataLevel>(JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, /* alwaysAddTypeAnnotationsForDerivedTypes */ false, Model, /*writingResponse*/ true));
        }

        [Fact]
        public void NoODataParameterShouldCreateMinimalMetadataLevel()
        {
            Assert.IsType<JsonMinimalMetadataLevel>(JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, /* alwaysAddTypeAnnotationsForDerivedTypes */ false, Model, /*writingResponse*/ true));
        }

        [Fact]
        public void FooFullMetadataShouldCreateMinimalMetadataLevel()
        {
            this.parameterList.Add(new KeyValuePair<string, string>("foo", "full"));
            Assert.IsType<JsonMinimalMetadataLevel>(JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, /* alwaysAddTypeAnnotationsForDerivedTypes */ false, Model, /*writingResponse*/ true));
        }

        [Fact]
        public void FooNoMetadataShouldCreateMinimalMetadataLevel()
        {
            this.parameterList.Add(new KeyValuePair<string, string>("foo", "none"));
            Assert.IsType<JsonMinimalMetadataLevel>(JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, /* alwaysAddTypeAnnotationsForDerivedTypes */ false, Model, /*writingResponse*/ true));
        }

        [Fact]
        public void FooMinimalMetadataShouldCreateMinimalMetadataLevel()
        {
            this.parameterList.Add(new KeyValuePair<string, string>("foo", "minimal"));
            Assert.IsType<JsonMinimalMetadataLevel>(JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, /* alwaysAddTypeAnnotationsForDerivedTypes */ false, Model, /*writingResponse*/ true));
        }

        [Fact]
        public void ODataFullMetadataParameterNotInFirstPostionShouldCreateFullMetadataLevel()
        {
            this.parameterList.Add(new KeyValuePair<string, string>("foo", "something"));
            this.parameterList.Add(new KeyValuePair<string, string>("odata.metadata", "full"));
            Assert.IsType<JsonFullMetadataLevel>(JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, /* alwaysAddTypeAnnotationsForDerivedTypes */ false, Model, /*writingResponse*/ true));
        }

        [Fact]
        public void MultipleODataParametersShouldCreateMetadataLevelBasedOnFirstEncountered()
        {
            // We could also throw in this case, but this should have already been validated.
            this.parameterList.Add(new KeyValuePair<string, string>("odata.metadata", "none"));
            this.parameterList.Add(new KeyValuePair<string, string>("odata.metadata", "full"));
            Assert.IsType<JsonNoMetadataLevel>(JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, /* alwaysAddTypeAnnotationsForDerivedTypes */ false, Model, /*writingResponse*/ true));
        }

        [Fact]
        public void MediaTypeWithNullParameterListShouldCreateMinimalMetadataLevel()
        {
            Assert.IsType<JsonMinimalMetadataLevel>(JsonLightMetadataLevel.Create(new ODataMediaType("application", "json"), MetadataDocumentUri, /* alwaysAddTypeAnnotationsForDerivedTypes */ false, Model, /*writingResponse*/ true));
        }
    }
}
