using System.Collections;
using UnityEngine;

namespace Scraft
{
    public class EntityFoam : MonoBehaviour
    {
        EntityFoamSpawn entityFoamSpawn;


        ParticleSystem particleSystem;
        ParticleSystem.ShapeModule shape;
        float defaultRadius;
        float offsetX;
        bool enable;
        float randomRange;

        void Start()
        {
            particleSystem = GetComponent<ParticleSystem>();
            shape = particleSystem.shape;
            shape.radius = defaultRadius;
            particleSystem.startSpeed = 0.1f;
            particleSystem.emissionRate = 7;

            StartCoroutine(reset());
        }

        public void initialized(EntityFoamSpawn entityFoamSpawn, float radius, float randomRange, float offsetX)
        {
            this.entityFoamSpawn = entityFoamSpawn;
            defaultRadius = radius;
            this.randomRange = randomRange;
            this.offsetX = offsetX;
        }

        IEnumerator reset()
        {
            while (true)
            {
                if (entityFoamSpawn == null)
                {
                    Destroy(gameObject);
                    break;
                }

                transform.position = new Vector3(entityFoamSpawn.worldFrontPoint.x + Random.Range(-randomRange, randomRange), 0, entityFoamSpawn.worldFrontPoint.z + Random.Range(-randomRange, randomRange)) + entityFoamSpawn.transform.forward * offsetX;
                transform.eulerAngles = new Vector3(0, entityFoamSpawn.angle, 0);
                particleSystem.enableEmission = entityFoamSpawn.enableEmission;
                yield return new WaitForSeconds(0.3f);
            }
        }
    }
}