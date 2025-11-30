using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft
{
	public class HttpResponse 
	{
		public long code { get; }
		public string body { get; }

		public HttpResponse(long code, string body)
        {
			this.code = code;
			this.body = body;
        }
	}
}
