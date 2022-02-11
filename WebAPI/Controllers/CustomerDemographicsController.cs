using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Handlers.CustomerDemographics.Commands;
using Business.Handlers.CustomerDemographics.Queries;
using Core.Utilities.Results;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    ///     CustomerDemographics If controller methods will not be Authorize, [AllowAnonymous] is used.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerDemographicsController : BaseApiController
    {
        /// <summary>
        ///     List CustomerDemographics
        /// </summary>
        /// <remarks>CustomerDemographics</remarks>
        /// <return>List CustomerDemographics</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<IEnumerable<CustomerDemographic>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetCustomerDemographicsQuery());
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     It brings the details according to its id.
        /// </summary>
        /// <remarks>CustomerDemographics</remarks>
        /// <return>CustomerDemographics List</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<CustomerDemographic>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await Mediator.Send(new GetCustomerDemographicQuery {Id = id});
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Add CustomerDemographic.
        /// </summary>
        /// <param name="createCustomerDemographic"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateCustomerDemographicCommand createCustomerDemographic)
        {
            var result = await Mediator.Send(createCustomerDemographic);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Update CustomerDemographic.
        /// </summary>
        /// <param name="updateCustomerDemographic"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCustomerDemographicCommand updateCustomerDemographic)
        {
            var result = await Mediator.Send(updateCustomerDemographic);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Delete CustomerDemographic.
        /// </summary>
        /// <param name="deleteCustomerDemographic"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteCustomerDemographicCommand deleteCustomerDemographic)
        {
            var result = await Mediator.Send(deleteCustomerDemographic);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}