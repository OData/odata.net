//---------------------------------------------------------------------
// <copyright file="ODataUriParserInjectionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Tests.UriParser.Binders;
using Microsoft.OData.UriParser;
using Microsoft.Test.OData.DependencyInjection;
using Xunit;

namespace Microsoft.OData.Tests.UriParser
{
    public class ODataUriParserInjectionTests
    {
        private readonly Uri ServiceRoot = new Uri("https://serviceRoot/");

        #region model
        private const string oneDriveModelString = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""oneDrive"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""drive"">
        <Key>
          <PropertyRef Name=""id"" />
        </Key>
        <Property Name=""id"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""owner"" Type=""oneDrive.identitySet"" />
        <NavigationProperty Name=""items"" Type=""Collection(oneDrive.item)"" ContainsTarget=""true"" />
      </EntityType>
      <EntityType Name=""specialItem"" OpenType=""true"" BaseType=""oneDrive.item"">
        <Property Name=""note"" Type=""Edm.String"" Nullable=""false"" />
      </EntityType>
      <EntityType Name=""item"" OpenType=""true"">
        <Key>
          <PropertyRef Name=""id"" />
        </Key>
        <Property Name=""content"" Type=""Edm.Stream"" />
        <Property Name=""createdBy"" Type=""oneDrive.identitySet"" />
        <Property Name=""id"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""parentReference"" Type=""oneDrive.itemReference"" />
        <Property Name=""size"" Type=""Edm.Int64"" />
        <Property Name=""folder"" Type=""oneDrive.folder"" />
        <NavigationProperty Name=""children"" Type=""Collection(oneDrive.item)"" ContainsTarget=""true"" />
        <NavigationProperty Name=""thumbnails"" Type=""Collection(oneDrive.thumbnailSet)"" ContainsTarget=""true"" />
      </EntityType>
      <ComplexType Name=""chunkedUploadSessionDescriptor"">
        <Property Name=""name"" Type=""Edm.String""/>
      </ComplexType>
      <ComplexType Name=""itemReference"">
        <Property Name=""driveId"" Type=""Edm.String"" />
        <Property Name=""id"" Type=""Edm.String"" />
        <Property Name=""path"" Type=""Edm.String"" />
      </ComplexType>
      <ComplexType Name=""uploadSession"">
        <Property Name=""uploadUrl"" Type=""Edm.String"" />
        <Property Name=""expirationDateTime"" Type=""Edm.DateTimeOffset"" />
        <Property Name=""nextExpectedRanges"" Type=""Collection(Edm.String)"" />
      </ComplexType>
      <ComplexType Name=""folder"">
        <Property Name=""childCount"" Type=""Edm.Int32"" />
      </ComplexType>
      <ComplexType Name=""identitySet"" OpenType=""true"">
        <Property Name=""application"" Type=""oneDrive.identity"" />
        <Property Name=""user"" Type=""oneDrive.identity"" />
      </ComplexType>
      <ComplexType Name=""identity"">
        <Property Name=""displayName"" Type=""Edm.String"" />
        <Property Name=""id"" Type=""Edm.String"" />
      </ComplexType>
      <EntityType Name=""thumbnailSet"" OpenType=""true"">
        <Key>
          <PropertyRef Name=""id"" />
        </Key>
        <Property Name=""id"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""large"" Type=""oneDrive.thumbnail"" />
      </EntityType>
      <ComplexType Name=""thumbnail"">
        <Property Name=""content"" Type=""Edm.Stream"" />
        <Property Name=""url"" Type=""Edm.String"" />
      </ComplexType>
      <EntityContainer Name=""oneDrive"">
        <Singleton Name=""drive"" Type=""oneDrive.drive"" />
        <EntitySet Name=""drives"" EntityType=""oneDrive.drive"" />
      </EntityContainer>
    </Schema>
    <Schema Namespace=""view"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <Function Name=""delta"" IsBound=""true"">
        <Parameter Name=""bindingParameter"" Type=""oneDrive.item"" Nullable=""false"" />
        <ReturnType Type=""Collection(oneDrive.item)"" Nullable=""false"" />
      </Function>
      <Function Name=""recent"" IsBound=""true"" IsComposable=""true"">
        <Parameter Name=""bindingParameter"" Nullable=""false"" Type=""oneDrive.drive""/>
        <ReturnType Nullable=""false"" Type=""Collection(oneDrive.item)""/>
      </Function>
      <Function Name=""search"" IsBound=""true"">
        <Parameter Name=""bindingParameter"" Type=""oneDrive.item"" Nullable=""false"" />
        <ReturnType Type=""Collection(oneDrive.item)"" Nullable=""false"" />
      </Function>
      <Function Name=""sharedWithMe"" IsBound=""true"">
        <Parameter Name=""bindingParameter"" Nullable=""false"" Type=""oneDrive.drive""/>
        <ReturnType Nullable=""false"" Type=""Collection(oneDrive.item)""/>
      </Function>
      <Function Name=""sharedWithMe"" IsBound=""true"">
        <Parameter Name=""bindingParameter"" Nullable=""false"" Type=""Collection(oneDrive.item)""/>
        <ReturnType Nullable=""false"" Type=""Collection(oneDrive.item)""/>
      </Function>
    </Schema>
    <Schema Namespace=""action"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <Action Name=""copy"" IsBound=""true"">
        <Parameter Name=""bindingParameter"" Type=""oneDrive.item"" Nullable=""false"" />
        <Parameter Name=""parentReference"" Type=""oneDrive.itemReference"" />
        <Parameter Name=""name"" Type=""Edm.String"" />
        <ReturnType Type=""oneDrive.item"" Nullable=""false"" />
      </Action>
    </Schema>
    <Schema Namespace=""upload"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <Action Name=""createSession"" IsBound=""true"">
        <Parameter Name=""bindingParameter"" Type=""oneDrive.item"" Nullable=""false"" />
        <Parameter Name=""item"" Type=""oneDrive.chunkedUploadSessionDescriptor"" />
        <ReturnType Type=""oneDrive.uploadSession"" Nullable=""false"" />
      </Action>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        #endregion

