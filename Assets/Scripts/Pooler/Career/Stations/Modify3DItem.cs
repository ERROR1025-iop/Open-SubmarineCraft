using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Scraft.BlockSpace;
using Scraft.StationSpace;

namespace Scraft
{

    public class Modify3DItem : MonoBehaviour
    {
        Camera Camera3DWorld;
        static public int colliderTerrainLayer;
        Vector3 position;
        bool isClickDown;
        bool isPlace;
        bool isCanMove;
        BoxCollider[] boxColliders;
        IPoint[] materialBlocksCoor;
        Station station;

        Vector3 shotPosition;
        Vector3 shotEulerAngle;

        Vector3 orgPosition;
        Vector3 orgEulerAngle;

        void Start()
        {
            Camera3DWorld = Camera.main;
            boxColliders = GetComponents<BoxCollider>();
            colliderTerrainLayer = 1 << 10;
            isClickDown = true;
            setColliderEnabled(false);           
            PoolerTureFalseSelector.instance.show(true, onTrueButtonClick, onFalseButtonClick, onRotateButtonClick, onUpButtonClick, onDownButtonClick, false);

            SubCamera.instance.setBuilderStationMode(true, MainSubmarine.transform.position);

            shotPosition = new Vector3(-14.103f, 22.2978f, -19.198f);
            shotEulerAngle = new Vector3(54.524f, 44.794f, 0);

            orgPosition = transform.position;
            orgEulerAngle = transform.eulerAngles;
        }

        public void setColliderEnabled(bool enabled)
        {
            foreach (BoxCollider boxCollider in boxColliders)
            {
                boxCollider.enabled = enabled;
            }
        }

        public void setIsCanMove(bool b)
        {
            isCanMove = b;
        }

        public void setStation(Station station)
        {
            this.station = station;
        }    

        void onTrueButtonClick()
        {
            if (transform.position.y > 0)
            {
                setColliderEnabled(true);
                isPlace = true;
                Destroy(this);
                SubCamera.instance.setBuilderStationMode(false, MainSubmarine.transform.position);
                PoolerTureFalseSelector.instance.show(false);              

                //拍摄缩略图
                if (station != null)
                {                 
                    station.updateThumbnailImage();
                }

            }
            else
            {
                IToast.instance.show("Please place in surface.", 100);
            }
        }

        void onFalseButtonClick()
        {           
            SubCamera.instance.setBuilderStationMode(false, MainSubmarine.transform.position);
            PoolerTureFalseSelector.instance.show(false);
            transform.position = orgPosition;
            transform.eulerAngles = orgEulerAngle;
            Destroy(this);
        }

        void onRotateButtonClick()
        {
            transform.Rotate(Vector3.up, 15);
        }


        void onUpButtonClick()
        {
            transform.Translate(Vector3.up * 0.1f, Space.World);
        }

        void onDownButtonClick()
        {
            transform.Translate(Vector3.down * 0.1f, Space.World);
        }

        void Update()
        {
            if (isCanMove)
            {
                bool isPointGUI = GameSetting.isAndroid ? Input.touchCount > 0 ? EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) : false : EventSystem.current.IsPointerOverGameObject();
                if (!isPointGUI && !isPlace)
                {
                    clickDown();
                    movingMouse();
                    clickUp();
                }
            }            
        }

        void clickDown()
        {
            if (Input.GetMouseButtonDown(0) && getMouseVectorOn3D(out position))
            {
                if (position.y > 0)
                {
                    transform.position = position;
                    isClickDown = true;
                }
            }
        }

        void movingMouse()
        {
            if (Input.GetMouseButton(0))
            {
                if (isClickDown && getMouseVectorOn3D(out position))
                {
                    if (position.y > 0)
                    {
                        transform.position = position;
                    }
                }
            }
        }

        void clickUp()
        {
            if (Input.GetMouseButtonUp(0))
            {
                isClickDown = false;
            }
        }

        bool getMouseVectorOn3D(out Vector3 point)
        {
            point = Vector3.zero;

            if (World.activeCamera < 2)
            {
                return false;
            }

            Ray ray = Camera3DWorld.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, colliderTerrainLayer))
            {
                bool isPointGUI = GameSetting.isAndroid ? Input.touchCount > 0 ? EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) : false : EventSystem.current.IsPointerOverGameObject();
                if (isPointGUI)
                {
                    return false;
                }
                point = hit.point;
                return true;
            }

            return false;
        }
    }

}