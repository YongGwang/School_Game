using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FlashingMesh : MonoBehaviour
{
    public MovePlayer PlayerScript;
    public GameObject[] Model = new GameObject[6];

    // Update is called once per frame
    void Update()
    {
        //float level = Mathf.Abs(Mathf.Sin(Time.time * 10));

        for(int i= 0; i< 5; i++)
        {
            if (PlayerScript.bFlash == false)
            {
                Model[i].SetActive(true);
            }

            if (PlayerScript.bFlash)
            {
                Model[i].SetActive(false);
            }

        }
        //var renderModel = gameObject.GetComponent<SkinnedMeshRenderer>();
        //renderModel.material.color = new Color(1f, 1f, 1f, 0f);
        //Debug.Log(PlayerScript.bFlash);
    }
}
