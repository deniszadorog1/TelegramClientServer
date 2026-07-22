using System;
using System.CodeDom;
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
using static MaterialDesignThemes.Wpf.Theme;
using TextBox = System.Windows.Controls.TextBox;

namespace TelegramVisualPart.UserControls.ChatControls.ChatButsControls
{
    /// <summary>
    /// Логика взаимодействия для TextBoxMenu.xaml
    /// </summary>
    public partial class TextBoxMenu : UserControl
    {
        private TextBox _box;
        private List<string> _history;
        private int _histIndex;

        public event Action UnReDoAction;
        public event Action SetPhoto;

        public TextBoxMenu(TextBox box, List<string> history)
        {
            _box = box;
            _history = history;
            _histIndex = history.FindIndex(s => s == box.Text);

            InitializeComponent();

            SetBasicParams();
        }

        public void SetBasicParams()
        {
            Undo.SetTextParams("Undo", "Ctrl+Z");
            Redo.SetTextParams("Redo", "Ctrl+Y");
            Cut.SetTextParams("Undo", "Ctrl+X");
            Copy.SetTextParams("Copy", "Ctrl+C");
            Paste.SetTextParams("Paste", "Ctrl+V");
            Delete.SetTextParams("Delete", string.Empty);
            SelectAll.SetTextParams("SelectAll", "Ctrl+A");
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not TextBoxMenuButton but) return;
            SetActions(but);
        }

        public void SetActions(TextBoxMenuButton button)
        {
            const int changeIndex = 1;
            if (button == Undo)
            {
                if (_histIndex - changeIndex < 0) return;
                _histIndex--;
                UnReDoAction?.Invoke();
                _box.Text = _history[_histIndex];
            }
            else if (button == Redo)
            {
                if (_histIndex + changeIndex >= _history.Count()) return;
                _histIndex++;
                UnReDoAction?.Invoke();
                _box.Text = _history[_histIndex];
            }
            else if (button == Cut)
            {
                Clipboard.SetText(_box.SelectedText);
                _box.SelectedText = string.Empty;
            }
            else if (button == Copy)
            {
                Clipboard.SetText(_box.SelectedText);

            }
            else if (button == Paste)
            {
                if (Clipboard.ContainsFileDropList())
                {
                    SetPhoto?.Invoke();
                    return;
                }
                _box.SelectedText = Clipboard.GetText();
            }
            else if (button == Delete)
            {
                _box.Text = string.Empty;
            }
            else if (button == SelectAll)
            {
                _box.Focus();
                _box.Select(0, _box.Text.Length);
            }
        }

        public void SetEnableStatus(bool isBoxSiEmpty)
        {
            if (isBoxSiEmpty)
            {
                Undo.SetEnableStatus(false);
                Redo.SetEnableStatus(false);
                Cut.SetEnableStatus(false);
                Copy.SetEnableStatus(false);
                Delete.SetEnableStatus(false);
                SelectAll.SetEnableStatus(false);
                return;
            }
            Undo.SetEnableStatus(true);
            Redo.SetEnableStatus(true);
            Cut.SetEnableStatus(true);
            Copy.SetEnableStatus(true);
            Delete.SetEnableStatus(true);
            SelectAll.SetEnableStatus(true);
        }
    }
}
