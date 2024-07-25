//---------------------------------------------------------------------
// <copyright file="ODataPathExtensionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using Microsoft.OData.UriParser;
using Xunit;
using Helpers = Microsoft.OData.Tests.UriParser.ModelBuildingHelpers;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class ODataPathExtensionsTests
    {
        private readonly Uri testBaseUri = new Uri("http://odatatest/");

        [Fact]
        public void TypeComputedForEntitySetSegment()
        {
            IEdmEntitySet entitySet = Helpers.BuildValidEntitySet();
            var path = new ODataPath(new ODataPathSegment[]
            {
                new EntitySetSegment(entitySet)
            });

            IEdmType entitySetCollection = new EdmCollectionType(new EdmEntityTypeReference(entitySet.EntityType, false));
            Assert.True(path.EdmType().IsEquivalentTo(entitySetCollection.ToTypeReference()));
        }

        [Fact]
        public void EntitySetComputedForEntitySetSegment()
        {
            var entitySet = Helpers.BuildValidEntitySet();
            var path = new ODataPath(new ODataPathSegment[]
            {
                new EntitySetSegment(entitySet)
            });

            Assert.Same(entitySet, path.NavigationSource());
        }

        [Fact]
        public void PathExtensionsIsCollectionWithEntitySetReturnsTrue()
        {
            var entitySet = Helpers.BuildValidEntitySet();
            var path = new ODataPath(new ODataPathSegment[]
            {
                new EntitySetSegment(entitySet)
            });

            Assert.True(path.IsCollection());
        }

        [Fact]
        public void PathExtensionsIsCollectionWithPropertyReturnsFalse()
        {
            var property = Helpers.BuildValidPrimitiveProperty();
            var path = new ODataPath(new ODataPathSegment[]
            {
                new PropertySegment(property)
            });

            Assert.False(path.IsCollection());
        }

        [Fact]
        public void PathExtensionsToExpandPathWithNonNavigationPropertyThrows()
        {
            var property = Helpers.BuildValidPrimitiveProperty();
            ODataSelectPath path = new ODataSelectPath(
                new ODataPathSegment[]
                {
                    new PropertySegment(HardCodedTestModel.GetPersonNameProp())
                }
            );

            Action expandPathAction = () => path.ToExpandPath();
            expandPathAction.Throws<ODataException>(ODataErrorStrings.ODataExpandPath_LastSegmentMustBeNavigationPropertyOrTypeSegment);
        }

        [Fact]
        public void PathExtensionsToExpandPathWithNavigationPropertyReturnsExpandedPath()
        {
            var property = Helpers.BuildValidPrimitiveProperty();
            ODataSelectPath path = new ODataSelectPath(
                new ODataPathSegment[]
                {
                    new NavigationPropertySegment(
                        HardCodedTestModel.GetPersonMyDogNavProp(), HardCodedTestModel.GetDogsSet())
                }
            );

            Assert.Equal(1, path.ToExpandPath().Count);
            Assert.Equal("MyDog", path.ToExpandPath().FirstSegment.Identifier);
        }

        [Fact]
        public void TestResourcePath()
        {
            string[] testCases = new string[]
            {
                //  EntityType
                "People",
                "People(1)",
                "People(2)/Name",
                "People(3)/Fully.Qualified.Namespace.Employee",
                "People(4)/Fully.Qualified.Namespace.Employee/Name",

                // Open EntityType
                "Paintings",
                "Paintings(0)/IAmOpenProperty",

                // EntityType With Multi keys
                "Lions(ID1=1,ID2=2)",
                "Lions(ID1=1,ID2=2)/AngerLevel",
                "Lions(ID1=1,ID2=2)/Fully.Qualified.Namespace.Lion",
                "Lions(ID1=1,ID2=2)/Fully.Qualified.Namespace.Lion/AngerLevel",
                
                // Singleton
                "Boss",
                "Boss/Name",
                "Boss/Fully.Qualified.Namespace.Person",
                "Boss/Fully.Qualified.Namespace.Person/Name",

                // Containment
                "People(1)/MyContainedDog",
                "People(2)/MyContainedChimeras",
                "People(3)/MyContainedChimeras(Rid=1,Gid=00000000-0000-0000-0000-000000000001,Name='Chi1',Upgraded=true)",
                "People(4)/Fully.Qualified.Namespace.Manager/MyContainedChimeras(Rid=1,Gid=00000000-0000-0000-0000-000000000002,Name='Chi1',Upgraded=true)",
                "People(4)/Fully.Qualified.Namespace.Manager/MyContainedChimeras(Rid=2,Gid=00000000-0000-0000-0000-000000000003,Name='Chi7',Upgraded=false)/Fully.Qualified.Namespace.Chimera/Level",
                "People(1)/Fully.Qualified.Namespace.Manager/DirectReports/Fully.Qualified.Namespace.Person",
                "People(1)/Fully.Qualified.Namespace.Manager/DirectReports(3)/MyContainedDog",
                "People(1)/Fully.Qualified.Namespace.Manager/DirectReports(3)/MyContainedDog/Nicknames",

                // Complex Type
                "People(1)/MyAddress",
                "People(1)/MyAddress/Fully.Qualified.Namespace.Address",
                "People(1)/MyAddress/City",
                "People(1)/MyAddress/Fully.Qualified.Namespace.Address/City",
                "People(1)/MyAddress/Fully.Qualified.Namespace.Address/NextHome/Fully.Qualified.Namespace.Address",
            };

            foreach (var testCase in testCases)
            {
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, this.testBaseUri, new Uri(this.testBaseUri, testCase));
                ODataPath path = parser.ParsePath();
                string result = path.ToResourcePathString(ODataUrlKeyDelimiter.Parentheses);
                Assert.Equal(testCase, result);
            }
        }

        [Fact]
        public void TestTrimEndingKey()
        {
            var testCases = new[]
            {
                new {
                    Query = "",
                    Result = ""
                },
                new {
                    Query = "People(1)",
                    Result = "People"
                },
                 new {
                    Query = "People",
                    Result = "People"
                },
                new {
                    Query = "People(1)/Fully.Qualified.Namespace.Person",
                    Result = "People/Fully.Qualified.Namespace.Person"
                },
                new {
                    Query = "People/Fully.Qualified.Namespace.Person",
                    Result = "People/Fully.Qualified.Namespace.Person"
                },
                // Having consecutive type segments is not useful, but ODataPath currently supports it.
                // The test case helps avoid accidental regressions.
                new {
                    Query = "People(1)/Fully.Qualified.Namespace.Person/Fully.Qualified.Namespace.Person",
                    Result = "People/Fully.Qualified.Namespace.Person/Fully.Qualified.Namespace.Person"
                },
                new
                {
                    Query = "Lions(ID1=1,ID2=2)",
                    Result = "Lions"
                },
                new
                {
                    Query = "People(4)/Fully.Qualified.Namespace.Manager/MyContainedChimeras(Rid=1,Gid=00000000-0000-0000-0000-000000000002,Name='Chi1',Upgraded=true)",
                    Result = "People(4)/Fully.Qualified.Namespace.Manager/MyContainedChimeras"
                },
            };

            foreach (var testCase in testCases)
            {
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, this.testBaseUri, new Uri(this.testBaseUri, testCase.Query));
                ODataPath path = parser.ParsePath();
                string originalPath = path.ToResourcePathString(ODataUrlKeyDelimiter.Parentheses);
                string result = path.TrimEndingKeySegment().ToResourcePathString(ODataUrlKeyDelimiter.Parentheses);
                Assert.Equal(testCase.Result, result);
                Assert.Equal(originalPath, path.ToResourcePathString(ODataUrlKeyDelimiter.Parentheses));
            }
        }

        [Fact]
        public void TestTrimEndingTypeCast()
        {
            var testCases = new[]
            {
                new {
                    Query= "People(1)/MyAddress/Fully.Qualified.Namespace.Address/NextHome",
                    TypeCast = "Fully.Qualified.Namespace.Address"
                },
                new
                {
                    Query = "People",
                    TypeCast = ""
                },
                new
                {
                    Query = "People",
                    TypeCast = "Fully.Qualified.Namespace.Employee"
                },
                new
                {
                    Query = "People",
                    TypeCast = "Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.Employee"
                },
                new
                {
                    Query = "Lions(ID1=1,ID2=2)",
                    TypeCast = "Fully.Qualified.Namespace.Lion"
                },
                new
                {
                    Query = "Boss",
                    TypeCast = "Fully.Qualified.Namespace.Person"
                },
                new
                {
                    Query = "Boss/Fully.Qualified.Namespace.Person/Name",
                    TypeCast = ""
                },
                new
                {
                    Query = "People(4)/Fully.Qualified.Namespace.Manager/MyContainedChimeras(Rid=1,Gid=00000000-0000-0000-0000-000000000002,Name='Chi1',Upgraded=true)",
                    TypeCast = "Fully.Qualified.Namespace.Chimera"
                },
                new
                {
                    Query = "People(1)/MyAddress/Fully.Qualified.Namespace.Address/NextHome",
                    TypeCast = "Fully.Qualified.Namespace.Address"
                },
                new
                {
                    Query = "",
                    TypeCast = ""
                }
            };

            foreach (var testCase in testCases)
            {
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, this.testBaseUri, new Uri(this.testBaseUri, testCase.Query + "/" + testCase.TypeCast));
                ODataPath path = parser.ParsePath();
                string originalPath = path.ToResourcePathString(ODataUrlKeyDelimiter.Parentheses);
                string result = path.TrimEndingTypeSegment().ToResourcePathString(ODataUrlKeyDelimiter.Parentheses);
                Assert.Equal(testCase.Query, result);
                Assert.Equal(originalPath, path.ToResourcePathString(ODataUrlKeyDelimiter.Parentheses));
            }
        }

        /// <summary>
        /// Test trimming the end of a path of it's key and type segments
        /// </summary>
        [Fact]
        public void TrimEndingTypeAndKeySegments()
        {
            var testCases = new[]
            {
                new {
                    Url = "",
                    Trimmed = ""
                },
                new {
                    Url = "People",
                    Trimmed = "People"
                },
                new {
                    Url = "People(1)",
                    Trimmed = "People"
                },
                new {
                    Url = "People/Fully.Qualified.Namespace.Employee",
                    Trimmed = "People"
                },
                new {
                    Url = "People(1)/Fully.Qualified.Namespace.Employee",
                    Trimmed = "People"
                },
                new {
                    Url = "People(1)/Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.Employee",
                    Trimmed = "People"
                },
                new {
                    Url = "People/Fully.Qualified.Namespace.Employee/1",
                    Trimmed = "People"
                },
                new {
                    Url = "People/Fully.Qualified.Namespace.Employee/1/Fully.Qualified.Namespace.Employee",
                    Trimmed = "People"
                },
                new {
                    Url = "People/Fully.Qualified.Namespace.Employee/1/MyAddress",
                    Trimmed = "People/Fully.Qualified.Namespace.Employee/1/MyAddress"
                },
                new {
                    Url = "People(1)/Fully.Qualified.Namespace.Employee/MyAddress",
                    Trimmed = "People/1/Fully.Qualified.Namespace.Employee/MyAddress"
                },
            };

            foreach (var testCase in testCases)
            {
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, this.testBaseUri, new Uri(this.testBaseUri, testCase.Url));
                ODataPath path = parser.ParsePath();
                var result = path.TrimEndingTypeAndKeySegments();
                Assert.Equal(testCase.Trimmed, result.ToResourcePathString(ODataUrlKeyDelimiter.Slash));
            }
        }

        [Fact]
        public void TestIsIndividualProperty()
        {
            string[] trueCases = new string[]
            {
                //  EntityType
                "People(2)/Name",
                "People(4)/Fully.Qualified.Namespace.Employee/Name",

                // Open EntityType
                "Paintings(0)/IAmOpenProperty",

                // EntityType With Multi keys
                "Lions(ID1=1,ID2=2)/AngerLevel",
                "Lions(ID1=1,ID2=2)/Fully.Qualified.Namespace.Lion/AngerLevel",
                
                // Singleton
                "Boss/Name",
                "Boss/Fully.Qualified.Namespace.Person/Name",

                // Containment
                "People(4)/Fully.Qualified.Namespace.Manager/MyContainedChimeras(Rid=2,Gid=00000000-0000-0000-0000-000000000003,Name='Chi7',Upgraded=false)/Fully.Qualified.Namespace.Chimera/Level",
                "People(1)/Fully.Qualified.Namespace.Manager/DirectReports(3)/MyContainedDog/Nicknames",

                // Complex Type
                "People(1)/MyAddress",
                "People(1)/MyAddress/Fully.Qualified.Namespace.Address",
                "People(1)/MyAddress/City",
                "People(1)/MyAddress/Fully.Qualified.Namespace.Address/City",
                "People(1)/MyAddress/Fully.Qualified.Namespace.Address/NextHome/Fully.Qualified.Namespace.Address",
            };

            string[] falseCases = new string[]
            {
                //  EntityType
                "People",
                "People(1)",
                "People(3)/Fully.Qualified.Namespace.Employee",

                // Open EntityType
                "Paintings",

                // EntityType With Multi keys
                "Lions(ID1=1,ID2=2)",
                "Lions(ID1=1,ID2=2)/Fully.Qualified.Namespace.Lion",
                
                // Singleton
                "Boss",
                "Boss/Fully.Qualified.Namespace.Person",

                // Containment
                "People(1)/MyContainedDog",
                "People(2)/MyContainedChimeras",
                "People(3)/MyContainedChimeras(Rid=1,Gid=00000000-0000-0000-0000-000000000001,Name='Chi1',Upgraded=true)",
                "People(4)/Fully.Qualified.Namespace.Manager/MyContainedChimeras(Rid=1,Gid=00000000-0000-0000-0000-000000000002,Name='Chi1',Upgraded=true)",
                "People(1)/Fully.Qualified.Namespace.Manager/DirectReports(3)/MyContainedDog",
            };

            foreach (var testCase in trueCases)
            {
                ODataPath path = new ODataUriParser(HardCodedTestModel.TestModel, this.testBaseUri, new Uri(this.testBaseUri, testCase)).ParsePath();
                bool result = path.IsIndividualProperty();
                Assert.True(result, string.Format("Resource path \"{0}\" should target at individual property", testCase));
            }

            foreach (var testCase in falseCases)
            {
                ODataPath path = new ODataUriParser(HardCodedTestModel.TestModel, this.testBaseUri, new Uri(this.testBaseUri, testCase)).ParsePath();
                bool result = path.IsIndividualProperty();
                Assert.False(result, string.Format("Resource path \"{0}\" should not target at individual property", testCase));
            }
        }

        [Theory]
        [InlineData("", 1, "(1)")]
        [InlineData("People", 1, "People(1)")]
        [InlineData("People/Fully.Qualified.Namespace.Manager", 1, "People(1)/Fully.Qualified.Namespace.Manager")]
        // Having consecutive type segments is not useful, but ODataPath currently supports it.
        // The test case helps avoid accidental regressions.
        [InlineData("People/Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.Manager", 1, "People(1)/Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.Manager")]
        [InlineData("People(1)/Fully.Qualified.Namespace.Manager/DirectReports", 3, "People(1)/Fully.Qualified.Namespace.Manager/DirectReports(3)")]
        [InlineData("People(1)/Fully.Qualified.Namespace.Manager/DirectReports/Fully.Qualified.Namespace.Manager", 3, "People(1)/Fully.Qualified.Namespace.Manager/DirectReports(3)/Fully.Qualified.Namespace.Manager")]
        [InlineData("People(1)/Fully.Qualified.Namespace.Manager/DirectReports/Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.Manager", 3, "People(1)/Fully.Qualified.Namespace.Manager/DirectReports(3)/Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.Manager")]
        public void TestAddKeySegment(string initialPath, int id,  string expectedPath)
        {
            ODataPath odataPath = new ODataUriParser(HardCodedTestModel.TestModel, this.testBaseUri, new Uri(this.testBaseUri, initialPath)).ParsePath();
            var keySegment = new KeySegment(new[] { new KeyValuePair<string, object>("Id", id) }, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            var newODataPath = odataPath.AddKeySegment(keySegment);

            Assert.Equal(odataPath.Count + 1, newODataPath.Count);
            Assert.Equal(expectedPath, newODataPath.ToResourcePathString(ODataUrlKeyDelimiter.Parentheses));
        }

        [Fact]
        public void TestAddKeySegmentThrowsIfSegmentNull()
        {
            ODataPath odataPath = new ODataUriParser(HardCodedTestModel.TestModel, this.testBaseUri, new Uri(this.testBaseUri, "People")).ParsePath();
            Assert.Throws<ArgumentNullException>(() => odataPath.AddKeySegment(null));
        }

        [Fact]
        public void TestAddSegment()
        {
            ODataPath odataPath = new ODataUriParser(HardCodedTestModel.TestModel, this.testBaseUri, new Uri(this.testBaseUri, "People(1)")).ParsePath();
            var newSegment = new PropertySegment(HardCodedTestModel.GetPersonAddressProp());
            var newPath = odataPath.AddSegment(newSegment);

            Assert.Equal(odataPath.Count + 1, newPath.Count);
            Assert.Equal("People(1)/MyAddress", newPath.ToResourcePathString(ODataUrlKeyDelimiter.Parentheses));
        }

        [Fact]
        public void TestAddSegmentThrowsIfSegmentNull()
        {
            ODataPath odataPath = new ODataUriParser(HardCodedTestModel.TestModel, this.testBaseUri, new Uri(this.testBaseUri, "People")).ParsePath();
            Assert.Throws<ArgumentNullException>(() => odataPath.AddSegment(null));
        }
    }
}
