using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameController : MonoBehaviour
{
    void Start()
    {
        QualitySettings.vSyncCount = 0;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Application.targetFrameRate = 30;
        }
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            Application.targetFrameRate = 60;
        }
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            Application.targetFrameRate = 90;
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            Application.targetFrameRate = 120;
        }
    }
}
