using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class BlocksEngine
    {

        public static BlocksEngine instance;

        public IPoint mapSize;
        public GameObject blocksParentObject;

        BlocksManager blocksManager;

        public Block[,] blocksMap;

        const int MAX_RELEASE_BLOCK_COUNT = 10000;
        int unUsedObjectStack;
        GameObject[] unUsedObjectArr;
        Vector3 unVisVertor;
        int updataLimitX;
        int threadUpdataLimitX;

        public BlocksEngine(GameObject blocksParentObject, IPoint mapSize)
        {
            instance = this;

            blocksManager = BlocksManager.instance;

            this.blocksParentObject = blocksParentObject;
            this.mapSize = mapSize;

            blocksMap = new Block[mapSize.x, mapSize.y];

            unUsedObjectArr = new GameObject[MAX_RELEASE_BLOCK_COUNT];
            unUsedObjectStack = 0;
            unVisVertor = new Vector3(0, 9999, 0);
        }

        /// <summary>
        /// 设置方块
        /// </summary>
        public void setBlock(IPoint coor, Block block)
        {
            /*if (coor.x > World.mapSize.x || coor.y > World.mapSize.y)
            {
                Debug.Log("out rang:" + coor.toString());
            }
            if (coor.x < 0 || coor.y < 0)
            {
                Debug.Log("out rang:" + coor.toString());
            }*/
            blocksMap[coor.x, coor.y] = block;
        }

        /// <summary>
        /// 获取方块
        /// </summary>
        public Block getBlock(int x, int y)
        {
            //isOutRang(x, y);
            return blocksMap[x, y];
        }

        /// <summary>
        /// 获取方块
        /// </summary>
        public Block getBlock(IPoint coor)
        {
            //isOutRang(coor);
            return blocksMap[coor.x, coor.y];
        }

        /// <summary>
        /// 获取方块并检查是否越界
        /// </summary>
        public Block getBlockWithCheck(IPoint coor)
        {
            if (!isOutRang(coor))
            {
                return blocksMap[coor.x, coor.y];
            }
            return null;
        }

        /// <summary>
        /// 是否超过数组界限
        /// </summary>
        public bool isOutRang(int x, int y)
        {
            if (x >= World.mapSize.x || y >= World.mapSize.y)
            {
                Debug.Log("out rang:(" + x + "," + y + ")");
                return true;
            }
            if (x < 0 || y < 0)
            {
                Debug.Log("out rang:(" + x + "," + y + ")");
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否超过数组界限
        /// </summary>
        public bool isOutRang(IPoint coor)
        {
            if(coor != null)
            {
                return isOutRang(coor.x, coor.y);
            }
            return true;
        }

        /// <summary>
        /// 创建（放置）方块
        /// </summary>    
        public Block createBlockBase(IPoint createCoor, Block blockStatic, bool clearUp)
        {
            Block oldBlock = getBlock(createCoor);
            if (clearUp)
            {
                if (oldBlock != null)
                {
                    addBlockToUnusedPool(oldBlock);
                }
            }
            else
            {
                oldBlock.setBlockVisible(false);
            }

            Block newBlock = blockStatic.clone(blocksParentObject, blocksManager, getUnusedBlockObject(), createCoor, mapSize);            
            setBlock(createCoor, newBlock);
            if (oldBlock != null)
            {
                newBlock.inheritAir(oldBlock.getStoreAir());
            }

            return newBlock;
        }


        /// <summary>
        /// 创建压缩方块
        /// </summary>   
        public Block createBlockWithNotPlace(Block blockStatic, Block creator)
        {
            Block newBlock = blockStatic.clone(blocksParentObject, blocksManager, getUnusedBlockObject(), creator.getCoor(), mapSize);
            //计算newBlock在creator方块的温度下需要的热量，然后给予newBlock该热量，然后creator减去该热量lock (this)
            PreBlockTemperature.SeparationTemperatureCalculation(creator, newBlock);
            newBlock.setPress(creator.getPress());
            newBlock.setBlockVisible(false);
            return newBlock;
        }

        /// <summary>
        /// 创建压缩方块
        /// </summary>   
        public Block createCompressBlock(Block blockStatic, Block toBlock)
        {
            if (toBlock.isFluid())
            {
                Block newBlock = createBlockWithNotPlace(blockStatic, toBlock);
                if (toBlock.equalPState(PState.liquild) || toBlock.equalPState(PState.mushy))
                {
                    (toBlock as LiquidBlock).addCompressChild(newBlock);
                }
                else if (toBlock.equalPState(PState.gas))
                {
                    (toBlock as GasBlock).addCompressChild(newBlock);
                }
                return newBlock;
            }
            return null;
        }


        /// <summary>
        /// 创建（放置）方块
        /// </summary>    
        public Block createBlock(IPoint createCoor, Block blockStatic,
            bool autoAdjustHeatCapacity = false)
        {
            Block oldBlock = getBlock(createCoor);
            Block newBlock = createBlockBase(createCoor, blockStatic, true);
            if (oldBlock != null)
            {
                newBlock.inheritHeatQuantity(oldBlock);
                if (autoAdjustHeatCapacity)
                {
                    newBlock.heatCapacity = oldBlock.density * oldBlock.heatCapacity / newBlock.density;
                }
            }
            return newBlock;
        }

        /// <summary>
        /// 创建（放置）方块
        /// </summary>    
        public Block createBlock(IPoint createCoor, Block blockStatic, float press,
            bool autoAdjustHeatCapacity = false)
        {
            Block new_block = createBlock(createCoor, blockStatic, autoAdjustHeatCapacity);
            new_block.setPress(press);
            return new_block;
        }

        /// <summary>
        /// 放置方块(block为已经实例化过的方块)
        /// </summary>  
        public void placeBlock(IPoint placeCoor, Block block)
        {
            Block old_block = getBlock(placeCoor);
            if (old_block != null)
            {
                addBlockToUnusedPool(old_block);
            }
            block.setBlockVisible(true);
            block.setPosition(placeCoor);
            setBlock(placeCoor, block);
        }

        /// <summary>
        /// 销毁方块
        /// </summary>    
        public Block removeBlock(IPoint removeCoor, bool clearUp)
        {          
            return removeBlock(removeCoor, clearUp, 0);
        }

        /// <summary>
        /// 销毁方块
        /// </summary>    
        public Block removeBlock(IPoint removeCoor, bool clearUp, float press)
        {
            Block old_block = getBlock(removeCoor);
            Block new_block = createBlockBase(removeCoor, blocksManager.air, clearUp);
            new_block.setTemperature(old_block.getTemperature());
            new_block.setPress(press);
            return new_block;
        }

        /// <summary>
        /// 销毁方块
        /// </summary>    
        public Block removeBlock(IPoint removeCoor)
        {
            return removeBlock(removeCoor, true);
        }

        void addBlockToUnusedPool(Block block)
        {
            if (World.GameMode == World.GameMode_Freedom)
            {
                block.onWorldModeDestroy();
            }

            GameObject blockObject = block.getBlockObject();
            if (blockObject != null)
            {
                if (unUsedObjectStack < MAX_RELEASE_BLOCK_COUNT)
                {
                    blockObject.transform.localPosition = unVisVertor;
                    unUsedObjectArr[unUsedObjectStack] = blockObject;
                    unUsedObjectStack++;
                    block.clear(false);
                }
                else
                {
                    block.clear(true);
                }
            }
            block = null;
        }

        /// <summary>
        /// 从销毁方块池中获取方块的GameObject
        /// </summary>    
        public GameObject getUnusedBlockObject()
        {
            if (unUsedObjectStack <= 0)
            {
                return null;
            }

            GameObject blockObject = unUsedObjectArr[unUsedObjectStack - 1];
            unUsedObjectStack--;

            if (blockObject != null)
            {
                return blockObject;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 清空销毁方块池
        /// </summary> 
        public void clearUnusedBlocksPool()
        {
            for (int i = 0; i < unUsedObjectStack; i++)
            {
                Object.Destroy(unUsedObjectArr[i]);
                unUsedObjectArr[i] = null;
            }
            unUsedObjectStack = 0;
        }

        /// <summary>
        /// 主更新
        /// </summary>
        public void updata(IRect updataRect, bool isIgnoreBlocks = false)
        {
            for (int x = updataRect.x; x < updataRect.getMaxX(); x++)
                for (int y = updataRect.y; y < updataRect.getMaxY(); y++)
                    if (x % 2 == updataLimitX)
                    {
                        Block block = getBlock(x, y);
                        if (block != null)
                        {
                            block.update(this);
                            Pooler.mass += block.getMass();
                        }
                    }
            updataLimitX = updataLimitX == 0 ? 1 : 0;
        }

        /// <summary>
        /// 线程更新
        /// </summary>
        public void threadUpdata(IRect updataRect)
        {
            for (int x = updataRect.x; x < updataRect.getMaxX(); x++)

                for (int y = updataRect.y; y < updataRect.getMaxY(); y++)
                {
                    if (x % 2 == threadUpdataLimitX)
                    {
                        Block block = getBlock(x, y);
                        if (block != null)
                        {
                            block.threadUpdate(this);
                        }
                    }

                }
            threadUpdataLimitX = threadUpdataLimitX == 0 ? 1 : 0;
        }

        /// <summary>
        /// 输出机械能给指定方块
        /// </summary>
        public void putMe(Block putter, IPoint putToCoor, float me)
        {
            getBlock(putToCoor).onReciverMe(me, putToCoor.relativeDir(putter.getCoor()), putter);
        }

        /// <summary>
        /// 输出弱电能给指定方块
        /// </summary>
        public void putWe(Block putter, IPoint putToCoor, float voltage)
        {
            getBlock(putToCoor).onReciverWe(voltage, putToCoor.relativeDir(putter.getCoor()), putter);
        }

        /// <summary>
        /// 输出电能给世界(废除使用pool.chargeElectric)
        /// </summary>
        [System.Obsolete("use pool.chargeElectric")]
        public void chargeElectric(Block generator, float charge)
        {
            //electricManager.chargeElectric(charge);
        }

        /// <summary>
        /// 向世界要电，返回缺额(废除使用pool.requireElectric)
        /// </summary>
        [System.Obsolete("use pool.requireElectric")]
        public float requireElectric(Block taker, float require)
        {
            //return electricManager.requireElectric(require);
            return 0;
        }

        /// <summary>
        /// wifi
        /// </summary>
        /*public void addWifiBlocks(Wifi block)
        {
            wifiManager.addWifiBlocks(block);
        }

        public void removeWifiBlocks(Wifi block)
        {
            wifiManager.removeWifiBlocks(block);
        }

        public List<Wifi> getWifiArr()
        {
            return wifiManager.getWifiArr();
        }*/

        public BlocksManager getBlocksManager()
        {
            return blocksManager;
        }
    }
}