using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TelegramLib.MainClasses;
using TelegramVisualPart.Helper;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using MaterialDesignThemes.Wpf.Internal;
using System.Security.AccessControl;

namespace TelegramVisualPart.Pages.LittleMenuPages
{
    /// <summary>
    /// Логика взаимодействия для SelectSoundTone.xaml
    /// </summary>
    public partial class SelectSoundTone : Page
    {
        private TelSystem _system;
        private string _tempChosenSound;
        private int _tempChosenVol;

        public SelectSoundTone(TelSystem system)
        {
            InitializeComponent();

            _system = system;
            SetBasicParams();
        }

        public void SetBasicParams()
        {
            SetBasicRadios();

            //Set system params(tone + volume)
            SetTempChosenValues();

            //Set Radio row height
            SetWindowHeight();
        }

        public void SetWindowHeight()
        {
            double height = StackNameBlock.Height;

            for(int i = 0; i < RadioStack.Children.Count; i++)
            {
                if (RadioStack.Children[i] is RadioButton but)
                {
                    height += but.Height;
                    height += but.Margin.Top;
                    height += but.Margin.Bottom;
                }
            }

            height += AddSound.Height;
            height += AddSound.Margin.Top;
            height += AddSound.Margin.Bottom;

            RadioRow.Height = new GridLength(height);

            double workRowHeight = RadioRow.Height.Value +
                VolumeRow.Height.Value + TextRow.Height.Value + ButsRow.Height.Value;

            Height = workRowHeight + UpperRow.Height.Value;
        }

        void SetTempChosenValues()
        {
            _tempChosenSound = _system.Settings.SoundNotifSettings.GetChosenSound();
            SetBasicSound();

            _tempChosenVol = _system.Settings.SoundNotifSettings.GetVolume();
            PersSlider.Value = _tempChosenVol;
            PercentsNumberBlock.Text = _tempChosenVol.ToString();
        }

        public void SetBasicSound()
        {
            if(_tempChosenSound is null)
            {
                NouSound.IsChecked = true;
            } 
            for(int i = 0; i < RadioStack.Children.Count; i++)
            {
                if (RadioStack.Children[i] is RadioButton radioBut && 
                    radioBut.Content.ToString() == _tempChosenSound)
                {
                    radioBut.IsChecked = true;
                }
            }
        }

        public void SetBasicRadios()
        {
            //Set default + no sound
            for (int i = 0; i < _system.Settings.SoundNotifSettings.MesSounds.Count; i++)
            {
                AddSoundRadio(_system.Settings.SoundNotifSettings.MesSounds[i]);
            }

            //Move add Sound in the end
            MoveAddSoundAtTheEnd();
        }

        public void MoveAddSoundAtTheEnd()
        {
            RadioStack.Children.Remove(AddSound);
            RadioStack.Children.Add(AddSound);
        }

        public void AddSoundRadio(string soundName)
        {
            RadioButton toAdd = new RadioButton()
            {
                Padding = new Thickness(10, 0, 0, 2),
                Content = soundName,
                Style = (Style)FindResource("RadiButStyle")
            };

            toAdd.MouseEnter += Radio_MouseEnter;
            toAdd.MouseLeave += Radio_MouseLeave;
            toAdd.Click += RadioBut_Checked;

            RadioStack.Children.Add(toAdd);
        }

        public void Radio_MouseEnter(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        public void Radio_MouseLeave(object sender, RoutedEventArgs e)
        {
            Cursor = null;
        }

        private void But_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            if (sender is System.Windows.Controls.Button but) but.Background =
                (SolidColorBrush)Application.Current.Resources["OtherButMouseEnter"];
        }

        private void But_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            if (sender is System.Windows.Controls.Button but)
                but.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void SaveBut_Click(object sender, RoutedEventArgs e)
        {
            //Set save Action
            _system.Settings.SoundNotifSettings.SetChosenSound(_tempChosenSound);
            _system.Settings.SoundNotifSettings.SetVolume(_tempChosenVol);

            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void AddSound_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Выберите MP3 файл",
                Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*",
                Multiselect = false
            };

            if (dlg.ShowDialog() == true)
            {
                string filePath = dlg.FileName;

                //Add in vis part
                AddVisSoundFromFile(Path.GetFileName(filePath));

                //Set it correct folder
                FilesAction.AddNewNotifSound(filePath);

                //set it in system
                _system.Settings.SoundNotifSettings.AddSound(Path.GetFileName(filePath));

                //Set it in db            
            }
        }

        public void AddVisSoundFromFile(string fileName)
        {
            if (RadioStack.Children.OfType<RadioButton>().FirstOrDefault(x => x.Content.ToString() == fileName) is not null) return;
            AddSoundRadio(fileName);
            MoveAddSoundAtTheEnd();

            SetWindowHeight();
        }

        public void PlaySound()
        {
            string chosenSound = _tempChosenSound;
            if (chosenSound == string.Empty) return;

            //Get sound path from files
            string path = FilesAction.GetSoundPath(chosenSound);

            //Get volume
            int.TryParse(PercentsNumberBlock.Text, out int vol);

            //Play it
            VisHelper.PlaySound(path, (double)vol / 100);
        }
        
        public void PlayOnChangedSound(string soundName)
        {
            if(_tempChosenSound != soundName) return;
            PlaySound();
        }

        private void NouSound_Checked(object sender, RoutedEventArgs e)
        {
            _tempChosenSound = null;

            VolumeRow.Height = new GridLength(0);
            SetWindowHeight();
        }

        private void RadioBut_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not RadioButton but) return;

            _tempChosenSound = but.Content.ToString();

            PlayOnChangedSound(but.Content.ToString());

            VolumeRow.Height = new GridLength(90);
            SetWindowHeight();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PercentsNumberBlock is null) return;
            PercentsNumberBlock.Text = PersSlider.Value.ToString();
            _tempChosenVol = (int)PersSlider.Value;
        }

        private void PersSlider_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void PersSlider_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }
    }
}
