using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.AchievementSpace
{
    public class ACManager
    {

        public static ACManager instance;

        ACBasic[] ACArr;
        public const int MAX_AC_ID = 100;
        int idStack;

        public ACBasic deepLv1;
        public ACBasic deepLv2;
        public ACBasic deepLv3;
        public ACBasic deepStayLv1;
        public ACBasic deepStayLv2;
        public ACBasic deepStayLv3;
        public ACBasic deepBackLv1;
        public ACBasic deepBackLv2;
        public ACBasic deepBackLv3;
        public ACBasic speedLv1;
        public ACBasic speedLv2;
        public ACBasic speedLv3;

        ACBarMono achBarMono;

        public ACManager()
        {
            ACArr = new ACBasic[MAX_AC_ID];
            idStack = 0;
            registerAchievements();
            instance = this;
            achBarMono = null;
        }

        public void setACBarMono(ACBarMono achBarMono)
        {
            this.achBarMono = achBarMono;
        }

        void registerAchievements()
        {
            deepLv1 = registerAchievement(new ACDeepLv1(getUnuserId()));
            deepLv2 = registerAchievement(new ACDeepLv2(getUnuserId()));
            deepLv3 = registerAchievement(new ACDeepLv3(getUnuserId()));
            deepStayLv1 = registerAchievement(new ACDeepStayLv1(getUnuserId()));
            deepStayLv2 = registerAchievement(new ACDeepStayLv2(getUnuserId()));
            deepStayLv3 = registerAchievement(new ACDeepStayLv3(getUnuserId()));
            deepBackLv1 = registerAchievement(new ACDeepBackLv1(getUnuserId()));
            deepBackLv2 = registerAchievement(new ACDeepBackLv2(getUnuserId()));
            deepBackLv3 = registerAchievement(new ACDeepBackLv3(getUnuserId()));
            speedLv1 = registerAchievement(new ACSpeedLv1(getUnuserId()));
            speedLv2 = registerAchievement(new ACSpeedLv2(getUnuserId()));
            speedLv3 = registerAchievement(new ACSpeedLv3(getUnuserId()));
        }

        ACBasic registerAchievement(ACBasic ac)
        {
            int id = ac.getId();
            ACArr[id] = ac;
            return ac;
        }

        int getUnuserId()
        {
            return idStack++;
        }

        public ACBasic getAchievementById(int id)
        {
            return ACArr[id];
        }

        public int getAchievementCount()
        {
            return idStack;
        }

        public void waveAchievementBar(ACBasic achievement)
        {
            if (achBarMono != null)
            {
                achBarMono.show(ILang.get("Achievement1") + "<color=red>\"" + achievement.getName() + "\"</color>!");
            }
        }
    }
}
