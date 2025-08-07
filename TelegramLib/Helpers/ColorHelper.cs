using System.Data.SqlTypes;

namespace TelegramLib.Helpers
{
    public class ColorHelper
    {
        public static (int R, int G, int B) _basicRGB = (128, 255, 128);

        public int Id { get; set; }
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public ColorHelper(int id, byte r, byte g, byte b)
        {
            Id = id;
            A = byte.MaxValue;
            R = r;
            G = g;
            B = b;
        }

        public ColorHelper(int id, byte a, byte r, byte g, byte b)
        {
            Id = id;
            A = a;
            R = r;
            G = g;
            B = b;
        }

        public ColorHelper(int id)
        {
            Id = id;
            A = byte.MaxValue;
            R = 128;
            G = 255;
            B = 128;
        }

        
    }
}
