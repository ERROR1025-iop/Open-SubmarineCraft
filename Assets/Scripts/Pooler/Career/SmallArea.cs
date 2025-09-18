using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scraft.BlockSpace;
using LitJson;
using UnityEditor;

namespace Scraft
{
    public class SmallArea : MonoBehaviour
    {
        public float radius;

        [Header("Ore probability")]
        [Range(0, 1)] public float stone;
        [Range(0, 1)] public float soil;
        [Range(0, 1)] public float ironOre;
        [Range(0, 1)] public float copperOre;
        [Range(0, 1)] public float sulfurOre;
        [Range(0, 1)] public float leadOre;
        [Range(0, 1)] public float coal;
        [Range(0, 1)] public float oil;
        [Range(0, 1)] public float naturalGas;
        [Range(0, 1)] public float gasHydrate;

        [Header("Ore probability")]
        public float temperture;

        [Header("Read only")]
        public int id;
        public float poor;
        public float poorRatio;
        public float oreRatio;
        public float enrichment;

        [Header("Selection")]
        public float sumProbabilitie;
        public float stoneSelection;
        public float soilSelection;
        public float ironOreSelection;
        public float copperOreSelection;
        public float sulfurOreSelection;
        public float leadOreSelection;
        public float coalSelection;
        public float oilSelection;
        public float naturalGasSelection;
        public float gasHydrateSelection;

        BlocksManager blocksManager;
        List<OreProbability> oreProbabilities;



        void Start()
        {
            onStart();
            id = AreaManager.smallAreaId++;
            AreaManager.smallAreas.Add(this);

            if (AreaManager.smallAreaDatas != null)
            {
                JsonData areaDatas = AreaManager.smallAreaDatas[name];
                onLoad(areaDatas);
            }
            updateOreRatio();
            updateEnrichment();
        }

        protected void onStart()
        {
            oreProbabilities = new List<OreProbability>();
            blocksManager = BlocksManager.instance;
            registerProbabilitys();
            poor = radius;
        }

        void registerProbabilitys()
        {
            oreProbabilities.Add(new OreProbability(blocksManager.stone, stone, false));
            oreProbabilities.Add(new OreProbability(blocksManager.soil, soil, false));
            oreProbabilities.Add(new OreProbability(blocksManager.ironOre, ironOre, true));
            oreProbabilities.Add(new OreProbability(blocksManager.copperOre, copperOre, true));
            oreProbabilities.Add(new OreProbability(blocksManager.sulfurOre, sulfurOre, true));
            oreProbabilities.Add(new OreProbability(blocksManager.leadOre, leadOre, true));
            oreProbabilities.Add(new OreProbability(blocksManager.coal, coal, true));
            oreProbabilities.Add(new OreProbability(blocksManager.oil, oil, true));
            oreProbabilities.Add(new OreProbability(blocksManager.naturalGas, naturalGas, true));
            oreProbabilities.Add(new OreProbability(blocksManager.gasHydrate, gasHydrate, true));
        }

        protected void updateOreRatio()
        {
            poorRatio = poor / radius;
            sumProbabilitie = 0;
            foreach (OreProbability p in oreProbabilities)
            {
                p.update(poorRatio);
                sumProbabilitie += p.probability;
                p.section = sumProbabilitie;
            }
            oreRatio = (sumProbabilitie - stone - soil) / sumProbabilitie;
        }

        protected void updateEnrichment()
        {
            enrichment = oreRatio * poorRatio;
            display();
        }

        void display()
        {
            stone = oreProbabilities[0].probability;
            soil = oreProbabilities[1].probability;
            ironOre = oreProbabilities[2].probability;
            copperOre = oreProbabilities[3].probability;
            sulfurOre = oreProbabilities[4].probability;
            leadOre = oreProbabilities[5].probability;
            coal = oreProbabilities[6].probability;
            oil = oreProbabilities[7].probability;
            naturalGas = oreProbabilities[8].probability;
            gasHydrate = oreProbabilities[9].probability;

            stoneSelection = oreProbabilities[0].section;
            soilSelection = oreProbabilities[1].section;
            ironOreSelection = oreProbabilities[2].section;
            copperOreSelection = oreProbabilities[3].section;
            sulfurOreSelection = oreProbabilities[4].section;
            leadOreSelection = oreProbabilities[5].section;
            coalSelection = oreProbabilities[6].section;
            oilSelection = oreProbabilities[7].section;
            naturalGasSelection = oreProbabilities[8].section;
            gasHydrateSelection = oreProbabilities[9].section;
        }

        public Block randomOreBlock()
        {
            float value = Random.value * sumProbabilitie;
            foreach (OreProbability p in oreProbabilities)
            {
                if (p.section > value)
                {
                    if (p.decByPoor)
                    {
                        poor -= 0.05f;
                    }
                    updateOreRatio();
                    updateEnrichment();
                    return p.block;
                }
            }
            return blocksManager.stone;
        }

        public virtual float getTemperture()
        {
            return temperture;
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public virtual void onSave(JsonWriter writer)
        {
            IUtils.keyValue2Writer(writer, "poor", poor);
        }

        public virtual void onLoad(JsonData jsonData)
        {
            poor = IUtils.getJsonValue2Float(jsonData, "poor", radius);
        }
    }

    public class OreProbability
    {
        public Block block;
        public float default_probability;
        public float probability;
        public float section;
        public bool decByPoor;

        public OreProbability(Block block, float probability, bool decByPoor)
        {
            this.block = block;
            default_probability = probability;
            this.probability = default_probability;
            this.decByPoor = decByPoor;
        }

        public void update(float poorRatio)
        {
            if (decByPoor)
            {
                probability = default_probability * poorRatio;
            }
        }
    }
}
