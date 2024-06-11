//-----------------------------------------------------------------------------
// <copyright file="PeopleController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common;
using System.Collections.Concurrent;

namespace Microsoft.OData.Client.E2E.Tests.AsyncRequestTests.Server
{
    public class PeopleController : ODataController
    {
        private static ConcurrentDictionary<Guid, string> OperationStatus = new ConcurrentDictionary<Guid, string>();
        private static ConcurrentDictionary<Guid, List<Person>> OperationResults = new ConcurrentDictionary<Guid, List<Person>>();

        public PeopleController() { }

        [HttpGet]
        public IActionResult Get()
        {
            if (Request.Headers.TryGetValue("Prefer", out var preferHeader) && preferHeader.Contains("respond-async"))
            {
                var operationId = Guid.NewGuid();
                OperationStatus[operationId] = "InProgress";

                // Start the long-running operation asynchronously
                Task.Run(() => LongRunningOperation(operationId));

                var locationUri = Url.RouteUrl(
                    "GetOperationStatus",
                    new { operationId },
                    Request.Scheme,
                    Request.Host.ToUriComponent()
                );

                Response.Headers.Add("Location", locationUri);
                Response.Headers.Add("Retry-After", "5");
                return Accepted();
            }
            else
            {
                // Handle synchronous operation
                var people = DefaultDataSource.People;
                return Ok(people);
            }
        }

        [HttpGet]
        [Route("People/{operationId}", Name = "GetOperationStatus")]
        public IActionResult GetOperationStatus(Guid operationId)
        {
            if (!OperationStatus.TryGetValue(operationId, out var status))
            {
                return NotFound();
            }

            if (status == "Completed")
            {
                OperationStatus.TryRemove(operationId, out _);
                if (OperationResults.TryRemove(operationId, out var result))
                {
                    return Ok(result);
                }
                return Ok("Operation completed successfully.");
            }

            var locationUri = Url.RouteUrl(
                "GetOperationStatus",
                new { operationId },
                Request.Scheme,
                Request.Host.ToUriComponent());

            Response.Headers.Add("Location", locationUri);
            Response.Headers.Add("Retry-After", "5");
            return Accepted();
        }

        private async Task LongRunningOperation(Guid operationId)
        {
            // Simulate a long-running operation
            await Task.Delay(6000); // Delay for 5 seconds

            // Fetch the data (or perform the actual long-running task)
            var people = DefaultDataSource.People;

            // Store the result of the operation
            OperationResults[operationId] = (List<Person>)people;

            // Mark the operation as completed
            OperationStatus[operationId] = "Completed";
        }

    }
}
