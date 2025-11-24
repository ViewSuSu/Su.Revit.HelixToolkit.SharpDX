using System.Windows.Media;
using Autodesk.Revit.DB;

namespace Su.Revit.HelixToolkit.SharpDX.Extensions
{
    /// <summary>
    /// RevitLineModel3DExtension - SharpDX 版本
    /// </summary>
    internal static class RevitLineModel3DExtension
    {
        /// <summary>
        /// 将 Revit 曲线转换为 LineGeometryModel3D
        /// </summary>
        internal static LineGeometryModel3D ToLineModel3D(
            this Curve curve,
            Document document,
            Color4 color,
            float thickness = 2f,
            float smoothness = 10f
        )
        {
            var points = curve.Tessellate();
            var geometry = new LineGeometry3D
            {
                Positions = new Vector3Collection(points.Count),
                Indices = new IntCollection((points.Count - 1) * 2),
            };

            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                geometry.Positions.Add(point.ToVector3());

                if (i < points.Count - 1)
                {
                    geometry.Indices.Add(i);
                    geometry.Indices.Add(i + 1);
                }
            }
            return CreateLineGeometry3D(geometry, color, thickness, smoothness);
        }

        /// <summary>
        /// 将 Revit 边转换为 LineGeometryModel3D
        /// </summary>
        internal static LineGeometryModel3D ToLineModel3D(
            this Edge edge,
            Document document,
            GeometryObjectOptions geometryObjectModel
        )
        {
            return edge.AsCurve()
                .ToLineModel3D(
                    document,
                    Colors.Black.ToColor4(),
                    geometryObjectModel.SolidEdgeThickness,
                    geometryObjectModel.SolidEdgeSmoothness
                );
        }

        /// <summary>
        /// 将多段线转换为 LineGeometryModel3D
        /// </summary>
        internal static LineGeometryModel3D ToLineModel3D(
            this PolyLine polyLine,
            Document document,
            GeometryObjectOptions geometryObjectModel
        )
        {
            var points = polyLine.GetCoordinates();
            var geometry = new LineGeometry3D
            {
                Positions = new Vector3Collection(points.Count),
                Indices = new IntCollection(points.Count * 2),
            };
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                geometry.Positions.Add(point.ToVector3());
                geometry.Indices.Add(i);
                geometry.Indices.Add((i + 1) % points.Count);
            }
            var color = geometryObjectModel.Color4;
            return CreateLineGeometry3D(geometry, color, 2f);
        }

        private static LineGeometryModel3D CreateLineGeometry3D(
            LineGeometry3D geometry,
            Color4 color,
            float thickness,
            float smoothness = 10f
        )
        {
            return new LineGeometryModel3D
            {
                Geometry = geometry,
                Color = color.ToColor(),
                Thickness = thickness,
                Smoothness = smoothness,
            };
        }
    }
}
