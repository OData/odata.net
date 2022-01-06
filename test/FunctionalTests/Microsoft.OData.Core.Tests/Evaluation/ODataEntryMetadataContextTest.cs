//---------------------------------------------------------------------
// <copyright file="ODataEntryMetadataContextTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.Evaluation
{
    public class ODataEntryMetadataContextTest
    {
        private static readonly EdmEntityType ActualEntityType;
        private static readonly EdmAction Action1;
        private static readonly EdmAction Action2;
        private static readonly EdmActionImport ActionImport1;
        private static readonly EdmActionImport ActionImport2;
        private static readonly EdmFunction Function1;
        private static readonly EdmFunction Function2;
        private static readonly EdmFunctionImport FunctionImport1;
        private static readonly EdmFunctionImport FunctionImport2;
        private ODataResourceMetadataContext entryMetadataContextWithoutModel;
        private ODataResourceMetadataContext entryMetadataContextWithModel;
        private ODataResource entry;
        private TestFeedAndEntryTypeContext typeContext;

        static ODataEntryMetadataContextTest()
        {
            ActualEntityType = new EdmEntityType("ns", "TypeName");
            ActualEntityType.AddKeys(new IEdmStructuralProperty[] {ActualEntityType.AddStructuralProperty("ID2", EdmPrimitiveTypeKind.Int32), ActualEntityType.AddStructuralProperty("ID3", EdmPrimitiveTypeKind.Int32)});
            ActualEntityType.AddStructuralProperty("Name2", EdmCoreModel.Instance.GetString(isNullable:true), /*defaultValue*/null);
            ActualEntityType.AddStructuralProperty("Name3", EdmCoreModel.Instance.GetString(isNullable: true), /*defaultValue*/null);
            ActualEntityType.AddStructuralProperty("StreamProp1", EdmPrimitiveTypeKind.Stream);
            ActualEntityType.AddStructuralProperty("StreamProp2", EdmPrimitiveTypeKind.Stream);

            var navProp1 = ActualEntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Target = ActualEntityType,
                TargetMultiplicity = EdmMultiplicity.Many,
                Name = "NavProp1"
            });

            var navProp2 = ActualEntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Target = ActualEntityType,
                TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                Name = "NavProp2"
            });

            var container = new EdmEntityContainer("Namespace", "Container");
            EdmEntitySet entitySet = container.AddEntitySet("EntitySet", ActualEntityType);

            entitySet.AddNavigationTarget(navProp1, entitySet);
            entitySet.AddNavigationTarget(navProp2, entitySet);

            Action1 = new EdmAction("Namespace", "Action1", null, true /*isBound*/, null /*entitySetPath*/);
            Action2 = new EdmAction("Namespace", "Action2", null, true /*isBound*/, null /*entitySetPath*/);
            ActionImport1 = new EdmActionImport(container, "ActionImport1", Action1);
            ActionImport2 = new EdmActionImport(container, "ActionImport2", Action2);
            Function1 = new EdmFunction("Namespace", "Function1", EdmCoreModel.Instance.GetString(isNullable: true), true /*isBound*/, null /*entitySetPath*/, true /*isComposable*/);
            Function2 = new EdmFunction("Namespace", "Function2", EdmCoreModel.Instance.GetString(isNullable: true), true /*isBound*/, null /*entitySetPath*/, true /*isComposable*/);
            FunctionImport1 = new EdmFunctionImport(container, "FunctionImport1", Function1);
            FunctionImport2 = new EdmFunctionImport(container, "FunctionImport2", Function2);
        }

        public ODataEntryMetadataContextTest()
        {
            this.entry = new ODataResource {TypeName = ActualEntityType.FullName()};
            this.typeContext = new TestFeedAndEntryTypeContext();
            this.entryMetadataContextWithoutModel = ODataResourceMetadataContext.Create(this.entry, this.typeContext, new ODataResourceSerializationInfo(), /*actualEntityType*/null, new TestMetadataContext(), new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.EntireSubtree), null);
            this.entryMetadataContextWithModel = ODataResourceMetadataContext.Create(this.entry, this.typeContext, /*serializationInfo*/null, ActualEntityType, new TestMetadataContext(), new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.EntireSubtree), null);
        }

        [Fact]
        public void CreateShouldReturnMetadataContextWithoutModel()
        {
            var entryMetadataContext = ODataResourceMetadataContext.Create(this.entry, this.typeContext, new ODataResourceSerializationInfo(), ActualEntityType, new TestMetadataContext(), new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.EntireSubtree), null);
            Assert.EndsWith("WithoutModel", entryMetadataContext.GetType().FullName);
        }

        [Fact]
        public void CreateShouldReturnMetadataContextWithModel()
        {
            var entryMetadataContext = ODataResourceMetadataContext.Create(this.entry, this.typeContext, /*serializationInfo*/null, ActualEntityType, new TestMetadataContext(), new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.EntireSubtree), null);
            Assert.EndsWith("WithModel", entryMetadataContext.GetType().FullName);
        }

        [Fact]
        public void EntryShouldReturnODataEntry()
        {
            Assert.Same(this.entry, this.entryMetadataContextWithoutModel.Resource);
            Assert.Same(this.entry, this.entryMetadataContextWithModel.Resource);
        }

        [Fact]
        public void TypeContextShouldReturnTheTypeContextInstance()
        {
            Assert.Same(this.typeContext, this.entryMetadataContextWithoutModel.TypeContext);
            Assert.Same(this.typeContext, this.entryMetadataContextWithModel.TypeContext);
        }

        #region ActualEntityTypeName
        [Fact]
        public void ActualEntityTypeNmeShouldReturnTypeName()
        {
            this.entry.TypeName = "ns.MyTypeName";
            Assert.Equal("ns.MyTypeName", this.entryMetadataContextWithoutModel.ActualResourceTypeName);
            Assert.Equal("ns.TypeName", this.entryMetadataContextWithModel.ActualResourceTypeName);
        }

        [Fact]
        public void ActualEntityTypeNmeShouldThrowForContextWithoutModelIfEntryTypeNameIsNull()
        {
            this.entry.TypeName = null;
            Action test = () => Assert.Null(this.entryMetadataContextWithoutModel.ActualResourceTypeName);
            test.Throws<ODataException>(Strings.ODataResourceTypeContext_ODataResourceTypeNameMissing);
        }
        #endregion ActualEntityTypeName

        #region KeyProperties
        [Fact]
        public void KeyPropertiesShouldThrowWhenEntryHasNoKeyPropertiesWithSerializationInfo()
        {
            Action test = () => Assert.NotNull(this.entryMetadataContextWithoutModel.KeyProperties);
            test.Throws<ODataException>(Strings.ODataResourceMetadataContext_EntityTypeWithNoKeyProperties(ActualEntityType.FullName()));
        }

        [Fact]
        public void KeyPropertiesShouldThrowWhenEntryHasNoKeyPropertiesAsSpecifiedInTheMetadata()
        {
            Action test = () => Assert.NotNull(this.entryMetadataContextWithModel.KeyProperties);
            test.Throws<ODataException>(Strings.EdmValueUtils_PropertyDoesntExist(ActualEntityType.FullName(), "ID2"));
        }

        [Fact]
        public void KeyPropertiesShouldThrowWhenSerializationInfoIsSetAndPropertyValueIsNonPrimitive()
        {
            this.entry.Properties = new[] { new ODataProperty { Name = "ID", Value = new ODataCollectionValue(), SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key } } };
            Action test = () => Assert.NotNull(this.entryMetadataContextWithoutModel.KeyProperties);
            test.Throws<ODataException>(Strings.ODataResourceMetadataContext_KeyOrETagValuesMustBePrimitiveValues("ID", "ns.TypeName"));
        }

        [Fact]
        public void KeyPropertiesShouldThrowWhenSerializationInfoIsSetAndPropertyValueIsNull()
        {
            this.entry.Properties = new[] {new ODataProperty {Name = "ID", Value = null, SerializationInfo = new ODataPropertySerializationInfo {PropertyKind = ODataPropertyKind.Key}}};
            Action test = () => Assert.NotNull(this.entryMetadataContextWithoutModel.KeyProperties);
            test.Throws<ODataException>(Strings.ODataResourceMetadataContext_NullKeyValue("ID", "ns.TypeName"));
        }

        [Fact]
        public void KeyPropertiesShouldThrowWhenMetadataIsPresentAndPropertyValueIsNonPrimitive()
        {
            this.entry.Properties = new[] {new ODataProperty {Name = "ID2", Value = new ODataCollectionValue()}};
            Action test = () => Assert.NotNull(this.entryMetadataContextWithModel.KeyProperties);
            test.Throws<ODataException>(Strings.ODataResourceMetadataContext_KeyOrETagValuesMustBePrimitiveValues("ID2", "ns.TypeName"));
        }

        [Fact]
        public void KeyPropertiesShouldThrowWhenMetadataIsPresentAndPropertyValueIsNull()
        {
            this.entry.Properties = new[] {new ODataProperty {Name = "ID2", Value = null}};
            Action test = () => Assert.NotNull(this.entryMetadataContextWithModel.KeyProperties);
            test.Throws<ODataException>(Strings.ODataResourceMetadataContext_NullKeyValue("ID2", "ns.TypeName"));
        }

        [Fact]
        public void KeyPropertiesShouldThrowWhenKeyPropertyInMetadataIsNotInEntry()
        {
            Action test = () => Assert.NotNull(this.entryMetadataContextWithModel.KeyProperties);
            test.Throws<ODataException>(Strings.EdmValueUtils_PropertyDoesntExist("ns.TypeName", "ID2"));
        }

        [Fact]
        public void ShouldGetKeyPropertiesBasedOnSerializationInfo()
        {
            this.entry.Properties = new[]
            {
                new ODataProperty { Name = "ID1", Value = 1, SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key } },
                new ODataProperty { Name = "ID2", Value = 2, SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key } },
                new ODataProperty { Name = "ID3", Value = 3 },
            };

            var keys = this.entryMetadataContextWithoutModel.KeyProperties;
            Assert.Equal(2, keys.Count);
            Assert.Contains(keys, p => p.Key == "ID1");
            Assert.Contains(keys, p => p.Key == "ID2");
        }

        [Fact]
        public void ShouldGetKeyPropertiesBasedOnMetadata()
        {
            this.entry.Properties = new[]
            {
                new ODataProperty { Name = "ID1", Value = 1 },
                new ODataProperty { Name = "ID2", Value = 2 },
                new ODataProperty { Name = "ID3", Value = 3 },
            };

            var keys = this.entryMetadataContextWithModel.KeyProperties;
            Assert.Equal(2, keys.Count);
            Assert.Contains(keys, p => p.Key == "ID2");
            Assert.Contains(keys, p => p.Key == "ID3");
        }

        #endregion KeyProperties

        #region ETagProperties
        [Fact]
        public void ETagPropertiesShouldReturnEmptyForEntryMetadataContextWithoutModelAndThereAreNoSerializationInfoOnEntryProperties()
        {
            Assert.Empty(this.entryMetadataContextWithoutModel.ETagProperties);
        }

        [Fact]
        public void ETagPropertiesShouldThrowWhenSerializationInfoIsSetAndPropertyValueIsNonPrimitive()
        {
            this.entry.Properties = new[] { new ODataProperty { Name = "Name", Value = new ODataCollectionValue(), SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.ETag } } };
            Action test = () => Assert.NotNull(this.entryMetadataContextWithoutModel.ETagProperties);
            test.Throws<ODataException>(Strings.ODataResourceMetadataContext_KeyOrETagValuesMustBePrimitiveValues("Name", "ns.TypeName"));
        }

        [Fact]
        public void ETagPropertiesShouldNotThrowWhenSerializationInfoIsSetAndPropertyValueIsNull()
        {
            this.entry.Properties = new[] { new ODataProperty { Name = "Name", Value = null, SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.ETag } } };
            var keys = this.entryMetadataContextWithoutModel.ETagProperties;
            Assert.Single(keys);
            Assert.Contains(keys, p => p.Key == "Name" && p.Value == null);
        }

        [Fact]
        public void ShouldGetETagPropertiesBasedOnSerializationInfo()
        {
            this.entry.Properties = new[]
            {
                new ODataProperty { Name = "Name1", Value = "value1", SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.ETag } },
                new ODataProperty { Name = "Name2", Value = "value2", SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.ETag } },
                new ODataProperty { Name = "Name3", Value = "value3" },
            };

            var keys = this.entryMetadataContextWithoutModel.ETagProperties;
            Assert.Equal(2, keys.Count());
            Assert.Contains(keys, p => p.Key == "Name1");
            Assert.Contains(keys, p => p.Key == "Name2");
        }
        #endregion ETagProperties

        #region SelectedNavigationProperties
        [Fact]
        public void SelectedNavigationPropertiesShouldReturnEmptyWithoutModel()
        {
            Assert.Empty(this.entryMetadataContextWithoutModel.SelectedNavigationProperties);
        }

        [Fact]
        public void SelectedNaigationPropertiesShouldReturnPropertiesBasedOnSelectAndMetadata()
        {
            var entryMetadataContext = ODataResourceMetadataContext.Create(new ODataResource(), new TestFeedAndEntryTypeContext(), /*serializationInfo*/null, ActualEntityType, new TestMetadataContext(), SelectedPropertiesNode.Create("NavProp1"), null);
            var keys = entryMetadataContext.SelectedNavigationProperties;
            var key = Assert.Single(keys);
            Assert.Equal("NavProp1", key.Name);
        }
        #endregion SelectedNavigationProperties

        #region SelectedStreamProperties
        [Fact]
        public void SelectedStreamPropertiesShouldReturnEmptyWithoutModel()
        {
            Assert.Empty(this.entryMetadataContextWithoutModel.SelectedStreamProperties);
        }

        [Fact]
        public void SelectedStreamPropertiesShouldReturnPropertiesBasedOnMetadata()
        {
            var entryMetadataContext = ODataResourceMetadataContext.Create(new ODataResource(), new TestFeedAndEntryTypeContext(), /*serializationInfo*/null, ActualEntityType, new TestMetadataContext(), SelectedPropertiesNode.Create("StreamProp1"), null);
            Assert.True(entryMetadataContext.SelectedStreamProperties.ContainsKey("StreamProp1"));
        }
        #endregion SelectedStreamProperties

        #region SelectedBindableOperations
        [Fact]
        public void SelectedBindableOperationsShouldReturnEmptyWithoutModel()
        {
            var metadataContext = new TestMetadataContext { GetBindableOperationsForTypeFunc = type => new IEdmOperation[] { Action1, Action2, Function1, Function2 }, OperationsBoundToStructuredTypeMustBeContainerQualifiedFunc = type => false };
            var entryMetadataContext = ODataResourceMetadataContext.Create(new ODataResource(), new TestFeedAndEntryTypeContext(), new ODataResourceSerializationInfo(), /*actualEntityType*/null, metadataContext, SelectedPropertiesNode.Create("Action1,Function1"), null);
            Assert.Empty(entryMetadataContext.SelectedBindableOperations);
        }

        [Fact]
        public void SelectedBindableOperationsShouldReturnPropertiesBasedOnMetadata()
        {
            var metadataContext = new TestMetadataContext { GetBindableOperationsForTypeFunc = type => new IEdmOperation[] { Action1, Action2, Function1, Function2 }, OperationsBoundToStructuredTypeMustBeContainerQualifiedFunc = type => false };
            var entryMetadataContext = ODataResourceMetadataContext.Create(new ODataResource(), new TestFeedAndEntryTypeContext(), /*serializationInfo*/null, ActualEntityType, metadataContext, SelectedPropertiesNode.Create("Action1,Function1"), null);
            var operations = entryMetadataContext.SelectedBindableOperations;
            Assert.Equal(2, operations.Count());
            Assert.Contains(Action1, operations);
            Assert.Contains(Function1, operations);
        }
        #endregion SelectedBindableOperations
    }
}
