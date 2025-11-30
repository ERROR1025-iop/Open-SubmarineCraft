using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Battlehub.RTCommon;
using Scraft;

namespace Scraft.DpartSpace
{
    public class Dpart
    {
        public static int gUid = 0;
        public int uid; 
        protected int dpartId;
        protected int identifyId;
        protected string dpartName;
        protected GameObject dpartObject;
        protected Transform dpartTrans;
        protected DpartParent dpartParent;
        protected int dpartChildCount;
        string iconTexturePath;
        Sprite iconSprite;
        string attributeCardName;
        GameObject gameParentObject;
        protected IPoint coor;
        string materialName;
        protected Color[] color;
        protected Material material;
        public Dpart mirrorDpart;
        public bool isSelecting;
        public GroupDpart groupDpart;
        public bool temporary;
        protected ModInfo modInfo;
        public bool isMod;
        public List<int> linkedUid = new List<int>();
        public bool drawArc = false; 

        public Dpart(int id, GameObject parentObject)
        {
            uid = getUid();
            dpartId = id;
            gameParentObject = parentObject;
            isSelecting = false;
            groupDpart = null;
            temporary = false;           
            color = new Color[2];
        }

        public void initDpart(string dpartName, string attributeCardName, ModInfo modInfo)
        {
            this.dpartName = dpartName;
            this.attributeCardName = attributeCardName; 
            this.modInfo = modInfo;
            isMod = modInfo != null;

            createDpartObject();
        }

        public static int getUid()
        {
            return gUid++;
        }

        void createDpartObject()
        {
            iconTexturePath = getSelfTexturePath();
            if (isMod)
            {
                Texture2D texture2D = IUtils.loadTexture2DFromSD(iconTexturePath);
                iconSprite = Sprite.Create(texture2D, new Rect(0f, 0f, 500f, 400f), new Vector2(0.5f, 0.5f));
            }
            else
            {
                iconSprite = Resources.Load(iconTexturePath, typeof(Sprite)) as Sprite;
            }
               

            if (gameParentObject != null)
            {
                if (isMod)
                {
                    AssetBundle ab = ModLoader.assetBundleDictionary[modInfo.name];
                    Object abObject = ab.LoadAsset(dpartName);
                    dpartObject = Object.Instantiate(abObject) as GameObject;
                }
                else
                {
                    dpartObject = Object.Instantiate(Resources.Load("dparts/prefabs/" + dpartName)) as GameObject;
                }                    

                dpartObject.transform.SetParent(gameParentObject.transform);

                dpartTrans = dpartObject.transform;
                material = dpartObject.GetComponent<Material>();

                dpartParent = dpartObject.GetComponent<DpartParent>();
                dpartParent.init(this);
                dpartParent.changeDpartChindrens(go => go.init(this));
                dpartChildCount = dpartParent.getChildrensCount();
            }
        }

        public void setGroupDpart(GroupDpart groupDpart)
        {
            this.groupDpart = groupDpart;
        }

        public string getBasicInformation()
        {
            string langName = isMod ? dpartName + ".info" : dpartName;
            string information = ILang.get(langName, isMod ?modInfo.name + ILang.getSelectedLangName() : "dpart-information");
            if (information.Equals(langName))
            {
                return ILang.get(dpartName, isMod ? modInfo.name + ILang.getSelectedLangName() : "dpart");
            }
            else
            {
                return ILang.get(dpartName, isMod ? modInfo.name + ILang.getSelectedLangName() : "dpart") + "," + information;
            }                
        }

        protected virtual string getSelfTexturePath()
        {
            if (isMod)
            {
                return string.Format("{0}{1}/res/{2}.png", GamePath.modFolder, modInfo.name, dpartName);
            }
            else
            {
                return "dparts/icon/" + dpartName;
            }            
        }

        public Sprite getIconSprite()
        {
            return iconSprite;
        }

        public virtual void setLayer(int layer)
        {
            dpartParent.changeDpartChindrens(child => child.gameObject.layer = layer);
        }       

        public void setVisible(bool isVisible)
        {
            setLayer(isVisible ? 8 : 9);
        }

