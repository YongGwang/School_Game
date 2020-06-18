using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For some test.
/// </summary>
/// =======================================================
/// Author : 2020/02/14 Sa
/// History Log :
///		2020/02/14(Sa) Initial
public class Tester : MonoBehaviour
{
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JoyConInputManager.Recenter(false);
            Debug.Log("Rr");
        }
    }
}
