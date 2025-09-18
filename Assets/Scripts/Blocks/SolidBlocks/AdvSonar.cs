using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class AdvSonar : LargeBlock
    {

        protected bool isWork;
        protected float comsume;
        protected IPoint startOffset;
        protected int startBlockCount;
        protected int startSprite;
        protected int indexOfSprite;
        protected int iconShowMode;

        public AdvSonar(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("advSonar", "sensor");

            thumbnailColor = IUtils.HexToColor("00BCFF");
            density = 7.1f;
            comsume = 10.0f;
            iconShowMode = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            AdvSonar block = new AdvSonar(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1400);
            block.initLargeBlock(new IPoint(3, 3));
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[4, 2] {
                    { blocksManager.steel.getId(), 8 },
                    { blocksManager.fineSteel.getId(), 1 },
                     { blocksManager.circuitBoard.getId(), 4 },
                    { blocksManager.semiconductor.getId(), 4 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 9);

            return syntInfos;
        }

        protected override void onLargeBlockInitFinish()
        {
            startBlockCount = size.x * size.y;
            startOffset = getOffset();
            startSprite = ((size.y - 1) - startOffset.y) * size.x + startOffset.x;
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            if (getCurrentBindId() == 6)
            {
                isWork = true;
            }
        }

        public override void onPreesButtonClick(bool isClick)
        {
            if (isClick)
            {
                setWork(!isWork);
            }
        }

        public override void onWorldModeClick()
        {
            base.onWorldModeClick();
            setWork(!isWork);
        }

        void setWork(bool work)
        {
            if (isOrigin())
            {
                if (iconShowMode > 2)
                {
                    iconShowMode = 0;
                }
                else
                {
                    iconShowMode++;
                }
                isWork = iconShowMode > 0;
            }
            else
            {
                AdvSonar block = getOrgBlock() as AdvSonar;
                if (block != null)
                {
                    block.onWorldModeClick();
                }
            }
        }

        

        public override bool isCanBind()
        {
            return true;
        }

        public override int[] getBindArr()
        {
            return new int[3] { 4, 5, 6 };
        }

        public override int getSyntIconSpriteIndex()
        {
            return 18;
        }

        public override void onWorldModeDestroy()
        {
            base.onWorldModeDestroy();
            if (isOrigin())
            {
                Icon3D.showMode = 0;
                iconShowMode = 0;
                isWork = false;
            }
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            if (isOrigin())
            {
                advSonarRule();
            }
            else
            {
                changeTexture(getIndexOfSprite());
            }

        }

        void advSonarRule()
        {
            if (isWork)
            {
                float receive = Pooler.instance.requireElectric(this, comsume);
                if (receive > comsume * 0.9f)
                {
                    Icon3D.showMode = iconShowMode;
                }
                else
                {
                    Icon3D.showMode = 0;
                }
            }
            else
            {
                Icon3D.showMode = 0;
            }
            changeTexture(Icon3D.showMode > 0 ? 1 : 0);
        }

        void changeTexture(int index)
        {
            setSpriteRect(startSprite + index * startBlockCount);
            indexOfSprite = index;
        }

        public int getIndexOfSprite()
        {
            if (isOrigin())
            {
                return indexOfSprite;
            }
            else
            {
                AdvSonar block = getOrgBlock() as AdvSonar;
                if (block != null)
                {
                    return block.getIndexOfSprite();
                }
            }
            return 0;
        }
    }
}