using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class DrillCore : RotationBlock
    {
        static public List<DrillCore> drillCores;

        public float speed;
        public bool m_isAttachDrillRS;

        public DrillCore(int id, GameObject parentObject, GameObject blockObject)
                : base(id, parentObject, blockObject)
        {
            initBlock("drillCore", "machine");

            thumbnailColor = new Color(0.4286f, 0.4286f, 0.4286f);
            density = 13.1f;
            m_isAttachDrillRS = false;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            DrillCore block = new DrillCore(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1400);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[2, 2] {                   
                    { blocksManager.fineSteel.getId(), 1 },
                    { blocksManager.shaft.getId(), 1 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void onRegister()
        {
            base.onRegister();

            if (drillCores == null)
            {
                drillCores = new List<DrillCore>();
            }
            else
            {
                drillCores.Clear();
            }
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            setSpriteRect(dir > 1 ? 1 : 0);
            drillCores.Add(this);
        }   

        public override void onRotateButtonClick()
        {
            base.onRotateButtonClick();
            setSpriteRect(dir > 1 ? 1 : 0);
        }

        protected override void setDir(int dir)
        {
            base.setDir(dir);
            setSpriteRect(dir > 1 ? 1 : 0);
        }

        public override void onWorldModeDestroy()
        {
            base.onWorldModeDestroy();      
        }

        public override void onReciverMe(float me, int putterDir, Block putter)
        {
            speed = me;
        }

        public void onAttachDrillRS()
        {
            m_isAttachDrillRS = true;
        }

        public bool isAttachDrillRS()
        {
            return m_isAttachDrillRS;
        }

        public float getSpeed()
        {
            return speed / 100;
        }

        public override bool isCanReceiveMe()
        {
            return true;
        }
    }
}
