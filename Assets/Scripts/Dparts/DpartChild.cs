using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub.RTCommon;

namespace Scraft.DpartSpace
{
    public class DpartChild : MonoBehaviour
    {

        public bool CanChangeMaterial = true;
        public bool isCombineRenderMesh = true;
        public bool isContainCollider = true;
        public bool isCombineCollider = true;
        public int changeColorPart = 0;

        [HideInInspector]
        public Collider childCollider;
        [HideInInspector]
        public Bounds colliderBounds;
        [HideInInspector]
        public MeshFilter meshFilter;
        Dpart dpart;
        Renderer childRenderer;
        QOutline QOutline;

        void Start()
        {
            if (World.GameMode == World.GameMode_Assembler && IRT.Selection != null)
            {
                IRT.Selection.SelectionChanged += OnSelectionChanged;
            }
        }

        public void init(Dpart dpart)
        {
            this.dpart = dpart;
            childRenderer = GetComponent<Renderer>();
            if (isContainCollider)
            {
                childCollider = GetComponent<Collider>();
                colliderBounds = childCollider.bounds;
            }
            if (changeColorPart > 0)
            {
                meshFilter = GetComponent<MeshFilter>();
                QOutline = gameObject.AddComponent<QOutline>();
                QOutline.OutlineColor = new Color(0, 1, 1);
                QOutline.OutlineWidth = 10;
                QOutline.enabled = false;
            }

            setOutline(false);
        }

        public void closeCollider(bool isClose)
        {
            if (isContainCollider)
            {
                childCollider.enabled = !isClose;
            }
        }

        public void setOutline(bool enable)
        {
            if (QOutline != null)
            {
                QOutline.enabled = enable;
            }
        }

        public Vector3 getColliderClosestPoint(Vector3 point)
        {
            return childCollider.ClosestPoint(point);
        }

        public void setMaterial(Material material)
        {
            if (CanChangeMaterial)
            {
                Material[] mats = childRenderer.materials;
                mats[0] = material;
                childRenderer.materials = mats;
            }
        }

        public void setColor(Color color, int part)
        {
            if (changeColorPart != part)
            {
                return;
            }

            Mesh mesh = new Mesh();
            Mesh orgMesh = meshFilter.mesh;
            int subCount = orgMesh.subMeshCount;
            mesh.vertices = orgMesh.vertices;
            for (int i = 0; i < subCount; i++)
            {
                mesh.SetTriangles(orgMesh.GetTriangles(i), i);
            }
            Color[] colors = new Color[mesh.vertexCount];
            int length = colors.Length;
            for (int i = 0; i < length; i++)
            {
                colors[i] = color;
            }
            //Debug.Log(color);
            mesh.colors = colors;
            mesh.uv = orgMesh.uv;
            mesh.RecalculateNormals();
            meshFilter.mesh = mesh;
        }

        public bool isContainCoor(IPoint coor)
        {
            if (isContainCollider)
            {
                Vector3 pos = coor.mapIPoint2WordVector(transform.localPosition.z + 100);
                Vector3 pos2 = transform.InverseTransformPoint(pos);
                return colliderBounds.Contains(pos2);
            }
            return false;
        }

        public virtual void setLayer(int layer)
        {
            gameObject.layer = layer;
        }

        public Dpart getDpart()
        {
            return dpart;
        }

        void OnSelectionChanged(Object[] unselectedObjects)
        {
            if (this != null && IRT.Selection.activeGameObject != null && dpart != null)
            {
                GameObject[] go = IRT.Selection.gameObjects;
                int count = go.Length;
                for (int i = 0; i < count; i++)
                {
                    if (go[i].Equals(gameObject))
                    {
                        go[i] = dpart.getGameObject();
                        break;
                    }
                }
                IRT.Selection.objects = go;
            }
        }
    }
}
