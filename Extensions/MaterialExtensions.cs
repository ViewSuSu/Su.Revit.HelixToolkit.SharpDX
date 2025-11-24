using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Su.Revit.HelixToolkit.SharpDX.Extensions
{
    internal static class MaterialExtensions
    {
        internal static PhongMaterial ToPhongMaterial(this Color4 color4)
        {
            return new PhongMaterial { DiffuseColor = color4, EmissiveColor = color4 };
        }

        internal static PhongMaterial ToPhongMaterial(this Autodesk.Revit.DB.Color color4)
        {
            return new PhongMaterial
            {
                DiffuseColor = color4.ToColor4(),
                EmissiveColor = color4.ToColor4(),
            };
        }

        internal static PhongMaterial ToPhongMaterial(this Autodesk.Revit.DB.Material material)
        {
            var color4 = material.ToMyColorWithTransparency().ToColor4();
            return new PhongMaterial { DiffuseColor = color4, EmissiveColor = color4 };
        }
    }
}
