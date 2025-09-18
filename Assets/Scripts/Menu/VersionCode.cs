using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Scraft
{
    public class VersionCode : MonoBehaviour
    {


        void Start()
        {
            transform.GetComponent<Text>().text = GameSetting.version;
        }

    }

}