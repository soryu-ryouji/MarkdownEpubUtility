using EpubBuilder.Core;
using NUnit.Framework;

[TestFixture]
public class TocElementTest
{
    [Test]
    public void ChildrenIsEmpty_ReturnTrue_WhenChildrenIsZero()
    {
        TocElement tocElem = new TocElement("114515", "1919810");
        bool result = tocElem.ChildrenIsEmpty();

        Assert.IsTrue(result);
    }

    [Test]
    public void ChildrenIsEmpty_ReturnFalse_WhenChildrenIsNotZero()
    {
        TocElement tocElement = new TocElement("114514", "1919810");
        TocElement newElem = new TocElement("233333", "5555555");
        tocElement.AddElem(newElem);
        
        bool result = tocElement.ChildrenIsEmpty();
        
        Assert.IsFalse(result);
    }

    [Test]
    public void UpLevel_ReturnTwo_WhenLevelIsTwo()
    {
        TocElement tocElement = new TocElement("114514", "1919810");
        tocElement.UpLevel(2);

        bool result = tocElement.Level == 2;

        Assert.IsTrue(result);
    }

    [Test]
    public void UpLevel_HasChild_WenLevelIsTwo()
    {
        TocElement tocElement = new TocElement("114514", "1919810");
        TocElement newElem = new TocElement("233333", "5555555");
        tocElement.AddChild(newElem);

        tocElement.UpLevel(2);

        List<TocElement> list = tocElement.Children;
        bool result = list.Last().Level == 3;

        Assert.IsTrue(result);
    }

    [Test]
    public void RenderToc_WhenHasOneElement()
    {
        TocElement tocElement = new TocElement("url:test", "test_one");

        (int, string) result = tocElement.RenderToc(0);
        
        string test = "<navPoint id=\"navPoint-1\">\n" +
                      "<navLabel><text>test_one</text></navLabel>\n" +
                      "<content src=\"url:test\"/>\n" +
                      "</navPoint>\n";
        bool isTrue = test == result.Item2;
        Assert.IsTrue(isTrue);
    }

    [Test]
    public void RenderToc_WhenHasOneTocElementAndOneChild()
    {
        TocElement tocElement = new TocElement("url:test", "test_one");
        TocElement newElem = new TocElement("url:test_1", "test_two");
        tocElement.AddChild(newElem);

        (int, string) result = tocElement.RenderToc(0);
        
        string test = "<navPoint id=\"navPoint-1\">\n" +
                      "<navLabel><text>test_one</text></navLabel>\n" +
                      "<content src=\"url:test\"/>\n" +
                      "<navPoint id=\"navPoint-2\">\n" +
                      "<navLabel><text>test_two</text></navLabel>\n" +
                      "<content src=\"url:test_1\"/>\n" +
                      "</navPoint>\n" +
                      "</navPoint>\n";
        bool isTrue = test == result.Item2;
        Assert.IsTrue(isTrue);
    }

    [Test]
    public void RenderToc_WhenHasOneTocElementAndThreeChild()
    {
        TocElement tocElement_1 = new TocElement("url:test", "toc_one");
        TocElement tocElement_2 = new TocElement("url:test", "toc_two");
        TocElement childElem_1 = new TocElement("url:test_1", "child_one");
        TocElement childElem_2 = new TocElement("url:test_1", "child_two");
        
        tocElement_1.AddChild((childElem_1));
        tocElement_1.AddElem(childElem_2);
        tocElement_1.AddElem(tocElement_2);

        (int, string) result = tocElement_1.RenderToc(0);
        string test = "<navPoint id=\"navPoint-1\">\n" +
                      "<navLabel><text>toc_one</text></navLabel>\n" +
                      "<content src=\"url:test\"/>\n" +
                      "<navPoint id=\"navPoint-2\">\n" +
                      "<navLabel><text>child_one</text></navLabel>\n" +
                      "<content src=\"url:test_1\"/>\n" +
                      "</navPoint>\n" +
                      "<navPoint id=\"navPoint-3\">\n" +
                      "<navLabel><text>child_two</text></navLabel>\n" +
                      "<content src=\"url:test_1\"/>\n" +
                      "</navPoint>\n" +
                      "<navPoint id=\"navPoint-4\">\n" +
                      "<navLabel><text>toc_two</text></navLabel>\n" +
                      "<content src=\"url:test\"/>\n" +
                      "</navPoint>\n" +
                      "</navPoint>\n";

        bool isTrue = test == result.Item2;

        Assert.IsTrue(isTrue);
    }

    [Test]
    public void RenderToc_WhenHasTwoTocElementAndTwoChild()
    {
        TocElement tocElement = new TocElement("url:test", "toctoc");
        TocElement tocElement_1 = new TocElement("url:test", "toc_one");
        TocElement tocElement_2 = new TocElement("url:test", "toc_two");
        TocElement childElem_1 = new TocElement("url:test_1", "child_one");
        TocElement childElem_2 = new TocElement("url:test_1", "child_two");
        tocElement_1.AddChild(childElem_1);
        tocElement_2.AddElem(childElem_2);
        
        tocElement.AddElem(tocElement_1);
        tocElement.AddElem(tocElement_2);

        (int, string) result = tocElement.RenderToc(0);

        string test = "<navPoint id=\"navPoint-1\">\n" +
                      "<navLabel><text>toctoc</text></navLabel>\n" +
                      "<content src=\"url:test\"/>\n" +
                      "<navPoint id=\"navPoint-2\">\n" +
                      "<navLabel><text>toc_one</text></navLabel>\n" +
                      "<content src=\"url:test\"/>\n" +
                      "<navPoint id=\"navPoint-3\">\n" +
                      "<navLabel><text>child_one</text></navLabel>\n" +
                      "<content src=\"url:test_1\"/>\n" +
                      "</navPoint>\n" +
                      "</navPoint>\n" +
                      "<navPoint id=\"navPoint-4\">\n" +
                      "<navLabel><text>toc_two</text></navLabel>\n" +
                      "<content src=\"url:test\"/>\n" +
                      "<navPoint id=\"navPoint-5\">\n" +
                      "<navLabel><text>child_two</text></navLabel>\n" +
                      "<content src=\"url:test_1\"/>\n" +
                      "</navPoint>\n" +
                      "</navPoint>\n" +
                      "</navPoint>\n";

        bool isTrue = test == result.Item2;
        Assert.IsTrue(isTrue);
    }
}