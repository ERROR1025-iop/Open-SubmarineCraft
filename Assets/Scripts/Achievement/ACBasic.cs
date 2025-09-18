using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.AchievementSpace
{
    public class ACBasic
    {


        protected int id;
        protected string name;
        protected bool m_isCompletion;

        private string key = "ZhongRuiChao_91miaoyue.com";

        public ACBasic(int id)
        {
            this.id = id;
        }

        protected void initAchievement(string name)
        {
            this.name = name;
            m_isCompletion = loadIsCompletion();
        }

        public int getId()
        {
            return id;
        }

        public string getName()
        {
            return ILang.get(name, "achievement");
        }

        public string getCondition()
        {
            return ILang.get(name + ".condition", "achievement");
        }

        public string getDescribe()
        {
            return ILang.get(name + ".describe", "achievement");
        }

        private bool loadIsCompletion()
        {
            bool isCompletion = false;
            if (PlayerPrefs.HasKey(name + ".isCompletion"))
            {
                isCompletion = (PlayerPrefs.GetString(name + ".isCompletion")).Equals("true") ? true : false;
            }
            return isCompletion;
        }

        public bool isCompletion()
        {
            return m_isCompletion;
        }

        public virtual void setCompletion(int condition)
        {

        }

        public virtual void removeCompletion(int condition)
        {

        }

        protected void completion()
        {
            m_isCompletion = true;
            ACManager.instance.waveAchievementBar(this);
            PlayerPrefs.SetString(name + ".isCompletion", m_isCompletion ? "true" : "false");
        }
    }
}