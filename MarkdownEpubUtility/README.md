# Markdown Epub Utility

Markdown Epub Utility is a cross platform library that converts Markdown documents into Epub e-books.

By using the 'SplitLevel' parameter, it is possible to achieve the segmentation and display of e-book content, avoiding loading delays caused by all content being squeezed into one 'HTML'.

**Example**

```csharp
// Build Epub
var epubMetadata = new EpubMetadata
{
    Title = "The Art of Unix Programming",
    Language = "en",
    Author = "Eric S. Raymond",
};

var mdPath = @"D:\Books\Novel\TheArtofUnixProgramming\TheArtofUnixProgramming.md";
var coverPath = @"D:\Books\Novel\TheArtofUnixProgramming\cover.jpg";
var buildPath = @"D:\TheArtofUnixProgramming.epub";

var buildMetadata = new BuildMetadata(mdPath, coverPath, pageSplitLevel:1);

var epub = new EpubBook(epubMetadata, buildMetadata);
epub.CreateEpub(buildPath);

// Open Book
var book = EpubBook.OpenBook(@"D:\The Art of Unix Programming.epub");
Console.WriteLine(book.Metadata);
var imageList = book.ExtractImage();

var imagePath = @"D:\images\";
if (!Directory.Exists(imagePath)) Directory.CreateDirectory(imagePath);
foreach (var image in items)
{
    var filePath = Path.Combine(imagePath, image.FileName);
    File.WriteAllBytes(filePath, image.Content);
}
```