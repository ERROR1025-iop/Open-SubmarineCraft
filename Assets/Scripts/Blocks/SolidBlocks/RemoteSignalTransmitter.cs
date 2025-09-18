using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class RemoteSignalTransmitter : RotationBlock
    {

        int distance;
        float voltage;

        public RemoteSignalTransmitter(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("remoteSignalTransmitter", "signal");
            density = 5.4f;
            transmissivity = 2.85f;
            currentSettingValue = 1;
            distance = currentSettingValue;
            voltage = 0;
            isCanChangeRedAndCrackTexture = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            RemoteSignalTransmitter block = new RemoteSignalTransmitter(blockId, parentObject, blockObject);
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

        public override void onReciverWe(float voltage, int putterDir, Block putter)
        {
            base.onReciverWe(voltage, putterDir, putter);

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

            this.voltage = voltage;

            setSpriteRect(voltage > LogicGate.StandardVoltage ? 1 : 0);

            if (detectCoor != putter.getCoor())
            {
                int outsideTag = Pooler.instance.getOutsideAreaTag(detectCoor);
                if (outsideTag == 0 || outsideTag == 5)
                {
                    BlocksEngine.instance.putWe(this, detectCoor, voltage - Cable.Voltage_Drop);
                }                
            }            
        }

        public override float getRomaoteMe()
        {
            return voltage;
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
            return "Transmit distance";
        }

        public override int isWeSystem()
        {
            return 1;
        }
    }
}
