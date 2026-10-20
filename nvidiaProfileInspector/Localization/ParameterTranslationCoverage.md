# 参数翻译覆盖清单

CustomSettingNames.xml 的全部 117 项参数名称和 114 条现有说明已覆盖；保留此前 Reference.xml 的 2 项译文，未扩展整个 Reference.xml 参数库。合计 119 项参数、116 条说明及 11 个分组标题。

2,422 个选项记录中，363 条可读标签有中文译文；2,059 条数字、FPS、容量、比例或技术标识保留原样。没有说明的 3 项不新增说明。

中文名称和选项用于显示及搜索；提示保留英文。编辑框、原始值、参数 ID 和导入导出数据继续使用原文。字段原文与翻译资源不匹配时回退英文。

原始值映射：设置 ID + 值类型 + 原始值，并核对原始标签。翻译文件为内嵌资源，不覆盖 CustomSettingNames.xml 或 Reference.xml。

上游存在相同数值使用不同标签，以及锐化参数不同数值共用数字标签的情况。保留这些原始记录及现有英文解析行为，不在翻译中修正元数据。rBAR 大小限制使用 QWORD，检查覆盖超过 32 位的原始值。

“RTX 动态亮丽”采用 [NVIDIA 官方译名](https://www.nvidia.cn/software/nvidia-app/)。

## 覆盖项

| 设置 ID | 中文名称 | 类型 | 来源 | 选项记录 | 中文标签 |
| --- | --- | --- | --- | ---: | ---: |
| `0x10E41E01` | DLSS 超分辨率 - 启用 DLL 覆盖 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x00634291` | DLSS 超分辨率 - 强制模型预设配置 | Dword | CustomSettingNames.xml | 3 | 2 |
| `0x10E41E02` | DLSS 光线重建 - 启用 DLL 覆盖 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x10E41E03` | DLSS 帧生成 - 启用 DLL 覆盖 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x10E41DF3` | DLSS 超分辨率 - 强制预设字母 | Dword | CustomSettingNames.xml | 17 | 16 |
| `0x10E41DF7` | DLSS 光线重建 - 强制预设字母 | Dword | CustomSettingNames.xml | 17 | 16 |
| `0x10E41E04` | DLSS-NR 神经渲染 - 启用 DLL 覆盖 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x10E41E05` | DLSS-NR 神经渲染 - 启用 Streamline 覆盖 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x10E41DF8` | DLSS-NR 神经渲染 - 强制预设字母 | Dword | CustomSettingNames.xml | 17 | 16 |
| `0x10E41E06` | DLSS 超分辨率 - 启用 Streamline 覆盖 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x10E41DF6` | DLSS 帧生成 - 场景自动切换检测 | Dword | CustomSettingNames.xml | 3 | 3 |
| `0x104596A1` | DLSS 帧生成 - 全屏菜单检测 | Dword | CustomSettingNames.xml | 3 | 2 |
| `0x104596A2` | DLSS 帧生成 - 全屏菜单覆盖率阈值 (float) | Dword | CustomSettingNames.xml | 0 | 0 |
| `0x104596A3` | DLSS 帧生成 - 全屏菜单 Alpha 阈值 (float) | Dword | CustomSettingNames.xml | 0 | 0 |
| `0x10AFB768` | DLSS 超分辨率 - 强制画质级别 | Dword | CustomSettingNames.xml | 7 | 5 |
| `0x10BD9423` | DLSS 光线重建 - 强制画质级别 | Dword | CustomSettingNames.xml | 7 | 5 |
| `0x10E41DF5` | DLSS 超分辨率 - 强制缩放比例 | Dword | CustomSettingNames.xml | 69 | 7 |
| `0x10C7D4A2` | DLSS 光线重建 - 强制缩放比例 | Dword | CustomSettingNames.xml | 69 | 7 |
| `0x104D6667` | DLSS 多帧生成 - 固定帧生成数量 | Dword | CustomSettingNames.xml | 6 | 0 |
| `0xB0D384C0` | 平滑动态 - 启用 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0xB0CC0875` | 平滑动态 - 允许的 API | Dword | CustomSettingNames.xml | 8 | 8 |
| `0xB053C379` | 平滑动态 - 调试日志级别 | Dword | CustomSettingNames.xml | 4 | 1 |
| `0xB01B8B02` | 平滑动态 - 调试条 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0xB03A4546` | 平滑动态 - Flip Metering 0 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0xB03A4547` | 平滑动态 - Flip Metering 1 | Dword | CustomSettingNames.xml | 3 | 1 |
| `0x00432F84` | RTX HDR - 驱动标志 | Dword | CustomSettingNames.xml | 6 | 6 |
| `0x00DD48FB` | RTX HDR - 启用 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x00DD48FC` | RTX HDR - 峰值亮度 | Dword | CustomSettingNames.xml | 0 | 0 |
| `0x00DD48FD` | RTX HDR - 中间灰 | Dword | CustomSettingNames.xml | 0 | 0 |
| `0x00DD48FE` | RTX HDR - 对比度 | Dword | CustomSettingNames.xml | 0 | 0 |
| `0x00DD48FF` | RTX HDR - 饱和度 | Dword | CustomSettingNames.xml | 0 | 0 |
| `0x00980880` | RTX 动态亮丽 - 启用 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x00ABAB13` | RTX 动态亮丽 - 饱和度增强 | Dword | CustomSettingNames.xml | 21 | 0 |
| `0x00ABAB22` | RTX 动态亮丽 - 强度 | Dword | CustomSettingNames.xml | 21 | 0 |
| `0x10AFB76C` | DLSS 超分辨率 - 将性能模式覆盖为超级性能 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x10E41DF4` | DLSS 超分辨率 - 将 DLSS 模式覆盖为 DLAA | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x10835005` | 帧率限制 - 后台应用程序 | Dword | CustomSettingNames.xml | 57 | 1 |
| `0x00AC8497` | 着色器缓存 - 大小 | Dword | CustomSettingNames.xml | 12 | 2 |
| `0x208F3F2E` | 着色器缓存 - 启用 (OpenGL) | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x000F00BA` | rBAR - 启用 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x000BFA21` | rBAR - 启用 v2 | Dword | CustomSettingNames.xml | 3 | 3 |
| `0x000F00BB` | rBAR - 选项 | Dword | CustomSettingNames.xml | 0 | 0 |
| `0x00E942FC` | rBAR - Intel CPU 排除 | Dword | CustomSettingNames.xml | 0 | 0 |
| `0x000F00FF` | rBAR - 大小限制 | Qword | CustomSettingNames.xml | 12 | 0 |
| `0x000F00BE` | rBAR - 标志 (0x000F00BE) | Dword | CustomSettingNames.xml | 0 | 0 |
| `0x000F00BF` | rBAR - 标志 (0x000F00BF) | String | CustomSettingNames.xml | 0 | 0 |
| `0x008DF510` | G-SYNC - 支持状态指示浮层 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x00DE429A` | 光线追踪 - 启用 (DXR) | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x20BC1A2B` | 光线追踪 - 启用 (Vulkan RT) | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x1075D972` | NVIDIA Ansel - 启用 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x809D5F60` | NVIDIA App 浮层 - 标志 | Dword | CustomSettingNames.xml | 3 | 3 |
| `0x00598928` | 锐化滤镜 - 启用 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x002ED8CD` | 锐化 - 强度 | Dword | CustomSettingNames.xml | 102 | 0 |
| `0x002ED8CE` | 锐化 - 忽略胶片颗粒 | Dword | CustomSettingNames.xml | 102 | 0 |
| `0x10835002` | 帧率限制器 V3 | Dword | CustomSettingNames.xml | 982 | 1 |
| `0x0005F543` | 超低延迟 - 控制面板状态 | Dword | CustomSettingNames.xml | 3 | 3 |
| `0x10835000` | 超低延迟 - 启用 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x00CE2692` | 纹理过滤 - 质量替代 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x0098C1AC` | 抗锯齿 - 启用 MFAA | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x0064B541` | 首选刷新率 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x10029538` | G-SYNC - 运行状态指示浮层 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x1194F158` | G-SYNC - 应用程序模式 | Dword | CustomSettingNames.xml | 3 | 3 |
| `0x1094F1F7` | G-SYNC - 全局模式 | Dword | CustomSettingNames.xml | 3 | 3 |
| `0x1094F157` | G-SYNC - 全局功能 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x10A879CF` | G-SYNC - 应用程序状态 | Dword | CustomSettingNames.xml | 5 | 5 |
| `0x10A879AC` | G-SYNC - 应用程序请求状态 | Dword | CustomSettingNames.xml | 5 | 5 |
| `0x103BCCB5` | 禁止各向异性过滤 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x00198FFF` | 着色器缓存 - 启用 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x000858F7` | 抗锯齿修正 (DX9) | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x101AE763` | 垂直同步 - 平滑 AFR 行为 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x00638E8F` | 纹理过滤 - 自动调整 LOD 偏移 (SGSSAA) | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x002CF156` | 翻转指示器 | Dword | CustomSettingNames.xml | 7 | 7 |
| `0x1074C972` | 抗锯齿 - 启用 FXAA | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x1068FB9C` | 抗锯齿 - FXAA 指示浮层 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x1034CB89` | 抗锯齿 - 启用 FXAA（NVIDIA 预定义） | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x2089BF6C` | 抗锯齿 - 线条伽马 | Dword | CustomSettingNames.xml | 3 | 3 |
| `0x00738E8F` | 纹理过滤 - LOD 偏移 (DX) | Dword | CustomSettingNames.xml | 49 | 0 |
| `0x20403F79` | 纹理过滤 - LOD 偏移 (OGL) | Dword | CustomSettingNames.xml | 97 | 0 |
| `0x00D55F7D` | 抗锯齿 - 兼容性 (DX9) | Dword | CustomSettingNames.xml | 0 | 0 |
| `0x00E32F8A` | 抗锯齿 - 兼容性 (DX1x) | Dword | CustomSettingNames.xml | 0 | 0 |
| `0x002C7F45` | 环境光遮蔽 - 兼容性 | Dword | CustomSettingNames.xml | 0 | 0 |
| `0x107D639D` | 抗锯齿 - 伽马校正 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x10ECDB82` | 抗锯齿 (MSAA) - 行为标志 | Dword | CustomSettingNames.xml | 8 | 8 |
| `0x107EFC5B` | 抗锯齿 (MSAA) - 模式 | Dword | CustomSettingNames.xml | 3 | 3 |
| `0x10D773D2` | 抗锯齿 (MSAA) - 设置 | Dword | CustomSettingNames.xml | 20 | 20 |
| `0x10FC2D9C` | 抗锯齿 - 透明度多重采样 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x10D48A85` | 抗锯齿 - 透明度超级采样 | Dword | CustomSettingNames.xml | 8 | 8 |
| `0x10D2BB16` | 各向异性过滤 - 模式 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x101E61A9` | 各向异性过滤 - 级别 | Dword | CustomSettingNames.xml | 8 | 2 |
| `0x0084CD70` | 各向异性过滤 - 优化 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x00E73211` | 各向异性过滤 - 采样优化 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x0019BB68` | 纹理过滤 - 负 LOD 偏移 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x00CE2691` | 纹理过滤 - 质量 | Dword | CustomSettingNames.xml | 4 | 4 |
| `0x002ECAF2` | 纹理过滤 - 三线性优化 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x00664339` | 环境光遮蔽 - 启用 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x00667329` | 环境光遮蔽 - 设置 | Dword | CustomSettingNames.xml | 3 | 3 |
| `0x007BA09E` | 最大预渲染帧数 | Dword | CustomSettingNames.xml | 9 | 1 |
| `0x20C1221E` | 线程优化 | Dword | CustomSettingNames.xml | 3 | 3 |
| `0x20FDD1F9` | 三重缓冲 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x00A879CF` | 垂直同步 | Dword | CustomSettingNames.xml | 7 | 7 |
| `0x005A375C` | 垂直同步 - 撕裂控制 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x1094F16F` | PhysX - 指示浮层 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x5067ECC4` | PhysX - 启用 32 位 GPU 加速 (RTX 50) | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x1057EB71` | 电源管理 - 模式 | Dword | CustomSettingNames.xml | 3 | 3 |
| `0x50166C5E` | CUDA - 强制 P2 状态 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x10ECECC9` | CUDA - 系统内存回退策略 | Dword | CustomSettingNames.xml | 3 | 3 |
| `0x20FF7493` | 扩展限制 (OGL) | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x200AEBFC` | 多显示器 / 混合 GPU 加速 | Dword | CustomSettingNames.xml | 3 | 3 |
| `0x2046B3ED` | 版本覆盖 (OGL) | Dword | CustomSettingNames.xml | 11 | 11 |
| `0x00A88888` | 多平面叠加 (MPO) 指示器 | Dword | CustomSettingNames.xml | 2 | 2 |
| `0x20D690F8` | Vulkan/OpenGL 呈现方式 | Dword | CustomSettingNames.xml | 3 | 3 |
| `0x20324987` | Vulkan/OpenGL 呈现方式 - 标志 | Dword | CustomSettingNames.xml | 10 | 9 |
| `0x10E41DF1` | DLSS 帧生成 - 强制预设字母 | Dword | CustomSettingNames.xml | 29 | 28 |
| `0x10308298` | DLSS 帧生成 - 强制模式 | Dword | CustomSettingNames.xml | 3 | 2 |
| `0x10CF4125` | DLSS 多帧生成 - 动态目标帧率 | Dword | CustomSettingNames.xml | 443 | 1 |
| `0x10562D0F` | DLSS 多帧生成 - 动态帧生成数量 | Dword | CustomSettingNames.xml | 6 | 5 |
| `0x00EAD189` | 着色器缓存 - 预编译选项 | Dword | CustomSettingNames.xml | 4 | 4 |
| `0x10835006` | 后台应用程序最大帧率 (NVCPL) | Dword | Reference.xml | 2 | 0 |
| `0x1083500A` | 控制面板帧率限制器 (NVCPL) | Dword | Reference.xml | 2 | 0 |

## 检查方式

从仓库根目录运行：

```powershell
dotnet build tests/LocalizationChecks/LocalizationChecks.csproj -c Release
tests/LocalizationChecks/bin/Release/net48/LocalizationChecks.exe -mock
tests/LocalizationChecks/bin/Release/net48/LocalizationChecks.exe --render zh-CN -mock
tests/LocalizationChecks/bin/Release/net48/LocalizationChecks.exe --render en -mock
```

检查使用隔离配置和模拟驱动。每种语言生成 8 类窗口 × 4 个主题 × 2 个比例，共 64 张布局图。MainExtended 覆盖 RTX HDR、MSAA、G-SYNC、着色器缓存、rBAR、CUDA 和 Vulkan/OpenGL。布局图为 96/144 DPI 离线渲染，不代表完整真实驱动交互验证。
