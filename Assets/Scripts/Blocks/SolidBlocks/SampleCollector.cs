using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scraft.BlockSpace
{
    public class SampleCollector : SolidBlock
    {       

        public SampleCollector(int id, GameObject parentObject, GameObject blockObject)
                : base(id, parentObject, blockObject)
        {
            initBlock("sampleCollector", "other");
            isCanChangeRedAndCrackTexture = false;
            thumbnailColor = new Color(0.0509804f, 0.6509804f, 0.8470589f);
            transmissivity = 4.2f;
            density = 10.85f;
            max_storeAir = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            SampleCollector block = new SampleCollector(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1450, 1500);
            return block;
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            setSpriteRect(collectedScientific > 0 ? 1 : 0);
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[1, 2] {
                    { blocksManager.steel.getId(), 4 }                     
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }      

        public override bool isCanCollectScientific()
        {
            return true;
        }

        public override void onCollectScientific(CollectedScientificInfo csInfo)
        {
            base.onCollectScientific(csInfo);
            collectedScientific = csInfo.point * 2f;
            setSpriteRect(1);
        }
    }
}
