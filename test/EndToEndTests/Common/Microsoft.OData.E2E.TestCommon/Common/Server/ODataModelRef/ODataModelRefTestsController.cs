//-----------------------------------------------------------------------------
// <copyright file="ODataModelRefTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.GPS;
using Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.Location;
using Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.TruckDemo;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef;

public class ODataModelRefTestsController : ODataController
{
    private static ODataModelRefDataSource _dataSource;

    [EnableQuery]
    [HttpGet("odata/Trucks")]
    public IActionResult GetTrucks()
    {
        return Ok(_dataSource.Trucks);
    }

    [EnableQuery]
    [HttpGet("odata/Trucks({key})")]
    public IActionResult GetATruck([FromRoute] string key)
    {
        var current = _dataSource.Trucks?.SingleOrDefault(t => t.Key == key);
        if (current == null)
        {
            return NoContent();
        }

        return Ok(current);
    }

    [EnableQuery]
    [HttpGet("odata/Trucks({key})/VehicleGPS")]
    public IActionResult GetTrucksVehicleGPS([FromRoute] string key)
    {
        var current = _dataSource.Trucks?.SingleOrDefault(t => t.Key == key);
        if(current == null)
        {
            return NotFound();
        }

        return Ok(current.VehicleGPS);
    }

    [EnableQuery]
    [HttpGet("odata/Trucks({key})/HeadUnit")]
    public IActionResult GetTrucksHeadUnit([FromRoute] string key)
    {
        var current = _dataSource.Trucks?.SingleOrDefault(t => t.Key == key);
        if (current == null)
        {
            return NotFound();
        }

        return Ok(current.HeadUnit);
    }

    [EnableQuery]
    [HttpGet("odata/Trucks({key})/VehicleGPSGroup")]
    public IActionResult GetTrucksVehicleGPSGroup([FromRoute] string key)
    {
        var current = _dataSource.Trucks?.SingleOrDefault(t => t.Key == key);
        if (current == null)
        {
            return NotFound();
        }

        return Ok(current.VehicleGPSGroup);
    }

    [EnableQuery]
    [HttpGet("odata/Trucks({key})/VehicleGPSGroupFromGPS")]
    public IActionResult GetTrucksVehicleGPSGroupFromGPS([FromRoute] string key)
    {
        var current = _dataSource.Trucks?.SingleOrDefault(t => t.Key == key);
        if (current == null)
        {
            return NotFound();
        }

        return Ok(current.VehicleGPSGroupFromGPS);
    }

    [EnableQuery]
    [HttpGet("odata/VehicleGPSSetInGPS")]
    public IActionResult GetVehicleGPSSetInGPS()
    {
        return Ok(_dataSource.VehicleGPSSetInGPS);
    }

    [EnableQuery]
    [HttpGet("odata/VehicleGPSSetInGPS({key})")]
    public IActionResult GetAVehicleGPSSetInGPS([FromRoute] string key)
    {
        var current = _dataSource.VehicleGPSSetInGPS?.SingleOrDefault(t => t.Key == key);
        if (current == null)
        {
            return NoContent();
        }

        return Ok(current);
    }

    [EnableQuery]
    [HttpGet("odata/VehicleGPSSetInGPS({key})/Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.TruckDemo.DerivedVehicleGPSType")]
    public IActionResult GetAVehicleGPSSetInGPSDerived([FromRoute] string key)
    {
        var current = _dataSource.VehicleGPSSetInGPS?.SingleOrDefault(t => t.Key == key) as DerivedVehicleGPSType;
        if (current == null)
        {
            return NoContent();
        }

        return Ok(current);
    }

    [EnableQuery]
    [HttpGet("odata/VehicleGPSSetInGPS/$count")]
    public IActionResult GetVehicleGPSSetInGPSCount()
    {
        return Ok(_dataSource.VehicleGPSSetInGPS);
    }

    [EnableQuery]
    [HttpGet("odata/VehicleGPSSet")]
    public IActionResult GetVehicleGPSSet()
    {
        return Ok(_dataSource.VehicleGPSSet);
    }

    [EnableQuery]
    [HttpGet("odata/VehicleGPSSet/$count")]
    public IActionResult GetVehicleGPSSetCount()
    {
        return Ok(_dataSource.VehicleGPSSet);
    }

    [EnableQuery]
    [HttpGet("odata/VehicleGPSSet({key})")]
    public IActionResult GetVehicleGPS([FromRoute] string key)
    {
        var current = _dataSource.VehicleGPSSet?.SingleOrDefault(v => v.Key == key);
        if(current == null)
        {
            return NoContent();
        }

        return Ok(current);
    }

