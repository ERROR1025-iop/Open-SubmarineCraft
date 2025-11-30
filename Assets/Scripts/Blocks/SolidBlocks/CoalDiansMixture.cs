using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class CoalDiansMixture : Coal
    {



        public CoalDiansMixture(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("coalDiansMixture", "material");
            thumbnailColor = new Color(0.1215f, 0.1215f, 0.1215f);

            transmissivity = 0.32f;
            calorific = 1720;
            unityCalorific = 30;
            burningPoint = 330;
            density = 1.02f;
            burningAir = 500;
            max_storeAir = 0;
            penetrationRate = 0.8f;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            CoalDiansMixture block = new CoalDiansMixture(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "null", 3300, 770);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[2, 2] {
                    { blocksManager.coalPowder.getId(), 2 },            
                    { blocksManager.dinas.getId(), 3 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 4);

            return syntInfos;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            if (!isNeedDelete())
            {
                dinasMoveRule(blocksEngine);
                if (dinasReduceRule(blocksEngine)) return;
            }

        }

        protected virtual bool dinasReduceRule(BlocksEngine blocksEngine)
        {
            if (temperature > 400)
            {
                if (noOxygenBurningTime > carbonizationMaxTime)
                {
                    if(temperature > 1000)
                    {
                        blocksEngine.createBlock(getCoor(), BlocksManager.instance.siliconCarbide, true);
                    }
                    else
                    {
                        blocksEngine.createBlock(getCoor(), BlocksManager.instance.silicon, true);
                    }                    
                }
                else
                {
                    noOxygenBurningTime++;
                }
            }
            return false;
        }      


        public override bool isRootUnlock()
        {
            return false;
        }        
    }
}