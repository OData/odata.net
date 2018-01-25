//---------------------------------------------------------------------
// <copyright file="OpenTypesWithoutTypeResolverTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.ClientWithoutTypeResolverTests
{
    using System;
    using Microsoft.OData.Client;
    using System.Linq;
    using Microsoft.Test.OData.Framework;
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.OpenTypesServiceReference;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class OpenTypesWithoutTypeResolverTests : EndToEndTestBase
    {
        #region Test Data
        private Row row1 = new Row
        {
            Id = Guid.NewGuid(),
            OpenBoolean = true,
            OpenDateTimeOffset = DateTimeOffset.Now,
            OpenDecimal = decimal.MinusOne,
            OpenDouble = double.NaN,
            OpenFloat = float.PositiveInfinity,
            OpenGuid = Guid.NewGuid(),
            OpenInt16 = Int16.MaxValue,
            OpenInt64 = Int64.MaxValue,
            OpenString = "hello world",
            OpenTime = TimeSpan.MaxValue,
        };

        private Row row2 = new Row
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
        };

        private IndexedRow row3 = new IndexedRow
        {
            Id = Guid.NewGuid(),
            OpenDouble = double.NaN,
        };
        #endregion

        private const int TestRowIndexId = 999;
        private Func<Type, string> typeNameResolver;

        public OpenTypesWithoutTypeResolverTests()
            : base(ServiceDescriptors.OpenTypesService)
        {
        }

        public override void CustomTestInitialize()
        {
            var contextWrapper = this.CreateContext();

            // Restore the type name resolver since we will be writing open complex properties.
            contextWrapper.ResolveName = this.typeNameResolver;

            contextWrapper.AddObject("Row", row1);
            contextWrapper.AddObject("Row", row2);

            var rowIndex = new RowIndex
            {
                Id = TestRowIndexId, 
                Rows = new DataServiceCollection<IndexedRow>(contextWrapper.Context), 
                IndexComments = "This is a test",
            };

            rowIndex.Rows.Add(row3);
            contextWrapper.AddObject("RowIndex", rowIndex);

            contextWrapper.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [TestMethod]
        public void ExpandQuery()
        {
            var contextWrapper = this.CreateContext();

            var query = contextWrapper.CreateQuery<RowIndex>("RowIndex").Expand(i => i.Rows);
            var results = query.Execute();
        }
#endif

        [TestMethod]
        public void ProjectionQuery()
        {
            var contextWrapper = this.CreateContext();

            var query = contextWrapper.CreateQuery<Row>("Row").Select(r => new {r.Id, r.OpenDouble});
            var results = query.ToList();
        }

        [TestMethod]
        public void DerivedTypesQuery()
        {
            var contextWrapper = this.CreateContext();

            var query = contextWrapper.CreateQuery<IndexedRow>("Row").OfType<IndexedRow>();
            var results = query.ToArray();
        }

        [TestMethod]
        public void BaseTypeQueryRealizesDerivedTypeObject()
        {
            var contextWrapper = this.CreateContext();
                
            var derivedQuery = contextWrapper.CreateQuery<IndexedRow>("Row").OfType<IndexedRow>().Take(1);
            var indexedRow = derivedQuery.Single();

            var baseQuery = contextWrapper.Execute<Row>(new Uri(this.ServiceUri.OriginalString + "/Row(" + indexedRow.Id.ToString() + ")"));
            var row = baseQuery.Single();

            Assert.IsTrue(row is IndexedRow);
        }

        [TestMethod]
        public void UpdateOpenProperties()
        {
            var contextWrapper = this.CreateContext();

            // Restore the type name resolver since we will be writing open complex properties.
            contextWrapper.ResolveName = this.typeNameResolver;

            var updateRow = contextWrapper.Context.Row.Where(r => r.Id == this.row2.Id).Single();

            updateRow.OpenComplex = new ContactDetails
                {
                    Byte = byte.MinValue,
                    Contacted = DateTimeOffset.Now,
                    Double = double.MaxValue,
                    FirstContacted = new byte[] {byte.MaxValue, byte.MinValue, (byte) 0},
                    GUID = Guid.NewGuid(),
                    LastContacted = DateTimeOffset.Now,
                    PreferedContactTime = TimeSpan.FromMilliseconds(1234D),
                    Short = short.MinValue,
                };

            updateRow.OpenBoolean = true;
            updateRow.OpenInt64 = Int64.MaxValue;
            updateRow.OpenDecimal = decimal.Zero;
            updateRow.OpenString = string.Empty;

            contextWrapper.UpdateObject(updateRow);
            contextWrapper.SaveChanges();
        }

        private DataServiceContextWrapper<DefaultContainer> CreateContext()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();
            context.Format.UseJson();
            ///context.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;
            context.ResolveType = null;

            // We need to be able to restore the type name resolver in cases 
            // where we are writing open complex property values, otherwise
            // the server will not be able to resolve them.
            this.typeNameResolver = context.ResolveName;
            context.ResolveName = null;

            return context;
        }
    }
}
