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
            "# This is One First heading",
            "Abscsdsadasdad",
            "## This is Second Heading",
            "Abababababbabababaa",
            "### This is Thirst Heading",
            "ssssdsdadadadadadasdadadsas",
            "# This is Two First Heading",
            "uuuuuuuuuuuuuuu"
        };
        var list = ParseMd.SplitPage(md);

        var headingOne = "This is One First heading";
        var headingOneContent = "Abscsdsadasdad";
        var headingTwo = "This is Two First Heading";
        var headingTwoContent = "uuuuuuuuuuuuuuu";


        // foreach (var unit in list.Pages)
        // {
        //     Console.WriteLine(unit.Content[0]);
        // }
        
        bool targetOne = headingOne == list.Pages[0].Heading;
        bool targetOneContent = headingOneContent == list.Pages[0].Content[0];
        bool targetTwo = headingTwo == list.Pages[1].Heading;
        bool targetTwoContent = headingTwoContent == list.Pages[1].Content[0];

        bool isTrue = targetOne && targetTwo && targetOneContent && targetTwoContent;
        
        Assert.IsTrue(isTrue);
    }
}