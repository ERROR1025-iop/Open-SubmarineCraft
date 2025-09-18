using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scraft.BlockSpace;

namespace Scraft
{
    public class CardsRegister
    {
        static CardsRegister instance;
        BlocksManager blocksManager;
        int cardCount;
        public List<CardInfo> cardInfos;

        static public CardsRegister get()
        {
            if (instance == null)
            {
                instance = new CardsRegister();
            }
            return instance;
        }

        CardsRegister()
        {
            blocksManager = BlocksManager.instance;
            registerCards();
        }

        void registerCards()
        {
            cardCount = 0;
            cardInfos = new List<CardInfo>();            

            registerCard("structure", "Blocks/steel", 0);
            registerCard("material", "Blocks/water", 1);
            registerCard("machine", "Builder/Cards/machine", 2);
            registerCard("industry", "Builder/Cards/industry", 3);
            registerCard("engine", "Builder/Cards/engine", 4);
            registerCard("power", "Builder/Cards/power", 5);
            registerCard("signal", "Builder/Cards/electron", 6);
            registerCard("sensor", "Builder/Cards/sensor", 7);
            registerCard("logical", "Builder/Cards/logical", 8);
            registerCard("nuclear", "Builder/Cards/nuclear", 9);
            registerCard("weapon", "Builder/Cards/weapon", 10);
            registerCard("other", "Builder/Cards/other", 11);
        }

        void registerCard(string name, string imgRes, int rank)
        {
            CardInfo card = new CardInfo(name, imgRes, rank);
            cardInfos.Add(card);
            cardCount++;
        }

        public CardInfo getCardInfoByName(string name)
        {
            foreach(CardInfo info in cardInfos)
            {              
                if (info.name.Equals(name))
                {
                    return info;
                }
            }
            return null;
        }
    }

    public class CardInfo
    {
        public string name;
        public string imgRes;
        public int rank;
        public bool isUnlock;

        public CardInfo(string name, string imgRes, int rank)
        {
            this.name = name;
            this.imgRes = imgRes;
            this.rank = rank;
            isUnlock = false;
        }
    }
}
