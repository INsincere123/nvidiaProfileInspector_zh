using System;
using System.Collections.Generic;

namespace nvidiaProfileInspector.Localization
{
    // IDs and meanings verified against CustomSettingNames.xml and Reference.xml.
    // Search-only aliases: never merged into driver metadata or serialized profiles.
    public static class SettingSearch
    {
        private static readonly Dictionary<uint, string> ChineseAliases = new Dictionary<uint, string>
        {
            [0x10E41E04] = "DLSS-NR 神经渲染 DLL 覆盖",
            [0x10E41E05] = "DLSS-NR 神经渲染 Streamline 覆盖",
            [0x10E41DF8] = "DLSS-NR 神经渲染 预设字母",
            [0x10E41E01] = "DLSS 超分辨率 超分 超采样 DLL 覆盖",
            [0x00634291] = "DLSS 超分辨率 模型预设配置",
            [0x10E41DF3] = "DLSS 超分辨率 预设字母",
            [0x10E41E06] = "DLSS 超分辨率 Streamline 覆盖",
            [0x10AFB768] = "DLSS 超分辨率 画质 质量模式",
            [0x10E41DF5] = "DLSS 超分辨率 缩放比例",
            [0x10AFB76C] = "DLSS 超分辨率 超级性能",
            [0x10E41DF4] = "DLSS DLAA 深度学习抗锯齿",
            [0x10E41E02] = "DLSS 光线重建 光追重建 DLL 覆盖",
            [0x10E41DF7] = "DLSS 光线重建 光追重建 预设字母",
            [0x10BD9423] = "DLSS 光线重建 光追重建 画质 质量模式",
            [0x10C7D4A2] = "DLSS 光线重建 光追重建 缩放比例",
            [0x10E41E03] = "DLSS 帧生成 补帧 插帧 DLL 覆盖",
            [0x10E41DF1] = "DLSS 帧生成 补帧 插帧 预设字母",
            [0x10308298] = "DLSS 帧生成 补帧 插帧 强制模式",
            [0x10E41DF6] = "DLSS 帧生成 场景自动切换检测",
            [0x104596A1] = "DLSS 帧生成 全屏菜单检测",
            [0x104596A2] = "DLSS 帧生成 全屏菜单覆盖阈值",
            [0x104596A3] = "DLSS 帧生成 全屏菜单透明度阈值",
            [0x104D6667] = "DLSS 多帧生成 补帧 插帧 固定帧数",
            [0x10CF4125] = "DLSS 多帧生成 动态目标帧率",
            [0x10562D0F] = "DLSS 多帧生成 动态帧数",
            [0xB0D384C0] = "平滑动态 平滑运动 驱动补帧 驱动插帧 启用",
            [0xB0CC0875] = "平滑动态 平滑运动 驱动补帧 驱动插帧 接口",
            [0xB053C379] = "平滑动态 平滑运动 调试日志",
            [0xB01B8B02] = "平滑动态 平滑运动 调试条",
            [0xB03A4546] = "平滑动态 平滑运动 翻转计量",
            [0xB03A4547] = "平滑动态 平滑运动 翻转计量",
            [0x00A879CF] = "垂直同步 同步模式",
            [0x005A375C] = "垂直同步 自适应同步 撕裂控制",
            [0x101AE763] = "垂直同步 平滑交替帧渲染 AFR",
            [0x10835002] = "帧率限制 帧率上限 限帧 锁帧",
            [0x10835005] = "后台帧率限制 后台限帧 后台锁帧",
            [0x10835006] = "后台帧率上限 后台限帧 后台锁帧",
            [0x1083500A] = "最大帧率 帧率上限 限帧 锁帧",
            [0x0005F543] = "超低延迟 低延迟模式 控制面板状态",
            [0x10835000] = "超低延迟 低延迟模式 启用",
            [0x007BA09E] = "最大预渲染帧数 低延迟",
            [0x1057EB71] = "电源管理 功耗管理 性能模式 最高性能",
            [0x00CE2691] = "纹理过滤 纹理筛选 质量 画质",
            [0x00CE2692] = "纹理过滤 纹理筛选 质量替代",
            [0x00638E8F] = "纹理过滤 纹理筛选 LOD 偏移 自动调整",
            [0x00738E8F] = "纹理过滤 纹理筛选 LOD 偏移 DX",
            [0x20403F79] = "纹理过滤 纹理筛选 LOD 偏移 OGL",
            [0x0019BB68] = "纹理过滤 纹理筛选 负 LOD 偏移",
            [0x002ECAF2] = "纹理过滤 纹理筛选 三线性优化"
        };

        public static bool Matches(uint settingId, string displayName, string alternateNames,
            string settingIdHex, string query, string originalName = null)
        {
            if (string.IsNullOrWhiteSpace(query)) return true;
            return Contains(displayName, query) || Contains(originalName, query) || Contains(alternateNames, query)
                || Contains(settingIdHex, query)
                || (ChineseAliases.TryGetValue(settingId, out var aliases) && Contains(aliases, query));
        }

        private static bool Contains(string text, string query) =>
            text?.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
