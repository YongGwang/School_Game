using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class GameStartParticleController : MonoBehaviour, ITimeControl
{
    void Awake()
    {
        foreach (Transform i in transform)
        {
            i.gameObject.SetActive(false);
        }
    }

    void ITimeControl.OnControlTimeStart()
    {
        foreach (Transform i in transform)
        {
            i.gameObject.SetActive(true);
        }
    }

    void ITimeControl.OnControlTimeStop()
    {
    }

    void ITimeControl.SetTime(double time)
    {
    }
}
