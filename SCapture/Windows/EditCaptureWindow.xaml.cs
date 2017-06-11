using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Win32;
using SCapture.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SCapture.Windows
{
    /// <summary>
    /// Interaction logic for EditCaptureWindow.xaml
    /// </summary>
    public partial class EditCaptureWindow : Window
    {
        public EditCaptureWindow(BitmapSource bSource)
        {
            InitializeComponent();

            this.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.ShowSystemMenuCommand, OnShowSystemMenu));

            CapturedImage.Source = bSource;

            this.Topmost = true;
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
            //SystemCommands.CloseWindow(this);
            Application.Current.Shutdown();
        }

        private void OnShowSystemMenu(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.ShowSystemMenu(this, GetMousePosition());
        }
        #endregion

        #region Helpers
        private Point GetMousePosition()
        {
            var position = Mouse.GetPosition(this);

            return new Point(position.X + this.Left, position.Y + this.Top);
        }
        #endregion

        #region Events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;
        }

        private void NewCaptureButton_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = $"screenshot_{DateTime.Now.ToString("yyyy_dd_MM_HH_mm_ss")}";
            saveFileDialog.Filter = "Bitmap Image (.bmp)|*.bmp|JPEG Image(.jpeg)|*.jpeg|PNG Image Image (.png)|*.png";

            if (saveFileDialog.ShowDialog() == true)
            {
                ScreenCapturer.Save(saveFileDialog.FileName, CapturedImage.Source as BitmapSource);
            }
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetImage(CapturedImage.Source as BitmapSource);
            MainWindow.notification.ShowBalloonTip("Screen Capturer", "Image copied to clipboard!", BalloonIcon.Info);
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(CapturedImage, "Capture");
            }
        }
        #endregion
    }
}
