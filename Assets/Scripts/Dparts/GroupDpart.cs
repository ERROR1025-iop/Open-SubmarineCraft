using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using Battlehub.RTCommon;

namespace Scraft.DpartSpace
{
    public class GroupDpart : Dpart
    {
        public List<Dpart> groupChildrens;
        Transform groupsParent;

        public GroupDpart(int id, GameObject parentObject) : base(id, parentObject)
        {
            groupsParent = parentObject.transform;
        }

        public void initGroupDpart(string name)
        {
            this.dpartName = name;
            dpartObject = new GameObject(name);
            dpartObject.AddComponent<ExposeToEditor>();
            DpartParent dpartParent = dpartObject.AddComponent<DpartParent>();
            dpartParent.init(this);
            dpartTrans = dpartObject.transform;
            dpartTrans.SetParent(groupsParent);
            dpartTrans.localPosition = Vector3.zero;
            groupChildrens = new List<Dpart>();

        }

        public override Dpart clone(GameObject parentObject)
        {
            GroupDpart groupDpart = new GroupDpart(-1, parentObject);
            groupDpart.initGroupDpart(getName());
            Dpart child;
            for (int i = 0; i < groupChildrens.Count; i++)
            {
                child = groupChildrens[i].clone(groupDpart.getGameObject());
                child.onBuilderModeCreate();
                child.copyTransform(groupChildrens[i]);
                child.setGroupDpart(groupDpart);
                groupDpart.addGroupChildrens(child);

                if (!child.isGroupDpart())
                {
                    child.setMaterial(groupChildrens[i].getMaterial());
                    child.setColor(groupChildrens[i].getColor(1), 1);
                    child.setColor(groupChildrens[i].getColor(2), 2);
                }
            }
            return groupDpart;
        }

        public void resizeCenterPoint()
        {
            Vector3 centerPoint = Vector3.zero;
            Vector3 position = Vector3.zero;
            int count = groupChildrens.Count;
            for (int i = 0; i < count; i++)
            {
                position = groupChildrens[i].getTransform().localPosition;
                centerPoint.x += position.x;
                centerPoint.y += position.y;
                centerPoint.z += position.z;
            }
            centerPoint = centerPoint / count;
            for (int i = 0; i < groupChildrens.Count; i++)
            {
                groupChildrens[i].getTransform().localPosition -= centerPoint;
            }

        }


        public override void setLayer(int layer)
        {
            changeChindrenDparts(go => go.setLayer(layer));
        }

        public void addGroupChildrens(Dpart childrensDpart)
        {
            groupChildrens.Add(childrensDpart);
        }

        public override void closeCollider(bool isClose)
        {
            changeChindrenDparts(go => go.closeCollider(isClose));
        }

        public override void onBuilderModeCreate()
        {
            changeChindrenDparts(go => go.onBuilderModeCreate());
        }

        public override void setMaterial(Material material)
        {
            this.material = material;
            changeChindrenDparts(go => go.setMaterial(material));
        }

        public override void setColor(Color color, int part)
        {
            this.color[part - 1] = color;
            changeChindrenDparts(go => go.setColor(color, part));
        }

        public override Vector3 getColliderClosestPoint(Vector3 point)
        {
            Vector3 closestPoint = Vector3.zero;
            Vector3 childClosestPoint;
            float distance = float.MaxValue;
            int count = groupChildrens.Count;
            for (int i = 0; i < count; i++)
            {
                childClosestPoint = groupChildrens[i].getColliderClosestPoint(point);
                float childDistance = (childClosestPoint - point).sqrMagnitude;
                if (distance > childDistance)
                {
                    distance = childDistance;
                    closestPoint = childClosestPoint;
                }
            }
            return closestPoint;
        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            setOutline(isSelect());
        }

        public override void setOutline(bool enable)
        {
            changeChindrenDparts(go => go.setOutline(enable));
        }

        public override void syncMirrorTransform(Dpart orgDpart, Dpart needMirrorDpart)
        {
            //base.syncMirrorTransform(orgDpart, needMirrorDpart);
            changeChindrenDparts(go => go.syncMirrorTransform(orgDpart, needMirrorDpart));
            syncMirrorMaterial(orgDpart, needMirrorDpart);            
        }

