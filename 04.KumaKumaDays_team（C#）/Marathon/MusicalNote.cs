using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MusicalNote
/// </summary>
/// =======================================================
/// Author : 2020/02/11(Sa)
/// History Log :
///		2020/02/11(Sa) Initial.
///		2020/02/13(Sa) Name Changed: Flower -> MusicalNote.
///		               Get rootObj by code, not assign in inspector.
public class MusicalNote : MonoBehaviour, IReactionable
{
    public void OnEnter(MarathonPlayerManager acter)
    {
        acter.SetPlayerState(MarathonPlayerManager.MarathonPlayerStatus.Dancing);
    }

    public void OnExit(MarathonPlayerManager acter)
	{
        var rootObj = transform.parent.parent.parent.gameObject;
        rootObj.GetComponent<Collider>().enabled = false;
		GetComponent<Collider>().enabled = false;
        Destroy(rootObj, 1.5f);
	}

	public void OnStay(MarathonPlayerManager acter) { }
}