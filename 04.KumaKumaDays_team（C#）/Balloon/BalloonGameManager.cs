using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// Manager input/rule about this mini game.
/// If connected both left and right joy-con,
/// use right one for input
/// </summary>
/// =======================================================
/// Author : 2019/12/12 Sa
/// History Log :
///		2019/12/12(Sa) Initial
///		2019/12/14(Sa) Add JoyCon input
///		2019/12/16(Sa) Change the way to get perfabs
///		2019/12/17(Sa) Add keyboard input
///		2019/12/18(Sa) Add animation part
///		2019/12/19(Sa) Add inflating sound effect
///		               Add time limit
///		               Add deer obj
///		               Add animation after "Ready!" sign
///		               Fix bug about joy-con input
///	    2019/12/22(Sa) Add freeze time after failed
///	                   Change joy-con input mode
///	    2019/12/23(Sa) Add commit
///	    2019/01/08(Sa) Change types of balloon
///	    2019/01/10(Sa) Change details
///	                   Move part of logic to Athlete class
///	    2019/01/27(Sa) Change score varibles to public static
///	    2019/01/29(Sa) Rival's animation had speed change now
public class BalloonGameManager : MonoBehaviour, ITimeControl
{
    public enum GameStatus
    {
        Standby,
        Playing,
        End
    }

    public enum GameResult
    {
        RivalWin,
        PlayerWin,
        Match,
        Undone
    }

    public static BalloonGameManager Instance = null;
    public Balloon PlayerBalloon { get; private set; }
    public static int PlayerScore { get; private set; }
    public static int RivalScore { get; private set; }

    public UnityEvent OnGameFinished = new UnityEvent();
    public GameResult PresentGameResult { get; private set; }
    public GameStatus PresentGameState { get; private set; }
    public float GameTime { get; private set; }
    public float GameTimeLimit { get; private set; }
    public List<Balloon> PlayerReleasedBalloons { get; private set; }
    public List<Balloon> RivalReleasedBalloons { get; private set; }
    public List<List<Balloon>> SortedPlayerReleasedBalloons { get; private set; }
    public List<List<Balloon>> SortedRivalReleasedBalloons { get; private set; }
    public bool IsPlayerFreezing { get; private set; }
    public bool IsRivalFreezing { get; private set; }
    public float PlayerJoyConInputSpeed
    {
        get { return playerJoyConInputSpeed; }
    }
    public float PlayerKeyboardInputSpeed
    {
        get { return playerKeyboardInputSpeed; }
    }
    public int BalloonScoreMultiplier
    {
        get { return balloonScoreMultiplier; }
    }


    [Header("Tutorial")]
    [SerializeField] private bool isTutorial = false;
    [SerializeField] private Joycon.Button readyJoyConButton = Joycon.Button.PLUS;
    [SerializeField] private string readyKeyboardButton = "Submit";
       
    [Header("Keyboard Input")]
    [SerializeField] private string inflateButton = "";
    [SerializeField] private string releaseButton = "";
    [Tooltip("キーボード入力で風船を膨らます速さ")]
    [SerializeField] private float keyboardInflateSpeed = 1.0f;

    [Header("JoyCon Input")]
    [Tooltip("左てのJoy-conを使うか")]
    [SerializeField] private bool isUseLeftJoycon = false;
    [Tooltip("Joy-con入力で風船を膨らます速さ")]
    [SerializeField] private float joyConInflateSpeed = 1.2f;
    [Tooltip("Joy-con入力の激しさがこれ以下であれば、風船を膨らまさない")]
    [SerializeField] private float joyConInputSensitivity = 1.0f;
    [Tooltip("Joy-con振りの角速度がこれ以上であれば、風船を飛ばさない")]
    [SerializeField] private float joyConReleaseSensitivity = -0.5f;

    [Header("UI")]
    [Tooltip("秒読み文字を表すTextMesh component")]
    [SerializeField] private TextMesh gameInfoText = null;
    [SerializeField] private TMP_Text playerWinLoseText;
    [SerializeField] private TMP_Text rivalWinLoseText;

