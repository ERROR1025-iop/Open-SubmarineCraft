using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace{ public class TurretShell : LargeBlock
    {
        int shellCount;
        int startSprite;

        public TurretShell(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("turretShell", "weapon");

            thumbnailColor = new Color(0.4686f, 0.4686f, 0.4686f);
            transmissivity = 3.2f;
            density = 18.0f;

            shellCount = 12;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            TurretShell block = new TurretShell(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1430, 1320);
            block.initLargeBlock(new IPoint(2, 1));
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.fineSteel.getId(), 2 },
                    { blocksManager.sulfurPowder.getId(), 5 },
                    { blocksManager.coalPowder.getId(), 5 }                    
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 2);

            return syntInfos;
        }

        public override bool giveOneShell(BlocksEngine blocksEngine, Block taker)
        {
            if (isOrigin() == false)
            {
                return false;
            }

            if (shellCount > 0)
            {
                shellCount--;

                if (shellCount == 0)
                {
                    setTorpedpTexture(blocksEngine, 1);
                }
                return true;
            }
            return false;
        }

        protected override void onLargeBlockInitFinish()
        {
            IPoint startOffset = getOffset();
            startSprite = ((size.y - 1) - startOffset.y) * size.x + startOffset.x;
        }

        protected void setTorpedpTexture(BlocksEngine blocksEngine, int count)
        {
            for (int offsetx = 0; offsetx < size.x; offsetx++)
            {
                for (int offsety = 0; offsety < size.y; offsety++)
                {
                    IPoint obOffset = new IPoint(offsetx, offsety);
                    TurretShell offsetBlock = blocksEngine.getBlock(getOriginPoint() + obOffset) as TurretShell;
                    if (offsetBlock == null)
                    {
                        continue;
                    }

                    offsetBlock.setSpriteRect(offsetBlock.startSprite + count * blockCount);
                    offsetBlock.setDensity(6 + count);
                }
            }
        }

        public override int getSyntIconSpriteIndex()
        {
            return 0;
        }
    }
}
