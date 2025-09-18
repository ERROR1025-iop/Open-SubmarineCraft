using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class PushCrane : RotationBlock
    {


        int Max_Distance;
        bool isMoveForward;
        int offset;
        int distance;
        Block backBlock;
        Block forwardBlock;
        Block forward2Block;
        bool limitMoved;
        bool isWork;
        bool isTurnOn;

        public PushCrane(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("pushCrane", "industry");
            thumbnailColor = new Color(0.168f, 0.890f, 0.0f);
            transmissivity = 2.85f;
            density = 5.3f;

            Max_Distance = 15;
            isWork = false;
            isTurnOn = false;
            distance = currentSettingValue;
            isMoveForward = true;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            PushCrane block = new PushCrane(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1550);
            return block;
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

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[2, 2] {
                    { blocksManager.steel.getId(), 5 },
                    { blocksManager.smallElectorEngine.getId(), 5 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 5);

            return syntInfos;
        }

        void craneRule(BlocksEngine blocksEngine)
        {
            if (isWork)
            {
                distance = currentSettingValue;
                if (limitMoved)
                {
                    limitMoved = false;
                    return;
                }
                if (isMoveForward && offset == distance)
                {
                    isMoveForward = false;
                    isTurnOn = false;
                    isWork = isTurnOn;
                }
                else if (isMoveForward == false && offset == 0)
                {
                    isMoveForward = true;
                    isTurnOn = false;
                    isWork = isTurnOn;
                }
                else if (isMoveForward)
                {
                    forwardBlock = getRelativeNeighborBlock(Dir.up);
                    if (forwardBlock.equalPState(PState.solid))
                    {
                        forward2Block = forwardBlock.getNeighborBlock(getRelativeDir(Dir.up));
                        if (forward2Block.isFluid() || forward2Block.isAir() || forward2Block.equalPState(PState.particle))
                        {
                            forwardBlock.moveTo(forward2Block.getCoor());
                            moveTo(getCoor().getDirPoint(dir));
                            offset++;
                            if (dir == Dir.up)
                                setLimitMoved(true);
                        }
                    }
                    else if (forwardBlock.isFluid() || forwardBlock.isAir() || forwardBlock.equalPState(PState.particle))
                    {
                        moveTo(getCoor().getDirPoint(dir));
                        offset++;
                        if (dir == Dir.up)
                            setLimitMoved(true);
                    }
                }
                else if (isMoveForward == false)
                {
                    backBlock = getRelativeNeighborBlock(Dir.down);
                    if (backBlock.isFluid() || backBlock.isAir() || backBlock.equalPState(PState.particle))
                    {
                        int backDir = dir + 2;
                        if (backDir > 3)
                        {
                            backDir -= 4;
                        }
                        moveTo(getCoor().getDirPoint(backDir));
                        if (backDir == Dir.up)
                            setLimitMoved(true);
                        offset--;
                    }
                }
            }
        }

        public override void onReciverWe(float value, int putterDir, Block putter)
        {
            if (putterDir == getRelativeDir(Dir.up) && putter.getCoor().isNeighborBlock(getCoor()))
            {
                return;
            }

            isTurnOn = value > LogicGate.StandardVoltage;
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
            return "move value";
        }
    }
}
