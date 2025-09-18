using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Generator : LargeFlipBlock
    {
        IPoint getMePoint;

        public Generator(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("generator", "power", true);

            thumbnailColor = new Color(0.0f, 0.3725f, 0.0196f);
            density = 7.1f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Generator block = new Generator(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1400);
            block.initLargeBlock(new IPoint(4, 3));
            block.initFlipLargeBlock(13);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.winding.getId(), 4 },
                    { blocksManager.steel.getId(), 12 },
                    { blocksManager.copper.getId(), 4 },
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 12);

            return syntInfos;
        }

        public override int getSyntIconSpriteIndex()
        {
            return 26;
        }

        protected override void onLargeBlockInitFinish()
        {
            getMePoint = new IPoint(isFlip ? 3 : 0, 1);
        }

        public override void onReciverMe(float me, int putterDir, Block putter)
        {
            me = Mathf.Abs(me);

            if (getOffset().equal(getMePoint))
            {
                Pooler.instance.chargeElectric(this, me * 0.01f * 0.9f);
            }

            Generator btnBlock = BlocksEngine.instance.getBlock(getReviseBlockCoor(new IPoint(0, 1))) as Generator;
            if (btnBlock != null)
            {
                if (me < 50)
                {
                    btnBlock.setSpriteRect(4);
                }
                else
                {
                    btnBlock.setSpriteRect(12);
                }
            }
        }

        public override bool isCanReceiveMe()
        {
            return true;
        }
    }
}
