//---------------------------------------------------------------------
// <copyright file="AutoComputePayloadMetadataInJsonIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Expressions;
using Xunit;

namespace Microsoft.OData.Core.Tests.IntegrationTests.Evaluation
{
    public class AutoComputePayloadMetadataInJsonIntegrationTests
    {
        private readonly ODataEntry entryWithPayloadMetadata = new ODataEntry
        {
            Properties = new[] {
                    new ODataProperty { Name = "ID", Value = 123 }, 
                    new ODataProperty
                    {
                        Name = "StreamProp1", 
                        Value = new ODataStreamReferenceValue
                        {
                            ContentType = "image/jpeg", 
                            EditLink = new Uri("http://example.com/stream/edit"),
                            ReadLink = new Uri("http://example.com/stream/read"),
                            ETag = "stream etag"
                        }
                    }},
            EditLink = new Uri("http://example.com/edit"),
            ReadLink = new Uri("http://example.com/read"),
            Id = new Uri("http://example.com/id"),
            ETag = "etag",
            MediaResource = new ODataStreamReferenceValue
            {
                ContentType = "image/png",
                EditLink = new Uri("http://example.com/mr/edit"),
                ReadLink = new Uri("http://example.com/mr/read"),
                ETag = "mr etag"
            },
        };

        private readonly ODataNavigationLink navLinkWithPayloadMetadata = new ODataNavigationLink
        {
            AssociationLinkUrl = new Uri("http://example.com/association"),
            IsCollection = true,
            Name = "DeferredNavLink",
            Url = new Uri("http://example.com/navigation")
        };

        private readonly ODataNavigationLink containedCollectionNavLinkWithPayloadMetadata = new ODataNavigationLink()
        {
            AssociationLinkUrl = new Uri("http://example.com/expanded/association"),
            IsCollection = true,
            Name = "ContainedNavProp",
            Url = new Uri("http://example.com/expanded/navigation")
        };

        private readonly ODataNavigationLink derivedContainedCollectionNavLinkWithPayloadMetadata = new ODataNavigationLink()
        {
            AssociationLinkUrl = new Uri("http://example.com/expanded/association"),
            IsCollection = true,
            Name = "DerivedContainedNavProp",
            Url = new Uri("http://example.com/expanded/navigation")
        };

        private readonly ODataNavigationLink containedNavLinkWithPayloadMetadata = new ODataNavigationLink()
        {
            AssociationLinkUrl = new Uri("http://example.com/expanded/association"),
            IsCollection = false,
            Name = "ContainedNonCollectionNavProp",
            Url = new Uri("http://example.com/expanded/navigation")
        };

        private readonly ODataNavigationLink expandedNavLinkWithPayloadMetadata = new ODataNavigationLink()
        {
            AssociationLinkUrl = new Uri("http://example.com/expanded/association"),
            IsCollection = true,
            Name = "ExpandedNavLink",
            Url = new Uri("http://example.com/expanded/navigation")
        };

        private readonly ODataNavigationLink navLinkWithoutPayloadMetadata = new ODataNavigationLink
        {
            IsCollection = true,
            Name = "DeferredNavLink",
        };

        private readonly ODataNavigationLink expandedNavLinkWithoutPayloadMetadata = new ODataNavigationLink()
        {
            IsCollection = true,
            Name = "ExpandedNavLink",
        };

        private readonly ODataNavigationLink unknownNonCollectionNavLinkWithPayloadMetadata = new ODataNavigationLink()
        {
            AssociationLinkUrl = new Uri("http://example.com/expanded/association"),
            IsCollection = false,
            Name = "UnknownNonCollectionNavProp",
            Url = new Uri("http://example.com/expanded/navigation")
        };

        private readonly ODataNavigationLink unknownCollectionNavLinkWithPayloadMetadata = new ODataNavigationLink()
        {
            AssociationLinkUrl = new Uri("http://example.com/expanded/association"),
            IsCollection = true,
            Name = "UnknownCollectionNavProp",
            Url = new Uri("http://example.com/expanded/navigation")
        };

        private static readonly EdmEntityType EntityType;
        private static readonly EdmEntityType DerivedType;
        private static readonly EdmEntityType AnotherEntityType;
        private static readonly EdmEntitySet EntitySet;
        private static readonly EdmModel Model;

        private const string PayloadWithNoMetadata = "{\"ID\":123,\"ExpandedNavLink\":[]}";

        private const string PayloadMetadataWithoutOpeningBrace =
                "\"@odata.id\":\"http://example.com/id\"," +
                "\"@odata.etag\":\"etag\"," +
                "\"@odata.editLink\":\"http://example.com/edit\"," +
                "\"@odata.readLink\":\"http://example.com/read\"," +
                "\"@odata.mediaEditLink\":\"http://example.com/mr/edit\"," +
                "\"@odata.mediaReadLink\":\"http://example.com/mr/read\"," +
                "\"@odata.mediaContentType\":\"image/png\"," +
                "\"@odata.mediaEtag\":\"mr etag\"," +
                "\"ID\":123," +
                "\"StreamProp1@odata.mediaEditLink\":\"http://example.com/stream/edit\"," +
                "\"StreamProp1@odata.mediaReadLink\":\"http://example.com/stream/read\"," +
                "\"StreamProp1@odata.mediaContentType\":\"image/jpeg\"," +
                "\"StreamProp1@odata.mediaEtag\":\"stream etag\"," +
                "\"DeferredNavLink@odata.associationLink\":\"http://example.com/association\"," +
                "\"DeferredNavLink@odata.navigationLink\":\"http://example.com/navigation\"," +
                "\"ExpandedNavLink@odata.associationLink\":\"http://example.com/expanded/association\"," +
                "\"ExpandedNavLink@odata.navigationLink\":\"http://example.com/expanded/navigation\"," +
                "\"ExpandedNavLink\":[]," +
                "\"#Action\":{\"title\":\"ActionTitle\",\"target\":\"http://example.com/DoAction\"}," +
                "\"#Function\":{\"title\":\"FunctionTitle\",\"target\":\"http://example.com/DoFunction\"}" +
            "}";

        private const string PayloadWithAllMetadataExceptODataDotContext = "{" + PayloadMetadataWithoutOpeningBrace;
        private const string PayloadWithAllMetadata =
            "{" +
            "\"@odata.context\":\"http://example.com/$metadata#EntitySet/$entity\"," +
            PayloadMetadataWithoutOpeningBrace;

        private const string PayloadMetadataWithoutOpeningBraceODataSimplified =
            "\"@id\":\"http://example.com/id\"," +
            "\"@etag\":\"etag\"," +
            "\"@editLink\":\"http://example.com/edit\"," +
            "\"@readLink\":\"http://example.com/read\"," +
            "\"@mediaEditLink\":\"http://example.com/mr/edit\"," +
            "\"@mediaReadLink\":\"http://example.com/mr/read\"," +
            "\"@mediaContentType\":\"image/png\"," +
            "\"@mediaEtag\":\"mr etag\"," +
            "\"ID\":123," +
            "\"StreamProp1@mediaEditLink\":\"http://example.com/stream/edit\"," +
            "\"StreamProp1@mediaReadLink\":\"http://example.com/stream/read\"," +
            "\"StreamProp1@mediaContentType\":\"image/jpeg\"," +
            "\"StreamProp1@mediaEtag\":\"stream etag\"," +
            "\"DeferredNavLink@associationLink\":\"http://example.com/association\"," +
            "\"DeferredNavLink@navigationLink\":\"http://example.com/navigation\"," +
            "\"ExpandedNavLink@associationLink\":\"http://example.com/expanded/association\"," +
            "\"ExpandedNavLink@navigationLink\":\"http://example.com/expanded/navigation\"," +
            "\"ExpandedNavLink\":[]," +
            "\"#Action\":{\"title\":\"ActionTitle\",\"target\":\"http://example.com/DoAction\"}," +
            "\"#Function\":{\"title\":\"FunctionTitle\",\"target\":\"http://example.com/DoFunction\"}" +
        "}";

        private const string PayloadWithAllMetadataODataSimplified =
            "{" +
            "\"@context\":\"http://example.com/$metadata#EntitySet/$entity\"," +
            PayloadMetadataWithoutOpeningBraceODataSimplified;

        private ODataEntry entryWithOnlyData;
        private ODataEntry entryWithOnlyData2;
        private ODataEntry entryWithOnlyData3;
        private ODataEntry derivedEntry;

        public AutoComputePayloadMetadataInJsonIntegrationTests()
        {
            entryWithPayloadMetadata.AddAction(new ODataAction { Metadata = new Uri("http://example.com/$metadata#Action"), Target = new Uri("http://example.com/DoAction"), Title = "ActionTitle" });
            entryWithPayloadMetadata.AddFunction(new ODataFunction() { Metadata = new Uri("http://example.com/$metadata#Function"), Target = new Uri("http://example.com/DoFunction"), Title = "FunctionTitle" });

            this.entryWithOnlyData = new ODataEntry { Properties = new[] { new ODataProperty { Name = "ID", Value = 123 }, new ODataProperty { Name = "Name", Value = "Bob" } }, };
            this.entryWithOnlyData2 = new ODataEntry { Properties = new[] { new ODataProperty { Name = "ID", Value = 234 }, new ODataProperty { Name = "Name", Value = "Foo" } }, };
            this.entryWithOnlyData3 = new ODataEntry { Properties = new[] { new ODataProperty { Name = "ID", Value = 345 }, new ODataProperty { Name = "Name", Value = "Bar" } }, };

            this.derivedEntry = new ODataEntry
            {
                TypeName = "Namespace.DerivedType",
                Properties = new[] { new ODataProperty { Name = "ID", Value = 345 }, new ODataProperty { Name = "Name", Value = "Bar" } },
            };
        }

        static AutoComputePayloadMetadataInJsonIntegrationTests()
        {
            EntityType = new EdmEntityType("Namespace", "EntityType", null, false, false, true);
            EntityType.AddKeys(EntityType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
            EntityType.AddStructuralProperty("StreamProp1", EdmPrimitiveTypeKind.Stream);
            EntityType.AddStructuralProperty("StreamProp2", EdmPrimitiveTypeKind.Stream);
            EntityType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(isNullable: true), null, EdmConcurrencyMode.Fixed);
            DerivedType = new EdmEntityType("Namespace", "DerivedType", EntityType, false, true);
            AnotherEntityType = new EdmEntityType("Namespace", "AnotherEntityType", null, false, false, true);
            AnotherEntityType.AddKeys(AnotherEntityType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
            AnotherEntityType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(isNullable: true), null, EdmConcurrencyMode.Fixed);

            var deferredNavLinkProp = EntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Target = EntityType,
                TargetMultiplicity = EdmMultiplicity.Many,
                Name = "DeferredNavLink"
            });

