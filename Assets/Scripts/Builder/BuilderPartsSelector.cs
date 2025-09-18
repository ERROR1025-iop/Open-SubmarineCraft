using UnityEngine;
using UnityEngine.UI;


namespace Scraft
{
    public class BuilderPartsSelector
    {

        Builder builder;
        Transform objectTrans;

        public string subname;

        public BuilderPartsSelector()
        {
            builder = Builder.instance;
            objectTrans = GameObject.Find("Canvas/Parts Move").transform;
            objectTrans.GetChild(0).GetComponent<Button>().onClick.AddListener(onConfirmButtonClick);
            objectTrans.GetChild(1).GetComponent<Button>().onClick.AddListener(onCancelButtonClick);
            show(false);
        }

        public void show(bool isShow)
        {
            objectTrans.gameObject.SetActive(isShow);
        }

        public void show(bool isShow, string subname)
        {
            objectTrans.gameObject.SetActive(isShow);
            this.subname = subname;
        }

        void onConfirmButtonClick()
        {
            show(false);
            builder.drawPartsToMap(subname);
            builder.clearPartsMap();
        }

        void onCancelButtonClick()
        {
            builder.clearPartsMap();
            show(false, "x");
        }
    }
}