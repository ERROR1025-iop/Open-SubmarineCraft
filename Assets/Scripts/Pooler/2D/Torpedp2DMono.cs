using UnityEngine;

namespace Scraft
{
    public class Torpedp2DMono : MonoBehaviour
    {

        float speed = 7;

        void Start()
        {

        }

        void Update()
        {
            transform.localPosition = new Vector3(transform.localPosition.x - speed * Time.deltaTime, transform.localPosition.y, -1);

            if (transform.localPosition.x <= -200f)
            {
                Destroy(this.gameObject);
            }
        }
    }
}