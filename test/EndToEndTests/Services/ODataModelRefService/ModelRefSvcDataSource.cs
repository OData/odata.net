//---------------------------------------------------------------------
// <copyright file="ModelRefSvcDataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System.Collections.Generic;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.SampleService.Models.ModelRefDemo;
    using Microsoft.OData.SampleService.Models.ModelRefDemo.GPS;
    using Microsoft.OData.SampleService.Models.ModelRefDemo.Location;
    using Microsoft.OData.SampleService.Models.ModelRefDemo.Map;
    using Microsoft.OData.SampleService.Models.ModelRefDemo.TruckDemo;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    public class ModelRefSvcDataSource : ODataReflectionDataSource
    {
        private IEdmModel edmModel;

        public EntityCollection<TruckType> Trucks { get; private set; }

        public EntityCollection<VehicleGPSType> VehicleGPSSet { get; private set; }

        public EntityCollection<DerivedVehicleGPSType> DerivedVehicleGPSSet { get; private set; }

        public EntityCollection<VehicleGPSType> VehicleGPSSetInGPS { get; private set; }

        public ModelRefSvcDataSource()
        {
            this.OperationProvider = new ModelRefServiceOperationProvider();
        }

        public override void Reset()
        {
            this.Trucks = new EntityCollection<TruckType>();
            this.VehicleGPSSet = new EntityCollection<VehicleGPSType>();
            this.DerivedVehicleGPSSet = new EntityCollection<DerivedVehicleGPSType>();
            this.VehicleGPSSetInGPS = new EntityCollection<VehicleGPSType>();
        }

        public override void Initialize()
        {
            #region Trucks
            this.Trucks.AddRange(new List<TruckType>()
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
                }
            });
            #endregion

            #region VehicleGPSSet

            this.VehicleGPSSet.AddRange(new List<VehicleGPSType>()
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
            });

            #endregion

            #region DerivedVehicleGPSSet
            this.DerivedVehicleGPSSet.AddRange(new List<DerivedVehicleGPSType>()
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
            });
            #endregion

            #region VehicleGPSSetInGPS
            this.VehicleGPSSetInGPS.AddRange(new List<VehicleGPSType>()
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
            });
            #endregion

            #region Navigation Property
            this.Trucks[0].VehicleGPSGroup.Add(this.VehicleGPSSet[0]);
            this.Trucks[0].VehicleGPSGroupFromGPS.Add(this.VehicleGPSSetInGPS[0]);
            #endregion
        }

        public override IEdmModel Model
        {
            get { return this.edmModel; }
        }

        public void ResetEdmModel()
        {
            this.edmModel = this.CreateModel();
        }

        protected override IEdmModel CreateModel()
        {
            return ModelRefInMemoryModel.CreateModelRefServiceModel();
        }

        protected override void ConfigureContainer(IContainerBuilder builder)
        {
            base.ConfigureContainer(builder);
            builder.AddService(ServiceLifetime.Scoped, sp => this.edmModel);
        }
    }
}