        IEdmModel oneDriveModel = CsdlReader.Parse(XmlReader.Create(new StringReader(oneDriveModelString)));
        IEdmComplexType folderType;
        IEdmEntityType itemType;
        IEdmEntityType specialItemType;
        IEdmEntityType driveType;
        IEdmEntitySet drivesEntitySet;
        IEdmSingleton driveSingleton;
        IEdmStructuralProperty folderProp;
        IEdmStructuralProperty sizeProp;
        IEdmNavigationProperty itemsNavProp;
        IEdmNavigationProperty childrenNavProp;
        IEdmNavigationProperty thumbnailsNavProp;
        IEdmNavigationSource containedItemsNav;
        IEdmNavigationSource containedthumbnailsNav;
        IEdmOperation copyOp;
        IEdmOperation searchOp;
        IEdmOperation shareItemBindCollectionOp;

        public ODataUriParserInjectionTests()
        {
            folderType = oneDriveModel.SchemaElements.OfType<IEdmComplexType>().Single(e => string.Equals(e.Name, "folder"));
            itemType = oneDriveModel.SchemaElements.OfType<IEdmEntityType>().Single(e => string.Equals(e.Name, "item"));
            specialItemType = oneDriveModel.SchemaElements.OfType<IEdmEntityType>().Single(e => string.Equals(e.Name, "specialItem"));
            folderProp = itemType.FindProperty("folder") as IEdmStructuralProperty;
            sizeProp = itemType.FindProperty("size") as IEdmStructuralProperty;
            driveType = oneDriveModel.SchemaElements.OfType<IEdmEntityType>().Single(e => string.Equals(e.Name, "drive"));
            itemsNavProp = driveType.DeclaredNavigationProperties().FirstOrDefault(p => p.Name == "items");
            childrenNavProp = itemType.DeclaredNavigationProperties().FirstOrDefault(p => p.Name == "children");
            thumbnailsNavProp = itemType.DeclaredNavigationProperties().FirstOrDefault(p => p.Name == "thumbnails");
            drivesEntitySet = oneDriveModel.EntityContainer.FindEntitySet("drives");
            driveSingleton = oneDriveModel.EntityContainer.FindSingleton("drive");
            containedItemsNav = drivesEntitySet.FindNavigationTarget(itemsNavProp);
            containedthumbnailsNav = containedItemsNav.FindNavigationTarget(thumbnailsNavProp);
            copyOp = oneDriveModel.SchemaElements.OfType<IEdmOperation>().FirstOrDefault(o => o.Name == "copy");
            searchOp = oneDriveModel.SchemaElements.OfType<IEdmOperation>().FirstOrDefault(o => o.Name == "search");
            shareItemBindCollectionOp = oneDriveModel.SchemaElements.OfType<IEdmOperation>().LastOrDefault(o => o.Name == "sharedWithMe");
        }

