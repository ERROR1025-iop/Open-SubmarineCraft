using System.Collections;
using UnityEngine;


namespace Scraft
{
    public class SubFoam : MonoBehaviour
    {
        public float offsetX;
        ParticleSystem particleSystem;
        ParticleSystem.ShapeModule shape;
        float defaultRadius;

        void Start()
        {
            particleSystem = GetComponent<ParticleSystem>();
            shape = particleSystem.shape;
            defaultRadius = MainSubmarine.bounds.size.z * 0.5f - 0.2f;
            shape.radius = defaultRadius;
            StartCoroutine(reset());
        }

        IEnumerator reset()
        {
            while (true)
            {
                float randomRange = Mathf.Clamp(MainSubmarine.speed, 0, 50) * 0.01f;
                transform.position = new Vector3(SubFoamSpawn.frontPoint.x + Random.Range(-randomRange, randomRange), 0, SubFoamSpawn.frontPoint.z + Random.Range(-randomRange, randomRange)) + MainSubmarine.transform.right * offsetX;
                transform.eulerAngles = new Vector3(0, SubFoamSpawn.angle, 0);
                particleSystem.enableEmission = SubFoamSpawn.enableEmission;
                particleSystem.startSpeed = SubFoamSpawn.startSpeed;
                particleSystem.emissionRate = SubFoamSpawn.m_emission;
                yield return new WaitForSeconds(1.75f);
            }
        }

    }
}

