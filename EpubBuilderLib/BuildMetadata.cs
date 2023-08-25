namespace EpubBuilder;

public class BuildMetadata
{
    public int PageSplitLevel { get; }

    public string CoverPath { get; }

    public string MdPath { get; }

    public BuildMetadata(string mdPath, string coverPath, int pageSplitLevel)
    {
        CoverPath = coverPath;
        MdPath = mdPath;
        PageSplitLevel = pageSplitLevel;
    }
}