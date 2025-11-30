using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// 编辑器工具：将指定 GameObject 下所有包含 Mesh 的子物体提升为其直接子对象，并删除无用节点。
/// </summary>
public static class OrganizeMeshHierarchyEditor
{
    [MenuItem("Tools/Organize Mesh Children", true)]
    private static bool ValidateOrganizeMeshChildren()
    {
        // 只有在选中一个 GameObject 时才启用菜单
        return Selection.activeGameObject != null;
    }

    [MenuItem("Tools/Organize Mesh Children")]
    private static void OrganizeMeshChildren()
    {
        GameObject root = Selection.activeGameObject;

        if (root == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a GameObject.", "OK");
            return;
        }

        List<GameObject> meshObjects = new List<GameObject>();

        // 递归收集所有包含 Mesh 的 GameObject
        CollectMeshObjects(root.transform, meshObjects);

        // 开始修改场景层级前，开启 Undo 记录（支持撤销）
        Undo.RegisterFullObjectHierarchyUndo(root, "Organize Mesh Children");

        // 存储需要删除的 GameObject（所有不含 Mesh 的）
        List<GameObject> toDelete = new List<GameObject>();

        // 遍历所有子对象，标记不含 Mesh 的为待删除
        GetChildrenRecursively(root.transform, (Transform t) =>
        {
            GameObject go = t.gameObject;
            if (!meshObjects.Contains(go))
            {
                toDelete.Add(go);
            }
        });

        // 先将所有含 Mesh 的对象移到 root 下作为直接子对象
        foreach (GameObject meshObj in meshObjects)
        {
            if (meshObj.transform.parent != root.transform)
            {
                Undo.SetTransformParent(meshObj.transform, root.transform, "Reparent Mesh Object");
            }
        }

        // 删除所有不含 Mesh 的 GameObject
        foreach (GameObject obj in toDelete)
        {
            if (obj != null)
            {
                Object objToDestroy = obj;
                if (Application.isBatchMode)
                {
                    Object.DestroyImmediate(objToDestroy);
                }
                else
                {
                    Object.DestroyImmediate(objToDestroy, false); // false 表示不包含 prefab 实例的修改
                }
            }
        }

        Debug.Log($"Organized '{root.name}'. Kept {meshObjects.Count} mesh objects.");
        EditorUtility.SetDirty(root); // 标记场景修改
    }

    /// <summary>
    /// 收集所有包含 MeshFilter 或 SkinnedMeshRenderer 的 GameObject
    /// </summary>
    private static void CollectMeshObjects(Transform transform, List<GameObject> list)
    {
        // 检查当前对象是否有 Mesh 组件
        if (HasMeshComponent(transform.gameObject))
        {
            list.Add(transform.gameObject);
        }

        // 递归遍历子对象
        for (int i = 0; i < transform.childCount; i++)
        {
            CollectMeshObjects(transform.GetChild(i), list);
        }
    }

    /// <summary>
    /// 遍历所有子对象（递归）
    /// </summary>
    private static void GetChildrenRecursively(Transform parent, System.Action<Transform> action)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            action(child);
            GetChildrenRecursively(child, action);
        }
    }

    /// <summary>
    /// 判断 GameObject 是否包含有效的 Mesh 组件
    /// </summary>
    private static bool HasMeshComponent(GameObject go)
    {
        MeshFilter mf = go.GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
            return true;

        SkinnedMeshRenderer smr = go.GetComponent<SkinnedMeshRenderer>();
        if (smr != null && smr.sharedMesh != null)
            return true;

        return false;
    }
}