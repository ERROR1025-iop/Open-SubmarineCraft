using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft.AchievementSpace
{
    public class Achievement : MonoBehaviour
    {

        public IScrollView iScrollView;

        ACManager achManager;

        void Start()
        {
            achManager = new ACManager();

            GameObject.Find("Canvas/Back").GetComponent<Button>().onClick.AddListener(onBackButtonClick);

            iScrollView.setPerCellHeight(220);
            initACCell();
        }

        void initACCell()
        {
            int count = achManager.getAchievementCount();
            for (int i = 0; i < count; i++)
            {
                ACBasic achievement = achManager.getAchievementById(i);
                ACCell cell = new ACCell(achievement, iScrollView);

            }
        }

        void onBackButtonClick()
        {
            Application.LoadLevel("menu");
        }

        void Update()
        {

        }
    }
}