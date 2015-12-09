//---------------------------------------------------------------------
// <copyright file="ExtensionTestBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Metadata;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Values;

namespace Microsoft.OData.Core.Tests.UriParser.Metadata
{
    public class ExtensionTestBase
    {
        #region Test init
        protected readonly Uri ServiceRoot = new Uri("http://host");
        protected static IEdmModel Model;
        protected static IEdmStructuralProperty PersonNameProp;
        protected static IEdmProperty PersonPen;
        protected static IEdmNavigationProperty PersonNavPen;
        protected static IEdmNavigationProperty PersonNavPen2;
        protected static IEdmStructuralProperty AddrProperty;
        protected static IEdmStructuralProperty ZipCodeProperty;
        protected static IEdmStructuralProperty PencilId;
        protected static IEdmComplexType AddrType;
        protected static IEdmEntityType PersonType;
        protected static IEdmEntityType StarPencil;
        protected static IEdmEntityType PetType;
        protected static IEdmEntitySet PeopleSet;
        protected static IEdmEntitySet PetSet;
        protected static IEdmOperation FindPencil2P;
        protected static IEdmOperation FindPencil1P;
        protected static IEdmOperation FindPencilCon;
        protected static IEdmOperation FindPencils;
        protected static IEdmOperation FindPencilsCon;
        protected static IEdmOperation FindPencilsConUpper;
        protected static IEdmOperation FindPencilsConNT;
        protected static EdmAction ChangeZip;
        protected static EdmFunction GetZip;
        protected static IEdmEntitySet PencilSet;
        protected static IEdmEntitySet PencilSetUpper;
        protected static IEdmSingleton Boss;
        protected static IEdmSingleton Bajie;
        protected static IEdmSingleton BajieUpper;
        protected static IEdmOperation Feed;
        protected static IEdmOperation GetColorCmyk;
        protected static IEdmOperationImport FeedImport;
        protected static IEdmOperationImport FeedConImport;
        protected static IEdmOperationImport FeedConUpperImport;
        protected static IEdmOperationImport GetColorCmykImport;
        protected static IEdmOperationImport GetMixedColor;
        protected static IEdmType MoonType;
        protected static IEdmEntitySet MoonSet;
        protected static IEdmEnumType Color;
        protected static IEdmType MoonType2;
        protected static IEdmEntitySet MoonSet2;

        static ExtensionTestBase()
        {
            var model = new EdmModel();
            Model = model;

            var person = new EdmEntityType("TestNS", "Person");
            var personId = person.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32);
            PersonNameProp = person.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            PersonPen = person.AddStructuralProperty("pen", EdmPrimitiveTypeKind.Binary);
            person.AddKeys(personId);
            PersonType = person;

            var address = new EdmComplexType("TestNS", "Address");
            AddrType = address;
            ZipCodeProperty = address.AddStructuralProperty("ZipCode", EdmPrimitiveTypeKind.String);
            AddrProperty = person.AddStructuralProperty("Addr", new EdmComplexTypeReference(address, false));

            var pencil = new EdmEntityType("TestNS", "Pencil");
            var pencilId = pencil.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            PencilId = pencilId;
            var pid = pencil.AddStructuralProperty("Pid", EdmPrimitiveTypeKind.Int32);
            pencil.AddKeys(pencilId);

            var starPencil = new EdmEntityType("TestNS", "StarPencil", pencil);
            model.AddElement(starPencil);
            var starPencilUpper = new EdmEntityType("TestNS", "STARPENCIL");
            model.AddElement(starPencilUpper);
            StarPencil = starPencil;

            var navInfo = new EdmNavigationPropertyInfo()
            {
                Name = "Pen",
                ContainsTarget = false,
                Target = pencil,
                TargetMultiplicity = EdmMultiplicity.One
            };

            PersonNavPen = person.AddUnidirectionalNavigation(navInfo);

            var navInfo2 = new EdmNavigationPropertyInfo()
            {
                Name = "Pen2",
                ContainsTarget = true,
                Target = pencil,
                TargetMultiplicity = EdmMultiplicity.One
            };

            PersonNavPen2 = person.AddUnidirectionalNavigation(navInfo2);

            var container = new EdmEntityContainer("Default", "Con1");
            var personSet = container.AddEntitySet("People", person);
            PencilSet = container.AddEntitySet("PencilSet", pencil);
            Boss = container.AddSingleton("Boss", person);
            Bajie = container.AddSingleton("Bajie", pencil);
            BajieUpper = container.AddSingleton("BAJIE", pencil);
            personSet.AddNavigationTarget(PersonNavPen, PencilSet);
            PeopleSet = personSet;

            var pencilSetUpper = container.AddEntitySet("PENCILSET", pencil);
            PencilSetUpper = pencilSetUpper;

