//---------------------------------------------------------------------
// <copyright file="FilterSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class FilterSegmentTests
    {
        #region Test Cases
        [Fact]
        public void FilterSegmentConstructWithNullParameterAliasShouldThrowException()
        {
            const string filterExpression = "@a1";

            FilterClause filterClause = CreateFilterClause(filterExpression);

            Action create = () => new FilterSegment(null, filterClause.RangeVariable, HardCodedTestModel.GetPet1Set());
            create.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: expression");
        }

        [Fact]
        public void FilterSegmentConstructWithNullRangeVariableShouldThrowException()
        {
            const string filterExpression = "@a1";

            FilterClause filterClause = CreateFilterClause(filterExpression);
            ParameterAliasNode expression = filterClause.Expression as ParameterAliasNode;

            Action create = () => new FilterSegment(expression, null, HardCodedTestModel.GetPet1Set());
            create.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: rangeVariable");
        }

        [Fact]
        public void FilterSegmentConstructWithNullNavigationSourceShouldThrowException()
        {
            const string filterExpression = "@a1";

            FilterClause filterClause = CreateFilterClause(filterExpression);
            ParameterAliasNode expression = filterClause.Expression as ParameterAliasNode;

            Action create = () => new FilterSegment(expression, filterClause.RangeVariable, null);
            create.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: navigationSource");
        }

        [Fact]
        public void FilterSegmentWithAllParametersShouldConstructSuccessfully()
        {
            const string filterExpression = "@a1";

            FilterClause filterClause = CreateFilterClause(filterExpression);
            ParameterAliasNode expression = filterClause.Expression as ParameterAliasNode;
            FilterSegment filterSegment = new FilterSegment(expression, filterClause.RangeVariable, HardCodedTestModel.GetPet1Set());

            filterSegment.Expression.Equals(expression).Should().BeTrue();
            filterSegment.RangeVariable.Equals(filterClause.RangeVariable).Should().BeTrue();
        }

        [Fact]
        public void FilterSegmentWithExpressionShouldConstructSuccessfully()
        {
            const string filterExpression = "ID eq 1";

            FilterClause filterClause = CreateFilterClause(filterExpression);
            BinaryOperatorNode expression = filterClause.Expression as BinaryOperatorNode;
            FilterSegment filterSegment = new FilterSegment(expression, filterClause.RangeVariable, HardCodedTestModel.GetPet1Set());

            filterSegment.Expression.Equals(expression).Should().BeTrue();
            filterSegment.RangeVariable.Equals(filterClause.RangeVariable).Should().BeTrue();
        }

        [Fact]
        public void FilterSegmentShouldNotBeSingleResult()
        {
            const string filterExpression = "@a1";

            FilterClause filterClause = CreateFilterClause(filterExpression);
            ParameterAliasNode expression = filterClause.Expression as ParameterAliasNode;
            FilterSegment filterSegment = new FilterSegment(expression, filterClause.RangeVariable, HardCodedTestModel.GetPet1Set());

            filterSegment.SingleResult.Should().BeFalse();
        }

        [Fact]
        public void FilterSegmentShouldHaveSameTypeAsRangeVariableTypeDefinition()
        {
            const string filterExpression = "@a1";

            FilterClause filterClause = CreateFilterClause(filterExpression);
            ParameterAliasNode expression = filterClause.Expression as ParameterAliasNode;
            FilterSegment filterSegment = new FilterSegment(expression, filterClause.RangeVariable, HardCodedTestModel.GetPet1Set());

            filterSegment.EdmType.Equals(filterClause.RangeVariable.TypeReference.Definition).Should().BeFalse();
        }

        [Fact]
        public void FilterSegmentsWithSameExpressionShouldBeEqual()
        {
            const string filterExpression = "@a1";

            FilterClause filterClause = CreateFilterClause(filterExpression);
            ParameterAliasNode expression = filterClause.Expression as ParameterAliasNode;
            FilterSegment filterSegment1 = new FilterSegment(expression, filterClause.RangeVariable, HardCodedTestModel.GetPet1Set());
            FilterSegment filterSegment2 = new FilterSegment(expression, filterClause.RangeVariable, HardCodedTestModel.GetPet1Set());

            filterSegment1.Equals(filterSegment2).Should().BeTrue();
            filterSegment2.Equals(filterSegment1).Should().BeTrue();
        }

        [Fact]
        public void FilterSegmentsWithDifferentExpressionsShouldNotBeEqual()
        {
            const string filterExpression1 = "@a1";
            const string filterExpression2 = "@a2";

            FilterClause filterClause1 = CreateFilterClause(filterExpression1);
            FilterClause filterClause2 = CreateFilterClause(filterExpression2);
            ParameterAliasNode expression1 = filterClause1.Expression as ParameterAliasNode;
            ParameterAliasNode expression2 = filterClause2.Expression as ParameterAliasNode;
            FilterSegment filterSegment1 = new FilterSegment(expression1, filterClause1.RangeVariable, HardCodedTestModel.GetPet1Set());
            FilterSegment filterSegment2 = new FilterSegment(expression2, filterClause2.RangeVariable, HardCodedTestModel.GetPet1Set());

            filterSegment1.Equals(filterSegment2).Should().BeFalse();
            filterSegment2.Equals(filterSegment1).Should().BeFalse();
        }

        [Fact]
        public void FilterSegmentsWithDifferentNavigationSourcesShouldNotBeEqual()
        {
            const string filterExpression1 = "@a1";

            FilterClause filterClause = CreateFilterClause(filterExpression1);
            ParameterAliasNode expression = filterClause.Expression as ParameterAliasNode;
            FilterSegment filterSegment1 = new FilterSegment(expression, filterClause.RangeVariable, HardCodedTestModel.GetPet1Set());
            FilterSegment filterSegment2 = new FilterSegment(expression, filterClause.RangeVariable, HardCodedTestModel.GetPeopleSet());

            filterSegment1.Equals(filterSegment2).Should().BeFalse();
            filterSegment2.Equals(filterSegment1).Should().BeFalse();
        }
        #endregion

        #region Helper Functions
        private FilterClause CreateFilterClause(string filterExpression)
        {
            const string uriBase = "http://service/";

            Uri uri = new Uri(uriBase + "Pet1Set?$filter=" + filterExpression);
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri(uriBase), uri);
            IEdmEntitySet entitySet = HardCodedTestModel.GetPet1Set();

            Dictionary<string, string> queryOptions = new Dictionary<string, string>()
            {
                { "$filter", filterExpression }
            };

            // Use query option parser to help create the filter because it takes care of a bunch of instantiations
            ODataQueryOptionParser queryOptionParser =
                new ODataQueryOptionParser(HardCodedTestModel.TestModel, entitySet.EntityType(), entitySet, queryOptions);

            return queryOptionParser.ParseFilter();
        }
        #endregion
    }
}
