using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI. Left distance to finish game.
/// </summary>
[RequireComponent(typeof(Slider))]
public class DistanceBar : MonoBehaviour
{
    [SerializeField] private Transform startPosition = null;
    [SerializeField] private Transform endPosition = null;
    private Slider distanceBar = null;

    private void Awake()
    {
        distanceBar = GetComponent<Slider>();
        distanceBar.maxValue = endPosition.position.z - startPosition.position.z;
    }


    private void Update()
    {
        var playerPos = MarathonPlayerManager.Instance.GetPlayerPosition();
        distanceBar.value = endPosition.position.z -
                            (endPosition.position.z - playerPos.z);
    }
}