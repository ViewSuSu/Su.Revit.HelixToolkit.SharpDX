using Autodesk.Revit.DB;

namespace Su.Revit.HelixToolkit.SharpDX.Extensions
{
    /// <summary>
    /// RevitPoint3DExtension
    /// </summary>
    internal static class RevitPoint3DExtension
    {
        /// <summary>
        /// 将 Revit 点转换为 PointGeometryModel3D
        /// </summary>
        internal static PointGeometryModel3D ToPointModel3D(
            this Autodesk.Revit.DB.Point point,
            Document document
        )
        {
            var geometry = new PointGeometry3D
            {
                Positions = new Vector3Collection([point.Coord.ToVector3()]),
            };

            var color = RevitGeometryColorUtils.GetColor(document, point);

            return new PointGeometryModel3D
            {
                Geometry = geometry,
                Color = color.ToColor(),
                Size = new System.Windows.Size(5, 5),
            };
        }
    }
}
