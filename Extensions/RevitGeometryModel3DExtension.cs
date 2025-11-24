using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Su.Revit.HelixToolkit.SharpDX.Extensions
{
    /// <summary>
    /// RevitGeometryModel3DExtension - SharpDX 版本（支持透明度和顶点颜色）
    /// </summary>
    internal static class RevitGeometryModel3DExtension
    {
        /// <summary>
        /// 将 Revit Solid 转换为 MeshGeometryModel3D
        /// </summary>
        internal static List<GeometryModel3D> ToMeshGeometryModel3D(
            this Solid solid,
            Document document,
            GeometryObjectOptions geometryObjectModel
        )
        {
            var meshGeometry3ds = solid.ToMeshGeometry3D(document, geometryObjectModel);
            return meshGeometry3ds; //meshGeometry3ds.ToGroupModel3D();
        }

        ///// <summary>
        ///// 将 Revit Mesh 转换为 MeshGeometryModel3D
        ///// </summary>
        //internal static MeshGeometryModel3D ToMeshGeometryModel3D(
        //    this Face face,
        //    Document document,
        //    VisualOptions visualOptions
        //)
        //{
        //    var geometry = face.ToMeshGeometry3D(document, visualOptions);
        //    return geometry.ToMeshGeometryModel3D();
        //}

        //internal static MeshGeometryModel3D ToMeshGeometryModel3D(this Mesh mesh, Document document)
        //{
        //    var geometry = mesh.ToMeshGeometry3D(document);
        //    return geometry.ToMeshGeometryModel3D();
        //}

        /// <summary>
        /// 将 MeshGeometry3D 转换为 MeshGeometryModel3D
        /// </summary>
        internal static MeshGeometryModel3D ToMeshGeometryModel3D(
            this MeshGeometry3D geometry,
            GeometryObjectOptions geometryObjectModel
        )
        {
            var meshGeometryModel3D = new MeshGeometryModel3D
            {
                Geometry = geometry,
                CullMode = CullMode.Back,
                Material = geometryObjectModel.Material,
            };
            return meshGeometryModel3D;
        }
    }
}