            var pencilRef = new EdmEntityTypeReference(pencil, false);
            EdmFunction findPencil1 = new EdmFunction("TestNS", "FindPencil", pencilRef, true, null, false);
            findPencil1.AddParameter("qid", new EdmEntityTypeReference(PersonType, false));
            model.AddElement(findPencil1);
            FindPencil1P = findPencil1;

            EdmFunction findPencil = new EdmFunction("TestNS", "FindPencil", pencilRef, true, null, false);
            findPencil.AddParameter("qid", new EdmEntityTypeReference(PersonType, false));
            findPencil.AddParameter("pid", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(findPencil);
            FindPencil2P = findPencil;

            EdmFunction findPencils = new EdmFunction("TestNS", "FindPencils", new EdmCollectionTypeReference(new EdmCollectionType(pencilRef)), true, null, false);
            findPencils.AddParameter("qid", new EdmEntityTypeReference(PersonType, false));
            model.AddElement(findPencils);
            FindPencils = findPencils;

            EdmFunction findPencilsCon = new EdmFunction("TestNS", "FindPencilsCon", new EdmCollectionTypeReference(new EdmCollectionType(pencilRef)), true, null, false);
            FindPencilsCon = findPencilsCon;
            findPencilsCon.AddParameter("qid", new EdmEntityTypeReference(PersonType, false));
            model.AddElement(findPencilsCon);

            EdmFunction findPencilsConUpper = new EdmFunction("TestNS", "FindPencilsCON", new EdmCollectionTypeReference(new EdmCollectionType(pencilRef)), true, null, false);
            FindPencilsConUpper = findPencilsConUpper;
            findPencilsConUpper.AddParameter("qid", new EdmEntityTypeReference(PersonType, false));
            model.AddElement(findPencilsConUpper);

            EdmFunction findPencilsConNT = new EdmFunction("TestNT", "FindPencilsCon", new EdmCollectionTypeReference(new EdmCollectionType(pencilRef)), true, null, false);
            FindPencilsConNT = findPencilsConNT;
            findPencilsConNT.AddParameter("qid", new EdmEntityTypeReference(PersonType, false));
            model.AddElement(findPencilsConNT);

            ChangeZip = new EdmAction("TestNS", "ChangeZip", null, true, null);
            ChangeZip.AddParameter("address", new EdmComplexTypeReference(address, false));
            ChangeZip.AddParameter("newZip", EdmCoreModel.Instance.GetString(false));
            model.AddElement(ChangeZip);

            GetZip = new EdmFunction("TestNS", "GetZip", EdmCoreModel.Instance.GetString(false), true, null, true);
            GetZip.AddParameter("address", new EdmComplexTypeReference(address, false));
            model.AddElement(GetZip);

            model.AddElement(person);
            model.AddElement(address);
            model.AddElement(pencil);
            model.AddElement(container);

            var feed = new EdmAction("TestNS", "Feed", null);
            Feed = feed;
            feed.AddParameter("pid", EdmCoreModel.Instance.GetInt32(false));

            FeedImport = container.AddActionImport("Feed", feed);
            FeedConImport = container.AddActionImport("FeedCon", feed);
            FeedConUpperImport = container.AddActionImport("FeedCON", feed);

            var pet = new EdmEntityType("TestNS", "Pet");
            PetType = pet;
            model.AddElement(pet);
            var key1 = pet.AddStructuralProperty("key1", EdmCoreModel.Instance.GetInt32(false));
            var key2 = pet.AddStructuralProperty("key2", EdmCoreModel.Instance.GetString(false));
            pet.AddKeys(key1, key2);
            var petSet = container.AddEntitySet("PetSet", pet);

            var petCon = new EdmEntityType("TestNS", "PetCon");
            model.AddElement(petCon);
            var key1Con = pet.AddStructuralProperty("key", EdmCoreModel.Instance.GetInt32(false));
            var key2Con = pet.AddStructuralProperty("KEY", EdmCoreModel.Instance.GetString(false));
            petCon.AddKeys(key1Con, key2Con);
            var petSetCon = container.AddEntitySet("PetSetCon", petCon);

            EdmEnumType colorType = new EdmEnumType("TestNS", "Color");
            Color = colorType;
            colorType.AddMember("Red", new EdmIntegerConstant(1L));
            colorType.AddMember("Blue", new EdmIntegerConstant(2L));
            model.AddElement(colorType);
            var moonType = new EdmEntityType("TestNS", "Moon");
            MoonType = moonType;
            var color = moonType.AddStructuralProperty("color", new EdmEnumTypeReference(colorType, false));
            moonType.AddKeys(color);
            model.AddElement(moonType);
            container.AddEntitySet("MoonSet", moonType);

            var moonType2 = new EdmEntityType("TestNS", "Moon2");
            MoonType2 = moonType2;
            var color2 = moonType2.AddStructuralProperty("color", new EdmEnumTypeReference(colorType, false));
            var id = moonType2.AddStructuralProperty("id", EdmCoreModel.Instance.GetInt32(false));
            moonType2.AddKeys(color2, id);
            model.AddElement(moonType2);
            container.AddEntitySet("MoonSet2", moonType2);

            EdmFunction findPencilCon = new EdmFunction("TestNS", "FindPencilCon", pencilRef, true, null, false);
            FindPencilCon = findPencilCon;
            findPencilCon.AddParameter("qid", new EdmEntityTypeReference(PersonType, false));
            findPencilCon.AddParameter("pid", EdmCoreModel.Instance.GetInt32(false));
            findPencilCon.AddParameter("PID", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(findPencilCon);

            EdmFunction getColorCmyk = new EdmFunction("TestNS", "GetColorCmyk", EdmCoreModel.Instance.GetString(false));
            getColorCmyk.AddParameter("co", new EdmEnumTypeReference(colorType, true));
            GetColorCmyk = getColorCmyk;
            model.AddElement(getColorCmyk);
            GetColorCmykImport = container.AddFunctionImport("GetColorCmykImport", getColorCmyk);

            EdmFunction getMixedColor = new EdmFunction("TestNS", "GetMixedColor", EdmCoreModel.Instance.GetString(false));
            getMixedColor.AddParameter("co", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEnumTypeReference(colorType, true))));
            model.AddElement(getMixedColor);
            GetMixedColor = container.AddFunctionImport("GetMixedColorImport", getMixedColor);
        }
        #endregion

