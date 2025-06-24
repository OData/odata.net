//---------------------------------------------------------------------
// <copyright file="ODataUriParserUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using Microsoft.OData.Core;
using Microsoft.OData.UriParser.Validation;

namespace Microsoft.OData.Tests.UriParser.Parsers
{
    public class UriTemplateParserTests
    {
        #region Key template tests
        [Fact]
        public void ParseKeyTemplateWithTemplateParser()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People({1})", UriKind.Relative))
            {
                EnableUriTemplateParsing = true
            };

            var path = uriParser.ParsePath();

            var keySegment = Assert.IsType<KeySegment>(path.LastSegment);
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            Assert.Equal("ID", keypair.Key);

            var edmTypeReference = ((IEdmEntityType)keySegment.EdmType).DeclaredKey.Single().Type;
            keypair.Value.ShouldBeUriTemplateExpression("{1}", edmTypeReference);
        }

        [Fact]
        public void ParseKeyTemplateWithTemplateParserWithWithWhitespaceInsideTemplateExpressionTest()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People({ 1  })"))
            {
                EnableUriTemplateParsing = true
            };

            var path = uriParser.ParsePath();
            var keySegment = Assert.IsType<KeySegment>(path.LastSegment);
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            Assert.Equal("ID", keypair.Key);

