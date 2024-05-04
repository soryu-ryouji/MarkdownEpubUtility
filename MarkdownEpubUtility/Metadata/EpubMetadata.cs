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

    public string ToOpf()
    {
        var metadataList = new List<string>
        {
            Title != "" ? $"<dc:title>{Title}</dc:title>" : "<dc:title>EpubBuilder</dc:title>",
            Author != "" ? $"<dc:creator>{Author}</dc:creator>" : "<dc:creator>Anonymous</dc:creator>",
            Language != "" ? $"<dc:language>{Language}</dc:language>" : "<dc:language>zh</dc:language>",
            Generator != "" ? $"<dc:publisher>{Generator}</dc:publisher>" : "<dc:publisher>EpubBuilder</dc:publisher>"
        };

        if (Description != "") metadataList.Add($"<dc:description>{Description}</dc:description>");
        if (License != "") metadataList.Add($"<dc:rights>{License}</dc:rights>");
        if (PublishedDate != "") metadataList.Add($"<dc:date>{PublishedDate}</dc:date>");
        if (ModifiedDate != "") metadataList.Add($"<dc:date>{ModifiedDate}</dc:date>");
        if (Subject != "") metadataList.Add($"<dc:subject>{Subject}</dc:subject>");
        if (Uuid != "") metadataList.Add($"""<dc:identifier id="uuid_id" opf:scheme="uuid">{Uuid}</dc:identifier>""");

        return string.Join("\n", metadataList);
    }

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