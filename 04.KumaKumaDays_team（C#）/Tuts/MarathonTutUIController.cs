using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Control state of marathon tut.Change UI.
/// </summary>
/// Author :  2019/02/24 Sa
/// =============================================
/// History Log :
///		2019/02/24(Sa) Initial
public class MarathonTutUIController : MonoBehaviour
{
    public static MarathonTutUIController Instace = null;

    [Header("UI")]
    [SerializeField] Image startContinueUI = null;
    [SerializeField] Image controllTutUI = null;
    [SerializeField] Animator inputHintAniComp = null;
    [SerializeField] Sprite[] startContinueSprites = null;
    [SerializeField] Sprite[] controllTutSprites = null;

    private void Awake()
    {
        if (Instace != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instace = this;
        }

        startContinueUI.sprite = startContinueSprites[0];
        controllTutUI.sprite = controllTutSprites[0];
        inputHintAniComp.SetBool("isBridgeTut", false);
    }

    private void OnDestroy()
    {
        Instace = null;
    }

    public void ChangeToBridgeUI()
    {
        startContinueUI.sprite = startContinueSprites[1];
        controllTutUI.sprite = controllTutSprites[1];
        inputHintAniComp.SetBool("isBridgeTut", true);
    }
}