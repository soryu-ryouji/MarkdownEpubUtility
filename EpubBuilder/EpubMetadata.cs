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
    public List<string> Description;

    // 电子书主题
    public List<string> Subject;

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
        Generator = "Csharp Epub Builder library";
        Description = new List<string>();
        Subject = new List<string>();
        License = "";
        PublishedDate = "";
        ModifiedDate = "";
        Uuid = "";
    }

    public string GenerateOpfMetadata()
    {
        var metadataList = new List<string>();
        // 标题元数据
        if (Title != "") metadataList.Add($"<dc:title>{Title}</dc:title>");
        else metadataList.Add("<dc:title>EpubBuilder</dc:title>");

        // UUID元数据
        if (Uuid != "")
            metadataList.Add($"<dc:identifier id=\"uuid_id\" opf:scheme=\"uuid\">{Uuid}</dc:identifier>");
        else
            metadataList.Add($"<dc:identifier id=\"uuid_id\" opf:scheme=\"uuid\">{GenerateUuid()}</dc:identifier>");

        // 语言元数据
        if (Language != "")
            metadataList.Add($"<dc:language>{Language}</dc:language>");
        else metadataList.Add("<dc:language>zh</dc:language>");

        // 主题或关键字
        if (Subject.Count != 0)
            metadataList.Add($"<dc:subject>{String.Join(",", Subject)}</dc:subject>");

        return string.Join("\n", metadataList);
    }

    private string GenerateUuid()
    {
        return Guid.NewGuid().ToString();
    }
}