//-----------------------------------------------------------------------------
// <copyright file="AsyncRequestTestsController.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Client.E2E.Tests.Common.Server;

namespace Microsoft.OData.Client.E2E.Tests.AsyncRequestTests.Server
{
    public class AsyncRequestTestsController : ODataController
    {
        private static ConcurrentDictionary<Guid, string> OperationStatus = new ConcurrentDictionary<Guid, string>();
        private static ConcurrentDictionary<Guid, List<Person>> OperationResults = new ConcurrentDictionary<Guid, List<Person>>();

        public AsyncRequestTestsController() { }

        [EnableQuery]
        [HttpGet("odata/People")]
        public IActionResult Get()
        {
            if (Request.Headers.TryGetValue("Prefer", out var preferHeader) && preferHeader.Contains("respond-async"))
            {
                var operationId = Guid.NewGuid();
                OperationStatus[operationId] = "InProgress";

                // Start the long-running operation asynchronously
                Task.Run(() => LongRunningOperation(operationId));

                // Construct the Location header URI with $async and $asyncToken
                var asyncToken = Guid.NewGuid(); // Or use any method to generate the asyncToken
                var locationUri = $"{Request.Scheme}://{Request.Host}/People{operationId}/$async?$asyncToken={asyncToken}";

                Response.Headers.Add("Location", locationUri);
                Response.Headers.Add("Retry-After", "5");
                return Accepted();
            }
            else
            {
                // Handle synchronous operation
                // Response.Headers.Add("Content-Type", "application/http");
                var people = DefaultDataSource.People;
                return Ok(people);
            }
        }

        [EnableQuery]
        [HttpGet]
        [Route("People{operationId}/$async")]
        public IActionResult GetOperationStatus(Guid operationId, [FromQuery] Guid asyncToken)
        {
            if (!OperationStatus.TryGetValue(operationId, out var status))
            {
                return NotFound();
            }

            var q = Request.Query["$asyncToken"];

            var locationUri = $"{Request.Scheme}://{Request.Host}/People{operationId}/$async?$asyncToken={q}";

            if (status == "Completed")
            {
                OperationStatus.TryRemove(operationId, out _);
                if (OperationResults.TryRemove(operationId, out var result))
                {
                    Response.Headers.Add("Content-Type", "application/http");
                    return Ok(result);
                }
            }

            Response.Headers.Add("Location", locationUri);
            Response.Headers.Add("Retry-After", "5");
            return Accepted();
        }

        private async Task LongRunningOperation(Guid operationId)
        {
            // Simulate a long-running operation
            await Task.Delay(6000); // Delay for 6 seconds

            // Fetch the data (or perform the actual long-running task)
            var people = DefaultDataSource.People.ToList();

            // Store the result of the operation
            OperationResults[operationId] = people;

            // Mark the operation as completed
            OperationStatus[operationId] = "Completed";
        }
    }
}