        [Fact]
        public void CustomizedUriPathParserTest()
        {
            var container = ContainerBuilderHelper.BuildContainer(builder => builder.AddService<UriPathParser, MultipleSegmentUriPathParser>(ServiceLifetime.Scoped));
            Uri fullUri = new Uri("https://serviceRoot/drives('b!3195njZm9ECS0rQfW5QyZ0iJh-jL7uZGn60CTehSbIwT3VAIax8sRKiyg_aD0HNV'):/items('01VL3Q7L36JOJUAPXGDNAZ4FVIGCTMLL46')/folder/childCount");
            var uriParser = new ODataUriParser(oneDriveModel, ServiceRoot, fullUri, container);

            var path = uriParser.ParsePath();

            var childCountProp = folderType.FindProperty("childCount");
            path.LastSegment.ShouldBePropertySegment(childCountProp);
        }

        [Fact]
        public void ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_FollowedByDynamicPathSegmentsAndProp()
        {
            var container = ContainerBuilderHelper.BuildContainer(builder => builder.AddService<UriPathParser, MultipleSegmentUriPathParser>(ServiceLifetime.Scoped));
            Uri fullUri = new Uri("https://serviceRoot/drives('b!3195njZm9ECS0rQfW5QyZ0iJh-jL7uZGn60CTehSbIwT3VAIax8sRKiyg_aD0HNV')/root:/OData/Doc/OData%20Client%20for%20.NET.pptx:/folder/childCount");
            var uriParser = new ODataUriParser(oneDriveModel, ServiceRoot, fullUri, container);

            var childCountProp = folderType.FindProperty("childCount");
            uriParser.ParseDynamicPathSegmentFunc = (previous, identifier, parenthesisExpression) =>
                {
                    switch (identifier)
                    {
                        case "root":
                        case "OData":
                        case "Doc":
                        case "OData Client for .NET.pptx":
                            return new List<ODataPathSegment> { new DynamicPathSegment(identifier, itemType, containedItemsNav, true) };
                        default:
                            throw new Exception("Not supported Type");
                    }
                };

            var parseDynamicPathSegmentFuncClone = uriParser.ParseDynamicPathSegmentFunc;
            Assert.Equal(uriParser.ParseDynamicPathSegmentFunc, parseDynamicPathSegmentFuncClone);
            var path = uriParser.ParsePath();

            path.ElementAt(2).ShouldBeDynamicPathSegment("root");
            path.ElementAt(3).ShouldBeDynamicPathSegment("OData");
            path.ElementAt(4).ShouldBeDynamicPathSegment("Doc");
            path.ElementAt(5).ShouldBeDynamicPathSegment("OData Client for .NET.pptx");
            path.LastSegment.ShouldBePropertySegment(childCountProp);
        }

        [Fact]
        public void ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_FollowedByProp()
        {
            ODataPath odataPath;
            ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithOtherPaths("folder/childCount", out odataPath);

            odataPath.ElementAt(2).ShouldBeDynamicPathSegment("root:/OData/Doc/OData Client for .NET.pptx:");
            odataPath.ElementAt(3).ShouldBePropertySegment(folderProp);
            odataPath.LastSegment.ShouldBePropertySegment(folderType.FindProperty("childCount"));
        }

