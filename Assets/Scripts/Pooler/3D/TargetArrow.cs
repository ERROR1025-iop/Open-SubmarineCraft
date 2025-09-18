using UnityEngine;

namespace Scraft
{
    public class TargetArrow : MonoBehaviour
    {


        void Update()
        {
            transform.Rotate(Vector3.right, 50 * Time.deltaTime);
        }
    }
}