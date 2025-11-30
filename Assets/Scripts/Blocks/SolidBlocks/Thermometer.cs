using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scraft.BlockSpace
{
    public class Thermometer : SolidBlock
    {
        float enrichment;
        SmallArea stayArea;

        public Thermometer(int id, GameObject parentObject, GameObject blockObject)
                : base(id, parentObject, blockObject)
        {
            initBlock("thermometer", "other");
            isCanChangeRedAndCrackTexture = false;
            thumbnailColor = new Color(0.902f, 0.902f, 0.902f);
            transmissivity = 4.2f;
            density = 10.85f;
            max_storeAir = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Thermometer block = new Thermometer(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1450, 1500);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[2, 2] {
                    { blocksManager.steel.getId(), 1 },
                     { blocksManager.distilledWater.getId(), 1 }                   
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            thermometerRule(blocksEngine);
        }

        void thermometerRule(BlocksEngine blocksEngine)
        {
            float t = getTemperature();
            int index = (int)(Mathf.Clamp(t, 0, 100) / 14.3f);
            setSpriteRect(index);           
        }

        public override bool isCanCollectScientific()
        {
            return true;
        }

        public override void onCollectScientific(CollectedScientificInfo csInfo)
        {
            base.onCollectScientific(csInfo);
            collectedScientific = csInfo.point * 1.5f;
        }
    }
}
