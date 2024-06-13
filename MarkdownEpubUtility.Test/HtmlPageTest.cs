using Xunit.Abstractions;

namespace MarkdownEpubUtility.Test;

public class HtmlPageTest
{
    private readonly ITestOutputHelper _output;

    public HtmlPageTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void HtmlPageToString_Test()
    {
        var htmlPage = new EpubPage
        {
            { new EpubPageItem("test_url", 1, "Page_1"), 2 },
            { new EpubPageItem("test_url", 2, "Page_2"), 2 },
            { new EpubPageItem("test_url", 2, "Page_3"), 2 },
            { new EpubPageItem("test_url", 2, "Page_4"), 2 },
            { new EpubPageItem("test_url", 3, "Page_5"), 2 },
            { new EpubPageItem("test_url", 4, "Page_6"), 2 },
            { new EpubPageItem("test_url", 5, "Page_7"), 2 },
            { new EpubPageItem("test_url", 1, "Page_7"), 2 }
        };

        var tree = htmlPage.ToTree();

        var resultStr =
            """
            Page_1
            │   ├─ Page_2
            │   ├─ Page_3
            │   └─ Page_4
            │       ├─ Page_5
            │       ├─ Page_6
            │       └─ Page_7
            Page_7

            """;
        Assert.Equal(tree.Split(Environment.NewLine), resultStr.Split(Environment.NewLine));
    }
}