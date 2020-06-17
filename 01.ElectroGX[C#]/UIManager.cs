using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public MovePlayer playerInfo;

    #region SpeedBar
    [Header("SpeedBar")]
    [SerializeField] private GameObject leftSideSpeedBarGameObj = null;
    private Image[] leftSideSpeedBar = null;
    [SerializeField] private GameObject rightSideSpeedBarGameObj = null;
    private Image[] rightSideSpeedBar = null;
    [SerializeField] private Image MiddleSpeedBar = null;
    #endregion

    #region ItemCount
    public Text ItemCount;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        ItemCount.text = "X 0";
        leftSideSpeedBar = leftSideSpeedBarGameObj.GetComponentsInChildren<Image>();
        rightSideSpeedBar = rightSideSpeedBarGameObj.GetComponentsInChildren<Image>();
        MovePlayer.OnGetSpeedUpItem += RefreshInfoAboutSpeedUpItem;
        MovePlayer.OnHitFence += CutBarCountWhenHitFence;
    }

    private void RefreshInfoAboutSpeedUpItem()
    {
        ItemCount.text = "X " + playerInfo.PresentItemGettedCount.ToString();
        RefreshSpeedBarNumber(playerInfo.PresentPlayerForwardSpeedStep);
        RefreshSpeedBarColor();
    }

    private void RefreshSpeedBarNumber(float barCount)
    {
        for (int i = 0; i < leftSideSpeedBar.Length; i++)
        {
            if (i <= barCount - 1)
            {
                leftSideSpeedBar[i].enabled = rightSideSpeedBar[i].enabled = true;
            }
            else
            {
                leftSideSpeedBar[i].enabled = rightSideSpeedBar[i].enabled = false;
            }
        }
    }
    private void RefreshSpeedBarColor()
    {
        float gbChannelVal = 1.0f - 1.0f / playerInfo.maxSpeedStep * playerInfo.PresentPlayerForwardSpeedStep;
        Color newColor = new Color(1.0f, gbChannelVal, gbChannelVal);
        foreach (var i in leftSideSpeedBar) i.color = newColor;
        foreach (var i in rightSideSpeedBar) i.color = newColor;
        MiddleSpeedBar.color = newColor;
    }
    private void CutBarCountWhenHitFence()
    {
        RefreshSpeedBarNumber(playerInfo.PresentPlayerForwardSpeedStep - 2);
        RefreshSpeedBarColor();
    }
    private void OnDestroy()
    {
        MovePlayer.OnGetSpeedUpItem -= RefreshInfoAboutSpeedUpItem;
        MovePlayer.OnHitFence -= CutBarCountWhenHitFence;
    }
}