using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft.BlockSpace
{
    public class SmallPropeller : Propeller
    {
        public SmallPropeller(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("smallPropeller", "machine");

            thumbnailColor = new Color(0f, 0f, 0f, 0f);
            transmissivity = 7.2f;
            density = 3.5f;
            updataFrameStack = 0;
            updataFrameDelayPerUnit = 30000;
            updataFrameDelayStack = 0;
            getMe = 0;
            outPutDir = 1;
            canUse = false;
            receivePoint = new IPoint(0, 1);
            max_storeAir = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SmallPropeller block = new SmallPropeller(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            block.initLargeBlock(new IPoint(3, 3));
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[1, 2] {
                    { blocksManager.steel.getId(), 9 }

            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 9);

            return syntInfos;
        }

        public override int getSyntIconSpriteIndex()
        {
            return 36;
        }

        public override float getForce()
        {
            if (getIsCanUse())
            {
                return speed * 0.0050f;
            }
            else
            {
                return 0;
            }
        }

        public override Color getThumbnailColor(JsonData blockData)
        {
            if (!m_isBroken)
            {
                if (m_offset.x == 1 || m_offset.y == 1)
                {
                    return new Color(0.4286f, 0.4286f, 0.4286f);
                }
            }

            return base.getThumbnailColor(blockData);
        }

        public override bool isRootUnlock()
        {
            return true;
        }
    }
}
