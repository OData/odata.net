//---------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
namespace Microsoft.OData.Tests.Nuget.Spatial
{
    using System;
    using System.Diagnostics;
    using Microsoft.Spatial;
    using Xunit;

    public class SpatialLibTest
    {
        [Fact]
        public void BasicTest()
        {
            GeometryPoint point1 = GeometryPoint.Create(1, 1);

            double expectedValue = 1.0;
            Assert.True(expectedValue.Equals(point1.X));
            Assert.True(expectedValue.Equals(point1.Y));
            Console.WriteLine(point1.CoordinateSystem);
        }
    }
}
