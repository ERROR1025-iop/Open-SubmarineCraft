using UnityEngine;
using UnityEngine.UI;
using Scraft.BlockSpace;


namespace Scraft
{
    public class BindSelector
    {

        Transform objectTrans;
        Builder builder;

        Text bindText;

        int arrStack;

        int[] lastBindValue;
        int[] lastBindArrStack;

        public BindSelector()
        {
            builder = Builder.instance;
            objectTrans = GameObject.Find("Canvas/bind").transform;
            bindText = objectTrans.GetChild(1).GetComponent<Text>();
            objectTrans.GetChild(0).GetComponent<Button>().onClick.AddListener(onClick);

            arrStack = 0;
            initLastBindValueArr();
        }

        void initLastBindValueArr()
        {
            lastBindValue = new int[BlocksManager.MAX_BLOCK_ID];
            lastBindArrStack = new int[BlocksManager.MAX_BLOCK_ID];
            for (int i = 0; i < BlocksManager.MAX_BLOCK_ID; i++)
            {
                lastBindValue[i] = -95979;
                lastBindArrStack[i] = -95979;
            }
        }

        public void show(bool isShow)
        {
            objectTrans.gameObject.SetActive(isShow);
            updataText();

        }

        public void returnLastBindValue(SolidBlock sblock)
        {
            if (sblock != null && sblock.isCanBind())
            {
                if (lastBindValue[sblock.getId()] != -95979)
                {
                    sblock.setBindId(lastBindValue[sblock.getId()]);
                }
                if (lastBindArrStack[sblock.getId()] != -95979)
                {
                    arrStack = lastBindArrStack[sblock.getId()];
                }
                else
                {
                    arrStack = 0;
                }
                updataText();
            }
        }

        void onClick()
        {
            SolidBlock block = builder.selectBlock as SolidBlock;
            if (block != null)
            {
                int[] bindArr = block.getBindArr();
                int length = bindArr.Length;
                if (arrStack >= length)
                {
                    arrStack = 0;
                    block.setBindId(0);
                }
                else
                {
                    int id = bindArr[arrStack];
                    block.setBindId(id);
                    arrStack++;
                }
                lastBindValue[block.getId()] = block.getCurrentBindId();
                lastBindArrStack[block.getId()] = arrStack;
                updataText();
            }
        }

        void updataText()
        {
            SolidBlock block = builder.selectBlock as SolidBlock;
            if (block != null)
            {
                int bindId = block.getCurrentBindId();
                bindText.text = ILang.get("bind" + bindId, "menu");
            }
        }
    }
}