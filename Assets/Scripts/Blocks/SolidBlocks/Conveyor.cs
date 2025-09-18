using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
namespace Scraft.BlockSpace
{
    public class Conveyor : RotationBlock
    {
        bool isWork;
        bool isRun;
        protected float comsume;

        int spriteIndex;      
        protected int transferDir;
        int coorY;
        int moveLimit;

        public Conveyor(int id, GameObject parentObject, GameObject blockObject)
               : base(id, parentObject, blockObject)
        {
            initBlock("conveyor", "industry");
            transmissivity = 3.2f;
            density = 10.3f;
            comsume = 0.2f;           
            isWork = false;
            isRun = false;
            isCanChangeRedAndCrackTexture = false;
            spriteIndex = 0;
            transferDir = 0;
            setBindId(6);
            moveLimit = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Conveyor block = new Conveyor(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[2, 2] {
                    { blocksManager.steel.getId(), 10 },
                    { blocksManager.smallElectorEngine.getId(), 5 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 10);

            return syntInfos;
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            isWork = getCurrentBindId() == 6;
            updateTransferDir();
            coorY = getCoor().y % 2;
        }

        public override void onBuilderModeCreated()
        {
            base.onBuilderModeCreated();
            updateSpriteRect();            
        }

        public override void onSettingValueChange()
        {
            base.onSettingValueChange();
            updateTransferDir();
        }

        void updateTransferDir()
        {
            transferDir = currentSettingValue == 0 ? 0 : 2; 
            setSpriteRect(transferDir + spriteIndex);
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
            conveyorRule();
            updateSpriteRect();
            moveLimit = moveLimit == 0 ? 1 : 0;
        }

        protected void conveyorRule()
        {
            if(coorY == moveLimit)
            {
                if(dir == Dir.left && transferDir == 0)
                {
                    return;
                }
                if (dir == Dir.right && transferDir == 2)
                {
                    return;
                }
            }

            if (isWork)
            {              
                float receive = Pooler.instance.requireElectric(this, comsume);
                if (receive > comsume * 0.9f)
                {
                    conveyorMethod();
                    isRun = true;
                }
                else
                {
                    isRun = false;
                }                
            }
            else
            {
                isRun = false;
            }
        }

        protected void conveyorMethod()
        {
            Block block = getRelativeNeighborBlock(Dir.up);            
            if(block != null && !block.isAir())
            {
                IPoint toCoor = block.getCoor().getDirPoint(getRelativeDir(Dir.right + transferDir));
                if (BlocksEngine.instance.getBlock(toCoor).isAir())
                {
                    block.moveTo(toCoor);
                }               
            }
        }

        void updateSpriteRect()
        {
            if (isRun)
            {
                spriteIndex = spriteIndex == 0 ? 1 : 0;
                setSpriteRect(transferDir + spriteIndex);
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
            return 0;
        }

        public override string getSettingValueName()
        {
            return "transport direction";
        }

        public override int[] getSettingValueRank()
        {
            return new int[2] { 0, 1 };
        }
       
    }
}