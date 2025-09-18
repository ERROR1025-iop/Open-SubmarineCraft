using UnityEngine;
using UnityEngine.UI;


namespace Scraft
{
    public class ShellTargetPlane : MonoBehaviour
    {
        static public ShellTargetPlane instance;

        public Text distanceTxt;
        public float distanceTxtHeight;

        Camera Camera3D;

        void Start()
        {
            instance = this;
            Camera3D = Camera.main;
        }

        static public void show(bool s)
        {
           
            if (!s)
            {
                instance.gameObject.transform.position = new Vector3(0, 9999, 0);
                instance.distanceTxt.transform.position = new Vector3(0, 9999, 0);
            }                  
        }

        static public void setPosition(Vector3 position)
        {
            instance.mSetPosition(position);
        }

        public void mSetPosition(Vector3 position)
        {
            transform.position = IUtils.vectorRoundInPlane(position, 0.01f);

            bool isFornt = Vector3.Dot(Camera3D.transform.forward, transform.position - Camera3D.transform.position) > 0;
            distanceTxt.gameObject.SetActive(isFornt);
            if (isFornt)
            {
                float distance = Vector3.Distance(transform.position, MainSubmarine.transform.position) * 10;
                distanceTxt.transform.position = Camera3D.WorldToScreenPoint(transform.position + new Vector3(0, distanceTxtHeight, 0));
                if (distance > 1200)
                {
                    distanceTxt.text = string.Format("[{0}km]", (distance * 0.001f).ToString("f1"));
                }
                else
                {
                    distanceTxt.text = string.Format("[{0}m]", distance.ToString("f0"));
                }
            }            
        }
    }
}
