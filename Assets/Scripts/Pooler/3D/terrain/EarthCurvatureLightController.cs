using Scraft;
using UnityEngine;

[RequireComponent(typeof(Transform), typeof(Light))]
public class EarthCurvatureLightController : MonoBehaviour
{
    Light cachedLight;

    [Tooltip("Distance at which the light reaches zero intensity")]
    public float maxDistance = 50f;

    float originalIntensity;
    Transform target;

    void Start()
    {
        cachedLight = GetComponent<Light>();
        originalIntensity = cachedLight.intensity;
        var camObj = MainSubmarine.instance.gameObject;
        target = camObj.transform;

        if (maxDistance <= 0f) maxDistance = 0.0001f;
    }

    void Update()
    {        
        float dist = Vector3.Distance(transform.position, target.position);
        float t = Mathf.Clamp01(dist / maxDistance);
        float newIntensity = Mathf.Lerp(originalIntensity, 0f, t);
        cachedLight.intensity = newIntensity;
    }
}