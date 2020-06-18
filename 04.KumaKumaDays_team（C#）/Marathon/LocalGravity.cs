using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalGravity : MonoBehaviour
{
	[SerializeField] private Vector3 runLocalGravity = Vector3.zero;
    [SerializeField] private Vector3 dashLocalGravity = Vector3.zero;

    private void Awake()
	{
		GetComponent<Rigidbody>().useGravity = false; 
	}

	private void FixedUpdate()
	{
        Vector3 applyGravity;
        if (MarathonPlayerManager.Instance.State == MarathonPlayerManager.MarathonPlayerStatus.Dashing)
        {
            applyGravity = dashLocalGravity;
        }
        else
        {
            applyGravity = runLocalGravity;
        }
        GetComponent<Rigidbody>().AddForce(applyGravity, ForceMode.Acceleration);
    }
}