using Scraft.DpartSpace;
using UnityEngine;
using UnityEngine.UI;


namespace Scraft
{
    public class DpartCardManager
    {

        DpartsManager dpartsManager;
        DpartsEngine dpartsEngine;
        public const int MAX_CARD_COUNT = 20;
        public static Dpart selectDPartStatic;
        int cardCount;
        public DpartCard[] cardsArr;
        public GameObject cardName;
        DpartCard activitedCard;
        DpartDrawer activitedDrawer;
        Text blockInformationText;
        Text cardText;
        RectTransform dpartRectTrans;
        GameObject dpartSelector;


        public DpartCardManager(DpartsManager dpartsManager, DpartsEngine dpartsEngine)
        {
            this.dpartsManager = dpartsManager;
            this.dpartsEngine = dpartsEngine;

            cardName = GameObject.Find("Canvas/dparts selector/card name");
            blockInformationText = GameObject.Find("block inc").GetComponent<Text>();
            dpartSelector = GameObject.Find("dparts selector");
            dpartRectTrans = dpartSelector.GetComponent<RectTransform>();
            cardText = cardName.GetComponent<Text>();

            registerCards();
            drawCardDrawer();

            setCardActivited(0);
            setDrawerActivited(0);
            selectDPartStatic = null;

            if (!GameSetting.isAndroid)
            {
                createThumbnailImage();
            }
        }

        void registerCards()
        {
            cardCount = 0;
            cardsArr = new DpartCard[MAX_CARD_COUNT];

            for (int i = 0; i < MAX_CARD_COUNT; i++)
            {
                cardsArr[i] = null;
            }

            registerCard("geometry", "Assembler/Cards/dpartCube", 0);
            registerCard("light", "Assembler/Cards/light", 1);
            registerCard("transmission", "Assembler/Cards/coveredPropeller", 2);
            registerCard("submarine", "Assembler/Cards/submarineCard", 3);
            registerCard("ship", "Assembler/Cards/shipCard", 4);
            registerCard("function", "Assembler/Cards/dpartTurret", 5);            
            //registerCard("other", "Assembler/Cards/dpartShipBridgeSlope", 6);


        }

        void registerCard(string name, string imgRes, int rank)
        {
            DpartCard card = new DpartCard(dpartsManager, this, name, imgRes, rank);
            cardsArr[cardCount] = card;
            cardCount++;
        }

        void drawCardDrawer()
        {
            for (int i = 0; i < DpartsManager.MAX_DPARTS_ID; i++)
            {
                Dpart dpart = dpartsManager.getDPartById(i);
                if (dpart != null && dpart.getAttributeCardName() != "null")
                {
                    DpartCard card = getCardByName(dpart.getAttributeCardName());
                    if (card != null)
                    {
                        card.addDrawer(dpart);
                    }
                }
            }
        }

        public void onDrawerClick(Dpart dpartStatic, int rank)
        {
            setDrawerActivited(rank);

            if (AssemblerInput.mouseSelectUnplaceDPart != null)
            {
                dpartsEngine.deleteDpart(AssemblerInput.mouseSelectUnplaceDPart);
            }
        }

        public void setDrawerActivited(int rank)
        {
            if (activitedDrawer != null)
            {
                activitedDrawer.setActivited(false);
            }
            activitedDrawer = activitedCard.setDrawerActivity(rank, true);
            selectDPartStatic = activitedDrawer.getDpart();
            blockInformationText.text = selectDPartStatic.getBasicInformation();
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

        public void createThumbnailImage()
        {
            Debug.Log("[Warning]Create thumbnail image is open!");
            for (int i = 0; i < cardCount; i++)
            {
                cardsArr[i].createThumbnailImage();
            }
        }


        public DpartCard getCardByName(string name)
        {
            for (int i = 0; i < MAX_CARD_COUNT; i++)
            {
                DpartCard card = cardsArr[i];
                if (card != null && card.getCardName().Equals(name))
                {
                    return card;
                }
            }
            return null;
        }
    }
}