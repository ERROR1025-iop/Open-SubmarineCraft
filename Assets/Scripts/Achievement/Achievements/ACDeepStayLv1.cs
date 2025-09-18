using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.AchievementSpace
{
    public class ACDeepStayLv1 : ACBasic
    {

        bool condition1;
        bool condition2;

        public ACDeepStayLv1(int id)
            : base(id)
        {
            initAchievement("deepStayLv1");

            condition1 = false;
            condition2 = false;
        }

        /// <summary>
        /// 1:深潜2000m
        /// 2:垂直速度<0m/s
        /// </summary>
        public override void setCompletion(int condition)
        {
            if (condition == 1)
            {
                condition1 = true;
            }
            else if (condition == 2)
            {
                condition2 = true;
            }

            if (condition1 && condition2)
            {
                completion();
            }
        }

        public override void removeCompletion(int condition)
        {
            if (condition == 1)
            {
                condition1 = false;
            }
            else if (condition == 2)
            {
                condition2 = false;
            }
        }
    }
}