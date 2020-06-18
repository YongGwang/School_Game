using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Logic about player's and deer' s animation
/// </summary>
/// =======================================================
/// Author : 2019/01/10(Sa)
/// History Log :
///		2019/01/10(Sa) Initial
public class AthleteAnimatonController : MonoBehaviour
{
    [Tooltip("プレイヤーは何もしない時、" +
    "空気をいれるアニメーションの速さ(1.0はアニメ本来の速さ)")]
    [SerializeField] private float defaultInflateAnimationSpeed = 0.2f;
    [Tooltip("入力なしで何秒後、アニメ速度は元に戻る")]
    [SerializeField] private float resetTime = 2.0f;
    [Tooltip("眩暈の長さ")]
    [SerializeField] private float dizzyTime = 2.0f;
    [Tooltip("眩暈Effectのオブジェクト")]
    [SerializeField] private GameObject dizzyEffectObj = null;

    private Animator animatorComp;

    private void Awake()
    {
        animatorComp = GetComponent<Animator>();
        Debug.Assert(animatorComp != null);
        dizzyEffectObj.SetActive(false);
    }

    #region PUBLIC
    public void SetInflateAnimationSpeed(float speed)
    {
        animatorComp.SetFloat("inflatingSpeed", speed == 0.0f ?
                               defaultInflateAnimationSpeed : speed);
        if (IsInvoking("ResetInflateAnimationSpeed"))
        {
            CancelInvoke("ResetInflateAnimationSpeed");
        }
        Invoke("ResetInflateAnimationSpeed", resetTime);
    }

    public void ResetInflateAnimationSpeed()
    {
        animatorComp.SetFloat("inflatingSpeed", defaultInflateAnimationSpeed);
    }

    public void SetFinishPose()
    {
        animatorComp.SetInteger("rivalWin=0playerWin=1match=2",
            (int)BalloonGameManager.Instance.PresentGameResult);
    }

    public void StartDizzy()
    {
        animatorComp.SetBool("isFreezing", true);
        dizzyEffectObj.SetActive(true);
        if (IsInvoking("DisableDizzy"))
        {
            CancelInvoke("DisableDizzy");
        }
        Invoke("DisableDizzy", dizzyTime);
    }
    #endregion

    #region NewAdded
    private void DisableDizzy()
    {
        dizzyEffectObj.SetActive(false);
        animatorComp.SetBool("isFreezing", false);
    }
    #endregion
}