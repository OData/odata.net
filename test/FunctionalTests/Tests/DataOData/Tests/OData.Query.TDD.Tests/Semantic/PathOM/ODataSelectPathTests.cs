//---------------------------------------------------------------------
// <copyright file="ODataSelectPathTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    [TestClass]
    public class ODataSelectPathTests
    {
        [TestMethod]
        public void SelectPathShouldNotAllowCountSegment()
        {
            Action buildWithCountSegment = () => new ODataSelectPath(CountSegment.Instance);
            buildWithCountSegment.ShouldThrow<ODataException>(ODataErrorStrings.ODataSelectPath_InvalidSelectPathSegmentType("Microsoft.OData.Core.UriParser.Semantic.CountSegment"));
        }

        [TestMethod]
        public void SelectPathShouldNotAllowValueSegment()
        {
            Action buildWithCountSegment = () => new ODataSelectPath(new ValueSegment(ModelBuildingHelpers.BuildValidEntityType()));
            buildWithCountSegment.ShouldThrow<ODataException>(ODataErrorStrings.ODataSelectPath_InvalidSelectPathSegmentType("Microsoft.OData.Core.UriParser.Semantic.ValueSegment"));
        }

        [TestMethod]
        public void SelectPathShouldAllowMultipleDifferentSegments()
        {
            List<ODataPathSegment> typeSegments = new List<ODataPathSegment>() { new TypeSegment(HardCodedTestModel.GetPersonType(), null), new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null) };
            new ODataSelectPath(typeSegments).Should().ContainExactly(typeSegments.ToArray());
        }

        [TestMethod]
        public void SelectPathShouldnotEndInTypeSegment()
        {
            List<ODataPathSegment> typeSegments = new List<ODataPathSegment>() { new TypeSegment(HardCodedTestModel.GetPersonType(), null) };
            Action createWithTypeAsLastSegment = () => new ODataSelectPath(typeSegments);
            createWithTypeAsLastSegment.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataSelectPath_CannotEndInTypeSegment);
        }

        [TestMethod]
        public void NavPropCanOnlyBeLastSegment()
        {
            List<ODataPathSegment> typeSegments = new List<ODataPathSegment>() { new TypeSegment(HardCodedTestModel.GetPersonType(), null), new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null), new PropertySegment(HardCodedTestModel.GetDogColorProp()) };
            Action createWithInteriorNavProp = () => new ODataSelectPath(typeSegments);
            createWithInteriorNavProp.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataSelectPath_NavPropSegmentCanOnlyBeLastSegment);
        }

        [TestMethod]
        public void OperationCanOnlyBeLastSegment()
        {
            List<ODataPathSegment> typeSegments = new List<ODataPathSegment>() { new TypeSegment(HardCodedTestModel.GetPersonType(), null), new OperationSegment(HardCodedTestModel.GetFunctionForGetCoolPeople(), HardCodedTestModel.GetPeopleSet()), new PropertySegment(HardCodedTestModel.GetDogColorProp()) };
            Action createWithInteriorNavProp = () => new ODataSelectPath(typeSegments);
            createWithInteriorNavProp.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataSelectPath_OperationSegmentCanOnlyBeLastSegment);
        }

        [TestMethod]
        public void OperationImportCanOnlyBeLastSegment()
        {
            List<ODataPathSegment> typeSegments = new List<ODataPathSegment>() { new OperationImportSegment(HardCodedTestModel.GetFunctionImportForGetCoolestPerson(), HardCodedTestModel.GetPeopleSet())};
            Action createWithInteriorNavProp = () => new ODataSelectPath(typeSegments);
            createWithInteriorNavProp.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataSelectPath_InvalidSelectPathSegmentType(typeof(OperationImportSegment).Name));
        }
    }
}