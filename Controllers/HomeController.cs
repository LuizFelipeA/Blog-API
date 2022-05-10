using Ether.Outcomes;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

[ApiController]
[Route("api/")]
public class HomeController : ControllerBase
{
    [HttpGet("health-check")]
    public IActionResult Get()
    {
        return Ok();
    }

    public IActionResult SuccessResponse<T>(int statusCode, T value, string? message = null)
        => StatusCode(
            statusCode: statusCode,
            Outcomes.Success()
                    .WithValue(value: value)
                    .WithMessage(message: message));

    public IActionResult SuccessResponse(int statusCode, string message)
        => StatusCode(
            statusCode: statusCode,
            Outcomes.Success()
                    .WithMessage(message: message));

    public IActionResult FailureResponse(int statusCode, string message)
        => StatusCode(
            statusCode: statusCode,
            Outcomes.Failure()
                    .WithMessage(message: message)
                    .WithStatusCode(statusCode: statusCode));

    public IActionResult FailureResponse(int statusCode, IEnumerable<string> messages)
        => StatusCode(
            statusCode: statusCode,
            Outcomes.Failure()
                    .WithMessage(messages: messages)
                    .WithStatusCode(statusCode: statusCode));
}