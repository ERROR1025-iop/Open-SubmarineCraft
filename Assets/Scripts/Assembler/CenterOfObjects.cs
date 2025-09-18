using UnityEngine;


namespace Scraft
{
    public class CenterOfObjects : MonoBehaviour
    {
        public Vector3 center;

        void Start()
        {

        }


        void Update()
        {
            center = IUtils.centerOfGameObjects(gameObject);
        }

        private void OnDrawGizmosSelected()
        {

            Gizmos.DrawWireSphere(center, 0.2f);
        }
    }
}