        public void copyTransform(Dpart copyedDpart)
        {
            dpartTrans.localPosition = copyedDpart.dpartTrans.localPosition;
            dpartTrans.localRotation = copyedDpart.dpartTrans.localRotation;
            dpartTrans.localScale = copyedDpart.dpartTrans.localScale;
        }

        public virtual void mirrorTransform(Dpart mirroredDpart)
        {
            syncMirrorTransform(mirroredDpart, this);

            this.mirrorDpart = mirroredDpart;
            mirroredDpart.setMirrorDpart(this);
        }

        public virtual void syncMirrorTransform(Dpart orgDpart, Dpart needMirrorDpart)
        {
            if (orgDpart == null || needMirrorDpart == null || orgDpart.dpartTrans == null || needMirrorDpart.dpartTrans == null)
            {
                return;
            }
            Vector3 mirrorPos = orgDpart.dpartTrans.localPosition;
            Vector3 mirrorRot = orgDpart.dpartTrans.localRotation.eulerAngles;
            Vector3 mirrorSca = orgDpart.dpartTrans.localScale;
            needMirrorDpart.dpartTrans.localPosition = new Vector3(mirrorPos.x, mirrorPos.y, -mirrorPos.z);

            if (dpartParent.isMirrorCanNegativeScale)
            {
                needMirrorDpart.dpartTrans.localRotation = Quaternion.Euler(new Vector3(-mirrorRot.x, -mirrorRot.y, mirrorRot.z));
                needMirrorDpart.dpartTrans.localScale = new Vector3(mirrorSca.x, mirrorSca.y, -mirrorSca.z);
            }
            else
            {
                needMirrorDpart.dpartTrans.localScale = mirrorSca;
                needMirrorDpart.dpartTrans.localRotation = Quaternion.Euler(new Vector3(mirrorRot.x, -mirrorRot.y, mirrorRot.z));
                needMirrorDpart.dpartTrans.RotateAround(needMirrorDpart.dpartTrans.localPosition, Vector3.up, 180);
            }

            if (!isGroupDpart())
            {      
                needMirrorDpart.setMaterial(orgDpart.getMaterial());
                needMirrorDpart.setColor(orgDpart.getColor(1), 1);
                needMirrorDpart.setColor(orgDpart.getColor(2), 2);
            }

        }

        public virtual void onBuilderModeSelecting()
        {
            if (mirrorDpart != null)
            {
                syncMirrorTransform(this, mirrorDpart);
            }

            if (groupDpart != null)
            {
                GameObject[] gos = IRT.Selection.gameObjects;
                for (int i = 0; i < gos.Length; i++)
                {
                    if (gos[i].Equals(dpartObject))
                    {
                        gos[i] = groupDpart.dpartObject;
                        break;
                    }
                }
                IRT.Selection.objects = gos;
            }

            DrawLinkedLine();
        }   

        public virtual void onBuilderModeUnSelecting()
        {
            if (dpartParent != null)
            {
                //dpartParent.changeDpartChindrens(go => go.setOutline(false));
            }

            drawArc = false;
        }

        public virtual void LateUpdate()
        {

        }

        

        public void DrawLinkedLine()
        {
            if (drawArc)
            {
                return;
            }
            drawArc = true;
            if (linkedUid.Count > 0)
            {
                var coordinates = new List<ArcDrawer.VectorPair>();
                for (int i = 0; i < linkedUid.Count; i++)
                {
                    DpartParent[] allParents = GameObject.Find("/3D DParts Map").GetComponentsInChildren<DpartParent>(true);
                    for (int j = 0; j < allParents.Length; j++)
                    {
                        DpartParent dp = allParents[j];
                        Dpart dpD = dp.getDpart();
                        if (linkedUid.Contains(dpD.uid))
                        {
                            var vp = new ArcDrawer.VectorPair();
                            vp.a = dpartObject.transform.position;
                            vp.b = dp.transform.position;
                            coordinates.Add(vp);
                        }
                    }
                }
                ArcDrawer.instance.coordinates = coordinates;
                ArcDrawer.instance.Regenerate();
            }
            else
            {
                ArcDrawer.instance.coordinates = new List<ArcDrawer.VectorPair>();
                ArcDrawer.instance.Regenerate();
            }
        }     

