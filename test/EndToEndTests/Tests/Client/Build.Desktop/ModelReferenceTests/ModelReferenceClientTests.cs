//---------------------------------------------------------------------
// <copyright file="ModelReferenceClientTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.ModelReferenceTests
{
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ModelReferenceServiceReference.Microsoft.OData.SampleService.Models.ModelRefDemo.GPS;
    using Microsoft.Test.OData.Services.TestServices.ModelReferenceServiceReference.Microsoft.OData.SampleService.Models.ModelRefDemo.Location;
    using Microsoft.Test.OData.Services.TestServices.ModelReferenceServiceReference.Microsoft.OData.SampleService.Models.ModelRefDemo.Map;
    using Microsoft.Test.OData.Services.TestServices.ModelReferenceServiceReference.Microsoft.OData.SampleService.Models.ModelRefDemo.TruckDemo;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModelReferenceClientTests : ODataWCFServiceTestsBase<TruckDemoService>
    {
        public ModelReferenceClientTests()
            : base(ServiceDescriptors.ModelRefServiceDescriptor)
        {
        }

        // Query set - Create entity - Get created entity - Update entity - Delete entity of set in referenced entity container
        // But type declared in main model
        [TestMethod]
        public void EntitySetDeclaredInReferencedModelE2E()
        {
            // Query Entity Set in GPS
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;
            var vehicleGPSSetInGPS = TestClientContext.VehicleGPSSetInGPS;
            foreach (var vehicleGPS in vehicleGPSSetInGPS)
            {
                Assert.IsTrue(vehicleGPS != null);
            }
            Assert.AreEqual(3, vehicleGPSSetInGPS.Count());

            // Create an entity in VehicleGPSSetInGPS
            var newVehicleGPS = new VehicleGPSType()
            {
                Key = "101",
                VehicleSpeed = 100.1,
                StartLocation = new GeoLocation()
                {
                    Lat = 1,
                    Long = 2,
                },
                EndLocation = new GeoLocation()
                {
                    Lat = 3,
                    Long = 4,
                },
                CurrentLocation = new GeoLocation()
                {
                    Lat = 1.2,
                    Long = 2.4,
                },
                LostSignalAlarm = new GPSLostSignalAlarmType()
                {
                    Severity = 1,
                    LastKnownLocation = new GeoLocation()
                    {
                        Lat = 2.1,
                        Long = 1.2,
                    }
                },
                Map = new MapType()
                {
                    MBytesDownloaded = 1.2,
                    ProviderName = "TESTNEW",
                    Uri = "TESTNEW.TEST",
                }
            };
            TestClientContext.AddToVehicleGPSSetInGPS(newVehicleGPS);
            TestClientContext.SaveChanges();

            // Get the created entity
            var queryable = TestClientContext.VehicleGPSSetInGPS.Where(vehicleGPS => vehicleGPS.Key == "101");
            VehicleGPSType newCreated = queryable.Single();
            Assert.AreEqual(100.1, newCreated.VehicleSpeed);

            // Update the created entity 
            newCreated.VehicleSpeed = 200.1;
            TestClientContext.UpdateObject(newCreated);
            TestClientContext.SaveChanges();

            // Query and Delete entity
            VehicleGPSType updated = queryable.Single();
            Assert.AreEqual(200.1, newCreated.VehicleSpeed);

            TestClientContext.DeleteObject(updated);
            TestClientContext.SaveChanges();
            Assert.AreEqual(3, vehicleGPSSetInGPS.Count());
        }

        // Query set - Create entity - Get created entity - Update entity - Delete entity of set in main entity container
        // But type declared in referenced model
        [TestMethod]
        public void TypeDeclaredInReferencedModelE2E()
        {
            // Query VehicleGPSSet
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;
            var vehicleGPSSet = TestClientContext.VehicleGPSSet;
            foreach (var vehicleGPS in vehicleGPSSet)
            {
                Assert.IsTrue(vehicleGPS != null);
            }
            Assert.AreEqual(3, vehicleGPSSet.Count());

            // Create an entity in VehicleGPSSet
            var newVehicleGPS = new VehicleGPSType()
            {
                Key = "101",
                VehicleSpeed = 100.1,
                StartLocation = new GeoLocation()
                {
                    Lat = 1,
                    Long = 2,
                },
                EndLocation = new GeoLocation()
                {
                    Lat = 3,
                    Long = 4,
                },
                CurrentLocation = new GeoLocation()
                {
                    Lat = 1.2,
                    Long = 2.4,
                },
                LostSignalAlarm = new GPSLostSignalAlarmType()
                {
                    Severity = 1,
                    LastKnownLocation = new GeoLocation()
                    {
                        Lat = 2.1,
                        Long = 1.2,
                    }
                },
                Map = new MapType()
                {
                    MBytesDownloaded = 1.2,
                    ProviderName = "TESTNEW",
                    Uri = "TESTNEW.TEST",
                }
            };
            TestClientContext.AddToVehicleGPSSet(newVehicleGPS);
            TestClientContext.SaveChanges();

            // Get the created entity
            var queryable = TestClientContext.VehicleGPSSet.Where(vehicleGPS => vehicleGPS.Key == "101");
            VehicleGPSType newCreated = queryable.Single();
            Assert.AreEqual(100.1, newCreated.VehicleSpeed);

            // Update the created entity 
            newCreated.VehicleSpeed = 200.1;
            TestClientContext.UpdateObject(newCreated);
            TestClientContext.SaveChanges();

            // Query and Delete entity
            VehicleGPSType updated = queryable.Single();
            Assert.AreEqual(200.1, newCreated.VehicleSpeed);

            TestClientContext.DeleteObject(updated);
            TestClientContext.SaveChanges();
            Assert.AreEqual(3, vehicleGPSSet.Count());
        }

        // Query set - Create entity - Get created entity - Update entity - Delete entity of set in main entity container
        // Type defined in main container, but derived from type referenced model
        [TestMethod]
        public void EntitySetDerivedFromTypeDeclaredInReferencedE2E()
        {
            // Query VehicleGPSSet
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;
            var derivedVehicleGPSSet = TestClientContext.DerivedVehicleGPSSet;
            foreach (var derivedVehicleGPS in derivedVehicleGPSSet)
            {
                Assert.IsTrue(derivedVehicleGPS != null);
            }
            Assert.AreEqual(2, derivedVehicleGPSSet.Count());

            // Create an entity in VehicleGPSSet
            var newVehicleGPS = new DerivedVehicleGPSType()
            {
                Key = "101",
                VehicleSpeed = 100.1,
                StartLocation = new GeoLocation()
                {
                    Lat = 1,
                    Long = 2,
                },
                EndLocation = new GeoLocation()
                {
                    Lat = 3,
                    Long = 4,
                },
                CurrentLocation = new GeoLocation()
                {
                    Lat = 1.2,
                    Long = 2.4,
                },
                LostSignalAlarm = new GPSLostSignalAlarmType()
                {
                    Severity = 1,
                    LastKnownLocation = new GeoLocation()
                    {
                        Lat = 2.1,
                        Long = 1.2,
                    }
                },
                Map = new MapType()
                {
                    MBytesDownloaded = 1.2,
                    ProviderName = "TESTNEW",
                    Uri = "TESTNEW.TEST",
                },
                DisplayName = "TESTTEST"
            };
            TestClientContext.AddToDerivedVehicleGPSSet(newVehicleGPS);
            TestClientContext.SaveChanges();

            // Get the created entity
            var queryable = TestClientContext.DerivedVehicleGPSSet.Where(vehicleGPS => vehicleGPS.Key == "101");
            VehicleGPSType newCreated = queryable.Single();
            Assert.AreEqual(100.1, newCreated.VehicleSpeed);

            // Update the created entity 
            newCreated.VehicleSpeed = 200.1;
            TestClientContext.UpdateObject(newCreated);
            TestClientContext.SaveChanges();

            // Query and Delete entity
            VehicleGPSType updated = queryable.Single();
            Assert.AreEqual(200.1, newCreated.VehicleSpeed);

            TestClientContext.DeleteObject(updated);
            TestClientContext.SaveChanges();
            Assert.AreEqual(2, derivedVehicleGPSSet.Count());
        }

        [TestMethod]
        public void QueryProperty()
        {
            // Load property and navigation property from type declared in main model
            var outsideGeoFenceAlarm = (TestClientContext.Trucks.Where(t => t.Key == "Key1")).Select(t => t.OutsideGeoFenceAlarm).Single();
            Assert.AreEqual(3, outsideGeoFenceAlarm.Severity);

            var truck = (TestClientContext.Trucks.Where(t => t.Key == "Key1")).Single();
            TestClientContext.LoadProperty(truck, "VehicleGPS");
            Assert.IsTrue(truck.VehicleGPS != null);

            // Load property from type declared in referenced model
            var currentLocation = (TestClientContext.VehicleGPSSet.Where(t => t.Key == "VehicleKey2")).Select(t => t.CurrentLocation).Single();
            Assert.AreEqual(12.3, currentLocation.Long);

            // Load property from type derived from declared in referenced model
            var displayName = (TestClientContext.DerivedVehicleGPSSet.Where(t => t.Key == "VehicleKey4")).Select(t => t.DisplayName).Single();
            Assert.AreEqual("DisplayName4", displayName);
        }

        [TestMethod]
        public void AddNavigationProperty()
        {
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;
            var truck = (TestClientContext.Trucks.Where(t => t.Key == "Key1")).Single();

            // Add Navigation Property to VehicleGPS in TruckType
            var newVehicleGPS = new VehicleGPSType()
            {
                Key = "99",
                VehicleSpeed = 100.1,
                StartLocation = new GeoLocation()
                {
                    Lat = 1,
                    Long = 2,
                },
                EndLocation = new GeoLocation()
                {
                    Lat = 3,
                    Long = 4,
                },
                CurrentLocation = new GeoLocation()
                {
                    Lat = 1.2,
                    Long = 2.4,
                },
                LostSignalAlarm = new GPSLostSignalAlarmType()
                {
                    Severity = 1,
                    LastKnownLocation = new GeoLocation()
                    {
                        Lat = 2.1,
                        Long = 1.2,
                    }
                },
                Map = new MapType()
                {
                    MBytesDownloaded = 1.2,
                    ProviderName = "TESTNEW",
                    Uri = "TESTNEW.TEST",
                }
            };
            TestClientContext.AddRelatedObject(truck, "VehicleGPSGroup", newVehicleGPS);
            TestClientContext.SaveChanges();

            TestClientContext.LoadProperty(truck, "VehicleGPSGroup");
            Assert.AreEqual(2, truck.VehicleGPSGroup.Count);

            // Add Navigation Property to VehicleGPSGroupFromGPS in TruckType
            var newVehicleGPSInGPS = new VehicleGPSType()
            {
                Key = "102",
                VehicleSpeed = 100.1,
                StartLocation = new GeoLocation()
                {
                    Lat = 1,
                    Long = 2,
                },
                EndLocation = new GeoLocation()
                {
                    Lat = 3,
                    Long = 4,
                },
                CurrentLocation = new GeoLocation()
                {
                    Lat = 1.2,
                    Long = 2.4,
                },
                LostSignalAlarm = new GPSLostSignalAlarmType()
                {
                    Severity = 1,
                    LastKnownLocation = new GeoLocation()
                    {
                        Lat = 2.1,
                        Long = 1.2,
                    }
                },
                Map = new MapType()
                {
                    MBytesDownloaded = 1.2,
                    ProviderName = "TESTNEW",
                    Uri = "TESTNEW.TEST",
                }
            };

            TestClientContext.AddRelatedObject(truck, "VehicleGPSGroupFromGPS", newVehicleGPSInGPS);
            TestClientContext.SaveChanges();

            TestClientContext.LoadProperty(truck, "VehicleGPSGroupFromGPS");
            Assert.AreEqual(2, truck.VehicleGPSGroupFromGPS.Count);
        }

        [TestMethod]
        public void TypeCast()
        {
            // Cast type from referenced model to type
            var querable1 = TestClientContext.VehicleGPSSet.Where(v => v.Key == "VehicleKey6");
            var derivedEntity = querable1.Single() as DerivedVehicleGPSType;
            Assert.AreEqual("DisplayName6", derivedEntity.DisplayName);

            var querable2 = TestClientContext.VehicleGPSSetInGPS.Where(v => v.Key == "DerivedVehicleGPSInGPSKey3");
            var derivedEntity2 = querable2.Single() as DerivedVehicleGPSType;
            Assert.AreEqual("DerivedVehicleGPSInGPSDP", derivedEntity2.DisplayName);

            // Verify Linq result
            var querable3 = TestClientContext.VehicleGPSSet.Where(v => v.Key == "VehicleKey6").OfType<DerivedVehicleGPSType>();
            var test = querable3 as DataServiceQuery;
            Assert.IsTrue(test.RequestUri.OriginalString.EndsWith("VehicleGPSSet/Microsoft.OData.SampleService.Models.ModelRefDemo.TruckDemo.DerivedVehicleGPSType('VehicleKey6')"));
        }
    }
}
