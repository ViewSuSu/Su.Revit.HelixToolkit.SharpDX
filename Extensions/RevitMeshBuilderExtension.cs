using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Autodesk.Revit.DB;
using Document = Autodesk.Revit.DB.Document;

namespace Su.Revit.HelixToolkit.SharpDX.Extensions
{
    /// <summary>
    /// RevitMeshBuilderExtension - SharpDX 版本
    /// </summary>
    internal static class RevitMeshBuilderExtension
    {
        public static Color4 DefaultColor = Colors.Gray.ToColor4();

        internal static List<GeometryModel3D> ToMeshGeometry3D(
            this Solid solid,
            Document document,
            GeometryObjectOptions geometryObjectModel
        )
        {
            List<GeometryModel3D> meshGeometry3Ds = [];
            using var solidOrShellTessellationControls = new SolidOrShellTessellationControls()
            {
                LevelOfDetail = geometryObjectModel.LevelOfDetail,
                MinAngleInTriangle = geometryObjectModel.MinAngleInTriangle,
                MinExternalAngleBetweenTriangles =
                    geometryObjectModel.MinExternalAngleBetweenTriangles,
            };
            var shell = SolidUtils.TessellateSolidOrShell(solid, solidOrShellTessellationControls);
            var face = solid.Faces.Cast<Face>().FirstOrDefault();
            var meshBuilder = new MeshBuilder();
            for (int i = 0; i < shell.ShellComponentCount; i++)
            {
                using var triangulatedShellComponent = shell.GetShellComponent(i);
                using var triangulationInterfaceForTriangulatedShellComponent =
                    new TriangulationInterfaceForTriangulatedShellComponent(
                        triangulatedShellComponent
                    );
                IList<TriOrQuadFacet> triOrQuadFacets = FacetingUtils.ConvertTrianglesToQuads(
                    triangulationInterfaceForTriangulatedShellComponent
                );
                foreach (TriOrQuadFacet triOrQuadFacet in triOrQuadFacets)
                {
                    var triangleCorners = new List<Vector3>();
                    for (int j = 0; j < triOrQuadFacet.NumberOfVertices; j++)
                    {
                        var index = triOrQuadFacet.GetVertexIndex(j);
                        triangleCorners.Add(
                            triangulatedShellComponent.GetVertex(index).ToVector3()
                        );
                    }
                    if (triangleCorners.Count != 0)
                    {
                        meshBuilder.AddPolygon(triangleCorners);
                    }
                }
            }
            var meshGeometry3D = meshBuilder.ToMeshGeometry3D();
            //meshGeometry3D.Colors =
            //[
            //    .. Enumerable.Repeat(geometryObjectModel.GetColor4(document), 1),
            //];
            meshGeometry3Ds.Add(meshGeometry3D.ToMeshGeometryModel3D(geometryObjectModel));
            //foreach (Face face in solid.Faces)
            //{
            //    var mesh = face.Triangulate(visualOptions.LevelOfDetail);
            //    if (mesh == null)
            //    {
            //        continue;
            //    }
            //    ;
            //    var triangleCorners = new Vector3[3];
            //    List<Color4> colors = new List<Color4>();
            //    var meshBuilder = new MeshBuilder();
            //    var color = Colors.Red.ToColor4(); //RevitGeometryColorUtils.GetColor(document, face);
            //    for (int i = 0; i < mesh.NumTriangles; i++)
            //    {
            //        MeshTriangle triangle = mesh.get_Triangle(i);
            //        triangleCorners[0] = triangle.get_Vertex(0).ToVector3();
            //        triangleCorners[1] = triangle.get_Vertex(1).ToVector3();
            //        triangleCorners[2] = triangle.get_Vertex(2).ToVector3();
            //        meshBuilder.AddPolygon(triangleCorners);
            //    }
            //    var meshGeometry3D = meshBuilder.ToMeshGeometry3D();
            //    meshGeometry3D.Colors = new Color4Collection(
            //        Enumerable.Repeat(color, mesh.NumTriangles)
            //    );
            //    meshGeometry3Ds.Add(meshGeometry3D.ToMeshGeometryModel3D());
            //}
            if (geometryObjectModel.IsDrawSolidEdges)
            {
                foreach (Edge edge in solid.Edges)
                {
                    meshGeometry3Ds.Add(edge.ToLineModel3D(document, geometryObjectModel));
                }
            }
            return meshGeometry3Ds;
        }

        ///// <summary>
        ///// 创建网格几何
        ///// </summary>
        //internal static MeshGeometry3D ToMeshGeometry3D(
        //    this Face face,
        //    Document document,
        //    GeometryObjectModel visualOptions
        //)
        //{
        //    var meshBuilder = new MeshBuilder(false, false);
        //    var triangleCorners = new Vector3[3];
        //    var mesh = face.Triangulate(visualOptions.LevelOfDetail);
        //    MeshGeometry3D geometry = null;
        //    Color4 color = default;
        //    if (mesh != null)
        //    {
        //        color = RevitGeometryColorUtils.GetColor(document, mesh);
        //        for (int i = 0; i < mesh.NumTriangles; i++)
        //        {
        //            MeshTriangle triangle = mesh.get_Triangle(i);
        //            triangleCorners[0] = triangle.get_Vertex(0).ToVector3();
        //            triangleCorners[1] = triangle.get_Vertex(1).ToVector3();
        //            triangleCorners[2] = triangle.get_Vertex(2).ToVector3();
        //            meshBuilder.AddPolygon(triangleCorners);
        //        }
        //    }
        //    geometry = meshBuilder.ToMeshGeometry3D();
        //    if (mesh != null)
        //        geometry.Colors = new Color4Collection(Enumerable.Repeat(color, mesh.NumTriangles));
        //    return geometry;
        //}

        //internal static MeshGeometry3D ToMeshGeometry3D(this Mesh mesh, Document document)
        //{
        //    var meshBuilder = new MeshBuilder(false, false);
        //    var triangleCorners = new Vector3[3];
        //    var color = RevitGeometryColorUtils.GetColor(document, mesh);
        //    for (int i = 0; i < mesh.NumTriangles; i++)
        //    {
        //        MeshTriangle triangle = mesh.get_Triangle(i);
        //        triangleCorners[0] = triangle.get_Vertex(0).ToVector3();
        //        triangleCorners[1] = triangle.get_Vertex(1).ToVector3();
        //        triangleCorners[2] = triangle.get_Vertex(2).ToVector3();
        //        meshBuilder.AddPolygon(triangleCorners);
        //    }
        //    var geometry = meshBuilder.ToMeshGeometry3D();
        //    geometry.Colors = [.. Enumerable.Repeat(color, mesh.NumTriangles)];
        //    return geometry;
        //}

        ///// <summary>
        ///// AddRevitMesh
        ///// </summary>
        ///// <param name="meshBuilder"></param>
        ///// <param name="mesh"></param>
        //public static void AddRevitMesh(this MeshBuilder meshBuilder, Mesh mesh)
        //{
        //    var triangleCorners = new Vector3[3];
        //    for (int i = 0; i < mesh.NumTriangles; i++)
        //    {
        //        MeshTriangle triangle = mesh.get_Triangle(i);
        //        triangleCorners[0] = triangle.get_Vertex(0).ToVector3();
        //        triangleCorners[1] = triangle.get_Vertex(1).ToVector3();
        //        triangleCorners[2] = triangle.get_Vertex(2).ToVector3();
        //        meshBuilder.AddPolygon(triangleCorners);
        //    }
        //}
    }
}
