//---------------------------------------------------------------------
// <copyright file="ODataPathParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ErrorStrings = Microsoft.OData.Core.Strings;

    [TestClass]
    public class ODataPathParserTests
    {
        private readonly Func<string, BatchReferenceSegment> callback = s => new BatchReferenceSegment(s, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
        private readonly ODataPathParser testSubject = new ODataPathParser(new ODataUriParserConfiguration(HardCodedTestModel.TestModel));
        private readonly ODataPathParser templateParser = new ODataPathParser(new ODataUriParserConfiguration(HardCodedTestModel.TestModel) { EnableUriTemplateParsing = true });

        [TestMethod]
        public void CallbackReturnsBatchReferenceSegment()
        {
            ODataPathParser pathParser = new ODataPathParser(new ODataUriParserConfiguration(HardCodedTestModel.TestModel) { BatchReferenceCallback = callback });
            IList<ODataPathSegment> segments = pathParser.ParsePath(new[] { "$0" });
            ODataPathSegment segment = segments.ToList().Single();
            segment.Identifier.Should().Be("$0");
            segment.TargetEdmNavigationSource.Should().Be(HardCodedTestModel.GetPeopleSet());
            segment.TargetEdmType.Should().Be(HardCodedTestModel.GetPersonType());
        }

        [TestMethod]
        public void RequestUriProcessorExtractSegmentIdentifierTest()
        {
            ExtractSegmentIdentifierAndParenthesisExpression("blah", "blah", null);
            ExtractSegmentIdentifierAndParenthesisExpression("multiple words", "multiple words", null);
            ExtractSegmentIdentifierAndParenthesisExpression("multiple(123)", "multiple", "123");
            ExtractSegmentIdentifierAndParenthesisExpression("multiple(123;321)", "multiple", "123;321");
            ExtractSegmentIdentifierAndParenthesisExpression("set()", "set", string.Empty);
        }

        [TestMethod]
        public void RequestUriProcessorExtractSegmentIdentifierErrorTest()
        {
            string actualIdentifier;
            string queryPortion;

            Action emptyString = () => ODataPathParser.ExtractSegmentIdentifierAndParenthesisExpression(string.Empty, out actualIdentifier, out queryPortion);
            emptyString.ShouldThrow<ODataException>(ErrorStrings.RequestUriProcessor_EmptySegmentInRequestUrl);

            Action noIdentifier = () => ODataPathParser.ExtractSegmentIdentifierAndParenthesisExpression("()", out actualIdentifier, out queryPortion);
            noIdentifier.ShouldThrow<ODataException>(ErrorStrings.RequestUriProcessor_EmptySegmentInRequestUrl);

            Action noEndParen = () => ODataPathParser.ExtractSegmentIdentifierAndParenthesisExpression("foo(", out actualIdentifier, out queryPortion);
            noEndParen.ShouldThrow<ODataException>(ErrorStrings.RequestUriProcessor_SyntaxError);
        }

        #region $ref cases
        [TestMethod]
        public void EntityReferenceFollowingEntityCollectionShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "$ref" });
            parsePath.ShouldThrow<ODataException>(ErrorStrings.PathParser_EntityReferenceNotSupported("People(1)"));
        }

        [TestMethod]
        public void EntityReferenceToNonexistentPropertyShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "foo", "$ref" });
            parsePath.ShouldThrow<ODataUnrecognizedPathException>().WithMessage(ErrorStrings.RequestUriProcessor_ResourceNotFound("foo"));
        }

        [TestMethod]
        public void EntityReferenceToOpenPropertyShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Paintings(1)", "foo", "$ref" });
            parsePath.ShouldThrow<ODataException>().WithMessage(ErrorStrings.PathParser_EntityReferenceNotSupported("foo"));
        }

        [TestMethod]
        public void EntityReferenceToPrimitivePropertyShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "Name", "$ref" });
            parsePath.ShouldThrow<ODataUnrecognizedPathException>().WithMessage(ErrorStrings.RequestUriProcessor_ValueSegmentAfterScalarPropertySegment("Name", UriQueryConstants.RefSegment));
        }

        [TestMethod]
        public void FurtherCompositionAfterEntityReferenceShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "MyDog", "$ref", "MyPeople" });
            parsePath.ShouldThrow<ODataUnrecognizedPathException>().WithMessage(ErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [TestMethod]
        public void FurtherCompositionAfterEntityReferenceWithKeyShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "MyDog", "MyPeople(2)", "$ref", "MyDog" });
            parsePath.ShouldThrow<ODataUnrecognizedPathException>().WithMessage(ErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [TestMethod]
        public void FurtherCompositionAfterEntityCollectionReferenceShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "MyDog", "MyPeople", "$ref", "1" });
            parsePath.ShouldThrow<ODataUnrecognizedPathException>().WithMessage(ErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [TestMethod]
        public void KeyAfterEntityReferenceWithKeyShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "MyDog", "MyPeople(5)", "$ref", "1" });
            parsePath.ShouldThrow<ODataUnrecognizedPathException>().WithMessage(ErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [TestMethod]
        public void KeyAfterEntityReferenceShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "MyDog", "$ref", "5" });
            parsePath.ShouldThrow<ODataUnrecognizedPathException>().WithMessage(ErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [TestMethod]
        public void EntityReferenceAfterEntityReferenceShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "MyDog", "$ref", "MyPeople", "$ref" });
            parsePath.ShouldThrow<ODataUnrecognizedPathException>().WithMessage(ErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [TestMethod]
        public void AnythingAfterCountAfterEntityReferenceShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "MyDog", "MyPeople", "$ref", "$count", "foo" });
            parsePath.ShouldThrow<ODataUnrecognizedPathException>().WithMessage(ErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [TestMethod]
        public void EntityReferenceForOpenPropertyShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Paintings(1)", "foo", "MyDog", "$ref" });
            parsePath.ShouldThrow<ODataException>().WithMessage(ErrorStrings.PathParser_EntityReferenceNotSupported("MyDog"));
        }

        [TestMethod]
        public void EntityReferenceForActionShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Dogs(1)", "Fully.Qualified.Namespace.Walk", "$ref" });
            parsePath.ShouldThrow<ODataException>().WithMessage(ErrorStrings.RequestUriProcessor_MustBeLeafSegment("Fully.Qualified.Namespace.Walk"));
        }

        [TestMethod]
        public void EntityReferenceForKeyShouldWork()
        {
            this.testSubject.ParsePath(new[] { "People(1)", "MyDog", "$ref" })
                .Last().ShouldBeNavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp());
        }

        [TestMethod]
        public void CountAfterEntityReferenceShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Dogs(1)", "MyPeople", "$ref", "$count" });
            parsePath.ShouldThrow<ODataUnrecognizedPathException>().WithMessage(ErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [TestMethod]
        public void CountAfterEntityReferenceWithKeyShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Dogs(1)", "MyPeople(5)", "$ref", "$count" });
            parsePath.ShouldThrow<ODataUnrecognizedPathException>().WithMessage(ErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }
        #endregion

        [TestMethod]
        public void ParenthesesAfterCollectionPropertyShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Dogs(1)", "Nicknames()" });
            parsePath.ShouldThrow<ODataException>().WithMessage(ErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [TestMethod]
        public void ParenthesesAfterAnythingThatIsASingleResultShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Dogs(1)", "Color()" });
            parsePath.ShouldThrow<ODataException>().WithMessage(ErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [TestMethod]
        public void SingletonShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "Boss" });
            path[0].ShouldBeSingletonSegment(HardCodedTestModel.GetBossSingleton());
        }

        [TestMethod]
        public void NavigationInSingletonShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "Boss", "MyDog" });
            path.Count.Should().Be(2);
            path[0].ShouldBeSingletonSegment(HardCodedTestModel.GetBossSingleton());
            path[1].ShouldBeNavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp()).
                And.NavigationSource.Should().BeSameAs(HardCodedTestModel.GetDogsSet());
        }

        [TestMethod]
        public void SingletonWithKeyShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Boss(1)" });
            parsePath.ShouldThrow<ODataException>().WithMessage(ErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [TestMethod]
        public void FunctionImportShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "FindMyOwner(dogsName='fido')" });
            var parameter = path[0].ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForFindMyOwner()).And.Parameters.Single();
            parameter.As<OperationSegmentParameter>().Value.As<ConstantNode>().Value.Should().Be("fido");
        }

        [TestMethod]
        public void FunctionImportShouldWorkWithNumericChar()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "FindMyOwner(dogsName='000110011E0124221929')" });
            var parameter = path[0].ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForFindMyOwner()).And.Parameters.Single();
            parameter.As<OperationSegmentParameter>().Value.As<ConstantNode>().Value.Should().Be("000110011E0124221929");
        }
        #region Template cases
        [TestMethod]
        public void NormalFunctionImportShouldWorkWithTemplateParser()
        {
            IList<ODataPathSegment> path = this.templateParser.ParsePath(new[] { "FindMyOwner(dogsName='fido')" });
            var parameter = path[0].ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForFindMyOwner()).And.Parameters.Single();
            parameter.As<OperationSegmentParameter>().Value.As<ConstantNode>().Value.Should().Be("fido");
        }

        [TestMethod]
        public void ActionWithTemplateShouldWork()
        {
            IEdmFunction operation = HardCodedTestModel.GetHasDogOverloadForPeopleWithTwoParameters();
            IList<ODataPathSegment> path = this.templateParser.ParsePath(new[] { "Boss", "Fully.Qualified.Namespace.HasDog(inOffice={fido})" });
            OperationSegmentParameter parameter = path[1].ShouldBeOperationSegment(operation).And.Parameters.Single();
            parameter.ShouldBeConstantParameterWithValueType("inOffice", new UriTemplateExpression { LiteralText = "{fido}", ExpectedType = operation.Parameters.Last().Type });
        }

        [TestMethod]
        public void FunctionImportWithTemplateShouldWork()
        {
            IEdmOperationImport operationImport = HardCodedTestModel.GetFunctionImportForFindMyOwner();
            IList<ODataPathSegment> path = this.templateParser.ParsePath(new[] { "FindMyOwner(dogsName={fido})" });
            OperationSegmentParameter parameter = path[0].ShouldBeOperationImportSegment(operationImport).And.Parameters.Single();
            parameter.ShouldBeConstantParameterWithValueType("dogsName", new UriTemplateExpression { LiteralText = "{fido}", ExpectedType = operationImport.Operation.Parameters.Single().Type });
        }

        [TestMethod]
        public void KeyWithTemplateShouldWork()
        {
            IList<ODataPathSegment> path = this.templateParser.ParsePath(new[] { "Dogs({fido})", "MyPeople" });
            var keySegment = path[1].As<KeySegment>();
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            keypair.Key.Should().Be("ID");
            keypair.Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression() { LiteralText = "{fido}", ExpectedType = keySegment.EdmType.As<IEdmEntityType>().DeclaredKey.Single().Type });
        }

        [TestMethod]
        public void NamedKeyWithTemplateShouldWork()
        {
            IList<ODataPathSegment> path = this.templateParser.ParsePath(new[] { "Dogs(ID={fido})", "MyPeople" });
            var keySegment = path[1].As<KeySegment>();
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            keypair.Key.Should().Be("ID");
            keypair.Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression() { LiteralText = "{fido}", ExpectedType = keySegment.EdmType.As<IEdmEntityType>().DeclaredKey.Single().Type });
        }

        [TestMethod]
        public void KeyAsSegmentWithTemplateShouldWork()
        {
            var keyAsSegmentTemplateParser = new ODataPathParser(new ODataUriParserConfiguration(HardCodedTestModel.TestModel) { EnableUriTemplateParsing = true, UrlConventions = ODataUrlConventions.KeyAsSegment });
            IList<ODataPathSegment> path = keyAsSegmentTemplateParser.ParsePath(new[] { "Dogs", "{fido}", "MyPeople" });
            var keySegment = path[1].As<KeySegment>();
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            keypair.Key.Should().Be("ID");
            keypair.Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression() { LiteralText = "{fido}", ExpectedType = keySegment.EdmType.As<IEdmEntityType>().DeclaredKey.Single().Type });
        }
        #endregion

        #region Short types
        [TestMethod]
        public void TestIntArgumentOnByteParameter()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.IsOlderThanByte(age=123)" });
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForIsOlderThanByte());
            var constantNode = path[2].As<OperationSegment>().Parameters.Single().Value.As<ConstantNode>();
            constantNode.Value.Should().Be((byte)123);
            constantNode.TypeReference.FullName().Should().Be("Edm.Byte");
        }

        [TestMethod]
        public void TestIntArgumentOnSByteParameter()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.IsOlderThanSByte(age=-128)" });
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForIsOlderThanSByte());
            var constantNode = path[2].As<OperationSegment>().Parameters.Single().Value.As<ConstantNode>();
            constantNode.Value.Should().Be((sbyte)-128);
            constantNode.TypeReference.FullName().Should().Be("Edm.SByte");
        }

        [TestMethod]
        public void TestIntArgumentOnShortParameter()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.IsOlderThanShort(age=12345)" });
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForIsOlderThanShort());
            var constantNode = path[2].As<OperationSegment>().Parameters.Single().Value.As<ConstantNode>();
            constantNode.Value.Should().Be((short)12345);
            constantNode.TypeReference.FullName().Should().Be("Edm.Int16");
        }

        [TestMethod]
        public void TestOverflowIntArgumentOnShortParameter()
        {
            // short.MaxValue + 1 = 32768
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.IsOlderThanShort(age=32768)" });
            parsePath.ShouldThrow<ODataException>().WithMessage(ErrorStrings.MetadataBinder_CannotConvertToType("Edm.Int32", "Edm.Int16"));
        }

        [TestMethod]
        public void TestSingleArgumentOnSingleParameter()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.IsOlderThanSingle(age=123.456)" });
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForIsOlderThanSingle());
            var constantNode = path[2].As<OperationSegment>().Parameters.Single().Value.As<ConstantNode>();
            constantNode.Value.Should().Be((float)123.456);
            constantNode.TypeReference.FullName().Should().Be("Edm.Single");
        }

        [TestMethod]
        public void TestDoubleArgumentOnSingleParameter()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.IsOlderThanSingle(age=123.45678987)" });
            parsePath.ShouldThrow<ODataException>().WithMessage(ErrorStrings.MetadataBinder_CannotConvertToType("Edm.Double", "Edm.Single"));
        }
        #endregion

        [TestMethod]
        public void ActionShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "Dogs(1)", "Fully.Qualified.Namespace.Walk" });
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetDogWalkAction());
        }

        [TestMethod]
        public void ActionBoundToPrimitiveTypeShouldThrow()
        {
            Action bindToPrimitiveType = () => this.testSubject.ParsePath(new[] { "Dogs(1)", "Color", "ChangeOwner" });
            bindToPrimitiveType.ShouldThrow<ODataException>().WithMessage(ErrorStrings.RequestUriProcessor_ValueSegmentAfterScalarPropertySegment("Color", "ChangeOwner"));
        }

        [TestMethod]
        public void CannotCallFunctionInOpenTypeSpace()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Paintings(0)", "OpenProperty", "Fully.Qualified.Namespace.FindMyOwner(dogsName='fido')" });
            parsePath.ShouldThrow<ODataException>().WithMessage(ErrorStrings.FunctionCallBinder_CallingFunctionOnOpenProperty("Fully.Qualified.Namespace.FindMyOwner"));
        }

        [TestMethod]
        public void UriParserExceptionDataTest()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Dogs(1)", "UNKNOW", "$ref" });
            ODataUnrecognizedPathException ex = parsePath.ShouldThrow<ODataUnrecognizedPathException>().And;
            ex.ParsedSegments.As<IEnumerable<ODataPathSegment>>().Count().Should().Be(2);
            ex.CurrentSegment.Should().Be("UNKNOW");
            ex.UnparsedSegments.As<IEnumerable<string>>().Count().Should().Be(1);
        }

        [TestMethod]
        public void LiteralTextShouldNeverBeNullForConstantNodeOfInt()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "GetPet1(id=1)" });
            path[0].As<OperationImportSegment>().Parameters.Single().Value.As<ConstantNode>().LiteralText.Should().Be("1");
        }

        [TestMethod]
        public void LiteralTextShouldNeverBeNullForConstantNodeOfNullableInt()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "GetCoolestPersonWithStyle(styleID=1)" });
            path[0].As<OperationImportSegment>().Parameters.Single().Value.As<ConstantNode>().LiteralText.Should().Be("1");
        }

        [TestMethod]
        public void LiteralTextShouldNeverBeNullForConstantNodeOfDouble()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "GetPet3(id=1.5)" });
            path[0].As<OperationImportSegment>().Parameters.Single().Value.As<ConstantNode>().LiteralText.Should().Be("1.5");
        }

        [TestMethod]
        public void LiteralTextShouldNeverBeNullForConstantNodeOfBoolean()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "GetPet5(id=true)" });
            path[0].As<OperationImportSegment>().Parameters.Single().Value.As<ConstantNode>().LiteralText.Should().Be("true");
        }

        [TestMethod]
        public void LiteralTextShouldNeverBeNullForConstantNodeOfString()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "FindMyOwner(dogsName='myDog')" });
            path[0].As<OperationImportSegment>().Parameters.Single().Value.As<ConstantNode>().LiteralText.Should().Be("'myDog'");
        }

        [TestMethod]
        public void ParseEnumAsFunctionParameterShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "TryGetPetCount(colorPattern=Fully.Qualified.Namespace.ColorPattern'Red')" });
        }

        [TestMethod]
        public void ParseCountAfterSinglePrimitiveShouldFail()
        {
            Action parse = () => this.testSubject.ParsePath(new[] { "Lions(ID1=1,ID2=2)", "AngerLevel", "$count" });
            parse.ShouldThrow<ODataUnrecognizedPathException>().WithMessage(Strings.RequestUriProcessor_ValueSegmentAfterScalarPropertySegment("AngerLevel", "$count"));
        }

        [TestMethod]
        public void ParseCountAfterSingleEnumShouldFail()
        {
            Action parse = () => this.testSubject.ParsePath(new[] { "Pet2Set(1)", "PetColorPattern", "$count" });
            parse.ShouldThrow<ODataUnrecognizedPathException>().WithMessage("The request URI is not valid. $count cannot be applied to the segment 'PetColorPattern' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [TestMethod]
        public void ParseCountAfterSingleComplexShouldFail()
        {
            Action parse = () => this.testSubject.ParsePath(new[] { "People(1)", "MyAddress", "$count" });
            parse.ShouldThrow<ODataUnrecognizedPathException>().WithMessage("The request URI is not valid. $count cannot be applied to the segment 'MyAddress' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [TestMethod]
        public void ParseCountAfterSingleDerivedComplexShouldFail()
        {
            Action parse = () => this.testSubject.ParsePath(new[] { "People(1)", "MyAddress", "Fully.Qualified.Namespace.HomeAddress", "$count" });
            parse.ShouldThrow<ODataUnrecognizedPathException>().WithMessage("The request URI is not valid. $count cannot be applied to the segment 'Fully.Qualified.Namespace.HomeAddress' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [TestMethod]
        public void ParseCountAfterSingleEntityShouldFail()
        {
            Action parse = () => this.testSubject.ParsePath(new[] { "People(1)", "$count" });
            parse.ShouldThrow<ODataUnrecognizedPathException>().WithMessage("The request URI is not valid. $count cannot be applied to the segment 'People' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [TestMethod]
        public void ParseCountAfterSingleDerivedEntityShouldFail()
        {
            Action parse = () => this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.Employee", "$count" });
            parse.ShouldThrow<ODataUnrecognizedPathException>().WithMessage("The request URI is not valid. $count cannot be applied to the segment 'Fully.Qualified.Namespace.Employee' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [TestMethod]
        public void ParseCountAfterCollectionOfPrimitiveShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "Lions(ID1=1,ID2=2)", "AttackDates", "$count" });
            path.Count.Should().Be(4);
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetLionSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID1", 1), new KeyValuePair<string, object>("ID2", 2));
            path[2].ShouldBePropertySegment(HardCodedTestModel.GetLionAttackDatesProp());
            path[3].ShouldBeCountSegment();
        }

        [TestMethod]
        public void ParseCountAfterCollectionOfComplexShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "PreviousAddresses", "$count" });
            path.Count.Should().Be(4);
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBePropertySegment(HardCodedTestModel.GetPersonPreviousAddressesProp());
            path[3].ShouldBeCountSegment();
        }

        [TestMethod]
        public void ParseCountAfterCollectionOfEnumShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "FavoriteColors", "$count" });
            path.Count.Should().Be(4);
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBePropertySegment(HardCodedTestModel.GetPersonFavoriteColorsProp());
            path[3].ShouldBeCountSegment();
        }

        [TestMethod]
        public void ParseCountAfterCollectionOfEntityShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "MyFriendsDogs", "$count" });
            path.Count.Should().Be(4);
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBeNavigationPropertySegment(HardCodedTestModel.GetPersonMyFriendsDogsProp());
            path[3].ShouldBeCountSegment();
        }

        [TestMethod]
        public void ParseCountAfterCollectionOfDerivedEntityShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "Dogs(1)", "MyPeople", "Fully.Qualified.Namespace.Employee", "$count" });
            path.Count.Should().Be(5);
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetDogsSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBeNavigationPropertySegment(HardCodedTestModel.GetDogMyPeopleNavProp());
            path[3].ShouldBeTypeSegment(new EdmCollectionType(HardCodedTestModel.GetEmployeeTypeReference()));
            path[4].ShouldBeCountSegment();
        }

        [TestMethod]
        public void ParseCountAfterUnboundFunctionReturnsCollectionOfDerivedEntityShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "GetCoolPeople(id=1,limit=1)", "Fully.Qualified.Namespace.Employee", "$count" });
            path.Count.Should().Be(3);
            path[0].ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetCoolPeople());
            path[1].ShouldBeTypeSegment(new EdmCollectionType(HardCodedTestModel.GetEmployeeTypeReference()));
            path[2].ShouldBeCountSegment();
        }

        [TestMethod]
        public void ParseCountAfterUnboundFunctionReturnsCollectionOfPrimitiveShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "GetSomeNumbers", "$count" });
            path.Count.Should().Be(2);
            path[0].ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetSomeNumbers());
            path[1].ShouldBeCountSegment();
        }

        [TestMethod]
        public void ParseCountAfterUnboundFunctionReturnsCollectionOfComplexShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "GetSomeAddresses", "$count" });
            path.Count.Should().Be(2);
            path[0].ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetSomeAddresses());
            path[1].ShouldBeCountSegment();
        }

        [TestMethod]
        public void ParseCountAfterUnboundFunctionReturnsCollectionOfEnumShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "GetSomeColors", "$count" });
            path.Count.Should().Be(2);
            path[0].ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetSomeColors());
            path[1].ShouldBeCountSegment();
        }

        [TestMethod]
        public void ParseCountAfterBoundFunctionReturnsCollectionOfPrimitiveShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.GetPriorAddresses", "$count" });
            path.Count.Should().Be(4);
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForGetPriorAddresses());
            path[3].ShouldBeCountSegment();
        }

        [TestMethod]
        public void ParseCountAfterBoundFunctionReturnsCollectionOfComplexShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.GetPriorNumbers", "$count" });
            path.Count.Should().Be(4);
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForGetPriorNumbers());
            path[3].ShouldBeCountSegment();
        }

        [TestMethod]
        public void ParseCountAfterBoundFunctionReturnsCollectionOfDerivedEntityShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.GetHotPeople(limit=1)", "Fully.Qualified.Namespace.Employee", "$count" });
            path.Count.Should().Be(5);
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForGetHotPeople());
            path[3].ShouldBeTypeSegment(new EdmCollectionType(HardCodedTestModel.GetEmployeeTypeReference()));
            path[4].ShouldBeCountSegment();
        }

        [TestMethod]
        public void ParseCountAfterBoundFunctionReturnsCollectionOfEnumShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.GetFavoriteColors", "$count" });
            path.Count.Should().Be(4);
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForGetFavoriteColors());
            path[3].ShouldBeCountSegment();
        }

        [TestMethod]
        public void ParseCountAfterUnboundFunctionReturnsSinglePrimitiveShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "GetSomeNumber", "$count" });
            parsePath.ShouldThrow<ODataException>().WithMessage(ErrorStrings.RequestUriProcessor_ValueSegmentAfterScalarPropertySegment("GetSomeNumber", "$count"));
        }

        [TestMethod]
        public void ParseCountAfterUnboundFunctionReturnsSingleComplexShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "GetSomeAddress", "$count" });
            parsePath.ShouldThrow<ODataUnrecognizedPathException>().WithMessage("The request URI is not valid. $count cannot be applied to the segment 'GetSomeAddress' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [TestMethod]
        public void ParseCountAfterUnboundFunctionReturnsSingleDerivedComplexShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "GetSomeAddress", "Fully.Qualified.Namespace.HomeAddress", "$count" });
            parsePath.ShouldThrow<ODataUnrecognizedPathException>().WithMessage("The request URI is not valid. $count cannot be applied to the segment 'Fully.Qualified.Namespace.HomeAddress' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [TestMethod]
        public void ParseCountAfterUnboundFunctionReturnsSingleEnumShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "GetSomeColor", "$count" });
            parsePath.ShouldThrow<ODataUnrecognizedPathException>().WithMessage("The request URI is not valid. $count cannot be applied to the segment 'GetSomeColor' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [TestMethod]
        public void ParseCountAfterUnboundFunctionReturnsSingleEntityShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "GetCoolestPerson", "$count" });
            parsePath.ShouldThrow<ODataUnrecognizedPathException>().WithMessage("The request URI is not valid. $count cannot be applied to the segment 'GetCoolestPerson' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [TestMethod]
        public void ParseCountAfterUnboundFunctionReturnsSingleDerivedEntityShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "GetCoolestPerson", "Fully.Qualified.Namespace.Employee", "$count" });
            parsePath.ShouldThrow<ODataUnrecognizedPathException>().WithMessage("The request URI is not valid. $count cannot be applied to the segment 'Fully.Qualified.Namespace.Employee' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [TestMethod]
        public void ParseCountAfterBoundFunctionReturnsSinglePrimitiveShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.HasJob", "$count" });
            parsePath.ShouldThrow<ODataException>().WithMessage(ErrorStrings.RequestUriProcessor_MustBeLeafSegment("Fully.Qualified.Namespace.HasJob"));
        }

        [TestMethod]
        public void ParseCountAfterBoundFunctionReturnsSingleComplexShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.GetPriorAddress", "$count" });
            parsePath.ShouldThrow<ODataException>().WithMessage("The request URI is not valid. $count cannot be applied to the segment 'Fully.Qualified.Namespace.GetPriorAddress' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [TestMethod]
        public void ParseCountAfterBoundFunctionReturnsSingleEnumShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.GetFavoriteColor", "$count" });
            parsePath.ShouldThrow<ODataUnrecognizedPathException>().WithMessage("The request URI is not valid. $count cannot be applied to the segment 'Fully.Qualified.Namespace.GetFavoriteColor' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [TestMethod]
        public void ParseCountAfterBoundFunctionReturnsSingleEntityShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.GetMyDog", "$count" });
            parsePath.ShouldThrow<ODataUnrecognizedPathException>().WithMessage("The request URI is not valid. $count cannot be applied to the segment 'Fully.Qualified.Namespace.GetMyDog' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [TestMethod]
        public void ParseCountAfterNonComposableFunctionShouldFail()
        {
            var point = GeographyPoint.Create(1, 2);
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.GetNearbyPriorAddresses(currentLocation=geography'" + SpatialHelpers.WriteSpatial(point) + "',limit=null)", "$count" });
            parsePath.ShouldThrow<ODataException>().WithMessage(ErrorStrings.RequestUriProcessor_MustBeLeafSegment("Fully.Qualified.Namespace.GetNearbyPriorAddresses"));
        }

        [TestMethod]
        public void ParseCountAfterActionShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Dogs(1)", "Fully.Qualified.Namespace.Walk", "$count" });
            parsePath.ShouldThrow<ODataException>().WithMessage(ErrorStrings.RequestUriProcessor_MustBeLeafSegment("Fully.Qualified.Namespace.Walk"));
        }

        [TestMethod]
        public void ParseCountAfterSingletonShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Boss", "$count" });
            parsePath.ShouldThrow<ODataUnrecognizedPathException>().WithMessage("The request URI is not valid. $count cannot be applied to the segment 'Boss' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [TestMethod]
        public void ParseCountAfterServiceRootShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "$count" });
            parsePath.ShouldThrow<ODataUnrecognizedPathException>().WithMessage("The request URI is not valid, the segment $count cannot be applied to the root of the service.");
        }

        [TestMethod]
        public void ParseBoundFunctionWithTypeDefinitionAsParameterAndReturnTypeShouldWork()
        {
            var path = this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.GetFullName(nickname='abc')" });
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForGetFullName());
            path[2].As<OperationSegment>().Parameters.Single().As<OperationSegmentParameter>().Value.As<ConvertNode>().Source.As<ConstantNode>().Value.Should().Be("abc");
        }

        private static void ExtractSegmentIdentifierAndParenthesisExpression(string segment, string expectedIdentifier, string expectedQueryPortion)
        {
            string actualIdentifier;
            string queryPortion;
            ODataPathParser.ExtractSegmentIdentifierAndParenthesisExpression(segment, out actualIdentifier, out queryPortion);
            actualIdentifier.Should().Be(expectedIdentifier);
            queryPortion.Should().Be(expectedQueryPortion);
        }
    }
}
