using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputs : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        // 检查鼠标是否连接
        if (Mouse.current != null)
        {
            // 读取鼠标位置
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Debug.Log("鼠标位置：" + mousePosition);
        }
    }
}
