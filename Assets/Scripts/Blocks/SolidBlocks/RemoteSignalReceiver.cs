using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class RemoteSignalReceiver : RotationBlock
    {

        float nt;
        int distance;

        public RemoteSignalReceiver(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("remoteSignalReceiver", "signal");
            density = 5.4f;
            transmissivity = 2.85f;
            currentSettingValue = 1;
            distance = currentSettingValue;
            isCanChangeRedAndCrackTexture = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            RemoteSignalReceiver block = new RemoteSignalReceiver(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1550);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[2, 2] {
                    { blocksManager.wifi.getId(), 1 },                   
                    { blocksManager.semiconductor.getId(), 6 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            receiveMeRule(blocksEngine);

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

        void receiveMeRule(BlocksEngine blocksEngine)
        {
            IPoint detectCoor = IPoint.zero;
            switch (dir)
            {
                case 1:
                    detectCoor = getCoor() + new IPoint(distance, 0);
                    break;
                case 2:
                    detectCoor = getCoor() + new IPoint(0, -distance);
                    break;
                case 3:
                    detectCoor = getCoor() + new IPoint(-distance, 0);
                    break;
                case 0:
                    detectCoor = getCoor() + new IPoint(0, distance);
                    break;
            }

            if (!blocksEngine.isOutRang(detectCoor.x, detectCoor.y))
            {
                Block detectBlock = blocksEngine.getBlock(detectCoor);
                if (detectBlock != null)
                {
                    nt = detectBlock.getRomaoteMe();
                    putWe(blocksEngine, Dir.up, nt);
                    putWe(blocksEngine, Dir.right, nt);
                    putWe(blocksEngine, Dir.down, nt);
                    putWe(blocksEngine, Dir.left, nt);
                }
            }

        }

        private void putWe(BlocksEngine blocksEngine, int dir, float voltage)
        {
            if (getRelativeDir(Dir.up) != dir)
                blocksEngine.putWe(this, getCoor().getDirPoint(dir), voltage - Cable.Voltage_Drop);
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
            return "Receive distance";
        }

        public override int isWeSystem()
        {
            return 1;
        }
    }
}
