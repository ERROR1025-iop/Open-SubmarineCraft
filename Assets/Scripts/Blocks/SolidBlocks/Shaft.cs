using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Shaft : RotationBlock
    {

        public Shaft(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("shaft", "machine");

            thumbnailColor = new Color(0.4286f, 0.4286f, 0.4286f);
            density = 13.1f;
            max_storeAir = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Shaft block = new Shaft(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1400);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[1, 2] {
                    { blocksManager.steel.getId(), 1 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void onRotateButtonClick()
        {
            setDir(dir == Dir.up ? Dir.right : Dir.up);
            lastPlaceDir = dir;
        }

        public override void onBuilderModeCreated()
        {
            base.onBuilderModeCreated();
            if (dir > 1)
            {
                setDir(dir - 2);
            }            
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
            BlocksEngine.instance.putMe(this, getCoor().getDirPoint(putterDir + 2), me);
        }

        public override bool isCanReceiveMe(Block putter)
        {
            IPoint left = getRelativeDirPoint(Dir.left);
            IPoint right = getRelativeDirPoint(Dir.right);
            if (putter.getCoor().equal(left) || putter.getCoor().equal(right))
            {
                return true;
            }
            return false;
        }

        public override bool isRootUnlock()
        {
            return true;
        }
    }
}
