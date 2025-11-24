using System.Windows.Media;

namespace Su.Revit.HelixToolkit.SharpDX.Utils
{
    internal struct MyColorWithTransparency
    {
        public static Color4 DefaultColor = Colors.Gray.ToColor4();

        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        public uint Transparency { get; set; }

        public MyColorWithTransparency(byte red, byte green, byte blue, uint transparency)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Transparency = transparency;
        }

        internal Color4 ToColor4()
        {
            return new Color4(Red / 255.0f, Green / 255.0f, Blue / 255.0f, Transparency / 255.0f);
        }
    }
}
