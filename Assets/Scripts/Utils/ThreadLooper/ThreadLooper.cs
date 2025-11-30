using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Scraft
{
    // 保持 Runable 委托定义，与原有代码兼容
    

    public class ThreadLooper : MonoBehaviour
    {
        private static ThreadLooper sInstance;

        private readonly object mLock = new object();
        private Queue<Runable> mRunableQueue = new Queue<Runable>();

        private int mMainThreadId;

        public static ThreadLooper get()
        {
            return sInstance;
        }

        private void Awake()
        {
            // 防止重复创建
            if (sInstance != null && sInstance != this)
            {
                Destroy(gameObject);
                return;
            }

            sInstance = this;
            mMainThreadId = Thread.CurrentThread.ManagedThreadId;

            // 关键：使此对象在场景切换时不被销毁
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            Queue<Runable> queueToProcess = null;

            // 在锁中拷贝队列内容，避免在Update中直接操作被多线程修改的队列
            lock (mLock)
            {
                if (mRunableQueue.Count > 0)
                {
                    queueToProcess = new Queue<Runable>(mRunableQueue);
                    mRunableQueue.Clear();
                }
            }

            // 在主线程中安全执行
            if (queueToProcess != null)
            {
                while (queueToProcess.Count > 0)
                {
                    Runable runable = queueToProcess.Dequeue();
                    runable?.Invoke();
                }
            }
        }

        public bool isMainThread()
        {
            return Thread.CurrentThread.ManagedThreadId == mMainThreadId;
        }

        public void runMainThread(Runable runable)
        {
            if (runable == null) return;

            lock (mLock)
            {
                mRunableQueue.Enqueue(runable);
            }
        }

        public void autoRunMainThread(Runable runable)
        {
            if (isMainThread())
            {
                runable?.Invoke();
            }
            else
            {
                runMainThread(runable);
            }
        }

        public void startCoroutine(IEnumerator routine)
        {
            StartCoroutine(routine);
        }
    }
}