        public void syncMirrorMaterial(Dpart orgDpart, Dpart needMirrorDpart)
        {
            int count = groupChildrens.Count;
            GroupDpart org = orgDpart as GroupDpart;
            GroupDpart mir = needMirrorDpart as GroupDpart;
            for (int i = 0; i < count; i++)
            {
                if (mir.getChild(i).isGroupDpart())
                {
                    syncMirrorMaterial(org.getChild(i), mir.getChild(i));
                }
                else
                {
                    mir.getChild(i).setMaterial(org.getChild(i).getMaterial());
                    mir.getChild(i).setColor(org.getChild(i).getColor(1), 1);
                    mir.getChild(i).setColor(org.getChild(i).getColor(2), 2);
                }
            }
        }

        public override JsonWriter onBuilderModeSave(JsonWriter writer)
        {
            Vector3 pos = dpartTrans.localPosition;
            Vector3 rot = dpartTrans.localRotation.eulerAngles;
            Vector3 sca = dpartTrans.localScale;

            IUtils.keyValue2Writer(writer, "id", dpartId);
            IUtils.keyValue2Writer(writer, "pos", IUtils.vector3Serialize(pos));
            IUtils.keyValue2Writer(writer, "rot", IUtils.vector3Serialize(rot));
            IUtils.keyValue2Writer(writer, "sca", IUtils.vector3Serialize(sca));
            IUtils.keyValue2Writer(writer, "m", mirrorDpart != null);

            IUtils.keyValue2Writer(writer, "count", groupChildrens.Count);

            writer.WritePropertyName("group");
            writer.WriteObjectStart();

            for (int i = 0; i < groupChildrens.Count; i++)
            {
                writer.WritePropertyName("" + i);
                writer.WriteObjectStart();
                groupChildrens[i].onBuilderModeSave(writer);
                writer.WriteObjectEnd();
            }
            writer.WriteObjectEnd();
            return writer;
        }

        public override void onBuilderModeLoad(JsonData dpartData, DpartsEngine dpartsEngine)
        {
            dpartTrans.localPosition = IUtils.vector3Parse(IUtils.getJsonValue2String(dpartData, "pos"));
            dpartTrans.localRotation = Quaternion.Euler(IUtils.vector3Parse(IUtils.getJsonValue2String(dpartData, "rot")));
            dpartTrans.localScale = IUtils.vector3Parse(IUtils.getJsonValue2String(dpartData, "sca"));
            int groupCount = IUtils.getJsonValue2Int(dpartData, "count");

            JsonData groupArrData = dpartData["group"];
            JsonData childData;
            Dpart child;
            for (int i = 0; i < groupCount; i++)
            {
                childData = groupArrData[i];
                int dpartId = IUtils.getJsonValue2Int(childData, "id");
                if (dpartId == -1)
                {
                    GroupDpart groupDpart = new GroupDpart(-1, getGameObject());
                    groupDpart.initGroupDpart("group_" + i);
                    child = groupDpart;
                }
                else
                {
                    child = DpartsManager.instance.getDPartById(dpartId).clone(getGameObject());
                }
                child.onBuilderModeLoad(childData, dpartsEngine);
                child.onBuilderModeCreate();
                child.setGroupDpart(this);
                addGroupChildrens(child);
            }

            if (!temporary && IUtils.getJsonValue2Bool(dpartData, "m"))
            {
                Dpart md = dpartsEngine.createMirrorDPart(this);
                md.onBuilderModeCreate();
            }

        }

        public override void clear()
        {
            changeChindrenDparts(go => go.clear());
            base.clear();
        }

        public Dpart getChild(int index)
        {
            return groupChildrens[index];
        }

        public void changeChindrenDparts(System.Action<Dpart> execute)
        {
            for (int i = 0; i < groupChildrens.Count; i++)
            {
                execute(groupChildrens[i]);
            }
        }
    }
}
