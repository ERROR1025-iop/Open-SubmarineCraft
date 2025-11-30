using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft
{
	public class HttpJsonResponse<T>
	{
         public int code { get; }
         public T data { get; }

        public string message { get; }


        public HttpJsonResponse(HttpResponse httpResponse)
        {
			if(httpResponse.code == 200)
            {
                Log.input().debug(httpResponse.body);
                try
                {
                    ResponseEntity<T> entity = JsonUtils.unserialize<ResponseEntity<T>>(httpResponse.body);
                    code = entity.code;
                    data = entity.data;
                    if(code <= 0)
                    {
                        throw new Exception("code <= 0");
                    }
                }
                catch (Exception)
                {
                    ResponseEntity<String> entityString = JsonUtils.unserialize<ResponseEntity<String>>(httpResponse.body);
                    message = entityString.data;
                }   
            }
            else
            {
                code = -(int)httpResponse.code;
                message = httpResponse.body;
            }
        }

        public class ResponseEntity<A>
        {
            public int code;
            public A data;
        }

        public class MessageEntity
        {
            public string msg;
        }
    }
}
