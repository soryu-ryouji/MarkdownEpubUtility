![Logo](./Docs/Images/logo.png)

# Epub Builder CLI

通过命令行创建Epub电子书

## Command Line Parameter

| ShortName | LongName    | Description         |
|-----------|-------------|---------------------|
| -m        | --markdown  | markdown 文件路径（必填）   |
| -c        | --cover     | cover 文件路径          |
| -b        | --buildpath | epub 文件生成路径         |
| -l        | --language  | epub 语言             |
| -t        | --title     | epub 电子书名称          |
| -a        | --author    | 作者姓名                |
| -u        | --uuid      | epub 唯一标识符          |
| -s        | --split     | 页文件分割等级             |
| -d        | --debug     | 开启debug模式，输出程序运行的信息 |

> split 参数介绍
>
> 当 s 为 2 时，一级标题和二级标题的内容会作为单独的页面切割，而二级标题以下的内容会被放在二级标题页面中展示

```shell
# Example in Windows
epubbuilder.exe -m C:\Users\Ryouji\Desktop\逆流纯真年代\逆流纯真年代.md -a 人间武库 -c C:\Users\Ryouji\Desktop\逆流纯真年代\cover.jpg -s 2

# 简短版
# 程序默认将 buildpath 设置为 markdown 文件的父目录，split 默认为 1，uuid 随机生成
epubbuilder.exe -m C:\Users\Ryouji\Desktop\逆流纯真年代\逆流纯真年代.md
```

## Docs

[Epub电子书格式](./Docs/Epub电子书格式.md)

## Todo List

- [x] 多级目录的实现
- [x] 添加子标签目录跳转支持
- [x] markdown文本渲染
- [ ] markdown图像文件插入

## LICENSE

![MIT](./Docs/Images/MIT.png)

Epub is released under the MIT