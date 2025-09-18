using UnityEngine;
using Scraft.AchievementSpace;


namespace Scraft
{
    public class SubAchievement : MonoBehaviour
    {

        ACManager achManager;

        void Start()
        {
            achManager = World.instance.achManager;
        }


        void FixedUpdate()
        {
            speedAchievement(MainSubmarine.speed);
            deepAchievement(MainSubmarine.deep);
            vSpeedAchievement(MainSubmarine.verticalSpeed);
        }

        void deepAchievement(float deep)
        {
            if (deep < 5)
            {
                if (!achManager.deepBackLv1.isCompletion())
                {
                    achManager.deepBackLv1.setCompletion(2);
                }
                if (!achManager.deepBackLv2.isCompletion())
                {
                    achManager.deepBackLv2.setCompletion(2);
                }
                if (!achManager.deepBackLv3.isCompletion())
                {
                    achManager.deepBackLv3.setCompletion(2);
                }
            }

            if (deep > 2000)
            {
                if (!achManager.deepLv1.isCompletion())
                {
                    achManager.deepLv1.setCompletion(1);
                }
                if (!achManager.deepStayLv1.isCompletion())
                {
                    achManager.deepStayLv1.setCompletion(1);
                }
                if (!achManager.deepBackLv1.isCompletion())
                {
                    achManager.deepBackLv1.setCompletion(1);
                }
            }
            else
            {
                if (!achManager.deepStayLv1.isCompletion())
                {
                    achManager.deepStayLv1.removeCompletion(1);
                }
            }

            if (deep > 6000)
            {
                if (!achManager.deepLv2.isCompletion())
                {
                    achManager.deepLv2.setCompletion(1);
                }
                if (!achManager.deepStayLv2.isCompletion())
                {
                    achManager.deepStayLv2.setCompletion(1);
                }
                if (!achManager.deepBackLv2.isCompletion())
                {
                    achManager.deepBackLv2.setCompletion(1);
                }
            }
            else
            {
                if (!achManager.deepStayLv2.isCompletion())
                {
                    achManager.deepStayLv2.removeCompletion(1);
                }
            }

            if (deep > 20000)
            {
                if (!achManager.deepLv3.isCompletion())
                {
                    achManager.deepLv3.setCompletion(1);
                }
                if (!achManager.deepStayLv3.isCompletion())
                {
                    achManager.deepStayLv3.setCompletion(1);
                }
                if (!achManager.deepBackLv3.isCompletion())
                {
                    achManager.deepBackLv3.setCompletion(1);
                }
            }
            else
            {
                if (!achManager.deepStayLv3.isCompletion())
                {
                    achManager.deepStayLv3.removeCompletion(1);
                }
            }
        }

        void vSpeedAchievement(float vSpeed)
        {
            if (vSpeed > 0)
            {
                if (!achManager.deepStayLv1.isCompletion())
                {
                    achManager.deepStayLv1.setCompletion(2);
                }
                if (!achManager.deepStayLv2.isCompletion())
                {
                    achManager.deepStayLv2.setCompletion(2);
                }
                if (!achManager.deepStayLv3.isCompletion())
                {
                    achManager.deepStayLv3.setCompletion(2);
                }
            }
            else
            {
                if (!achManager.deepStayLv1.isCompletion())
                {
                    achManager.deepStayLv1.removeCompletion(2);
                }
                if (!achManager.deepStayLv2.isCompletion())
                {
                    achManager.deepStayLv2.removeCompletion(2);
                }
                if (!achManager.deepStayLv3.isCompletion())
                {
                    achManager.deepStayLv3.removeCompletion(2);
                }
            }
        }

        void speedAchievement(float speed)
        {
            if (speed > 20)
            {
                if (!achManager.speedLv1.isCompletion())
                {
                    achManager.speedLv1.setCompletion(1);
                }
            }
            if (speed > 100)
            {
                if (!achManager.speedLv2.isCompletion())
                {
                    achManager.speedLv2.setCompletion(1);
                }
            }
            if (speed > 340)
            {
                if (!achManager.speedLv3.isCompletion())
                {
                    achManager.speedLv3.setCompletion(1);
                }
            }
        }
    }
}