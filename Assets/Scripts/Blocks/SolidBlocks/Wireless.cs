using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace{ public class Wireless : SolidBlock
    {

        public Wireless(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("wireless", "other");

            transmissivity = 4.2f;
            density = 1.85f;
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            Pooler.wirelessCount++;
        }

        public override void onWorldModeDestroy()
        {
            Pooler.instance.decWirelessCount();
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Wireless block = new Wireless(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1550);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[2, 2] {
                    { blocksManager.steel.getId(), 1 },
                    { blocksManager.circuitBoard.getId(), 1 },

            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        protected override bool solidBreakRule(BlocksEngine blocksEngine)
        {
            if (press > maxPress)
            {
                blocksEngine.removeBlock(getCoor());
                return true;
            }

            Block up_block = getNeighborBlock(Dir.up);
            Block right_block = getNeighborBlock(Dir.right);
            Block down_block = getNeighborBlock(Dir.down);
            Block left_block = getNeighborBlock(Dir.left);

            if (up_block.equalPState(PState.liquild) && right_block.equalPState(PState.liquild) && down_block.equalPState(PState.liquild) && left_block.equalPState(PState.liquild))
            {
                float mp = Mathf.Max(up_block.getPress(), right_block.getPress(), down_block.getPress(), left_block.getPress());
                if (mp > maxPress)
                {
                    blocksEngine.removeBlock(getCoor());
                    return true;
                }
            }
            return false;
        }

        public override bool isRootUnlock()
        {
            return true;
        }

        public override bool isCanCollectScientific()
        {
            return true;
        }

        public override void onCollectScientific(CollectedScientificInfo csInfo)
        {
            base.onCollectScientific(csInfo);
            collectedScientific = csInfo.point;
        }
    }
}
