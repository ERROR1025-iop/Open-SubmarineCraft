using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scraft.BlockSpace;

namespace Scraft.StationSpace
{
    public class ComponentInfo
    {
        public int id;
        public string name;
        public Block[] blocks;
        public int[] counts;
        public int canStoreSoild;
        public int canStoreLiquid;
        public int canStorePower;
        public GameObject gameObject;
        public Sprite thumbnailSprite;
        public int[,] synt;

        public ComponentInfo(int id, string name, int[] canStore, Block[] blocks, int[] counts)
        {
            this.id = id;
            this.name = name;
            this.blocks = blocks;
            this.counts = counts;
            canStoreSoild = canStore[0];
            canStoreLiquid = canStore[1];
            canStorePower = canStore[2];
            thumbnailSprite = getThumbnailSprite();
            gameObject = Resources.Load<GameObject>("Stations/Prefabs/" + name);

            int syntCount = blocks.Length;
            synt = new int[syntCount, 2];
            for (int i = 0; i < syntCount; i++)
            {
                synt[i, 0] = blocks[i].getId();
                synt[i, 1] = counts[i];
            }
        }

        public StationComponent instanceComponet()
        {
            return Object.Instantiate(gameObject).GetComponent<StationComponent>();
        }

        public Sprite getThumbnailSprite()
        {
            if (thumbnailSprite == null)
            {
                Texture2D texture2D;
                thumbnailSprite = Resources.Load<Sprite>("Stations/Icons/" + name);
                if (thumbnailSprite == null)
                {
                    texture2D = createThumbnailImage();
                    thumbnailSprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
                }
            }
            return thumbnailSprite;
        }

        public Texture2D createThumbnailImage()
        {
            if (GameSetting.isAndroid)
            {
                return null;
            }

            //拍摄缩略图         
            GameObject go = Object.Instantiate(Resources.Load("Stations/Prefabs/" + name)) as GameObject;
            go.transform.SetParent(PoolerShotCamera.preShotParent);
            go.transform.localPosition = Vector3.zero;
            string savePath = Application.dataPath + "/Resources/Stations/Icons/" + name + ".png";
            PoolerShotCamera.shotCamera.orthographicSize = 5;
            Texture2D texture2D = AssemblerUtils.createGameObjectThumbnailImage(go, PoolerShotCamera.shotCamera, savePath, new Rect(0, 0, 500, 400));
            thumbnailSprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
            Object.DestroyImmediate(go);
            Debug.Log("[CTI]Create stations component thumbnail [" + name + "] successed!");
            return texture2D;
        }
    }
}
