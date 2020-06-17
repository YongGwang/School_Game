using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// block line
/// </summary>
/// =======================================================
/// Author : 2020/02/23(Sa)
/// History Log :
///		2020/02/23(Sa) Initial
public class BlockLine : MonoBehaviour
{
    public UnityAction OnPlayerEnter = null;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<MarathonPlayerManager>() != null)
        {
            OnPlayerEnter?.Invoke();
        }
    }
}