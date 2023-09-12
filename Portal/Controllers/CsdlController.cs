namespace Portal.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

    [ApiController]
    [Route("csdls")]
    public sealed class CsdlController : ControllerBase
    {
        private const string hardCodedCsdl = "hard coded csdl; change this with whatever you are testing for";

        [HttpPut]
        public IActionResult Put([ValidateNever] [FromBody] string data)
        {
            var location = "https://" + HttpContext.Request.Host + "/csdls/for_saketh";
            Response.Headers.Add("Location", location);
            return Ok();
        }

        [HttpGet]
        [Route("for_saketh")]
        public IActionResult Get()
        {
            return Ok(hardCodedCsdl);
        }
    }
}
