using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft.BlockSpace
{
    public class Electrolyser : SolidBlock
    {
        bool isWork;
        bool isRun;
        protected float comsume;

        int spriteIndex;

        protected Block targetBlock;
        protected float progress;
        protected float speed;
        protected int totalTargetCount;
        protected int targetCount;
        protected bool productAir;

        BlocksManager blocksManager;

        public Electrolyser(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("electrolyser", "industry");

            thumbnailColor = new Color(0.498f, 0.498f, 0.498f);
            density = 7.85f;
            transmissivity = 7.2f;
            max_storeAir = 0;
            isWork = false;
            isRun = false;
            isCanChangeRedAndCrackTexture = false;
            spriteIndex = 0;
            comsume = 500;

            blocksManager = BlocksManager.instance;

            targetBlock = null;
            progress = 0;
            targetCount = 0;
            totalTargetCount = 0;
            speed = 0.3f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Electrolyser block = new Electrolyser(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[4, 2] {
                    { blocksManager.copper.getId(), 1 },
                    { blocksManager.coal.getId(), 1 },
                    { blocksManager.circuitBoard.getId(), 2 },
                    { blocksManager.fineSteel.getId(), 4 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        protected virtual void compositeTableMethod()
        {
            if (compositeMethod(blocksManager.water, blocksManager.chlorine, 20, false)) return;
            if (compositeMethod(blocksManager.distilledWater, blocksManager.hydrogen, 20, true)) return;           

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
            centrifugeRule();
            setSpriteRect(isRun ? 1 : 0);
        }

        protected void centrifugeRule()
        {
            if (isWork)
            {
                float receive = Pooler.instance.requireElectric(this, comsume);
                if (receive > comsume * 0.9f)
                {
                    rawMethod();
                    centrifugeMethod();
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

        protected void rawMethod()
        {
            if (targetBlock == null)
            {                              
                compositeTableMethod();
            }
        }

        protected bool compositeMethod(Block raw, Block product, int count, bool isProductAir)
        {
            Block inBlock= getNeighborBlock(Dir.down);
            if (!inBlock.equalBlock(raw))
            {
                inBlock = getNeighborBlock(Dir.left);
                if (!inBlock.equalBlock(raw))
                {
                    inBlock = getNeighborBlock(Dir.right);
                    if (!inBlock.equalBlock(raw))
                    {
                        inBlock = getNeighborBlock(Dir.up);
                        if (!inBlock.equalBlock(raw))
                        {
                            inBlock = null;
                        }
                    }
                }
            }            

            if (inBlock != null)
            {
                BlocksEngine.instance.removeBlock(inBlock.getCoor());
                targetBlock = product;
                progress = 0;
                targetCount = count;
                totalTargetCount = count;
                productAir = isProductAir;
                return true;
            }
            return false;
        }


        protected void centrifugeMethod()
        {
            if (targetBlock != null)
            {
                if (productAir)
                {
                    Pooler.instance.chargeAir(500);
                }
                if (progress >= 1)
                {
                    IPoint airCoor = getAirBlockCoor();
                    if (null != airCoor)
                    {                        
                        BlocksEngine.instance.createBlock(airCoor, targetBlock);                       
                        if (targetCount <= 1)
                        {
                            targetBlock = null;
                            targetCount = 0;                            
                            productAir = false;
                        }
                        else
                        {
                            targetCount--;
                            progress = 0;                            
                        }

                    }
                }
                else
                {
                    progress += speed;                   
                }                
            }
            if (isSelecting)
            {
                BlockProgress.instance.setValue(1 -((float)targetCount / totalTargetCount));
            }
        }

        public IPoint getAirBlockCoor()
        {
            if (getNeighborBlock(Dir.down).isAir())
            {
                return getCoor().getDirPoint(Dir.down);
            }
            if (getNeighborBlock(Dir.left).isAir())
            {
                return getCoor().getDirPoint(Dir.left);
            }
            if (getNeighborBlock(Dir.right).isAir())
            {
                return getCoor().getDirPoint(Dir.right);
            }
            if (getNeighborBlock(Dir.up).isAir())
            {
                return getCoor().getDirPoint(Dir.right);
            }
            return null;
        }

        public override void onReciverWe(float voltage, int putterDir, Block putter)
        {
            base.onReciverWe(voltage, putterDir, putter);
            isWork = voltage > LogicGate.StandardVoltage;
        }

        public override float getProgress()
        {
            return progress;
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

        public override bool isCargohold()
        {
            return true;
        }

        public override JsonWriter onWorldModeSave(JsonWriter writer)
        {
            writer = base.onWorldModeSave(writer);
            IUtils.keyValue2Writer(writer, "ta", targetBlock);
            IUtils.keyValue2Writer(writer, "pg", progress);
            IUtils.keyValue2Writer(writer, "co", targetCount);
            IUtils.keyValue2Writer(writer, "pa", productAir);
            return writer;
        }

        public override void onWorldModeLoad(JsonData blockData, IPoint coor)
        {
            base.onWorldModeLoad(blockData, coor);
            targetBlock = IUtils.getJsonValue2Block(blockData, "ta", null);
            progress = IUtils.getJsonValue2Float(blockData, "pg");
            targetCount = IUtils.getJsonValue2Int(blockData, "co");
            productAir = IUtils.getJsonValue2Bool(blockData, "pa");
        }
    }
}
