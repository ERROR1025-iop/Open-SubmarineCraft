using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class AdvElectorHeater : ElectorHeater
    {
        

        public AdvElectorHeater(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("advElectorHeater", "industry");                     
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            AdvElectorHeater block = new AdvElectorHeater(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "fineSteelLiquid", 2700, 1750);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.electorHeater.getId(), 1 },
                    { blocksManager.fineSteel.getId(), 1 },
                    { blocksManager.siliconCarbide.getId(), 2 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override int[] getSettingValueRank()
        {
            return new int[2] { 0, 3000 };
        }
    }
}
