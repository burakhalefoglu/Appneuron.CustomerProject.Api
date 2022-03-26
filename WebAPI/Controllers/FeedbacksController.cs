using Business.Handlers.Feedbacks.Commands;
using Microsoft.AspNetCore.Mvc;
using IResult = Core.Utilities.Results.IResult;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FeedbacksController : BaseApiController
{
    /// <summary>
    ///     Add Rate.
    /// </summary>
    /// <returns></returns>
    [Produces("application/json", "text/plain")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateFeedbackCommand createFeedbackCommand)
    {
        var result = await Mediator.Send(createFeedbackCommand);
        if (result.Success) return Ok(result);
        return BadRequest(result);
    }
}