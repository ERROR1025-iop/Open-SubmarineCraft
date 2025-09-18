using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scraft.BlockSpace
{
    public class MineralDetector : SolidBlock
    {
        float enrichment;
        SmallArea stayArea;
        float output;

        public MineralDetector(int id, GameObject parentObject, GameObject blockObject)
                : base(id, parentObject, blockObject)
        {
            initBlock("mineralDetector", "sensor");
            isCanChangeRedAndCrackTexture = false;
            thumbnailColor = new Color(0.902f, 0.902f, 0.902f);
            transmissivity = 4.2f;
            density = 10.85f;          
            max_storeAir = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            MineralDetector block = new MineralDetector(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1450, 1500);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.steel.getId(), 1 },
                     { blocksManager.circuitBoard.getId(), 4 },
                    { blocksManager.semiconductor.getId(), 6 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            mineralDetectorRule(blocksEngine);
        }

        void mineralDetectorRule(BlocksEngine blocksEngine)
        {

            if(AreaManager.staySmallArea != null)
            {
                stayArea = AreaManager.staySmallArea;
            }
            else if(AreaManager.stayArea != null)
            {
                stayArea = AreaManager.stayArea;
            }

            if(stayArea != null)
            {
                enrichment = stayArea.enrichment;
                setSpriteRect((int)(Mathf.Clamp01(enrichment) * 6.99999f));

                output = enrichment * 100;
                putWe(blocksEngine, Dir.up, output);
                putWe(blocksEngine, Dir.right, output);
                putWe(blocksEngine, Dir.down, output);
                putWe(blocksEngine, Dir.left, output);
            }
        }

        private void putWe(BlocksEngine blocksEngine, int dir, float voltage)
        {
            blocksEngine.putWe(this, getCoor().getDirPoint(dir), voltage);
        }

        public override float getRomaoteMe()
        {
            return output;
        }
    }
}
