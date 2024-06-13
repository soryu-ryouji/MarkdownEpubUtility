![logo](./docs/images/logo-with-title-light.png)

## Markdown Epub Utility

[简体中文](./README-zh.md)

Markdown Epub Utility is a cross platform library that converts Markdown documents into Epub e-books.

By using the 'SplitLevel' parameter, it is possible to achieve the segmentation and display of e-book content, avoiding loading delays caused by all content being squeezed into one 'HTML'.

**Example**

```csharp
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
epub.CreateEpub().Save(buildPath);
```

### To do List

- [ ] Extract Markdown from Epub.

## Markdown Epub Utility CLI

| short name | long name    | description                          |
| ---------- | ------------ | ------------------------------------ |
| `-m`       | `--markdown` | Path to the markdown file (required) |
| `-c`       | `--cover`    | Path to the cover file               |
| `-b`       | `--build`    | Path to generate the EPUB file       |
| `-l`       | `--language` | Language for the EPUB                |
| `-t`       | `--title`    | Title of the EPUB ebook              |
| `-a`       | `--author`   | Author's name                        |
| `-u`       | `--uuid`     | Unique identifier for the EPUB       |
| `-s`       | `--split`    | Level of page file splitting         |

**Example**

```shell
eb build --markdowm file.md --cover cover.jpg --split 2 -author author

eb build -m file.md -c cover.jpg -s 2 -a author
```

# LICENSE

![MIT](./docs/images/MIT.png)

EpubBuilder is released under the MIT