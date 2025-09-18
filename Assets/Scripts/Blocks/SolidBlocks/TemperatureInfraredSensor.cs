using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class TemperatureInfraredSensor : RotationBlock
    {

        float nt;
        int distance;

        public TemperatureInfraredSensor(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("temperatureInfraredSensor", "sensor");
            density = 5.4f;
            transmissivity = 2.85f;
            currentSettingValue = 1;
            distance = currentSettingValue;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            TemperatureInfraredSensor block = new TemperatureInfraredSensor(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1550);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.circuitBoard.getId(), 3 },
                    { blocksManager.steel.getId(), 1 },
                    { blocksManager.semiconductor.getId(), 6 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            detectTemperatureRule(blocksEngine);

        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            distance = currentSettingValue;
        }

        public override void onSettingValueChange()
        {
            base.onSettingValueChange();
            distance = currentSettingValue;
        }

        void detectTemperatureRule(BlocksEngine blocksEngine)
        {
            IPoint detectCoor = IPoint.zero;
            switch (dir)
            {
                case 0:
                    detectCoor = getCoor() + new IPoint(distance, 0);
                    break;
                case 1:
                    detectCoor = getCoor() + new IPoint(0, -distance);
                    break;
                case 2:
                    detectCoor = getCoor() + new IPoint(-distance, 0);
                    break;
                case 3:
                    detectCoor = getCoor() + new IPoint(0, distance);
                    break;
            }

            if (!blocksEngine.isOutRang(detectCoor.x, detectCoor.y))
            {
                Block detectBlock = blocksEngine.getBlock(detectCoor);
                if (detectBlock != null)
                {
                    nt = detectBlock.getTemperature() + 0.1f;
                    putWe(blocksEngine, Dir.up, nt);
                    putWe(blocksEngine, Dir.right, nt);
                    putWe(blocksEngine, Dir.down, nt);
                    putWe(blocksEngine, Dir.left, nt);
                }             
            }
                     
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

        public override int[] getSettingValueRank()
        {
            return new int[2] { 1, 20 };
        }

        public override string getSettingValueName()
        {
            return "detect distance";
        }

        public override int isWeSystem()
        {
            return 1;
        }       
    }
}
