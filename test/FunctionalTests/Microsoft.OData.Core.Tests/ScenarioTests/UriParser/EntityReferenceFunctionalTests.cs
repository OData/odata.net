//---------------------------------------------------------------------
// <copyright file="EntityReferenceFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.UriParser;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
{
    /// <summary>
    /// Tests to check working of Entity Reference ($ref) 
    /// </summary>
    public class EntityReferenceFunctionalTests
    {
        [Fact]
        public void UseMultipleEscapeSequencesWithRefInKeyAsSegment()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/$/$/People/1/$/$/MyDog/$/$/MyPeople/$/$/$ref/$/$")) { UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash }.ParsePath();
            path.LastSegment.ShouldBeNavigationPropertyLinkSegment(HardCodedTestModel.GetDogMyPeopleNavProp());
        }

        [Fact]
        public void KeyLookupCannotAppearAfterCountAfterEntityReference()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs(1)/MyPeople/$ref/$count(1)", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [Fact]
        public void KeyLookupCannotAppearAfterEntityReference()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs(1)/$ref(1)", ODataErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [Fact]
        public void CountCannotAppearAfterEntityReferenceCollectionProperties()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs(1)/MyPeople/$ref/$count", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [Fact]
        public void EntityReferenceCannotAppearAfterAnEntitySet()
        {
            // TODO: We can improve error message drastically when we refactor path parsing
            PathFunctionalTestsUtil.RunParseErrorPath("People/$ref", ODataErrorStrings.PathParser_EntityReferenceNotSupported("People"));
        }

        [Fact]
        public void EntityReferenceCannotAppearAfterAComplexProperty()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/MyAddress/$ref", ODataErrorStrings.PathParser_EntityReferenceNotSupported("MyAddress"));
        }

        [Fact]
        public void ValidNavigationPropertyBeforeEntityReference()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(7)/MyDog/$ref");
            VerificationHelpers.VerifyPath(path, new Action<ODataPathSegment>[]
            {
                s => s.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet()),
                s => s.ShouldBeSimpleKeySegment(7),
                s => s.ShouldBeNavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp()),
            });
        }


        [Fact]
        public void KeyOnCollectionEntityReferencesShouldWork()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Dogs(7)/MyPeople(2)/$ref");
            VerificationHelpers.VerifyPath(path, new Action<ODataPathSegment>[]
            {
                s => s.ShouldBeEntitySetSegment(HardCodedTestModel.GetDogsSet()),
                s => s.ShouldBeSimpleKeySegment(7),
                s => s.ShouldBeNavigationPropertyLinkSegment(HardCodedTestModel.GetDogMyPeopleNavProp()),
                s => s.ShouldBeSimpleKeySegment(2)
            });
        }

        [Fact]
        public void CannotGoToPropetyOnEntityReference()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(7)/MyDog/$ref/Color", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [Fact]
        public void ParsePathWithLinks()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://www.odata.com/OData"), new Uri("http://www.odata.com/OData/People(1)/MyDog/$ref"));
            ODataUri parsedUri = parser.ParseUri();
            List<ODataPathSegment> path = parsedUri.Path.ToList();
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBeNavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp());
        }

    }
}