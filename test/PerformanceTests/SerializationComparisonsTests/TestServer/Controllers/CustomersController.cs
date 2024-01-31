using System.Collections.Generic;
using ExperimentsLib;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestServer.Controllers
{
    [ApiController]
    public class CustomersController : ControllerBase
    {
        [HttpGet("customers/{writer}")]
        public IEnumerable<Customer> Get([FromQuery] int? count, [FromQuery] bool? largeFields)
        {
            var data = largeFields == true ?
                CustomerDataSet.GetDataWithLargeFields(count ?? 100)
                : CustomerDataSet.GetCustomers(count ?? 100);
            return data;
        }
    }
}
