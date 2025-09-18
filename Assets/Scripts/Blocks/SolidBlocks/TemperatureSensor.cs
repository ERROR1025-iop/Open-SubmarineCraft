using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace{ public class TemperatureSensor : RotationBlock
    {

        float nt;
        int wid;

        public TemperatureSensor(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("temperatureSensor", "sensor");
            density = 5.4f;
            transmissivity = 2.85f;
            currentSettingValue = 100;
            wid = currentSettingValue;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            TemperatureSensor block = new TemperatureSensor(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1550);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.circuitBoard.getId(), 1 },
                    { blocksManager.steel.getId(), 1 },
                    { blocksManager.semiconductor.getId(), 2 }
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
            wid = currentSettingValue;
        }

        public override void onSettingValueChange()
        {
            base.onSettingValueChange();
            wid = currentSettingValue;
        }

        void detectTemperatureRule(BlocksEngine blocksEngine)
        {

            Block detectBlock = getRelativeNeighborBlock(Dir.right);
            nt = detectBlock.getTemperature() + 0.1f;
            putWe(blocksEngine, Dir.up, nt);
            putWe(blocksEngine, Dir.right, nt);
            putWe(blocksEngine, Dir.down, nt);
            putWe(blocksEngine, Dir.left, nt);

            List<Wifi> wifiArr = Wifi.wifiArr;
            foreach (Wifi block in wifiArr)
            {
                if (block.getWifiId() == wid)
                {
                    if (GameSetting.isChannel100Activity || wid != 100)
                    {
                        blocksEngine.putWe(this, block.getCoor(), nt - 1);
                    }
                }
            }
        }

        public override float getRomaoteMe()
        {
            return nt;
        }

        private void putWe(BlocksEngine blocksEngine, int dir, float voltage)
        {
            if (getRelativeDir(Dir.right) != dir)
                blocksEngine.putWe(this, getCoor().getDirPoint(dir), voltage);
        }

        public override int isCanSettingValue()
        {
            return 0;
        }

        public override string getSettingValueName()
        {
            return "channel";
        }

        public override int isWeSystem()
        {
            return 1;
        }

        public override bool isCanSendWifi()
        {
            return true;
        }     
    }
}
