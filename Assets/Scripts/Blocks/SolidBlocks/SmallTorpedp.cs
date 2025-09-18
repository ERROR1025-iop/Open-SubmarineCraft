using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace{ public class SmallTorpedp : Torpedp
    {
        int torpedpCount;
        int startSprite;

        public SmallTorpedp(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("smallTorpedp", "weapon");

            thumbnailColor = new Color(0.4686f, 0.4686f, 0.4686f);
            transmissivity = 3.2f;
            density = 18.0f;

            torpedpCount = 3;

        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SmallTorpedp block = new SmallTorpedp(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1430, 1320);
            block.initLargeBlock(new IPoint(5, 1));
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[4, 2] {
                    { blocksManager.fineSteel.getId(), 5 },                   
                    { blocksManager.sulfurPowder.getId(), 5 },
                    { blocksManager.coalPowder.getId(), 10 },
                    { blocksManager.semiconductor.getId(), 6 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 5);

            return syntInfos;
        }

        public override bool giveOneTorpedp(Block taker)
        {
            if (isOrigin() == false)
            {
                return false;
            }

            if (torpedpCount > 0)
            {
                torpedpCount--;

                switch (torpedpCount)
                {
                    case 0:
                        setTorpedpTexture(1);
                        break;
                    default:
                        break;
                }
                return true;
            }
            return false;
        }

        public override int getSyntIconSpriteIndex()
        {
            return 10;
        }
    }
}
