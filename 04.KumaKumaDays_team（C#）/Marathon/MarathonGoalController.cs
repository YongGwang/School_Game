using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Goal
/// </summary>
/// =======================================================
/// Author : Sa
/// History Log :
///		2020/02/12(Sa) Initial.
public class MarathonGoalController : MonoBehaviour, IReactionable
{
    public static MarathonGoalController Instance = null;
    public UnityAction OnPlayerReachGoal = null;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    public void OnEnter(MarathonPlayerManager acter)
    {
        acter.SetPlayerState(MarathonPlayerManager.MarathonPlayerStatus.ReachGoal);
        OnPlayerReachGoal?.Invoke();
    }

    public void OnExit(MarathonPlayerManager acter)
    {
    }

    public void OnStay(MarathonPlayerManager acter)
    {
    }
}