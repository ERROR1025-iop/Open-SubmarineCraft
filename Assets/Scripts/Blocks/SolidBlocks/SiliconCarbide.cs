using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class SiliconCarbide : SolidBlock
    {

        public SiliconCarbide(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("siliconCarbide", "structure");

            thumbnailColor = new Color(0.294f, 0.294f, 0.294f);
            transmissivity = 1.2f;
            density = 3.22f;
            heatCapacity = 712;
            isCanbeCorrosion = false;
            penetrationRate = 0.4f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SiliconCarbide block = new SiliconCarbide(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 2700, 623);
            return block;
        }
    }
}
