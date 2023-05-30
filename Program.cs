using EpubBuilder.Core;
namespace EpubBuilder;

class Program
{
    public static void Main(string[] args)
    {
        var buildedData = ParseCLI.ParseCommandLineArgs(args);
        Epub epub = new Epub();
        epub.Generate(buildedData);
    }
}