            var edmTypeReference = ((IEdmEntityType)keySegment.EdmType).DeclaredKey.Single().Type;
            keypair.Value.ShouldBeUriTemplateExpression("{ 1  }", edmTypeReference);
        }

        [Fact]
        public void ParseKeyTemplateWithTemplateParserWithWithWhitespaceOutsideofTempateExpressionTest()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People(  {1} )", UriKind.Relative))
            {
                EnableUriTemplateParsing = true
            };

            var path = uriParser.ParsePath();
            var keySegment = Assert.IsType<KeySegment>(path.LastSegment);
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            Assert.Equal("ID", keypair.Key);

            var edmTypeReference = ((IEdmEntityType)keySegment.EdmType).DeclaredKey.Single().Type;
            keypair.Value.ShouldBeUriTemplateExpression("{1}", edmTypeReference);
        }

        [Fact]
        public void ParseKeyTemplateWithNonTemplateParserShouldThrow()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People({1})"));
            Action action = () => uriParser.ParsePath();
            action.Throws<ODataException>(SRResources.RequestUriProcessor_SyntaxError);
        }

        [Fact]
        public void ParseKeyTemplateAsSegmentWithTemplateParser()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People/{1}"))
            {
                EnableUriTemplateParsing = true,
                UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash
            };

            var path = uriParser.ParsePath();
            var keySegment = Assert.IsType<KeySegment>(path.LastSegment);
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            Assert.Equal("ID", keypair.Key);

            var edmTypeReference = ((IEdmEntityType)keySegment.EdmType).DeclaredKey.Single().Type;
            keypair.Value.ShouldBeUriTemplateExpression("{1}", edmTypeReference);
        }

        [Fact]
        public void ParseKeyTemplateAsSegmentTemplateWithNonTemplateParserShouldThrow()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People/{1}"))
            {
                UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash
            };

            Action action = () => uriParser.ParsePath();
            action.Throws<ODataException>(SRResources.RequestUriProcessor_SyntaxError);
        }

        [Fact]
        public void ParseMultiKeyTemplateWithTemplateParser()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("Chimeras(  Name={NAME},Gid =  {GID},  Rid={RID}, Upgraded={UPGRADE})", UriKind.Relative))
            {
                EnableUriTemplateParsing = true
            };

            var path = uriParser.ParsePath();
            var keySegment = Assert.IsType<KeySegment>(path.LastSegment);
            IList<KeyValuePair<string, object>> keys = keySegment.Keys.ToList();
            List<IEdmStructuralProperty> keyTypes = ((IEdmEntityType)keySegment.EdmType).DeclaredKey.ToList();

            Assert.Equal(4, keys.Count);

            Assert.Equal("Rid", keys[0].Key);
            keys[0].Value.ShouldBeUriTemplateExpression("{RID}", keyTypes[0].Type);

            Assert.Equal("Gid", keys[1].Key);
            keys[1].Value.ShouldBeUriTemplateExpression("{GID}", keyTypes[1].Type);

            Assert.Equal("Name", keys[2].Key);
            keys[2].Value.ShouldBeUriTemplateExpression("{NAME}", keyTypes[2].Type);

            Assert.Equal("Upgraded", keys[3].Key);
            keys[3].Value.ShouldBeUriTemplateExpression("{UPGRADE}", keyTypes[3].Type);
        }

        [Fact]
        public void ParseEnumKeyTemplateWithTemplateParser()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("Shapes({enumKey})", UriKind.Relative))
            {
                EnableUriTemplateParsing = true
            };

            var path = uriParser.ParsePath();

            var keySegment = Assert.IsType<KeySegment>(path.LastSegment);
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            Assert.Equal("Color", keypair.Key);

            var edmTypeReference = ((IEdmEntityType)keySegment.EdmType).DeclaredKey.Single().Type;
            keypair.Value.ShouldBeUriTemplateExpression("{enumKey}", edmTypeReference);
        }

        [Fact]
        public void ParseEnumKeyTemplateAsSegmentWithTemplateParser()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/Shapes/{enumKey}"))
            {
                EnableUriTemplateParsing = true,
                UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash
            };

            var path = uriParser.ParsePath();
            var keySegment = Assert.IsType<KeySegment>(path.LastSegment);
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            Assert.Equal("Color", keypair.Key);
            var edmTypeReference = ((IEdmEntityType)keySegment.EdmType).DeclaredKey.Single().Type;
            keypair.Value.ShouldBeUriTemplateExpression("{enumKey}", edmTypeReference);
        }
        #endregion

        #region Parameter template tests
        [Fact]
        public void ParseParameterTemplateWithTemplateParser()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People(1)/Fully.Qualified.Namespace.HasHat(onCat={why555})"))
            {
                EnableUriTemplateParsing = true
            };

            IEdmFunction function = HardCodedTestModel.TestModel.FindOperations("Fully.Qualified.Namespace.HasHat").Single(f => f.Parameters.Count() == 2)as IEdmFunction;
            Assert.NotNull(function);
            var path = uriParser.ParsePath();
            OperationSegmentParameter parameter = path.LastSegment.ShouldBeOperationSegment(function).Parameters.Single();
            parameter.ShouldBeConstantParameterWithValueType("onCat", new UriTemplateExpression { LiteralText = "{why555}", ExpectedType = function.Parameters.Last().Type });
        }

        [Fact]
        public void ParseParameterTemplateOfEnumTypeWithTemplateParser()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("GetPetCount(colorPattern={COLOR})", UriKind.Relative))
            {
                EnableUriTemplateParsing = true
            };

            IEdmOperationImport operationImport = HardCodedTestModel.TestModel.EntityContainer.FindOperationImports("GetPetCount").Single();
            var path = uriParser.ParsePath();
            OperationSegmentParameter parameter = path.LastSegment.ShouldBeOperationImportSegment(operationImport).Parameters.Single();
            parameter.ShouldBeConstantParameterWithValueType("colorPattern", new UriTemplateExpression { LiteralText = "{COLOR}", ExpectedType = operationImport.Operation.Parameters.Single().Type });
        }

        [Fact]
        public void ParseParameterTemplateWithNonTemplateParserShouldThrow()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People(1)/Fully.Qualified.Namespace.HasHat(onCat={why555})"));
            Action action = () => uriParser.ParsePath();
            action.Throws<ODataException>(Error.Format(SRResources.ExpressionLexer_ExpectedLiteralToken, "{why555}"));
        }

        [Fact]
        public void ParseMultiParameterTemplateWithTemplateParser()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People(1)/Fully.Qualified.Namespace.HasDog(name={NAME} ,inOffice={INOFFICE})"))
            {
                EnableUriTemplateParsing = true
            };

            var operation = HardCodedTestModel.GetHasDogOverloadForPeopleWithThreeParameters();
            var typesDic = operation.Parameters.ToDictionary(_ => _.Name, _ => _.Type);

            var path = uriParser.ParsePath();
            IList<OperationSegmentParameter> parameters = path.LastSegment.ShouldBeOperationSegment(operation).Parameters.ToList();
            parameters[0].ShouldBeConstantParameterWithValueType("name", new UriTemplateExpression { LiteralText = "{NAME}", ExpectedType = typesDic["name"] });
            parameters[1].ShouldBeConstantParameterWithValueType("inOffice", new UriTemplateExpression { LiteralText = "{INOFFICE}", ExpectedType = typesDic["inOffice"] });
        }

        [Fact]
        public void ParseParameterTemplateForFunctionImportWithTemplateParser()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("IsAddressGood(address={ADDR})", UriKind.Relative))
            {
                EnableUriTemplateParsing = true
            };

            IEdmFunctionImport functionImport = HardCodedTestModel.TestModel.EntityContainer.FindOperationImports("IsAddressGood").Single() as IEdmFunctionImport;
            Assert.NotNull(functionImport);
            var path = uriParser.ParsePath();
            OperationSegmentParameter parameter = path.LastSegment.ShouldBeOperationImportSegment(functionImport).Parameters.Single();
            parameter.ShouldBeConstantParameterWithValueType("address", new UriTemplateExpression { LiteralText = "{ADDR}", ExpectedType = functionImport.Function.Parameters.Single().Type });
        }
        #endregion

        #region Path segment template tests
        [Fact]
        public void ParsePathTemplateSegmentWithTemplateParser()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People({1})/{some}", UriKind.Relative))
            {
                EnableUriTemplateParsing = true
            };

            var paths = uriParser.ParsePath().ToList();

            var keySegment = Assert.IsType<KeySegment>(paths[1]);
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            Assert.Equal("ID", keypair.Key);
            var edmTypeReference = ((IEdmEntityType)keySegment.EdmType).DeclaredKey.Single().Type;
            keypair.Value.ShouldBeUriTemplateExpression("{1}", edmTypeReference);

            var templateSegment = Assert.IsType<PathTemplateSegment>(paths[2]);
            Assert.Equal("{some}", templateSegment.LiteralText);
        }

        [Fact]
        public void EntityReferenceCannotAppearAfterAPathTemplateSegment()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People({1})/{some}/$ref", UriKind.Relative))
            {
                EnableUriTemplateParsing = true
            };

            Action action = () => uriParser.ParsePath();
            action.Throws<ODataUnrecognizedPathException>(Error.Format(SRResources.RequestUriProcessor_MustBeLeafSegment, "{some}"));
        }

        [Fact]
        public void PathTemplateSegmentShouldBeLastSegment()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People({1})/{some}/what", UriKind.Relative))
            {
                EnableUriTemplateParsing = true
            };

            Action action = () => uriParser.ParsePath();
            action.Throws<ODataUnrecognizedPathException>(Error.Format(SRResources.RequestUriProcessor_MustBeLeafSegment, "{some}"));
        }

        [Fact]
        public void PathTemplateSegmentShouldNotBeFirstSegment()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("{some}", UriKind.Relative))
            {
                EnableUriTemplateParsing = true
            };

            Action action = () => uriParser.ParsePath();
            action.Throws<ODataUnrecognizedPathException>(Error.Format(SRResources.RequestUriProcessor_ResourceNotFound, "{some}"));
        }
        #endregion

        #region Mixed template tests
        [Fact]
        public void ParseMixedKeyAndParameterTemplateWithTemplateParser()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People({KEY})/Fully.Qualified.Namespace.HasDog(name={NAME} ,inOffice={INOFFICE})"))
            {
                EnableUriTemplateParsing = true
            };

            var operation = HardCodedTestModel.GetHasDogOverloadForPeopleWithThreeParameters();
            var typesDic = operation.Parameters.ToDictionary(_ => _.Name, _ => _.Type);

            var path = uriParser.ParsePath();
            var keySegment = Assert.IsType<KeySegment>(path.ElementAt(1));
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            Assert.Equal("ID", keypair.Key);
            var edmTypeReference = ((IEdmEntityType)keySegment.EdmType).DeclaredKey.Single().Type;
            keypair.Value.ShouldBeUriTemplateExpression("{KEY}", edmTypeReference);
            IList<OperationSegmentParameter> parameters = path.LastSegment.ShouldBeOperationSegment(operation).Parameters.ToList();
            parameters[0].ShouldBeConstantParameterWithValueType("name", new UriTemplateExpression { LiteralText = "{NAME}", ExpectedType = typesDic["name"] });
            parameters[1].ShouldBeConstantParameterWithValueType("inOffice", new UriTemplateExpression { LiteralText = "{INOFFICE}", ExpectedType = typesDic["inOffice"] });
        }
        #endregion

        #region Validation tests
        [Fact]
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
            IEdmFunction function = HardCodedTestModel.TestModel.FindOperations("Fully.Qualified.Namespace.HasHat").Single(f => f.Parameters.Count() == 2)as IEdmFunction;
            foreach (var input in inputs)
            {
                var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People(1)/Fully.Qualified.Namespace.HasHat(onCat=" + input + ")", UriKind.Relative))
                {
                    EnableUriTemplateParsing = true
                };

                var path = uriParser.ParsePath();
                OperationSegmentParameter parameter = path.LastSegment.ShouldBeOperationSegment(function).Parameters.Single();
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
                var keySegment = Assert.IsType<KeySegment>(path.LastSegment);
                KeyValuePair<string, object> keypair = keySegment.Keys.Single();
                Assert.Equal("ID", keypair.Key);
                var edmTypeReference = ((IEdmEntityType)keySegment.EdmType).DeclaredKey.Single().Type;
                keypair.Value.ShouldBeUriTemplateExpression(input, edmTypeReference);
            }
        }

        [Fact]
        public void ValidateUntypedTemplateSegment()
        {
            // The following URL results in an untyped URI template as the last segment.
            // Validation needs to handle the untyped segment when validating the URL.
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People/{myId}/MyDog/{myDogId}"))
            {
                EnableUriTemplateParsing = true
            };

            IEnumerable<ODataUrlValidationMessage> validationMessages;
            Assert.True(parser.Validate(ODataUrlValidationRuleSet.AllRules, out validationMessages));
        }

        [Fact]
        public void ErrorParameterTemplateInputShouldThrow()
        {
            var errorCases = new[]
            {
                new {Input = "{"    , Error = SRResources.ExpressionLexer_UnbalancedBracketExpression},
                new {Input = "}"    , Error = Error.Format(SRResources.ExpressionLexer_InvalidCharacter, "}", 6, "onCat=}")},
                new {Input = "{}1"  , Error = Error.Format(SRResources.ExpressionLexer_SyntaxError, 9, "onCat={}1")},
                new {Input = "}{"   , Error = Error.Format(SRResources.ExpressionLexer_InvalidCharacter, "}", 6, "onCat=}{")},
                new {Input = "{{}"  , Error = SRResources.ExpressionLexer_UnbalancedBracketExpression}, // Thrown by ODataPathParser::TryBindingParametersAndMatchingOperation
                new {Input = "{}}"  , Error = Error.Format(SRResources.ExpressionLexer_InvalidCharacter, "}", 8, "onCat={}}")},
                new {Input = "{#}"  , Error = SRResources.RequestUriProcessor_SyntaxError},
            };

            foreach (var errorCase in errorCases)
            {
                var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People(1)/Fully.Qualified.Namespace.HasHat(onCat=" + errorCase.Input + ")", UriKind.Relative))
                {
                    EnableUriTemplateParsing = true
                };

                Action action = () => uriParser.ParsePath();
                action.Throws<ODataException>(errorCase.Error);
            }
        }

        [Fact]
        public void ErrorKeyTemplateInputShouldThrow()
        {
            var errorCases = new[]
            {
                new {Input = "{"    , Error = SRResources.ExpressionLexer_UnbalancedBracketExpression},
                new {Input = "}"    , Error = Error.Format(SRResources.ExpressionLexer_InvalidCharacter, "}", 1, "(})")},
                new {Input = "{}1"  , Error = Error.Format(SRResources.UriQueryExpressionParser_CloseParenOrCommaExpected, 3, "({}1)")}, //Strings.BadRequest_KeyCountMismatch("Fully.Qualified.Namespace.Person")},
                new {Input = "}{"   , Error = Error.Format(SRResources.ExpressionLexer_InvalidCharacter, "}", 1, "(}{)")},
                new {Input = "{{}"  , Error = SRResources.ExpressionLexer_UnbalancedBracketExpression}, // Thrown by ODataPathParser::TryBindKeyFromParentheses
                new {Input = "{}}"  , Error = Error.Format(SRResources.ExpressionLexer_InvalidCharacter, "}", 3, "({}})")},
                new {Input = "{#}"  , Error = SRResources.RequestUriProcessor_SyntaxError},
            };

            foreach (var errorCase in errorCases)
            {
                var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People(" + errorCase.Input + ")"))
                {
                    EnableUriTemplateParsing = true
                };

                Action action = () => uriParser.ParsePath();
                action.Throws<ODataException>(errorCase.Error);
            }
        }
        #endregion
    }
}
