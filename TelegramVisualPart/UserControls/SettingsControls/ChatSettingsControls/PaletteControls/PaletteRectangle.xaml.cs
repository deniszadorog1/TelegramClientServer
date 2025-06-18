using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using TelegramVisualPart.Services;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;


namespace TelegramVisualPart.UserControls.SettingsControls.ChatSettingsControls.PaletteControls
{
    /// <summary>
    /// Логика взаимодействия для PaletteRectangle.xaml
    /// </summary>
    public partial class PaletteRectangle : UserControl
    {
        Cursor _fanCurs;
        internal ColorConvertService _tempColor = new ColorConvertService(255, 120, 100, 30);
        private SolidColorBrush _colorToPaint = new SolidColorBrush(Color.FromArgb(255, 0, 154, 0));

        public event EventHandler SpecialConditionTriggered;
        private Point _colorPoint = new Point(0, 0);

        public PaletteRectangle()
        {
            InitializeComponent();

            string dir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            dir += "\\Visuals\\Cursors\\tungTungSahurCursor.cur";
            _fanCurs = new Cursor(dir);

            SetBasicParams();
        }

        public void SetBasicParams()
        {
            _tempColor.R = _colorToPaint.Color.R;
            _tempColor.G = _colorToPaint.Color.G;
            _tempColor.B = _colorToPaint.Color.B;


            _tempColor.RGBtoHSL(_colorToPaint.Color);

            System.Windows.Media.Color color =
                 GetColorWithThHighestLuminosity(_colorToPaint.Color);

            (int, int)? colorCord = GetColorCord(color.R, color.G, color.B);

            InitCordInSpec(colorCord.Value.Item1, colorCord.Value.Item2);
        }

        public void SetBoxes()
        {

        }

        public void InitCordInSpec(int y, int x)
        {
            Canvas.SetTop(CircleBut, y);
            Canvas.SetLeft(CircleBut, x);
        }

