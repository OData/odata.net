//---------------------------------------------------------------------
// <copyright file="JsonLightMetadataLevelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Test.OData.TDD.Tests.Writer.JsonLight
{
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.JsonLight;
    using Microsoft.Test.OData.TDD.Tests.Common;

    [TestClass]
    public class JsonLightMetadataLevelTests
    {
        private static readonly Uri MetadataDocumentUri = new Uri("http://host/service.svc/$metadata", UriKind.Absolute);
        private static readonly IEdmModel Model = TestUtils.WrapReferencedModelsToMainModel(new EdmModel());
        private List<KeyValuePair<string, string>> parameterList;
        private ODataMediaType applicationJsonMediaType;

        [TestInitialize]
        public void Initialize()
        {
            this.parameterList = new List<KeyValuePair<string, string>>();
            this.applicationJsonMediaType = new ODataMediaType("application", "json", this.parameterList);
        }

        [TestMethod]
        public void ODataMinimalMetadataShouldCreateMinimalMetadataLevel()
        {
            this.parameterList.Add(new KeyValuePair<string, string>("odata.metadata", "minimal"));
            JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, Model, /*writingResponse*/ true).Should().BeOfType<JsonMinimalMetadataLevel>();
        }

        [TestMethod]
        public void ODataFullMetadataShouldCreateFullMetadataLevel()
        {
            this.parameterList.Add(new KeyValuePair<string, string>("odata.metadata", "full"));
            JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, Model, /*writingResponse*/ true).Should().BeOfType<JsonFullMetadataLevel>();
        }

        [TestMethod]
        public void ODataNoMetadataShouldCreateNoMetadataLevel()
        {
            this.parameterList.Add(new KeyValuePair<string, string>("odata.metadata", "none"));
            JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, Model, /*writingResponse*/ true).Should().BeOfType<JsonNoMetadataLevel>();
        }

        [TestMethod]
        public void NoODataParameterShouldCreateMinimalMetadataLevel()
        {
            JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, Model, /*writingResponse*/ true).Should().BeOfType<JsonMinimalMetadataLevel>();
        }

        [TestMethod]
        public void FooFullMetadataShouldCreateMinimalMetadataLevel()
        {
            this.parameterList.Add(new KeyValuePair<string, string>("foo", "full"));
            JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, Model, /*writingResponse*/ true).Should().BeOfType<JsonMinimalMetadataLevel>();
        }

        [TestMethod]
        public void FooNoMetadataShouldCreateMinimalMetadataLevel()
        {
            this.parameterList.Add(new KeyValuePair<string, string>("foo", "none"));
            JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, Model, /*writingResponse*/ true).Should().BeOfType<JsonMinimalMetadataLevel>();
        }

        [TestMethod]
        public void FooMinimalMetadataShouldCreateMinimalMetadataLevel()
        {
            this.parameterList.Add(new KeyValuePair<string, string>("foo", "minimal"));
            JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, Model, /*writingResponse*/ true).Should().BeOfType<JsonMinimalMetadataLevel>();
        }

        [TestMethod]
        public void ODataFullMetadataParameterNotInFirstPostionShouldCreateFullMetadataLevel()
        {
            this.parameterList.Add(new KeyValuePair<string, string>("foo", "something"));
            this.parameterList.Add(new KeyValuePair<string, string>("odata.metadata", "full"));
            JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, Model, /*writingResponse*/ true).Should().BeOfType<JsonFullMetadataLevel>();
        }

        [TestMethod]
        public void MultipleODataParametersShouldCreateMetadataLevelBasedOnFirstEncountered()
        {
            // We could also throw in this case, but this should have already been validated.
            this.parameterList.Add(new KeyValuePair<string, string>("odata.metadata", "none"));
            this.parameterList.Add(new KeyValuePair<string, string>("odata.metadata", "full"));
            JsonLightMetadataLevel.Create(this.applicationJsonMediaType, MetadataDocumentUri, Model, /*writingResponse*/ true).Should().BeOfType<JsonNoMetadataLevel>();
        }

        [TestMethod]
        public void MediaTypeWithNullParameterListShouldCreateMinimalMetadataLevel()
        {
            JsonLightMetadataLevel.Create(new ODataMediaType("application", "json"), MetadataDocumentUri, Model, /*writingResponse*/ true).Should().BeOfType<JsonMinimalMetadataLevel>();
        }
    }
}
