using NUnit.Framework;
using EpubBuilder.Core;

namespace EpubBuilder.Tests;

[TestFixture]
public class TestParseMd
{
    [Test]
    public void SplitPage()
    {
        List<string> md = new List<string>()
        {
            "# 第一章 御宅族的拟日本",
            "## 1-1 何谓御宅族系文化",
            "### “御宅族系文化”所展现的后现代主义姿态",
            "大概沒有人不知道“御宅族”这个词吧？",
            "由动画和漫画所代表的御宅族系文化，至今仍多被视为是属于年轻人的文化。",
            "### 御宅族的三个世代",
            "我想在此简单叙述一下，为何本书不说“御宅族文化”，而使用“御宅族系文化”这种曖昧表现的理由。",
            "由于前述那些复杂的的状况，九〇年代对于“何谓御宅族”、“御宅族化的东西是什么”、“谁是御宅族，谁又不是御宅族”等问题，在御宅族之间累积了莫大的争议。",
            "## 1-2 御宅族的拟日本",
            "### 何谓后现代？",
            "笔者过去曾记述，御宅族系文化的结构，基本上极为展现了后现代主义的本质。",
            "很多读者对于“后现代”（Post modern）一词，应该会觉得很耳熟，“post”指的是“之后的事物”，“modern”意味着现代。",
            "# 第二章 数据库动物",
            "## 2-1 御宅族与后现代",
            "### 拟像的增殖",
            "如果只是主张御宅族系文化的本质，与后现代社会结构间有着深厚关联，並无新意。以下两点，早已被指出是御宅族系文化在后现代的特征。"
        };
        var list = ParseMd.SplitPage(md,3);

        bool isTrue = list.PageElemList[0].Heading == "第一章 御宅族的拟日本" &&
                      list.PageElemList[0].Content[0] == "# 第一章 御宅族的拟日本" &&
                      list.PageElemList[0].ChildrenPage[0].Heading == "1-1 何谓御宅族系文化" &&
                      list.PageElemList[0].ChildrenPage[0].Content[0] == "## 1-1 何谓御宅族系文化" &&
                      list.PageElemList[0].ChildrenPage[0].ChildrenPage[0].Heading == "“御宅族系文化”所展现的后现代主义姿态" &&
                      list.PageElemList[0].ChildrenPage[0].ChildrenPage[0].Content[0] == "### “御宅族系文化”所展现的后现代主义姿态" &&
                      list.PageElemList[0].ChildrenPage[0].ChildrenPage[0].Content[1] == "大概沒有人不知道“御宅族”这个词吧？" &&
                      list.PageElemList[0].ChildrenPage[0].ChildrenPage[0].Content[2] ==
                      "由动画和漫画所代表的御宅族系文化，至今仍多被视为是属于年轻人的文化。" &&
                      list.PageElemList[0].ChildrenPage[0].ChildrenPage[1].Heading == "御宅族的三个世代" &&
                      list.PageElemList[0].ChildrenPage[0].ChildrenPage[1].Content[0] == "### 御宅族的三个世代" &&
                      list.PageElemList[0].ChildrenPage[0].ChildrenPage[1].Content[1] ==
                      "我想在此简单叙述一下，为何本书不说“御宅族文化”，而使用“御宅族系文化”这种曖昧表现的理由。" &&
                      list.PageElemList[0].ChildrenPage[0].ChildrenPage[1].Content[2] ==
                      "由于前述那些复杂的的状况，九〇年代对于“何谓御宅族”、“御宅族化的东西是什么”、“谁是御宅族，谁又不是御宅族”等问题，在御宅族之间累积了莫大的争议。" &&
                      list.PageElemList[0].ChildrenPage[1].Heading == "1-2 御宅族的拟日本" &&
                      list.PageElemList[0].ChildrenPage[1].Content[0] == "## 1-2 御宅族的拟日本" &&
                      list.PageElemList[0].ChildrenPage[1].ChildrenPage[0].Heading == "何谓后现代？" &&
                      list.PageElemList[0].ChildrenPage[1].ChildrenPage[0].Content[0] == "### 何谓后现代？" &&
                      list.PageElemList[0].ChildrenPage[1].ChildrenPage[0].Content[1] ==
                      "笔者过去曾记述，御宅族系文化的结构，基本上极为展现了后现代主义的本质。" &&
                      list.PageElemList[0].ChildrenPage[1].ChildrenPage[0].Content[2] ==
                      "很多读者对于“后现代”（Post modern）一词，应该会觉得很耳熟，“post”指的是“之后的事物”，“modern”意味着现代。" &&
                      list.PageElemList[1].Heading == "第二章 数据库动物" &&
                      list.PageElemList[1].Content[0] == "# 第二章 数据库动物" &&
                      list.PageElemList[1].ChildrenPage[0].Heading == "2-1 御宅族与后现代" &&
                      list.PageElemList[1].ChildrenPage[0].Content[0] == "## 2-1 御宅族与后现代" &&
                      list.PageElemList[1].ChildrenPage[0].ChildrenPage[0].Heading == "拟像的增殖" &&
                      list.PageElemList[1].ChildrenPage[0].ChildrenPage[0].Content[0] == "### 拟像的增殖" &&
                      list.PageElemList[1].ChildrenPage[0].ChildrenPage[0].Content[1] == "如果只是主张御宅族系文化的本质，与后现代社会结构间有着深厚关联，並无新意。以下两点，早已被指出是御宅族系文化在后现代的特征。";

        Assert.IsTrue(isTrue);
    }
}