        public (int, int)? GetColorCord(byte r, byte g, byte b) 
        {
            Color targetColor = Color.FromArgb(255, r, g, b);
            const int argbMult = 4;
            const int gStep = 1;
            const int rStep = 2;
            const int aStep = 3;


            System.Windows.Controls.Image specimage = ConvertRectangleFillToImage();
            RenderTargetBitmap renderTarget = specimage.Source as RenderTargetBitmap;

            int width = renderTarget.PixelWidth;
            int height = renderTarget.PixelHeight;
            int stride = width * argbMult;
            byte[] pixels = new byte[height * stride];
            renderTarget.CopyPixels(pixels, stride, 0);


            int closestR = byte.MinValue;
            int closetG = byte.MinValue;
            int closetB = byte.MinValue;

            int closetX = -1;
            int closetY = -1;

            double minDistance = double.MaxValue;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * argbMult;
                    byte bCheck = pixels[index];
                    byte gCheck = pixels[index + gStep];
                    byte rCheck = pixels[index + rStep];
                    byte aCheck = pixels[index + aStep];

                    if (rCheck == targetColor.R && gCheck == targetColor.G &&
                        bCheck == targetColor.B && aCheck == targetColor.A)
                    {
                        return (y, x);
                    }

                    double distance = Math.Sqrt(
                      Math.Pow(rCheck - targetColor.R, 2) +
                      Math.Pow(gCheck - targetColor.G, 2) +
                      Math.Pow(bCheck - targetColor.B, 2) +
                      Math.Pow(aCheck - targetColor.A, 2)
                  );

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closetX = x;
                        closetY = y;
                    }
                }
            }

            if (minDistance == double.MaxValue) return null;

            return (closetY, closetX);
        }

        private const int _dpi = 96;
        public System.Windows.Controls.Image ConvertRectangleFillToImage()
        {
            int width = (int)this.Width;
            int height = (int)this.Height;

            RenderTargetBitmap renderTarget = new RenderTargetBitmap(
                width, height, _dpi, _dpi, PixelFormats.Pbgra32);

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(RectPalette.Fill, null, new Rect(0, 0, width, height));
            }
            renderTarget.Render(drawingVisual);
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = renderTarget;
            return image;
        }

        private System.Windows.Media.Color GetColorWithThHighestLuminosity(System.Windows.Media.Color color)
        {
            const int startLum = 1;
            ColorConvertService tempColor = new ColorConvertService(color);
            tempColor.Luminance = startLum;
            System.Windows.Media.Color res = tempColor.HSLtoRGB();
            return res;
        }


        private System.Windows.Media.Color GetColorAtPosition
                (System.Windows.Shapes.Rectangle rectangle, int x, int y)
        {
            const int startLocParam = 1;
            const int amountOfPixels = 4;

            const int firstpixelIndex = 3;
            const int secondPixelIndex = 2;
            const int thirdPixelIndex = 1;
            const int forthPixelIndex = 0;

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                (int)rectangle.Width, (int)rectangle.Height, _dpi, _dpi, PixelFormats.Pbgra32);
            rectangle.Measure(new System.Windows.Size((int)rectangle.Width, (int)rectangle.Height));
            rectangle.Arrange(new Rect(new System.Windows.Size((int)rectangle.Width, (int)rectangle.Height)));
            renderTargetBitmap.Render(rectangle);

            var croppedBitmap = new CroppedBitmap(renderTargetBitmap,
                new Int32Rect(x, y, startLocParam, startLocParam));
            byte[] pixels = new byte[amountOfPixels];
            croppedBitmap.CopyPixels(pixels, amountOfPixels, 0);

            return System.Windows.Media.Color.FromArgb(pixels[firstpixelIndex],
                pixels[secondPixelIndex], pixels[thirdPixelIndex], pixels[forthPixelIndex]);
        }

        public void UpdateShowColor()
        {
            System.Windows.Media.Color mediaColor =
            GetColorAtPosition(RectPalette,
            (int)_colorPoint.X, (int)_colorPoint.Y);

            //HexTable.Text = GetHexFromColor(mediaColor.R, mediaColor.G, mediaColor.B);
            ReInitColorShowColorPanel(mediaColor);
        }
        public void ReInitColorShowColorPanel(System.Windows.Media.Color color)
        {
            const int lumDevider = 100;
            _tempColor = new ColorConvertService(color);
            _tempColor.RGBtoHSL(color);
/*            double lum = _pallete.TempL;
            lum /= lumDevider;
            _tempColor.Luminance = lum;
            System.Windows.Media.Color newColor = _tempColor.HSLtoRGB();

            HexTable.InfoTextBox.Text = _tempColor.GetHexFromRGB();
            ChosenColorShow.Background = new SolidColorBrush(newColor);

            InitStartLuminanceGradientValue(color);
            UpdateColorParams(newColor);*/
        }





        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = _fanCurs;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private bool _isDragging = false;
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _isDragging = true;
                Mouse.Capture(this);
            }
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                _colorPoint = e.GetPosition(this);

                Canvas.SetLeft(CircleBut, _colorPoint.X );
                Canvas.SetTop(CircleBut, _colorPoint.Y );

                if (_colorPoint.X > this.ActualWidth - 1) _colorPoint.X = this.ActualWidth- 1;
                else if (_colorPoint.X < 0) _colorPoint.X = 0;

                if (_colorPoint.Y > this.ActualHeight - 1) _colorPoint.Y = this.ActualHeight - 1;
                else if (_colorPoint.Y < 1) _colorPoint.Y = 1;


                Canvas.SetLeft(CircleBut, _colorPoint.X);
                Canvas.SetTop(CircleBut, _colorPoint.Y);

                UpdateShowColor();

                SpecialConditionTriggered?.Invoke(this, EventArgs.Empty);
            }
        }

        public void AdjustBrushLuminosity(/*double factor,*/ double bright)
        {
            var originalBrush = (DrawingBrush)this.Resources["SpectrumWithVerticalFade"];
            var clonedBrush = originalBrush.Clone();

            RectPalette.Fill = clonedBrush;

            if (clonedBrush.Drawing is DrawingGroup drawingGroup)
            {
                foreach (var drawing in drawingGroup.Children)
                {
                    if (drawing is GeometryDrawing geo && geo.Brush is LinearGradientBrush lgb)
                    {
                        foreach (var stop in lgb.GradientStops)
                        {
                            Color oldColor = stop.Color;


                            byte r = (byte)(oldColor.R + (Colors.White.R - oldColor.R) * bright);
                            byte g = (byte)(oldColor.G + (Colors.White.G - oldColor.G) * bright);
                            byte b = (byte)(oldColor.B + (Colors.White.B - oldColor.B) * bright);
                            stop.Color =  Color.FromArgb(oldColor.A, r, g, b);

                      /*      
                            byte r = Clamp((int)(oldColor.R * factor));
                            byte g = Clamp((int)(oldColor.G * factor));
                            byte b = Clamp((int)(oldColor.B * factor));

                            stop.Color = Color.FromArgb(oldColor.A, r, g, b);*/
                        }
                    }
                }
            }
        }

        // Допоміжна функція для обмеження значення в межах 0–255
        private byte Clamp(int value)
        {
            return (byte)(value < 0 ? 0 : value > 255 ? 255 : value);
        }



        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                Mouse.Capture(null);
            }
        }
    }
}
