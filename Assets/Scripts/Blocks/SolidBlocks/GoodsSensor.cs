using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class GoodsSensor : RotationBlock
    {

        float nt;
        int settingPstate;

        public GoodsSensor(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("goodsSensor", "sensor");
            density = 5.4f;
            transmissivity = 2.85f;
            currentSettingValue = 0;
            settingPstate = currentSettingValue;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            GoodsSensor block = new GoodsSensor(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1550);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.circuitBoard.getId(), 2 },
                    { blocksManager.steel.getId(), 1 },
                    { blocksManager.semiconductor.getId(), 2 },
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
            settingPstate = currentSettingValue;
        }

        public override void onSettingValueChange()
        {
            base.onSettingValueChange();
            settingPstate = currentSettingValue;
        }

        void detectBlockRule(BlocksEngine blocksEngine)
        {

            Block detectBlock = getRelativeNeighborBlock(Dir.right);
            nt = LogicGate.StandardLowVoltage;
            switch (settingPstate)
            {
                case 0: nt = detectBlock.isAir() ? LogicGate.StandardHeightVoltage : nt; break;
                case 1: nt = detectBlock.equalPState(PState.solid) ? LogicGate.StandardHeightVoltage : nt; break;
                case 2: nt = (detectBlock.equalPState(PState.liquild) || detectBlock.equalPState(PState.mushy)) ? LogicGate.StandardHeightVoltage : nt; break;
                case 3: nt = (!detectBlock.isAir() && detectBlock.equalPState(PState.gas)) ? LogicGate.StandardHeightVoltage : nt; break;
                case 4: nt = detectBlock.equalPState(PState.particle) ? LogicGate.StandardHeightVoltage : nt; break;
                default: nt = LogicGate.StandardLowVoltage; break;
            }

            putWe(blocksEngine, Dir.up, nt);
            putWe(blocksEngine, Dir.right, nt);
            putWe(blocksEngine, Dir.down, nt);
            putWe(blocksEngine, Dir.left, nt);
        }

        private void putWe(BlocksEngine blocksEngine, int dir, float voltage)
        {
            if (getRelativeDir(Dir.right) != dir)
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
            return "detect pstate";
        }

        public override int[] getSettingValueRank()
        {
            return new int[2] { 0, 4 };
        }

        public override int isWeSystem()
        {
            return 1;
        }
    }
}
    
