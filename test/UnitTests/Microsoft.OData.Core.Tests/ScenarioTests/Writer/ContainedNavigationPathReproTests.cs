//---------------------------------------------------------------------
// <copyright file="ContainedNavigationPathReproTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.Writer
{
    public class ContainedNavigationPathReproTests
    {
        [Theory]
        [InlineData(true, false)]
        [InlineData(true, true)]
        [InlineData(false, false)]
        [InlineData(false, true)]
        public void WriteNestedContainedResourcesUsesExplicitIdsWhenKeysAreMissing(bool parentHasId, bool useAbsoluteIds)
        {
            Uri serviceRoot = new Uri("https://service.test/");
            Uri driveItemId = parentHasId ? CreateResourceId(serviceRoot, "driveItems('parent')", useAbsoluteIds) : null;
            Uri listItemId = CreateResourceId(serviceRoot, "driveItems('parent')/listItem", useAbsoluteIds);
            Uri fieldsId = CreateResourceId(serviceRoot, "driveItems('parent')/listItem/fields", useAbsoluteIds);

            string payload = WriteSingleContainedResources(
                serviceRoot,
                driveItemId,
                listItemId,
                fieldsId,
                setCustomLinks: !parentHasId);

            Assert.Contains($"\"@odata.id\":\"{fieldsId.OriginalString}\"", payload);
        }

        [Fact]
        public void WriteNestedContainedResourcesUsesRelativeIdsWithoutServiceRoot()
        {
            string payload = WriteSingleContainedResources(
                serviceRoot: null,
                new Uri("driveItems('parent')", UriKind.Relative),
                new Uri("driveItems('parent')/listItem", UriKind.Relative),
                new Uri("driveItems('parent')/listItem/fields", UriKind.Relative),
                setCustomLinks: false);

            Assert.Contains("\"fields\":{}", payload);
        }

        [Fact]
        public void WriteNestedContainedCollectionUsesEachResourcesExplicitIdWhenKeysAreMissing()
        {
            Uri serviceRoot = new Uri("https://service.test/");
            EdmModel model = CreateModel(
                listItemNavigationName: "items",
                listItemMultiplicity: EdmMultiplicity.Many,
                out EdmEntityType driveItemType,
                out EdmEntitySet driveItems);
            Uri requestUri = new Uri(serviceRoot, "driveItems('parent')");
            ODataMessageWriterSettings settings = CreateSettings(model, serviceRoot, requestUri);

            ODataResource driveItem = new ODataResource
            {
                Id = new Uri("driveItems('parent')", UriKind.Relative)
            };
            ODataResource listItem = new ODataResource
            {
                Id = new Uri("driveItems('parent')/items('child')", UriKind.Relative)
            };
            ODataResource fields = new ODataResource
            {
                Id = new Uri("driveItems('parent')/items('child')/fields", UriKind.Relative)
            };

            using (MemoryStream stream = new MemoryStream())
            {
                InMemoryMessage message = new InMemoryMessage { Stream = stream };
                using (ODataMessageWriter messageWriter = new ODataMessageWriter((IODataResponseMessage)message, settings, model))
                {
                    ODataWriter resourceWriter = messageWriter.CreateODataResourceWriter(driveItems, driveItemType);
                    resourceWriter.WriteStart(driveItem);
                    resourceWriter.WriteStart(new ODataNestedResourceInfo { Name = "items", IsCollection = true });
                    resourceWriter.WriteStart(new ODataResourceSet());
                    resourceWriter.WriteStart(listItem);
                    resourceWriter.WriteStart(new ODataNestedResourceInfo { Name = "fields", IsCollection = false });
                    resourceWriter.WriteStart(fields);
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.Flush();
                }

                string payload = Encoding.UTF8.GetString(stream.ToArray());
                Assert.Contains("\"@odata.id\":\"driveItems('parent')/items('child')/fields\"", payload);
            }
        }

        [Fact]
        public void WriteNestedContainedCollectionDoesNotDuplicateKeyInRecoveredResourcePath()
        {
            Uri serviceRoot = new Uri("https://service.test/");
            EdmModel model = CreateModel(
                listItemNavigationName: "items",
                listItemMultiplicity: EdmMultiplicity.Many,
                out EdmEntityType driveItemType,
                out EdmEntitySet driveItems);
            Uri requestUri = new Uri(serviceRoot, "driveItems('parent')");
            ODataMessageWriterSettings settings = CreateSettings(model, serviceRoot, requestUri);
            settings.SetContentType("application/json;odata.metadata=minimal", null);
            Uri listItemId = new Uri("driveItems('parent')/items('child')", UriKind.Relative);
            int keySegmentCount;

            using (MemoryStream stream = new MemoryStream())
            {
                InMemoryMessage message = new InMemoryMessage { Stream = stream };
                using (ODataMessageWriter messageWriter = new ODataMessageWriter((IODataResponseMessage)message, settings, model))
                {
                    ODataWriter resourceWriter = messageWriter.CreateODataResourceWriter(driveItems, driveItemType);
                    resourceWriter.WriteStart(new ODataResource
                    {
                        EditLink = new Uri("driveItems('parent')", UriKind.Relative)
                    });
                    resourceWriter.WriteStart(new ODataNestedResourceInfo
                    {
                        Name = "items",
                        IsCollection = true,
                        Url = new Uri("driveItems('parent')/items", UriKind.Relative),
                        AssociationLinkUrl = new Uri("driveItems('parent')/items/$ref", UriKind.Relative)
                    });
                    resourceWriter.WriteStart(new ODataResourceSet());
                    resourceWriter.WriteStart(new ODataResource
                    {
                        Id = listItemId,
                        Properties = new[] { new ODataProperty { Name = "id", Value = "child" } }
                    });
                    resourceWriter.WriteStart(new ODataNestedResourceInfo { Name = "fields", IsCollection = false });
                    keySegmentCount = GetCurrentPath(resourceWriter).OfType<KeySegment>().Count();
                    resourceWriter.WriteStart(new ODataResource
                    {
                        Id = new Uri($"{listItemId.OriginalString}/fields", UriKind.Relative)
                    });
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.Flush();
                }

                string payload = Encoding.UTF8.GetString(stream.ToArray());
                Assert.Equal(2, keySegmentCount);
                Assert.Contains("\"@odata.id\":\"driveItems('parent')/items('child')/fields\"", payload);
            }
        }

        [Fact]
        public void WriteNestedContainedCollectionAppendsKeyToRecoveredCollectionPath()
        {
            Uri serviceRoot = new Uri("https://service.test/");
            EdmModel model = CreateModel(
                listItemNavigationName: "items",
                listItemMultiplicity: EdmMultiplicity.Many,
                out EdmEntityType driveItemType,
                out EdmEntitySet driveItems);
            Uri requestUri = new Uri(serviceRoot, "driveItems('parent')");
            ODataMessageWriterSettings settings = CreateSettings(model, serviceRoot, requestUri);
            settings.SetContentType("application/json;odata.metadata=minimal", null);
            ODataPath recoveredPath;

            using (MemoryStream stream = new MemoryStream())
            {
                InMemoryMessage message = new InMemoryMessage { Stream = stream };
                using (ODataMessageWriter messageWriter = new ODataMessageWriter((IODataResponseMessage)message, settings, model))
                {
                    ODataWriter resourceWriter = messageWriter.CreateODataResourceWriter(driveItems, driveItemType);
                    resourceWriter.WriteStart(new ODataResource
                    {
                        EditLink = new Uri("driveItems('parent')", UriKind.Relative)
                    });
                    resourceWriter.WriteStart(new ODataNestedResourceInfo
                    {
                        Name = "items",
                        IsCollection = true,
                        Url = new Uri("driveItems('parent')/items", UriKind.Relative),
                        AssociationLinkUrl = new Uri("driveItems('parent')/items/$ref", UriKind.Relative)
                    });
                    resourceWriter.WriteStart(new ODataResourceSet());
                    resourceWriter.WriteStart(new ODataResource
                    {
                        Id = new Uri("driveItems('parent')/items", UriKind.Relative),
                        Properties = new[] { new ODataProperty { Name = "id", Value = "child" } }
                    });
                    resourceWriter.WriteStart(new ODataNestedResourceInfo { Name = "fields", IsCollection = false });
                    recoveredPath = GetCurrentPath(resourceWriter);
                    resourceWriter.WriteStart(new ODataResource
                    {
                        Id = new Uri("driveItems('parent')/items('child')/fields", UriKind.Relative)
                    });
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.Flush();
                }
            }

            Assert.Equal(2, recoveredPath.OfType<KeySegment>().Count());
            Assert.Equal(
                "driveItems('parent')/items('child')/fields",
                recoveredPath.ToResourcePathString(ODataUrlKeyDelimiter.Parentheses));
        }

        [Fact]
        public void RecoveredTypeCastResourcePathIsTreatedAsSingleResource()
        {
            Uri serviceRoot = new Uri("https://service.test/");
            EdmModel model = CreateModel(
                listItemNavigationName: "items",
                listItemMultiplicity: EdmMultiplicity.Many,
                out EdmEntityType driveItemType,
                out EdmEntitySet driveItems);
            EdmEntityType listItemType = (EdmEntityType)model.FindDeclaredType("Microsoft.Graph.ListItem");
            EdmEntityType derivedListItemType = model.AddEntityType("Microsoft.Graph", "DerivedListItem", listItemType);
            Uri requestUri = new Uri(serviceRoot, "driveItems('parent')");
            ODataMessageWriterSettings settings = CreateSettings(model, serviceRoot, requestUri);
            settings.SetContentType("application/json;odata.metadata=minimal", null);
            Uri listItemId = new Uri(
                "driveItems('parent')/items('child')/Microsoft.Graph.DerivedListItem",
                UriKind.Relative);
            ODataPath recoveredPath;

            using (MemoryStream stream = new MemoryStream())
            {
                InMemoryMessage message = new InMemoryMessage { Stream = stream };
                using (ODataMessageWriter messageWriter = new ODataMessageWriter((IODataResponseMessage)message, settings, model))
                {
                    ODataWriter resourceWriter = messageWriter.CreateODataResourceWriter(driveItems, driveItemType);
                    resourceWriter.WriteStart(new ODataResource
                    {
                        EditLink = new Uri("driveItems('parent')", UriKind.Relative)
                    });
                    resourceWriter.WriteStart(new ODataNestedResourceInfo
                    {
                        Name = "items",
                        IsCollection = true,
                        Url = new Uri("driveItems('parent')/items", UriKind.Relative),
                        AssociationLinkUrl = new Uri("driveItems('parent')/items/$ref", UriKind.Relative)
                    });
                    resourceWriter.WriteStart(new ODataResourceSet());
                    resourceWriter.WriteStart(new ODataResource
                    {
                        Id = listItemId,
                        TypeName = derivedListItemType.FullTypeName()
                    });
                    resourceWriter.WriteStart(new ODataNestedResourceInfo { Name = "fields", IsCollection = false });
                    recoveredPath = GetCurrentPath(resourceWriter);
                    resourceWriter.WriteStart(new ODataResource
                    {
                        Id = new Uri($"{listItemId.OriginalString}/fields", UriKind.Relative)
                    });
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.Flush();
                }
            }

            Assert.Contains(recoveredPath, segment => segment is TypeSegment);
            Assert.Equal(
                "driveItems('parent')/items('child')/Microsoft.Graph.DerivedListItem/fields",
                recoveredPath.ToResourcePathString(ODataUrlKeyDelimiter.Parentheses));
        }

        [Fact]
        public void RecoveredCollectionPathWithoutKeyDoesNotIdentifyCurrentResource()
        {
            Uri serviceRoot = new Uri("https://service.test/");
            EdmModel model = CreateModel(
                listItemNavigationName: "items",
                listItemMultiplicity: EdmMultiplicity.Many,
                out EdmEntityType driveItemType,
                out EdmEntitySet driveItems);
            Uri requestUri = new Uri(serviceRoot, "driveItems('parent')");
            ODataMessageWriterSettings settings = CreateSettings(model, serviceRoot, requestUri);
            settings.SetContentType("application/json;odata.metadata=minimal", null);
            ODataPath recoveredPath;

            using (MemoryStream stream = new MemoryStream())
            {
                InMemoryMessage message = new InMemoryMessage { Stream = stream };
                using (ODataMessageWriter messageWriter = new ODataMessageWriter((IODataResponseMessage)message, settings, model))
                {
                    ODataWriter resourceWriter = messageWriter.CreateODataResourceWriter(driveItems, driveItemType);
                    resourceWriter.WriteStart(new ODataResource
                    {
                        EditLink = new Uri("driveItems('parent')", UriKind.Relative)
                    });
                    resourceWriter.WriteStart(new ODataNestedResourceInfo
                    {
                        Name = "items",
                        IsCollection = true,
                        Url = new Uri("driveItems('parent')/items", UriKind.Relative),
                        AssociationLinkUrl = new Uri("driveItems('parent')/items/$ref", UriKind.Relative)
                    });
                    resourceWriter.WriteStart(new ODataResourceSet());
                    resourceWriter.WriteStart(new ODataResource
                    {
                        Id = new Uri("driveItems('parent')/items", UriKind.Relative)
                    });
                    resourceWriter.WriteStart(new ODataNestedResourceInfo { Name = "fields", IsCollection = false });
                    recoveredPath = GetCurrentPath(resourceWriter);
                    resourceWriter.WriteStart(new ODataResource
                    {
                        Id = new Uri("driveItems('parent')/items/fields", UriKind.Relative)
                    });
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.Flush();
                }
            }

            Assert.Null(recoveredPath);
        }

        [Fact]
        public void InvalidExplicitIdsPreserveMissingContainedPathException()
        {
            Uri serviceRoot = new Uri("https://service.test/");
            ODataException exception = Assert.Throws<ODataException>(() => WriteSingleContainedResources(
                serviceRoot,
                new Uri("unknown('parent')", UriKind.Relative),
                new Uri("unknown('parent')/listItem", UriKind.Relative),
                new Uri("unknown('parent')/listItem/fields", UriKind.Relative),
                setCustomLinks: true));

            Assert.Equal(
                SRResources.ODataWriterCore_PathInODataUriMustBeSetWhenWritingContainedElement,
                exception.Message);
        }

        private static string WriteSingleContainedResources(
            Uri serviceRoot,
            Uri driveItemId,
            Uri listItemId,
            Uri fieldsId,
            bool setCustomLinks)
        {
            EdmModel model = CreateModel(
                listItemNavigationName: "listItem",
                listItemMultiplicity: EdmMultiplicity.ZeroOrOne,
                out EdmEntityType driveItemType,
                out EdmEntitySet driveItems);
                ODataMessageWriterSettings settings;
                if (serviceRoot == null)
                {
                    settings = new ODataMessageWriterSettings
                    {
                        ODataUri = new ODataUri(),
                        EnableMessageStreamDisposal = false
                    };
                    settings.SetContentType("application/json;odata.metadata=none", null);
                }
                else
                {
                    Uri requestUri = new Uri(serviceRoot, "driveItems('parent')");
                    settings = CreateSettings(model, serviceRoot, requestUri);
                }

            ODataNestedResourceInfo listItemInfo = new ODataNestedResourceInfo
            {
                Name = "listItem",
                IsCollection = false
            };
            if (setCustomLinks)
            {
                listItemInfo.Url = listItemId;
                listItemInfo.AssociationLinkUrl = new Uri($"{listItemId.OriginalString}/$ref", listItemId.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
            }

            using (MemoryStream stream = new MemoryStream())
            {
                InMemoryMessage message = new InMemoryMessage { Stream = stream };
                using (ODataMessageWriter messageWriter = new ODataMessageWriter((IODataResponseMessage)message, settings, model))
                {
                    ODataWriter resourceWriter = messageWriter.CreateODataResourceWriter(driveItems, driveItemType);
                    resourceWriter.WriteStart(new ODataResource { Id = driveItemId });
                    resourceWriter.WriteStart(listItemInfo);
                    resourceWriter.WriteStart(new ODataResource { Id = listItemId });
                    resourceWriter.WriteStart(new ODataNestedResourceInfo { Name = "fields", IsCollection = false });
                    resourceWriter.WriteStart(new ODataResource { Id = fieldsId });
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.Flush();
                }

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        private static EdmModel CreateModel(
            string listItemNavigationName,
            EdmMultiplicity listItemMultiplicity,
            out EdmEntityType driveItemType,
            out EdmEntitySet driveItems)
        {
            EdmModel model = new EdmModel();
            driveItemType = model.AddEntityType("Microsoft.Graph", "DriveItem");
            driveItemType.AddKeys(driveItemType.AddStructuralProperty("id", EdmPrimitiveTypeKind.String));
            EdmEntityType listItemType = model.AddEntityType("Microsoft.Graph", "ListItem");
            listItemType.AddKeys(listItemType.AddStructuralProperty("id", EdmPrimitiveTypeKind.String));
            EdmEntityType fieldsType = model.AddEntityType("Microsoft.Graph", "FieldValueSet");
            fieldsType.AddKeys(fieldsType.AddStructuralProperty("id", EdmPrimitiveTypeKind.String));

            driveItemType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = listItemNavigationName,
                    Target = listItemType,
                    TargetMultiplicity = listItemMultiplicity,
                    ContainsTarget = true
                });
            listItemType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "fields",
                    Target = fieldsType,
                    TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                    ContainsTarget = true
                });

            EdmEntityContainer container = model.AddEntityContainer("Microsoft.Graph", "Container");
            driveItems = container.AddEntitySet("driveItems", driveItemType);
            return model;
        }

        private static ODataMessageWriterSettings CreateSettings(EdmModel model, Uri serviceRoot, Uri requestUri)
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings
            {
                ODataUri = new ODataUri
                {
                    RequestUri = requestUri,
                    ServiceRoot = serviceRoot,
                    Path = new ODataUriParser(model, serviceRoot, requestUri).ParsePath()
                },
                EnableMessageStreamDisposal = false
            };
            settings.SetContentType("application/json;odata.metadata=full", null);
            return settings;
        }

        private static Uri CreateResourceId(Uri serviceRoot, string path, bool absolute)
        {
            return absolute ? new Uri(serviceRoot, path) : new Uri(path, UriKind.Relative);
        }

        private static ODataPath GetCurrentPath(ODataWriter writer)
        {
            PropertyInfo currentScopeProperty = typeof(ODataWriterCore).GetProperty(
                "CurrentScope",
                BindingFlags.Instance | BindingFlags.NonPublic);
            ODataWriterCore.Scope currentScope = (ODataWriterCore.Scope)currentScopeProperty.GetValue(writer);
            return currentScope.ODataUri.Path;
        }
    }
}
