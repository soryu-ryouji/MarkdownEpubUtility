using Xunit.Abstractions;

namespace EpubBuilderLib.Test;

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
        var htmlPage = new HtmlPages();
        // htmlPage.AddElem(new PageElem(1, "Page_1"),2);
        // htmlPage.AddElem(new PageElem(2, "Page_2"),2);
        // htmlPage.AddElem(new PageElem(2, "Page_3"),2);
        // htmlPage.AddElem(new PageElem(2, "Page_4"),2);
        // htmlPage.AddElem(new PageElem(3, "Page_5"),2);
        // htmlPage.AddElem(new PageElem(4, "Page_6"),2);
        // htmlPage.AddElem(new PageElem(5, "Page_7"),2);
        // htmlPage.AddElem(new PageElem(1, "Page_7"),2);

        var tree = htmlPage.ToString();

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
        
        // Assert.Equal(tree,resultStr);
    }
}