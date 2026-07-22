using System;
using System.Collections.Generic;
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
using TelegramVisualPart.Enums.Menus;

namespace TelegramVisualPart.UserControls.ChatControls.LittleMenusContols
{
    /// <summary>
    /// Логика взаимодействия для MuteDurationBox.xaml
    /// </summary>
    public partial class MuteDurationBox : UserControl
    {
        public MuteDurationBox()
        {
            InitializeComponent();
        }

        private DatePartType _type;
        private  int _maxValue = 59; 

        public void SetParams(DatePartType type)
        {
            _type = type;

            SetAdditionalText();
            SetMaxNumber();
        }

        public void SetMaxNumber()
        {
            const int maxMin = 59;
            const int maxHour = 23;
            const int maxDay = 31;

            _maxValue =
                _type == DatePartType.Minute ? maxMin :
                _type == DatePartType.Hour ? maxHour :
                _type == DatePartType.Day ? maxDay :
                throw new Exception("WTF");
        }
    

        public void SetAdditionalText()
        {
            AdditionalTextBlock.Text =
                _type == DatePartType.Minute ? "Minutes" :
                _type == DatePartType.Hour ? "Hours" :
                _type == DatePartType.Day ? "Days" : 
                throw new Exception("WTF");
        }

        private void TimeBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //WTF is _
            e.Handled = !int.TryParse(e.Text, out _);

        }

        private void TimeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(TimeBox.Text, out int value))
            {
                if (value > _maxValue)
                {
                    TimeBox.Text = _maxValue.ToString();
                    TimeBox.CaretIndex = TimeBox.Text.Length;
                }
            }
        }
    }
}
