![logo](./docs/images/logo-with-title-light.png)

## Markdown Epub Utility

[![NuGet Version](https://img.shields.io/nuget/v/MarkdownEpubUtility)](https://www.nuget.org/packages/MarkdownEpubUtility)

Markdown Epub Utility 是一个将 Markdown 文档转换为 Epub 电子书的跨平台的库。

通过 `SplitLevel` 参数，可以实现电子书内容的分割显示，避免了所有内容挤在一张 `html`中造成了加载迟缓。

**Example**

```c#
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

## Markdown Epub Utility CLI

| short name | long name    | description               |
| ---------- | ------------ | ------------------------- |
| `-m`       | `--markdown` | markdown 文件路径（必填） |
| `-c`       | `--cover`    | cover 文件路径            |
| `-b`       | `--build`    | epub 文件生成路径         |
| `-l`       | `--language` | epub 语言                 |
| `-t`       | `--title`    | epub 电子书名称           |
| `-a`       | `--author`   | 作者姓名                  |
| `-u`       | `--uuid`     | epub 唯一标识符           |
| `-s`       | `--split`    | 页文件分割等级            |

**Example**

```shell
eb build -m E:\天之炽\天之炽.md -c E:\天之炽\cover.jpg -b E:\天之炽\天之炽.epub -s 2
```

# LICENSE

![MIT](./docs/images/MIT.png)

EpubBuilder is released under the MIT