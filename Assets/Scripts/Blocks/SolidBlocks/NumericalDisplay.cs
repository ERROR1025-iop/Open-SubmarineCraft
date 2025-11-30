using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace{ public class NumericalDisplay : SolidBlock
    {
        float voltage;
        int digit;
        int showNumber;
        bool isStateChanged;

        public NumericalDisplay(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("numericalDisplay", "signal");
            density = 3.1f;
            transmissivity = 2.85f;
            digit = currentSettingValue;
            showNumber = 10;
            voltage = 0;
            isCanChangeRedAndCrackTexture = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            NumericalDisplay block = new NumericalDisplay(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1550);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.circuitBoard.getId(), 1 },
                    { blocksManager.steel.getId(), 1 },
                    { blocksManager.semiconductor.getId(), 2 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void onBuilderModeCreated()
        {
            base.onBuilderModeCreated();
            Block right = getNeighborBlock(Dir.right);
            if (right != null && right.equalBlock(this))
            {
                NumericalDisplay rbd = right as NumericalDisplay;
                if (rbd != null)
                {
                    int rsv = rbd.getCurrentSettingValue();
                    if (rsv < 7)
                    {
                        currentSettingValue = rsv + 1;
                    }
                    else
                    {
                        currentSettingValue = 0;
                    }
                }
            }
        }

        public override void onReciverWe(float value, int putterDir, Block putter)
        {

            if (voltage >= value || value <= 0)
            {
                return;
            }
            isStateChanged = true;
            voltage = value;

            //Block up_block = getNeighborBlock(blocksEngine, Dir.up);
            Block right_block = getNeighborBlock(Dir.right);
            //Block down_block = getNeighborBlock(blocksEngine, Dir.down);
            Block left_block = getNeighborBlock(Dir.left);

            //putWeMethod(blocksEngine, up_block, putter);
            putWeMethod(right_block, putter);
            //putWeMethod(blocksEngine, down_block, putter);
            putWeMethod(left_block, putter);

        }

        void putWeMethod(Block block, Block putter)
        {
            if (block.getCoor() != putter.getCoor())
            {
                BlocksEngine.instance.putWe(this, block.getCoor(), voltage - Cable.Voltage_Drop);
            }
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            displayRule(blocksEngine);
            setSpriteRect(Mathf.Abs(showNumber));
            voltage = 0;
        }

        void displayRule(BlocksEngine blocksEngine)
        {
            digit = currentSettingValue;
            switch (digit)
            {
                case 0:
                    showNumber = (int)voltage % 10;
                    break;
                case 1:
                    showNumber = (int)voltage / 10 % 10;
                    if (voltage > -10 && voltage < 0)
                    {
                        showNumber = 10;
                    }
                    break;
                case 2:
                    showNumber = (int)voltage / 100 % 10;
                    if (voltage > -100 && voltage < -10)
                    {
                        showNumber = 10;
                    }
                    break;
                case 3:
                    showNumber = (int)voltage / 1000 % 10;
                    if (voltage > -1000 && voltage < -100)
                    {
                        showNumber = 10;
                    }
                    break;
                case 4:
                    showNumber = (int)voltage / 10000 % 10;
                    if (voltage > -10000 && voltage < -1000)
                    {
                        showNumber = 10;
                    }
                    break;
                case 5:
                    showNumber = (int)voltage / 100000 % 10;
                    if (voltage > -100000 && voltage < -10000)
                    {
                        showNumber = 10;
                    }
                    break;
                case 6:
                    showNumber = (int)voltage / 1000000 % 10;
                    if (voltage > -1000000 && voltage < -100000)
                    {
                        showNumber = 10;
                    }
                    break;    
                case 7:
                    showNumber = (int)voltage / 10000000 % 10;
                    if (voltage > -10000000 && voltage < -1000000)
                    {
                        showNumber = 10;
                    }
                    break;
            }
        }

        public override int isCanSettingValue()
        {
            return 0;
        }

        public override int[] getSettingValueRank()
        {
            return new int[2] { 0, 7 };
        }

        public override string getSettingValueName()
        {
            return "display";
        }

        public override int isWeSystem()
        {
            return 1;
        }
          
    }
}
