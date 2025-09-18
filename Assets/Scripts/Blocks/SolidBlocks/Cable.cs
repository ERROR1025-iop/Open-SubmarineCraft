using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Cable : SolidBlock
    {
        public static float Voltage_Drop = 0.01f;

        float voltage;

        public Cable(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("cable", "signal", true);

            thumbnailColor = new Color(0.8823f, 0.329f, 0f);
            density = 2.9f;
            transmissivity = 7.32f;

            voltage = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Cable block = new Cable(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "copperLiquid", 1357, 1120);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[1, 2] {                   
                     { blocksManager.copper.getId(), 1 }
                  
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void onReciverWe(float value, int putterDir, Block putter)
        {

            if (voltage >= value || value <= 0)
            {
                return;
            }
            voltage = value;

            Block up_block = getNeighborBlock(Dir.up);
            Block right_block = getNeighborBlock(Dir.right);
            Block down_block = getNeighborBlock(Dir.down);
            Block left_block = getNeighborBlock(Dir.left);

            putWeMethod(up_block, putter);
            putWeMethod(right_block, putter);
            putWeMethod(down_block, putter);
            putWeMethod(left_block, putter);
        }

        void putWeMethod(Block block, Block putter)
        {
            if (block.getCoor() != putter.getCoor())
            {
                BlocksEngine.instance.putWe(this, block.getCoor(), voltage - Voltage_Drop);
            }
        }

        public override float getRomaoteMe()
        {
            return voltage;
        }


        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            voltage -= 200;
            if (voltage < 0)
            {
                voltage = 0;
            }
        }

        public override int isWeSystem()
        {
            return 1;
        }
    }
}