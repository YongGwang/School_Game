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

    public void OnEnter(MarathonPlayerManager acter)
    {
        acter.SetPlayerState(MarathonPlayerManager.MarathonPlayerStatus.PassingBridge);
        acter.UpdateBridgeInfo(this);
    }

    public void OnExit(MarathonPlayerManager acter)
    {
        if (acter.State == MarathonPlayerManager.MarathonPlayerStatus.Falling) return;
        acter.SetPlayerState(MarathonPlayerManager.MarathonPlayerStatus.Running);
        acter.RecenterPlayerJoyCon();
        acter.UpdateBridgeInfo();
    }

    public void OnStay(MarathonPlayerManager acter) { }

    public void BeforePlayerFelledOut(MarathonPlayerManager acter)
    {
        GetComponent<Collider>().enabled = false;
        playerFellOutPosition = acter.GetPlayerPosition();
    }

    public void OnPlayerReturn()
    {
        GetComponent<Collider>().enabled = true;
    }
}