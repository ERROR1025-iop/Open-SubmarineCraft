using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft.BlockSpace
{


    public class Centrifuge : RotationBlock
    {
        bool isWork;
        bool isRun;
        protected float comsume;
        protected float receive;

        int spriteIndex;

        protected Block targetBlock;
        protected float progress;
        protected float speed;
        protected int targetCount;

        BlocksManager blocksManager;

        public Centrifuge(int id, GameObject parentObject, GameObject blockObject)
                  : base(id, parentObject, blockObject)
        {
            initBlock("centrifuge", "industry");
            transmissivity = 3.2f;
            density = 15.3f;
            comsume = 100f;
            isWork = false;
            isRun = false;
            isCanChangeRedAndCrackTexture = false;
            spriteIndex = 0;
            receive = 0;

            blocksManager = BlocksManager.instance;

            targetBlock = null;
            progress = 0;
            targetCount = 0;
            speed = 0.005f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Centrifuge block = new Centrifuge(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.fineSteel.getId(), 4 },
                     { blocksManager.circuitBoard.getId(), 2 },
                      { blocksManager.smallElectorEngine.getId(), 1 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        protected virtual void compositeTableMethod(Block inBlock)
        {
            if (compositeMethod(blocksManager.coarseIronOre, blocksManager.fineIronOre, 2, inBlock)) return;
            if (compositeMethod(blocksManager.coarseCopperOre, blocksManager.fineCopperOre, 2, inBlock)) return;
            if (compositeMethod(blocksManager.coarseSulfurOre, blocksManager.fineSulfurOre, 2, inBlock)) return;
            if (compositeMethod(blocksManager.coarseLeadOre, blocksManager.fineLeadOre, 2, inBlock)) return;
            if (compositeMethod(blocksManager.dinas, blocksManager.dinas, 1, inBlock)) return;            

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
                receive += Pooler.instance.requireElectric(this, comsume);
                if (receive > comsume * 0.9f)
                {
                    receive = 0;
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
                Block inBlock = getRelativeNeighborBlock(Dir.left);
                compositeTableMethod(inBlock);
            }
        }

        protected bool compositeMethod(Block raw, Block product, int count, Block inBlock)
        {
            if (raw.equalBlock(product))
            {
                float p = Random.value;
                if (p < 0.02f)
                {
                    product = blocksManager.uraniumPowder;
                    count = 1;
                }
                else if (p > 0.02f && p < 0.058f)
                {
                    product = blocksManager.coalPowder;
                    count = 1 + (int)(Random.value * 2);
                }
                else if (p > 0.058f && p < 0.114f)
                {
                    product = blocksManager.copperPowder;
                    count = 1 + (int)(Random.value * 2);
                }
                else if (p > 0.114f && p < 0.170f)
                {
                    product = blocksManager.ironPowder;
                    count = 1 + (int)(Random.value * 2);
                }
                else if (p > 0.170f && p < 0.226f)
                {
                    product = blocksManager.sulfurPowder;
                    count = 1 + (int)(Random.value * 2);
                }
                else if (p > 0.226f && p < 0.3f)
                {
                    product = blocksManager.leadPowder;
                    count = 1 + (int)(Random.value * 2);
                }
            }

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


        protected void centrifugeMethod()
        {
            if (targetBlock != null)
            {
                if (progress >= 1)
                {
                    Block outBlock = getRelativeNeighborBlock(Dir.right);
                    if (outBlock.isAir())
                    {
                        BlocksEngine.instance.createBlock(outBlock.getCoor(), targetBlock);                        
                        if(targetCount <= 1)
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
