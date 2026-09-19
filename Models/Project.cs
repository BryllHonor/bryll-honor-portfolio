namespace BryllHonorPortfolio.Models;

public class Project
{
    public string Id { get; set; } = "";
    public string Term { get; set; } = "";
    public string Code { get; set; } = "";
    public string Title { get; set; } = "";
    public string Repo { get; set; } = "";
    public string GithubUrl { get; set; } = "";
    public string Description { get; set; } = "";
    public List<string> Stack { get; set; } = new();
    public string Thumbnail { get; set; } = "";
}

public record ProjectGroup(string Term, List<Project> Projects);
