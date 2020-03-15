using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoDeal.Helper
{
    public class StartThirdExeHelper
    {
        /// <summary>
        /// 获取程序基础目录
        /// </summary>
        private static readonly string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// 定义视频文件路径
        /// </summary>
        private static string exePath = basePath + "watermark\\EasyVideoLogoRemover.exe";
        public static void Start()
        {

            Process p = Process.Start(exePath);
            p.WaitForExit();//关键，等待外部程序退出后才能往下执行
        }
    }
}
