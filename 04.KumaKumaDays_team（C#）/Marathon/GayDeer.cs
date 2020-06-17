using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gay deer
/// </summary>
/// =======================================================
/// Author : 2020/02/11(Sa)
/// History Log :
///		2020/02/11(Sa) Initial
public class GayDeer : MonoBehaviour, IReactionable
{
    [SerializeField] private float moveSpeed = 1.0f;

    private void Update()
    {
		transform.Translate(Vector3.forward * moveSpeed);
    }

    public void OnEnter(MarathonPlayerManager acter)
    {
        acter.SetPlayerState(MarathonPlayerManager.MarathonPlayerStatus.Dizzying);
    }

    public void OnStay(MarathonPlayerManager acter) { }

    public void OnExit(MarathonPlayerManager acter) { }
}