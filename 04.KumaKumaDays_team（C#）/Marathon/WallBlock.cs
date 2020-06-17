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
    public void OnExit(MarathonPlayerManager acter) { }

    public void OnStay(MarathonPlayerManager acter) { }

    public void OnEnter(MarathonPlayerManager acter)
    {
        if (acter.State != MarathonPlayerManager.MarathonPlayerStatus.Running)
        {
            return;
        }

        AudioManager.Instance.Play("HitWallBlock");
        acter.SetPlayerState(MarathonPlayerManager.MarathonPlayerStatus.BeKnockedBack);
    }
}