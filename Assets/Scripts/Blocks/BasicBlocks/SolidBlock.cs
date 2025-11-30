using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft.BlockSpace
{
    public class SolidBlock : Block
    {

        protected float meltingPoint;
        protected float maxPress;
        protected float outsidePress;

        protected Block liquidBlockStatic;

        bool isTurnRed;
        bool isCrack;

        protected int currentBindId;
        protected int currentSettingValue;

        protected bool isCanChangeRedAndCrackTexture;

        protected float collectedScientific;
        public int collectedScientificId;
        protected CollectedScientificInfo m_csInfo;

        protected bool isCanbeCorrosion;
        protected float max_corrosion;
        protected float corrosion;

        protected bool isSelecting;


        public SolidBlock(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            pState = PState.solid;
            isTurnRed = false;
            isCrack = false;
            isCanChangeRedAndCrackTexture = true;
            canStoreInTag = 1;

            currentBindId = 0;
            outsidePress = 0;

            collectedScientific = 0;

            max_storeAir = 300;

            isCanbeCorrosion = true;
            max_corrosion = 100;
            penetrationRate = 0.9f;
        }

        public void initSolidBlock(BlocksManager blocksManager, string liquidBlockName, float meltingPoint, float maxPress)
        {
            liquidBlockStatic = blocksManager.getBlockByName(liquidBlockName);
            this.meltingPoint = meltingPoint;
            this.maxPress = maxPress;
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();

            corrosion = max_corrosion;
        }

        public virtual bool onCorrosion(Block corrosiveSolution, Block product, float corrosive)
        {
            if (isCanbeCorrosion)
            {
                corrosion -= corrosive;
                if(corrosion < 0)
                {
                    BlocksEngine.instance.createBlock(getCoor(), product);
                    BlocksEngine.instance.removeBlock(corrosiveSolution.getCoor());
                    return true;
                }
            }
            return false;
        }

        public override void update(BlocksEngine blocksEngine)
        {
            base.update(blocksEngine);

            if (solidMeltRule(blocksEngine)) return;
            if (solidBreakRule(blocksEngine)) return;

            if (isCanChangeRedAndCrackTexture)
            {
                solidRedRule();
                solidCrackRule();
            }            
        }

        public override void threadUpdate(BlocksEngine blocksEngine)
        {
            base.threadUpdate(blocksEngine);

            soildPressRule(blocksEngine);
        }

        protected virtual void dinasMoveRule(BlocksEngine blocksEngine)
        {
            Block down_block = getNeighborBlock(Dir.down);
            if (down_block.isFluid() || down_block.isAir())
            {
                moveTo(down_block.getCoor());
            }
        }

        protected virtual bool solidBreakRule(BlocksEngine blocksEngine)
        {
            if (press > maxPress)
            {
                blocksEngine.removeBlock(getCoor());
                return true;
            }
            return false;
        }

        protected virtual void solidCrackRule()
        {
            if (press > maxPress * 0.9)
            {
                if (isCrack == false)
                {
                    Pooler.instance.playMatelCrashSound();
                }
                changeToCrackTexture();
                isCrack = true;
            }
        }

        protected void soildPressRule(BlocksEngine blocksEngine)
        {
            Block up_block = getNeighborBlock(Dir.up);
            Block right_block = getNeighborBlock(Dir.right);
            Block down_block = getNeighborBlock(Dir.down);
            Block left_block = getNeighborBlock(Dir.left);

            float p1 = 0, p2 = 0;
            float op1 = 0, op2 = 0;

            //===========左右方块===========
            if (right_block.equalPState(PState.solid) && left_block.equalPState(PState.solid))
            {
                p1 = Mathf.Abs(right_block.getOusidePress() - left_block.getOusidePress()) * 0.5f;
                if (p1 > maxPress * 0.8f)
                {
                    p1 = maxPress * 0.8f;
                }
                op1 = Mathf.Max(right_block.getOusidePress(), left_block.getOusidePress()) * 0.8f;
            }
            else if (right_block.equalPState(PState.solid) || left_block.equalPState(PState.solid))
            {
                p1 = Mathf.Abs(right_block.getOusidePress() - left_block.getOusidePress()) * 0.5f;
                if (p1 > maxPress * 0.8f)
                {
                    if (right_block.equalPState(PState.solid))
                    {
                        if (p1 < right_block.getTiPress())
                        {
                            p1 = maxPress * 0.8f;
                        }
                    }
                    else
                    {
                        if (p1 < left_block.getTiPress())
                        {
                            p1 = maxPress * 0.8f;
                        }
                    }
                }
                op1 = right_block.equalPState(PState.solid) ? left_block.getOusidePress() : right_block.getOusidePress();
            }
            else
            {
                p1 = Mathf.Abs(right_block.getTiPress() - left_block.getTiPress());
            }


            //===========上下方块===========
            if (up_block.equalPState(PState.solid) && down_block.equalPState(PState.solid))
            {
                p2 = Mathf.Abs(up_block.getOusidePress() - down_block.getOusidePress()) * 0.5f;
                if (p2 > maxPress * 0.8f)
                {
                    p2 = maxPress * 0.8f;
                }
                op2 = Mathf.Max(up_block.getOusidePress(), down_block.getOusidePress()) * 0.8f;
            }
            else if (up_block.equalPState(PState.solid) || down_block.equalPState(PState.solid))
            {
                p2 = Mathf.Abs(up_block.getOusidePress() - down_block.getOusidePress()) * 0.5f;
                if (p2 > maxPress * 0.8f)
                {
                    if (up_block.equalPState(PState.solid))
                    {
                        if (p2 < up_block.getTiPress())
                        {
                            p2 = maxPress * 0.8f;
                        }
                    }
                    else
                    {
                        if (p2 < down_block.getTiPress())
                        {
                            p2 = maxPress * 0.8f;
                        }
                    }
                }
                op2 = up_block.equalPState(PState.solid) ? down_block.getOusidePress() : up_block.getOusidePress();
            }
            else
            {
                p2 = Mathf.Abs(up_block.getTiPress() - down_block.getTiPress());
            }
            outsidePress = Mathf.Max(op1, op2);
            setPress(Mathf.Max(p1, p2));

        }

        bool solidMeltRule(BlocksEngine blocksEngine)
        {
            if (temperature > meltingPoint)
            {
                if (liquidBlockStatic != null)
                {
                    blocksEngine.createBlock(getCoor(), liquidBlockStatic, press, true);
                }
                else
                {
                    blocksEngine.createBlock(getCoor(), blocksEngine.getBlocksManager().air, press);
                }
                return true;
            }
            return false;
        }

        protected virtual void solidRedRule()
        {

            if (isTurnRed == false && temperature > meltingPoint * 0.9)
            {
                changeToRedTexture();
            }
            else if (isTurnRed == true && temperature < meltingPoint * 0.9)
            {
                isTurnRed = false;

                if (isCrack)
                {
                    changeToCrackTexture();
                }
                else
                {
                    changeToNormalTexture();
                }
            }
        }

        protected virtual void changeToCrackTexture()
        {
            if (isTurnRed)
            {
                setSpriteRect(3);
            }
            else
            {
                setSpriteRect(1);
            }
            isCrack = true;
        }

        protected virtual void changeToNormalTexture()
        {
            setSpriteRect(0);
            isTurnRed = false;
            isCrack = false;
        }

        protected virtual void changeToRedTexture()
        {
            if (isCrack)
            {
                setSpriteRect(3);
            }
            else
            {
                setSpriteRect(2);
            }
            isTurnRed = true;
        }

        public virtual int getCurrentBindId()
        {
            return currentBindId;
        }

        public virtual void setBindId(int id)
        {
            currentBindId = id;
        }

        public virtual void setSettingValue(int v)
        {
            currentSettingValue = v;
            onSettingValueChange();
        }

        public virtual string getSettingValueName()
        {
            return "setting value";
        }

        public virtual int[] getSettingValueRank()
        {
            return new int[2] { -999999, 999999 };
        }

        public virtual int getCurrentSettingValue()
        {
            return currentSettingValue;
        }

        public virtual void onSettingValueChange()
        {

        }

        /// <summary>
        /// 获取可以绑定的按钮
        /// 1:上浮;2:下沉;3:油门杆;4:按钮1;5:按钮2;6:自启动;
        /// </summary>
        public virtual int[] getBindArr()
        {
            return null;
        }

        /// <summary>
        /// 是否可以收集科研点数
        /// </summary>
        public virtual bool isCanCollectScientific()
        {
            return false;
        }

        /// <summary>
        /// 显示进度
        /// </summary>
        public virtual float getProgress()
        {
            return 0;
        }

        public float getCollectedScientific()
        {
            return collectedScientific;
        }

        /// <summary>
        /// 重置所有收集的点数
        /// </summary>
        public void resetCollectedScientific()
        {
            collectedScientific = 0;
            m_csInfo = null;
        }

        /// <summary>
        /// 当收集点数时
        /// </summary>
        public virtual void onCollectScientific(CollectedScientificInfo csInfo)
        {            
            if(m_csInfo != null)
            {
                AreaManager.instance.DecCollectedScientificLayeredByName(m_csInfo.name, m_csInfo.layered, collectedScientificId);
            }
            m_csInfo = csInfo;
        }

        public override string getBasicInformation()
        {
            string info = base.getBasicInformation()
                + "," + ILang.get("melting point", "menu") + ":" + meltingPoint
                + "," + ILang.get("max press", "menu") + ":" + maxPress;

            if (!isLargerBlock())
            {
                info +=  "," + ILang.get("Mass", "menu") + ":" + getMass();
            }

            return info;

        }

        public override float getOusidePress()
        {
            return outsidePress;
        }

        public virtual void onPreesButtonClick(bool isClick)
        {

        }

        public void setCollectedScientificId(int csId)
        {
            collectedScientificId = csId;
        }

        public int getCollectedScientificId()
        {
            SolidBlock solidBlock = BlocksManager.instance.getBlockById(getId()) as SolidBlock;
            if(solidBlock != null)
            {
                return solidBlock.collectedScientificId;
            }
            return 0;
        }

        public virtual void onPowerBarPush(float value)
        {

        }

        public virtual void onDirectionBarPush(int value)
        {

        }

        public void setIsSelecting(bool isSelecting)
        {
            this.isSelecting = isSelecting;
        }

        public override JsonWriter onWorldModeSave(JsonWriter writer)
        {
            writer = base.onWorldModeSave(writer);
            if (isCanBind())
            {
                IUtils.keyValue2Writer(writer, "b", currentBindId);
            }
            if (isCanSettingValue() != -1)
            {
                IUtils.keyValue2Writer(writer, "s", currentSettingValue);
            }
            if (isCanCollectScientific())
            {
                IUtils.keyValue2Writer(writer, "cs", collectedScientific);
                IUtils.keyValue2Writer(writer, "hci", m_csInfo != null);
                if (m_csInfo != null)
                {
                    m_csInfo.onSave(writer);
                }
            }
            return writer;
        }

        public override void onWorldModeLoad(JsonData blockData, IPoint coor)
        {
            base.onWorldModeLoad(blockData, coor);
            if (isCanBind())
            {
                currentBindId = IUtils.getJsonValue2Int(blockData, "b");
            }
            if (isCanSettingValue() != -1)
            {
                setSettingValue(IUtils.getJsonValue2Int(blockData, "s"));              

            }
            if (isCanCollectScientific())
            {
                collectedScientific = IUtils.getJsonValue2Float(blockData, "cs");
                if( IUtils.getJsonValue2Bool(blockData, "hci"))
                {
                    m_csInfo = CollectedScientificInfo.onLoad(blockData);
                }          
            }
        }

        public override bool isCollider()
        {
            return true;
        }

        public override float getMeltingPoint()
        {
            return meltingPoint;
        }

        public override float getSolidMaxPress()
        {
            return maxPress;
        }

    }

}