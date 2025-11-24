using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Su.Revit.HelixToolkit.SharpDX.Utils
{
    public enum TextureType
    {
        /// <summary>
        /// 混凝土
        /// </summary>
        Concrete,

        /// <summary>
        /// 钢筋
        /// </summary>
        Rebar,
    }

    public static class TextureResourcesUtils
    {
        /// <summary>
        /// 获取贴图资源流
        /// </summary>
        /// <param name="textureType"></param>
        /// <returns></returns>
        public static Stream GetDefaultTextureStream(TextureType textureType)
        {
            string baseNs = Path.GetFileNameWithoutExtension(
                typeof(TextureResourcesUtils).Assembly.Location
            );
            return typeof(TextureResourcesUtils).Assembly.GetManifestResourceStream(
                $"{baseNs}.Resources.Textures.{textureType}.jpg"
            );
        }
    }
}
