using Autodesk.Revit.DB;

namespace Su.Revit.HelixToolkit.SharpDX.Extensions
{
    /// <summary>
    /// RevitVector3DExtension
    /// </summary>
    internal static class RevitVector3DExtension
    {
        internal static Vector3 ToVector3(this XYZ vector)
        {
            return new Vector3((float)vector.X, (float)vector.Y, (float)vector.Z);
        }
    }
}
