using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A temp test dummy
/// </summary>
/// Author :  2019/02/13 Sa
/// History Log :
///		2019/02/13(Sa) Initial
public class TestDummy : MonoBehaviour
{
	[SerializeField] private float moveSpeed = 200.0f;
	[SerializeField] private Cinemachine.CinemachinePathBase cinemachinePath = null;

    void Update()
    {
		Utility.MoveObjByWASD(transform,  moveSpeed);
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Utility.ResetInNearestPath(transform, cinemachinePath);
		}
    }
}