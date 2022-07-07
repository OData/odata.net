//---------------------------------------------------------------------
// <copyright file="DollarApplyGroupByTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using Xunit;

namespace Microsoft.OData.Client.Tests.ALinq
{
    public class DollarApplyGroupByTests : DollarApplyTestsBase
    {
        public DollarApplyGroupByTests() : base()
        {
        }

        [Fact]
        public void GroupByResultSelector_Sum_ByConstant()
        {
            var queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1, (d2, d3) => new
            {
                SumIntProp = d3.Sum(d4 => d4.IntProp),
                SumNullableIntProp = d3.Sum(d4 => d4.NullableIntProp),
                SumDoubleProp = d3.Sum(d4 => d4.DoubleProp),
                SumNullableDoubleProp = d3.Sum(d4 => d4.NullableDoubleProp),
                SumDecimalProp = d3.Sum(d4 => d4.DecimalProp),
                SumNullableDecimalProp = d3.Sum(d4 => d4.NullableDecimalProp),
                SumLongProp = d3.Sum(d4 => d4.LongProp),
                SumNullableLongProp = d3.Sum(d4 => d4.NullableLongProp),
                SumSingleProp = d3.Sum(d4 => d4.SingleProp),
                SumNullableSingleProp = d3.Sum(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=aggregate(" +
                    "IntProp with sum as SumIntProp,NullableIntProp with sum as SumNullableIntProp," +
                    "DoubleProp with sum as SumDoubleProp,NullableDoubleProp with sum as SumNullableDoubleProp," +
                    "DecimalProp with sum as SumDecimalProp,NullableDecimalProp with sum as SumNullableDecimalProp," +
                    "LongProp with sum as SumLongProp,NullableLongProp with sum as SumNullableLongProp," +
                    "SingleProp with sum as SumSingleProp,NullableSingleProp with sum as SumNullableSingleProp)", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Sum_ByConstant();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Single(aggregateResult);

            var singleResult = aggregateResult.First();

            Assert.Equal(506, singleResult.SumIntProp);
            Assert.Equal(530, singleResult.SumNullableIntProp);
            Assert.Equal(464.72, singleResult.SumDoubleProp);
            Assert.Equal(534.02, singleResult.SumNullableDoubleProp);
            Assert.Equal(559.4M, singleResult.SumDecimalProp);
            Assert.Equal(393.7M, singleResult.SumNullableDecimalProp);
            Assert.Equal(1298L, singleResult.SumLongProp);
            Assert.Equal(993L, singleResult.SumNullableLongProp);
            Assert.Equal(333.79f, singleResult.SumSingleProp);
            Assert.Equal(528.44f, singleResult.SumNullableSingleProp);
        }

        [Fact]
        public void GroupByResultSelector_Sum_BySingleProperty()
        {
            var queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.RowParity, (d2, d3) => new
            {
                RowParity = d2,
                SumIntProp = d3.Sum(d4 => d4.IntProp),
                SumNullableIntProp = d3.Sum(d4 => d4.NullableIntProp),
                SumDoubleProp = d3.Sum(d4 => d4.DoubleProp),
                SumNullableDoubleProp = d3.Sum(d4 => d4.NullableDoubleProp),
                SumDecimalProp = d3.Sum(d4 => d4.DecimalProp),
                SumNullableDecimalProp = d3.Sum(d4 => d4.NullableDecimalProp),
                SumLongProp = d3.Sum(d4 => d4.LongProp),
                SumNullableLongProp = d3.Sum(d4 => d4.NullableLongProp),
                SumSingleProp = d3.Sum(d4 => d4.SingleProp),
                SumNullableSingleProp = d3.Sum(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=groupby((RowParity),aggregate(" +
                    "IntProp with sum as SumIntProp,NullableIntProp with sum as SumNullableIntProp," +
                    "DoubleProp with sum as SumDoubleProp,NullableDoubleProp with sum as SumNullableDoubleProp," +
                    "DecimalProp with sum as SumDecimalProp,NullableDecimalProp with sum as SumNullableDecimalProp," +
                    "LongProp with sum as SumLongProp,NullableLongProp with sum as SumNullableLongProp," +
                    "SingleProp with sum as SumSingleProp,NullableSingleProp with sum as SumNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Sum_BySingleProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Even", aggregateResult[0].RowParity);
            Assert.Equal("Odd", aggregateResult[1].RowParity);
        }

        [Fact]
        public void GroupByResultSelector_Sum_ByMultipleProperties()
        {
            var queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.RowParity, d1.RowCategory }, (d2, d3) => new
            {
                d2.RowParity,
                d2.RowCategory,
                SumIntProp = d3.Sum(d4 => d4.IntProp),
                SumNullableIntProp = d3.Sum(d4 => d4.NullableIntProp),
                SumDoubleProp = d3.Sum(d4 => d4.DoubleProp),
                SumNullableDoubleProp = d3.Sum(d4 => d4.NullableDoubleProp),
                SumDecimalProp = d3.Sum(d4 => d4.DecimalProp),
                SumNullableDecimalProp = d3.Sum(d4 => d4.NullableDecimalProp),
                SumLongProp = d3.Sum(d4 => d4.LongProp),
                SumNullableLongProp = d3.Sum(d4 => d4.NullableLongProp),
                SumSingleProp = d3.Sum(d4 => d4.SingleProp),
                SumNullableSingleProp = d3.Sum(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=groupby((RowParity,RowCategory),aggregate(" +
                    "IntProp with sum as SumIntProp,NullableIntProp with sum as SumNullableIntProp," +
                    "DoubleProp with sum as SumDoubleProp,NullableDoubleProp with sum as SumNullableDoubleProp," +
                    "DecimalProp with sum as SumDecimalProp,NullableDecimalProp with sum as SumNullableDecimalProp," +
                    "LongProp with sum as SumLongProp,NullableLongProp with sum as SumNullableLongProp," +
                    "SingleProp with sum as SumSingleProp,NullableSingleProp with sum as SumNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Sum_ByMultipleProperties();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(3, aggregateResult.Length);
            Assert.True(aggregateResult[0].RowParity.Equals("Even") && aggregateResult[0].RowCategory.Equals("Composite"));
            Assert.True(aggregateResult[1].RowParity.Equals("Odd") && aggregateResult[1].RowCategory.Equals("None"));
            Assert.True(aggregateResult[2].RowParity.Equals("Odd") && aggregateResult[2].RowCategory.Equals("Prime"));
        }

        [Fact]
        public void GroupByResultSelector_Average_ByConstant()
        {
            var queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1, (d2, d3) => new
            {
                AverageIntProp = d3.Average(d4 => d4.IntProp),
                AverageNullableIntProp = d3.Average(d4 => d4.NullableIntProp),
                AverageDoubleProp = d3.Average(d4 => d4.DoubleProp),
                AverageNullableDoubleProp = d3.Average(d4 => d4.NullableDoubleProp),
                AverageDecimalProp = d3.Average(d4 => d4.DecimalProp),
                AverageNullableDecimalProp = d3.Average(d4 => d4.NullableDecimalProp),
                AverageLongProp = d3.Average(d4 => d4.LongProp),
                AverageNullableLongProp = d3.Average(d4 => d4.NullableLongProp),
                AverageSingleProp = d3.Average(d4 => d4.SingleProp),
                AverageNullableSingleProp = d3.Average(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=aggregate(" +
                    "IntProp with average as AverageIntProp,NullableIntProp with average as AverageNullableIntProp," +
                    "DoubleProp with average as AverageDoubleProp,NullableDoubleProp with average as AverageNullableDoubleProp," +
                    "DecimalProp with average as AverageDecimalProp,NullableDecimalProp with average as AverageNullableDecimalProp," +
                    "LongProp with average as AverageLongProp,NullableLongProp with average as AverageNullableLongProp," +
                    "SingleProp with average as AverageSingleProp,NullableSingleProp with average as AverageNullableSingleProp)", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Average_ByConstant();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Single(aggregateResult);

            var singleResult = aggregateResult.First();

            Assert.Equal(101.2, singleResult.AverageIntProp);
            Assert.Equal(132.5, singleResult.AverageNullableIntProp);
            Assert.Equal(92.944, singleResult.AverageDoubleProp);
            Assert.Equal(133.505, singleResult.AverageNullableDoubleProp);
            Assert.Equal(111.88M, singleResult.AverageDecimalProp);
            Assert.Equal(98.425M, singleResult.AverageNullableDecimalProp);
            Assert.Equal(259.6, singleResult.AverageLongProp);
            Assert.Equal(248.25, singleResult.AverageNullableLongProp);
            Assert.Equal(66.758f, singleResult.AverageSingleProp);
            Assert.Equal(132.11f, singleResult.AverageNullableSingleProp);
        }

        [Fact]
        public void GroupByResultSelector_Average_BySingleProperty()
        {
            var queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.RowParity, (d2, d3) => new
            {
                RowParity = d2,
                AverageIntProp = d3.Average(d4 => d4.IntProp),
                AverageNullableIntProp = d3.Average(d4 => d4.NullableIntProp),
                AverageDoubleProp = d3.Average(d4 => d4.DoubleProp),
                AverageNullableDoubleProp = d3.Average(d4 => d4.NullableDoubleProp),
                AverageDecimalProp = d3.Average(d4 => d4.DecimalProp),
                AverageNullableDecimalProp = d3.Average(d4 => d4.NullableDecimalProp),
                AverageLongProp = d3.Average(d4 => d4.LongProp),
                AverageNullableLongProp = d3.Average(d4 => d4.NullableLongProp),
                AverageSingleProp = d3.Average(d4 => d4.SingleProp),
                AverageNullableSingleProp = d3.Average(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=groupby((RowParity),aggregate(" +
                    "IntProp with average as AverageIntProp,NullableIntProp with average as AverageNullableIntProp," +
                    "DoubleProp with average as AverageDoubleProp,NullableDoubleProp with average as AverageNullableDoubleProp," +
                    "DecimalProp with average as AverageDecimalProp,NullableDecimalProp with average as AverageNullableDecimalProp," +
                    "LongProp with average as AverageLongProp,NullableLongProp with average as AverageNullableLongProp," +
                    "SingleProp with average as AverageSingleProp,NullableSingleProp with average as AverageNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Average_BySingleProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Even", aggregateResult[0].RowParity);
            Assert.Equal("Odd", aggregateResult[1].RowParity);
        }

        [Fact]
        public void GroupByResultSelector_Average_ByMultipleProperties()
        {
            var queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.RowParity, d1.RowCategory }, (d2, d3) => new
            {
                d2.RowParity,
                d2.RowCategory,
                AverageIntProp = d3.Average(d4 => d4.IntProp),
                AverageNullableIntProp = d3.Average(d4 => d4.NullableIntProp),
                AverageDoubleProp = d3.Average(d4 => d4.DoubleProp),
                AverageNullableDoubleProp = d3.Average(d4 => d4.NullableDoubleProp),
                AverageDecimalProp = d3.Average(d4 => d4.DecimalProp),
                AverageNullableDecimalProp = d3.Average(d4 => d4.NullableDecimalProp),
                AverageLongProp = d3.Average(d4 => d4.LongProp),
                AverageNullableLongProp = d3.Average(d4 => d4.NullableLongProp),
                AverageSingleProp = d3.Average(d4 => d4.SingleProp),
                AverageNullableSingleProp = d3.Average(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=groupby((RowParity,RowCategory),aggregate(" +
                    "IntProp with average as AverageIntProp,NullableIntProp with average as AverageNullableIntProp," +
                    "DoubleProp with average as AverageDoubleProp,NullableDoubleProp with average as AverageNullableDoubleProp," +
                    "DecimalProp with average as AverageDecimalProp,NullableDecimalProp with average as AverageNullableDecimalProp," +
                    "LongProp with average as AverageLongProp,NullableLongProp with average as AverageNullableLongProp," +
                    "SingleProp with average as AverageSingleProp,NullableSingleProp with average as AverageNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Average_ByMultipleProperties();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(3, aggregateResult.Length);
            Assert.True(aggregateResult[0].RowParity.Equals("Even") && aggregateResult[0].RowCategory.Equals("Composite"));
            Assert.True(aggregateResult[1].RowParity.Equals("Odd") && aggregateResult[1].RowCategory.Equals("None"));
            Assert.True(aggregateResult[2].RowParity.Equals("Odd") && aggregateResult[2].RowCategory.Equals("Prime"));
        }

        [Fact]
        public void GroupByResultSelector_Min_ByConstant()
        {
            var queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1, (d2, d3) => new
            {
                MinIntProp = d3.Min(d4 => d4.IntProp),
                MinNullableIntProp = d3.Min(d4 => d4.NullableIntProp),
                MinDoubleProp = d3.Min(d4 => d4.DoubleProp),
                MinNullableDoubleProp = d3.Min(d4 => d4.NullableDoubleProp),
                MinDecimalProp = d3.Min(d4 => d4.DecimalProp),
                MinNullableDecimalProp = d3.Min(d4 => d4.NullableDecimalProp),
                MinLongProp = d3.Min(d4 => d4.LongProp),
                MinNullableLongProp = d3.Min(d4 => d4.NullableLongProp),
                MinSingleProp = d3.Min(d4 => d4.SingleProp),
                MinNullableSingleProp = d3.Min(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=aggregate(" +
                    "IntProp with min as MinIntProp,NullableIntProp with min as MinNullableIntProp," +
                    "DoubleProp with min as MinDoubleProp,NullableDoubleProp with min as MinNullableDoubleProp," +
                    "DecimalProp with min as MinDecimalProp,NullableDecimalProp with min as MinNullableDecimalProp," +
                    "LongProp with min as MinLongProp,NullableLongProp with min as MinNullableLongProp," +
                    "SingleProp with min as MinSingleProp,NullableSingleProp with min as MinNullableSingleProp)", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Min_ByConstant();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Single(aggregateResult);

            var singleResult = aggregateResult.First();

            Assert.Equal(63, singleResult.MinIntProp);
            Assert.Equal(34, singleResult.MinNullableIntProp);
            Assert.Equal(2.34, singleResult.MinDoubleProp);
            Assert.Equal(16.1, singleResult.MinNullableDoubleProp);
            Assert.Equal(42.70M, singleResult.MinDecimalProp);
            Assert.Equal(12.90M, singleResult.MinNullableDecimalProp);
            Assert.Equal(220L, singleResult.MinLongProp);
            Assert.Equal(201L, singleResult.MinNullableLongProp);
            Assert.Equal(1.29f, singleResult.MinSingleProp);
            Assert.Equal(81.94f, singleResult.MinNullableSingleProp);
        }

        [Fact]
        public void GroupByResultSelector_Min_BySingleProperty()
        {
            var queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.RowParity, (d2, d3) => new
            {
                RowParity = d2,
                MinIntProp = d3.Min(d4 => d4.IntProp),
                MinNullableIntProp = d3.Min(d4 => d4.NullableIntProp),
                MinDoubleProp = d3.Min(d4 => d4.DoubleProp),
                MinNullableDoubleProp = d3.Min(d4 => d4.NullableDoubleProp),
                MinDecimalProp = d3.Min(d4 => d4.DecimalProp),
                MinNullableDecimalProp = d3.Min(d4 => d4.NullableDecimalProp),
                MinLongProp = d3.Min(d4 => d4.LongProp),
                MinNullableLongProp = d3.Min(d4 => d4.NullableLongProp),
                MinSingleProp = d3.Min(d4 => d4.SingleProp),
                MinNullableSingleProp = d3.Min(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=groupby((RowParity),aggregate(" +
                    "IntProp with min as MinIntProp,NullableIntProp with min as MinNullableIntProp," +
                    "DoubleProp with min as MinDoubleProp,NullableDoubleProp with min as MinNullableDoubleProp," +
                    "DecimalProp with min as MinDecimalProp,NullableDecimalProp with min as MinNullableDecimalProp," +
                    "LongProp with min as MinLongProp,NullableLongProp with min as MinNullableLongProp," +
                    "SingleProp with min as MinSingleProp,NullableSingleProp with min as MinNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Min_BySingleProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Even", aggregateResult[0].RowParity);
            Assert.Equal("Odd", aggregateResult[1].RowParity);
        }

        [Fact]
        public void GroupByResultSelector_Min_ByMultiProperties()
        {
            var queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.RowParity, d1.RowCategory }, (d2, d3) => new
            {
                d2.RowParity,
                d2.RowCategory,
                MinIntProp = d3.Min(d4 => d4.IntProp),
                MinNullableIntProp = d3.Min(d4 => d4.NullableIntProp),
                MinDoubleProp = d3.Min(d4 => d4.DoubleProp),
                MinNullableDoubleProp = d3.Min(d4 => d4.NullableDoubleProp),
                MinDecimalProp = d3.Min(d4 => d4.DecimalProp),
                MinNullableDecimalProp = d3.Min(d4 => d4.NullableDecimalProp),
                MinLongProp = d3.Min(d4 => d4.LongProp),
                MinNullableLongProp = d3.Min(d4 => d4.NullableLongProp),
                MinSingleProp = d3.Min(d4 => d4.SingleProp),
                MinNullableSingleProp = d3.Min(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format(
                    "{0}/Numbers?$apply=groupby((RowParity,RowCategory),aggregate(" +
                    "IntProp with min as MinIntProp,NullableIntProp with min as MinNullableIntProp," +
                    "DoubleProp with min as MinDoubleProp,NullableDoubleProp with min as MinNullableDoubleProp," +
                    "DecimalProp with min as MinDecimalProp,NullableDecimalProp with min as MinNullableDecimalProp," +
                    "LongProp with min as MinLongProp,NullableLongProp with min as MinNullableLongProp," +
                    "SingleProp with min as MinSingleProp,NullableSingleProp with min as MinNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Min_ByMultipleProperties();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(3, aggregateResult.Length);
            Assert.True(aggregateResult[0].RowParity.Equals("Even") && aggregateResult[0].RowCategory.Equals("Composite"));
            Assert.True(aggregateResult[1].RowParity.Equals("Odd") && aggregateResult[1].RowCategory.Equals("None"));
            Assert.True(aggregateResult[2].RowParity.Equals("Odd") && aggregateResult[2].RowCategory.Equals("Prime"));
        }

        [Fact]
        public void GroupByResultSelector_Max_ByConstant()
        {
            var queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1, (d2, d3) => new
            {
                MaxIntProp = d3.Max(d4 => d4.IntProp),
                MaxNullableIntProp = d3.Max(d4 => d4.NullableIntProp),
                MaxDoubleProp = d3.Max(d4 => d4.DoubleProp),
                MaxNullableDoubleProp = d3.Max(d4 => d4.NullableDoubleProp),
                MaxDecimalProp = d3.Max(d4 => d4.DecimalProp),
                MaxNullableDecimalProp = d3.Max(d4 => d4.NullableDecimalProp),
                MaxLongProp = d3.Max(d4 => d4.LongProp),
                MaxNullableLongProp = d3.Max(d4 => d4.NullableLongProp),
                MaxSingleProp = d3.Max(d4 => d4.SingleProp),
                MaxNullableSingleProp = d3.Max(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=aggregate(" +
                "IntProp with max as MaxIntProp,NullableIntProp with max as MaxNullableIntProp," +
                "DoubleProp with max as MaxDoubleProp,NullableDoubleProp with max as MaxNullableDoubleProp," +
                "DecimalProp with max as MaxDecimalProp,NullableDecimalProp with max as MaxNullableDecimalProp," +
                "LongProp with max as MaxLongProp,NullableLongProp with max as MaxNullableLongProp," +
                "SingleProp with max as MaxSingleProp,NullableSingleProp with max as MaxNullableSingleProp)", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Max_ByConstant();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Single(aggregateResult);

            var singleResult = aggregateResult.First();

            Assert.Equal(141, singleResult.MaxIntProp);
            Assert.Equal(199, singleResult.MaxNullableIntProp);
            Assert.Equal(155.85, singleResult.MaxDoubleProp);
            Assert.Equal(178.49, singleResult.MaxNullableDoubleProp);
            Assert.Equal(173.90M, singleResult.MaxDecimalProp);
            Assert.Equal(157.30M, singleResult.MaxNullableDecimalProp);
            Assert.Equal(300L, singleResult.MaxLongProp);
            Assert.Equal(295L, singleResult.MaxNullableLongProp);
            Assert.Equal(171.22f, singleResult.MaxSingleProp);
            Assert.Equal(174.99f, singleResult.MaxNullableSingleProp);
        }

        [Fact]
        public void GroupByResultSelector_Max_BySingleProperty()
        {
            var queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.RowParity, (d2, d3) => new
            {
                RowParity = d2,
                MaxIntProp = d3.Max(d4 => d4.IntProp),
                MaxNullableIntProp = d3.Max(d4 => d4.NullableIntProp),
                MaxDoubleProp = d3.Max(d4 => d4.DoubleProp),
                MaxNullableDoubleProp = d3.Max(d4 => d4.NullableDoubleProp),
                MaxDecimalProp = d3.Max(d4 => d4.DecimalProp),
                MaxNullableDecimalProp = d3.Max(d4 => d4.NullableDecimalProp),
                MaxLongProp = d3.Max(d4 => d4.LongProp),
                MaxNullableLongProp = d3.Max(d4 => d4.NullableLongProp),
                MaxSingleProp = d3.Max(d4 => d4.SingleProp),
                MaxNullableSingleProp = d3.Max(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=groupby((RowParity),aggregate(" +
                "IntProp with max as MaxIntProp,NullableIntProp with max as MaxNullableIntProp," +
                "DoubleProp with max as MaxDoubleProp,NullableDoubleProp with max as MaxNullableDoubleProp," +
                "DecimalProp with max as MaxDecimalProp,NullableDecimalProp with max as MaxNullableDecimalProp," +
                "LongProp with max as MaxLongProp,NullableLongProp with max as MaxNullableLongProp," +
                "SingleProp with max as MaxSingleProp,NullableSingleProp with max as MaxNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Max_BySingleProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Even", aggregateResult[0].RowParity);
            Assert.Equal("Odd", aggregateResult[1].RowParity);
        }

        [Fact]
        public void GroupByResultSelector_Max_ByMultipleProperties()
        {
            var queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.RowParity, d1.RowCategory }, (d2, d3) => new
            {
                d2.RowParity,
                d2.RowCategory,
                MaxIntProp = d3.Max(d4 => d4.IntProp),
                MaxNullableIntProp = d3.Max(d4 => d4.NullableIntProp),
                MaxDoubleProp = d3.Max(d4 => d4.DoubleProp),
                MaxNullableDoubleProp = d3.Max(d4 => d4.NullableDoubleProp),
                MaxDecimalProp = d3.Max(d4 => d4.DecimalProp),
                MaxNullableDecimalProp = d3.Max(d4 => d4.NullableDecimalProp),
                MaxLongProp = d3.Max(d4 => d4.LongProp),
                MaxNullableLongProp = d3.Max(d4 => d4.NullableLongProp),
                MaxSingleProp = d3.Max(d4 => d4.SingleProp),
                MaxNullableSingleProp = d3.Max(d4 => d4.NullableSingleProp),
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=groupby((RowParity,RowCategory),aggregate(" +
                "IntProp with max as MaxIntProp,NullableIntProp with max as MaxNullableIntProp," +
                "DoubleProp with max as MaxDoubleProp,NullableDoubleProp with max as MaxNullableDoubleProp," +
                "DecimalProp with max as MaxDecimalProp,NullableDecimalProp with max as MaxNullableDecimalProp," +
                "LongProp with max as MaxLongProp,NullableLongProp with max as MaxNullableLongProp," +
                "SingleProp with max as MaxSingleProp,NullableSingleProp with max as MaxNullableSingleProp))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Max_ByMultipleProperties();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(3, aggregateResult.Length);
            Assert.True(aggregateResult[0].RowParity.Equals("Even") && aggregateResult[0].RowCategory.Equals("Composite"));
            Assert.True(aggregateResult[1].RowParity.Equals("Odd") && aggregateResult[1].RowCategory.Equals("None"));
            Assert.True(aggregateResult[2].RowParity.Equals("Odd") && aggregateResult[2].RowCategory.Equals("Prime"));
        }

        [Fact]
        public void GroupByResultSelector_Count_ByConstant()
        {
            var queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1, (d2, d3) => new
            {
                Count = d3.Count(),
                CountDistinct = d3.CountDistinct(d4 => d4.RowCategory)
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=aggregate(" +
                "$count as Count,RowCategory with countdistinct as CountDistinct)", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Count_ByConstant();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Single(aggregateResult);
            Assert.Equal(5, aggregateResult[0].Count);
            Assert.Equal(3, aggregateResult[0].CountDistinct);
        }

        [Fact]
        public void GroupByResultSelector_Count_BySingleProperty()
        {
            var queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.RowParity, (d2, d3) => new
            {
                RowParity = d2,
                Count = d3.Count(),
                CountDistinct = d3.CountDistinct(d4 => d4.RowCategory)
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=groupby((RowParity),aggregate(" +
                "$count as Count,RowCategory with countdistinct as CountDistinct))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Count_BySingleProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Even", aggregateResult[0].RowParity);
            Assert.Equal("Odd", aggregateResult[1].RowParity);
        }

        [Fact]
        public void GroupByResultSelector_Count_ByMultipleProperties()
        {
            var queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.RowParity, d1.RowCategory }, (d2, d3) => new
            {
                d2.RowParity,
                d2.RowCategory,
                Count = d3.Count(),
                CountDistinct = d3.CountDistinct(d4 => d4.RowCategory)
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=groupby((RowParity,RowCategory),aggregate(" +
                "$count as Count,RowCategory with countdistinct as CountDistinct))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_Count_ByMultipleProperties();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(3, aggregateResult.Length);
            Assert.True(aggregateResult[0].RowParity.Equals("Even") && aggregateResult[0].RowCategory.Equals("Composite"));
            Assert.True(aggregateResult[1].RowParity.Equals("Odd") && aggregateResult[1].RowCategory.Equals("None"));
            Assert.True(aggregateResult[2].RowParity.Equals("Odd") && aggregateResult[2].RowCategory.Equals("Prime"));
        }

        [Fact]
        public void GroupByResultSelector_MixedScenarios()
        {
            var queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.RowParity, (d2, d3) => new
            {
                RowParity = d2,
                SumIntProp = d3.Sum(d4 => d4.IntProp),
                AverageDoubleProp = d3.Average(d4 => d4.DoubleProp),
                MinDecimalProp = d3.Min(d4 => d4.DecimalProp),
                MaxLongProp = d3.Max(d4 => d4.LongProp),
                Count = d3.Count(),
                CountDistinct = d3.CountDistinct(d4 => d4.RowCategory)
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=groupby((RowParity),aggregate(" +
                "IntProp with sum as SumIntProp,DoubleProp with average as AverageDoubleProp," +
                "DecimalProp with min as MinDecimalProp,LongProp with max as MaxLongProp," +
                "$count as Count,RowCategory with countdistinct as CountDistinct))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_WithMixedAggregations();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Even", aggregateResult[0].RowParity);
            Assert.Equal("Odd", aggregateResult[1].RowParity);
        }

        [Fact]
        public void GroupByResultSelector_UsingMemberInitialization()
        {
            var queryable = this.dsContext.CreateQuery<Number>(numbersEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.RowParity, (d2, d3) => new NumbersGroupedResult
            {
                RowParity = d2,
                SumIntProp = d3.Sum(d4 => d4.IntProp),
                AverageDoubleProp = d3.Average(d4 => d4.DoubleProp),
                MinDecimalProp = d3.Min(d4 => d4.DecimalProp),
                MaxLongProp = d3.Max(d4 => d4.LongProp),
                Count = d3.Count(),
                CountDistinct = d3.CountDistinct(d4 => d4.RowCategory)
            });

            Assert.Equal(
                string.Format("{0}/Numbers?$apply=groupby((RowParity),aggregate(" +
                "IntProp with sum as SumIntProp,DoubleProp with average as AverageDoubleProp," +
                "DecimalProp with min as MinDecimalProp,LongProp with max as MaxLongProp," +
                "$count as Count,RowCategory with countdistinct as CountDistinct))", serviceUri),
                aggregateQuery.ToString()
            );

            MockGroupBy_WithMixedAggregations();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Even", aggregateResult[0].RowParity);
            Assert.Equal("Odd", aggregateResult[1].RowParity);
        }

        [Fact]
        public void GroupByResultSelector_BySingleNavProperty()
        {
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.Product.Color,
                (d1, d2) => new
                {
                    Color = d1,
                    SumAmount = d2.Sum(d3 => d3.Amount),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinAmount = d2.Min(d3 => d3.Amount),
                    MaxAmount = d2.Max(d3 => d3.Amount)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Color),aggregate(" +
                "Amount with sum as SumAmount,Amount with average as AvgAmount," +
                "Amount with min as MinAmount,Amount with max as MaxAmount))", serviceUri),
                aggregateQuery.ToString());

            MockGroupBySingleNavProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Brown", aggregateResult[0].Color);
            Assert.Equal(6M, aggregateResult[0].AvgAmount);
            Assert.Equal(8M, aggregateResult[0].MaxAmount);
            Assert.Equal("White", aggregateResult[1].Color);
            Assert.Equal(12M, aggregateResult[1].SumAmount);
            Assert.Equal(1M, aggregateResult[1].MinAmount);
        }

        [Fact]
        public void GroupByResultSelector_ByMultipleNavProperties()
        {
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.Product.Color, d1.Customer.Country },
                (d1, d2) => new
                {
                    d1.Color,
                    d1.Country,
                    SumAmount = d2.Sum(d3 => d3.Amount),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinAmount = d2.Min(d3 => d3.Amount),
                    MaxAmount = d2.Max(d3 => d3.Amount)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Color,Customer/Country),aggregate(" +
                "Amount with sum as SumAmount,Amount with average as AvgAmount," +
                "Amount with min as MinAmount,Amount with max as MaxAmount))", serviceUri),
                aggregateQuery.ToString());

            MockGroupByMultipleNavProperties();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Brown", aggregateResult[0].Color);
            Assert.Equal(6M, aggregateResult[0].AvgAmount);
            Assert.Equal(8M, aggregateResult[0].MaxAmount);
            Assert.Equal("Netherlands", aggregateResult[1].Country);
            Assert.Equal(5M, aggregateResult[1].SumAmount);
            Assert.Equal(1M, aggregateResult[1].MinAmount);
        }

        [Fact]
        public void GroupByResultSelector_BySingleNavProperty_TargetingNavProperty()
        {
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.Currency.Code,
                (d1, d2) => new
                {
                    Currency = d1,
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgTaxRate = d2.Average(d3 => d3.Product.TaxRate),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxTaxRate = d2.Max(d3 => d3.Product.TaxRate)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Currency/Code),aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Product/TaxRate with average as AvgTaxRate," +
                "Product/TaxRate with min as MinTaxRate,Product/TaxRate with max as MaxTaxRate))", serviceUri),
                aggregateQuery.ToString());

            MockGroupBySingleNavProperty_AggregationsTargetingNavProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("EUR", aggregateResult[0].Currency);
            Assert.Equal(0.113333M, aggregateResult[0].AvgTaxRate);
            Assert.Equal(0.14M, aggregateResult[0].MaxTaxRate);
            Assert.Equal("USD", aggregateResult[1].Currency);
            Assert.Equal(0.46M, aggregateResult[1].SumTaxRate);
            Assert.Equal(0.06M, aggregateResult[1].MinTaxRate);
        }

        [Fact]
        public void GroupByResultSelector_ByMultipleNavProperties_TargetingNavProperty()
        {
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.Product.Color, d1.Customer.Country },
                (d1, d2) => new
                {
                    d1.Color,
                    d1.Country,
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgTaxRate = d2.Average(d3 => d3.Product.TaxRate),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxTaxRate = d2.Max(d3 => d3.Product.TaxRate)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Color,Customer/Country),aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Product/TaxRate with average as AvgTaxRate," +
                "Product/TaxRate with min as MinTaxRate,Product/TaxRate with max as MaxTaxRate))", serviceUri),
                aggregateQuery.ToString());

            MockGroupByMultipleNavProperties_AggregationsTargetingNavProperty();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("White", aggregateResult[0].Color);
            Assert.Equal(0.113333M, aggregateResult[0].AvgTaxRate);
            Assert.Equal(0.14M, aggregateResult[0].MaxTaxRate);
            Assert.Equal("USA", aggregateResult[1].Country);
            Assert.Equal(0.12M, aggregateResult[1].SumTaxRate);
            Assert.Equal(0.06M, aggregateResult[1].MinTaxRate);
        }

        [Fact]
        public void GroupByResultSelector_ByConstant_TargetingNavProperty()
        {
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1,
                (d1, d2) => new
                {
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgTaxRate = d2.Average(d3 => d3.Product.TaxRate),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxTaxRate = d2.Max(d3 => d3.Product.TaxRate)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Product/TaxRate with average as AvgTaxRate," +
                "Product/TaxRate with min as MinTaxRate,Product/TaxRate with max as MaxTaxRate)", serviceUri),
                aggregateQuery.ToString());

            MockGroupByConstant_AggregationsTargetingNavProperty();

            var aggregateResult = Assert.Single(aggregateQuery.ToArray());

            Assert.Equal(0.8M, aggregateResult.SumTaxRate);
            Assert.Equal(0.06M, aggregateResult.MinTaxRate);
        }

        [Fact]
        public void GroupByResultSelector_ByConstant_MixedScenarios()
        {
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1,
                (d1, d2) => new
                {
                    GroupingConstant = d1,
                    GibberishConstant = "dfksjfl",
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxAmount = d2.Max(d3 => d3.Amount),
                    GroupCount = d2.Count(),
                    DistinctCurrency = d2.CountDistinct(d3 => d3.Currency.Code)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Amount with average as AvgAmount," +
                "Product/TaxRate with min as MinTaxRate,Amount with max as MaxAmount," +
                "$count as GroupCount,Currency/Code with countdistinct as DistinctCurrency)", serviceUri),
                aggregateQuery.ToString());

            MockGroupByConstant_MixedScenarios();

            var aggregateResult = Assert.Single(aggregateQuery.ToArray());

            Assert.Equal(1, aggregateResult.GroupingConstant);
            Assert.Equal("dfksjfl", aggregateResult.GibberishConstant);
            Assert.Equal(3M, aggregateResult.AvgAmount);
            Assert.Equal(8M, aggregateResult.MaxAmount);
            Assert.Equal(2, aggregateResult.DistinctCurrency);
        }

        [Fact]
        public void GroupByResultSelector_BySingleNavProperty_MixedScenarios()
        {
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.Product.Category.Id,
                (d1, d2) => new
                {
                    GibberishConstant = "dfksjfl",
                    CategoryId = d1,
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxAmount = d2.Max(d3 => d3.Amount),
                    GroupCount = d2.Count(),
                    DistinctCurrency = d2.CountDistinct(d3 => d3.Currency.Code)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Category/Id),aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Amount with average as AvgAmount," +
                "Product/TaxRate with min as MinTaxRate,Amount with max as MaxAmount," +
                "$count as GroupCount,Currency/Code with countdistinct as DistinctCurrency))", serviceUri),
                aggregateQuery.ToString());

            MockGroupBySingleNavProperty_MixedScenarios();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("dfksjfl", aggregateResult[0].GibberishConstant);
            Assert.Equal(4M, aggregateResult[0].AvgAmount);
            Assert.Equal(8M, aggregateResult[0].MaxAmount);
            Assert.Equal(2, aggregateResult[0].DistinctCurrency);
            Assert.Equal("PG2", aggregateResult[1].CategoryId);
            Assert.Equal(0.56M, aggregateResult[1].SumTaxRate);
            Assert.Equal(0.14M, aggregateResult[1].MinTaxRate);
            Assert.Equal(4, aggregateResult[1].GroupCount);
        }

        [Fact]
        public void GroupByResultSelector_ByMultipleNavProperties_MixedScenarios()
        {
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.Product.Category.Id, d1.Customer.Country },
                (d1, d2) => new
                {
                    GibberishConstant = "dfksjfl",
                    CategoryId = d1.Id,
                    d1.Country,
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxAmount = d2.Max(d3 => d3.Amount),
                    GroupCount = d2.Count(),
                    DistinctCurrency = d2.CountDistinct(d3 => d3.Currency.Code)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Category/Id,Customer/Country),aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Amount with average as AvgAmount," +
                "Product/TaxRate with min as MinTaxRate,Amount with max as MaxAmount," +
                "$count as GroupCount,Currency/Code with countdistinct as DistinctCurrency))", serviceUri),
                aggregateQuery.ToString());

            MockGroupByMultipleNavProperties_MixedScenarios();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("dfksjfl", aggregateResult[0].GibberishConstant);
            Assert.Equal("Netherlands", aggregateResult[0].Country);
            Assert.Equal(2M, aggregateResult[0].AvgAmount);
            Assert.Equal(2M, aggregateResult[0].MaxAmount);
            Assert.Equal(1, aggregateResult[0].DistinctCurrency);
            Assert.Equal("PG2", aggregateResult[1].CategoryId);
            Assert.Equal(0.28M, aggregateResult[1].SumTaxRate);
            Assert.Equal(0.14M, aggregateResult[1].MinTaxRate);
            Assert.Equal(2, aggregateResult[1].GroupCount);
        }

        [Fact]
        public void GroupByResultSelector_ByConstant_UsingMemberInitialization()
        {
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => 1,
                (d1, d2) => new SalesGroupedResult01
                {
                    GroupingConstant = d1,
                    GibberishConstant = "dfksjfl",
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxAmount = d2.Max(d3 => d3.Amount),
                    GroupCount = d2.Count(),
                    DistinctCurrency = d2.CountDistinct(d3 => d3.Currency.Code)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Amount with average as AvgAmount," +
                "Product/TaxRate with min as MinTaxRate,Amount with max as MaxAmount," +
                "$count as GroupCount,Currency/Code with countdistinct as DistinctCurrency)", serviceUri),
                aggregateQuery.ToString());

            MockGroupByConstant_MixedScenarios();

            var aggregateResult = Assert.Single(aggregateQuery.ToArray());

            Assert.Equal(1, aggregateResult.GroupingConstant);
            Assert.Equal("dfksjfl", aggregateResult.GibberishConstant);
            Assert.Equal(0.8M, aggregateResult.SumTaxRate);
            Assert.Equal(0.06M, aggregateResult.MinTaxRate);
            Assert.Equal(8, aggregateResult.GroupCount);
        }

        [Fact]
        public void GroupByResultSelector_BySingleNavProperty_UsingMemberInitialization()
        {
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => d1.Product.Category.Id,
                (d1, d2) => new SalesGroupedResult02
                {
                    GibberishConstant = "dfksjfl",
                    CategoryId = d1,
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxAmount = d2.Max(d3 => d3.Amount),
                    GroupCount = d2.Count(),
                    DistinctCurrency = d2.CountDistinct(d3 => d3.Currency.Code)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Category/Id),aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Amount with average as AvgAmount," +
                "Product/TaxRate with min as MinTaxRate,Amount with max as MaxAmount," +
                "$count as GroupCount,Currency/Code with countdistinct as DistinctCurrency))", serviceUri),
                aggregateQuery.ToString());

            MockGroupBySingleNavProperty_MixedScenarios();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("PG1", aggregateResult[0].CategoryId);
            Assert.Equal(0.24M, aggregateResult[0].SumTaxRate);
            Assert.Equal(0.06M, aggregateResult[0].MinTaxRate);
            Assert.Equal(4, aggregateResult[0].GroupCount);
            Assert.Equal("dfksjfl", aggregateResult[1].GibberishConstant);
            Assert.Equal(2M, aggregateResult[1].AvgAmount);
            Assert.Equal(4M, aggregateResult[1].MaxAmount);
            Assert.Equal(2, aggregateResult[1].DistinctCurrency);
        }

        [Fact]
        public void GroupByResultSelector_ByMultipleNavProperties_UsingMemberInitialization()
        {
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(d1 => new { d1.Product.Category.Id, d1.Customer.Country },
                (d1, d2) => new SalesGroupedResult03
                {
                    GibberishConstant = "dfksjfl",
                    CategoryId = d1.Id,
                    Country = d1.Country,
                    SumTaxRate = d2.Sum(d3 => d3.Product.TaxRate),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinTaxRate = d2.Min(d3 => d3.Product.TaxRate),
                    MaxAmount = d2.Max(d3 => d3.Amount),
                    GroupCount = d2.Count(),
                    DistinctCurrency = d2.CountDistinct(d3 => d3.Currency.Code)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Category/Id,Customer/Country),aggregate(" +
                "Product/TaxRate with sum as SumTaxRate,Amount with average as AvgAmount," +
                "Product/TaxRate with min as MinTaxRate,Amount with max as MaxAmount," +
                "$count as GroupCount,Currency/Code with countdistinct as DistinctCurrency))", serviceUri),
                aggregateQuery.ToString());

            MockGroupByMultipleNavProperties_MixedScenarios();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("dfksjfl", aggregateResult[0].GibberishConstant);
            Assert.Equal("PG1", aggregateResult[0].CategoryId);
            Assert.Equal(0.06M, aggregateResult[0].SumTaxRate);
            Assert.Equal(0.06M, aggregateResult[0].MinTaxRate);
            Assert.Equal(1, aggregateResult[0].GroupCount);
            Assert.Equal("Netherlands", aggregateResult[1].Country);
            Assert.Equal(1.5M, aggregateResult[1].AvgAmount);
            Assert.Equal(2M, aggregateResult[1].MaxAmount);
            Assert.Equal(1, aggregateResult[1].DistinctCurrency);
        }

        [Fact]
        public void GroupByResultSelector_UsingConstructorInitialization()
        {
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(
                d1 => d1.Product.Category.Name,
                (d1, d2) => new SalesGroupedResult04(d1, d2.Average(d3 => d3.Amount)));

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Category/Name),aggregate(" +
                "Amount with average as averageAmount))", serviceUri),
                aggregateQuery.ToString());

            MockGroupBy_ConstructorInitialization();

            var aggregateResult = Assert.Single(aggregateQuery.ToArray());

            Assert.Equal("Food", aggregateResult.CategoryName);
            Assert.Equal(4M, aggregateResult.AverageAmount);
        }

        [Fact]
        public void GroupByResultSelector_UsingConstructorAndMemberInitialization()
        {
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(
                d1 => d1.Product.Category.Name,
                (d1, d2) => new SalesGroupedResult05(d2.Average(d3 => d3.Amount))
                {
                    CategoryName = d1
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Category/Name),aggregate(" +
                "Amount with average as averageAmount))", serviceUri),
                aggregateQuery.ToString());

            MockGroupBy_ConstructorInitialization();

            var aggregateResult = Assert.Single(aggregateQuery.ToArray());

            Assert.Equal("Food", aggregateResult.CategoryName);
            Assert.Equal(4M, aggregateResult.AverageAmount);
        }

        [Fact]
        public void GroupByResultSelector_WithSupportedMemberAccessOnConstantGroupingExpression()
        {
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(
                d1 => "foobar",
                (d2, d3) => new
                {
                    FoobarLength = d2.Length,
                    AverageAmount = d3.Average(d4 => d4.Amount)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=aggregate(Amount with average as AverageAmount)", serviceUri),
                aggregateQuery.ToString());

            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Sales" +
                    "(AverageAmount)\"," +
                    "\"value\":[{{\"@odata.id\":null,\"AverageAmount\":4.000000}}]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);

            var aggregateResultEnumerator = aggregateQuery.GetEnumerator();
            aggregateResultEnumerator.MoveNext();
            var aggregateResult = aggregateResultEnumerator.Current;

            Assert.Equal(6, aggregateResult.FoobarLength);
            Assert.Equal(4M, aggregateResult.AverageAmount);
        }

        [Fact]
        public void GroupByResultSelector_WithSupportedMemberAccessOnSinglePropertyGroupingExpression()
        {
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(
                d1 => d1.Time.Year,
                (d2, d3) => new
                {
                    YearStr = d2.ToString(),
                    AverageAmount = d3.Average(d4 => d4.Amount)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Time/Year),aggregate(Amount with average as AverageAmount))", serviceUri),
                aggregateQuery.ToString());

            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Sales" +
                    "(Time(Year),AverageAmount)\"," +
                    "\"value\":[{{\"@odata.id\":null,\"Time\":{{\"@odata.id\":null,\"Year\":2012}},\"AverageAmount\":4.000000}}]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);

            var aggregateResultEnumerator = aggregateQuery.GetEnumerator();
            aggregateResultEnumerator.MoveNext();
            var aggregateResult = aggregateResultEnumerator.Current;

            Assert.Equal("2012", aggregateResult.YearStr);
            Assert.Equal(4M, aggregateResult.AverageAmount);
        }

        [Fact]
        public void GroupByResultSelector_WithSupportedMethodCallOnKnownPrimitiveTypes()
        {
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(
                d1 => new { d1.Time.Year, CategoryName = d1.Product.Category.Name, d1.CurrencyCode },
                (d2, d3) => new
                {
                    FoobarLength = "foobar".Length,
                    TenStr = 10.ToString(),
                    YearStr = d2.Year.ToString(),
                    CategoryNameLength = d2.CategoryName.Length,
                    d2.CurrencyCode,
                    AverageAmount = d3.Average(d4 => d4.Amount).ToString(),
                    SumAmount = d3.Sum(d4 => d4.Amount),
                    MinAmount = d3.Min(d4 => d4.Amount).ToString()
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Time/Year,Product/Category/Name,CurrencyCode)," +
                "aggregate(Amount with average as AverageAmount,Amount with sum as SumAmount,Amount with min as MinAmount))", serviceUri),
                aggregateQuery.ToString());

            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Sales" +
                    "(Time(Year),Product(Category(Name)),AverageAmount)\"," +
                    "\"value\":[{{\"@odata.id\":null,\"CurrencyCode\":\"EUR\",\"AverageAmount\":1.500000,\"SumAmount\":3.00,\"MinAmount\":1.00," +
                    "\"Time\":{{\"@odata.id\":null,\"Year\":2012}}," +
                    "\"Product\":{{\"@odata.id\":null,\"Category\":{{\"@odata.id\":null,\"Name\":\"Non-Food\"}}}}}}]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);

            var aggregateResultEnumerator = aggregateQuery.GetEnumerator();
            aggregateResultEnumerator.MoveNext();
            var aggregateResult = aggregateResultEnumerator.Current;

            Assert.Equal(6, aggregateResult.FoobarLength);
            Assert.Equal("10", aggregateResult.TenStr);
            Assert.Equal("2012", aggregateResult.YearStr);
            Assert.Equal(8, aggregateResult.CategoryNameLength);
            Assert.Equal("EUR", aggregateResult.CurrencyCode);
            Assert.Equal("1.5", aggregateResult.AverageAmount);
            Assert.Equal(3M, aggregateResult.SumAmount);
            Assert.Equal("1", aggregateResult.MinAmount);
        }

        [Fact]
        public void GroupByResultSelector_UsingConstructorAndMemberInitializationWithSupportedMethodCallOnKnownPrimitiveTypes()
        {
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(
                d1 => new { d1.Time.Year, CategoryName = d1.Product.Category.Name, d1.CurrencyCode },
                (d2, d3) => new SalesGroupedResult06(d2.CategoryName.Length, d2.CurrencyCode, d3.Average(d4 => d4.Amount).ToString())
                {
                    FoobarLength = "foobar".Length,
                    TenStr = 10.ToString(),
                    YearStr = d2.Year.ToString(),
                    SumAmount = d3.Sum(d4 => d4.Amount),
                    MinAmount = d3.Min(d4 => d4.Amount).ToString()
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Time/Year,Product/Category/Name,CurrencyCode)," +
                "aggregate(Amount with average as averageAmount,Amount with sum as SumAmount,Amount with min as MinAmount))", serviceUri),
                aggregateQuery.ToString());

            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Sales" +
                    "(Time(Year),Product(Category(Name)),AverageAmount)\"," +
                    "\"value\":[{{\"@odata.id\":null,\"CurrencyCode\":\"EUR\",\"averageAmount\":1.500000,\"SumAmount\":3.00,\"MinAmount\":1.00," +
                    "\"Time\":{{\"@odata.id\":null,\"Year\":2012}}," +
                    "\"Product\":{{\"@odata.id\":null,\"Category\":{{\"@odata.id\":null,\"Name\":\"Non-Food\"}}}}}}]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);

            var aggregateResultEnumerator = aggregateQuery.GetEnumerator();
            aggregateResultEnumerator.MoveNext();
            var aggregateResult = aggregateResultEnumerator.Current;

            Assert.Equal(6, aggregateResult.FoobarLength);
            Assert.Equal("10", aggregateResult.TenStr);
            Assert.Equal("2012", aggregateResult.YearStr);
            Assert.Equal(8, aggregateResult.CategoryNameLength);
            Assert.Equal("EUR", aggregateResult.CurrencyCode);
            Assert.Equal("1.5", aggregateResult.AverageAmount);
            Assert.Equal(3M, aggregateResult.SumAmount);
            Assert.Equal("1", aggregateResult.MinAmount);
        }

        [Fact]
        public void GroupByResultSelector_UsingMemberInitializationInKeySelector()
        {
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(
                d1 => new SalesGroupingKey01 { Color = d1.Product.Color, Country = d1.Customer.Country },
                (d1, d2) => new
                {
                    d1.Color,
                    d1.Country,
                    SumAmount = d2.Sum(d3 => d3.Amount),
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    MinAmount = d2.Min(d3 => d3.Amount),
                    MaxAmount = d2.Max(d3 => d3.Amount)
                });

            Assert.Equal(
                string.Format("{0}/Sales?$apply=groupby((Product/Color,Customer/Country),aggregate(" +
                "Amount with sum as SumAmount,Amount with average as AvgAmount," +
                "Amount with min as MinAmount,Amount with max as MaxAmount))", serviceUri),
                aggregateQuery.ToString());

            MockGroupByMultipleNavProperties();

            var aggregateResult = aggregateQuery.ToArray();

            Assert.Equal(2, aggregateResult.Length);
            Assert.Equal("Brown", aggregateResult[0].Color);
            Assert.Equal(6M, aggregateResult[0].AvgAmount);
            Assert.Equal(8M, aggregateResult[0].MaxAmount);
            Assert.Equal("Netherlands", aggregateResult[1].Country);
            Assert.Equal(5M, aggregateResult[1].SumAmount);
            Assert.Equal(1M, aggregateResult[1].MinAmount);
        }

        [Fact]
        public void GroupByResultSelector_ConstructorInitializationInKeySelectorNotPermitted()
        {
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var aggregateQuery = queryable.GroupBy(
                d1 => new SalesGroupingKey02(d1.Product.Color, d1.Customer.Country),
                (d1, d2) => new
                {
                    d1.Color,
                    d1.Country,
                    AvgAmount = d2.Average(d3 => d3.Amount),
                    SumAmount = d2.Sum(d3 => d3.Amount)
                });

            Assert.Equal(
                "Error translating Linq expression to URI: " + Strings.ALinq_InvalidGroupByKeySelector("new SalesGroupingKey02(d1.Product.Color, d1.Customer.Country)"),
                aggregateQuery.ToString());
        }

        [Fact]
        public void GroupByResultSelector_OnFilteredInputSet_ExpressionTranslatedToExpectedUri()
        {
            // Arrange
            var queryable = this.dsContext.CreateQuery<Sale>(salesEntitySetName);

            var query = queryable.Where(d => d.CurrencyCode.Equals("USD"))
                .GroupBy(d1 => new { d1.Product.Color }, (d1, d2) => new
                {
                    ProductAvgTaxRate = d2.Average(d3 => d3.Product.TaxRate)
                });

            // Act & Assert
            var expectedAggregateUri = $"{serviceUri}/{salesEntitySetName}?$apply=filter(CurrencyCode eq 'USD')" +
                $"/groupby((Product/Color),aggregate(Product/TaxRate with average as ProductAvgTaxRate))";
            Assert.Equal(expectedAggregateUri, query.ToString());
        }

        #region Mock Aggregation Responses

        private void MockGroupBy_Sum_ByConstant()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "SumIntProp,SumNullableIntProp," +
                "SumDoubleProp,SumNullableDoubleProp," +
                "SumDecimalProp,SumNullableDecimalProp," +
                "SumLongProp,SumNullableLongProp," +
                "SumSingleProp,SumNullableSingleProp)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null," +
                "\"SumIntProp\":506,\"SumNullableIntProp\":530," +
                "\"SumDoubleProp\":464.72,\"SumNullableDoubleProp\":534.02," +
                "\"SumDecimalProp\":559.4,\"SumNullableDecimalProp\":393.7," +
                "\"SumLongProp\":1298,\"SumNullableLongProp\":993," +
                "\"SumSingleProp\":333.79,\"SumNullableSingleProp\":528.44}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Sum_BySingleProperty()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "RowParity," +
                "SumIntProp,SumNullableIntProp," +
                "SumDoubleProp,SumNullableDoubleProp," +
                "SumDecimalProp,SumNullableDecimalProp," +
                "SumLongProp,SumNullableLongProp," +
                "SumSingleProp,SumNullableSingleProp)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Even\"," +
                "\"SumIntProp\":132,\"SumNullableIntProp\":146," +
                "\"SumDoubleProp\":46.53,\"SumNullableDoubleProp\":343.8," +
                "\"SumDecimalProp\":342.30,\"SumNullableDecimalProp\":100.60," +
                "\"SumLongProp\":481,\"SumNullableLongProp\":544," +
                "\"SumSingleProp\":221.88,\"SumNullableSingleProp\":286.03}}," +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Odd\"," +
                "\"SumIntProp\":374,\"SumNullableIntProp\":384," +
                "\"SumDoubleProp\":418.19,\"SumNullableDoubleProp\":190.22," +
                "\"SumDecimalProp\":217.10,\"SumNullableDecimalProp\":293.10," +
                "\"SumLongProp\":817,\"SumNullableLongProp\":449," +
                "\"SumSingleProp\":111.91,\"SumNullableSingleProp\":242.41}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Sum_ByMultipleProperties()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "RowParity,RowCategory," +
                "SumIntProp,SumNullableIntProp," +
                "SumDoubleProp,SumNullableDoubleProp," +
                "SumDecimalProp,SumNullableDecimalProp," +
                "SumLongProp,SumNullableLongProp," +
                "SumSingleProp,SumNullableSingleProp)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Even\",\"RowCategory\":\"Composite\"," +
                "\"SumIntProp\":63,\"SumNullableIntProp\":146," +
                "\"SumDoubleProp\":44.19,\"SumNullableDoubleProp\":165.31," +
                "\"SumDecimalProp\":173.90,\"SumNullableDecimalProp\":null," +
                "\"SumLongProp\":259,\"SumNullableLongProp\":249," +
                "\"SumSingleProp\":171.22,\"SumNullableSingleProp\":174.99}}," +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Odd\",\"RowCategory\":\"None\"," +
                "\"SumIntProp\":109,\"SumNullableIntProp\":199," +
                "\"SumDoubleProp\":155.85,\"SumNullableDoubleProp\":null," +
                "\"SumDecimalProp\":101.60,\"SumNullableDecimalProp\":122.90," +
                "\"SumLongProp\":300,\"SumNullableLongProp\":201," +
                "\"SumSingleProp\":107.66,\"SumNullableSingleProp\":81.94}}," +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Odd\",\"RowCategory\":\"Prime\"," +
                "\"SumIntProp\":265,\"SumNullableIntProp\":185," +
                "\"SumDoubleProp\":262.34,\"SumNullableDoubleProp\":190.22," +
                "\"SumDecimalProp\":115.50,\"SumNullableDecimalProp\":170.20," +
                "\"SumLongProp\":517,\"SumNullableLongProp\":248," +
                "\"SumSingleProp\":4.25,\"SumNullableSingleProp\":160.47}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Average_ByConstant()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "AverageIntProp,AverageNullableIntProp," +
                "AverageDoubleProp,AverageNullableDoubleProp," +
                "AverageDecimalProp,AverageNullableDecimalProp," +
                "AverageLongProp,AverageNullableLongProp," +
                "AverageSingleProp,AverageNullableSingleProp)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null," +
                "\"AverageIntProp\":101.2,\"AverageNullableIntProp\":132.5," +
                "\"AverageDoubleProp\":92.944,\"AverageNullableDoubleProp\":133.505," +
                "\"AverageDecimalProp\":111.88,\"AverageNullableDecimalProp\":98.425," +
                "\"AverageLongProp\":259.6,\"AverageNullableLongProp\":248.25," +
                "\"AverageSingleProp\":66.758,\"AverageNullableSingleProp\":132.11}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Average_BySingleProperty()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "RowParity," +
                "AverageIntProp,AverageNullableIntProp," +
                "AverageDoubleProp,AverageNullableDoubleProp," +
                "AverageDecimalProp,AverageNullableDecimalProp," +
                "AverageLongProp,AverageNullableLongProp," +
                "AverageSingleProp,AverageNullableSingleProp)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Even\"," +
                "\"AverageIntProp\":66.0,\"AverageNullableIntProp\":146.0," +
                "\"AverageDoubleProp\":23.265,\"AverageNullableDoubleProp\":171.9," +
                "\"AverageDecimalProp\":171.15,\"AverageNullableDecimalProp\":100.60," +
                "\"AverageLongProp\":240.5,\"AverageNullableLongProp\":272.0," +
                "\"AverageSingleProp\":110.94,\"AverageNullableSingleProp\":143.015}}," +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Odd\"," +
                "\"AverageIntProp\":124.67,\"AverageNullableIntProp\":128.0," +
                "\"AverageDoubleProp\":139.397,\"AverageNullableDoubleProp\":95.11," +
                "\"AverageDecimalProp\":72.37,\"AverageNullableDecimalProp\":97.70," +
                "\"AverageLongProp\":272.33,\"AverageNullableLongProp\":224.5," +
                "\"AverageSingleProp\":37.30,\"AverageNullableSingleProp\":121.205}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Average_ByMultipleProperties()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "RowParity,RowCategory," +
                "AverageIntProp,AverageNullableIntProp," +
                "AverageDoubleProp,AverageNullableDoubleProp," +
                "AverageDecimalProp,AverageNullableDecimalProp," +
                "AverageLongProp,AverageNullableLongProp," +
                "AverageSingleProp,AverageNullableSingleProp)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Even\",\"RowCategory\":\"Composite\"," +
                "\"AverageNullableIntProp\":146,\"AverageIntProp\":63," +
                "\"AverageNullableDoubleProp\":165.31,\"AverageDoubleProp\":44.19," +
                "\"AverageNullableDecimalProp\":null,\"AverageDecimalProp\":173.9," +
                "\"AverageNullableLongProp\":249,\"AverageLongProp\":259," +
                "\"AverageNullableSingleProp\":174.99,\"AverageSingleProp\":171.22}}," +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Odd\",\"RowCategory\":\"None\"," +
                "\"AverageNullableIntProp\":199,\"AverageIntProp\":109," +
                "\"AverageNullableDoubleProp\":null,\"AverageDoubleProp\":155.85," +
                "\"AverageNullableDecimalProp\":122.9,\"AverageDecimalProp\":101.6," +
                "\"AverageNullableLongProp\":201,\"AverageLongProp\":300," +
                "\"AverageNullableSingleProp\":81.94,\"AverageSingleProp\":107.66}}," +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Odd\",\"RowCategory\":\"Prime\"," +
                "\"AverageNullableIntProp\":92.5,\"AverageIntProp\":132.5," +
                "\"AverageNullableDoubleProp\":95.11,\"AverageDoubleProp\":131.17," +
                "\"AverageNullableDecimalProp\":85.1,\"AverageDecimalProp\":57.75," +
                "\"AverageNullableLongProp\":248,\"AverageLongProp\":258.5," +
                "\"AverageNullableSingleProp\":160.47,\"AverageSingleProp\":2.125}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Min_ByConstant()
        {
            string mockResponse = string.Format("{{" +
                   "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                   "MinIntProp,MinNullableIntProp," +
                   "MinDoubleProp,MinNullableDoubleProp," +
                   "MinDecimalProp,MinNullableDecimalProp," +
                   "MinLongProp,MinNullableLongProp," +
                   "MinSingleProp,MinNullableSingleProp)\"," +
                   "\"value\":[" +
                   "{{\"@odata.id\":null," +
                   "\"MinIntProp\":63,\"MinNullableIntProp\":34," +
                   "\"MinDoubleProp\":2.34,\"MinNullableDoubleProp\":16.1," +
                   "\"MinDecimalProp\":42.70,\"MinNullableDecimalProp\":12.90," +
                   "\"MinLongProp\":220,\"MinNullableLongProp\":201," +
                   "\"MinSingleProp\":1.29,\"MinNullableSingleProp\":81.94}}" +
                   "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Min_BySingleProperty()
        {
            string mockResponse = string.Format("{{" +
                   "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                   "RowParity," +
                   "MinIntProp,MinNullableIntProp," +
                   "MinDoubleProp,MinNullableDoubleProp," +
                   "MinDecimalProp,MinNullableDecimalProp," +
                   "MinLongProp,MinNullableLongProp," +
                   "MinSingleProp,MinNullableSingleProp)\"," +
                   "\"value\":[" +
                   "{{\"@odata.id\":null," +
                   "\"RowParity\":\"Even\"," +
                   "\"MinIntProp\":63,\"MinNullableIntProp\":146," +
                   "\"MinDoubleProp\":2.34,\"MinNullableDoubleProp\":165.31," +
                   "\"MinDecimalProp\":168.40,\"MinNullableDecimalProp\":100.60," +
                   "\"MinLongProp\":222,\"MinNullableLongProp\":249," +
                   "\"MinSingleProp\":50.66,\"MinNullableSingleProp\":111.04}}," +
                   "{{\"@odata.id\":null," +
                   "\"RowParity\":\"Odd\"," +
                   "\"MinIntProp\":109,\"MinNullableIntProp\":34," +
                   "\"MinDoubleProp\":129.37,\"MinNullableDoubleProp\":16.1," +
                   "\"MinDecimalProp\":42.70,\"MinNullableDecimalProp\":12.90," +
                   "\"MinLongProp\":220,\"MinNullableLongProp\":201," +
                   "\"MinSingleProp\":1.29,\"MinNullableSingleProp\":81.94}}" +
                   "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Min_ByMultipleProperties()
        {
            string mockResponse = string.Format("{{" +
                   "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                   "RowParity,RowCategory" +
                   "MinIntProp,MinNullableIntProp," +
                   "MinDoubleProp,MinNullableDoubleProp," +
                   "MinDecimalProp,MinNullableDecimalProp," +
                   "MinLongProp,MinNullableLongProp," +
                   "MinSingleProp,MinNullableSingleProp)\"," +
                   "\"value\":[" +
                   "{{\"@odata.id\":null," +
                   "\"RowParity\":\"Even\",\"RowCategory\":\"Composite\"," +
                   "\"MinNullableIntProp\":146,\"MinIntProp\":63," +
                   "\"MinNullableDoubleProp\":165.31,\"MinDoubleProp\":44.19," +
                   "\"MinNullableDecimalProp\":null,\"MinDecimalProp\":173.9," +
                   "\"MinNullableLongProp\":249,\"MinLongProp\":259," +
                   "\"MinNullableSingleProp\":174.99,\"MinSingleProp\":171.22}}," +
                   "{{\"@odata.id\":null," +
                   "\"RowParity\":\"Odd\",\"RowCategory\":\"None\"," +
                   "\"MinNullableIntProp\":199,\"MinIntProp\":109," +
                   "\"MinNullableDoubleProp\":null,\"MinDoubleProp\":155.85," +
                   "\"MinNullableDecimalProp\":122.9,\"MinDecimalProp\":101.6," +
                   "\"MinNullableLongProp\":201,\"MinLongProp\":300," +
                   "\"MinNullableSingleProp\":81.94,\"MinSingleProp\":107.66}}," +
                   "{{\"@odata.id\":null," +
                   "\"RowParity\":\"Odd\",\"RowCategory\":\"Prime\"," +
                   "\"MinNullableIntProp\":34,\"MinIntProp\":124," +
                   "\"MinNullableDoubleProp\":16.1,\"MinDoubleProp\":129.37," +
                   "\"MinNullableDecimalProp\":12.9,\"MinDecimalProp\":42.7," +
                   "\"MinNullableLongProp\":248,\"MinLongProp\":220," +
                   "\"MinNullableSingleProp\":160.47,\"MinSingleProp\":1.29}}" +
                   "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Max_ByConstant()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "MaxIntProp,MaxNullableIntProp," +
                "MaxDoubleProp,MaxNullableDoubleProp," +
                "MaxDecimalProp,MaxNullableDecimalProp," +
                "MaxLongProp,MaxNullableLongProp," +
                "MaxSingleProp,MaxNullableSingleProp)\"," +
                "\"value\":[{{" +
                "\"@odata.id\":null," +
                "\"MaxIntProp\":141,\"MaxNullableIntProp\":199," +
                "\"MaxDoubleProp\":155.85,\"MaxNullableDoubleProp\":178.49," +
                "\"MaxDecimalProp\":173.90,\"MaxNullableDecimalProp\":157.30," +
                "\"MaxLongProp\":300,\"MaxNullableLongProp\":295," +
                "\"MaxSingleProp\":171.22,\"MaxNullableSingleProp\":174.99}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Max_BySingleProperty()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "RowParity," +
                "MaxIntProp,MaxNullableIntProp," +
                "MaxDoubleProp,MaxNullableDoubleProp," +
                "MaxDecimalProp,MaxNullableDecimalProp," +
                "MaxLongProp,MaxNullableLongProp," +
                "MaxSingleProp,MaxNullableSingleProp)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Even\"," +
                "\"MaxIntProp\":69,\"MaxNullableIntProp\":146," +
                "\"MaxDoubleProp\":44.19,\"MaxNullableDoubleProp\":178.49," +
                "\"MaxDecimalProp\":173.90,\"MaxNullableDecimalProp\":100.60," +
                "\"MaxLongProp\":259,\"MaxNullableLongProp\":295," +
                "\"MaxSingleProp\":171.22,\"MaxNullableSingleProp\":174.99}}," +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Odd\"," +
                "\"MaxIntProp\":141,\"MaxNullableIntProp\":199," +
                "\"MaxDoubleProp\":155.85,\"MaxNullableDoubleProp\":174.12," +
                "\"MaxDecimalProp\":101.60,\"MaxNullableDecimalProp\":157.30," +
                "\"MaxLongProp\":300,\"MaxNullableLongProp\":248," +
                "\"MaxSingleProp\":107.66,\"MaxNullableSingleProp\":160.47}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Max_ByMultipleProperties()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "RowParity,RowCategory," +
                "MaxIntProp,MaxNullableIntProp," +
                "MaxDoubleProp,MaxNullableDoubleProp," +
                "MaxDecimalProp,MaxNullableDecimalProp," +
                "MaxLongProp,MaxNullableLongProp," +
                "MaxSingleProp,MaxNullableSingleProp)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Even\",\"RowCategory\":\"Composite\"," +
                "\"MaxNullableIntProp\":146,\"MaxIntProp\":63," +
                "\"MaxNullableDoubleProp\":165.31,\"MaxDoubleProp\":44.19," +
                "\"MaxNullableDecimalProp\":null,\"MaxDecimalProp\":173.9," +
                "\"MaxNullableLongProp\":249,\"MaxLongProp\":259," +
                "\"MaxNullableSingleProp\":174.99,\"MaxSingleProp\":171.22}}," +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Odd\",\"RowCategory\":\"None\"," +
                "\"MaxNullableIntProp\":199,\"MaxIntProp\":109," +
                "\"MaxNullableDoubleProp\":null,\"MaxDoubleProp\":155.85," +
                "\"MaxNullableDecimalProp\":122.9,\"MaxDecimalProp\":101.6," +
                "\"MaxNullableLongProp\":201,\"MaxLongProp\":300," +
                "\"MaxNullableSingleProp\":81.94,\"MaxSingleProp\":107.66}}," +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Odd\",\"RowCategory\":\"Prime\"," +
                "\"MaxNullableIntProp\":151,\"MaxIntProp\":141," +
                "\"MaxNullableDoubleProp\":174.12,\"MaxDoubleProp\":132.97," +
                "\"MaxNullableDecimalProp\":157.3,\"MaxDecimalProp\":72.8," +
                "\"MaxNullableLongProp\":248,\"MaxLongProp\":297," +
                "\"MaxNullableSingleProp\":160.47,\"MaxSingleProp\":2.96}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Count_ByConstant()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(Count,CountDistinct)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null,\"CountDistinct\":3,\"Count\":5}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Count_BySingleProperty()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(RowParity,Count,CountDistinct)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null,\"RowParity\":\"Even\",\"CountDistinct\":2,\"Count\":2}}," +
                "{{\"@odata.id\":null,\"RowParity\":\"Odd\",\"CountDistinct\":2,\"Count\":3}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_Count_ByMultipleProperties()
        {
            string mockResponse = string.Format("{{" +
                "\"@odata.context\":\"{0}/$metadata#Numbers(RowParity,RowCategory,Count,CountDistinct)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null,\"RowCategory\":\"Composite\",\"RowParity\":\"Even\",\"CountDistinct\":1,\"Count\":1}}," +
                "{{\"@odata.id\":null,\"RowCategory\":\"None\",\"RowParity\":\"Odd\",\"CountDistinct\":1,\"Count\":1}}," +
                "{{\"@odata.id\":null,\"RowCategory\":\"Prime\",\"RowParity\":\"Odd\",\"CountDistinct\":1,\"Count\":2}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_WithMixedAggregations()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "RowParity,SumIntProp,AverageDoubleProp,MinDecimalProp,MaxLongProp,Count,CountDistinct)\"," +
                "\"value\":[" +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Even\"," +
                "\"Count\":2,\"CountDistinct\":2," +
                "\"SumIntProp\":132,\"AverageDoubleProp\":23.265," +
                "\"MinDecimalProp\":168.4,\"MaxLongProp\":259}}," +
                "{{\"@odata.id\":null," +
                "\"RowParity\":\"Odd\"," +
                "\"Count\":3,\"CountDistinct\":2," +
                "\"SumIntProp\":374,\"AverageDoubleProp\":139.4," +
                "\"MinDecimalProp\":42.7,\"MaxLongProp\":300}}" +
                "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBy_ConstructorInitialization()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Sales" +
                    "(Product(Category(Name)),averageAmount)\"," +
                    "\"value\":[" +
                    "{{\"@odata.id\":null,\"averageAmount\":4.000000," +
                    "\"Product\":{{\"@odata.id\":null,\"Category\":{{\"@odata.id\":null,\"Name\":\"Food\"}}}}}}" +
                    "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockCountDistinct()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "CountDistinctRowParity)\"," +
                "\"value\":[{{\"@odata.id\":null,\"CountDistinctRowParity\":3}}]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockCountDistinct_TargetingNavProperty()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Numbers(" +
                "CountDistinctCustomerCountry)\"," +
                "\"value\":[{{\"@odata.id\":null,\"CountDistinctCustomerCountry\":2}}]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBySingleNavProperty()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Sales" +
                    "(Product(Color),SumAmount,AvgAmount,MinAmount,MaxAmount)\"," +
                    "\"value\":[" +
                    "{{\"@odata.id\":null,\"MaxAmount\":8.00,\"MinAmount\":4.00,\"AvgAmount\":6.000000,\"SumAmount\":12.00," +
                    "\"Product\":{{\"@odata.id\":null,\"Color\":\"Brown\"}}}}," +
                    "{{\"@odata.id\":null,\"MaxAmount\":4.00,\"MinAmount\":1.00,\"AvgAmount\":2.000000,\"SumAmount\":12.00," +
                    "\"Product\":{{\"@odata.id\":null,\"Color\":\"White\"}}}}" +
                    "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupByMultipleNavProperties()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Sales" +
                    "(Product(Color),Customer(Country),SumAmount,AvgAmount,MinAmount,MaxAmount)\"," +
                    "\"value\":[" +
                    "{{\"@odata.id\":null,\"MaxAmount\":8.00,\"MinAmount\":4.00,\"AvgAmount\":6.000000,\"SumAmount\":12.00," +
                    "\"Customer\":{{\"@odata.id\":null,\"Country\":\"USA\"}}," +
                    "\"Product\":{{\"@odata.id\":null,\"Color\":\"Brown\"}}}}," +
                    "{{\"@odata.id\":null,\"MaxAmount\":2.00,\"MinAmount\":1.00,\"AvgAmount\":1.666666,\"SumAmount\":5.00," +
                    "\"Customer\":{{\"@odata.id\":null,\"Country\":\"Netherlands\"}}," +
                    "\"Product\":{{\"@odata.id\":null,\"Color\":\"White\"}}}}" +
                    "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBySingleNavProperty_AggregationsTargetingNavProperty()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Sales" +
                    "(Currency(Code),SumTaxRate,AvgTaxRate,MinTaxRate,MaxTaxRate)\"," +
                    "\"value\":[" +
                    "{{\"@odata.id\":null,\"MaxTaxRate\":0.14,\"MinTaxRate\":0.06,\"AvgTaxRate\":0.113333,\"SumTaxRate\":0.34," +
                    "\"Currency\":{{\"@odata.id\":null,\"Code\":\"EUR\"}}}}," +
                    "{{\"@odata.id\":null,\"MaxTaxRate\":0.14,\"MinTaxRate\":0.06,\"AvgTaxRate\":0.092000,\"SumTaxRate\":0.46," +
                    "\"Currency\":{{\"@odata.id\":null,\"Code\":\"USD\"}}}}" +
                    "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupByMultipleNavProperties_AggregationsTargetingNavProperty()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Sales" +
                    "(Product(Color),Customer(Country),SumTaxRate,AvgTaxRate,MinTaxRate,MaxTaxRate)\"," +
                    "\"value\":[" +
                    "{{\"@odata.id\":null,\"MaxTaxRate\":0.14,\"MinTaxRate\":0.06,\"AvgTaxRate\":0.113333,\"SumTaxRate\":0.34," +
                    "\"Customer\":{{\"@odata.id\":null,\"Country\":\"Netherlands\"}}," +
                    "\"Product\":{{\"@odata.id\":null,\"Color\":\"White\"}}}}," +
                    "{{\"@odata.id\":null,\"MaxTaxRate\":0.06,\"MinTaxRate\":0.06,\"AvgTaxRate\":0.060000,\"SumTaxRate\":0.12," +
                    "\"Customer\":{{\"@odata.id\":null,\"Country\":\"USA\"}}," +
                    "\"Product\":{{\"@odata.id\":null,\"Color\":\"Brown\"}}}}" +
                    "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupByConstant_AggregationsTargetingNavProperty()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Sales" +
                    "(SumTaxRate,AvgTaxRate,MinTaxRate,MaxTaxRate)\"," +
                    "\"value\":[" +
                    "{{\"@odata.id\":null,\"MaxTaxRate\":0.14,\"MinTaxRate\":0.06,\"AvgTaxRate\":0.100000,\"SumTaxRate\":0.80}}" +
                    "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupByConstant_MixedScenarios()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Sales" +
                    "(SumTaxRate,AvgAmount,MinTaxRate,MaxAmount,GroupCount,DistinctCurrency)\"," +
                    "\"value\":[" +
                    "{{\"@odata.id\":null,\"DistinctCurrency\":2,\"GroupCount\":8," +
                    "\"MaxAmount\":8.00,\"MinTaxRate\":0.06,\"AvgAmount\":3.000000,\"SumTaxRate\":0.80}}" +
                    "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupBySingleNavProperty_MixedScenarios()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Sales" +
                    "(Product(Category(Id)),SumTaxRate,AvgAmount,MinTaxRate,MaxAmount,GroupCount,DistinctCurrency)\"," +
                    "\"value\":[" +
                    "{{\"@odata.id\":null,\"DistinctCurrency\":2,\"GroupCount\":4," +
                    "\"MaxAmount\":8.00,\"MinTaxRate\":0.06,\"AvgAmount\":4.000000,\"SumTaxRate\":0.24," +
                    "\"Product\":{{\"@odata.id\":null,\"Category\":{{\"@odata.id\":null,\"Id\":\"PG1\"}}}}}}," +
                    "{{\"@odata.id\":null,\"DistinctCurrency\":2,\"GroupCount\":4," +
                    "\"MaxAmount\":4.00,\"MinTaxRate\":0.14,\"AvgAmount\":2.000000,\"SumTaxRate\":0.56," +
                    "\"Product\":{{\"@odata.id\":null,\"Category\":{{\"@odata.id\":null,\"Id\":\"PG2\"}}}}}}" +
                    "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        private void MockGroupByMultipleNavProperties_MixedScenarios()
        {
            string mockResponse = string.Format("{{\"@odata.context\":\"{0}/$metadata#Sales" +
                    "(Product(Category(Id)),Customer(Country),SumTaxRate,AvgAmount,MinTaxRate,MaxAmount,GroupCount,DistinctCurrency)\"," +
                    "\"value\":[" +
                    "{{\"@odata.id\":null,\"DistinctCurrency\":1,\"GroupCount\":1," +
                    "\"MaxAmount\":2.00,\"MinTaxRate\":0.06,\"AvgAmount\":2.000000,\"SumTaxRate\":0.06," +
                    "\"Customer\":{{\"@odata.id\":null,\"Country\":\"Netherlands\"}}," +
                    "\"Product\":{{\"@odata.id\":null,\"Category\":{{\"@odata.id\":null,\"Id\":\"PG1\"}}}}}}," +
                    "{{\"@odata.id\":null,\"DistinctCurrency\":1,\"GroupCount\":2," +
                    "\"MaxAmount\":2.00,\"MinTaxRate\":0.14,\"AvgAmount\":1.500000,\"SumTaxRate\":0.28," +
                    "\"Customer\":{{\"@odata.id\":null,\"Country\":\"Netherlands\"}}," +
                    "\"Product\":{{\"@odata.id\":null,\"Category\":{{\"@odata.id\":null,\"Id\":\"PG2\"}}}}}}" +
                    "]}}", serviceUri);

            InterceptRequestAndMockResponse(mockResponse);
        }

        #endregion Mock Aggregation Responses

        #region Helper Classes

        class NumbersGroupedResult
        {
            public string RowParity { get; set; }
            public int SumIntProp { get; set; }
            public double AverageDoubleProp { get; set; }
            public decimal MinDecimalProp { get; set; }
            public long MaxLongProp { get; set; }
            public int Count { get; set; }
            public int CountDistinct { get; set; }
        }

        class SalesGroupedResult
        {
            public decimal SumTaxRate { get; set; }
            public decimal AvgAmount { get; set; }
            public decimal MinTaxRate { get; set; }
            public decimal MaxAmount { get; set; }
            public int GroupCount { get; set; }
            public int DistinctCurrency { get; set; }
        }

        class SalesGroupedResult01 : SalesGroupedResult
        {
            public int GroupingConstant { get; set; }
            public string GibberishConstant { get; set; }
        }

        class SalesGroupedResult02 : SalesGroupedResult
        {
            public string GibberishConstant { get; set; }
            public string CategoryId { get; set; }
        }

        class SalesGroupedResult03 : SalesGroupedResult
        {
            public string GibberishConstant { get; set; }
            public string CategoryId { get; set; }
            public string Country { get; set; }
        }

        class SalesGroupedResult04
        {
            public SalesGroupedResult04(string categoryName, decimal averageAmount)
            {
                CategoryName = categoryName;
                AverageAmount = averageAmount;
            }

            public string CategoryName { get; }
            public decimal AverageAmount { get; }
        }

        class SalesGroupedResult05
        {
            public SalesGroupedResult05(decimal averageAmount)
            {
                AverageAmount = averageAmount;
            }

            public string CategoryName { get; set; }
            public decimal AverageAmount { get; }
        }

        class SalesGroupedResult06
        {
            public SalesGroupedResult06(int categoryNameLength, string currencyCode, string averageAmount)
            {
                this.CategoryNameLength = categoryNameLength;
                this.CurrencyCode = currencyCode;
                this.AverageAmount = averageAmount;
            }

            public string TenStr { get; set; }
            public int FoobarLength { get; set; }
            public string YearStr { get; set; }
            public int CategoryNameLength { get; }
            public string CurrencyCode { get; }
            public string AverageAmount { get; }
            public decimal SumAmount { get; set; }
            public string MinAmount { get; set; }
        }

        class SalesGroupedResult07
        {
            public string CategoryName { get; set; }
            public decimal AverageAmount { get; set; }
        }

        class SalesGroupingKey01
        {
            public string Color { get; set; }
            public string Country { get; set; }
        }

        class SalesGroupingKey02
        {
            public SalesGroupingKey02(string color, string country)
            {
                this.Color = color;
                this.Country = country;
            }
            public string Color { get; }
            public string Country { get; }
        }

        #endregion Helper Classes
    }
}
