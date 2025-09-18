using System.IO;
using UnityEngine;
using UnityEngine.UI;


namespace Scraft
{
    public class AssemblerLoaderDrawer
    {
        AssemblerLoader loadSelector;
        FileInfo folder;
        IScrollView iScrollView;
        GameObject loadDrawerObject;
        int rank;
        Text fileNameText;
        bool isLoadThu;

        private Sprite activitySprite;
        private Sprite unactivitySprite;
        private Sprite nullSprite;
        private Image loadDrawerImage;
        private Image thumbnailImage;

        public AssemblerLoaderDrawer(AssemblerLoader loadSelector, IScrollView iScrollView, FileInfo folder, int rank)
        {
            this.loadSelector = loadSelector;
            this.folder = folder;
            this.rank = rank;

            loadDrawerObject = Object.Instantiate(Resources.Load("Prefabs/Builder/load drawer")) as GameObject;
            iScrollView.addCell(loadDrawerObject.transform);

            loadDrawerObject.GetComponent<Button>().onClick.AddListener(onClick);

            GameObject fileNameObject = loadDrawerObject.transform.GetChild(0).gameObject;
            fileNameText = fileNameObject.GetComponent<Text>();

            fileNameText.text = folder.Name.Substring(folder.Name.LastIndexOf("\\") + 1, (folder.Name.LastIndexOf(".") - folder.Name.LastIndexOf("\\") - 1));

            activitySprite = Resources.Load("builder/load-selected", typeof(Sprite)) as Sprite;
            unactivitySprite = Resources.Load("builder/load-unselected", typeof(Sprite)) as Sprite;
            nullSprite = Resources.Load("builder/null", typeof(Sprite)) as Sprite;

            loadDrawerImage = loadDrawerObject.GetComponent<Image>();
            thumbnailImage = loadDrawerObject.transform.GetChild(1).GetComponent<Image>();
            loadThumbnail();

            setActivited(false);
        }

        private void loadThumbnail()
        {
            thumbnailImage.sprite = nullSprite;
            Texture2D texture2D;
            try
            {
                texture2D = IUtils.loadTexture2DFromSD(GamePath.assemblerThumbnailFolder + getSubName() + ".thu");
                isLoadThu = true;
            }
            catch
            {
                isLoadThu = false;
                return;
            }
            thumbnailImage.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
            IUtils.resetImageSize(thumbnailImage, new Vector2(60, 40));

        }

        public void createThumbnailImage()
        {
            if (isLoadThu)
            {
                return;
            }

            Texture2D texture2D = AssemblerUtils.createDpartThumbnailImage(Assembler.dpartsEngine, folder.FullName, GamePath.assemblerThumbnailFolder, getSubName());
            thumbnailImage.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
        }

        public string getSubName()
        {
            return fileNameText.text;
        }

        public string getFileName()
        {
            return folder.Name;
        }

        public FileInfo getFolder()
        {
            return folder;
        }

        public int getRank()
        {
            return rank;
        }

        public void setActivited(bool isActivited)
        {
            if (isActivited)
            {
                loadDrawerImage.sprite = activitySprite;
            }
            else
            {
                loadDrawerImage.sprite = unactivitySprite;
            }
        }

        public void onClick()
        {
            loadSelector.onDrawerClick(rank);
        }

        public void clear()
        {
            Object.Destroy(loadDrawerObject);
        }
    }
}