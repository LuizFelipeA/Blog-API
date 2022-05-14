using Ether.Outcomes;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

public class HomeController : ControllerBase
{
    protected IActionResult SuccessResponse<T>(int statusCode, T value, string message = null)
        => StatusCode(
            statusCode: statusCode,
            Outcomes.Success()
                    .WithValue(value: value)
                    .WithMessage(message: message));

    protected IActionResult SuccessResponse(int statusCode, string message)
        => StatusCode(
            statusCode: statusCode,
            Outcomes.Success()
                    .WithMessage(message: message));

    protected IActionResult FailureResponse(int statusCode, string message)
        => StatusCode(
            statusCode: statusCode,
            Outcomes.Failure()
                    .WithMessage(message: message)
                    .WithStatusCode(statusCode: statusCode));

    protected IActionResult FailureResponse(int statusCode, IEnumerable<string> messages)
        => StatusCode(
            statusCode: statusCode,
            Outcomes.Failure()
                    .WithMessage(messages: messages)
                    .WithStatusCode(statusCode: statusCode));
}