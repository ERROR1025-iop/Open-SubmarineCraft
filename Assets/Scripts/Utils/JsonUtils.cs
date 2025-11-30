using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft
{
	public class JsonUtils 
	{		

        public static string serialize(object entity)
        {            
            return JsonConvert.SerializeObject(entity);
        }

        public static T unserialize<T>(string text)
        {
            try {
                return JsonConvert.DeserializeObject<T>(text);
            }catch(Exception e)
            {
                Debug.LogError("json反序列错误:" + text);
            }
            return (T)new object();
        }
    }
}
