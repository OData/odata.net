using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Core.Aggregation;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xbehave;
//using Xunit;
using Xunit;
using Xunit.Extensions;
using System.Diagnostics.CodeAnalysis;
using AggregationTestProject.Common;
using FluentAssertions;

namespace AggregationTestProject
{
    public class ApplyParserTests
    {

        public static TheoryDataSet<string> GetStringToParseThatDoesNotMatchModel
        {
            get
            {
                return new TheoryDataSet<string>()
                {
                
                    "$apply=aggregate(xxx mul Product/TaxRate with sum as Tax)"
                };
            }

        }

        public static TheoryDataSet<string> GetMultipleTrasformationStringToParse
        {
            get
            {
                return new TheoryDataSet<string>()
                {
                    "$apply=aggregate(Amount mul Product/TaxRate with sum as Tax)/groupby(Amount)/aggregate(Amount with sum as total)/groupby((Product/TaxRate) aggregate(Amount with sum as Total))" 
                };
            }
            
        }

        //"$apply=groupby(round(Product/TaxRate))"
        public static TheoryDataSet<string> GetGroupByWithoutAggregateStringsToParse
        {
            get
            {
                return new TheoryDataSet<string>()
                {
                   "$apply=groupby(Amount,Id)",
                   "$apply=groupby(Time with DayOfWeek as day)",
                   "groupby(round(Product/TaxRate))"
                };
            }
        }


        public static TheoryDataSet<string> GetGroupByStringsWithAggregateToParse
        {
            get
            {
                return new TheoryDataSet<string>()
                {
                    "$apply=groupby((Product/TaxRate,Product/Category/Name,Id) aggregate(Amount with sum as Total))" ,
                    "$apply=groupby((Product/TaxRate mul 2 with round as RoundTax) aggregate(Amount with sum as Total))",
                    "$apply=groupby((round(Product/TaxRate)) aggregate(Amount with sum as Total))" 
                };
            }
        }



        public static TheoryDataSet<string> GetAggregateStringsToParse
        {
            get
            {
                return new TheoryDataSet<string>()
                {
                    "aggregate(Amount with sum as Total)",
                    "aggregate(Product/TaxRate with sum as Total)"
                };
            }
        }


        public static TheoryDataSet<string> GetAggregateNoWithOrAggregationProperty
        {
            get
            {
                return new TheoryDataSet<string>()
                {
                    "aggregate(Amount wit sum as Total)",
                    "aggregate(Amount withh sum as Total)",
                    "aggregate(with sum as Total)",
                };
            }
        }

        public static TheoryDataSet<string> GetGroupByStringsWithWrongNumberOfParenthesis
        {
            get
            {
                return new TheoryDataSet<string>()
                {
                   "groupby(((Product/TaxRate)",
                   "groupby(Product/TaxRate)))",
                };
            }
        }


        public static TheoryDataSet<string> GetGroupByStringsWithWrongNumberOfParenthesis2
        {
            get
            {
                return new TheoryDataSet<string>()
                {
                    "groupby((Product/TaxRate) aggregate(Amount with sum as Total)))",
                    "groupby(((Product/TaxRate) aggregate(Amount with sum as Total))",
                    "groupby((Product/TaxRate))"
                };
            }
        }


        public static TheoryDataSet<string> GetUnsupportedTrasformations
        {
            get
            {
                return new TheoryDataSet<string>()
                {
                   "groupby1((Product/TaxRate)",
                   "aggregate1(Product/TaxRate))",
                };
            }
        }


        public static TheoryDataSet<string> GetNoTrasformations
        {
            get
            {
                return new TheoryDataSet<string>()
                {
                   "Product/TaxRate"
                };
            }
        }

        

        public static TheoryDataSet<string> GetFilterString
        {
            get
            {
                return new TheoryDataSet<string>()
                {
                    "filter(Amount gt 50)",
                    "filter(Amount gt 50)/aggregate(Amount with sum as Total)",
                    "filter(round(Product/TaxRate) gt 50)"
                };
            }
        }

        [Scenario]
        [PropertyData("GetNoTrasformations")]
        public void NoTrasformationsThrowsODataException(string query)
        {
            var model =
                Common.TestModelBuilder.CreateModel(new Type[] { typeof(Category), typeof(Product), typeof(Sales) });
            IEdmType edmType = model.FindDeclaredType(typeof(Sales).FullName);
            var config = new ODataUriParserConfiguration(model);
            var exception = default(Exception);

            "Given a message to parse {0}".Given(() => { });
            "When I try to parse".When(
                () =>
                {
                    exception = Record.Exception(() => ApplyParser.ParseApplyImplementation(query, config, edmType, null));
                });
            "Then an OData exception is thrown".Then(() => exception.Should().BeOfType<Microsoft.OData.Core.ODataException>());
        }


