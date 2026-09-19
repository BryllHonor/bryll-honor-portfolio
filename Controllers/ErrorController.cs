using BryllHonorPortfolio.Models;
using Microsoft.AspNetCore.Mvc;

namespace BryllHonorPortfolio.Controllers;

[Route("error")]
public class ErrorController : Controller
{
    [Route("{code:int?}")]
    public IActionResult Index(int? code)
    {
        var status = code ?? 500;
        Response.StatusCode = status;

        var (title, message) = status switch
        {
            404 => ("Page not found", "That page doesn't exist."),
            403 => ("Error", "Your session expired or the form was resubmitted. Please try again."),
            400 => ("Error", "That request could not be processed."),
            _ => ("Something went wrong", "Something went wrong on our end. Please try again.")
        };

        return View("Error", new ErrorViewModel(status, title, message));
    }
}
