//---------------------------------------------------------------------
// <copyright file="ODataUriParserUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;
using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Test.OData.Query.TDD.Tests
{
    [TestClass]
    public class UriTemplateTests
    {
        #region Key template tests
        [TestMethod]
        public void ParseKeyTemplateWithTemplateParser()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People({1})", UriKind.Relative))
            {
                EnableUriTemplateParsing = true
            };

            var path = uriParser.ParsePath();

            var keySegment = path.LastSegment.As<KeySegment>();
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            keypair.Key.Should().Be("ID");

            keypair.Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression { LiteralText = "{1}", ExpectedType = keySegment.EdmType.As<IEdmEntityType>().DeclaredKey.Single().Type });
        }

        [TestMethod]
        public void ParseKeyTemplateWithTemplateParserWithWithWhitespaceInsideTemplateExpressionTest()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People({ 1  })"))
            {
                EnableUriTemplateParsing = true
            };

            var path = uriParser.ParsePath();
            var keySegment = path.LastSegment.As<KeySegment>();
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            keypair.Key.Should().Be("ID");
            keypair.Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression { LiteralText = "{ 1  }", ExpectedType = keySegment.EdmType.As<IEdmEntityType>().DeclaredKey.Single().Type });
        }

        [TestMethod]
        public void ParseKeyTemplateWithTemplateParserWithWithWhitespaceOutsideofTempateExpressionTest()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People(  {1} )", UriKind.Relative))
            {
                EnableUriTemplateParsing = true
            };

            var path = uriParser.ParsePath();
            var keySegment = path.LastSegment.As<KeySegment>();
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            keypair.Key.Should().Be("ID");
            keypair.Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression { LiteralText = "{1}", ExpectedType = keySegment.EdmType.As<IEdmEntityType>().DeclaredKey.Single().Type });
        }

        [TestMethod]
        public void ParseKeyTemplateWithNonTemplateParserShouldThrow()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People({1})"));
            Action action = () => uriParser.ParsePath();
            action.ShouldThrow<ODataException>().WithMessage(Strings.RequestUriProcessor_SyntaxError);
        }

        [TestMethod]
        public void ParseKeyTemplateAsSegmentWithTemplateParser()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People/{1}"))
            {
                EnableUriTemplateParsing = true,
                UrlConventions = ODataUrlConventions.KeyAsSegment
            };

            var path = uriParser.ParsePath();
            var keySegment = path.LastSegment.As<KeySegment>();
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            keypair.Key.Should().Be("ID");
            keypair.Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression { LiteralText = "{1}", ExpectedType = keySegment.EdmType.As<IEdmEntityType>().DeclaredKey.Single().Type });
        }

        [TestMethod]
        public void ParseKeyTemplateAsSegmentTemplateWithNonTemplateParserShouldThrow()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People/{1}"))
            {
                UrlConventions = ODataUrlConventions.KeyAsSegment
            };

            Action action = () => uriParser.ParsePath();
            action.ShouldThrow<ODataException>().WithMessage(Strings.RequestUriProcessor_SyntaxError);
        }

        [TestMethod]
        public void ParseMultiKeyTemplateWithTemplateParser()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("Chimeras(  Name={NAME},Gid =  {GID},  Rid={RID}, Upgraded={UPGRADE})", UriKind.Relative))
            {
                EnableUriTemplateParsing = true
            };

            var path = uriParser.ParsePath();
            var keySegment = path.LastSegment.As<KeySegment>();
            IList<KeyValuePair<string, object>> keys = keySegment.Keys.ToList();
            List<IEdmStructuralProperty> keyTypes = keySegment.EdmType.As<IEdmEntityType>().DeclaredKey.ToList();

            keys.Count.Should().Be(4);
            keys[0].Key.Should().Be("Rid");
            keys[0].Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression { LiteralText = "{RID}", ExpectedType = keyTypes[0].Type });
            keys[1].Key.Should().Be("Gid");
            keys[1].Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression { LiteralText = "{GID}", ExpectedType = keyTypes[1].Type });
            keys[2].Key.Should().Be("Name");
            keys[2].Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression { LiteralText = "{NAME}", ExpectedType = keyTypes[2].Type });
            keys[3].Key.Should().Be("Upgraded");
            keys[3].Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression { LiteralText = "{UPGRADE}", ExpectedType = keyTypes[3].Type });
        }
        #endregion

        #region Parameter template tests
        [TestMethod]
        public void ParseParameterTemplateWithTemplateParser()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People(1)/Fully.Qualified.Namespace.HasHat(onCat={why555})"))
            {
                EnableUriTemplateParsing = true
            };

            IEdmFunction function = HardCodedTestModel.TestModel.FindOperations("Fully.Qualified.Namespace.HasHat").Single(f => f.Parameters.Count() == 2).As<IEdmFunction>();
            var path = uriParser.ParsePath();
            OperationSegmentParameter parameter = path.LastSegment.ShouldBeOperationSegment(function).And.Parameters.Single();
            parameter.ShouldBeConstantParameterWithValueType("onCat", new UriTemplateExpression { LiteralText = "{why555}", ExpectedType = function.Parameters.Last().Type });
        }

        [TestMethod]
        public void ParseParameterTemplateOfEnumTypeWithTemplateParser()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("GetPetCount(colorPattern={COLOR})", UriKind.Relative))
            {
                EnableUriTemplateParsing = true
            };

            IEdmOperationImport operationImport = HardCodedTestModel.TestModel.EntityContainer.FindOperationImports("GetPetCount").Single();
            var path = uriParser.ParsePath();
            OperationSegmentParameter parameter = path.LastSegment.ShouldBeOperationImportSegment(operationImport).And.Parameters.Single();
            parameter.ShouldBeConstantParameterWithValueType("colorPattern", new UriTemplateExpression { LiteralText = "{COLOR}", ExpectedType = operationImport.Operation.Parameters.Single().Type });
        }

        [TestMethod]
        public void ParseParameterTemplateWithNonTemplateParserShouldThrow()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People(1)/Fully.Qualified.Namespace.HasHat(onCat={why555})"));
            Action action = () => uriParser.ParsePath();
            action.ShouldThrow<ODataException>().WithMessage(Strings.JsonReader_MissingColon("why555"));
        }

        [TestMethod]
        public void ParseMultiParameterTemplateWithTemplateParser()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People(1)/Fully.Qualified.Namespace.HasDog(name={NAME} ,inOffice={INOFFICE})"))
            {
                EnableUriTemplateParsing = true
            };

            var operation = HardCodedTestModel.GetHasDogOverloadForPeopleWithThreeParameters();
            var typesDic = operation.Parameters.ToDictionary(_ => _.Name, _ => _.Type);

            var path = uriParser.ParsePath();
            IList<OperationSegmentParameter> parameters = path.LastSegment.ShouldBeOperationSegment(operation).And.Parameters.ToList();
            parameters[0].ShouldBeConstantParameterWithValueType("name", new UriTemplateExpression { LiteralText = "{NAME}", ExpectedType = typesDic["name"] });
            parameters[1].ShouldBeConstantParameterWithValueType("inOffice", new UriTemplateExpression { LiteralText = "{INOFFICE}", ExpectedType = typesDic["inOffice"] });
        }

        [TestMethod]
        public void ParseParameterTemplateForFunctionImportWithTemplateParser()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("IsAddressGood(address={ADDR})", UriKind.Relative))
            {
                EnableUriTemplateParsing = true
            };

            IEdmFunctionImport functionImport = HardCodedTestModel.TestModel.EntityContainer.FindOperationImports("IsAddressGood").Single().As<IEdmFunctionImport>();
            var path = uriParser.ParsePath();
            OperationSegmentParameter parameter = path.LastSegment.ShouldBeOperationImportSegment(functionImport).And.Parameters.Single();
            parameter.ShouldBeConstantParameterWithValueType("address", new UriTemplateExpression { LiteralText = "{ADDR}", ExpectedType = functionImport.Function.Parameters.Single().Type });
        }
        #endregion

        #region Path segment template tests
        [TestMethod]
        public void ParsePathTemplateSegmentWithTemplateParser()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People({1})/{some}", UriKind.Relative))
            {
                EnableUriTemplateParsing = true
            };

            var paths = uriParser.ParsePath().ToList();
            
            var keySegment = paths[1].As<KeySegment>();
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            keypair.Key.Should().Be("ID");
            keypair.Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression { LiteralText = "{1}", ExpectedType = keySegment.EdmType.As<IEdmEntityType>().DeclaredKey.Single().Type });

            var templateSegment = paths[2].As<PathTemplateSegment>();
            templateSegment.LiteralText.Should().Be("{some}");
        }

        [TestMethod]
        public void PathTemplateSegmentShouldBeLastSegment()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People({1})/{some}/what", UriKind.Relative))
            {
                EnableUriTemplateParsing = true
            };

            Action action = () => uriParser.ParsePath();
            action.ShouldThrow<ODataException>().WithMessage(Strings.RequestUriProcessor_MustBeLeafSegment("{some}"));
        }

        [TestMethod]
        public void PathTemplateSegmentShouldNotBeFirstSegment()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("{some}", UriKind.Relative))
            {
                EnableUriTemplateParsing = true
            };

            Action action = () => uriParser.ParsePath();
            action.ShouldThrow<ODataException>().WithMessage(Strings.RequestUriProcessor_ResourceNotFound("{some}"));
        }
        #endregion

        #region Mixed template tests
        [TestMethod]
        public void ParseMixedKeyAndParameterTemplateWithTemplateParser()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People({KEY})/Fully.Qualified.Namespace.HasDog(name={NAME} ,inOffice={INOFFICE})"))
            {
                EnableUriTemplateParsing = true
            };

            var operation = HardCodedTestModel.GetHasDogOverloadForPeopleWithThreeParameters();
            var typesDic = operation.Parameters.ToDictionary(_ => _.Name, _ => _.Type);

            var path = uriParser.ParsePath();
            var keySegment = path.ElementAt(1).As<KeySegment>();
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            keypair.Key.Should().Be("ID");
            keypair.Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression { LiteralText = "{KEY}", ExpectedType = keySegment.EdmType.As<IEdmEntityType>().DeclaredKey.Single().Type });
            IList<OperationSegmentParameter> parameters = path.LastSegment.ShouldBeOperationSegment(operation).And.Parameters.ToList();
            parameters[0].ShouldBeConstantParameterWithValueType("name", new UriTemplateExpression { LiteralText = "{NAME}", ExpectedType = typesDic["name"] });
            parameters[1].ShouldBeConstantParameterWithValueType("inOffice", new UriTemplateExpression { LiteralText = "{INOFFICE}", ExpectedType = typesDic["inOffice"] });
        }
        #endregion

        #region Validation tests
        [TestMethod]
        public void ValidTemplateInputShouldWork()
        {
            var inputs = new[]
            {
                "{}"        , "{_12}"       , "{12}"        , "{j}"         , "{count}"         ,
                "{temp1}"   , "{top_of_p}"  , "{skip12}"    , "{LastNum}"   , "{_1V!@$%^&*}"    ,
                "{(}"       , "{)}"         , "{)(*&^%$@!}" , "{___}"       , "{_v1V)_}"        ,
                "{變量}"    , "{嵐山}"       , "{춘향전}"    , "{的1_vé@_@}" , "{{}and{}}"       ,
            };

            // Validate template for parameters
            IEdmFunction function = HardCodedTestModel.TestModel.FindOperations("Fully.Qualified.Namespace.HasHat").Single(f => f.Parameters.Count() == 2).As<IEdmFunction>();
            foreach (var input in inputs)
            {
                var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People(1)/Fully.Qualified.Namespace.HasHat(onCat=" + input + ")", UriKind.Relative))
                {
                    EnableUriTemplateParsing = true
                };

                var path = uriParser.ParsePath();
                OperationSegmentParameter parameter = path.LastSegment.ShouldBeOperationSegment(function).And.Parameters.Single();
                parameter.ShouldBeConstantParameterWithValueType("onCat", new UriTemplateExpression { LiteralText = input, ExpectedType = function.Parameters.Last().Type });
            }

            // Validate template for keys
            foreach (var input in inputs)
            {
                var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People(" + input + ")"))
                {
                    EnableUriTemplateParsing = true
                };

                var path = uriParser.ParsePath();
                var keySegment = path.LastSegment.As<KeySegment>();
                KeyValuePair<string, object> keypair = keySegment.Keys.Single();
                keypair.Key.Should().Be("ID");
                keypair.Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression { LiteralText = input, ExpectedType = keySegment.EdmType.As<IEdmEntityType>().DeclaredKey.Single().Type });
            }
        }

        [TestMethod]
        public void ErrorParameterTemplateInputShouldThrow()
        {
            var errorCases = new[]
            {
                new {Input = "{"    , Error = Strings.ExpressionLexer_UnbalancedBracketExpression},
                new {Input = "}"    , Error = Strings.ExpressionLexer_InvalidCharacter("}", 6, "onCat=}")},
                new {Input = "{}1"  , Error = Strings.ExpressionLexer_SyntaxError(9, "onCat={}1")},
                new {Input = "}{"   , Error = Strings.ExpressionLexer_InvalidCharacter("}", 6, "onCat=}{")},
                new {Input = "{{}"  , Error = Strings.ExpressionLexer_UnbalancedBracketExpression}, // Thrown by ODataPathParser::TryBindingParametersAndMatchingOperation
                new {Input = "{}}"  , Error = Strings.ExpressionLexer_InvalidCharacter("}", 8, "onCat={}}")},
                new {Input = "{#}"  , Error = Strings.RequestUriProcessor_SyntaxError},
            };

            foreach (var errorCase in errorCases)
            {
                var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People(1)/Fully.Qualified.Namespace.HasHat(onCat=" + errorCase.Input + ")", UriKind.Relative))
                {
                    EnableUriTemplateParsing = true
                };

                Action action = () => uriParser.ParsePath();
                action.ShouldThrow<ODataException>().WithMessage(errorCase.Error);
            }
        }

        [TestMethod]
        public void ErrorKeyTemplateInputShouldThrow()
        {
            var errorCases = new[]
            {
                new {Input = "{"    , Error = Strings.ExpressionLexer_UnbalancedBracketExpression},
                new {Input = "}"    , Error = Strings.ExpressionLexer_InvalidCharacter("}", 1, "(})")},
                new {Input = "{}1"  , Error = Strings.UriQueryExpressionParser_CloseParenOrCommaExpected(3, "({}1)")}, //Strings.BadRequest_KeyCountMismatch("Fully.Qualified.Namespace.Person")},
                new {Input = "}{"   , Error = Strings.ExpressionLexer_InvalidCharacter("}", 1, "(}{)")},
                new {Input = "{{}"  , Error = Strings.ExpressionLexer_UnbalancedBracketExpression}, // Thrown by ODataPathParser::TryBindKeyFromParentheses
                new {Input = "{}}"  , Error = Strings.ExpressionLexer_InvalidCharacter("}", 3, "({}})")},
                new {Input = "{#}"  , Error = Strings.RequestUriProcessor_SyntaxError},
            };

            foreach (var errorCase in errorCases)
            {
                var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People(" + errorCase.Input + ")"))
                {
                    EnableUriTemplateParsing = true
                };

                Action action = () => uriParser.ParsePath();
                action.ShouldThrow<ODataException>().WithMessage(errorCase.Error);
            }
        }
        #endregion
    }
}
