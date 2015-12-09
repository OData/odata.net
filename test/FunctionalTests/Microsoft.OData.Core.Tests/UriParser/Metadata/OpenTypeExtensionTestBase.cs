//---------------------------------------------------------------------
// <copyright file="OpenTypeExtensionTestBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Metadata;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;

namespace Microsoft.OData.Core.Tests.UriParser.Metadata
{
    public class OpenTypeExtensionTestBase
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
        protected static IEdmEntitySet PeopleSet;
        protected static IEdmOperation FindPencil2P;
        protected static IEdmOperation FindPencil1P;
        protected static IEdmOperation FindPencilsCon;
        protected static IEdmOperation FindPencilsConNT;
        protected static EdmAction ChangeZip;
        protected static EdmFunction GetZip;
        protected static IEdmEntitySet PencilSet;

        static OpenTypeExtensionTestBase()
        {
            var model = new EdmModel();
            Model = model;

            var person = new EdmEntityType("TestNS", "Person", null, false, true);
            var personId = person.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32);
            PersonNameProp = person.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            PersonPen = person.AddStructuralProperty("pen", EdmPrimitiveTypeKind.Binary);
            person.AddKeys(personId);
            PersonType = person;

            var address = new EdmComplexType("TestNS", "Address", null, false, true);
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
            personSet.AddNavigationTarget(PersonNavPen, PencilSet);
            PeopleSet = personSet;

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

            EdmFunction findPencilsCon = new EdmFunction("TestNS", "FindPencilsCon", new EdmCollectionTypeReference(new EdmCollectionType(pencilRef)), true, null, false);
            FindPencilsCon = findPencilsCon;
            findPencilsCon.AddParameter("qid", new EdmEntityTypeReference(PersonType, false));
            model.AddElement(findPencilsCon);

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

        protected void TestExtension<TParser, TResult>(
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
                parse(getCaseInsensitiveParser());
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
