using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Refrigerator : SolidBlock
    {
        bool isWork;
        protected float comsume;

        public Refrigerator(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("refrigerator", "industry");
            isCanChangeRedAndCrackTexture = false;
            transmissivity = 3.2f;
            density = 10.3f;
            comsume = 1.0f;
            currentSettingValue = 0;
            isWork = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Refrigerator block = new Refrigerator(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.steel.getId(), 4},
                    { blocksManager.chlorine.getId(), 5 },
                    { blocksManager.circuitBoard.getId(), 6 }
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

            refrigeratorRule(blocksEngine);
        }

        protected void refrigeratorRule(BlocksEngine blocksEngine)
        {
            if (isWork)
            {
                float receive = Pooler.instance.requireElectric(this, Mathf.Abs(currentSettingValue * comsume));
                if (receive > 0)
                {
                    float ot = getTemperature();
                    if (ot > currentSettingValue)
                    {
                        decHeatQuantity(receive * 10000);
                    }
                    setSpriteRect(2);
                }
                else
                {
                    setSpriteRect(1);
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
            return new int[2] { -40, 500 };
        }

        public override string getSettingValueName()
        {
            return "temperature";
        }
    }
}
