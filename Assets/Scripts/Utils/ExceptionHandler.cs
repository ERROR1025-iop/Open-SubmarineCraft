using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System;

namespace Scraft {
    public class ExceptionHandler : MonoBehaviour
    {
        
        void Start()
        {
            Application.RegisterLogCallback(Handler);
        }

        void OnDestory()
        {
            //清除注册
            Application.RegisterLogCallback(null);
        }

        void Handler(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            {
                string logPath = GamePath.logFolder + DateTime.Now.ToString("yyyy_MM_dd") + ".txt";
                //打印日志
                if (Directory.Exists(GamePath.logFolder))
                {
                    string log = "\r\n============================\r\n";
                    log += "[time]:" + DateTime.Now.ToString() + "\r\n";
                    log += "[scene]:" + SceneManager.GetActiveScene().name + "\r\n";
                    log += "[type]:" + type.ToString() + "\r\n";
                    log += "[exception message]:" + logString + "\r\n";
                    log += "[stack trace]:" + stackTrace + "\r\n";

                    FileInfo fi = new FileInfo(logPath);
                    if (fi.Exists)
                    {
                        string olog = IUtils.readFromTxt(logPath);
                        IUtils.write2txt(logPath, olog + log);
                    }
                    else
                    {
                        IUtils.write2txt(logPath, log);
                    }
                        
                    if(IToast.instance != null)
                    {
                        //IToast.instance.show(string.Format(ILang.get("log error"), logPath), 200);
                    }                    
                }               
            }
        }

        static public void writeLog(string message)
        {
            string logPath = GamePath.logFolder + DateTime.Now.ToString("yyyy_MM_dd") + "_msg.txt";
            //打印日志
            if (Directory.Exists(GamePath.logFolder))
            {
                string log = "[time]:" + DateTime.Now.ToString() + "\r\n";
                log += "[scene]:" + SceneManager.GetActiveScene().name + "\r\n";
                log += "[type]:message\r\n";
                log += "[exception message]:" + message + "\r\n";
                FileInfo fi = new FileInfo(logPath);
                if (fi.Exists)
                {
                    string olog = IUtils.readFromTxt(logPath);
                    IUtils.write2txt(logPath, olog + log);
                }
                else
                {
                    IUtils.write2txt(logPath, log);
                }
                if (IToast.instance != null)
                {
                    //IToast.instance.show(string.Format(ILang.get("log error"), logPath), 100);
                }
            }
        }
    }
}
