//---------------------------------------------------------------------
// <copyright file="ODataPathParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Tests.ScenarioTests.UriBuilder;
using Microsoft.OData.UriParser;
using Microsoft.Spatial;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser.Parsers
{
    public class ODataPathParserTests
    {
        private readonly Func<string, BatchReferenceSegment> callback = s => new BatchReferenceSegment(s, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
        private readonly ODataPathParser testSubject = new ODataPathParser(new ODataUriParserConfiguration(HardCodedTestModel.TestModel));
        private readonly ODataPathParser templateParser = new ODataPathParser(new ODataUriParserConfiguration(HardCodedTestModel.TestModel) { EnableUriTemplateParsing = true, UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses });
        private readonly ODataPathParser keyAsSegmentTemplateParser = new ODataPathParser(new ODataUriParserConfiguration(HardCodedTestModel.TestModel) { EnableUriTemplateParsing = true, UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash });

        [Fact]
        public void CallbackReturnsBatchReferenceSegment()
        {
            ODataPathParser pathParser = new ODataPathParser(new ODataUriParserConfiguration(HardCodedTestModel.TestModel) { BatchReferenceCallback = callback });
            IList<ODataPathSegment> segments = pathParser.ParsePath(new[] { "$0" });
            ODataPathSegment segment = segments.ToList().Single();
            Assert.Equal("$0", segment.Identifier);
            Assert.Same(HardCodedTestModel.GetPeopleSet(), segment.TargetEdmNavigationSource);
            Assert.Same(HardCodedTestModel.GetPersonType(), segment.TargetEdmType);
        }

        [Fact]
        public void RequestUriProcessorExtractSegmentIdentifierTest()
        {
            ExtractSegmentIdentifierAndParenthesisExpression("blah", "blah", null);
            ExtractSegmentIdentifierAndParenthesisExpression("multiple words", "multiple words", null);
            ExtractSegmentIdentifierAndParenthesisExpression("multiple(123)", "multiple", "123");
            ExtractSegmentIdentifierAndParenthesisExpression("multiple(123;321)", "multiple", "123;321");
            ExtractSegmentIdentifierAndParenthesisExpression("set()", "set", string.Empty);
        }

        [Fact]
        public void RequestUriProcessorExtractSegmentIdentifierErrorTest()
        {
            string actualIdentifier;
            string queryPortion;

            Action emptyString = () => ODataPathParser.ExtractSegmentIdentifierAndParenthesisExpression(string.Empty, out actualIdentifier, out queryPortion);
            emptyString.Throws<ODataUnrecognizedPathException>(ErrorStrings.RequestUriProcessor_EmptySegmentInRequestUrl);

            Action noIdentifier = () => ODataPathParser.ExtractSegmentIdentifierAndParenthesisExpression("()", out actualIdentifier, out queryPortion);
            noIdentifier.Throws<ODataUnrecognizedPathException>(ErrorStrings.RequestUriProcessor_EmptySegmentInRequestUrl);

            Action noEndParen = () => ODataPathParser.ExtractSegmentIdentifierAndParenthesisExpression("foo(", out actualIdentifier, out queryPortion);
            noEndParen.Throws<ODataException>(ErrorStrings.RequestUriProcessor_SyntaxError);
        }

        #region $ref cases
        [Fact]
        public void EntityReferenceFollowingEntityCollectionShouldWork()
        {
            this.testSubject.ParsePath(new[] { "People(1)", "$ref" })
                .Last().ShouldBeReferenceSegment(HardCodedTestModel.GetPeopleSet());
        }

        [Fact]
        public void EntityReferenceToNonexistentPropertyShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "foo", "$ref" });
            parsePath.Throws<ODataUnrecognizedPathException>(ErrorStrings.RequestUriProcessor_ResourceNotFound("foo"));
        }

        [Fact]
        public void EntityReferenceToOpenPropertyShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Paintings(1)", "foo", "$ref" });
            parsePath.Throws<ODataException>(ErrorStrings.PathParser_EntityReferenceNotSupported("foo"));
        }

        [Fact]
        public void EntityReferenceToPrimitivePropertyShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "Name", "$ref" });
            parsePath.Throws<ODataUnrecognizedPathException>(ErrorStrings.RequestUriProcessor_ValueSegmentAfterScalarPropertySegment("Name", UriQueryConstants.RefSegment));
        }

        [Fact]
        public void FurtherCompositionAfterEntityReferenceShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "MyDog", "$ref", "MyPeople" });
            parsePath.Throws<ODataUnrecognizedPathException>(ErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [Fact]
        public void FurtherCompositionAfterEntityReferenceWithKeyShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "MyDog", "MyPeople(2)", "$ref", "MyDog" });
            parsePath.Throws<ODataUnrecognizedPathException>(ErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [Fact]
        public void FurtherCompositionAfterEntityCollectionReferenceShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "MyDog", "MyPeople", "$ref", "1" });
            parsePath.Throws<ODataUnrecognizedPathException>(ErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [Fact]
        public void KeyAfterEntityReferenceWithKeyShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "MyDog", "MyPeople(5)", "$ref", "1" });
            parsePath.Throws<ODataUnrecognizedPathException>(ErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [Fact]
        public void KeyAfterEntityReferenceShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "MyDog", "$ref", "5" });
            parsePath.Throws<ODataUnrecognizedPathException>(ErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [Fact]
        public void EntityReferenceAfterEntityReferenceShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "MyDog", "$ref", "MyPeople", "$ref" });
            parsePath.Throws<ODataUnrecognizedPathException>(ErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [Fact]
        public void AnythingAfterCountAfterEntityReferenceShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "MyDog", "MyPeople", "$ref", "$count", "foo" });
            parsePath.Throws<ODataUnrecognizedPathException>(ErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [Fact]
        public void EntityReferenceForOpenPropertyShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Paintings(1)", "foo", "MyDog", "$ref" });
            parsePath.Throws<ODataException>(ErrorStrings.PathParser_EntityReferenceNotSupported("MyDog"));
        }

        [Fact]
        public void EntityReferenceForActionShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Dogs(1)", "Fully.Qualified.Namespace.Walk", "$ref" });
            parsePath.Throws<ODataException>(ErrorStrings.RequestUriProcessor_MustBeLeafSegment("Fully.Qualified.Namespace.Walk"));
        }

        [Fact]
        public void EntityReferenceForKeyShouldWork()
        {
            this.testSubject.ParsePath(new[] { "People(1)", "MyDog", "$ref" })
                .Last().ShouldBeNavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp());
        }

        [Fact]
        public void CountAfterEntityReferenceShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Dogs(1)", "MyPeople", "$ref", "$count" });
            parsePath.Throws<ODataUnrecognizedPathException>(ErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }

        [Fact]
        public void CountAfterEntityReferenceWithKeyShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Dogs(1)", "MyPeople(5)", "$ref", "$count" });
            parsePath.Throws<ODataUnrecognizedPathException>(ErrorStrings.RequestUriProcessor_MustBeLeafSegment(UriQueryConstants.RefSegment));
        }
        #endregion

        [Fact]
        public void ParenthesesAfterCollectionPropertyShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Dogs(1)", "Nicknames()" });
            parsePath.Throws<ODataException>(ErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [Fact]
        public void ParenthesesAfterAnythingThatIsASingleResultShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Dogs(1)", "Color()" });
            parsePath.Throws<ODataException>(ErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [Fact]
        public void SingletonShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "Boss" });
            path[0].ShouldBeSingletonSegment(HardCodedTestModel.GetBossSingleton());
        }

        [Fact]
        public void NavigationInSingletonShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "Boss", "MyDog" });
            Assert.Equal(2, path.Count);
            path[0].ShouldBeSingletonSegment(HardCodedTestModel.GetBossSingleton());
            path[1].ShouldBeNavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp());
            var navigationSegment = Assert.IsType<NavigationPropertySegment>(path[1]);
            Assert.Same(HardCodedTestModel.GetDogsSet(), navigationSegment.NavigationSource);
        }

        [Fact]
        public void SingletonWithKeyShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Boss(1)" });
            parsePath.Throws<ODataException>(ErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [Fact]
        public void FunctionImportShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "FindMyOwner(dogsName='fido')" });
            var parameter = path[0].ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForFindMyOwner()).Parameters.Single();
            var segmentParameter = Assert.IsType<OperationSegmentParameter>(parameter);
            var node = Assert.IsType<ConstantNode>(segmentParameter.Value);
            Assert.Equal("fido", node.Value);
        }

        [Fact]
        public void FunctionImportShouldWorkWithNumericChar()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "FindMyOwner(dogsName='000110011E0124221929')" });
            var parameter = path[0].ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForFindMyOwner()).Parameters.Single();

            var segmentParameter = Assert.IsType<OperationSegmentParameter>(parameter);
            var node = Assert.IsType<ConstantNode>(segmentParameter.Value);
            Assert.Equal("000110011E0124221929", node.Value);
        }

        #region Template cases without KeyAsSgment
        [Fact]
        public void NormalFunctionImportShouldWorkWithTemplateParser()
        {
            IList<ODataPathSegment> path = this.templateParser.ParsePath(new[] { "FindMyOwner(dogsName='fido')" });
            var parameter = path[0].ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForFindMyOwner()).Parameters.Single();

            var segmentParameter = Assert.IsType<OperationSegmentParameter>(parameter);
            var node = Assert.IsType<ConstantNode>(segmentParameter.Value);
            Assert.Equal("fido", node.Value);
        }

        [Fact]
        public void ActionWithTemplateShouldWork()
        {
            IEdmFunction operation = HardCodedTestModel.GetHasDogOverloadForPeopleWithTwoParameters();
            IList<ODataPathSegment> path = this.templateParser.ParsePath(new[] { "Boss", "Fully.Qualified.Namespace.HasDog(inOffice={fido})" });
            OperationSegmentParameter parameter = path[1].ShouldBeOperationSegment(operation).Parameters.Single();
            parameter.ShouldBeConstantParameterWithValueType("inOffice", new UriTemplateExpression { LiteralText = "{fido}", ExpectedType = operation.Parameters.Last().Type });
        }

        [Fact]
        public void FunctionImportWithTemplateShouldWork()
        {
            IEdmOperationImport operationImport = HardCodedTestModel.GetFunctionImportForFindMyOwner();
            IList<ODataPathSegment> path = this.templateParser.ParsePath(new[] { "FindMyOwner(dogsName={fido})" });
            OperationSegmentParameter parameter = path[0].ShouldBeOperationImportSegment(operationImport).Parameters.Single();
            parameter.ShouldBeConstantParameterWithValueType("dogsName", new UriTemplateExpression { LiteralText = "{fido}", ExpectedType = operationImport.Operation.Parameters.Single().Type });
        }

        [Fact]
        public void KeyWithTemplateShouldWork()
        {
            IList<ODataPathSegment> path = this.templateParser.ParsePath(new[] { "Dogs({fido})", "MyPeople" });
            var keySegment = Assert.IsType<KeySegment>(path[1]);
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            Assert.Equal("ID", keypair.Key);
            var uriExpression = Assert.IsType<UriTemplateExpression>(keypair.Value);
            Assert.Equal("{fido}", uriExpression.LiteralText);
            var edmTypeReference = ((IEdmEntityType)keySegment.EdmType).DeclaredKey.Single().Type;
            Assert.True(uriExpression.ExpectedType.IsEquivalentTo(edmTypeReference));
        }

        [Fact]
        public void NamedKeyWithTemplateShouldWork()
        {
            IList<ODataPathSegment> path = this.templateParser.ParsePath(new[] { "Dogs(ID={fido})", "MyPeople" });
            var keySegment = Assert.IsType<KeySegment>(path[1]);
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            Assert.Equal("ID", keypair.Key);
            var uriExpression = Assert.IsType<UriTemplateExpression>(keypair.Value);
            Assert.Equal("{fido}", uriExpression.LiteralText);
            var edmTypeReference = ((IEdmEntityType)keySegment.EdmType).DeclaredKey.Single().Type;
            Assert.True(uriExpression.ExpectedType.IsEquivalentTo(edmTypeReference));
        }

        [Fact]
        public void ParseTemplateWithTypeCastShouldWork()
        {
            IList<ODataPathSegment> path = this.keyAsSegmentTemplateParser.ParsePath(new[] { "Dogs(ID={fido})", "MyPeople", "Fully.Qualified.Namespace.Employee" });
            var keySegment = Assert.IsType<KeySegment>(path[1]);
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            Assert.Equal("ID", keypair.Key);

            var uriExpression = Assert.IsType<UriTemplateExpression>(keypair.Value);
            Assert.Equal("{fido}", uriExpression.LiteralText);
            var edmTypeReference = ((IEdmEntityType)keySegment.EdmType).DeclaredKey.Single().Type;
            Assert.True(uriExpression.ExpectedType.IsEquivalentTo(edmTypeReference));

            var typeSegment = Assert.IsType<TypeSegment>(path[3]);
            Assert.Equal("Fully.Qualified.Namespace.Employee", typeSegment.Identifier);
        }
        #endregion

        #region Template cases with KeyAsSgment
        [Fact]
        public void KeyAsSegmentWithTemplateShouldWork()
        {
            IList<ODataPathSegment> path = this.keyAsSegmentTemplateParser.ParsePath(new[] { "Dogs", "{fido}", "MyPeople" });
            var keySegment = Assert.IsType<KeySegment>(path[1]);
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            Assert.Equal("ID", keypair.Key);
            var uriExpression = Assert.IsType<UriTemplateExpression>(keypair.Value);
            Assert.Equal("{fido}", uriExpression.LiteralText);

            var edmTypeReference = ((IEdmEntityType)keySegment.EdmType).DeclaredKey.Single().Type;
            Assert.True(uriExpression.ExpectedType.IsEquivalentTo(edmTypeReference));
        }

        [Fact]
        public void ParseTemplateWithTypeCastWhenKeyAsSegmentShouldWork()
        {
            IList<ODataPathSegment> path = this.keyAsSegmentTemplateParser.ParsePath(new[] { "Dogs", "{fido}", "MyPeople", "Fully.Qualified.Namespace.Employee" });
            var keySegment = Assert.IsType<KeySegment>(path[1]);
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            Assert.Equal("ID", keypair.Key);

            var uriExpression = Assert.IsType<UriTemplateExpression>(keypair.Value);
            Assert.Equal("{fido}", uriExpression.LiteralText);

            var edmTypeReference = ((IEdmEntityType)keySegment.EdmType).DeclaredKey.Single().Type;
            Assert.True(uriExpression.ExpectedType.IsEquivalentTo(edmTypeReference));

            var typeSegment = Assert.IsType<TypeSegment>(path[3]);
            Assert.Equal("Fully.Qualified.Namespace.Employee", typeSegment.Identifier);
        }

        [Fact]
        public void NormalFunctionImportShouldWorkWithTemplateParserWhenKeyAsSegmentShouldWork()
        {
            IList<ODataPathSegment> path = this.keyAsSegmentTemplateParser.ParsePath(new[] { "FindMyOwner(dogsName='fido')" });
            var parameter = path[0].ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForFindMyOwner()).Parameters.Single();
            var node = Assert.IsType<ConstantNode>(Assert.IsType<OperationSegmentParameter>(parameter).Value);
            Assert.Equal("fido", node.Value);
        }

        [Fact]
        public void ActionWithTemplateShouldWorkWhenKeyAsSegmentShouldWork()
        {
            IEdmFunction operation = HardCodedTestModel.GetHasDogOverloadForPeopleWithTwoParameters();
            IList<ODataPathSegment> path = this.keyAsSegmentTemplateParser.ParsePath(new[] { "Boss", "Fully.Qualified.Namespace.HasDog(inOffice={fido})" });
            OperationSegmentParameter parameter = path[1].ShouldBeOperationSegment(operation).Parameters.Single();
            parameter.ShouldBeConstantParameterWithValueType("inOffice", new UriTemplateExpression { LiteralText = "{fido}", ExpectedType = operation.Parameters.Last().Type });
        }

        [Fact]
        public void FunctionImportWithTemplateShouldWorkWhenKeyAsSegmentShouldWork()
        {
            IEdmOperationImport operationImport = HardCodedTestModel.GetFunctionImportForFindMyOwner();
            IList<ODataPathSegment> path = this.keyAsSegmentTemplateParser.ParsePath(new[] { "FindMyOwner(dogsName={fido})" });
            OperationSegmentParameter parameter = path[0].ShouldBeOperationImportSegment(operationImport).Parameters.Single();
            parameter.ShouldBeConstantParameterWithValueType("dogsName", new UriTemplateExpression { LiteralText = "{fido}", ExpectedType = operationImport.Operation.Parameters.Single().Type });
        }
        #endregion

        #region Short types
        [Fact]
        public void TestIntArgumentOnByteParameter()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.IsOlderThanByte(age=123)" });
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForIsOlderThanByte());
            var constantNode = Assert.IsType<ConstantNode>(Assert.IsType<OperationSegment>(path[2]).Parameters.Single().Value);
            Assert.Equal((byte)123, constantNode.Value);
            Assert.Equal("Edm.Byte", constantNode.TypeReference.FullName());
        }

        [Fact]
        public void TestIntArgumentOnSByteParameter()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.IsOlderThanSByte(age=-128)" });
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForIsOlderThanSByte());
            var constantNode = Assert.IsType<ConstantNode>(Assert.IsType<OperationSegment>(path[2]).Parameters.Single().Value);
            Assert.Equal((sbyte)-128, constantNode.Value);
            Assert.Equal("Edm.SByte", constantNode.TypeReference.FullName());
        }

        [Fact]
        public void TestIntArgumentOnShortParameter()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.IsOlderThanShort(age=12345)" });
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForIsOlderThanShort());
            var constantNode = Assert.IsType<ConstantNode>(Assert.IsType<OperationSegment>(path[2]).Parameters.Single().Value);
            Assert.Equal((short)12345, constantNode.Value);
            Assert.Equal("Edm.Int16", constantNode.TypeReference.FullName());
        }

        [Fact]
        public void TestOverflowIntArgumentOnShortParameter()
        {
            // short.MaxValue + 1 = 32768
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.IsOlderThanShort(age=32768)" });
            parsePath.Throws<ODataException>(ErrorStrings.MetadataBinder_CannotConvertToType("Edm.Int32", "Edm.Int16"));
        }

        [Fact]
        public void TestSingleArgumentOnSingleParameter()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.IsOlderThanSingle(age=123.456)" });
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForIsOlderThanSingle());

            var constantNode = Assert.IsType<ConstantNode>(Assert.IsType<OperationSegment>(path[2]).Parameters.Single().Value);
            Assert.Equal((float)123.456, constantNode.Value);
            Assert.Equal("Edm.Single", constantNode.TypeReference.FullName());
        }

        [Fact]
        public void TestDoubleArgumentOnSingleParameter()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.IsOlderThanSingle(age=123.45678987)" });
            parsePath.Throws<ODataException>(ErrorStrings.MetadataBinder_CannotConvertToType("Edm.Double", "Edm.Single"));
        }
        #endregion

        [Theory]
        [InlineData("Fully.Qualified.Namespace.Walk")]
        [InlineData("MainAlias.Walk")]
        public void ActionShouldWork(string actionSegment)
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "Dogs(1)", actionSegment });
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetDogWalkAction());
        }

        [Fact]
        public void ActionBoundToPrimitiveTypeShouldThrow()
        {
            Action bindToPrimitiveType = () => this.testSubject.ParsePath(new[] { "Dogs(1)", "Color", "ChangeOwner" });
            bindToPrimitiveType.Throws<ODataUnrecognizedPathException>(ErrorStrings.RequestUriProcessor_ValueSegmentAfterScalarPropertySegment("Color", "ChangeOwner"));
        }

        [Fact]
        public void CannotCallFunctionInOpenTypeSpace()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Paintings(0)", "OpenProperty", "Fully.Qualified.Namespace.FindMyOwner(dogsName='fido')" });
            parsePath.Throws<ODataException>(ErrorStrings.FunctionCallBinder_CallingFunctionOnOpenProperty("Fully.Qualified.Namespace.FindMyOwner"));
        }

        [Fact]
        public void UriParserExceptionDataTest()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Dogs(1)", "UNKNOW", "$ref" });
            ODataUnrecognizedPathException ex = Assert.Throws<ODataUnrecognizedPathException>(parsePath);
            Assert.Equal(2, ex.ParsedSegments.Count());
            Assert.Equal("UNKNOW", ex.CurrentSegment);
            Assert.Equal("$ref", Assert.Single(ex.UnparsedSegments));
        }

        [Fact]
        public void LiteralTextShouldNeverBeNullForConstantNodeOfInt()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "GetPet1(id=1)" });
            var operationImportSegment = Assert.IsType<OperationImportSegment>(path[0]);
            var node = Assert.IsType<ConstantNode>(Assert.Single(operationImportSegment.Parameters).Value);
            Assert.Equal("1", node.LiteralText);
        }

        [Fact]
        public void LiteralTextShouldNeverBeNullForConstantNodeOfNullableInt()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "GetCoolestPersonWithStyle(styleID=1)" });

            var operationImportSegment = Assert.IsType<OperationImportSegment>(path[0]);
            var node = Assert.IsType<ConstantNode>(Assert.Single(operationImportSegment.Parameters).Value);
            Assert.Equal("1", node.LiteralText);
        }

        [Fact]
        public void LiteralTextShouldNeverBeNullForConstantNodeOfDouble()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "GetPet3(id=1.5)" });

            var operationImportSegment = Assert.IsType<OperationImportSegment>(path[0]);
            var node = Assert.IsType<ConstantNode>(Assert.Single(operationImportSegment.Parameters).Value);
            Assert.Equal("1.5", node.LiteralText);
        }

        [Fact]
        public void LiteralTextShouldNeverBeNullForConstantNodeOfBoolean()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "GetPet5(id=true)" });

            var operationImportSegment = Assert.IsType<OperationImportSegment>(path[0]);
            var node = Assert.IsType<ConstantNode>(Assert.Single(operationImportSegment.Parameters).Value);
            Assert.Equal("true", node.LiteralText);
        }

        [Fact]
        public void LiteralTextShouldNeverBeNullForConstantNodeOfString()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "FindMyOwner(dogsName='myDog')" });

            var operationImportSegment = Assert.IsType<OperationImportSegment>(path[0]);
            var node = Assert.IsType<ConstantNode>(Assert.Single(operationImportSegment.Parameters).Value);
            Assert.Equal("'myDog'", node.LiteralText);
        }

        [Fact]
        public void ParseEnumAsFunctionParameterShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "TryGetPetCount(colorPattern=Fully.Qualified.Namespace.ColorPattern'Red')" });

            var operationImportSegment = Assert.IsType<OperationImportSegment>(path[0]);
            var node = Assert.IsType<ConstantNode>(Assert.Single(operationImportSegment.Parameters).Value);
            Assert.Equal("Fully.Qualified.Namespace.ColorPattern'1'", node.LiteralText);
        }

        [Fact]
        public void ParseCountAfterSinglePrimitiveShouldFail()
        {
            Action parse = () => this.testSubject.ParsePath(new[] { "Lions(ID1=1,ID2=2)", "AngerLevel", "$count" });
            parse.Throws<ODataUnrecognizedPathException>(Strings.RequestUriProcessor_ValueSegmentAfterScalarPropertySegment("AngerLevel", "$count"));
        }

        [Fact]
        public void ParseCountAfterSingleEnumShouldFail()
        {
            Action parse = () => this.testSubject.ParsePath(new[] { "Pet2Set(1)", "PetColorPattern", "$count" });
            parse.Throws<ODataUnrecognizedPathException>("The request URI is not valid. $count cannot be applied to the segment 'PetColorPattern' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [Fact]
        public void ParseCountAfterSingleComplexShouldFail()
        {
            Action parse = () => this.testSubject.ParsePath(new[] { "People(1)", "MyAddress", "$count" });
            parse.Throws<ODataUnrecognizedPathException>("The request URI is not valid. $count cannot be applied to the segment 'MyAddress' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [Fact]
        public void ParseCountAfterSingleDerivedComplexShouldFail()
        {
            Action parse = () => this.testSubject.ParsePath(new[] { "People(1)", "MyAddress", "Fully.Qualified.Namespace.HomeAddress", "$count" });
            parse.Throws<ODataUnrecognizedPathException>("The request URI is not valid. $count cannot be applied to the segment 'Fully.Qualified.Namespace.HomeAddress' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [Fact]
        public void ParseCountAfterSingleEntityShouldFail()
        {
            Action parse = () => this.testSubject.ParsePath(new[] { "People(1)", "$count" });
            parse.Throws<ODataUnrecognizedPathException>("The request URI is not valid. $count cannot be applied to the segment 'People' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [Fact]
        public void ParseCountAfterSingleDerivedEntityShouldFail()
        {
            Action parse = () => this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.Employee", "$count" });
            parse.Throws<ODataUnrecognizedPathException>("The request URI is not valid. $count cannot be applied to the segment 'Fully.Qualified.Namespace.Employee' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [Fact]
        public void ParseCountAfterCollectionOfPrimitiveShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "Lions(ID1=1,ID2=2)", "AttackDates", "$count" });
            Assert.Equal(4, path.Count);
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetLionSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID1", 1), new KeyValuePair<string, object>("ID2", 2));
            path[2].ShouldBePropertySegment(HardCodedTestModel.GetLionAttackDatesProp());
            path[3].ShouldBeCountSegment();
        }

        [Fact]
        public void ParseCountAfterCollectionOfComplexShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "PreviousAddresses", "$count" });
            Assert.Equal(4, path.Count);
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBePropertySegment(HardCodedTestModel.GetPersonPreviousAddressesProp());
            path[3].ShouldBeCountSegment();
        }

        [Fact]
        public void ParseCountAfterCollectionOfEnumShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "FavoriteColors", "$count" });
            Assert.Equal(4, path.Count);
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBePropertySegment(HardCodedTestModel.GetPersonFavoriteColorsProp());
            path[3].ShouldBeCountSegment();
        }

        [Fact]
        public void ParseCountAfterCollectionOfEntityShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "MyFriendsDogs", "$count" });
            Assert.Equal(4, path.Count);
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBeNavigationPropertySegment(HardCodedTestModel.GetPersonMyFriendsDogsProp());
            path[3].ShouldBeCountSegment();
        }

        [Fact]
        public void ParseCountAfterCollectionOfDerivedEntityShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "Dogs(1)", "MyPeople", "Fully.Qualified.Namespace.Employee", "$count" });
            Assert.Equal(5, path.Count);
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetDogsSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBeNavigationPropertySegment(HardCodedTestModel.GetDogMyPeopleNavProp());
            path[3].ShouldBeTypeSegment(new EdmCollectionType(HardCodedTestModel.GetEmployeeTypeReference()));
            path[4].ShouldBeCountSegment();
        }

        [Fact]
        public void ParseCountAfterUnboundFunctionReturnsCollectionOfDerivedEntityShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "GetCoolPeople(id=1,limit=1)", "Fully.Qualified.Namespace.Employee", "$count" });
            Assert.Equal(3, path.Count);
            path[0].ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetCoolPeople());
            path[1].ShouldBeTypeSegment(new EdmCollectionType(HardCodedTestModel.GetEmployeeTypeReference()));
            path[2].ShouldBeCountSegment();
        }

        [Fact]
        public void ParseCountAfterUnboundFunctionReturnsCollectionOfPrimitiveShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "GetSomeNumbers", "$count" });
            Assert.Equal(2, path.Count);
            path[0].ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetSomeNumbers());
            path[1].ShouldBeCountSegment();
        }

        [Fact]
        public void ParseCountAfterUnboundFunctionReturnsCollectionOfComplexShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "GetSomeAddresses", "$count" });
            Assert.Equal(2, path.Count);
            path[0].ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetSomeAddresses());
            path[1].ShouldBeCountSegment();
        }

        [Fact]
        public void ParseCountAfterUnboundFunctionReturnsCollectionOfEnumShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "GetSomeColors", "$count" });
            Assert.Equal(2, path.Count);
            path[0].ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetSomeColors());
            path[1].ShouldBeCountSegment();
        }

        [Theory]
        [InlineData("Fully.Qualified.Namespace.GetPriorAddresses")]
        [InlineData("MainAlias.GetPriorAddresses")]
        public void ParseCountAfterBoundFunctionReturnsCollectionOfPrimitiveShouldWork(string functionSegment)
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", functionSegment, "$count" });
            Assert.Equal(4, path.Count);
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForGetPriorAddresses());
            path[3].ShouldBeCountSegment();
        }

        [Fact]
        public void ParseCountAfterBoundFunctionReturnsCollectionOfComplexShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.GetPriorNumbers", "$count" });
            Assert.Equal(4, path.Count);
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForGetPriorNumbers());
            path[3].ShouldBeCountSegment();
        }

        [Fact]
        public void ParseCountAfterBoundFunctionReturnsCollectionOfDerivedEntityShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.GetHotPeople(limit=1)", "Fully.Qualified.Namespace.Employee", "$count" });
            Assert.Equal(5, path.Count);
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForGetHotPeople());
            path[3].ShouldBeTypeSegment(new EdmCollectionType(HardCodedTestModel.GetEmployeeTypeReference()));
            path[4].ShouldBeCountSegment();
        }

        [Fact]
        public void ParseCountAfterBoundFunctionReturnsCollectionOfEnumShouldWork()
        {
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.GetFavoriteColors", "$count" });
            Assert.Equal(4, path.Count);
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForGetFavoriteColors());
            path[3].ShouldBeCountSegment();
        }

        [Fact]
        public void ParseCountAfterUnboundFunctionReturnsSinglePrimitiveShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "GetSomeNumber", "$count" });
            parsePath.Throws<ODataUnrecognizedPathException>(ErrorStrings.RequestUriProcessor_ValueSegmentAfterScalarPropertySegment("GetSomeNumber", "$count"));
        }

        [Fact]
        public void ParseCountAfterUnboundFunctionReturnsSingleComplexShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "GetSomeAddress", "$count" });
            parsePath.Throws<ODataUnrecognizedPathException>("The request URI is not valid. $count cannot be applied to the segment 'GetSomeAddress' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [Fact]
        public void ParseCountAfterUnboundFunctionReturnsSingleDerivedComplexShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "GetSomeAddress", "Fully.Qualified.Namespace.HomeAddress", "$count" });
            parsePath.Throws<ODataUnrecognizedPathException>("The request URI is not valid. $count cannot be applied to the segment 'Fully.Qualified.Namespace.HomeAddress' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [Fact]
        public void ParseCountAfterUnboundFunctionReturnsSingleEnumShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "GetSomeColor", "$count" });
            parsePath.Throws<ODataUnrecognizedPathException>("The request URI is not valid. $count cannot be applied to the segment 'GetSomeColor' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [Fact]
        public void ParseCountAfterUnboundFunctionReturnsSingleEntityShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "GetCoolestPerson", "$count" });
            parsePath.Throws<ODataUnrecognizedPathException>("The request URI is not valid. $count cannot be applied to the segment 'GetCoolestPerson' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [Fact]
        public void ParseCountAfterUnboundFunctionReturnsSingleDerivedEntityShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "GetCoolestPerson", "Fully.Qualified.Namespace.Employee", "$count" });
            parsePath.Throws<ODataUnrecognizedPathException>("The request URI is not valid. $count cannot be applied to the segment 'Fully.Qualified.Namespace.Employee' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [Fact]
        public void ParseCountAfterBoundFunctionReturnsSinglePrimitiveShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.HasJob", "$count" });
            parsePath.Throws<ODataException>(ErrorStrings.RequestUriProcessor_MustBeLeafSegment("Fully.Qualified.Namespace.HasJob"));
        }

        [Fact]
        public void ParseCountAfterBoundFunctionReturnsSingleComplexShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.GetPriorAddress", "$count" });
            parsePath.Throws<ODataUnrecognizedPathException>("The request URI is not valid. $count cannot be applied to the segment 'Fully.Qualified.Namespace.GetPriorAddress' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [Fact]
        public void ParseCountAfterBoundFunctionReturnsSingleEnumShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.GetFavoriteColor", "$count" });
            parsePath.Throws<ODataUnrecognizedPathException>("The request URI is not valid. $count cannot be applied to the segment 'Fully.Qualified.Namespace.GetFavoriteColor' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [Fact]
        public void ParseCountAfterBoundFunctionReturnsSingleEntityShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.GetMyDog", "$count" });
            parsePath.Throws<ODataUnrecognizedPathException>("The request URI is not valid. $count cannot be applied to the segment 'Fully.Qualified.Namespace.GetMyDog' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [Fact]
        public void ParseCountAfterNonComposableFunctionShouldFail()
        {
            var point = GeographyPoint.Create(1, 2);
            Action parsePath = () => this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.GetNearbyPriorAddresses(currentLocation=geography'" + SpatialHelpers.WriteSpatial(point) + "',limit=null)", "$count" });
            parsePath.Throws<ODataException>(ErrorStrings.RequestUriProcessor_MustBeLeafSegment("Fully.Qualified.Namespace.GetNearbyPriorAddresses"));
        }

        [Fact]
        public void ParseCountAfterActionShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Dogs(1)", "Fully.Qualified.Namespace.Walk", "$count" });
            parsePath.Throws<ODataException>(ErrorStrings.RequestUriProcessor_MustBeLeafSegment("Fully.Qualified.Namespace.Walk"));
        }

        [Fact]
        public void ParseCountAfterSingletonShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "Boss", "$count" });
            parsePath.Throws<ODataUnrecognizedPathException>("The request URI is not valid. $count cannot be applied to the segment 'Boss' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [Fact]
        public void ParseCountAfterServiceRootShouldFail()
        {
            Action parsePath = () => this.testSubject.ParsePath(new[] { "$count" });
            parsePath.Throws<ODataUnrecognizedPathException>("The request URI is not valid, the segment $count cannot be applied to the root of the service.");
        }

        [Fact]
        public void ParseKeyInParenthesisAfterBoundFunctionReturnsCollectionOfEntitiesShouldWork()
        {
            // People(1)/Fully.Qualified.Namespace.AllMyFriendsDogs(inOffice = true)(42)
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.AllMyFriendsDogs(inOffice=true)(42)" });
            Assert.Equal(4, path.Count);
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForAllMyFriendsDogs(2));
            path[3].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 42));
        }

        [Fact]
        public void ParseKeyAsSegmentAfterBoundFunctionReturnsCollectionOfEntitiesShouldWork()
        {
            // People(1)/Fully.Qualified.Namespace.AllMyFriendsDogs(inOffice = true)/42
            IList<ODataPathSegment> path = this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.AllMyFriendsDogs(inOffice=true)", "42" });
            Assert.Equal(4, path.Count);
            path[0].ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            path[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1));
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForAllMyFriendsDogs(2));
            path[3].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 42));
        }

        [Fact]
        public void ParseBoundFunctionWithTypeDefinitionAsParameterAndReturnTypeShouldWork()
        {
            var path = this.testSubject.ParsePath(new[] { "People(1)", "Fully.Qualified.Namespace.GetFullName(nickname='abc')" });
            path[2].ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForGetFullName());

            var parameterValue = Assert.IsType<OperationSegmentParameter>(Assert.Single(Assert.IsType<OperationSegment>(path[2]).Parameters)).Value;
            var node = Assert.IsType<ConstantNode>(Assert.IsType<ConvertNode>(parameterValue).Source);
            Assert.Equal("abc", node.Value);
        }

        private static void ExtractSegmentIdentifierAndParenthesisExpression(string segment, string expectedIdentifier, string expectedQueryPortion)
        {
            string actualIdentifier;
            string queryPortion;
            ODataPathParser.ExtractSegmentIdentifierAndParenthesisExpression(segment, out actualIdentifier, out queryPortion);
            Assert.Equal(expectedIdentifier, actualIdentifier);
            Assert.Equal(expectedQueryPortion, queryPortion);
        }
    }
}
