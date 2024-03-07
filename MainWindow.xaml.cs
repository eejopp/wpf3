using System;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        CommonOpenFilePicker filePicker = new CommonOpenFilePicker();
        List<string> historyList = new List<string>();
        DispatcherTimer timer;
        bool? IsRepeat { get; set; } = false;
        bool IsPlaying { get; set; } = false;

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
            myMediaElement.MediaEnded += MyMediaElement_MediaEnded;
            
        }

        private void MyMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (IsRepeat.Value)
            {
                myMediaElement.Play();
                IsPlaying = true;
            }
            else
            {
                PlayNext();
            }
        }

        void PlayNext()
        {
            int selectedIndex = lbMusicList.SelectedIndex;
            if (selectedIndex < lbMusicList.Items.Count - 1)
            {
                lbMusicList.SelectedIndex = selectedIndex + 1;
            }
            else lbMusicList.SelectedIndex = 0;
        }

        void PlayPrev()
        {
            int selectedIndex = lbMusicList.SelectedIndex;
            if (selectedIndex > 0)
            {
                lbMusicList.SelectedIndex = selectedIndex - 1;
            }
            else
            {
                myMediaElement.Stop();
                IsPlaying = false;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {

            timelineSlider.Value = myMediaElement.Position.TotalSeconds;
            tbTimer.Text = myMediaElement.Position.ToString(@"hh\:mm\:ss");
            if (myMediaElement.NaturalDuration.HasTimeSpan)
                tbTimer2.Text =(myMediaElement.NaturalDuration.TimeSpan - myMediaElement.Position).ToString(@"hh\:mm\:ss");
        }

        void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            filePicker.SelectFolder();
            if (filePicker.musicFiles.Any())
            {
                lbMusicList.ItemsSource = null;
                lbMusicList.ItemsSource = filePicker.musicFiles;
                lbMusicList.SelectedIndex = 0;
                //Play(filePicker.musicFiles[0]);
            }
            else
            {
                MessageBox.Show("No supported audio files found in the selected folder.");
            }

        }

        void Play(string path)
        {
            myMediaElement.Source = new Uri(path);
            myMediaElement.Play();
            historyList.Add(myMediaElement.Source.OriginalString);
            timelineSlider.Value = 0;
            IsPlaying = true;
            if (myMediaElement.NaturalDuration.HasTimeSpan)
                timelineSlider.Maximum = myMediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;


        }

        // Play the media.
        void OnMouseDownPlayMedia(object sender, RoutedEventArgs e)
        {

            // The Play method will begin the media if it is not currently active or
            // resume media if it is paused. This has no effect if the media is
            // already running.
            if (IsPlaying)
            {
                myMediaElement.Pause();
                IsPlaying = false;
            }
            else
            {
                IsPlaying = true;
                myMediaElement.Play();
            }
            
            // Initialize the MediaElement property values.
            InitializePropertyValues();
        }

        // Pause the media.
        void OnMouseDownPauseMedia(object sender, RoutedEventArgs e)
        {

            // The Pause method pauses the media if it is currently running.
            // The Play method can be used to resume.
            myMediaElement.Pause();
        }

        // Stop the media.
        void OnMouseDownStopMedia(object sender, RoutedEventArgs e)
        {

            // The Stop method stops and resets the media to be played from
            // the beginning.
            myMediaElement.Stop();
            IsPlaying = false;
            
        }

        // Change the volume of the media.
        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            myMediaElement.Volume = (double)volumeSlider.Value;
        }

        // Change the speed of the media.
        private void ChangeMediaSpeedRatio(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
           // myMediaElement.SpeedRatio = (double)speedRatioSlider.Value;
        }

        // When the media opens, initialize the "Seek To" slider maximum value
        // to the total number of miliseconds in the length of the media clip.
        private void Element_MediaOpened(object sender, EventArgs e)
        {
            timelineSlider.Maximum = myMediaElement.NaturalDuration.TimeSpan.TotalSeconds;
        }

        // When the media playback is finished. Stop() the media to seek to media start.
        private void Element_MediaEnded(object sender, EventArgs e)
        {
            myMediaElement.Stop();
            timelineSlider.Value = 0;
            IsPlaying = false;
        }


        void InitializePropertyValues()
        {
            myMediaElement.Volume = (double)volumeSlider.Value;
        }


        private void OnMouseDownRepeat(object sender, RoutedEventArgs e)
        {
            IsRepeat = tbRepeat.IsChecked;
        }

        private void OnMouseDownShuffle(object sender, RoutedEventArgs e)
        {
            if (tbShuffle.IsChecked.Value)
            {
                Shuffle<string>(filePicker.musicFiles);
                UpdateListBox();
            }
            else
            {
                filePicker.musicFiles.Sort();
                UpdateListBox();
            }
        }

        private void OnMouseForwardBackMedia(object sender, RoutedEventArgs e)
        {
            PlayNext();
        }

        private void OnMouseDownBackMedia(object sender, RoutedEventArgs e)
        {
            PlayPrev();
        }

        private void ShowHistory(object sender, RoutedEventArgs e)
        {
            History history = new History(historyList);
            if (history.ShowDialog() == true)
            {
                //Play(history.Selected);
                filePicker.musicFiles.Add(history.Selected);
                UpdateListBox();
                lbMusicList.SelectedIndex = lbMusicList.Items.Count - 1;
            };
        }

        void UpdateListBox()
        {
            lbMusicList.ItemsSource = null;
            lbMusicList.ItemsSource = filePicker.musicFiles;

        }

        private void selectMusic(object sender, SelectionChangedEventArgs e)
        {
            if (lbMusicList.SelectedItem != null)

            {
                Play((string)lbMusicList.SelectedItem);
            }
        }


        private static Random rng = new Random();

        public static List<T> Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }


        private void timelineSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(sender);
            if (sender is Slider)
            {
                int SliderValue = (int)timelineSlider.Value;

                // Overloaded constructor takes the arguments days, hours, minutes, seconds, milliseconds.
                // Create a TimeSpan with miliseconds equal to the slider value.
                TimeSpan ts = new TimeSpan(0, 0, 0, SliderValue,0);
                myMediaElement.Position = ts;
            }

        }

        private void tbShuffle_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
