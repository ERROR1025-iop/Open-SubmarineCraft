using UnityEngine;

namespace Scraft
{
    public class SubStabilization : MonoBehaviour
    {
        public bool openGravityStabilization = true;

        public float seaLevel = 0f;
        float lerpSpeed = 5;

        new Rigidbody rigidbody;

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
        }


        void FixedUpdate()
        {
            Vector3 roto = transform.rotation.eulerAngles;
            if (roto.x != 0 || roto.z != 0)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, roto.y, 0)), lerpSpeed * Time.deltaTime);
            }

            if (openGravityStabilization)
            {
                rigidbody.useGravity = transform.localPosition.y > seaLevel;
            }

        }
    }
}