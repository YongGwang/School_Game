using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UseStickRotate : MonoBehaviour
{
	private PointerEventData eventDataCurrentPosition;
	private Vector3 RotateLimit;
	private float initialVelocity = 0.0f;
	private float rotspeed=200.0f;

	void Update()
	{
		GetInitialVelocity();
		Rot(initialVelocity);
		ClampRot(-45, 45);
	}

	private void GetInitialVelocity()
	{
		var getstick = JoyConInputManager.Getstick(false);
		initialVelocity = getstick[0] * rotspeed;
	}

	private void Rot(float initialVelocity)
	{
		transform.Rotate(0, initialVelocity  * Time.deltaTime, 0);
	}

	private void ClampRot(float minY, float maxY)
	{
		RotateLimit = transform.localEulerAngles;

		if (transform.localEulerAngles.y > maxY && transform.localEulerAngles.y <= maxY + 10)
		{
			RotateLimit.y = maxY;
		}
		else if (transform.localEulerAngles.y > 350 + minY && transform.localEulerAngles.y < 360 + minY)
		{
			RotateLimit.y = 360 + minY;
		}
		transform.localEulerAngles = RotateLimit;
	}
}
