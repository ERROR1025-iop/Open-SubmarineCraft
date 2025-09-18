using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblerDirectionalLight : MonoBehaviour
{

    public Transform cameraMain;

    void Update()
    {
        transform.rotation = cameraMain.rotation;
    }
}
