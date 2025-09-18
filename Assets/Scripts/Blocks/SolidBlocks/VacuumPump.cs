using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class VacuumPump : AdvPump
    { 

        public VacuumPump(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("vacuumPump", "machine");

            thumbnailColor = new Color(0.894f, 0.450f, 0.313f);
            density = 11.1f;
            storageWater = null;
            storageAirPress = 0;
            comsume = 5.0f;            
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            VacuumPump block = new VacuumPump(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            block.initRotationBlock();
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[2, 2] {
                    { blocksManager.advPump.getId(), 1 },
                    { blocksManager.steel.getId(), 1 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        protected override void releaseAirMethod(Block outBlock)
        {
            if (storageAirPress > 0)
            {
                if (outBlock.isAir() || outBlock.isFluid())
                {
                    outBlock.addPress(storageAirPress);
                }
                else if (outBlock.equalBlock(this))
                {
                    (outBlock as AdvPump).pushInAirPress(storageAirPress);
                }
            }
        }

        protected override void inputAirMethod(Block inBlock, Block outBlock)
        {
            float op = inBlock.getPress();
            float p = op - 10;
            if (p < 0)
            {
                p = 0;
            }
            inBlock.setPress(p);
            (outBlock as AdvPump).pushInAirPress(op - p);
        }

        protected override void outputAirMethod(Block inBlock, Block outBlock)
        {
            float op = inBlock.getPress();
            float p = op - 10;
            if (p < 0)
            {
                p = 0;
            }
            inBlock.setPress(p);
            outBlock.addPress(op - p);
        }
    }
}