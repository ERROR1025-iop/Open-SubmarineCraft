using UnityEngine;

namespace Scraft
{
    public class HeatMesh : MonoBehaviour
    {
        int xSize, ySize;
        private Vector3[] vertices;


        void Start()
        {
            xSize = Pooler.instance.getShipRect().x;
            ySize = Pooler.instance.getShipRect().y;

            Generate();
        }

        void Generate()
        {
            vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        }

    }
}
