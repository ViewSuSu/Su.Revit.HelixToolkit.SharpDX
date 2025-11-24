using System.Windows.Media;
using Color = Autodesk.Revit.DB.Color;

namespace Su.Revit.HelixToolkit.SharpDX.Extensions
{
    /// <summary>
    /// RevitColorExtension - SharpDX 版本（支持透明度）
    /// </summary>
    internal static class RevitColorExtension
    {
        /// <summary>
        /// 将 System.Windows.Media.Color 转换为 Revit Color
        /// </summary>
        internal static Color ToRevitColor(this System.Windows.Media.Color color)
        {
            return new Color(color.R, color.G, color.B);
        }

        /// <summary>
        /// 将 Revit Color 转换为 Color4 (SharpDX)，可指定透明度
        /// </summary>
        internal static Color4 ToColor4(this Color color, float alpha = 1.0f)
        {
            if (color is null)
                return Colors.DarkGray.ToColor4(alpha);

            return new Color4(color.Red / 255.0f, color.Green / 255.0f, color.Blue / 255.0f, alpha);
        }

        /// <summary>
        /// 将 System.Windows.Media.Color 转换为 Color4 (SharpDX)
        /// </summary>
        internal static Color4 ToColor4(
            this System.Windows.Media.Color color,
            float alpha = default
        )
        {
            return new Color4(
                color.R / 255.0f,
                color.G / 255.0f,
                color.B / 255.0f,
                alpha == default ? color.A / 255.0f : alpha
            );
        }
    }
}
