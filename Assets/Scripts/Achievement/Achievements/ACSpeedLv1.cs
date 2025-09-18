using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.AchievementSpace
{
    public class ACSpeedLv1 : ACBasic
    {

        public ACSpeedLv1(int id)
            : base(id)
        {
            initAchievement("speedLv1");
        }

        /// <summary>
        /// 速度达到20m/s
        /// </summary>
        public override void setCompletion(int condition)
        {
            if (condition == 1)
            {
                completion();
            }
        }
    }
}