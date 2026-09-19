using BryllHonorPortfolio.Models;
using BryllHonorPortfolio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BryllHonorPortfolio.Controllers;

[Authorize] // equivalent to the requireLogin middleware on both routes
public class CommentController : Controller
{
    private readonly ProjectRepository _projects;
    private readonly CommentRepository _comments;

    public CommentController(ProjectRepository projects, CommentRepository comments)
    {
        _projects = projects;
        _comments = comments;
    }

    [HttpPost("/projects/{id}/comments")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(string id, CommentFormModel model)
    {
        var project = _projects.FindById(id);
        if (project is null) return NotFound();

        try
        {
            _comments.Create(project.Id, User.Identity!.Name!, model.Body);
            TempData["FlashSuccess"] = "Comment posted.";
        }
        catch (ArgumentException ex)
        {
            TempData["FlashError"] = ex.Message;
        }

        return Redirect($"/projects/{project.Id}#comments");
    }

    [HttpPost("/projects/{id}/comments/{commentId}/delete")]
    [ValidateAntiForgeryToken]
    public IActionResult Destroy(string id, string commentId)
    {
        var project = _projects.FindById(id);
        if (project is null) return NotFound();

        var existing = _comments.ForProject(project.Id).FirstOrDefault(c => c.Id == commentId);

        if (existing is null || existing.Author != User.Identity!.Name)
        {
            TempData["FlashError"] = "You can only delete your own comments.";
            return Redirect($"/projects/{project.Id}#comments");
        }

        _comments.Delete(project.Id, commentId);
        TempData["FlashSuccess"] = "Comment deleted.";
        return Redirect($"/projects/{project.Id}#comments");
    }
}
