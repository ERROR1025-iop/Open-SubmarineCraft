using UnityEngine;


namespace Scraft
{
    public class DpartMaterial
    {

        int materialId;
        string name;
        Material material;

        public DpartMaterial(string name, int rank)
        {
            this.name = name;
            materialId = rank;
            material = Resources.Load("dparts/ColorMat/" + name, typeof(Material)) as Material;
        }

        public int getMaterialId()
        {
            return materialId;
        }

        public Material getMaterial()
        {
            return material;
        }

        public string getName()
        {
            return name;
        }
    }
}