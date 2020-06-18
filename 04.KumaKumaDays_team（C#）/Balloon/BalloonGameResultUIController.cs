using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Update score info for balloon game result
/// </summary>
/// =======================================================
/// Author : 2019/01/10 Sa
/// History Log :
///		2019/01/10(Sa) Initial
///		2019/01/11(Sa) Change code structure
///		2019/01/19(Sa) Rewrite. This script only control result score UI now
///		2019/01/20(Sa) Add detials about balloon move to death
public class BalloonGameResultUIController : MonoBehaviour
{
    public static BalloonGameResultUIController Instance = null;
    public bool IsScoreCalcFinished { get; private set; }
    public UnityEvent OnFinishCalcResult = new UnityEvent();

    [Header("ObjAssign")]
    [SerializeField] TMP_Text playerScoreText = null;
    [SerializeField] TMP_Text rivalScoreText = null;
    [SerializeField] TMP_Text playerWinLoseText = null;
    [SerializeField] TMP_Text rivalWinLoseText = null;

    [Header("SpawnScoreText")]
    [SerializeField] GameObject scoreText = null;
    [SerializeField] Transform playerBalloonGatherTrans = null;
    [SerializeField] Transform rivalBalloonGatherTrans = null;
    [SerializeField] RectTransform playerRootUI = null;
    [SerializeField] RectTransform rivalRootUI = null;
    [SerializeField] Vector2 positionOffset = Vector2.zero;

    [Header("ScoreTextAni")]
    [SerializeField] float journeyTime = 3.0f;
    [SerializeField] float moveTimeIntervalPreType = 2.0f;
    [SerializeField] float moveTimeIntervalPreBalloon = 1.6f;

    private int playerScore = 0;
    private int rivalScore = 0;

    private void Awake()
    {
        if (Instance != null) Destroy(Instance.gameObject);
        Instance = this;

        IsScoreCalcFinished = false;
        playerScore = 0;
        rivalScore = 0;
    }

    #region PUBLIC
    public void SpawnScoreText(Balloon.Player balloonOwner,
                               Balloon balloon)
    {
        RectTransform targetTrans = null;
        switch (balloonOwner)
        {
            case Balloon.Player.GamePlayer:
                targetTrans = playerRootUI;
                break;
            case Balloon.Player.AI:
                targetTrans = rivalRootUI;
                break;
        }
        var textObj = Instantiate(scoreText, targetTrans);
        textObj.GetComponent<RectTransform>().localPosition = positionOffset;
        textObj.GetComponent<BalloonScoreText>().InitializeScoreText(balloonOwner, balloon);
    }

    public void UpdateTotalScore(Balloon.Player balloonOwner, int addScore)
    {
        switch (balloonOwner)
        {
            case Balloon.Player.GamePlayer:
                playerScore += addScore;
                playerScoreText.text = playerScore.ToString();
                break;
            case Balloon.Player.AI:
                rivalScore += addScore;
                rivalScoreText.text = rivalScore.ToString();
                break;
        }
    }
    #endregion

    #region NewAdded
    /// <summary>
    /// The entry of move balloon to calc score.
    /// </summary>
    public void StartProcess()
    {
        StartCoroutine(TriggerBalloonByType(
                       BalloonGameManager.Instance.SortedPlayerReleasedBalloons));
        StartCoroutine(TriggerBalloonByType(
                       BalloonGameManager.Instance.SortedRivalReleasedBalloons));
        StartCoroutine(AppearFinishResult());
    }

    private IEnumerator TriggerBalloonByType(List<List<Balloon>> sortedBalloons)
    {
        foreach (var i in sortedBalloons)
        {
            yield return TriggerBalloonByCount(i);
            yield return new WaitForSeconds(moveTimeIntervalPreType);
        }
    }

    private IEnumerator TriggerBalloonByCount(List<Balloon> balloons)
    {
        foreach (var balloon in balloons)
        {
            balloon.GetComponent<Rigidbody>().detectCollisions = false;
            switch (balloon.Owner)
            {
                case Balloon.Player.GamePlayer:
                    balloon.Devote(playerBalloonGatherTrans.position, this.journeyTime);
                    break;
                case Balloon.Player.AI:
                    balloon.Devote(rivalBalloonGatherTrans.position, this.journeyTime);
                    break;
            }
            yield return new WaitForSeconds(moveTimeIntervalPreBalloon);
        }
    }

    /// <summary>
    /// Appear final game result UI.
    /// If show UI, stop invoke of increment index.
    /// </summary>
    private IEnumerator AppearFinishResult()
    {
        while (playerScore != BalloonGameManager.PlayerScore ||
            rivalScore != BalloonGameManager.RivalScore)
        {
            yield return null;
        }

        // After updated every balloon type count ui
        IsScoreCalcFinished = true;
        OnFinishCalcResult?.Invoke();
        switch (BalloonGameManager.Instance.PresentGameResult)
        {
            case BalloonGameManager.GameResult.PlayerWin:
                playerWinLoseText.text = "Player\nWin";
                rivalWinLoseText.text = "Deer\nLose";
                break;
            case BalloonGameManager.GameResult.Match:
                playerWinLoseText.text = "Match";
                rivalWinLoseText.text = "Match";
                break;
            case BalloonGameManager.GameResult.RivalWin:
                playerWinLoseText.text = "Player\nLose";
                rivalWinLoseText.text = "Deer\nWin";
                break;
            default:
                Debug.Assert(false, "Wrong SwitchCase: ResultUIController.cs");
                break;
        }
    }
    #endregion

    private void OnDestroy()
    {
        Instance = null;
    }
}