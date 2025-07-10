//---------------------------------------------------------------------
// <copyright file="AnnotationSerializationController.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server.Models;

namespace Microsoft.OData.Client.E2E.Tests.AnnotationSerializationTests.Server
{
    public class OrdersController : ODataController
    {
        [EnableQuery]
        public ActionResult<IEnumerable<Order>> Get()
        {
            return Ok(AnnotationSerializationDataSource.Orders);
        }

        [EnableQuery]
        public ActionResult<Order> Get(int key)
        {
            var order = AnnotationSerializationDataSource.Orders.FirstOrDefault(o => o.Id == key);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        public ActionResult Post([FromBody] Order order)
        {
            if (order == null)
            {
                return BadRequest("Order cannot be null.");
            }

            AnnotationSerializationDataSource.Orders.Add(order);
            return Created(order);
        }
    }
}
