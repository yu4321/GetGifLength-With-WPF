using Microsoft.Win32;
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
using System.Windows.Threading;
using XamlAnimatedGif;

namespace WpfGifPlayer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private int count = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void openFile_Click(object sender, RoutedEventArgs e)
        {
            var nextUri = OpenFile("Gif이미지를 선택해주세요", "Image (*.gif)|*.gif");
            if (nextUri != string.Empty)
            {
                Dispatcher.Invoke(() =>
                {
                    UnloadFile();
                    LoadFile(nextUri);
                });
            }
        }

        private void UnloadFile()
        {
            try
            {
                holder.Source = null;
                var ss = AnimationBehavior.GetSourceStream(holder);
                ss.Dispose();
            }
            catch(Exception e)
            {
                Console.WriteLine("Unload File Failed - " + e);
            }

        }

        private void LoadFile(string nextUri)
        {
            var ms = new MemoryStream(File.ReadAllBytes(nextUri));
            var img1 = new BitmapImage();
            img1.BeginInit();
            img1.CacheOption = BitmapCacheOption.OnLoad;
            img1.StreamSource = ms;
            img1.EndInit();

            var duration = GifExtension.GetGifDuration(img1);
            AnimationBehavior.SetSourceStream(holder, img1.StreamSource);
            StartRepeatChecker(duration);
        }

        public string OpenFile(string caption, string filter = "All files (*.*)|*.*")
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.CurrentDirectory;// Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            dialog.Title = caption;
            dialog.Filter = filter;
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            //dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == true) return dialog.FileName;

            return string.Empty;
        }

        private void StartRepeatChecker(TimeSpan sp)
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

            count = 0;
            duration.Text = sp.ToString(@"mm\:ss\.ff");
            repeated.Text = "0";

            timer = new DispatcherTimer();
            timer.Interval = sp;

            timer.Tick += (s, e) =>
            {
                count++;
                Console.WriteLine($"completed {count}");
                repeated.Text = count.ToString();
            };

            timer.Start();
        }
    }
}
