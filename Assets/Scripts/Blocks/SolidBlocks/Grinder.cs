using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
namespace Scraft.BlockSpace
{
    public class Grinder : RotationBlock
    {
        bool isWork;
        bool isRun;
        protected float comsume;
        protected float receive;

        int spriteIndex;

        BlocksManager blocksManager;

        protected Block targetBlock;
        protected float progress;
        protected float speed;
        protected int targetCount;
        

        public Grinder(int id, GameObject parentObject, GameObject blockObject)
               : base(id, parentObject, blockObject)
        {
            initBlock("grinder", "industry");
            transmissivity = 3.2f;
            density = 10.3f;
            comsume = 50f;
            isWork = false;
            isRun = false;
            isCanChangeRedAndCrackTexture = false;
            spriteIndex = 0;
            receive = 0;

            blocksManager = BlocksManager.instance;

            targetBlock = null;
            progress = 0;
            targetCount = 0;
            speed = 0.01f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Grinder block = new Grinder(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.fineSteel.getId(), 2 },
                    { blocksManager.steel.getId(), 1 },
                    { blocksManager.smallElectorEngine.getId(), 1 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        protected virtual void compositeTableMethod(Block inBlock)
        {
            if (compositeMethod(blocksManager.stone, blocksManager.dinas, 1, inBlock)) return;
            if (compositeMethod(blocksManager.ironOre, blocksManager.coarseIronOre, 3, inBlock)) return;
            if (compositeMethod(blocksManager.copperOre, blocksManager.coarseCopperOre, 3, inBlock)) return;
            if (compositeMethod(blocksManager.sulfurOre, blocksManager.coarseSulfurOre, 3, inBlock)) return;
            if (compositeMethod(blocksManager.leadOre, blocksManager.coarseLeadOre, 3, inBlock)) return;
            if (compositeMethod(blocksManager.coal, blocksManager.coalPowder, 2, inBlock)) return;
            if (compositeMethod(blocksManager.steel, blocksManager.ironPowder, 2, inBlock)) return;
            if (compositeMethod(blocksManager.copper, blocksManager.ironPowder, 2, inBlock)) return;
            if (compositeMethod(blocksManager.sulfur, blocksManager.ironPowder, 2, inBlock)) return;
            if (compositeMethod(blocksManager.lead, blocksManager.leadPowder, 2, inBlock)) return;


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
            grinderRule();          
            updateSpriteRect();
        }

        protected void grinderRule()
        {
            if (isWork)
            {
                receive += Pooler.instance.requireElectric(this, comsume);
                if (receive > comsume * 0.9f)
                {
                    receive = 0;
                    rawMethod();
                    grinderMethod();
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
            if(targetBlock == null)
            {
                Block inBlock = getRelativeNeighborBlock(Dir.left);
                compositeTableMethod(inBlock);
            }        
        }

        protected bool compositeMethod(Block raw, Block product, int count, Block inBlock)
        {
            if (inBlock.equalBlock(raw))
            {
                BlocksEngine.instance.removeBlock(inBlock.getCoor());
                targetBlock = product;
                progress = 0;
                targetCount = count;
                return true;
            }
            return false;
        }

        protected void grinderMethod()
        {
            if (targetBlock != null)
            {
                if (progress >= 1)
                {
                    Block outBlock = getRelativeNeighborBlock(Dir.right);
                    if (outBlock.isAir())
                    {
                        BlocksEngine.instance.createBlock(outBlock.getCoor(), targetBlock);
                        if (targetCount <= 1)
                        {
                            targetBlock = null;
                            targetCount = 0;
                            progress = 0;
                        }
                        else
                        {
                            targetCount--;
                        }

                    }
                }
                else
                {
                    progress += speed;
                    if (isSelecting)
                    {
                        BlockProgress.instance.setValue(progress);
                    }
                }
            }
        }

        void updateSpriteRect()
        {
            if (isRun)
            {
                spriteIndex = spriteIndex == 0 ? 1 : 0;
                setSpriteRect(spriteIndex);
            }
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
            return writer;
        }

        public override void onWorldModeLoad(JsonData blockData, IPoint coor)
        {
            base.onWorldModeLoad(blockData, coor);
            targetBlock = IUtils.getJsonValue2Block(blockData, "ta", null);
            progress = IUtils.getJsonValue2Float(blockData, "pg");
            targetCount = IUtils.getJsonValue2Int(blockData, "co");
        }
    }
}
