//---------------------------------------------------------------------
// <copyright file="ODataEntryMetadataContextTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.Evaluation;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.Evaluation
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
        private ODataEntryMetadataContext entryMetadataContextWithoutModel;
        private ODataEntryMetadataContext entryMetadataContextWithModel;
        private ODataEntry entry;
        private TestFeedAndEntryTypeContext typeContext;

        static ODataEntryMetadataContextTest()
        {
            ActualEntityType = new EdmEntityType("ns", "TypeName");
            ActualEntityType.AddKeys(new IEdmStructuralProperty[] {ActualEntityType.AddStructuralProperty("ID2", EdmPrimitiveTypeKind.Int32), ActualEntityType.AddStructuralProperty("ID3", EdmPrimitiveTypeKind.Int32)});
            ActualEntityType.AddStructuralProperty("Name2", EdmCoreModel.Instance.GetString(isNullable:true), /*defaultValue*/null, EdmConcurrencyMode.Fixed);
            ActualEntityType.AddStructuralProperty("Name3", EdmCoreModel.Instance.GetString(isNullable: true), /*defaultValue*/null, EdmConcurrencyMode.Fixed);
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
            this.entry = new ODataEntry {TypeName = ActualEntityType.FullName()};
            this.typeContext = new TestFeedAndEntryTypeContext();
            this.entryMetadataContextWithoutModel = ODataEntryMetadataContext.Create(this.entry, this.typeContext, new ODataFeedAndEntrySerializationInfo(), /*actualEntityType*/null, new TestMetadataContext(), SelectedPropertiesNode.EntireSubtree);
            this.entryMetadataContextWithModel = ODataEntryMetadataContext.Create(this.entry, this.typeContext, /*serializationInfo*/null, ActualEntityType, new TestMetadataContext(), SelectedPropertiesNode.EntireSubtree);
        }

        [Fact]
        public void CreateShouldReturnMetadataContextWithoutModel()
        {
            var entryMetadataContext = ODataEntryMetadataContext.Create(this.entry, this.typeContext, new ODataFeedAndEntrySerializationInfo(), ActualEntityType, new TestMetadataContext(), SelectedPropertiesNode.EntireSubtree);
            entryMetadataContext.GetType().FullName.EndsWith("WithoutModel").Should().BeTrue();
        }

        [Fact]
        public void CreateShouldReturnMetadataContextWithModel()
        {
            var entryMetadataContext = ODataEntryMetadataContext.Create(this.entry, this.typeContext, /*serializationInfo*/null, ActualEntityType, new TestMetadataContext(), SelectedPropertiesNode.EntireSubtree);
            entryMetadataContext.GetType().FullName.EndsWith("WithModel").Should().BeTrue();
        }

        [Fact]
        public void EntryShouldReturnODataEntry()
        {
            this.entryMetadataContextWithoutModel.Entry.Should().BeSameAs(this.entry);
            this.entryMetadataContextWithModel.Entry.Should().BeSameAs(this.entry);
        }

        [Fact]
        public void TypeContextShouldReturnTheTypeContextInstance()
        {
            this.entryMetadataContextWithoutModel.TypeContext.Should().BeSameAs(this.typeContext);
            this.entryMetadataContextWithModel.TypeContext.Should().BeSameAs(this.typeContext);
        }

        #region ActualEntityTypeName
        [Fact]
        public void ActualEntityTypeNmeShouldReturnTypeName()
        {
            this.entry.TypeName = "ns.MyTypeName";
            this.entryMetadataContextWithoutModel.ActualEntityTypeName.Should().Be("ns.MyTypeName");
            this.entryMetadataContextWithModel.ActualEntityTypeName.Should().Be("ns.TypeName");
        }

        [Fact]
        public void ActualEntityTypeNmeShouldThrowForContextWithoutModelIfEntryTypeNameIsNull()
        {
            this.entry.TypeName = null;
            Action test = () => this.entryMetadataContextWithoutModel.ActualEntityTypeName.Should().BeNull();
            test.ShouldThrow<ODataException>(Strings.ODataFeedAndEntryTypeContext_ODataEntryTypeNameMissing);
        }
        #endregion ActualEntityTypeName

        #region KeyProperties
        [Fact]
        public void KeyPropertiesShouldThrowWhenEntryHasNoKeyPropertiesWithSerializationInfo()
        {
            Action test = () => this.entryMetadataContextWithoutModel.KeyProperties.Should().NotBeNull();
            test.ShouldThrow<ODataException>(Strings.ODataEntryMetadataContext_EntityTypeWithNoKeyProperties(ActualEntityType.FullName()));
        }

        [Fact]
        public void KeyPropertiesShouldThrowWhenEntryHasNoKeyPropertiesAsSpecifiedInTheMetadata()
        {
            Action test = () => this.entryMetadataContextWithModel.KeyProperties.Should().NotBeNull();
            test.ShouldThrow<ODataException>(Strings.ODataEntryMetadataContext_EntityTypeWithNoKeyProperties(ActualEntityType.FullName()));
        }

        [Fact]
        public void KeyPropertiesShouldThrowWhenSerializationInfoIsSetAndPropertyValueIsNonPrimitive()
        {
            this.entry.Properties = new[] {new ODataProperty {Name = "ID", Value = new ODataComplexValue(), SerializationInfo = new ODataPropertySerializationInfo {PropertyKind = ODataPropertyKind.Key}}};
            Action test = () => this.entryMetadataContextWithoutModel.KeyProperties.Should().NotBeNull();
            test.ShouldThrow<ODataException>(Strings.ODataEntryMetadataContext_KeyOrETagValuesMustBePrimitiveValues("ID", "ns.TypeName"));
        }

        [Fact]
        public void KeyPropertiesShouldThrowWhenSerializationInfoIsSetAndPropertyValueIsNull()
        {
            this.entry.Properties = new[] {new ODataProperty {Name = "ID", Value = null, SerializationInfo = new ODataPropertySerializationInfo {PropertyKind = ODataPropertyKind.Key}}};
            Action test = () => this.entryMetadataContextWithoutModel.KeyProperties.Should().NotBeNull();
            test.ShouldThrow<ODataException>(Strings.ODataEntryMetadataContext_NullKeyValue("ID", "ns.TypeName"));
        }

        [Fact]
        public void KeyPropertiesShouldThrowWhenMetadataIsPresentAndPropertyValueIsNonPrimitive()
        {
            this.entry.Properties = new[] {new ODataProperty {Name = "ID2", Value = new ODataComplexValue()}};
            Action test = () => this.entryMetadataContextWithModel.KeyProperties.Should().NotBeNull();
            test.ShouldThrow<ODataException>(Strings.ODataEntryMetadataContext_KeyOrETagValuesMustBePrimitiveValues("ID2", "ns.TypeName"));
        }

        [Fact]
        public void KeyPropertiesShouldThrowWhenMetadataIsPresentAndPropertyValueIsNull()
        {
            this.entry.Properties = new[] {new ODataProperty {Name = "ID2", Value = null}};
            Action test = () => this.entryMetadataContextWithModel.KeyProperties.Should().NotBeNull();
            test.ShouldThrow<ODataException>(Strings.ODataEntryMetadataContext_NullKeyValue("ID2", "ns.TypeName"));
        }

        [Fact]
        public void KeyPropertiesShouldThrowWhenKeyPropertyInMetadataIsNotInEntry()
        {
            Action test = () => this.entryMetadataContextWithModel.KeyProperties.Should().NotBeNull();
            test.ShouldThrow<ODataException>(Strings.EdmValueUtils_PropertyDoesntExist("ns.TypeName", "ID2"));            
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

            this.entryMetadataContextWithoutModel.KeyProperties.Should().Contain(p => p.Key == "ID1").And.Contain(p => p.Key == "ID2").And.HaveCount(2);
        }

        [Fact]
        public void ShouldGetKeyPropertiesBasedOnMetadata()
        {
            this.entry.Properties = new[]
            {
                new ODataProperty { Name = "ID1", Value = 1, SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key } },
                new ODataProperty { Name = "ID2", Value = 2, SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key } },
                new ODataProperty { Name = "ID3", Value = 3 },
            };

            this.entryMetadataContextWithModel.KeyProperties.Should().Contain(p => p.Key == "ID2").And.Contain(p => p.Key == "ID3").And.HaveCount(2);
        }
        #endregion KeyProperties

        #region ETagProperties
        [Fact]
        public void ETagPropertiesShouldReturnEmptyForEntryMetadataContextWithoutModelAndThereAreNoSerializationInfoOnEntryProperties()
        {
            this.entryMetadataContextWithoutModel.ETagProperties.Should().BeEmpty();
        }

        [Fact(Skip = "we should remove this case because we need model to calc ETag")]
        public void ETagPropertiesShouldReturnEmptyForEntryMetadataContextWithModelWithoutETagsAndThereAreSerializationInfoOnEntryProperties()
        {
            var odataEntry = new ODataEntry { Properties = new[] { new ODataProperty { Name = "Name", Value = null, SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.ETag } } } };
            var entryMetadataContext = ODataEntryMetadataContext.Create(odataEntry, this.typeContext, /*serializationInfo*/null, new EdmEntityType("ns", "TypeName"), new TestMetadataContext(), SelectedPropertiesNode.EntireSubtree);
            entryMetadataContext.ETagProperties.Should().BeEmpty();
        }

        [Fact]
        public void ETagPropertiesShouldThrowWhenSerializationInfoIsSetAndPropertyValueIsNonPrimitive()
        {
            this.entry.Properties = new[] { new ODataProperty { Name = "Name", Value = new ODataComplexValue(), SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.ETag }}};
            Action test = () => this.entryMetadataContextWithoutModel.ETagProperties.Should().NotBeNull();
            test.ShouldThrow<ODataException>(Strings.ODataEntryMetadataContext_KeyOrETagValuesMustBePrimitiveValues("Name", "ns.TypeName"));
        }

        [Fact]
        public void ETagPropertiesShouldNotThrowWhenSerializationInfoIsSetAndPropertyValueIsNull()
        {
            this.entry.Properties = new[] { new ODataProperty { Name = "Name", Value = null, SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.ETag } } };
            this.entryMetadataContextWithoutModel.ETagProperties.Should().HaveCount(1).And.Contain(p => p.Key == "Name" && p.Value == null);
        }

        [Fact(Skip = "we should remove this case because we need model to calc ETag")]
        public void ETagPropertiesShouldThrowWhenMetadataIsPresentAndPropertyValueIsNonPrimitive()
        {
            this.entry.Properties = new[] { new ODataProperty { Name = "Name2", Value = new ODataComplexValue() } };
            Action test = () => this.entryMetadataContextWithModel.ETagProperties.Should().NotBeNull();
            test.ShouldThrow<ODataException>(Strings.ODataEntryMetadataContext_KeyOrETagValuesMustBePrimitiveValues("Name2", "ns.TypeName"));
        }

        [Fact(Skip = "we should remove this case because we need model to calc ETag")]
        public void ETagPropertiesShouldNotThrowWhenMetadataIsPresentAndPropertyValueIsNull()
        {
            this.entry.Properties = new[] { new ODataProperty { Name = "Name2", Value = null }, new ODataProperty { Name = "Name3", Value = null }};
            this.entryMetadataContextWithModel.ETagProperties.Should().HaveCount(2).And.Contain(p => p.Key == "Name2" && p.Value == null).And.Contain(p => p.Key == "Name3" && p.Value == null);
        }

        [Fact(Skip = "we should remove this case because we need model to calc ETag")]
        public void ETagPropertiesShouldThrowWhenETagPropertyInMetadataIsNotInEntry()
        {
            Action test = () => this.entryMetadataContextWithModel.ETagProperties.Should().NotBeNull();
            test.ShouldThrow<ODataException>(Strings.EdmValueUtils_PropertyDoesntExist("ns.TypeName", "Name2"));
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

            this.entryMetadataContextWithoutModel.ETagProperties.Should().Contain(p => p.Key == "Name1").And.Contain(p => p.Key == "Name2").And.HaveCount(2);
        }

        [Fact(Skip = "we should remove this case because we need model to calc ETag")]
        public void ShouldGetETagPropertiesBasedOnMetadata()
        {
            this.entry.Properties = new[]
            {
                new ODataProperty { Name = "Name1", Value = "value1", SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.ETag } },
                new ODataProperty { Name = "Name2", Value = "value2", SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.ETag } },
                new ODataProperty { Name = "Name3", Value = "value3" },
            };

            this.entryMetadataContextWithModel.ETagProperties.Should().Contain(p => p.Key == "Name2").And.Contain(p => p.Key == "Name3").And.HaveCount(2);
        }
        #endregion ETagProperties

        #region SelectedNavigationProperties
        [Fact]
        public void SelectedNavigationPropertiesShouldReturnEmptyWithoutModel()
        {
            this.entryMetadataContextWithoutModel.SelectedNavigationProperties.Should().BeEmpty();
        }

        [Fact]
        public void SelectedNaigationPropertiesShouldReturnPropertiesBasedOnSelectAndMetadata()
        {
            var entryMetadataContext = ODataEntryMetadataContext.Create(new ODataEntry(), new TestFeedAndEntryTypeContext(), /*serializationInfo*/null, ActualEntityType, new TestMetadataContext(), SelectedPropertiesNode.Create("NavProp1"));
            entryMetadataContext.SelectedNavigationProperties.Should().HaveCount(1).And.Contain(p => p.Name == "NavProp1");
        }
        #endregion SelectedNavigationProperties

        #region SelectedStreamProperties
        [Fact]
        public void SelectedStreamPropertiesShouldReturnEmptyWithoutModel()
        {
            this.entryMetadataContextWithoutModel.SelectedStreamProperties.Should().BeEmpty();            
        }

        [Fact]
        public void SelectedStreamPropertiesShouldReturnPropertiesBasedOnMetadata()
        {
            var entryMetadataContext = ODataEntryMetadataContext.Create(new ODataEntry(), new TestFeedAndEntryTypeContext(), /*serializationInfo*/null, ActualEntityType, new TestMetadataContext(), SelectedPropertiesNode.Create("StreamProp1"));
            entryMetadataContext.SelectedStreamProperties.ContainsKey("StreamProp1").Should().BeTrue();
        }
        #endregion SelectedStreamProperties

        #region SelectedBindableOperations
        [Fact]
        public void SelectedBindableOperationsShouldReturnEmptyWithoutModel()
        {
            var metadataContext = new TestMetadataContext { GetBindableOperationsForTypeFunc = type => new IEdmOperation[] { Action1, Action2, Function1, Function2 }, OperationsBoundToEntityTypeMustBeContainerQualifiedFunc = type => false };
            var entryMetadataContext = ODataEntryMetadataContext.Create(new ODataEntry(), new TestFeedAndEntryTypeContext(), new ODataFeedAndEntrySerializationInfo(), /*actualEntityType*/null, metadataContext, SelectedPropertiesNode.Create("Action1,Function1"));
            entryMetadataContext.SelectedBindableOperations.Should().BeEmpty();
        }

        [Fact]
        public void SelectedBindableOperationsShouldReturnPropertiesBasedOnMetadata()
        {
            var metadataContext = new TestMetadataContext { GetBindableOperationsForTypeFunc = type => new IEdmOperation[] { Action1, Action2, Function1, Function2 }, OperationsBoundToEntityTypeMustBeContainerQualifiedFunc = type => false };
            var entryMetadataContext = ODataEntryMetadataContext.Create(new ODataEntry(), new TestFeedAndEntryTypeContext(), /*serializationInfo*/null, ActualEntityType, metadataContext, SelectedPropertiesNode.Create("Action1,Function1"));
            entryMetadataContext.SelectedBindableOperations.Should().HaveCount(2).And.Contain(Action1).And.Contain(Function1);
        }
        #endregion SelectedBindableOperations
    }
}
