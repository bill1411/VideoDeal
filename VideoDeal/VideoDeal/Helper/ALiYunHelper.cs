using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.vod.Model.V20170321;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoDeal.Helper
{
    public class ALiYunHelper
    {
        #region 系统参数
        /// <summary>
        /// 上传后视频ID
        /// </summary>
        public static string videoId = string.Empty;
        /// <summary>
        /// 上传后视频m3u8地址
        /// </summary>
        public static string videoUrl = string.Empty;
        /// <summary>
        /// 视频封面图片
        /// </summary>
        public static string videoImgUrl = string.Empty;
        /// <summary>
        /// 上传凭证
        /// </summary>
        public static string videoToken = string.Empty;
        /// <summary>
        /// 上传地址
        /// </summary>
        public static string videoAddress = string.Empty;
        #endregion

        /// <summary>
        /// 获取Client对象
        /// </summary>
        private static readonly DefaultAcsClient client = InitVodClient();

        #region 初始化Client对象
        private static DefaultAcsClient InitVodClient()
        {
            // 点播服务接入区域
            string regionId = "cn-shanghai";
            string accessId = ConfigurationSettings.AppSettings["AccessID"].ToString();
            string accessSecret = ConfigurationSettings.AppSettings["AccessSecret"].ToString();

            IClientProfile profile = DefaultProfile.GetProfile(regionId, accessId, accessSecret);
            // DefaultProfile.AddEndpoint(regionId, regionId, "vod", "vod." + regionId + ".aliyuncs.com");
            return new DefaultAcsClient(profile);
        }
        #endregion

        #region 获取上传凭证和地址
        /// <summary>
        /// 上传视频获取上传凭证和地址
        /// </summary>
        /// <param name="title">视频名称</param>
        /// <param name="fileName">视频地址（需带后缀名）</param>
        public static void GetVideoUploadParameter(string title,string fileName)
        {
            try
            {
                // 构造请求
                CreateUploadVideoRequest request = new CreateUploadVideoRequest();
                request.Title = title;
                request.FileName = fileName;
                // request.Tags = "tags1,tags2";
                // request.Description = "this is a sample description";
                // request.CoverURL = coverUrl;
                // request.CateId = -1;
                // request.TemplateGroupId = "278840921dee4963bb5862b43aeb2273";
                // 初始化客户端
                //DefaultAcsClient client = InitVodClient();
                // 发起请求，并得到响应结果
                CreateUploadVideoResponse response = client.GetAcsResponse(request);
                //视频ID
                videoId = response.VideoId;
                //上传凭证
                videoToken = response.UploadAuth;
                //上传地址
                videoAddress = response.UploadAddress;

            }
            catch (ServerException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            catch (ClientException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        #endregion

        #region 刷新上传凭证
        /// <summary>
        /// 刷新上传凭证
        /// </summary>
        /// <param name="videoId">视频ID</param>
        public static void RefreshUploadVideo(string videoId)
        {
            try
            {
                // 构造请求
                RefreshUploadVideoRequest request = new RefreshUploadVideoRequest();
                request.VideoId = videoId;
                // 发起请求，并得到 response
                RefreshUploadVideoResponse response = client.GetAcsResponse(request);
                //上传凭证
                videoToken = response.UploadAuth;
                //上传地址
                videoAddress = response.UploadAddress;
            }
            catch (ServerException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            catch (ClientException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        #endregion

        #region 获取播放地址
        /// <summary>
        /// 获取播放地址
        /// </summary>
        /// <param name="vodeoId">视频ID</param>
        public static void GetPlayUrl(string videoId)
        {
            try
            {
                // 构造请求
                GetPlayInfoRequest request = new GetPlayInfoRequest();
                request.VideoId = videoId;
                // request.AuthTimeout = 3600;
                // 初始化客户端
                //DefaultAcsClient client = InitVodClient();
                // 发起请求，并得到 response
                GetPlayInfoResponse response = client.GetAcsResponse(request);
                //获取封面图片
                videoImgUrl =  response.VideoBase.CoverURL;
                List<GetPlayInfoResponse.GetPlayInfo_PlayInfo> playInfoList = response.PlayInfoList;
                foreach (var playInfo in response.PlayInfoList)
                {
                    if (playInfo.Format == "m3u8")
                        videoUrl = playInfo.PlayURL;   //获取视频的播放地址
                }
            }
            catch (ServerException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            catch (ClientException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        #endregion

        #region 删除视频（可多个同时删除）
        /// <summary>
        /// 删除视频（可多个同时删除）
        /// </summary>
        /// <param name="videoIds">要删除的视频VideoID值 比如："videoId1,videoId2"</param>
        public static void DeleteVideo(string videoIds)
        {
            try
            {
                // 构造请求
                DeleteVideoRequest request = new DeleteVideoRequest();
                request.VideoIds = videoIds;                // 初始化客户端
                //DefaultAcsClient client = InitVodClient();
                // 发起请求，并得到 response
                DeleteVideoResponse response = client.GetAcsResponse(request);
                Console.WriteLine("RequestId = " + response.RequestId);
            }
            catch (ServerException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            catch (ClientException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        #endregion

    }
}
