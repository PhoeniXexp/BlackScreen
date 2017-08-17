﻿using System;
using System.Collections.Generic;
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

        private void _time()
        {
            while (true)
            {
                var src = DateTime.Now;
                var dt = src.Hour.ToString() + ":" + src.Minute.ToString();

                Dispatcher.Invoke(() =>
                {
                    time_text.Content = dt;
                });

                Thread.Sleep(60000);
            }
        }
    }
}
