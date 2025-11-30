using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Air : GasBlock
    {

        public Air(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("air", "null");
            canStoreInTag = 0;
            density = 0.0129f;
            transmissivity = 21f;
            m_isFluid = false;
            price = 0;
            heatCapacity = 1012f;

            max_storeAir = 1000;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Air block = new Air(blockId, parentObject, blockObject);
            return block;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            transmissivity = Mathf.Clamp(press * 0.21f, 0.001f, 22f);
        }       
    }
}