    [EnableQuery]
    [HttpGet("odata/VehicleGPSSet({key})/Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.TruckDemo.DerivedVehicleGPSType")]
    public IActionResult GetVehicleGPSDerivedVehicleGPSType([FromRoute] string key)
    {
        var current = _dataSource.VehicleGPSSet?.SingleOrDefault(v => v.Key == key) as DerivedVehicleGPSType;
        if (current == null)
        {
            return NoContent();
        }

        return Ok(current);
    }

    [EnableQuery]
    [HttpGet("odata/VehicleGPSSet({key})/Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.GPS.GetVehicleSpeed()")]
    public IActionResult GetVehicleSpeed([FromRoute] string key)
    {
        var current = _dataSource.VehicleGPSSet?.SingleOrDefault(v => v.Key == key);
        if (current == null)
        {
            return NoContent();
        }

        return Ok(current.VehicleSpeed);
    }

    [EnableQuery]
    [HttpPost("odata/VehicleGPSSet({key})/Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.GPS.ResetVehicleSpeed")]
    public IActionResult ResetVehicleSpeed([FromRoute] string key, [FromODataBody] double targetValue)
    {
        var current = _dataSource.VehicleGPSSet?.SingleOrDefault(v => v.Key == key);
        if (current == null)
        {
            return NoContent();
        }

        current.VehicleSpeed = targetValue;
        return Ok(current.VehicleSpeed);
    }

    [EnableQuery]
    [HttpGet("odata/DerivedVehicleGPSSet")]
    public IActionResult GetDerivedVehicleGPSSet()
    {
        return Ok(_dataSource.DerivedVehicleGPSSet);
    }

    [EnableQuery]
    [HttpGet("odata/DerivedVehicleGPSSet({key})")]
    public IActionResult GetADerivedVehicleGPS([FromRoute] string key)
    {
        var current = _dataSource.DerivedVehicleGPSSet?.SingleOrDefault(t => t.Key == key);
        if (current == null)
        {
            return NoContent();
        }

        return Ok(current);
    }

    [EnableQuery]
    [HttpGet("odata/DerivedVehicleGPSSet({key})/Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.GPS.VehicleGPSType/StartLocation")]
    public IActionResult GetDerivedVehicleGPSStartLocation([FromRoute] string key)
    {
        var current = _dataSource.DerivedVehicleGPSSet?.SingleOrDefault(t => t.Key == key) as VehicleGPSType;
        if (current == null)
        {
            return NoContent();
        }

        return Ok(current.StartLocation);
    }

    [EnableQuery]
    [HttpGet("odata/DerivedVehicleGPSSet/$count")]
    public IActionResult GetDerivedVehicleGPSSetCount()
    {
        return Ok(_dataSource.DerivedVehicleGPSSet);
    }

    [EnableQuery]
    [HttpGet("odata/GetDefaultOutsideGeoFenceAlarm()")]
    public IActionResult GetDefaultOutsideGeoFenceAlarm()
    {
        return Ok(new OutsideGeoFenceAlarmType
        {
            Severity = 1,
            Location = new GeoLocation
            {
                Lat = 1.1,
                Long = 2.2,
            }
        });
    }

    [HttpPatch("odata/VehicleGPSSetInGPS({key})")]
    public IActionResult PatchVehicleGPSSetInGPS([FromRoute] string key, [FromBody] Delta<VehicleGPSType> delta)
    {
        var current = _dataSource.VehicleGPSSetInGPS?.SingleOrDefault(v => v.Key == key);
        if(current == null)
        {
            return NotFound();
        }

        var patched = delta.Patch(current);
        return Updated(patched);
    }

    [HttpPatch("odata/VehicleGPSSet({key})")]
    public IActionResult PatchVehicleGPSSet([FromRoute] string key, [FromBody] Delta<VehicleGPSType> delta)
    {
        var current = _dataSource.VehicleGPSSet?.SingleOrDefault(v => v.Key == key);
        if (current == null)
        {
            return NotFound();
        }

        var patched = delta.Patch(current);
        return Updated(patched);
    }

    [HttpPatch("odata/Trucks({key})/HeadUnit")]
    public IActionResult PatchTruckHeadUnit([FromRoute] string key, [FromBody] Delta<HeadUnitType> delta)
    {
        var current = _dataSource.Trucks?.SingleOrDefault(t => t.Key == key);
        if (current == null)
        {
            return NotFound();
        }

        var patched = delta.Patch(current.HeadUnit);
        return Updated(patched);
    }

