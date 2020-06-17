using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

/// <summary>
/// Scene movement controller, and game state
/// </summary>
/// =======================================================
/// Author : 2020/02/25 Sa
/// History Log :
///		2020/02/25(Sa) Initial
public class MarathonGameManager : MonoBehaviour, ITimeControl
{
    public static MarathonGameManager Instance = null;
    public bool isBridgeTut { get; private set; }

    [Header("Tutorial")]
    [SerializeField] private bool isTutorial = false;
    [SerializeField] private Joycon.Button readyJoyConButton = Joycon.Button.PLUS;
    [SerializeField] private string readyKeyboardButton = "Submit";

    [Space]
    [SerializeField] private float delayPermitInputTime = 1.5f;

    private AsyncOperation async;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;

        isBridgeTut = false;
    }

    private void Start()
    {
        async = Utility.MoveNextSceneAsync();
        if (!isTutorial)
        {
            MarathonGoalController.Instance.OnPlayerReachGoal += OnPlayerReachGoal;
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Update()
    {
        if (isTutorial)
        {
            if (!isBridgeTut)
            {
                if (JoyConInputManager.GetJoyConButtonDown(readyJoyConButton) ||
                    Input.GetButtonDown(readyKeyboardButton))
                {
                    AudioManager.Instance.Play("ZRTouch");
                    isBridgeTut = true;
                    MarathonTutUIController.Instace.ChangeToBridgeUI();
                }
            }
            else
            {
                if (JoyConInputManager.GetJoyConButtonDown(readyJoyConButton) ||
                    Input.GetButtonDown(readyKeyboardButton))
                {
                    AudioManager.Instance.Play("ZRTouch");
                    async.allowSceneActivation = true;
                    Resources.UnloadUnusedAssets();
                    System.GC.Collect();
                }
            }
        }
        else
        {
            if (MarathonScoreController.Instance.IsFinishedResultTimeline)
            {
                if (Input.anyKeyDown || JoyConInputManager.GetJoyConAnykeyDown())
                {
                    AudioManager.Instance.Play("ZRTouch");
                    async.allowSceneActivation = true;
                    Resources.UnloadUnusedAssets();
                    System.GC.Collect();
                }
            }
        }
    }

    private void OnPlayerReachGoal()
    {
        AudioManager.Instance.Stop("MarathonBGM");
        Invoke("PermitInput", delayPermitInputTime);
        MarathonPlayerManager.Instance.OnPlayerReachGoal();
        MarathonGameTimerController.Instance.ShutDownCount();
    }

    #region TimelineInterface
    void ITimeControl.SetTime(double time)
    {
    }

    void ITimeControl.OnControlTimeStart()
    {
    }

    void ITimeControl.OnControlTimeStop()
    {
        MarathonPlayerManager.Instance?.OnGameStart();
        MarathonGameTimerController.Instance?.StartCount();
        AudioManager.Instance?.Play("MarathonBGM");
    }
    #endregion
}