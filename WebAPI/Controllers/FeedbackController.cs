using Business.Handlers.Feedbacks.Commands;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FeedbackController : BaseApiController
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