        [Fact]
        public void ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_FollowedByNavProp()
        {
            ODataPath odataPath;
            ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithOtherPaths("children", out odataPath);

            odataPath.LastSegment.ShouldBeNavigationPropertySegment(childrenNavProp);
        }

        [Fact]
        public void ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_FollowedByAction()
        {
            ODataPath odataPath;
            ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithOtherPaths("action.copy", out odataPath);

            odataPath.LastSegment.ShouldBeOperationSegment(copyOp);
        }

        [Fact]
        public void ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_FollowedByFunction()
        {
            ODataPath odataPath;
            ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithOtherPaths("view.search", out odataPath);

            odataPath.LastSegment.ShouldBeOperationSegment(searchOp);
        }

        [Fact]
        public void ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithExpand()
        {
            ODataPath odataPath;
            var uriParser = ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithQueryOption("$expand=thumbnails,children($select=size)", out odataPath);
            var expandClause = uriParser.ParseSelectAndExpand();
            var items = expandClause.SelectedItems;
            items.First().ShouldBeSelectedItemOfType<ExpandedNavigationSelectItem>()
                .And.PathToNavigationProperty.Single().ShouldBeNavigationPropertySegment(thumbnailsNavProp);
            items.Last().ShouldBeSelectedItemOfType<ExpandedNavigationSelectItem>()
                .And.SelectAndExpand.SelectedItems.Single().ShouldBePathSelectionItem(new ODataPath(new PropertySegment(sizeProp)));
        }

        [Fact]
        public void ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithSelect()
        {
            ODataPath odataPath;
            var uriParser = ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithQueryOption("$select=size,folder/childCount", out odataPath);
            var expandClause = uriParser.ParseSelectAndExpand();
            var items = expandClause.SelectedItems;
            items.First().ShouldBePathSelectionItem(new ODataPath(new PropertySegment(itemType.FindProperty("size") as IEdmStructuralProperty)));
            items.Last().ShouldBePathSelectionItem(new ODataPath(new PropertySegment(folderProp), new PropertySegment(folderType.FindProperty("childCount") as IEdmStructuralProperty)));
        }

        private ODataUriParser ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithQueryOption(string queryOptions, out ODataPath odataPath)
        {
            string originUriString = "https://serviceRoot/drives('b!3195njZm9ECS0rQfW5QyZ0iJh-jL7uZGn60CTehSbIwT3VAIax8sRKiyg_aD0HNV')/root:/OData/Doc/OData%20Client%20for%20.NET.pptx";
            Uri fullUri = new Uri(originUriString + "?" + queryOptions);

            return ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment(fullUri, out odataPath);
        }

        private ODataUriParser ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithOtherPaths(string path, out ODataPath odataPath)
        {
            string originUriString = "https://serviceRoot/drives('b!3195njZm9ECS0rQfW5QyZ0iJh-jL7uZGn60CTehSbIwT3VAIax8sRKiyg_aD0HNV')/root:/OData/Doc/OData%20Client%20for%20.NET.pptx";
            Uri fullUri = new Uri(originUriString + ":/" + path);

            return ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment(fullUri, out odataPath);
        }

        private ODataUriParser ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment(Uri fullUri, out ODataPath odataPath)
        {
            var container = ContainerBuilderHelper.BuildContainer(builder => builder.AddService<UriPathParser, SingleSegmentUriPathParser>(ServiceLifetime.Scoped));

            var uriParser = new ODataUriParser(oneDriveModel, ServiceRoot, fullUri, container);
            uriParser.ParseDynamicPathSegmentFunc = (previous, identifier, parenthesisExpression) =>
            {
                switch (identifier)
                {
                    case "root:/OData/Doc/OData Client for .NET.pptx":
                    case "root:/OData/Doc/OData Client for .NET.pptx:":
                        return new List<ODataPathSegment> { new DynamicPathSegment(identifier, itemType, containedItemsNav, true) };
                    default:
                        throw new Exception("Not supported Type");
                }
            };
            odataPath = uriParser.ParsePath();

            return uriParser;
        }

