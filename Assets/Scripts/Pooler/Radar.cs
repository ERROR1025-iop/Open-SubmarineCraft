using Scraft.BlockSpace;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class Radar
    {
        public float radarZoom = 5;

        public static Radar instance;

        Transform radarTrans;
        Transform pointParent;
        public RectTransform radarRectTrans;
        public RectTransform pointParentRectTrans;
        public RectTransform lineRectTrans;
        Text speedText;
        Text posText;

        GameObject upArrObject;
        GameObject rightArrObject;
        GameObject downArrObject;
        GameObject leftArrObject;

        public Radar()
        {
            instance = this;

            radarTrans = GameObject.Find("Canvas/radar rect/radar").transform;
            pointParent = radarTrans.GetChild(0);
            lineRectTrans = radarTrans.GetChild(5).GetComponent<RectTransform>();
            speedText = GameObject.Find("Canvas/radar rect/radar text rect/speed").GetComponent<Text>();
            posText = GameObject.Find("Canvas/radar rect/radar text rect/coor").GetComponent<Text>();

            upArrObject = radarTrans.GetChild(1).gameObject;
            rightArrObject = radarTrans.GetChild(2).gameObject;
            downArrObject = radarTrans.GetChild(3).gameObject;
            leftArrObject = radarTrans.GetChild(4).gameObject;

            radarRectTrans = radarTrans.GetComponent<RectTransform>();
            pointParentRectTrans = pointParent.GetComponent<RectTransform>();

        }

        public void setSpeed(float speed)
        {
            //speedText.text = speed.ToString("f1") + "m/s";
            speedText.text = (speed * 1.5f * 3.6f ).ToString("f1") + "km/h";
        }

        public void setAngle(float angle)
        {
            pointParent.localEulerAngles = new Vector3(0, 0, angle - 90);
        }

        public void setLocation(Vector2 loc)
        {
            posText.text = loc.x.ToString("f1") + " , " + loc.y.ToString("f1");
        }

        public void addPoint(Transform point)
        {
            point.SetParent(pointParent);
        }

        public void setLineAngle(float angle)
        {
            lineRectTrans.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            float lineSizeAngle = angle;
            if (lineSizeAngle > 45)
            {
                while (lineSizeAngle > 45)
                {
                    lineSizeAngle -= 90;
                }
            }
            else if (lineSizeAngle < 0)
            {
                while (lineSizeAngle < 0)
                {
                    lineSizeAngle += 90;
                }
            }

            if (lineSizeAngle < 45)
            {
                lineRectTrans.sizeDelta = new Vector2(1, 81 / Mathf.Sin(IUtils.angle2radian(90 - Mathf.Abs(lineSizeAngle))));
            }
            else
            {
                lineRectTrans.sizeDelta = new Vector2(1, 81 / Mathf.Sin(IUtils.angle2radian(Mathf.Abs(lineSizeAngle))));
            }

        }

        public void showArr(int dir, bool isShow)
        {
            switch (dir)
            {
                case Dir.up:
                    upArrObject.SetActive(isShow);
                    break;
                case Dir.right:
                    rightArrObject.SetActive(isShow);
                    break;
                case Dir.down:
                    downArrObject.SetActive(isShow);
                    break;
                case Dir.left:
                    leftArrObject.SetActive(isShow);
                    break;
                default:
                    break;
            }
        }

        public bool isShowArr(int dir)
        {
            switch (dir)
            {
                case Dir.up:
                    return upArrObject.activeSelf;
                case Dir.right:
                    return rightArrObject.activeSelf;
                case Dir.down:
                    return downArrObject.activeSelf;
                case Dir.left:
                    return leftArrObject.activeSelf;
                default:
                    return false;
            }
        }
    }
}