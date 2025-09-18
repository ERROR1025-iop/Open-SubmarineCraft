using Scraft.BlockSpace;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class Drawer
    {

        GameObject drawerObject;
        IScrollView cardIScrollView;
        CardManager cardManager;
        Card card;
        Image drawerImage;
        Block block;
        int rank;
        Sprite activityDrawerSprite;
        Sprite unactivityDrawerSprite;

        public Drawer(CardManager cardManager, Card card, IScrollView cardIScrollView, Block block, int rank)
        {
            this.cardManager = cardManager;
            this.card = card;
            this.cardIScrollView = cardIScrollView;
            this.block = block;
            this.rank = rank;

            drawerObject = Object.Instantiate(Resources.Load("Prefabs/Builder/drawer")) as GameObject;
            cardIScrollView.addCell(drawerObject.transform);

            drawerObject.GetComponent<Button>().onClick.AddListener(onDrawerClick);

            drawerImage = drawerObject.GetComponent<Image>();

            GameObject drawerItemObject = drawerObject.transform.GetChild(0).gameObject;
            drawerItemObject.GetComponent<Image>().sprite = block.getSyntIconSprite();

            GameObject drawerNameObject = drawerObject.transform.GetChild(1).gameObject;
            Text drawername = drawerNameObject.GetComponent<Text>();
            drawername.text = block.getLangName();

            //GameObject drawerPriceObject = drawerObject.transform.GetChild(2).gameObject;
            //Text drawerPrice = drawerPriceObject.GetComponent<Text>();
            //drawerPrice.text = "$" + block.getTotalPrice().ToString();

            activityDrawerSprite = Resources.Load("builder/drawer-selected", typeof(Sprite)) as Sprite;
            unactivityDrawerSprite = Resources.Load("builder/drawer-unselected", typeof(Sprite)) as Sprite;

            setActivited(false);
        }

        void onDrawerClick()
        {
            cardManager.onDrawerClick(block, rank);
        }

        public Card getCard()
        {
            return card;
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

        public Block getBlock()
        {
            return block;
        }

        void changeImageRes(GameObject gameOject, string resPath)
        {
            Image image = gameOject.GetComponent<Image>();
            image.sprite = Resources.Load(resPath, typeof(Sprite)) as Sprite;
        }
    }
}