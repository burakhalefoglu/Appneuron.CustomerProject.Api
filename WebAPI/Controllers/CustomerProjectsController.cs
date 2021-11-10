using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Handlers.CustomerProjects.Commands;
using Business.Handlers.CustomerProjects.Queries;
using Core.Utilities.Results;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    ///     CustomerProjects If controller methods will not be Authorize, [AllowAnonymous] is used.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerProjectsController : BaseApiController
    {
        /// <summary>
        ///     List CustomerProjects
        /// </summary>
        /// <remarks>CustomerProjects</remarks>
        /// <return>List CustomerProjects</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<IEnumerable<CustomerProject>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetCustomerProjectsQuery());
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     List CustomerProjects
        /// </summary>
        /// <remarks>CustomerProjects</remarks>
        /// <return>List CustomerProjects</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<IEnumerable<CustomerProject>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getbyuser")]
        public async Task<IActionResult> GetUsersList()
        {
            var result = await Mediator.Send(new GetCustomerProjectLookupQuery());
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     It brings the details according to its id.
        /// </summary>
        /// <remarks>CustomerProjects</remarks>
        /// <return>CustomerProjects List</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<CustomerProject>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetByProjectKey(string id)
        {
            var result = await Mediator.Send(new GetCustomerProjectQuery { ProjectKey = id });
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Add CustomerProject.
        /// </summary>
        /// <param name="createCustomerProject"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateCustomerProjectCommand createCustomerProject)
        {
            var result = await Mediator.Send(createCustomerProject);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Update CustomerProject.
        /// </summary>
        /// <param name="updateCustomerProject"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCustomerProjectCommand updateCustomerProject)
        {
            var result = await Mediator.Send(updateCustomerProject);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Delete CustomerProject.
        /// </summary>
        /// <param name="deleteCustomerProject"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteCustomerProjectCommand deleteCustomerProject)
        {
            var result = await Mediator.Send(deleteCustomerProject);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}