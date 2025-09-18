using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class AdvPump : Pump
    {

        protected Block storageWater;
        protected float storageAirPress;

        public AdvPump(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("advPump", "machine");

            thumbnailColor = new Color(0f, 0.7764f, 1f);
            density = 11.1f;
            storageWater = null;
            storageAirPress = 0;
            comsume = 5.0f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            AdvPump block = new AdvPump(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            block.initRotationBlock();
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[2, 2] {
                    { blocksManager.pump.getId(), 1 },
                    { blocksManager.fineSteel.getId(), 1 }                    
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        protected override void pumpRule(BlocksEngine blocksEngine)
        {
            if (isWork)
            {
                Block inBlock = getRelativeNeighborBlock(Dir.down);
                Block outBlock = getRelativeNeighborBlock(Dir.up);
                float dpress = outBlock.getPress() - inBlock.getPress();
                comsume = dpress > 500 ? 5 + dpress * dpressComsume : 5;                

                float receive = Pooler.instance.requireElectric(this, comsume);
                if (receive > 0)
                {
                    Block air = blocksEngine.getBlocksManager().air;

                    releaseAirMethod(outBlock);

                    if (isStorageWater())
                    {

                        if (outBlock.equalBlock(air))
                        {
                            blocksEngine.placeBlock(outBlock.getCoor(), storageWater);
                            storageWater = null;
                        }
                        else if (outBlock.isFluid())
                        {
                            pumpCompressMethod(blocksEngine, storageWater, outBlock, true);
                            storageWater = null;
                        }
                        else if (outBlock.equalBlock(this))
                        {
                            pumpTransferMethod(blocksEngine, storageWater, outBlock as AdvPump, true);

                        }
                    }
                    else if (inBlock.isFluid())
                    {
                        if (outBlock.equalBlock(air))
                        {
                            inBlock.moveTo(outBlock.getCoor());
                        }
                        else if (outBlock.isFluid())
                        {
                            pumpCompressMethod(blocksEngine, inBlock, outBlock, false);
                        }
                        else if (outBlock.equalBlock(this))
                        {
                            pumpTransferMethod(blocksEngine, inBlock, outBlock as AdvPump, false);
                        }
                    }else if (inBlock.isAir())
                    {
                        if (outBlock.isAir() || outBlock.isFluid())
                        {
                            outputAirMethod(inBlock, outBlock);
                        }
                        else if (outBlock.equalBlock(this))
                        {
                            inputAirMethod(inBlock, outBlock);
                        }
                    }
                }
                setSpriteRect(1);
            }
            else
            {
                setSpriteRect(0);
            }
        }

        protected virtual void releaseAirMethod(Block outBlock)
        {
            if (storageAirPress > 0)
            {
                if (outBlock.isAir() || outBlock.isFluid())
                {
                    outBlock.addPress(storageAirPress);
                }
                else if (outBlock.equalBlock(this))
                {
                    (outBlock as AdvPump).pushInAirPress(storageAirPress);
                }
            }
        }

        protected virtual void inputAirMethod(Block inBlock, Block outBlock)
        {

        }

        protected virtual void outputAirMethod(Block inBlock, Block outBlock)
        {
            
        }

        protected void pumpCompressMethod(BlocksEngine blocksEngine, Block inBlock, Block outBlock, bool isFormAdvPump)
        {

            Block air = blocksEngine.getBlocksManager().air;

            if (outBlock.equalPState(PState.liquild))
            {
                if (((LiquidBlock)outBlock).addCompressChild(inBlock))
                {
                    if (isFormAdvPump)
                    {
                        storageWater = null;
                    }
                    else
                    {
                        blocksEngine.createBlock(inBlock.getCoor(), air, false);
                    }

                }
            }
            else if (outBlock.equalPState(PState.gas))
            {
                if (((GasBlock)outBlock).addCompressChild(inBlock))
                {
                    if (isFormAdvPump)
                    {
                        storageWater = null;
                    }
                    else
                    {
                        blocksEngine.createBlock(inBlock.getCoor(), air, false);
                    }
                }
            }
        }

        protected void pumpTransferMethod(BlocksEngine blocksEngine, Block inBlock, AdvPump outBlock, bool isFormAdvPump)
        {
            if (outBlock != null && !outBlock.isStorageWater())
            {
                outBlock.pushInWater(inBlock);
                if (isFormAdvPump)
                {
                    storageWater = null;
                }
                else
                {
                    blocksEngine.createBlock(inBlock.getCoor(), blocksEngine.getBlocksManager().air, false);
                }
            }
        }

        public void pushInAirPress(float press) 
        {
            storageAirPress = press;
        }

        public bool isStorageWater()
        {
            return storageWater != null;
        }

        public void pushInWater(Block inBlock)
        {
            if (!isStorageWater())
            {
                storageWater = inBlock;
            }
        }

        protected override void solidCrackRule()
        {

        }

        protected override void solidRedRule()
        {

        }
    }
}