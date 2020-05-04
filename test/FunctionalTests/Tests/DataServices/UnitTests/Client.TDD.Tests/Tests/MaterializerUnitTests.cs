//---------------------------------------------------------------------
// <copyright file="MaterializerUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Client;
using Util = Microsoft.OData.Client.Util;
using Xunit;

namespace AstoriaUnitTests.Tests
{
    public class MaterializerUnitTests
    {
        [Fact]
        public void Continuation_Creates_Incorrect_DataServiceVersion()
        {
            // Regression test for:Client always sends DSV=2.0 when following a continuation token
            ProjectionPlan plan = new ProjectionPlan()
            {
                LastSegmentType = typeof(int),
                ProjectedType = typeof(int)
            };

            var continuationToken = DataServiceQueryContinuation.Create(new Uri("http://localhost/Set?$skiptoken='Me'"), plan);
            QueryComponents queryComponents = continuationToken.CreateQueryComponents();

            //OData-Version of  query components should be empty for Continuation token
            Assert.Same(queryComponents.Version, Util.ODataVersionEmpty);
        }
    }
}
