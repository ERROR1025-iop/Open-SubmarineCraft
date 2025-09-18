using Scraft.DpartSpace;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class AttributeColor : MonoBehaviour
    {

        public static AttributeColor instance;

        public static int selectColorPart;
        public static Color[] selectColors;

        public Transform userdColorGridTrans;
        public Transform materialsGridTrans;
        public GameObject part1;
        public GameObject part2;

        public static DpartMaterial selectedShareMaterialStatic;

        DpartsEngine dpartsEngine;
        DpartMaterialsManager dpartMaterialsManager;

        Image part1Image;
        Image part2Image;

        int UserdColorCellcount = 5;
        AttributeUsedColorCell[] userdColorCells;
        Queue<Color> userdColors;

        private void Awake()
        {
            instance = this;
            dpartsEngine = Assembler.dpartsEngine;
            dpartMaterialsManager = DpartMaterialsManager.instance;
            createMaterialCell();
            selectedShareMaterialStatic = dpartMaterialsManager.getMaterialById(0);

            part1Image = part1.GetComponent<Image>();
            part2Image = part2.GetComponent<Image>();
            part1.GetComponent<Button>().onClick.AddListener(onPart1ButtonClick);
            part2.GetComponent<Button>().onClick.AddListener(onPart2ButtonClick);

            onPart1ButtonClick();

            selectColors = new Color[2];
            selectColors[0] = Color.white;
            selectColors[1] = Color.white;

            userdColorCells = new AttributeUsedColorCell[UserdColorCellcount];
            userdColors = new Queue<Color>();

            createUserdColorCell();
        }

        void createUserdColorCell()
        {
            for (int i = 0; i < UserdColorCellcount; i++)
            {
                AttributeUsedColorCell cell = new AttributeUsedColorCell(this);
                userdColorCells[i] = cell;
            }
        }

        public void addUsedColorCell(Color color)
        {
            if (!userdColors.Contains(color))
            {
                userdColors.Enqueue(color);
            }
            if (userdColors.Count > 5)
            {
                userdColors.Dequeue();
            }
            int i = 0;
            foreach (Color c in userdColors)
            {
                userdColorCells[userdColors.Count - 1 - i].setColor(c);
                i++;
            }
        }

        void createMaterialCell()
        {
            int count = dpartMaterialsManager.getMaterialCount();
            for (int i = 0; i < count; i++)
            {
                AttributeMaterialCell cell = new AttributeMaterialCell(this, dpartMaterialsManager.getMaterialById(i));
            }
        }

        public void onMaterialCellClick(int colorId, AttributeMaterialCell cell)
        {
            selectedShareMaterialStatic = cell.getDpartMaterial();
            changeSelectDpart(dpart => dpart.setMaterial(selectedShareMaterialStatic.getMaterial()));
        }

        public void onPart1ButtonClick()
        {
            selectColorPart = 1;
            part1Image.color = Color.blue;
            part2Image.color = Color.white;
        }

        public void onPart2ButtonClick()
        {
            selectColorPart = 2;
            part1Image.color = Color.white;
            part2Image.color = Color.blue;
        }

        public void onOpenColorSelectorButtonClick()
        {
            AssemblerColorSelector.instance.show(onColorSelectorConfirmButtonClick);
        }

        public void onUserColorCellClick(Color color)
        {
            selectColors[selectColorPart - 1] = color;
            changeSelectDpart(dpart => dpart.setColor(color, selectColorPart));
        }

        void onColorSelectorConfirmButtonClick()
        {
            selectColors[selectColorPart - 1] = AssemblerColorSelector.color;
            changeSelectDpart(dpart => dpart.setColor(AssemblerColorSelector.color, selectColorPart));
            addUsedColorCell(AssemblerColorSelector.color);
        }

        void changeSelectDpart(System.Action<Dpart> execute)
        {
            GameObject[] selection = IRT.Selection.gameObjects;
            if (selection == null)
            {
                return;
            }

            for (int i = 0; i < selection.Length; ++i)
            {
                GameObject selectedObj = selection[i];
                if (selectedObj != null)
                {
                    Dpart dpart = selectedObj.GetComponent<DpartParent>().getDpart();
                    execute(dpart);

                }
            }
        }



    }
}
