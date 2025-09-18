using System.IO;
using UnityEngine;
using UnityEngine.UI;


namespace Scraft
{
    public class CustomDpartsSelector
    {
        public static CustomDpartsSelector instance;

        bool isShow;
        RectTransform mainTrans;

        int cardCount;
        public const int MAX_CARD_COUNT = 20;
        public static GameObject groupsSelectorParent;
        public CustomCard[] cardsArr;
        CustomCard activitedCard;
        CustomDrawer activitedDrawer;
        Text blockInformationText;
        Text cardText;
        int loadThumbnailImageCount;
        RectTransform addCardRectTrans;


        public CustomDpartsSelector()
        {
            instance = this;

            mainTrans = GameObject.Find("custom dparts selector").GetComponent<RectTransform>();
            cardText = GameObject.Find("Canvas/custom dparts selector/card name").GetComponent<Text>();
            blockInformationText = GameObject.Find("block inc").GetComponent<Text>();
            groupsSelectorParent = GameObject.Find("Groups Selector Parent");

            registerCards();
            drawCardDrawer();
            detectionRedundantFiles();
            drawAddCard();

            setCardActivited(0);
            setDrawerActivited(0);

            loadThumbnailImageCount = 0;

            if (AssemblerAddCustomBox.instance != null)
            {
                AssemblerAddCustomBox.instance.refreshDropDown();
            }
        }

        void detectionRedundantFiles()
        {
            for (int j = 0; j < cardCount; j++)
            {
                IUtils.detectionRedundantFiles(GamePath.customFolder + cardsArr[j].getCardName(), "*.ass", GamePath.customThumbnailFolder + cardsArr[j].getCardName());
            }
        }

        void registerCards()
        {
            cardCount = 0;
            cardsArr = new CustomCard[MAX_CARD_COUNT];

            for (int i = 0; i < MAX_CARD_COUNT; i++)
            {
                cardsArr[i] = null;
            }

            registerCard("default", "Assembler/Cards/dpartCube", 0, true);
            registerCard("submarine", "Assembler/Cards/submarineCard", 1, true);
            registerCard("ship", "Assembler/Cards/shipCard", 2, true);            
        }

        public bool registerNewCustomCard(string name, bool createFolder)
        {
            if (getCardByName(name) != null)
            {
                return false;
            }

            registerCard(name, "Assembler/Cards/dpartCube", cardCount, createFolder);
            addCardRectTrans.anchoredPosition3D = new Vector3(-25f, -51.3f - 35 * cardCount, 0);
            setCardActivited(cardCount - 1);

            if (AssemblerAddCustomBox.instance != null)
            {
                AssemblerAddCustomBox.instance.refreshDropDown();
            }

            return true;
        }

        void registerCard(string name, string imgRes, int rank, bool createFolder)
        {
            CustomCard card = new CustomCard(this, name, imgRes, rank);
            cardsArr[cardCount] = card;
            cardCount++;

            if (createFolder)
            {
                IUtils.createFolder(GamePath.customFolder + name);
                IUtils.createFolder(GamePath.customThumbnailFolder + name);
            }

            if (AssemblerAddCustomBox.instance != null)
            {
                AssemblerAddCustomBox.instance.addDropDownOption(ILang.get(name, "card"));
            }
        }

        void drawCardDrawer()
        {
            if (Directory.Exists(GamePath.shipsFolder))
            {
                DirectoryInfo direction = new DirectoryInfo(GamePath.customFolder);
                DirectoryInfo[] folders = direction.GetDirectories("*", SearchOption.TopDirectoryOnly);

                for (int i = 0; i < folders.Length; i++)
                {
                    bool isExitCard = false;
                    for (int j = 0; j < cardCount; j++)
                    {
                        if (cardsArr[j].getCardName().Equals(folders[i].Name))
                        {
                            isExitCard = true;
                            break;
                        }
                    }
                    if (isExitCard)
                    {
                        continue;
                    }
                    registerCard(folders[i].Name, "Assembler/Cards/dpartCube", cardCount, true);
                }
            }
        }

        void drawAddCard()
        {
            GameObject cardObject = Object.Instantiate(Resources.Load("Prefabs/Assembler/add card")) as GameObject;
            cardObject.GetComponent<Button>().onClick.AddListener(onAddCardClick);
            cardObject.transform.SetParent(mainTrans.transform, false);
            addCardRectTrans = cardObject.GetComponent<RectTransform>();
            addCardRectTrans.anchoredPosition3D = new Vector3(-25f, -51.3f - 35 * cardCount, 0);
        }

        void onAddCardClick()
        {
            AssemblerAddCustomCardBox.instance.show(true);
        }

        public void updata()
        {
            if (loadThumbnailImageCount < cardCount)
            {
                if (cardsArr[loadThumbnailImageCount].createThumbnailImage())
                {
                    loadThumbnailImageCount++;
                }
            }
        }

        public void onDrawerClick(string groupName, string fileName, int rank)
        {
            setDrawerActivited(rank);
        }

        void setDrawerActivited(int rank)
        {
            if (activitedDrawer != null)
            {
                activitedDrawer.setActivited(false);
            }
        }

        public void setCardActivited(int rank)
        {
            for (int i = 0; i < cardCount; i++)
            {
                if (i == rank)
                {
                    cardsArr[i].setActivited(true);
                    activitedCard = cardsArr[i];
                }
                else
                {
                    cardsArr[i].setActivited(false);
                }
            }

            cardText.text = ILang.get(cardsArr[rank].getCardName(), "card");
        }

        public void show(bool show)
        {
            isShow = show;
            if (show)
            {
                mainTrans.anchoredPosition = new Vector2(-74.9f, 0);
            }
            else
            {
                mainTrans.anchoredPosition = new Vector2(9999f, 0);
            }
        }

        public CustomCard getCardById(int id)
        {
            return cardsArr[id];
        }

        public CustomCard getCardByName(string name)
        {
            for (int i = 0; i < MAX_CARD_COUNT; i++)
            {
                CustomCard card = cardsArr[i];
                if (card != null && card.getCardName().Equals(name))
                {
                    return card;
                }
            }
            return null;
        }
    }
}
