using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scraft.BlockSpace;

namespace Scraft.StationSpace
{
    public class SyntClassCell : IGridScrollViewCell
    {
        public SyntClass syntClass;

        Image icon;

        public SyntClassCell(IGridScrollViewInfo gridScrollViewInfo) : base(gridScrollViewInfo)
        {
            icon = rectTransform.GetChild(0).GetComponent<Image>();

            clearInformation();
        }

        public override void setInformation<T>(T info)
        {
            base.setInformation(info);
            syntClass = info as SyntClass;
            icon.sprite = syntClass.iconSprite;
        }        
    }

    public class SyntClass
    {
        public CardInfo cardInfo;
        public List<Block> blocks;
        public string name;

        public Sprite iconSprite;

        public SyntClass(CardInfo cardInfo)
        {
            this.cardInfo = cardInfo;
            name = cardInfo.name;
            iconSprite = Resources.Load(cardInfo.imgRes, typeof(Sprite)) as Sprite;
            blocks = new List<Block>();
        }

        public void addBlock(Block block)
        {
            blocks.Add(block);
        }

        static public SyntClass getSyntClassByName(List<SyntClass> syntClasses, string name)
        {
            for (int i = 0; i < syntClasses.Count; i++)
            {
                SyntClass syntClass = syntClasses[i];
                if (syntClass.name.Equals(name))
                {
                    return syntClass;
                }
            }
            return null;
        }
    }
}
