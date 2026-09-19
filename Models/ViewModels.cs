namespace BryllHonorPortfolio.Models;

public record HomeIndexViewModel(List<ProjectGroup> Groups, int Total);

public record ProjectShowViewModel(Project Project, List<Comment> Comments, Project? Prev, Project? Next);

public record ErrorViewModel(int Status, string Title, string Message);
