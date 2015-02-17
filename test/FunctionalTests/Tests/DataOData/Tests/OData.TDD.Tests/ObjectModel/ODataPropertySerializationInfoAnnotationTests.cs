//---------------------------------------------------------------------
// <copyright file="ODataPropertySerializationInfoAnnotationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.ObjecetModel
{
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class ODataPropertySerializationInfoAnnotationTests
    {
        [TestMethod]
        public void PropertyKindShouldBeUnspecifiedOnCreation()
        {
            var serializationInfo = new ODataPropertySerializationInfo();
            serializationInfo.PropertyKind.Should().Be(ODataPropertyKind.Unspecified);
        }
    }
}