    [HttpPatch("odata/DerivedVehicleGPSSet({key})")]
    public IActionResult PatchDerivedVehicleGPSSet([FromRoute] string key, [FromBody] Delta<DerivedVehicleGPSType> delta)
    {
        var current = _dataSource.DerivedVehicleGPSSet?.SingleOrDefault(v => v.Key == key);
        if (current == null)
        {
            return NotFound();
        }

        var patched = delta.Patch(current);
        return Updated(patched);
    }

    [HttpPost("odata/VehicleGPSSetInGPS")]
    public IActionResult AddVehicleGPSSetInGPS([FromBody] VehicleGPSType vehicleGPS)
    {
        _dataSource.VehicleGPSSetInGPS?.Add(vehicleGPS);
        return Created(vehicleGPS);
    }

    [HttpPost("odata/VehicleGPSSet")]
    public IActionResult AddVehicleGPSSet([FromBody] VehicleGPSType vehicleGPS)
    {
        _dataSource.VehicleGPSSet?.Add(vehicleGPS);
        return Created(vehicleGPS);
    }

    [HttpPost("odata/DerivedVehicleGPSSet")]
    public IActionResult AddDerivedVehicleGPSType([FromBody] DerivedVehicleGPSType vehicleGPS)
    {
        _dataSource.DerivedVehicleGPSSet?.Add(vehicleGPS);
        return Created(vehicleGPS);
    }

    [HttpPost("odata/Trucks({key})/VehicleGPSGroup")]
    public IActionResult AddTruckVehicleGPSGroup([FromRoute] string key, [FromBody] VehicleGPSType vehicleGPSType)
    {
        var current = _dataSource.Trucks?.SingleOrDefault(t => t.Key == key);
        if(current == null)
        {
            return NotFound();
        }

        current.VehicleGPSGroup = current.VehicleGPSGroup ?? new Collection<VehicleGPSType>();
        current.VehicleGPSGroup.Add(vehicleGPSType);

        return Created(vehicleGPSType);
    }

    [HttpPost("odata/Trucks({key})/VehicleGPSGroupFromGPS")]
    public IActionResult AddTruckVehicleGPSGroupFromGPS([FromRoute] string key, [FromBody] VehicleGPSType vehicleGPSType)
    {
        var current = _dataSource.Trucks?.SingleOrDefault(t => t.Key == key);
        if (current == null)
        {
            return NotFound();
        }

        current.VehicleGPSGroupFromGPS = current.VehicleGPSGroupFromGPS ?? new Collection<VehicleGPSType>();
        current.VehicleGPSGroupFromGPS.Add(vehicleGPSType);

        return Created(vehicleGPSType);
    }

    [HttpPost("odata/ResetVehicleSpeedToValue")]
    public IActionResult ResetVehicleSpeedToValue([FromODataBody] double targetValue)
    {
        if(_dataSource.VehicleGPSSetInGPS == null)
        {
            return NoContent();
        }

        for(int i = 0; i < _dataSource.VehicleGPSSetInGPS.Count; i++)
        {
            _dataSource.VehicleGPSSetInGPS[i].VehicleSpeed = targetValue;
        }

        return Ok(_dataSource.VehicleGPSSetInGPS);
    }

    [HttpDelete("odata/VehicleGPSSetInGPS({key})")]
    public IActionResult RemoveVehicleGPSSetInGPS([FromRoute] string key)
    {
        var current = _dataSource.VehicleGPSSetInGPS?.SingleOrDefault(v => v.Key == key);
        if (current == null)
        {
            return NoContent();
        }

        _dataSource.VehicleGPSSetInGPS?.Remove(current);
        return NoContent();
    }

    [HttpDelete("odata/VehicleGPSSet({key})")]
    public IActionResult RemoveVehicleGPSType([FromRoute] string key)
    {
        var current = _dataSource.VehicleGPSSet?.SingleOrDefault(v => v.Key == key);
        if (current == null)
        {
            return NoContent();
        }

        _dataSource.VehicleGPSSet?.Remove(current);
        return NoContent();
    }

    [HttpDelete("odata/DerivedVehicleGPSSet({key})")]
    public IActionResult RemoveDerivedVehicleGPSType([FromRoute] string key)
    {
        var current = _dataSource.DerivedVehicleGPSSet?.SingleOrDefault(v => v.Key == key);
        if (current == null)
        {
            return NoContent();
        }

        _dataSource.DerivedVehicleGPSSet?.Remove(current);
        return NoContent();
    }

    [HttpPost("odata/odatamodelreftests/Default.ResetDefaultDataSource")]
    public IActionResult ResetDefaultDataSource()
    {
        _dataSource = ODataModelRefDataSource.CreateInstance();

        return Ok();
    }
}
