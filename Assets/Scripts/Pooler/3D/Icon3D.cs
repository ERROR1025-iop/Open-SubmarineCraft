using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class Icon3D : MonoBehaviour
    {
        public static GameObject s_TempIcon;
        public GameObject iconGameObject;
        public bool hasDistanceTxt;
        public bool transparentWithDistance;
        /// <summary>
        ///0:none;1:all;2:self torpedp+target;3:torpedp
        /// </summary>
        static public int showMode = 0;

        /// <summary>
        ///0:none;1:self torpedp;2:target;3:target torpedp
        /// </summary>
        public int iconTag = 0;

        static Transform CanvasTrans;
        static Camera camera3D;
        Text distanceTxt;
        RectTransform iconRectTrans;
        Image iconImage;
        Color color;

        void Awake()
        {
            if(iconGameObject == null)
            {
                iconGameObject = s_TempIcon;
            }
            iconGameObject = Instantiate(iconGameObject);
            iconGameObject.layer = 8;
            if (CanvasTrans == null)
            {
                CanvasTrans = GameObject.Find("Canvas").transform;
                camera3D = Camera.main;
            }
            iconGameObject.transform.SetParent(CanvasTrans);
            iconRectTrans = iconGameObject.GetComponent<RectTransform>();
            iconImage = iconGameObject.GetComponent<Image>();
            color = iconImage.color;

            if (hasDistanceTxt)
            {
                distanceTxt = iconGameObject.transform.GetChild(0).GetComponent<Text>();
            }
        }

        public void setIconColor(Color color)
        {
            this.color = color;
            iconImage.color = color;
            if (hasDistanceTxt)
            {
                distanceTxt.color = color;
            }
        }

        /// <summary>
        ///0:none;1:self torpedp;2:target;3:target torpedp
        /// </summary>
        public void setIconTag(int tag)
        {
            iconTag = tag;
        }

        void Update()
        {
            if (!Pooler.isGlobalShowIcon)
            {
                iconGameObject.SetActive(false);
                return;
            }

            if (World.activeCamera == 1)
            {
                iconGameObject.SetActive(false);
                return;
            }

            if (showMode == 0)
            {
                iconGameObject.SetActive(false);
                return;
            }
            if (showMode != 1)
            {
                if (showMode != 2 && !(iconTag == 1 || iconTag == 2))
                {
                    iconGameObject.SetActive(false);
                    return;
                }
                if (showMode != 3 && !(iconTag == 1 || iconTag == 3))
                {
                    iconGameObject.SetActive(false);
                    return;
                }
            }

            iconGameObject.transform.position = camera3D.WorldToScreenPoint(transform.position);

            if (World.activeCamera == 0)
            {
                if (iconRectTrans.anchoredPosition.x > -163 || iconRectTrans.anchoredPosition.y < 100)
                {
                    iconGameObject.SetActive(false);
                    return;
                }
            }

            bool isFornt = Vector3.Dot(camera3D.transform.forward, transform.position - camera3D.transform.position) > 0;
            iconGameObject.SetActive(isFornt);


            float distance = Vector3.Distance(transform.position, MainSubmarine.transform.position) * 10;
            if (transparentWithDistance)
            {
                float a = 1.1f - (IUtils.sigmoid(distance * 0.0005f) - 0.5f) * 2;
                setIconColor(new Color(color.r, color.g, color.b, a));
            }

            if (hasDistanceTxt)
            {
                if (distance > 1200)
                {
                    distanceTxt.text = string.Format("{0}km", (distance * 0.001f).ToString("f1"));
                }
                else
                {
                    distanceTxt.text = string.Format("{0}m", distance.ToString("f0"));
                }
            }
        }

        void OnDestroy()
        {
            Destroy(iconGameObject);
        }
    }
}
