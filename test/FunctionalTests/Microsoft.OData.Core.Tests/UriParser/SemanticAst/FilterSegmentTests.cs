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
        public void FilterSegmentConstructWithNullAliasShouldThrowException()
        {
            const string filterExpression = "@a1";

            FilterClause filterClause = CreateFilterClause(filterExpression);

            Action create = () => new FilterSegment(null, filterClause.RangeVariable, HardCodedTestModel.GetPet1Type(), false);
            create.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: alias");
        }

        [Fact]
        public void FilterSegmentConstructWithNullRangeVariableShouldThrowException()
        {
            const string filterExpression = "@a1";

            FilterClause filterClause = CreateFilterClause(filterExpression);
            ParameterAliasNode alias = filterClause.Expression as ParameterAliasNode;

            Action create = () => new FilterSegment(alias, null, HardCodedTestModel.GetPet1Type(), false);
            create.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: rangeVariable");
        }

        [Fact]
        public void FilterSegmentConstructWithNullTypeShouldThrowException()
        {
            const string filterExpression = "@a1";

            FilterClause filterClause = CreateFilterClause(filterExpression);
            ParameterAliasNode alias = filterClause.Expression as ParameterAliasNode;

            Action create = () => new FilterSegment(alias, filterClause.RangeVariable, null, false);
            create.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: bindingType");
        }

        [Fact]
        public void FilterSegmentWithAllParametersSuccessfully()
        {
            const string filterExpression = "@a1";

            FilterClause filterClause = CreateFilterClause(filterExpression);
            ParameterAliasNode alias = filterClause.Expression as ParameterAliasNode;
            FilterSegment filterSegment = new FilterSegment(alias, filterClause.RangeVariable, HardCodedTestModel.GetPet1Type(), false);

            filterSegment.Alias.Equals(alias).Should().BeTrue();
            filterSegment.RangeVariable.Equals(filterClause.RangeVariable).Should().BeTrue();
        }

        [Fact]
        public void FilterSegmentShouldNotBeSingleResult()
        {
            const string filterExpression = "@a1";

            FilterClause filterClause = CreateFilterClause(filterExpression);
            ParameterAliasNode alias = filterClause.Expression as ParameterAliasNode;
            FilterSegment filterSegment = new FilterSegment(alias, filterClause.RangeVariable, HardCodedTestModel.GetPet1Type(), false);

            filterSegment.SingleResult.Should().BeFalse();
        }

        [Fact]
        public void FilterSegmentShouldHaveSameTypeAsRangeVariableTypeDefinition()
        {
            const string filterExpression = "@a1";

            FilterClause filterClause = CreateFilterClause(filterExpression);
            ParameterAliasNode alias = filterClause.Expression as ParameterAliasNode;
            FilterSegment filterSegment = new FilterSegment(alias, filterClause.RangeVariable, HardCodedTestModel.GetPet1Type(), false);

            filterSegment.EdmType.Equals(filterClause.RangeVariable.TypeReference.Definition).Should().BeTrue();
        }

        [Fact]
        public void FilterSegmentsWithSameExpressionShouldBeEqual()
        {
            const string filterExpression = "@a1";

            FilterClause filterClause = CreateFilterClause(filterExpression);
            ParameterAliasNode alias = filterClause.Expression as ParameterAliasNode;
            FilterSegment filterSegment1 = new FilterSegment(alias, filterClause.RangeVariable, HardCodedTestModel.GetPet1Type(), false);
            FilterSegment filterSegment2 = new FilterSegment(alias, filterClause.RangeVariable, HardCodedTestModel.GetPet1Type(), false);

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
            ParameterAliasNode alias1 = filterClause1.Expression as ParameterAliasNode;
            ParameterAliasNode alias2 = filterClause2.Expression as ParameterAliasNode;
            FilterSegment filterSegment1 = new FilterSegment(alias1, filterClause1.RangeVariable, HardCodedTestModel.GetPet1Type(), false);
            FilterSegment filterSegment2 = new FilterSegment(alias2, filterClause2.RangeVariable, HardCodedTestModel.GetPet1Type(), false);

            filterSegment1.Equals(filterSegment2).Should().BeFalse();
            filterSegment2.Equals(filterSegment1).Should().BeFalse();
        }

        [Fact]
        public void FilterSegmentsWithDifferentTypesShouldNotBeEqual()
        {
            const string filterExpression1 = "@a1";

            FilterClause filterClause = CreateFilterClause(filterExpression1);
            ParameterAliasNode alias = filterClause.Expression as ParameterAliasNode;
            FilterSegment filterSegment1 = new FilterSegment(alias, filterClause.RangeVariable, HardCodedTestModel.GetPet1Type(), false);
            FilterSegment filterSegment2 = new FilterSegment(alias, filterClause.RangeVariable, HardCodedTestModel.GetPersonType(), false);

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
