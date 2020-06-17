using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Trigger for spawning white bear
/// </summary>
/// =======================================================
/// Author : 2020/02/11(Sa)
/// History Log :
///		2020/02/11(Sa) Initial
///		2020/02/13(Sa) Change rot method
public class Missile : MonoBehaviour, IReactionable
{
    [SerializeField] private float rotSpeed = 12.0f;
    private Vector3 initialPosition = Vector3.zero;
    private Vector3 destination = Vector3.zero;
    private float moveSpeed = 20.0f;
    private Vector3 moveDir = Vector3.zero;

    private void Update()
    {
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
        transform.rotation *= Quaternion.Euler(rotSpeed * Time.deltaTime, 0.0f, 0.0f);
    }

    // ==================================================================
    // Public
    // ==================================================================
    public void Initialize(Vector3 aimOffset, float moveSpeed)
    {
        destination = MarathonPlayerManager.Instance.GetPlayerPosition() + aimOffset;
        this.moveSpeed = moveSpeed;
        moveDir = (destination - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(moveDir, transform.up);
    }

    public void OnEnter(MarathonPlayerManager acter)
    {
        acter.SetPlayerState(MarathonPlayerManager.MarathonPlayerStatus.Dizzying);

        /// Bounce from player
        var randVect = UnityEngine.Random.onUnitSphere;
        if (randVect.y < 0.0f)
        {
            randVect = Vector3.Scale(randVect, Vector3.down);
        }
        // Ensure randVect is point front of player
        if (Vector3.Dot(randVect, acter.GetPlayerPosition()) < 0.0f)
        {
            randVect = Vector3.Scale(randVect, new Vector3(1.0f, 1.0f, -1.0f));
        }
        moveDir = randVect.normalized;
    }

    public void OnStay(MarathonPlayerManager acter) { }

    public void OnExit(MarathonPlayerManager acter) { }
}