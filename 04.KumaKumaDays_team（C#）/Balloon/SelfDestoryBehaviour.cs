using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Set self-destory
/// Author : song
/// Update :
///     2019/11/27(Sa) fix bug  
/// </summary>
public class SelfDestoryBehaviour : MonoBehaviour
{
    [SerializeField] private float countDownTime = 4.0f;

    private void Start()
    {
        Destroy(this.gameObject, countDownTime);
    }
}