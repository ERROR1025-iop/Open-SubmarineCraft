using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace {
    public class WoodCargohold : Wood
    {
        public WoodCargohold(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("woodCargohold", "industry");
            thumbnailColor = new Color(0.678f, 0.5372f, 0.3176f);
            isCanChangeRedAndCrackTexture = false;
            transmissivity = 0.32f;
            density = 0.75f;

            burningPoint = 270;
            isBurning = false;
            calorific = 395;
            unityCalorific = 4;

            m_normalSpriteIndex = 0;
            m_buringSpriteIndex = 1;
            heatCapacity = 2730f;
            max_storeAir = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            WoodCargohold block = new WoodCargohold(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "null", 3300, 530);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[1, 2] {
                    { blocksManager.wood.getId(), 4 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 4);

            return syntInfos;
        }

        public override bool isCargohold()
        {
            return true;
        }
    }
}
