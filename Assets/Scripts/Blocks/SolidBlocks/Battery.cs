using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Scraft.BlockSpace
{
    public class Battery : SolidBlock
    {
        public static List<Battery> battersArr;

        protected float m_MaxElectric;
        float m_storeElectric;

        public Battery(int id, GameObject parentObject, GameObject blockObject)
            : base(id, parentObject, blockObject)
        {
            initBlock("battery", "power");
            isCanChangeRedAndCrackTexture = false;
            thumbnailColor = new Color(0.902f, 0.902f, 0.902f);
            transmissivity = 4.2f;
            density = 10.85f;
            m_MaxElectric = 1400;
            m_storeElectric = 0;
            max_storeAir = 0;
        }

        public override Block clone(GameObject parentObject, BlocksManager blocksManager, GameObject blockObject)
        {
            Battery block = new Battery(blockId, parentObject, blockObject);
            block.initSolidBlock(blocksManager, "steelLiquid", 1179, 964);
            return block;
        }

        public override SyntInfo[] getSyntInfo(BlocksManager blocksManager)
        {
            int[,] syntData = new int[4, 2] {
                    { blocksManager.steel.getId(), 1 },
                     { blocksManager.copper.getId(), 1 },
                    { blocksManager.sulphuricAcid.getId(), 1 },
                    { blocksManager.lead.getId(), 1 }
            };

            SyntInfo[] syntInfos = new SyntInfo[1];
            syntInfos[0] = new SyntInfo(this, syntData, 1);

            return syntInfos;
        }

        public override void onRegister()
        {
            base.onRegister();

            if (battersArr == null)
            {
                battersArr = new List<Battery>();
            }
            else
            {
                battersArr.Clear();
            }
        }

        public override void onWorldModeDestroy()
        {
            base.onWorldModeDestroy();

            m_isNeedDelete = true;
            battersArr.Remove(this);
            PoolerUI.instance.updateMaxElectric();
        }

        public override void onPoolerModeInitFinish()
        {
            base.onPoolerModeInitFinish();
            battersArr.Add(this);            
            m_storeElectric = GameSetting.isCareer ? 0 : m_MaxElectric;
            PoolerUI.instance.updateMaxElectric();
        }

        public float getStroreElectric()
        {
            int index = (int)(Mathf.Clamp01(m_storeElectric / m_MaxElectric) * 2.5f);
            setSpriteRect(index);
            return m_storeElectric;
        }

        public virtual float chargeElectric(Block generator, float charge)
        {
            if (m_storeElectric < m_MaxElectric)
            {
                if (m_storeElectric + charge <= m_MaxElectric)
                {
                    m_storeElectric += charge;
                    return 0;
                }
                else
                {
                    float overflow = m_storeElectric + charge - m_MaxElectric;
                    m_storeElectric = m_MaxElectric;
                    return overflow;
                }
            }
            else
            {
                return charge;
            }
        }

        public virtual float requireElectric(Block taker, float require)
        {
            if (m_storeElectric > 0)
            {
                if (m_storeElectric - require > 0)
                {
                    m_storeElectric -= require;
                    return require;
                }
                else
                {
                    float surplus = m_storeElectric;
                    m_storeElectric = 0;
                    return surplus;
                }
            }
            else
            {
                return 0;
            }
        }

        public override float getMaxStoreElectric()
        {
            return m_MaxElectric;
        }

        public override JsonWriter onWorldModeSave(JsonWriter writer)
        {
            writer = base.onWorldModeSave(writer);
            IUtils.keyValue2Writer(writer, "se", m_storeElectric);
            return writer;
        }

        public override void onWorldModeLoad(JsonData blockData, IPoint coor)
        {
            base.onWorldModeLoad(blockData, coor);
            m_storeElectric = IUtils.getJsonValue2Float(blockData, "se");

        }
    }
}