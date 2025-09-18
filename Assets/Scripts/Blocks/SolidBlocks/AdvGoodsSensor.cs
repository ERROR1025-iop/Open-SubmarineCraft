using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class AdvGoodsSensor : RotationBlock
    {

        float nt;
        int targetDir;

        public AdvGoodsSensor(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("advGoodsSensor", "sensor");
            density = 5.4f;
            transmissivity = 2.85f;
            currentSettingValue = 0;            
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            AdvGoodsSensor block = new AdvGoodsSensor(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1550);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.goodsSensor.getId(), 2 },
                    { blocksManager.circuitBoard.getId(), 2 },
                    { blocksManager.steel.getId(), 1 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            detectBlockRule(blocksEngine);
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            setTargetDir(currentSettingValue + 1);
        }

        public override void onBuilderModeCreated()
        {
            base.onBuilderModeCreated();
            setTargetDir(currentSettingValue + 1);
        }

        public override void onSettingValueChange()
        {
            base.onSettingValueChange();            
            setTargetDir(currentSettingValue + 1);
        }

        void setTargetDir(int dir)
        {
            targetDir = dir;
            setSpriteRect(targetDir - 1);
        }

        void detectBlockRule(BlocksEngine blocksEngine)
        {
            nt = LogicGate.StandardLowVoltage;
            Block detectBlock = getRelativeNeighborBlock(Dir.up);
            if (detectBlock.equalBlock(getRelativeNeighborBlock(targetDir)))
            {
                nt = LogicGate.StandardHeightVoltage;
            }

            putWe(blocksEngine, Dir.up, nt);
            putWe(blocksEngine, Dir.right, nt);
            putWe(blocksEngine, Dir.down, nt);
            putWe(blocksEngine, Dir.left, nt);
        }

        private void putWe(BlocksEngine blocksEngine, int dir, float voltage)
        {
            if (getRelativeDir(Dir.up) != dir)
                blocksEngine.putWe(this, getCoor().getDirPoint(dir), voltage);
        }

        public override float getRomaoteMe()
        {
            return nt;
        }

        public override int isCanSettingValue()
        {
            return 0;
        }

        public override string getSettingValueName()
        {
            return "direction";
        }

        public override int[] getSettingValueRank()
        {
            return new int[2] { 0, 2 };
        }

        public override int isWeSystem()
        {
            return 1;
        }
    }
}

