[English](README.md) | [简体中文](README.zh-CN.md)

![NVIDIA Profile Inspector 主窗口](npi_screenshot.png)

# NVIDIA Profile Inspector

[![许可证：MIT](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![平台](https://img.shields.io/badge/platform-Windows-blue.svg)](#运行要求)
[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-512BD4.svg)](#从源码构建)

NVIDIA Profile Inspector 是一款 Windows 工具，通过 NVIDIA Driver Settings API 编辑
NVIDIA 驱动配置。它可以访问 NVIDIA 控制面板使用的驱动配置数据库，并提供许多隐藏或未公开说明的
配置选项，方便创建、调整、导出、导入和恢复各个应用程序的配置。

本工具面向高级用户，适用于需要精确控制游戏和应用程序配置、兼容性标志、帧率限制器、同步选项、
纹理过滤、SLI 设置、Optimus 行为、DLSS 覆盖，以及标准控制面板中不一定提供的其他驱动级开关的场景。

## 功能亮点

- 查看和编辑全局及各个应用程序的 NVIDIA 驱动配置。
- 添加或移除分配给配置的应用程序。
- 为驱动数据库中没有的游戏或应用程序创建自定义配置。
- 快速搜索和筛选配置选项。
- 在常用、已知和扩展驱动设置之间切换。
- 将常用设置标记为收藏。
- 使用内置位编辑器查看和编辑位掩码类设置。
- 导出和导入 `.nip` 配置备份。
- 导出和导入 NVIDIA 文本格式的配置转储。
- 恢复单个设置、整个配置或驱动默认值。
- 在已安装的 NVIDIA 驱动支持时，使用新版 DLSS、DLSS 光线重建和 DLSS 帧生成覆盖选项。
- 选择多种界面主题，并可选用 Windows 11 背景效果。
- 在应用程序内检查 GitHub 发布版本。

## 下载

从 [GitHub Releases 页面](https://github.com/Orbmu2k/nvidiaProfileInspector/releases)
下载最新官方构建版本。实验性构建发布后，也可在同一页面获取预发布版本：
[GitHub 预发布版本](https://github.com/Orbmu2k/nvidiaProfileInspector/releases?q=prerelease%3Atrue)。

发布压缩包包含可执行文件及运行所需的文件。将压缩包解压到可写入的文件夹，然后启动
`nvidiaProfileInspector.exe`。

## 运行要求

- Windows，并已安装兼容的 NVIDIA 显卡驱动。
- 已安装驱动所支持的 NVIDIA GPU。
- .NET Framework 4.8。
- 管理员权限。NVIDIA Profile Inspector 必须以管理员身份启动，才能将更改写入 NVIDIA 驱动配置数据库。

## 快速开始

1. 启动 `nvidiaProfileInspector.exe`。
2. 从配置框中选择一个配置，或输入游戏或应用程序的配置名称并按 Enter，查找匹配的配置。
3. 使用搜索框或设置模式选择器，找到要编辑的设置。
4. 在设置列表中修改选项值。
5. 单击 **应用更改**，将配置更改写入 NVIDIA 驱动数据库。

尝试高级设置之前，请先导出当前配置或所有自定义配置，以便日后恢复到已知正常的状态。

## 配置备份

NVIDIA Profile Inspector 可以将配置更改保存为 `.nip` 文件。这适用于分享单个游戏配置、
保存个人预设，或在更新驱动之前备份已修改的驱动配置。

推荐的备份流程：

1. 选择要保留的配置。
2. 使用 **导出** 导出单个配置，或从导出菜单中导出所有自定义配置。
3. 将生成的 `.nip` 文件保存在驱动安装文件夹之外的位置。
4. 需要时使用 **导入** 恢复或合并配置。

## 高级设置须知

NVIDIA Profile Inspector 提供的许多选项属于隐藏、实验性、已弃用、特定驱动版本专用，
或 NVIDIA 未公开说明的设置。某个值在一种游戏、驱动或 GPU 架构上表现良好，
在其他环境中却可能没有效果或引发问题。

尽可能针对单个配置进行修改，并保留备份。在重置整个驱动数据库之前，优先尝试恢复单个设置或配置。

## 命令行

NVIDIA Profile Inspector 提供少量启动选项，用于导入、导出和维护任务。

```powershell
nvidiaProfileInspector.exe [options] [profile1.nip] [profile2.nip ...]
```

| 选项 | 说明 |
| --- | --- |
| `<file>.nip` | 导入一个或多个 `.nip` 配置文件。如果已有实例正在运行，则将文件转交给该实例。 |
| `-silentImport` | 导入 `.nip` 文件，不显示常规的成功提示对话框。 |
| `-silent` | `-silentImport` 的别名。 |
| `-exportCustomized` | 将所有自定义配置导出为带时间戳的 `.nip` 文件，保存在可执行文件所在目录，然后退出。 |
| `-createCSN` | 将内嵌的 `CustomSettingNames.xml` 文件写入当前工作目录，然后退出。 |
| `-showOnlyCSN` | 启动界面，仅显示自定义设置。 |
| `-disableScan` | 启动界面，不执行初始配置扫描。 |

示例：

```powershell
nvidiaProfileInspector.exe ".\MyProfile.nip"
nvidiaProfileInspector.exe -silentImport ".\ProfileA.nip" ".\ProfileB.nip"
nvidiaProfileInspector.exe -exportCustomized
```

## 从源码构建

本项目是面向 .NET Framework 4.8 的 WPF 桌面应用程序。

```powershell
git clone https://github.com/Orbmu2k/nvidiaProfileInspector.git
cd nvidiaProfileInspector
dotnet build .\nvidiaProfileInspector\nvidiaProfileInspector.sln -c Release
```

编译后的应用程序输出至：

```text
nvidiaProfileInspector\bin\Release\net48\
```

也可以使用 Visual Studio 直接打开 `nvidiaProfileInspector\nvidiaProfileInspector.sln`。

## 项目结构

```text
nvidiaProfileInspector/
+-- Common/                 驱动配置服务、元数据、导入导出逻辑
+-- Native/                 NVAPI 和 Win32 互操作
+-- Services/               主题、更新检查等应用程序服务
+-- UI/                     WPF 视图、视图模型、控件、样式和主题
+-- CustomSettingNames.xml  已知设置的易读名称和说明
`-- Reference.xml           复制到输出文件夹的参考设置元数据
```

## 相关资源

- [GitHub Releases](https://github.com/Orbmu2k/nvidiaProfileInspector/releases)
- [问题反馈和功能请求](https://github.com/Orbmu2k/nvidiaProfileInspector/issues)
- [STEP Project NVIDIA Inspector 指南](https://wiki.step-project.com/Guide:NVIDIA_Inspector)
- [PCGamingWiki：Nvidia Profile Inspector](https://www.pcgamingwiki.com/wiki/Nvidia_Profile_Inspector)

## 参与贡献

欢迎提交错误报告、设置更新、兼容性说明和拉取请求。
报告问题时，请附上 NVIDIA 驱动版本、Windows 版本、GPU 型号、受影响的配置或可执行文件，
以及复现问题的步骤。

## 许可证

NVIDIA Profile Inspector 采用 [MIT 许可证](LICENSE) 发布。
