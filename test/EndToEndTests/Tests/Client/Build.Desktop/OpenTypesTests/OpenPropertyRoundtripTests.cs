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
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class OpenPropertyRoundtripTests : EndToEndTestBase
    {
        public OpenPropertyRoundtripTests()
            : base(ServiceDescriptors.OpenTypesService)
        {
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void InsertRoundTripOpenComplexProperty()
        {
            this.RunOnAtomAndJsonFormats(
                this.CreateContext,
                (contextWrapper) =>
                    {
                        var newRow = new Row
                            {
                                Id = Guid.NewGuid(),
                                OpenComplex = new ContactDetails
                                    {
                                        Byte = byte.MaxValue,
                                        Short = short.MaxValue,
                                        LastContacted = DateTimeOffset.Now,
                                        Contacted = DateTimeOffset.Now,
                                        GUID = Guid.NewGuid(),
                                        PreferedContactTime = TimeSpan.MaxValue,
                                        SignedByte = sbyte.MaxValue,
                                        Double = double.MaxValue,
                                        Single = Single.PositiveInfinity,
                                        Int = 0,
                                        Long = long.MinValue,
                                    },
                            };

                        this.InsertNewRowRoundtrip(contextWrapper, newRow);
                    });
        }

        [TestMethod]
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
                                    row.OpenComplex = new ContactDetails
                                        {
                                            Double = double.NaN,
                                            GUID = Guid.NewGuid(),
                                            PreferedContactTime = TimeSpan.MinValue,
                                        };
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
                                    row.OpenComplex = null;
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

        private static void AssertAreEqual(Row expected, Row actual)
        {
            Assert.AreEqual(expected.Id, actual.Id, "Row Id values don't match");
            Assert.AreEqual(expected.OpenBoolean, actual.OpenBoolean, "Row OpenBoolean values don't match");
            Assert.AreEqual(expected.OpenDateTimeOffset, actual.OpenDateTimeOffset, "Row OpenDateTimeOffset values don't match");
            Assert.AreEqual(expected.OpenDecimal, actual.OpenDecimal, "Row OpenDecimal values don't match");
            Assert.AreEqual(expected.OpenFloat, actual.OpenFloat, "Row OpenFloat values don't match");
            Assert.AreEqual(expected.OpenGuid, actual.OpenGuid, "Row OpenGuid values don't match");
            Assert.AreEqual(expected.OpenInt16, actual.OpenInt16, "Row OpenInt16 values don't match");
            Assert.AreEqual(expected.OpenInt64, actual.OpenInt64, "Row OpenInt64 values don't match");
            Assert.AreEqual(expected.OpenString, actual.OpenString, "Row OpenString values don't match");
            Assert.AreEqual(expected.OpenTime, actual.OpenTime, "Row OpenTime values don't match");
            Assert.AreEqual(expected.OpenComplex, actual.OpenComplex, "Row OpenComplex values don't match");

            if (expected.OpenComplex != null)
            {
                Assert.AreEqual(expected.OpenComplex.Byte, actual.OpenComplex.Byte, "Row OpenComplex.Byte values don't match");
                Assert.AreEqual(expected.OpenComplex.Short, actual.OpenComplex.Short, "Row OpenComplex.Short values don't match");
                Assert.AreEqual(expected.OpenComplex.LastContacted, actual.OpenComplex.LastContacted, "Row OpenComplex.LastContacted values don't match");
                Assert.AreEqual(expected.OpenComplex.Contacted, actual.OpenComplex.Contacted, "Row OpenComplex.Contacted values don't match");
                Assert.AreEqual(expected.OpenComplex.GUID, actual.OpenComplex.GUID, "Row OpenComplex.GUID values don't match");
                Assert.AreEqual(expected.OpenComplex.PreferedContactTime, actual.OpenComplex.PreferedContactTime, "Row OpenComplex.PreferedContactTime values don't match");
                Assert.AreEqual(expected.OpenComplex.SignedByte, actual.OpenComplex.SignedByte, "Row OpenComplex.SignedByte values don't match");
                Assert.AreEqual(expected.OpenComplex.Double, actual.OpenComplex.Double, "Row OpenComplex.Double values don't match");
                Assert.AreEqual(expected.OpenComplex.Single, actual.OpenComplex.Single, "Row OpenComplex.Single values don't match");
                Assert.AreEqual(expected.OpenComplex.Int, actual.OpenComplex.Int, "Row OpenComplex.Int values don't match");
                Assert.AreEqual(expected.OpenComplex.Long, actual.OpenComplex.Long, "Row OpenComplex.Long values don't match");
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
            Assert.IsNotNull(retrievedRow, "Failed to retrieve new row");
            AssertAreEqual(newRow, retrievedRow);
        }

        private void UpdateRowRoundtrip(DataServiceContextWrapper<DefaultContainer> contextWrapper, Guid rowId, Action<Row> updateRow)
        {
            Row testRow = contextWrapper.Context.Row.Where(r => r.Id == rowId).Single();
            updateRow(testRow);
            contextWrapper.UpdateObject(testRow);
            contextWrapper.SaveChanges();

            var retrievedRow = contextWrapper.CreateQuery<Row>("Row").Where(r => r.Id == rowId).SingleOrDefault();
            Assert.IsNotNull(retrievedRow, "Failed to retrieve updated row");
            AssertAreEqual(testRow, retrievedRow);
        }
    }
}
