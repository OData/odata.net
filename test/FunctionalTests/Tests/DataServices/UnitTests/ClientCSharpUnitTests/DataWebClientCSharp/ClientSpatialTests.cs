//---------------------------------------------------------------------
// <copyright file="ClientSpatialTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Client;
using System.Linq;
using Microsoft.Spatial;
using AstoriaUnitTests.Stubs;
using AstoriaUnitTests.Stubs.DataServiceProvider;
using AstoriaUnitTests.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AstoriaUnitTests.DataWebClientCSharp
{
    [TestClass]
    public class ClientSpatialTests
    {
        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/881
        [Ignore] // Remove Atom
        // [TestMethod]
        public void TestCollectionOfSpatialTypes()
        {
            DSPUnitTestServiceDefinition roadTripServiceDefinition = GetRoadTripServiceDefinition(typeof(GeographyPoint), TestPoint.DefaultValues, false, false,
                (m) =>
                {
                    var resourceType = m.GetResourceType("TripLeg");
                    var primitiveType = Microsoft.OData.Service.Providers.ResourceType.GetPrimitiveResourceType(typeof(GeographyPoint));
                    m.AddCollectionProperty(resourceType, "PointsOfInterest", primitiveType);
                },
                (name, values) =>
                {
                    if (name == "TripLeg")
                    {
                        var list = values.Select(kvp => kvp.Value).ToList();
                        values.Add(new KeyValuePair<string, object>("PointsOfInterest", list));
                    }
                });

            using (TestWebRequest request = roadTripServiceDefinition.CreateForInProcessWcf())
            {
                request.StartService();

                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //context.EnableAtom = true;
                //context.Format.UseAtom();
                var tripLegs = context.CreateQuery<TripLegWithCollection<GeographyPoint>>("TripLegs").Where(t => t.ID == SpatialTestUtil.DefaultId).ToList();
                var tripLeg = tripLegs.Single();
                Assert.AreEqual(2, tripLeg.PointsOfInterest.Count(), "didn't materialize all the elements");
            }
        }

        internal static DSPUnitTestServiceDefinition GetRoadTripServiceDefinition(Type geographyType, GeographyPropertyValues defaultValues, bool useComplexType = false, bool useOpenTypes = false, Action<DSPMetadata> modifyMetadata = null, Action<string, List<KeyValuePair<string, object>>> modifyPropertyValues = null)
        {
            DSPMetadata roadTripMetadata = SpatialTestUtil.CreateRoadTripMetadata(geographyType, useComplexType, useOpenTypes, modifyMetadata);
            return SpatialTestUtil.CreateRoadTripServiceDefinition(roadTripMetadata, defaultValues, DSPDataProviderKind.CustomProvider, useComplexType, modifyPropertyValues);
        }

        private class TripLegWithCollection<T> where T : Geography
        {
            public int ID { get; set; }
            public T GeographyProperty1 { get; set; }
            public T GeographyProperty2 { get; set; }
            public ICollection<T> PointsOfInterest { get; set; }
        }
    }
}
