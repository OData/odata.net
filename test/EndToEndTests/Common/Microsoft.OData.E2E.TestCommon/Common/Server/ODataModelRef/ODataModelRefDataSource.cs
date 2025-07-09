//-----------------------------------------------------------------------------
// <copyright file="ODataModelRefDataSource.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.ObjectModel;
using Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.GPS;
using Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.Location;
using Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.Map;
using Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.TruckDemo;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef;

public class ODataModelRefDataSource
{
    public static ODataModelRefDataSource CreateInstance()
    {
        return new ODataModelRefDataSource();
    }

    public ODataModelRefDataSource()
    {
        ResetData();
        InitializeData();
    }

    private void ResetData()
    {
        this.Trucks?.Clear();
        this.VehicleGPSSet?.Clear();
        this.DerivedVehicleGPSSet?.Clear();
        this.VehicleGPSSetInGPS?.Clear();
    }

    public IList<TruckType>? Trucks { get; private set; }
    public IList<VehicleGPSType>? VehicleGPSSet { get; private set; }
    public IList<DerivedVehicleGPSType>? DerivedVehicleGPSSet { get; private set; }
    public IList<VehicleGPSType>? VehicleGPSSetInGPS { get; private set; }

    private void InitializeData()
    {
        #region Trucks
        this.Trucks = new List<TruckType>()
        {
            new TruckType()
            {
                Key = "Key1",
                VIN = "Vin1",
                FuelLevel = 1,
                ACState = true,
                TruckIsHomeFuelLevel = 1.1,
                TruckStoppedAlarm = new TruckStoppedAlarmType()
                {
                    Severity = 2,
                    LocationAndFuel = new LocationAndFuel()
                    {
                        FuelLevel = 1.2,
                        Location = new GeoLocation()
                        {
                            Lat = 101.1,
                            Long = 28.4,
                        }
                    }
                },
                OutsideGeoFenceAlarm = new OutsideGeoFenceAlarmType()
                {
                    Severity = 3,
                    Location = new GeoLocation()
                    {
                        Lat = 201.1,
                        Long = 11.34,
                    }
                },
                HeadUnit = new HeadUnitType()
                {
                    SerialNo = "SerialNo1",
                    DimmingLevel = 3.5,
                },
                VehicleGPS = new VehicleGPSType()
                {
                    Key = "VehicleKey1",
                    VehicleSpeed = 120,
                    StartLocation = new GeoLocation()
                    {
                        Lat = 19.1,
                        Long = 12.3,
                    },
                    EndLocation = new GeoLocation()
                    {
                        Lat = 18.1,
                        Long = 12.3,
                    },
                    CurrentLocation = new GeoLocation()
                    {
                        Lat = 18.7,
                        Long = 12.3,
                    },
                    Map = new MapType()
                    {
                        MBytesDownloaded = 12.3,
                        ProviderName = "ProviderName1",
                        Uri = "Test",
                    },
                    LostSignalAlarm = new GPSLostSignalAlarmType()
                    {
                        Severity = 3,
                        LastKnownLocation = new GeoLocation()
                        {
                            Lat = 12,
                            Long = 12,
                        }
                    }
                },
                VehicleGPSGroup = new Collection<VehicleGPSType>(),
                VehicleGPSGroupFromGPS = new Collection<VehicleGPSType>()
            }
        };
        #endregion

        #region VehicleGPSSet

        this.VehicleGPSSet = new List<VehicleGPSType>()
        {
            new VehicleGPSType()
            {
                Key = "VehicleKey2",
                VehicleSpeed = 120,
                StartLocation = new GeoLocation()
                {
                    Lat = 19.1,
                    Long = 12.3,
                },
                EndLocation = new GeoLocation()
                {
                    Lat = 18.1,
                    Long = 12.3,
                },
                CurrentLocation = new GeoLocation()
                {
                    Lat = 18.7,
                    Long = 12.3,
                },
                Map = new MapType()
                {
                    MBytesDownloaded = 12.3,
                    ProviderName = "ProviderName2",
                    Uri = "Test2",
                },
                LostSignalAlarm = new GPSLostSignalAlarmType()
                {
                    Severity = 3,
                    LastKnownLocation = new GeoLocation()
                    {
                        Lat = 12,
                        Long = 12,
                    }
                }
            },
            new VehicleGPSType()
            {
                Key = "VehicleKey3",
                VehicleSpeed = 120,
                StartLocation = new GeoLocation()
                {
                    Lat = 19.1,
                    Long = 12.3,
                },
                EndLocation = new GeoLocation()
                {
                    Lat = 18.1,
                    Long = 12.3,
                },
                CurrentLocation = new GeoLocation()
                {
                    Lat = 18.7,
                    Long = 12.3,
                },
                Map = new MapType()
                {
                    MBytesDownloaded = 12.3,
                    ProviderName = "ProviderName3",
                    Uri = "Test3",
                },
                LostSignalAlarm = new GPSLostSignalAlarmType()
                {
                    Severity = 3,
                    LastKnownLocation = new GeoLocation()
                    {
                        Lat = 12,
                        Long = 12,
                    }
                }
            },
            new DerivedVehicleGPSType()
            {
                Key = "VehicleKey6",
                VehicleSpeed = 120,
                StartLocation = new GeoLocation()
                {
                    Lat = 19.1,
                    Long = 12.3,
                },
                EndLocation = new GeoLocation()
                {
                    Lat = 18.1,
                    Long = 12.3,
                },
                CurrentLocation = new GeoLocation()
                {
                    Lat = 18.7,
                    Long = 12.3,
                },
                Map = new MapType()
                {
                    MBytesDownloaded = 12.3,
                    ProviderName = "ProviderName6",
                    Uri = "Test4",
                },
                LostSignalAlarm = new GPSLostSignalAlarmType()
                {
                    Severity = 3,
                    LastKnownLocation = new GeoLocation()
                    {
                        Lat = 12,
                        Long = 12,
                    }
                },
                DisplayName = "DisplayName6"
            },
        };

        #endregion

        #region DerivedVehicleGPSSet
        this.DerivedVehicleGPSSet = new List<DerivedVehicleGPSType>()
        {
            new DerivedVehicleGPSType()
            {
                Key = "VehicleKey4",
                VehicleSpeed = 120,
                StartLocation = new GeoLocation()
                {
                    Lat = 19.1,
                    Long = 12.3,
                },
                EndLocation = new GeoLocation()
                {
                    Lat = 18.1,
                    Long = 12.3,
                },
                CurrentLocation = new GeoLocation()
                {
                    Lat = 18.7,
                    Long = 12.3,
                },
                Map = new MapType()
                {
                    MBytesDownloaded = 12.3,
                    ProviderName = "ProviderName4",
                    Uri = "Test4",
                },
                LostSignalAlarm = new GPSLostSignalAlarmType()
                {
                    Severity = 3,
                    LastKnownLocation = new GeoLocation()
                    {
                        Lat = 12,
                        Long = 12,
                    }
                },
                DisplayName = "DisplayName4"
            },
            new DerivedVehicleGPSType()
            {
                Key = "VehicleKey5",
                VehicleSpeed = 120,
                StartLocation = new GeoLocation()
                {
                    Lat = 19.1,
                    Long = 12.3,
                },
                EndLocation = new GeoLocation()
                {
                    Lat = 18.1,
                    Long = 12.3,
                },
                CurrentLocation = new GeoLocation()
                {
                    Lat = 18.7,
                    Long = 12.3,
                },
                Map = new MapType()
                {
                    MBytesDownloaded = 12.3,
                    ProviderName = "ProviderName5",
                    Uri = "Test5",
                },
                LostSignalAlarm = new GPSLostSignalAlarmType()
                {
                    Severity = 3,
                    LastKnownLocation = new GeoLocation()
                    {
                        Lat = 12,
                        Long = 12,
                    }
                },
                DisplayName = "DisplayName5"
            },
        };
        #endregion

        #region VehicleGPSSetInGPS
        this.VehicleGPSSetInGPS = new List<VehicleGPSType>()
        {
            new VehicleGPSType()
            {
                Key = "VehicleGPSSetInGPSKey1",
                VehicleSpeed = 120,
                StartLocation = new GeoLocation()
                {
                    Lat = 19.1,
                    Long = 12.3,
                },
                EndLocation = new GeoLocation()
                {
                    Lat = 18.1,
                    Long = 12.3,
                },
                CurrentLocation = new GeoLocation()
                {
                    Lat = 18.7,
                    Long = 12.3,
                },
                Map = new MapType()
                {
                    MBytesDownloaded = 12.3,
                    ProviderName = "VehicleGPSSetInGPSProviderName1",
                    Uri = "Test2",
                },
                LostSignalAlarm = new GPSLostSignalAlarmType()
                {
                    Severity = 3,
                    LastKnownLocation = new GeoLocation()
                    {
                        Lat = 12,
                        Long = 12,
                    }
                }
            },
            new VehicleGPSType()
            {
                Key = "VehicleGPSSetInGPSKey2",
                VehicleSpeed = 120,
                StartLocation = new GeoLocation()
                {
                    Lat = 19.1,
                    Long = 12.3,
                },
                EndLocation = new GeoLocation()
                {
                    Lat = 18.1,
                    Long = 12.3,
                },
                CurrentLocation = new GeoLocation()
                {
                    Lat = 18.7,
                    Long = 12.3,
                },
                Map = new MapType()
                {
                    MBytesDownloaded = 12.3,
                    ProviderName = "VehicleGPSSetInGPSProviderName2",
                    Uri = "Test3",
                },
                LostSignalAlarm = new GPSLostSignalAlarmType()
                {
                    Severity = 3,
                    LastKnownLocation = new GeoLocation()
                    {
                        Lat = 12,
                        Long = 12,
                    }
                }
            },
            new DerivedVehicleGPSType()
            {
                Key = "DerivedVehicleGPSInGPSKey3",
                VehicleSpeed = 120,
                StartLocation = new GeoLocation()
                {
                    Lat = 19.1,
                    Long = 12.3,
                },
                EndLocation = new GeoLocation()
                {
                    Lat = 18.1,
                    Long = 12.3,
                },
                CurrentLocation = new GeoLocation()
                {
                    Lat = 18.7,
                    Long = 12.3,
                },
                Map = new MapType()
                {
                    MBytesDownloaded = 12.3,
                    ProviderName = "VehicleGPSSetInGPSProviderName3",
                    Uri = "Test4",
                },
                LostSignalAlarm = new GPSLostSignalAlarmType()
                {
                    Severity = 3,
                    LastKnownLocation = new GeoLocation()
                    {
                        Lat = 12,
                        Long = 12,
                    }
                },
                DisplayName = "DerivedVehicleGPSInGPSDP"
            },
        };
        #endregion

        #region Navigation Property
        this.Trucks[0].VehicleGPSGroup.Add(this.VehicleGPSSet[0]);
        this.Trucks[0].VehicleGPSGroupFromGPS.Add(this.VehicleGPSSetInGPS[0]);
        #endregion
    }
}
