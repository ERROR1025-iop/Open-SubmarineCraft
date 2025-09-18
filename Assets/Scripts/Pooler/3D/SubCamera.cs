using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class SubCamera : MonoBehaviour
    {

        static public SubCamera instance;
                
        static public bool isAimMode;
        public GameObject aimMask;
        Transform aimTransform;
        bool isShowMask;

        static public bool isFollowMode;
        public Button followButton;
        Text followButtonText;
        Transform followTransform;
        static public Transform lastWeaponTransform;
        Transform followRoot;

        static public bool isBuildStationMode;      

        Vector3 lastPosition;
        Quaternion lastRotation;

        Camera camera3D;
        new Light light;
        Vector3 lastPos;
        bool lockRotate;
        bool isPointGUI;        
        Transform followSubTrans;
        Transform rotateCenterTrans;
        static public bool canMove;
        static public bool canScale;
        static public bool canRotate;

        Vector3 lastPonitPos;
        Vector3 center;

        float speed = 1f;
        float horizontal;
        float vertical;

        void Awake()
        {
            camera3D = GetComponent<Camera>();
            light = GetComponent<Light>();
            lockRotate = false;
            instance = this;
            isAimMode = false;
            isFollowMode = false;
            isBuildStationMode = false;
            canMove = true;
            canScale = true;
            canRotate = true;
            rotateCenterTrans = MainSubmarine.transform;
            followSubTrans = GameObject.Find("3D FollowSub").transform;
            followRoot = GameObject.Find("3D FollowRoot").transform;

            followButton.onClick.AddListener(onFollowButtonClick);
            followButtonText = followButton.transform.GetChild(0).GetComponent<Text>();

            lastPosition = transform.localPosition;
            lastRotation = transform.localRotation;   

            light.enabled = false;          
        }


        void onFollowButtonClick()
        {
            setFollow(!isFollowMode);
        }

        public void setFollow(bool open)
        {
            isFollowMode = open;
            if (isFollowMode)
            {
                if (lastWeaponTransform != null)
                {
                    lastPosition = transform.localPosition;
                    lastRotation = transform.localRotation;
                    camera3D.fieldOfView = 60;
                    aimMask.SetActive(false);

                    followTransform = lastWeaponTransform;
                    followRoot.position = followTransform.position;                    
                    rotateCenterTrans = followRoot;
                    transform.SetParent(followRoot);
                    transform.localPosition = followTransform.forward * -2;
                    transform.LookAt(followTransform);
                    light.enabled = true;
                }
                else
                {
                    isFollowMode = false;
                }
            }
            else
            {
                if (isAimMode && aimTransform != null)
                {
                    setAimMode(true, aimTransform, lastRotation.eulerAngles, isShowMask);
                }
                else
                {
                    resetCamera(true);
                }
                followTransform = null;
                light.enabled = false;
            }
            followButtonText.text = ILang.get(isFollowMode ? "Cancel" : "Follow");
        }

        void follow()
        {
            if (isFollowMode && followTransform != null)
            {
                followRoot.position = followTransform.position;
            }
        }

        public void onFollowTransformDestory(Transform wt)
        {
            if (isFollowMode && followTransform.Equals(wt))
            {                              
                rotateCenterTrans = followRoot;
                Invoke("returnToMainSubmarine", 1.0f);
            }
        }

        void returnToMainSubmarine()
        {
            setFollow(false);
        }

        public void setAimMode(bool open, Transform rotateCenterTrans, Vector3 eulerAngles, bool showMask)
        {
            isAimMode = open;
            if (isAimMode)
            {
                canMove = false;
                isShowMask = showMask;
                aimTransform = rotateCenterTrans;
                this.rotateCenterTrans = rotateCenterTrans;
                transform.SetParent(rotateCenterTrans);
                transform.localPosition = Vector3.zero;
                transform.localEulerAngles = eulerAngles;
                aimMask.SetActive(showMask);
                canScale = false;
            }
            else
            {
                aimMask.SetActive(false);
                resetCamera(false);
            }
        }

        public void setBuilderStationMode(bool set, Vector3 position)
        {
            isBuildStationMode = set;
            if (isBuildStationMode)
            {
                canScale = false;
                canMove = false;
                canRotate = false;
                lastPosition = transform.localPosition;
                lastRotation = transform.localRotation;

                transform.position = position + new Vector3(0,50,0);
                transform.eulerAngles = new Vector3(89, -89, 0);
                if(World.activeCamera < 2)
                {
                    PoolerUI.instance.onPeriscopeButtonClick();
                }                
            }
            else
            {
                resetCamera(true);
            }
        }

        public void resetCamera(bool resetPosition)
        {
            transform.SetParent(PoolerUI.instance.viewLockButton.value ? MainSubmarine.transform : followSubTrans);
            if (resetPosition)
            {
                transform.localPosition = lastPosition;
                transform.localRotation = lastRotation;
                rotateCenterTrans = MainSubmarine.transform;
            }
            canMove = true;
            canScale = true;
            canRotate = true;
        }

        void Update()
        {

            follow();
            setRadarLineAngle();

            //防止摄像机翻滚
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);

            if (World.activeCamera < 2)
            {
                return;
            }

            isPointGUI = IUtils.isPointGUI();
            move();
            rotate();
            pcScale();
        }

        void move()
        {
            if (!canMove)
            {
                return;
            }

            if (!isPointGUI)
            {
                if ((GameSetting.isAndroid && Input.touchCount == 2) || Input.GetMouseButton(1))
                {
                    if (lastPonitPos.Equals(Vector3.zero))
                    {
                        lastPonitPos = Input.mousePosition;
                    }
                    Vector3 dv = (Input.mousePosition - lastPonitPos) * 0.01f;
                    transform.Translate(-dv);
                    lastPonitPos = Input.mousePosition;
                    canRotate = false;
                }
            }

            if (GameSetting.isAndroid && Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                lastPonitPos = Vector3.zero;
                canRotate = true;
            }
        }

        void rotate()
        {
            if (!canRotate)
            {
                return;
            }

            if (isPointGUI && Input.GetMouseButtonDown(0))
            {
                lockRotate = true;
            }

            if (PoolerItemSelector.instance.joystick1.isPointed)
            {
                lockRotate = true;
            }
           
            if (!lockRotate && !isPointGUI && Input.GetMouseButton(0))
            {
                if (World.activeCamera >= 2)
                {
                    if (!GameSetting.isAndroid || Input.touchCount == 1)
                    {
                        rotateX();
                        rotateY();                        
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                lastPos = new Vector3(0, 0, 0);
                lockRotate = false;              
            }            
        }

        void setRadarLineAngle()
        {
            Vector3 cameraVector = new Vector3(transform.forward.x, 0, transform.forward.z);
            //Debug.DrawLine(transform.localPosition, transform.localPosition + cameraVector);
            Vector3 rotation = Quaternion.LookRotation(cameraVector).eulerAngles;
            MainSubmarine.fireAngle = rotation.y + 90;
            Radar.instance.setLineAngle(MainSubmarine.getAngle() - MainSubmarine.fireAngle);
        }

        void rotateX()
        {
            float speed = isAimMode ? camera3D.fieldOfView * 0.5f : 30;
            if (lastPos.x == 0)
            {
                lastPos.x = IUtils.reviseMousePos(Input.mousePosition).x;
            }
            float dy = (IUtils.reviseMousePos(Input.mousePosition).x - lastPos.x) * 0.005f * speed;
            transform.RotateAround(rotateCenterTrans.position, Vector3.up, dy * 3f);
            lastPos.x = IUtils.reviseMousePos(Input.mousePosition).x;

        }

        void rotateY()
        {
            float speed = isAimMode ? camera3D.fieldOfView * 0.5f : 30;
            if (lastPos.y == 0)
            {
                lastPos.y = IUtils.reviseMousePos(Input.mousePosition).y;
            }
            float dx = (IUtils.reviseMousePos(Input.mousePosition).y - lastPos.y) * 0.005f * speed;
            transform.RotateAround(rotateCenterTrans.position, transform.right, -dx * 3f);
            lastPos.y = IUtils.reviseMousePos(Input.mousePosition).y;

        }

        void pcScale()
        {
            if (GameSetting.isAndroid)
            {
                return;
            }

            if (isAimMode)
            {
                float speed = 50;
                float dy = -Input.GetAxis("Mouse ScrollWheel") * speed;
                float size = camera3D.fieldOfView;
                if ((size > 2 && dy < 0) || size < 60 && dy > 0)
                {
                    camera3D.fieldOfView = Mathf.Clamp(size + dy, 2, 60);
                }
            }
            else if(canScale)
            {
                float Distance = 0;
                float speed = transform.parent.Equals(MainSubmarine.transform) ? 1 : 5;
                Distance -= Input.GetAxis("Mouse ScrollWheel") * speed;
                transform.Translate(0, 0, -Distance);
            }
        }
    }
}