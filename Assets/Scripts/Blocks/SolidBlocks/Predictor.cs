using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scraft.DpartSpace;

namespace Scraft.BlockSpace
{
    public class Predictor : SolidBlock
    {

        protected bool isWork;
        protected float comsume;

        Block advSonar;
  
        float predictorData;

        int lightPoint;
        int twinkleCycle;
        int twinkleTick;

        public Predictor(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("predictor", "signal");

            thumbnailColor = IUtils.HexToColor("006155");
            density = 7.85f;
            transmissivity = 7.2f;
            isCanChangeRedAndCrackTexture = false;

            comsume = 1.0f;
            twinkleCycle = 2;
            twinkleTick = 0;   
            lightPoint = 0;

            predictorData = 0;    

        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Predictor block = new Predictor(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1750);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.circuitBoard.getId(), 6 },
                    { blocksManager.steel.getId(), 1 },
                    { blocksManager.semiconductor.getId(), 6 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            predictorRule(blocksEngine);
            textureTwinkleRule();
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            advSonar = BlocksManager.instance.advSonar;
        }

        void predictorRule(BlocksEngine blocksEngine)
        {
            float receive = Pooler.instance.requireElectric(this, comsume);
            if (receive > comsume * 0.9f)
            {
                if (checkNeighborBlockEqualMethod() && Icon3D.showMode != 0)
                {
                    isWork = true;
                    predictorData = Pooler.shotPredictorData;

                    putWe(blocksEngine, Dir.up, predictorData);
                    putWe(blocksEngine, Dir.right, predictorData);
                    putWe(blocksEngine, Dir.down, predictorData);
                    putWe(blocksEngine, Dir.left, predictorData);
                }
                else
                {
                    isWork = false;
                }       
            }
            else
            {
                isWork = false;
            }
        }

        private void putWe(BlocksEngine blocksEngine, int dir, float voltage)
        {
            blocksEngine.putWe(this, getCoor().getDirPoint(dir), voltage);
        }

        public override float getRomaoteMe()
        {
            return predictorData;
        }

        bool checkNeighborBlockEqualMethod()
        {            
            if (getNeighborBlock(Dir.left).equalBlock(advSonar))
            {
                return true;
            }
            if (getNeighborBlock(Dir.right).equalBlock(advSonar))
            {
                return true;
            }
            if (getNeighborBlock(Dir.up).equalBlock(advSonar))
            {
                return true;
            }
            if (getNeighborBlock(Dir.down).equalBlock(advSonar))
            {
                return true;
            }
            return false;
        }

        void textureTwinkleRule()
        {
            if (isWork)
            {
                if(twinkleTick < twinkleCycle)
                {
                    twinkleTick++;
                }
                else
                {                    
                    setSpriteRect(lightPoint);
                    twinkleTick = 0;
                    lightPoint++;
                    if(lightPoint > 3)
                    {
                        lightPoint = 0;
                    }
                }
            }
            else
            {
                setSpriteRect(1);
            }
        }   
    }
}
