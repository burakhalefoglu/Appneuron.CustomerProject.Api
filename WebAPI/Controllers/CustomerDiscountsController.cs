using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Handlers.CustomerDiscounts.Commands;
using Business.Handlers.CustomerDiscounts.Queries;
using Core.Utilities.Results;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    ///     CustomerDiscounts If controller methods will not be Authorize, [AllowAnonymous] is used.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerDiscountsController : BaseApiController
    {
        /// <summary>
        ///     List CustomerDiscounts
        /// </summary>
        /// <remarks>CustomerDiscounts</remarks>
        /// <return>List CustomerDiscounts</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<IEnumerable<CustomerDiscount>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetCustomerDiscountsQuery());
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     It brings the details according to its id.
        /// </summary>
        /// <remarks>CustomerDiscounts</remarks>
        /// <return>CustomerDiscounts List</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<CustomerDiscount>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetCustomerDiscountQuery());
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Add CustomerDiscount.
        /// </summary>
        /// <param name="createCustomerDiscount"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateCustomerDiscountCommand createCustomerDiscount)
        {
            var result = await Mediator.Send(createCustomerDiscount);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Update CustomerDiscount.
        /// </summary>
        /// <param name="updateCustomerDiscount"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCustomerDiscountCommand updateCustomerDiscount)
        {
            var result = await Mediator.Send(updateCustomerDiscount);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Delete CustomerDiscount.
        /// </summary>
        /// <param name="deleteCustomerDiscount"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteCustomerDiscountCommand deleteCustomerDiscount)
        {
            var result = await Mediator.Send(deleteCustomerDiscount);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}