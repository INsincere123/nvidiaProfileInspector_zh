# 常用参数翻译覆盖清单

本轮覆盖 49 项参数、46 条现有说明及 11 个分组标题。1,965 个选项记录中，181 条可读标签有中文译文；1,784 条数字、FPS、比例或技术标识保留原样。没有说明的 3 项不新增说明。

中文名称和选项用于显示及搜索；提示保留英文。编辑框、原始值、参数 ID 和导入导出数据继续使用原文。字段原文与翻译资源不匹配时回退英文。

原始值映射：设置 ID + 值类型 + 原始值，并核对原始标签。翻译文件为内嵌资源，不覆盖 CustomSettingNames.xml 或 Reference.xml。

## 覆盖项

| 设置 ID | 中文名称 | 来源 | 选项记录 | 中文标签 |
| --- | --- | --- | ---: | ---: |
| `0x10E41E01` | DLSS 超分辨率 - 启用 DLL 覆盖 | CustomSettingNames.xml | 2 | 2 |
| `0x00634291` | DLSS 超分辨率 - 强制模型预设配置 | CustomSettingNames.xml | 3 | 2 |
| `0x10E41E02` | DLSS 光线重建 - 启用 DLL 覆盖 | CustomSettingNames.xml | 2 | 2 |
| `0x10E41E03` | DLSS 帧生成 - 启用 DLL 覆盖 | CustomSettingNames.xml | 2 | 2 |
| `0x10E41DF3` | DLSS 超分辨率 - 强制预设字母 | CustomSettingNames.xml | 17 | 16 |
| `0x10E41DF7` | DLSS 光线重建 - 强制预设字母 | CustomSettingNames.xml | 17 | 16 |
| `0x10E41E04` | DLSS-NR 神经渲染 - 启用 DLL 覆盖 | CustomSettingNames.xml | 2 | 2 |
| `0x10E41E05` | DLSS-NR 神经渲染 - 启用 Streamline 覆盖 | CustomSettingNames.xml | 2 | 2 |
| `0x10E41DF8` | DLSS-NR 神经渲染 - 强制预设字母 | CustomSettingNames.xml | 17 | 16 |
| `0x10E41E06` | DLSS 超分辨率 - 启用 Streamline 覆盖 | CustomSettingNames.xml | 2 | 2 |
| `0x10E41DF6` | DLSS 帧生成 - 场景自动切换检测 | CustomSettingNames.xml | 3 | 3 |
| `0x104596A1` | DLSS 帧生成 - 全屏菜单检测 | CustomSettingNames.xml | 3 | 2 |
| `0x104596A2` | DLSS 帧生成 - 全屏菜单覆盖率阈值 (float) | CustomSettingNames.xml | 0 | 0 |
| `0x104596A3` | DLSS 帧生成 - 全屏菜单 Alpha 阈值 (float) | CustomSettingNames.xml | 0 | 0 |
| `0x10AFB768` | DLSS 超分辨率 - 强制画质级别 | CustomSettingNames.xml | 7 | 5 |
| `0x10BD9423` | DLSS 光线重建 - 强制画质级别 | CustomSettingNames.xml | 7 | 5 |
| `0x10E41DF5` | DLSS 超分辨率 - 强制缩放比例 | CustomSettingNames.xml | 69 | 7 |
| `0x10C7D4A2` | DLSS 光线重建 - 强制缩放比例 | CustomSettingNames.xml | 69 | 7 |
| `0x104D6667` | DLSS 多帧生成 - 固定帧生成数量 | CustomSettingNames.xml | 6 | 0 |
| `0xB0D384C0` | 平滑动态 - 启用 | CustomSettingNames.xml | 2 | 2 |
| `0xB0CC0875` | 平滑动态 - 允许的 API | CustomSettingNames.xml | 8 | 8 |
| `0xB053C379` | 平滑动态 - 调试日志级别 | CustomSettingNames.xml | 4 | 1 |
| `0xB01B8B02` | 平滑动态 - 调试条 | CustomSettingNames.xml | 2 | 2 |
| `0xB03A4546` | 平滑动态 - Flip Metering 0 | CustomSettingNames.xml | 2 | 2 |
| `0xB03A4547` | 平滑动态 - Flip Metering 1 | CustomSettingNames.xml | 3 | 1 |
| `0x10AFB76C` | DLSS 超分辨率 - 将性能模式覆盖为超级性能 | CustomSettingNames.xml | 2 | 2 |
| `0x10E41DF4` | DLSS 超分辨率 - 将 DLSS 模式覆盖为 DLAA | CustomSettingNames.xml | 2 | 2 |
| `0x10835005` | 帧率限制 - 后台应用程序 | CustomSettingNames.xml | 57 | 1 |
| `0x10835002` | 帧率限制器 V3 | CustomSettingNames.xml | 982 | 1 |
| `0x0005F543` | 超低延迟 - 控制面板状态 | CustomSettingNames.xml | 3 | 3 |
| `0x10835000` | 超低延迟 - 启用 | CustomSettingNames.xml | 2 | 2 |
| `0x00CE2692` | 纹理过滤 - 质量替代 | CustomSettingNames.xml | 2 | 2 |
| `0x101AE763` | 垂直同步 - 平滑 AFR 行为 | CustomSettingNames.xml | 2 | 2 |
| `0x00638E8F` | 纹理过滤 - 自动调整 LOD 偏移 (SGSSAA) | CustomSettingNames.xml | 2 | 2 |
| `0x00738E8F` | 纹理过滤 - LOD 偏移 (DX) | CustomSettingNames.xml | 49 | 0 |
| `0x20403F79` | 纹理过滤 - LOD 偏移 (OGL) | CustomSettingNames.xml | 97 | 0 |
| `0x0019BB68` | 纹理过滤 - 负 LOD 偏移 | CustomSettingNames.xml | 2 | 2 |
| `0x00CE2691` | 纹理过滤 - 质量 | CustomSettingNames.xml | 4 | 4 |
| `0x002ECAF2` | 纹理过滤 - 三线性优化 | CustomSettingNames.xml | 2 | 2 |
| `0x007BA09E` | 最大预渲染帧数 | CustomSettingNames.xml | 9 | 1 |
| `0x00A879CF` | 垂直同步 | CustomSettingNames.xml | 7 | 7 |
| `0x005A375C` | 垂直同步 - 撕裂控制 | CustomSettingNames.xml | 2 | 2 |
| `0x1057EB71` | 电源管理 - 模式 | CustomSettingNames.xml | 3 | 3 |
| `0x10E41DF1` | DLSS 帧生成 - 强制预设字母 | CustomSettingNames.xml | 29 | 28 |
| `0x10308298` | DLSS 帧生成 - 强制模式 | CustomSettingNames.xml | 3 | 2 |
| `0x10CF4125` | DLSS 多帧生成 - 动态目标帧率 | CustomSettingNames.xml | 443 | 1 |
| `0x10562D0F` | DLSS 多帧生成 - 动态帧生成数量 | CustomSettingNames.xml | 6 | 5 |
| `0x10835006` | 后台应用程序最大帧率 (NVCPL) | Reference.xml | 2 | 0 |
| `0x1083500A` | 控制面板帧率限制器 (NVCPL) | Reference.xml | 2 | 0 |

## 检查方式

从仓库根目录运行：

```powershell
dotnet build tests/LocalizationChecks/LocalizationChecks.csproj -c Release
tests/LocalizationChecks/bin/Release/net48/LocalizationChecks.exe -mock
tests/LocalizationChecks/bin/Release/net48/LocalizationChecks.exe --render zh-CN -mock
tests/LocalizationChecks/bin/Release/net48/LocalizationChecks.exe --render en -mock
```

检查使用隔离配置和模拟驱动。布局图为 96/144 DPI 离线渲染，不代表完整真实驱动交互验证。
