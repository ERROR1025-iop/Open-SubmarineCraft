using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace{ public class TerrainSonar : SolidBlock
    {
        
        protected int terrainPosYPropertyToID;
        float distance;


        public TerrainSonar(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("terrainSonar", "sensor");
            isCanChangeRedAndCrackTexture = false;
            thumbnailColor = new Color(0.0f, 0.4705f, 0.8352f);
            density = 15.4f;
            transmissivity = 2.85f;          
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            TerrainSonar block = new TerrainSonar(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1535, 1550);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.circuitBoard.getId(), 4 },
                    { blocksManager.steel.getId(), 1 },
                    { blocksManager.semiconductor.getId(), 2 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void onWorldModeDestroy()
        {
            base.onWorldModeDestroy();
            Pooler.isOpen_TerrainSonar = false;
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            terrainPosYPropertyToID = Shader.PropertyToID("_terrainPosY");
        }       

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            terrainSonarRule(blocksEngine);
        }

        protected void terrainSonarRule(BlocksEngine blocksEngine)
        {
            setSpriteRect(1);
            getTerrainHeightMethod(blocksEngine);
            Pooler.isOpen_TerrainSonar = true;

            int index = Mathf.Clamp( (int)(distance / 700), 0, 7);
            setSpriteRect(index);
        }

        void getTerrainHeightMethod(BlocksEngine blocksEngine)
        {
            float terrainHeight = Pooler.terrainPosY;
            //Debug.Log(terrainHeight);

            float mapHeight = -terrainHeight * 10;
            distance = mapHeight - MainSubmarine.deep;

            putWe(blocksEngine, Dir.up, distance);
            putWe(blocksEngine, Dir.right, distance);
            putWe(blocksEngine, Dir.down, distance);
            putWe(blocksEngine, Dir.left, distance);
        }

        private void putWe(BlocksEngine blocksEngine, int dir, float voltage)
        {
            blocksEngine.putWe(this, getCoor().getDirPoint(dir), voltage);
        }

        public override float getRomaoteMe()
        {
            return distance;
        }
    }
}
