using System;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace Su.Revit.HelixToolkit.SharpDX
{
    public sealed class Viewport3DXOptions
    {
        /// <summary>
        /// 是否开启场景外渲染擦除
        /// </summary>
        public bool EnableRenderFrustum { get; set; } = true;

        /// <summary>
        /// 抗锯齿等级
        /// </summary>
        public FXAALevel FXAALevel { get; set; } = FXAALevel.Medium;

        /// <summary>
        /// 背景色
        /// </summary>
        public Color BackgroundColor { get; set; } = Colors.White;
    }
}
