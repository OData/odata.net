//---------------------------------------------------------------------
// <copyright file="ExpandDepthAndCountValidatorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Semantic;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.Parsers
{
    public class ExpandDepthAndCountValidatorTests
    {
        private readonly SelectExpandClause treeWithDepth2;
        private readonly SelectExpandClause treeWithDepth1;
        private readonly SelectExpandClause emptyTree;
        private readonly SelectExpandClause treeWithWidth2;
        private readonly SelectExpandClause treeWithWidth2AndWithRefOption;
        private readonly SelectExpandClause treeWithDepthAndWidth2WithRepeatedParent;
        private readonly SelectExpandClause bigComplexTree;

        public ExpandDepthAndCountValidatorTests()
        {
            this.emptyTree = new SelectExpandClause(new SelectItem[0], true);
            var pathToMyDog = new ODataExpandPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), HardCodedTestModel.GetDogsSet()));
            this.treeWithDepth1 = new SelectExpandClause(new[] { new ExpandedNavigationSelectItem(pathToMyDog, HardCodedTestModel.GetDogsSet(), this.emptyTree) }, true);
            var pathToPerson = new ODataExpandPath(new NavigationPropertySegment(HardCodedTestModel.GetDogMyPeopleNavProp(), HardCodedTestModel.GetPeopleSet()));
            this.treeWithDepth2 = new SelectExpandClause(new[] { new ExpandedNavigationSelectItem(pathToPerson, HardCodedTestModel.GetPeopleSet(), this.treeWithDepth1) }, true);
            var pathToLions = new ODataExpandPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyLionsNavProp(), HardCodedTestModel.GetLionSet()));
            this.treeWithWidth2 = new SelectExpandClause(new[] { new ExpandedNavigationSelectItem(pathToMyDog, HardCodedTestModel.GetDogsSet(), this.emptyTree), new ExpandedNavigationSelectItem(pathToLions, HardCodedTestModel.GetLionSet(), this.emptyTree) }, true);
            this.treeWithWidth2AndWithRefOption = new SelectExpandClause(new[] { new ExpandedReferenceSelectItem(pathToMyDog, HardCodedTestModel.GetDogsSet()), new ExpandedNavigationSelectItem(pathToLions, HardCodedTestModel.GetLionSet(), this.emptyTree) }, true);
            this.treeWithDepthAndWidth2WithRepeatedParent = new SelectExpandClause(new[] { new ExpandedNavigationSelectItem(pathToPerson, HardCodedTestModel.GetPeopleSet(), this.treeWithWidth2) }, true);
            var pathToPaintings = new ODataExpandPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyPaintingsNavProp(), HardCodedTestModel.GetPaintingsSet()));
            this.bigComplexTree = new SelectExpandClause(new[] { 
                new ExpandedNavigationSelectItem(pathToMyDog, HardCodedTestModel.GetDogsSet(), this.treeWithDepthAndWidth2WithRepeatedParent), 
                new ExpandedNavigationSelectItem(pathToLions, HardCodedTestModel.GetLionSet(), this.emptyTree),
                new ExpandedNavigationSelectItem(pathToPaintings, HardCodedTestModel.GetPaintingsSet(), this.emptyTree) }, true);
        }

        [Fact]
        public void ValidatorShouldThrowOnTreeThatIsTooDeep()
        {
            ValidatorShouldThrow(this.treeWithDepth2, ODataErrorStrings.UriParser_ExpandDepthExceeded(2, 1), maxDepth: 1);
        }

        [Fact]
        public void ValidatorShouldNotThrowOnTreeThatIsExactlyAsDeepAsAllowed()
        {
            ValidatorShouldNotThrow(this.treeWithDepth2, maxDepth: 2);
        }

        [Fact]
        public void ValidatorNotShouldThrowOnEmptyTreeIfMaxDepthIsZero()
        {
            ValidatorShouldNotThrow(this.emptyTree, maxDepth: 0);
        }

        [Fact]
        public void ValidatorNotShouldThrowOnEmptyTreeIfMaxCountIsZero()
        {
            ValidatorShouldNotThrow(this.emptyTree, maxCount: 0);
        }

        [Fact]
        public void ValidatorShouldFailIfDepthExceedsCount()
        {
            ValidatorShouldThrow(this.treeWithDepth2, ODataErrorStrings.UriParser_ExpandCountExceeded(2, 1), maxCount: 1);
        }

        [Fact]
        public void ValidatorShouldFailIfNumberOfExpandItemsExceedsSetting()
        {
            ValidatorShouldThrow(this.treeWithWidth2, ODataErrorStrings.UriParser_ExpandCountExceeded(2, 1), maxCount: 1);
        }

        [Fact]
        public void ValidatorShouldNotFailIfNumberOfExpandItemsMatchesSetting()
        {
            ValidatorShouldNotThrow(this.treeWithWidth2, maxCount: 2);
        }

        [Fact]
        public void ValidatorShouldFailIfNumberOfExpandItemsExceedsSettingWhichWithRefOption()
        {
            ValidatorShouldThrow(this.treeWithWidth2AndWithRefOption , ODataErrorStrings.UriParser_ExpandCountExceeded(2, 1), maxCount: 1);
        }

        [Fact]
        public void ValidatorShouldNotFailIfNumberOfExpandItemsMatchesSettingWhichWithRefOption()
        {
            ValidatorShouldNotThrow(this.treeWithWidth2AndWithRefOption, maxCount: 2);
        }

        [Fact]
        public void ValidatorShouldNotCountRepeatedParentNodesTowardsLimitMoreThanOnce()
        {
            ValidatorShouldNotThrow(this.treeWithDepthAndWidth2WithRepeatedParent, maxCount: 3);
        }

        [Fact]
        public void ValidatorShouldCountParentNodesTowardsLimit()
        {
            ValidatorShouldThrow(this.treeWithDepthAndWidth2WithRepeatedParent, ODataErrorStrings.UriParser_ExpandCountExceeded(3, 2), maxCount: 2);
        }

        [Fact]
        public void ValidatorShouldFailOnBigComplexTreeIfDepthExceedsLimit()
        {
            ValidatorShouldThrow(this.bigComplexTree, ODataErrorStrings.UriParser_ExpandDepthExceeded(3, 2), maxDepth: 2);
        }

        [Fact]
        public void ValidatorShouldFailOnBigComplexTreeIfCountExceedsLimit()
        {
            ValidatorShouldThrow(this.bigComplexTree, ODataErrorStrings.UriParser_ExpandCountExceeded(6, 5), maxCount: 5);
        }

        [Fact]
        public void ValidatorShouldNotFailOnBigComplexTreeIfDepthMatchesLimit()
        {
            ValidatorShouldNotThrow(this.bigComplexTree, maxDepth: 3);
        }

        [Fact]
        public void ValidatorShouldNotFailOnBigComplexTreeIfCountMatchesLimit()
        {
            ValidatorShouldNotThrow(this.bigComplexTree, maxCount: 6);
        }

        [Fact]
        public void ValidatorShouldFailImmediatelyIfTheCountLimitIsExceeded()
        {
            // note that size is 2 but the error message only says it found 1, since it stopped at that point.
            ValidatorShouldThrow(this.treeWithWidth2, ODataErrorStrings.UriParser_ExpandCountExceeded(1, 0), maxCount: 0);
        }

        [Fact]
        public void ValidatorShouldFailImmediatelyIfTheDepthLimitIsExceeded()
        {
            // note that size is 2 but the error message only says it found 1, since it stopped at that point.
            ValidatorShouldThrow(this.treeWithDepth2, ODataErrorStrings.UriParser_ExpandDepthExceeded(1, 0), maxDepth: 0);
        }

        private static void ValidatorShouldThrow(SelectExpandClause selectExpandClause, string expectedMessage, int maxDepth = 100, int maxCount = 100)
        {
            var testSubject = new ExpandDepthAndCountValidator(maxDepth, maxCount);
            Action validate = () => testSubject.Validate(selectExpandClause);
            validate.ShouldThrow<ODataException>().WithMessage(expectedMessage);
        }

        private static void ValidatorShouldNotThrow(SelectExpandClause selectExpandClause, int maxDepth = 100, int maxCount = 100)
        {
            var testSubject = new ExpandDepthAndCountValidator(maxDepth, maxCount);
            testSubject.Validate(selectExpandClause);
        }
    }
}