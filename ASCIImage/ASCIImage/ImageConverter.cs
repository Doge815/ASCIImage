using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace ASCIImage
{
    static class ImageConverter
    {
        private const int char_x = 1;
        private const int char_y = 2;
        private static Dictionary<float, char> brightness;

        public static string GetStringyImage(string file, int parts, Action<double>SetValue)
        {
            if (brightness == null)
            {
                brightness = new Dictionary<float, char>();
                string inp = Properties.Resources.brightness;
                string[] pairs = inp.Split(new []{ "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach(string pair in pairs)
                {
                    string[] val = pair.Split('|');
                    brightness.Add(float.Parse(val[0]), Convert.ToChar(val[1]));
                }
            }
            SetValue(0);
            Bitmap image = new Bitmap(System.Drawing.Image.FromFile(file));
            double[,] values = new double[(int)Math.Ceiling((float)image.Width / char_x / parts),
                (int)Math.Ceiling((float)image.Height / char_y / parts)];
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    values[(int)Math.Floor((float)x / char_x / parts),
                        (int)Math.Floor((float)y / char_y / parts)] +=
                        (double)image.GetPixel(x, y).GetBrightness() / parts / parts / char_x / char_y;
                }
                SetValue(Math.Ceiling(100d * x / image.Width));
            }
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < (int)Math.Ceiling((float)image.Height / char_y / parts); y++)
            {
                for (int x = 0; x < (int)Math.Ceiling((float)image.Width / char_x / parts); x++)
                {
                    var b = (float)(Math.Round(values[x, y] * (brightness.Count-1)) /
                        (brightness.Count-1));
                    var value = brightness[b];
                    sb.Append(value);
                }
                sb.Append(Environment.NewLine);
                SetValue(Math.Ceiling(100d * y * char_y * parts / image.Height));
                System.Diagnostics.Debug.Print((Math.Ceiling(100d * y * char_y * parts / image.Height).ToString()));
            }
            SetValue(100);
            return sb.ToString();
        }

        public static System.Drawing.Image GetImagyImage(string text, int size, Action<double>SetValue)
        { 
            SetValue(100);
            string[] lines = text.Split('\n');
            Font font = new Font("DejaVu Sans Mono", size);
            SizeF lineSize;
            SolidBrush brush = new SolidBrush(System.Drawing.Color.Black);
            PointF point = new PointF();

            using (Bitmap b = new Bitmap(1,1))
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    lineSize = g.MeasureString(lines[0], font);
                }
            }
#if false
            Bitmap image = new Bitmap((int)Math.Ceiling(lineSize.Width),
                (int)Math.Ceiling(lineSize.Height * lines.Length),
                System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);

            System.Drawing.Imaging.BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, image.PixelFormat);
            
            IntPtr ptr = imageData.Scan0;
            Graphics graphic = Graphics.FromHdc(ptr);
            graphic.Clear(System.Drawing.Color.White);
            for(int i = 0; i< lines.Length; i++)
            {
                graphic.DrawString(lines[i], font, brush, point);
                point.Y += lineSize.Height;
                SetValue(100d * i / lines.Length);
            }
            image.UnlockBits(imageData);
            */
            return image;
#else

#endif

        }

        //modified from https://stackoverflow.com/questions/88488/getting-a-drawingcontext-for-a-wpf-writeablebitmap
        public static BitmapSource CreateBitmapSource(int width, int height, double dpi, Action<DrawingContext> render)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                render(drawingContext);
            }
            RenderTargetBitmap bitmap = new RenderTargetBitmap(
                width, height, dpi, dpi, PixelFormats.BlackWhite);
            bitmap.Render(drawingVisual);

            return bitmap;
        }

        //from https://stackoverflow.com/questions/5867657/copying-from-bitmapsource-to-writablebitmap
        public static WriteableBitmap CreateWriteableBitmap(BitmapSource bitmapSource)
        {
            int stride = bitmapSource.PixelWidth * (bitmapSource.Format.BitsPerPixel + 7) / 8;

            byte[] data = new byte[stride * bitmapSource.PixelHeight];

            bitmapSource.CopyPixels(data, stride, 0);

            WriteableBitmap target = new WriteableBitmap(
                bitmapSource.PixelWidth,
                bitmapSource.PixelHeight,
                bitmapSource.DpiX, bitmapSource.DpiY,
                bitmapSource.Format, null);
            target.WritePixels(
                new Int32Rect(0, 0, bitmapSource.PixelWidth, 
                bitmapSource.PixelHeight), data, stride, 0);
            return target;
        }

        //from https://stackoverflow.com/questions/17298034/converting-writeablebitmap-to-bitmap-in-c-sharp
        public static System.Drawing.Bitmap CreateBitmap(WriteableBitmap writeableBitmap)
        {
            System.Drawing.Bitmap bmp;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create((BitmapSource)writeableBitmap));
                enc.Save(outStream);
                bmp = new System.Drawing.Bitmap(outStream);
            }
            return bmp;
        }
    }
}
