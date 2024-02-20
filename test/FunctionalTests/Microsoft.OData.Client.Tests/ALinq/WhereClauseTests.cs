//---------------------------------------------------------------------
// <copyright file="DollarFilterWithCastTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Tests.ALinq
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using Microsoft.OData.Client.Tests.Tracking;
    using Microsoft.OData.Edm.Csdl;
    using Xunit;

    /// <summary>
    /// Dollar Filter with cast tests
    /// </summary>
    public class WhereClauseTests : DollarApplyTestsBase
    {
        private const string Response = @"{""@odata.context"":""http://localhost:8000/$metadata#Products"",""value"":[{""Id"":1,""Name"":""Hat""},{""Id"":2,""Name"":""Socks""},{""Id"":3,""Name"":""Scarf""}]}";

        public WhereClauseTests() : base()
        {
        }

        [Fact]
        public void WhereClause_WithKeyFieldOnly()
        {
            var queryable = this.dsContext.CreateQuery<Product>("Products");

            var query = queryable.Where(p => p.Id == "p1");
            Assert.Equal(
                $"{serviceUri}/Products?$filter=Id eq 'p1'",
                query.ToString());
        }

        [Fact]
        public void WhereClause_WithKeyAndNonKeyFields()
        {
            var products = this.dsContext.CreateQuery<Product>("Products");

            var query = products.Where(p => p.Id == "p1" && p.Color == "Red");
            Assert.Equal(
                $"{serviceUri}/Products?$filter=Id eq 'p1' and Color eq 'Red'",
                query.ToString()
            );

        }


        [Fact]
        public void WhereClause_WithNonKeyFieldsOnly()
        {
            var products = this.dsContext.CreateQuery<Product>("Products");

            var query = products.Where(p => p.TaxRate < 0.2m || p.Color == "Red");
            Assert.Equal(
                $"{serviceUri}/Products?$filter=TaxRate lt 0.2 or Color eq 'Red'",
                query.ToString()
            );
        }
    }
}
