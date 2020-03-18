using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.vod.Model.V20170321;
using Newtonsoft.Json.Linq;
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

        #region 提交截图作业  需要更高的权限
        /// <summary>
        /// 提交截图作业  需要更高的权限
        /// </summary>
        public static void SubmitSnapshotJob()
        {
            try
            {
                // 发起请求，并得到 response
                SubmitSnapshotJobResponse response = SubmitSnapshotJob(client);
                Console.WriteLine("RequestId = " + response.RequestId);
            }
            catch (ServerException e)
            {
                if (e.RequestId != null)
                {
                    Console.WriteLine("RequestId = " + e.RequestId);
                }
                Console.WriteLine("ErrorCode = " + e.ErrorCode);
                Console.WriteLine("ErrorMessage = " + e.ErrorMessage);
            }
            catch (ClientException e)
            {
                if (e.RequestId != null)
                {
                    Console.WriteLine("RequestId = " + e.RequestId);
                }
                Console.WriteLine("ErrorCode = " + e.ErrorCode);
                Console.WriteLine("ErrorMessage = " + e.ErrorMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine("ErrorMessage = " + e.ToString());
            }
        }
        #endregion

        #region 查询截图信息 需要更高的权限
        /// <summary>
        /// 查询截图信息 需要更高的权限
        /// </summary>
        public static void SearchSnapshotJob()
        {
            try
            {
                // 发起请求，并得到 response
                ListSnapshotsResponse response = ListSnapshots(client);
                if(response.MediaSnapshot.Snapshots.Count>0)
                foreach (var item in response.MediaSnapshot.Snapshots)
                {
                    Console.WriteLine("图片的Url地址： " + item.Url);
                } 
            }
            catch (ServerException e)
            {
                if (e.RequestId != null)
                {
                    Console.WriteLine("RequestId = " + e.RequestId);
                }
                Console.WriteLine("ErrorCode = " + e.ErrorCode);
                Console.WriteLine("ErrorMessage = " + e.ErrorMessage);
            }
            catch (ClientException e)
            {
                if (e.RequestId != null)
                {
                    Console.WriteLine("RequestId = " + e.RequestId);
                }
                Console.WriteLine("ErrorCode = " + e.ErrorCode);
                Console.WriteLine("ErrorMessage = " + e.ErrorMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine("ErrorMessage = " + e.ToString());
            }
        }
        #endregion

        #region 构建查询截图作业参数 【私有方法】
        /// <summary>
        /// 构建查询截图作业参数 【私有方法】
        /// </summary>
        /// <param name="client"></param>
        /// <returns>ListSnapshotsResponse</returns>
        private static ListSnapshotsResponse ListSnapshots(DefaultAcsClient client)
        {
            // 构造请求
            ListSnapshotsRequest request = new ListSnapshotsRequest();
            // 视频ID
            request.VideoId = videoId;
            // 截图类型
            request.SnapshotType = "CoverSnapshot";
            request.PageNo = "1";
            request.PageSize = "20";
            return client.GetAcsResponse(request);
        }
        #endregion

        #region  构建截图作业参数【私有方法】
        /// <summary>
        /// 构建截图作业参数【私有方法】  备注：截图模板ID在阿里云后台创建
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private static SubmitSnapshotJobResponse SubmitSnapshotJob(DefaultAcsClient client)
        {
            // 构造请求
            SubmitSnapshotJobRequest request = new SubmitSnapshotJobRequest();
            //需要截图的视频ID(推荐传递截图模板ID)
            request.VideoId = videoId;
            //截图模板ID [可在阿里云后台配置模板ID]  当然也可以在下面参数中定义截图宽高等信息
            request.SnapshotTemplateId = "";
            //如果设置了SnapshotTemplateId，会忽略下面参数
            request.Count = 1;                  //截图的最大数量。默认值：1
            request.SpecifiedOffsetTime = 0;    //截图指定时间的起始点，单位：毫秒。默认值：0
            request.Interval = 1;               //截图间隔时间
            request.Width = "640";
            request.Height = "360";
            request.SpriteSnapshotConfig = BuildSnapshotTemplateConfig();
            return client.GetAcsResponse(request);
        }
        #endregion

        #region 构建雪碧图截图配置【私有方法】

        /// <summary>
        /// 构建雪碧图截图配置【私有方法】
        /// </summary>
        /// <returns>雪碧图截图配置.</returns>
        private static string BuildSnapshotTemplateConfig()
        {
            // 覆盖参数
            JObject spriteSnapshotConfig = new JObject();
            spriteSnapshotConfig.Add("CellWidth", "120");
            spriteSnapshotConfig.Add("CellHeight", "68");
            spriteSnapshotConfig.Add("Columns", "3");
            spriteSnapshotConfig.Add("Lines", "10");
            spriteSnapshotConfig.Add("Padding", "20");
            spriteSnapshotConfig.Add("Margin", "50");
            //保留雪碧图原始图
            spriteSnapshotConfig.Add("KeepCellPic", "keep");
            spriteSnapshotConfig.Add("Color", "tomato");
            return spriteSnapshotConfig.ToString();
        }
        #endregion

    }
}