        public void setMirrorDpart(Dpart mirroredDpart)
        {
            this.mirrorDpart = mirroredDpart;
        }

        public void cancelMirrorDpart()
        {
            if (mirrorDpart != null)
            {
                if (mirrorDpart.mirrorDpart != null)
                {
                    mirrorDpart.mirrorDpart = null;
                }
                mirrorDpart = null;
            }
        }

        public virtual void setMaterial(Material material)
        {
            if (material == null)
            {
                return;
            }

            this.material = material;
            dpartParent.changeDpartChindrens(go => go.setMaterial(material));
        }

        public Material getMaterial()
        {
            return material;
        }

        public string getMaterialName()
        {
            return material.name;
        }

        public virtual void setColor(Color color, int part)
        {
            if (color != null)
            {
                this.color[part - 1] = color;
                dpartParent.changeDpartChindrens(go => go.setColor(color, part));
            }
        }

        public Color getColor(int part)
        {
            return color[part - 1];
        }

        public virtual void setOutline(bool enable)
        {
            dpartParent.changeDpartChindrens(go => go.setOutline(enable));
        }

        /// <summary>
        /// 依附于哪个分类
        /// </summary>
        public string getAttributeCardName()
        {
            return attributeCardName;
        }

        public bool isGroupDpart()
        {
            return dpartId == -1;
        }

        public int getId()
        {
            return dpartId;
        }

        public bool equal(Dpart dpart)
        {
            return dpartId == dpart.getId();
        }

        public void setIdentifyId(int id)
        {
            identifyId = id;
        }

        public int getIdentifyId()
        {
            return identifyId;
        }

        public string getName()
        {
            return dpartName;
        }

        public GameObject getGameObject()
        {
            return dpartObject;
        }

        public Transform getTransform()
        {
            return dpartTrans;
        }

        public virtual void closeCollider(bool isClose)
        {
            dpartParent.changeDpartChindrens(go => go.closeCollider(isClose));
        }

        public virtual Vector3 getColliderClosestPoint(Vector3 point)
        {
            if (dpartChildCount == 1)
            {
                return dpartParent.getChild(0).getColliderClosestPoint(point);
            }
            else
            {
                Vector3 closestPoint = Vector3.zero;
                Vector3 childClosestPoint;
                float distance = float.MaxValue;
                for (int i = 0; i < dpartChildCount; i++)
                {
                    DpartChild dpartChild = dpartParent.getChild(i);
                    if (dpartChild.isContainCollider)
                    {
                        childClosestPoint = dpartChild.getColliderClosestPoint(point);
                        float childDistance = (childClosestPoint - point).sqrMagnitude;
                        if (distance > childDistance)
                        {
                            distance = childDistance;
                            closestPoint = childClosestPoint;
                        }
                    }
                }
                return closestPoint;
            }
        }

        /// <summary>
        /// （由引擎调用）克隆（实例化）方块.1
        /// </summary>
        public virtual Dpart clone(GameObject parentObject)
        {
            Dpart dpart = new Dpart(dpartId, parentObject);
            dpart.initDpart(dpartName, attributeCardName, modInfo);
            if (World.GameMode == World.GameMode_Assembler)
            {
                dpart.setMaterial(AttributeColor.selectedShareMaterialStatic.getMaterial());
                if (AttributeColor.selectColors != null)
                {
                    dpart.setColor(AttributeColor.selectColors[0], 1);
                    dpart.setColor(AttributeColor.selectColors[1], 2);
                }
            }

            return dpart;
        }

        public void set2DMapCoor(IPoint coor)
        {
            this.coor = coor;          
        }

        public IPoint get2DMapCoor()
        {
            return coor;
        }

        public bool isContainCoor(IPoint coor)
        {
            for (int i = 0; i < dpartChildCount; i++)
            {
                if (dpartParent.getChild(i).isContainCoor(coor))
                {
                    return true;
                }
            }
            return false;
        }

        public bool isSelect()
        {
            if (groupDpart != null)
            {
                return groupDpart.isSelect();
            }
            else
            {
                return isSelecting;
            }
        }

