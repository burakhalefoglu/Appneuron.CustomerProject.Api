﻿using Business.Handlers.Logs.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Utilities.Results;

namespace WebAPI.Controllers
{
    /// <summary>
    /// If controller methods will not be Authorize, [AllowAnonymous] is used.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : BaseApiController
    {
        ///<summary>
        ///List Logs
        ///</summary>
        ///<remarks>bla bla bla Logs</remarks>
        ///<return>Logs List</return>
        ///<response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetLogDtoQuery());
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}