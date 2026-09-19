using BryllHonorPortfolio.Models;
using BryllHonorPortfolio.Services;
using Microsoft.AspNetCore.Mvc;

namespace BryllHonorPortfolio.Controllers;

public class ProjectController : Controller
{
    private readonly ProjectRepository _projects;
    private readonly CommentRepository _comments;

    public ProjectController(ProjectRepository projects, CommentRepository comments)
    {
        _projects = projects;
        _comments = comments;
    }

    [HttpGet("/")]
    public IActionResult Index()
    {
        var groups = _projects.GroupedByTerm();
        var total = _projects.All().Count;

        ViewData["Title"] = "Bryll Honor — Portfolio";
        return View(new HomeIndexViewModel(groups, total));
    }

    [HttpGet("/projects/{id}")]
    public IActionResult Show(string id)
    {
        var project = _projects.FindById(id);
        if (project is null) return NotFound();

        var all = _projects.All();
        var index = all.FindIndex(p => p.Id == project.Id);
        var prev = index > 0 ? all[index - 1] : null;
        var next = index < all.Count - 1 ? all[index + 1] : null;
        var comments = _comments.ForProject(project.Id);

        ViewData["Title"] = $"{project.Title} — Bryll Honor";
        return View(new ProjectShowViewModel(project, comments, prev, next));
    }
}
