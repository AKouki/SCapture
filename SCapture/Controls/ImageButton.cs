using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SCapture
{
    /// <summary>
    /// Custom Button with Image and Text
    /// </summary>
    public class ImageButton : Button
    {
        /// <summary>
        /// Image Source
        /// </summary>
        public ImageSource Source
        {
            get { return base.GetValue(SourceProperty) as ImageSource; }
            set { base.SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Text Orientation (Right or Bellow of the button)
        /// </summary>
        public Orientation TextOrientation
        {
            get { return (Orientation)GetValue(TextOrientationProperty); }
            set { base.SetValue(TextOrientationProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
          DependencyProperty.Register("Source", typeof(ImageSource), typeof(ImageButton));

        public static readonly DependencyProperty TextOrientationProperty =
          DependencyProperty.Register("TextOrientation", typeof(Orientation), typeof(ImageButton), new UIPropertyMetadata(Orientation.Horizontal));
    }
}
