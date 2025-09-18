using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace{ public class PressGenerator : RotationBlock
    {

        protected float generationCoefficient;
        protected float minPress;

        public PressGenerator(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("pressGenerator", "null");
            density = 3.2f;

            generationCoefficient = 0.01f;
            minPress = 500;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            PressGenerator block = new PressGenerator(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "copperLiquid", 1357, 1750);
            block.initRotationBlock();
            return block;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            //pressGeneratorRule(blocksEngine); 废弃的
        }

        protected virtual void pressGeneratorRule(BlocksEngine blocksEngine)
        {
            Block up_block = getNeighborBlock(Dir.up);
            Block right_block = getNeighborBlock(Dir.right);
            Block down_block = getNeighborBlock(Dir.down);
            Block left_block = getNeighborBlock(Dir.left);

            float p1 = 0, p2 = 0;
            if (!up_block.equalPState(PState.solid) && !down_block.equalPState(PState.solid))
            {
                p1 = up_block.getPress() - down_block.getPress();
            }

            if (!left_block.equalPState(PState.solid) && !right_block.equalPState(PState.solid))
            {
                p2 = left_block.getPress() - right_block.getPress();
            }

            if (Mathf.Abs(p1) > Mathf.Abs(p2))
            {
                if (p1 > minPress)
                {
                    moveToByDir(Dir.down);
                    Pooler.instance.chargeElectric(this, p1 * generationCoefficient);
                }
                else if (p1 < -minPress)
                {
                    moveToByDir(Dir.up);
                    Pooler.instance.chargeElectric(this, -p1 * generationCoefficient);
                }
            }
            else
            {
                if (p2 > minPress)
                {
                    moveToByDir(Dir.right);
                    Pooler.instance.chargeElectric(this, p2 * generationCoefficient);
                }
                else if (p2 < -minPress)
                {
                    moveToByDir(Dir.left);
                    Pooler.instance.chargeElectric(this, -p2 * generationCoefficient);
                }
            }
        }
    }
}