        [Scenario]
        [PropertyData("GetUnsupportedTrasformations")]
        public void UnsupportedTrasformationsThrowsODataException(string query)
        {
            var model =
                Common.TestModelBuilder.CreateModel(new Type[] { typeof(Category), typeof(Product), typeof(Sales) });
            IEdmType edmType = model.FindDeclaredType(typeof(Sales).FullName);
            var config = new ODataUriParserConfiguration(model);
            var exception = default(Exception);

            "Given a message to parse {0}".Given(() => { });
            "When I try to parse".When(
                () =>
                {
                    exception = Record.Exception(() => ApplyParser.ParseApplyImplementation(query, config, edmType, null));
                });
            "Then an OData exception is thrown".Then(() => exception.Should().BeOfType<Microsoft.OData.Core.ODataException>());
        }




        [Scenario]
        [PropertyData("GetFilterString")]
        public void ParseFilterCaluse(string query)
        {
            var model =
                Common.TestModelBuilder.CreateModel(new Type[] { typeof(Category), typeof(Product), typeof(Sales) });
            IEdmType edmType = model.FindDeclaredType(typeof(Sales).FullName);
            var config = new ODataUriParserConfiguration(model);
            var result = default(ApplyClause);

            "Given a message to parse {0}".Given(() => { });
            "When I try to parse".When(
                () =>
                {
                    result = ApplyParser.ParseApplyImplementation(query, config, edmType, null);
                });
            "the transformation is filter".Then(
                () => result.Transformations.First().Item2.Should().BeOfType<ApplyFilterClause>());
        }


        [Scenario]
        [PropertyData("GetGroupByStringsWithWrongNumberOfParenthesis2")]
        public void GroupByStringsWithWrongNumberOfParenthesisPass(string query)
        {
            var model =
                Common.TestModelBuilder.CreateModel(new Type[] {typeof (Category), typeof (Product), typeof (Sales)});
            IEdmType edmType = model.FindDeclaredType(typeof (Sales).FullName);
            var config = new ODataUriParserConfiguration(model);
            var result = default(ApplyClause);

            "Given a message to parse {0}".Given(() => { });
            "When I try to parse".When(
                () =>
                {
                    result = ApplyParser.ParseApplyImplementation(query, config, edmType, null);
                });
            "the transformation is groupby".Then(
                () => result.Transformations.First().Item2.Should().BeOfType<ApplyGroupbyClause>());
        }

        [Scenario]
        [PropertyData("GetGroupByStringsWithWrongNumberOfParenthesis")]
        public void GroupByStringsWithWrongNumberOfParenthesisPass2(string query)
        {
            var model =
                Common.TestModelBuilder.CreateModel(new Type[] { typeof(Category), typeof(Product), typeof(Sales) });
            IEdmType edmType = model.FindDeclaredType(typeof(Sales).FullName);
            var config = new ODataUriParserConfiguration(model);
            var result = default(ApplyClause);

            "Given a message to parse {0}".Given(() => { });
            "When I try to parse".When(
                () =>
                {
                    result = ApplyParser.ParseApplyImplementation(query, config, edmType, null);
                });
            "the transformation is groupby".Then(
                () => result.Transformations.First().Item2.Should().BeOfType<ApplyGroupbyClause>());
        }
        
        [Scenario]
        [PropertyData("GetAggregateNoWithOrAggregationProperty")]
        public void AggregateNoWithOrAggregationPropertyThrowsODataException(string query)
        {
            var model =
                Common.TestModelBuilder.CreateModel(new Type[] {typeof (Category), typeof (Product), typeof (Sales)});
            IEdmType edmType = model.FindDeclaredType(typeof (Sales).FullName);
            var config = new ODataUriParserConfiguration(model);
            var exception = default(Exception);

            "Given a message to parse {0}".Given(() => { });
            "When I try to parse".When(
                () =>
                {
                    exception = Record.Exception(() => ApplyParser.ParseApplyImplementation(query, config, edmType, null));
                });
            "Then an OData exception is thrown".Then(() => exception.Should().BeOfType<Microsoft.OData.Core.ODataException>());
        }
        
        [Scenario]
        [PropertyData("GetAggregateStringsToParse")]
        public void ParseAggregate(string query)
        {
            var model =
                Common.TestModelBuilder.CreateModel(new Type[] { typeof(Category), typeof(Product), typeof(Sales) });
            IEdmType edmType = model.FindDeclaredType(typeof(Sales).FullName);
            var config = new ODataUriParserConfiguration(model);
            var result = default(ApplyClause);

            "Given a message to parse {0}".Given(() => { });
            "When I try to parse".When(
                () =>
                {
                    result = ApplyParser.ParseApplyImplementation(query, config, edmType, null);
                });
            "The transformation is aggregate".Then(() => result.Transformations.First().Item2.Should().BeOfType<ApplyAggregateClause>());
            "The Alias is Total".Then(() => ((ApplyAggregateClause) result.Transformations.First().Item2).Alias.Should().Be("Total"));
            "The aggregation method is Sum".Then(() => ((ApplyAggregateClause)result.Transformations.First().Item2).AggregationMethod.Should().Be("sum"));
            "The aggregation expression exist".Then(() => ((ApplyAggregateClause)result.Transformations.First().Item2).AggregatablePropertyExpression.Expression.Should().NotBeNull());
            "The aggregation expression type exist".Then(() => ((ApplyAggregateClause)result.Transformations.First().Item2).AggregatablePropertyExpression.ItemType.Should().NotBeNull());
        }
        
