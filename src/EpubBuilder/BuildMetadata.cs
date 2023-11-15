namespace EpubBuilder;

public class BuildMetadata(List<string> mdLines, string mdPath, string coverPath, int pageSplitLevel)
{
    public List<string> MdLines { get; set; } = mdLines ?? [];
    public string MdPath { get; } = mdPath;
    public string CoverPath { get; } = coverPath;
    public int PageSplitLevel { get; } = Math.Max(1, pageSplitLevel);

    public override string ToString()
    {
        return 
        $"""
        MdPath: {MdPath}
        CoverPath: {CoverPath}
        PageSplitLevel: {PageSplitLevel}
        """;
    }
}