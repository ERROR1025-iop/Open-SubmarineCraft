using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft
{
    public class IPreload : MonoBehaviour
    {

        public GameObject prefab;
        public float distance;

        string prefabPath;
        bool isInstance;
        int interval_time;

        JsonData info;

        public delegate void InstanceFinishDelegate(IPreload preload, GameObject gameObject);
        public event InstanceFinishDelegate OnInstanceFinish;

        public void initialized(string prefabPath, float distance)
        {
            this.prefabPath = prefabPath;
            gameObject.name = string.Format("preload({0})", prefabPath);
            initialized(distance);
        }

        public void initialized(GameObject prefab, float distance)
        {
            this.prefab = prefab;
            gameObject.name = string.Format("preload({0})", prefab.name);
            initialized(distance);   
        }

        public void setInfo(JsonData info)
        {
            this.info = info;
        }

        public JsonData getInfo()
        {
            return info;
        }

        void initialized(float distance)
        {
            isInstance = false;
            this.distance = distance;
            interval_time = (int)(Random.value * 80f);
        }

        void Update()
        {
            if(interval_time > 100)
            {
                if(Vector3.Distance( MainSubmarine.transform.position, transform.position) < distance)
                {
                    instanceGameObject();
                }
                interval_time = 0;
            }
            interval_time++;
        }

        public void instanceGameObject()
        {
            if (isInstance)
            {
                return;
            }

            Transform Ins_transform;

            if(prefab != null)
            {
                Ins_transform = Instantiate(prefab).transform;
            }
            else
            {
                Ins_transform = Instantiate(Resources.Load<GameObject>(prefabPath)).transform;
            }

            Ins_transform.position = transform.position;
            Ins_transform.rotation = transform.rotation;
            Ins_transform.localScale = transform.localScale;

            if(OnInstanceFinish != null)
            {
                OnInstanceFinish(this, Ins_transform.gameObject);
            }            

            isInstance = true;
        }
    }
}
