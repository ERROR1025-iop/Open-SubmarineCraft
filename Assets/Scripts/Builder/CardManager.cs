using UnityEngine;
using UnityEngine.UI;
using Scraft.BlockSpace;
using System.Collections.Generic;

namespace Scraft
{
    public class CardManager
    {
        static public CardManager instance;
        BlocksManager blocksManager;
        public const int MAX_CARD_COUNT = 20;
        public static Block selectBlockStatic;
        int cardCount;
        CardsRegister cardsRegister;
        List<CardInfo> cardInfos;
        public Card[] cardsArr;
        public Drawer[] drawerArr;
        public GameObject cardName;
        Card activitedCard;
        Drawer activitedDrawer;
        Text blockInformationText;
        Text cargoCountText;

        public CardManager(BlocksManager blocksManager)
        {
            instance = this;
            this.blocksManager = blocksManager;
            cardsRegister = CardsRegister.get();

            cardName = GameObject.Find("card name");
            blockInformationText = GameObject.Find("block inc").GetComponent<Text>();
            cargoCountText = GameObject.Find("cargo count").GetComponent<Text>();

            registerCards();
            drawCardDrawer();
            resortCards();

            setCardActivited(0);
            setDrawerActivited(0);

        }

        void registerCards()
        {
            cardCount = 0;
            cardsArr = new Card[MAX_CARD_COUNT];

            for (int i = 0; i < MAX_CARD_COUNT; i++)
            {
                cardsArr[i] = null;
            }

            cardInfos = cardsRegister.cardInfos;
            foreach (CardInfo cardInfo in cardInfos)
            {
                registerCard(cardInfo.name, cardInfo.imgRes, cardInfo.rank);
            }
        }

        void registerCard(string name, string imgRes, int rank)
        {
            Card card = new Card(blocksManager, this, name, imgRes, rank);
            cardsArr[cardCount] = card;
            cardCount++;
        }

        void drawCardDrawer()
        {
            int blocksCount = blocksManager.getBlockCount();
            drawerArr = new Drawer[blocksCount];
            for (int i = 0; i < blocksCount; i++)
            {
                Block block = blocksManager.getBlockById(i);
                if (block != null && block.getAttributeCardName() != "null")
                {
                    if (!GameSetting.isCareer || blocksManager.getIsUnlock(i))
                    {
                        Card card = getCardByName(block.getAttributeCardName());
                        if (card != null)
                        {
                            drawerArr[i] = card.addDrawer(block);
                        }
                    }
                }
            }
        }

        void resortCards()
        {
            if (GameSetting.isCareer)
            {
                int index = 0;
                for (int i = 0; i < MAX_CARD_COUNT; i++)
                {
                    Card card = cardsArr[i];
                    if (card != null)
                    {
                        if (card.getDrawerCount() == 0)
                        {
                            card.setEnable(false);
                            cardInfos[i].isUnlock = true;
                        }
                        else
                        {
                            card.setIndex(index++);
                        }
                    }
                }
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

            Text cardText = cardName.GetComponent<Text>();
            cardText.text = ILang.get(cardsArr[rank].getCardName(), "card");
        }

        public void onDrawerClick(Block block, int rank)
        {
            setDrawerActivited(rank);
        }

        void setDrawerActivited(int rank)
        {
            if (activitedDrawer != null)
            {
                activitedDrawer.setActivited(false);
            }
            activitedDrawer = activitedCard.setDrawerActivity(rank, true);
            selectBlockStatic = activitedDrawer.getBlock();
            blockInformationText.text = selectBlockStatic.getBasicInformation();
            updatecargoCount();
            Builder.changeMode(0, selectBlockStatic.getLangName());
        }

        public void updatecargoCount()
        {
            int cargoCount = blocksManager.getMainStationCargoCount(selectBlockStatic);
            cargoCountText.color = cargoCount > 0 ? Color.green : Color.red;
            cargoCountText.text = string.Format("{0}:{1}", ILang.get("Count"), cargoCount == int.MaxValue ? ILang.get("Unlimit") : cargoCount.ToString());
        }

        public Block getActivitedBlock()
        {
            return activitedDrawer.getBlock();
        }

        public Drawer getDrawerByBlockId(int blockId)
        {
            return drawerArr[blockId];
        }

        public void setDrawerActivited(Drawer drawer, bool isNeedChangeMode)
        {
            if (activitedDrawer != null)
            {
                activitedDrawer.setActivited(false);
            }
            activitedDrawer = drawer;
            setCardActivited(drawer.getCard().getRank());
            selectBlockStatic = activitedDrawer.getBlock();
            blockInformationText.text = selectBlockStatic.getBasicInformation();
            updatecargoCount();
            if (isNeedChangeMode)
            {
                Builder.changeMode(0, selectBlockStatic.getLangName());
            }
        }

        public Card getCardByName(string name)
        {
            for (int i = 0; i < MAX_CARD_COUNT; i++)
            {
                Card card = cardsArr[i];
                if (card != null && card.getCardName().Equals(name))
                {
                    return card;
                }
            }
            return null;
        }

    }
}