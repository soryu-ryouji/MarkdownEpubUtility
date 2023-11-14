namespace EpubBuilder;

/// <summary>
/// Epub Metadata
/// Epub 电子书元数据
/// </summary>
public class EpubMetadata
{
    // 电子书标题
    public string Title;

    // 电子书作者
    public string Author;

    // 电子书语言
    public string Language;

    // 电子书发布者
    public string Generator;

    // 电子书描述
    public string Description;

    // 电子书主题
    public string Subject;

    // 电子书许可证
    public string License;

    // 电子书发布时间
    public string PublishedDate;

    // 电子书修改时间
    public string ModifiedDate;

    // 电子书唯一标识符
    public string Uuid;

    public EpubMetadata()
    {
        Title = "";
        Author = "";
        Language = "";
        Generator = "";
        Description = "";
        Subject = "";
        License = "";
        PublishedDate = "";
        ModifiedDate = "";
        Uuid = "";
    }

    public string GenerateOpfMetadata()
    {
        var metadataList = new List<string>
        {
            Title != "" ? $"<dc:title>{Title}</dc:title>" : "<dc:title>EpubBuilder</dc:title>",
            Author != "" ? $"<dc:creator>{Author}</dc:creator>" : "<dc:creator>Anonymous</dc:creator>",
            Language != "" ? $"<dc:language>{Language}</dc:language>" : "<dc:language>zh</dc:language>",
            Generator != "" ? $"<dc:publisher>{Generator}</dc:publisher>" : "<dc:publisher>EpubBuilder</dc:publisher>"
        };

        // Description Metadata
        if (Description != "") metadataList.Add($"<dc:description>{Description}</dc:description>");
        // License Metadata
        if (License != "") metadataList.Add($"<dc:rights>{License}</dc:rights>");
        // PublishedDate Metadata
        if (PublishedDate != "") metadataList.Add($"<dc:date>{PublishedDate}</dc:date>");
        // ModifiedDate Metadata
        if (ModifiedDate != "") metadataList.Add($"<dc:date>{ModifiedDate}</dc:date>");
        // Subject Metadata
        if (Subject != "") metadataList.Add($"<dc:subject>{Subject}</dc:subject>");
        // UUID Metadata
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