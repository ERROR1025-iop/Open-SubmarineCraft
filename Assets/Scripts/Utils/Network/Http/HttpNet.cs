using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Scraft
{
    public delegate void HttpCallback(HttpResponse response);

    public class HttpNet 
    {
        public HttpNet()
        {
            
        }

        // 已有的回调式 post 方法
        public void post(HttpRequest request, HttpCallback callback)
        {
            ThreadLooper.get().startCoroutine(postRequest(request, callback));
        }

        // 新增：异步版本的 post 方法，返回 Task<HttpResponse>
        public async Task<HttpResponse> PostAsync(HttpRequest request)
        {
            var tcs = new TaskCompletionSource<HttpResponse>();

            ThreadLooper.get().startCoroutine(postRequestAsyncWrapper(request, response =>
            {
                tcs.SetResult(response);
            }));

            return await tcs.Task;
        }

        private IEnumerator postRequestAsyncWrapper(HttpRequest request, System.Action<HttpResponse> actionCallback)
        {
            // 关键：将 Action<HttpResponse> 转为 HttpCallback
            HttpCallback callback = new HttpCallback(actionCallback);
            yield return postRequest(request, callback);
        }
        // 原来的协程处理逻辑（保持不变）
        private IEnumerator postRequest(HttpRequest request, HttpCallback callback)
        {
            string httpLog = "发送post请求:\n" + request.getUrl() + "\n";
            var www = UnityWebRequest.Post(request.getUrl(), request.getFormDatas());
            yield return www.SendWebRequest();            

            if (www.isNetworkError || www.isHttpError)
            {
                httpLog += "\n请求返回:\ncode:" + www.responseCode + "\nerror:" + www.downloadHandler.text;
                Log.input().info(httpLog);
                callback(new HttpResponse(www.responseCode, www.error + www.downloadHandler.text));               
            }
            else
            {
                httpLog += "请求返回:\ncode:" + www.responseCode + "\nbody:" + www.downloadHandler.text;
                Log.input().info(httpLog);
                callback(new HttpResponse(www.responseCode, www.downloadHandler.text));
            }
        }
    }
}