        [Fact]
        public void ParseDynamicPathSegmentFunc_ReturnNavAndSegment_WithProperty()
        {
            Uri fullUri = new Uri("https://serviceRoot/drive/root:/OData/Doc/OData%20Client%20for%20.NET.pptx:/size");

            var path = ParseDynamicPathSegmentFunc_ReturnNavAndSegment(fullUri);
            path.ElementAt(1).ShouldBeNavigationPropertySegment(itemsNavProp);
            path.ElementAt(2).ShouldBeKeySegment(new KeyValuePair<string, object>("id", "test"));
            path.LastSegment.ShouldBePropertySegment(sizeProp);
        }

        [Fact]
        public void ParseDynamicPathSegmentFunc_FirstSegmentReturnNavAndSegment_WithProperty()
        {
            Uri fullUri = new Uri("https://serviceRoot/customizedDrive/root:/OData/Doc/OData%20Client%20for%20.NET.pptx:/size");

            var path = ParseDynamicPathSegmentFunc_ReturnNavAndSegment(fullUri);
            path.First().ShouldBeSingletonSegment(driveSingleton);
            path.ElementAt(1).ShouldBeNavigationPropertySegment(itemsNavProp);
            path.ElementAt(2).ShouldBeKeySegment(new KeyValuePair<string, object>("id", "test"));
            path.LastSegment.ShouldBePropertySegment(sizeProp);
        }

        private ODataPath ParseDynamicPathSegmentFunc_ReturnNavAndSegment(Uri fullUri)
        {
            var container = ContainerBuilderHelper.BuildContainer(builder => builder.AddService<UriPathParser, SingleSegmentUriPathParser>(ServiceLifetime.Scoped));
            var uriParser = new ODataUriParser(oneDriveModel, ServiceRoot, fullUri, container);

            uriParser.ParseDynamicPathSegmentFunc = (previous, identifier, parenthesisExpression) =>
            {
                switch (identifier)
                {
                    case "customizedDrive":
                        return new List<ODataPathSegment>
                        {
                            new SingletonSegment(driveSingleton)
                            {
                                Identifier = identifier
                            }
                        };
                    case "root:/OData/Doc/OData Client for .NET.pptx:":
                        var navPropSeg = new NavigationPropertySegment(itemsNavProp, containedItemsNav);
                        var keySegment = new KeySegment(navPropSeg, new Dictionary<string, object>() { { "id", "test" } }, itemType, containedItemsNav);
                        return new List<ODataPathSegment>
                        {
                            navPropSeg,
                            keySegment
                        };
                    default:
                        throw new Exception("Not supported Type");
                }
            };

            return uriParser.ParsePath();
        }

        [Fact]
        public void ParseDynamicPathSegmentFunc_ReturnOperationSegment_WithCollectionReturnType_WithCount()
        {
            var container = ContainerBuilderHelper.BuildContainer(builder => builder.AddService<UriPathParser, SingleSegmentUriPathParser>(ServiceLifetime.Scoped));
            Uri fullUri = new Uri("https://serviceRoot/drives('b!3195njZm9ECS0rQfW5QyZ0iJh-jL7uZGn60CTehSbIwT3VAIax8sRKiyg_aD0HNV')/recent/$count");
            var uriParser = new ODataUriParser(oneDriveModel, ServiceRoot, fullUri, container);

            var operation = oneDriveModel.SchemaElements.OfType<IEdmOperation>().FirstOrDefault(o => o.Name == "recent");
            uriParser.ParseDynamicPathSegmentFunc = (previous, identifier, parenthesisExpression) =>
            {
                switch (identifier)
                {
                    case "recent":
                        var operationSegment = new OperationSegment(operation, containedItemsNav as IEdmEntitySetBase)
                        {
                            Identifier = identifier
                        };
                        return new List<ODataPathSegment>
                        {
                            operationSegment
                        };
                    default:
                        throw new Exception("Not supported Type");
                }
            };

            var path = uriParser.ParsePath();
            path.ElementAt(2).ShouldBeOperationSegment(operation);
            path.LastSegment.ShouldBeCountSegment();
        }