        [Scenario]
        [PropertyData("GetGroupByWithoutAggregateStringsToParse")]
        public void ParseGroupByStringsWithoutAggregate(string query)
        {
            var model =
                Common.TestModelBuilder.CreateModel(new Type[] { typeof(Category), typeof(Product), typeof(Sales) });
            IEdmType edmType = model.FindDeclaredType(typeof(Sales).FullName);
            var config = new ODataUriParserConfiguration(model);
            var result = default(ApplyClause);

            "Given a message to parse {0}".Given(() => { });
            "When I try to parse".When(
                () =>
                {
                    result = ApplyParser.ParseApplyImplementation(query, config, edmType, null);
                });
            "The transformation is groupby".Then(() => result.Transformations.First().Item2.Should().BeOfType<ApplyGroupbyClause>());
            "The transformation contains aggregate".Then(() => ((ApplyGroupbyClause)result.Transformations.First().Item2).Aggregate.Should().BeNull());
           
        }
        
        [Scenario]
        [PropertyData("GetGroupByStringsWithAggregateToParse")]
        public void ParseGroupByStringsWithAggregate(string query)
        {
            var model =
                Common.TestModelBuilder.CreateModel(new Type[] { typeof(Category), typeof(Product), typeof(Sales) });
            IEdmType edmType = model.FindDeclaredType(typeof(Sales).FullName);
            var config = new ODataUriParserConfiguration(model);
            var result = default(ApplyClause);

            "Given a message to parse {0}".Given(() => { });
            "When I try to parse".When(
                () =>
                {
                    result = ApplyParser.ParseApplyImplementation(query, config, edmType, null);
                });
            "the transformation is groupby".Then(() => result.Transformations.First().Item2.Should().BeOfType<ApplyGroupbyClause>());
            "the transformation contains aggregate".Then(() => ((ApplyGroupbyClause)result.Transformations.First().Item2).Aggregate.Should().NotBeNull());
            "the transformation contains aggregate".Then(() => ((ApplyGroupbyClause)result.Transformations.First().Item2).Aggregate.Should().BeOfType<ApplyAggregateClause>());
            "The aggregation method is Sum".Then(
               () =>
                   ((ApplyGroupbyClause)result.Transformations.First().Item2).Aggregate.AggregationMethod.Should()
                       .Be("sum"));
            "The aggregation property is Amount".Then(
               () =>
                   ((ApplyGroupbyClause)result.Transformations.First().Item2).Aggregate.AggregatableProperty.Should()
                       .Be("Amount"));
            "The Alias is Total".Then(
              () =>
                  ((ApplyGroupbyClause)result.Transformations.First().Item2).Aggregate.Alias.Should()
                      .Be("Total"));
        }
        
        [Scenario]
        [PropertyData("GetMultipleTrasformationStringToParse")]
        public void ParseMultipleTransformations(string query)
        {
            var model =
                Common.TestModelBuilder.CreateModel(new Type[] { typeof(Category), typeof(Product), typeof(Sales) });
            IEdmType edmType = model.FindDeclaredType(typeof(Sales).FullName);
            var config = new ODataUriParserConfiguration(model);
            var result = default(ApplyClause);

            "Given a message to parse {0}".Given(() => { });
            "When I try to parse".When(
                () =>
                {
                    result = ApplyParser.ParseApplyImplementation(query, config, edmType, null);
                });
            "we should find 4 transformations".Then(() => result.Transformations.Count.Should().Be(4));
            "the first transformation is aggregate".Then(() => result.Transformations.First().Item2.Should().BeOfType<ApplyAggregateClause>());
            "the last transformation is groupby".Then(() => result.Transformations.Last().Item2.Should().BeOfType<ApplyGroupbyClause>());

        }
        
        [Scenario]
        [PropertyData("GetStringToParseThatDoesNotMatchModel")]
        public void ParsingStringThatDoesNotMatchModelThrowsODataException(string query)
        {
            
            var model = Common.TestModelBuilder.CreateModel(new Type[] { typeof(Category), typeof(Product), typeof(Sales) });
            IEdmType edmType = model.FindDeclaredType(typeof(Sales).FullName);
            var config = new ODataUriParserConfiguration(model);
            var exception = default(Exception);

            "Given a message to parse {0}".Given(() => { });
            "When I try to parse".When(
                 () =>
                 {
                      exception = Record.Exception(() => ApplyParser.ParseApplyImplementation(query, config, edmType, null));
                 });
            "Then an OData exception is thrown".Then(() => exception.Should().BeOfType<Microsoft.OData.Core.ODataException>());
        }
    }
}
