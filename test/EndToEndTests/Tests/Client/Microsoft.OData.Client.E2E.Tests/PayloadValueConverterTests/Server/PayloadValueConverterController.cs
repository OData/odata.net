//-----------------------------------------------------------------------------
// <copyright file="PayloadValueConverterController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Microsoft.OData.Client.E2E.Tests.PayloadValueConverterTests.Server
{
    public class PayloadValueConverterController : ODataController
    {
        private static PayloadValueConverterDataSource _dataSource;

        [EnableQuery]
        [HttpGet("odata/People")]
        public IActionResult Get()
        {
            var people = _dataSource.People;

            return Ok(people);
        }

        [EnableQuery]
        [HttpGet("odata/People({key})")]
        public IActionResult Get(int key)
        {
            var person = _dataSource.People.FirstOrDefault(a => a.Id == key);

            if (person == null)
            {
                return NotFound();
            }

            return Ok(person);
        }

        [EnableQuery]
        [HttpGet("odata/People({key})/Picture")]
        public IActionResult GetPersonHomeAddress(int key)
        {
            var person = _dataSource.People.FirstOrDefault(a => a.Id == key);

            if (person == null)
            {
                return NotFound();
            }

            var personPicture = person.Picture;

            return Ok(personPicture);
        }

        [HttpPost("odata/People")]
        public IActionResult Post([FromBody] Person person)
        {
            _dataSource.People.Add(person);

            return Created(person);
        }


        [HttpPost("odata/payloadvalueconverter/Default.ResetDataSource")]
        public IActionResult ResetDataSource()
        {
            _dataSource = PayloadValueConverterDataSource.CreateInstance();

            return Ok();
        }
    }
}