        [Fact]
        public void ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithCollectionReturnType_WithCount()
        {
            Uri fullUri = new Uri("https://serviceRoot/drive/recent/$count");

            var path = ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithCollectionReturnType(fullUri);
            path.ElementAt(1).ShouldBeDynamicPathSegment("recent");
            path.LastSegment.ShouldBeCountSegment();
        }

        [Fact]
        public void ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithCollectionReturnType_WithTypeCast()
        {
            Uri fullUri = new Uri("https://serviceRoot/drive/recent/oneDrive.specialItem");

            var path = ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithCollectionReturnType(fullUri);
            path.ElementAt(1).ShouldBeDynamicPathSegment("recent");
            path.LastSegment.ShouldBeTypeSegment(new EdmCollectionType(new EdmEntityTypeReference(specialItemType, false)));
        }

        [Fact]
        public void ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithCollectionReturnType_WithBoundFunction()
        {
            Uri fullUri = new Uri("https://serviceRoot/drive/recent/view.sharedWithMe");
            var path = ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithCollectionReturnType(fullUri);

            path.ElementAt(1).ShouldBeDynamicPathSegment("recent");
            path.LastSegment.ShouldBeOperationSegment(shareItemBindCollectionOp);
        }

        [Fact]
        public void ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithCollectionReturnType_FollowedByKey()
        {
            Uri fullUri = new Uri("https://serviceRoot/drive/recent('01VL3Q7L36JOJUAPXGDNAZ4FVIGCTMLL46')/size");
            var path = ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithCollectionReturnType(fullUri);

            path.ElementAt(1).ShouldBeDynamicPathSegment("recent");
            path.ElementAt(2).ShouldBeKeySegment(new KeyValuePair<string, object>("id", "01VL3Q7L36JOJUAPXGDNAZ4FVIGCTMLL46"));
            path.LastSegment.ShouldBePropertySegment(sizeProp);
        }

        [Fact]
        public void ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithCollectionReturnType_FollowedByKey_KeyAsSegment()
        {
            Uri fullUri = new Uri("https://serviceRoot/drive/recent/01VL3Q7L36JOJUAPXGDNAZ4FVIGCTMLL46/size");
            var path = ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithCollectionReturnType(fullUri, ODataUrlKeyDelimiter.Slash);

            path.ElementAt(1).ShouldBeDynamicPathSegment("recent");
            path.ElementAt(2).ShouldBeKeySegment(new KeyValuePair<string, object>("id", "01VL3Q7L36JOJUAPXGDNAZ4FVIGCTMLL46"));
            path.LastSegment.ShouldBePropertySegment(sizeProp);
        }

        [Fact]
        public void ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithFilter()
        {
            Uri fullUri = new Uri("https://serviceRoot/drive/recent?$filter=folder/childCount eq 20");
            ODataPath odataPath;

            var uriParser = ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithCollectionReturnType(fullUri, out odataPath);
            var filterClause = uriParser.ParseFilter();
            var binaryNode = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And;
            binaryNode.Left.ShouldBeSingleValuePropertyAccessQueryNode(folderType.FindProperty("childCount"));
            binaryNode.Right.ShouldBeConstantQueryNode(20);
        }

        [Fact]
        public void ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithOrderBy()
        {
            Uri fullUri = new Uri("https://serviceRoot/drive/recent?$orderby=size");
            ODataPath odataPath;
            var uriParser = ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithCollectionReturnType(fullUri, out odataPath);
            uriParser.ParseOrderBy().Expression.ShouldBeSingleValuePropertyAccessQueryNode(sizeProp);
        }

