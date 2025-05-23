﻿//---------------------------------------------------------------------
// <copyright file="FullPayloadValidateTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.Writer.Json
{
    public class FullPayloadValidateTests
    {
        #region Declaration And Initialization
        private static readonly EdmEntityType EntityType;
        private static readonly EdmEntityType DerivedType;
        private static readonly EdmEntitySet EntitySet;
        private static readonly EdmModel Model;
        private static readonly EdmModel ModelWithFunction;

        private ODataResource entryWithOnlyData1;
        private ODataResource entryWithOnlyData2;
        private ODataResource entryWithOnlyData3;

        private readonly ODataNestedResourceInfo expandedCollectionNavLink = new ODataNestedResourceInfo()
        {
            Url = new Uri("http://example.org/odata.svc/navigation"),
            IsCollection = true,
            Name = "ExpandedCollectionNavProp",
        };

        private readonly ODataNestedResourceInfo expandedNavLink = new ODataNestedResourceInfo()
        {
            IsCollection = false,
            Name = "ExpandedNavProp",
        };

        private readonly ODataNestedResourceInfo containedCollectionNavLink = new ODataNestedResourceInfo()
        {
            Url = new Uri("http://example.org/odata.svc/navigation"),
            IsCollection = true,
            Name = "ContainedCollectionNavProp",
        };

        private readonly ODataNestedResourceInfo containedNavLink = new ODataNestedResourceInfo()
        {
            IsCollection = false,
            Name = "ContainedNavProp",
        };

        static FullPayloadValidateTests()
        {
            EntityType = new EdmEntityType("Namespace", "EntityType", null, false, true, false);
            EntityType.AddKeys(EntityType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
            IEdmStructuralProperty nameProperty = EntityType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(isNullable: true), null);
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
            Model.SetOptimisticConcurrencyAnnotation(EntitySet, new[] { nameProperty });

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
            this.entryWithOnlyData1 = new ODataResource { Properties = new[] { new ODataProperty { Name = "ID", Value = 101 }, new ODataProperty { Name = "Name", Value = "Alice" } }, };
            this.entryWithOnlyData2 = new ODataResource { Properties = new[] { new ODataProperty { Name = "ID", Value = 102 }, new ODataProperty { Name = "Name", Value = "Bob" } }, };
            this.entryWithOnlyData3 = new ODataResource { Properties = new[] { new ODataProperty { Name = "ID", Value = 103 }, new ODataProperty { Name = "Name", Value = "Charlie" } }, };
        }
        #endregion Declaration & Initialization

        #region Containment Tests
        [Fact]
        public void WritingTopLevelContainedFeed()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataResourceSet(), this.entryWithOnlyData2
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
            Assert.Equal(expectedPayload, result);
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
            Assert.Equal(expectedPayload, result);
        }

        [Fact]
        public void WritingTopLevelContainedFeedWithTypeCast()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataResourceSet(), this.entryWithOnlyData2
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
            Assert.Equal(expectedPayload, result);
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
            Assert.Equal(expectedPayload, result);
        }

        [Fact]
        public void WritingFeedWithFunctionAndAction()
        {
            ODataResourceSet feed = new ODataResourceSet();
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
            Assert.Equal(expectedPayload, result);
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
            List<ODataResourceSet> feedList = new List<ODataResourceSet>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, ModelWithFunction))
            {
                Assert.Equal(ODataPayloadKind.ResourceSet, messageReader.DetectPayloadKind().Single().PayloadKind);

                var reader = messageReader.CreateODataResourceSetReader();
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceSetEnd:
                            feedList.Add(reader.Item as ODataResourceSet);
                            break;
                    }
                }
            }

            ODataResourceSet feed = feedList[0];
            Assert.Single(feed.Actions);
            Assert.Single(feed.Functions);
        }

        [Fact]
        public async void ReadDeltaFeedTest_CanReadAsyncDeltaRequests()
        {
            string payload = "{\"" +
                             "@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet/$delta\"," +
                             "\"@odata.count\":1000," +
                             "\"value\":[" +
                                           "{" +
                                              "\"ID\":101,\"Name\":\"Alice\"" +
                                           "}" +
                                       "]" +
                             "}";

            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            List<ODataDeltaResourceSet> feedList = new List<ODataDeltaResourceSet>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, ModelWithFunction))
            {
                var result = await messageReader.DetectPayloadKindAsync();

                Assert.Equal(ODataPayloadKind.Delta, result.Single().PayloadKind);

                var reader = await messageReader.CreateODataDeltaResourceSetReaderAsync();
                while (await reader.ReadAsync())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.DeltaResourceSetEnd:
                            feedList.Add(reader.Item as ODataDeltaResourceSet);
                            break;
                    }
                }
            }

            ODataDeltaResourceSet set = Assert.IsType<ODataDeltaResourceSet>(Assert.Single(feedList));

            Assert.Equal(1000, set.Count);
        }

        [Fact]
        public void WritingFeedExpandWithCollectionContainedElement()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataResourceSet(),
                this.entryWithOnlyData1,
                this.containedCollectionNavLink,
                new ODataResourceSet(),
                this.entryWithOnlyData2,
                this.containedCollectionNavLink,
                new ODataResourceSet(),
                this.entryWithOnlyData3,
            };

            const string selectClause = "ContainedCollectionNavProp";
            const string expandClause = "ContainedCollectionNavProp($select=ContainedCollectionNavProp;$expand=ContainedCollectionNavProp)";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause, "EntitySet");

            const string expectedPayload = "{\"" +
                                                "@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(ContainedCollectionNavProp,ContainedCollectionNavProp(ContainedCollectionNavProp,ContainedCollectionNavProp()))\"," +
                                                "\"value\":[" +
                                                    "{" +
                                                         "\"ID\":101,\"Name\":\"Alice\"," +
                                                        "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                                        "\"ContainedCollectionNavProp\":" +
                                                            "[" +
                                                                "{" +
                                                                    "\"ID\":102,\"Name\":\"Bob\"," +
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
            Assert.Equal(expectedPayload, result);
        }

        [Fact]
        public void WritingFeedExpandWithNonCollectionContainedElement()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataResourceSet(),
                this.entryWithOnlyData1,
                this.containedNavLink,
                this.entryWithOnlyData2,
                this.containedNavLink,
                this.entryWithOnlyData3
            };

            const string selectClause = "ContainedNavProp";
            const string expandClause = "ContainedNavProp($select=ContainedNavProp;$expand=ContainedNavProp)";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause, "EntitySet");

            string expectedPayload = "{\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(ContainedNavProp,ContainedNavProp(ContainedNavProp,ContainedNavProp()))\"," +
                                            "\"value\":[" +
                                                "{" +
                                                    "\"ID\":101,\"Name\":\"Alice\"," +
                                                    "\"ContainedNavProp\":" +
                                                        "{" +
                                                            "\"ID\":102,\"Name\":\"Bob\"," +
                                                            "\"ContainedNavProp\":" +
                                                            "{" +
                                                                "\"ID\":103,\"Name\":\"Charlie\"" +
                                                            "}" +
                                                        "}" +
                                                "}" +
                                            "]" +
                                        "}";
            Assert.Equal(expectedPayload, result);
        }

        [Fact]
        public void WritingEntryExpandWithMixedCollectionAndNonCollectionContainedElement()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                this.entryWithOnlyData1,
                this.containedCollectionNavLink,
                new ODataResourceSet(),
                this.entryWithOnlyData2,
                this.containedNavLink,
                this.entryWithOnlyData3,
            };

            const string selectClause = "ContainedCollectionNavProp";
            const string expandClause = "ContainedCollectionNavProp($select=ContainedNavProp;$expand=ContainedNavProp)";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause, "EntitySet(101)");

            string expectedPayload = "{\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(ContainedCollectionNavProp,ContainedCollectionNavProp(ContainedNavProp,ContainedNavProp()))/$entity\"," +
                                        "\"ID\":101,\"Name\":\"Alice\"," +
                                        "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                        "\"ContainedCollectionNavProp\":" +
                                            "[" +
                                                "{" +
                                                    "\"ID\":102,\"Name\":\"Bob\"," +
                                                    "\"ContainedNavProp\":" +
                                                    "{" +
                                                        "\"ID\":103,\"Name\":\"Charlie\"" +
                                                    "}" +
                                                "}" +
                                            "]" +
                                        "}";
            Assert.Equal(expectedPayload, result);
        }

        [Fact]
        public void WritingEntryExpandWithExpandCollectionNavPropAndNonCollectionContainedElement()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                this.entryWithOnlyData1,
                this.expandedCollectionNavLink,
                new ODataResourceSet(),
                this.entryWithOnlyData2,
                this.containedNavLink,
                this.entryWithOnlyData3
            };

            const string selectClause = "ExpandedCollectionNavProp";
            const string expandClause = "ExpandedCollectionNavProp($select=ContainedNavProp;$expand=ContainedNavProp)";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause, "EntitySet(101)");

            string expectedPayload = "{" +
                                        "\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(ExpandedCollectionNavProp,ExpandedCollectionNavProp(ContainedNavProp,ContainedNavProp()))/$entity\"," +
                                        "\"ID\":101,\"Name\":\"Alice\"," +
                                        "\"ExpandedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                        "\"ExpandedCollectionNavProp\":" +
                                            "[" +
                                                "{" +
                                                    "\"ID\":102,\"Name\":\"Bob\"," +
                                                    "\"ContainedNavProp\":" +
                                                        "{" +
                                                            "\"ID\":103,\"Name\":\"Charlie\"" +
                                                        "}" +
                                                "}" +
                                            "]" +

                                    "}";
            Assert.Equal(expectedPayload, result);
        }

        [Fact]
        public void WritingEntryExpandWithCollectionContainedElementAndExpandNavProp()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                this.entryWithOnlyData1,
                this.containedCollectionNavLink,
                new ODataResourceSet(),
                this.entryWithOnlyData2,
                this.expandedNavLink,
                this.entryWithOnlyData3
            };

            const string selectClause = "ContainedCollectionNavProp";
            const string expandClause = "ContainedCollectionNavProp($select=ExpandedNavProp;$expand=ExpandedNavProp)";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause, "EntitySet(101)");

            string expectedPayload = "{" +
                                        "\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(ContainedCollectionNavProp,ContainedCollectionNavProp(ExpandedNavProp,ExpandedNavProp()))/$entity\"," +
                                        "\"ID\":101,\"Name\":\"Alice\"," +
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
            Assert.Equal(expectedPayload, result);
        }

        [Fact]
        public void WritingFeedExpandWithCollectionContainedElementWithTypeCast()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataResourceSet(),
                this.entryWithOnlyData1,
                this.containedCollectionNavLink,
                new ODataResourceSet(),
                this.entryWithOnlyData2,
                this.containedCollectionNavLink,
                new ODataResourceSet(),
                this.entryWithOnlyData3,
            };

            const string selectClause = "Namespace.DerivedType/ContainedCollectionNavProp";
            const string expandClause = "Namespace.DerivedType/ContainedCollectionNavProp($select=Namespace.DerivedType/ContainedCollectionNavProp;$expand=Namespace.DerivedType/ContainedCollectionNavProp)";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause, "EntitySet");

            const string expectedPayload = "{\"" +
                                                "@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(Namespace.DerivedType/ContainedCollectionNavProp,Namespace.DerivedType/ContainedCollectionNavProp(Namespace.DerivedType/ContainedCollectionNavProp,Namespace.DerivedType/ContainedCollectionNavProp()))\"," +
                                                "\"value\":[" +
                                                    "{" +
                                                         "\"ID\":101,\"Name\":\"Alice\"," +
                                                        "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                                        "\"ContainedCollectionNavProp\":" +
                                                            "[" +
                                                                "{" +
                                                                    "\"ID\":102,\"Name\":\"Bob\"," +
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
            Assert.Equal(expectedPayload, result);
        }

        [Fact]
        public void WritingFeedExpandWithNonCollectionContainedElementWithTypeCast()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataResourceSet(),
                this.entryWithOnlyData1,
                this.containedNavLink,
                this.entryWithOnlyData2,
                this.containedNavLink,
                this.entryWithOnlyData3
            };

            const string selectClause = "ContainedNavProp";
            const string expandClause = "ContainedNavProp($select=Namespace.DerivedType/ContainedNavProp;$expand=Namespace.DerivedType/ContainedNavProp)";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, DerivedType, selectClause, expandClause, "EntitySet/Namespace.DerivedType");

            string expectedPayload = "{\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet/Namespace.DerivedType(ContainedNavProp,ContainedNavProp(Namespace.DerivedType/ContainedNavProp,Namespace.DerivedType/ContainedNavProp()))\"," +
                                            "\"value\":[" +
                                                "{" +
                                                    "\"ID\":101,\"Name\":\"Alice\"," +
                                                    "\"ContainedNavProp\":" +
                                                        "{" +
                                                            "\"ID\":102,\"Name\":\"Bob\"," +
                                                            "\"ContainedNavProp\":" +
                                                            "{" +
                                                                "\"ID\":103,\"Name\":\"Charlie\"" +
                                                            "}" +
                                                        "}" +
                                                "}" +
                                            "]" +
                                        "}";
            Assert.Equal(expectedPayload, result);
        }

        [Fact]
        public void WritingEntryExpandWithMixedCollectionAndNonCollectionContainedElementWithTypeCast()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                this.entryWithOnlyData1,
                this.containedCollectionNavLink,
                new ODataResourceSet(),
                this.entryWithOnlyData2,
                this.containedNavLink,
                this.entryWithOnlyData3,
            };

            const string selectClause = "Namespace.DerivedType/ContainedCollectionNavProp";
            const string expandClause = "Namespace.DerivedType/ContainedCollectionNavProp($select=ContainedNavProp;$expand=ContainedNavProp)";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause, "EntitySet(101)");

            string expectedPayload = "{\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(Namespace.DerivedType/ContainedCollectionNavProp,Namespace.DerivedType/ContainedCollectionNavProp(ContainedNavProp,ContainedNavProp()))/$entity\"," +
                                        "\"ID\":101,\"Name\":\"Alice\"," +
                                        "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                        "\"ContainedCollectionNavProp\":" +
                                            "[" +
                                                "{" +
                                                    "\"ID\":102,\"Name\":\"Bob\"," +
                                                    "\"ContainedNavProp\":" +
                                                    "{" +
                                                        "\"ID\":103,\"Name\":\"Charlie\"" +
                                                    "}" +
                                                "}" +
                                            "]" +
                                        "}";
            Assert.Equal(expectedPayload, result);
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
            List<ODataResource> entryList = new List<ODataResource>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var reader = messageReader.CreateODataResourceReader();
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceEnd:
                            entryList.Add(reader.Item as ODataResource);
                            break;
                    }
                }
            }

            ODataResource charileEntry = entryList[0];
            ODataResource bobEntry = entryList[1];
            ODataResource aliceEntry = entryList[2];

            Assert.Equal("http://example.org/odata.svc/EntitySet(101)/ContainedCollectionNavProp(102)/ContainedNavProp", charileEntry.Id.OriginalString);
            Assert.Equal("http://example.org/odata.svc/EntitySet(101)/ContainedCollectionNavProp(102)", bobEntry.Id.OriginalString);
            Assert.Equal("http://example.org/odata.svc/EntitySet(101)", aliceEntry.Id.OriginalString);
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
                new ODataResourceSet(),
                this.entryWithOnlyData2,
                new ODataNavigationLinkEnd(),
                this.containedNavLink,
                this.entryWithOnlyData3,
            };

            const string selectClause = "ID,Name";
            const string expandClause = "ExpandedNavProp,ContainedCollectionNavProp($select=ID),ContainedNavProp($select=ID,Name)";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause, "EntitySet(101)");

            string expectedPayload = "{\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(ID,Name,ExpandedNavProp(),ContainedCollectionNavProp(ID),ContainedNavProp(ID,Name))/$entity\"," +
                                            "\"ID\":101,\"Name\":\"Alice\"," +
                                            "\"ExpandedNavProp\":{\"ID\":102,\"Name\":\"Bob\"}," +
                                            "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                            "\"ContainedCollectionNavProp\":[" +
                                                "{\"ID\":102,\"Name\":\"Bob\"}" +
                                            "]," +
                                            "\"ContainedNavProp\":{\"ID\":103,\"Name\":\"Charlie\"}" +
                                        "}";
            Assert.Equal(expectedPayload, result);
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
            List<ODataResource> entryList = new List<ODataResource>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var reader = messageReader.CreateODataResourceReader();
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceEnd:
                            entryList.Add(reader.Item as ODataResource);
                            break;
                    }
                }
            }

            ODataResource bobEntry = entryList[0];
            ODataResource containedBobEntry = entryList[1];
            ODataResource containedCharileEntry = entryList[2];
            ODataResource topLevelAliceEntry = entryList[3];

            Assert.Equal("http://example.org/odata.svc/EntitySet(102)", bobEntry.Id.OriginalString);
            Assert.Equal("http://example.org/odata.svc/EntitySet(101)/ContainedCollectionNavProp(102)", containedBobEntry.Id.OriginalString);
            Assert.Equal("http://example.org/odata.svc/EntitySet(101)/ContainedNavProp", containedCharileEntry.Id.OriginalString);
            Assert.Equal("http://example.org/odata.svc/EntitySet(101)", topLevelAliceEntry.Id.OriginalString);
        }

        [Fact]
        public void ReadingContainedWithSubContextUrl()
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
            List<ODataResource> entryList = new List<ODataResource>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var reader = messageReader.CreateODataResourceReader();
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceEnd:
                            entryList.Add(reader.Item as ODataResource);
                            break;
                    }
                }
            }

            foreach (var oDataEntry in entryList)
            {
                Assert.NotNull(oDataEntry.Id);
            }

        }
        #endregion Containment Tests

        #region Inlinecount Tests
        [Fact]
        public void WritingTopLevelInlinecountTest()
        {
            ODataResourceSet feed = new ODataResourceSet { Count = 1 };

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
            Assert.Equal(expectedPayload, result);
        }

        [Fact]
        public void WritingNestedInlinecountTest()
        {
            ODataResourceSet feed = new ODataResourceSet { Count = 1 };

            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataResourceSet(),
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
                                                    "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                                    "\"ContainedCollectionNavProp@odata.count\":1," +
                                                    "\"ContainedCollectionNavProp\":[]" +
                                               "}" +
                                            "]" +
                                        "}";
            Assert.Equal(expectedPayload, result);
        }

        [Fact]
        public void WritingNestedInlinecountWithoutContentTest()
        {
            this.containedCollectionNavLink.Count = 42;
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataResourceSet(),
                this.entryWithOnlyData1,
                this.containedCollectionNavLink
            };

            string resourcePath = "EntitySet";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType, null, null, resourcePath);

            string expectedPayload = "{" +
                                            "\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet\"," +
                                            "\"value\":[" +
                                                "{" +
                                                    "\"ID\":101,\"Name\":\"Alice\"," +
                                                    "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                                    "\"ContainedCollectionNavProp@odata.count\":42" +
                                                "}" +
                                            "]" +
                                        "}";
            Assert.Equal(expectedPayload, result);
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
            List<ODataResourceSet> feedList = new List<ODataResourceSet>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var reader = messageReader.CreateODataResourceSetReader();
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceSetEnd:
                            feedList.Add(reader.Item as ODataResourceSet);
                            break;
                    }
                }
            }

            ODataResourceSet topFeed = feedList[0];
            Assert.Equal(1881, topFeed.Count);
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
            List<ODataResourceSet> feedList = new List<ODataResourceSet>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var reader = messageReader.CreateODataResourceSetReader();
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceSetEnd:
                            feedList.Add(reader.Item as ODataResourceSet);
                            break;
                    }
                }
            }

            ODataResourceSet innerFeed = feedList[0];
            Assert.Equal(1900, innerFeed.Count);
            ODataResourceSet topFeed = feedList[1];
            Assert.Null(topFeed.Count);
        }

        [Fact]
        public void ReadingNestedInlinecountWithoutContentTest()
        {
            string payload = "{" +
                                "\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet\"," +
                                "\"value\":[" +
                                    "{" +
                                        "\"ID\":101,\"Name\":\"Alice\"," +
                                        "\"ContainedCollectionNavProp@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(101)/ContainedCollectionNavProp\"," +
                                        "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                        "\"ContainedCollectionNavProp@odata.count\":51" +
                                    "}" +
                                "]" +
                            "}";
            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            List<ODataResourceSet> feedList = new List<ODataResourceSet>();

            ODataNestedResourceInfo nestedResourceInfo = null;
            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, Model))
            {
                var reader = messageReader.CreateODataResourceSetReader();
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceSetEnd:
                            feedList.Add(reader.Item as ODataResourceSet);
                            break;

                        case ODataReaderState.NestedResourceInfoStart:
                            //  The reader will return nested resource infos for all navigation properties since by default all properties are selected.
                            if (reader.Item is ODataNestedResourceInfo nestedInfo && nestedInfo.Name == "ContainedCollectionNavProp")
                            {
                                nestedResourceInfo = nestedInfo;
                            }
                            break;
                    }
                }
            }

            Assert.Single(feedList); // only contains the toplevel
            Assert.NotNull(nestedResourceInfo);
            Assert.Equal(51, nestedResourceInfo.Count);
        }
        #endregion Inlinecount Tests

        [Fact]
        public void ShouldAlwaysWriteAdditionalPropertyForOpenType()
        {
            var entry = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty { Name = "ID", Value = 102 },
                    new ODataProperty { Name = "Name", Value = "Bob" },
                    new ODataProperty { Name = "Prop1", Value = "Var1" },
                    new ODataProperty { Name = "UntypedProperty", Value = new ODataUntypedValue {RawValue="\"rawValue\"" } }
                },
            };

            ODataItem[] itemsToWrite = { entry };

            string expectedPayload =
                                  "{\"" +
                                    "@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet/$entity\"," +
                                    "\"ID\":102,\"Name\":\"Bob\",\"Prop1\":\"Var1\",\"UntypedProperty\":\"rawValue\"" +
                                  "}";

            string result = this.GetWriterOutputForContentTypeAndKnobValue(
                "application/json;odata.metadata=minimal",
                true,
                itemsToWrite,
                Model,
                EntitySet,
                EntityType,
                enableBasicValidation: true);
            Assert.Equal(expectedPayload, result);

            result = this.GetWriterOutputForContentTypeAndKnobValue(
                "application/json;odata.metadata=minimal",
                true,
                itemsToWrite,
                Model,
                EntitySet,
                EntityType,
                enableBasicValidation: false);
            Assert.Equal(expectedPayload, result);
        }

        [Fact]
        public void ShouldWriteNestedContextUrlIfCanNotBeInferred()
        {
            var entryWithOnlyData2WithSerializationInfo = new ODataResource
            {
                Properties = new[] { new ODataProperty { Name = "ID", Value = 102 }, new ODataProperty { Name = "Name", Value = "Bob" } },
                SerializationInfo = new ODataResourceSerializationInfo()
                {
                    NavigationSourceName = "FooSet",
                    NavigationSourceEntityTypeName = "NS.BarType"
                },
            };

            ODataItem[] itemsToWrite = new ODataItem[]
            {
                this.entryWithOnlyData1,
                this.containedCollectionNavLink,
                new ODataResourceSet(),
                entryWithOnlyData2WithSerializationInfo,
            };

            const string selectClause = "ContainedNavProp";
            const string expandClause = "ContainedNavProp";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, EntitySet, EntityType, selectClause, expandClause, "EntitySet");

            string expectedPayload = "{\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(ContainedNavProp,ContainedNavProp())/$entity\"," +
                                        "\"ID\":101,\"Name\":\"Alice\"," +
                                        "\"ContainedCollectionNavProp@odata.navigationLink\":\"http://example.org/odata.svc/navigation\"," +
                                        "\"ContainedCollectionNavProp\":[{\"ID\":102,\"Name\":\"Bob\"}]" +
                                      "}";
            Assert.Equal(expectedPayload, result);
        }

        [Fact]
        public void WriteContextMetadataEntityApplyComputeProperties()
        {
            ODataItem[] itemsToWrite = new ODataItem[]
            {
                new ODataResourceSet(), this.entryWithOnlyData2
            };

            IEdmNavigationProperty containedNavProp = EntityType.FindProperty("ContainedCollectionNavProp") as IEdmNavigationProperty;
            IEdmEntitySetBase contianedEntitySet = EntitySet.FindNavigationTarget(containedNavProp) as IEdmEntitySetBase;
            string resourcePath = "EntitySet(123)/ContainedCollectionNavProp";
            string applyClause = "compute(ID mul 2 as idMul2,length(Name) as nameLenght)";
            string result = this.GetWriterOutputForContentTypeAndKnobValue("application/json;odata.metadata=minimal", true, itemsToWrite, Model, contianedEntitySet, EntityType, null, null, resourcePath, applyClause);

            Assert.StartsWith("{\"@odata.context\":\"http://example.org/odata.svc/$metadata#EntitySet(123)/ContainedCollectionNavProp", result);
        }

        #region Help Methods
        private string GetWriterOutputForContentTypeAndKnobValue(
            string contentType,
            bool autoComputePayloadMetadata,
            ODataItem[] itemsToWrite,
            EdmModel edmModel,
            IEdmEntitySetBase edmEntitySet,
            EdmEntityType edmEntityType,
            string selectClause = null,
            string expandClause = null,
            string resourcePath = null,
            string applyClause = null,
            bool enableBasicValidation = true)
        {
            MemoryStream outputStream = new MemoryStream();
            IODataResponseMessage message = new InMemoryMessage() { Stream = outputStream };
            message.SetHeader("Content-Type", contentType);
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings()
            {
                Validations = (enableBasicValidation ? ValidationKinds.All : ValidationKinds.None),
            };

            var parser = new ODataQueryOptionParser(edmModel, edmEntityType, edmEntitySet, new Dictionary<string, string> { { "$expand", expandClause }, { "$select", selectClause }, { "$apply", applyClause } });
            var result = parser.ParseSelectAndExpand();

            ODataUri odataUri = new ODataUri()
            {
                ServiceRoot = new Uri("http://example.org/odata.svc"),
                SelectAndExpand = parser.ParseSelectAndExpand(),
                Apply = parser.ParseApply()
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

                if (itemsToWrite[currentIdx] is ODataResourceSet)
                {
                    ODataWriter writer = messageWriter.CreateODataResourceSetWriter(edmEntitySet, edmEntityType);
                    this.WriteFeed(writer, itemsToWrite, ref currentIdx);
                }
                else if (itemsToWrite[currentIdx] is ODataResource)
                {
                    ODataWriter writer = messageWriter.CreateODataResourceWriter(edmEntitySet, edmEntityType);
                    this.WriteEntry(writer, itemsToWrite, ref currentIdx);
                }
                else
                {
                    Assert.True(false, "Top level item to write must be entry or feed.");
                }

                Assert.Equal(itemsToWrite.Length, currentIdx);

                outputStream.Seek(0, SeekOrigin.Begin);
                output = new StreamReader(outputStream).ReadToEnd();
            }

            return output;
        }

        private void WriteFeed(ODataWriter writer, ODataItem[] itemsToWrite, ref int currentIdx)
        {
            if (currentIdx < itemsToWrite.Length)
            {
                ODataResourceSet feed = (ODataResourceSet)itemsToWrite[currentIdx++];
                writer.WriteStart(feed);
                while (currentIdx < itemsToWrite.Length && itemsToWrite[currentIdx] is ODataResource)
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
                ODataResource entry = (ODataResource)itemsToWrite[currentIdx++];
                writer.WriteStart(entry);
                while (currentIdx < itemsToWrite.Length)
                {
                    if (itemsToWrite[currentIdx] is ODataNestedResourceInfo)
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
                ODataNestedResourceInfo link = (ODataNestedResourceInfo)itemsToWrite[currentIdx++];
                writer.WriteStart(link);
                if (currentIdx < itemsToWrite.Length)
                {
                    if (itemsToWrite[currentIdx] is ODataResource)
                    {
                        this.WriteEntry(writer, itemsToWrite, ref currentIdx);
                    }
                    else if (itemsToWrite[currentIdx] is ODataResourceSet)
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
