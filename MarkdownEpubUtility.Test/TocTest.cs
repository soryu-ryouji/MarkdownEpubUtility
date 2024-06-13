using Xunit.Abstractions;

namespace MarkdownEpubUtility.Test;

public class TocTest
{
    private readonly ITestOutputHelper _output;

    public TocTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void TocToString_Test()
    {
        var toc = new EpubToc
        {
            new EpubTocItem("", "Page_1", 1),
            new EpubTocItem("", "Page_2", 2),
            new EpubTocItem("", "Page_3", 2),
            new EpubTocItem("", "Page_4", 2),
            new EpubTocItem("", "Page_5", 1),
            new EpubTocItem("", "Page_6", 2),
            new EpubTocItem("", "Page_7", 3),
            new EpubTocItem("", "Page_8", 4)
        };

        string tree = toc.ToTree();

        var resultStr =
            """
            Page_1
            │   ├─ Page_2
            │   ├─ Page_3
            │   └─ Page_4
            Page_5
                └─ Page_6
                    └─ Page_7
                        └─ Page_8

            """;

        Assert.Equal(tree, resultStr);
    }
}