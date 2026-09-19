using System.Text.Json;
using System.Text.RegularExpressions;
using BryllHonorPortfolio.Models;

namespace BryllHonorPortfolio.Services;

public class CommentRepository
{
    private const int MaxAuthorLen = 40;
    private const int MaxBodyLen = 600;

    private readonly string _dataPath;
    private static readonly object FileLock = new();
    private static readonly JsonSerializerOptions ReadOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public CommentRepository(IWebHostEnvironment env)
    {
        _dataPath = Path.Combine(env.ContentRootPath, "Data", "comments.json");
    }

    public List<Comment> ForProject(string projectId)
    {
        var all = ReadAll();
        return all.TryGetValue(projectId, out var comments) ? comments : new List<Comment>();
    }

    public Comment Create(string projectId, string author, string? body)
    {
        var cleanAuthor = Sanitize(author, MaxAuthorLen);
        if (string.IsNullOrEmpty(cleanAuthor)) cleanAuthor = "Anonymous";
        var cleanBody = Sanitize(body, MaxBodyLen);

        if (string.IsNullOrEmpty(cleanBody))
            throw new ArgumentException("Comment text is required.");

        lock (FileLock)
        {
            var all = ReadAll();
            if (!all.TryGetValue(projectId, out var list))
            {
                list = new List<Comment>();
                all[projectId] = list;
            }

            var comment = new Comment
            {
                Id = Guid.NewGuid().ToString(),
                Author = cleanAuthor,
                Body = cleanBody,
                CreatedAt = DateTime.UtcNow
            };

            list.Add(comment);
            WriteAll(all);
            return comment;
        }
    }

    public bool Delete(string projectId, string commentId)
    {
        lock (FileLock)
        {
            var all = ReadAll();
            if (!all.TryGetValue(projectId, out var list)) return false;

            var before = list.Count;
            all[projectId] = list.Where(c => c.Id != commentId).ToList();
            WriteAll(all);
            return all[projectId].Count < before;
        }
    }

    private Dictionary<string, List<Comment>> ReadAll()
    {
        lock (FileLock)
        {
            if (!File.Exists(_dataPath)) return new Dictionary<string, List<Comment>>();

            var raw = File.ReadAllText(_dataPath);
            if (string.IsNullOrWhiteSpace(raw)) return new Dictionary<string, List<Comment>>();

            return JsonSerializer.Deserialize<Dictionary<string, List<Comment>>>(raw, ReadOptions)
                   ?? new Dictionary<string, List<Comment>>();
        }
    }

    private void WriteAll(Dictionary<string, List<Comment>> data)
    {

        var tmpPath = _dataPath + ".tmp";
        File.WriteAllText(tmpPath, JsonSerializer.Serialize(data, WriteOptions));
        File.Move(tmpPath, _dataPath, overwrite: true);
    }

    private static string Sanitize(string? value, int maxLen)
    {
        if (string.IsNullOrEmpty(value)) return "";
        var cleaned = Regex.Replace(value, @"[\u0000-\u001F\u007F]", "");
        cleaned = cleaned.Trim();
        return cleaned.Length > maxLen ? cleaned[..maxLen] : cleaned;
    }
}
