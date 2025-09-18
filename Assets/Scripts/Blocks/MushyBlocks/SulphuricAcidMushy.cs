using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class SulphuricAcidMushy : MushyBlock
    {

        public SulphuricAcidMushy(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("sulphuricAcidMushy", "null");
            thumbnailColor = new Color(0.8784314f, 0.8392158f, 0.4784314f);
            density = 1.8305f;
            transmissivity = 0.35f;            
            heatCapacity = 3500f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SulphuricAcidMushy block = new SulphuricAcidMushy(blockId, parentObject, blockObject);
            block.initMushyBlock(blocksManager, 337, "sulphuricAcid", "sulphuricAcidGas");
            return block;
        }
    }
}