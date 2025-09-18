using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft.BlockSpace{ public class Torpedp : LargeBlock
    {

        int torpedpCount;
        int startSprite;

        public Torpedp(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("torpedp", "weapon");

            thumbnailColor = new Color(0.4686f, 0.4686f, 0.4686f);
            transmissivity = 3.2f;
            density = 18.0f;

            torpedpCount = 9;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Torpedp block = new Torpedp(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1430, 1320);
            block.initLargeBlock(new IPoint(5, 3));
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[4, 2] {
                    { blocksManager.fineSteel.getId(), 15 },
                    { blocksManager.sulfurPowder.getId(), 15 },
                    { blocksManager.coalPowder.getId(), 30 },
                    { blocksManager.semiconductor.getId(), 18 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 15);

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
                    case 6:
                        setTorpedpTexture(1);
                        break;
                    case 3:
                        setTorpedpTexture(2);
                        break;
                    case 0:
                        setTorpedpTexture(3);
                        break;
                    default:
                        break;
                }
                return true;
            }
            return false;
        }

        protected override void onLargeBlockInitFinish()
        {
            IPoint startOffset = getOffset();
            startSprite = ((size.y - 1) - startOffset.y) * size.x + startOffset.x;
        }

        protected void setTorpedpTexture(int count)
        {
            for (int offsetx = 0; offsetx < size.x; offsetx++)
            {
                for (int offsety = 0; offsety < size.y; offsety++)
                {
                    IPoint obOffset = new IPoint(offsetx, offsety);
                    Torpedp offsetBlock = BlocksEngine.instance.getBlock(getOriginPoint() + obOffset) as Torpedp;
                    if (offsetBlock == null)
                    {
                        continue;
                    }

                    offsetBlock.setSpriteRect(offsetBlock.startSprite + count * blockCount);
                    offsetBlock.setDensity(density - 4.0f);
                }
            }
        }

        public override int getSyntIconSpriteIndex()
        {
            return 60;
        }

        public override Color getThumbnailColor(JsonData blockData)
        {
            if (IUtils.getJsonValue2Bool(blockData, "ib") == false)
            {
                IPoint toffset = new IPoint(IUtils.getJsonValue2Int(blockData, "ox"), IUtils.getJsonValue2Int(blockData, "oy"));
                if (toffset.x == 0)
                {
                    return new Color(1.0f, 0.9490f, 0.0f);
                }
            }

            return base.getThumbnailColor(blockData);
        }
    }
}
