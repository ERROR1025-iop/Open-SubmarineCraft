using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class GearSet : SolidBlock
    {
        bool isReceiveMe;
        int ConnectCount;

        int updataFrameDelayPerUnit;
        int updataFrameDelayStack;
        int updataFrameStack;

        float speed;
        float targetSpeed;

        public GearSet(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("gearSet", "machine");

            thumbnailColor = new Color(0.4286f, 0.4286f, 0.4286f);
            density = 13.1f;
            isReceiveMe = false;
            ConnectCount = 1;

            updataFrameStack = 0;
            updataFrameDelayPerUnit = 30000;
            updataFrameDelayStack = 0;

            isCanChangeRedAndCrackTexture = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            GearSet block = new GearSet(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1400);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[1, 2] {
                    { blocksManager.steel.getId(), 3 }                    
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 2);

            return syntInfos;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            gearSetRule();
            isReceiveMe = false;
        }

        void gearSetRule()
        {
            speed = Mathf.Lerp(speed, targetSpeed, 0.02f);
            float AbsSpeed = Mathf.Abs(speed);

            if (AbsSpeed > 300)
            {
                updataFrameDelayPerUnit = 0;
            }
            else if (AbsSpeed < 1)
            {
                updataFrameDelayPerUnit = 30000;
            }
            else if (AbsSpeed > 1)
            {
                updataFrameDelayPerUnit = (int)(300 / AbsSpeed);
            }
        }

        public override bool frameUpdate()
        {
            if (updataFrameDelayStack < updataFrameDelayPerUnit)
            {
                updataFrameDelayStack++;
                return true;
            }
            updataFrameStack = updataFrameStack > 1 ? 0 : updataFrameStack;
            setSpriteRect(updataFrameStack);

            updataFrameStack++;
            updataFrameDelayStack = 0;
            return true;
        }        

        public override void onWorldModeDestroy()
        {
            base.onWorldModeDestroy();

            BlocksEngine.instance.putMe(this, getCoor().getDirPoint(Dir.up), 0);
            BlocksEngine.instance.putMe(this, getCoor().getDirPoint(Dir.right), 0);
            BlocksEngine.instance.putMe(this, getCoor().getDirPoint(Dir.down), 0);
            BlocksEngine.instance.putMe(this, getCoor().getDirPoint(Dir.left), 0);
        }

        public override void onReciverMe(float me, int putterDir, Block putter)
        {
            targetSpeed = me;
            if (!isReceiveMe)
            {
                if (ConnectCount > 0)
                {
                    me = me / ConnectCount;
                    ConnectCount = 0;
                    putMeMethod(me, Dir.left, putterDir);
                    putMeMethod(me, Dir.right, putterDir);
                    putMeMethod(me, Dir.up, putterDir);
                    putMeMethod(me, Dir.down, putterDir);
                }
                isReceiveMe = true;
            }
        }

        public void putMeMethod(float me, int outputDir, int putterDir)
        {
            if (outputDir != putterDir)
            {
                Block block = BlocksEngine.instance.getBlock(getCoor().getDirPoint(outputDir));
                if (block.isCanReceiveMe())
                {
                    BlocksEngine.instance.putMe(this, block.getCoor(), me);
                    ConnectCount++;
                }
            }
        }

        public override bool isCanReceiveMe()
        {
            return true;
        }
      
    }
}
