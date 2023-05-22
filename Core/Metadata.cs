namespace EpubBuilder.Core;

/// <summary>
/// Epub Metadata
/// Epub 电子书元数据
/// </summary>
public class Metadata
{
    public string Title;
    public string Author;
    public string Lang;
    public string Generator;
    public string TocName;
    public List<string> Description;
    public List<string> Subject;
    public string License;
    public string PublishedDate;
    public string ModifiedDate;
    public string Uuid;
    
    public Metadata()
    {
        Title = "";
        Author = "";
        Lang = "";
        Generator = "Csharp Epub Builder library";
        TocName = "";
        Description = new List<string>();
        Subject = new List<string>();
        License = "";
        PublishedDate = "";
        ModifiedDate = "";
        Uuid = "";
    }
}