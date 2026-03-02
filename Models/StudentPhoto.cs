namespace StudentApi.Models;

public class StudentPhoto
{
    public int StudentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public byte[] PhotoData { get; set; } = Array.Empty<byte>();
}
