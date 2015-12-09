//---------------------------------------------------------------------
// <copyright file="FullPayloadValidateTests.cs" company="Microsoft">
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
using Xunit;
using ErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Writer.JsonLight
{
    public class FullPayloadValidateTests
    {
        #region Declaration And Initialization
        private static readonly EdmEntityType EntityType;
        private static readonly EdmEntityType DerivedType;
        private static readonly EdmEntitySet EntitySet;
        private static readonly EdmModel Model;
        private static readonly EdmModel ModelWithFunction;

        private ODataEntry entryWithOnlyData1;
        private ODataEntry entryWithOnlyData2;
        private ODataEntry entryWithOnlyData3;

        private readonly ODataNavigationLink expandedCollectionNavLink = new ODataNavigationLink()
        {
            Url = new Uri("http://example.org/odata.svc/navigation"),
            IsCollection = true,
            Name = "ExpandedCollectionNavProp",
        };

        private readonly ODataNavigationLink expandedNavLink = new ODataNavigationLink()
        {
            IsCollection = false,
            Name = "ExpandedNavProp",
        };

        private readonly ODataNavigationLink containedCollectionNavLink = new ODataNavigationLink()
        {
            Url = new Uri("http://example.org/odata.svc/navigation"),
            IsCollection = true,
            Name = "ContainedCollectionNavProp",
        };

        private readonly ODataNavigationLink containedNavLink = new ODataNavigationLink()
        {
            IsCollection = false,
            Name = "ContainedNavProp",
        };

        static FullPayloadValidateTests()
        {
            EntityType = new EdmEntityType("Namespace", "EntityType", null, false, false, false);
            EntityType.AddKeys(EntityType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
            EntityType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(isNullable: true), null, EdmConcurrencyMode.Fixed);
            DerivedType = new EdmEntityType("Namespace", "DerivedType", EntityType, false, true);

            var expandedCollectionNavProp = EntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Target = EntityType,
                TargetMultiplicity = EdmMultiplicity.Many,
                Name = "ExpandedCollectionNavProp"
            });

            var expandedNavProp = EntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Target = EntityType,
                TargetMultiplicity = EdmMultiplicity.One,
                Name = "ExpandedNavProp"
            });

            EntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Target = EntityType,
                TargetMultiplicity = EdmMultiplicity.Many,
                Name = "ContainedCollectionNavProp",
                ContainsTarget = true
            });

            EntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Target = EntityType,
                TargetMultiplicity = EdmMultiplicity.One,
                Name = "ContainedNavProp",
                ContainsTarget = true
            });

            var container = new EdmEntityContainer("Namespace", "Container");
            EntitySet = container.AddEntitySet("EntitySet", EntityType);

            EntitySet.AddNavigationTarget(expandedNavProp, EntitySet);
            EntitySet.AddNavigationTarget(expandedCollectionNavProp, EntitySet);

            Model = new EdmModel();
            Model.AddElement(EntityType);
            Model.AddElement(DerivedType);
            Model.AddElement(container);

            ModelWithFunction = new EdmModel();
            ModelWithFunction.AddElement(EntityType);
            ModelWithFunction.AddElement(DerivedType);
            ModelWithFunction.AddElement(container);

            EdmEntityTypeReference entityTypeReference = new EdmEntityTypeReference(EntityType, false);
            EdmCollectionTypeReference typeReference = new EdmCollectionTypeReference(new EdmCollectionType(entityTypeReference));
            EdmFunction function = new EdmFunction("Namespace", "Function", EdmCoreModel.Instance.GetBoolean(true), true, null, false);
            function.AddParameter("bindingParameter", typeReference);
            ModelWithFunction.AddElement(function);

            EdmAction action = new EdmAction("Namespace", "Action", EdmCoreModel.Instance.GetBoolean(true), true, null);
            action.AddParameter("bindingParameter", typeReference);
            ModelWithFunction.AddElement(action);
        }

        public FullPayloadValidateTests()
        {
            this.entryWithOnlyData1 = new ODataEntry { Properties = new[] { new ODataProperty { Name = "ID", Value = 101 }, new ODataProperty { Name = "Name", Value = "Alice" } }, };
            this.entryWithOnlyData2 = new ODataEntry { Properties = new[] { new ODataProperty { Name = "ID", Value = 102 }, new ODataProperty { Name = "Name", Value = "Bob" } }, };
            this.entryWithOnlyData3 = new ODataEntry { Properties = new[] { new ODataProperty { Name = "ID", Value = 103 }, new ODataProperty { Name = "Name", Value = "Charlie" } }, };
        }
        #endregion Declaration & Initialization

        #region Containment Tests
        [Fact]
        public void WritingTopLevelContainedFeed()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataFeed(), this.entryWithOnlyData2
            };

            IEdmNavigationProperty containedNavProp = EntityType.FindProperty("ContainedCollectionNavProp") as IEdmNavigationProperty;
            IEdmEntitySetBase contianedEntitySet = EntitySet.FindNavigationTarget(containedNavProp) as IEdmEntitySetBase;
            string resourcePath = "EntitySet(123)/ContainedCollectionNavProp";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, contianedEntitySet, EntityType, null, null, resourcePath);

            string expectedPayload = "{\"" +
                                            "@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(123)/ContainedCollectionNavProp\"," +
                                            "\"value\":[" +
                                                "{\"" +
                                                    "ID\":102,\"Name\":\"Bob\"" +
                                                "}" +
                                            "]" +
                                        "}";
            result.Should().Be(expectedPayload);
        }

        [Fact]
        public void WritingTopLevelContainedEntry()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                this.entryWithOnlyData2
            };

            IEdmNavigationProperty containedNavProp = EntityType.FindProperty("ContainedNavProp") as IEdmNavigationProperty;
            IEdmEntitySetBase contianedEntitySet = EntitySet.FindNavigationTarget(containedNavProp) as IEdmEntitySetBase;
            string resourcePath = "EntitySet(123)/ContainedNavProp";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, contianedEntitySet, EntityType, null, null, resourcePath);

            string expectedPayload = "{\"" +
                                            "@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(123)/ContainedNavProp/$entity\"," +
                                            "\"ID\":102,\"Name\":\"Bob\"" +
                                        "}";
            result.Should().Be(expectedPayload);
        }

        [Fact]
        public void WritingTopLevelContainedFeedWithTypeCast()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataFeed(), this.entryWithOnlyData2
            };

            IEdmNavigationProperty containedNavProp = EntityType.FindProperty("ContainedCollectionNavProp") as IEdmNavigationProperty;
            IEdmEntitySetBase contianedEntitySet = EntitySet.FindNavigationTarget(containedNavProp) as IEdmEntitySetBase;
            string resourcePath = "EntitySet(123)/ContainedCollectionNavProp/Namespace.DerivedType";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, contianedEntitySet, DerivedType, null, null, resourcePath);

            string expectedPayload = "{\"" +
                                            "@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(123)/ContainedCollectionNavProp/Namespace.DerivedType\"," +
                                            "\"value\":[" +
                                                "{\"" +
                                                    "ID\":102,\"Name\":\"Bob\"" +
                                                "}" +
                                            "]" +
                                        "}";
            result.Should().Be(expectedPayload);
        }

        [Fact]
        public void WritingTopLevelContainedEntryWithTypeCast()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                this.entryWithOnlyData2
            };

            IEdmNavigationProperty containedNavProp = EntityType.FindProperty("ContainedNavProp") as IEdmNavigationProperty;
            IEdmEntitySetBase contianedEntitySet = EntitySet.FindNavigationTarget(containedNavProp) as IEdmEntitySetBase;
            string resourcePath = "EntitySet(123)/ContainedNavProp/Namespace.DerivedType";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, contianedEntitySet, DerivedType, null, null, resourcePath);

            string expectedPayload = "{\"" +
                                            "@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(123)/ContainedNavProp/Namespace.DerivedType/$entity\"," +
                                            "\"ID\":102,\"Name\":\"Bob\"" +
                                        "}";
            result.Should().Be(expectedPayload);
        }

        [Fact]
        public void WritingFeedWithFunctionAndAction()
        {
            ODataFeed feed = new ODataFeed();
            feed.AddAction(new ODataAction { Metadata = new Uri("http://example.org/odata.svc/$metadata#Action"), Target = new Uri("http://example.org/odata.svc/DoAction"), Title = "ActionTitle" });
            feed.AddFunction(new ODataFunction() { Metadata = new Uri("http://example.org/odata.svc/$metadata#Function"), Target = new Uri("http://example.org/odata.svc/DoFunction"), Title = "FunctionTitle" });

            ODataItem[] itemsToWrite = new ODataItem[]
            {
                feed,
                this.entryWithOnlyData1,
            };

            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType);

            const string expectedPayload = "{\"" +
                                                "@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet\"," +
                                                 "\"#Action\":{" +
                                                        "\"title\":\"ActionTitle\"," +
                                                        "\"target\":\"http://example.org/odata.svc/DoAction\"" +
                                                    "}," +
                                                    "\"#Function\":{" +
                                                        "\"title\":\"FunctionTitle\"," +
                                                        "\"target\":\"http://example.org/odata.svc/DoFunction\"" +
                                                    "}," +
                                                "\"value\":[" +
                                                    "{" +
                                                         "\"ID\":101,\"Name\":\"Alice\"" +
                                                    "}" +
                                                    "]" +
                                            "}";
            result.Should().Be(expectedPayload);
        }

        [Fact]
        public void ReadFeedWithActionAndFunctionTest()
        {
            string payload = "{\"" +
                                                "@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet\"," +
                                                "\"#Namespace.Action\":{" +
                                                        "\"title\":\"ActionTitle\"," +
                                                        "\"target\":\"http://example.org/odata.svc/DoAction\"" +
                                                    "}," +
                                                    "\"#Namespace.Function\":{" +
                                                        "\"title\":\"FunctionTitle\"," +
                                                        "\"target\":\"http://example.org/odata.svc/DoFunction\"" +
                                                    "}," +
                                                "\"value\":[" +
                                                    "{" +
                                                         "\"ID\":101,\"Name\":\"Alice\"" +
                                                    "}" +
                                                    "]" +
                                            "}";
            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            List<ODataFeed> feedList = new List<ODataFeed>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, ModelWithFunction))
            {
                messageReader.DetectPayloadKind().Single().PayloadKind.Should().Be(ODataPayloadKind.Feed);

                var reader = messageReader.CreateODataFeedReader();
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.FeedEnd:
                            feedList.Add(reader.Item as ODataFeed);
                            break;
                    }
                }
            }

            ODataFeed feed = feedList[0];
            feed.Actions.Count().Should().Be(1);
            feed.Functions.Count().Should().Be(1);
        }

        [Fact]
        public void WritingFeedExpandWithCollectionContainedElement()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataFeed(),
                this.entryWithOnlyData1,
                this.containedCollectionNavLink,
                new ODataFeed(),
                this.entryWithOnlyData2,
                this.containedCollectionNavLink,
                new ODataFeed(),
                this.entryWithOnlyData3,
            };

            const string selectClause = "ContainedCollectionNavProp";
            const string expandClause = "ContainedCollectionNavProp($select=ContainedCollectionNavProp;$expand=ContainedCollectionNavProp)";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause, "EntitySet");

            const string expectedPayload = "{\"" +
                                                "@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(ContainedCollectionNavProp,ContainedCollectionNavProp(ContainedCollectionNavProp))\"," +
                                                "\"value\":[" +
                                                    "{" +
                                                         "\"ID\":101,\"Name\":\"Alice\"," +
                                                        "\"ContainedCollectionNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/ContainedCollectionNavProp(ContainedCollectionNavProp)\"," +
                                                        "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                                        "\"ContainedCollectionNavProp\":" +
                                                            "[" +
                                                                "{" +
                                                                    "\"ID\":102,\"Name\":\"Bob\"," +
                                                                    "\"ContainedCollectionNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/ContainedCollectionNavProp(102)/ContainedCollectionNavProp\"," +
                                                                    "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                                                    "\"ContainedCollectionNavProp\":" +
                                                                        "[" +
                                                                            "{\"ID\":103,\"Name\":\"Charlie\"}" +
                                                                        "]" +

                                                                "}" +
                                                            "]" +
                                                    "}" +
                                                 "]" +
                                            "}";
            result.Should().Be(expectedPayload);
        }

        [Fact]
        public void WritingFeedExpandWithNonCollectionContainedElement()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataFeed(), 
                this.entryWithOnlyData1,
                this.containedNavLink,
                this.entryWithOnlyData2,
                this.containedNavLink, 
                this.entryWithOnlyData3
            };

            const string selectClause = "ContainedNavProp";
            const string expandClause = "ContainedNavProp($select=ContainedNavProp;$expand=ContainedNavProp)";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause, "EntitySet");

            string expectedPayload = "{\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(ContainedNavProp,ContainedNavProp(ContainedNavProp))\"," +
                                            "\"value\":[" +
                                                "{" +
                                                    "\"ID\":101,\"Name\":\"Alice\"," +
                                                    "\"ContainedNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/ContainedNavProp(ContainedNavProp)/$entity\"," +
                                                    "\"ContainedNavProp\":" +
                                                        "{" +
                                                            "\"ID\":102,\"Name\":\"Bob\"," +
                                                            "\"ContainedNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/ContainedNavProp/ContainedNavProp/$entity\"," +
                                                            "\"ContainedNavProp\":" +
                                                            "{" +
                                                                "\"ID\":103,\"Name\":\"Charlie\"" +
                                                            "}" +
                                                        "}" +
                                                "}" +
                                            "]" +
                                        "}";
            result.Should().Be(expectedPayload);
        }

        [Fact]
        public void WritingEntryExpandWithMixedCollectionAndNonCollectionContainedElement()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                this.entryWithOnlyData1,
                this.containedCollectionNavLink,
                new ODataFeed(), 
                this.entryWithOnlyData2,
                this.containedNavLink, 
                this.entryWithOnlyData3,
            };

            const string selectClause = "ContainedCollectionNavProp";
            const string expandClause = "ContainedCollectionNavProp($select=ContainedNavProp;$expand=ContainedNavProp)";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause, "EntitySet(101)");

            string expectedPayload = "{\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(ContainedCollectionNavProp,ContainedCollectionNavProp(ContainedNavProp))/$entity\"," +
                                        "\"ID\":101,\"Name\":\"Alice\"," +
                                        "\"ContainedCollectionNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/ContainedCollectionNavProp(ContainedNavProp)\"," +
                                        "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                        "\"ContainedCollectionNavProp\":" +
                                            "[" +
                                                "{" +
                                                    "\"ID\":102,\"Name\":\"Bob\"," +
                                                    "\"ContainedNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/ContainedCollectionNavProp(102)/ContainedNavProp/$entity\"," +
                                                    "\"ContainedNavProp\":" +
                                                    "{" +
                                                        "\"ID\":103,\"Name\":\"Charlie\"" +
                                                    "}" +
                                                "}" +
                                            "]" +
                                        "}";
            result.Should().Be(expectedPayload);
        }

        [Fact]
        public void WritingEntryExpandWithExpandCollectionNavPropAndNonCollectionContainedElement()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                this.entryWithOnlyData1,
                this.expandedCollectionNavLink,
                new ODataFeed(), 
                this.entryWithOnlyData2,
                this.containedNavLink, 
                this.entryWithOnlyData3
            };

            const string selectClause = "ExpandedCollectionNavProp";
            const string expandClause = "ExpandedCollectionNavProp($select=ContainedNavProp;$expand=ContainedNavProp)";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause, "EntitySet(101)");

            string expectedPayload = "{" +
                                        "\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(ExpandedCollectionNavProp,ExpandedCollectionNavProp(ContainedNavProp))/$entity\"," +
                                        "\"ID\":101,\"Name\":\"Alice\"," +
                                        "\"ExpandedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                        "\"ExpandedCollectionNavProp\":" +
                                            "[" +
                                                "{" +
                                                    "\"ID\":102,\"Name\":\"Bob\"," +
                                                    "\"ContainedNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(102)/ContainedNavProp/$entity\"," +
                                                    "\"ContainedNavProp\":" +
                                                        "{" +
                                                            "\"ID\":103,\"Name\":\"Charlie\"" +
                                                        "}" +
                                                "}" +
                                            "]" +

                                    "}";
            result.Should().Be(expectedPayload);
        }

        [Fact]
        public void WritingEntryExpandWithCollectionContainedElementAndExpandNavProp()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                this.entryWithOnlyData1,
                this.containedCollectionNavLink, 
                new ODataFeed(), 
                this.entryWithOnlyData2,
                this.expandedNavLink,
                this.entryWithOnlyData3
            };

            const string selectClause = "ContainedCollectionNavProp";
            const string expandClause = "ContainedCollectionNavProp($select=ExpandedNavProp;$expand=ExpandedNavProp)";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause, "EntitySet(101)");

            string expectedPayload = "{" +
                                        "\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(ContainedCollectionNavProp,ContainedCollectionNavProp(ExpandedNavProp))/$entity\"," +
                                        "\"ID\":101,\"Name\":\"Alice\"," +
                                        "\"ContainedCollectionNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/ContainedCollectionNavProp(ExpandedNavProp)\"," +
                                        "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                        "\"ContainedCollectionNavProp\":" +
                                            "[" +
                                                "{" +
                                                    "\"ID\":102,\"Name\":\"Bob\"," +
                                                    "\"ExpandedNavProp\":" +
                                                        "{" +
                                                            "\"ID\":103,\"Name\":\"Charlie\"" +
                                                        "}" +
                                                "}" +
                                            "]" +
                                    "}";
            result.Should().Be(expectedPayload);
        }

        [Fact]
        public void WritingFeedExpandWithCollectionContainedElementWithTypeCast()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataFeed(),
                this.entryWithOnlyData1,
                this.containedCollectionNavLink,
                new ODataFeed(),
                this.entryWithOnlyData2,
                this.containedCollectionNavLink,
                new ODataFeed(),
                this.entryWithOnlyData3,
            };

            const string selectClause = "Namespace.DerivedType/ContainedCollectionNavProp";
            const string expandClause = "Namespace.DerivedType/ContainedCollectionNavProp($select=Namespace.DerivedType/ContainedCollectionNavProp;$expand=Namespace.DerivedType/ContainedCollectionNavProp)";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause, "EntitySet");

            const string expectedPayload = "{\"" +
                                                "@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(Namespace.DerivedType/ContainedCollectionNavProp,Namespace.DerivedType/ContainedCollectionNavProp(Namespace.DerivedType/ContainedCollectionNavProp))\"," +
                                                "\"value\":[" +
                                                    "{" +
                                                         "\"ID\":101,\"Name\":\"Alice\"," +
                                                        "\"ContainedCollectionNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/Namespace.DerivedType/ContainedCollectionNavProp(Namespace.DerivedType/ContainedCollectionNavProp)\"," +
                                                        "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                                        "\"ContainedCollectionNavProp\":" +
                                                            "[" +
                                                                "{" +
                                                                    "\"ID\":102,\"Name\":\"Bob\"," +
                                                                    "\"ContainedCollectionNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/Namespace.DerivedType/ContainedCollectionNavProp(102)/Namespace.DerivedType/ContainedCollectionNavProp\"," +
                                                                    "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                                                    "\"ContainedCollectionNavProp\":" +
                                                                        "[" +
                                                                            "{\"ID\":103,\"Name\":\"Charlie\"}" +
                                                                        "]" +
                                                                "}" +
                                                            "]" +
                                                    "}" +
                                                 "]" +
                                            "}";
            result.Should().Be(expectedPayload);
        }

        [Fact]
        public void WritingFeedExpandWithNonCollectionContainedElementWithTypeCast()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataFeed(), 
                this.entryWithOnlyData1,
                this.containedNavLink,
                this.entryWithOnlyData2,
                this.containedNavLink, 
                this.entryWithOnlyData3
            };

            const string selectClause = "ContainedNavProp";
            const string expandClause = "ContainedNavProp($select=Namespace.DerivedType/ContainedNavProp;$expand=Namespace.DerivedType/ContainedNavProp)";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, DerivedType, selectClause, expandClause, "EntitySet/Namespace.DerivedType");

            string expectedPayload = "{\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet/Namespace.DerivedType(ContainedNavProp,ContainedNavProp(Namespace.DerivedType/ContainedNavProp))\"," +
                                            "\"value\":[" +
                                                "{" +
                                                    "\"ID\":101,\"Name\":\"Alice\"," +
                                                    "\"ContainedNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/Namespace.DerivedType/ContainedNavProp(Namespace.DerivedType/ContainedNavProp)/$entity\"," +
                                                    "\"ContainedNavProp\":" +
                                                        "{" +
                                                            "\"ID\":102,\"Name\":\"Bob\"," +
                                                            "\"ContainedNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/Namespace.DerivedType/ContainedNavProp/Namespace.DerivedType/ContainedNavProp/$entity\"," +
                                                            "\"ContainedNavProp\":" +
                                                            "{" +
                                                                "\"ID\":103,\"Name\":\"Charlie\"" +
                                                            "}" +
                                                        "}" +
                                                "}" +
                                            "]" +
                                        "}";
            result.Should().Be(expectedPayload);
        }

        [Fact]
        public void WritingEntryExpandWithMixedCollectionAndNonCollectionContainedElementWithTypeCast()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                this.entryWithOnlyData1,
                this.containedCollectionNavLink,
                new ODataFeed(), 
                this.entryWithOnlyData2,
                this.containedNavLink, 
                this.entryWithOnlyData3,
            };

            const string selectClause = "Namespace.DerivedType/ContainedCollectionNavProp";
            const string expandClause = "Namespace.DerivedType/ContainedCollectionNavProp($select=ContainedNavProp;$expand=ContainedNavProp)";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause, "EntitySet(101)");

            string expectedPayload = "{\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(Namespace.DerivedType/ContainedCollectionNavProp,Namespace.DerivedType/ContainedCollectionNavProp(ContainedNavProp))/$entity\"," +
                                        "\"ID\":101,\"Name\":\"Alice\"," +
                                        "\"ContainedCollectionNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/Namespace.DerivedType/ContainedCollectionNavProp(ContainedNavProp)\"," +
                                        "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                        "\"ContainedCollectionNavProp\":" +
                                            "[" +
                                                "{" +
                                                    "\"ID\":102,\"Name\":\"Bob\"," +
                                                    "\"ContainedNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/Namespace.DerivedType/ContainedCollectionNavProp(102)/ContainedNavProp/$entity\"," +
                                                    "\"ContainedNavProp\":" +
                                                    "{" +
                                                        "\"ID\":103,\"Name\":\"Charlie\"" +
                                                    "}" +
                                                "}" +
                                            "]" +
                                        "}";
            result.Should().Be(expectedPayload);
        }

        [Fact]
        public void ReadingEntryExpandWithMixedCollectionAndNonCollectionContainedElementWithTypeCast()
        {
            string payload = "{\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(Namespace.DerivedType/ContainedCollectionNavProp,Namespace.DerivedType/ContainedCollectionNavProp(ContainedNavProp))/$entity\"," +
                                        "\"ID\":101,\"Name\":\"Alice\"," +
                                        "\"ContainedCollectionNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/Namespace.DerivedType/ContainedCollectionNavProp(ContainedNavProp)\"," +
                                        "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                        "\"ContainedCollectionNavProp\":" +
                                            "[" +
                                                "{" +
                                                    "\"ID\":102,\"Name\":\"Bob\"," +
                                                    "\"ContainedNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/Namespace.DerivedType/ContainedCollectionNavProp(102)/ContainedNavProp/$entity\"," +
                                                    "\"ContainedNavProp\":" +
                                                    "{" +
                                                        "\"ID\":103,\"Name\":\"Charlie\"" +
                                                    "}" +
                                                "}" +
                                            "]" +
                                        "}";
            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            List<ODataEntry> entryList = new List<ODataEntry>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var reader = messageReader.CreateODataEntryReader();
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.EntryEnd:
                            entryList.Add(reader.Item as ODataEntry);
                            break;
                    }
                }
            }

            ODataEntry charileEntry = entryList[0];
            ODataEntry bobEntry = entryList[1];
            ODataEntry aliceEntry = entryList[2];

            charileEntry.Id.Should().Be("http://example.org/odata.svc/EntitySet(101)/Namespace.DerivedType/ContainedCollectionNavProp(102)/ContainedNavProp");
            bobEntry.Id.Should().Be("http://example.org/odata.svc/EntitySet(101)/ContainedCollectionNavProp(102)");
            aliceEntry.Id.Should().Be("http://example.org/odata.svc/EntitySet(101)");
        }

        [Fact]
        public void WritingEntryExpandWithMixedCollectionAndNonCollectionContainedElementAtSameLevel()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                this.entryWithOnlyData1,
                this.expandedNavLink,
                this.entryWithOnlyData2,
                new ODataNavigationLinkEnd(),
                this.containedCollectionNavLink,
                new ODataFeed(), 
                this.entryWithOnlyData2,
                new ODataNavigationLinkEnd(), 
                this.containedNavLink, 
                this.entryWithOnlyData3,
            };

            const string selectClause = "ID,Name";
            const string expandClause = "ExpandedNavProp,ContainedCollectionNavProp($select=ID),ContainedNavProp($select=ID,Name)";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause, "EntitySet(101)");

            string expectedPayload = "{\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(ID,Name,ExpandedNavProp,ContainedCollectionNavProp,ContainedNavProp,ContainedCollectionNavProp(ID),ContainedNavProp(ID,Name))/$entity\"," +
                                            "\"ID\":101,\"Name\":\"Alice\"," +
                                            "\"ExpandedNavProp\":{\"ID\":102,\"Name\":\"Bob\"}," +
                                            "\"ContainedCollectionNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/ContainedCollectionNavProp(ID)\"," +
                                            "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                            "\"ContainedCollectionNavProp\":[" +
                                                "{\"ID\":102,\"Name\":\"Bob\"}" +
                                            "]," +
                                            "\"ContainedNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/ContainedNavProp(ID,Name)/$entity\"," +
                                            "\"ContainedNavProp\":{\"ID\":103,\"Name\":\"Charlie\"}" +
                                        "}";
            result.Should().Be(expectedPayload);
        }

        [Fact]
        public void ReadingEntryExpandWithMixedCollectionAndNonCollectionContainedElementAtSameLevel()
        {
            string payload = "{\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(ID,Name,ExpandedNavProp,ContainedCollectionNavProp,ContainedNavProp,ContainedCollectionNavProp(ID),ContainedNavProp(ID,Name))/$entity\"," +
                                            "\"ID\":101,\"Name\":\"Alice\"," +
                                            "\"ExpandedNavProp\":{\"ID\":102,\"Name\":\"Bob\"}," +
                                            "\"ContainedCollectionNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/ContainedCollectionNavProp(ID)\"," +
                                            "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                            "\"ContainedCollectionNavProp\":[" +
                                                "{\"ID\":102,\"Name\":\"Bob\"}" +
                                            "]," +
                                            "\"ContainedNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/ContainedNavProp(ID,Name)/$entity\"," +
                                            "\"ContainedNavProp\":{\"ID\":103,\"Name\":\"Charlie\"}" +
                                        "}";
            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            List<ODataEntry> entryList = new List<ODataEntry>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var reader = messageReader.CreateODataEntryReader();
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.EntryEnd:
                            entryList.Add(reader.Item as ODataEntry);
                            break;
                    }
                }
            }

            ODataEntry bobEntry = entryList[0];
            ODataEntry containedBobEntry = entryList[1];
            ODataEntry containedCharileEntry = entryList[2];
            ODataEntry topLevelAliceEntry = entryList[3];

            bobEntry.Id.Should().Be("http://example.org/odata.svc/EntitySet(102)");
            containedBobEntry.Id.Should().Be("http://example.org/odata.svc/EntitySet(101)/ContainedCollectionNavProp(102)");
            containedCharileEntry.Id.Should().Be("http://example.org/odata.svc/EntitySet(101)/ContainedNavProp");
            topLevelAliceEntry.Id.Should().Be("http://example.org/odata.svc/EntitySet(101)");
        }

        [Fact]
        public void ReadingContainedWithSubContextUrlShouldThrow()
        {
            string payload = "{\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(ID,Name,ExpandedNavProp,ContainedCollectionNavProp,ContainedNavProp,ContainedCollectionNavProp(ID),ContainedNavProp(ID,Name))/$entity\"," +
                                            "\"ID\":101,\"Name\":\"Alice\"," +
                                            "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                            "\"ContainedCollectionNavProp\":[" +
                                                "{\"ID\":102,\"Name\":\"Bob\"}" +
                                            "]" +
                                        "}";
            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            List<ODataEntry> entryList = new List<ODataEntry>();

            Action readContainedEntry = () =>
            {
                using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
                {
                    var reader = messageReader.CreateODataEntryReader();
                    while (reader.Read())
                    {
                        switch (reader.State)
                        {
                            case ODataReaderState.EntryEnd:
                                entryList.Add(reader.Item as ODataEntry);
                                break;
                        }
                    }
                }

                foreach (var oDataEntry in entryList)
                {
                    oDataEntry.Id.Should().NotBeNull();
                }
            };

            readContainedEntry.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataMetadataBuilder_MissingODataUri);
        }
        #endregion Containment Tests

        #region Inlinecount Tests
        [Fact]
        public void WritingTopLevelInlinecountTest()
        {
            ODataFeed feed = new ODataFeed { Count = 1 };

            ODataItem[] itemsToWrite = new ODataItem[]
            {
                feed, 
                this.entryWithOnlyData1,
            };

            string resourcePath = "EntitySet";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType, null, null, resourcePath);

            string expectedPayload = "{" +
                                            "\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet\"," +
                                            "\"@odata.count\":1," +
                                            "\"value\":[" +
                                                "{\"" +
                                                    "ID\":101,\"Name\":\"Alice\"" +
                                                "}" +
                                            "]" +
                                        "}";
            result.Should().Be(expectedPayload);
        }

        [Fact]
        public void WritingNestedInlinecountTest()
        {
            ODataFeed feed = new ODataFeed { Count = 1 };

            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataFeed(), 
                this.entryWithOnlyData1,
                this.containedCollectionNavLink,
                feed
            };

            string resourcePath = "EntitySet";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType, null, null, resourcePath);

            string expectedPayload = "{" +
                                            "\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet\"," +
                                            "\"value\":[" +
                                                "{" +
                                                    "\"ID\":101,\"Name\":\"Alice\"," +
                                                    "\"ContainedCollectionNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/ContainedCollectionNavProp\"," +
                                                    "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                                    "\"ContainedCollectionNavProp@odata.count\":1," +
                                                    "\"ContainedCollectionNavProp\":[]" +
                                                "}" +
                                            "]" +
                                        "}";
            result.Should().Be(expectedPayload);
        }

        [Fact]
        public void ReadingTopLevelInlinecountTest()
        {
            string payload = "{" +
                                "\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet\"," +
                                "\"@odata.count\":1881," +
                                "\"value\":[" +
                                    "{\"" +
                                        "ID\":101,\"Name\":\"Alice\"" +
                                    "}" +
                                "]" +
                            "}";
            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            List<ODataFeed> feedList = new List<ODataFeed>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var reader = messageReader.CreateODataFeedReader();
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.FeedEnd:
                            feedList.Add(reader.Item as ODataFeed);
                            break;
                    }
                }
            }

            ODataFeed topFeed = feedList[0];
            topFeed.Count.Should().Be(1881);
        }

        [Fact]
        public void ReadingNestedInlinecountTest()
        {
            string payload = "{" +
                                "\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet\"," +
                                "\"value\":[" +
                                    "{" +
                                        "\"ID\":101,\"Name\":\"Alice\"," +
                                        "\"ContainedCollectionNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/ContainedCollectionNavProp\"," +
                                        "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                        "\"ContainedCollectionNavProp@odata.count\":1900," +
                                        "\"ContainedCollectionNavProp\":[]" +
                                    "}" +
                                "]" +
                            "}";
            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            List<ODataFeed> feedList = new List<ODataFeed>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var reader = messageReader.CreateODataFeedReader();
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.FeedEnd:
                            feedList.Add(reader.Item as ODataFeed);
                            break;
                    }
                }
            }

            ODataFeed innerFeed = feedList[0];
            innerFeed.Count.Should().Be(1900);
            ODataFeed topFeed = feedList[1];
            topFeed.Count.Should().Be(null);
        }
        #endregion Inlinecount Tests

        [Fact]
        public void ShouldWriteAdditionalPropertyWhenFullValidationDisabled()
        {
            var entry = new ODataEntry
            {
                Properties = new[]
                {
                    new ODataProperty { Name = "ID", Value = 102 },
                    new ODataProperty { Name = "Name", Value = "Bob" },
                    new ODataProperty { Name = "Prop1", Value = "Var1" }
                },
            };

            ODataItem[] itemsToWrite = { entry };

            Action action = () => this.GetWriterOutputForContentTypeAndKnobValue(
                "application/json;odata.metadata=minimal",
                true,
                itemsToWrite,
                Model,
                EntitySet,
                EntityType,
                enableFullValidation: true);
            action.ShouldThrow<ODataException>().WithMessage(
                Strings.ValidationUtils_PropertyDoesNotExistOnType("Prop1", "Namespace.EntityType"));

            string result = this.GetWriterOutputForContentTypeAndKnobValue(
                "application/json;odata.metadata=minimal",
                true,
                itemsToWrite,
                Model,
                EntitySet,
                EntityType,
                enableFullValidation: false);
            string expectedPayload = 
                                  "{\"" +
                                    "@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet/$entity\"," +
                                    "\"ID\":102,\"Name\":\"Bob\",\"Prop1\":\"Var1\"" +
                                  "}";
            result.Should().Be(expectedPayload);
        }

        [Fact(Skip = "Ignore this until writing nested context URL is supported")]
        public void ShouldWriteNestedContextUrlIfCanNotBeInferred()
        {
            var entryWithOnlyData2WithSerializationInfo = new ODataEntry
            {
                Properties = new[] { new ODataProperty { Name = "ID", Value = 102 }, new ODataProperty { Name = "Name", Value = "Bob" } },
                SerializationInfo = new ODataFeedAndEntrySerializationInfo()
                {
                    NavigationSourceName = "FooSet",
                    NavigationSourceEntityTypeName = "NS.BarType"
                },
            };

            ODataItem[] itemsToWrite = new ODataItem[]
            {
                this.entryWithOnlyData1,
                this.containedCollectionNavLink, 
                new ODataFeed(), 
                entryWithOnlyData2WithSerializationInfo,
            };

            const string selectClause = "ContainedNavProp";
            const string expandClause = "ContainedNavProp";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause, "EntitySet");

            string expectedPayload = "{\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(ContainedNavProp)/$entity\"," +
                                        "\"ID\":101,\"Name\":\"Alice\"," +
                                        "\"ContainedCollectionNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/ContainedCollectionNavProp\"," +
                                        "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                        "\"ContainedCollectionNavProp\":[{\"ID\":102,\"Name\":\"Bob\"}]" +
                                      "}";
            result.Should().Be(expectedPayload);
        }

        #region Help Methods
        private string GetWriterOutputForContentTypeAndKnobValue(
            string contentType,
            bool autoComputePayloadMetadataInJson,
            ODataItem[] itemsToWrite,
            EdmModel edmModel,
            IEdmEntitySetBase edmEntitySet,
            EdmEntityType edmEntityType,
            string selectClause = null,
            string expandClause = null,
            string resourcePath = null,
            bool enableFullValidation = true)
        {
            MemoryStream outputStream = new MemoryStream();
            IODataResponseMessage message = new InMemoryMessage() { Stream = outputStream };
            message.SetHeader("Content-Type", contentType);
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings()
            {
                AutoComputePayloadMetadataInJson = autoComputePayloadMetadataInJson,
                EnableFullValidation = enableFullValidation
            };

            var result = new ODataQueryOptionParser(edmModel, edmEntityType, edmEntitySet, new Dictionary<string, string> { { "$expand", expandClause }, { "$select", selectClause } }).ParseSelectAndExpand();

            ODataUri odataUri = new ODataUri()
            {
                ServiceRoot = new Uri("http://example.org/odata.svc"),
                SelectAndExpand = result
            };

            if (resourcePath != null)
            {
                Uri requestUri = new Uri("http://example.org/odata.svc/" + resourcePath);
                odataUri.RequestUri = requestUri;
                odataUri.Path = new ODataUriParser(edmModel, new Uri("http://example.org/odata.svc/"), requestUri).ParsePath();
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
                while (currentIdx < itemsToWrite.Length)
                {
                    if (itemsToWrite[currentIdx] is ODataNavigationLink)
                    {
                        this.WriteLink(writer, itemsToWrite, ref currentIdx);
                    }
                    else if (itemsToWrite[currentIdx] is ODataNavigationLinkEnd)
                    {
                        currentIdx++;
                        writer.WriteEnd();
                        return;
                    }
                    else
                    {
                        break;
                    }
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
        #endregion Help Methods

        private sealed class ODataNavigationLinkEnd : ODataItem
        {
        }
    }
}
