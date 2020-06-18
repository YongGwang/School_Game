using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class GameStartUISwitcher : MonoBehaviour, ITimeControl
{
    [SerializeField] private Sprite[] sprites = new Sprite[2];

    void Awake()
    {
        GetComponent<Image>().sprite = sprites[0];
    }

    void ITimeControl.OnControlTimeStart()
    {
        GetComponent<Image>().sprite = sprites[1];
    }

    void ITimeControl.OnControlTimeStop()
    {
    }

    void ITimeControl.SetTime(double time)
    {
    }
}
