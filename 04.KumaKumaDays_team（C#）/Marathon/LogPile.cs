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

    public void OnEnter(MarathonPlayerManager acter)
    {
        if (acter.State != MarathonPlayerManager.MarathonPlayerStatus.Dashing)
        {
            acter.SetPlayerState(MarathonPlayerManager.MarathonPlayerStatus.BeKnockedBack);
        }

        AudioManager.Instance.Play("HitLogPile");
        Instantiate(crashEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void OnExit(MarathonPlayerManager acter) { }

    public void OnStay(MarathonPlayerManager acter) { }
}