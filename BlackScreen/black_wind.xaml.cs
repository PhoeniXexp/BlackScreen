using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BlackScreen
{
    /// <summary>
    /// Логика взаимодействия для black_wind.xaml
    /// </summary>
    public partial class black_wind : Window
    {
        public black_wind()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            Thread tm = new Thread(_time)
            {
                IsBackground = true
            };
            tm.Start();
        }
        
        private Bitmap bmp = Properties.Resources.image2;

        private void interf()
        {
            imgpush(imgrand(10));
            Thread.Sleep(100);
            imgpush(imgrand(8));
            Thread.Sleep(100);
            imgpush(imgrand(6));
            Thread.Sleep(100);
            imgpush(imgrand(4));
            Thread.Sleep(100);
            imgpush(imgrand(6));
            Thread.Sleep(100);
            imgpush(imgrand(8));
            Thread.Sleep(100);
            imgpush(imgrand(10));
            Thread.Sleep(100);
            imgpush(bmp);
        }

        private Bitmap imgrand(int k)
        {
            var bitmap = new Bitmap(bmp);
            Random rnd = new Random();

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    System.Drawing.Color color = bitmap.GetPixel(i, j);
                    byte r = (byte)(rnd.Next(0, k) == 1 ? 255 : color.R);
                    byte b = (byte)(rnd.Next(0, k) == 1 ? 255 : color.B);
                    byte g = (byte)(rnd.Next(0, k) == 1 ? 255 : color.G);

                    bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(255, r, g, b));
                }
            }
            return bitmap;
        }

        private void imgpush(Bitmap b)
        {
            Dispatcher.Invoke(() =>
            {

                using (MemoryStream memory = new MemoryStream())
                {
                    b.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    memory.Position = 0;
                    BitmapImage bitmapimage = new BitmapImage();
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();

                    imagebrush.ImageSource = bitmapimage;
                }
            });
        }

        private void _time()
        {
            while (true)
            {
                
                var src = DateTime.Now;
                var tmp = src.Minute.ToString();

                if (tmp.Length == 1) tmp = "0" + tmp;

                var dt = src.Hour.ToString() + ":" + tmp;

                Dispatcher.Invoke(() =>
                {
                    time_text.Content = dt;
                    this.Topmost = true;
                });
                
                Thread.Sleep(60000);

                //interf();
            }
        }
    }
}
