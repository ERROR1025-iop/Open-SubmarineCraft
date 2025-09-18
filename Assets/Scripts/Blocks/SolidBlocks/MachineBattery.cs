using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace{ public class MachineBattery : Battery
    {
        public MachineBattery(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("machineBattery", "power");            
            thumbnailColor = new Color(0.886f, 0.509f, 0.1f);
            transmissivity = 4.2f;
            density = 10.85f;
            m_MaxElectric = 1400;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            MachineBattery block = new MachineBattery(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1179, 964);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[2, 2] {
                    { blocksManager.battery.getId(), 1 },
                    { blocksManager.circuitBoard.getId(), 1 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override float requireElectric(Block taker, float require)
        {
            if (taker == null || taker.equalBlock(BlocksManager.instance.electorEngine))
            {
                return 0;
            }
            return base.requireElectric(taker, require);
        }
    }
}
