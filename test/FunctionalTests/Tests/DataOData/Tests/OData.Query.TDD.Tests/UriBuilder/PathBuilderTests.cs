//---------------------------------------------------------------------
// <copyright file="PathBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.UriBuilder
{
    using System;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriBuilder;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PathBuilderTests : UriBuilderTestBase
    {
        [TestMethod]
        public void BuildPathWithSimpleEntitySet()
        {
            Uri queryUri = new Uri("Dogs", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Dogs"), actualUri);
        }

        [TestMethod]
        public void ParsePathWithNavigationPropertyLinks()
        {
            Uri queryUri = new Uri("People(1)/MyDog/$ref", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People(1)/MyDog/$ref"), actualUri);        
        }

        [TestMethod]
        public void BuildPathWithSimpleServiceOperation()
        {
            Uri queryUri = new Uri("GetCoolPeople", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetCoolPeople"), actualUri);
        }

        [TestMethod]
        public void BuildPathWithSimpleActionImport()
        {
            Uri queryUri = new Uri("ResetAllData", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/ResetAllData"), actualUri);
        }

        [TestMethod]
        public void BuildPathWithVoidServiceOperationShouldAllowButIgnoreEmptyParens()
        {
            Uri queryUri = new Uri("GetNothing()", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetNothing"), actualUri);
        }

        [TestMethod]
        public void BuildPathWithPrimitiveServiceOperationIsComposable()
        {
            Uri queryUri = new Uri("GetSomeNumber/$value", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetSomeNumber/$value"), actualUri);
        }

        [TestMethod]
        public void BuildPathWithPrimitiveServiceOperationShouldAllowButIgnoreEmptyParens()
        {
            Uri queryUri = new Uri("GetSomeNumber()", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetSomeNumber"), actualUri);
        }

        [TestMethod]
        public void BuildPathWithComplexServiceOperationIsComposable()
        {
            Uri queryUri = new Uri("GetSomeAddress/City", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetSomeAddress/City"), actualUri);
        }

        [TestMethod]
        public void BuildPathWithEntityServiceOperationIsComposable()
        {
            Uri queryUri = new Uri("GetCoolestPerson/Fully.Qualified.Namespace.Employee", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetCoolestPerson/Fully.Qualified.Namespace.Employee"), actualUri);
        }

        [TestMethod]
        public void BuildPathWithEntitySetServiceOperationIsComposable()
        {
            Uri queryUri = new Uri("GetCoolPeople(1)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetCoolPeople(1)"), actualUri);
        }

        [TestMethod]
        public void BuildPathWithSimpleKeyLookup()
        {
            Uri queryUri = new Uri("Dogs(1)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Dogs(1)"), actualUri);
        }

        [TestMethod]
        public void BuildPathWithSimpleKeyLookupWithKeysAsSegments()
        {
            Uri queryUri = new Uri("Dogs/1", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.KeyAsSegment, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Dogs/1"), actualUri);
        }

        [TestMethod]
        public void BuildPathWithSimpleKeyLookupWithKeysAsSegmentsFollowedByNavigation()
        {
            Uri queryUri = new Uri("People/1/Birthdate", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.KeyAsSegment, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People/1/Birthdate"), actualUri);
        }

        [TestMethod]
        public void BuildPathWithEscapeMarkerWithTypeSegmentInKeyAsSegment()
        {
            Uri queryUri = new Uri("People/$/Fully.Qualified.Namespace.Employee", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.KeyAsSegment, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People/Fully.Qualified.Namespace.Employee"), actualUri);
        }

        [TestMethod]
        public void BuildPathWithRelativeFullUri()
        {
            Uri queryUri = new Uri("Dogs", UriKind.Relative);
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/service.svc"), queryUri);
            ODataUri odataUri = odataUriParser.ParseUri();

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.AreEqual(new Uri("http://gobbldygook/service.svc/Dogs"), actualUri);
        }

        [TestMethod]
        public void TrailingEscapeMarkerShouldBeIgnoredInKeyAsSegment()
        {
            Uri queryUri = new Uri("People/$", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.KeyAsSegment, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People"), actualUri);
        }

        [TestMethod]
        public void DontUseEscapeSequenceInKeyAsSegment()
        {
            Uri queryUri = new Uri("Users/Fully.Qualified.Namespace.User", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.KeyAsSegment, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Users/Fully.Qualified.Namespace.User"), actualUri);
        }

        [TestMethod]
        public void UseMultipleEscapeSequencesWithCountInKeyAsSegment()
        {
            Uri queryUri = new Uri("$/$/People/1/$/$/MyDog/$/$/MyPeople/$/$/$count/$/$", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.KeyAsSegment, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People/1/MyDog/MyPeople/$count"), actualUri);
        }

        [TestMethod]
        public void PathThatIsOnlyEscapeSegments()
        {
            Uri queryUri = new Uri("$/$/$", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook"), actualUri);
        }

        [TestMethod]
        public void BuildPathWithBoundActionOnEntity()
        {
            Uri queryUri = new Uri("Dogs(1)/Fully.Qualified.Namespace.Walk", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Dogs(1)/Fully.Qualified.Namespace.Walk"), actualUri);
        }

        [TestMethod]
        public void BoundActionThatReturnsEntitiesShouldHaveSetComputedCorrectly()
        {
            Uri queryUri = new Uri("People(1)/Fully.Qualified.Namespace.Move", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People(1)/Fully.Qualified.Namespace.Move"), actualUri);
        }

        [TestMethod]
        public void ActionOnEntityCollection()
        {
            Uri queryUri = new Uri("People(1)/Fully.Qualified.Namespace.AllMyFriendsDogs_NoSet()", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People(1)/Fully.Qualified.Namespace.AllMyFriendsDogs_NoSet"), actualUri);
        }

        [TestMethod]
        public void StructuralPropertyOnEntity()
        {
            Uri queryUri = new Uri("People(1)/Birthdate", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People(1)/Birthdate"), actualUri);
        }

        [TestMethod]
        public void OpenPropertyOnOpenEntity()
        {
            Uri queryUri = new Uri("Paintings(1)/SomeOpenProp", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Paintings(1)/SomeOpenProp"), actualUri);
        }

        [TestMethod]
        public void OpenPropertyOnOpenComplex()
        {
            Uri queryUri = new Uri("People(1)/MyOpenAddress/SomeOpenProp", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People(1)/MyOpenAddress/SomeOpenProp"), actualUri);
        }

        [TestMethod]
        public void CastShouldBeAllowedOnSingleEntity()
        {
            Uri queryUri = new Uri("People(2)/Fully.Qualified.Namespace.Employee", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People(2)/Fully.Qualified.Namespace.Employee"), actualUri);
        }

        [TestMethod]
        public void CastShouldBeAllowedOnEntityCollection()
        {
            Uri queryUri = new Uri("People/Fully.Qualified.Namespace.Employee", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People/Fully.Qualified.Namespace.Employee"), actualUri);
        }

        [TestMethod]
        public void KeyLookupsCanBeOnCollectionNavigationProperties()
        {
            Uri queryUri = new Uri("Dogs(1)/MyPeople(2)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Dogs(1)/MyPeople(2)"), actualUri);
        }

        [TestMethod]
        public void MultipartKeyLookup()
        {
            Uri queryUri = new Uri("Lions(ID1=32, ID2=64)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.KeyAsSegment, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Lions(ID1=32,ID2=64)"), actualUri);
        }

        [TestMethod]
        public void KeysExpressionsCanHaveWhitespace()
        {
            Uri queryUri = new Uri("Dogs( ID  =  1 )", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Dogs(1)"), actualUri);
        }

        [TestMethod]
        public void TypeCastOnCollectionHasCollectionType()
        {
            Uri queryUri = new Uri("People/Fully.Qualified.Namespace.Employee", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People/Fully.Qualified.Namespace.Employee"), actualUri);
        }

        [TestMethod]
        public void KeysExpressionsCanAppearOnTypeSegments()
        {
            Uri queryUri = new Uri("People/Fully.Qualified.Namespace.Employee(1)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People/Fully.Qualified.Namespace.Employee(1)"), actualUri);
        }

        [TestMethod]
        public void DerivedPropertyAccessAfterKeysExpressionsOnTypeSegmentIsOk()
        {
            Uri queryUri = new Uri("People/Fully.Qualified.Namespace.Manager(1)/NumberOfReports", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People/Fully.Qualified.Namespace.Manager(1)/NumberOfReports"), actualUri);
        }

        [TestMethod]
        public void MultipartKeysExpressionsCanHaveWhitespace()
        {
            Uri queryUri = new Uri("Lions( ID1 = 32  ,   ID2   =    64    )", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Lions(ID1=32,ID2=64)"), actualUri);
        }

        [TestMethod]
        public void NumberOfKeyExpressionsCanBeLessThanNumberOfDeclaredKeysIfPreviousSegmentHasReferentialIntegrityConstraint()
        {
            Uri queryUri = new Uri("People(32)/MyLions(64)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People(32)/MyLions(ID1=32,ID2=64)"), actualUri);
        }

        [TestMethod]
        public void ExplicitKeysCanBeNamed()
        {
            Uri queryUri = new Uri("People(32)/MyLions(ID2=64)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People(32)/MyLions(ID1=32,ID2=64)"), actualUri);
        }

        [TestMethod]
        public void EntitySetKeyWithOpertionalSuffix()
        {
            // long
            Uri queryUri = new Uri("Pet1Set(102)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Pet1Set(102)"), actualUri);

            queryUri = new Uri("Pet1Set(102l)", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Pet1Set(102)"), actualUri);

            // single
            queryUri = new Uri("Pet2Set(102.0)", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Pet2Set(102)"), actualUri);

            queryUri = new Uri("Pet2Set(102.0F)", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Pet2Set(102)"), actualUri);

            // double
            queryUri = new Uri("Pet3Set(12.0)", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Pet3Set(12.0)"), actualUri);

            queryUri = new Uri("Pet3Set(12d)", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Pet3Set(12.0)"), actualUri);

            // decimal
            queryUri = new Uri("Pet4Set(102.0)", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Pet4Set(102.0)"), actualUri);

            queryUri = new Uri("Pet4Set(102m)", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Pet4Set(102)"), actualUri);
        }

        [TestMethod]
        public void FunctionParameterWithOpertianalSuffix()
        {
            // long
            Uri queryUri = new Uri("GetPet1(id=102)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetPet1(id=102)"), actualUri);

            queryUri = new Uri("GetPet1(id=102L)", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetPet1(id=102L)"), actualUri);

            // float
            queryUri = new Uri("GetPet2(id=102)", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetPet2(id=102)"), actualUri);

            queryUri = new Uri("GetPet2(id=102F)", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetPet2(id=102F)"), actualUri);

            // double
            queryUri = new Uri("GetPet3(id=102.0)", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetPet3(id=102.0)"), actualUri);

            queryUri = new Uri("GetPet3(id=102D)", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetPet3(id=102D)"), actualUri);

            // decimal
            queryUri = new Uri("GetPet4(id=102.0)", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetPet4(id=102.0)"), actualUri);

            queryUri = new Uri("GetPet4(id=102.0)", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetPet4(id=102.0)"), actualUri);
        }

        [TestMethod]
        public void FunctionParameterDoublePrecision()
        {
            Uri queryUri = new Uri("GetPet2(id=-3.40282347E+38)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetPet2(id=-3.40282347E+38)"), actualUri);
        }

        [TestMethod]
        public void FunctionParameterDecimalBound()
        {
            Uri queryUri = new Uri("GetPet4(id=79228162514264337593543950335)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetPet4(id=79228162514264337593543950335)"), actualUri);

            queryUri = new Uri("GetPet4(id=-79228162514264337593543950335)", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetPet4(id=-79228162514264337593543950335)"), actualUri);
        }

        [TestMethod]
        public void FunctionParameterBooleanTrue()
        {
            Uri queryUri = new Uri("GetPet5(id=true)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetPet5(id=true)"), actualUri);

            queryUri = new Uri("GetPet5(id=false)", UriKind.Relative);
            actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetPet5(id=false)"), actualUri);
        }

        [TestMethod]
        public void BatchRequest()
        {
            Uri queryUri = new Uri("$batch", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/$batch"), actualUri);
        }

        [TestMethod]
        public void CountOnEntitySetIsValid()
        {
            Uri queryUri = new Uri("Dogs/$count", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Dogs/$count"), actualUri);
        }

        [TestMethod]
        public void CountOnCollectionReturnedByFunctionIsValid()
        {
            Uri queryUri = new Uri("People(0)/Fully.Qualified.Namespace.AllMyFriendsDogs/$count", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People(0)/Fully.Qualified.Namespace.AllMyFriendsDogs/$count"), actualUri);
        }

        [TestMethod]
        public void CountCanAppearOnCollectionNavigationProperties()
        {
            Uri queryUri = new Uri("Dogs(1)/MyPeople/$count", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Dogs(1)/MyPeople/$count"), actualUri);
        }

        [TestMethod]
        public void ValidMetadataRequest()
        {
            Uri queryUri = new Uri("$metadata", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/$metadata"), actualUri);
        }

        [TestMethod]
        public void ValueRequestOnPrimitivePropertyIsValid()
        {
            Uri queryUri = new Uri("Dogs(1)/Color/$value", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Dogs(1)/Color/$value"), actualUri);
        }

        [TestMethod]
        public void ValueRequestOnComplexPropertyIsValid()
        {
            Uri queryUri = new Uri("People(1)/MyAddress/$value", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People(1)/MyAddress/$value"), actualUri);
        }

        [TestMethod]
        public void AccessNamedStream()
        {
            Uri queryUri = new Uri("Dogs(-31)/NamedStream", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Dogs(-31)/NamedStream"), actualUri);
        }

        [TestMethod]
        public void PropertiesOnOpenPropertiesBecomeOpenProperties()
        {
            Uri queryUri = new Uri("Paintings(-415)/OpenOne/OpenTwo/OpenThree", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Paintings(-415)/OpenOne/OpenTwo/OpenThree"), actualUri);
        }

        [TestMethod]
        public void PropertyWithPeriodsInNameIsFound()
        {
            Uri queryUri = new Uri("People(1)/Prop.With.Periods", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People(1)/Prop.With.Periods"), actualUri);
        }

        [TestMethod]
        public void SegmentWithPeriodsOnOpenTypeIsAProperty()
        {
            Uri queryUri = new Uri("Paintings(1)/Not.A.Type.Or.Operation", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Paintings(1)/Not.A.Type.Or.Operation"), actualUri); 
        }

        [TestMethod]
        public void SegmentsWithPeriodsAfterOpenPropertyIsAnOpenProperty()
        {
            Uri queryUri = new Uri("Paintings(-415)/OpenProperty/Not.A.Type.Or.Operation", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Paintings(-415)/OpenProperty/Not.A.Type.Or.Operation"), actualUri); 
        }

        [TestMethod]
        public void InV3WeCreateOpenPropertiesOverTypeSegments()
        {
            Uri queryUri = new Uri("Paintings(-415)/OpenProperty/Fully.Qualified.Namespace.Person", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Paintings(-415)/OpenProperty/Fully.Qualified.Namespace.Person"), actualUri); 
        }


        [TestMethod]
        public void UriOverloadSmokeTest()
        {
            Uri queryUri = new Uri("Dogs", UriKind.Relative);
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("https://www.tomatosoup.com:1234/OData/V3/"), queryUri);
            ODataUri odataUri = odataUriParser.ParseUri();

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.AreEqual(new Uri("https://www.tomatosoup.com:1234/OData/V3/Dogs"), actualUri);
        }

        [TestMethod]
        public void TrailingDollarSegmentIsIgnored()
        {
            Uri queryUri = new Uri("People/$", UriKind.Relative);
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("https://www.blah.org/OData/"), queryUri);
            ODataUri odataUri = odataUriParser.ParseUri();

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.AreEqual(new Uri("https://www.blah.org/OData/People"), actualUri);
        }

        [TestMethod]
        public void FunctionsOnCollectionsWithParametersWork()
        {
            Uri queryUri = new Uri("People/Fully.Qualified.Namespace.AllHaveDog(inOffice=true)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People/Fully.Qualified.Namespace.AllHaveDog(inOffice=true)"), actualUri); 
        }

        [TestMethod]
        public void LongFunctionChain()
        {
            Uri queryUri = new Uri("People(1)/Fully.Qualified.Namespace.AllMyFriendsDogs()/Fully.Qualified.Namespace.OwnerOfFastestDog()/MyDog/MyPeople/Fully.Qualified.Namespace.AllHaveDog(inOffice=true)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People(1)/Fully.Qualified.Namespace.AllMyFriendsDogs/Fully.Qualified.Namespace.OwnerOfFastestDog/MyDog/MyPeople/Fully.Qualified.Namespace.AllHaveDog(inOffice=true)"), actualUri); 
        }

        [TestMethod]
        public void FunctioImportnWithoutBindingParameterShouldWorkInPath()
        {
            Uri queryUri = new Uri("FindMyOwner(dogsName='fido')", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/FindMyOwner(dogsName='fido')"), actualUri); 
        }

        [TestMethod]
        public void GeometryAndNullParameterValuesShouldWorkInPath()
        {
            var point = GeometryPoint.Create(1, 2);
            Uri queryUri = new Uri("Paintings(0)/Fully.Qualified.Namespace.GetColorAtPosition(position=geometry'" + SpatialHelpers.WriteSpatial(point) + "',includeAlpha=null)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Paintings(0)/Fully.Qualified.Namespace.GetColorAtPosition(position=geometry'" + SpatialHelpers.WriteSpatial(point) + "',includeAlpha=null)"), actualUri); 
        }

        [TestMethod]
        public void GeographyAndNullParameterValuesShouldWorkInPath()
        {
            var point = GeographyPoint.Create(1, 2);
            Uri queryUri = new Uri("People(0)/Fully.Qualified.Namespace.GetNearbyPriorAddresses(currentLocation=geography'" + SpatialHelpers.WriteSpatial(point) + "',limit=null)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/People(0)/Fully.Qualified.Namespace.GetNearbyPriorAddresses(currentLocation=geography'" + SpatialHelpers.WriteSpatial(point) + "',limit=null)"), actualUri); 
        }

        #region enum property in path
        [TestMethod]
        public void EnumPropertyOfEntity()
        {
            Uri queryUri = new Uri("Pet2Set(1)/PetColorPattern", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Pet2Set(1)/PetColorPattern"), actualUri); 
        }

        [TestMethod]
        public void EnumPropertyValueOfEntity()
        {
            Uri queryUri = new Uri("Pet2Set(1)/PetColorPattern/$value", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Pet2Set(1)/PetColorPattern/$value"), actualUri); 
        }
        #endregion

        #region enum parameter in path
        [TestMethod]
        public void ParsePath_NullableEnumInFunction()
        {
            Uri queryUri = new Uri("GetPetCountNullable(colorPattern=Fully.Qualified.Namespace.ColorPattern'BlueYellowStriped')", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetPetCountNullable(colorPattern=Fully.Qualified.Namespace.ColorPattern'22')"), actualUri); 
        }

        [TestMethod]
        public void ParsePath_EnumInFunction()
        {
            Uri queryUri = new Uri("GetPetCount(colorPattern=Fully.Qualified.Namespace.ColorPattern'BlueYellowStriped')", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetPetCount(colorPattern=Fully.Qualified.Namespace.ColorPattern'BlueYellowStriped')"), actualUri); 
        }

        [TestMethod]
        public void ParsePath_EnumInFunction_undefined()
        {
            Uri queryUri = new Uri("GetPetCount(colorPattern=Fully.Qualified.Namespace.ColorPattern'99999222')", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetPetCount(colorPattern=Fully.Qualified.Namespace.ColorPattern'99999222')"), actualUri); 
        }
        #endregion
        #region type definition

        [TestMethod]
        public void KeyOfTypeDefinitionShouldWork()
        {
            Uri queryUri = new Uri("Pet6Set(5.1)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/Pet6Set(5.1)"), actualUri); 
        }

        [TestMethod]
        public void FunctionImportWithTypeDefinitionShouldWork()
        {
            Uri queryUri = new Uri("GetPet6(id=5.1)", UriKind.Relative);
            Uri actualUri = UriBuilder(queryUri, ODataUrlConventions.Default, settings);
            Assert.AreEqual(new Uri("http://gobbledygook/GetPet6(id=5.1)"), actualUri); 
        }
        #endregion
    }
}
