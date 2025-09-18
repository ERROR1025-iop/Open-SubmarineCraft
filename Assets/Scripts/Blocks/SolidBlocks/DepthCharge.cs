using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class DepthCharge : SolidBlock
    {
        int shellCount;
        int startSprite;

        public DepthCharge(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("depthCharge", "weapon");

            thumbnailColor = new Color(0.4686f, 0.4686f, 0.4686f);
            transmissivity = 3.2f;
            density = 18.0f;
            shellCount = 3;
            isCanChangeRedAndCrackTexture = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            DepthCharge block = new DepthCharge(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);           
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.fineSteel.getId(), 1 },
                    { blocksManager.sulfurPowder.getId(), 3 },
                    { blocksManager.coalPowder.getId(), 3 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override bool giveOneDepthCharge(BlocksEngine blocksEngine, Block taker)
        {          
            if (shellCount > 0)
            {
                shellCount--;

                if (shellCount == 0)
                {
                    setSpriteRect(1);
                    setDensity(5);
                }
                return true;
            }
            return false;
        }    
    }
}
