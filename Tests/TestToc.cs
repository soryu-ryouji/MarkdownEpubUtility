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

        var unit = toc.TocElemList.Last();
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

        bool result = (toc.TocElemList.Last().Title == "head_elem") &&
                      (toc.TocElemList.Last().Children.Last().Title == "child_elem");
        
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
        // PageList Struct:
        // page1
        // |—— page2
        // |—— page3
        // |   |—— page4
        // |   |—— page5
        // |—— page6
        PageList pageList = new PageList();
        PageElement page1 = new PageElement(1,"page1");
        PageElement page2 = new PageElement(2,"page2");
        PageElement page3 = new PageElement(2,"page3");
        PageElement page4 = new PageElement(3,"page4");
        PageElement page5 = new PageElement(3,"page5");
        PageElement page6 = new PageElement(2,"page6");

        pageList.AddPageElem(page1,2);
        pageList.AddPageElem(page2,2);
        pageList.AddPageElem(page3,2);
        pageList.AddPageElem(page4,2);
        pageList.AddPageElem(page5,2);
        pageList.AddPageElem(page6,2);

        Toc toc = new Toc();

        toc.AddTocElemFromPageElem(pageList.PageElemList[0],0,2);

        // Toc Struct:
        // page1
        // |—— page2
        // |—— page3
        // |   |—— page4
        // |   |—— page5
        // |—— page6
        bool isTrue = toc.TocElemList[0].Title == "page1" &&
                      toc.TocElemList[0].Children[0].Title == "page2" &&
                      toc.TocElemList[0].Children[1].Title == "page3" &&
                      toc.TocElemList[0].Children[1].Children[0].Title == "page4" &&
                      toc.TocElemList[0].Children[1].Children[1].Title == "page5" &&
                      toc.TocElemList[0].Children[2].Title == "page6";
        Assert.IsTrue(isTrue);
    }

    [Test]
    public void GenerateTocFromPageList()
    {
        // PageList Struct:
        // page1
        // |—— page2
        // |—— page3
        // |   |—— page4
        // |   |—— page5
        // |—— page6
        // page7
        // |—— page8
        // |—— page9
        // |   |—— page10
        // |   |—— page11
        // |—— page12
        PageList pageList = new PageList();
        PageElement page1 = new PageElement(1,"page1");
        PageElement page2 = new PageElement(2,"page2");
        PageElement page3 = new PageElement(2,"page3");
        PageElement page4 = new PageElement(3,"page4");
        PageElement page5 = new PageElement(3,"page5");
        PageElement page6 = new PageElement(2,"page6");
        PageElement page7 = new PageElement(1,"page7");
        PageElement page8 = new PageElement(2,"page8");
        PageElement page9 = new PageElement(2,"page9");
        PageElement page10 = new PageElement(3,"page10");
        PageElement page11 = new PageElement(3,"page11");
        PageElement page12 = new PageElement(2,"page12");

        pageList.AddPageElem(page1,2);
        pageList.AddPageElem(page2,2);
        pageList.AddPageElem(page3,2);
        pageList.AddPageElem(page4,2);
        pageList.AddPageElem(page5,2);
        pageList.AddPageElem(page6,2);
        pageList.AddPageElem(page7,2);
        pageList.AddPageElem(page8,2);
        pageList.AddPageElem(page9,2);
        pageList.AddPageElem(page10,2);
        pageList.AddPageElem(page11,2);
        pageList.AddPageElem(page12,2);

        Toc toc = new Toc();
        toc.GenerateTocFromPageList(pageList,2);
        
        // Toc Struct
        // page1
        // |—— page2
        // |—— page3
        // |   |—— page4
        // |   |—— page5
        // |—— page6
        // page7
        // |—— page8
        // |—— page9
        // |   |—— page10
        // |   |—— page11
        // |—— page12
        bool isTrue = toc.TocElemList[0].Title == "page1" &&
                      toc.TocElemList[0].Children[0].Title == "page2" &&
                      toc.TocElemList[0].Children[1].Title == "page3" &&
                      toc.TocElemList[0].Children[1].Children[0].Title == "page4" &&
                      toc.TocElemList[0].Children[1].Children[1].Title == "page5" &&
                      toc.TocElemList[0].Children[2].Title == "page6" &&
                      toc.TocElemList[1].Title == "page7" &&
                      toc.TocElemList[1].Children[0].Title == "page8" &&
                      toc.TocElemList[1].Children[1].Title == "page9" &&
                      toc.TocElemList[1].Children[1].Children[0].Title == "page10" &&
                      toc.TocElemList[1].Children[1].Children[1].Title == "page11" &&
                      toc.TocElemList[1].Children[2].Title == "page12";
        Assert.IsTrue(isTrue);
    }
}
