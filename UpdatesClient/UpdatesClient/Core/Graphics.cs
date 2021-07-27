using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UpdatesClient.Core
{
    public static class Graphics
    {
        public static ImageBrush GetGridBackGround(FrameworkElement element, UIElement parent, ImageBrush imgBg, double width, double height)
        {
            Point relativePoint = element.TranslatePoint(new Point(0, 0), parent);

            var image = (BitmapSource)imgBg.ImageSource;
            double w = width / image.Width;
            double h = height / image.Height;

            CroppedBitmap im = new CroppedBitmap(image,
                new Int32Rect((int)(relativePoint.X * w), (int)(relativePoint.Y * h), (int)(element.Width * w), (int)(element.Height * h)));
            return new ImageBrush(im);
        }
    }
}
