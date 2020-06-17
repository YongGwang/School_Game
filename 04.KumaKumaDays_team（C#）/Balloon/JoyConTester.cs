using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// For test joy-con
/// </summary>
/// Author : 2019/12/15 Sa
public class JoyConTester : MonoBehaviour
{
    Vector3 prevAcc = Vector3.zero;
    const float MOVE_PER_CLOCK = 0.01f;
    Vector3 acc;

    private void Start()
    {
    }

    private void Update()
    {
        // If left joycon, inverse the up down direction
        var gryo = JoyConInputManager.GetJoyConGyro(false);
        acc = JoyConInputManager.GetJoyConAccel(false);
        transform.localRotation = JoyConInputManager.GetJoyConOrientation(false);
        //var rotZ = TransformUtils.GetInspectorRotation(transform).z;
        //if (rotZ < 0 && Mathf.Abs(rotZ + 90.0f) > 22) return;
        //if (rotZ > 0 && Mathf.Abs(rotZ - 90.0f) > 22) return;
        //roundAccX = Mathf.Round(acc.x * 10.0f) / 10.0f;
        //transform.position = new Vector3(transform.position.x, transform.position.y + roundAccX * 12.0f * Time.deltaTime, transform.position.z);
        //Debug.Log(gryo);
        //Vector3 presentAcc = JoyConInputManager.GetJoyConAccel(false);
        //transform.Translate((prevAcc - presentAcc) * Time.deltaTime * speed);
        //prevAcc = presentAcc;
    }

    private void OnGUI()
    {
        //var style = GUI.skin.GetStyle("label");
        //style.fontSize = 22;
        //GUILayout.Label("accX:" + roundAccX);
    }
}