//---------------------------------------------------------------------
// <copyright file="ODataPathInfoTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser
{
    public class ODataPathInfoTests
    {
        private readonly EdmEntityType personEntityType;
        private readonly EdmNavigationProperty friendsNavigationProperty;
        private readonly EdmEntitySet peopleEntitySet;

        public ODataPathInfoTests()
        {
            var model = new EdmModel();
            this.personEntityType = model.AddEntityType("NS", "Person");
            this.personEntityType.AddKeys(personEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, isNullable: false));

            this.friendsNavigationProperty = this.personEntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "Friends",
                Target = this.personEntityType,
                TargetMultiplicity = EdmMultiplicity.Many
            });

            var entityContainer = model.AddEntityContainer("NS", "Default");
            this.peopleEntitySet = entityContainer.AddEntitySet("People", personEntityType);
        }

        [Fact]
        public void TestODataPathInfo()
        {
            var odataPathInfo = new ODataPathInfo(this.personEntityType, this.peopleEntitySet);

            Assert.Equal(this.peopleEntitySet, odataPathInfo.TargetNavigationSource);
            Assert.Equal(this.personEntityType, odataPathInfo.TargetEdmType);
            Assert.Equal(this.personEntityType, odataPathInfo.TargetStructuredType);
            Assert.Empty(odataPathInfo.Segments);
        }

        [Fact]
        public void TestODataPathInfoForEmptySegmentsInODataPath()
        {
            var odataPathInfo = new ODataPathInfo(new ODataPath());

            Assert.Null(odataPathInfo.TargetNavigationSource);
            Assert.Null(odataPathInfo.TargetEdmType);
            Assert.Null(odataPathInfo.TargetStructuredType);
            Assert.Empty(odataPathInfo.Segments);
        }

        [Fact]
        public void TestODataPathInfoForEntitySetSegmentInODataPath()
        {
            var odataPath = new ODataPath(new EntitySetSegment(this.peopleEntitySet));
            var odataPathInfo = new ODataPathInfo(odataPath);

            Assert.Equal(this.peopleEntitySet, odataPathInfo.TargetNavigationSource);
            Assert.Equal(this.personEntityType, odataPathInfo.TargetEdmType);
            Assert.Equal(this.personEntityType, odataPathInfo.TargetStructuredType);
            Assert.Single(odataPathInfo.Segments);
        }

        [Fact]
        public void TestODataPathInfoForKeySegmentInODataPath()
        {
            var odataPath = new ODataPath(
                new EntitySetSegment(this.peopleEntitySet),
                new KeySegment(new[] { new KeyValuePair<string, object>("Id", 1) }, this.personEntityType, this.peopleEntitySet));
            var odataPathInfo = new ODataPathInfo(odataPath);

            Assert.Equal(this.peopleEntitySet, odataPathInfo.TargetNavigationSource);
            Assert.Equal(this.personEntityType, odataPathInfo.TargetEdmType);
            Assert.Equal(this.personEntityType, odataPathInfo.TargetStructuredType);
            Assert.Equal(2, odataPathInfo.Segments.Count());
        }

        [Fact]
        public void TestODataPathInfoForNavigationPropertySegmentInODataPath()
        {
            var odataPath = new ODataPath(
                new EntitySetSegment(this.peopleEntitySet),
                new KeySegment(new[] { new KeyValuePair<string, object>("Id", 1) }, this.personEntityType, this.peopleEntitySet),
                new NavigationPropertySegment(this.friendsNavigationProperty, this.peopleEntitySet));
            var odataPathInfo = new ODataPathInfo(odataPath);

            Assert.Equal(this.peopleEntitySet, odataPathInfo.TargetNavigationSource);
            Assert.Equal(this.personEntityType, odataPathInfo.TargetEdmType);
            Assert.Equal(this.personEntityType, odataPathInfo.TargetStructuredType);
            Assert.Equal(3, odataPathInfo.Segments.Count());
        }
    }
}
