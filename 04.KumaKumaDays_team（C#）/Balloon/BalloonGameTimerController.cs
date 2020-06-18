using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI about game left time
/// </summary>
/// =======================================================
/// Author : 2019/01/12 Sa
/// History Log :
///		2019/01/12(Sa) Initial
///		2019/02/20(Sa) Arrow rot' clockwise base on TimeCircle_Front's clockwise
[RequireComponent(typeof(Image))]
public class BalloonGameTimerController : MonoBehaviour
{
    [Tooltip("%. これを下回ったら、拡大縮小アニメ始まる")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float aniStartPrecent = 0.25f;
    [Tooltip("拡大縮小アニメの量")]
    [SerializeField] private float aniAmp = 0.05f;
    [Tooltip("拡大縮小アニメの速度")]
    [SerializeField] private float aniSpeed = 0.25f;
    [SerializeField] private float strikeHintWhenLeft = 10.0f;

    [SerializeField] private Image beSlicedImageComp = null;
    [SerializeField] private Image arrowImageComp = null;
    private Quaternion arrowInitialRot = new Quaternion();

    private void Awake()
    {
        arrowInitialRot = arrowImageComp.rectTransform.localRotation;
    }

    private void Update()
    {
        if (BalloonGameManager.Instance.PresentGameState == 
            BalloonGameManager.GameStatus.End)
        {
            return;
        }

        var currentGameTime = BalloonGameManager.Instance.GameTime;
        var gameTimeLimit = BalloonGameManager.Instance.GameTimeLimit;

        if (gameTimeLimit - currentGameTime <= strikeHintWhenLeft)
        {
            if (!AudioManager.Instance.CheckIsPlaying("TimeHint"))
            {
                AudioManager.Instance.Play("TimeHint");
            }
        }

        float timePercent = Mathf.Clamp01(currentGameTime / gameTimeLimit);
        beSlicedImageComp.fillAmount = 1.0f - timePercent;
        var temp = GetComponent<Image>().fillClockwise ? 360.0f : -360.0f; 
        arrowImageComp.rectTransform.localRotation = arrowInitialRot * Quaternion.Euler(0.0f,
            0.0f, temp * timePercent);
        if (1.0f - timePercent <= aniStartPrecent)
        {
            GetComponent<RectTransform>().localScale += Vector3.one *
                (2.0f * Mathf.PingPong(Time.time * aniSpeed, aniAmp) - aniAmp);
        }
    }
}