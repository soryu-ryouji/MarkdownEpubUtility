# Epub Builder CLI

通过命令行创建Epub电子书

## Command Line Parameter

-m / --markdown : markdown 文件路径（必填）

-c / --cover : cover 文件路径

-b / buildpath : epub 文件生成路径

-l / language : epub 语言

-t / title : epub 电子书名称

-a / author : 作者姓名

-u / uuid : epub 唯一标识符

-d / debug : 开启debug模式，会输出程序运行的信息


```shell
# Example in Windows
epubbuilder -m C:\Users\Ryouji\Desktop\逆流纯真年代\逆流纯真年代.md -a 人间武库 -c C:\Users\Ryouji\Desktop\逆流纯真年代\cover.jpg
```

## Docs

[程序执行流程](./Docs/程序执行流程.md)

[Epub电子书格式](./Docs/Epub电子书格式.md)

## Todo List

- [x] 多级目录的实现
- [ ] 添加子标签目录跳转支持
- [ ] markdown文本渲染
- [ ] markdown图像文件插入

## LICENSE

![MIT](./Docs/Images/MIT.png)

Epub is released under the MIT