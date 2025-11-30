using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class ElectorHeater : SolidBlock
    {

        bool isWork;
        protected float comsume;

        public ElectorHeater(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("electorHeater", "industry");
            transmissivity = 3.2f;
            density = 10.3f;
            comsume = 1.0f;
            currentSettingValue = 1000;
            isWork = false;
            isCanChangeRedAndCrackTexture = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            ElectorHeater block = new ElectorHeater(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.winding.getId(), 2 },
                    { blocksManager.fineSteel.getId(), 1 },
                    { blocksManager.circuitBoard.getId(), 2 }                   
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            isWork = getCurrentBindId() == 6;

        }

        public override void onWorldModeClick()
        {
            base.onWorldModeClick();
            isWork = !isWork;
        }

        public override void onPreesButtonClick(bool isClick)
        {
            isWork = isClick;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);

            electorHeaterRule(blocksEngine);
        }

        protected void electorHeaterRule(BlocksEngine blocksEngine)
        {
            if (isWork)
            {
                if(temperature < meltingPoint * 0.9f)
                {
                    float real_comsume = Mathf.Abs(currentSettingValue * comsume * 0.3f);
                    float receive = Pooler.instance.requireElectric(this, real_comsume);
                    if (receive > real_comsume * 0.9f)
                    {
                        float ot = getTemperature();
                        if (ot < currentSettingValue)
                        {
                            //e * 100 = hq
                            float hq = receive * 100;
                            Debug.Log("Heater hq:" + hq);
                            getNeighborBlock(Dir.up).addHeatQuantity(hq);
                        }
                        setSpriteRect(1);
                    }
                    else
                    {
                        setSpriteRect(0);
                    }
                }
                else
                {
                    setSpriteRect(0);
                }
            }
            else
            {
                setSpriteRect(0);
            }
        }

        public override void onReciverWe(float voltage, int putterDir, Block putter)
        {
            base.onReciverWe(voltage, putterDir, putter);
            isWork = voltage > LogicGate.StandardVoltage;
        }

        public override int isWeSystem()
        {
            return 1;
        }

        public override bool isCanBind()
        {
            return true;
        }

        public override int[] getBindArr()
        {
            return new int[3] { 4, 5, 6 };
        }

        public override int isCanSettingValue()
        {
            return 3;
        }

        public override int[] getSettingValueRank()
        {
            return new int[2] { 0, 1300 };
        }

        public override string getSettingValueName()
        {
            return "power";
        }
    }
}
