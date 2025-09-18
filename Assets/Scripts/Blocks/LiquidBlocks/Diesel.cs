using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Diesel : LiquidBlock
    {

        float burningPoint;

        public Diesel(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("diesel", "material");

            thumbnailColor = new Color(0.568f, 0.482f, 0.2588f);            
            density = 0.85f;
            transmissivity = 0.61f;
            burningPoint = 220.0f;
            calorific = 2604.0f;
            canStoreInTag = 2;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Diesel block = new Diesel(blockId, parentObject, blockObject);
            block.initLiquidBlock(blocksManager, 180, 4, "dieselMushy", "null", 10);
            return block;
        }

        public override string getBasicInformation()
        {
            return base.getBasicInformation()
                + "," + ILang.get("calorific", "menu") + ":" + calorific;
        }

    }
}