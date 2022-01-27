using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Handlers.AppneuronProducts.Commands;
using Business.Handlers.AppneuronProducts.Queries;
using Core.Utilities.Results;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    ///     AppneuronProducts If controller methods will not be Authorize, [AllowAnonymous] is used.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AppneuronProductsController : BaseApiController
    {
        /// <summary>
        ///     List AppneuronProducts
        /// </summary>
        /// <remarks>AppneuronProducts</remarks>
        /// <return>List AppneuronProducts</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<IEnumerable<AppneuronProduct>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetAppneuronProductsQuery());
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     It brings the details according to its id.
        /// </summary>
        /// <remarks>AppneuronProducts</remarks>
        /// <return>AppneuronProducts List</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<AppneuronProduct>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await Mediator.Send(new GetAppneuronProductQuery {Id = id});
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Add AppneuronProduct.
        /// </summary>
        /// <param name="createAppneuronProduct"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateAppneuronProductCommand createAppneuronProduct)
        {
            var result = await Mediator.Send(createAppneuronProduct);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Update AppneuronProduct.
        /// </summary>
        /// <param name="updateAppneuronProduct"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateAppneuronProductCommand updateAppneuronProduct)
        {
            var result = await Mediator.Send(updateAppneuronProduct);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Delete AppneuronProduct.
        /// </summary>
        /// <param name="deleteAppneuronProduct"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteAppneuronProductCommand deleteAppneuronProduct)
        {
            var result = await Mediator.Send(deleteAppneuronProduct);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}