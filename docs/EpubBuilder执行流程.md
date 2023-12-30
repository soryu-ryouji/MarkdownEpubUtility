# Epub Builder 执行流程

1. Get EpubMetadata and BuildMetadata
2. Convert markdown relative paths to absolute paths
3. Generate HtmlPages with markdown
4. Generate EpubContents with HtmlPages
5. Generate opf and toc file with EpubContents
6. Create Epub ZipFile and add all EpubContent to it