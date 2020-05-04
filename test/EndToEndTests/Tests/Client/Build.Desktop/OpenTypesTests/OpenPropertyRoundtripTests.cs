//---------------------------------------------------------------------
// <copyright file="OpenPropertyRoundtripTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.OpenTypesTests
{
    using System;
    using System.Linq;
    using Microsoft.Test.OData.Framework;
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.OpenTypesServiceReference;
    using Xunit;
    using Xunit.Abstractions;

    public class OpenPropertyRoundtripTests : EndToEndTestBase
    {
        public OpenPropertyRoundtripTests(ITestOutputHelper helper)
            : base(ServiceDescriptors.OpenTypesService, helper)
        {
        }

        [Fact(Skip="VSUpgrade19 - DataDriven Test")]
        public void InsertRoundtripPrimitiveProperties()
        {
            this.RunOnAtomAndJsonFormats(
                this.CreateContext,
                (contextWrapper) =>
                    {
                        var newRow = new Row
                            {
                                Id = Guid.NewGuid(),
                                OpenBoolean = true,
                                OpenDateTimeOffset = DateTimeOffset.Now,
                                OpenDecimal = decimal.MinusOne,
                                OpenFloat = float.PositiveInfinity,
                                OpenGuid = Guid.NewGuid(),
                                OpenInt16 = Int16.MaxValue,
                                OpenInt64 = Int64.MaxValue,
                                OpenString = "hello world",
                                OpenTime = TimeSpan.MaxValue,
                            };

                        this.InsertNewRowRoundtrip(contextWrapper, newRow);
                    });
        }

        [Fact(Skip= "VSUpgrade19 - DataDriven Test")]
        public void InsertRoundtripPropertiesWithNullValues()
        {
            this.RunOnAtomAndJsonFormats(
                this.CreateContext,
                (contextWrapper) =>
                    {
                        var newRow = new Row
                            {
                                Id = Guid.NewGuid(),
                                OpenBoolean = null,
                                OpenDateTimeOffset = null,
                                OpenDecimal = null,
                                OpenFloat = null,
                                OpenGuid = null,
                                OpenInt16 = null,
                                OpenInt64 = null,
                                OpenString = null,
                                OpenTime = null,
                                OpenComplex = null,
                            };

                        this.InsertNewRowRoundtrip(contextWrapper, newRow);
                    });
        }

        [Fact(Skip= "VSUpgrade19 - DataDriven Test")]
        public void UpdateRoundTrip()
        {
            this.RunOnAtomAndJsonFormats(
                this.CreateContext,
                (contextWrapper) =>
                    {
                        Guid testRowId = contextWrapper.Context.Row.Take(1).Single().Id;

                        // Flip half of the properties to null
                        this.UpdateRowRoundtrip(
                            contextWrapper,
                            testRowId,
                            (row) =>
                                {
                                    row.OpenBoolean = null;
                                    row.OpenDateTimeOffset = DateTimeOffset.Now;
                                    row.OpenDecimal = null;
                                    row.OpenFloat = float.NegativeInfinity;
                                    row.OpenGuid = null;
                                    row.OpenInt16 = Int16.MinValue;
                                    row.OpenInt64 = null;
                                    row.OpenString = string.Empty;
                                    row.OpenTime = null;
                                });

                        // Now flip the nulls to values, and vice versa
                        this.UpdateRowRoundtrip(
                            contextWrapper,
                            testRowId,
                            (row) =>
                                {
                                    row.OpenBoolean = false;
                                    row.OpenDateTimeOffset = null;
                                    row.OpenDecimal = decimal.MinusOne;
                                    row.OpenFloat = null;
                                    row.OpenGuid = Guid.NewGuid();
                                    row.OpenInt16 = null;
                                    row.OpenInt64 = Int64.MinValue;
                                    row.OpenString = null;
                                    row.OpenTime = TimeSpan.Zero;
                                });
                    });
        }

        private static void AssertEqual(Row expected, Row actual)
        {
            //Row Id values don't match
            Assert.Equal(expected.Id, actual.Id);
            //Row OpenBoolean values don't match
            Assert.Equal(expected.OpenBoolean, actual.OpenBoolean);
            //Row OpenDateTimeOffset values don't match
            Assert.Equal(expected.OpenDateTimeOffset, actual.OpenDateTimeOffset);
            //Row OpenDecimal values don't match
            Assert.Equal(expected.OpenDecimal, actual.OpenDecimal);
            //Row OpenFloat values don't match
            Assert.Equal(expected.OpenFloat, actual.OpenFloat);
            //Row OpenGuid values don't match
            Assert.Equal(expected.OpenGuid, actual.OpenGuid);
            //Row OpenInt16 values don't match
            Assert.Equal(expected.OpenInt16, actual.OpenInt16);
            //Row OpenInt64 values don't match
            Assert.Equal(expected.OpenInt64, actual.OpenInt64);
            //Row OpenString values don't match
            Assert.Equal(expected.OpenString, actual.OpenString);
            //Row OpenTime values don't match
            Assert.Equal(expected.OpenTime, actual.OpenTime);
            //Row OpenComplex values don't match
            Assert.Equal(expected.OpenComplex, actual.OpenComplex);

            if (expected.OpenComplex != null)
            {
                //Row OpenComplex.Byte values don't match
                Assert.Equal(expected.OpenComplex.Byte, actual.OpenComplex.Byte);
                //Row OpenComplex.Short values don't match
                Assert.Equal(expected.OpenComplex.Short, actual.OpenComplex.Short);
                //Row OpenComplex.LastContacted values don't match
                Assert.Equal(expected.OpenComplex.LastContacted, actual.OpenComplex.LastContacted);
                //Row OpenComplex.Contacted values don't match
                Assert.Equal(expected.OpenComplex.Contacted, actual.OpenComplex.Contacted);
                //Row OpenComplex.GUID values don't match
                Assert.Equal(expected.OpenComplex.GUID, actual.OpenComplex.GUID);
                //Row OpenComplex.PreferedContactTime values don't match
                Assert.Equal(expected.OpenComplex.PreferedContactTime, actual.OpenComplex.PreferedContactTime);
                //Row OpenComplex.SignedByte values don't match
                Assert.Equal(expected.OpenComplex.SignedByte, actual.OpenComplex.SignedByte);
                //Row OpenComplex.Double values don't match
                Assert.Equal(expected.OpenComplex.Double, actual.OpenComplex.Double);
                //Row OpenComplex.Single values don't match
                Assert.Equal(expected.OpenComplex.Single, actual.OpenComplex.Single);
                //Row OpenComplex.Int values don't match
                Assert.Equal(expected.OpenComplex.Int, actual.OpenComplex.Int);
                //Row OpenComplex.Long values don't match
                Assert.Equal(expected.OpenComplex.Long, actual.OpenComplex.Long);
            }
        }

        private DataServiceContextWrapper<DefaultContainer> CreateContext()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();
            ///context.UndeclaredPropertyBehavior = Microsoft.OData.Client.UndeclaredPropertyBehavior.Support;
            return context;
        }

        private void InsertNewRowRoundtrip(DataServiceContextWrapper<DefaultContainer> contextWrapper, Row newRow)
        {
            Guid newRowId = newRow.Id;
            contextWrapper.Context.AddToRow(newRow);
            contextWrapper.SaveChanges();

            var retrievedRow = contextWrapper.CreateQuery<Row>("Row").Where(r => r.Id == newRowId).SingleOrDefault();
            //Failed to retrieve new row
            Assert.NotNull(retrievedRow);
            AssertEqual(newRow, retrievedRow);
        }

        private void UpdateRowRoundtrip(DataServiceContextWrapper<DefaultContainer> contextWrapper, Guid rowId, Action<Row> updateRow)
        {
            Row testRow = contextWrapper.Context.Row.Where(r => r.Id == rowId).Single();
            updateRow(testRow);
            contextWrapper.UpdateObject(testRow);
            contextWrapper.SaveChanges();

            var retrievedRow = contextWrapper.CreateQuery<Row>("Row").Where(r => r.Id == rowId).SingleOrDefault();
            //Failed to retrieve updated row
            Assert.NotNull(retrievedRow);
            AssertEqual(testRow, retrievedRow);
        }
    }
}
