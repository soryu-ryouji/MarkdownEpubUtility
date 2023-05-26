namespace EpubBuilder.Tests;
using NUnit.Framework;

using EpubBuilder.Core;

[TestFixture]
public class TestToc
{
    [Test]
    public void ChildrenIsEmpty_ReturnTrue_WhenChildrenIsZero()
    {
        Toc toc = new Toc();
        bool result = toc.ChildrenIsEmpty();

        Assert.IsTrue(result);
    }
    
    [Test]
    public void ChildrenIsEmpty_ReturnFalse_WhenChildrenIsNotZero()
    {
        Toc toc = new Toc();
        TocElement tocElement = new TocElement("114514", "1919810");
        toc.AddElem(tocElement);
        
        bool result = toc.ChildrenIsEmpty();
        
        Assert.IsFalse(result);
    }

    [Test]
    public void AddElem_WhenAddOneTocElem()
    {
        Toc toc = new Toc();
        TocElement tocElement = new TocElement("114514", "1919810");
        toc.AddElem(tocElement);

        var unit = toc.Elements.Last();
        bool result = unit == tocElement;
        
        Assert.IsTrue(result);
    }
    
    [Test]
    public void AddElem_WhenAddOneTocElemAndOneHisChildElem()
    {
        Toc toc = new Toc();
        TocElement tocElement = new TocElement("114514", "head_elem");
        TocElement childElem = new TocElement("child", "child_elem");
        childElem.Level = 2;
        toc.AddElem(tocElement);
        toc.AddElem(childElem);

        bool result = (toc.Elements.Last().Title == "head_elem") &&
                      (toc.Elements.Last().Children.Last().Title == "child_elem");
        
        Assert.IsTrue(result);
    }

    [Test]
    public void RenderToc_WhenHasOneElem()
    {
        Toc toc = new Toc();
        TocElement tocElement = new TocElement("114514", "head_elem");
        toc.AddElem(tocElement);

        string result = toc.RenderToc();

        string test = "<navPoint id=\"navPoint-1\">\n" +
                      "<navLabel><text>head_elem</text></navLabel>\n" +
                      "<content src=\"114514\"/>\n" +
                      "</navPoint>\n";
        bool isTrue = test == result;
        
        Assert.IsTrue(isTrue);
    }
    
    [Test]
    public void RenderToc_WhenHasTwoChildElem()
    {
        Toc toc = new Toc();
        TocElement tocElement = new TocElement("114514", "head_elem");
        TocElement childElem_1 = new TocElement("child_1", "child_elem_1");
        TocElement childElem_2 = new TocElement("child_2", "child_elem_2");
        childElem_1.Level = 2;
        childElem_2.Level = 2;
        toc.AddElem(tocElement);
        toc.AddElem(childElem_1);
        toc.AddElem(childElem_2);

        string result = toc.RenderToc();

        string test = "<navPoint id=\"navPoint-1\">\n" +
                      "<navLabel><text>head_elem</text></navLabel>\n" +
                      "<content src=\"114514\"/>\n" +
                      "<navPoint id=\"navPoint-2\">\n" +
                      "<navLabel><text>child_elem_1</text></navLabel>\n" +
                      "<content src=\"child_1\"/>\n" +
                      "</navPoint>\n" +
                      "<navPoint id=\"navPoint-3\">\n" +
                      "<navLabel><text>child_elem_2</text></navLabel>\n" +
                      "<content src=\"child_2\"/>\n" +
                      "</navPoint>\n" +
                      "</navPoint>\n";
        
        bool isTrue = test == result;
        
        Assert.IsTrue(isTrue);
    }
    [Test]
    public void RenderToc_WhenHasThreeChildElem()
    {
        Toc toc = new Toc();
        TocElement tocElement = new TocElement("114514", "head_elem");
        TocElement childElem_1 = new TocElement("child_1", "child_elem_1");
        TocElement childElem_2 = new TocElement("child_2", "child_elem_2");
        TocElement childElem_3 = new TocElement("child_3", "child_elem_3");
        childElem_1.Level = 2;
        childElem_2.Level = 3;
        childElem_3.Level = 2;
        toc.AddElem(tocElement);
        toc.AddElem(childElem_1);
        toc.AddElem(childElem_2);
        toc.AddElem(childElem_3);

        string result = toc.RenderToc();

        string test = "<navPoint id=\"navPoint-1\">\n" +
                      "<navLabel><text>head_elem</text></navLabel>\n" +
                      "<content src=\"114514\"/>\n" +
                      "<navPoint id=\"navPoint-2\">\n" +
                      "<navLabel><text>child_elem_1</text></navLabel>\n" +
                      "<content src=\"child_1\"/>\n" +
                      "<navPoint id=\"navPoint-3\">\n" +
                      "<navLabel><text>child_elem_2</text></navLabel>\n" +
                      "<content src=\"child_2\"/>\n" +
                      "</navPoint>\n" +
                      "</navPoint>\n" +
                      "<navPoint id=\"navPoint-4\">\n" +
                      "<navLabel><text>child_elem_3</text></navLabel>\n" +
                      "<content src=\"child_3\"/>\n" +
                      "</navPoint>\n" +
                      "</navPoint>\n";
        bool isTrue = test == result;
        
        Assert.IsTrue(isTrue);
    }

    [Test]
    public void AddTocElemFromPageElem()
    {
        throw new NotImplementedException();
    }
}
