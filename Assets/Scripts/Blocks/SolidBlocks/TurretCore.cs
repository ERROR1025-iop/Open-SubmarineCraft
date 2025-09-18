using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace{ public class TurretCore : LargeBlock
    {
        int readyCycle;
        bool m_isReady;
        bool m_isFire;
        int readyingTick;
        bool isNoShell;
        int fireShellsCount;

        protected IPoint startOffset;
        protected int startBlockCount;
        protected int startSprite;
        protected int indexOfSprite;

        float predictorData;

        public TurretCore(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("turretCore", "weapon");

            thumbnailColor = new Color(0.3686f, 0.3686f, 0.3686f);
            transmissivity = 3.2f;
            density = 12.3f;

            m_isReady = false;
            m_isFire = false;
            readyingTick = 0;
            readyCycle = 20;
            isNoShell = false;
            fireShellsCount = 1;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            TurretCore block = new TurretCore(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1430, 1320);
            block.initLargeBlock(new IPoint(3, 3));
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[3, 2] {
                    { blocksManager.fineSteel.getId(), 9 },
                    { blocksManager.smallElectorEngine.getId(), 6 },
                     { blocksManager.circuitBoard.getId(), 6 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 9);

            return syntInfos;
        }

        public override int getSyntIconSpriteIndex()
        {
            return 18;
        }

        protected override void onLargeBlockInitFinish()
        {
            startBlockCount = size.x * size.y;
            startOffset = getOffset();
            startSprite = ((size.y - 1) - startOffset.y) * size.x + startOffset.x;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);
            if (isOrigin())
            {
                turretReadyCycleRule();
            }
            else
            {
                changeTexture(getIndexOfSprite());
            }
        }
        
        public void setFireShellCount(int count)
        {
            if (isOrigin())
            {
                fireShellsCount = count;
            }
            else
            {
                TurretCore block = getOrgBlock() as TurretCore;
                if (block != null)
                {
                    block.setFireShellCount(count);
                }
            }            
        }

        public void setReadyingTime(int time)
        {
            if (isOrigin())
            {
                readyCycle = time;
            }
            else
            {
                TurretCore block = getOrgBlock() as TurretCore;
                if (block != null)
                {
                    block.setReadyingTime(time);
                }
            }
        }

        public bool fire()
        {
            if (isOrigin())
            {
                if (isReady())
                {
                    isNoShell = false;
                    m_isReady = false;
                    m_isFire = true;
                    return true;
                }
                return false;
            }
            else
            {
                TurretCore block = getOrgBlock() as TurretCore;
                if (block != null)
                {
                    return block.fire();
                }
            }
            return false;
        }

        public float getPredictorData()
        {
            if (isOrigin())
            {
                return predictorData;
            }
            else
            {
                TurretCore block = getOrgBlock() as TurretCore;
                if (block != null)
                {
                    return block.getPredictorData();
                }
            }
            return predictorData;          
        }

        protected void turretReadyCycleRule()
        {
            if (!isReady() && !isNoShell)
            {
                if (readyingTick < readyCycle)
                {
                    readyingTick++;
                    changeTexture(1);
                }
                else
                {
                    bool isTakeShell = false;
                    for(int i=0; i< fireShellsCount; i++)
                    {
                        if (takeShellMethod())
                        {
                            isTakeShell = true;
                        }
                    }

                    if (isTakeShell)
                    {
                        setIsReady(true);
                        changeTexture(0);
                    }
                    else
                    {
                        isNoShell = true;
                    }
                    readyingTick = 0;
                }
            }
        }

        protected virtual bool takeShellMethod()
        {
            return Pooler.instance.takeOneShell(this);
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
                TurretCore block = getOrgBlock() as TurretCore;
                if (block != null)
                {
                    return block.getIndexOfSprite();
                }
            }
            return 0;
        }

        public bool isReady()
        {
            if (isOrigin())
            {
                return m_isReady;
            }
            else
            {
                TurretCore block = getOrgBlock() as TurretCore;
                if (block != null)
                {
                    return block.isReady();
                }
            }
            return false;
        }

        public void setIsReady(bool b)
        {
            if (isOrigin())
            {
                m_isReady = b;
            }
            else
            {
                TurretCore block = getOrgBlock() as TurretCore;
                if (block != null)
                {
                    block.setIsReady(b);
                }
            }
        }

        public override void onReciverWe(float voltage, int putterDir, Block putter)
        {
            base.onReciverWe(voltage, putterDir, putter);

            if (isOrigin())
            {
                predictorData = (int)voltage;
            }
            else
            {
                TurretCore block = getOrgBlock() as TurretCore;
                if (block != null)
                {
                    block.onReciverWe(voltage, putterDir, putter);
                }
            }
        }
    }
}
