using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace{ public class Wifi : SolidBlock
    {

        public static List<Wifi> wifiArr;

        bool isInitWifi;
        int wid;

        float voltage;

        public Wifi(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("wifi", "signal");
            isCanChangeRedAndCrackTexture = false;
            thumbnailColor = new Color(0.0f, 0.4705f, 0.8352f);
            density = 4.1f;
            transmissivity = 2.85f;
            currentSettingValue = 100;
            wid = currentSettingValue;
            isInitWifi = false;

        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Wifi block = new Wifi(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1550);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[2, 2] {
                    { blocksManager.circuitBoard.getId(), 1 },
                    { blocksManager.cable.getId(), 2 },

            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 2);

            return syntInfos;
        }

        public override void onRegister()
        {
            base.onRegister();

            if (wifiArr == null)
            {
                wifiArr = new List<Wifi>();
            }
            else
            {
                wifiArr.Clear();
            }
        }

        public override void clear(bool destroy)
        {
            wifiArr.Remove(this);
            base.clear(destroy);
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);

            if (isInitWifi == false)
            {
                wifiArr.Add(this);
                isInitWifi = true;
            }

            voltage -= 200;
            if (voltage < 0)
            {
                voltage = 0;
            }
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

        public override void onReciverWe(float value, int putterDir, Block putter)
        {
            if (voltage >= value || value <= 0)
            {
                return;
            }
            voltage = value;
            setSpriteRect(voltage > LogicGate.StandardVoltage ? 1 : 0);

            if (!putter.isCanSendWifi())
            {
                foreach (Wifi block in wifiArr)
                {
                    if (block.getWifiId() == wid)
                    {
                        if (GameSetting.isChannel100Activity || wid != 100)
                        {
                            BlocksEngine.instance.putWe(this, block.getCoor(), voltage - Cable.Voltage_Drop);                            
                        }
                    }
                }
            }

            if (!putter.getCoor().isNeighborBlock(getCoor()))
            {
                Block up_block = getNeighborBlock(Dir.up);
                Block right_block = getNeighborBlock(Dir.right);
                Block down_block = getNeighborBlock(Dir.down);
                Block left_block = getNeighborBlock(Dir.left);

                putWeMethod(up_block, putter);
                putWeMethod(right_block, putter);
                putWeMethod(down_block, putter);
                putWeMethod(left_block, putter);
            }

            voltage = 0;
        }

        public override float getRomaoteMe()
        {
            return voltage;
        }

        void putWeMethod(Block block, Block putter)
        {
            if (block.getCoor() != putter.getCoor())
            {
                BlocksEngine.instance.putWe(this, block.getCoor(), voltage - Cable.Voltage_Drop);
            }
        }

        public int getWifiId()
        {
            return wid;
        }

        public override int isWeSystem()
        {
            return 1;
        }

        public override bool isCanSendWifi()
        {
            return true;
        }

        public override int isCanSettingValue()
        {
            return 0;
        }

        public override string getSettingValueName()
        {
            return "channel";
        }
    }
}
