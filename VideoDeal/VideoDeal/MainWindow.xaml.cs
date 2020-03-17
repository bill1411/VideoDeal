﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// <summary>
        /// 文件过滤条件
        /// </summary>
        private static readonly string fileFilter = "MP4文件|*.mp4|flv文件|*.flv|所有文件|*.*";
        /// <summary>
        /// 视频过滤条件
        /// </summary>
        private static readonly string videoFilter = "MP4文件|*.mp4|flv文件|*.flv";

        public MainWindow()
        {
            InitializeComponent();

            #region 加载图片
            BitmapImage bt = new BitmapImage(new Uri("Img\\icon\\copy.png", UriKind.Relative));
            this.copyVideoId.Source = bt;
            this.copyVideoAuth.Source = bt;
            this.copyVideoAddress.Source = bt;
            this.copySql.Source = bt;
            #endregion

        }

        #region 打开视频
        private void btnAddVideo_Click(object sender, RoutedEventArgs e)
        {
            videoPath = OpenFileDialog(fileFilter);
        }
        #endregion

        #region 将视频左转90度
        private void btnLeft90_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(videoPath))
            {
                string dealVideFilePath = SaveFileDialog(videoFilter);
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
                string dealVideFilePath = SaveFileDialog(videoFilter);
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

        #region 获取视频上传参数【auth、address及Id】
        /// <summary>
        /// 获取视频上传参数【auth、address及Id】
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            #region 清空内容
            this.txtVideoAddress.Text = "";
            this.txtVideoAuth.Text = "";
            this.txtVideoId.Text = "";
            #endregion

            //获取文件路径
            string videoPath = OpenFileDialog(fileFilter);
            //获取文件名
            string title = System.IO.Path.GetFileNameWithoutExtension(videoPath);
            //获取上传视频的参数
            ALiYunHelper.GetVideoUploadParameter(title,videoPath);
            txtVideoId.Text = ALiYunHelper.videoId;
            txtVideoAuth.Text = ALiYunHelper.videoToken;
            txtVideoAddress.Text = ALiYunHelper.videoAddress;

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

        #region 获取图片及视频参数并生成Sql语句
        private void btnCutImg_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ALiYunHelper.videoAddress))
            {
                MessageBox.Show("参数不完整", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            //调用获取url的方法
            ALiYunHelper.GetPlayUrl(ALiYunHelper.videoId);
            if (!string.IsNullOrWhiteSpace(ALiYunHelper.videoImgUrl) && !string.IsNullOrWhiteSpace(ALiYunHelper.videoUrl))
            {
                this.txtSql.Text = "insert into be_video values('" + ALiYunHelper.videoId + "',"
                                                             + "'" + ALiYunHelper.videoImgUrl + "',"
                                                             + "'" + txtCustomerName.Text + "押车视频',"
                                                             + "1,getdate(),getdate(),1,0,"
                                                             + "'" + ALiYunHelper.videoUrl + "')";
            }
            else
            {
                MessageBox.Show("参数不完整，请排查操作过程");
            }

        }
        #endregion

        #region 打开上传页面
        private void btnOpenHtml_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://api.clxchina.cn:9099/aliyun_video_upload/video-upload.html");
        }
        #endregion

        #region 拷贝上传地址
        private void clikc_copy_void_address(object sender, RoutedEventArgs e)
        { 
            Clipboard.SetText(this.txtVideoAddress.Text);
            MessageBox.Show("数据复制成功");

        }
        #endregion

        #region 拷贝上传凭证
        private void clikc_copy_void_auth(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.txtVideoAuth.Text);
            MessageBox.Show("数据复制成功");
        }
        #endregion

        #region 拷贝上传视频ID
        private void clikc_copy_void_id(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.txtVideoId.Text);
            MessageBox.Show("数据复制成功");
        }
        #endregion

        #region 拷贝最终生成的sql语句
        private void clikc_copy_sql(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(this.txtSql.Text);
            MessageBox.Show("数据复制成功");
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
