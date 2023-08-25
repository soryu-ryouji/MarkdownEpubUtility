using Xunit.Abstractions;

namespace EpubBuilderLib.Test;

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
        var toc = new Toc();
        toc.AddElem(new TocElem("", "Page_1", 1));
        toc.AddElem(new TocElem("", "Page_2", 2));
        toc.AddElem(new TocElem("", "Page_3", 2));
        toc.AddElem(new TocElem("", "Page_4", 2));
        toc.AddElem(new TocElem("", "Page_5", 1));
        toc.AddElem(new TocElem("", "Page_6", 2));
        toc.AddElem(new TocElem("", "Page_7", 3));
        toc.AddElem(new TocElem("", "Page_8", 4));

        var tree = toc.ToString();

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
        
        Assert.Equal(tree,resultStr);
    }
}