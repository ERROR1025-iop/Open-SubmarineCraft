using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft {
    public class INetGetSD : MonoBehaviour
    {
        public bool sendRequire;

        bool isResponsed;

        int income;

        void Start()
        {
            isResponsed = false;

            if (sendRequire)
            {
                UnityAndroidEnter.CallGetServiceDiamonds();
                StartCoroutine(requireServiceDiamonds());
            }
        }

        IEnumerator requireServiceDiamonds()
        {            
            yield return new WaitForSeconds(2);
            if (!isResponsed)
            {
                UnityAndroidEnter.CallGetServiceDiamonds();
            }            
        }

        void onGetServiceDiamondsCallBack(string countStr)
        {
            isResponsed = true;
            income = 0;
            if(int.TryParse(countStr, out income))
            {
                if(income > 0)
                {
                    IConfigBox.instance.show(string.Format(ILang.get("You earn income diamonds {0}"), income.ToString()), onEarnIncomeComfirm, onEarnIncomeComfirm);
                    
                }              
            }         
        }

        void onEarnIncomeComfirm()
        {
            ISecretLoad.init();
            ISecretLoad.setDiamonds(ISecretLoad.getDiamonds() + income);
            if (IScientificAndDiamonds.instance_Diamonds != null)
            {
                IScientificAndDiamonds.instance_Diamonds.UpdateNumberText();
            }
            income = 0;
        }

        void onBuySaveCallBack(string sale)
        {
            int count = 0;
            if (int.TryParse(sale, out count))
            {               
                ISecretLoad.init();
                ISecretLoad.setDiamonds(ISecretLoad.getDiamonds() - count);

                if (IScientificAndDiamonds.instance_Diamonds != null)
                {
                    IScientificAndDiamonds.instance_Diamonds.UpdateNumberText();
                }
            }
        }
    }
   
}