    [Header("DeerBehaviour")]
    [Tooltip("鹿始めの入力速さ")]
    [SerializeField] private float rivalInitialInputSpeed = 12.0f;
    [Tooltip("鹿は正しく風船を飛ばす確率")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float rivalSuccessProp = 0.66f;
    [Tooltip("プレイヤーは飛ばした数がここに達したら、AIは何とかする")]
    [SerializeField] private List<int> aiCheckPoint = new List<int>();
    [Tooltip("Max Speed")]
    [SerializeField] private float maxInputSpeed = 24.0f;
    [Tooltip("Speed Up Multiplier")]
    [SerializeField] private float aiSpeedMultiplier = 0.5f;

    [Header("Gameplay")]
    [Tooltip("一回ゲームの長さ")]
    [SerializeField] private float gameTimeLimit = 12.0f;
    [Tooltip("風船が爆発したら何秒間入力できないか")]
    [SerializeField] private float failFreezeTime = 1.0f;
    [Tooltip("風船が飛んだら、何秒間後新たな風船を生成するか")]
    [SerializeField] private float balloonSpawnInterval = 0.5f;
    [SerializeField] private int balloonScoreMultiplier = 4;

    [Header("Animation")]
    [Tooltip("キーボードのアニメ速さにの影響力")]
    [SerializeField] private float keyboardInflateAniSpeedMultiplier = 1.0f;
    [Tooltip("Joy-conのアニメ速さにの影響力")]
    [SerializeField] private float joyConInflateAniSpeedMultiplier = 1.0f;
    [Tooltip("ライバルのアニメ速さにの影響力")]
    [SerializeField] private float rivalInflateAniSpeedMultiplier = 1.0f;
    // If no input, after this sec, reset input cout per sec to 0
    [Tooltip("キーボードの入力が止まったら、何秒後入力速さの測定数値を0にするか。" +
        "入力が速ければアニメが速くなる")]
    [SerializeField] private float keyboardRestInputSpeedInterval = 1.0f;
    [SerializeField] private TimelineAsset gameStartTimelineAssert = null;
    [SerializeField] private TimelineAsset gameEndTimelineAssert = null;

    [Header("Sound")]
    [Tooltip("キーボード入力速さのsound effect長さにの影響力。速ければ、短くなる")]
    [SerializeField] private float keyboardInputPitchMultiplier = 0.6f;
    [Tooltip("Joy-con入力速さのsound effect長さにの影響力。速ければ、短くなる")]
    [SerializeField] private float joyConInputPitchMultiplier = 1.6f;

    [Header("Others")]
    [Tooltip("プレイヤーのAthleteクラスを指定ください")]
    [SerializeField] private AthleteAnimatonController player = null;
    [Tooltip("鹿のAthleteクラスを指定ください")]
    [SerializeField] private AthleteAnimatonController rival = null;
    [Tooltip("風船のPrefabを指定ください")]
    [SerializeField] private GameObject balloonBase = null;
    [Tooltip("プレイヤーの風船はどこに生成されるか")]
    [SerializeField] private Transform playerNozzleTrans = null;
    [Tooltip("鹿の風船はどこに生成されるか")]
    [SerializeField] private Transform rivalBalloonMarkTransform = null;

    private Balloon rivalBalloon = null;
    private float inflatedAirVolume = 0.0f;
    private int keyboardInputCount = 0;
    private float keyboardInputPerSec = 0.0f;
    private bool isNoInput = true;
    private float keyboardNoInputLength = 0.0f;
    private float keyboardInputLength = 0.0f;
    private int playerBalloonIndex = 0;
    private int rivalBalloonIndex = 0;
    private List<Balloon.BalloonType> balloonSpawnList;
    private PlayableDirector playableDirector;
    private bool willRivalReleaseBalloon = false;
    private Queue<int> aiCheckPointQueue = new Queue<int>();
    private float rivalPresentInputSpeed = 0.0f;
    private AsyncOperation async = null;
    private float playerJoyConInputSpeed = 0.0f;
    private float playerKeyboardInputSpeed = 0.0f;

//======================================================================================
// Public methods
//======================================================================================
    public static T GetRandomEnum<T>()
    {
        System.Array a = System.Enum.GetValues(typeof(T));
        T V = (T)a.GetValue(UnityEngine.Random.Range(0, a.Length));
        return V;
    }

    public static T GetEnumByIndex<T>(int index)
    {
        System.Array a = System.Enum.GetValues(typeof(T));
        return (T)a.GetValue(index);
    }

    public static int GetEnumCount<T>()
    {
        System.Array a = System.Enum.GetValues(typeof(T));
        return a.Length;
    }

//======================================================================================
// Inherited methods
//======================================================================================
	private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;

        IsPlayerFreezing = false;
        IsRivalFreezing = false;
        willRivalReleaseBalloon = false;
        PresentGameResult = GameResult.Undone;
        playerBalloonIndex = 0;
        rivalBalloonIndex = 0;
        PlayerScore = 0;
        RivalScore = 0;
        PresentGameState = GameStatus.Standby;
        keyboardInputPitchMultiplier = 0.6f;
        joyConInputPitchMultiplier = 1.6f;
        keyboardInputLength = 0.0f;
        PlayerBalloon = null;
        rivalBalloon = null;
        inflatedAirVolume = 0.0f;
        keyboardInputCount = 0;
        keyboardInputPerSec = 0.0f;
        keyboardNoInputLength = 0.0f;
        isNoInput = true;
        keyboardInputLength = 0.0f;
        GameTime = 0.0f;
        balloonSpawnList = new List<Balloon.BalloonType>();
        playableDirector = GetComponent<PlayableDirector>();
        if (!isTutorial) playableDirector.Play(gameStartTimelineAssert);
        GameTimeLimit = gameTimeLimit;
        PlayerReleasedBalloons = new List<Balloon>();
        RivalReleasedBalloons = new List<Balloon>();
        SortedPlayerReleasedBalloons = new List<List<Balloon>>();
        SortedRivalReleasedBalloons = new List<List<Balloon>>();
        rivalPresentInputSpeed = rivalInitialInputSpeed;
        playerJoyConInputSpeed = 0.0f;
        playerKeyboardInputSpeed = 0.0f;

        // Settings
        SpawnPlayerBalloon();
        if(!isTutorial) SpawnRivalBalloon();
        foreach (var i in aiCheckPoint)
        {
            aiCheckPointQueue.Enqueue(i);
        }
    }

