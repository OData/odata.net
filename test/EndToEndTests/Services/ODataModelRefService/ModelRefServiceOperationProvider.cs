//---------------------------------------------------------------------
// <copyright file="ModelRefServiceOperationProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.SampleService.Models.ModelRefDemo
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.OData.SampleService.Models.ModelRefDemo.GPS;
    using Microsoft.OData.SampleService.Models.ModelRefDemo.Location;
    using Microsoft.OData.SampleService.Models.ModelRefDemo.TruckDemo;
    using Microsoft.Test.OData.Services.ODataWCFService;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    public class ModelRefServiceOperationProvider : ODataReflectionOperationProvider
    {
        public void ResetDataSource()
        {
            var dataSource = DataSourceManager.GetCurrentDataSource<ModelRefSvcDataSource>();
            dataSource.Reset();
            dataSource.Initialize();
        }

        public void ResetVehicleSpeed(VehicleGPSType targetVehicleGPS, double targetValue)
        {
            targetVehicleGPS.VehicleSpeed = targetValue;
        }

        public double GetVehicleSpeed(VehicleGPSType targetVehicleGPS)
        {
            return targetVehicleGPS.VehicleSpeed;
        }

        public void SetACState(TruckType targetTruck, bool state)
        {
            targetTruck.ACState = state;
        }

        public void TurnOffAC(TruckType targetTruck)
        {
            targetTruck.ACState = false;
        }

        public OutsideGeoFenceAlarmType GetDefaultOutsideGeoFenceAlarm()
        {
            return new OutsideGeoFenceAlarmType
            {
                Severity = 1,
                Location = new GeoLocation
                {
                    Lat = 1.1,
                    Long = 2.2,
                }
            };
        }

        public Collection<VehicleGPSType> ResetVehicleSpeedToValue(double targetValue)
        {
            IEnumerable<VehicleGPSType> vehicleGPSSetInGPS = GetRootQuery("VehicleGPSSetInGPS") as IEnumerable<VehicleGPSType>;

            Collection<VehicleGPSType> returnVehicleGPSType = new Collection<VehicleGPSType>();

            foreach (var vehicleGPS in vehicleGPSSetInGPS)
            {
                vehicleGPS.VehicleSpeed = targetValue;
                returnVehicleGPSType.Add(vehicleGPS);
            }
            return returnVehicleGPSType;
        }

        private static object GetRootQuery(string propertyName)
        {
            var dataSource = DataSourceManager.GetCurrentDataSource<ModelRefSvcDataSource>();
            return dataSource.GetType().GetProperty(propertyName).GetValue(dataSource, null);
        }
    }
}
