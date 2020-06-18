using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RotateDestoryBlock : MonoBehaviour
{
    [SerializeField] private bool isUsingLeftJoyCon = false;
	[SerializeField] private TextMeshProUGUI Counter = null;
	[SerializeField] private int countdown = 0;
	private GameObject player;
	private int Outputcount;
	private float count = 0.0f;
	private bool ShouldRotate;
    private void Awake()
    {
		Counter.enabled = false;
		ShouldRotate = false;
		Outputcount = 1;
	}
    void Update()
    {
		if (!ShouldRotate) return;
		getGyro();
    }

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			ShouldRotate = true;
			Counter.enabled = true;
			player = collision.gameObject;
			//collision.gameObject.transform.localRotation = Quaternion.identity;
			//MarathonPlayer.isDown = true;
		}
	}

	private void getGyro()
    {
        var getGyroy = JoyConInputManager.GetJoyConGyro(isUsingLeftJoyCon).y;
		if (Outputcount > 0)
		{
			if (getGyroy < -2 || getGyroy > 2)
			{
				count += Mathf.Abs(getGyroy);
				Outputcount = countdown - (int)(count / 100.0f);
				Counter.text = "" + Outputcount;
				Debug.Log(count);
			}
		}
		else
		{
			Counter.enabled = false;
			//MarathonPlayer.isDown = false;
			ShouldRotate = false;
			player.transform.localRotation = Quaternion.identity;
			JoyConInputManager.Recenter(isUsingLeftJoyCon);
			Destroy(this.gameObject);
		}

    }

}
