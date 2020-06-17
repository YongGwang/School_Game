using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Honey jay. Add dash energy
/// </summary>
/// =======================================================
/// Author : 2020/02/11(Sa)
/// History Log :
///		2020/02/11(Sa) Initial
public class HoneyJar : MonoBehaviour, IReactionable
{
    [Range(0.0f, 1.0f)]
    [SerializeField] private float honeyValue = 0.1f;

    void IReactionable.OnExit(MarathonPlayerManager acter) { }

    void IReactionable.OnStay(MarathonPlayerManager acter) { }

    void IReactionable.OnEnter(MarathonPlayerManager acter)
    {
        AudioManager.Instance.Play("GetHoney");
        acter.AddDashEnergy(honeyValue);
        Destroy(gameObject);
    }
}