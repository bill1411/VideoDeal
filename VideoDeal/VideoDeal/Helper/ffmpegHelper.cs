using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace VideoDeal.Helper
{
    public class ffmpegHelper
    {
        /// <summary>
        /// 获取ffmpeg.exe文件路径
        /// </summary>
        public static string FFmpegPath = Environment.CurrentDirectory + "\\ffmpeg\\ffmpeg.exe";

        //ffmpeg执行文件的路径
        private static string ffmpeg = System.AppDomain.CurrentDomain.BaseDirectory + "ffmpeg\\ffmpeg.exe";
        private static int? width;    //视频宽度
        private static int? height;   //视频高度

        #region 属性访问
        /// <summary>
        /// 获取宽度
        /// </summary>
        /// <returns></returns>
        public static int? GetWidth()
        {
            return width;
        }
        /// <summary>
        /// 获取高度
        /// </summary>
        /// <returns></returns>
        public static int? GetHeight()
        {
            return height;
        }
        #endregion 

        #region 从视频画面中截取一帧画面为图片
        /// <summary>
        /// 从视频画面中截取一帧画面为图片
        /// </summary>
        /// <param name="VideoName">视频文件，绝对路径</param>
        /// <param name="Width">图片的宽:620</param>
        /// <param name="Height">图片的长:360</param>
        /// <param name="CutTimeFrame">开始截取的时间如:"1"【单位秒】</param>
        /// <param name="PicPath">截图文件的保存路径【含文件名及后缀名】</param>
        /// <param name="SleepTime">线程挂起等待时间，单位毫秒【默认值是7000】</param>
        /// <returns>截图成功返回截图路径，失败返回空</returns>
        public static string GetPicFromVideo(string VideoName, int Width, int Height, string CutTimeFrame, string PicPath, int SleepTime)
        {
            //获取视频长宽尺寸
            GetMovWidthAndHeight(VideoName);
            if (!string.IsNullOrWhiteSpace(width.ToString()))   //说明获取到了视频的长宽参数
            {
                int resultWidht;
                int resultHeight;
                DealWidthAndHeight(int.Parse(width.ToString()), int.Parse(height.ToString()), Width, Height, out resultWidht, out resultHeight);

                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(ffmpeg);
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.Arguments = " -i " + VideoName
                                    + " -y -f image2 -ss " + CutTimeFrame
                                    + " -t 0.001 -s " + resultWidht.ToString() + "*" + resultHeight.ToString()
                                    + " " + PicPath;  //設定程式執行參數
                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                    Thread.Sleep(SleepTime);//线程挂起，等待ffmpeg截图完毕
                }
                catch (Exception e)
                {
                    return e.Message;
                }

                //返回视频图片完整路径
                if (System.IO.File.Exists(PicPath))
                    return PicPath;
                return "";
            }
            else
                return "";
        }
        #endregion

        #region 获取视频的帧宽度和帧高度
        /// <summary>
        /// 获取视频的帧宽度和帧高度
        /// </summary>
        /// <param name="videoFilePath">mov文件的路径</param>
        /// <returns>null表示获取宽度或高度失败</returns>
        public static void GetMovWidthAndHeight(string videoFilePath)
        {
            width = null;
            height = null;
            try
            {
                //判断文件是否存在
                if (File.Exists(videoFilePath))
                {
                    string output;
                    string error;
                    //执行命令
                    ExecuteCommand("\"" + ffmpeg + "\"" + " -i " + "\"" + videoFilePath + "\"", out output, out error);

                    if (!string.IsNullOrEmpty(error))
                    {
                        width = null;
                        height = null;

                        //通过正则表达式获取信息里面的宽度信息
                        Regex regex = new Regex("(\\d{2,4})x(\\d{2,4})", RegexOptions.Compiled);
                        Match m = regex.Match(error);
                        if (m.Success)
                        {
                            width = int.Parse(m.Groups[1].Value);
                            height = int.Parse(m.Groups[2].Value);
                        }
                    }
                }
            }
            catch (Exception)
            { }
        }
        #endregion 

        #region  处理图片宽高比例截图问题
        /// <summary>
        /// 处理图片宽高比例截图问题
        /// </summary>
        /// <param name="videoWidht">304</param>
        /// <param name="videoHeight">640</param>
        /// <param name="imgWidth">640</param>
        /// <param name="imgHeight">360</param>
        /// <param name="width">最终处理的图片宽</param>
        /// <param name="height">最终处理的图片高</param>
        private static void DealWidthAndHeight(int videoWidht, int videoHeight, int imgWidth, int imgHeight, out int width, out int height)
        {
            if (videoWidht < videoHeight)  //说明是竖屏拍摄
            {
                if (imgWidth > videoWidht)
                    width = videoWidht;
                else
                    width = imgWidth;
                height = videoHeight;
            }
            else                           //说明是横屏拍摄
            {
                if (imgHeight > videoHeight)
                    height = videoHeight;
                else
                    height = imgHeight;
                width = videoWidht;
            }
        }
        #endregion

        #region 视频旋转
        /// <summary>
        /// 视频旋转
        /// </summary>
        /// <param name="videoFilePath">视频绝对路径</param>
        /// <param name="dealVideFilePath">视频旋转后保存路径</param>
        /// <param name="flag">1=顺时针旋转90度  2=逆时针旋转90度</param>
        /// <returns>true  成功  false  失败</returns>
        public static bool VideoRotate(string videoFilePath, string dealVideFilePath, string flag)
        {
            //ffmpeg -i success.mp4 -metadata:s:v rotate="90" -codec copy output_success.mp4
            string output;
            string error;
            //执行命令
            ExecuteCommand("\"" + ffmpeg + "\"" + " -y -i " + "\"" + videoFilePath + "\"" + " -vf transpose=" + flag + " -acodec copy " + "\"" + dealVideFilePath + "\"", out output, out error);
            if (File.Exists(dealVideFilePath))
                return true;
            else
                return false;
        }
        #endregion

        #region 给视频添加水印
        /// <summary>
        /// 给视频添加水印
        /// </summary>
        /// <param name="videoFilePath">原视频位置</param>
        /// <param name="dealVideFilePath">处理后的视频位置</param>
        /// <param name="waterPicPath">水印图片</param>
        /// <param name="location">水印距离视频的左上角坐标比如： 10:10</param>
        /// <returns></returns>
        public static bool VideoWaterMark(string videoFilePath, string dealVideFilePath, string waterPicPath, string location)
        {
            //ffmpeg -i success.mp4 -metadata:s:v rotate="90" -codec copy output_success.mp4
            string output;
            string error;
            //执行命令
            ExecuteCommand("\"" + ffmpeg + "\"" + " -i " + "\"" + videoFilePath + "\"" + " -i " + "\"" + waterPicPath + "\"" + " -filter_complex overlay=" + location + " \"" + dealVideFilePath + "\"", out output, out error);
            if (File.Exists(dealVideFilePath))
                return true;
            else
                return false;
        }
        #endregion

        #region 让ffmpeg执行一条命令
        /// <summary>
        /// 让ffmpeg执行一条command命令
        /// </summary>
        /// <param name="command">需要执行的Command</param>
        /// <param name="output">输出</param>
        /// <param name="error">错误</param>
        private static void ExecuteCommand(string command, out string output, out string error)
        {
            try
            {
                //创建一个进程
                Process pc = new Process();
                pc.StartInfo.FileName = command;
                pc.StartInfo.UseShellExecute = false;
                pc.StartInfo.RedirectStandardOutput = true;
                pc.StartInfo.RedirectStandardError = true;
                pc.StartInfo.CreateNoWindow = true;

                //启动进程
                pc.Start();

                //准备读出输出流和错误流
                string outputData = string.Empty;
                string errorData = string.Empty;
                pc.BeginOutputReadLine();
                pc.BeginErrorReadLine();

                pc.OutputDataReceived += (ss, ee) =>
                {
                    outputData += ee.Data;
                };

                pc.ErrorDataReceived += (ss, ee) =>
                {
                    errorData += ee.Data;
                };

                //等待退出
                pc.WaitForExit();

                //关闭进程
                pc.Close();

                //返回流结果
                output = outputData;
                error = errorData;
            }
            catch (Exception ex)
            {
                output = null;
                error = null;
            }
        }
        #endregion

    }
}
