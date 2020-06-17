using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public MovePlayer playerObj;
    private Vector3 Location;
    private float zLocation;


    // Start is called before the first frame update
    void Start()
    {
        Location = transform.position - playerObj.transform.position;
    }
    
    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        //if (!GameManager.bIsGameStarted) return;

        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.

        if(playerObj.bCameraStop == true)
        {
            transform.position = transform.position;
        }
        else if (playerObj)
	{
            transform.position = new Vector3(0, 2.85f, playerObj.transform.position.z + Location.z);

        }
    }
}
