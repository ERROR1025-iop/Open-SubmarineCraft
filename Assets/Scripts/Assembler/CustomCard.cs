using System.IO;
using UnityEngine;
using UnityEngine.UI;


namespace Scraft
{
    public class CustomCard
    {
        private CustomDpartsSelector customDpartsSelector;
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

        CustomDrawer[] drawerArr;
        int drawerStack;
        int loadThumbnailImageCount;
        int filesCount;


        public string cardFolder;

        public CustomCard(CustomDpartsSelector customDpartsSelector, string name, string imgRes, int rank)
        {
            this.customDpartsSelector = customDpartsSelector;
            this.name = name;
            this.imgRes = imgRes;
            this.rank = rank;

            dpartSelector = GameObject.Find("custom dparts selector");
            drawerArr = new CustomDrawer[MAX_DRAWER_COUNT];
            drawerStack = 0;
            cardFolder = GamePath.customFolder + name;

            drawCard();
            drawCardBox();
            drawDrawer();
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

        void drawDrawer()
        {
            if (Directory.Exists(cardFolder))
            {
                DirectoryInfo direction = new DirectoryInfo(cardFolder);
                FileInfo[] folders = direction.GetFiles("*.ass", SearchOption.TopDirectoryOnly);

                filesCount = folders.Length;
                drawerStack = 0;
                for (int i = 0; i < filesCount; i++)
                {
                    addDrawer(folders[i]);
                }
            }
        }

        public bool createThumbnailImage()
        {
            if (loadThumbnailImageCount < filesCount)
            {
                CustomDrawer loadDrawer = drawerArr[loadThumbnailImageCount];
                if (loadDrawer != null)
                {
                    loadDrawer.createThumbnailImage();
                }
                loadThumbnailImageCount++;
                return false;
            }
            return true;
        }

        public void onCardClick()
        {
            customDpartsSelector.setCardActivited(rank);
        }

        void drawCardBox()
        {
            cardBoxObject = Object.Instantiate(Resources.Load("Prefabs/Assembler/ass custom card scroll View")) as GameObject;
            cardBoxObject.transform.SetParent(dpartSelector.transform, false);
            RectTransform rt = cardBoxObject.GetComponent<RectTransform>();
            rt.offsetMax = new Vector2(rt.offsetMax.x, -33.745f);
            rt.offsetMin = new Vector2(rt.offsetMin.x, 60f);
            rt.anchoredPosition = new Vector2(-93f, rt.anchoredPosition.y);
            cardIScrollView = cardBoxObject.GetComponent<IScrollView>();
            cardIScrollView.setPerCellHeight(43);
        }

        public void addDrawer(FileInfo folder)
        {
            CustomDrawer drawer = new CustomDrawer(customDpartsSelector, name, cardIScrollView, folder, drawerStack);
            drawerArr[drawerStack] = drawer;
            drawerStack++;
        }

        public void refreshDrawer()
        {
            GroupDpartsSelector.selectDpart = null;
            for (int i = 0; i < MAX_DRAWER_COUNT; i++)
            {
                CustomDrawer drawer = drawerArr[i];
                if (drawer != null)
                {
                    drawer.clear();
                    drawerArr[i] = null;
                }
            }
            drawDrawer();
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

        public CustomDrawer setDrawerActivity(int rank, bool isActivity)
        {
            CustomDrawer drawer = drawerArr[rank];
            drawer.setActivited(isActivity);
            return drawer;
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