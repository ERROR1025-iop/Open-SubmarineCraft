using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.AchievementSpace
{
    public class ACDeepLv1 : ACBasic
    {

        public ACDeepLv1(int id)
            : base(id)
        {
            initAchievement("deepLv1");
        }

        /// <summary>
        /// 1:深潜2000m
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