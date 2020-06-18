using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bridge
/// </summary>
/// =======================================================
/// Author : 2020/02/11(Sa)
/// History Log :
///		2020/02/11(Sa) Initial
public class Bridge : MonoBehaviour, IReactionable
{
    public Vector3 playerFellOutPosition { get; private set; }

    private void Awake()
    {
        playerFellOutPosition = Vector3.zero;
    }

    public void OnEnter(MarathonPlayerManager actor)
    {
        actor.SetPlayerState(MarathonPlayerManager.MarathonPlayerStatus.PassingBridge);
        actor.UpdateBridgeInfo(this);
    }

    public void OnExit(MarathonPlayerManager actor)
    {
        if (actor.State == MarathonPlayerManager.MarathonPlayerStatus.Falling) return;
        actor.SetPlayerState(MarathonPlayerManager.MarathonPlayerStatus.Running);
        actor.RecenterPlayerJoyCon();
        actor.UpdateBridgeInfo();
    }

    public void OnStay(MarathonPlayerManager actor){}

    public void BeforePlayerFelledOut(MarathonPlayerManager actor)
    {
        GetComponent<Collider>().enabled = false;
        playerFellOutPosition = actor.GetPlayerPosition();
    }

    public void OnPlayerReturn()
    {
        GetComponent<Collider>().enabled = true;
    }
}