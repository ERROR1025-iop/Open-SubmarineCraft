using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scraft {
    public class CraftCamera : MonoBehaviour
    {
        Vector3 lastPonitPos;
        Vector3 lastMapPos;

        Camera camera;

        static public int collider2DLayer;

        void Start()
        {
            camera = Camera.main;
            lastMapPos = camera.transform.localPosition;
            lastPonitPos = Vector3.zero;

            collider2DLayer = 1 << 13;
        }


        void Update()
        {
            Vector3 v;
            bool result = getMouseVector(out v);
            if (result)
            {
                moveMap(v);
            }
        }

        void moveMap(Vector3 v)
        {
            if (Input.GetMouseButton(0))
            {               
                if (lastPonitPos.Equals(Vector3.zero))
                {
                    lastPonitPos = Input.mousePosition;
                }
                Vector3 dv = (Input.mousePosition - lastPonitPos) * camera.orthographicSize * 0.005f;
                camera.transform.localPosition = lastMapPos - dv;
                lastPonitPos = Input.mousePosition;
                lastMapPos = camera.transform.localPosition;
            }
            if (Input.GetMouseButtonUp(0))
            {
                lastPonitPos = Vector3.zero;
            }
        }

        bool getMouseVector(out Vector3 vector)
        {
            vector = Vector3.zero;            
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

                bool isPointGUI = GameSetting.isAndroid ? Input.touchCount > 0 ? EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) : false : EventSystem.current.IsPointerOverGameObject();
                if (isPointGUI)
                {
                    return false;
                }
                vector = hit.point;
                if (Input.GetMouseButtonDown(0))
                {
                    if (hit.collider.CompareTag("preload ship"))
                    {
                        CraftShipIcon craftShipIcon = hit.collider.GetComponent<CraftShipIcon>();
                        craftShipIcon.onShipIconClick();
                    }
                }
                return true;
            }
            return false;
        }
    }
}
