# L4D2ModManager
A mod manager for the game _Left 4 dead 2_

[中文版说明](#中文版说明)

## What's this
I feel tired and difficult to manage all my subscribed workshop items, the conflicts among MODs provided by game are not relliable, what's more? they doesn't give me some options to category my MODs.
Then I created this to help me deal with my subscribed MODs.

So as you think, this guy is used to manager game MODs, find out it there some conflicts, and have a easier way to figure out conflicts.

The screen shot of software is just like below :

![](./Introduction/english/screenshot.png)

## Functions
- Read subscribed items' information from Steam workshop (logined Steam is required)
- Read local VPK (MOD) files (not complete)
- Classify all MODs by tags
- Detect conflicts by tags
- Set On/Off or Subscribe/Unsubscribe without launching game

## Incomplete functions
- Detect conflicts by reading local VPK (MOD) files
- Classify MODs by custom rules
- Download MODs without launching game

## How to download it
Here is the ZIP file link : [Download](./Release/Release.zip)

## Development environment
- .NET 4.5.2
- Visual Studio Community 2017 (C# 5.0)
- WPF (Windows Presentation Foundation)

## Library and packages
- System.Drawing
- System.Net.Http
- Newtonsoft.Json (you can find dll file in this project)
- Windows API Code Pack (install : input the command `Install-Package WindowsAPICodePack-Shell` in the `Package Manager Console` )

## Used works
Thanks for these excellent works, you guys really help me a lot!
- [Facepunch/Steamworks](https://github.com/Facepunch/Facepunch.Steamworks) Steam APIs in C#
- [maddnias/SharpVPK](https://github.com/maddnias/SharpVPK) Read VPK files
- The Mozilla Universal Character Encoding Detector, I get it from [here](https://github.com/lucentsky/UniversalCharsetDetection)

I did some changes in these works for fixing bugs or extend them, you can see the changes in `ForkInfo.txt`.

## License
[MIT](./LICENSE)

Do whatever you want!

## Contact me
cxw39@foxmail.com


****


### 中文版说明
# 求生之路2 MOD管理器
## 这是什么？
管理游戏创意工坊的内容太不方便了，在游戏里有些冲突的MOD不会被标注出来，MOD也不能很好地分类展示。所以就有了这玩意。

软件截图：

![](./Introduction/simple-chinese/screenshot.png)

## 功能
- 从Steam创意工坊读取订阅物品信息(需要在已登陆的Steam)
- 从本地文件读取VPK(MOD)文件
- 将所有MOD按标签分类
- 按标签检测MOD冲突
- 在软件内完成设置MOD的开启/关闭、以及订阅/取消订阅等操作

## 未完成的功能
- 从VPK(MOD)文件中读取信息进行冲突检测
- 自定义MOD分类规则
- 在软件内完成MOD的下载等操作

## 下载方式
压缩包下载链接：[下载](./Release/Release.zip)

## 开发环境
- .NET 4.5.2
- Visual Studio Community 2017 (C# 5.0)
- WPF (Windows Presentation Foundation)

## 引用/库/程序包
- System.Drawing
- System.Net.Http
- Newtonsoft.Json (项目文件里包含dll文件)
- Windows API Code Pack (安装方法：在`程序包管理控制台`里面输入命令`Install-Package WindowsAPICodePack-Shell`)

## 使用的其他项目
感谢大佬们的开源项目！
- [Facepunch/Steamworks](https://github.com/Facepunch/Facepunch.Steamworks) Steam APIs in C#
- [maddnias/SharpVPK](https://github.com/maddnias/SharpVPK) 读取VPK文件
- The Mozilla Universal Character Encoding Detector, Mozilla的字符编码检测程序，我从[这里](https://github.com/lucentsky/UniversalCharsetDetection)下载的源码

这些项目我做了一些改动来修复BUG或者完善了程序, 修改的内容纪录在 `ForkInfo.txt`里面.

## 许可协议
[MIT](./LICENSE)

想干嘛干嘛！

## 联系方式
cxw39@foxmail.com
