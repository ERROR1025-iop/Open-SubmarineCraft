using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    public string target = "Menu";
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            SceneManager.LoadScene(target);
        });
    }
}
