using System;
using System.IO;
using Autodesk.Revit.DB;
using Material = HelixToolkit.Wpf.SharpDX.Material;

namespace Su.Revit.HelixToolkit.SharpDX
{
    /// <summary>
    /// 表示用于渲染 Revit GeometryObject 的渲染选项。
    /// </summary>
    public sealed class GeometryObjectOptions
    {
        /// <summary>
        /// 构造函数。
        /// 使用指定的 <see cref="GeometryObject"/> 和 Revit <see cref="Autodesk.Revit.DB.Material"/>。
        /// </summary>
        /// <param name="geometryObject">要渲染的几何对象。</param>
        /// <param name="material">用于渲染的 Revit 材质，可为空。</param>
        public GeometryObjectOptions(
            GeometryObject geometryObject,
            Autodesk.Revit.DB.Material material = null
        )
        {
            GeometryObject = geometryObject;
            if (material != null)
            {
                Material = material.ToPhongMaterial();
                this.Color4 = material.ToMyColorWithTransparency().ToColor4();
            }
            else
            {
                Material = MyColorWithTransparency.DefaultColor.ToPhongMaterial();
                this.Color4 = MyColorWithTransparency.DefaultColor;
            }
        }

        /// <summary>
        /// 构造函数。
        /// 使用指定的 <see cref="GeometryObject"/>、颜色和透明度。
        /// </summary>
        /// <param name="geometryObject">要渲染的几何对象。</param>
        /// <param name="color">用于渲染的颜色。</param>
        /// <param name="alpha">透明度（0~1 之间），1 表示完全不透明。</param>
        public GeometryObjectOptions(
            GeometryObject geometryObject,
            System.Windows.Media.Color color,
            float alpha = 1
        )
        {
            GeometryObject = geometryObject;
            if (color != null)
            {
                Material = color.ToColor4(alpha).ToPhongMaterial();
                Color4 = color.ToColor4(alpha);
            }
            else
            {
                Material = MyColorWithTransparency.DefaultColor.ToPhongMaterial();
                Color4 = MyColorWithTransparency.DefaultColor;
            }
        }

        /// <summary>
        /// 构造函数。
        /// 使用指定的 <see cref="GeometryObject"/>、贴图流、自发光颜色和透明度。
        /// </summary>
        /// <param name="geometryObject"> 要渲染的几何对象。</param>
        /// <param name="textureStream"> 贴图流。</param>
        /// <param name="emissiveColor"> 自发光颜色。</param>
        /// <param name="alpha"> 透明度（0~1 之间），1 表示完全不透明。</param>
        public GeometryObjectOptions(
            GeometryObject geometryObject,
            Stream textureStream,
            System.Windows.Media.Color emissiveColor,
            float alpha = 1
        )
        {
            this.GeometryObject = geometryObject;
            Material = new PBRMaterial()
            {
                EmissiveColor = emissiveColor.ToColor4(alpha),
                EmissiveMap = TextureModel.Create(textureStream),
                RenderEmissiveMap = true,
                MetallicFactor = 1.0f,
                RoughnessFactor = 0.05f,
                ReflectanceFactor = 0.5f,
                RenderEnvironmentMap = true,
            };
            Color4 = MyColorWithTransparency.DefaultColor;
        }

        private GeometryObjectOptions() { }

        /// <summary>
        /// 模型的渲染细节等级（0~1）。
        /// 数值越高，网格面越密集、精度越高，但性能消耗也越大。
        /// </summary>
        public double LevelOfDetail { get; set; } = 0.5;

        /// <summary>
        /// 三角面生成的最小夹角（弧度）。
        /// 用于控制网格生成时的平滑度。
        /// </summary>
        public double MinAngleInTriangle { get; set; } = 0;

        /// <summary>
        /// 相邻三角面之间的最小外角（弧度）。
        /// 用于判断曲面之间的平滑过渡程度。
        /// </summary>
        public double MinExternalAngleBetweenTriangles { get; set; } = Math.PI * 2;

        /// <summary>
        /// 是否绘制 Solid 的轮廓线。
        /// 如果为 true，将会在渲染时显示边界线条。
        /// </summary>
        public bool IsDrawSolidEdges { get; set; } = true;

        /// <summary>
        /// Solid 轮廓线的粗细（像素单位）。
        /// </summary>
        public float SolidEdgeThickness { get; set; } = 2f;

        /// <summary>
        /// Solid 轮廓线的平滑程度。
        /// 数值越大，边缘越平滑。
        /// </summary>
        public float SolidEdgeSmoothness { get; set; } = 10f;

        /// <summary>
        /// 要渲染的 Revit 几何对象。
        /// </summary>
        public GeometryObject GeometryObject { get; }

        internal Material Material { get; }

        internal Color4 Color4 { get; }

        //internal Color4 GetColor4(Document document)
        //{
        //    if (Material != null)
        //    {
        //        return Material.ToMyColorWithTransparency().ToColor4();
        //    }
        //    if (Color != null)
        //    {
        //        return Color.ToColor4(Alpha);
        //    }
        //    return MyColorWithTransparency.DefaultColor;
        //}
        //internal Material GetPhongMaterial(Document document)
        //{
        //    if (Material != null)
        //    {
        //        return Material.ToMyColorWithTransparency().ToColor4().ToPhongMaterial();
        //    }
        //    if (Color != null)
        //    {
        //        return Color.ToColor4(Alpha).ToPhongMaterial();
        //    }
        //    return MyColorWithTransparency.DefaultColor.ToPhongMaterial();
        //}

        /// <summary>
        /// 获取用于 HelixToolkit 渲染的 <see cref="Color4"/> 对象。
        /// 优先从 Revit 材质中读取颜色与透明度，
        /// 否则使用自定义颜色与 Alpha 值。
        /// </summary>
        /// <param name="document">当前 Revit 文档对象。</param>
        /// <returns>对应的 <see cref="Color4"/> 颜色。</returns>
    }
}
