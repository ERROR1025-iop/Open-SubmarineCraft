using System.Collections;
using UnityEngine;

namespace Scraft
{
    public class SubFoamSpawn : MonoBehaviour
    {
        static public Vector3 frontPoint;
        public GameObject foam;
        public GameObject foam_normal;
        public GameObject foam_wave;

        [Range(0, 15)]
        public float emission;
        [Range(0, 1)]
        public float intervalTime;

        static public float m_emission;

        Vector3 shipStartLocal;
        int hitLayer;
        float hitDistance;
        bool isInit = false;

        static public float angle;
        static public bool enableEmission;
        static public float startSpeed;

        void Start()
        {
            hitLayer = 1 << 8;
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(frontPoint, 0.2f);
        }

        void initialized()
        {
            Vector3 center = MainSubmarine.bounds.center;
            shipStartLocal = new Vector3(MainSubmarine.bounds.min.x - 1, center.y, center.z);
            hitDistance = MainSubmarine.bounds.size.x + 1;
            StartCoroutine(addFoam());

            isInit = true;
        }

        IEnumerator addFoam()
        {
            float shipX = MainSubmarine.bounds.size.x;
            int count = (int)(shipX / 0.2f);

            for (int i = 0; i < 3; i++)
            {
                addFoam(0);
                yield return new WaitForSeconds(intervalTime);
            }

            for (int i = 0; i < count; i++)
            {
                addFoam(i * 0.2f);
                yield return new WaitForSeconds(intervalTime);
            }
        }

        void addFoam(float offset)
        {
            //SubFoam subFoam = (Instantiate(foam) as GameObject).GetComponent<SubFoam>();
            SubFoam subFoam = (Instantiate(GameSetting.waveMode > 0 ? foam_wave : foam_normal) as GameObject).GetComponent<SubFoam>();
            subFoam.transform.SetParent(GameObject.Find("runtime_gen").transform, true);
            subFoam.gameObject.layer = 8;
            subFoam.offsetX = offset;
        }

        void FixedUpdate()
        {
            if (!isInit)
            {
                initialized();
            }

            Vector3 right = new Vector3(transform.right.x, 0, transform.right.z);
            Vector3 shipStart = transform.TransformPoint(shipStartLocal);
            Vector3 rayStart = new Vector3(shipStart.x, 0, shipStart.z);
            Debug.DrawRay(rayStart, right);
            Ray ray = new Ray(rayStart, right);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, hitDistance, hitLayer))
            {
                frontPoint = hit.point;

                Vector3 velocityPlane = new Vector3(MainSubmarine.rigidbody.velocity.x, 0, MainSubmarine.rigidbody.velocity.z);
                angle = Vector3.Angle(velocityPlane, Vector3.back);
                enableEmission = MainSubmarine.speed > 0.5f;
                startSpeed = MainSubmarine.speed > 2 ? 0.02f : MainSubmarine.speed * 0.01f;
                m_emission = emission;
            }
            else
            {
                enableEmission = false;
            }
        }
    }
}