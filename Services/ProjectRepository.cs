using System.Text.Json;
using BryllHonorPortfolio.Models;

namespace BryllHonorPortfolio.Services;

public class ProjectRepository
{
    private readonly string _dataPath;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ProjectRepository(IWebHostEnvironment env)
    {
        _dataPath = Path.Combine(env.ContentRootPath, "Data", "projects.json");
    }

    public List<Project> All()
    {
        var raw = File.ReadAllText(_dataPath);
        return JsonSerializer.Deserialize<List<Project>>(raw, JsonOptions) ?? new List<Project>();
    }

    public Project? FindById(string id) => All().FirstOrDefault(p => p.Id == id);

    public List<ProjectGroup> GroupedByTerm()
    {
        var projects = All();
        var order = new List<string>();
        var groups = new Dictionary<string, List<Project>>();

        foreach (var p in projects)
        {
            if (!groups.TryGetValue(p.Term, out var list))
            {
                list = new List<Project>();
                groups[p.Term] = list;
                order.Add(p.Term);
            }
            list.Add(p);
        }

        return order.Select(term => new ProjectGroup(term, groups[term])).ToList();
    }
}
