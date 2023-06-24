namespace EpubBuilder;

public class BuildMetadata
{
    private readonly int _pageSplitLevel;
    private readonly string _coverPath;
    private readonly string _mdPath;

    public int PageSplitLevel
    {
        get { return _pageSplitLevel; }
    }

    public string CoverPath
    {
        get { return _coverPath; }
    }

    public string MdPath
    {
        get { return _mdPath; }
    }

    public BuildMetadata(string mdPath, string coverPath, int pageSplitLevel)
    {
        _coverPath = coverPath;
        _mdPath = mdPath;
        _pageSplitLevel = pageSplitLevel;
    }
}