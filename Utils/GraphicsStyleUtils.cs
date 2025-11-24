using Autodesk.Revit.DB;

namespace Su.Revit.HelixToolkit.SharpDX.Utils
{
    /// <summary>
    /// GraphicsStyleUtils
    /// </summary>
    internal static class GraphicsStyleUtils
    {
        /// <summary>
        /// GetLineColor
        /// </summary>
        /// <param name="document"></param>
        /// <param name="graphicsStyleId"></param>
        /// <returns></returns>
        internal static Autodesk.Revit.DB.Color GetLineColor(
            Document document,
            ElementId graphicsStyleId
        )
        {
            try
            {
                if (graphicsStyleId != ElementId.InvalidElementId)
                {
                    GraphicsStyle graphicsStyle =
                        document.GetElement(graphicsStyleId) as GraphicsStyle;

                    return graphicsStyle.GraphicsStyleCategory.LineColor;
                }
            }
            catch { }
            return null;
        }
    }
}
