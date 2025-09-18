using Scraft.DpartSpace;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class DpartDrawer
    {

        GameObject drawerObject;
        IScrollView cardIScrollView;
        DpartCardManager dpartCardManager;
        Image drawerImage;
        Image drawerIconImage;
        Dpart dpart;
        int rank;
        string drawername;
        Sprite activityDrawerSprite;
        Sprite unactivityDrawerSprite;
        bool isLoadThu;

        public DpartDrawer(DpartCardManager dpartCardManager, IScrollView cardIScrollView, Dpart dpart, int rank)
        {
            this.dpartCardManager = dpartCardManager;
            this.cardIScrollView = cardIScrollView;
            this.dpart = dpart;
            this.rank = rank;

            drawerObject = Object.Instantiate(Resources.Load("Prefabs/Assembler/dpart drawer")) as GameObject;
            cardIScrollView.addCell(drawerObject.transform);

            drawerObject.GetComponent<Button>().onClick.AddListener(onDrawerClick);

            drawerImage = drawerObject.GetComponent<Image>();

            drawerIconImage = drawerObject.transform.GetChild(0).gameObject.GetComponent<Image>();
            drawerIconImage.sprite = dpart.getIconSprite();
            isLoadThu = dpart.getIconSprite() != null;

            drawername = ILang.get(dpart.getName(), "dpart");

            activityDrawerSprite = Resources.Load("builder/drawer-selected", typeof(Sprite)) as Sprite;
            unactivityDrawerSprite = Resources.Load("builder/drawer-unselected", typeof(Sprite)) as Sprite;

            setActivited(false);
        }

        public void createThumbnailImage()
        {
            if (isLoadThu)
            {
                return;
            }

            if(dpart.isMod)
            {
                return;
            }

            GameObject go = Object.Instantiate(Resources.Load("Dparts/Prefabs/" + dpart.getName())) as GameObject;
            go.transform.SetParent(Assembler.preShotParent.transform);
            go.transform.localPosition = Vector3.zero;
            IUtils.changeLayerWithChildrens(go, 8);
            string savePath = Application.dataPath + "/Resources/Dparts/Icon/" + dpart.getName() + ".png";
            Texture2D texture2D = AssemblerUtils.createGameObjectThumbnailImage(go, Assembler.preShotCamera, savePath, new Rect(0, 0, 500, 400));
            IUtils.changeLayerWithChildrens(go, 9);
            drawerIconImage.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);

            Debug.Log("[CTI]Create thumbnail [" + dpart.getName() + "] successed!");
        }

        public Dpart getDpart()
        {
            return dpart;
        }

        void onDrawerClick()
        {
            dpartCardManager.onDrawerClick(dpart, rank);
        }

        public void setActivited(bool isActivited)
        {
            if (isActivited)
            {
                drawerImage.sprite = activityDrawerSprite;
            }
            else
            {
                drawerImage.sprite = unactivityDrawerSprite;
            }
        }
    }
}