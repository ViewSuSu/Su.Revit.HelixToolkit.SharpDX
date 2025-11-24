using Autodesk.Revit.DB;

namespace Su.Revit.HelixToolkit.SharpDX.Utils
{
    /// <summary>
    /// MaterialUtils
    /// </summary>
    internal static class MaterialUtils
    {
        internal static MyColorWithTransparency ToMyColorWithTransparency(
            this Autodesk.Revit.DB.Material material
        )
        {
            return GetMaterialColorWithTransparency(material.Document, material.Id);
        }

        /// <summary>
        /// GetMaterialColorWithTransparency
        /// </summary>
        /// <param name="document"></param>
        /// <param name="materialId"></param>
        /// <returns></returns>
        internal static MyColorWithTransparency GetMaterialColorWithTransparency(
            Document document,
            ElementId materialId
        )
        {
            try
            {
                if (materialId != ElementId.InvalidElementId)
                {
                    if (document.GetElement(materialId) is not Autodesk.Revit.DB.Material material)
                        return default;

                    var colorMaterial = material.Color;
                    var transparency0To255 = (uint)((1.0 - material.Transparency / 100.0) * 255.0);

                    return new MyColorWithTransparency(
                        colorMaterial.Red,
                        colorMaterial.Green,
                        colorMaterial.Blue,
                        transparency0To255
                    );
                }
            }
            catch { }
            return default;
        }
    }
}
