//---------------------------------------------------------------------
// <copyright file="ODataUriParserUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Unit tests for ODataUriParser.
    /// </summary>
    [TestClass]
    public class ODataUriParserUnitTests
    {
        private readonly Uri ServiceRoot = new Uri("http://host");
        private readonly Uri FullUri = new Uri("http://host/People");

        [TestMethod]
        public void NoneQueryOptionShouldWork()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri);
            var path = uriParser.ParsePath();
            path.Should().HaveCount(1);
            path.LastSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            uriParser.ParseFilter().Should().BeNull();
            uriParser.ParseSelectAndExpand().Should().BeNull();
            uriParser.ParseOrderBy().Should().BeNull();
            uriParser.ParseTop().Should().Be(null);
            uriParser.ParseSkip().Should().Be(null);
            uriParser.ParseCount().Should().Be(null);
            uriParser.ParseSearch().Should().BeNull();
            uriParser.ParseDeltaToken().Should().BeNull();
        }

        [TestMethod]
        public void EmptyValueQueryOptionShouldWork()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri(FullUri, "?$filter=&$select=&$expand=&$orderby=&$top=&$skip=&$count=&$search=&$unknow=&$unknowvalue&$deltaToken="));
            var path = uriParser.ParsePath();
            path.Should().HaveCount(1);
            path.LastSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
            uriParser.ParseFilter().Should().BeNull();
            var results = uriParser.ParseSelectAndExpand();
            results.AllSelected.Should().BeTrue();
            results.SelectedItems.Should().HaveCount(0);
            uriParser.ParseOrderBy().Should().BeNull();
            Action action = () => uriParser.ParseTop();
            action.ShouldThrow<ODataException>().WithMessage(Strings.SyntacticTree_InvalidTopQueryOptionValue(""));
            action = () => uriParser.ParseSkip();
            action.ShouldThrow<ODataException>().WithMessage(Strings.SyntacticTree_InvalidSkipQueryOptionValue(""));
            action = () => uriParser.ParseCount();
            action.ShouldThrow<ODataException>().WithMessage(Strings.ODataUriParser_InvalidCount(""));
            action = () => uriParser.ParseSearch();
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_ExpressionExpected(0, ""));
            uriParser.ParseDeltaToken().Should().BeEmpty();
        }

        [TestMethod]
        public void ModelCannotBeNull()
        {
            Action createWithNullModel = () => new ODataUriParser(null, ServiceRoot, FullUri);
            createWithNullModel.ShouldThrow<Exception>(Error.ArgumentNull("model").ToString());
        }

        [TestMethod]
        public void ModelIsSetCorrectly()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri);
            parser.Model.Should().Be(HardCodedTestModel.TestModel);
        }

        [TestMethod]
        public void FilterLimitIsSettable()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { FilterLimit = 3 } };
            parser.Settings.FilterLimit.Should().Be(3);
        }

        [TestMethod]
        public void FilterLimitIsRespectedForFilter()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri("http://host/People?$filter=1 eq 1")) { Settings = { FilterLimit = 0 } };
            Action parseWithLimit = () => parser.ParseFilter();
            parseWithLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
        }

        [TestMethod]
        public void FilterLimitWithInterestingTreeStructures()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri("http://host/People?$filter=MyDog/Color eq 'Brown' or MyDog/Color eq 'White'")) { Settings = { FilterLimit = 5 } };
            Action parseWithLimit = () => parser.ParseFilter();
            parseWithLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
        }

        [TestMethod]
        public void NegativeFilterLimitThrows()
        {
            Action negativeLimit = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { FilterLimit = -98798 } };
            negativeLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriParser_NegativeLimit);
        }

        [TestMethod]
        public void OrderbyLimitIsSettable()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { OrderByLimit = 3 } };
            parser.Settings.OrderByLimit.Should().Be(3);
        }

        [TestMethod]
        public void OrderByLimitIsRespectedForOrderby()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri("http://host/People?$orderby= 1 eq 1")) { Settings = { OrderByLimit = 0 } };
            Action parseWithLimit = () => parser.ParseOrderBy();
            parseWithLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
        }

        [TestMethod]
        public void OrderByLimitWithInterestingTreeStructures()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri("http://host/People?$orderby=MyDog/MyPeople/MyDog/MyPeople/MyPaintings asc")) { Settings = { OrderByLimit = 5 } };
            Action parseWithLimit = () => parser.ParseOrderBy();
            parseWithLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
        }

        [TestMethod]
        public void OrderByLimitCannotBeNegative()
        {
            Action parseWithNegativeLimit = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { OrderByLimit = -9879 } };
            parseWithNegativeLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriParser_NegativeLimit);
        }

        [TestMethod]
        public void PathLimitIsSettable()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { PathLimit = 3 } };
            parser.Settings.PathLimit.Should().Be(3);
        }

        [TestMethod]
        public void PathLimitIsRespectedForPath()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/", UriKind.Absolute), new Uri("http://gobbldygook/path/to/something", UriKind.Absolute)) { Settings = { PathLimit = 0 } };
            Action parseWithLimit = () => parser.ParsePath();
            parseWithLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryPathParser_TooManySegments);
        }

        [TestMethod]
        public void PathLimitCannotBeNegative()
        {
            Action parseWithNegativeLimit = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { PathLimit = -8768 } };
            parseWithNegativeLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriParser_NegativeLimit);
        }

        [TestMethod]
        public void SelectExpandLimitIsSettable()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { SelectExpandLimit = 3 } };
            parser.Settings.SelectExpandLimit.Should().Be(3);
        }

        [TestMethod]
        public void SelectExpandLimitIsRespectedForSelectExpand()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri("http://host/People?$select=MyDog&$expand=MyDog($select=color)")) { Settings = { SelectExpandLimit = 0 } };
            Action parseWithLimit = () => parser.ParseSelectAndExpand();
            parseWithLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
        }

        [TestMethod]
        public void NegativeSelectExpandLimitIsRespected()
        {
            Action parseWithNegativeLimit = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri) { Settings = { SelectExpandLimit = -87657 } };
            parseWithNegativeLimit.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriParser_NegativeLimit);
        }

        [TestMethod]
        public void ServiceRootUriIsSet()
        {
            var serviceRoot = new Uri("http://example.com/Foo/");
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, FullUri);
            parser.ServiceRoot.Should().BeSameAs(serviceRoot);
        }

        [TestMethod]
        public void ServiceRootMustBeAbsoluteUri()
        {
            var serviceRoot = new Uri("one/two/three", UriKind.Relative);
            Action create = () => new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, new Uri("test", UriKind.Relative));
            create.ShouldThrow<ODataException>();

            serviceRoot = new Uri("one/two/three", UriKind.RelativeOrAbsolute);
            create = () => new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, new Uri("test", UriKind.Relative));
            create.ShouldThrow<ODataException>();
        }

        [TestMethod]
        public void DefaultUrlConventionsShouldBeDefault()
        {
            new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri).UrlConventions.Should().BeSameAs(ODataUrlConventions.Default);
        }

        [TestMethod]
        public void UrlConventionsCannotBeSetToNull()
        {
            Action setToNull = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri).UrlConventions = null;
            setToNull.ShouldThrow<ArgumentNullException>().WithMessage("UrlConventions", ComparisonMode.EquivalentSubstring);
        }

        [TestMethod]
        public void DefaultEnableTemplateParsingShouldBeFalse()
        {
            new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri).EnableUriTemplateParsing.Should().BeFalse();
        }

        [TestMethod]
        public void DefaultEnableCaseInsensitiveBuiltinIdentifierShouldBeFalse()
        {
            new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri).Resolver.EnableCaseInsensitive.Should().BeFalse();
        }

        [TestMethod]
        public void MaxExpandDepthCannotBeNegative()
        {
            Action setNegative = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri).Settings.MaximumExpansionDepth = -1;
            setNegative.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriParser_NegativeLimit);
        }

        [TestMethod]
        public void MaxExpandCountCannotBeNegative()
        {
            Action setNegative = () => new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, FullUri).Settings.MaximumExpansionCount = -1;
            setNegative.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriParser_NegativeLimit);
        }

        [TestMethod]
        public void ParseSelectExpandForContainment()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri("http://host/People?$select=MyContainedDog&$expand=MyContainedDog")) { Settings = { SelectExpandLimit = 5 } };
            SelectExpandClause containedSelectExpandClause = parser.ParseSelectAndExpand();
            IEnumerator<SelectItem> enumerator = containedSelectExpandClause.SelectedItems.GetEnumerator();
            enumerator.MoveNext();
            ExpandedNavigationSelectItem expandedNavigationSelectItem = enumerator.Current as ExpandedNavigationSelectItem;
            expandedNavigationSelectItem.Should().NotBeNull();
            (expandedNavigationSelectItem.NavigationSource is IEdmContainedEntitySet).Should().BeTrue();
        }

        [TestMethod]
        public void DefaultParameterAliasNodesShouldBeEmtpy()
        {
            var uriParser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host"), new Uri("http://host/People"));
            uriParser.ParameterAliasNodes.Count.Should().Be(0);
        }

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

            keypair.Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression() { LiteralText = "{1}", ExpectedType = keySegment.EdmType.As<IEdmEntityType>().DeclaredKey.Single().Type });
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
            keypair.Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression() { LiteralText = "{ 1  }", ExpectedType = keySegment.EdmType.As<IEdmEntityType>().DeclaredKey.Single().Type });
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
            keypair.Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression() { LiteralText = "{1}", ExpectedType = keySegment.EdmType.As<IEdmEntityType>().DeclaredKey.Single().Type });
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
            keypair.Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression() { LiteralText = "{1}", ExpectedType = keySegment.EdmType.As<IEdmEntityType>().DeclaredKey.Single().Type });
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
            keys[0].Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression() { LiteralText = "{RID}", ExpectedType = keyTypes[0].Type });
            keys[1].Key.Should().Be("Gid");
            keys[1].Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression() { LiteralText = "{GID}", ExpectedType = keyTypes[1].Type });
            keys[2].Key.Should().Be("Name");
            keys[2].Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression() { LiteralText = "{NAME}", ExpectedType = keyTypes[2].Type });
            keys[3].Key.Should().Be("Upgraded");
            keys[3].Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression() { LiteralText = "{UPGRADE}", ExpectedType = keyTypes[3].Type });
        }

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
            keypair.Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression() { LiteralText = "{KEY}", ExpectedType = keySegment.EdmType.As<IEdmEntityType>().DeclaredKey.Single().Type });
            IList<OperationSegmentParameter> parameters = path.LastSegment.ShouldBeOperationSegment(operation).And.Parameters.ToList();
            parameters[0].ShouldBeConstantParameterWithValueType("name", new UriTemplateExpression { LiteralText = "{NAME}", ExpectedType = typesDic["name"] });
            parameters[1].ShouldBeConstantParameterWithValueType("inOffice", new UriTemplateExpression { LiteralText = "{INOFFICE}", ExpectedType = typesDic["inOffice"] });
        }

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
                keypair.Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression() { LiteralText = input, ExpectedType = keySegment.EdmType.As<IEdmEntityType>().DeclaredKey.Single().Type });
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
                new {Input = "{}1"  , Error = ODataErrorStrings.UriQueryExpressionParser_CloseParenOrCommaExpected(3, "({}1)")}, //Strings.BadRequest_KeyCountMismatch("Fully.Qualified.Namespace.Person")},
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

        #region Relative full path smoke test
        [TestMethod]
        public void AbsoluteUriInConstructorShouldThrow()
        {
            Action action = () => new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host/People(1)"));
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriParser_FullUriMustBeRelative);
        }

        [TestMethod]
        public void ParsePathShouldWork()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People", UriKind.Relative)).ParsePath();
            path.Should().HaveCount(1);
            path.LastSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
        }

        [TestMethod]
        public void ParseQueryOptionsShouldWork()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People?$filter=MyDog/Color eq 'Brown'&$select=ID&$expand=MyDog&$orderby=ID&$top=1&$skip=2&$count=true&$search=FA&$unknow=&$unknowvalue&$deltaToken=def", UriKind.Relative));
            parser.ParseSelectAndExpand().Should().NotBeNull();
            parser.ParseFilter().Should().NotBeNull();
            parser.ParseOrderBy().Should().NotBeNull();
            parser.ParseTop().Should().Be(1);
            parser.ParseSkip().Should().Be(2);
            parser.ParseCount().Should().Be(true);
            parser.ParseSearch().Should().NotBeNull();
            parser.ParseDeltaToken().Should().Be("def");
        }
        #endregion

        #region ODataUnrecognizedPathException tests
        [TestMethod]
        public void ParsePathExceptionDataTestWithInvalidEntitySet()
        {
            Action action = () => new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People1(1)/MyDog/Color", UriKind.Relative)).ParsePath();
            ODataUnrecognizedPathException ex = action.ShouldThrow<ODataUnrecognizedPathException>().And;
            ex.ParsedSegments.As<IEnumerable<ODataPathSegment>>().Count().Should().Be(0);
            ex.CurrentSegment.Should().Be("People1(1)");
            ex.UnparsedSegments.As<IEnumerable<string>>().Count().Should().Be(2);
        }

        [TestMethod]
        public void ParsePathExceptionDataTestWithInvalidContainedNavigationProperty()
        {
            Action action = () => new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People(1)/MyDog1/Color", UriKind.Relative)).ParsePath();
            ODataUnrecognizedPathException ex = action.ShouldThrow<ODataUnrecognizedPathException>().And;
            ex.ParsedSegments.As<IEnumerable<ODataPathSegment>>().Count().Should().Be(2);
            ex.CurrentSegment.Should().Be("MyDog1");
            ex.UnparsedSegments.As<IEnumerable<string>>().Count().Should().Be(1);
        }

        [TestMethod]
        public void ParsePathExceptionDataTestInvalidNavigationProperty()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("People(1)/MyDog/Color1", UriKind.Relative));
            Action action = () => parser.ParsePath();
            ODataUnrecognizedPathException ex = action.ShouldThrow<ODataUnrecognizedPathException>().And;
            ex.ParsedSegments.As<IEnumerable<ODataPathSegment>>().Count().Should().Be(3);
            ex.CurrentSegment.Should().Be("Color1");
            ex.UnparsedSegments.As<IEnumerable<string>>().Count().Should().Be(0);
            parser.ParameterAliasNodes.Count.Should().Be(0);
        }

        [TestMethod]
        public void ParsePathExceptionDataTestWithAlias()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("GetCoolestPersonWithStyle(styleID=@p1)/UndeclaredProperty?@p1=32", UriKind.Relative));
            Action action = () => parser.ParsePath();
            ODataUnrecognizedPathException ex = action.ShouldThrow<ODataUnrecognizedPathException>().And;
            ex.ParsedSegments.As<IEnumerable<ODataPathSegment>>().Count().Should().Be(1);
            ex.CurrentSegment.Should().Be("UndeclaredProperty");
            ex.UnparsedSegments.As<IEnumerable<string>>().Count().Should().Be(0);

            var aliasNode = parser.ParameterAliasNodes;
            aliasNode.Count.Should().Be(1);
            aliasNode["@p1"].ShouldBeConstantQueryNode(32);
        }
        #endregion
    }
}
