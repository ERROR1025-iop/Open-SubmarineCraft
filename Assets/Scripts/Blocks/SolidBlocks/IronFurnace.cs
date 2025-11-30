using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace{ public class IronFurnace : SolidBlock
    {

        protected bool isWork;
        protected float storeFuel;
        protected float fuelTotalCalorific;
        protected float comsumeAir;

        public IronFurnace(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("ironFurnace", "industry");
            transmissivity = 3.2f;
            density = 10.3f;
            storeFuel = 0;
            isCanChangeRedAndCrackTexture = false;
            comsumeAir = 300;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            IronFurnace block = new IronFurnace(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[1, 2] {
                    { blocksManager.steel.getId(), 8 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);

            furnaceRule(blocksEngine);
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

        protected void furnaceRule(BlocksEngine blocksEngine)
        {

            if (isWork && storeFuel > 0)
            {
                float receive = Pooler.instance.requireAir(comsumeAir);
                if (receive > comsumeAir * 0.9f)
                {
                    Block up_block = getNeighborBlock(Dir.up);
                    float unityFuel = fuelTotalCalorific / 100;
                    storeFuel -= unityFuel;
                    if (up_block.isAir())
                    {
                        Block fireBlockStatic = blocksEngine.getBlocksManager().fire;
                        Fire fire = blocksEngine.createBlock(up_block.getCoor(), fireBlockStatic, press) as Fire;                        
                        fire.initFire("null", unityFuel);
                    }
                    else
                    {
                        up_block.addHeatQuantity(Fire.C2HQ(unityFuel));
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
                if (!absorbRule(blocksEngine))
                {
                    setSpriteRect(0);
                }
            }

        }

        protected virtual bool absorbRule(BlocksEngine blocksEngine)
        {

            if (storeFuel <= 0)
            {
                if (absorbMethod(blocksEngine, Dir.down)) return true;
                if (absorbMethod(blocksEngine, Dir.right)) return true;
                if (absorbMethod(blocksEngine, Dir.left)) return true;               
                if (absorbMethod(blocksEngine, Dir.up)) return true;
                if (absorbCoalMethod(blocksEngine)) return true;

            }
            return false;
        }

        protected virtual bool absorbMethod(BlocksEngine blocksEngine, int dir)
        {
            Block block = getNeighborBlock(dir);
            if (block.getCalorific() > 0 && !block.equalBlock(BlocksManager.instance.fire) && !block.equalPState(PState.solid))
            {
                fuelTotalCalorific = block.getCalorific();
                storeFuel += fuelTotalCalorific;
                blocksEngine.removeBlock(block.getCoor());
                return true;
            }
            return false;
        }

        protected virtual bool absorbCoalMethod(BlocksEngine blocksEngine)
        {
            Coal coal = Coal.getCoalBlock();
            if (coal != null)
            {
                fuelTotalCalorific = coal.getCalorific();
                storeFuel = fuelTotalCalorific;
                blocksEngine.removeBlock(coal.getCoor());
                return true;
            }
            return false;
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
    }
}
