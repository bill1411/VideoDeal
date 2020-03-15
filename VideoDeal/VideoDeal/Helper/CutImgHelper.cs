using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows;

namespace VideoDeal.Helper
{
    public static class CutImgHelper
    {
        #region 获取屏幕图片
        /// <summary>
        /// 获取全屏幕图片
        /// </summary>
        /// <returns></returns>
        public static Bitmap GetScreenSnapshot()
        {
            System.Drawing.Rectangle rc = SystemInformation.VirtualScreen;
            var bitmap = new Bitmap(rc.Width, rc.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics memoryGrahics = Graphics.FromImage(bitmap))
            {
                memoryGrahics.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size, CopyPixelOperation.SourceCopy);
            }

            return bitmap;
        }
        #endregion

        #region 将winForm截图的bmp图片转为wpf中的BitmapSource对象
        /// <summary>
        /// 将winForm截图的bmp图片转为wpf中的BitmapSource对象
        /// </summary>
        /// <param name="bmp">bmp图片</param>
        /// <returns></returns>
        public static BitmapSource ToBitmapSource(this Bitmap bmp)
        {
            BitmapSource returnSource;
            try
            {
                returnSource = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch
            {
                returnSource = null;
            }
            return returnSource;

        }
        #endregion
    }
}
