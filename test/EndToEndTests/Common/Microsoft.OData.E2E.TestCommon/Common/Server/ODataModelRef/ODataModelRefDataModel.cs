//-----------------------------------------------------------------------------
// <copyright file="ODataModelRefDataModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using EfKey = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.Map
{
    public class MapType
    {
        public string? ProviderName { get; set; }
        public string? Uri { get; set; }
        public double MBytesDownloaded { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}

namespace Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.Location
{
    // ComplexType GeoLocation (Location.csdl)
    public class GeoLocation
    {
        public double Lat { get; set; }
        public double Long { get; set; }
        public DateTime UpdatedTime { get; set; }
    }

    // ComplexType OutsideGeoFenceAlarmType (Location.csdl)
    public class OutsideGeoFenceAlarmType
    {
        public int Severity { get; set; }
        public GeoLocation? Location { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}

namespace Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.GPS
{
    using Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.Location;
    using Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.Map;

    // ComplexType GPSLostSignalAlarmType (GPS.csdl)
    public class GPSLostSignalAlarmType
    {
        public int Severity { get; set; }
        public GeoLocation? LastKnownLocation { get; set; }
        public DateTime UpdatedTime { get; set; }
    }

    // EntityType (Containment) VehicleGPSType (GPS.csdl)
    public class VehicleGPSType
    {
        [EfKey]
        public string Key { get; set; }
        public double VehicleSpeed { get; set; }
        public GeoLocation? StartLocation { get; set; }
        public GeoLocation? EndLocation { get; set; }
        public GeoLocation? CurrentLocation { get; set; }
        public MapType? Map { get; set; }
        public GPSLostSignalAlarmType? LostSignalAlarm { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}

namespace Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.TruckDemo
{
    using System.Collections.ObjectModel;
    using Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.GPS;
    using Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.Location;
    using Microsoft.OData.ModelBuilder;

    // ComplexType LocationAndFuel (TruckDemo.csdl)
    public class LocationAndFuel
    {
        public double FuelLevel { get; set; }
        public GeoLocation? Location { get; set; }
        public DateTime UpdatedTime { get; set; }
    }

    // ComplexType TruckStoppedAlarmType (TruckDemo.csdl)
    public class TruckStoppedAlarmType
    {
        public int Severity { get; set; }
        public LocationAndFuel? LocationAndFuel { get; set; }
        public DateTime UpdatedTime { get; set; }
    }

    // EntityType (Containment) HeadUnitType (TruckDemo.csdl)
    public class HeadUnitType
    {
        [EfKey]
        public string SerialNo { get; set; }
        public double DimmingLevel { get; set; }
        public DateTime UpdatedTime { get; set; }
    }

    public class DerivedVehicleGPSType : VehicleGPSType
    {
        public string? DisplayName { get; set; }
    }

    // EntityType TruckType (TruckDemo.csdl)
    public class TruckType
    {
        [EfKey]
        public string Key { get; set; }
        public string VIN { get; set; }
        public double FuelLevel { get; set; }
        public bool ACState { get; set; }
        public double TruckIsHomeFuelLevel { get; set; }
        public TruckStoppedAlarmType? TruckStoppedAlarm { get; set; }
        public OutsideGeoFenceAlarmType? OutsideGeoFenceAlarm { get; set; }

        [Contained]
        public HeadUnitType HeadUnit { get; set; }

        [Contained]
        public VehicleGPSType VehicleGPS { get; set; }

        public Collection<VehicleGPSType>? VehicleGPSGroupFromGPS { get; set; }

        public Collection<VehicleGPSType>? VehicleGPSGroup { get; set; }

        public DateTime UpdatedTime { get; set; }
    }
}
