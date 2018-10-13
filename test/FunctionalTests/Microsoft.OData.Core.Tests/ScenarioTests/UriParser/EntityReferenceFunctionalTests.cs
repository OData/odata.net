//---------------------------------------------------------------------
// <copyright file="EntityReferenceFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
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
        public void EntityReferenceCannotAppearAfterMetadata()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("$metadata/$ref", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment("$metadata"));
        }

        [Fact]
        public void EntityReferenceCanAppearAfterSingleton()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Boss/$ref");

            ReferenceSegment referenceSegment = path.LastSegment as ReferenceSegment;
            referenceSegment.TargetEdmNavigationSource.Should().Be(HardCodedTestModel.GetBossSingleton());
            referenceSegment.SingleResult.Should().BeTrue();
        }

        [Fact]
        public void EntityReferenceCannotAppearAfterBatch()
        {
            // Note: Case where $ref is after batch reference is in PathFunctionaltests.cs (EntityReferenceCannotAppearAfterBatchReference)
            PathFunctionalTestsUtil.RunParseErrorPath("$batch/$ref", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.BatchSegment));
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
        public void EntityReferenceCannotAppearAfterProperty()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/SSN/$ref", ODataErrorStrings.RequestUriProcessor_ValueSegmentAfterScalarPropertySegment("SSN", "$ref"));
        }

        [Fact]
        public void CountCannotAppearAfterEntityReferenceCollectionProperties()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs(1)/MyPeople/$ref/$count", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [Fact]
        public void EntityReferenceCanAppearAfterAnEntitySet()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People/$ref");
            path.LastSegment.ShouldBeReferenceSegment(HardCodedTestModel.GetPeopleSet());
        }

        [Fact]
        public void EntityReferenceCanAppearAfterACollectionValuedNavigationProperty()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Dogs(1)/MyPeople/$ref");
            path.LastSegment.ShouldBeNavigationPropertyLinkSegment(HardCodedTestModel.GetDogMyPeopleNavProp());
        }

        [Fact]
        public void EntityReferenceCanAppearAfterASingleValuedNavigationProperty()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Dogs(1)/FastestOwner/$ref");
            path.LastSegment.ShouldBeNavigationPropertyLinkSegment(HardCodedTestModel.GetDogFastestOwnerNavProp());
        }

        [Fact]
        public void EntityReferenceCanAppearAfterAFilteredEntitySet()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People/$filter(@p1)/$ref?@p1=true");
            path.LastSegment.ShouldBeReferenceSegment(HardCodedTestModel.GetPeopleSet());
        }

        [Fact]
        public void EntityReferenceCannotAppearAfterAValueSegment()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/$value/$ref", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment("$value"));
        }

        [Fact]
        public void EntityReferenceCanAppearAfterAComplexProperty()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/MyAddress/$ref");
            path.LastSegment.ShouldBeReferenceSegment(HardCodedTestModel.GetPeopleSet());
        }

        [Fact]
        public void EntityReferenceCannotAppearAfterReferenceSegment()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People/$ref/$ref", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment("$ref"));
        }

        [Fact]
        public void EntityReferenceCannotAppearAfterEachSegment()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People/$each/$ref", ODataErrorStrings.RequestUriProcessor_OnlySingleActionCanProceedEachPathSegment);
        }

        [Fact]
        public void EntityReferenceCanAppearAfterCastedType()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People/Fully.Qualified.Namespace.Employee/$ref");
            path.LastSegment.ShouldBeReferenceSegment(HardCodedTestModel.GetPeopleSet());
        }

        [Fact]
        public void EntityReferenceCanAppearAfterBoundFunctionReturningCollection()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People/Fully.Qualified.Namespace.GetPeopleWhoHaveDogs/$ref");
            path.LastSegment.ShouldBeReferenceSegment(HardCodedTestModel.GetPeopleSet());
        }

        [Fact]
        public void EntityReferenceCanAppearAfterBoundFunctionReturningSingleEntity()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People/Fully.Qualified.Namespace.GetPersonWhoHasSmartestDog/$ref");
            path.LastSegment.ShouldBeReferenceSegment(HardCodedTestModel.GetPeopleSet());
        }

        [Fact]
        public void EntityReferenceCannotAppearAfterBoundAction()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People/Fully.Qualified.Namespace.AdoptShibaInu/$ref", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment("Fully.Qualified.Namespace.AdoptShibaInu"));
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