        #region TestMethod
        protected void TestUriParserExtension<TResult>(
            string originalStr,
            string caseInsensitiveStr,
            Func<ODataUriParser, TResult> parse,
            Action<TResult> verify,
            string errorMessage,
            IEdmModel model,
            Action<ODataUriParser> parserSet)
        {
            Uri originalCase = new Uri(originalStr, UriKind.Relative);
            Uri caseInsensitiveCase = new Uri(caseInsensitiveStr, UriKind.Relative);
            TestExtension(
                () => new ODataUriParser(model, originalCase),
                () => new ODataUriParser(model, caseInsensitiveCase),
                parse,
                verify,
                errorMessage,
                model,
                parserSet);
        }

        protected void TestExtension<TParser,TResult>(
            Func<TParser> getOriginalParser,
            Func<TParser> getCaseInsensitiveParser,
            Func<TParser, TResult> parse,
            Action<TResult> verify,
            string errorMessage,
            IEdmModel model,
            Action<TParser> parserSet)
        {
            // Original case should pass
            TParser parser = getOriginalParser();
            verify(parse(parser));

            // Original case should pass with CaseInsensitive parser
            parser = getOriginalParser();
            parserSet(parser);
            verify(parse(parser));

            // CaseInsensitive case should fail with original parser, or null result excepted when error message be null.
            if (errorMessage != null)
            {
                Action action = () => parse(getCaseInsensitiveParser());
                action.ShouldThrow<ODataException>().WithMessage(errorMessage);
            }
            else
            {
                parse(getCaseInsensitiveParser()).Should().BeNull();
            }

            // CaseInsensitive case should pass with CaseInsensitive parser
            parser = getCaseInsensitiveParser();
            parserSet(parser);
            verify(parse(parser));
        }

        protected void TestConflict<TResult>(
            string originalStr,
            Func<ODataUriParser, TResult> parse,
            Action<TResult> verify,
            string conflictMessage,
            IEdmModel model,
            ODataUriResolver resolver)
        {
            Uri originalCase = new Uri(originalStr, UriKind.Relative);

            if (verify != null)
            {
                // Original case should pass
                ODataUriParser parser = new ODataUriParser(model, originalCase);
                verify(parse(parser));
            }

            // Original case should fail with CaseInsensitive parser with errorMessage
            Action action = () => parse(new ODataUriParser(model, originalCase) { Resolver = resolver });
            action.ShouldThrow<ODataException>().WithMessage(conflictMessage);
        }


        protected void TestNotExist<TResult>(
            string originalStr,
            Func<ODataUriParser, TResult> parse,
            string message,
            IEdmModel model,
            Action<ODataUriParser> parserSet)
        {
            Uri originalCase = new Uri(originalStr, UriKind.Relative);

            // Original case should fail with message
            ODataUriParser parser = new ODataUriParser(model, originalCase);
            Action action = () => parse(parser);
            action.ShouldThrow<ODataException>().WithMessage(message);

            // Original case should fail with CaseInsensitive parser with same errorMessage
            parser = new ODataUriParser(model, originalCase);
            parserSet(parser);
            action = () => parse(parser);
            action.ShouldThrow<ODataException>().WithMessage(message);
        }
        #endregion
    }
}
