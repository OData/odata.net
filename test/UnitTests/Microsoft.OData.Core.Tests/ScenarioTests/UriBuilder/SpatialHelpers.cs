//---------------------------------------------------------------------
// <copyright file="SpatialHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Microsoft.OData.Tests.ScenarioTests.UriBuilder
{
    internal static class SpatialHelpers
    {
        internal static string WriteSpatial<T>(T spatialValue)
        {
            using (var writer = new StringWriter())
            {
                Geometry geometry;
                if (spatialValue is Microsoft.OData.Spatial.Geography geographyValue)
                {
                    geometry = geographyValue;
                }
                else if (spatialValue is Microsoft.OData.Spatial.Geometry geometryValue)
                {
                    geometry = geometryValue;
                }
                else
                {
                    throw new InvalidOperationException("Unsupported spatial type.");
                }

                new WKTWriter().Write(geometry, writer);
                return writer.ToString();
            }
        }
    }
}