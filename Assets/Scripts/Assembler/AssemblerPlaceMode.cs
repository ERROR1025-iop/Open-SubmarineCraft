using UnityEngine;
using UnityEngine.UI;


namespace Scraft
{
    public class AssemblerPlaceMode
    {
        public static ISwitchImageTextButton NoneButton;
        public static ISwitchImageTextButton MirrorButton;
        public static ISwitchImageTextButton CenterButton;

        Transform placeModeBtnTrans;
        Sprite[] btnSprites;

        public AssemblerPlaceMode()
        {
            placeModeBtnTrans = GameObject.Find("PlaceModeBtn").transform;
            NoneButton = placeModeBtnTrans.GetChild(0).GetComponent<ISwitchImageTextButton>();
            MirrorButton = placeModeBtnTrans.GetChild(1).GetComponent<ISwitchImageTextButton>();
            CenterButton = placeModeBtnTrans.GetChild(2).GetComponent<ISwitchImageTextButton>();

            placeModeBtnTrans.GetChild(0).GetComponent<Button>().onClick.AddListener(onNoneButtonClick);
            placeModeBtnTrans.GetChild(1).GetComponent<Button>().onClick.AddListener(onMirrorButtonClick);
            placeModeBtnTrans.GetChild(2).GetComponent<Button>().onClick.AddListener(onCenterButtonClick);

            btnSprites = Resources.LoadAll<Sprite>("assembler/placeMode");

            Assembler.changePlaceMode(0);
        }

        void onNoneButtonClick()
        {
            Assembler.changePlaceMode(0);
            NoneButton.change(btnSprites[5], NoneButton.showString);
            MirrorButton.change(btnSprites[2], MirrorButton.showString);
            CenterButton.change(btnSprites[0], CenterButton.showString);
        }

        void onMirrorButtonClick()
        {
            Assembler.changePlaceMode(1);
            NoneButton.change(btnSprites[4], NoneButton.showString);
            MirrorButton.change(btnSprites[3], MirrorButton.showString);
            CenterButton.change(btnSprites[0], CenterButton.showString);
        }

        void onCenterButtonClick()
        {
            Assembler.changePlaceMode(2);
            NoneButton.change(btnSprites[4], NoneButton.showString);
            MirrorButton.change(btnSprites[2], MirrorButton.showString);
            CenterButton.change(btnSprites[1], CenterButton.showString);
        }
    }
}