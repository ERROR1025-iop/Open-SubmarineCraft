using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Scraft
{
    /// <summary>
    /// 用于检测主角（MainSubmarine）是否在该区域内，并提供进入/离开回调。
    /// 不修改基类 `AreaDetector`。
    /// </summary>
    public class AreaDetectorActive : AreaDetector 
    {
        [Header("Callbacks")]
        [Tooltip("当主角进入区域时触发（可在Inspector中绑定方法）")]
        public UnityEvent onEnter;

        [Tooltip("当主角离开区域时触发（可在Inspector中绑定方法）")]
        public UnityEvent onExit;

        // 内部状态：记录上一次检测时主角是否在区域内
        private bool isInside = false;

        private void Start()
        {
            // 初始化状态（如果主角已经存在并在场景中）
            UpdateInsideStateInitial();
        }

        private void LateUpdate()
        {
            // 运行时持续检测主角是否在区域中
            if (MainSubmarine.instance == null || MainSubmarine.transform == null) return;

            bool nowInside = IsPointInArea(MainSubmarine.transform.position);

            if (nowInside && !isInside)
            {
                // 发生进入
                isInside = true;
                onEnter?.Invoke();
                OnEnterArea();
            }
            else if (!nowInside && isInside)
            {
                // 发生离开
                isInside = false;
                onExit?.Invoke();
                OnExitArea();
            }
        }        

        private void UpdateInsideStateInitial()
        {
            if (MainSubmarine.instance == null || MainSubmarine.transform == null) return;
            isInside = IsPointInArea(MainSubmarine.transform.position);
        }

        /// <summary>
        /// 子类可以重写此方法以响应进入事件（同时会触发 `onEnter` UnityEvent）。
        /// </summary>
        protected virtual void OnEnterArea() { 
            
        }

        /// <summary>
        /// 子类可以重写此方法以响应离开事件（同时会触发 `onExit` UnityEvent）。
        /// </summary>
        protected virtual void OnExitArea() {
            
        }
    }
}