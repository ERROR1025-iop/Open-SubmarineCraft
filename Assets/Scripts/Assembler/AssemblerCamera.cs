using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Scraft
{
    public class AssemblerCamera : MonoBehaviour
    {

        public IJoystick joystick;

        Vector3 lastPonitPos;
        Vector3 center;
        public Camera camera3D;

        float speed = 1f;
        float horizontal;
        float vertical;
        public static float canvasW;

        void Start()
        {
            lastPonitPos = Vector3.zero;
            var canvasScaler = GameObject.Find("Canvas").GetComponent<CanvasScaler>();
            canvasW = canvasScaler.referenceResolution.x;
        }


        void Update()
        {

            if (joystick.isPointed)
            {
                rotate();
            }

            if (GameSetting.isAndroid)
            {
                androidMove();
            }
            else
            {
                androidMove();
                pcMove();
                pcScale();
                pcRotate();
            }
        }

        void rotate()
        {
            center = IUtils.centerOfGameObjects(IRT.Selection.gameObjects);

            transform.RotateAround(center, Vector3.up, joystick.x * 3f);
            transform.RotateAround(center, transform.right, -joystick.y * 3f);

            //transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.eulerAngles.x - IJoystick.y * 3f, transform.localRotation.eulerAngles.y + IJoystick.x * 3f, 0));
        }

        void androidMove()
        {
            if (Input.touchCount == 2)
            {
                if (Input.GetMouseButton(0))
                {
                    if (lastPonitPos.Equals(Vector3.zero))
                    {
                        lastPonitPos = Input.mousePosition;
                    }
                    Vector3 dv = (Input.mousePosition - lastPonitPos) * 0.01f;
                    camera3D.transform.Translate(-dv);
                    lastPonitPos = Input.mousePosition;

                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                lastPonitPos = Vector3.zero;
            }
        }

        void pcMove()
        {
            horizontal = Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0;
            vertical = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
            transform.Translate(horizontal * Time.deltaTime * speed * 5, vertical * Time.deltaTime * speed * 5, 0);
        }

        void pcScale()
        {
            float Distance = 0;
            Distance -= Input.GetAxis("Mouse ScrollWheel") * 3f;
            transform.Translate(0, 0, -Distance);
        }

        void pcRotate()
        {
            if ((Input.GetMouseButton(1) || Input.GetMouseButton(2)) && !Input.GetMouseButton(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    center = IUtils.centerOfGameObjects(IRT.Selection.gameObjects);
                    moveX();
                    moveY();
                }
            }

            if ((Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2)) && !Input.GetMouseButton(0))
            {
                lastPonitPos = Vector3.zero;
            }
        }

        void moveX()
        {
            if (lastPonitPos.x == 0)
            {
                lastPonitPos.x = IUtils.reviseMousePos(Input.mousePosition, canvasW).x;
            }
            float dy = (IUtils.reviseMousePos(Input.mousePosition, canvasW).x - lastPonitPos.x) * 0.2f * speed;

            lastPonitPos.x = IUtils.reviseMousePos(Input.mousePosition, canvasW).x;
            transform.RotateAround(center, Vector3.up, dy);
        }

        void moveY()
        {
            if (lastPonitPos.y == 0)
            {
                lastPonitPos.y = IUtils.reviseMousePos(Input.mousePosition, canvasW).y;
            }
            float dy = (IUtils.reviseMousePos(Input.mousePosition, canvasW).y - lastPonitPos.y) * 0.2f * speed;

            lastPonitPos.y = IUtils.reviseMousePos(Input.mousePosition, canvasW).y;
            transform.RotateAround(center, transform.right, -dy);
        }
    }
}