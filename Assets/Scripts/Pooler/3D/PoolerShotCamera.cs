using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft {
    public class PoolerShotCamera : MonoBehaviour
    {
        static public PoolerShotCamera instance;

        static public Camera shotCamera;

        static public Transform preShotParent;

        void Awake()
        {
            instance = this;
            shotCamera = GetComponent<Camera>();
            shotCamera.enabled = false;
            preShotParent = GameObject.Find("PreShotParent").transform;
        }

        public Texture2D shot(Vector3 position,Vector3 eulerAngle ,float size,string savePath, Rect rect)
        {
            shotCamera.enabled = true;
            transform.position = position;
            transform.eulerAngles = eulerAngle;
            shotCamera.orthographicSize = size;
            Texture2D texture2D = IUtils.captureScreen(shotCamera, rect);
            IUtils.saveTexture2D2SD(texture2D, savePath);
            shotCamera.enabled = false;
            return texture2D;
        }
    }
}
