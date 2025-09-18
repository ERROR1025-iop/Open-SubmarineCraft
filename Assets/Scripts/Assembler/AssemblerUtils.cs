using LitJson;
using Scraft.DpartSpace;
using UnityEngine;


namespace Scraft
{
    public class AssemblerUtils
    {
        static public Texture2D createDpartThumbnailImage(DpartsEngine dpartsEngine, string folderFullName, string savePath, string dpartName)
        {
            GroupDpart groupDpart = loadGroupDpart(dpartsEngine, Assembler.preShotParent, folderFullName, dpartName);
            groupDpart.setVisible(true);
            Texture2D texture2D = createGameObjectThumbnailImage(Assembler.preShotParent, Assembler.preShotCamera, savePath + dpartName + ".thu", new Rect(0, 0, 800, 480));
            groupDpart.clear();
            return texture2D;
        }

        static public GroupDpart unGroupDpartIfItIsOneGroup(GroupDpart groupDpart)
        {
            int childrenCount = groupDpart.groupChildrens.Count;
            if (childrenCount == 1)
            {
                Dpart dpart = groupDpart.groupChildrens[0];
                if (dpart != null && dpart.isGroupDpart())
                {
                    GroupDpart newGroupDpart = dpart as GroupDpart;
                    newGroupDpart.getTransform().SetParent(groupDpart.getTransform().parent);
                    newGroupDpart.setGroupDpart(null);
                    groupDpart.groupChildrens.Clear();
                    groupDpart.clear();
                    return newGroupDpart;
                }
            }
            return groupDpart;
        }

        static public GroupDpart loadGroupDpart(DpartsEngine dpartsEngine, GameObject parent, string folderFullName, string dpartName)
        {
            GroupDpart groupDpart = new GroupDpart(-1, parent);
            groupDpart.initGroupDpart(dpartName);

            JsonData jsonData = JsonMapper.ToObject(IUtils.readFromTxt(folderFullName));

            int blocksCount = IUtils.getJsonValue2Int(jsonData, "count");
            JsonData blocksArrData = jsonData["dparts"];

            JsonData dpartData;
            Dpart dpart;

            for (int i = 0; i < blocksCount; i++)
            {
                dpartData = blocksArrData[i];

                int dpartId = IUtils.getJsonValue2Int(dpartData, "id");
                if (dpartId == -1)
                {
                    GroupDpart childGroupDpart = new GroupDpart(-1, groupDpart.getGameObject());
                    childGroupDpart.initGroupDpart("group_x" + i);
                    dpart = childGroupDpart;
                }
                else
                {
                    dpart = DpartsManager.instance.getDPartById(dpartId).clone(groupDpart.getGameObject());
                }
                dpart.temporary = true;
                dpart.onBuilderModeLoad(dpartData, dpartsEngine);
                dpart.onBuilderModeCreate();
                groupDpart.addGroupChildrens(dpart);
            }

            groupDpart.resizeCenterPoint();
            return groupDpart;
        }

        static public Texture2D createGameObjectThumbnailImage(GameObject group, Camera camera, string savePath, Rect rect)
        {
            Bounds bounds = IUtils.GetBounds(group);
            camera.transform.LookAt(bounds.center);
            camera.orthographicSize = bounds.extents.magnitude;
            Texture2D texture2D = IUtils.captureScreen(camera, rect);
            IUtils.saveTexture2D2SD(texture2D, savePath);
            return texture2D;
        }
    }
}