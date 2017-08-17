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
using Interceptor;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using Keys = Interceptor.Keys;

namespace BlackScreen
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NotifyIcon ni;

        private bool sc2 = false;

        public MainWindow()
        {
            InitializeComponent();

            System.Windows.Forms.ContextMenu _contextMenu = new System.Windows.Forms.ContextMenu();

            _contextMenu.MenuItems.Add("Выход", new EventHandler(_exitMenu));

            ni = new NotifyIcon()
            {
                Icon = Properties.Resources.Monitor_02,
                Visible = true,
                ContextMenu = _contextMenu,
                Text = "Black Screen"
            };
            this.Hide();
            this.WindowState = WindowState.Minimized;

            ScreenInit();

            Start();
        }

        private void ScreenInit()
        {
            Screen[] sc = Screen.AllScreens;

            System.Drawing.Rectangle r1 = sc[0].WorkingArea;
            black_sc1.Top = r1.Top;
            black_sc1.Left = r1.Left;

            if (sc.Count() > 1)
            {
                sc2 = true;
                System.Drawing.Rectangle r2 = sc[1].WorkingArea;
                black_sc2.Top = r2.Top;
                black_sc2.Left = r2.Left;
            }

        }

        private void _exitMenu(object sender, EventArgs e)
        {
            Stop();
        }

        private Thread callbackThread;

        private event EventHandler<KeyPressedEventArgs> OnKeyPressed;
        private event EventHandler<MousePressedEventArgs> OnMousePressed;

        private IntPtr context;
        private int device, devk = 3, devm = 11;

        private bool user_out = false;
        private bool _lock = false;

        private black_wind black_sc1 = new black_wind();
        private black_wind black_sc2 = new black_wind();

        private void hook()
        {
            bool s = true;
            bool l1 = true;
            bool l2 = true;
            bool winkey = false;

            Stroke stroke = new Stroke();
            InterceptionDriver.SetFilter(context, InterceptionDriver.IsMouse, (Int32)MouseFilterMode.All);
            InterceptionDriver.SetFilter(context, InterceptionDriver.IsKeyboard, (Int32)KeyboardFilterMode.All);

            while (InterceptionDriver.Receive(context, device = InterceptionDriver.Wait(context), ref stroke, 1) > 0)
            {
                s = true;
                if (InterceptionDriver.IsMouse(device) > 0)
                {
                    if (l1)
                    {
                        l1 = false;
                        devm = device;
                    }
                    user_out = false;
                    if (_lock) s = false;

                    if (OnMousePressed != null)
                    {
                        var args = new MousePressedEventArgs() { X = stroke.Mouse.X, Y = stroke.Mouse.Y, State = stroke.Mouse.State, Rolling = stroke.Mouse.Rolling };
                        OnMousePressed(this, args);

                        if (args.Handled)
                        {
                            continue;
                        }
                        stroke.Mouse.X = args.X;
                        stroke.Mouse.Y = args.Y;
                        stroke.Mouse.State = args.State;
                        stroke.Mouse.Rolling = args.Rolling;
                    }
                }

                if (InterceptionDriver.IsKeyboard(device) > 0)
                {
                    if (l2)
                    {
                        l2 = false;
                        devk = device;
                    }

                    if (_lock) s = false;

                    user_out = false;

                    if (stroke.Key.Code == Keys.WindowsKey)
                    {
                        if (stroke.Key.State == KeyState.E0)
                        {
                            winkey = true;
                        }
                        else winkey = false;
                    }

                    if (stroke.Key.Code == Keys.NumpadMinus)
                    {
                        try
                        {
                            Thread blk = new Thread(block);
                            blk.IsBackground = true;
                            blk.Start();
                            
                            s = false;
                        }
                        catch { System.Windows.MessageBox.Show("Error"); }
                    }

                    if (stroke.Key.Code == Keys.Q)
                    {
                        if (winkey)
                        {
                            try
                            {
                                Thread unblk = new Thread(unblock);
                                unblk.IsBackground = true;
                                unblk.Start();                                
                            }
                            catch { System.Windows.MessageBox.Show("Error"); }
                            s = true;
                        }
                    }

                    if (OnKeyPressed != null)
                    {
                        var args = new KeyPressedEventArgs() { Key = stroke.Key.Code, State = stroke.Key.State };
                        OnKeyPressed(this, args);

                        if (args.Handled)
                        {
                            continue;
                        }
                        stroke.Key.Code = args.Key;
                        stroke.Key.State = args.State;
                    }
                }

                if (s) InterceptionDriver.Send(context, device, ref stroke, 1);
            }

            Stop();
        }        

        private void _timer()
        {
            int timer = 0;
            while (true)
            {
                Thread.Sleep(10000);
                if (user_out)
                {
                    timer++;
                    if (timer == 6)
                    {
                        block();
                    }
                }
                else
                {
                    user_out = true;
                    timer = 0;
                }                
            }
        }

        private void Start()
        {
            context = InterceptionDriver.CreateContext();

            if (context != IntPtr.Zero)
            {
                callbackThread = new Thread(new ThreadStart(hook))
                {
                    Priority = ThreadPriority.Highest,
                    IsBackground = true
                };
                callbackThread.Start();
            }

            Thread tmr = new Thread(_timer)
            {
                IsBackground = true
            };
            tmr.Start();
        }

        private void block()
        {
            Dispatcher.Invoke(() =>
            {
                _lock = true;

                black_sc1.Show();
                black_sc1.Topmost = true;
                black_sc1.WindowState = WindowState.Maximized;
                black_sc1.Cursor = System.Windows.Input.Cursors.None;

                if (sc2)
                {
                    black_sc2.Show();
                    black_sc2.Topmost = true;
                    black_sc2.WindowState = WindowState.Maximized;
                    black_sc2.Cursor = System.Windows.Input.Cursors.None;
                }

            });
        }

        private void unblock()
        {
            Dispatcher.Invoke(() =>
            {
                _lock = false;

                black_sc1.Hide();
                black_sc1.WindowState = WindowState.Minimized;

                if (sc2)
                {
                    black_sc2.Hide();
                    black_sc2.WindowState = WindowState.Minimized;
                }
            });
        }

        private void Stop()
        {
            if (context != IntPtr.Zero)
            {
                InterceptionDriver.DestroyContext(context);
            }

            ni.Dispose();

            Dispatcher.Invoke(() =>
            {
                black_sc1.Close();
                black_sc2.Close();
                this.Close();
            });
        }        
    }
}
