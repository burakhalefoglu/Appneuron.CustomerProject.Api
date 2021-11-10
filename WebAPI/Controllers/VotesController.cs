using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Handlers.Votes.Commands;
using Business.Handlers.Votes.Queries;
using Core.Utilities.Results;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    ///     Votes If controller methods will not be Authorize, [AllowAnonymous] is used.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class VotesController : BaseApiController
    {
        /// <summary>
        ///     List Votes
        /// </summary>
        /// <remarks>Votes</remarks>
        /// <return>List Votes</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<IEnumerable<Vote>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetVotesQuery());
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     It brings the details according to its id.
        /// </summary>
        /// <remarks>Votes</remarks>
        /// <return>Votes List</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<Vote>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById(short id)
        {
            var result = await Mediator.Send(new GetVoteQuery { Id = id });
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Add Vote.
        /// </summary>
        /// <param name="createVote"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateVoteCommand createVote)
        {
            var result = await Mediator.Send(createVote);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Update Vote.
        /// </summary>
        /// <param name="updateVote"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateVoteCommand updateVote)
        {
            var result = await Mediator.Send(updateVote);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Delete Vote.
        /// </summary>
        /// <param name="deleteVote"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteVoteCommand deleteVote)
        {
            var result = await Mediator.Send(deleteVote);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}