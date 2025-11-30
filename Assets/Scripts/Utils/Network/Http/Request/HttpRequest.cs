using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Scraft
{
	public class HttpRequest 
	{
		WWWForm mWWWForm = new WWWForm();
		protected string mUrl;

		public HttpRequest(string url)
        {
			mUrl = url;
		}

		public void addFormData(string key, string value)
		{			
			mWWWForm.AddField(key, value);
		}

		public WWWForm getFormDatas()
        {
			return mWWWForm;
		}

		public string getUrl()
        {
			return mUrl;

		}
	}
}
