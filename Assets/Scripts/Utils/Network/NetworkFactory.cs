using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft
{
    public class NetworkFactory 
    {
        //public const string SCRAFT_HOST = "http://127.0.0.1:3200";
        public const string SCRAFT_HOST = "https://scraft.aipie.cool";
        //public const string AUTH_HOST = "http://127.0.0.1:9010";
        public const string AUTH_HOST = "https://auth.aipie.cool";
        private static object sSingletonLock = new object();
        private static HttpNet sHttpNet;

        public static HttpNet getHttpNet()
        {
            if (sHttpNet == null)
            {
                lock (sSingletonLock)
                {
                    if (sHttpNet == null)
                    {                       
                        sHttpNet = new HttpNet();
                    }
                }
            }
            return sHttpNet;
        }
    }
}
