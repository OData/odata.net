//---------------------------------------------------------------------
// <copyright file="MaterializerUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Client;
using Microsoft.Spatial;
using System.Xml.Linq;
using AstoriaUnitTests.TDD.Common;
using DataSpatialUnitTests.Utils;
using Microsoft.OData;
using Microsoft.Spatial.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Util = Microsoft.OData.Client.Util;

namespace AstoriaUnitTests.Tests
{
    [TestClass]
    public class MaterializerUnitTests
    {
        [TestMethod]
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
            Assert.AreSame(queryComponents.Version, Util.ODataVersionEmpty, "OData-Version of  query components should be empty for Continuation token");
        }
    }
}
