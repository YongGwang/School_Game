using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AnyKeyToNextScene
/// </summary>
/// =======================================================
/// Author : 2019/02/05 Sa
/// History Log :
///		2019/02/05(Sa) Initial
public class AnyKeyToNextScene : MonoBehaviour
{
    private void Update()
    {
        if (Input.anyKey || JoyConInputManager.GetJoyConAnykeyDown())
        {
			Utility.MoveNextScene();
        }
    }
}