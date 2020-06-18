using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Log pile
/// </summary>
/// =======================================================
/// Author : 2020/02/11(Sa)
/// History Log :
///		2020/02/11(Sa) Initial
public class LogPile : MonoBehaviour, IReactionable
{
    [SerializeField] private GameObject crashEffect = null;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    public void OnEnter(MarathonPlayerManager actor)
    {
        if (actor.State != MarathonPlayerManager.MarathonPlayerStatus.Dashing)
        {
            actor.SetPlayerState(MarathonPlayerManager.MarathonPlayerStatus.BeKnockedBack);
        }

        AudioManager.Instance.Play("HitLogPile");
        Instantiate(crashEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void OnExit(MarathonPlayerManager actor){}

    public void OnStay(MarathonPlayerManager actor){}
}