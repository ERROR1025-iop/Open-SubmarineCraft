using Scraft.DpartSpace;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class DpartCard
    {

        private DpartsManager dpartsManager;
        private DpartCardManager dpartCardManager;
        private string name;
        private string imgRes;
        private int rank;


        private GameObject cardObject;
        private GameObject itemObject;
        private GameObject dpartSelector;

        private Image cardImage;
        private Sprite activityCardSprite;
        private Sprite unactivityCardSprite;

        GameObject cardBoxObject;
        IScrollView cardIScrollView;
        public const int MAX_DRAWER_COUNT = 50;

        DpartDrawer[] drawerArr;
        int drawerStack;


        public DpartCard(DpartsManager dpartsManager, DpartCardManager dpartCardManager, string name, string imgRes, int rank)
        {
            this.dpartsManager = dpartsManager;
            this.name = name;
            this.imgRes = imgRes;
            this.rank = rank;
            this.dpartCardManager = dpartCardManager;

            dpartSelector = GameObject.Find("dparts selector");

            drawerArr = new DpartDrawer[MAX_DRAWER_COUNT];

            drawerStack = 0;

            drawCard();
            drawCardBox();



        }

        void drawCard()
        {
            cardObject = Object.Instantiate(Resources.Load("Prefabs/Builder/card")) as GameObject;
            cardObject.transform.SetParent(dpartSelector.transform, false);
            cardObject.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(-25f, -51.3f - 35 * rank, 0);

            cardObject.GetComponent<Button>().onClick.AddListener(onCardClick);


            itemObject = cardObject.transform.GetChild(0).gameObject;

            Image itemImage = itemObject.GetComponent<Image>();
            itemImage.sprite = Resources.Load(imgRes, typeof(Sprite)) as Sprite;


            cardImage = cardObject.GetComponent<Image>();
            activityCardSprite = Resources.Load("builder/card-selected", typeof(Sprite)) as Sprite;
            unactivityCardSprite = Resources.Load("builder/card-unselected", typeof(Sprite)) as Sprite;
        }

        public void onCardClick()
        {
            dpartCardManager.setCardActivited(rank);
        }

        void drawCardBox()
        {
            cardBoxObject = Object.Instantiate(Resources.Load("Prefabs/Assembler/ass card scroll View")) as GameObject;
            cardBoxObject.transform.SetParent(dpartSelector.transform, false);
            RectTransform rt = cardBoxObject.GetComponent<RectTransform>();
            rt.offsetMax = new Vector2(rt.offsetMax.x, -33.745f);
            rt.offsetMin = new Vector2(rt.offsetMin.x, 60f);
            rt.anchoredPosition = new Vector2(-93f, rt.anchoredPosition.y);
            cardIScrollView = cardBoxObject.GetComponent<IScrollView>();
            cardIScrollView.setPerCellHeight(43);
        }

        public void addDrawer(Dpart dpart)
        {
            DpartDrawer drawer = new DpartDrawer(dpartCardManager, cardIScrollView, dpart, drawerStack);
            drawerArr[drawerStack] = drawer;
            drawerStack++;
        }

        public void setActivited(bool isActivited)
        {
            if (isActivited)
            {
                cardImage.sprite = activityCardSprite;
            }
            else
            {
                cardImage.sprite = unactivityCardSprite;
            }

            cardBoxObject.SetActive(isActivited);
        }

        public DpartDrawer setDrawerActivity(int rank, bool isActivity)
        {
            DpartDrawer drawer = drawerArr[rank];
            drawer.setActivited(isActivity);
            return drawer;
        }

        public void createThumbnailImage()
        {
            for (int i = 0; i < drawerStack; i++)
            {
                drawerArr[i].createThumbnailImage();
            }
        }

        public string getCardName()
        {
            return name;
        }

        void changeImageRes(GameObject gameOject, string resPath)
        {
            Image image = gameOject.GetComponent<Image>();
            image.sprite = Resources.Load(resPath, typeof(Sprite)) as Sprite;
        }
    }
}