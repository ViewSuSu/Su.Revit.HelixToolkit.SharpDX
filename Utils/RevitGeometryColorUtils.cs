using Autodesk.Revit.DB;

namespace Su.Revit.HelixToolkit.SharpDX.Utils
{
    /// <summary>
    /// RevitGeometryColorUtils - SharpDX 版本（支持透明度）
    /// </summary>
    internal static class RevitGeometryColorUtils
    {
        ///// <summary>
        ///// 获取面的颜色（包含透明度）
        ///// </summary>
        //internal static Color4 GetColor(Document document, Face face)
        //{
        //    var colorTrans = MaterialUtils.GetMaterialColorWithTransparency(
        //        document,
        //        face.MaterialElementId
        //    );
        //    float alpha = 1.0f;

        //    if (!colorTrans.Equals(default))
        //    {
        //        alpha = colorTrans.Transparency / 255.0f;
        //    }

        //    Color4 color;
        //    if (!colorTrans.Equals(default))
        //    {
        //        color = colorTrans.ToColor4(); // 内部已经包含透明度
        //    }
        //    else
        //    {
        //        color = new Color4(0.8f, 0.8f, 0.8f, alpha); // 默认浅灰
        //    }

        //    if (color.Equals(Color4.White))
        //    {
        //        if (face.IsTwoSided)
        //        {
        //            var lineColor = GraphicsStyleUtils.GetLineColor(document, face.GraphicsStyleId);
        //            color = lineColor.ToColor4(alpha);
        //        }

        //        if (color.Equals(Color4.White))
        //            color = new Color4(0.8f, 0.8f, 0.8f, alpha);
        //    }

        //    return color;
        //}

        ///// <summary>
        ///// 获取网格的颜色（包含透明度）
        ///// </summary>
        //internal static Color4 GetColor(Document document, Mesh mesh)
        //{
        //    var colorTrans = MaterialUtils.GetMaterialColorWithTransparency(
        //        document,
        //        mesh.MaterialElementId
        //    );
        //    float alpha = 1.0f;

        //    if (colorTrans.Equals(default(MyColorWithTransparency)))
        //        alpha = colorTrans.Transparency / 255.0f;

        //    Color4 color = !colorTrans.Equals(default)
        //        ? colorTrans.ToColor4()
        //        : new Color4(0.8f, 0.8f, 0.8f, alpha);

        //    if (color.Equals(Color4.White))
        //        color = new Color4(0.8f, 0.8f, 0.8f, alpha);

        //    return color;
        //}

        /// <summary>
        /// 获取几何对象的颜色（包含透明度）
        /// </summary>
        internal static Color4 GetColor(Document document, GeometryObject geometryObject)
        {
            var lineColor = GraphicsStyleUtils.GetLineColor(
                document,
                geometryObject.GraphicsStyleId
            );
            float alpha = 1.0f;

            Color4 color = lineColor.ToColor4(alpha);

            if (color.Equals(Color4.White))
                color = new Color4(0.2f, 0.2f, 0.8f, alpha); // 默认蓝色

            return color;
        }
    }
}
