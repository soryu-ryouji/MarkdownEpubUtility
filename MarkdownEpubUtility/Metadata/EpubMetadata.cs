namespace MarkdownEpubUtility;

public class EpubMetadata
{
    public string Title = "";
    public string Author = "";
    public string Language = "";
    public string Generator = "";
    public string Description = "";
    public string Subject = "";
    public string License = "";
    public string PublishedDate = "";
    public string ModifiedDate = "";
    public string Uuid = "";

    public override string ToString()
    {
        return
        $"""
        Title: {Title}
        Author: {Author}
        Language: {Language}
        Generator: {Generator}
        Description: {Description}
        Subject: {Subject}
        License: {License}
        PublishedDate: {PublishedDate}
        ModifiedDate: {ModifiedDate}
        Uuid: {Uuid}
        """;
    }
}