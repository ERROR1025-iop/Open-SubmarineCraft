using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEditor;

namespace Scraft.BlockSpace
{
    public class PreBlockTemperature : PreBlock
    {
        // 还有一个变量protected float density在PreBlock中，代表该方块的单位质量
        public float temperature;
        public float transmissivity;
        public float heatCapacity; 
        public float heatQuantity;

        public PreBlockTemperature()
        {

        }

        public void setTemperature(float t)
        {
            temperature = t;
            heatQuantity = temperature * density * heatCapacity;
        }

        public void inheritHeatQuantity(PreBlockTemperature oldBlock)
        {
            setHeatQuantity(oldBlock.heatQuantity);
        }

        static public void SeparationTemperatureCalculation(PreBlockTemperature oldBlock, PreBlockTemperature newBlock)
        {
            // 计算 newBlock 达到 creator 温度所需的热量（假设初始 heatQuantity = 0）
            float heatRequired = oldBlock.temperature * newBlock.density * newBlock.heatCapacity;

            // 从 creator 转移热量到 newBlock
            oldBlock.heatQuantity -= heatRequired;
            newBlock.heatQuantity = heatRequired;
            newBlock.temperature = oldBlock.temperature; // 显式设置温度

            // 更新 creator 的温度（因其热量减少）
            oldBlock.temperature = oldBlock.heatQuantity / (oldBlock.density * oldBlock.heatCapacity);
        }

        public float getTemperature()
        {
            return temperature;
        }

        public void setTransmissivity(float t)
        {
            transmissivity = t;
        }


        public void addTemperature(float add_t)
        {
            setTemperature(temperature + add_t);
        }

        /// <summary>
        /// 线程更新
        /// </summary>
        public void addHeatQuantity(float q)
        {
            heatQuantity += q;
            float totalHeatCapacity = density * heatCapacity;
            if (totalHeatCapacity > 0)
                temperature = heatQuantity / totalHeatCapacity;
        }
        public void setHeatQuantity(float q)
        {
            heatQuantity = q;
            float totalHeatCapacity = density * heatCapacity;
            temperature = heatQuantity / totalHeatCapacity;
            // if (temperature < 25)
            // {
            //     Debug.Log("Temperature too low: " + temperature + ", block: " + coor.toString());
            // }
        }

        /// <summary>
        /// 线程更新
        /// </summary>
        public virtual void threadUpdate(BlocksEngine blocksEngine)
        {
            temperatureRule(blocksEngine);
        }

        /// <summary>
        /// 温度传递规律（修复：防止低热容方块温度反超）
        /// </summary>
        public virtual void temperatureRule(BlocksEngine blocksEngine)
        {
            Block[] neighbors = new Block[]
            {
                getNeighborBlock(Dir.up),
                getNeighborBlock(Dir.right),
                getNeighborBlock(Dir.down),
                getNeighborBlock(Dir.left)
            };

            float deltaTime = PoolBlockParams.instance.temperature_diff;            

            foreach (Block neighborBlock in neighbors)
            {
                if (neighborBlock == null || !(neighborBlock is PreBlockTemperature other))
                    continue;

                PreBlockTemperature otherTemp = other;

                float thisHeatCap = density * heatCapacity;
                float otherHeatCap = otherTemp.density * otherTemp.heatCapacity;

                if (thisHeatCap <= 0 || otherHeatCap <= 0)
                    continue;

                float tempDiff = temperature - otherTemp.temperature;
                if (Mathf.Abs(tempDiff) < 0.5f) // 小于 0.00001℃ 视为无温差
                    continue;

                   

                // 双方 transmissivity 调和平均
                float t1 = transmissivity;
                float t2 = otherTemp.transmissivity;
                float effectiveTrans = (t1 + t2 > 0) ? (2.0f * t1 * t2 / (t1 + t2)) : 0;

                // 基础传热速率
                float heatTransferRate = effectiveTrans * tempDiff * deltaTime;
                float dQ = heatTransferRate * 0.04f;

                if (tempDiff < 0) dQ = -dQ;

                // ✅ 计算平衡温度（热平衡理论值）
                float totalHeat = heatQuantity + otherTemp.heatQuantity;
                float totalCap = thisHeatCap + otherHeatCap;
                float equilibriumTemp = totalHeat / totalCap;

                // ✅ 计算最大安全传热量（防止越过平衡点）
                float maxDQ;
                if (temperature > otherTemp.temperature)
                {
                    maxDQ = (temperature - equilibriumTemp) * thisHeatCap;
                }
                else
                {
                    maxDQ = (otherTemp.temperature - equilibriumTemp) * otherHeatCap;
                }

                // ✅ 阻尼控制：每步最多传递趋向平衡热量的 50%
                float dampingRatio = 0.5f; // 可调，0.3~1.0
                float safeMaxDQ = maxDQ * dampingRatio;

                dQ = Mathf.Clamp(dQ, 0, safeMaxDQ);
                if (tempDiff < 0) dQ = -dQ;

                // 保存旧温度用于断言
                float oldTempA = temperature;
                float oldTempB = otherTemp.temperature;

                // 执行热量交换
                heatQuantity -= dQ;
                otherTemp.heatQuantity += dQ;

                // 更新温度
                temperature = heatQuantity / thisHeatCap;
                otherTemp.temperature = otherTemp.heatQuantity / otherHeatCap;                
            }
        }
        
    }
}