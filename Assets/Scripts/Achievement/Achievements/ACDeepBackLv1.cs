using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.AchievementSpace
{
    public class ACDeepBackLv1 : ACBasic
    {

        bool condition1;
        bool condition2;

        public ACDeepBackLv1(int id)
            : base(id)
        {
            initAchievement("deepBackLv1");

            condition1 = false;
            condition2 = false;
        }

        /// <summary>
        /// 1:深潜2000m
        /// 2:条件1达成后深度<5m
        /// </summary>
        public override void setCompletion(int condition)
        {
            if (condition == 1)
            {
                condition1 = true;
            }
            else if (condition == 2)
            {
                if (condition1 == true)
                {
                    completion();
                }
            }
        }
    }
}