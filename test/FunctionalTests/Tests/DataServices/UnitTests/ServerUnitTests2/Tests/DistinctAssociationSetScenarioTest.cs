//---------------------------------------------------------------------
// <copyright file="DistinctAssociationSetScenarioTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Tests.Util;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests to ensure that service authors can overwrite header values in our public hooks and the changes are persisted.
    /// </summary>
    [TestClass]
    public class DistinctAssociationSetScenarioTest
    {
        [TestCategory("Partition2"), TestMethod]
        public void MultipleAssociationsToTheSameEntitySetResultInDistinctAssociationSetNames()
        {
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.ServiceType = typeof(DistinctAssociationSetService);
                request.FullRequestUriString = "http://host/$metadata";
                request.HttpMethod = "GET";
                request.SendRequest();
                string response = new StreamReader(request.GetResponseStream()).ReadToEnd();
                //count the number of occurrances of ReferenceProductMetatdatas and VariableProductMetadatas, they should be equal.
                Regex referenceCountRegex = new Regex("ReferenceProduct_Metadatas");
                Regex variableCountRegex = new Regex("VariableProduct_Metadatas");

                referenceCountRegex.Matches(response).Count.Should().Be(variableCountRegex.Matches(response).Count);
            }
        }
    }
}
