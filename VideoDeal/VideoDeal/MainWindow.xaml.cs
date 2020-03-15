using Microsoft.Win32;
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
using VideoDeal.Helper;

namespace VideoDeal
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 定义原视频文件路径
        /// </summary>
        private static string videoPath = string.Empty;
        public MainWindow()
        {
            InitializeComponent();
        }

        #region 打开视频
        private void btnAddVideo_Click(object sender, RoutedEventArgs e)
        {
            videoPath = OpenFileDialog("MP4文件|*.mp4|flv文件|*.flv|所有文件|*.*");
        }
        #endregion

        #region 将视频左转90度
        private void btnLeft90_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(videoPath))
            {
                string dealVideFilePath = SaveFileDialog("MP4文件|*.mp4|flv文件|*.flv");
                bool result = ffmpegHelper.VideoRotate(videoPath, dealVideFilePath, "2");
                if (result)
                    MessageBox.Show("操作成功");
                else
                    MessageBox.Show("操作失败");
            }
            else
                MessageBox.Show("未添加源视频");
        }
        #endregion

        #region 将视频左转90度
        private void btnRight90_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(videoPath))
            {
                string dealVideFilePath = SaveFileDialog("MP4文件|*.mp4|flv文件|*.flv");
                bool result = ffmpegHelper.VideoRotate(videoPath, dealVideFilePath, "1");
                if (result)
                    MessageBox.Show("操作成功");
                else
                    MessageBox.Show("操作失败");
            }
            else
                MessageBox.Show("未添加源视频");
        }
        #endregion

        #region 添加水印
        private void btnAddWaterMark_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(videoPath))
            {
                MessageBox.Show("未添加源视频", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            //启动添加水印的第三方软件
            StartThirdExeHelper.Start();
        }
        #endregion

        #region 上传文件到阿里云
        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 播放视频
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(videoPath))
            {
                MessageBox.Show("未添加源视频","系统提示：",MessageBoxButton.OK,MessageBoxImage.Information);
                return;
            }
            MediaPlayer.Source = new Uri(videoPath);
            MediaPlayer.Play();
        }
        #endregion

        #region 截图
        private void btnCutImg_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(videoPath))
            {
                MessageBox.Show("未添加源视频", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }



        }
        #endregion

        #region 打开文件对话框
        /// <summary>
        /// 打开文件对话框
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <returns>返回文件路径</returns>
        public static string OpenFileDialog(string filter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = filter; //"MP4文件|*.mp4|flv文件|*.flv|所有文件|*.*";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "mp4";   //设置默认扩展名
            if (openFileDialog.ShowDialog() == false)
                return "";
            return openFileDialog.FileName;
        }
        #endregion

        #region 保存文件对话框
        /// <summary>
        /// 保存文件对话框
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <returns>返回文件路径</returns>
        public static string SaveFileDialog(string filter)
        {

            //创建一个保存文件式的对话框
            SaveFileDialog sfd = new SaveFileDialog();
            //设置这个对话框的起始保存路径
            sfd.InitialDirectory = @"D:\";
            //设置保存的文件的类型，注意过滤器的语法
            sfd.Filter = filter;  // "PNG图片|*.png|JPG图片|*.jpg";
            //调用ShowDialog()方法显示该对话框，该方法的返回值代表用户是否点击了确定按钮
            if (sfd.ShowDialog() == true)
                return sfd.FileName;
            else
                return "" ;
        }
        #endregion
    }
}
