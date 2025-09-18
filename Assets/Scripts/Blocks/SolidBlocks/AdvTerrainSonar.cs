using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class AdvTerrainSonar : LargeBlock
    {
        protected bool isWork;
        protected float comsume;
        protected IPoint startOffset;
        protected int startBlockCount;
        protected int startSprite;
        protected int indexOfSprite;
        protected bool openAdvTerrainSonar;

        public AdvTerrainSonar(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("advTerrainSonar", "sensor");

            thumbnailColor = IUtils.HexToColor("353535");
            density = 7.1f;
            comsume = 10.0f;
            openAdvTerrainSonar = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            AdvTerrainSonar block = new AdvTerrainSonar(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1400);
            block.initLargeBlock(new IPoint(5, 1));
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[5, 2] {
                    { blocksManager.steel.getId(), 4 },
                    { blocksManager.fineSteel.getId(), 1 },
                     { blocksManager.terrainSonar.getId(), 1 },
                     { blocksManager.circuitBoard.getId(), 2 },
                    { blocksManager.semiconductor.getId(), 2 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 5);

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
                isWork = !isWork;
            }
            else
            {
                AdvTerrainSonar block = getOrgBlock() as AdvTerrainSonar;
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
            return 10;
        }

        public override void onWorldModeDestroy()
        {
            base.onWorldModeDestroy();
            if (isOrigin())
            {
                openAdvTerrainSonar = false;
                Pooler.instance.setAdvTerrainSonarOpen(openAdvTerrainSonar);
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
                    openAdvTerrainSonar = true;
                }
                else
                {
                    openAdvTerrainSonar = false;
                }
            }
            else
            {
                openAdvTerrainSonar = false;
            }
            Pooler.instance.setAdvTerrainSonarOpen(openAdvTerrainSonar);
            changeTexture(openAdvTerrainSonar ? 1 : 0);
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
                AdvTerrainSonar block = getOrgBlock() as AdvTerrainSonar;
                if (block != null)
                {
                    return block.getIndexOfSprite();
                }
            }
            return 0;
        }
    }
}