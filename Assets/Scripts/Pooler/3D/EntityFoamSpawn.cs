using System.Collections;
using UnityEngine;

namespace Scraft
{
    public class EntityFoamSpawn : MonoBehaviour
    {

        public GameObject foam;
        public MeshFilter meshFilter;

        [Range(0, 15)]
        public float emission;
        [Range(0, 1)]
        public float intervalTime;
        [Range(0, 1)]
        public float radius;
        [Range(0, 1)]
        public float randomRange;
        public int startSpawn;
        public float createMaxDeep;
        public Vector3 frontPoint;

        [HideInInspector]
        public float angle;
        [HideInInspector]
        public bool enableEmission;
        [HideInInspector]
        public Vector3 worldFrontPoint;

        bool isInit = false;

        void Start()
        {

        }

        void initialized()
        {
            StartCoroutine(addFoam());

            isInit = true;
        }

        IEnumerator addFoam()
        {
            float shipX = meshFilter.mesh.bounds.size.x;
            int count = (int)(shipX / 0.1f);

            for (int i = 0; i < startSpawn; i++)
            {
                addFoam(0);
                yield return new WaitForSeconds(intervalTime);
            }

            for (int i = 0; i < count; i++)
            {
                addFoam(i * 0.1f);
                yield return new WaitForSeconds(intervalTime);
            }
        }

        void addFoam(float offset)
        {
            EntityFoam entityFoam = (Instantiate(foam) as GameObject).GetComponent<EntityFoam>();
            entityFoam.gameObject.layer = 8;
            entityFoam.initialized(this, radius, randomRange, offset);
        }

        void FixedUpdate()
        {
            if (!isInit)
            {
                initialized();
            }

            enableEmission = transform.position.y > -createMaxDeep;
            if (enableEmission)
            {
                worldFrontPoint = transform.TransformPoint(frontPoint);
            }
        }
    }
}