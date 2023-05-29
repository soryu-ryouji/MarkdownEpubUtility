namespace EpubBuilder.Tests;
using NUnit.Framework;
using EpubBuilder.Core;

[TestFixture]
public class TestEpub
{
    [Test]
    public void TestGenerateOpfManifestPageItem()
    {
        var manifestList = new List<string>();
        int chapterNum = 0;
        
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

        Epub epub = new Epub();
        epub.GenerateOpfManifestPageItem(manifestList, pageList.PageElemList.First(), chapterNum, 2);

        string str = String.Join("\n", manifestList);
    }

    [Test]
    public void TestGenerateOpfManifest()
    {
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

        Epub epub = new Epub();
        var str = epub.GenerateOpfManifest(pageList, 1);
        Console.WriteLine(str);
    }

    [Test]
    public void TestGenerateOpf()
    {
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
 
         Epub epub = new Epub();
         epub.AddMetadata(MetadataType.Title,"TestBookTitle");
         var str = epub.GenerateOpf(pageList, 2);
         Console.WriteLine(str);       
    }
}