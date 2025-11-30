using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Neutron : ParticleBlock
    {

        public Neutron(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("neutron", "nuclear");

            thumbnailColor = new Color(0f, 0f, 1f);
            density = 0.0129f;
            transmissivity = 3.62f;            
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Neutron block = new Neutron(blockId, parentObject, blockObject);
            block.initParticleBlock(blocksManager);
            return block;
        }

        public override bool isCareerModeStoreUnlimit()
        {
            return true;
        }
    }
}