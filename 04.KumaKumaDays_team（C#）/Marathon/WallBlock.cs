using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wall on the corner, counter player
/// </summary>
/// =======================================================
/// Author : 2020/02/11(Sa)
/// History Log :
///		2020/02/11(Sa) Initial
public class WallBlock : MonoBehaviour, IReactionable
{
    public void OnExit(MarathonPlayerManager actor) { }

    public void OnStay(MarathonPlayerManager actor) { }

    public void OnEnter(MarathonPlayerManager actor)
    {
        if (actor.State != MarathonPlayerManager.MarathonPlayerStatus.Running)
        {
            return;
        }

        AudioManager.Instance.Play("HitWallBlock");
        actor.SetPlayerState(MarathonPlayerManager.MarathonPlayerStatus.BeKnockedBack);
    }
}