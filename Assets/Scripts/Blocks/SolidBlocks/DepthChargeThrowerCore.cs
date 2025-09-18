using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class DepthChargeThrowerCore : TurretCore
    { 

        public DepthChargeThrowerCore(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("depthChargeThrowerCore", "weapon");

            thumbnailColor = new Color(0.3686f, 0.3686f, 0.3686f);
            transmissivity = 3.2f;
            density = 12.3f;        
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            DepthChargeThrowerCore block = new DepthChargeThrowerCore(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1430, 1320);
            block.initLargeBlock(new IPoint(3, 3));
            return block;
        }

        protected override bool takeShellMethod()
        {
            return Pooler.instance.takeOneDepthCharge(this);
        }

        public override int isCanSettingValue()
        {
            return 2;
        }

        public override int[] getSettingValueRank()
        {
            return new int[2] { 0, 3200 };
        }

        public override string getSettingValueName()
        {
            return "explosion depth";
        }
    }
}
