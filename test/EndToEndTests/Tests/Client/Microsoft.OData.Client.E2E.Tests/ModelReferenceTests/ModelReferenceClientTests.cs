//---------------------------------------------------------------------
// <copyright file="ModelReferenceClientTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.ODataModelRef.GPS;
using Microsoft.OData.E2E.TestCommon.Common.Client.ODataModelRef.TruckDemo;
using Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef;
using Xunit;
using GeoLocation = Microsoft.OData.E2E.TestCommon.Common.Client.ODataModelRef.Location.GeoLocation;
using GPSLostSignalAlarmType = Microsoft.OData.E2E.TestCommon.Common.Client.ODataModelRef.GPS.GPSLostSignalAlarmType;
using MapType = Microsoft.OData.E2E.TestCommon.Common.Client.ODataModelRef.Map.MapType;
using VehicleGPSType = Microsoft.OData.E2E.TestCommon.Common.Client.ODataModelRef.GPS.VehicleGPSType;

namespace Microsoft.OData.Client.E2E.Tests.ModelReferenceTests;

public class ModelReferenceClientTests : EndToEndTestBase<ModelReferenceClientTests.TestsStartup>
{
    private readonly Uri _baseUri;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(ODataModelRefTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
            {
                opt.AddRouteComponents("odata", ODataModelRefEdmModel.GetEdmModel()).EnableQueryFeatures();
            });
        }
    }

    public ModelReferenceClientTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");
    }

    // Query set - Create entity - Get created entity - Update entity - Delete entity of set in referenced entity container
    // But type declared in main model
    [Fact]
    public async Task QueryCreateUpdateDelete_EntitySetInReferencedModel()
    {
        // Arrange
        var context = this.ContextWrapper();
        context.MergeOption = MergeOption.OverwriteChanges;

        // Query Entity Set in GPS
        var vehicleGPSSetInGPS = context.VehicleGPSSetInGPS;
        Assert.Equal(3, vehicleGPSSetInGPS.Count());

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

        context.AddToVehicleGPSSetInGPS(newVehicleGPS);
        await context.SaveChangesAsync();

        // Get the created entity
        var queryable = context.VehicleGPSSetInGPS.Where(vehicleGPS => vehicleGPS.Key == "101");
        VehicleGPSType newCreated = queryable.Single();
        Assert.Equal(100.1, newCreated.VehicleSpeed);

        // Update the created entity 
        newCreated.VehicleSpeed = 200.1;
        context.UpdateObject(newCreated);
        await context.SaveChangesAsync();

        // Query and Delete entity
        VehicleGPSType updated = queryable.Single();
        Assert.Equal(200.1, newCreated.VehicleSpeed);

        context.DeleteObject(updated);
        await context.SaveChangesAsync();
        Assert.Equal(3, vehicleGPSSetInGPS.Count());
    }

    // Query set - Create entity - Get created entity - Update entity - Delete entity of set in main entity container
    // But type declared in referenced model
    [Fact]
    public async Task QueryCreateUpdateDelete_TypeDeclaredInReferencedModel()
    {
        // Arrange
        var context = this.ContextWrapper();
        context.MergeOption = MergeOption.OverwriteChanges;

        // Query VehicleGPSSet
        var vehicleGPSSet = context.VehicleGPSSet;
        Assert.Equal(3, vehicleGPSSet.Count());

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

        context.AddToVehicleGPSSet(newVehicleGPS);
        await context.SaveChangesAsync();

        // Get the created entity
        var queryable = context.VehicleGPSSet.Where(vehicleGPS => vehicleGPS.Key == "101");
        VehicleGPSType newCreated = queryable.Single();
        Assert.Equal(100.1, newCreated.VehicleSpeed);

        // Update the created entity 
        newCreated.VehicleSpeed = 200.1;
        context.UpdateObject(newCreated);
        await context.SaveChangesAsync();

        // Query and Delete entity
        VehicleGPSType updated = queryable.Single();
        Assert.Equal(200.1, newCreated.VehicleSpeed);

        context.DeleteObject(updated);
        await context.SaveChangesAsync();
        Assert.Equal(3, vehicleGPSSet.Count());
    }

    // Query set - Create entity - Get created entity - Update entity - Delete entity of set in main entity container
    // Type defined in main container, but derived from type referenced model
    [Fact]
    public void QueryCreateUpdateDelete_EntitySetDerivedFromReferencedModelType()
    {
        // Arrange
        var context = this.ContextWrapper();
        context.MergeOption = MergeOption.OverwriteChanges;

        // Query VehicleGPSSet
        var derivedVehicleGPSSet = context.DerivedVehicleGPSSet;
        Assert.Equal(2, derivedVehicleGPSSet.Count());

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

        context.AddToDerivedVehicleGPSSet(newVehicleGPS);
        context.SaveChanges();

        // Get the created entity
        var queryable = context.DerivedVehicleGPSSet.Where(vehicleGPS => vehicleGPS.Key == "101");
        VehicleGPSType newCreated = queryable.Single();
        Assert.Equal(100.1, newCreated.VehicleSpeed);

        // Update the created entity 
        newCreated.VehicleSpeed = 200.1;
        context.UpdateObject(newCreated);
        context.SaveChanges();

        // Query and Delete entity
        VehicleGPSType updated = queryable.Single();
        Assert.Equal(200.1, newCreated.VehicleSpeed);

        context.DeleteObject(updated);
        context.SaveChanges();
        Assert.Equal(2, derivedVehicleGPSSet.Count());
    }

    [Fact]
    public void QueryPropertiesAndNavigationProperties_FromMainModels()
    {
        // Arrange
        var context = this.ContextWrapper();

        // Load property and navigation property from type declared in main model
        var property = (context.Trucks.Where(t => t.Key == "Key1")).Select(t => new { t.TruckStoppedAlarm }).Single();
        Assert.Equal(2, property.TruckStoppedAlarm.Severity);
        Assert.Equal((double)1.2, property.TruckStoppedAlarm.LocationAndFuel.FuelLevel);
        Assert.Equal((double)101.1, property.TruckStoppedAlarm.LocationAndFuel.Location.Lat);

        var truck = (context.Trucks.Where(t => t.Key == "Key1")).Single();
        context.LoadProperty(truck, "HeadUnit");
        Assert.NotNull(truck.HeadUnit);
        Assert.Equal("SerialNo1", truck.HeadUnit.SerialNo);
        Assert.Equal((double)3.5, truck.HeadUnit.DimmingLevel);
    }

    [Fact]
    public void QueryPropertiesAndNavigationProperties_FromReferencedModels()
    {
        // Arrange
        var context = this.ContextWrapper();

        // Load property and navigation property from type declared in main model
        var outsideGeoFenceAlarm = (context.Trucks.Where(t => t.Key == "Key1")).Select(t => new { t.OutsideGeoFenceAlarm }).Single();
        Assert.Equal(3, outsideGeoFenceAlarm.OutsideGeoFenceAlarm.Severity);

        var navigationProperty = (context.Trucks.Where(t => t.Key == "Key1")).Select(t => new { t.VehicleGPS }).Single();
        Assert.NotNull(navigationProperty.VehicleGPS);
        Assert.Equal("VehicleKey1", navigationProperty.VehicleGPS.Key);
        Assert.Equal((double)120, navigationProperty.VehicleGPS.VehicleSpeed);
        Assert.Equal((double)19.1, navigationProperty.VehicleGPS.StartLocation.Lat);

        var truck = (context.Trucks.Where(t => t.Key == "Key1")).Single();
        context.LoadProperty(truck, "VehicleGPS");
        Assert.NotNull(truck.VehicleGPS);
        Assert.Equal("VehicleKey1", truck.VehicleGPS.Key);
        Assert.Equal((double)120, truck.VehicleGPS.VehicleSpeed);
        Assert.Equal((double)19.1, truck.VehicleGPS.StartLocation.Lat);

        // Load property from type declared in referenced model
        var currentLocation = (context.VehicleGPSSet.Where(t => t.Key == "VehicleKey2")).Select(t => new { t.CurrentLocation }).Single();
        Assert.Equal(12.3, currentLocation.CurrentLocation.Long);

        // Load property from type derived from declared in referenced model
        var displayName = (context.DerivedVehicleGPSSet.Where(t => t.Key == "VehicleKey4")).Select(t => new { t.DisplayName }).Single();
        Assert.Equal("DisplayName4", displayName.DisplayName);
    }

    [Fact]
    public async Task AddNavigationProperties_ToEntitiesInMainModels()
    {
        // Arrange
        var context = this.ContextWrapper();
        context.MergeOption = MergeOption.OverwriteChanges;

        var truck = (context.Trucks.Where(t => t.Key == "Key1")).Single();

        var headUnit = HeadUnitType.CreateHeadUnitType("SomeSerialNo");
        headUnit.DimmingLevel = 5.6;

        context.UpdateRelatedObject(truck, "HeadUnit", headUnit);
        await context.SaveChangesAsync();

        context.LoadProperty(truck, "HeadUnit");
        Assert.NotNull(truck.HeadUnit);
        Assert.Equal("SomeSerialNo", truck.HeadUnit.SerialNo);
        Assert.Equal((double)5.6, truck.HeadUnit.DimmingLevel);
    }

    [Fact]
    public async Task AddNavigationProperties_ToEntitiesInReferencedModels()
    {
        // Arrange
        var context = this.ContextWrapper();
        context.MergeOption = MergeOption.OverwriteChanges;

        var truck = (context.Trucks.Where(t => t.Key == "Key1")).Single();

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

        context.AddRelatedObject(truck, "VehicleGPSGroup", newVehicleGPS);
        await context.SaveChangesAsync();

        context.LoadProperty(truck, "VehicleGPSGroup");
        Assert.Equal(2, truck.VehicleGPSGroup.Count);

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

        context.AddRelatedObject(truck, "VehicleGPSGroupFromGPS", newVehicleGPSInGPS);
        await context.SaveChangesAsync();

        context.LoadProperty(truck, "VehicleGPSGroupFromGPS");
        Assert.Equal(2, truck.VehicleGPSGroupFromGPS.Count);
    }

    [Fact]
    public void TypeCasting_FromReferencedModelToDerivedType()
    {
        // Arrange
        var context = this.ContextWrapper();

        // Cast type from referenced model to type
        var queryable1 = context.VehicleGPSSet.Where(v => v.Key == "VehicleKey6");
        var derivedEntity = queryable1.Single() as DerivedVehicleGPSType;
        Assert.Equal("DisplayName6", derivedEntity?.DisplayName);

        var queryable2 = context.VehicleGPSSetInGPS.Where(v => v.Key == "DerivedVehicleGPSInGPSKey3");
        var derivedEntity2 = queryable2.Single() as DerivedVehicleGPSType;
        Assert.Equal("DerivedVehicleGPSInGPSDP", derivedEntity2?.DisplayName);

        // Verify Linq result
        var queryable3 = (context.VehicleGPSSet.OfType<DerivedVehicleGPSType>() as DataServiceQuery<DerivedVehicleGPSType>).ByKey("VehicleKey6");
        Assert.EndsWith("VehicleGPSSet/Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.TruckDemo.DerivedVehicleGPSType('VehicleKey6')", queryable3.Query.RequestUri.OriginalString);
    }

    #region Private

    private TruckDemoService ContextWrapper()
    {
        var context = new TruckDemoService(_baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        ResetDefaultDataSource(context);

        return context;
    }

    private void ResetDefaultDataSource(TruckDemoService context)
    {
        var actionUri = new Uri(_baseUri + "odatamodelreftests/Default.ResetDefaultDataSource", UriKind.Absolute);
        context.Execute(actionUri, "POST");
    }

    #endregion
}
