namespace EpubBuilder;

public class BuildMetadata
{
    public List<string> MdLines { get; set;}
    public string MdPath { get; }
    public string CoverPath { get; }
    public int PageSplitLevel { get; }

    public BuildMetadata(List<string> mdLines, string mdPath, string coverPath, int pageSplitLevel)
    {
        MdLines = mdLines ?? new();
        MdPath = mdPath;
        CoverPath = coverPath;
        PageSplitLevel = Math.Max(1, pageSplitLevel);
    }

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