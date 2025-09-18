using System.Collections.Generic;



namespace Scraft
{
    public class DpartMaterialsManager
    {

        public static DpartMaterialsManager instance;

        public List<DpartMaterial> MaterialsArray;

        public DpartMaterialsManager()
        {
            instance = this;
            MaterialsArray = new List<DpartMaterial>();
            registerMaterials();
            AttributeColor.selectedShareMaterialStatic = getMaterialById(0);
        }

        void registerMaterials()
        {
            registerMaterial("normal");
            registerMaterial("emission");
            registerMaterial("transparent");
        }

        void registerMaterial(string name)
        {
            DpartMaterial color = new DpartMaterial(name, MaterialsArray.Count);
            MaterialsArray.Add(color);
        }

        public int getMaterialCount()
        {
            return MaterialsArray.Count;
        }

        public DpartMaterial getMaterialById(int id)
        {
            return MaterialsArray[id];
        }

        public DpartMaterial getMaterialByName(string name)
        {
            DpartMaterial color;
            for (int i = 0; i < MaterialsArray.Count; i++)
            {
                color = MaterialsArray[i];
                if (color != null && color.getName().Equals(name))
                {
                    return color;
                }
            }
            return null;
        }
    }
}