namespace TelegramLib.Helpers
{
    public class ColorHelper
    {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public ColorHelper(byte r, byte g, byte b)
        {
            A = byte.MaxValue;
            R = r;
            G = g;
            B = b;
        }

        public ColorHelper(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }
    }
}
