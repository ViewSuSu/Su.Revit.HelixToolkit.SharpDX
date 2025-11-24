using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Su.Revit.HelixToolkit.SharpDX.Extensions
{
    /// <summary>
    /// RevitModel3DExtension - SharpDX 版本
    /// </summary>
    internal static class RevitModel3DExtension
    {
        /// <summary>
        /// 将几何对象转换为 Model3D
        /// </summary>
        internal static List<GeometryModel3D> ToGeometryModel3Ds(
            this GeometryObjectOptions geometryObject,
            Document document
        )
        {
            return geometryObject.GeometryObject switch
            {
                Solid solid => solid.Volume > 0
                    ? solid.ToMeshGeometryModel3D(document, geometryObject)
                    : [],
                Curve curve => [curve.ToLineModel3D(document, geometryObject.Color4)],
                Point point => [point.ToPointModel3D(document)],
                PolyLine polyLine => [polyLine.ToLineModel3D(document, geometryObject)],
                _ => [],
            };
        }
    }
}
