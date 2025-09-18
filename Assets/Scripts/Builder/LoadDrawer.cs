using LitJson;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class LoadDrawer
    {

        LoadSelector loadSelector;
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

        public LoadDrawer(LoadSelector loadSelector, IScrollView iScrollView, FileInfo folder, int rank)
        {
            this.loadSelector = loadSelector;
            this.folder = folder;
            this.rank = rank;

            loadDrawerObject = Object.Instantiate(Resources.Load("Prefabs/Builder/load drawer")) as GameObject;
            iScrollView.addCell(loadDrawerObject.transform);

            loadDrawerObject.GetComponent<Button>().onClick.AddListener(onClick);


            GameObject fileNameObject = loadDrawerObject.transform.GetChild(0).gameObject;
            fileNameText = fileNameObject.GetComponent<Text>();

            fileNameText.text = IUtils.removeFileSuffix(folder.Name);

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
                texture2D = IUtils.loadTexture2DFromSD(GamePath.builderThumbnailFolder + getSubName() + ".thu");
                texture2D.filterMode = FilterMode.Point;
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

            Texture2D texture2D = Builder.instance.createThumbnailTexture2D(JsonMapper.ToObject(IUtils.readFromTxt(folder.FullName)));
            if (texture2D == null)
            {
                thumbnailImage.sprite = nullSprite;
                return;
            }
            texture2D.filterMode = FilterMode.Point;
            thumbnailImage.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
            IUtils.resetImageSize(thumbnailImage, new Vector2(60, 40));
            byte[] png = texture2D.EncodeToPNG();
            File.WriteAllBytes(GamePath.builderThumbnailFolder + getSubName() + ".thu", png);
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

        void onClick()
        {
            loadSelector.onDrawerClick(rank);
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

        public void clear()
        {
            Object.Destroy(loadDrawerObject);
        }
    }
}