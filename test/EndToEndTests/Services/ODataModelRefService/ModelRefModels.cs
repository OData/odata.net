//---------------------------------------------------------------------
// <copyright file="ModelRefModels.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.SampleService.Models.ModelRefDemo.Map
{
    using Microsoft.Test.OData.Services.ODataWCFService;

    // ComplexType Map (Map.csdl)
    public class MapType : ClrObject
    {
        public string ProviderName { get; set; }
        public string Uri { get; set; }
        public double MBytesDownloaded { get; set; }
    }
}

namespace Microsoft.OData.SampleService.Models.ModelRefDemo.Location
{
    using Microsoft.Test.OData.Services.ODataWCFService;

    // ComplexType GeoLocation (Location.csdl)
    public class GeoLocation : ClrObject
    {
        public double Lat { get; set; }
        public double Long { get; set; }
    }

    // ComplexType OutsideGeoFenceAlarmType (Location.csdl)
    public class OutsideGeoFenceAlarmType : ClrObject
    {
        public int Severity { get; set; }
        public GeoLocation Location { get; set; }
    }
}

namespace Microsoft.OData.SampleService.Models.ModelRefDemo.GPS
{
    using Microsoft.OData.SampleService.Models.ModelRefDemo.Location;
    using Microsoft.OData.SampleService.Models.ModelRefDemo.Map;
    using Microsoft.Test.OData.Services.ODataWCFService;

    // ComplexType GPSLostSignalAlarmType (GPS.csdl)
    public class GPSLostSignalAlarmType : ClrObject
    {
        public int Severity { get; set; }
        public GeoLocation LastKnownLocation { get; set; }
    }

    // EntityType (Containment) VehicleGPSType (GPS.csdl)
    public class VehicleGPSType : ClrObject
    {
        public string Key { get; set; }
        public double VehicleSpeed { get; set; }
        public GeoLocation StartLocation { get; set; }
        public GeoLocation EndLocation { get; set; }
        public GeoLocation CurrentLocation { get; set; }
        public MapType Map { get; set; }
        public GPSLostSignalAlarmType LostSignalAlarm { get; set; }
    }
}

namespace Microsoft.OData.SampleService.Models.ModelRefDemo.TruckDemo
{
    using Microsoft.OData.SampleService.Models.ModelRefDemo.GPS;
    using Microsoft.OData.SampleService.Models.ModelRefDemo.Location;
    using Microsoft.Test.OData.Services.ODataWCFService;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    // ComplexType LocationAndFuel (TruckDemo.csdl)
    public class LocationAndFuel : ClrObject
    {
        public double FuelLevel { get; set; }
        public GeoLocation Location { get; set; }
    }

    // ComplexType TruckStoppedAlarmType (TruckDemo.csdl)
    public class TruckStoppedAlarmType : ClrObject
    {
        public int Severity { get; set; }
        public LocationAndFuel LocationAndFuel { get; set; }
    }

    // EntityType (Containment) HeadUnitType (TruckDemo.csdl)
    public class HeadUnitType : ClrObject
    {
        public string SerialNo { get; set; }
        public double DimmingLevel { get; set; }
    }

    public class DerivedVehicleGPSType : VehicleGPSType
    {
        public string DisplayName { get; set; }
    }

    // EntityType TruckType (TruckDemo.csdl)
    public class TruckType : ClrObject
    {
        private EntityCollection<VehicleGPSType> vehicleGPSGroup;
        private EntityCollection<VehicleGPSType> vehicleGPSGroupFromGPS;

        public TruckType()
        {
            vehicleGPSGroup = new EntityCollection<VehicleGPSType>(DataSourceManager.GetCurrentDataSource<ModelRefSvcDataSource>().VehicleGPSSet);
            vehicleGPSGroupFromGPS = new EntityCollection<VehicleGPSType>(DataSourceManager.GetCurrentDataSource<ModelRefSvcDataSource>().VehicleGPSSetInGPS);
        }

        public string Key { get; set; }
        public string VIN { get; set; }
        public double FuelLevel { get; set; }
        public bool ACState { get; set; }
        public double TruckIsHomeFuelLevel { get; set; }
        public TruckStoppedAlarmType TruckStoppedAlarm { get; set; }
        public OutsideGeoFenceAlarmType OutsideGeoFenceAlarm { get; set; }
        public HeadUnitType HeadUnit { get; set; }
        public VehicleGPSType VehicleGPS { get; set; }

        public EntityCollection<VehicleGPSType> VehicleGPSGroupFromGPS
        {
            get
            {
                return this.vehicleGPSGroupFromGPS.Cleanup();
            }
        }

        public EntityCollection<VehicleGPSType> VehicleGPSGroup
        {
            get
            {
                return this.vehicleGPSGroup.Cleanup();
            }
        }
    }
}