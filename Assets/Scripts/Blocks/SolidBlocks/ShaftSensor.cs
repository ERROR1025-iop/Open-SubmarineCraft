using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class ShaftSensor : Shaft
    {

        int wid;
        float outputWe;

        public ShaftSensor(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("shaftSensor", "sensor");

            thumbnailColor = new Color(0.4286f, 0.4286f, 0.4286f);
            density = 16.7f;

            currentSettingValue = 100;
            wid = currentSettingValue;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            ShaftSensor block = new ShaftSensor(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1400);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.shaft.getId(), 1},
                    { blocksManager.circuitBoard.getId(), 1 },
                    { blocksManager.steel.getId(), 1 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
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

        public override void onReciverMe(float me, int putterDir, Block putter)
        {
            BlocksEngine.instance.putMe(this, getCoor().getDirPoint(putterDir + 2), me);

            outputWe = Mathf.Abs(me);

            BlocksEngine.instance.putWe(this, getCoor().getDirPoint(Dir.up), outputWe - Cable.Voltage_Drop);
            BlocksEngine.instance.putWe(this, getCoor().getDirPoint(Dir.down), outputWe - Cable.Voltage_Drop);

            List<Wifi> wifiArr = Wifi.wifiArr;
            foreach (Wifi block in wifiArr)
            {
                if (block.getWifiId() == wid)
                {
                    if (GameSetting.isChannel100Activity || wid != 100)
                    {
                        BlocksEngine.instance.putWe(this, block.getCoor(), outputWe - Cable.Voltage_Drop);
                    }
                }
            }
        }

        public override float getRomaoteMe()
        {
            return outputWe;
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

        public override bool isRootUnlock()
        {
            return false;
        }
    }
}
