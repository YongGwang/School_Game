using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    [SerializeField] private float destoryTime = 2.0f;

    private void Start()
    {
        Destroy(gameObject, destoryTime);
    }
}