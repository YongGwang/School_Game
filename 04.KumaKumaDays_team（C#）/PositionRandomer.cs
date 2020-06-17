using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Randomly movement
/// </summary>
/// =======================================================
/// Author : 2019/02/02 Sa
/// History Log :
///		2019/02/02(Sa) Initial
[RequireComponent(typeof(Transform))]
public class PositionRandomer : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    [SerializeField] private float amp = 0.0f;
    [Range(0.1f, 0.001f)]
    [SerializeField] private float speed = 0.0f;
    private Vector3 initialPostion = Vector3.zero;
    private Vector3 nextPostion = Vector3.zero;

    private void Awake()
    {
    }

    private void Start()
    {
        initialPostion = transform.position;
        nextPostion = initialPostion + Random.insideUnitSphere * amp;
    }

    private void Update()
    {
        if (nextPostion == transform.position)
        {
            nextPostion = initialPostion + Random.insideUnitSphere * amp;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, nextPostion, speed * Time.deltaTime);
        }
    }
}