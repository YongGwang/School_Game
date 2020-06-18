using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SexyFoxTrigger : MonoBehaviour
{
	private GameObject SexyFox = null;

	private void Awake()
	{
		SexyFox = transform.GetChild(0).gameObject;
		SexyFox.SetActive(false);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			SexyFox.SetActive(true);
		}
	}
}