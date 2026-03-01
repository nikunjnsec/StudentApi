using System.Text.Json.Serialization;

namespace StudentApi.Models;

public class Subject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ClassId { get; set; }

    [JsonIgnore]
    public Class? Class { get; set; }
}
