using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Text;
using System.Security.Cryptography;
using System;

namespace Scraft{ public class TpHttpManager
    {

        UnityAction call;

        //public static string serverIp = "http://127.0.0.1:801/thinkphp/scraft/";
        public static string serverIp = "http://scraft.91miaoyue.com/scraft/";
        //public static string serverIp = "http://www.91miaoyue.com/scraft/";
        public string tpAppPath;
        public string tpEnterFile;
        public string result;
        public string url;
        public WWWForm param;
        private List<string> postParam;
        public WWW postData;
        public IConfigBox configBox;
        public string loadingMsg;
        public IToast toast;
        public bool isDone;

        public TpHttpManager(IConfigBox configBox, IToast toast)
        {
            this.configBox = configBox;
            this.toast = toast;
            tpEnterFile = serverIp + "index.php";
            param = new WWWForm();
            postParam = new List<string>();
            loadingMsg = ILang.get("Loading", "menu");
        }

        public TpHttpManager()
        {
            tpEnterFile = serverIp + "index.php";
            param = new WWWForm();
            postParam = new List<string>();
            loadingMsg = ILang.get("Loading", "menu");
        }

        public void setTpPost(string controller, string file)
        {
            url = tpEnterFile + "/" + controller + "/" + file + ".html";
        }

        public void setTpPost(string controller, string file, string getParam)
        {
            url = tpEnterFile + "/" + controller + "/" + file + ".html" + "?" + getParam;
        }

        public void addPostParam(string key, string value)
        {
            param.AddField(key, value);
            postParam.Add(value);
        }

        public void addVersion()
        {
            addPostParam("version", GameSetting.version);
        }

        public void addToken()
        {
            string timeStamp = GetTimeStamp();
            addPostParam("timestamp", timeStamp);
            string value = "";
            foreach (string v in postParam)
            {
                value += v;
            }
            Debug.Log(value);
            value = MD5(value);
            Debug.Log(value);
            param.AddField("sign", value);
        }

        public void setListener(UnityAction listener)
        {
            this.call = listener;
        }

        public void setLoadingMsg(string t)
        {
            loadingMsg = t;
        }

        public void send()
        {
            Debug.Log(url);
            postData = new WWW(url, param);
            isDone = false;
            if (toast != null)
            {
                toast.show(loadingMsg);
            }

        }

        public void reset()
        {
            param = new WWWForm();
        }

        public void updata()
        {
            if (postData.isDone && !isDone)
            {
                isDone = true;
                if (toast != null)
                {
                    toast.hide();
                }
                if (postData.error != null && toast != null)
                {
                    toast.show(postData.error, 50);
                }
                else
                {
                    result = postData.text;
                    Debug.Log(result);
                    if (!result.Contains("Successed") && toast != null)
                    {
                        toast.show("no suc", 50);
                    }
                    else if (call != null)
                    {                       
                        call();
                    }

                }                
                reset();
            }
        }

        /// <summary> 
        /// 获取时间戳 
        /// </summary> 
        /// <returns></returns> 
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        public static string MD5(string source)
        {           
            byte[] sor = Encoding.UTF8.GetBytes(source);
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] result = md5.ComputeHash(sor);
            StringBuilder strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                strbul.Append(result[i].ToString("x2"));
            }
            string md5Result = strbul.ToString();          
            return md5Result;
        }
    }

}
