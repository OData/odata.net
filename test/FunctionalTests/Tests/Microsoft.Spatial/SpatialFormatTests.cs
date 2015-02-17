//---------------------------------------------------------------------
// <copyright file="SpatialFormatTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataSpatialUnitTests.Tests
{
    using System;
    using DataSpatialUnitTests.Utils;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SpatialReaderTests
    {
        [TestMethod]
        public void ErrorOnNullDestinationInCtor()
        {
            Action act = () => new TrivialReader(null);
            SpatialTestUtils.VerifyExceptionThrown<ArgumentNullException>(act, "Value cannot be null.\r\nParameter name: destination");
        }

        [TestMethod]
        public void ErrorOnNullInputToReadMethods()
        {
            var reader = new TrivialReader(new CallSequenceLoggingPipeline());
            Action[] acts = {() => reader.ReadGeography(null), () => reader.ReadGeometry(null)};

            foreach(var act in acts)
            {
                SpatialTestUtils.VerifyExceptionThrown<ArgumentNullException>(act, "Value cannot be null.\r\nParameter name: input");
            }
        }
    }
}
