using Hardcodet.Wpf.TaskbarNotification;
using SCapture.Classes;
using SCapture.Properties;
using SCapture.Windows;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SCapture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static TaskbarIcon notification = new TaskbarIcon();

        public MainWindow()
        {
            InitializeComponent();

            this.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.ShowSystemMenuCommand, OnShowSystemMenu));
        }

        #region Command Binding
        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != ResizeMode.NoResize;
        }

        private void OnMinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void OnRestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        private void OnCloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void OnShowSystemMenu(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.ShowSystemMenu(this, GetMousePosition());
        }
        #endregion

        #region Buttons / Actions
        private void CaptureRegionButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new RegionCaptureWindow().ShowDialog();

            if (!Settings.Default.AlwaysOpenToEditor)
                this.Show();
        }

        private void CaptureWindowButton_Click(object sender, RoutedEventArgs e)
        {
            notification.ShowBalloonTip(this.Title, "Click the window you want to capture.", BalloonIcon.Info);

            ClickWindowToCapture();
        }

        /// <summary>
        /// Capture window
        /// The whole idea is to pick the window that we want to capture with mouse click
        /// First, we will use an transparent window to catch user click point
        /// Then we will use 'WindowFromPoint' native method to get the window handle
        /// </summary>
        private void ClickWindowToCapture()
        {
            // The target window
            IntPtr hWnd;

            // Create a dummy window to capture click point
            Window DummyWindow = new Window()
            {
                WindowStyle = WindowStyle.None,
                Topmost = true,
                ShowInTaskbar = false,
                AllowsTransparency = true,
                Background = Brushes.Black,
                Opacity = 0.1,
                WindowState = WindowState.Maximized,
                ResizeMode = ResizeMode.NoResize
            };

            Point ClickedPoint = new Point();

            // On press {ESC}: Cancel window capturing
            DummyWindow.KeyUp += (s, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    DummyWindow.Close();
                    this.Show();
                }
            };

            // On mouse click: Get position
            DummyWindow.MouseDown += (s, e) =>
            {
                ClickedPoint = e.GetPosition(DummyWindow);
            };

            // After click
            DummyWindow.MouseUp += (s, e) =>
            {
                if (ClickedPoint.X > 5 &&
                    ClickedPoint.X < (SystemParameters.PrimaryScreenWidth - 5) &&
                    ClickedPoint.Y > 5 &&
                    ClickedPoint.Y < (SystemParameters.PrimaryScreenHeight - 5))
                {
                    // We don't need it anymore
                    DummyWindow.Close();

                    // Get window handle from that position
                    hWnd = NativeMethods.WindowFromPoint(new System.Drawing.Point((int)ClickedPoint.X, (int)ClickedPoint.Y));
                    NativeMethods.SetForegroundWindow(hWnd);

                    // Capture window
                    BitmapSource bSource = ScreenCapturer.CaptureWindow(hWnd);
                    HandleCapture(bSource);
                }
            };

            notification.ShowBalloonTip(this.Title, "Click the window you want to capture", BalloonIcon.Info);

            this.Hide();
            DummyWindow.Show();
        }

        private void CaptureFullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            BitmapSource bSource = ScreenCapturer.CaptureFullScreen();
            HandleCapture(bSource);
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().ShowDialog();
        }
        #endregion

        #region Helpers
        private Point GetMousePosition()
        {
            var position = Mouse.GetPosition(this);
            return new Point(position.X + this.Left, position.Y + this.Top);
        }

        public void HandleCapture(BitmapSource bSource)
        {
            if (Settings.Default.AlwaysCopyToClipboard)
                Clipboard.SetImage(bSource);

            if (Settings.Default.AlwaysOpenToEditor)
            {
                new EditCaptureWindow(bSource).Show();
                this.Close();
            }
            else
            {
                this.Show();

                if (ScreenCapturer.Save(bSource))
                {
                    notification.ShowBalloonTip(this.Title, "File saved!", BalloonIcon.Info);
                }
                else
                    MessageBox.Show("Oups! We couldn't save this file. Please check permissions.");
            }
        }
        #endregion
    }
}
