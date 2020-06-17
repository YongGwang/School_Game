using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinMotion : MonoBehaviour
{
	public Vector3 spinSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(spinSpeed);
    }
}
