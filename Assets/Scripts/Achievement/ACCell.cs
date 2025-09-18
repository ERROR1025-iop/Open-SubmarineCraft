using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

namespace Scraft.AchievementSpace
{
    public class ACCell
    {

        ACBasic achievement;

        GameObject cellObject;
        Image cardImage;
        Text titleText;
        Text conditionText;
        Text completionText;
        Text describeText;

        private static Sprite activityCardSprite;
        private static Sprite unactivityCardSprite;

        public ACCell(ACBasic achievement, IScrollView iScrollView)
        {
            this.achievement = achievement;

            cellObject = Object.Instantiate(Resources.Load("Prefabs/Achievement/ACCell")) as GameObject;
            cardImage = cellObject.GetComponent<Image>();
            titleText = cellObject.transform.GetChild(0).GetComponent<Text>();
            conditionText = cellObject.transform.GetChild(1).GetComponent<Text>();
            completionText = cellObject.transform.GetChild(2).GetComponent<Text>();
            describeText = cellObject.transform.GetChild(3).GetComponent<Text>();
            iScrollView.addCell(cellObject.transform);

            unactivityCardSprite = Resources.Load("builder/card-selected", typeof(Sprite)) as Sprite;
            activityCardSprite = Resources.Load("builder/card-unselected", typeof(Sprite)) as Sprite;

            init();
        }

        void init()
        {
            bool isCompletion = achievement.isCompletion();
            if (isCompletion)
            {
                cardImage.sprite = activityCardSprite;
                completionText.text = ILang.get("Completion");
                completionText.color = new Color(0, 1, 0);
            }
            else
            {
                cardImage.sprite = unactivityCardSprite;
                completionText.text = ILang.get("Not reached");
                completionText.color = new Color(0.3f, 0.3f, 0.3f);
            }

            titleText.text = achievement.getName();
            conditionText.text = ILang.get("condition") + ":" + achievement.getCondition();
            describeText.text = achievement.getDescribe();
        }
    }
}