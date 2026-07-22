using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Point = System.Windows.Point;

namespace TelegramVisualPart.UserControls.SettingsControls.ChatSettingsControls.PaletteControls
{
    /// <summary>
    /// Логика взаимодействия для ColorPickerLine.xaml
    /// </summary>
    public partial class ColorPickerLine : UserControl
    {
        public GradientStop _stop;
        public double _lum;
        public event EventHandler SpecialConditionTriggered;

        public ColorPickerLine()
        {
            InitializeComponent();

            SetOwnGradientBrush();
        }

        public void SetOwnGradientBrush()
        {
            Point endPoint = new Point(1, 0);
            const double firstStop = 0.1;
            const double secStop = 1.0;
            const int recSize = 1;
            var gradient = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = endPoint
            };

            _stop = new GradientStop(Colors.Red, firstStop);
            gradient.GradientStops.Add(_stop);
            gradient.GradientStops.Add(new GradientStop(Colors.White, secStop));

            var geometryDrawing = new GeometryDrawing
            {
                Geometry = new RectangleGeometry(new Rect(0, 0, recSize, recSize)),
                Brush = gradient
            };

            var drawingGroup = new DrawingGroup();
            drawingGroup.Children.Add(geometryDrawing);

            var drawingBrush = new DrawingBrush
            {
                Drawing = drawingGroup,
                Stretch = Stretch.Fill
            };

            ColorSpecter.Fill = drawingBrush;

            _stop.Color = Colors.Blue;
        }

        const int _baseMargin = 7;
        private void SetMarginToTriangles(PackIcon icon, Point position)
        {
            const int sizeSub = 14;
            icon.Margin = new Thickness(
                position.X < 0 ? 0 :
                position.X > this.ActualWidth - sizeSub ? this.ActualWidth - sizeSub : position.X,
                0, 0, 0);
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
                Point clickPosition = e.GetPosition((IInputElement)sender);
                SetMarginToTriangles(UpperTriangle, clickPosition);
                SetMarginToTriangles(BottomTriangle, clickPosition);

                SetLumValue();

                SpecialConditionTriggered?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetLumValue()
        {
            const int maxDivider = 100;
            double onePart = ColorSpecter.ActualWidth / maxDivider;

            _lum = (UpperTriangle.Margin.Left / onePart) / maxDivider;
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
