//---------------------------------------------------------------------
// <copyright file="EntityReferenceFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic.Functional
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
    #endregion Using Directives

    /// <summary>
    /// Tests to check working of Entity Reference ($ref) 
    /// </summary>
    [TestClass]
    public class EntityReferenceFunctionalTests
    {
        [TestMethod]
        public void UseMultipleEscapeSequencesWithRefInKeyAsSegment()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/$/$/People/1/$/$/MyDog/$/$/MyPeople/$/$/$ref/$/$")) { UrlConventions = ODataUrlConventions.KeyAsSegment }.ParsePath();
            path.LastSegment.ShouldBeNavigationPropertyLinkSegment(HardCodedTestModel.GetDogMyPeopleNavProp());
        }

        [TestMethod]
        public void KeyLookupCannotAppearAfterCountAfterEntityReference()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs(1)/MyPeople/$ref/$count(1)", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [TestMethod]
        public void KeyLookupCannotAppearAfterEntityReference()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs(1)/$ref(1)", ODataErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [TestMethod]
        public void CountCannotAppearAfterEntityReferenceCollectionProperties()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs(1)/MyPeople/$ref/$count", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [TestMethod]
        public void EntityReferenceCannotAppearAfterAnEntitySet()
        {
            // TODO: We can improve error message drastically when we refactor path parsing
            PathFunctionalTestsUtil.RunParseErrorPath("People/$ref", ODataErrorStrings.PathParser_EntityReferenceNotSupported("People"));
        }

        [TestMethod]
        public void EntityReferenceCannotAppearAfterAComplexProperty()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/MyAddress/$ref", ODataErrorStrings.PathParser_EntityReferenceNotSupported("MyAddress"));
        }

        [TestMethod]
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


        [TestMethod]
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

        [TestMethod]
        public void CannotGoToPropetyOnEntityReference()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(7)/MyDog/$ref/Color", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [Ignore]
        [TestMethod]
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