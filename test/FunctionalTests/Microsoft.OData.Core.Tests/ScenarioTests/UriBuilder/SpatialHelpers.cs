//---------------------------------------------------------------------
// <copyright file="SpatialHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using Microsoft.Spatial;

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriBuilder
{
    internal static class SpatialHelpers
    {
        private static readonly WellKnownTextSqlFormatter WellKnownTextFormatter = WellKnownTextSqlFormatter.Create();

        internal static string WriteSpatial<T>(T spatialValue) where T : ISpatial
        {
            using (var writer = new StringWriter())
            {
                WellKnownTextFormatter.Write(spatialValue, writer);
                return writer.ToString();
            }
        }
    }
}