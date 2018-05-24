using EHttp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaiduTranslateSDK
{
    public static class BDTranslate
    {
        /// <summary>
        /// 初始化成功
        /// </summary>
        private static bool _initSuccess = false;
        /// <summary>
        /// 初始化api url
        /// </summary>
        public static string InitApiUrl { get; set; } = "http://fanyi.baidu.com/";
        /// <summary>
        /// 检测语言代码的Api
        /// </summary>
        public static string LangdetectApiUrl { get; set; } = "http://fanyi.baidu.com/langdetect/";
        /// <summary>
        /// 翻译api url
        /// </summary>
        public static string TranslateApiUrl { get; set; } = "http://fanyi.baidu.com/basetrans/";
        /// <summary>
        /// 签名
        /// </summary>
        public static string Sign { get; set; }
        /// <summary>
        /// Token
        /// </summary>
        public static string Token { get; set; }
        /// <summary>
        /// 请求数据
        /// </summary>
        public static HttpData ReqData { get; set; }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public static bool Init()
        {
            if (_initSuccess)
                return true;

            ReqData = new HttpData();
            ReqData.Method = RequestType.GET;
            ReqData.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            ReqData.Host = "fanyi.baidu.com";
            ReqData.Origin = "http://fanyi.baidu.com";
            ReqData.Referer = "http://fanyi.baidu.com";
            ReqData.Accept = "*/*";
            ReqData.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_1 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E304 Safari/602.1";
            ReqData.Data = new Dictionary<string, string>();
            var data = InitApiUrl.HttpRequest(ReqData);
            _initSuccess = data;
            return data;
        }

        /// <summary>
        /// 重新初始化
        /// </summary>
        /// <returns></returns>
        public static bool ReInit()
        {
            if (!_initSuccess)
                return true;
            _initSuccess = false;

            return Init();
        }

        /// <summary>
        /// 立即翻译
        /// </summary>
        /// <param name="str">翻译的字符串</param>
        /// <param name="from">源语言代码</param>
        /// <param name="to">目标语言代码</param>
        /// <param name="tryGetFromCodeCount">源语言是auto,尝试获取源语言代码的次数</param>
        /// <param name="tryTranslateCount">尝试翻译的次数</param>
        /// <returns>响应结果(请求出错返回null)</returns>
        public static dynamic Translate(this string str, string from = "auto", string to = "zh", int tryGetFromCodeCount = 10, int tryTranslateCount = 10)
        {
            if (!_initSuccess)
            {
                throw new Exception("SDK Uninitialized！");
            }

            #region 自动检测输入的语言

            if (from == "auto")
            {
                var tryGetFromCodeJS = 1;
                while (true)
                {
                    var tmp = Langdetect(str);
                    if (tmp != null && tmp.error == 0)
                    {
                        from = tmp.lan;
                        break;
                    }

                    // 检测源语言失败
                    if (tryGetFromCodeJS++ == 10)
                        throw new Exception("Automatic detection failure!");
                }

            }

            #endregion


            #region 设置请求数据

            ReqData.Method = RequestType.POST;
            // 查询数据
            ReqData.Data["query"] = str;
            ReqData.Data["from"] = from;
            ReqData.Data["to"] = to;

            #endregion


            #region 进行翻译

            var tryTranslateJS = 1;
            while (true)
            {
                // 请求翻译结果
                if (TranslateApiUrl.HttpRequest(ReqData))
                {
                    return JsonConvert.DeserializeObject<dynamic>(ReqData.ResponseData);
                }
                // 请求失败计数跳出循环
                if (tryTranslateJS++ == tryTranslateCount)
                {
                    break;
                }
            }


            #endregion



            return null;
        }

        // http://fanyi.baidu.com/langdetect
        /// <summary>
        /// 自动检测输入的语言类型
        /// 
        /// {错误码,信息,语言编码}
        /// 
        /// {"error":0,"msg":"success","lan":"en"}
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static dynamic Langdetect(this string str)
        {
            var tmpStr = string.Empty;


            ReqData.Method = RequestType.POST;
            ReqData.Data.Clear();

            // 切割过长字符串
            if (str.Length > 20 && str.Length != 20)
            {
                tmpStr = str.Substring(0, 20);
            }
            else
            {
                tmpStr = str;
            }

            ReqData.Data["query"] = tmpStr;


            if (LangdetectApiUrl.HttpRequest(ReqData))
            {
                return JsonConvert.DeserializeObject<dynamic>(ReqData.ResponseData);
            }



            return null;
        }
    }
}
