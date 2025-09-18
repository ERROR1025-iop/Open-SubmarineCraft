using Scraft.BlockSpace;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class RotateSelector
    {

        Transform objectTrans;
        Builder builder;


        public RotateSelector()
        {
            builder = Builder.instance;
            objectTrans = GameObject.Find("Canvas/rotate").transform;
            objectTrans.GetComponent<Button>().onClick.AddListener(onClick);
        }

        public void show(bool isShow)
        {
            objectTrans.gameObject.SetActive(isShow);
        }

        void onClick()
        {
            Block block = builder.selectBlock;
            if (block != null)
            {
                block.onRotateButtonClick();
            }
        }
    }
}
