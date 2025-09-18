using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Scraft.DpartSpace;

namespace Scraft
{
    public class CustomDrawer
    {
        GameObject drawerObject;
        IScrollView cardIScrollView;
        CustomDpartsSelector customDpartsSelector;
        Image drawerImage;
        FileInfo folder;
        int rank;
        string groupName;
        Sprite activityDrawerSprite;
        Sprite unactivityDrawerSprite;
        bool isLoadThu;
        Image thumbnailImage;
        Text drawername;
        Sprite nullSprite;
        bool isLoadGameObject;
        GroupDpart groupDpart;

        public CustomDrawer(CustomDpartsSelector customDpartsSelector, string groupName, IScrollView cardIScrollView, FileInfo folder, int rank)
        {
            this.cardIScrollView = cardIScrollView;
            this.folder = folder;
            this.rank = rank;
            this.customDpartsSelector = customDpartsSelector;
            this.groupName = groupName;

            drawerObject = Object.Instantiate(Resources.Load("Prefabs/Assembler/group drawer cell")) as GameObject;
            cardIScrollView.addCell(drawerObject.transform);

            drawerObject.GetComponent<Button>().onClick.AddListener(onDrawerClick);

            drawerImage = drawerObject.GetComponent<Image>();

            GameObject drawerItemObject = drawerObject.transform.GetChild(0).gameObject;
            thumbnailImage = drawerItemObject.GetComponent<Image>();

            GameObject drawerNameObject = drawerObject.transform.GetChild(1).gameObject;
            drawername = drawerNameObject.GetComponent<Text>();
            drawername.text = folder.Name.Substring(folder.Name.LastIndexOf("\\") + 1, (folder.Name.LastIndexOf(".") - folder.Name.LastIndexOf("\\") - 1));

            activityDrawerSprite = Resources.Load("builder/drawer-selected", typeof(Sprite)) as Sprite;
            unactivityDrawerSprite = Resources.Load("builder/drawer-unselected", typeof(Sprite)) as Sprite;
            nullSprite = Resources.Load("builder/null", typeof(Sprite)) as Sprite;

            loadThumbnail();
            setActivited(false);
        }

        private void loadThumbnail()
        {
            thumbnailImage.sprite = nullSprite;
            Texture2D texture2D;
            try
            {
                texture2D = IUtils.loadTexture2DFromSD(GamePath.customThumbnailFolder + groupName + "/" + getDrawerName() + ".thu");
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

            Texture2D texture2D = AssemblerUtils.createDpartThumbnailImage(Assembler.dpartsEngine, folder.FullName, GamePath.customThumbnailFolder + groupName + "/", getDrawerName());
            thumbnailImage.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
            IUtils.resetImageSize(thumbnailImage, new Vector2(60, 40));
        }

        public string getDrawerName()
        {
            return drawername.text;
        }

        void onDrawerClick()
        {
            if (!isLoadGameObject)
            {
                loadGameObject();
            }
            GroupDpartsSelector.selectDpart = groupDpart;
            customDpartsSelector.onDrawerClick(groupName, getDrawerName(), rank);
        }

        void loadGameObject()
        {
            groupDpart = AssemblerUtils.loadGroupDpart(Assembler.dpartsEngine, GroupDpartsSelector.groupsSelectorParent, folder.FullName, getDrawerName());
            groupDpart = AssemblerUtils.unGroupDpartIfItIsOneGroup(groupDpart);
            groupDpart.setVisible(false);
            isLoadGameObject = true;
        }

        public void setActivited(bool isActivited)
        {
            if (isActivited)
            {
                drawerImage.sprite = activityDrawerSprite;
            }
            else
            {
                drawerImage.sprite = unactivityDrawerSprite;
            }
        }

        public void clear()
        {
            Object.Destroy(drawerObject);
        }
    }
}