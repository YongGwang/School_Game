using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GayDeerTrigger : MonoBehaviour, IReactionable
{
    [Tooltip("この時間後、Gayを生成し発射する")]
    [SerializeField] private float deerSpawnDelay = 4.0f;
    [SerializeField] private float gayDeerLiftLength = 8.0f;
    [Tooltip("この時間後、矢印のHintを削除する")]
    [SerializeField] private float hintLifeLength = 5.0f;
    [SerializeField] private GameObject arrowEffect = null;
    [SerializeField] private GameObject gayDeerPrefab = null;

    private bool isTriggered = false;

    private void Awake()
    {
        isTriggered = false;
        arrowEffect.SetActive(false);
    }

    public void OnEnter(MarathonPlayerManager acter)
    {
        if (isTriggered)    return;
        else                isTriggered = true;

        ShowHint();
        Invoke("SpawnGayDeer", deerSpawnDelay);
        Destroy(gameObject, hintLifeLength);
    }

    public void OnExit(MarathonPlayerManager acter) { }

    public void OnStay(MarathonPlayerManager acter) { }

    private void ShowHint()
    {
        AudioManager.Instance.Play("GayDeerHint");
        arrowEffect.SetActive(true);
        arrowEffect.GetComponentInChildren<ParticleSystem>().Play();
    }

    private void SpawnGayDeer()
    {
        GameObject newDeer = Instantiate(gayDeerPrefab, transform.position, transform.rotation);
        Destroy(newDeer, gayDeerLiftLength);
    }
}