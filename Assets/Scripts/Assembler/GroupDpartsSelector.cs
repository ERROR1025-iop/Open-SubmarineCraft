using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Scraft.DpartSpace;

namespace Scraft
{
    public class GroupDpartsSelector
    {
        bool isShow;
        RectTransform mainTrans;

        int cardCount;
        public const int MAX_CARD_COUNT = 20;
        public static GroupDpart selectDpart;
        public static GameObject groupsSelectorParent;
        public GroupCard[] cardsArr;
        GroupCard activitedCard;
        GroupDrawer activitedDrawer;
        Text blockInformationText;
        Text cardText;
        int loadThumbnailImageCount;


        public GroupDpartsSelector()
        {
            mainTrans = GameObject.Find("group dparts selector").GetComponent<RectTransform>();
            cardText = GameObject.Find("Canvas/group dparts selector/card name").GetComponent<Text>();
            blockInformationText = GameObject.Find("block inc").GetComponent<Text>();
            groupsSelectorParent = GameObject.Find("Groups Selector Parent");

            registerCards();
            drawCardDrawer();
            detectionRedundantFiles();

            setCardActivited(0);
            setDrawerActivited(0);

            loadThumbnailImageCount = 0;
        }

        void registerCards()
        {
            cardCount = 0;
            cardsArr = new GroupCard[MAX_CARD_COUNT];

            for (int i = 0; i < MAX_CARD_COUNT; i++)
            {
                cardsArr[i] = null;
            }

            registerCard("other", "Assembler/Cards/dpartCube", 0, true);
            registerCard("submarine", "Assembler/Cards/submarineCard", 1, true);
            registerCard("ship", "Assembler/Cards/shipCard", 2, true);           
            registerCard("prefab", "Assembler/Cards/prefab", 3, true);
        }

        void registerCard(string name, string imgRes, int rank, bool createFolder)
        {
            GroupCard card = new GroupCard(this, name, imgRes, rank);
            cardsArr[cardCount] = card;
            cardCount++;

            if (createFolder)
            {
                IUtils.createFolder(GamePath.groupsFolder + name);
                IUtils.createFolder(GamePath.groupsThumbnailFolder + name);
            }
        }

        void detectionRedundantFiles()
        {
            for (int j = 0; j < cardCount; j++)
            {
                IUtils.detectionRedundantFiles(GamePath.groupsFolder + cardsArr[j].getCardName(), "*.ass", GamePath.groupsThumbnailFolder + cardsArr[j].getCardName());
            }
        }

        void drawCardDrawer()
        {
            if (Directory.Exists(GamePath.shipsFolder))
            {
                DirectoryInfo direction = new DirectoryInfo(GamePath.groupsFolder);
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
                    registerCard(folders[i].Name, "dparts/icon/dpartCube", cardCount, true);
                }
            }
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
    }
}