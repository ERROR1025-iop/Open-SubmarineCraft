using UnityEngine;


namespace Scraft
{
    public class AssemblerWeightCenterSphere : MonoBehaviour
    {
        static public AssemblerWeightCenterSphere instance;

        public GameObject dpartParent;

        Vector3 center;

        void Start()
        {
            instance = this;
        }


        void Update()
        {
            if (Assembler.IS_Show_WeighCenter && IUtils.weigthCenterOfGameObjects(dpartParent, out center, Assembler.massOffset))
            {
                transform.position = center;
            }
            else
            {
                transform.position = new Vector3(0, 9999, 0);
            }
        }
    }
}