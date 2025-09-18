using UnityEngine;

namespace Scraft
{
    public class FollowSub : MonoBehaviour
    {
        Transform selfSubTrans;

        void Start()
        {
            selfSubTrans = MainSubmarine.transform;
        }


        void Update()
        {
            transform.localPosition = selfSubTrans.position;
        }
    }
}