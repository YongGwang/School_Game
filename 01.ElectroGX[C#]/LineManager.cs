using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    public GameObject CopyObject;
    public GameObject[] LinePrefabs;

    private Transform playerTransform;
    private float spawnZ = 0.0f;
    private float LineLength = 20.32f;
    private int amnLinesOnScreen = 4;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = CopyObject.gameObject.transform;

        for(int i = 0; i < amnLinesOnScreen; i++)
        {
            SpawnLines();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform)
        {
            if (playerTransform.position.z > (spawnZ - amnLinesOnScreen * LineLength))
            {
                SpawnLines();
            } 
        }
    }

    void SpawnLines(int PrefabIndex = -1)
    {
        GameObject Go;
        Go = Instantiate(LinePrefabs[0]) as GameObject;
        Go.transform.SetParent(transform);
        Go.transform.position = Vector3.forward * spawnZ;
        spawnZ += LineLength;
    }
}