        public virtual void onBuilderModeCreate()
        {
            setLayer(8);
            if (material == null)
            {
                setMaterial(AttributeColor.selectedShareMaterialStatic.getMaterial());
            }
        }

        public virtual void onPoolerModeCreate()
        {
            setLayer(8);
            if (material == null && !isGroupDpart())
            {
                setMaterial(AttributeColor.selectedShareMaterialStatic.getMaterial());
            }
            foreach(var e in dpartObject.transform.GetComponentsInChildren<ExposeToEditor>())
            {
                Object.Destroy(e);
            }
        }      

        /// <summary>
        /// 建造模式下的保存信息流
        /// </summary>
        public virtual JsonWriter onBuilderModeSave(JsonWriter writer)
        {
            Vector3 pos = dpartTrans.localPosition;
            Vector3 rot = dpartTrans.localRotation.eulerAngles;
            Vector3 sca = dpartTrans.localScale;

            IUtils.keyValue2Writer(writer, "id", dpartId);
            IUtils.keyValue2Writer(writer, "uid", uid);
            IUtils.keyValue2Writer(writer, "mat", getMaterialName());
            IUtils.keyValue2Writer(writer, "pos", IUtils.vector3Serialize(pos));
            IUtils.keyValue2Writer(writer, "rot", IUtils.vector3Serialize(rot));
            IUtils.keyValue2Writer(writer, "sca", IUtils.vector3Serialize(sca));
            IUtils.keyValue2Writer(writer, "m", mirrorDpart != null);
            IUtils.keyValue2Writer(writer, "col1", color[0] == null ? "null" : ColorUtility.ToHtmlStringRGB(color[0]));
            IUtils.keyValue2Writer(writer, "col2", color[1] == null ? "null" : ColorUtility.ToHtmlStringRGB(color[1]));
            IUtils.keyValue2Writer(writer, "linked", IUtils.serializeIntArray(linkedUid.ToArray()));

            return writer;
        }

        /// <summary>
        /// 建造模式下的读取信息流
        /// </summary>
        public virtual void onBuilderModeLoad(JsonData dpartData, DpartsEngine dpartsEngine)
        {
            uid = IUtils.getJsonValue2Int(dpartData, "uid", getUid());
            if (uid >= gUid)
            {
                gUid = uid + 1;
            }
            dpartTrans.localPosition = IUtils.vector3Parse(IUtils.getJsonValue2String(dpartData, "pos"));
            dpartTrans.localRotation = Quaternion.Euler(IUtils.vector3Parse(IUtils.getJsonValue2String(dpartData, "rot")));
            dpartTrans.localScale = IUtils.vector3Parse(IUtils.getJsonValue2String(dpartData, "sca"));
            DpartMaterial material = DpartMaterialsManager.instance.getMaterialByName(IUtils.getJsonValue2String(dpartData, "mat"));
            if (material != null)
            {
                setMaterial(material.getMaterial());
            }

            string col1 = IUtils.getJsonValue2String(dpartData, "col1");
            string col2 = IUtils.getJsonValue2String(dpartData, "col2");
            if (col1 != null && !col1.Equals("null"))
            {
                Color color1 = IUtils.HexToColor(col1);
                setColor(color1, 1);
                if (World.GameMode == World.GameMode_Assembler && AttributeColor.instance != null)
                {
                    AttributeColor.instance.addUsedColorCell(color1);
                }
            }

            if (col2 != null && !col2.Equals("null"))
            {
                Color color2 = IUtils.HexToColor(col2);
                setColor(color2, 2);
                if (World.GameMode == World.GameMode_Assembler && AttributeColor.instance != null)
                {
                    AttributeColor.instance.addUsedColorCell(color2);
                }
            }

            if (!temporary && IUtils.getJsonValue2Bool(dpartData, "m"))
            {
                Dpart md = dpartsEngine.createMirrorDPart(this);
                md.onBuilderModeCreate();
            }
            linkedUid = new List<int>(IUtils.unserializeIntArrayWithDefault(IUtils.getJsonValue2String(dpartData, "linked")));
        }

        public virtual void clear()
        {
            Object.Destroy(dpartObject);
        }
    }
}


