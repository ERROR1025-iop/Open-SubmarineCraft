using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class AdvCrane : RotationBlock
    {

        int Max_Count;
        int Max_Distance;
        bool isMoveForward;
        int offset;
        int distance;
        int count;
        int blockLong;
        bool limitMoved;
        Block forwardBlock;
        Block backBlock;
        Block[] moveArr;
        bool isWork;
        bool isTurnOn;
        IPoint receiveMeCoor;

        public AdvCrane(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("advCrane", "industry");

            thumbnailColor = new Color(0.0f, 0.4705f, 0.8352f);
            transmissivity = 2.85f;
            density = 5.3f;

            Max_Count = 10;
            Max_Distance = 15;
            isWork = false;
            isTurnOn = false;
            distance = currentSettingValue;
            count = currentSettingValue;
            isMoveForward = true;
            blockLong = 0;
            moveArr = new Block[Max_Count];
            clearMoveArr();
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            AdvCrane block = new AdvCrane(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1550);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.crane.getId(), 1 },
                    { blocksManager.remoteSignalReceiver.getId(), 1 },
                    { blocksManager.circuitBoard.getId(), 1 },
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void onWorldModeClick()
        {
            isTurnOn = !isTurnOn;
            isWork = isTurnOn;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            craneRule(blocksEngine);
        }

        void craneRule(BlocksEngine blocksEngine)
        {
            if(receiveMeCoor != null)
            {
                Block meBlock = blocksEngine.getBlock(receiveMeCoor);
                if (meBlock != null && !meBlock.isNeedDelete())
                {
                    int receiveDistance = (int)(meBlock.getRomaoteMe() / 100f);
                    if (distance != receiveDistance)
                    {
                        distance = receiveDistance;
                    }
                }
            } 
        

            if (offset != distance)
            {
                isMoveForward = offset < distance;                

                count = currentSettingValue;
                if (limitMoved)
                {
                    limitMoved = false;
                    return;
                }
                if (isMoveForward && offset == distance)
                {                    
                    isTurnOn = false;
                    isWork = isTurnOn;
                }
                else if (isMoveForward == false && offset == distance)
                {                    
                    isTurnOn = false;
                    isWork = isTurnOn;
                }
                else if (isMoveForward)
                {
                    blockLong = fillMoveArr(blocksEngine);
                    if (forwardBlock.isFluid() || forwardBlock.isAir() || forwardBlock.equalPState(PState.particle))
                    {
                        blockMoveForward(blocksEngine);
                        offset++;
                        moveTo(getCoor().getDirPoint(dir));
                        if (dir == Dir.up)
                            setLimitMoved(true);
                    }
                }
                else if (isMoveForward == false)
                {
                    blockLong = fillMoveArr(blocksEngine);
                    backBlock = getRelativeNeighborBlock(Dir.down);
                    if (backBlock.isFluid() || backBlock.isAir() || backBlock.equalPState(PState.particle))
                    {
                        int backDir = dir + 2;
                        if (backDir > 3)
                        {
                            backDir -= 4;
                        }
                        moveTo(getCoor().getDirPoint(backDir));
                        blockMoveBack(blocksEngine, backDir);
                        if (backDir == Dir.up)
                            setLimitMoved(true);
                        offset--;
                    }
                }
            }
        }

        protected virtual void blockMoveForward(BlocksEngine blocksEngine)
        {
            for (int i = blockLong - 1; i >= 0; i--)
            {
                Block block = moveArr[i];
                if (block != null)
                {
                    block.moveTo(block.getCoor().getDirPoint(dir));
                }
            }
        }

        protected virtual void blockMoveBack(BlocksEngine blocksEngine, int backDir)
        {
            for (int i = 0; i < blockLong; i++)
            {
                Block block = moveArr[i];
                if (block != null)
                {
                    block.moveTo(block.getCoor().getDirPoint(backDir));
                }
            }
        }

        int fillMoveArr(BlocksEngine blocksEngine)
        {
            clearMoveArr();
            switch (dir)
            {
                case 0:
                    {
                        for (int i = 0; i < count; i++)
                        {
                            Block block = blocksEngine.getBlock(getCoor().x, getCoor().y + i + 1);
                            if (block.equalPState(PState.solid) && block.isBorder() == false)
                            {
                                moveArr[i] = block;
                            }
                            else
                            {
                                forwardBlock = blocksEngine.getBlock(getCoor().x, getCoor().y + i + 2);
                                return i;
                            }

                        }
                        forwardBlock = blocksEngine.getBlock(getCoor().x, getCoor().y + count + 1);
                        return count;
                    }
                case 1:
                    {
                        for (int i = 0; i < count; i++)
                        {
                            Block block = blocksEngine.getBlock(getCoor().x + i + 1, getCoor().y);
                            if (block.equalPState(PState.solid) && block.isBorder() == false)
                            {
                                moveArr[i] = block;
                            }
                            else
                            {
                                forwardBlock = blocksEngine.getBlock(getCoor().x + i + 2, getCoor().y);
                                return i;
                            }

                        }
                        forwardBlock = blocksEngine.getBlock(getCoor().x + count + 1, getCoor().y);
                        return count;
                    }
                case 2:
                    {
                        for (int i = 0; i < count; i++)
                        {
                            Block block = blocksEngine.getBlock(getCoor().x, getCoor().y - i - 1);
                            if (block.equalPState(PState.solid) && block.isBorder() == false)
                            {
                                moveArr[i] = block;
                            }
                            else
                            {
                                forwardBlock = blocksEngine.getBlock(getCoor().x, getCoor().y - i - 2);
                                return i;
                            }

                        }
                        forwardBlock = blocksEngine.getBlock(getCoor().x, getCoor().y - count - 1);
                        return count;
                    }
                case 3:
                    {
                        for (int i = 0; i < count; i++)
                        {
                            Block block = blocksEngine.getBlock(getCoor().x - i - 1, getCoor().y);
                            if (block.equalPState(PState.solid) && block.isBorder() == false)
                            {
                                moveArr[i] = block;
                            }
                            else
                            {
                                forwardBlock = blocksEngine.getBlock(getCoor().x - i - 2, getCoor().y);
                                return i;
                            }

                        }
                        forwardBlock = blocksEngine.getBlock(getCoor().x - count - 1, getCoor().y);
                        return count;
                    }
            }
            return 0;
        }

        void clearMoveArr()
        {
            for (int i = 0; i < 5; i++)
            {
                moveArr[i] = null;
            }
        }

        public override void onReciverWe(float value, int putterDir, Block putter)
        {
            if (putterDir == getRelativeDir(Dir.up) && putter.getCoor().isNeighborBlock(getCoor()))
            {
                return;
            }

            int receiveDistance = (int)(value / 100f);
            if(distance != receiveDistance)
            {
                distance = receiveDistance;
                receiveMeCoor = putter.getCoor();
            }
            isWork = isTurnOn;
        }

        public override int isWeSystem()
        {
            return 1;
        }

        void setLimitMoved(bool limit)
        {
            limitMoved = limit;
        }

        public override int isCanSettingValue()
        {
            return 0;
        }

        public override int[] getSettingValueRank()
        {
            return new int[2] { 0, 20 };
        }

        public override string getSettingValueName()
        {
            return "blocks count";
        }
    }
}