        [Fact]
        public void ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithCount()
        {
            Uri fullUri = new Uri("https://serviceRoot/drive/recent?$count=true");
            ODataPath odataPath;
            var uriParser = ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithCollectionReturnType(fullUri, out odataPath);
            uriParser.ParseCount().Should().BeTrue();
        }

        private ODataPath ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithCollectionReturnType(Uri fullUri, ODataUrlKeyDelimiter uriConventions = null)
        {
            ODataPath odataPath;
            ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithCollectionReturnType(fullUri, out odataPath, uriConventions);
            return odataPath;
        }

        private ODataUriParser ParseDynamicPathSegmentFunc_ReturnDynamicPathSegment_WithCollectionReturnType(Uri fullUri, out ODataPath odataPath, ODataUrlKeyDelimiter uriConventions = null)
        {
            var container = ContainerBuilderHelper.BuildContainer(builder => builder.AddService<UriPathParser, SingleSegmentUriPathParser>(ServiceLifetime.Scoped));
            var uriParser = new ODataUriParser(oneDriveModel, ServiceRoot, fullUri, container);

            if (uriConventions != null)
            {
                uriParser.UrlKeyDelimiter = uriConventions;
            }

            var operation = oneDriveModel.SchemaElements.OfType<IEdmOperation>().FirstOrDefault(o => o.Name == "recent");
            uriParser.ParseDynamicPathSegmentFunc = (previous, identifier, parenthesisExpression) =>
            {
                var dynamicPathSeg = new DynamicPathSegment(identifier, operation.ReturnType.Definition, containedItemsNav, false);

                var segments = new List<ODataPathSegment>
                {
                    dynamicPathSeg
                };

                if (parenthesisExpression != null)
                {
                    segments.Add(new KeySegment(dynamicPathSeg, new Dictionary<string, object>() { { "id", parenthesisExpression.Trim('\'') } }, itemType, null));
                }

                return segments;
            };

            odataPath = uriParser.ParsePath();
            return uriParser;

        }
    }

    public enum UriPathParserType
    {
        SingleSegment,
        MultipleSegments,
    }

    public class SingleSegmentUriPathParser : CustomizedUriPathParser
    {
        public SingleSegmentUriPathParser(ODataUriParserSettings settings)
            : base(settings)
        {
            UriPathParserType = UriParser.UriPathParserType.SingleSegment;
        }
    }

    public class MultipleSegmentUriPathParser : CustomizedUriPathParser
    {
        public MultipleSegmentUriPathParser(ODataUriParserSettings settings)
            : base(settings)
        {
            UriPathParserType = UriParser.UriPathParserType.MultipleSegments;
        }
    }

    public class CustomizedUriPathParser : UriPathParser
    {
        public UriPathParserType UriPathParserType { get; set; }
        public CustomizedUriPathParser(ODataUriParserSettings settings)
            : base(settings)
        { }

        public override ICollection<string> ParsePathIntoSegments(Uri fullUri, Uri serviceBaseUri)
        {
            Uri uri = fullUri;
            int numberOfSegmentsToSkip = 0;

            numberOfSegmentsToSkip = serviceBaseUri.AbsolutePath.Split('/').Length - 1;
            string[] uriSegments = uri.AbsolutePath.Split('/');

            List<string> segments = new List<string>();
            List<string> tmpSegments = new List<string>();
            bool end = true;
            for (int i = numberOfSegmentsToSkip; i < uriSegments.Length; i++)
            {
                string segment = uriSegments[i];
                if (UriPathParserType == UriParser.UriPathParserType.SingleSegment)
                {
                    if (segment.EndsWith(":") || !end && i == uriSegments.Length - 1)
                    {
                        end = !end;
                        if (end)
                        {
                            tmpSegments.Add(segment);
                            segments.Add(Uri.UnescapeDataString(string.Join("/", tmpSegments)));
                            continue;
                        }
                    }

                    if (!end)
                    {
                        tmpSegments.Add(segment);
                        continue;
                    }
                }

                segments.Add(Uri.UnescapeDataString(segment.TrimEnd(':')));
            }

            return segments.ToArray();
        }
    }
}
