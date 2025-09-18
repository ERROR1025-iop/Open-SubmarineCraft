using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Searchlight : SolidBlock
    {

        bool isOpen;
        bool isCharge;
        protected float comsume;

        public Searchlight(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("searchlight", "other");
            isCanChangeRedAndCrackTexture = false;
            transmissivity = 4.2f;
            density = 1.85f;
            isOpen = false;
            isCharge = false;
            comsume = 0.5f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Searchlight block = new Searchlight(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1550);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.steel.getId(), 1},
                    { blocksManager.semiconductor.getId(), 6 },
                    { blocksManager.copper.getId(), 1 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            Block leftBlock = getNeighborBlock(Dir.left);
            int tag = Pooler.instance.getOutsideAreaTag(leftBlock.getCoor());
            if (leftBlock.equalPState(PState.solid) == false && tag > 1 && tag < 4)
            {
                Pooler.searchlightCount++;
            }
        }

        public override void onWorldModeDestroy()
        {
            Pooler.instance.decSearchlightCount();
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            checkStateRule(blocksEngine);
            comsumeElectricRule(blocksEngine);
        }

        protected void comsumeElectricRule(BlocksEngine blocksEngine)
        {
            if (isOpen)
            {
                float receive = Pooler.instance.requireElectric(this, comsume);
                if (receive > 0)
                {
                    isCharge = true;
                    MainSubmarine.lightLevel = Pooler.searchlightCount;
                }
                else
                {
                    isCharge = false;
                    MainSubmarine.lightLevel = 0;
                }
            }
        }

        protected virtual void checkStateRule(BlocksEngine blocksEngine)
        {
            isOpen = PoolerUI.instance.lightButton.value;
            if (isOpen && isCharge)
            {
                setSpriteRect(1);
            }
            else
            {
                setSpriteRect(0);
            }
        }     

    }
}
