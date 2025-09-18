using Scraft.BlockSpace;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class RadarPoint : MonoBehaviour
    {

        public GameObject pointObject;
        public bool hasDeepTxt;
        public bool isShowBoradArrow;

        Radar radar;
        RectTransform pointRectTrans;
        Text deepTxt;

        Rect lifeRect;
        Vector2 wp;
        Vector2 pp;

        void Start()
        {

            pointObject = Instantiate(pointObject);

            radar = Radar.instance;
            radar.addPoint(pointObject.transform);
            pointRectTrans = pointObject.GetComponent<RectTransform>();
            pointRectTrans.localScale = Vector3.one;

            if (hasDeepTxt)
            {
                deepTxt = pointObject.transform.GetChild(0).GetComponent<Text>();
            }

            lifeRect = new Rect(-80f, -80f, 160f, 160f);
        }

        public void SetActive(bool a)
        {
            pointObject.SetActive(a);
            enabled = a;
        }


        void LateUpdate()
        {
            Vector2 radarPosition = (IUtils.vector3DConvert2DPlant(transform.position) * 10 - MainSubmarine.instance.getLocation()) * 0.1f;
            pointRectTrans.anchoredPosition = radarPosition / radar.radarZoom;
            radarPointUpdata();

            if (hasDeepTxt)
            {
                updataDeepTxt();
            }
        }

        void updataDeepTxt()
        {
            float deep = -IUtils.coor3DConvert2D(transform.position).y;
            if (deep < 0)
            {
                deep = 0;
            }
            deepTxt.text = deep.ToString("f0");
        }

        /// <summary>
        ///雷达点位更新
        /// </summary>
        void radarPointUpdata()
        {
            pointRectTrans.localEulerAngles = new Vector3(0, 0, -MainSubmarine.getAngle());
            wp = radar.pointParentRectTrans.TransformPoint(pointRectTrans.anchoredPosition3D);
            pp = radar.radarRectTrans.InverseTransformPoint(wp);
            pointObject.SetActive(lifeRect.Contains(pp));

            if (isShowBoradArrow)
            {
                if (radar.isShowArr(Dir.up) == false)
                {
                    radar.showArr(Dir.up, isShowArr(Dir.up));
                }
                if (radar.isShowArr(Dir.down) == false)
                {
                    radar.showArr(Dir.down, isShowArr(Dir.down));
                }
                if (radar.isShowArr(Dir.left) == false)
                {
                    radar.showArr(Dir.left, isShowArr(Dir.left));
                }
                if (radar.isShowArr(Dir.right) == false)
                {
                    radar.showArr(Dir.right, isShowArr(Dir.right));
                }
            }
        }

        bool isShowArr(int dir)
        {
            if (dir == Dir.up && pp.y > 80)
            {
                return true;
            }
            else if (dir == Dir.down && pp.y < -80)
            {
                return true;
            }
            else if (dir == Dir.left && pp.x < -80)
            {
                return true;
            }
            else if (dir == Dir.right && pp.x > 80)
            {
                return true;
            }

            return false;
        }

        void OnDestroy()
        {
            Destroy(pointObject);
        }
    }
}