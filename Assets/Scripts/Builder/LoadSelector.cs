using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class LoadSelector
    {

        public Builder builder;
        public bool isLoadPart;
        GameObject loadSelectorObject;
        IScrollView iScrollView;

        public const int MAX_LOADDRAWER_COUNT = 200;
        LoadDrawer[] loadDrawerArr;
        LoadDrawer activitedDrawer;
        Text titleText;

        int filesCount;
        int loadThumbnailImageCount;

        public LoadSelector()
        {
            builder = Builder.instance;

            loadSelectorObject = GameObject.Find("load selector");
            titleText = loadSelectorObject.transform.GetChild(1).GetChild(0).GetComponent<Text>();

            iScrollView = GameObject.Find("Canvas/load selector/Scroll View").GetComponent<IScrollView>();

            loadDrawerArr = new LoadDrawer[MAX_LOADDRAWER_COUNT];

            loadSelectorObject.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(onConfirmButtonClick);
            loadSelectorObject.transform.GetChild(3).gameObject.GetComponent<Button>().onClick.AddListener(onCancelButtonClick);
            loadSelectorObject.transform.GetChild(4).gameObject.GetComponent<Button>().onClick.AddListener(onDelButtonClick);

            hide();

            IUtils.detectionRedundantFiles(GamePath.shipsFolder, "*.ship", GamePath.builderThumbnailFolder);
        }

        public void show()
        {
            titleText.text = ILang.get(isLoadPart ? "Load part" : "Load");
            loadSelectorObject.SetActive(true);
        }

        public void hide()
        {
            loadSelectorObject.SetActive(false);
        }

        public void setFile(FileInfo[] folders)
        {
            activitedDrawer = null;
            for (int i = 0; i < MAX_LOADDRAWER_COUNT; i++)
            {
                LoadDrawer loadDrawer = loadDrawerArr[i];
                if (loadDrawer != null)
                {
                    loadDrawer.clear();
                    loadDrawerArr[i] = null;
                }
            }

            iScrollView.setPerCellHeight(54);

            filesCount = folders.Length;
            for (int i = 0; i < filesCount; i++)
            {
                FileInfo folder = folders[i];
                LoadDrawer loadDrawer = new LoadDrawer(this, iScrollView, folder, i);
                loadDrawerArr[i] = loadDrawer;
            }
            loadThumbnailImageCount = 0;
        }

        public void updata()
        {
            if (loadThumbnailImageCount < filesCount)
            {
                LoadDrawer loadDrawer = loadDrawerArr[loadThumbnailImageCount];
                if (loadDrawer != null)
                {
                    loadDrawer.createThumbnailImage();
                }
                loadThumbnailImageCount++;
            }
        }

        public void onDrawerClick(int rank)
        {
            if (activitedDrawer != null)
            {
                activitedDrawer.setActivited(false);
            }

            activitedDrawer = loadDrawerArr[rank];
            activitedDrawer.setActivited(true);
        }

        public void onConfirmButtonClick()
        {
            if (activitedDrawer != null)
            {
                if (isLoadPart)
                {
                    Builder.changeMode(5);
                    builder.loadParts(activitedDrawer.getSubName());
                }
                else
                {
                    builder.loadBuilderMap(activitedDrawer.getSubName());
                }
                hide();
            }
        }

        public void onCancelButtonClick()
        {
            Builder.changeMode(1);
            hide();
        }

        public void onDelButtonClick()
        {
            if (activitedDrawer != null)
            {
                IConfigBox.instance.show(ILang.get("Config delete", "menu") + " " + activitedDrawer.getSubName() + " ?", onDelConfigButtonClick, null);
            }

        }

        void onDelConfigButtonClick()
        {
            if (activitedDrawer != null)
            {
                activitedDrawer.getFolder().Delete();
                DirectoryInfo direction = new DirectoryInfo(GamePath.shipsFolder);
                FileInfo[] folders = direction.GetFiles("*.ship", SearchOption.TopDirectoryOnly);
                setFile(folders);
            }
        }
    }
}