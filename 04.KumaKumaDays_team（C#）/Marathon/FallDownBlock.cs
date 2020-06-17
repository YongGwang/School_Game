using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDownBlock : MonoBehaviour
{
    [SerializeField] private float triggerDistance = 5.0f;

    void Update()
    {
        var playerPos = MarathonPlayerManager.Instance.GetPlayerPosition();
		if (this.transform.position.z - playerPos.z < triggerDistance)
		{
			this.GetComponent<Rigidbody>().useGravity = true;
			this.GetComponent<BoxCollider>().isTrigger = false;
		}
		if (this.transform.position.y < -100) Destroy(this.gameObject);
    }
}