            var expandedNavLinkProp = EntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Target = EntityType,
                TargetMultiplicity = EdmMultiplicity.Many,
                Name = "ExpandedNavLink"
            });

            var navLinkDeclaredOnlyInModelProp = EntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Target = EntityType,
                TargetMultiplicity = EdmMultiplicity.Many,
                Name = "NavLinkDeclaredOnlyInModel"
            });

            EntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Target = EntityType,
                TargetMultiplicity = EdmMultiplicity.Many,
                Name = "ContainedNavProp",
                ContainsTarget = true
            });

            DerivedType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Target = EntityType,
                TargetMultiplicity = EdmMultiplicity.Many,
                Name = "DerivedContainedNavProp",
                ContainsTarget = true
            });

            EntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Target = EntityType,
                TargetMultiplicity = EdmMultiplicity.One,
                Name = "ContainedNonCollectionNavProp",
                ContainsTarget = true
            });

            EntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Target = AnotherEntityType,
                TargetMultiplicity = EdmMultiplicity.Many,
                Name = "AnotherContainedNavProp",
                ContainsTarget = true
            });

            EntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Target = AnotherEntityType,
                TargetMultiplicity = EdmMultiplicity.One,
                Name = "AnotherContainedNonCollectionNavProp",
                ContainsTarget = true
            });

            EntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Target = EntityType,
                TargetMultiplicity = EdmMultiplicity.One,
                Name = "UnknownNonCollectionNavProp",
                ContainsTarget = false
            });

            EntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Target = EntityType,
                TargetMultiplicity = EdmMultiplicity.Many,
                Name = "UnknownCollectionNavProp",
                ContainsTarget = false
            });

            var container = new EdmEntityContainer("Namespace", "Container");
            EntitySet = container.AddEntitySet("EntitySet", EntityType);

            EntitySet.AddNavigationTarget(deferredNavLinkProp, EntitySet);
            EntitySet.AddNavigationTarget(expandedNavLinkProp, EntitySet);
            EntitySet.AddNavigationTarget(navLinkDeclaredOnlyInModelProp, EntitySet);

            Model = new EdmModel();
            Model.AddElement(EntityType);
            Model.AddElement(DerivedType);
            Model.AddElement(AnotherEntityType);
            Model.AddElement(container);

            var alwaysBindableAction1 = new EdmAction("Namespace", "AlwaysBindableAction1", null /*returnType*/, true /*isBound*/, null /*entitySetPath*/);
            alwaysBindableAction1.AddParameter(new EdmOperationParameter(alwaysBindableAction1, "p", new EdmEntityTypeReference(EntityType, isNullable: true)));
            Model.AddElement(alwaysBindableAction1);
            var alwaysBindableActionImport1 = new EdmActionImport(container, "AlwaysBindableAction1", alwaysBindableAction1);
            container.AddElement(alwaysBindableActionImport1);

            var alwaysBindableAction2 = new EdmAction("Namespace", "AlwaysBindableAction2", null /*returnType*/, true /*isBound*/, null /*entitySetPath*/);
            alwaysBindableAction2.AddParameter(new EdmOperationParameter(alwaysBindableAction2, "p", new EdmEntityTypeReference(EntityType, isNullable: true)));
            Model.AddElement(alwaysBindableAction2);
            var alwaysBindableActionImport2 = new EdmActionImport(container, "AlwaysBindableAction2", alwaysBindableAction2);
            container.AddElement(alwaysBindableActionImport2);

            var action1 = new EdmAction("Namespace", "Action1", null /*returnType*/, false /*isBound*/, null /*entitySetPath*/);
            action1.AddParameter(new EdmOperationParameter(action1, "p", new EdmEntityTypeReference(EntityType, isNullable: true)));
            Model.AddElement(action1);
            var actionImport1 = new EdmActionImport(container, "Action1", action1);
            container.AddElement(actionImport1);

            var action2 = new EdmAction("Namespace", "Action1", null /*returnType*/, false /*isBound*/, null /*entitySetPath*/);
            action2.AddParameter(new EdmOperationParameter(action2, "p", new EdmEntityTypeReference(EntityType, isNullable: true)));
            Model.AddElement(action2);
            var actionImport2 = new EdmActionImport(container, "Action1", action2);
            container.AddElement(actionImport2);

            var alwaysBindableFunction1 = new EdmFunction("Namespace", "AlwaysBindableFunction1", EdmCoreModel.Instance.GetString(isNullable: true), true /*isBound*/, null /*entitySetPath*/, false /*iscomposable*/);
            alwaysBindableFunction1.AddParameter("p", new EdmEntityTypeReference(EntityType, isNullable: true));
            Model.AddElement(alwaysBindableFunction1);
            var alwaysBindableFunctionImport1 = new EdmFunctionImport(container, "AlwaysBindableFunction1", alwaysBindableFunction1);
            container.AddElement(alwaysBindableFunctionImport1);

            var alwaysBindableFunction2 = new EdmFunction("Namespace", "AlwaysBindableFunction2", EdmCoreModel.Instance.GetString(isNullable: true), true /*isBound*/, null /*entitySetPath*/, false /*iscomposable*/);
            alwaysBindableFunction2.AddParameter("p", new EdmEntityTypeReference(EntityType, isNullable: true));
            Model.AddElement(alwaysBindableFunction2);
            var alwaysBindableFunctionImport2 = new EdmFunctionImport(container, "AlwaysBindableFunction2", alwaysBindableFunction2);
            container.AddElement(alwaysBindableFunctionImport2);

            var function1 = new EdmFunction("Namespace", "Function1", EdmCoreModel.Instance.GetString(isNullable: true), false /*isBound*/, null /*entitySetPath*/, false /*iscomposable*/);
            function1.AddParameter("p", new EdmEntityTypeReference(EntityType, isNullable: true));
            Model.AddElement(function1);
            var functionImport1 = new EdmFunctionImport(container, "Function1", function1);
            container.AddElement(functionImport1);

            var function2 = new EdmFunction("Namespace", "Function1", EdmCoreModel.Instance.GetString(isNullable: true), false /*isBound*/, null /*entitySetPath*/, false /*iscomposable*/);
            function2.AddParameter("p", new EdmEntityTypeReference(EntityType, isNullable: true));
            Model.AddElement(function2);
            var functionImport2 = new EdmFunctionImport(container, "Function1", function2);
            container.AddElement(functionImport2);

            var function3 = new EdmFunction("Namespace", "Function3", new EdmEntityTypeReference(EntityType, false), true /*isBound*/, new EdmPathExpression("p/ContainedNonCollectionNavProp"), false /*iscomposable*/);
            function3.AddParameter("p", new EdmEntityTypeReference(EntityType, isNullable: true));
            Model.AddElement(function3);
        }

        [Fact]
        public void WritingSimplifiedODataAnnotationsInFullMetadataMode()
        {
            GetWriterOutputForEntryWithPayloadMetadata("application/json;odata.metadata=full", false, odataSimplified: true)
                .Should().Be(PayloadWithAllMetadataODataSimplified);
        }

        [Fact]
        public void WritingInNoMetadataModeShouldStripPayloadMetadataIfAutoComputePayloadMetadataInJsonIsTrue()
        {
            GetWriterOutputForEntryWithPayloadMetadata("application/json;odata.metadata=none", true)
                .Should().Be(PayloadWithNoMetadata);
        }

        [Fact]
        public void WritingInNoMetadataModeShouldNotStripPayloadMetadataIfAutoComputePayloadMetadataInJsonIsFalse()
        {
            GetWriterOutputForEntryWithPayloadMetadata("application/json;odata.metadata=none", false)
                .Should().Be(PayloadWithAllMetadataExceptODataDotContext);
        }

        [Fact]
        public void WritingInMinimalMetadataModeShouldNotStripPayloadMetadataIfAutoComputePayloadMetadataInJsonIsTrue()
        {
            GetWriterOutputForEntryWithPayloadMetadata("application/json;odata.metadata=minimal", true)
                .Should().Be(PayloadWithAllMetadata);
        }

        [Fact]
        public void WritingInMinimalMetadataModeShouldNotStripPayloadMetadataIfAutoComputePayloadMetadataInJsonIsFalse()
        {
            GetWriterOutputForEntryWithPayloadMetadata("application/json;odata.metadata=minimal", false)
                .Should().Be(PayloadWithAllMetadata);
        }

        [Fact]
        public void WritingInFullMetadataModeShouldNotStripPayloadMetadataIfAutoComputePayloadMetadataInJsonIsFalse()
        {
            GetWriterOutputForEntryWithPayloadMetadata("application/json;odata.metadata=full", false)
                .Should().Be(PayloadWithAllMetadata);
        }

        [Fact]
        public void WritingInFullMetadataModeShouldNotStripPayloadMetadataAndShouldWriteMissingMetadataIfAutoComputePayloadMetadataInJsonIsTrue()
        {
            const string expectedPayload = "{" +
                               "\"@odata.context\":\"http://example.com/$metadata#EntitySet/$entity\"," +
                               "\"@odata.id\":\"http://example.com/id\"," +
                               "\"@odata.etag\":\"etag\"," +
                               "\"@odata.editLink\":\"http://example.com/edit\"," +
                               "\"@odata.readLink\":\"http://example.com/read\"," +
                               "\"@odata.mediaEditLink\":\"http://example.com/mr/edit\"," +
                               "\"@odata.mediaReadLink\":\"http://example.com/mr/read\"," +
                               "\"@odata.mediaContentType\":\"image/png\"," +
                               "\"@odata.mediaEtag\":\"mr etag\"," +
                               "\"ID\":123," +
                               "\"StreamProp1@odata.mediaEditLink\":\"http://example.com/stream/edit\"," +
                               "\"StreamProp1@odata.mediaReadLink\":\"http://example.com/stream/read\"," +
                               "\"StreamProp1@odata.mediaContentType\":\"image/jpeg\"," +
                               "\"StreamProp1@odata.mediaEtag\":\"stream etag\"," +
                               "\"StreamProp2@odata.mediaEditLink\":\"http://example.com/edit/StreamProp2\"," +
                               "\"StreamProp2@odata.mediaReadLink\":\"http://example.com/read/StreamProp2\"," +
                               "\"DeferredNavLink@odata.associationLink\":\"http://example.com/association\"," +
                               "\"DeferredNavLink@odata.navigationLink\":\"http://example.com/navigation\"," +
                               "\"ExpandedNavLink@odata.associationLink\":\"http://example.com/expanded/association\"," +
                               "\"ExpandedNavLink@odata.navigationLink\":\"http://example.com/expanded/navigation\"," +
                               "\"ExpandedNavLink\":[]," +
                               "\"NavLinkDeclaredOnlyInModel@odata.associationLink\":\"http://example.com/read/NavLinkDeclaredOnlyInModel/$ref\"," +
                               "\"NavLinkDeclaredOnlyInModel@odata.navigationLink\":\"http://example.com/read/NavLinkDeclaredOnlyInModel\"," +
                               "\"ContainedNavProp@odata.associationLink\":\"http://example.com/read/ContainedNavProp/$ref\"," +
                               "\"ContainedNavProp@odata.navigationLink\":\"http://example.com/read/ContainedNavProp\"," +
                               "\"ContainedNonCollectionNavProp@odata.associationLink\":\"http://example.com/read/ContainedNonCollectionNavProp/$ref\"," +
                               "\"ContainedNonCollectionNavProp@odata.navigationLink\":\"http://example.com/read/ContainedNonCollectionNavProp\"," +
                               "\"AnotherContainedNavProp@odata.associationLink\":\"http://example.com/read/AnotherContainedNavProp/$ref\"," +
                               "\"AnotherContainedNavProp@odata.navigationLink\":\"http://example.com/read/AnotherContainedNavProp\"," +
                               "\"AnotherContainedNonCollectionNavProp@odata.associationLink\":\"http://example.com/read/AnotherContainedNonCollectionNavProp/$ref\"," +
                               "\"AnotherContainedNonCollectionNavProp@odata.navigationLink\":\"http://example.com/read/AnotherContainedNonCollectionNavProp\"," +
                               "\"UnknownNonCollectionNavProp@odata.associationLink\":\"http://example.com/read/UnknownNonCollectionNavProp/$ref\"," +
                               "\"UnknownNonCollectionNavProp@odata.navigationLink\":\"http://example.com/read/UnknownNonCollectionNavProp\"," +
                               "\"UnknownCollectionNavProp@odata.associationLink\":\"http://example.com/read/UnknownCollectionNavProp/$ref\"," +
                               "\"UnknownCollectionNavProp@odata.navigationLink\":\"http://example.com/read/UnknownCollectionNavProp\"," +
                               "\"#Action\":{\"title\":\"ActionTitle\",\"target\":\"http://example.com/DoAction\"}," +
                               "\"#Namespace.AlwaysBindableAction1\":{\"title\":\"Namespace.AlwaysBindableAction1\",\"target\":\"http://example.com/edit/Namespace.AlwaysBindableAction1\"}," +
                               "\"#Namespace.AlwaysBindableAction2\":{\"title\":\"Namespace.AlwaysBindableAction2\",\"target\":\"http://example.com/edit/Namespace.AlwaysBindableAction2\"}," +
                               "\"#Function\":{\"title\":\"FunctionTitle\",\"target\":\"http://example.com/DoFunction\"}," +
                               "\"#Namespace.AlwaysBindableFunction1\":{\"title\":\"Namespace.AlwaysBindableFunction1\",\"target\":\"http://example.com/edit/Namespace.AlwaysBindableFunction1\"}," +
                               "\"#Namespace.AlwaysBindableFunction2\":{\"title\":\"Namespace.AlwaysBindableFunction2\",\"target\":\"http://example.com/edit/Namespace.AlwaysBindableFunction2\"}," +
                               "\"#Namespace.Function3\":{\"title\":\"Namespace.Function3\",\"target\":\"http://example.com/edit/Namespace.Function3\"}" +
                               "}";

            var actualPayload = GetWriterOutputForEntryWithPayloadMetadata("application/json;odata.metadata=full", true);
            actualPayload.Should().Be(expectedPayload);
        }

        [Fact]
        public void WritingInFullMetadataModeShouldWriteMissingMetadataForEntryWithOnlyDataIfAutoComputePayloadMetadataInJsonIsTrue()
        {
            const string expectedPayload = "{" +
                                           "\"@odata.context\":\"http://example.com/$metadata#EntitySet/$entity\"," +
                                           "\"@odata.id\":\"EntitySet(123)\"," +
                                           "\"@odata.etag\":\"W/\\\"'Bob'\\\"\"," +
                                           "\"@odata.editLink\":\"EntitySet(123)\"," +
                                           "\"@odata.mediaEditLink\":\"EntitySet(123)/$value\"," +
                                           "\"ID\":123," +
                                           "\"Name\":\"Bob\"," +
                                           "\"StreamProp1@odata.mediaEditLink\":\"http://example.com/EntitySet(123)/StreamProp1\"," +
                                           "\"StreamProp1@odata.mediaReadLink\":\"http://example.com/EntitySet(123)/StreamProp1\"," +
                                           "\"StreamProp2@odata.mediaEditLink\":\"http://example.com/EntitySet(123)/StreamProp2\"," +
                                           "\"StreamProp2@odata.mediaReadLink\":\"http://example.com/EntitySet(123)/StreamProp2\"," +
                                           "\"DeferredNavLink@odata.associationLink\":\"http://example.com/EntitySet(123)/DeferredNavLink/$ref\"," +
                                           "\"DeferredNavLink@odata.navigationLink\":\"http://example.com/EntitySet(123)/DeferredNavLink\"," +
                                           "\"ExpandedNavLink@odata.associationLink\":\"http://example.com/EntitySet(123)/ExpandedNavLink/$ref\"," +
                                           "\"ExpandedNavLink@odata.navigationLink\":\"http://example.com/EntitySet(123)/ExpandedNavLink\"," +
                                           "\"ExpandedNavLink\":[]," +
                                           "\"NavLinkDeclaredOnlyInModel@odata.associationLink\":\"http://example.com/EntitySet(123)/NavLinkDeclaredOnlyInModel/$ref\"," +
                                           "\"NavLinkDeclaredOnlyInModel@odata.navigationLink\":\"http://example.com/EntitySet(123)/NavLinkDeclaredOnlyInModel\"," +
                                           "\"ContainedNavProp@odata.associationLink\":\"http://example.com/EntitySet(123)/ContainedNavProp/$ref\"," +
                                           "\"ContainedNavProp@odata.navigationLink\":\"http://example.com/EntitySet(123)/ContainedNavProp\"," +
                                           "\"ContainedNonCollectionNavProp@odata.associationLink\":\"http://example.com/EntitySet(123)/ContainedNonCollectionNavProp/$ref\"," +
                                           "\"ContainedNonCollectionNavProp@odata.navigationLink\":\"http://example.com/EntitySet(123)/ContainedNonCollectionNavProp\"," +
                                           "\"AnotherContainedNavProp@odata.associationLink\":\"http://example.com/EntitySet(123)/AnotherContainedNavProp/$ref\"," +
                                           "\"AnotherContainedNavProp@odata.navigationLink\":\"http://example.com/EntitySet(123)/AnotherContainedNavProp\"," +
                                           "\"AnotherContainedNonCollectionNavProp@odata.associationLink\":\"http://example.com/EntitySet(123)/AnotherContainedNonCollectionNavProp/$ref\"," +
                                           "\"AnotherContainedNonCollectionNavProp@odata.navigationLink\":\"http://example.com/EntitySet(123)/AnotherContainedNonCollectionNavProp\"," +
                                           "\"UnknownNonCollectionNavProp@odata.associationLink\":\"http://example.com/EntitySet(123)/UnknownNonCollectionNavProp/$ref\"," +
                                           "\"UnknownNonCollectionNavProp@odata.navigationLink\":\"http://example.com/EntitySet(123)/UnknownNonCollectionNavProp\"," +
                                           "\"UnknownCollectionNavProp@odata.associationLink\":\"http://example.com/EntitySet(123)/UnknownCollectionNavProp/$ref\"," +
                                           "\"UnknownCollectionNavProp@odata.navigationLink\":\"http://example.com/EntitySet(123)/UnknownCollectionNavProp\"," +
                                           "\"#Namespace.AlwaysBindableAction1\":{\"title\":\"Namespace.AlwaysBindableAction1\",\"target\":\"http://example.com/EntitySet(123)/Namespace.AlwaysBindableAction1\"}," +
                                           "\"#Namespace.AlwaysBindableAction2\":{\"title\":\"Namespace.AlwaysBindableAction2\",\"target\":\"http://example.com/EntitySet(123)/Namespace.AlwaysBindableAction2\"}," +
                                           "\"#Namespace.AlwaysBindableFunction1\":{\"title\":\"Namespace.AlwaysBindableFunction1\",\"target\":\"http://example.com/EntitySet(123)/Namespace.AlwaysBindableFunction1\"}," +
                                           "\"#Namespace.AlwaysBindableFunction2\":{\"title\":\"Namespace.AlwaysBindableFunction2\",\"target\":\"http://example.com/EntitySet(123)/Namespace.AlwaysBindableFunction2\"}," +
                                           "\"#Namespace.Function3\":{\"title\":\"Namespace.Function3\",\"target\":\"http://example.com/EntitySet(123)/Namespace.Function3\"}" +
                                           "}";
            GetWriterOutputForEntryWithOnlyData("application/json;odata.metadata=full", true)
                .Should().Be(expectedPayload);
        }

        [Fact]
        public void WritingInFullMetadataModeWithProjectionShouldWriteMissingMetadataForEntryWithOnlyDataIfAutoComputePayloadMetadataInJsonIsTrue()
        {
            const string expectedPayload = "{" +
                                           "\"@odata.context\":\"http://example.com/$metadata#EntitySet(StreamProp1,Namespace.AlwaysBindableAction1,Namespace.AlwaysBindableFunction1)/$entity\"," +
                                           "\"@odata.id\":\"EntitySet(123)\"," +
                                           "\"@odata.etag\":\"W/\\\"'Bob'\\\"\"," +
                                           "\"@odata.editLink\":\"EntitySet(123)\"," +
                                           "\"@odata.mediaEditLink\":\"EntitySet(123)/$value\"," +
                                           "\"ID\":123," +
                                           "\"Name\":\"Bob\"," +
                                           "\"StreamProp1@odata.mediaEditLink\":\"http://example.com/EntitySet(123)/StreamProp1\"," +
                                           "\"StreamProp1@odata.mediaReadLink\":\"http://example.com/EntitySet(123)/StreamProp1\"," +
                                           "\"DeferredNavLink@odata.associationLink\":\"http://example.com/EntitySet(123)/DeferredNavLink/$ref\"," +
                                           "\"DeferredNavLink@odata.navigationLink\":\"http://example.com/EntitySet(123)/DeferredNavLink\"," +
                                           "\"ExpandedNavLink@odata.associationLink\":\"http://example.com/EntitySet(123)/ExpandedNavLink/$ref\"," +
                                           "\"ExpandedNavLink@odata.navigationLink\":\"http://example.com/EntitySet(123)/ExpandedNavLink\"," +
                                           "\"ExpandedNavLink\":[]," +
                                           "\"#Namespace.AlwaysBindableAction1\":{\"title\":\"Namespace.AlwaysBindableAction1\",\"target\":\"http://example.com/EntitySet(123)/Namespace.AlwaysBindableAction1\"}," +
                                           "\"#Namespace.AlwaysBindableFunction1\":{\"title\":\"Namespace.AlwaysBindableFunction1\",\"target\":\"http://example.com/EntitySet(123)/Namespace.AlwaysBindableFunction1\"}" +
                                           "}";
            GetWriterOutputForEntryWithOnlyData("application/json;odata.metadata=full", true, "StreamProp1,Namespace.AlwaysBindableAction1,Namespace.AlwaysBindableFunction1")
                .Should().Be(expectedPayload);
        }

        [Fact]
        public void WritingInFullMetadataModeWithExpandAndProjectionShouldWriteMissingMetadataForProjectedPropertiesIfAutoComputePayloadMetadataInJsonIsTrue()
        {
            const string expectedPayload =
                "{" +
                    "\"@odata.context\":\"http://example.com/$metadata#EntitySet(StreamProp1,Namespace.AlwaysBindableAction1,Namespace.AlwaysBindableFunction1,DeferredNavLink,ExpandedNavLink,ExpandedNavLink(StreamProp1,Namespace.AlwaysBindableAction1,ExpandedNavLink,ExpandedNavLink(StreamProp2,Namespace.AlwaysBindableAction1)))\"," +
                    "\"value\":[" +
                    "{" +
                        "\"@odata.id\":\"EntitySet(123)\"," +
                        "\"@odata.etag\":\"W/\\\"'Bob'\\\"\"," +
                        "\"@odata.editLink\":\"EntitySet(123)\"," +
                        "\"@odata.mediaEditLink\":\"EntitySet(123)/$value\"," +
                        "\"ID\":123," +
                        "\"Name\":\"Bob\"," +
                        "\"StreamProp1@odata.mediaEditLink\":\"http://example.com/EntitySet(123)/StreamProp1\"," +
                        "\"StreamProp1@odata.mediaReadLink\":\"http://example.com/EntitySet(123)/StreamProp1\"," +
                        "\"ExpandedNavLink@odata.associationLink\":\"http://example.com/expanded/association\"," +
                        "\"ExpandedNavLink@odata.navigationLink\":\"http://example.com/expanded/navigation\"," +
                        "\"ExpandedNavLink\":[" +
                        "{" +
                            "\"@odata.id\":\"EntitySet(234)\"," +
                            "\"@odata.etag\":\"W/\\\"'Foo'\\\"\"," +
                            "\"@odata.editLink\":\"EntitySet(234)\"," +
                            "\"@odata.mediaEditLink\":\"EntitySet(234)/$value\"," +
                            "\"ID\":234," +
                            "\"Name\":\"Foo\"," +
                            "\"StreamProp1@odata.mediaEditLink\":\"http://example.com/EntitySet(234)/StreamProp1\"," +
                            "\"StreamProp1@odata.mediaReadLink\":\"http://example.com/EntitySet(234)/StreamProp1\"," +
                            "\"DeferredNavLink@odata.associationLink\":\"http://example.com/EntitySet(234)/DeferredNavLink/$ref\"," +
                            "\"DeferredNavLink@odata.navigationLink\":\"http://example.com/EntitySet(234)/DeferredNavLink\"," +
                            "\"ExpandedNavLink@odata.associationLink\":\"http://example.com/expanded/association\"," +
                            "\"ExpandedNavLink@odata.navigationLink\":\"http://example.com/expanded/navigation\"," +
                            "\"ExpandedNavLink\":[" +
                            "{" +
                                "\"@odata.id\":\"EntitySet(345)\"," +
                                "\"@odata.etag\":\"W/\\\"'Bar'\\\"\"," +
                                "\"@odata.editLink\":\"EntitySet(345)\"," +
                                "\"@odata.mediaEditLink\":\"EntitySet(345)/$value\"," +
                                "\"ID\":345," +
                                "\"Name\":\"Bar\"," +
                                "\"StreamProp2@odata.mediaEditLink\":\"http://example.com/EntitySet(345)/StreamProp2\"," +
                                "\"StreamProp2@odata.mediaReadLink\":\"http://example.com/EntitySet(345)/StreamProp2\"," +
                                "\"#Namespace.AlwaysBindableAction1\":{\"title\":\"Namespace.AlwaysBindableAction1\",\"target\":\"http://example.com/EntitySet(345)/Namespace.AlwaysBindableAction1\"}" +
                            "}]," +
                            "\"#Namespace.AlwaysBindableAction1\":{\"title\":\"Namespace.AlwaysBindableAction1\",\"target\":\"http://example.com/EntitySet(234)/Namespace.AlwaysBindableAction1\"}" +
                        "}]," +
                        "\"DeferredNavLink@odata.associationLink\":\"http://example.com/EntitySet(123)/DeferredNavLink/$ref\"," +
                        "\"DeferredNavLink@odata.navigationLink\":\"http://example.com/EntitySet(123)/DeferredNavLink\"," +
                        "\"#Namespace.AlwaysBindableAction1\":{\"title\":\"Namespace.AlwaysBindableAction1\",\"target\":\"http://example.com/EntitySet(123)/Namespace.AlwaysBindableAction1\"}," +
                        "\"#Namespace.AlwaysBindableFunction1\":{\"title\":\"Namespace.AlwaysBindableFunction1\",\"target\":\"http://example.com/EntitySet(123)/Namespace.AlwaysBindableFunction1\"}" +
                    "}]" +
                "}";

            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataFeed(), this.entryWithOnlyData,
                this.expandedNavLinkWithPayloadMetadata, new ODataFeed(), this.entryWithOnlyData2, this.navLinkWithoutPayloadMetadata,
                this.expandedNavLinkWithPayloadMetadata, new ODataFeed(), this.entryWithOnlyData3
            };

            const string selectClause = "StreamProp1,Namespace.AlwaysBindableAction1,Namespace.AlwaysBindableFunction1,DeferredNavLink";
            const string expandClause = "ExpandedNavLink($select=StreamProp1,Namespace.AlwaysBindableAction1;$expand=ExpandedNavLink($select=StreamProp2,Namespace.AlwaysBindableAction1))";
            this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=full", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause)
                .Should().Be(expectedPayload);
        }

        [Fact]
        public void WritingInFullMetadataModeWithExpandTheExpanedODataEntryShouldContainsParentODataEntryMetadataBuilderIfAutoComputePayloadMetadataInJsonIsTrue()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataFeed(), this.entryWithOnlyData,
                this.expandedNavLinkWithPayloadMetadata, new ODataFeed(), this.entryWithOnlyData2, this.navLinkWithoutPayloadMetadata,
                this.expandedNavLinkWithPayloadMetadata, new ODataFeed(), this.entryWithOnlyData3
            };

            const string selectClause = "StreamProp1,Namespace.AlwaysBindableAction1,Namespace.AlwaysBindableFunction1,DeferredNavLink";
            const string expandClause = "ExpandedNavLink($select=StreamProp1,Namespace.AlwaysBindableAction1;$expand=ExpandedNavLink($select=StreamProp2,Namespace.AlwaysBindableAction1))";
            this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=full", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause);

            this.entryWithOnlyData.MetadataBuilder.ParentMetadataBuilder.Should().BeNull();
            this.entryWithOnlyData2.MetadataBuilder.ParentMetadataBuilder.Should().Be(this.entryWithOnlyData.MetadataBuilder);
            this.entryWithOnlyData3.MetadataBuilder.ParentMetadataBuilder.Should().Be(this.entryWithOnlyData2.MetadataBuilder);
        }

        [Fact]
        public void WritingInFullMetadataModeWithExpandWithCollectionContainedElementIfAutoComputePayloadMetadataInJsonIsTrue()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataFeed(), this.entryWithOnlyData,
                this.containedCollectionNavLinkWithPayloadMetadata, new ODataFeed(), this.entryWithOnlyData2
            };

            const string selectClause = "ContainedNavProp";
            const string expandClause = "ExpandedNavLink($select=ContainedNavProp)";
            this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=full", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause, "EntitySet(123)");

            Uri containedId = new Uri("http://example.com/EntitySet(123)/ContainedNavProp(234)");
            this.entryWithOnlyData2.Id.Should().Be(containedId);
        }

        [Fact]
        public void WritingInFullMetadataModeWithTopLevelContainedEntity()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                this.entryWithOnlyData
            };

            IEdmNavigationProperty containedNavProp = EntityType.FindProperty("ContainedNavProp") as IEdmNavigationProperty;
            IEdmEntitySetBase contianedEntitySet = EntitySet.FindNavigationTarget(containedNavProp) as IEdmEntitySetBase;
            string resourcePath = "EntitySet(123)/ContainedNavProp(123)";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=full", true, itemsToWrite, Model, contianedEntitySet, EntityType, null, null, resourcePath);

            Uri containedId = new Uri("http://example.com/EntitySet(123)/ContainedNavProp(123)");
            this.entryWithOnlyData.Id.Should().Be(containedId);

            string expectedContextUriString = "$metadata#EntitySet(123)/ContainedNavProp/$entity";
            result.Should().Contain(expectedContextUriString);
        }

        [Fact]
        public void WritingInFullMetadataModeWithTopLevelContainedEntityWithFunctionUriPath()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                this.entryWithOnlyData
            };

            IEdmNavigationProperty containedNavProp = EntityType.FindProperty("ContainedNonCollectionNavProp") as IEdmNavigationProperty;
            IEdmEntitySetBase contianedEntitySet = EntitySet.FindNavigationTarget(containedNavProp) as IEdmEntitySetBase;
            string resourcePath = "EntitySet(123)/Namespace.Function3";
            Action test = () => this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=full", true, itemsToWrite, Model, contianedEntitySet, EntityType, null, null, resourcePath);

            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataContextUriBuilder_ODataPathInvalidForContainedElement(resourcePath));
        }

        [Fact]
        public void WritingInFullMetadataModeWithTopLevelContainedEntryWithTypeCast()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                this.entryWithOnlyData
            };

            IEdmNavigationProperty containedDerivedNavProp = EntityType.FindProperty("ContainedNavProp") as IEdmNavigationProperty;
            IEdmEntitySetBase contianedEntitySet = EntitySet.FindNavigationTarget(containedDerivedNavProp) as IEdmEntitySetBase;
            string resourcePath = "EntitySet(123)/ContainedNavProp(123)/Namespace.DerivedType/";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=full", true, itemsToWrite, Model, contianedEntitySet, DerivedType, null, null, resourcePath);

            Uri containedId = new Uri("http://example.com/EntitySet(123)/ContainedNavProp(123)");
            this.entryWithOnlyData.Id.Should().Be(containedId);

            string expectedContextUriString = "$metadata#EntitySet(123)/ContainedNavProp/Namespace.DerivedType/$entity";
            result.Should().Contain(expectedContextUriString);
        }

        [Fact]
        public void WritingInFullMetadataModeWithTopLevelContainedFeedWithTypeCast()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataFeed(), this.entryWithOnlyData, this.entryWithOnlyData2
            };

            IEdmNavigationProperty containedDerivedNavProp = EntityType.FindProperty("ContainedNavProp") as IEdmNavigationProperty;
            IEdmEntitySetBase contianedEntitySet = EntitySet.FindNavigationTarget(containedDerivedNavProp) as IEdmEntitySetBase;
            string resourcePath = "EntitySet(123)/ContainedNavProp/Namespace.DerivedType/";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=full", true, itemsToWrite, Model, contianedEntitySet, DerivedType, null, null, resourcePath);

            string expectedContextUriString = "$metadata#EntitySet(123)/ContainedNavProp/Namespace.DerivedType\"";
            result.Should().Contain(expectedContextUriString);
        }

        [Fact]
        public void WritingInFullMetadataModeWithTopLevelContainedEntityInFeed()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataFeed(), this.entryWithOnlyData
            };

            IEdmNavigationProperty containedNavProp = EntityType.FindProperty("ContainedNavProp") as IEdmNavigationProperty;
            IEdmEntitySetBase contianedEntitySet = EntitySet.FindNavigationTarget(containedNavProp) as IEdmEntitySetBase;
            string resourcePath = "EntitySet(123)/ContainedNavProp";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=full", true, itemsToWrite, Model, contianedEntitySet, EntityType, null, null, resourcePath);

            Uri containedId = new Uri("http://example.com/EntitySet(123)/ContainedNavProp(123)");
            this.entryWithOnlyData.Id.Should().Be(containedId);

            string expectedContextUriString = "$metadata#EntitySet(123)/ContainedNavProp";
            result.Should().Contain(expectedContextUriString);
        }

        [Fact]
        public void WritingInFullMetadataModeWithExpandWithContainedElementIfAutoComputePayloadMetadataInJsonIsTrue()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataFeed(), this.entryWithOnlyData,
                this.containedNavLinkWithPayloadMetadata, this.entryWithOnlyData2,
                this.containedNavLinkWithPayloadMetadata, this.entryWithOnlyData3
            };

            const string selectClause = "ContainedNonCollectionNavProp";
            const string expandClause = "ExpandedNavLink($select=ContainedNonCollectionNavProp;$expand=ExpandedNavLink($select=ContainedNonCollectionNavProp))";
            this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=full", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause, "EntitySet");

            Uri containedIdLevel1 = new Uri("http://example.com/EntitySet(123)/ContainedNonCollectionNavProp");
            this.entryWithOnlyData2.Id.Should().Be(containedIdLevel1);
            Uri containedIdLevel2 = new Uri("http://example.com/EntitySet(123)/ContainedNonCollectionNavProp/ContainedNonCollectionNavProp");
            this.entryWithOnlyData3.Id.Should().Be(containedIdLevel2);
        }

        [Fact]
        public void WritingInFullMetadataModeWithContainedEntityInBaseEntityInFeed()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataFeed(), 
                this.derivedEntry,
                this.containedNavLinkWithPayloadMetadata,
                this.entryWithOnlyData
            };

            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=full", true, itemsToWrite, Model, EntitySet, DerivedType, null, null, "EntitySet");

            Uri containedId = new Uri("http://example.com/EntitySet(345)/ContainedNonCollectionNavProp");
            this.entryWithOnlyData.Id.Should().Be(containedId);
        }

        [Fact]
        public void WritingInFullMetadataModeWithDirectyAccessContainedEntityInFeed()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataFeed(), 
                this.derivedEntry,
            };

            var navProp = EntityType.FindProperty("ContainedNonCollectionNavProp") as IEdmNavigationProperty;
            var containedEntitySet = EntitySet.FindNavigationTarget(navProp) as IEdmEntitySetBase;

            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=full", true, itemsToWrite, Model, containedEntitySet, DerivedType, null, null, "EntitySet(0)/Namespace.DerivedType/ContainedNonCollectionNavProp");

            Uri containedId = new Uri("http://example.com/EntitySet(0)/ContainedNonCollectionNavProp");
            this.derivedEntry.Id.Should().Be(containedId);
        }

        [Fact]
        public void WritingInFullMetadataModeWithDirectyAccessDerivedContainedEntityInFeed()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataFeed(), 
                this.derivedEntry,
            };

            var navProp = DerivedType.FindProperty("DerivedContainedNavProp") as IEdmNavigationProperty;
            var containedEntitySet = EntitySet.FindNavigationTarget(navProp) as IEdmEntitySetBase;

            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=full", true, itemsToWrite, Model, containedEntitySet, DerivedType, null, null, "EntitySet(0)/Namespace.DerivedType/DerivedContainedNavProp");

            Uri containedId = new Uri("http://example.com/EntitySet(0)/Namespace.DerivedType/DerivedContainedNavProp(345)");
            this.derivedEntry.Id.Should().Be(containedId);
        }

        [Fact]
        public void WritingInFullMetadataModeWithContainedEntityInDerivedEntityInFeed()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataFeed(), 
                this.derivedEntry,
                this.derivedContainedCollectionNavLinkWithPayloadMetadata,
                new ODataFeed(), 
                this.entryWithOnlyData
            };

            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=full", true, itemsToWrite, Model, EntitySet, DerivedType, null, null, "EntitySet");

            Uri containedId = new Uri("http://example.com/EntitySet(345)/Namespace.DerivedType/DerivedContainedNavProp(123)");
            this.entryWithOnlyData.Id.Should().Be(containedId);
        }

        [Fact]
        public void WritingInFullMetadataModeWithExpandWithContainedElementShouldThrowExceptionIfODataPathIsNotSet()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataFeed(), this.entryWithOnlyData,
                this.containedNavLinkWithPayloadMetadata, this.entryWithOnlyData2,
                this.containedNavLinkWithPayloadMetadata, this.entryWithOnlyData3
            };

            const string selectClause = "ContainedNonCollectionNavProp";
            const string expandClause = "ExpandedNavLink($select=ContainedNonCollectionNavProp;$expand=ExpandedNavLink($select=ContainedNonCollectionNavProp))";


            Action test = () => this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=full", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause);
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_PathInODataUriMustBeSetWhenWritingContainedElement);
        }

        [Fact]
        public void WritingInFullMetadataModeWithExpandWithContainedElementShouldThrowExceptionIfEntryKeyIsNotSet()
        {
            var entryWithoutKey = new ODataEntry { Properties = new[] { new ODataProperty { Name = "Name", Value = "IHaveNoKey" } }, };
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataFeed(), entryWithoutKey,
                this.containedNavLinkWithPayloadMetadata, this.entryWithOnlyData2,
                this.containedNavLinkWithPayloadMetadata, this.entryWithOnlyData3
            };

            const string selectClause = "ContainedNonCollectionNavProp";
            const string expandClause = "ExpandedNavLink($select=ContainedNonCollectionNavProp;$expand=ExpandedNavLink($select=ContainedNonCollectionNavProp))";

            Action test = () => this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=full", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause);
            test.ShouldThrow<ODataException>().WithMessage(Strings.EdmValueUtils_PropertyDoesntExist("Namespace.EntityType", "ID"));
        }

        [Fact]
        public void WritingInFullMetadataModeForNavigationPropertyWithoutBindingShouldThrowODataFeedAndEntryTypeContext_MetadataOrSerializationInfoMissingException()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataFeed(), this.entryWithOnlyData,
                this.unknownCollectionNavLinkWithPayloadMetadata, new ODataFeed(), this.entryWithOnlyData2
            };

            const string selectClause = "UnknownCollectionNavProp";
            const string expandClause = "ExpandedNavLink($expand=UnknownCollectionNavProp)";

            Action test = () => this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=full", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause);
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataFeedAndEntryTypeContext_MetadataOrSerializationInfoMissing);
        }

        [Fact]
        public void WritingInFullMetadataModeForNavigationPropertyWithoutBindingShouldPassIfSerializationInfoHaveBeenSetOnTheEntry()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataFeed(), this.entryWithOnlyData,
                this.unknownCollectionNavLinkWithPayloadMetadata, new ODataFeed(), this.entryWithOnlyData2
            };

            this.entryWithOnlyData2.TypeName = EntityType.FullName();
            this.entryWithOnlyData2.MediaResource = new ODataStreamReferenceValue();
            this.entryWithOnlyData2.Properties.First(p => p.Name == "ID").SetSerializationInfo(new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key });
            this.entryWithOnlyData2.Properties.First(p => p.Name == "Name").SetSerializationInfo(new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.ETag });
            this.entryWithOnlyData2.SerializationInfo = new ODataFeedAndEntrySerializationInfo()
            {
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                ExpectedTypeName = EntityType.FullName(),
                NavigationSourceEntityTypeName = EntitySet.EntityType().FullName(),
                IsFromCollection = true,
                NavigationSourceName = EntitySet.Name
            };

            const string selectClause = "UnknownCollectionNavProp";
            const string expandClause = "ExpandedNavLink($expand=UnknownCollectionNavProp)";

            this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=full", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause);
            entryWithOnlyData2.Id.Should().Be("http://example.com/EntitySet(234)");
        }

        [Fact]
        public void ReadingInMinialMetadataModeWithContainedElementShouldBeAbleToGenerateId()
        {
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://example.com/$metadata#EntitySet(ContainedNavProp2,ExpandedNavLink,ExpandedNavLink(ContainedNavProp2))\"," +
                    "\"value\":[" +
                    "{" +
                        "\"ContainedNonCollectionNavProp@odata.context\":\"http://example.com/$metadata#EntitySet(123)/ContainedNonCollectionNavProp/$entity\"," +
                        "\"ContainedNonCollectionNavProp\":" +
                        "{" +
                            "\"ID\": 234," +
                            "\"Name\":\"Foo\"" +
                        "}," +
                    "\"ID\" : 123," +
                    "\"Name\" : \"Bob\"" +
                "} ] }";

            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", "application/json;odata.metadata=mini");
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            ODataEntry topLevelEntry = null;
            List<ODataEntry> entryList = new List<ODataEntry>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var reader = messageReader.CreateODataFeedReader(EntitySet, EntityType);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.EntryEnd:
                            topLevelEntry = (ODataEntry)reader.Item;
                            entryList.Add(topLevelEntry);
                            break;
                    }
                }
            }

            Uri containedId = new Uri("http://example.com/EntitySet(123)/ContainedNonCollectionNavProp");

            ODataEntry containedEntry = entryList[0];
            containedEntry.Id.Should().Be(containedId);
        }

        [Fact]
        public void ReadingInMinialMetadataModeWithContainedEntitySetShouldBeAbleToGenerateId()
        {
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://example.com/$metadata#EntitySet(1)/ContainedNavProp(ID)\"," +
                    "\"value\":[" +
                    "{" +
                    "\"ID\" : 123" +
                    "}" +
                "] }";

            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            ODataEntry topLevelEntry = null;
            List<ODataEntry> entryList = new List<ODataEntry>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var navProp = EntityType.FindProperty("ContainedNavProp") as IEdmNavigationProperty;
                var containedEntitySet = EntitySet.FindNavigationTarget(navProp) as IEdmEntitySetBase;
                var reader = messageReader.CreateODataFeedReader(containedEntitySet, EntityType);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.EntryEnd:
                            topLevelEntry = (ODataEntry)reader.Item;
                            entryList.Add(topLevelEntry);
                            break;
                    }
                }
            }

            Uri containedId = new Uri("http://example.com/EntitySet(1)/ContainedNavProp(123)");

            ODataEntry containedEntry = entryList[0];
            containedEntry.Id.Should().Be(containedId);
        }

        [Fact]
        public void ReadingInMinialMetadataModeWithContainedEntitySetOfAnotherTypeShouldBeAbleToGenerateId()
        {
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://example.com/$metadata#EntitySet(1)/AnotherContainedNavProp\"," +
                    "\"value\":[" +
                    "{" +
                    "\"ID\" : 123," +
                    "\"Name\" : \"Bob\"" +
                    "}" +
                "] }";

            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            ODataEntry topLevelEntry = null;
            List<ODataEntry> entryList = new List<ODataEntry>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var navProp = EntityType.FindProperty("AnotherContainedNavProp") as IEdmNavigationProperty;
                var containedEntitySet = EntitySet.FindNavigationTarget(navProp) as IEdmEntitySetBase;
                var reader = messageReader.CreateODataFeedReader(containedEntitySet, AnotherEntityType);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.EntryEnd:
                            topLevelEntry = (ODataEntry)reader.Item;
                            entryList.Add(topLevelEntry);
                            break;
                    }
                }
            }

            Uri containedId = new Uri("http://example.com/EntitySet(1)/AnotherContainedNavProp(123)");

            ODataEntry containedEntry = entryList[0];
            containedEntry.Id.Should().Be(containedId);
        }

        [Fact]
        public void ReadingInMinialMetadataModeUseDefaultCtorWithContainedEntitySetOfAnotherTypeShouldBeAbleToGenerateId()
        {
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://example.com/$metadata#EntitySet(1)/AnotherContainedNavProp\"," +
                    "\"value\":[" +
                    "{" +
                    "\"ID\" : 123," +
                    "\"Name\" : \"Bob\"" +
                    "}" +
                "] }";

            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            ODataEntry topLevelEntry = null;
            List<ODataEntry> entryList = new List<ODataEntry>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var reader = messageReader.CreateODataFeedReader();
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.EntryEnd:
                            topLevelEntry = (ODataEntry)reader.Item;
                            entryList.Add(topLevelEntry);
                            break;
                    }
                }
            }

            Uri containedId = new Uri("http://example.com/EntitySet(1)/AnotherContainedNavProp(123)");

            ODataEntry containedEntry = entryList[0];
            containedEntry.Id.Should().Be(containedId);
        }

        [Fact]
        public void ReadingInMinialMetadataModeWithContainedNonCollectionEntitySetShouldBeAbleToGenerateId()
        {
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://example.com/$metadata#EntitySet(1)/ContainedNonCollectionNavProp/$entity\"," +
                    "\"ID\" : 123," +
                    "\"Name\" : \"Bob\"" +
                "}";

            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            ODataEntry topLevelEntry = null;
            List<ODataEntry> entryList = new List<ODataEntry>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var navProp = EntityType.FindProperty("ContainedNonCollectionNavProp") as IEdmNavigationProperty;
                var containedEntitySet = EntitySet.FindNavigationTarget(navProp) as IEdmEntitySetBase;
                var reader = messageReader.CreateODataEntryReader(containedEntitySet, EntityType);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.EntryEnd:
                            topLevelEntry = (ODataEntry)reader.Item;
                            entryList.Add(topLevelEntry);
                            break;
                    }
                }
            }

            Uri containedId = new Uri("http://example.com/EntitySet(1)/ContainedNonCollectionNavProp");

            ODataEntry containedEntry = entryList[0];
            containedEntry.Id.Should().Be(containedId);
        }

        [Fact]
        public void ReadingInMinialMetadataModeWithContainedNonCollectionEntitySetOfAnotherTypeShouldBeAbleToGenerateId()
        {
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://example.com/$metadata#EntitySet(1)/AnotherContainedNonCollectionNavProp/$entity\"," +
                    "\"ID\" : 123," +
                    "\"Name\" : \"Bob\"" +
                "}";

            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            ODataEntry topLevelEntry = null;
            List<ODataEntry> entryList = new List<ODataEntry>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var navProp = EntityType.FindProperty("AnotherContainedNonCollectionNavProp") as IEdmNavigationProperty;
                var containedEntitySet = EntitySet.FindNavigationTarget(navProp) as IEdmEntitySetBase;
                var reader = messageReader.CreateODataEntryReader(containedEntitySet, AnotherEntityType);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.EntryEnd:
                            topLevelEntry = (ODataEntry)reader.Item;
                            entryList.Add(topLevelEntry);
                            break;
                    }
                }
            }

            Uri containedId = new Uri("http://example.com/EntitySet(1)/AnotherContainedNonCollectionNavProp");

            ODataEntry containedEntry = entryList[0];
            containedEntry.Id.Should().Be(containedId);
        }

        [Fact]
        public void ReadingInMinialMetadataModeWithTwoLevelContainedEntitySetShouldBeAbleToGenerateId()
        {
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://example.com/$metadata#EntitySet(1)/ContainedNavProp(2)/ContainedNavProp\"," +
                    "\"value\":[" +
                    "{" +
                    "\"ID\" : 123," +
                    "\"Name\" : \"Bob\"" +
                    "}" +
                "] }";

            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            ODataEntry topLevelEntry = null;
            List<ODataEntry> entryList = new List<ODataEntry>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var navProp = EntityType.FindProperty("ContainedNavProp") as IEdmNavigationProperty;
                var containedEntitySet = EntitySet.FindNavigationTarget(navProp) as IEdmEntitySetBase;
                containedEntitySet = containedEntitySet.FindNavigationTarget(navProp) as IEdmEntitySetBase;
                var reader = messageReader.CreateODataFeedReader(containedEntitySet, EntityType);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.EntryEnd:
                            topLevelEntry = (ODataEntry)reader.Item;
                            entryList.Add(topLevelEntry);
                            break;
                    }
                }
            }

            Uri containedId = new Uri("http://example.com/EntitySet(1)/ContainedNavProp(2)/ContainedNavProp(123)");

            ODataEntry containedEntry = entryList[0];
            containedEntry.Id.Should().Be(containedId);
        }

        [Fact]
        public void ReadingInMinialMetadataModeWithTwoLevelContainedEntitySetOfAnotherTypeShouldBeAbleToGenerateId()
        {
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://example.com/$metadata#EntitySet(1)/ContainedNavProp(2)/AnotherContainedNavProp\"," +
                    "\"value\":[" +
                    "{" +
                    "\"ID\" : 123," +
                    "\"Name\" : \"Bob\"" +
                    "}" +
                "] }";

            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            ODataEntry topLevelEntry = null;
            List<ODataEntry> entryList = new List<ODataEntry>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var navProp = EntityType.FindProperty("ContainedNavProp") as IEdmNavigationProperty;
                var containedEntitySet = EntitySet.FindNavigationTarget(navProp) as IEdmEntitySetBase;
                var anotherNavProp = EntityType.FindProperty("AnotherContainedNavProp") as IEdmNavigationProperty;
                containedEntitySet = containedEntitySet.FindNavigationTarget(anotherNavProp) as IEdmEntitySetBase;
                var reader = messageReader.CreateODataFeedReader(containedEntitySet, AnotherEntityType);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.EntryEnd:
                            topLevelEntry = (ODataEntry)reader.Item;
                            entryList.Add(topLevelEntry);
                            break;
                    }
                }
            }

            Uri containedId = new Uri("http://example.com/EntitySet(1)/ContainedNavProp(2)/AnotherContainedNavProp(123)");

            ODataEntry containedEntry = entryList[0];
            containedEntry.Id.Should().Be(containedId);
        }

        [Fact]
        public void ReadingInMinialMetadataModeWithContainedEntitySetAndTypeCastShouldBeAbleToGenerateId()
        {
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://example.com/$metadata#EntitySet(1)/ContainedNavProp/Namespace.EntityType\"," +
                    "\"value\":[" +
                    "{" +
                    "\"ID\" : 123," +
                    "\"Name\" : \"Bob\"" +
                    "}" +
                "] }";

            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            ODataEntry topLevelEntry = null;
            List<ODataEntry> entryList = new List<ODataEntry>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var navProp = EntityType.FindProperty("ContainedNavProp") as IEdmNavigationProperty;
                var containedEntitySet = EntitySet.FindNavigationTarget(navProp) as IEdmEntitySetBase;
                var reader = messageReader.CreateODataFeedReader(containedEntitySet, EntityType);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.EntryEnd:
                            topLevelEntry = (ODataEntry)reader.Item;
                            entryList.Add(topLevelEntry);
                            break;
                    }
                }
            }

            Uri containedId = new Uri("http://example.com/EntitySet(1)/ContainedNavProp(123)");

            ODataEntry containedEntry = entryList[0];
            containedEntry.Id.Should().Be(containedId);
        }

        [Fact]
        public void ReadingInMinialMetadataModeWithNonCollectionContainedEntitySetAndTypeCastShouldBeAbleToGenerateId()
        {
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://example.com/$metadata#EntitySet(1)/ContainedNonCollectionNavProp/Namespace.EntityType(ID)/$entity\"," +
                    "\"ID\" : 123" +
                "}";

            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            ODataEntry topLevelEntry = null;
            List<ODataEntry> entryList = new List<ODataEntry>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var navProp = EntityType.FindProperty("ContainedNonCollectionNavProp") as IEdmNavigationProperty;
                var containedEntitySet = EntitySet.FindNavigationTarget(navProp) as IEdmEntitySetBase;
                var reader = messageReader.CreateODataEntryReader(containedEntitySet, EntityType);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.EntryEnd:
                            topLevelEntry = (ODataEntry)reader.Item;
                            entryList.Add(topLevelEntry);
                            break;
                    }
                }
            }

            Uri containedId = new Uri("http://example.com/EntitySet(1)/ContainedNonCollectionNavProp");

            ODataEntry containedEntry = entryList[0];
            containedEntry.Id.Should().Be(containedId);
        }

        [Fact]
        public void ReadingInMinialMetadataModeWithExpandedCollectionContainedEntitySetAndTypeCastShouldBeAbleToGenerateId()
        {
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://example.com/$metadata#EntitySet(ContainedNavProp/Namespace.EntityType)/$entity\"," +
                    "\"ID\" : 123," +
                    "\"Name\" : \"Bob\"," +
                    "\"ContainedNavProp@odata.context\":\"http://example.com/$metadata#EntitySet(123)/ContainedNavProp/Namespace.EntityType\"," +
                    "\"ContainedNavProp\":" +
                        "[{" +
                            "\"ID\" : 234," +
                            "\"Name\" : \"Foo\"" +
                        "}]" +
                "}";

            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            ODataEntry topLevelEntry = null;
            List<ODataEntry> entryList = new List<ODataEntry>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var reader = messageReader.CreateODataEntryReader(EntitySet, EntityType);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.EntryEnd:
                            topLevelEntry = (ODataEntry)reader.Item;
                            entryList.Add(topLevelEntry);
                            break;
                    }
                }
            }

            Uri containedId = new Uri("http://example.com/EntitySet(123)/ContainedNavProp(234)");

            ODataEntry containedEntry = entryList[0];
            containedEntry.Id.Should().Be(containedId);
        }

        [Fact]
        public void ReadingInMinialMetadataModeWithExpandedNonCollectionContainedEntitySetAndTypeCastShouldBeAbleToGenerateId()
        {
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://example.com/$metadata#EntitySet(ContainedNonCollectionNavProp/Namespace.EntityType)/$entity\"," +
                    "\"ID\" : 123," +
                    "\"Name\" : \"Bob\"," +
                    "\"ContainedNonCollectionNavProp@odata.context\":\"http://example.com/$metadata#EntitySet(123)/ContainedNonCollectionNavProp/Namespace.EntityType/$entity\"," +
                    "\"ContainedNonCollectionNavProp\":" +
                        "{" +
                            "\"ID\" : 234," +
                            "\"Name\" : \"Foo\"" +
                        "}" +
                "}";

            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            ODataEntry topLevelEntry = null;
            List<ODataEntry> entryList = new List<ODataEntry>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var reader = messageReader.CreateODataEntryReader(EntitySet, EntityType);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.EntryEnd:
                            topLevelEntry = (ODataEntry)reader.Item;
                            entryList.Add(topLevelEntry);
                            break;
                    }
                }
            }

            Uri containedId = new Uri("http://example.com/EntitySet(123)/ContainedNonCollectionNavProp");

            ODataEntry containedEntry = entryList[0];
            containedEntry.Id.Should().Be(containedId);
        }

        [Fact]
        public void WritingInFullMetadataModeWithExpandAndProjectionWithMissingStreamAndActionAndFunctionWhenAutoComputePayloadMetadataInJsonIsTrue()
        {
            const string expectedPayload =
                "{" +
                    "\"@odata.context\":\"http://example.com/$metadata#EntitySet(StreamProp1,Namespace.AlwaysBindableAction1,Namespace.AlwaysBindableFunction1,DeferredNavLink,ExpandedNavLink,ExpandedNavLink(StreamProp1,Namespace.AlwaysBindableAction1,ExpandedNavLink,ExpandedNavLink(StreamProp2,Namespace.AlwaysBindableAction1)))\"," +
                    "\"value\":[" +
                    "{" +
                        "\"@odata.type\":\"#Namespace.EntityType\"," +
                        "\"@odata.id\":\"EntitySet(123)\"," +
                        "\"@odata.etag\":\"W/\\\"'Bob'\\\"\"," +
                        "\"@odata.editLink\":\"EntitySet(123)\"," +
                        "\"@odata.mediaEditLink\":\"EntitySet(123)/$value\"," +
                        "\"ID\":123," +
                        "\"Name\":\"Bob\"," +
                        "\"ExpandedNavLink@odata.associationLink\":\"http://example.com/expanded/association\"," +
                        "\"ExpandedNavLink@odata.navigationLink\":\"http://example.com/expanded/navigation\"," +
                        "\"ExpandedNavLink\":[" +
                        "{" +
                            "\"@odata.type\":\"#Namespace.EntityType\"," +
                            "\"@odata.id\":\"EntitySet(234)\"," +
                            "\"@odata.etag\":\"W/\\\"'Foo'\\\"\"," +
                            "\"@odata.editLink\":\"EntitySet(234)\"," +
                            "\"@odata.mediaEditLink\":\"EntitySet(234)/$value\"," +
                            "\"ID\":234," +
                            "\"Name\":\"Foo\"," +
                            "\"DeferredNavLink@odata.associationLink\":\"http://example.com/EntitySet(234)/DeferredNavLink/$ref\"," +
                            "\"DeferredNavLink@odata.navigationLink\":\"http://example.com/EntitySet(234)/DeferredNavLink\"," +
                            "\"ExpandedNavLink@odata.associationLink\":\"http://example.com/EntitySet(234)/ExpandedNavLink/$ref\"," +
                            "\"ExpandedNavLink@odata.navigationLink\":\"http://example.com/EntitySet(234)/ExpandedNavLink\"," +
                            "\"ExpandedNavLink\":[" +
                            "{" +
                                "\"@odata.type\":\"#Namespace.EntityType\"," +
                                "\"@odata.id\":\"EntitySet(345)\"," +
                                "\"@odata.etag\":\"W/\\\"'Bar'\\\"\"," +
                                "\"@odata.editLink\":\"EntitySet(345)\"," +
                                "\"@odata.mediaEditLink\":\"EntitySet(345)/$value\"," +
                                "\"ID\":345," +
                                "\"Name\":\"Bar\"" +
                            "}]" +
                        "}]" +
                    "}]" +
                "}";

            ODataFeedAndEntrySerializationInfo serializationInfo = new ODataFeedAndEntrySerializationInfo { NavigationSourceName = EntitySet.Name, NavigationSourceEntityTypeName = EntityType.FullName(), ExpectedTypeName = EntityType.FullName() };

            var feed = new ODataFeed();
            feed.SetSerializationInfo(serializationInfo);
            this.entryWithOnlyData.TypeName = EntityType.FullName();
            this.entryWithOnlyData.MediaResource = new ODataStreamReferenceValue();
            this.entryWithOnlyData.Properties.First(p => p.Name == "ID").SetSerializationInfo(new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key });
            this.entryWithOnlyData.Properties.First(p => p.Name == "Name").SetSerializationInfo(new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.ETag });
            this.entryWithOnlyData2.TypeName = EntityType.FullName();
            this.entryWithOnlyData2.MediaResource = new ODataStreamReferenceValue();
            this.entryWithOnlyData2.Properties.First(p => p.Name == "ID").SetSerializationInfo(new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key });
            this.entryWithOnlyData2.Properties.First(p => p.Name == "Name").SetSerializationInfo(new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.ETag });
            this.entryWithOnlyData3.TypeName = EntityType.FullName();
            this.entryWithOnlyData3.MediaResource = new ODataStreamReferenceValue();
            this.entryWithOnlyData3.Properties.First(p => p.Name == "ID").SetSerializationInfo(new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key });
            this.entryWithOnlyData3.Properties.First(p => p.Name == "Name").SetSerializationInfo(new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.ETag });

            ODataItem[] itemsToWrite = new ODataItem[]
            {
                feed, this.entryWithOnlyData,
                this.expandedNavLinkWithPayloadMetadata, feed, this.entryWithOnlyData2, this.navLinkWithoutPayloadMetadata,
                this.expandedNavLinkWithoutPayloadMetadata, feed, this.entryWithOnlyData3
            };

            const string selectClause = "StreamProp1,Namespace.AlwaysBindableAction1,Namespace.AlwaysBindableFunction1,DeferredNavLink";
            const string expandClause = "ExpandedNavLink($select=StreamProp1,Namespace.AlwaysBindableAction1;$expand=ExpandedNavLink($select=StreamProp2,Namespace.AlwaysBindableAction1))";
            this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=full", true, itemsToWrite, edmModel: Model, edmEntitySet: null, edmEntityType: EntityType, selectClause: selectClause, expandClause: expandClause)
                .Should().Be(expectedPayload);
        }

        [Fact]
        public void WritingInFullMetadataModeWithExpandAndProjectionWithModelWhenAutoComputePayloadMetadataInJsonIsTrue()
        {
            const string expectedPayload =
                "{" +
                    "\"@odata.context\":\"http://example.com/$metadata#EntitySet(StreamProp1,Namespace.AlwaysBindableAction1,Namespace.AlwaysBindableFunction1,DeferredNavLink,ExpandedNavLink,ExpandedNavLink(StreamProp1,Namespace.AlwaysBindableAction1,ExpandedNavLink,ExpandedNavLink(StreamProp2,Namespace.AlwaysBindableAction1)))\"," +
                    "\"value\":[" +
                    "{" +
                        "\"@odata.type\":\"#Namespace.EntityType\"," +
                        "\"@odata.id\":\"EntitySet(123)\"," +
                        "\"@odata.etag\":\"W/\\\"'Bob'\\\"\"," +
                        "\"@odata.editLink\":\"EntitySet(123)\"," +
                        "\"@odata.mediaEditLink\":\"EntitySet(123)/$value\"," +
                        "\"ID\":123," +
                        "\"Name\":\"Bob\"," +
                        "\"StreamProp1@odata.mediaEditLink\":\"http://example.com/EntitySet(123)/StreamProp1\"," +
                        "\"StreamProp1@odata.mediaReadLink\":\"http://example.com/EntitySet(123)/StreamProp1\"," +
                        "\"ExpandedNavLink@odata.associationLink\":\"http://example.com/expanded/association\"," +
                        "\"ExpandedNavLink@odata.navigationLink\":\"http://example.com/expanded/navigation\"," +
                        "\"ExpandedNavLink\":[" +
                        "{" +
                            "\"@odata.type\":\"#Namespace.EntityType\"," +
                            "\"@odata.id\":\"EntitySet(234)\"," +
                            "\"@odata.etag\":\"W/\\\"'Foo'\\\"\"," +
                            "\"@odata.editLink\":\"EntitySet(234)\"," +
                            "\"@odata.mediaEditLink\":\"EntitySet(234)/$value\"," +
                            "\"ID\":234," +
                            "\"Name\":\"Foo\"," +
                            "\"StreamProp1@odata.mediaEditLink\":\"http://example.com/EntitySet(234)/StreamProp1\"," +
                            "\"StreamProp1@odata.mediaReadLink\":\"http://example.com/EntitySet(234)/StreamProp1\"," +
                            "\"DeferredNavLink@odata.associationLink\":\"http://example.com/EntitySet(234)/DeferredNavLink/$ref\"," +
                            "\"DeferredNavLink@odata.navigationLink\":\"http://example.com/EntitySet(234)/DeferredNavLink\"," +
                            "\"ExpandedNavLink@odata.associationLink\":\"http://example.com/expanded/association\"," +
                            "\"ExpandedNavLink@odata.navigationLink\":\"http://example.com/expanded/navigation\"," +
                            "\"ExpandedNavLink\":[" +
                            "{" +
                                "\"@odata.type\":\"#Namespace.EntityType\"," +
                                "\"@odata.id\":\"EntitySet(345)\"," +
                                "\"@odata.etag\":\"W/\\\"'Bar'\\\"\"," +
                                "\"@odata.editLink\":\"EntitySet(345)\"," +
                                "\"@odata.mediaEditLink\":\"EntitySet(345)/$value\"," +
                                "\"ID\":345," +
                                "\"Name\":\"Bar\"," +
                                "\"StreamProp2@odata.mediaEditLink\":\"http://example.com/EntitySet(345)/StreamProp2\"," +
                                "\"StreamProp2@odata.mediaReadLink\":\"http://example.com/EntitySet(345)/StreamProp2\"," +
                                "\"#Container.AlwaysBindableAction1\":{\"title\":\"Container.AlwaysBindableAction1\",\"target\":\"http://example.com/EntitySet(345)/Container.AlwaysBindableAction1\"}" +
                            "}]," +
                            "\"#Container.AlwaysBindableAction1\":{\"title\":\"Container.AlwaysBindableAction1\",\"target\":\"http://example.com/EntitySet(234)/Container.AlwaysBindableAction1\"}" +
                        "}]," +
                        "\"#Container.AlwaysBindableAction1\":{\"title\":\"Container.AlwaysBindableAction1\",\"target\":\"http://example.com/EntitySet(123)/Container.AlwaysBindableAction1\"}," +
                        "\"#Container.AlwaysBindableFunction1\":{\"title\":\"Container.AlwaysBindableFunction1\",\"target\":\"http://example.com/EntitySet(123)/Container.AlwaysBindableFunction1\"}" +
                    "}]" +
                "}";

            ODataFeedAndEntrySerializationInfo serializationInfo = new ODataFeedAndEntrySerializationInfo { NavigationSourceName = EntitySet.Name, NavigationSourceEntityTypeName = EntityType.FullName(), ExpectedTypeName = EntityType.FullName() };

            var feed = new ODataFeed();
            feed.SetSerializationInfo(serializationInfo);

            this.entryWithOnlyData = new ODataEntry
            {
                TypeName = EntityType.FullName(),
                MediaResource = new ODataStreamReferenceValue(),
                Properties = new[]
                {
                    new ODataProperty { Name = "ID", Value = 123, SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }},
                    new ODataProperty { Name = "Name", Value = "Bob", SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.ETag }},
                    new ODataProperty { Name = "StreamProp1", Value = new ODataStreamReferenceValue() }
                },
            };
            this.entryWithOnlyData.AddAction(new ODataAction { Metadata = new Uri("http://example.com/$metadata#Container.AlwaysBindableAction1") });
            this.entryWithOnlyData.AddFunction(new ODataFunction { Metadata = new Uri("#Container.AlwaysBindableFunction1", UriKind.Relative) });

            this.entryWithOnlyData2 = new ODataEntry
            {
                TypeName = EntityType.FullName(),
                MediaResource = new ODataStreamReferenceValue(),
                Properties = new[]
                {
                    new ODataProperty { Name = "ID", Value = 234, SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }},
                    new ODataProperty { Name = "Name", Value = "Foo", SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.ETag }},
                    new ODataProperty { Name = "StreamProp1", Value = new ODataStreamReferenceValue() }
                },
            };
            this.entryWithOnlyData2.AddAction(new ODataAction { Metadata = new Uri("http://example.com/$metadata#Container.AlwaysBindableAction1") });

            this.entryWithOnlyData3 = new ODataEntry
            {
                TypeName = EntityType.FullName(),
                MediaResource = new ODataStreamReferenceValue(),
                Properties = new[]
                {
                    new ODataProperty { Name = "ID", Value = 345, SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }},
                    new ODataProperty { Name = "Name", Value = "Bar", SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.ETag }},
                    new ODataProperty { Name = "StreamProp2", Value = new ODataStreamReferenceValue() }
                },
            };
            this.entryWithOnlyData3.AddAction(new ODataAction { Metadata = new Uri("http://example.com/$metadata#Container.AlwaysBindableAction1") });

            ODataItem[] itemsToWrite = new ODataItem[]
            {
                feed, this.entryWithOnlyData,
                this.expandedNavLinkWithPayloadMetadata, feed, this.entryWithOnlyData2, this.navLinkWithoutPayloadMetadata,
                this.expandedNavLinkWithPayloadMetadata, feed, this.entryWithOnlyData3
            };

            const string selectClause = "StreamProp1,Namespace.AlwaysBindableAction1,Namespace.AlwaysBindableFunction1,DeferredNavLink";
            const string expandClause = "ExpandedNavLink($select=StreamProp1,Namespace.AlwaysBindableAction1;$expand=ExpandedNavLink($select=StreamProp2,Namespace.AlwaysBindableAction1))";
            this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=full", true, itemsToWrite, edmModel: Model, edmEntitySet: null, edmEntityType: EntityType, selectClause: selectClause, expandClause: expandClause)
                .Should().Be(expectedPayload);
        }

        [Fact]
        public void WritingInMinimalMetadataModeWithExpandAndProjectionWithModelWhenAutoComputePayloadMetadataInJsonIsTrue()
        {
            const string expectedPayload =
                "{" +
                    "\"@odata.context\":\"http://example.com/$metadata#EntitySet(StreamProp1,Namespace.AlwaysBindableAction1,Namespace.AlwaysBindableFunction1,DeferredNavLink,ExpandedNavLink,ExpandedNavLink(StreamProp1,Namespace.AlwaysBindableAction1,ExpandedNavLink,ExpandedNavLink(StreamProp2,Namespace.AlwaysBindableAction1)))\"," +
                    "\"value\":[" +
                    "{" +
                        "\"ID\":123," +
                        "\"Name\":\"Bob\"," +
                        "\"ExpandedNavLink@odata.associationLink\":\"http://example.com/expanded/association\"," +
                        "\"ExpandedNavLink@odata.navigationLink\":\"http://example.com/expanded/navigation\"," +
                        "\"ExpandedNavLink\":[" +
                        "{" +
                            "\"ID\":234," +
                            "\"Name\":\"Foo\"," +
                            "\"ExpandedNavLink\":[" +
                            "{" +
                                "\"ID\":345," +
                                "\"Name\":\"Bar\"," +
                                "\"#Container.AlwaysBindableAction1\":{}" +
                            "}]," +
                            "\"#Container.AlwaysBindableAction1\":{}" +
                        "}]," +
                        "\"#Container.AlwaysBindableAction1\":{}," +
                        "\"#Container.AlwaysBindableFunction1\":{}" +
                    "}]" +
                "}";

            ODataFeedAndEntrySerializationInfo serializationInfo = new ODataFeedAndEntrySerializationInfo { NavigationSourceName = EntitySet.Name, NavigationSourceEntityTypeName = EntityType.FullName(), ExpectedTypeName = EntityType.FullName() };

            var feed = new ODataFeed();
            feed.SetSerializationInfo(serializationInfo);

            this.entryWithOnlyData = new ODataEntry
            {
                TypeName = EntityType.FullName(),
                MediaResource = new ODataStreamReferenceValue(),
                Properties = new[]
                {
                    new ODataProperty { Name = "ID", Value = 123, SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }},
                    new ODataProperty { Name = "Name", Value = "Bob", SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.ETag }},
                    new ODataProperty { Name = "StreamProp1", Value = new ODataStreamReferenceValue() }
                },
            };
            this.entryWithOnlyData.AddAction(new ODataAction { Metadata = new Uri("http://example.com/$metadata#Container.AlwaysBindableAction1") });
            this.entryWithOnlyData.AddFunction(new ODataFunction { Metadata = new Uri("#Container.AlwaysBindableFunction1", UriKind.Relative) });

            this.entryWithOnlyData2 = new ODataEntry
            {
                TypeName = EntityType.FullName(),
                MediaResource = new ODataStreamReferenceValue(),
                Properties = new[]
                {
                    new ODataProperty { Name = "ID", Value = 234, SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }},
                    new ODataProperty { Name = "Name", Value = "Foo", SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.ETag }},
                    new ODataProperty { Name = "StreamProp1", Value = new ODataStreamReferenceValue() }
                },
            };
            this.entryWithOnlyData2.AddAction(new ODataAction { Metadata = new Uri("http://example.com/$metadata#Container.AlwaysBindableAction1") });

            this.entryWithOnlyData3 = new ODataEntry
            {
                TypeName = EntityType.FullName(),
                MediaResource = new ODataStreamReferenceValue(),
                Properties = new[]
                {
                    new ODataProperty { Name = "ID", Value = 345, SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }},
                    new ODataProperty { Name = "Name", Value = "Bar", SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.ETag }},
                    new ODataProperty { Name = "StreamProp2", Value = new ODataStreamReferenceValue() }
                },
            };
            this.entryWithOnlyData3.AddAction(new ODataAction { Metadata = new Uri("http://example.com/$metadata#Container.AlwaysBindableAction1") });

            ODataItem[] itemsToWrite = new ODataItem[]
            {
                feed, this.entryWithOnlyData,
                this.expandedNavLinkWithPayloadMetadata, feed, this.entryWithOnlyData2, this.navLinkWithoutPayloadMetadata,
                this.expandedNavLinkWithoutPayloadMetadata, feed, this.entryWithOnlyData3
            };

            const string selectClause = "StreamProp1,Namespace.AlwaysBindableAction1,Namespace.AlwaysBindableFunction1,DeferredNavLink";
            const string expandClause = "ExpandedNavLink($select=StreamProp1,Namespace.AlwaysBindableAction1;$expand=ExpandedNavLink($select=StreamProp2,Namespace.AlwaysBindableAction1))";
            this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, edmModel: Model, edmEntitySet: null, edmEntityType: EntityType, selectClause: selectClause, expandClause: expandClause)
                .Should().Be(expectedPayload);
        }

        private string GetWriterOutputForEntryWithPayloadMetadata(
            string contentType,
            bool autoComputePayloadMetadataInJson,
            string selectClause = null,
            bool odataSimplified = false)
        {
            ODataItem[] itemsToWrite = new ODataItem[] { this.entryWithPayloadMetadata, this.navLinkWithPayloadMetadata, this.expandedNavLinkWithPayloadMetadata, new ODataFeed() };
            return this.GetWriterOutputForContentTypeAndKnobValue(contentType, autoComputePayloadMetadataInJson, itemsToWrite, Model, EntitySet, EntityType, selectClause, odataSimplified: odataSimplified);
        }

        private string GetWriterOutputForEntryWithOnlyData(
            string contentType,
            bool autoComputePayloadMetadataInJson,
            string selectClause = null)
        {
            ODataItem[] itemsToWrite = new ODataItem[] { this.entryWithOnlyData, this.navLinkWithoutPayloadMetadata, this.expandedNavLinkWithoutPayloadMetadata, new ODataFeed() };
            return this.GetWriterOutputForContentTypeAndKnobValue(contentType, autoComputePayloadMetadataInJson, itemsToWrite, Model, EntitySet, EntityType, selectClause);
        }

        private string GetWriterOutputForContentTypeAndKnobValue(string contentType, bool autoComputePayloadMetadataInJson, ODataItem[] itemsToWrite, EdmModel edmModel, IEdmEntitySetBase edmEntitySet, EdmEntityType edmEntityType, string selectClause = null, string expandClause = null, string resourcePath = null, bool odataSimplified = false)
        {
            MemoryStream outputStream = new MemoryStream();
            IODataResponseMessage message = new InMemoryMessage() { Stream = outputStream };
            message.SetHeader("Content-Type", contentType);
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings()
            {
                AutoComputePayloadMetadataInJson = autoComputePayloadMetadataInJson,
                ODataSimplified = odataSimplified
            };

            var result = new ODataQueryOptionParser(edmModel, edmEntityType, edmEntitySet, new Dictionary<string, string> { { "$select", selectClause }, { "$expand", expandClause } }).ParseSelectAndExpand();

            ODataUri odataUri = new ODataUri()
            {
                ServiceRoot = new Uri("http://example.com"),
                SelectAndExpand = result
            };

            if (resourcePath != null)
            {
                Uri requestUri = new Uri("http://example.com/" + resourcePath);
                odataUri.RequestUri = requestUri;
                odataUri.Path = new ODataUriParser(edmModel, new Uri("http://example.com"), requestUri).ParsePath();
            }

            settings.ODataUri = odataUri;

            string output;
            using (var messageWriter = new ODataMessageWriter(message, settings, edmModel))
            {
                int currentIdx = 0;

                if (itemsToWrite[currentIdx] is ODataFeed)
                {
                    ODataWriter writer = messageWriter.CreateODataFeedWriter(edmEntitySet, edmEntityType);
                    this.WriteFeed(writer, itemsToWrite, ref currentIdx);
                }
                else if (itemsToWrite[currentIdx] is ODataEntry)
                {
                    ODataWriter writer = messageWriter.CreateODataEntryWriter(edmEntitySet, edmEntityType);
                    this.WriteEntry(writer, itemsToWrite, ref currentIdx);
                }
                else
                {
                    Assert.True(false, "Top level item to write must be entry or feed.");
                }

                currentIdx.Should().Be(itemsToWrite.Length, "Invalid list of items to write.");

                outputStream.Seek(0, SeekOrigin.Begin);
                output = new StreamReader(outputStream).ReadToEnd();
            }

            return output;
        }

        private void WriteFeed(ODataWriter writer, ODataItem[] itemsToWrite, ref int currentIdx)
        {
            if (currentIdx < itemsToWrite.Length)
            {
                ODataFeed feed = (ODataFeed)itemsToWrite[currentIdx++];
                writer.WriteStart(feed);
                while (currentIdx < itemsToWrite.Length && itemsToWrite[currentIdx] is ODataEntry)
                {
                    this.WriteEntry(writer, itemsToWrite, ref currentIdx);
                }

                writer.WriteEnd();
            }
        }

        private void WriteEntry(ODataWriter writer, ODataItem[] itemsToWrite, ref int currentIdx)
        {
            if (currentIdx < itemsToWrite.Length)
            {
                ODataEntry entry = (ODataEntry)itemsToWrite[currentIdx++];
                writer.WriteStart(entry);
                while (currentIdx < itemsToWrite.Length && itemsToWrite[currentIdx] is ODataNavigationLink)
                {
                    this.WriteLink(writer, itemsToWrite, ref currentIdx);
                }

                writer.WriteEnd();
            }
        }

        private void WriteLink(ODataWriter writer, ODataItem[] itemsToWrite, ref int currentIdx)
        {
            if (currentIdx < itemsToWrite.Length)
            {
                ODataNavigationLink link = (ODataNavigationLink)itemsToWrite[currentIdx++];
                writer.WriteStart(link);
                if (currentIdx < itemsToWrite.Length)
                {
                    if (itemsToWrite[currentIdx] is ODataEntry)
                    {
                        this.WriteEntry(writer, itemsToWrite, ref currentIdx);
                    }
                    else if (itemsToWrite[currentIdx] is ODataFeed)
                    {
                        this.WriteFeed(writer, itemsToWrite, ref currentIdx);
                    }
                }

                writer.WriteEnd();
            }
        }
    }
}
