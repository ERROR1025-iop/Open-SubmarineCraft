using UnityEngine;
using UnityEngine.UI;
using Scraft.BlockSpace;

namespace Scraft
{
    public class Card
    {
        private BlocksManager blocksManager;
        private CardManager cardManager;
        private string name;
        private string imgRes;
        private int rank;

        private GameObject cardObject;
        private GameObject itemObject;
        private GameObject blockSelector;

        private Image cardImage;
        private Sprite activityCardSprite;
        private Sprite unactivityCardSprite;

        GameObject cardBoxObject;
        IScrollView cardIScrollView;
        public const int MAX_DRAWER_COUNT = 50;

        Drawer[] drawerArr;
        int drawerStack;

        public Card(BlocksManager blocksManager, CardManager cardManager, string name, string imgRes, int rank)
        {
            this.blocksManager = blocksManager;
            this.name = name;
            this.imgRes = imgRes;
            this.rank = rank;
            this.cardManager = cardManager;

            blockSelector = GameObject.Find("block selector");

            drawerArr = new Drawer[MAX_DRAWER_COUNT];

            drawerStack = 0;

            drawCard();
            drawCardBox();

        }

        public void setIndex(int index)
        {
            cardObject.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(-25f, -51.3f - 35 * index, 0);
        }

        public void setEnable(bool enable)
        {
            cardObject.SetActive(enable);
        }

        void drawCard()
        {
            cardObject = Object.Instantiate(Resources.Load("Prefabs/Builder/card")) as GameObject;
            cardObject.transform.SetParent(blockSelector.transform);
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
            cardManager.setCardActivited(rank);
        }

        void drawCardBox()
        {
            cardBoxObject = Object.Instantiate(Resources.Load("Prefabs/Builder/lab card scroll View")) as GameObject;
            cardBoxObject.transform.SetParent(blockSelector.transform);
            RectTransform rt = cardBoxObject.GetComponent<RectTransform>();
            rt.offsetMax = new Vector2(rt.offsetMax.x, -33.745f);
            rt.offsetMin = new Vector2(rt.offsetMin.x, 5.15f);
            rt.anchoredPosition = new Vector2(-93f, rt.anchoredPosition.y);
            cardIScrollView = cardBoxObject.GetComponent<IScrollView>();
            cardIScrollView.setPerCellHeight(43);
        }

        public Drawer addDrawer(Block block)
        {
            Drawer drawer = new Drawer(cardManager, this, cardIScrollView, block, drawerStack);
            drawerArr[drawerStack] = drawer;
            drawerStack++;
            return drawer;
        }

        public int getDrawerCount()
        {
            return drawerStack;
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

        public Drawer setDrawerActivity(int rank, bool isActivity)
        {
            Drawer drawer = drawerArr[rank];
            drawer.setActivited(isActivity);
            return drawer;
        }

        public string getCardName()
        {
            return name;
        }

        public int getRank()
        {
            return rank;
        }

        void changeImageRes(GameObject gameOject, string resPath)
        {
            Image image = gameOject.GetComponent<Image>();
            image.sprite = Resources.Load(resPath, typeof(Sprite)) as Sprite;
        }
    }
}