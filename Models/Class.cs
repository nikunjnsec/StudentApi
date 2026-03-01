using System.Text.Json.Serialization;

namespace StudentApi.Models;

public class Class
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    [JsonIgnore]
    public Subject? Subject { get; set; }
}
