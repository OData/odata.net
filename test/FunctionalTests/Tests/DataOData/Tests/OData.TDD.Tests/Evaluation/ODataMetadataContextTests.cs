//---------------------------------------------------------------------
// <copyright file="ODataMetadataContextTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Evaluation
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm.Library.Values;
    using Microsoft.OData.Edm.Values;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.Test.OData.TDD.Tests.Common;
    using Microsoft.Test.OData.TDD.Tests.Reader.JsonLight;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataMetadataContextTests
    {
        private IEdmModel edmModel;

        [TestInitialize]
        public void TestInitialize()
        {
            this.edmModel = TestModel.BuildDefaultTestModel();
        }

        [TestMethod]
        public void GetEntityMetadataBuilderShouldThrowWhenMetadataDocumentUriIsNull()
        {
            ODataMetadataContext context = new ODataMetadataContext(
                true /*readingResponse*/,
                ODataReaderBehavior.DefaultBehavior.OperationsBoundToEntityTypeMustBeContainerQualified, 
                new EdmTypeReaderResolver(this.edmModel, ODataReaderBehavior.DefaultBehavior), 
                this.edmModel,
                null /*metadataDocumentUri*/,
                null /*requestUri*/);
            IEdmEntitySet set = this.edmModel.EntityContainer.FindEntitySet("Products");
            ODataEntry entry = TestUtils.CreateODataEntry(set, new EdmStructuredValue(new EdmEntityTypeReference(set.EntityType(), true), new IEdmPropertyValue[0]), set.EntityType());
            Action action = () => context.GetEntityMetadataBuilderForReader(new TestJsonLightReaderEntryState { Entry = entry, SelectedProperties = SelectedPropertiesNode.EntireSubtree }, null);
            action.ShouldThrow<ODataException>().WithMessage(Strings.ODataJsonLightEntryMetadataContext_MetadataAnnotationMustBeInPayload("odata.context"));
        }

        [TestMethod]
        public void GetEntityMetadataBuilderShouldNotThrowWhenMetadataDocumentUriIsNonNullAndAbsolute()
        {
            ODataMetadataContext context = new ODataMetadataContext(
                true /*readingResponse*/,
                ODataReaderBehavior.DefaultBehavior.OperationsBoundToEntityTypeMustBeContainerQualified,
                new EdmTypeReaderResolver(this.edmModel, ODataReaderBehavior.DefaultBehavior),
                this.edmModel,
                new Uri("http://myservice.svc/$metadata", UriKind.Absolute),
                null /*requestUri*/);
            IEdmEntitySet set = this.edmModel.EntityContainer.FindEntitySet("Products");
            ODataEntry entry = TestUtils.CreateODataEntry(set, new EdmStructuredValue(new EdmEntityTypeReference(set.EntityType(), true), new IEdmPropertyValue[0]), set.EntityType());
            Action action = () => context.GetEntityMetadataBuilderForReader(new TestJsonLightReaderEntryState { Entry = entry, SelectedProperties = new SelectedPropertiesNode("*")}, null);
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void CheckForOperationsRequiringContainerQualificationFallBackToWhetherTypeIsOpen()
        {
            var closedType = new EdmEntityType("NS", "Type1");
            var openType = new EdmEntityType("NS", "Type2", null, false, true);
            
            ODataMetadataContext context = new ODataMetadataContext(
                true /*readingResponse*/,
                ODataReaderBehavior.DefaultBehavior.OperationsBoundToEntityTypeMustBeContainerQualified,
                new EdmTypeReaderResolver(this.edmModel, ODataReaderBehavior.DefaultBehavior),
                this.edmModel,
                null /*metadataDocumentUri*/,
                null /*requestUri*/);
            context.OperationsBoundToEntityTypeMustBeContainerQualified(closedType).Should().BeFalse();
            context.OperationsBoundToEntityTypeMustBeContainerQualified(openType).Should().BeTrue();
        }
    }
}
