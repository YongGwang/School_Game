using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI about player info hint. BalloonGame.
/// </summary>
/// =======================================================
/// Author : 2019/01/12 Sa
/// History Log :
///		2019/01/12(Sa) Initial
public class PlayerHintUIController : MonoBehaviour
{
    [SerializeField] float shakeAniSpeedMultiplier = 1.0f;
    [SerializeField] float shakeAniSpeedBase = 1.0f;

    private enum UIStatue
    {
        None = 0,
        ShakeIt,
        OmitIt,
        Freeze
    }

    private UIStatue presentState = UIStatue.None;
    private Image playerHintUIComp = null;
    private Animator animComp = null;

    #region InheritMethods
    private void Awake()
    {
        presentState = UIStatue.None;
        playerHintUIComp = GetComponentInChildren<Image>();
        animComp = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        var playerBalloonRef = BalloonGameManager.Instance.PlayerBalloon;
        if (playerBalloonRef)
        {
            playerHintUIComp.color = new Color(1, 1, 1, 1);
            switch (playerBalloonRef.State)
            {
                case Balloon.BalloonStatus.Normal:
                    presentState = UIStatue.ShakeIt;
                    float aniSpeed;
                    if (BalloonGameManager.Instance.PlayerJoyConInputSpeed != 0.0f)
                    {
                        aniSpeed = BalloonGameManager.Instance.PlayerJoyConInputSpeed;
                    }
                    else
                    {
                        aniSpeed = BalloonGameManager.Instance.PlayerKeyboardInputSpeed;
                    }
                    animComp.SetFloat("ShakeAniSpeed", shakeAniSpeedBase + 
                                      aniSpeed * shakeAniSpeedMultiplier);
                    break;
                case Balloon.BalloonStatus.Danger:
                    presentState = UIStatue.OmitIt;
                    break;
            }
        }
        else
        {
            if (BalloonGameManager.Instance.IsPlayerFreezing)
            {
                presentState = UIStatue.Freeze;
            }
            else
            {
                presentState = UIStatue.None;
                playerHintUIComp.color = new Color(0, 0, 0, 0);
            }
        }

        animComp.SetInteger("UIState", (int)presentState);
    }
    #endregion
}