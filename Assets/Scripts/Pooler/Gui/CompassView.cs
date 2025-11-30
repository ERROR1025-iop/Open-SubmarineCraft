using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    public class CompassView: MonoBehaviour
    {
        Transform compassTrans;
        Transform compassArrayTrans;

        Transform playerTrans;

        void Start()
        {
            compassTrans = transform;
            compassArrayTrans = transform.GetChild(0);
            playerTrans = MainSubmarine.instance.gameObject.transform;
        }

        void Update()
        {
            float playerYAngle = playerTrans.eulerAngles.y;
            compassTrans.localEulerAngles = new Vector3(0, 0, playerYAngle);

            compassArrayTrans.localEulerAngles = new Vector3(0, 0, -playerYAngle);
        }
    }
}