    private void Start()
    {
        async = Utility.MoveNextSceneAsync();
    }

    void ITimeControl.SetTime(double time) { }

    void ITimeControl.OnControlTimeStart()
    {
        PresentGameState = GameStatus.Playing;
    }

    void ITimeControl.OnControlTimeStop() { }

    private void Update()
    {
		#region Development
		if (Input.GetKeyDown(KeyCode.F1))
		{
			Utility.ReloadCurrentScene();
		}

		if (Input.GetKeyDown(KeyCode.F2))
		{
			Utility.MoveNextScene();
		}
		#endregion

        if (isTutorial)
        {
            var isUsedJoyCon = HandlePlayerJoyConInput();
            if (!isUsedJoyCon)
            {
                HandlePlayerKeyboardInput();
            }
            if (JoyConInputManager.GetJoyConButtonDown(readyJoyConButton) ||
                Input.GetButtonDown(readyKeyboardButton))
            {
                AudioManager.Instance.Play("ZRTouch");
                async.allowSceneActivation = true;
            }

            return;
        }

		switch (PresentGameState)
        {
            case GameStatus.Standby:
                break;
            case GameStatus.Playing:
                if (GameTime > gameTimeLimit)
                {
                    GameTime = 0.0f;
                    PrepareData();
                    OnGameFinished?.Invoke();
                    playableDirector.Play(gameEndTimelineAssert);
                    AudioManager.Instance.Stop("TimeHint");
                    AudioManager.Instance.Stop("BalloonReady");
                    AudioManager.Instance.Play("SceneEnd");
                    playerJoyConInputSpeed = 0.0f;
                    playerKeyboardInputSpeed = 0.0f;
                    PresentGameState = GameStatus.End;
                }
                else
                {
                    GameTime += Time.deltaTime;
                    var timeLeft = gameTimeLimit - GameTime;

                    gameInfoText.text = "Time left: " + Mathf.RoundToInt(timeLeft) + "s";
                }

                var isUsedJoyCon = HandlePlayerJoyConInput();
                if (!isUsedJoyCon)
                {
                    HandlePlayerKeyboardInput();
                }
                HandleRivalBehaviour();
                ProcessAI();
                break;
            case GameStatus.End:
                if (BalloonGameResultUIController.Instance.IsScoreCalcFinished)
                {
                    if (JoyConInputManager.GetJoyConAnykeyDown() || Input.anyKeyDown)
                    {
                        AudioManager.Instance.Play("ZRTouch");
                        async.allowSceneActivation = true;
                        Resources.UnloadUnusedAssets();
                        System.GC.Collect();
                    }
                }
                break;
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

//======================================================================================
// Private methods
//======================================================================================
	/// <summary>
	/// Calc game player total score and sort balloons by type
	/// </summary>
	private void PrepareData()
    {
        // For-Loop by balloon type and add to list
        for (int i = 0; i < GetEnumCount<Balloon.BalloonType>(); i++)
        {
            var type = GetEnumByIndex<Balloon.BalloonType>(i);
            SortedPlayerReleasedBalloons.Add(PlayerReleasedBalloons.FindAll(
                x => x.PresentBallonType == type));
            SortedRivalReleasedBalloons.Add(RivalReleasedBalloons.FindAll(
                x => x.PresentBallonType == type));
        }

        // Calc total score
        foreach (var i in PlayerReleasedBalloons)
        {
            PlayerScore += i.GetBalloonScore();
        }

        foreach (var i in RivalReleasedBalloons)
        {
            RivalScore += i.GetBalloonScore();
        }

        // Get game result by total score
        if (PlayerScore > RivalScore)
        {
            PresentGameResult = GameResult.PlayerWin;
        }
        else if (PlayerScore == RivalScore)
        {
            PresentGameResult = GameResult.Match;
        }
        else
        {
            PresentGameResult = GameResult.RivalWin;
        }
    }

    private void ProcessAI()
    {
        if (PlayerReleasedBalloons.Count >= aiCheckPointQueue.Peek())
        {
            Debug.Log("Rival speed up");
            aiCheckPointQueue.Dequeue();
            // Add input speed of rival
            rivalPresentInputSpeed += maxInputSpeed * aiSpeedMultiplier;
        }
    }

    private void HandleRivalBehaviour()
    {
        if (!rivalBalloon) return;

        // Release the balloon
        if (rivalBalloon.State == Balloon.BalloonStatus.Danger &&
            willRivalReleaseBalloon)
        {
            ReleaseBalloon(false);
            willRivalReleaseBalloon = UnityEngine.Random.value < rivalSuccessProp;
            return;
        }

        // Balloon boom
        if (rivalBalloon.Inflate(rivalPresentInputSpeed * Time.deltaTime, 1.0f))
        {
            rival.ResetInflateAnimationSpeed();
            willRivalReleaseBalloon = UnityEngine.Random.value < rivalSuccessProp;
            Invoke("SpawnRivalBalloon", failFreezeTime);
            IsRivalFreezing = true;
            rivalBalloon = null;
            rival.StartDizzy();
        }
        else
        {
            rival.SetInflateAnimationSpeed(rivalPresentInputSpeed * rivalInflateAniSpeedMultiplier);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Is used joyCon for input?</returns>
    private bool HandlePlayerJoyConInput()
    {
        if (!PlayerBalloon) return false;
        if (!isUseLeftJoycon && !JoyConInputManager.IsUsingRightJoyCon()) return false;
        else if (isUseLeftJoycon && !JoyConInputManager.IsUsingLeftJoyCon()) return false;

        /// Handle release balloon
        var gyroInfo = JoyConInputManager.GetJoyConGyro(isUseLeftJoycon);
        if (gyroInfo.y < joyConReleaseSensitivity ||
            JoyConInputManager.GetJoyConButtonDown(Joycon.Button.SHOULDER_2))
        {
            if (PlayerBalloon.State != Balloon.BalloonStatus.Normal)
            {
                JoyConInputManager.TriggerRumble(false, 180.0f, 360.0f, 0.3f, 200);
                ReleaseBalloon(true);
            }
            return true;
        }

        /// Handle inflate balloon and animation
        if (Mathf.Abs(gyroInfo.y) > Mathf.Abs(joyConReleaseSensitivity)) return false;
        Vector3 joyAccel = JoyConInputManager.GetJoyConAccel(isUseLeftJoycon);
        float accelDelta = joyAccel.sqrMagnitude - joyConInputSensitivity;
        if (accelDelta < 0.0f)
        {
            player.ResetInflateAnimationSpeed();
            return false;
        }

        float newAir = accelDelta * joyConInflateSpeed * Time.deltaTime;
        if (newAir == 0.0f) return false;
        playerJoyConInputSpeed = newAir * joyConInflateAniSpeedMultiplier;
        player.SetInflateAnimationSpeed(playerJoyConInputSpeed);
        inflatedAirVolume += newAir;
        if (PlayerBalloon.Inflate(newAir, newAir * joyConInputPitchMultiplier))
        {
            // Process after balloon boomed
            JoyConInputManager.TriggerRumble(false, 360.0f, 180.0f, 0.7f, 400);
            PlayerBalloon = null;
            Invoke("SpawnPlayerBalloon", failFreezeTime);
            IsPlayerFreezing = true;
            player.StartDizzy();
            player.ResetInflateAnimationSpeed();
        }
        return true;
    }

    private bool HandlePlayerKeyboardInput()
    {
        // About APM calc
        keyboardInputLength += Time.deltaTime;
        if (isNoInput) keyboardNoInputLength += Time.deltaTime;
        keyboardInputPerSec = (float)keyboardInputCount / keyboardInputLength;
        if (keyboardNoInputLength > keyboardRestInputSpeedInterval)
        {
            keyboardNoInputLength = 0.0f;
            keyboardInputCount = 0;
            keyboardInputLength = 0.0f;
        }
        isNoInput = true;
        playerKeyboardInputSpeed = keyboardInputPerSec * keyboardInflateAniSpeedMultiplier;
        player.SetInflateAnimationSpeed(playerKeyboardInputSpeed);

        if (!PlayerBalloon) return false;

        // When inflating balloon
        if (Input.GetButtonDown(inflateButton))
        {
            keyboardNoInputLength = 0.0f;

            // Calc ani speed and set it
            isNoInput = false;
            keyboardInputCount++;

            if (PlayerBalloon.Inflate(keyboardInflateSpeed,
                keyboardInputPerSec * keyboardInputPitchMultiplier))
            {
                PlayerBalloon = null;
                player.StartDizzy();
                player.ResetInflateAnimationSpeed();
                Invoke("SpawnPlayerBalloon", failFreezeTime);
                IsPlayerFreezing = true;
            }
        }

        if (Input.GetButtonDown(releaseButton))
        {
            ReleaseBalloon(true);
        }

        return true;
    }

    private void SpawnPlayerBalloon()
    {
        SpawnBalloon(true);
        IsPlayerFreezing = false;
    }

    private void SpawnRivalBalloon()
    {
        SpawnBalloon(false);
        IsRivalFreezing = false;
    }

    /// <summary>
    /// The method that you can specify spawn whose balloon.
    /// Instead of this, use SpawnPlayerBallon and SpawnRivalBalloon
    /// </summary>
    /// <param name="isPlayerBalloon"></param>
    private void SpawnBalloon(bool isPlayerBalloon)
    {
        Transform targetTrans = null;
        if (isPlayerBalloon) targetTrans = playerNozzleTrans;
        else targetTrans = rivalBalloonMarkTransform;

        GameObject newBalloon = Instantiate(balloonBase, targetTrans.position,
            targetTrans.rotation);
        var targetBalloonComp = newBalloon.GetComponent<Balloon>();
        // Cacl balloon type
        int tempIndex;
        if (isPlayerBalloon) tempIndex = playerBalloonIndex;
        else tempIndex = rivalBalloonIndex;

        if (balloonSpawnList.Count - 1 < tempIndex)
        {
            balloonSpawnList.Add(GetRandomEnum<Balloon.BalloonType>());
        }

        if (isPlayerBalloon)
        {
            PlayerBalloon = targetBalloonComp;
            targetBalloonComp.InitializeBalloon(balloonSpawnList[tempIndex],
                                                Balloon.Player.GamePlayer);
        }
        else
        {
            rivalBalloon = targetBalloonComp;
            targetBalloonComp.InitializeBalloon(balloonSpawnList[tempIndex],
                                                Balloon.Player.AI);
        }
    }

    /// <summary>
    /// Handle process about release balloon
    /// </summary>
    /// <param name="isPlayerBalloon"></param>
    /// <returns>Released balloon successfully?</returns>
    private bool ReleaseBalloon(bool isPlayerBalloon)
    {
        if (isPlayerBalloon)
        {
            if (PlayerBalloon.State == Balloon.BalloonStatus.Normal) return false;

            AudioManager.Instance.Play("EmitBalloon");
            playerBalloonIndex++;
            PlayerBalloon.MoveAway();
            PlayerReleasedBalloons.Add(PlayerBalloon);
            PlayerBalloon = null;
        }
        else
        {
            if (rivalBalloon.State == Balloon.BalloonStatus.Normal) return false;

            AudioManager.Instance.Play("EmitBalloon");
            rivalBalloonIndex++;
            rivalBalloon.MoveAway();
            RivalReleasedBalloons.Add(rivalBalloon);
            rivalBalloon = null;
        }

        // Spawn balloon
        if (isPlayerBalloon) Invoke("SpawnPlayerBalloon", balloonSpawnInterval);
        else Invoke("SpawnRivalBalloon", balloonSpawnInterval);

        return true;
    }

    public void playWinLoseAnim()
    {
        player.SetFinishPose();
        rival.SetFinishPose();
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        var style = GUI.skin.GetStyle("label");
        style.fontSize = 24;

        GUILayout.BeginArea(new Rect(450, 0, 480, 900));
        GUILayout.Label("inflatedAirVolume = " + inflatedAirVolume);
        GUILayout.Label("Joy-con accel Z   = " + JoyConInputManager.GetJoyConAccel(false).z);
        GUILayout.EndArea();
    }
#endif
}