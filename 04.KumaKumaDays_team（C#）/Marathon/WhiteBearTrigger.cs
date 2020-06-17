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
///		2020/02/12(Sa) Multiple missiles and fire interval
public class WhiteBearTrigger : MonoBehaviour, IReactionable
{
    [Header("Missile")]
    [SerializeField] private float missileFireDelay = 1.0f;
    [SerializeField] private Vector3 missileAimOffset = Vector3.zero;
    [SerializeField] private Vector3 missileSpawnPostionOffset = Vector3.zero;
    [SerializeField] private Vector3 missileSpawnRotationOffset = Vector3.zero;
    [SerializeField] private float missileLifeLength = 10.0f;
    [SerializeField] private float missileMoveSpeed = 100.0f;
    [Tooltip("How many missiles")]
    [SerializeField] private int missileCount = 3;
    [Tooltip("何秒おきに、ミサイル発射")]
    [SerializeField] private float missileFireInterval = 1.2f;

    [Header("WhiteBear")]
    [SerializeField] private float whiteLiftLength = 10.0f;
    [SerializeField] private GameObject whiteBearPrefab = null;
    [SerializeField] private GameObject missilePrefab = null;

    private Transform whiteBearSpawnPositionMark = null;
    private Animator whiteBearAnimComp = null;
    private bool isTriggered = false;

    private void Awake()
    {
        isTriggered = false;
        whiteBearSpawnPositionMark = transform.GetChild(0);
    }

    public void OnEnter(MarathonPlayerManager acter)
    {
        if (isTriggered) return;
        else isTriggered = true;

        var newBear = Instantiate(whiteBearPrefab,
                                  whiteBearSpawnPositionMark.position,
                                  whiteBearSpawnPositionMark.rotation);
        whiteBearAnimComp = newBear.transform.GetChild(0).GetComponent<Animator>();
        Debug.Assert(whiteBearAnimComp != null);
        Destroy(newBear, whiteLiftLength);
        StartCoroutine(FireMissile());
    }

    public void OnStay(MarathonPlayerManager acter) { }

    public void OnExit(MarathonPlayerManager acter) { }

    private IEnumerator FireMissile()
    {
        yield return new WaitForSeconds(missileFireDelay);
        for (int i = 0; i < missileCount; i++)
        {
            whiteBearAnimComp.SetTrigger("FireMissile");
            AudioManager.Instance.Play("WhiteBearThrow");
            var missile = Instantiate(missilePrefab,
                                      whiteBearSpawnPositionMark.position + missileSpawnPostionOffset,
                                      Quaternion.Euler(missileSpawnRotationOffset) * whiteBearSpawnPositionMark.rotation);
            missile.GetComponent<Missile>().Initialize(missileAimOffset, missileMoveSpeed);
            Destroy(missile, missileLifeLength);
            yield return new WaitForSeconds(missileFireInterval);
        }
    }
}